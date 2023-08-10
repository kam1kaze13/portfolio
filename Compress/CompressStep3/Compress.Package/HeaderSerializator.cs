using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compress.Package
{
    // reading/writing PackageHeader into Stream
    public partial class HeaderSerializator
    {
        public PackageHeader Load(Stream input)
        {
            int count = this.LoadInt(input);
            ItemHeader[] item = new ItemHeader[count];

            for (int i = 0; i < count; i++)
            {
                item[i] = this.LoadItemHeader(input);
            }

            var packageHeader = new PackageHeader { Items = item.ToList(), };

            return packageHeader;
        }

        public void Save(Stream output, PackageHeader header)
        {
            this.SaveInt(output, Convert.ToInt32(header.Items.Count));
            foreach (var item in header.Items)
            {
                this.SaveItemHeader(output, item);
            }
        }

        private ItemHeader LoadItemHeader(Stream input)
        {
            var type = this.LoadInt(input);

            ItemHeader itemHeader = null;

            var path = this.LoadString(input);
            var modified = this.LoadDateTime(input);

            if (type == Convert.ToInt32(ItemType.File))
            {
                itemHeader = this.LoadFileHeader(input);
            }
            else
            {
                itemHeader = this.LoadDirectoryHeader(input);
            }

            itemHeader.Path = path;
            itemHeader.Modified = modified;

            return itemHeader;
        }

        private void SaveItemHeader(Stream output, ItemHeader itemHeader)
        {
            switch (itemHeader)
            {
                case FileHeader fileHeader:
                    {
                        this.SaveFileHeader(output, fileHeader);
                        break;
                    }

                case DirectoryHeader dirHeader:
                    {
                        this.SaveDirectoryHeader(output, dirHeader);
                        break;
                    }
            }
        }

        private FileHeader LoadFileHeader(Stream input)
        {
            var fileHeader = new FileHeader();

            fileHeader.PackedLength = this.LoadLong(input);
            fileHeader.UnpackedLength = this.LoadLong(input);

            return fileHeader;
        }

        private void SaveFileHeader(Stream output, FileHeader fileHeader)
        {
            this.SaveInt(output, Convert.ToInt32(ItemType.File));//type            
            this.SaveString(output, fileHeader.Path);//path
            this.SaveDateTime(output, fileHeader.Modified);//date of last modified
            this.SaveLong(output, fileHeader.PackedLength);//packed length
            this.SaveLong(output, fileHeader.UnpackedLength);//unpacked length
        }

        private DirectoryHeader LoadDirectoryHeader(Stream input)
        {
            var dirHeader = new DirectoryHeader();

            return dirHeader;
        }

        private void SaveDirectoryHeader(Stream output, DirectoryHeader directoryHeader)
        {
            this.SaveInt(output, Convert.ToInt32(ItemType.Directory));//type
            this.SaveString(output, directoryHeader.Path);//path
            this.SaveDateTime(output, directoryHeader.Modified);//date of last modified
        }

        private string LoadString(Stream input)
        {
            var lengthStr = this.LoadInt(input);
            var buffer = new byte[lengthStr];
            input.Read(buffer, 0, buffer.Length);
            return Encoding.UTF8.GetString(buffer);
        }

        private void SaveString(Stream output, string str)
        {
            var buffer = Encoding.UTF8.GetBytes(str);
            this.SaveInt(output, buffer.Length);
            output.Write(buffer, 0, buffer.Length);
        }

        private DateTime LoadDateTime(Stream input)
        {
            var longTime = this.LoadLong(input);
            DateTime dt = DateTime.FromBinary(longTime);
            return dt;
        }

        private void SaveDateTime(Stream output, DateTime dt)
        {
            var buffer = BitConverter.GetBytes(dt.Ticks);
            output.Write(buffer, 0, buffer.Length);
        }

        private int LoadInt(Stream input)
        {
            var buffer = new byte[4];
            input.Read(buffer, 0, 4);
            return BitConverter.ToInt32(buffer, 0);
        }

        private void SaveInt(Stream output, int i)
        {
            var buffer = BitConverter.GetBytes(i);
            output.Write(buffer, 0, buffer.Length);
        }

        private long LoadLong(Stream input)
        {
            var buffer = new byte[8];
            input.Read(buffer, 0, 8);
            return BitConverter.ToInt64(buffer, 0);
        }

        private void SaveLong(Stream output, long l)
        {
            var buffer = BitConverter.GetBytes(l);
            output.Write(buffer, 0, buffer.Length);
        }
    }
}
