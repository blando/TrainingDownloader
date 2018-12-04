using System;
using System.Collections.Generic;
using System.Text;
using CitiDownloader.wrappers;
using CitiDownloader.services;
using NUnit.Framework.Constraints;
using Moq;
using NUnit.Framework;
using SimpleFixture;
using CitiDownloader.models;
using System.Net;

namespace CitiDownloaderTests.services
{
    [TestFixture]
    public class CitiServiceTests
    {
        private Fixture fixture;
        public CitiServiceTests()
        {
            this.fixture = new Fixture();
        }

        [Test]
        public void GetFullRecordsSuccessfulTest()
        {
            // Setup
            string returnFile = fixture.Generate<string>();
            List<CitiRecord> returnCitiRecords = fixture.Generate<List<CitiRecord>>();
            Mock<ICitiDownloadService> mockCitiDownloadService = new Mock<ICitiDownloadService>();
            mockCitiDownloadService.Setup(f => f.DownloadFullFile()).Returns(returnFile);
            Mock<ICsvWrapper> mockCsvWrapper = new Mock<ICsvWrapper>();
            mockCsvWrapper.Setup(f => f.GetCitiRecords(returnFile)).Returns(returnCitiRecords);

            // Execute
            ICitiService citiService = new CitiService(mockCitiDownloadService.Object, mockCsvWrapper.Object);
            List<CitiRecord> response = citiService.GetFullRecords();

            // Verify
            Assert.That(response == returnCitiRecords);
            mockCitiDownloadService.Verify(f => f.DownloadFullFile(), Times.Once);
            mockCsvWrapper.Verify(f => f.GetCitiRecords(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void GetFullRecordsWebExceptionTest()
        {
            // Setup
            string returnFile = fixture.Generate<string>();
            List<CitiRecord> returnCitiRecords = fixture.Generate<List<CitiRecord>>();
            Mock<ICitiDownloadService> mockCitiDownloadService = new Mock<ICitiDownloadService>();
            mockCitiDownloadService.Setup(f => f.DownloadFullFile()).Throws(new WebException());
            Mock<ICsvWrapper> mockCsvWrapper = new Mock<ICsvWrapper>();
            mockCsvWrapper.Setup(f => f.GetCitiRecords(returnFile)).Returns(returnCitiRecords);

            // Execute
            ICitiService citiService = new CitiService(mockCitiDownloadService.Object, mockCsvWrapper.Object);
            ActualValueDelegate<object> testDelegate = () => citiService.GetFullRecords();

            // Verify
            Assert.That(testDelegate, Throws.TypeOf<WebException>());
        }

        [Test]
        public void GetFullRecordsExceptionTest()
        {
            // Setup
            string returnFile = fixture.Generate<string>();
            List<CitiRecord> returnCitiRecords = fixture.Generate<List<CitiRecord>>();
            Mock<ICitiDownloadService> mockCitiDownloadService = new Mock<ICitiDownloadService>();
            mockCitiDownloadService.Setup(f => f.DownloadFullFile()).Returns(returnFile);
            Mock<ICsvWrapper> mockCsvWrapper = new Mock<ICsvWrapper>();
            mockCsvWrapper.Setup(f => f.GetCitiRecords(returnFile)).Throws(new Exception());

            // Execute
            ICitiService citiService = new CitiService(mockCitiDownloadService.Object, mockCsvWrapper.Object);
            ActualValueDelegate<object> testDelegate = () => citiService.GetFullRecords();

            // Verify
            Assert.That(testDelegate, Throws.TypeOf<Exception>());
        }

        [Test]
        public void GetIncrementalRecordsSuccessfulTest()
        {
            // Setup
            string returnFile = fixture.Generate<string>();
            List<CitiRecord> returnCitiRecords = fixture.Generate<List<CitiRecord>>();
            Mock<ICitiDownloadService> mockCitiDownloadService = new Mock<ICitiDownloadService>();
            mockCitiDownloadService.Setup(f => f.DownloadIncrementalFile()).Returns(returnFile);
            Mock<ICsvWrapper> mockCsvWrapper = new Mock<ICsvWrapper>();
            mockCsvWrapper.Setup(f => f.GetCitiRecords(returnFile)).Returns(returnCitiRecords);

            // Execute
            ICitiService citiService = new CitiService(mockCitiDownloadService.Object, mockCsvWrapper.Object);
            List<CitiRecord> response = citiService.GetIncrementalRecords();

            // Verify
            Assert.That(response == returnCitiRecords);
            mockCitiDownloadService.Verify(f => f.DownloadIncrementalFile(), Times.Once);
            mockCsvWrapper.Verify(f => f.GetCitiRecords(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void GetIncrementalRecordsWebExceptionTest()
        {
            // Setup
            string returnFile = fixture.Generate<string>();
            List<CitiRecord> returnCitiRecords = fixture.Generate<List<CitiRecord>>();
            Mock<ICitiDownloadService> mockCitiDownloadService = new Mock<ICitiDownloadService>();
            mockCitiDownloadService.Setup(f => f.DownloadIncrementalFile()).Throws(new WebException());
            Mock<ICsvWrapper> mockCsvWrapper = new Mock<ICsvWrapper>();
            mockCsvWrapper.Setup(f => f.GetCitiRecords(returnFile)).Returns(returnCitiRecords);

            // Execute
            ICitiService citiService = new CitiService(mockCitiDownloadService.Object, mockCsvWrapper.Object);
            ActualValueDelegate<object> testDelegate = () => citiService.GetIncrementalRecords();

            // Verify
            Assert.That(testDelegate, Throws.TypeOf<WebException>());
        }

        [Test]
        public void GetIncrementalRecordsExceptionTest()
        {
            // Setup
            string returnFile = fixture.Generate<string>();
            List<CitiRecord> returnCitiRecords = fixture.Generate<List<CitiRecord>>();
            Mock<ICitiDownloadService> mockCitiDownloadService = new Mock<ICitiDownloadService>();
            mockCitiDownloadService.Setup(f => f.DownloadIncrementalFile()).Returns(returnFile);
            Mock<ICsvWrapper> mockCsvWrapper = new Mock<ICsvWrapper>();
            mockCsvWrapper.Setup(f => f.GetCitiRecords(returnFile)).Throws(new Exception());

            // Execute
            ICitiService citiService = new CitiService(mockCitiDownloadService.Object, mockCsvWrapper.Object);
            ActualValueDelegate<object> testDelegate = () => citiService.GetIncrementalRecords();

            // Verify
            Assert.That(testDelegate, Throws.TypeOf<Exception>());
        }
    }
}
