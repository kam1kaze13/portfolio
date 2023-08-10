using HttpServer.Core;
using System;

namespace HttpServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var program = new Program();
            var server = new Core.HttpServer(300, 10, 2000, new HttpProtocolExecutor(), new HttpServerEngine(program.GetContentStore()));
            server.Start();
        }

        private IContentStore GetContentStore()
        {
            var fileStore = new FileDirectoryContentStore("Content");
            var cgiStore = new CgiContentStore();
            var fastCgiStore = new FastCGIContentStore();

            var contentStoreSelector = new ContentStoreSelector();
            contentStoreSelector.AddRule(ContentRules.Extension(".jsk"), cgiStore);
            contentStoreSelector.AddRule(ContentRules.Directory("/Content"), fileStore);
            contentStoreSelector.AddRule(ContentRules.Directory("/FastCGI"), fastCgiStore);

            return contentStoreSelector;
        }
    }
}
