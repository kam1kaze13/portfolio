using HttpFileDownloader.Core;
using System;

namespace HttpFileDownloader
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("111");
            var httpDownloader = new HttpDownloader();

            httpDownloader.Download("https://www.sample-videos.com/img/Sample-jpg-image-30mb.jpg");
        }
    }
}
