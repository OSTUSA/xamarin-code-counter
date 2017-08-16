using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace CodeCounter
{
    public class Solution
    {
        public string Name = string.Empty;
        public List<Project> ProjectFiles = new List<Project>();
        public override string ToString()
        {
            return Name;
        }

        public int UniqueLinesOfCode
        {
            get
            {
                return (from project in ProjectFiles select project.UniqueLinesOfCode).Sum();
			}
        }

        public int SharedLinesOfCode
        {
            get
            {
                return (from project in ProjectFiles select project.SharedLinesOfCode).Sum();
            }
        }

        public int TotalLinesOfCode
        {
            get
            {
                return (from project in ProjectFiles select project.TotalLinesOfCode).Sum();
            }
        }

        public string GetLinesOfCodeByProject()
        {
			StringBuilder result = new StringBuilder();


            result.Append("Lines\t%Total\tShared\tName");
            result.Append(Environment.NewLine);

            int totalLines = TotalLinesOfCode;

            foreach (var project in ProjectFiles.OrderByDescending(p => p.TotalLinesOfCode))
            {
                result.Append(project.TotalLinesOfCode + "\t");
                result.Append(Math.Round(((project.TotalLinesOfCode / (double)totalLines) * 100), 3).ToString() + "\t");
                result.Append(!project.IsUniqueCode + "\t");
				result.Append(project.Name + "\t");
				result.Append(Environment.NewLine);
            }

            return result.ToString();
        }

       
        public Solution(string filePath)
        {
            int projCount = 0;
            string directory = Path.GetDirectoryName(filePath);
            foreach (string line in File.ReadLines(filePath))
            {
                if (line.StartsWith("Project", StringComparison.OrdinalIgnoreCase))
				{
                    string projectPath = line.Split(',')[1].Replace('"', ' ').Replace('\\', '/').Trim();
                    if (projectPath.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase))
                    {
						ProjectFiles.Add(new Project(Path.Combine(directory, projectPath)));
                        projCount++;
					}
				}
			}
        }
    }
}
