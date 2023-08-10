using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.Core
{
    public class FileDirectoryContentStore : IContentStore
    {
        private readonly string root;

        public FileDirectoryContentStore(string root)
        {
            this.root = root;
        }
        public IContent GetContent(string path)
        {
            var normalizedPath = this.NormalizePath(path);

            try
            {
                FileStream fs = File.OpenRead(this.root + normalizedPath);
                FileInfo fileInfo = new FileInfo(this.root + normalizedPath);
                return new FileDirectoryContent(normalizedPath, fs,fileInfo.LastWriteTimeUtc);
            }
            catch
            {
                return null;
            }
        }

        private string NormalizePath(string path)
        {
            return Path.Combine("/", path).Replace(@"\", "/").ToLower();
        }

        private class FileDirectoryContent : IContent
        {
            private string path;
            private CustomHttpResponse response;

            public FileDirectoryContent(string path, Stream body, DateTime lastModified)
            {
                this.path = path;
                this.response = new CustomHttpResponse
                {
                    Body = StreamUtil.StreamToString(body),
                    IfModifiedSince = lastModified,                    
                };
            }

            public string Path
            {
                get { return this.path; }
            }

            public CustomHttpResponse GetResponse(CustomHttpRequest request)
            {               
                return this.response;
            }
        }
    }
}
