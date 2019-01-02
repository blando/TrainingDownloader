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
using CitiDownloader.models.entities;
using static CitiDownloader.services.LogService;
using CitiDownloader.exceptions;

namespace CitiDownloaderTests.services
{
    [TestFixture]
    public class CitiServiceTests
    {
        private Fixture fixture;
        private Mock<ICitiDownloadService> mockCitiDownloadService;
        private Mock<ICsvClient> mockCsvClient;
        private Mock<ILearnerWebServices> mockLearnerWebService;
        private Mock<ILogService> mockLogService;
        private Mock<IReportingService> mockReportingService;
        private Mock<ISftpClient> mockSftpClient;

        public CitiServiceTests()
        {
            this.fixture = new Fixture();
        }

        private void SetupMocks()
        {
            mockCitiDownloadService = new Mock<ICitiDownloadService>();
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
            List<CitiRecord> returnCitiRecords = fixture.Generate<List<CitiRecord>>();
            mockCitiDownloadService.Setup(f => f.DownloadFile()).Returns(returnFile);
            mockCsvClient.Setup(f => f.GetCitiRecords(returnFile)).Returns(returnCitiRecords);

            // Execute
            ICitiService citiService = new CitiService(mockCitiDownloadService.Object, mockCsvClient.Object, mockLearnerWebService.Object, mockLogService.Object, mockReportingService.Object, mockSftpClient.Object);
            List<CitiRecord> response = citiService.GetRecords();

            // Verify
            Assert.That(response == returnCitiRecords);
            mockCitiDownloadService.Verify(f => f.DownloadFile(), Times.Once);
            mockCsvClient.Verify(f => f.GetCitiRecords(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void GetRecordsWebExceptionTest()
        {
            // Setup
            SetupMocks();
            string returnFile = fixture.Generate<string>();
            List<CitiRecord> returnCitiRecords = fixture.Generate<List<CitiRecord>>();
            mockCitiDownloadService.Setup(f => f.DownloadFile()).Throws(new WebException());
            mockCsvClient.Setup(f => f.GetCitiRecords(returnFile)).Returns(returnCitiRecords);

            // Execute
            ICitiService citiService = new CitiService(mockCitiDownloadService.Object, mockCsvClient.Object, mockLearnerWebService.Object, mockLogService.Object, mockReportingService.Object, mockSftpClient.Object);
            List<CitiRecord> response = citiService.GetRecords();

            // Verify
            Assert.That(response == null);
            mockCitiDownloadService.Verify(f => f.DownloadFile(), Times.Once);
            mockCitiDownloadService.VerifyNoOtherCalls();
            mockCsvClient.Verify(f => f.GetCitiRecords(It.IsAny<string>()), Times.Never);
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
            List<CitiRecord> returnCitiRecords = fixture.Generate<List<CitiRecord>>();
            mockCitiDownloadService.Setup(f => f.DownloadFile()).Throws(new Exception());
            mockCsvClient.Setup(f => f.GetCitiRecords(returnFile)).Returns(returnCitiRecords);

            // Execute
            ICitiService citiService = new CitiService(mockCitiDownloadService.Object, mockCsvClient.Object, mockLearnerWebService.Object, mockLogService.Object, mockReportingService.Object, mockSftpClient.Object);
            List<CitiRecord> response = citiService.GetRecords();

            // Verify
            Assert.That(response == null);
            mockCitiDownloadService.Verify(f => f.DownloadFile(), Times.Once);
            mockCitiDownloadService.VerifyNoOtherCalls();
            mockCsvClient.Verify(f => f.GetCitiRecords(It.IsAny<string>()), Times.Never);
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
            ICitiService citiService = new CitiService(mockCitiDownloadService.Object, mockCsvClient.Object, mockLearnerWebService.Object, mockLogService.Object, mockReportingService.Object, mockSftpClient.Object);
            citiService.InsertSingleHistoryRecord(fakeHistory);

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
            ICitiService citiService = new CitiService(mockCitiDownloadService.Object, mockCsvClient.Object, mockLearnerWebService.Object, mockLogService.Object, mockReportingService.Object, mockSftpClient.Object);
            citiService.InsertSingleHistoryRecord(fakeHistory);

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
            ICitiService citiService = new CitiService(mockCitiDownloadService.Object, mockCsvClient.Object, mockLearnerWebService.Object, mockLogService.Object, mockReportingService.Object, mockSftpClient.Object);
            Assert.Throws<Exception>(delegate { citiService.InsertSingleHistoryRecord(fakeHistory); });

            // Verify
            mockLearnerWebService.Verify(f => f.InsertHistory(It.IsAny<History>(), out insertedResponse), Times.Once);
            mockLogService.Verify(f => f.LogMessage(It.IsAny<string>(), It.IsAny<EventType>()), Times.Never);
        }

        [Test]
        public void FindUserSuccessTest()
        {
            // Setup
            SetupMocks();
            CitiRecord fakeCitiRecord = fixture.Generate<CitiRecord>();
            string fakeUnivId = fixture.Generate<string>();
            mockLearnerWebService.Setup(f => f.FindUser(fakeCitiRecord)).Returns(fakeUnivId);

            // Execute
            ICitiService citiService = new CitiService(mockCitiDownloadService.Object, mockCsvClient.Object, mockLearnerWebService.Object, mockLogService.Object, mockReportingService.Object, mockSftpClient.Object);
            string response = citiService.FindUser(fakeCitiRecord);

            // Verify
            Assert.That(response == fakeUnivId);
            mockLearnerWebService.Verify(f => f.FindUser(It.IsAny<CitiRecord>()), Times.Once);
            mockLearnerWebService.VerifyNoOtherCalls();
        }

        [Test]
        public void FindUserInvalidUserReturnsNullTest()
        {
            // Setup
            SetupMocks();
            CitiRecord fakeCitiRecord = fixture.Generate<CitiRecord>();
            string fakeUnivId = fixture.Generate<string>();
            mockLearnerWebService.Setup(f => f.FindUser(fakeCitiRecord)).Throws(new InvalidUserException("test"));

            // Execute
            ICitiService citiService = new CitiService(mockCitiDownloadService.Object, mockCsvClient.Object, mockLearnerWebService.Object, mockLogService.Object, mockReportingService.Object, mockSftpClient.Object);
            string response = citiService.FindUser(fakeCitiRecord);

            // Verify
            Assert.That(response == null);
            mockLearnerWebService.Verify(f => f.FindUser(It.IsAny<CitiRecord>()), Times.Once);
            mockLearnerWebService.VerifyNoOtherCalls();
            mockLogService.Verify(f => f.LogMessage(It.IsAny<string>(), It.IsAny<EventType>()), Times.Once);
            mockLogService.VerifyNoOtherCalls();
        }

        [Test]
        public void FindUserUnknownUserReturnsNullTest()
        {
            // Setup
            SetupMocks();
            CitiRecord fakeCitiRecord = fixture.Generate<CitiRecord>();
            string fakeUnivId = fixture.Generate<string>();
            mockLearnerWebService.Setup(f => f.FindUser(fakeCitiRecord)).Throws(new UnknownUserException("test"));

            // Execute
            ICitiService citiService = new CitiService(mockCitiDownloadService.Object, mockCsvClient.Object, mockLearnerWebService.Object, mockLogService.Object, mockReportingService.Object, mockSftpClient.Object);
            string response = citiService.FindUser(fakeCitiRecord);

            // Verify
            Assert.That(response == null);
            mockLearnerWebService.Verify(f => f.FindUser(It.IsAny<CitiRecord>()), Times.Once);
            mockLearnerWebService.VerifyNoOtherCalls();
            mockReportingService.Verify(f => f.ReportUnknownUser(It.IsAny<CitiRecord>(), It.IsAny<List<string>>()), Times.Once);
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
            CitiRecord fakeCitiRecord = fixture.Generate<CitiRecord>();
            string fakeUnivId = fixture.Generate<string>();
            mockLearnerWebService.Setup(f => f.FindUser(fakeCitiRecord)).Throws(new Exception());

            // Execute
            ICitiService citiService = new CitiService(mockCitiDownloadService.Object, mockCsvClient.Object, mockLearnerWebService.Object, mockLogService.Object, mockReportingService.Object, mockSftpClient.Object);
            string response = citiService.FindUser(fakeCitiRecord);

            // Verify
            Assert.That(response == null);
            mockLearnerWebService.Verify(f => f.FindUser(It.IsAny<CitiRecord>()), Times.Once);
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
            CitiRecord fakeCitiRecord = fixture.Generate<CitiRecord>();
            IsuImportHistory fakeIsuImportHistory = fixture.Generate<IsuImportHistory>();
            mockLearnerWebService.Setup(f => f.InsertImportHistory(fakeCitiRecord)).Returns(fakeIsuImportHistory);

            // Execute
            ICitiService citiService = new CitiService(mockCitiDownloadService.Object, mockCsvClient.Object, mockLearnerWebService.Object, mockLogService.Object, mockReportingService.Object, mockSftpClient.Object);
            IsuImportHistory response = citiService.InsertImportHistory(fakeCitiRecord);

            // Verify
            Assert.That(response == fakeIsuImportHistory);
            mockLearnerWebService.Verify(f => f.InsertImportHistory(It.IsAny<CitiRecord>()), Times.Once);
            mockLearnerWebService.VerifyNoOtherCalls();
            mockLogService.VerifyNoOtherCalls();
            mockReportingService.VerifyNoOtherCalls();
        }

        [Test]
        public void InsertImportHistoryThrowsExceptionTest()
        {
            // Setup
            SetupMocks();
            CitiRecord fakeCitiRecord = fixture.Generate<CitiRecord>();
            IsuImportHistory fakeIsuImportHistory = fixture.Generate<IsuImportHistory>();
            mockLearnerWebService.Setup(f => f.InsertImportHistory(fakeCitiRecord)).Throws(new Exception());

            // Execute
            ICitiService citiService = new CitiService(mockCitiDownloadService.Object, mockCsvClient.Object, mockLearnerWebService.Object, mockLogService.Object, mockReportingService.Object, mockSftpClient.Object);
            IsuImportHistory response = citiService.InsertImportHistory(fakeCitiRecord);

            // Verify
            Assert.That(response == null);
            mockLearnerWebService.Verify(f => f.InsertImportHistory(It.IsAny<CitiRecord>()), Times.Once);
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
            CitiRecord fakeCitiRecord = fixture.Generate<CitiRecord>();
            History outHistory = fixture.Generate<History>();
            mockLearnerWebService.Setup(f => f.IsVerified(fakeCitiRecord)).Returns(true);
            mockLearnerWebService.Setup(f => f.GetHistoryByCurriculaId(fakeCitiRecord)).Returns(outHistory);

            // Execute
            ICitiService citiService = new CitiService(mockCitiDownloadService.Object, mockCsvClient.Object, mockLearnerWebService.Object, mockLogService.Object, mockReportingService.Object, mockSftpClient.Object);
            bool response = citiService.IsRecordVerified(fakeCitiRecord, out History responseHistory);

            // Verify
            Assert.That(response == true);
            Assert.That(responseHistory == outHistory);
            mockLearnerWebService.Verify(f => f.IsVerified(It.IsAny<CitiRecord>()), Times.Once);
            mockLearnerWebService.Verify(f => f.GetHistoryByCurriculaId(It.IsAny<CitiRecord>()), Times.Once);
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
            CitiRecord fakeCitiRecord = fixture.Generate<CitiRecord>();
            History outHistory = fixture.Generate<History>();
            mockLearnerWebService.Setup(f => f.IsVerified(fakeCitiRecord)).Returns(false);

            // Execute
            ICitiService citiService = new CitiService(mockCitiDownloadService.Object, mockCsvClient.Object, mockLearnerWebService.Object, mockLogService.Object, mockReportingService.Object, mockSftpClient.Object);
            bool response = citiService.IsRecordVerified(fakeCitiRecord, out History responseHistory);

            // Verify
            Assert.That(response == false);
            Assert.That(responseHistory == null);
            mockLearnerWebService.Verify(f => f.IsVerified(It.IsAny<CitiRecord>()), Times.Once);
            mockLearnerWebService.Verify(f => f.GetHistoryByCurriculaId(It.IsAny<CitiRecord>()), Times.Never);
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
            CitiRecord fakeCitiRecord = fixture.Generate<CitiRecord>();
            History outHistory = fixture.Generate<History>();
            mockLearnerWebService.Setup(f => f.IsVerified(fakeCitiRecord)).Throws(new Exception());

            // Execute
            ICitiService citiService = new CitiService(mockCitiDownloadService.Object, mockCsvClient.Object, mockLearnerWebService.Object, mockLogService.Object, mockReportingService.Object, mockSftpClient.Object);
            bool response = citiService.IsRecordVerified(fakeCitiRecord, out History responseHistory);

            // Verify
            Assert.That(response == false);
            Assert.That(responseHistory == null);
            mockLearnerWebService.Verify(f => f.IsVerified(It.IsAny<CitiRecord>()), Times.Once);
            mockLearnerWebService.Verify(f => f.GetHistoryByCurriculaId(It.IsAny<CitiRecord>()), Times.Never);
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
            CitiRecord fakeCitiRecord = fixture.Generate<CitiRecord>();
            History outHistory = fixture.Generate<History>();
            mockLearnerWebService.Setup(f => f.IsVerified(fakeCitiRecord)).Returns(true);
            mockLearnerWebService.Setup(f => f.GetHistoryByCurriculaId(fakeCitiRecord)).Throws(new Exception());

            // Execute
            ICitiService citiService = new CitiService(mockCitiDownloadService.Object, mockCsvClient.Object, mockLearnerWebService.Object, mockLogService.Object, mockReportingService.Object, mockSftpClient.Object);
            bool response = citiService.IsRecordVerified(fakeCitiRecord, out History responseHistory);

            // Verify
            Assert.That(response == false);
            Assert.That(responseHistory == null);
            mockLearnerWebService.Verify(f => f.IsVerified(It.IsAny<CitiRecord>()), Times.Once);
            mockLearnerWebService.Verify(f => f.GetHistoryByCurriculaId(It.IsAny<CitiRecord>()), Times.Once);
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
            CitiRecord fakeCitiRecord = fixture.Generate<CitiRecord>();
            string fakeCourseId = fixture.Generate<string>();
            mockLearnerWebService.Setup(f => f.FindCourseId(fakeCitiRecord)).Returns(fakeCourseId);

            // Execute
            ICitiService citiService = new CitiService(mockCitiDownloadService.Object, mockCsvClient.Object, mockLearnerWebService.Object, mockLogService.Object, mockReportingService.Object, mockSftpClient.Object);
            string response = citiService.FindCourse(fakeCitiRecord);

            // Verify
            Assert.That(response == fakeCourseId);
            mockLearnerWebService.Verify(f => f.FindCourseId(It.IsAny<CitiRecord>()), Times.Once);
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
            CitiRecord fakeCitiRecord = fixture.Generate<CitiRecord>();
            string fakeCourseId = fixture.Generate<string>();
            mockLearnerWebService.Setup(f => f.FindCourseId(fakeCitiRecord)).Throws(new UnknownCourseException("test"));

            // Execute
            ICitiService citiService = new CitiService(mockCitiDownloadService.Object, mockCsvClient.Object, mockLearnerWebService.Object, mockLogService.Object, mockReportingService.Object, mockSftpClient.Object);
            string response = citiService.FindCourse(fakeCitiRecord);

            // Verify
            Assert.That(response == null);
            mockLearnerWebService.Verify(f => f.FindCourseId(It.IsAny<CitiRecord>()), Times.Once);
            mockLearnerWebService.VerifyNoOtherCalls();
            mockLogService.Verify(f => f.LogMessage(It.IsAny<string>(), It.IsAny<EventType>()), Times.Once);
            mockLogService.Verify(f => f.GetCacheAndFlush(), Times.Once);
            mockLogService.VerifyNoOtherCalls();
            mockReportingService.Verify(f => f.ReportUnknownCourse(It.IsAny<CitiRecord>(), It.IsAny<List<string>>()), Times.Once);
            mockReportingService.Verify(f => f.ReportSystemError(It.IsAny<SystemError>(), It.IsAny<List<string>>()), Times.Never);
            mockReportingService.VerifyNoOtherCalls();
        }

        [Test]
        public void FindCourseThrowsExceptionTest()
        {
            // Setup
            SetupMocks();
            CitiRecord fakeCitiRecord = fixture.Generate<CitiRecord>();
            string fakeCourseId = fixture.Generate<string>();
            mockLearnerWebService.Setup(f => f.FindCourseId(fakeCitiRecord)).Throws(new Exception());

            // Execute
            ICitiService citiService = new CitiService(mockCitiDownloadService.Object, mockCsvClient.Object, mockLearnerWebService.Object, mockLogService.Object, mockReportingService.Object, mockSftpClient.Object);
            string response = citiService.FindCourse(fakeCitiRecord);

            // Verify
            Assert.That(response == null);
            mockLearnerWebService.Verify(f => f.FindCourseId(It.IsAny<CitiRecord>()), Times.Once);
            mockLearnerWebService.VerifyNoOtherCalls();
            mockLogService.Verify(f => f.LogMessage(It.IsAny<string>(), It.IsAny<EventType>()), Times.Once);
            mockLogService.Verify(f => f.GetCacheAndFlush(), Times.Once);
            mockLogService.VerifyNoOtherCalls();
            mockReportingService.Verify(f => f.ReportUnknownCourse(It.IsAny<CitiRecord>(), It.IsAny<List<string>>()), Times.Never);
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
            ICitiService citiService = new CitiService(mockCitiDownloadService.Object, mockCsvClient.Object, mockLearnerWebService.Object, mockLogService.Object, mockReportingService.Object, mockSftpClient.Object);
            citiService.UploadHistoryRecords(fakeHistories);

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
            ICitiService citiService = new CitiService(mockCitiDownloadService.Object, mockCsvClient.Object, mockLearnerWebService.Object, mockLogService.Object, mockReportingService.Object, mockSftpClient.Object);
            Assert.Throws<Exception>(delegate { citiService.UploadHistoryRecords(fakeHistories); });

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
            ICitiService citiService = new CitiService(mockCitiDownloadService.Object, mockCsvClient.Object, mockLearnerWebService.Object, mockLogService.Object, mockReportingService.Object, mockSftpClient.Object);
            Assert.Throws<Exception>(delegate { citiService.UploadHistoryRecords(fakeHistories); });

            // Verify
            mockCsvClient.Verify(f => f.WriteHistoryRecordsToFile(It.IsAny<List<History>>()), Times.Once);
            mockCsvClient.VerifyNoOtherCalls();
            mockSftpClient.Verify(f => f.Upload(It.IsAny<string>()), Times.Once);
            mockSftpClient.VerifyNoOtherCalls();
            Mock.VerifyAll(mockSftpClient);
        }
    }
}
