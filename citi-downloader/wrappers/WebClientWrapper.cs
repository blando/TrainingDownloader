using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace CitiDownloader.wrappers
{
    public class WebClientWrapper : IWebClientWrapper
    {
        public void DownloadFile(string source, string destination)
        {
            using (WebClient client = new WebClient())
            {
                 client.DownloadFile(source, destination);
            }
        }
    }
}
