using System;
using System.Collections.Generic;
using System.IO;
using static CodeCounter.Program;

namespace CodeCounter
{
    public class FileInfo
    {
        public string Path = "";
        public List<Solution> Solutions = new List<Solution>();
        public int LinesOfCode = 0;
        public override string ToString()
        {
            return Path;
        }
    }
}
