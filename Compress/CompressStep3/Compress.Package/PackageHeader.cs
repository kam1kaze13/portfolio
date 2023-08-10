using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compress.Package
{
    public class PackageHeader
    {
        // list of all items in package, files and directories
        public List<ItemHeader> Items { get; set; } = new List<ItemHeader>();

        // represents root directory level of package
        public DirectoryHeader RootDirectory { get; set; }

        public void ReconstructInternalRelations(long contentStart)
        {
            var offset = contentStart;
            this.itemDic = new Dictionary<string, ItemHeader>();
            foreach (var item in this.Items)
            {
                this.itemDic.Add(item.Path, item);
                item.Directory = DirectoryHeader.FromDirectory(item.Path);
                Type t = item.GetType();
                if (t.Equals(typeof(FileHeader)))
                {
                    var fileItem = item as FileHeader;
                    fileItem.StartOffset = offset;
                    offset += fileItem.PackedLength;
                }
                else
                {
                    var dirItem = item as DirectoryHeader;
                    var childs = this.Items.Where(i => Path.GetDirectoryName(i.Path) == dirItem.Path);
                    dirItem.Items = childs.ToList();                                 
                }
            }
        }

        // return item with Path == 'path' otherwise null
        public ItemHeader GetItemForPath(string path)
        {
            if (itemDic.TryGetValue(path, out var item))
                return item;
            else
                return null;
        }

        private Dictionary<string, ItemHeader> itemDic;
    }

    public abstract class ItemHeader
    {
        public string Path { get; set; }

        public DateTime Modified { get; set; }

        public DirectoryHeader Directory { get; set; }

        public virtual ItemHeader Clone()
        {
            return (ItemHeader)this.MemberwiseClone();
        }
    }

    public class FileHeader : ItemHeader
    {
        public long PackedLength { get; set; }

        public long UnpackedLength { get; set; }

        public long StartOffset { get; set; }
    }

    public class DirectoryHeader : ItemHeader
    {
        public static DirectoryHeader FromDirectory(string path)
        {
            var directoryInfo = new DirectoryInfo(path);
            return new DirectoryHeader
            {
                Path = System.IO.Path.GetFileName(path),
                Modified = directoryInfo.LastWriteTimeUtc,
            };
        }

        public List<ItemHeader> Items { get; set; } = new List<ItemHeader>();
    }
}
