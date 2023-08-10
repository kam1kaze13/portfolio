using HttpFileDownloader.Core;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HttpFileDownloader.Test
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void MarkRegionTest()
        {
            var regionsTest = new List<DownloadRegion>();
            regionsTest.Add(new DownloadRegion
            {
                Start = 0,
                Length = 200,
                State = DownloadState.Free,
            });
            regionsTest.Add(new DownloadRegion
            {
                Start = 200,
                Length = 300,
                State = DownloadState.Free,
            });

            var dm = new DownloadMap(500);
            dm.MarkRegion(0, 200, DownloadState.Free);
            Assert.AreEqual(regionsTest, dm.GetRegions());
        }

        [Test]
        public void DownloadTest()
        {
            var httpDownloader = new HttpDownloader();

            httpDownloader.Download("https://www.sample-videos.com/img/Sample-jpg-image-30mb.jpg");
        }
    }
}