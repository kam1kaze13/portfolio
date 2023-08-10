using System;
using System.Collections.Generic;

namespace Compress.Package
{
    public abstract class PackageWriterTask
    {
        public ItemHeader OutputItem { get; set; }
    }

    public class CopyTask : PackageWriterTask
    {
        public ItemHeader Item { get; set; }
    }

    public class PackTask : PackageWriterTask
    {
        public ExternalInternalPath Path { get; set; }
    }

    public interface IPackageWriter
    {
        // write new package on the base of tasks list
        // tasks includes
        //      CopyTask means copy part of old package into new place in new package
        //      PackTask means packing of file
        PackageHeader Write(string newPackageName, string oldPackageName, List<PackageWriterTask> tasks, Action<FileProcessingEventArgs> fileProcessing);
    }
}