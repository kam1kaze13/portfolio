using Compress.Package;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Compress.Win
{
    public class PackageCommands
    {
        public static void ExecuteAddCommand(string packagePath, List<string> paths, Action<FileProcessingEventArgs> updateProgressBar, string packageTo = "", int bitsCount = 20)
        {
            var package = new PackageFactory(bitsCount).CreatePackage();

            package.FileProcessing += (_, args) =>
            {
                updateProgressBar(args);
            };

            package.Open(packagePath);

            package.Add(paths, packageTo);
        }

        public static void ExecuteExtractCommand(string packagePath, string pathTo ,List<string> paths, Action<FileProcessingEventArgs> updateProgressBar,bool askToOverwrite = true)
        {
            var package = new PackageFactory().CreatePackage();

            package.FileProcessing += (_, args) =>
            {
                updateProgressBar(args);
            };

            package.Open(packagePath);

            if (askToOverwrite)
            {
                package.AskToOverwrite += (_, args) =>
                {
                    var fileSystem = new FileSystem();
                    if (fileSystem.FileExists(args.Path))
                    {
                        DialogResult result = MessageBox.Show($"File {Path.GetFileName(args.Path)} already exists! Do you want overwrite it?",
                                "Confirm action", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                        if (result == DialogResult.Yes)
                        {
                            args.Overwrite = true;
                        }
                        else
                        {
                            args.Overwrite = false;
                        }
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

            package.Extract(paths, pathTo);
        }

        public static void ExecuteDeleteCommand(string packagePath, List<string> paths, Action<FileProcessingEventArgs> updateProgressBar)
        {
            var package = new PackageFactory().CreatePackage();      

            package.FileProcessing += (_, args) =>
            {
                updateProgressBar(args);
            };

            package.Open(packagePath);

            package.Delete(paths);
        }
    }
}
