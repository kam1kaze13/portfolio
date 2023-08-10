using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compress.Core;

namespace Compress.Package
{
    public class PackageWriter : IPackageWriter
    {
        public PackageWriter(IFileSystem fileSystem, ICryptoFactory packer)
        {
            this.fileSystem = fileSystem;
            this.packer = packer;
        }
        public PackageHeader Write(string newPackageName, string oldPackageName, List<PackageWriterTask> tasks, Action<FileProcessingEventArgs> fileProcessing)
        {
            var newHeader = new PackageHeader();
            
            foreach (var task in tasks)
            {
                if (task is CopyTask copyTask)
                {
                    newHeader.Items.Add(copyTask.Item);
                } 
                else if (task is PackTask packTask)
                {
                    newHeader.Items.Add(new FileHeader 
                    { 
                        Path = packTask.Path.InternalPath, 
                        Modified = this.fileSystem.GetLastWriteTimeUtc(packTask.Path.ExternalPath), 
                        PackedLength = 0, 
                        UnpackedLength = this.fileSystem.GetFileLength(packTask.Path.ExternalPath) 
                    });
                }
            }

            var output = this.fileSystem.Open(newPackageName, FileMode.CreateNew);
            this.serizalizator.Save(output, newHeader);

            FileStream input = null;
            try 
            {
                if (tasks.Any(i => i is CopyTask))
                {
                    input = new FileStream(oldPackageName, FileMode.Open);

                    var copyTasks = tasks.Where(i => i is CopyTask);

                    foreach (CopyTask task in copyTasks)
                    {
                        var item = newHeader.Items.Find(i => i.Path == task.Item.Path);
                        if (item is FileHeader fileItem)
                        {
                            input.Position = fileItem.StartOffset;

                            var copyStream = new StreamTransporter();

                            copyStream.DataProcessed += (_, args) =>
                            {
                                var e = new FileProcessingEventArgs
                                {
                                    FileName = Path.GetFileName(fileItem.Path),
                                    Type = FileProcessingType.Copy,
                                    Total = args.Total,
                                    TotalProcessed = args.TotalProcessed
                                };

                                fileProcessing(e);
                            };

                            copyStream.Copy(input, output, fileItem.PackedLength);
                        }                      
                    }
                }
            }
            finally 
            { 
                input?.Dispose(); 
            }

            var packTasks = tasks.Where(i => i is PackTask);

            foreach (PackTask task in packTasks)
            {
                Stream inputFile = null;
                try
                {
                    inputFile = this.fileSystem.Open(task.Path.ExternalPath, FileMode.Open);
                    var lengthBefore = output.Position;

                    if (Path.GetExtension(task.Path.ExternalPath) == ".pkg")
                    {
                        var copyStream = new StreamTransporter();

                        copyStream.DataProcessed += (_, args) =>
                        {
                            var e = new FileProcessingEventArgs
                            {
                                FileName = Path.GetFileName(task.Path.InternalPath),
                                Type = FileProcessingType.Copy,
                                Total = args.Total,
                                TotalProcessed = args.TotalProcessed
                            };

                            fileProcessing(e);
                        };

                        copyStream.Copy(inputFile, output, inputFile.Length);
                    }
                    else
                    {
                        var cryptoStream = new CryptoStreamTransporter();

                        cryptoStream.DataProcessed += (_, args) =>
                        {
                            var e = new FileProcessingEventArgs
                            {
                                FileName = Path.GetFileName(task.Path.InternalPath),
                                Type = FileProcessingType.Pack,
                                Total = args.Total,
                                TotalProcessed = args.TotalProcessed
                            };

                            fileProcessing(e);
                        };

                        cryptoStream.Pack(inputFile, output, inputFile.Length, this.packer.CreatePacker());
                    }

                    var item = newHeader.Items.Find(i => i.Path == task.Path.InternalPath);
                    if (item is FileHeader fileItem)
                    {
                        fileItem.PackedLength = output.Position - lengthBefore;
                    }
                }
                finally
                {
                    inputFile?.Dispose();
                }
                
            }

            output.Position = 0;
            this.serizalizator.Save(output, newHeader);
            newHeader.ReconstructInternalRelations(output.Position);
           
            output.Dispose();

            return newHeader;
        }

        private HeaderSerializator serizalizator = new HeaderSerializator();

        private readonly ICryptoFactory packer;
        private readonly IFileSystem fileSystem;        
    }
}
