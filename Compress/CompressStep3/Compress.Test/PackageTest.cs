﻿using Compress.Package;
using Compress.Core;
using Compress.Test.Package;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compress.Test
{
    class PackageTest
    {
        private IServiceProvider GetServicesForRealFileSystem()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddTransient<ICryptoPacker, LzwPacker>();
            serviceCollection.AddTransient<ICryptoUnpacker, LzwUnpacker>();
            serviceCollection.AddTransient<IFileSystem, FileSystem>();
            serviceCollection.AddTransient<IPackageWriter, PackageWriter>();
            serviceCollection.AddTransient<IPackageExtractor, PackageExtractor>();
            serviceCollection.AddTransient<PackageFile>();

            return serviceCollection.BuildServiceProvider();
        }

        private IServiceProvider GetServicesForMemoryFileSystem()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSingleton<IFileSystem>(new MemoryFileSystem(GetPathFor(@"TestData")));
            serviceCollection.AddSingleton<IPackageWriter, PackageWriter>();
            serviceCollection.AddSingleton<IPackageExtractor, PackageExtractor>();
            serviceCollection.AddTransient<PackageFile>();

            return serviceCollection.BuildServiceProvider();
        }

        [Test]
        public void Package1Test()
        {
            var services = GetServicesForRealFileSystem();

            var fileSystem = services.GetRequiredService<IFileSystem>();
            fileSystem.Delete(GetPathFor(@"TestData\TestFile1.pkg"));

            //var package = services.GetRequiredService<PackageFile>();

            var package = new PackageFactory().CreatePackage();

            package.Open(GetPathFor(@"TestData\TestFile1.pkg"));

            package.Add(new List<string> {
                GetPathFor(@"TestData\TestFile1.txt"),
            });

            fileSystem.Delete(GetPathFor(@"TestData\TestFile1.pkg"));
        }

        [Test]
        public void Package2Test()
        {
            var services = GetServicesForRealFileSystem();

            var fileSystem = services.GetRequiredService<IFileSystem>();
            fileSystem.Delete(GetPathFor(@"TestData\TestFile2.pkg"));

            var package = services.GetRequiredService<PackageFile>();
            package.Open(GetPathFor(@"TestData\TestFile2.pkg"));

            package.Add(new List<string> {
                GetPathFor(@"TestData\TestFile1.txt"),
                GetPathFor(@"TestData\TestFile2.fb2"),
            });

            fileSystem.Delete(GetPathFor(@"TestData\TestFile2.pkg"));
        }

        [Test]
        public void Package3Test()
        {
            var services = GetServicesForRealFileSystem();

            var fileSystem = services.GetRequiredService<IFileSystem>();
            fileSystem.Delete(GetPathFor(@"TestData\TestFile3.pkg"));

            var package = services.GetRequiredService<PackageFile>();
            package.Open(GetPathFor(@"TestData\TestFile3.pkg"));

            package.Add(new List<string> {
                GetPathFor(@"TestData"),
            });

            fileSystem.Delete(GetPathFor(@"TestData\TestFile3.pkg"));
        }

        [Test]
        public void Package4Test()
        {
            var services = GetServicesForRealFileSystem();

            var fileSystem = services.GetRequiredService<IFileSystem>();
            fileSystem.Delete(GetPathFor(@"TestData\TestFile4.pkg"));

            var package = services.GetRequiredService<PackageFile>();
            package.Open(GetPathFor(@"TestData\TestFile4.pkg"));

            package.Add(new List<string> {
                GetPathFor(@"TestData\TestFile1.txt"),
                GetPathFor(@"TestData\TestFile2.fb2"),
            });

            package.Add(new List<string> {
                GetPathFor(@"TestData\TestFile2.fb2"),
            });

            fileSystem.Delete(GetPathFor(@"TestData\TestFile4.pkg"));
        }

        [Test]
        public void Unpack1Test()
        {
            var services = GetServicesForRealFileSystem();

            var fileSystem = services.GetRequiredService<IFileSystem>();
            fileSystem.Delete(GetPathFor(@"TestData\TestFile1.pkg"));

            var package = services.GetRequiredService<PackageFile>();
            package.Open(GetPathFor(@"TestData\TestFile1.pkg"));

            package.Add(new List<string> {
                GetPathFor(@"TestData\TestFile1.txt"),
                GetPathFor(@"TestData\TestFile2.fb2"),
            });

            fileSystem.Delete(GetPathFor(@"TestData\extracted"));

            package.Extract(new List<string> { "TestFile1.txt" }, GetPathFor(@"TestData\extracted"));

            fileSystem.Delete(GetPathFor(@"TestData\extracted"));
            fileSystem.Delete(GetPathFor(@"TestData\TestFile1.pkg"));
        }

        [Test]
        public void Delete1Test()
        {
            var services = GetServicesForRealFileSystem();

            var fileSystem = services.GetRequiredService<IFileSystem>();
            fileSystem.Delete(GetPathFor(@"TestData\TestFile1.pkg"));

            var package = services.GetRequiredService<PackageFile>();
            package.Open(GetPathFor(@"TestData\TestFile1.pkg"));

            package.Add(new List<string> {
                GetPathFor(@"TestData\TestFile1.txt"),
                GetPathFor(@"TestData\TestFile2.fb2"),
            });

            package.Delete(new List<string> { @"TestFile2.fb2" });

            fileSystem.Delete(GetPathFor(@"TestData\extracted"));

            package.Extract(new List<string> { "TestFile1.txt" }, GetPathFor(@"TestData\extracted"));

            fileSystem.Delete(GetPathFor(@"TestData\extracted\TestFile1.txt"));
        }

        [Test]
        public void MemoryComplex1Test()
        {
            var services = GetServicesForMemoryFileSystem();
            Complex1Test(services);
        }
        [Test]
        public void RealComplex1Test()
        {
            var services = GetServicesForRealFileSystem();
            Complex1Test(services);
        }

        public void Complex1Test(IServiceProvider services)
        {
            var fileSystem = services.GetRequiredService<IFileSystem>();

            this.CreateFileSample1(fileSystem);

            fileSystem.Delete(GetPathFor(@"TestData\TestFile1.pkg"));

            var package = services.GetRequiredService<PackageFile>();
            package.Open(GetPathFor(@"TestData\TestFile1.pkg"));

            package.Add(new List<string> {
                GetPathFor(@"TestData\Books"),
                GetPathFor(@"TestData\Pedia"),
            });

            fileSystem.Delete(GetPathFor(@"TestData\Pedia\Nature\Mammal\Cat_article.txt"));
            fileSystem.CreateTextFile(GetPathFor(@"TestData\Pedia\Nature\Mammal\Cat_article.txt"), "Advanced cats' information");
            fileSystem.CreateTextFile(GetPathFor(@"TestData\Pedia\Nature\Mammal\Orka_article.txt"), "Killer whale");
            package.Add(new List<string> { GetPathFor(@"TestData\Pedia") });

            package.Delete(new List<string> { @"Pedia\Nature\Wolf_article.txt", @"Pedia\Geography" });

            fileSystem.Delete(GetPathFor(@"TestData\extracted"));

            package.Extract(new List<string> { "Pedia", "Books" }, GetPathFor(@"TestData\extracted"));

            Assert.IsTrue(fileSystem.DirectoryExists(GetPathFor(@"TestData\extracted\Pedia\Nature")));
            Assert.IsFalse(fileSystem.DirectoryExists(GetPathFor(@"TestData\extracted\Pedia\Geography")));
            Assert.IsTrue(fileSystem.DirectoryExists(GetPathFor(@"TestData\extracted\Pedia\Nature")));
            Assert.IsFalse(fileSystem.FileExists(GetPathFor(@"TestData\extracted\Pedia\Nature\Wolf_article.txt")));

            Assert.AreEqual(fileSystem.ReadTextFile(GetPathFor(@"TestData\Pedia\Nature\Mammal\Orka_article.txt")), "Killer whale");
            Assert.AreEqual(fileSystem.ReadTextFile(GetPathFor(@"TestData\Pedia\Nature\Mammal\Cat_article.txt")), "Advanced cats' information");

            fileSystem.Delete(GetPathFor(@"TestData\extracted\"));
            fileSystem.Delete(GetPathFor(@"TestData\Books"));
            fileSystem.Delete(GetPathFor(@"TestData\Pedia\"));
        }


        [Test]
        public void MemoryAddIntoTest()
        {
            var services = GetServicesForMemoryFileSystem();
            AddIntoTest(services);
        }
        [Test]
        public void RealAddIntoTest()
        {
            var services = GetServicesForRealFileSystem();
            AddIntoTest(services);
        }

        public void AddIntoTest(IServiceProvider services)
        {
            var fileSystem = services.GetRequiredService<IFileSystem>();

            this.CreateFileSample1(fileSystem);

            fileSystem.Delete(GetPathFor(@"TestData\TestFile1.pkg"));

            //var package = services.GetRequiredService<PackageFile>();

            var package = new PackageFactory().CreatePackage();
            package.Open(GetPathFor(@"TestData\TestFile1.pkg"));

            package.Add(new List<string> {
                GetPathFor(@"TestData\Books"),
                GetPathFor(@"TestData\Pedia"),
            });

            package.CreateDirectory("Alt");
            package.Add(new List<string> { GetPathFor(@"TestData\Pedia") }, "Alt");

            fileSystem.Delete(GetPathFor(@"TestData\extracted"));

            package.Extract(new List<string> { "Pedia", "Alt" }, GetPathFor(@"TestData\extracted"));

            Assert.IsTrue(fileSystem.DirectoryExists(GetPathFor(@"TestData\extracted\Pedia\Nature")));
            Assert.IsTrue(fileSystem.DirectoryExists(GetPathFor(@"TestData\extracted\Alt\Pedia\Nature")));

            Assert.AreEqual(fileSystem.ReadTextFile(GetPathFor(@"TestData\extracted\Alt\Pedia\Nature\Mammal\Wolf_article.txt")), "Wolf attacks!");

            fileSystem.Delete(GetPathFor(@"TestData\extracted\"));
            fileSystem.Delete(GetPathFor(@"TestData\Books"));
            fileSystem.Delete(GetPathFor(@"TestData\Pedia\"));
        }

        private void CreateFileSample1(IFileSystem fileSystem)
        {
            fileSystem.Delete(GetPathFor(@"TestData\Books"));
            fileSystem.Delete(GetPathFor(@"TestData\Pedia\"));
            fileSystem.CreateTextFile(GetPathFor(@"TestData\Books\index.txt"), "Some index");
            fileSystem.CreateTextFile(GetPathFor(@"TestData\Books\Ivanov\bestseller.txt"), "Text of Ivanov's bestseller");
            fileSystem.CreateTextFile(GetPathFor(@"TestData\Books\Yakovlev\my_way.txt"), "Yak father living way");
            fileSystem.CreateTextFile(GetPathFor(@"TestData\Pedia\Nature\Mammal_article.txt"), "Mammal pedia article");
            fileSystem.CreateTextFile(GetPathFor(@"TestData\Pedia\Nature\Mammal\Cat_article.txt"), "Some information about cats");
            fileSystem.CreateTextFile(GetPathFor(@"TestData\Pedia\Nature\Mammal\Wolf_article.txt"), "Wolf attacks!");
            fileSystem.CreateTextFile(GetPathFor(@"TestData\Pedia\Geography\USA_article.txt"), "Only world hegemon");
        }

        private string GetPathFor(string name)
        {
            return Path.Combine(TestContext.CurrentContext.TestDirectory, name);
        }
    }
}
