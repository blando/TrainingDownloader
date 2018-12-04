using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using SimpleFixture;
using CitiDownloader.wrappers;
using CitiDownloader.services;
using NUnit.Framework.Constraints;
using System.Net;

namespace CitiDownloaderTests.services
{
    [TestFixture]
    public class CitiDownloaderServiceTests
    {
        private Fixture fixture;
        public CitiDownloaderServiceTests()
        {
            this.fixture = new Fixture();
        }

        [Test]
        public void DownloadFullFileSuccessfulTest()
        {
            // Setup
            string fullFile = fixture.Generate<string>();
            string fullSavePath = fixture.Generate<string>();
            Mock<ApplicationConfiguration> mockApplicationConfiguration = new Mock<ApplicationConfiguration>();
            mockApplicationConfiguration.SetupGet(p => p.FullFile).Returns(fullFile);
            mockApplicationConfiguration.SetupGet(p => p.FullSavePath).Returns(fullSavePath);
            Mock<IWebClientWrapper> mockWebClientWrapper = new Mock<IWebClientWrapper>();
            mockWebClientWrapper.Setup(f => f.DownloadFile(fullFile, fullSavePath)).Verifiable();


            // Execute
            ICitiDownloadService citiDownloadService = new CitiDownloadService(mockApplicationConfiguration.Object, mockWebClientWrapper.Object);
            string response = citiDownloadService.DownloadFullFile();


            // Verify
            Assert.That(response == fullSavePath);
            Mock.Verify(mockWebClientWrapper);
            mockWebClientWrapper.Verify(f => f.DownloadFile(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void DownloadFullFileWebExceptionTest()
        {
            // Setup
            string fullFile = fixture.Generate<string>();
            string fullSavePath = fixture.Generate<string>();
            Mock<ApplicationConfiguration> mockApplicationConfiguration = new Mock<ApplicationConfiguration>();
            mockApplicationConfiguration.SetupGet(p => p.FullFile).Returns(fullFile);
            mockApplicationConfiguration.SetupGet(p => p.FullSavePath).Returns(fullSavePath);
            Mock<IWebClientWrapper> mockWebClientWrapper = new Mock<IWebClientWrapper>();
            mockWebClientWrapper.Setup(f => f.DownloadFile(fullFile, fullSavePath)).Throws(new WebException());

            // Execute
            ICitiDownloadService citiDownloadService = new CitiDownloadService(mockApplicationConfiguration.Object, mockWebClientWrapper.Object);
            ActualValueDelegate<object> testDelegate = () => citiDownloadService.DownloadFullFile();

            // Verify
            Assert.That(testDelegate, Throws.TypeOf<WebException>());
        }

        [Test]
        public void DownloadIncrementalFileSuccessfulTest()
        {
            // Setup
            string incrementalFile = fixture.Generate<string>();
            string incrementalSavePath = fixture.Generate<string>();
            Mock<ApplicationConfiguration> mockApplicationConfiguration = new Mock<ApplicationConfiguration>();
            mockApplicationConfiguration.SetupGet(p => p.IncrementalFile).Returns(incrementalFile);
            mockApplicationConfiguration.SetupGet(p => p.IncrementalSavePath).Returns(incrementalSavePath);
            Mock<IWebClientWrapper> mockWebClientWrapper = new Mock<IWebClientWrapper>();
            mockWebClientWrapper.Setup(f => f.DownloadFile(incrementalFile, incrementalSavePath)).Verifiable();


            // Execute
            ICitiDownloadService citiDownloadService = new CitiDownloadService(mockApplicationConfiguration.Object, mockWebClientWrapper.Object);
            string response = citiDownloadService.DownloadIncrementalFile();


            // Verify
            Assert.That(response == incrementalSavePath);
            Mock.Verify(mockWebClientWrapper);
            mockWebClientWrapper.Verify(f => f.DownloadFile(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void DownloadIncrementalFileWebExceptionTest()
        {
            // Setup
            string incrementalFile = fixture.Generate<string>();
            string incrementalSavePath = fixture.Generate<string>();
            Mock<ApplicationConfiguration> mockApplicationConfiguration = new Mock<ApplicationConfiguration>();
            mockApplicationConfiguration.SetupGet(p => p.IncrementalFile).Returns(incrementalFile);
            mockApplicationConfiguration.SetupGet(p => p.IncrementalSavePath).Returns(incrementalSavePath);
            Mock<IWebClientWrapper> mockWebClientWrapper = new Mock<IWebClientWrapper>();
            mockWebClientWrapper.Setup(f => f.DownloadFile(incrementalFile, incrementalSavePath)).Throws(new WebException());

            // Execute
            ICitiDownloadService citiDownloadService = new CitiDownloadService(mockApplicationConfiguration.Object, mockWebClientWrapper.Object);
            ActualValueDelegate<object> testDelegate = () => citiDownloadService.DownloadIncrementalFile();

            // Verify
            Assert.That(testDelegate, Throws.TypeOf<WebException>());
        }

    }
}
