using Compress.Package;
using Compress.Test.Package;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compress.Test
{
    class FileSystemTest
    {

        [Test]
        public void MemorySimpleTest()
        {
            var fileSystem = new MemoryFileSystem(GetPathFor(@"TestData"));
            SimpleTest(fileSystem);
        }

        [Test]
        public void MemoryEnclosedFoldersTest()
        {
            var fileSystem = new MemoryFileSystem(GetPathFor(@"TestData"));
            EnclosedFoldersTest(fileSystem);
        }

        [Test]
        public void RealSimpleTest()
        {
            var fileSystem = new FileSystem();
            SimpleTest(fileSystem);
        }

        [Test]
        public void RealEnclosedFoldersTest()
        {
            var fileSystem = new FileSystem();
            EnclosedFoldersTest(fileSystem);
        }

        public void SimpleTest(IFileSystem fileSystem)
        {
            fileSystem.CreateDirectory(GetPathFor(@"TestData\NewDir"));
            fileSystem.CreateTextFile(GetPathFor(@"TestData\NewDir\test.txt"), "some text data");

            Assert.IsTrue(fileSystem.DirectoryExists(GetPathFor(@"TestData\NewDir")));
            Assert.IsTrue(fileSystem.FileExists(GetPathFor(@"TestData\NewDir\test.txt")));
            Assert.IsFalse(fileSystem.FileExists(GetPathFor(@"TestData\NewDir\")));
            Assert.IsFalse(fileSystem.DirectoryExists(GetPathFor(@"TestData\NewDir\test.txt")));
            Assert.IsTrue(fileSystem.IsDirectory(GetPathFor(@"TestData\NewDir\")));

            Assert.AreEqual(fileSystem.ReadTextFile(GetPathFor(@"TestData\NewDir\test.txt")), "some text data");

            fileSystem.Delete(GetPathFor(@"TestData\NewDir\test.txt"));
            Assert.IsFalse(fileSystem.FileExists(GetPathFor(@"TestData\NewDir\test.txt")));

            fileSystem.Delete(GetPathFor(@"TestData\NewDir\"));
            Assert.IsFalse(fileSystem.DirectoryExists(GetPathFor(@"TestData\NewDir")));
        }


        public void EnclosedFoldersTest(IFileSystem fileSystem)
        {
            fileSystem.CreateTextFile(GetPathFor(@"TestData\Dir1\Dir2\Dir3\test.txt"), "some text data");

            Assert.IsTrue(fileSystem.DirectoryExists(GetPathFor(@"TestData\Dir1\Dir2")));
            Assert.IsTrue(fileSystem.FileExists(GetPathFor(@"TestData\Dir1\Dir2\Dir3\test.txt")));

            fileSystem.CreateDirectory(GetPathFor(@"TestData\Dir1\Dir4\Dir5"));

            Assert.IsTrue(fileSystem.DirectoryExists(GetPathFor(@"TestData\Dir1\Dir4")));

            fileSystem.Delete(GetPathFor(@"TestData\Dir1\Dir2"));

            Assert.IsFalse(fileSystem.DirectoryExists(GetPathFor(@"TestData\Dir1\Dir2\Dir3")));
            Assert.IsFalse(fileSystem.FileExists(GetPathFor(@"TestData\Dir1\Dir2\Dir3\test.txt")));

            fileSystem.Delete(GetPathFor(@"TestData\Dir1"));

            Assert.IsFalse(fileSystem.DirectoryExists(GetPathFor(@"TestData\Dir1\Dir4")));
            Assert.IsFalse(fileSystem.DirectoryExists(GetPathFor(@"TestData\Dir1")));
        }

        private string GetPathFor(string name)
        {
            return Path.Combine(TestContext.CurrentContext.TestDirectory, name);
        }
    }
}
