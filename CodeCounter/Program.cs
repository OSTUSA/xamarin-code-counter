
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;

namespace CodeCounter
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
				var solution = new Solution("/Users/stackunderflows/Projects/MyProject.sln");
				new Program().Run(solution);
            }
        }

        void Run(Solution solution)
        {
            Console.WriteLine("Total\tUnique\tShared\tUnique%\tShared%");
            Console.WriteLine("{0}\t{1}\t{2}\t{3:p}\t{4:p}",
                //solution.Name,
                solution.TotalLinesOfCode,
                solution.UniqueLinesOfCode,
                solution.SharedLinesOfCode,
                solution.UniqueLinesOfCode / (double)solution.TotalLinesOfCode,
                solution.SharedLinesOfCode / (double)solution.TotalLinesOfCode);

            Console.WriteLine(' ');
            Console.WriteLine(' ');

            Console.WriteLine(solution.GetLinesOfCodeByProject());
            Console.ReadLine();
        }
    }
}
