using Compress.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compress.Package
{
    public class PackageFile
    {
        public PackageFile(IFileSystem fileSystem, IPackageWriter packageWriter, IPackageExtractor packageExtractor)
        {
            this.fileSystem = fileSystem;
            this.packageWriter = packageWriter;
            this.packageExtractor = packageExtractor;
        }

        // anchor package into external @filePath
        // if it exists then header should be read
        public void Open(string filePath)
        {           
            using (Stream input = this.fileSystem.Open(filePath, FileMode.OpenOrCreate))
            {
                this.Open(input);
            }
            this.packagePath = filePath;
        }

        public void Open(Stream input)
        {
            var serializator = new HeaderSerializator();
            var lengthBefore = input.Position;
            this.header = serializator.Load(input);
            var lengthAfter = input.Position;
            this.header.ReconstructInternalRelations(lengthAfter - lengthBefore);
        }

        //path is internal path in package
        public Stream OpenFile(string path)
        {
            Stream input = this.fileSystem.Open(this.packagePath, FileMode.Open);
            var item = this.header.GetItemForPath(path);

            if (item is FileHeader fileHeader)
            {
                input.Seek(fileHeader.StartOffset, SeekOrigin.Begin);
            }
            
            return input;
        }

        // add several external @paths into package to internal package @packagePath
        public void Add(List<string> paths, string packagePath = "")
        {
            List<PackageWriterTask> tasks = new List<PackageWriterTask>();

            FileHelper helper = new FileHelper(this.fileSystem);

            List<ExternalInternalPath> extIntPaths =
                helper.CheckAndGetPaths(Path.GetDirectoryName(this.packagePath), packagePath, paths);

            foreach (var extIntPath in extIntPaths)
            {
                var item = this.header.GetItemForPath(extIntPath.InternalPath);
                if (item != null)
                {
                    this.header.Items.Remove(item);
                }
            }

            foreach (var item in this.Items)
            {
                tasks.Add(new CopyTask { Item = item.Clone() });
            }

            foreach (var extIntPath in extIntPaths)
            {
                switch (extIntPath.Type)
                {
                    case ItemType.File:
                        {
                            tasks.Add(new PackTask { Path = extIntPath });
                            break;
                        }
                    case ItemType.Directory:
                        {
                            tasks.Add(new CopyTask { Item = new DirectoryHeader
                            {
                                Path = extIntPath.InternalPath,
                                Modified = this.fileSystem.GetLastWriteTimeUtc(extIntPath.ExternalPath)
                            }
                            });
                            break;
                        }
                }

            }

            this.ApplyChanges(tasks);
        }

        // apply tasks by creation of new package
        private void ApplyChanges(List<PackageWriterTask> tasks)
        {
            this.header = packageWriter.Write(this.packagePath + "v2", this.packagePath, tasks, 
                process =>
                    {
                        this.FileProcessing(this, process);
                    });
            this.fileSystem.Delete(this.packagePath);
            this.fileSystem.Move(this.packagePath + "v2", this.packagePath);
        }

        // extract internal @paths to external @pathTo
        public void Extract(List<string> paths, string pathTo)
        {
            this.fileSystem.CreateDirectory(pathTo);
            using (Stream input = this.fileSystem.Open(this.packagePath, FileMode.Open))
            {
                foreach (var path in paths)
                {           
                    packageExtractor.ExtractFileTo(input, this.header.GetItemForPath(path), pathTo,
                        extPath => {
                            var e = new AskToOverwriteEventArgs { Path = extPath };
                            this.AskToOverwrite(this, e);
                            return e.Overwrite;
                        },
                        process =>
                        {
                            this.FileProcessing(this, process);
                        }
                        );
                }
            }
        }

        // delete several internal @paths 
        public void Delete(List<string> paths)
        {
            List<PackageWriterTask> tasks = new List<PackageWriterTask>();

            foreach (var item in this.Items)
            {
                if (!paths.Contains(item.Path))
                    tasks.Add(new CopyTask { Item = item.Clone() });
            }

            this.ApplyChanges(tasks);
        }

        // create directory inside package
        // @path is an internal path to dir which can contains several un-existing directories
        public void CreateDirectory(string path)
        {
            List<PackageWriterTask> tasks = new List<PackageWriterTask>();

            foreach (var item in this.Items)
            {
                tasks.Add(new CopyTask { Item = item.Clone() });
            }

            tasks.Add(new CopyTask
            {
                Item = new DirectoryHeader
                {
                    Path = path,
                    Modified = DateTime.UtcNow
                }
            });

            this.ApplyChanges(tasks);
        }

        public IList<ItemHeader> Items
        {
            get => this.header.Items.Select(i => i).ToList();
        }

        public DirectoryHeader RootDirectory
        {
            get => this.header.RootDirectory;
        } 

        public event EventHandler<AskToOverwriteEventArgs> AskToOverwrite = delegate { };

        public event EventHandler<FileProcessingEventArgs> FileProcessing = delegate { };

        private string packagePath;
        private PackageHeader header = new PackageHeader();
        private readonly IFileSystem fileSystem;
        private readonly IPackageWriter packageWriter;
        private readonly IPackageExtractor packageExtractor;
    }
}
