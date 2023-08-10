using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.Core
{
    public class ContentStoreSelector : IContentStore
    {
        private Queue<(Func<string, bool>, IContentStore)> rules = new();

        public IContent GetContent(string path)
        {
            string fullPath = this.NormalizePath(path);

            foreach (var rule in rules)
            {
                if (rule.Item1(fullPath))
                {
                    IContent content;

                    if (rule.Item2 is CgiContentStore)
                    {
                        content = rule.Item2.GetContent(fullPath);
                    }
                    else
                    {
                        content = rule.Item2.GetContent(fullPath.Substring(fullPath.IndexOf("/", fullPath.IndexOf("/") + 1)));
                    }
                    
                    if (content != null)
                        return content;
                }
            }

            return null;
        }

        public void AddRule(Func<string, bool> condition, IContentStore store) 
        {
            rules.Enqueue((condition, store));
        }

        private string NormalizePath(string path)
        {
            return Path.Combine("/", path).Replace(@"\", "/").ToLower();
        }
    }
}
