using System;
using System.Collections.Generic;
using System.Text;
using TrainingDownloader.wrappers;
using TrainingDownloader.services;
using NUnit.Framework.Constraints;
using Moq;
using NUnit.Framework;
using SimpleFixture;
using TrainingDownloader.models;
using System.Net;
using TrainingDownloader.models.entities;
using static TrainingDownloader.services.LogService;
using TrainingDownloader.exceptions;
using System.Linq;

namespace TrainingDownloaderTests.services
{
    [TestFixture]
    public class vendorServiceTests
    {
        private Fixture fixture { get; set; }
        private Mock<IVendorDownloadService> mockVendorDownloadService { get; set; }
        private Mock<ICsvClient> mockCsvClient { get; set; }
        private Mock<ILearnerWebServices> mockLearnerWebService { get; set; }
        private Mock<ILogService> mockLogService { get; set; }
        private Mock<IReportingService> mockReportingService { get; set; }
        private Mock<ISftpClient> mockSftpClient { get; set; }

        public vendorServiceTests()
        {
            this.fixture = new Fixture();
        }

        private void SetupMocks()
        {
            mockVendorDownloadService = new Mock<IVendorDownloadService>();
            mockCsvClient = new Mock<ICsvClient>();
            mockLearnerWebService = new Mock<ILearnerWebServices>();
            mockLogService = new Mock<ILogService>();
            mockReportingService = new Mock<IReportingService>();
            mockSftpClient = new Mock<ISftpClient>();
        }

