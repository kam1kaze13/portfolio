using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HttpFileDownloader.Core
{
    public class Strategy
    {
        private int maxRegionCount = 10;
        private long maxRegionSize = 1048576;

        private DownloadMap downloadMap;       

        public Strategy(DownloadMap downloadMap)
        {
            this.downloadMap = downloadMap;
        }

        public void Split()
        {
            var regions = this.downloadMap.GetRegions();

            var planned = regions.Where(i => i != null && i.State == DownloadState.Planned).Count();
            var inProcess = regions.Where(i => i != null && i.State == DownloadState.InProcess).Count();

            while (planned + inProcess < maxRegionCount)
            {
                var freeRegion = regions.Where(i => i.State == DownloadState.Free).ToList().FirstOrDefault();

                if (freeRegion == null)
                    break;

                if (freeRegion.Length > 2 * this.maxRegionSize)
                    this.downloadMap.MarkRegion(freeRegion.Start, this.maxRegionSize, DownloadState.Planned);
                else if (freeRegion.Length > this.maxRegionSize)
                    this.downloadMap.MarkRegion(freeRegion.Start, freeRegion.Length / 2, DownloadState.Planned);
                else
                    this.downloadMap.MarkRegion(freeRegion.Start, freeRegion.Length, DownloadState.Planned);

                ++planned;
            }
        }
    }
}
