using Compress.Core;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compress.Package
{
    public class PackageFactory
    {
        public PackageFactory(int MaxCodeBitCount = 20)
        {
            this.MaxCodeBitCount = MaxCodeBitCount;

            this.serviceCollection = new ServiceCollection();

            this.serviceCollection.AddTransient<IFileSystem, FileSystem>();
            this.serviceCollection.AddTransient<IPackageWriter, PackageWriter>();
            this.serviceCollection.AddTransient<IPackageExtractor, PackageExtractor>();
            this.SetCryptoFactory(new LzwFactory(this.MaxCodeBitCount));
            this.serviceCollection.AddTransient<PackageFile>();
        }

        public PackageFile CreatePackage()
        {         
            var services = this.serviceCollection.BuildServiceProvider();

            return services.GetRequiredService<PackageFile>();
        }

        public void SetCryptoFactory(ICryptoFactory cryptoFactory)
        {
            this.serviceCollection.AddSingleton<ICryptoFactory>(cryptoFactory);
        }

        public void SetPackageFileSystem()
        {
            this.serviceCollection.AddSingleton<PackageFileSystem>();
        }

        private ServiceCollection serviceCollection;
        private int MaxCodeBitCount;
    }
}
