using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HttpFileDownloader.Core
{
    public class HttpDownloader
    {
        private const int BufferSize = 10240;// for headers

        private DownloadMap downloadMap;
        private IPEndPoint endPoint;
        private string url;

        public void Download(string url)
        {
            this.url = url;
            string downloadsFolderPath = Syroot.Windows.IO.KnownFolders.Downloads.Path;
            var uri = new Uri(url);
            var fileServerPath = uri.LocalPath;
            var fileName = downloadsFolderPath + "\\" + fileServerPath.Substring(fileServerPath.LastIndexOf("/") + 1);
            var ipAddress = NetUtil.ResolveIpAddress(uri.Host);
            this.endPoint = new IPEndPoint(ipAddress, 80);

            var contentLength = GetContentLength();

            this.downloadMap = new DownloadMap(contentLength);
            var strategy = new Strategy(this.downloadMap);
            var fileWriter = new FileWriter(fileName);

            while (!this.downloadMap.IsDownloaded())
            {
                strategy.Split();

                var regions = this.downloadMap.GetRegions();

                var planned = regions.Where(i => i != null && i.State == DownloadState.Planned).ToList();

                foreach (var region in planned)
                {
                    this.downloadMap.MarkRegion(region.Start, region.Length, DownloadState.InProcess);
                    Task.Run(() => GetPartOfFile(region, fileWriter));
                    Thread.Sleep(100);                  
                }
            }              
        }

        private long GetContentLength()
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(this.endPoint);

            var byteRequest = HttpProtocol.CreateRequest(new HttpRequest(HttpMethod.HEAD, this.url));

            socket.Send(byteRequest);

            var buffer = new byte[BufferSize];

            int bytesReceive = socket.Receive(buffer);

            var response = HttpProtocol.ParseOutput(buffer, 0, bytesReceive);

            socket.Shutdown(SocketShutdown.Both);
            socket.Close();

            return response.ContentLength;
        }

        private void GetPartOfFile(DownloadRegion region, FileWriter writer)
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(endPoint);

            var byteRequest = HttpProtocol.CreateRequest(new HttpRequest(HttpMethod.GET, this.url, region.Start, region.Length));

            socket.Send(byteRequest);

            var buffer = new byte[region.Length + BufferSize];

            int bytesReceive;
            int offset = 0;

            do
            {
                bytesReceive = socket.Receive(buffer, offset, (int)(buffer.Length-offset), SocketFlags.None);
                offset += bytesReceive;
            }
            while (bytesReceive > 0);

            var response = HttpProtocol.ParseOutput(buffer, 0, offset);
            writer.Write(response.Body, region.Start);

            this.downloadMap.MarkRegion(region.Start, region.Length, DownloadState.Downloaded);

            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }
    }
}
