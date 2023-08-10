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

            if (args[0] == "-p")
            {
                if (System.IO.File.Exists(args[1]))
                {
                    if (System.IO.File.Exists(args[2]))
                    {
                        Console.WriteLine("Attention! File {0} already exist! Do you want overwrite it?", args[2]);
                        string answer = Console.ReadLine();
                        if (answer == "y")
                        {
                            PackFile(args[1], args[2]);
                            Console.WriteLine("File {0} successfully packed!", args[1]);
                            return;
                        }
                        else
                        {
                            Console.WriteLine("End of the program");
                            return;
                        }
                    }
                    else
                    {
                        PackFile(args[1], args[2]);
                        Console.WriteLine("File {0} successfully packed!", args[1]);
                        return;
                    }
                }
                else
                {
                    Console.WriteLine("Error! File {0} doesn't exist!", args[1]);
                    return;
                }
             }

            if (args[0] == "-u")
            {
                if (args.Length == 2)
                {
                    if (args[1].Contains(".z"))
                    {
                        UnpackFile(args[1], args[1].Substring(0,args[1].Length-2));
                        Console.WriteLine("File {0} successfully unpacked!", args[1]);
                        return;
                    }
                    else
                    {
                        UnpackFile(args[1], args[1].Insert(args[1].IndexOf('.')+1,"unpacked."));
                        Console.WriteLine("File {0} successfully unpacked!", args[1]);
                        return;
                    }
                }
                else if (System.IO.File.Exists(args[1]))
                {
                    if (System.IO.File.Exists(args[2]))
                    {
                        Console.WriteLine("Attention! File {0} already exist! Do you want overwrite it?", args[2]);
                        string answer = Console.ReadLine();
                        if (answer == "y")
                        {
                            UnpackFile(args[1], args[2]);
                            Console.WriteLine("File {0} successfully unpacked!", args[1]);
                            return;
                        }
                        else
                        {
                            Console.WriteLine("End of the program");
                            return;
                        }
                    }
                    else
                    {
                        UnpackFile(args[1], args[2]);
                        Console.WriteLine("File {0} successfully unpacked!", args[1]);
                        return;
                    }
                }
                else
                {
                    Console.WriteLine("Error! File {0} doesn't exist!", args[1]);
                    return;
                }
            }

            if (System.IO.File.Exists(args[0]))
            {
                if (args.Length == 1)
                {
                    string outFile = args[0] + ".z";
                    PackFile(args[0], outFile);
                    Console.WriteLine("File {0} successfully packed!", args[0]);
                    return;
                }
                else if (System.IO.File.Exists(args[1]))
                {
                    Console.WriteLine("Attention! File {0} already exist! Do you want overwrite it?", args[1]);
                    string answer = Console.ReadLine();
                    if (answer == "y")
                    {
                        PackFile(args[0], args[1]);
                        Console.WriteLine("File {0} successfully packed!", args[0]);
                        return;
                    }
                    else
                    {
                        Console.WriteLine("End of the program");
                        return;
                    }
                }
                else
                {
                    PackFile(args[0], args[1]);
                    Console.WriteLine("File {0} successfully packed!", args[0]);
                    return;
                }
            }
            else
            {
                Console.WriteLine("Error! File {0} doesn't exist!", args[0]);
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
            // 1) compress -p TestFile.txt TestFile.txt.z ++++++++++++++++++++++++++++++++++++++++++++
            //  utility should pack TestFile.txt in TestFile.txt.z
            //
            // 2) compress -u TestFile.txt.z TestFile.txt ++++++++++++++++++++++++++++++++++++++++++++
            //  utility should pack TestFile.txt in TestFile.txt.z
            //
            // 3) compress TestFile.txt TestFile.txt.z ++++++++++++++++++++++++++++++++++++++++++++
            //  -p command is omit, but it works like 1) because -a command is optional
            //
            // 4) compress TestFile.txt ++++++++++++++++++++++++++++++++++++++++++++
            //  TestFile.txt will be packed into TestFile.txt.z
            //  if output file is omit for packing
            //  then output file name is constructed from input by adding .z into the end
            //
            // 5) compress -u TestFile.txt.z ++++++++++++++++++++++++++++++++++++++++++++
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
            {
                CryptoStreamTransporter ls = new CryptoStreamTransporter();

                ICryptoPacker packer = new LzwPacker();

                System.IO.FileInfo file = new System.IO.FileInfo(Path.GetFullPath(inputFileName));
                long size = file.Length;

                ls.Pack(input, output, size, packer);
            }
        }

        private static void UnpackFile(string inputFileName, string outputFileName)
        {
            using (var input = new FileStream(Path.GetFullPath(inputFileName), FileMode.Open))
            using (var output = new FileStream(Path.GetFullPath(outputFileName), FileMode.Create))
            {
                CryptoStreamTransporter ls = new CryptoStreamTransporter();

                ICryptoUnpacker unpacker = new LzwUnpacker();

                System.IO.FileInfo file = new System.IO.FileInfo(Path.GetFullPath(inputFileName));
                long size = file.Length;

                ls.Unpack(input, output, size, unpacker);
            }
        }

        private static void PrintHelp()
        {
            // todo: implement
            Console.WriteLine("Program usage two arguments: -p and -u");
        }
    }
}
