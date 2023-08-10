using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compress.Package
{
    public class PackageFileSystem : IFileSystem
    { 
        public PackageFileSystem(PackageFile package)
        {
            this.package = package;
        }
        public List<string> GetRootPaths()
        {
            var list = new List<string>();
            foreach (var item in this.package.Items)
            {
                list.Add("\\" + item.Path);
            }
            return list;
        }

        public Dictionary<string, object> GetProperties(string path)
        {
            return new Dictionary<string, object>();
        }

        public void CreateDirectory(string path)
        {
            this.package.Add(new List<string> { path });
        }

        public void Delete(string path)
        {
            this.package.Delete(new List<string> { path });
        }

        public bool DirectoryExists(string path)
        {
            var dir = this.package.Items.Where(item => item.Path == path && item is DirectoryHeader);
            if (dir.Count() != 0)
                return true;
            return false;
        }

        public bool FileExists(string path)
        {
            var file = this.package.Items.Where(item => item.Path == path && item is FileHeader);
            if (file.Count() != 0)
                return true;
            return false;
        }

        public IList<string> GetChildItems(string path)
        {
            var childs = this.package.Items.Where(i => Path.GetDirectoryName(i.Path) == path).Select(i=> i.Path);
            var list = new List<string>();
            foreach (var item in childs)
                list.Add(item);
            return list;
        }

        public long GetFileLength(string path)
        {
            long length = 0;
            var file = this.package.Items.Where(i => i.Path == path);
            if (file.Count() !=0)
            {
                var fileHeader = file.First() as FileHeader;
                length = fileHeader.PackedLength;
            }

            return length;
        }

        public DateTime GetLastWriteTimeUtc(string path)
        {
            throw new NotImplementedException();
        }

        public bool IsDirectory(string path)
        {
            var item = this.package.Items.Where(i => i.Path == path);
            if (item is DirectoryHeader)
                return true;

            return false;
        }

        public void Move(string from, string to)
        {
            throw new NotImplementedException();
        }

        public Stream Open(string path, FileMode mode)
        {
            Stream output = null;
            switch(mode)
            {
                case FileMode.Open :
                    {
                        output = this.package.OpenFile(path);
                        break;
                    }

                case FileMode.Truncate :
                case FileMode.Create :
                    {
                        string tempPath = Path.Combine(Directory.GetCurrentDirectory(), path) + "v2";
                        var temp = File.Create(tempPath);
                        var packageStream = new PackageStream(temp);
                        packageStream.onStreamClose += (_, args) =>
                        {
                            var list = new List<string>();
                            list.Add(path);
                            this.package.Add(list);
                            this.Delete(tempPath);
                        };
                        output = packageStream;
                        break;
                    }
                case FileMode.Append :
                    {
                        break;
                    }
            }
            return output;
        }

        public void CreateTextFile(string path, string content)
        {
            throw new NotImplementedException();
        }

        public string ReadTextFile(string path)
        {
            throw new NotImplementedException();
        }

        private readonly PackageFile package;
    }
}