        [Test]
        public void GetRecordsSuccessfulTest()
        {
            // Setup
            SetupMocks();
            string returnFile = fixture.Generate<string>();
            List<VendorRecord> returnVendorRecords = fixture.Generate<List<VendorRecord>>();
            mockVendorDownloadService.Setup(f => f.DownloadFile()).Returns(returnFile);
            mockCsvClient.Setup(f => f.GetVendorRecords(returnFile)).Returns(returnVendorRecords);

            // Execute
            IVendorService vendorService = new VendorService(mockVendorDownloadService.Object, mockCsvClient.Object, mockLearnerWebService.Object, mockLogService.Object, mockReportingService.Object, mockSftpClient.Object);
            List<VendorRecord> response = vendorService.GetRecords();

            // Verify
            Assert.That(response == returnVendorRecords);
            mockVendorDownloadService.Verify(f => f.DownloadFile(), Times.Once);
            mockCsvClient.Verify(f => f.GetVendorRecords(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void GetRecordsWebExceptionTest()
        {
            // Setup
            SetupMocks();
            string returnFile = fixture.Generate<string>();
            List<VendorRecord> returnVendorRecords = fixture.Generate<List<VendorRecord>>();
            mockVendorDownloadService.Setup(f => f.DownloadFile()).Throws(new WebException());
            mockCsvClient.Setup(f => f.GetVendorRecords(returnFile)).Returns(returnVendorRecords);

            // Execute
            IVendorService vendorService = new VendorService(mockVendorDownloadService.Object, mockCsvClient.Object, mockLearnerWebService.Object, mockLogService.Object, mockReportingService.Object, mockSftpClient.Object);
            List<VendorRecord> response = vendorService.GetRecords();

            // Verify
            Assert.That(response == null);
            mockVendorDownloadService.Verify(f => f.DownloadFile(), Times.Once);
            mockVendorDownloadService.VerifyNoOtherCalls();
            mockCsvClient.Verify(f => f.GetVendorRecords(It.IsAny<string>()), Times.Never);
            mockCsvClient.VerifyNoOtherCalls();
            mockLogService.Verify(f => f.LogMessage(It.IsAny<string>(), It.IsAny<EventType>()), Times.Exactly(2));
            mockLogService.Verify(f => f.GetCacheAndFlush(), Times.Once);
            mockLogService.VerifyNoOtherCalls();
            mockReportingService.Verify(f => f.ReportSystemError(It.IsAny<SystemError>(), It.IsAny<List<string>>()), Times.Once);
            mockReportingService.VerifyNoOtherCalls();
        }

        [Test]
        public void GetRecordsExceptionTest()
        {
            // Setup
            SetupMocks();
            string returnFile = fixture.Generate<string>();
            List<VendorRecord> returnVendorRecords = fixture.Generate<List<VendorRecord>>();
            mockVendorDownloadService.Setup(f => f.DownloadFile()).Throws(new Exception());
            mockCsvClient.Setup(f => f.GetVendorRecords(returnFile)).Returns(returnVendorRecords);

            // Execute
            IVendorService vendorService = new VendorService(mockVendorDownloadService.Object, mockCsvClient.Object, mockLearnerWebService.Object, mockLogService.Object, mockReportingService.Object, mockSftpClient.Object);
            List<VendorRecord> response = vendorService.GetRecords();

            // Verify
            Assert.That(response == null);
            mockVendorDownloadService.Verify(f => f.DownloadFile(), Times.Once);
            mockVendorDownloadService.VerifyNoOtherCalls();
            mockCsvClient.Verify(f => f.GetVendorRecords(It.IsAny<string>()), Times.Never);
            mockCsvClient.VerifyNoOtherCalls();
            mockLogService.Verify(f => f.LogMessage(It.IsAny<string>(), It.IsAny<EventType>()), Times.Exactly(2));
            mockLogService.Verify(f => f.GetCacheAndFlush(), Times.Once);
            mockLogService.VerifyNoOtherCalls();
            mockReportingService.Verify(f => f.ReportSystemError(It.IsAny<SystemError>(), It.IsAny<List<string>>()), Times.Once);
            mockReportingService.VerifyNoOtherCalls();
        }

        [Test]
        public void InsertSingleHistoryRecordInsertedTrueTest()
        {
            // Setup
            SetupMocks();
            History fakeHistory = fixture.Generate<History>();
            int responseCurriculaId = fixture.Generate<int>();
            bool insertedResponse = true;
            mockLearnerWebService.Setup(f => f.InsertHistory(fakeHistory, out insertedResponse)).Returns(responseCurriculaId);

            // Execute
            IVendorService vendorService = new VendorService(mockVendorDownloadService.Object, mockCsvClient.Object, mockLearnerWebService.Object, mockLogService.Object, mockReportingService.Object, mockSftpClient.Object);
            vendorService.InsertSingleHistoryRecord(fakeHistory);

            // Verify
            mockLearnerWebService.Verify(f => f.InsertHistory(It.IsAny<History>(), out insertedResponse), Times.Once);
            mockLogService.Verify(f => f.LogMessage(It.IsAny<string>(), It.IsAny<EventType>()), Times.Once);
        }

        [Test]
        public void InsertSingleHistoryRecordInsertedFalseTest()
        {
            // Setup
            SetupMocks();
            History fakeHistory = fixture.Generate<History>();
            int responseCurriculaId = fixture.Generate<int>();
            bool insertedResponse = false;
            mockLearnerWebService.Setup(f => f.InsertHistory(fakeHistory, out insertedResponse)).Returns(responseCurriculaId);

            // Execute
            IVendorService vendorService = new VendorService(mockVendorDownloadService.Object, mockCsvClient.Object, mockLearnerWebService.Object, mockLogService.Object, mockReportingService.Object, mockSftpClient.Object);
            vendorService.InsertSingleHistoryRecord(fakeHistory);

            // Verify
            mockLearnerWebService.Verify(f => f.InsertHistory(It.IsAny<History>(), out insertedResponse), Times.Once);
            mockLogService.Verify(f => f.LogMessage(It.IsAny<string>(), It.IsAny<EventType>()), Times.Never);
        }

        [Test]
        public void InsertSingleHistoryRecordThrowsExceptionTest()
        {
            // Setup
            SetupMocks();
            History fakeHistory = fixture.Generate<History>();
            int responseCurriculaId = fixture.Generate<int>();
            bool insertedResponse = false;
            mockLearnerWebService.Setup(f => f.InsertHistory(fakeHistory, out insertedResponse)).Throws(new Exception());

            // Execute & Verify
            IVendorService vendorService = new VendorService(mockVendorDownloadService.Object, mockCsvClient.Object, mockLearnerWebService.Object, mockLogService.Object, mockReportingService.Object, mockSftpClient.Object);
            Assert.Throws<Exception>(delegate { vendorService.InsertSingleHistoryRecord(fakeHistory); });

            // Verify
            mockLearnerWebService.Verify(f => f.InsertHistory(It.IsAny<History>(), out insertedResponse), Times.Once);
            mockLogService.Verify(f => f.LogMessage(It.IsAny<string>(), It.IsAny<EventType>()), Times.Never);
        }

        [Test]
        public void FindUserSuccessTest()
        {
            // Setup
            SetupMocks();
            VendorRecord fakeVendorRecord = fixture.Generate<VendorRecord>();
            string fakeUnivId = fixture.Generate<string>();
            mockLearnerWebService.Setup(f => f.FindUser(fakeVendorRecord)).Returns(fakeUnivId);

            // Execute
            IVendorService vendorService = new VendorService(mockVendorDownloadService.Object, mockCsvClient.Object, mockLearnerWebService.Object, mockLogService.Object, mockReportingService.Object, mockSftpClient.Object);
            string response = vendorService.FindUser(fakeVendorRecord);

            // Verify
            Assert.That(response == fakeUnivId);
            mockLearnerWebService.Verify(f => f.FindUser(It.IsAny<VendorRecord>()), Times.Once);
            mockLearnerWebService.VerifyNoOtherCalls();
        }

        [Test]
        public void FindUserInvalidUserReturnsNullTest()
        {
            // Setup
            SetupMocks();
            VendorRecord fakeVendorRecord = fixture.Generate<VendorRecord>();
            string fakeUnivId = fixture.Generate<string>();
            mockLearnerWebService.Setup(f => f.FindUser(fakeVendorRecord)).Throws(new InvalidUserException("test"));

            // Execute
            IVendorService vendorService = new VendorService(mockVendorDownloadService.Object, mockCsvClient.Object, mockLearnerWebService.Object, mockLogService.Object, mockReportingService.Object, mockSftpClient.Object);
            string response = vendorService.FindUser(fakeVendorRecord);

            // Verify
            Assert.That(response == null);
            mockLearnerWebService.Verify(f => f.FindUser(It.IsAny<VendorRecord>()), Times.Once);
            mockLearnerWebService.VerifyNoOtherCalls();
            mockLogService.Verify(f => f.LogMessage(It.IsAny<string>(), It.IsAny<EventType>()), Times.Once);
            mockLogService.VerifyNoOtherCalls();
        }

        [Test]
        public void FindUserUnknownUserReturnsNullTest()
        {
            // Setup
            SetupMocks();
            VendorRecord fakeVendorRecord = fixture.Generate<VendorRecord>();
            string fakeUnivId = fixture.Generate<string>();
            mockLearnerWebService.Setup(f => f.FindUser(fakeVendorRecord)).Throws(new UnknownUserException("test"));

            // Execute
            IVendorService vendorService = new VendorService(mockVendorDownloadService.Object, mockCsvClient.Object, mockLearnerWebService.Object, mockLogService.Object, mockReportingService.Object, mockSftpClient.Object);
            string response = vendorService.FindUser(fakeVendorRecord);

            // Verify
            Assert.That(response == null);
            mockLearnerWebService.Verify(f => f.FindUser(It.IsAny<VendorRecord>()), Times.Once);
            mockLearnerWebService.VerifyNoOtherCalls();
            mockReportingService.Verify(f => f.ReportUnknownUser(It.IsAny<VendorRecord>(), It.IsAny<List<string>>()), Times.Once);
            mockReportingService.VerifyNoOtherCalls();
            mockLogService.Verify(f => f.LogMessage(It.IsAny<string>(), It.IsAny<EventType>()), Times.Once);
            mockLogService.Verify(f => f.GetCacheAndFlush(), Times.Once);
            mockLogService.VerifyNoOtherCalls();
        }

        [Test]
        public void FindUserExceptionReturnsNullTest()
        {
            // Setup
            SetupMocks();
            VendorRecord fakeVendorRecord = fixture.Generate<VendorRecord>();
            string fakeUnivId = fixture.Generate<string>();
            mockLearnerWebService.Setup(f => f.FindUser(fakeVendorRecord)).Throws(new Exception());

            // Execute
            IVendorService vendorService = new VendorService(mockVendorDownloadService.Object, mockCsvClient.Object, mockLearnerWebService.Object, mockLogService.Object, mockReportingService.Object, mockSftpClient.Object);
            string response = vendorService.FindUser(fakeVendorRecord);

            // Verify
            Assert.That(response == null);
            mockLearnerWebService.Verify(f => f.FindUser(It.IsAny<VendorRecord>()), Times.Once);
            mockLearnerWebService.VerifyNoOtherCalls();
            mockReportingService.Verify(f => f.ReportSystemError(It.IsAny<SystemError>(), It.IsAny<List<string>>()), Times.Once);
            mockReportingService.VerifyNoOtherCalls();
            mockLogService.Verify(f => f.LogMessage(It.IsAny<string>(), It.IsAny<EventType>()), Times.Once);
            mockLogService.Verify(f => f.GetCacheAndFlush(), Times.Once);
            mockLogService.VerifyNoOtherCalls();
        }

        [Test]
        public void InsertImportHistoryTest()
        {
            // Setup
            SetupMocks();
            VendorRecord fakeVendorRecord = fixture.Generate<VendorRecord>();
            IsuImportHistory fakeIsuImportHistory = fixture.Generate<IsuImportHistory>();
            mockLearnerWebService.Setup(f => f.InsertImportHistory(fakeVendorRecord)).Returns(fakeIsuImportHistory);

            // Execute
            IVendorService vendorService = new VendorService(mockVendorDownloadService.Object, mockCsvClient.Object, mockLearnerWebService.Object, mockLogService.Object, mockReportingService.Object, mockSftpClient.Object);
            IsuImportHistory response = vendorService.InsertImportHistory(fakeVendorRecord);

            // Verify
            Assert.That(response == fakeIsuImportHistory);
            mockLearnerWebService.Verify(f => f.InsertImportHistory(It.IsAny<VendorRecord>()), Times.Once);
            mockLearnerWebService.VerifyNoOtherCalls();
            mockLogService.VerifyNoOtherCalls();
            mockReportingService.VerifyNoOtherCalls();
        }

        [Test]
        public void InsertImportHistoryThrowsExceptionTest()
        {
            // Setup
            SetupMocks();
            VendorRecord fakeVendorRecord = fixture.Generate<VendorRecord>();
            IsuImportHistory fakeIsuImportHistory = fixture.Generate<IsuImportHistory>();
            mockLearnerWebService.Setup(f => f.InsertImportHistory(fakeVendorRecord)).Throws(new Exception());

            // Execute
            IVendorService vendorService = new VendorService(mockVendorDownloadService.Object, mockCsvClient.Object, mockLearnerWebService.Object, mockLogService.Object, mockReportingService.Object, mockSftpClient.Object);
            IsuImportHistory response = vendorService.InsertImportHistory(fakeVendorRecord);

            // Verify
            Assert.That(response == null);
            mockLearnerWebService.Verify(f => f.InsertImportHistory(It.IsAny<VendorRecord>()), Times.Once);
            mockLearnerWebService.VerifyNoOtherCalls();
            mockLogService.Verify(f => f.LogMessage(It.IsAny<string>(), It.IsAny<EventType>()), Times.Once);
            mockLogService.Verify(f => f.GetCacheAndFlush(), Times.Once);
            mockLogService.VerifyNoOtherCalls();
            mockReportingService.Verify(f => f.ReportSystemError(It.IsAny<SystemError>(), It.IsAny<List<string>>()), Times.Once);
            mockReportingService.VerifyNoOtherCalls();
        }

        [Test]
        public void IsRecordVerifiedTrueTest()
        {
            // Setup
            SetupMocks();
            VendorRecord fakeVendorRecord = fixture.Generate<VendorRecord>();
            History outHistory = fixture.Generate<History>();
            mockLearnerWebService.Setup(f => f.IsVerified(fakeVendorRecord)).Returns(true);
            mockLearnerWebService.Setup(f => f.GetHistoryByCurriculaId(fakeVendorRecord)).Returns(outHistory);

            // Execute
            IVendorService vendorService = new VendorService(mockVendorDownloadService.Object, mockCsvClient.Object, mockLearnerWebService.Object, mockLogService.Object, mockReportingService.Object, mockSftpClient.Object);
            bool response = vendorService.IsRecordVerified(fakeVendorRecord, out History responseHistory);

            // Verify
            Assert.That(response == true);
            Assert.That(responseHistory == outHistory);
            mockLearnerWebService.Verify(f => f.IsVerified(It.IsAny<VendorRecord>()), Times.Once);
            mockLearnerWebService.Verify(f => f.GetHistoryByCurriculaId(It.IsAny<VendorRecord>()), Times.Once);
            mockLearnerWebService.VerifyNoOtherCalls();
            mockLogService.Verify(f => f.LogMessage(It.IsAny<string>(), It.IsAny<EventType>()), Times.Once);
            mockLogService.Verify(f => f.GetCacheAndFlush(), Times.Never);
            mockLogService.VerifyNoOtherCalls();
            mockReportingService.Verify(f => f.ReportSystemError(It.IsAny<SystemError>(), It.IsAny<List<string>>()), Times.Never);
            mockReportingService.VerifyNoOtherCalls();

        }

        [Test]
        public void IsRecordVerifiedFalseTest()
        {
            // Setup
            SetupMocks();
            VendorRecord fakeVendorRecord = fixture.Generate<VendorRecord>();
            History outHistory = fixture.Generate<History>();
            mockLearnerWebService.Setup(f => f.IsVerified(fakeVendorRecord)).Returns(false);

            // Execute
            IVendorService vendorService = new VendorService(mockVendorDownloadService.Object, mockCsvClient.Object, mockLearnerWebService.Object, mockLogService.Object, mockReportingService.Object, mockSftpClient.Object);
            bool response = vendorService.IsRecordVerified(fakeVendorRecord, out History responseHistory);

            // Verify
            Assert.That(response == false);
            Assert.That(responseHistory == null);
            mockLearnerWebService.Verify(f => f.IsVerified(It.IsAny<VendorRecord>()), Times.Once);
            mockLearnerWebService.Verify(f => f.GetHistoryByCurriculaId(It.IsAny<VendorRecord>()), Times.Never);
            mockLearnerWebService.VerifyNoOtherCalls();
            mockLogService.Verify(f => f.LogMessage(It.IsAny<string>(), It.IsAny<EventType>()), Times.Never);
            mockLogService.Verify(f => f.GetCacheAndFlush(), Times.Never);
            mockLogService.VerifyNoOtherCalls();
            mockReportingService.Verify(f => f.ReportSystemError(It.IsAny<SystemError>(), It.IsAny<List<string>>()), Times.Never);
            mockReportingService.VerifyNoOtherCalls();

        }

        [Test]
        public void IsRecordVerifiedIsVerifiedThrowsExceptionTest()
        {
            // Setup
            SetupMocks();
            VendorRecord fakeVendorRecord = fixture.Generate<VendorRecord>();
            History outHistory = fixture.Generate<History>();
            mockLearnerWebService.Setup(f => f.IsVerified(fakeVendorRecord)).Throws(new Exception());

            // Execute
            IVendorService vendorService = new VendorService(mockVendorDownloadService.Object, mockCsvClient.Object, mockLearnerWebService.Object, mockLogService.Object, mockReportingService.Object, mockSftpClient.Object);
            bool response = vendorService.IsRecordVerified(fakeVendorRecord, out History responseHistory);

            // Verify
            Assert.That(response == false);
            Assert.That(responseHistory == null);
            mockLearnerWebService.Verify(f => f.IsVerified(It.IsAny<VendorRecord>()), Times.Once);
            mockLearnerWebService.Verify(f => f.GetHistoryByCurriculaId(It.IsAny<VendorRecord>()), Times.Never);
            mockLearnerWebService.VerifyNoOtherCalls();
            mockLogService.Verify(f => f.LogMessage(It.IsAny<string>(), It.IsAny<EventType>()), Times.Once);
            mockLogService.Verify(f => f.GetCacheAndFlush(), Times.Once);
            mockLogService.VerifyNoOtherCalls();
            mockReportingService.Verify(f => f.ReportSystemError(It.IsAny<SystemError>(), It.IsAny<List<string>>()), Times.Once);
            mockReportingService.VerifyNoOtherCalls();

        }

        [Test]
        public void IsRecordVerifiedGetHistoryThrowsExceptionTest()
        {
            // Setup
            SetupMocks();
            VendorRecord fakeVendorRecord = fixture.Generate<VendorRecord>();
            History outHistory = fixture.Generate<History>();
            mockLearnerWebService.Setup(f => f.IsVerified(fakeVendorRecord)).Returns(true);
            mockLearnerWebService.Setup(f => f.GetHistoryByCurriculaId(fakeVendorRecord)).Throws(new Exception());

            // Execute
            IVendorService vendorService = new VendorService(mockVendorDownloadService.Object, mockCsvClient.Object, mockLearnerWebService.Object, mockLogService.Object, mockReportingService.Object, mockSftpClient.Object);
            bool response = vendorService.IsRecordVerified(fakeVendorRecord, out History responseHistory);

            // Verify
            Assert.That(response == false);
            Assert.That(responseHistory == null);
            mockLearnerWebService.Verify(f => f.IsVerified(It.IsAny<VendorRecord>()), Times.Once);
            mockLearnerWebService.Verify(f => f.GetHistoryByCurriculaId(It.IsAny<VendorRecord>()), Times.Once);
            mockLearnerWebService.VerifyNoOtherCalls();
            mockLogService.Verify(f => f.LogMessage(It.IsAny<string>(), It.IsAny<EventType>()), Times.Exactly(2));
            mockLogService.Verify(f => f.GetCacheAndFlush(), Times.Once);
            mockLogService.VerifyNoOtherCalls();
            mockReportingService.Verify(f => f.ReportSystemError(It.IsAny<SystemError>(), It.IsAny<List<string>>()), Times.Once);
            mockReportingService.VerifyNoOtherCalls();

        }

        [Test]
        public void FindCourseTest()
        {
            // Setup
            SetupMocks();
            VendorRecord fakeVendorRecord = fixture.Generate<VendorRecord>();
            string fakeCourseId = fixture.Generate<string>();
            mockLearnerWebService.Setup(f => f.FindCourseId(fakeVendorRecord)).Returns(fakeCourseId);

            // Execute
            IVendorService vendorService = new VendorService(mockVendorDownloadService.Object, mockCsvClient.Object, mockLearnerWebService.Object, mockLogService.Object, mockReportingService.Object, mockSftpClient.Object);
            string response = vendorService.FindCourse(fakeVendorRecord);

            // Verify
            Assert.That(response == fakeCourseId);
            mockLearnerWebService.Verify(f => f.FindCourseId(It.IsAny<VendorRecord>()), Times.Once);
            mockLearnerWebService.VerifyNoOtherCalls();
            mockLogService.Verify(f => f.LogMessage(It.IsAny<string>(), It.IsAny<EventType>()), Times.Once);
            mockLogService.VerifyNoOtherCalls();
            mockReportingService.Verify(f => f.ReportSystemError(It.IsAny<SystemError>(), It.IsAny<List<string>>()), Times.Never);
            mockReportingService.VerifyNoOtherCalls();
        }

        [Test]
        public void FindCourseThrowsUnknownCourseExceptionTest()
        {
            // Setup
            SetupMocks();
            VendorRecord fakeVendorRecord = fixture.Generate<VendorRecord>();
            string fakeCourseId = fixture.Generate<string>();
            mockLearnerWebService.Setup(f => f.FindCourseId(fakeVendorRecord)).Throws(new UnknownCourseException("test"));

            // Execute
            IVendorService vendorService = new VendorService(mockVendorDownloadService.Object, mockCsvClient.Object, mockLearnerWebService.Object, mockLogService.Object, mockReportingService.Object, mockSftpClient.Object);
            string response = vendorService.FindCourse(fakeVendorRecord);

            // Verify
            Assert.That(response == null);
            mockLearnerWebService.Verify(f => f.FindCourseId(It.IsAny<VendorRecord>()), Times.Once);
            mockLearnerWebService.VerifyNoOtherCalls();
            mockLogService.Verify(f => f.LogMessage(It.IsAny<string>(), It.IsAny<EventType>()), Times.Once);
            mockLogService.Verify(f => f.GetCacheAndFlush(), Times.Once);
            mockLogService.VerifyNoOtherCalls();
            mockReportingService.Verify(f => f.ReportUnknownCourse(It.IsAny<VendorRecord>(), It.IsAny<List<string>>()), Times.Once);
            mockReportingService.Verify(f => f.ReportSystemError(It.IsAny<SystemError>(), It.IsAny<List<string>>()), Times.Never);
            mockReportingService.VerifyNoOtherCalls();
        }

        [Test]
        public void FindCourseThrowsExceptionTest()
        {
            // Setup
            SetupMocks();
            VendorRecord fakeVendorRecord = fixture.Generate<VendorRecord>();
            string fakeCourseId = fixture.Generate<string>();
            mockLearnerWebService.Setup(f => f.FindCourseId(fakeVendorRecord)).Throws(new Exception());

            // Execute
            IVendorService vendorService = new VendorService(mockVendorDownloadService.Object, mockCsvClient.Object, mockLearnerWebService.Object, mockLogService.Object, mockReportingService.Object, mockSftpClient.Object);
            string response = vendorService.FindCourse(fakeVendorRecord);

            // Verify
            Assert.That(response == null);
            mockLearnerWebService.Verify(f => f.FindCourseId(It.IsAny<VendorRecord>()), Times.Once);
            mockLearnerWebService.VerifyNoOtherCalls();
            mockLogService.Verify(f => f.LogMessage(It.IsAny<string>(), It.IsAny<EventType>()), Times.Once);
            mockLogService.Verify(f => f.GetCacheAndFlush(), Times.Once);
            mockLogService.VerifyNoOtherCalls();
            mockReportingService.Verify(f => f.ReportUnknownCourse(It.IsAny<VendorRecord>(), It.IsAny<List<string>>()), Times.Never);
            mockReportingService.Verify(f => f.ReportSystemError(It.IsAny<SystemError>(), It.IsAny<List<string>>()), Times.Once);
            mockReportingService.VerifyNoOtherCalls();
        }

        [Test]
        public void UploadHistoryRecordsTest()
        {
            // Setup
            SetupMocks();
            List<History> fakeHistories = fixture.Generate<List<History>>();
            string fakeFilePath = fixture.Generate<string>();
            mockCsvClient.Setup(f => f.WriteHistoryRecordsToFile(fakeHistories)).Returns(fakeFilePath);
            mockSftpClient.Setup(f => f.Upload(fakeFilePath)).Verifiable();

            // Execute
            IVendorService vendorService = new VendorService(mockVendorDownloadService.Object, mockCsvClient.Object, mockLearnerWebService.Object, mockLogService.Object, mockReportingService.Object, mockSftpClient.Object);
            vendorService.UploadHistoryRecords(fakeHistories);

            // Verify
            mockCsvClient.Verify(f => f.WriteHistoryRecordsToFile(It.IsAny<List<History>>()), Times.Once);
            mockCsvClient.VerifyNoOtherCalls();
            mockSftpClient.Verify(f => f.Upload(It.IsAny<string>()), Times.Once);
            mockSftpClient.VerifyNoOtherCalls();
            Mock.VerifyAll(mockSftpClient);
        }

        [Test]
        public void UploadHistoryRecordsWriteThrowsExceptionTest()
        {
            // Setup
            SetupMocks();
            List<History> fakeHistories = fixture.Generate<List<History>>();
            string fakeFilePath = fixture.Generate<string>();
            mockCsvClient.Setup(f => f.WriteHistoryRecordsToFile(fakeHistories)).Throws(new Exception());

            // Execute & Verify
            IVendorService vendorService = new VendorService(mockVendorDownloadService.Object, mockCsvClient.Object, mockLearnerWebService.Object, mockLogService.Object, mockReportingService.Object, mockSftpClient.Object);
            Assert.Throws<Exception>(delegate { vendorService.UploadHistoryRecords(fakeHistories); });

            // Verify
            mockCsvClient.Verify(f => f.WriteHistoryRecordsToFile(It.IsAny<List<History>>()), Times.Once);
            mockCsvClient.VerifyNoOtherCalls();
            mockSftpClient.Verify(f => f.Upload(It.IsAny<string>()), Times.Never);
            mockSftpClient.VerifyNoOtherCalls();
        }

        [Test]
        public void UploadHistoryRecordUploadThrowsExceptionTest()
        {
            // Setup
            SetupMocks();
            List<History> fakeHistories = fixture.Generate<List<History>>();
            string fakeFilePath = fixture.Generate<string>();
            mockCsvClient.Setup(f => f.WriteHistoryRecordsToFile(fakeHistories)).Returns(fakeFilePath);
            mockSftpClient.Setup(f => f.Upload(fakeFilePath)).Throws(new Exception()).Verifiable();

            // Execute & Verify
            IVendorService vendorService = new VendorService(mockVendorDownloadService.Object, mockCsvClient.Object, mockLearnerWebService.Object, mockLogService.Object, mockReportingService.Object, mockSftpClient.Object);
            Assert.Throws<Exception>(delegate { vendorService.UploadHistoryRecords(fakeHistories); });

            // Verify
            mockCsvClient.Verify(f => f.WriteHistoryRecordsToFile(It.IsAny<List<History>>()), Times.Once);
            mockCsvClient.VerifyNoOtherCalls();
            mockSftpClient.Verify(f => f.Upload(It.IsAny<string>()), Times.Once);
            mockSftpClient.VerifyNoOtherCalls();
            Mock.VerifyAll(mockSftpClient);
        }
    }
}
