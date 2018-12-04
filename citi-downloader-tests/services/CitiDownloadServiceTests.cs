using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Moq;
using SimpleFixture;


namespace CitiDownloaderTests.services
{
    [TestFixture]
    public class CitiDownloadServiceTests
    {
        [Test]
        public void DownloadFullFileSuccessfulTest()
        {
            // Setup
            string FullSavePath = "C:\\Dwonloads\filie.csv";
            Mock<ApplicationConfiguration> mockApplicationConfiguration = new Mock<ApplicationConfiguration>();
            Mock<IWebClientWrapper> mockWebClientWrapper = new Mock<IWebClientWrapper>();
            mockWebClientWrapper.Setup(f => f.DownloadFile(It.IsAny<string>(), FullSavePath));

            // Execute
            ICitiDownloadService citiDownloadService = new CitiDownloadService();

            // Verify
           
        }
    }
}
