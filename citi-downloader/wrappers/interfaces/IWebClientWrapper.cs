using System;
using System.Collections.Generic;
using System.Text;

namespace CitiDownloader.wrappers
{
    public interface IWebClientWrapper
    {
        void DownloadFile(string source, string destination);
    }
}
