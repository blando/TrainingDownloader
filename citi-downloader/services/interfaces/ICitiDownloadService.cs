using System;
using System.Collections.Generic;
using System.Text;

namespace CitiDownloader.services
{
    public interface ICitiDownloadService
    {
        string DownloadFullFile();
        string DownloadIncrementalFile();
    }
}
