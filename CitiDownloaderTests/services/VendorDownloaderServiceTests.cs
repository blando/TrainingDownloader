using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using SimpleFixture;
using TrainingDownloader.wrappers;
using TrainingDownloader.services;
using TrainingDownloader.configurations;
using NUnit.Framework.Constraints;
using System.Net;

namespace TrainingDownloaderTests.services
{
    [TestFixture]
    public class VendorDownloaderServiceTests
    {
        private Fixture fixture { get; set; }
        private ApplicationConfiguration applicationConfiguration;

        public VendorDownloaderServiceTests()
        {
            this.fixture = new Fixture();
        }

        private void SetupMocks()
        {
            applicationConfiguration = fixture.Generate<ApplicationConfiguration>();
        }

        [Test]
        public void DownloadFullFileSuccessfulTest()
        {
            // Setup
            SetupMocks();
            string fullFile = applicationConfiguration.DownloadUrl;
            string fullSavePath = applicationConfiguration.SaveFilePath;
            Mock<IWebClientWrapper> mockWebClientWrapper = new Mock<IWebClientWrapper>();
            mockWebClientWrapper.Setup(f => f.DownloadFile(fullFile, fullSavePath)).Verifiable();


            // Execute
            IVendorDownloadService citiDownloadService = new VendorDownloadService(applicationConfiguration, mockWebClientWrapper.Object);
            string response = citiDownloadService.DownloadFile();


            // Verify
            Assert.That(response == fullSavePath);
            Mock.Verify(mockWebClientWrapper);
            mockWebClientWrapper.Verify(f => f.DownloadFile(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void DownloadFullFileWebExceptionTest()
        {
            // Setup
            SetupMocks();
            string fullFile = applicationConfiguration.DownloadUrl;
            string fullSavePath = applicationConfiguration.SaveFilePath;
            Mock<IWebClientWrapper> mockWebClientWrapper = new Mock<IWebClientWrapper>();
            mockWebClientWrapper.Setup(f => f.DownloadFile(fullFile, fullSavePath)).Throws(new WebException());

            // Execute
            IVendorDownloadService citiDownloadService = new VendorDownloadService(applicationConfiguration, mockWebClientWrapper.Object);
            ActualValueDelegate<object> testDelegate = () => citiDownloadService.DownloadFile();

            // Verify
            Assert.That(testDelegate, Throws.TypeOf<WebException>());
        }

        [Test]
        public void DownloadIncrementalFileSuccessfulTest()
        {
            // Setup
            SetupMocks();
            string incrementalFile = applicationConfiguration.DownloadUrl;
            string incrementalSavePath = applicationConfiguration.SaveFilePath;
            Mock<IWebClientWrapper> mockWebClientWrapper = new Mock<IWebClientWrapper>();
            mockWebClientWrapper.Setup(f => f.DownloadFile(incrementalFile, incrementalSavePath)).Verifiable();


            // Execute
            IVendorDownloadService citiDownloadService = new VendorDownloadService(applicationConfiguration, mockWebClientWrapper.Object);
            string response = citiDownloadService.DownloadFile();


            // Verify
            Assert.That(response == incrementalSavePath);
            Mock.Verify(mockWebClientWrapper);
            mockWebClientWrapper.Verify(f => f.DownloadFile(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void DownloadIncrementalFileWebExceptionTest()
        {
            // Setup
            SetupMocks();
            string incrementalFile = applicationConfiguration.DownloadUrl;
            string incrementalSavePath = applicationConfiguration.SaveFilePath;
            Mock<IWebClientWrapper> mockWebClientWrapper = new Mock<IWebClientWrapper>();
            mockWebClientWrapper.Setup(f => f.DownloadFile(incrementalFile, incrementalSavePath)).Throws(new WebException());

            // Execute
            IVendorDownloadService citiDownloadService = new VendorDownloadService(applicationConfiguration, mockWebClientWrapper.Object);
            ActualValueDelegate<object> testDelegate = () => citiDownloadService.DownloadFile();

            // Verify
            Assert.That(testDelegate, Throws.TypeOf<WebException>());
        }

    }
}
