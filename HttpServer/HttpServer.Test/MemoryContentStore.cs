using HttpServer.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.Test
{
    public class MemoryContentStore : IContentStore
    {
        private Dictionary<string, MemoryContent> store = new Dictionary<string, MemoryContent>();

        public void AddContentItem(string path, string body, DateTime lastModified)
        {
            string normalizedPath = this.NormalizePath(path);
            this.store.Add(normalizedPath, new MemoryContent(normalizedPath, body, lastModified));
        }

        public IContent GetContent(string path)
        {
            MemoryContent content;
            this.store.TryGetValue(this.NormalizePath(path), out content);

            return content;
        }

        private string NormalizePath(string path)
        {
            return Path.Combine("/", path).Replace(@"\", "/").ToLower();
        }

        private class MemoryContent : IContent
        {
            private readonly string path;
            private readonly CustomHttpResponse response;

            public MemoryContent(string path, string body, DateTime lastModified)
            {
                this.path = path;
                this.response = new CustomHttpResponse
                {
                    Body = body,
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
