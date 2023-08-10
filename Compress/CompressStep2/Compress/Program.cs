using Compress.Core;
using System;
using System.IO;

namespace Compress
{
    class Program
    {
        static void Main(string[] args)
        {
            // if there is no arguments or -h command is passed then show help message and quit
            if (args.Length == 0 || args[0] == "-h")
            {
                PrintHelp();
                return;
            }

            // todo:
            // utility should support -u command and -p command (-p command can be omitted)
            // first one for unpacking file, second one for packing
            // another two possible arguments are names of files,
            // first one is input file (for make an action with)
            // second one is output file
            // if input file doesn't exist then utility should show error message about it and quit
            // if output file exists then utility should ask user about overwriting file,
            // if user types 'y' then it should continue and overwrite this file
            // otherwise quit
            //
            // examples:
            // 1) compress -p TestFile.txt TestFile.txt.z
            //  utility should pack TestFile.txt in TestFile.txt.z
            //
            // 2) compress -u TestFile.txt.z TestFile.txt
            //  utility should pack TestFile.txt in TestFile.txt.z
            //
            // 3) compress TestFile.txt TestFile.txt.z
            //  -p command is omit, but it works like 1) because -a command is optional
            //
            // 4) compress TestFile.txt
            //  TestFile.txt will be packed into TestFile.txt.z
            //  if output file is omit for packing
            //  then output file name is constructed from input by adding .z into the end
            //
            // 5) compress -u TestFile.txt.z
            //  TestFile.txt.z will be unpacked into TestFile.txt
            //  if output file is omit for unpacking and input file name has extension .z 
            //  then output file name is constructed from input by removing .z from the end
            //
            // 6) compress -u TestFile.txt
            //  TestFile.txt will be unpacked into TestFile.unpacked.txt
            //  if output file is omit for unpacking and input file name has different extension than .z 
            //  then output file name is constructed from input by adding .unpack before extension

        }

        private static void PackFile(string inputFileName, string outputFileName)
        {
            using (var input = new FileStream(Path.GetFullPath(inputFileName), FileMode.Open))
            using (var output = new FileStream(Path.GetFullPath(outputFileName), FileMode.Create))
            using (var ls = new LzwStreamWriter(output))
            {
                CopyStream(input, ls);
            }
        }

        private static void UnpackFile(string inputFileName, string outputFileName)
        {
            using (var input = new FileStream(Path.GetFullPath(inputFileName), FileMode.Open))
            using (var ls = new LzwStreamReader(input))
            using (var output = new FileStream(Path.GetFullPath(outputFileName), FileMode.Create))
            {
                CopyStream(ls, output);
            }
        }

        private static void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[32768];
            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, read);
            }
        }

        private static void PrintHelp()
        {
            // todo: implement
            Console.WriteLine("Program usage two arguments: -p and -u");
        }
    }
}
