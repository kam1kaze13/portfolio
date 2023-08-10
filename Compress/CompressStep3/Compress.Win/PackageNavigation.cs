using Compress.Package;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Compress.Win
{
    public class PackageNavigation
    {
        public PackageNavigation()
        {
            this.fileSystemStack = new Stack<IFileSystem>();
        }
        public void OpenPackage(DataGridView dataGridView, ToolStripTextBox tbAddress)
        {
            if (fileSystemStack.Count == 0)
            {
                var packageFactory = new PackageFactory();

                var package = packageFactory.CreatePackage();

                var packagePath = tbAddress.Text.Substring(0, tbAddress.Text.IndexOf(".pkg") + 4);

                package.Open(packagePath);

                var packageFS = new PackageFileSystem(package);

                fileSystemStack.Push(packageFS);

                PutToDataGridView(package.Items.Where(i => i.Path == i.Directory.Path).ToList(), dataGridView);
            }
            else
            {
                var currentFS = fileSystemStack.Peek();

                var str = tbAddress.Text.Substring(tbAddress.Text.IndexOf(".pkg") + 5);
                str = PathHelper.RemoveEndSeparator(str);

                var stream = currentFS.Open(str, FileMode.Open);

                var packageFactory = new PackageFactory();
                packageFactory.SetPackageFileSystem();

                var package = packageFactory.CreatePackage();

                package.Open(stream);

                var packageFS = new PackageFileSystem(package);

                fileSystemStack.Push(packageFS);

                PutToDataGridView(package.Items.Where(i => i.Path == i.Directory.Path).ToList(), dataGridView);

                stream.Close();
            }          
        }

        public void SearchFiles(DataGridView dataGridView, ToolStripTextBox tbAddress)
        {
            dataGridView.Rows.Clear();

            Image imgFolder = Properties.Resources.icon_folder;

            if (tbAddress.Text == "")
            {
                DriveInfo[] allDrives = DriveInfo.GetDrives();

                foreach (DriveInfo d in allDrives)
                {
                    dataGridView.Rows.Add(imgFolder, d.Name, "Driver");
                }
            }
            else
            {
                DirectoryInfo dir = new DirectoryInfo(tbAddress.Text);
                DirectoryInfo[] dirs = dir.GetDirectories();

                foreach (DirectoryInfo dirInfo in dirs)
                {
                    dataGridView.Rows.Add(imgFolder, dirInfo, "Folder", File.GetLastWriteTime(dirInfo.FullName));
                }

                // Set a default icon for the file.
                Icon iconForFile = SystemIcons.WinLogo;
                FileInfo[] files = dir.GetFiles();

                foreach (FileInfo fileInfo in files)
                {
                    iconForFile = System.Drawing.Icon.ExtractAssociatedIcon(fileInfo.FullName);

                    dataGridView.Rows.Add(iconForFile, fileInfo, "File " + '"' + fileInfo.Extension.Substring(1, fileInfo.Extension.Length - 1).ToUpper() + '"',
                               File.GetLastWriteTime(fileInfo.FullName), fileInfo.Length);
                }
            }
        }

        public void Up(DataGridView dataGridView, ToolStripTextBox tbAddress)
        {
            var path = tbAddress.Text;

            if (Path.GetExtension(path) == ".pkg")
                fileSystemStack.Pop();

            if (fileSystemStack.Count == 1)
                fileSystemStack.Pop();

            if (path != "")
            {
                while (path[path.Length - 1] != '\\')
                {
                    path = path.Remove(path.Length - 1, 1);
                    if (path.Length == 0)
                        break;
                }
                if (path.Length == 3)
                    path = "";
            }
            tbAddress.Text = PathHelper.RemoveEndSeparator(path);

            if (tbAddress.Text.Contains(".pkg"))
                this.OpenPackage(dataGridView, tbAddress);
            else
                this.SearchFiles(dataGridView, tbAddress);
        }

        private static void PutToDataGridView(IList<ItemHeader> items, DataGridView dataGridView)
        {
            dataGridView.Rows.Clear();

            Icon iconForFile = SystemIcons.WinLogo;
            Image folder = Properties.Resources.icon_folder;
            foreach (var item in items)
            {
                if (item is FileHeader fileItem)
                {
                    var ext = Path.GetExtension(fileItem.Path);
                    iconForFile = IconHelper.GetIcon(fileItem.Path);
                    dataGridView.Rows.Add(iconForFile, Path.GetFileName(fileItem.Path), "File " + '"' + ext.Substring(1, ext.Length - 1).ToUpper() + '"',
                            fileItem.Modified, fileItem.UnpackedLength, fileItem.PackedLength);
                }
                if (item is DirectoryHeader dirItem)
                {
                    dataGridView.Rows.Add(folder, Path.GetFileName(dirItem.Path), "Folder ", dirItem.Modified);
                }
            }
        }

        private Stack<IFileSystem> fileSystemStack;
    }
}
