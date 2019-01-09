using System.Net;

namespace TrainingDownloader.wrappers
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
