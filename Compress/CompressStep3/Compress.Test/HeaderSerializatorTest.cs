using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Compress.Package;
using NUnit.Framework;

namespace Compress.Test
{
    class HeaderSerializatorTest
    {
        [Test]
        public void WriteReadTestFile1Test()
        {
            var header = new PackageHeader
            {
                Items = new List<ItemHeader>()
                {
                    new FileHeader
                    {
                        Path = @"SomeFileName.txt",
                        PackedLength = 18832,
                        UnpackedLength = 67274,
                    },
                    new DirectoryHeader
                    {
                        Path = @"Folder",
                    },
                    new FileHeader
                    {
                        Path = @"Folder\Another file.fb2",
                        PackedLength = 103447,
                        UnpackedLength = 33458,
                    },
                    new FileHeader
                    {
                        Path = @"Folder\Caption.jpg",
                        PackedLength = 54377,
                        UnpackedLength = 52959,
                    },
                },
            };

            using (var ms = new MemoryStream())
            {
                var serializator = new HeaderSerializator();
                serializator.Save(ms, header);

                ms.Position = 0;
                var loadedHeader = serializator.Load(ms);

                AreEqualByJson(header, loadedHeader);
            }
        }

        [Test]
        public void DateTimeSerializtionTest()
        {
            var dt = DateTime.UtcNow;
            long l = dt.Ticks;
            var dt2 = DateTime.FromBinary(l);
            Assert.AreEqual(dt, dt2);
        }

        public static void AreEqualByJson(object expected, object actual)
        {
            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            var expectedJson = serializer.Serialize(expected);
            var actualJson = serializer.Serialize(actual);
            Assert.AreEqual(expectedJson, actualJson);
        }
    }
}
