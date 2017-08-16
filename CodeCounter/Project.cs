﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace CodeCounter
{
    public enum ProjectType
    {
		Shared = 0,
		Android = 1,
        iOS = 2,
    }

    public class Project
    {
        private List<string> _filesToExclude = new List<string>() { "AssemblyInfo.cs", "Resource.designer.cs" };
        public List<FileInfo> CodeFiles = new List<FileInfo>();
        public string FilePath;

        public ProjectType ProjectType { get; private set; }

        public string Name { get; private set; }

        Dictionary<string, FileInfo> _files = new Dictionary<string, FileInfo>();

        void AddRef(string path)
        {
            if (_files.ContainsKey(path))
            {
                CodeFiles.Add(_files[path]);
            }
            else
            {
                var info = new FileInfo { Path = path.Replace("\\", "/"), };
                _files[path] = info;
                CodeFiles.Add(info);
            }
        }

        public Project(string filePath)
        {
            FilePath = filePath;
            var dir = Path.GetDirectoryName(filePath);
            Name = Path.GetFileNameWithoutExtension(filePath);
             
            var doc = XDocument.Load(filePath);
            var query = from x in doc.Descendants()
                    let e = x as XElement
                    where e != null
                    where e.Name.LocalName == "Compile" || e.Name.LocalName =="EmbeddedResource" || e.Name.LocalName == "ObjcBindingCoreSource" || e.Name.LocalName == "TransformFile"
                    where e.Attributes().Any(a => a.Name.LocalName == "Include")
                    select e.Attribute("Include").Value;

            foreach (var inc in query)
            {
                // Only count CS and Xaml files. Also count xml files for Android bindings
                if (inc.EndsWith(".cs", StringComparison.OrdinalIgnoreCase) || inc.EndsWith(".xaml", StringComparison.OrdinalIgnoreCase) || inc.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
                    AddRef(Path.GetFullPath(Path.Combine(dir, inc))); 
            }

			query = from x in doc.Descendants()
						let e = x as XElement
						where e != null
						where e.Name.LocalName == "Reference" 
						where e.Attributes().Any(a => a.Name.LocalName == "Include")
						select e.Attribute("Include").Value;

            // Figure out the project type based on references.
            foreach (var item in query)
            {
                if(item.Equals("Mono.Android", StringComparison.OrdinalIgnoreCase))
                {
                    // It's An Android project
                    ProjectType = ProjectType.Android;
                    break;
                }
                else if (item.StartsWith("Xamarin.Forms.Platform.iOS", StringComparison.OrdinalIgnoreCase) || item.StartsWith("Xamarin.iOS", StringComparison.OrdinalIgnoreCase))
                {
                    ProjectType = ProjectType.iOS;
                    break;
				}
			}

            // Get the lines of code
            foreach (var file in _files.Values)
            {
                try
                {
                    string fileName = Path.GetFileName(file.Path);
                    if (!_filesToExclude.Contains(fileName))
                    {
                        Debug.WriteLine(fileName);
						file.LinesOfCode = File.ReadAllLines(file.Path).Length;
					}
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
        }

        public int TotalLinesOfCode
        {
            get
            {
                var total = (from f in CodeFiles
                             select f.LinesOfCode).Sum();

                return total;
            }
        }

        public bool IsUniqueCode
        {
            get
            {
                //string fileName = Path.GetFileNameWithoutExtension(FilePath);
                //return fileName.EndsWith("ios", StringComparison.OrdinalIgnoreCase) || fileName.EndsWith("android", StringComparison.OrdinalIgnoreCase) || fileName.EndsWith("droid", StringComparison.OrdinalIgnoreCase);
                return ProjectType != ProjectType.Shared;
            }
        }

        public int UniqueLinesOfCode
        {
            get
            {
                string fileName = Path.GetFileNameWithoutExtension(FilePath);
                if (IsUniqueCode)
                {
                    return TotalLinesOfCode;
                }

				return 0;
            }
        }

        public int SharedLinesOfCode
        {
            get
            {
                if (!IsUniqueCode)
                {
                    return TotalLinesOfCode;
                }

                return 0;
            }
        }
    }
}
