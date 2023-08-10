using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpFileDownloader.Core
{
    public interface IDownloadStrategy
    {
        void Download(string url);
    }
}
