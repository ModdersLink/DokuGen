using System;

namespace DokuGen
{
    class Program
    {
        static void Main(string[] p_Args)
        {
            if (p_Args.Length < 2)
            {
                Console.WriteLine("DokuGen.exe <input directory> <output directory>");
                Console.WriteLine("Input directory should contain the .dll and .xml files that go with it.");
                Console.WriteLine("The output directory path is where all created wiki files will go.");
                Console.WriteLine("All directories are WITHOUT the ending trailing slash.");
                return;
            }

            var s_Generator = new Generator()
            {
                InputDirectory = p_Args[0],
                OutputDirectory = p_Args[1]
            };

            if (!s_Generator.Read())
            {
                Console.WriteLine("There was an error creating the documentation.");
            }
        }


    }
}
