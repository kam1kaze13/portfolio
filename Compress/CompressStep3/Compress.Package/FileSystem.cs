using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compress.Package
{
    public class FileSystem : BaseFileSystem
    {
        public override void CreateDirectory(string path)
        {
            if (!DirectoryExists(path))
                Directory.CreateDirectory(path);            
        }

        public override void Delete(string path)
        {
            if (DirectoryExists(path))
                Directory.Delete(path,true);
            else if (FileExists(path))
                File.Delete(path);
        }

        public override bool DirectoryExists(string path)
        {
            if (Directory.Exists(path))
                return true;
            else return false;
        }

        public override bool FileExists(string path)
        {
            if (File.Exists(path))
                return true;
            else return false;
        }

        public override IList<string> GetChildItems(string path)
        {
            if (!DirectoryExists(path))
                throw new FileNotFoundException("Directory {path} doesn't exist");
            else 
            {
                return Directory.GetFileSystemEntries(path);
            }
        }

        public override long GetFileLength(string path)
        {
            if (FileExists(path))
                return new FileInfo(path).Length;
            else
                throw new FileNotFoundException("File {path} doesn't exist");
        }

        public override DateTime GetLastWriteTimeUtc(string path)
        {
            if (IsDirectory(path))
            {
                if (DirectoryExists(path))
                    return new DirectoryInfo(path).LastWriteTimeUtc;
                else
                    throw new FileNotFoundException("Directory {path} doesn't exist");
            }
            else
            {
                if (FileExists(path))
                    return new FileInfo(path).LastWriteTimeUtc;
                else
                    throw new FileNotFoundException("File {path} doesn't exist");
            }

        }

        public override bool IsDirectory(string path)
        {
            if (DirectoryExists(path) || FileExists(path))
            {
                FileAttributes attr = File.GetAttributes(path);

                if (attr.HasFlag(FileAttributes.Directory))
                    return true;
                else return false;
            }
            else
                throw new FileNotFoundException("{path} doesn't exist");           
        }

        public override void Move(string from, string to)
        {
            if (IsDirectory(from))
            {
                if (DirectoryExists(to))
                    return;
                Directory.Move(from, to);
            }
            else
            {
                if (FileExists(to))
                    return;
                File.Move(from, to);
            }
                
        }

        public override Stream Open(string path, FileMode mode)
        {
            if (!DirectoryExists(path))
            {
                CreateDirectory(Path.GetDirectoryName(path));                
            }

            return File.Open(path, mode);
        }
    }
}
