using Compress.Core;
using Compress.Package;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Compress.Cmd
{
    class Program
    {
        // common format of utility call:
        // Compress.Cmd {command} {package name} {option} {path}
        // {command} {package name} are required and are always first 2 arguments
        // there could be multiple options and multiple paths
        // as a {path} is meant any argument after first two {command} and {package name} arguments which starts not from "-"
        // {option} is started from "-"
        // options can be included to list of args in any order after {command} and {package name} arguments
        //
        // list of commands:
        //
        // "a": add all (external) {path} to {package name}
        // {package name} could exist, in this case files are added to (or replaced in) existing package
        // otherwise {package name} is created
        // if {package name} has no extension .pkg extension is meant
        // {path} should be valid external path (in file system) to file or directory
        // if it is relative then it should be relative to current path
        // possibly option are
        // "k{max key lenght}" means maximal length in bits of key for LZW algo, length can not be lesser than 9
        // "i{internal directory name} means internal path to directory inside package to which {path} are added
        // 
        // examples:
        // 1) Compress.Cmd a package file1.txt dir1
        //  file file1.txt and directory dir1 are added to package.pkg package
        //  (take a look that .pkg is omitted)
        //
        // 2) Compress.Cmd a package.pkg file1.txt -t18 file2.txt
        //  files file1.txt and file2.txt are added to package.pkg package with max key length 18 bits
        //
        // 3) Compress.Cmd a package.pkg -idir file1.txt 
        //  file1.txt is added to package.pkg into internal directory dir/
        //
        //
        // "x": extract all (internal) {path} from {package name}
        // {package name} must exist
        // if {package name} has no extension .pkg extension is meant
        // {path} should be valid internal path inside package to file or directory
        // possibly option are
        // "e{external directory name} means external path to directory to which paths are extracted
        //   if it doesn't exist it should be created
        // "f" means that all answers to overwrite questions are "yes", therefore user should not be asked about it
        // 
        // examples:
        // 1) Compress.Cmd x package file1.txt dir1
        //  file file1.txt and directory dir1 are extracted from package.pkg package
        //  (take a look that .pkg is omitted)
        //  if some file already exists then user is asked about overwriting this file
        //
        // 2) Compress.Cmd a package.pkg file1.txt -f file2.txt
        //  files file1.txt and file2.txt are extracted from package.pkg,
        //  if these files already exist then they are overwritten without questions
        //
        // 3) Compress.Cmd x package.pkg -ec:/dir file1.txt 
        //  file1.txt is extracted from package.pkg into external directory c:/dir/
        //
        //
        // "d": delete all (internal) {path} from {package name}
        // {package name} must exist
        // if {package name} has no extension .pkg extension is meant
        // {path} should be valid internal path inside package to file or directory
        // 
        // examples:
        // 1) Compress.Cmd d package file1.txt dir1
        //  file file1.txt and directory dir1 are deleted from package.pkg package
        //  (take a look that .pkg is omitted)
        //
        //
        // "h": usage will be shown
        //
        static void Main(string[] args)
        {
            string utilityName = Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().FullName).Split(',')[0];

            // if there is no arguments or h command is passed then show help message and quit
            if (args.Length == 0 || args[0] == "h")
            {
                PrintHelp(utilityName);
                return;
            }

            if (args.Length < 2)
            {
                Console.WriteLine($"Format of utility usage is '{utilityName} (command) (package_name) [option] [path]'. Type '{utilityName} h' for help.");
                return;
            }

            string packagePath = args[1];
            if (string.IsNullOrEmpty(Path.GetExtension(packagePath)))
                packagePath += ".pkg";

            var options = GetOptions(args.Skip(2)).ToList();

            var paths = GetPaths(args.Skip(2)).ToList();

            packagePath = MakePathRoot(packagePath);

            if (args[0] == "a")
                ExecuteAddCommand(packagePath, options, paths);
            else if (args[0] == "x")
                ExecuteExtractCommand(packagePath, options, paths);
            else if (args[0] == "d")
                ExecuteDeleteCommand(packagePath, options, paths);
        }

        private static void ExecuteAddCommand(string packagePath, List<Option> options, List<string> paths)
        {
            var package = new PackageFactory().CreatePackage();

            package.FileProcessing += (_, args) =>
            {
                PrintProgress(args.FileName, args.Type, args.Total, args.TotalProcessed);
                
            };

            package.Open(packagePath);

            var rootedPaths = new List<string>();

            foreach (var path in paths)
            {
                rootedPaths.Add(MakePathRoot(path));
            }

            if (options.Any(opt => opt.Name == "k"))
            {
                var bits = Convert.ToInt32(options.Find(opt => opt.Name == "k").Value);
                package = new PackageFactory(bits).CreatePackage();

                package.FileProcessing += (_, args) =>
                {
                    PrintProgress(args.FileName, args.Type, args.Total, args.TotalProcessed);
                };

                package.Open(packagePath);
            }

            if (options.Any(opt => opt.Name == "i"))
            {
                var dir = options.Find(opt => opt.Name == "i").Value;
                package.Add(rootedPaths, dir);
            }
            else 
                package.Add(rootedPaths);
        }

        private static void ExecuteExtractCommand(string packagePath, List<Option> options, List<string> paths)
        {
            var fileSystem = new FileSystem();

            if (!fileSystem.FileExists(packagePath))
            {
                Console.WriteLine($"Package {packagePath} doesn't exists!");
                return;
            }

            var package = new PackageFactory().CreatePackage();

            package.FileProcessing += (_, args) =>
            {
                PrintProgress(args.FileName, args.Type, args.Total, args.TotalProcessed);
            };

            package.Open(packagePath);

            string extPath = Directory.GetCurrentDirectory();

            if (options.Any( opt => opt.Name == "e"))
            {
                var path = options.Find(opt => opt.Name == "e");
                extPath = MakePathRoot(path.Value);
            }

            if (!options.Any( opt => opt.Name == "f"))
            {
                package.AskToOverwrite += (_, args) =>
                {
                    if (fileSystem.FileExists(args.Path))
                    {
                        Console.WriteLine($"File {Path.GetFileName(args.Path)} already exists! Do you want overwrite it? (Yes - y, No - n)");

                        string answer = Console.ReadLine();
                        while (answer != "y" & answer != "n")
                        {
                            Console.WriteLine("Invalid expression! Please, write y or n");
                            answer = Console.ReadLine();                           
                        }
                        if (answer == "y")
                            args.Overwrite = true;
                        else if (answer == "n")
                            args.Overwrite = false;
                    }
                };
            }
            else
            {
                package.AskToOverwrite += (_, args) =>
                {
                    args.Overwrite = true;
                };
            }

            package.Extract(paths, extPath);
        }

        private static void ExecuteDeleteCommand(string packagePath, List<Option> options, List<string> paths)
        {

            var fileSystem = new FileSystem();

            if (!fileSystem.FileExists(packagePath))
            {
                Console.WriteLine($"Package {packagePath} doesn't exists!");
                return;
            }

            var package = new PackageFactory().CreatePackage();

            package.Open(packagePath);

            package.FileProcessing += (_, args) =>
            {
                PrintProgress(args.FileName, args.Type, args.Total, args.TotalProcessed);
            };

            package.Delete(paths);
        }


        private class Option
        {
            public string Name { get; set; }

            public string Value { get; set; }
        }

        private static IEnumerable<Option> GetOptions(IEnumerable<string> args)
        {
            foreach (var option in args)
            {
                if (option.StartsWith("-"))
                {
                    yield return new Option
                    {
                        Name = option.Substring(1, 1),
                        Value = option.Substring(2)
                    };
                }
            }
        }

        private static IEnumerable<string> GetPaths(IEnumerable<string> args)
        {
            foreach (var path in args)
            {
                if (!path.StartsWith("-"))
                {
                    yield return path;
                }
            }
        }

        private static string MakePathRoot(string path)
        {
            return Path.IsPathRooted(path) 
                    ? path 
                    : Path.Combine(Directory.GetCurrentDirectory(), path);
        }

        private static void PrintHelp(string utilityName)
        {
            Console.WriteLine("Common format of utility call:");
            Console.WriteLine($"{utilityName} (command) (package name) [option] [path]");
            Console.WriteLine("(command) (package name) are required and are always first 2 arguments");
            Console.WriteLine("there could be multiple options and multiple paths");
            Console.WriteLine("as a (path) is meant any argument after first two (command) and (package name) arguments which starts not from '-'");
            Console.WriteLine("(option) is started from '-'");
            Console.WriteLine("options can be included to list of args in any order after (command) and (package name) arguments");
            Console.WriteLine();
            Console.WriteLine("list of commands:");
            Console.WriteLine();
            Console.WriteLine("'a': add all (external) (path) to (package name)");
            Console.WriteLine("(package name) could exist, in this case files are added to (or replaced in) existing package");
            Console.WriteLine("otherwise (package name) is created");
            Console.WriteLine("if (package name) has no extension .pkg extension is meant");
            Console.WriteLine("(path) should be valid external path (in file system) to file or directory");
            Console.WriteLine("if it is relative then it should be relative to current path");
            Console.WriteLine("possibly option are");
            Console.WriteLine("'k(max key lenght)' means maximal length in bits of key for LZW algo, length can not be lesser than 9");
            Console.WriteLine("'i(internal directory name) means internal path to directory inside package to which (path) are added");
            Console.WriteLine("");
            Console.WriteLine("examples:");
            Console.WriteLine($"1) {utilityName} a package file1.txt dir1");
            Console.WriteLine(" file file1.txt and directory dir1 are added to package.pkg package");
            Console.WriteLine(" (take a look that .pkg is omitted)");
            Console.WriteLine();
            Console.WriteLine($"2) {utilityName} a package.pkg file1.txt -t18 file2.txt");
            Console.WriteLine(" files file1.txt and file2.txt are added to package.pkg package with max key length 18 bits");
            Console.WriteLine();
            Console.WriteLine($"3) {utilityName} a package.pkg -idir file1.txt ");
            Console.WriteLine(" file1.txt is added to package.pkg into internal directory dir/");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("'x': extract all (internal) (path) from (package name)");
            Console.WriteLine("(package name) must exist");
            Console.WriteLine("if (package name) has no extension .pkg extension is meant");
            Console.WriteLine("(path) should be valid internal path inside package to file or directory");
            Console.WriteLine("possibly option are");
            Console.WriteLine("'e(external directory name) means external path to directory to which paths are extracted");
            Console.WriteLine("  if it doesn't exist it should be created");
            Console.WriteLine("'f' means that all answers to overwrite questions are 'yes', therefore user should not be asked about it");
            Console.WriteLine("");
            Console.WriteLine("examples:");
            Console.WriteLine($"1) {utilityName} x package file1.txt dir1");
            Console.WriteLine(" file file1.txt and directory dir1 are extracted from package.pkg package");
            Console.WriteLine(" (take a look that .pkg is omitted)");
            Console.WriteLine(" if some file already exists then user is asked about overwriting this file");
            Console.WriteLine();
            Console.WriteLine($"2) {utilityName} a package.pkg file1.txt -f file2.txt");
            Console.WriteLine(" files file1.txt and file2.txt are extracted from package.pkg,");
            Console.WriteLine(" if these files already exist then they are overwritten without questions");
            Console.WriteLine();
            Console.WriteLine($"3) {utilityName} x package.pkg -ec:/dir file1.txt ");
            Console.WriteLine(" file1.txt is extracted from package.pkg into external directory c:/dir/");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("'d': delete all (internal) (path) from (package name)");
            Console.WriteLine("(package name) must exist");
            Console.WriteLine("if (package name) has no extension .pkg extension is meant");
            Console.WriteLine("(path) should be valid internal path inside package to file or directory");
            Console.WriteLine("");
            Console.WriteLine("examples:");
            Console.WriteLine($"1) {utilityName} d package file1.txt dir1");
            Console.WriteLine(" file file1.txt and directory dir1 are deleted from package.pkg package");
            Console.WriteLine(" (take a look that .pkg is omitted)");
            Console.WriteLine();
        }

        public static void PrintProgress(string fileName, FileProcessingType type, long total, long totalProcessed)
        {
            var x = Console.CursorLeft;
            var y = Console.CursorTop;            
            Console.WriteLine($"File name: {fileName}");
            Console.WriteLine($"Operation: {type}");
            Console.WriteLine($"File size: {total}");
            Console.WriteLine($"Done: {totalProcessed}");
            Console.SetCursorPosition(x, y);
        }
    }
}
