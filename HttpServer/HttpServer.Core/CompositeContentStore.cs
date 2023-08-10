using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.Core
{
    public class CompositeContentStore : IContentStore
    {
        private Dictionary<string, IContentStore> stores = new();

        public void AddAssociation(string path, IContentStore store)
        {
            this.stores.Add(this.NormalizePath(path), store);
        }

        public IContent GetContent(string path)
        {
            string fullPath = this.NormalizePath(path);
            foreach (var item in this.stores)
            {
                if (fullPath.StartsWith(item.Key))
                {
                    var content = item.Value.GetContent(fullPath.Substring(item.Key.Length));
                    if (content != null)
                        return content;
                }
            }

            return null;
        }

        private string NormalizePath(string path)
        {
            return Path.Combine("/", path).Replace(@"\", "/").ToLower();
        }
    }
}
