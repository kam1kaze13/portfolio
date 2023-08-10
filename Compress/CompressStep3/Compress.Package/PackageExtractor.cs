using Compress.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compress.Package
{
    public class PackageExtractor : IPackageExtractor
    {
        public PackageExtractor (IFileSystem fileSystem, ICryptoFactory unpacker)
        {
            this.fileSystem = fileSystem;
            this.unpacker = unpacker;
        }

        public void ExtractFileTo(Stream packageStream, ItemHeader item, string pathTo, Func<string, bool> shouldOverwrite, Action<FileProcessingEventArgs> fileProcessing)
        {
            switch (item)
            {
                case FileHeader fileHeader:
                    {
                        var newPath = Path.Combine(pathTo, Path.GetFileName(fileHeader.Path));

                        bool overwrite = true;

                        if (this.fileSystem.FileExists(newPath))
                            overwrite = shouldOverwrite(newPath);

                        if (overwrite)
                        {
                            Stream output = null;
                            try
                            {
                                output = this.fileSystem.Open(newPath, FileMode.OpenOrCreate);
                                packageStream.Position = fileHeader.StartOffset;

                                if (Path.GetExtension(newPath) == ".pkg")
                                {
                                    var copyStream = new StreamTransporter();

                                    copyStream.DataProcessed += (_, args) =>
                                    {
                                        var e = new FileProcessingEventArgs
                                        {
                                            FileName = Path.GetFileName(fileHeader.Path),
                                            Type = FileProcessingType.Copy,
                                            Total = args.Total,
                                            TotalProcessed = args.TotalProcessed
                                        };

                                        fileProcessing(e);
                                    };

                                    copyStream.Copy(packageStream, output, fileHeader.PackedLength);
                                }
                                else
                                {
                                    var cryptoStream = new CryptoStreamTransporter();

                                    cryptoStream.DataProcessed += (_, args) =>
                                    {
                                        var e = new FileProcessingEventArgs
                                        {
                                            FileName = Path.GetFileName(fileHeader.Path),
                                            Type = FileProcessingType.Unpack,
                                            Total = args.Total,
                                            TotalProcessed = args.TotalProcessed
                                        };

                                        fileProcessing(e);
                                    };

                                    cryptoStream.Unpack(packageStream, output, fileHeader.PackedLength, this.unpacker.CreateUnpacker());
                                }                                                        
                            }
                            finally
                            {
                                output?.Dispose();
                            }
                        }
                        break;
                    }

                case DirectoryHeader dirHeader:
                    {
                        var newPath = Path.Combine(pathTo, Path.GetFileName(dirHeader.Path));
                        if (shouldOverwrite(newPath) == default || shouldOverwrite(newPath))
                        {
                            this.fileSystem.CreateDirectory(newPath);

                            foreach (var childItem in dirHeader.Items)
                            {
                                ExtractFileTo(packageStream, childItem, newPath, shouldOverwrite, fileProcessing);
                            }
                        }                                        
                        break;
                    }
            }
        }

        private readonly ICryptoFactory unpacker;
        private readonly IFileSystem fileSystem;
    }
}
