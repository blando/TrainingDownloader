using System;
using System.Collections.Generic;
using System.Text;

namespace TrainingDownloader.wrappers
{
    public interface IWebClientWrapper
    {
        void DownloadFile(string source, string destination);
    }
}
