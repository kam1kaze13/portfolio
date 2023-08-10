using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HttpFileDownloader.Core
{
    public enum DownloadState
    {
        Free,
        Downloaded,
        Planned,
        InProcess,
    }

    public record DownloadRegion
    {
        public long Start { get; set; }

        public long Length { get; set; }

        public DownloadState State { get; set; }
    }


    public class DownloadMap
    {
        private List<DownloadRegion> downloadRegions;

        public DownloadMap(List<DownloadRegion> downloadRegions)
        {
            this.downloadRegions = downloadRegions;
        }

        public DownloadMap(long size)
        {
            this.downloadRegions = new List<DownloadRegion>();
            downloadRegions.Add(new DownloadRegion
            {
                Start = 0,
                Length = size,
                State = DownloadState.Free
            });
        }

        public bool IsDownloaded()
        {
            var downloaded = this.downloadRegions.Where(i => i != null && i.State == DownloadState.Downloaded).Count();
            if (downloaded == this.downloadRegions.Count)
                return true;

            return false;
        }

        public List<DownloadRegion> GetRegions()
        {
            return this.downloadRegions;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void MarkRegion(long offset, long size, DownloadState state)
        {
            var region = this.downloadRegions.Where(i => i.Start == offset).First();
            if (region == null)
            {
                return;
            }

            this.downloadRegions.Add(new DownloadRegion
            {
                Start = offset,
                Length = size,
                State = state
            });

            if (region.Length - size > 0)
            {
                this.downloadRegions.Add(new DownloadRegion
                {
                    Start = size+offset,
                    Length = region.Length - size,
                    State = region.State
                });
            }
            
            this.downloadRegions.Remove(region);
        }
    }

}
