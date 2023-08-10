using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HttpFileDownloader.Core
{
    public class FileWriter
    {
        private readonly string fileName;

        public FileWriter(string fileName)
        {
            this.fileName = fileName;
            File.Create(fileName);
        }

        static ReaderWriterLockSlim locker = new ReaderWriterLockSlim();

        public void Write(byte[] data, long start)
        {
            locker.EnterWriteLock();
            try
            {
                using (var fs = new FileStream(this.fileName, FileMode.Open))
                {
                    fs.Seek(start, SeekOrigin.Begin);
                    fs.Write(data, 0, data.Length);
                }
            }
            finally
            {
                locker.ExitWriteLock();
            }
        }
    }
}
