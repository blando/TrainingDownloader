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
using CitiDownloader.configurations;
using CitiDownloader.models.entities;
using CitiDownloader.exceptions;

namespace CitiDownloaderTests.services
{
    [TestFixture]
    public class TrainingServiceTests
    {
        private Mock<ICitiService> mockCitiService;
        private Mock<ILearnerWebServices> mockLearnerWebServices;
        private Mock<ILogService> mockLogService;
        private Mock<IReportingService> mockReportingService;
        private Mock<ApplicationConfiguration> mockApplicationConfiguration;
        private Mock<IMailService> mockMailService;
        private Fixture fixture;
        private string appConfigTestFile = "test-settings.json";

        public TrainingServiceTests()
        {
            fixture = new Fixture();
        }

        private void SetupMocks()
        {
            mockCitiService = new Mock<ICitiService>();
            mockLearnerWebServices = new Mock<ILearnerWebServices>();
            mockLogService = new Mock<ILogService>();
            mockMailService = new Mock<IMailService>();
            mockReportingService = new Mock<IReportingService>();
            object[] args = new object[] { new string[] { "full", "upload", appConfigTestFile } };
            mockApplicationConfiguration = new Mock<ApplicationConfiguration>(MockBehavior.Loose, args);
        }


        [Test]
        public void ProcessRecordsNullTest()
        {
            // Setup
            SetupMocks();
            List<CitiRecord> fakeCitiRecords = null;
            mockCitiService.Setup(f => f.GetRecords()).Returns(fakeCitiRecords);

            // Execute
            ITrainingService trainingService = new TrainingService(mockCitiService.Object, mockLearnerWebServices.Object, mockLogService.Object, mockReportingService.Object, mockApplicationConfiguration.Object, mockMailService.Object);
            trainingService.ProcessRecords();

            // Verify
            mockCitiService.Verify(f => f.GetRecords(), Times.Once);
            mockCitiService.VerifyNoOtherCalls();
            mockLearnerWebServices.VerifyNoOtherCalls();
            mockMailService.Verify(f => f.SendMessages(), Times.Once);
            mockMailService.VerifyNoOtherCalls();
        }

        [Test]
        public void ProcessRecordsZeroTest()
        {
            // Setup
            SetupMocks();
            List<CitiRecord> fakeCitiRecords = new List<CitiRecord>();
            mockCitiService.Setup(f => f.GetRecords()).Returns(fakeCitiRecords);

            // Execute
            ITrainingService trainingService = new TrainingService(mockCitiService.Object, mockLearnerWebServices.Object, mockLogService.Object, mockReportingService.Object, mockApplicationConfiguration.Object, mockMailService.Object);
            trainingService.ProcessRecords();

            // Verify
            mockCitiService.Verify(f => f.GetRecords(), Times.Once);
            mockCitiService.VerifyNoOtherCalls();
            mockLearnerWebServices.VerifyNoOtherCalls();
            mockMailService.Verify(f => f.SendMessages(), Times.Once);
            mockMailService.VerifyNoOtherCalls();
        }

        [Test]
        public void ProcessRecordsImportHistoryZeroIdTest()
        {
            // Setup
            SetupMocks();
            List<CitiRecord> fakeCitiRecords = new List<CitiRecord>();
            CitiRecord fakeCitiRecord = fixture.Generate<CitiRecord>();
            fakeCitiRecords.Add(fakeCitiRecord);
            mockCitiService.Setup(f => f.GetRecords()).Returns(fakeCitiRecords);

            IsuImportHistory fakeIsuImportHisotry = fixture.Generate<IsuImportHistory>();
            fakeIsuImportHisotry.Id = 0;
            mockCitiService.Setup(f => f.InsertImportHistory(fakeCitiRecord)).Returns(fakeIsuImportHisotry);

            // Execute
            ITrainingService trainingService = new TrainingService(mockCitiService.Object, mockLearnerWebServices.Object, mockLogService.Object, mockReportingService.Object, mockApplicationConfiguration.Object, mockMailService.Object);
            trainingService.ProcessRecords();

            // Verify
            mockCitiService.Verify(f => f.GetRecords(), Times.Once);
            mockCitiService.Verify(f => f.InsertImportHistory(It.IsAny<CitiRecord>()), Times.Once);
            mockCitiService.VerifyNoOtherCalls();
            mockLearnerWebServices.VerifyNoOtherCalls();
            mockMailService.Verify(f => f.SendMessages(), Times.Once);
            mockMailService.VerifyNoOtherCalls();

        }

        [Test]
        public void ProcessRecordsGetByCurriculaIdReturnsHistoryUploadTest()
        {
            // Setup
            SetupMocks();
            Random random = new Random();
            List<CitiRecord> fakeCitiRecords = new List<CitiRecord>();
            CitiRecord fakeCitiRecord = fixture.Generate<CitiRecord>();
            fakeCitiRecords.Add(fakeCitiRecord);
            mockCitiService.Setup(f => f.GetRecords()).Returns(fakeCitiRecords);

            IsuImportHistory fakeIsuImportHisotry = fixture.Generate<IsuImportHistory>();
            fakeIsuImportHisotry.Id = random.Next(1,10);
            mockCitiService.Setup(f => f.InsertImportHistory(fakeCitiRecord)).Returns(fakeIsuImportHisotry);

            History fakeHistory = fixture.Generate<History>();
            mockLearnerWebServices.Setup(f => f.GetHistoryByCurriculaId(fakeCitiRecord)).Returns(fakeHistory);
            mockApplicationConfiguration.SetupGet(p => p.processType).Returns(ApplicationConfiguration.ProcessType.Upload);

            // Execute
            ITrainingService trainingService = new TrainingService(mockCitiService.Object, mockLearnerWebServices.Object, mockLogService.Object, mockReportingService.Object, mockApplicationConfiguration.Object, mockMailService.Object);
            trainingService.ProcessRecords();

            // Verify
            mockCitiService.Verify(f => f.GetRecords(), Times.Once);
            mockCitiService.Verify(f => f.UploadHistoryRecords(It.IsAny<List<History>>()), Times.Once);
            mockCitiService.Verify(f => f.InsertImportHistory(It.IsAny<CitiRecord>()), Times.Once);
            mockCitiService.VerifyNoOtherCalls();
            mockLearnerWebServices.Verify(f => f.GetHistoryByCurriculaId(It.IsAny<CitiRecord>()), Times.Once);
            mockLearnerWebServices.VerifyNoOtherCalls();
            mockMailService.Verify(f => f.SendMessages(), Times.Once);
            mockMailService.VerifyNoOtherCalls();
        }

        [Test]
        public void ProcessRecordsGetByCurriculaIdReturnsHistoryInsertTest()
        {
            // Setup
            SetupMocks();
            Random random = new Random();
            List<CitiRecord> fakeCitiRecords = new List<CitiRecord>();
            CitiRecord fakeCitiRecord = fixture.Generate<CitiRecord>();
            fakeCitiRecords.Add(fakeCitiRecord);
            mockCitiService.Setup(f => f.GetRecords()).Returns(fakeCitiRecords);

            IsuImportHistory fakeIsuImportHisotry = fixture.Generate<IsuImportHistory>();
            fakeIsuImportHisotry.Id = random.Next(1, 10);
            mockCitiService.Setup(f => f.InsertImportHistory(fakeCitiRecord)).Returns(fakeIsuImportHisotry);

            History fakeHistory = fixture.Generate<History>();
            mockLearnerWebServices.Setup(f => f.GetHistoryByCurriculaId(fakeCitiRecord)).Returns(fakeHistory);
            mockApplicationConfiguration.SetupGet(p => p.processType).Returns(ApplicationConfiguration.ProcessType.Insert);

            // Execute
            ITrainingService trainingService = new TrainingService(mockCitiService.Object, mockLearnerWebServices.Object, mockLogService.Object, mockReportingService.Object, mockApplicationConfiguration.Object, mockMailService.Object);
            trainingService.ProcessRecords();

            // Verify
            mockCitiService.Verify(f => f.GetRecords(), Times.Once);
            mockCitiService.Verify(f => f.UploadHistoryRecords(It.IsAny<List<History>>()), Times.Never);
            mockCitiService.Verify(f => f.InsertImportHistory(It.IsAny<CitiRecord>()), Times.Once);
            mockCitiService.VerifyNoOtherCalls();
            mockLearnerWebServices.Verify(f => f.GetHistoryByCurriculaId(It.IsAny<CitiRecord>()), Times.Once);
            mockLearnerWebServices.VerifyNoOtherCalls();
            mockMailService.Verify(f => f.SendMessages(), Times.Once);
            mockMailService.VerifyNoOtherCalls();
        }

        [Test]
        public void ProcessRecordsIsVerifiedUploadTest()
        {
            // Setup
            SetupMocks();
            Random random = new Random();
            List<CitiRecord> fakeCitiRecords = new List<CitiRecord>();
            CitiRecord fakeCitiRecord = fixture.Generate<CitiRecord>();
            fakeCitiRecords.Add(fakeCitiRecord);
            mockCitiService.Setup(f => f.GetRecords()).Returns(fakeCitiRecords);

            IsuImportHistory fakeIsuImportHisotry = fixture.Generate<IsuImportHistory>();
            fakeIsuImportHisotry.Id = random.Next(1, 10);
            mockCitiService.Setup(f => f.InsertImportHistory(fakeCitiRecord)).Returns(fakeIsuImportHisotry);

            History fakeHistory1 = null;
            mockLearnerWebServices.Setup(f => f.GetHistoryByCurriculaId(fakeCitiRecord)).Returns(fakeHistory1);

            History fakeHistory2 = fixture.Generate<History>();
            mockCitiService.Setup(f => f.IsRecordVerified(fakeCitiRecord, out fakeHistory2)).Returns(true);

            mockApplicationConfiguration.SetupGet(p => p.processType).Returns(ApplicationConfiguration.ProcessType.Upload);

            // Execute
            ITrainingService trainingService = new TrainingService(mockCitiService.Object, mockLearnerWebServices.Object, mockLogService.Object, mockReportingService.Object, mockApplicationConfiguration.Object, mockMailService.Object);
            trainingService.ProcessRecords();

            // Verify
            mockCitiService.Verify(f => f.GetRecords(), Times.Once);
            mockCitiService.Verify(f => f.UploadHistoryRecords(It.IsAny<List<History>>()), Times.Once);
            mockCitiService.Verify(f => f.InsertImportHistory(It.IsAny<CitiRecord>()), Times.Once);
            mockCitiService.Verify(f => f.IsRecordVerified(It.IsAny<CitiRecord>(), out fakeHistory2), Times.Once);
            mockCitiService.VerifyNoOtherCalls();
            mockLearnerWebServices.Verify(f => f.GetHistoryByCurriculaId(It.IsAny<CitiRecord>()), Times.Once);
            mockLearnerWebServices.VerifyNoOtherCalls();
            mockMailService.Verify(f => f.SendMessages(), Times.Once);
            mockMailService.VerifyNoOtherCalls();
        }

        [Test]
        public void ProcessRecordsIsVerifiedInsertTest()
        {
            // Setup
            SetupMocks();
            Random random = new Random();
            List<CitiRecord> fakeCitiRecords = new List<CitiRecord>();
            CitiRecord fakeCitiRecord = fixture.Generate<CitiRecord>();
            fakeCitiRecords.Add(fakeCitiRecord);
            mockCitiService.Setup(f => f.GetRecords()).Returns(fakeCitiRecords);

            IsuImportHistory fakeIsuImportHisotry = fixture.Generate<IsuImportHistory>();
            fakeIsuImportHisotry.Id = random.Next(1, 10);
            mockCitiService.Setup(f => f.InsertImportHistory(fakeCitiRecord)).Returns(fakeIsuImportHisotry);

            History fakeHistory1 = null;
            mockLearnerWebServices.Setup(f => f.GetHistoryByCurriculaId(fakeCitiRecord)).Returns(fakeHistory1);

            History fakeHistory2 = fixture.Generate<History>();
            mockCitiService.Setup(f => f.IsRecordVerified(fakeCitiRecord, out fakeHistory2)).Returns(true);

            mockApplicationConfiguration.SetupGet(p => p.processType).Returns(ApplicationConfiguration.ProcessType.Insert);

            // Execute
            ITrainingService trainingService = new TrainingService(mockCitiService.Object, mockLearnerWebServices.Object, mockLogService.Object, mockReportingService.Object, mockApplicationConfiguration.Object, mockMailService.Object);
            trainingService.ProcessRecords();

            // Verify
            mockCitiService.Verify(f => f.GetRecords(), Times.Once);
            mockCitiService.Verify(f => f.UploadHistoryRecords(It.IsAny<List<History>>()), Times.Never);
            mockCitiService.Verify(f => f.InsertImportHistory(It.IsAny<CitiRecord>()), Times.Once);
            mockCitiService.Verify(f => f.IsRecordVerified(It.IsAny<CitiRecord>(), out fakeHistory2), Times.Once);
            mockCitiService.VerifyNoOtherCalls();
            mockLearnerWebServices.Verify(f => f.GetHistoryByCurriculaId(It.IsAny<CitiRecord>()), Times.Once);
            mockLearnerWebServices.VerifyNoOtherCalls();
            mockMailService.Verify(f => f.SendMessages(), Times.Once);
            mockMailService.VerifyNoOtherCalls();
        }

        [Test]
        public void ProcessRecordsFindUserNullTest()
        {
            // Setup
            SetupMocks();
            Random random = new Random();
            List<CitiRecord> fakeCitiRecords = new List<CitiRecord>();
            CitiRecord fakeCitiRecord = fixture.Generate<CitiRecord>();
            fakeCitiRecords.Add(fakeCitiRecord);
            mockCitiService.Setup(f => f.GetRecords()).Returns(fakeCitiRecords);

            IsuImportHistory fakeIsuImportHisotry = fixture.Generate<IsuImportHistory>();
            fakeIsuImportHisotry.Id = random.Next(1, 10);
            mockCitiService.Setup(f => f.InsertImportHistory(fakeCitiRecord)).Returns(fakeIsuImportHisotry);

            History fakeHistory1 = null;
            mockLearnerWebServices.Setup(f => f.GetHistoryByCurriculaId(fakeCitiRecord)).Returns(fakeHistory1);

            History fakeHistory2 = null;
            mockCitiService.Setup(f => f.IsRecordVerified(fakeCitiRecord, out fakeHistory2)).Returns(false);

            string fakeUnivId = null;
            mockCitiService.Setup(f => f.FindUser(fakeCitiRecord)).Returns(fakeUnivId);

            //mockApplicationConfiguration.SetupGet(p => p.processType).Returns(ApplicationConfiguration.ProcessType.Insert);

            // Execute
            ITrainingService trainingService = new TrainingService(mockCitiService.Object, mockLearnerWebServices.Object, mockLogService.Object, mockReportingService.Object, mockApplicationConfiguration.Object, mockMailService.Object);
            trainingService.ProcessRecords();

            // Verify
            mockCitiService.Verify(f => f.GetRecords(), Times.Once);
            mockCitiService.Verify(f => f.UploadHistoryRecords(It.IsAny<List<History>>()), Times.Never);
            mockCitiService.Verify(f => f.InsertImportHistory(It.IsAny<CitiRecord>()), Times.Once);
            mockCitiService.Verify(f => f.IsRecordVerified(It.IsAny<CitiRecord>(), out fakeHistory2), Times.Once);
            mockCitiService.Verify(f => f.FindUser(It.IsAny<CitiRecord>()), Times.Once);
            mockCitiService.VerifyNoOtherCalls();
            mockLearnerWebServices.Verify(f => f.GetHistoryByCurriculaId(It.IsAny<CitiRecord>()), Times.Once);
            mockLearnerWebServices.VerifyNoOtherCalls();
            mockMailService.Verify(f => f.SendMessages(), Times.Once);
            mockMailService.VerifyNoOtherCalls();
        }

        [Test]
        public void ProcessRecordsFindCourseNullTest()
        {
            // Setup
            SetupMocks();
            Random random = new Random();
            List<CitiRecord> fakeCitiRecords = new List<CitiRecord>();
            CitiRecord fakeCitiRecord = fixture.Generate<CitiRecord>();
            fakeCitiRecords.Add(fakeCitiRecord);
            mockCitiService.Setup(f => f.GetRecords()).Returns(fakeCitiRecords);

            IsuImportHistory fakeIsuImportHisotry = fixture.Generate<IsuImportHistory>();
            fakeIsuImportHisotry.Id = random.Next(1, 10);
            mockCitiService.Setup(f => f.InsertImportHistory(fakeCitiRecord)).Returns(fakeIsuImportHisotry);

            History fakeHistory1 = null;
            mockLearnerWebServices.Setup(f => f.GetHistoryByCurriculaId(fakeCitiRecord)).Returns(fakeHistory1);

            History fakeHistory2 = null;
            mockCitiService.Setup(f => f.IsRecordVerified(fakeCitiRecord, out fakeHistory2)).Returns(false);

            string fakeUnivId = fixture.Generate<string>();
            mockCitiService.Setup(f => f.FindUser(fakeCitiRecord)).Returns(fakeUnivId);

            string fakeCourseId = null;
            mockCitiService.Setup(f => f.FindCourse(fakeCitiRecord)).Returns(fakeCourseId);

            //mockApplicationConfiguration.SetupGet(p => p.processType).Returns(ApplicationConfiguration.ProcessType.Insert);

            // Execute
            ITrainingService trainingService = new TrainingService(mockCitiService.Object, mockLearnerWebServices.Object, mockLogService.Object, mockReportingService.Object, mockApplicationConfiguration.Object, mockMailService.Object);
            trainingService.ProcessRecords();

            // Verify
            mockCitiService.Verify(f => f.GetRecords(), Times.Once);
            mockCitiService.Verify(f => f.UploadHistoryRecords(It.IsAny<List<History>>()), Times.Never);
            mockCitiService.Verify(f => f.InsertImportHistory(It.IsAny<CitiRecord>()), Times.Once);
            mockCitiService.Verify(f => f.IsRecordVerified(It.IsAny<CitiRecord>(), out fakeHistory2), Times.Once);
            mockCitiService.Verify(f => f.FindUser(It.IsAny<CitiRecord>()), Times.Once);
            mockCitiService.Verify(f => f.FindCourse(It.IsAny<CitiRecord>()), Times.Once);
            mockCitiService.VerifyNoOtherCalls();
            mockLearnerWebServices.Verify(f => f.GetHistoryByCurriculaId(It.IsAny<CitiRecord>()), Times.Once);
            mockLearnerWebServices.VerifyNoOtherCalls();
            mockMailService.Verify(f => f.SendMessages(), Times.Once);
            mockMailService.VerifyNoOtherCalls();
        }

        [Test]
        public void ProcessRecordsGetHistoryByCriteriaReturnsHistoryTest()
        {
            // Setup
            SetupMocks();
            Random random = new Random();
            List<CitiRecord> fakeCitiRecords = new List<CitiRecord>();
            CitiRecord fakeCitiRecord = fixture.Generate<CitiRecord>();
            fakeCitiRecords.Add(fakeCitiRecord);
            mockCitiService.Setup(f => f.GetRecords()).Returns(fakeCitiRecords);

            IsuImportHistory fakeIsuImportHisotry = fixture.Generate<IsuImportHistory>();
            fakeIsuImportHisotry.Id = random.Next(1, 10);
            mockCitiService.Setup(f => f.InsertImportHistory(fakeCitiRecord)).Returns(fakeIsuImportHisotry);

            History fakeHistory1 = null;
            mockLearnerWebServices.Setup(f => f.GetHistoryByCurriculaId(fakeCitiRecord)).Returns(fakeHistory1);

            History fakeHistory2 = null;
            mockCitiService.Setup(f => f.IsRecordVerified(fakeCitiRecord, out fakeHistory2)).Returns(false);

            string fakeUnivId = fixture.Generate<string>();
            mockCitiService.Setup(f => f.FindUser(fakeCitiRecord)).Returns(fakeUnivId);

            string fakeCourseId = fixture.Generate<string>();
            mockCitiService.Setup(f => f.FindCourse(fakeCitiRecord)).Returns(fakeCourseId);

            History fakeHistory3 = fixture.Generate<History>();
            mockLearnerWebServices.Setup(f => f.GetHistoryByCitiIdCourseIdDate(fakeCitiRecord)).Returns(fakeHistory3);

            mockApplicationConfiguration.SetupGet(p => p.processType).Returns(ApplicationConfiguration.ProcessType.Insert);

            // Execute
            ITrainingService trainingService = new TrainingService(mockCitiService.Object, mockLearnerWebServices.Object, mockLogService.Object, mockReportingService.Object, mockApplicationConfiguration.Object, mockMailService.Object);
            trainingService.ProcessRecords();

            // Verify
            mockCitiService.Verify(f => f.GetRecords(), Times.Once);
            mockCitiService.Verify(f => f.UploadHistoryRecords(It.IsAny<List<History>>()), Times.Never);
            mockCitiService.Verify(f => f.InsertImportHistory(It.IsAny<CitiRecord>()), Times.Once);
            mockCitiService.Verify(f => f.IsRecordVerified(It.IsAny<CitiRecord>(), out fakeHistory2), Times.Once);
            mockCitiService.Verify(f => f.FindUser(It.IsAny<CitiRecord>()), Times.Once);
            mockCitiService.Verify(f => f.FindCourse(It.IsAny<CitiRecord>()), Times.Once);
            mockCitiService.VerifyNoOtherCalls();
            mockLearnerWebServices.Verify(f => f.GetHistoryByCurriculaId(It.IsAny<CitiRecord>()), Times.Once);
            mockLearnerWebServices.Verify(f => f.GetHistoryByCitiIdCourseIdDate(It.IsAny<CitiRecord>()), Times.Once);
            mockLearnerWebServices.VerifyNoOtherCalls();
            mockMailService.Verify(f => f.SendMessages(), Times.Once);
            mockMailService.VerifyNoOtherCalls();
        }

        [Test]
        public void ProcessRecordsGetHistoryByCriteriaReturnsNullUploadTest()
        {
            // Setup
            SetupMocks();
            Random random = new Random();
            List<CitiRecord> fakeCitiRecords = new List<CitiRecord>();
            CitiRecord fakeCitiRecord = fixture.Generate<CitiRecord>();
            fakeCitiRecords.Add(fakeCitiRecord);
            mockCitiService.Setup(f => f.GetRecords()).Returns(fakeCitiRecords);

            IsuImportHistory fakeIsuImportHisotry = fixture.Generate<IsuImportHistory>();
            fakeIsuImportHisotry.Id = random.Next(1, 10);
            mockCitiService.Setup(f => f.InsertImportHistory(fakeCitiRecord)).Returns(fakeIsuImportHisotry);

            History fakeHistory1 = null;
            mockLearnerWebServices.Setup(f => f.GetHistoryByCurriculaId(fakeCitiRecord)).Returns(fakeHistory1);

            History fakeHistory2 = null;
            mockCitiService.Setup(f => f.IsRecordVerified(fakeCitiRecord, out fakeHistory2)).Returns(false);

            string fakeUnivId = fixture.Generate<string>();
            mockCitiService.Setup(f => f.FindUser(fakeCitiRecord)).Returns(fakeUnivId);

            string fakeCourseId = fixture.Generate<string>();
            mockCitiService.Setup(f => f.FindCourse(fakeCitiRecord)).Returns(fakeCourseId);

            History fakeHistory3 = fixture.Generate<History>();
            mockLearnerWebServices.Setup(f => f.GetHistoryByCitiIdCourseIdDate(fakeCitiRecord)).Returns(fakeHistory3);

            mockApplicationConfiguration.SetupGet(p => p.processType).Returns(ApplicationConfiguration.ProcessType.Upload);

            // Execute
            ITrainingService trainingService = new TrainingService(mockCitiService.Object, mockLearnerWebServices.Object, mockLogService.Object, mockReportingService.Object, mockApplicationConfiguration.Object, mockMailService.Object);
            trainingService.ProcessRecords();

            // Verify
            mockCitiService.Verify(f => f.GetRecords(), Times.Once);
            mockCitiService.Verify(f => f.UploadHistoryRecords(It.IsAny<List<History>>()), Times.Once);
            mockCitiService.Verify(f => f.InsertImportHistory(It.IsAny<CitiRecord>()), Times.Once);
            mockCitiService.Verify(f => f.IsRecordVerified(It.IsAny<CitiRecord>(), out fakeHistory2), Times.Once);
            mockCitiService.Verify(f => f.FindUser(It.IsAny<CitiRecord>()), Times.Once);
            mockCitiService.Verify(f => f.FindCourse(It.IsAny<CitiRecord>()), Times.Once);
            mockCitiService.VerifyNoOtherCalls();
            mockLearnerWebServices.Verify(f => f.GetHistoryByCurriculaId(It.IsAny<CitiRecord>()), Times.Once);
            mockLearnerWebServices.Verify(f => f.GetHistoryByCitiIdCourseIdDate(It.IsAny<CitiRecord>()), Times.Once);
            mockLearnerWebServices.VerifyNoOtherCalls();
            mockMailService.Verify(f => f.SendMessages(), Times.Once);
            mockMailService.VerifyNoOtherCalls();
        }

        [Test]
        public void ProcessRecordsCreateHistoryUploadTest()
        {
            // Setup
            SetupMocks();
            Random random = new Random();
            List<CitiRecord> fakeCitiRecords = new List<CitiRecord>();
            CitiRecord fakeCitiRecord = fixture.Generate<CitiRecord>();
            fakeCitiRecords.Add(fakeCitiRecord);
            mockCitiService.Setup(f => f.GetRecords()).Returns(fakeCitiRecords);

            IsuImportHistory fakeIsuImportHisotry = fixture.Generate<IsuImportHistory>();
            fakeIsuImportHisotry.Id = random.Next(1, 10);
            mockCitiService.Setup(f => f.InsertImportHistory(fakeCitiRecord)).Returns(fakeIsuImportHisotry);

            History fakeHistory1 = null;
            mockLearnerWebServices.Setup(f => f.GetHistoryByCurriculaId(fakeCitiRecord)).Returns(fakeHistory1);

            History fakeHistory2 = null;
            mockCitiService.Setup(f => f.IsRecordVerified(fakeCitiRecord, out fakeHistory2)).Returns(false);

            string fakeUnivId = fixture.Generate<string>();
            mockCitiService.Setup(f => f.FindUser(fakeCitiRecord)).Returns(fakeUnivId);

            string fakeCourseId = fixture.Generate<string>();
            mockCitiService.Setup(f => f.FindCourse(fakeCitiRecord)).Returns(fakeCourseId);

            History fakeHistory3 = null;
            mockLearnerWebServices.Setup(f => f.GetHistoryByCitiIdCourseIdDate(fakeCitiRecord)).Returns(fakeHistory3);

            History fakeHistory4 = fixture.Generate<History>();
            mockLearnerWebServices.Setup(f => f.CreateHistoryRecord(fakeCitiRecord)).Returns(fakeHistory4);
            mockCitiService.Setup(f => f.InsertSingleHistoryRecord(fakeHistory4)).Verifiable();
            mockLearnerWebServices.Setup(f => f.SetInsertedForImportRecord(fakeIsuImportHisotry.Id)).Verifiable();

            mockApplicationConfiguration.SetupGet(p => p.processType).Returns(ApplicationConfiguration.ProcessType.Upload);

            // Execute
            ITrainingService trainingService = new TrainingService(mockCitiService.Object, mockLearnerWebServices.Object, mockLogService.Object, mockReportingService.Object, mockApplicationConfiguration.Object, mockMailService.Object);
            trainingService.ProcessRecords();

            // Verify
            mockCitiService.Verify(f => f.GetRecords(), Times.Once);
            mockCitiService.Verify(f => f.UploadHistoryRecords(It.IsAny<List<History>>()), Times.Once);
            mockCitiService.Verify(f => f.InsertImportHistory(It.IsAny<CitiRecord>()), Times.Once);
            mockCitiService.Verify(f => f.IsRecordVerified(It.IsAny<CitiRecord>(), out fakeHistory2), Times.Once);
            mockCitiService.Verify(f => f.FindUser(It.IsAny<CitiRecord>()), Times.Once);
            mockCitiService.Verify(f => f.FindCourse(It.IsAny<CitiRecord>()), Times.Once);
            mockCitiService.Verify(f => f.InsertSingleHistoryRecord(It.IsAny<History>()), Times.Once);
            mockCitiService.VerifyNoOtherCalls();
            mockLearnerWebServices.Verify(f => f.GetHistoryByCurriculaId(It.IsAny<CitiRecord>()), Times.Once);
            mockLearnerWebServices.Verify(f => f.GetHistoryByCitiIdCourseIdDate(It.IsAny<CitiRecord>()), Times.Once);
            mockLearnerWebServices.Verify(f => f.CreateHistoryRecord(It.IsAny<CitiRecord>()), Times.Once);
            mockLearnerWebServices.Verify(f => f.CreateHistoryRecord(It.IsAny<CitiRecord>()), Times.Once);
            mockLearnerWebServices.Verify(f => f.SetInsertedForImportRecord(It.IsAny<int>()), Times.Once);
            mockLearnerWebServices.VerifyNoOtherCalls();
            mockMailService.Verify(f => f.SendMessages(), Times.Once);
            mockMailService.VerifyNoOtherCalls();
            Mock.VerifyAll(mockCitiService);
            Mock.VerifyAll(mockLearnerWebServices);
        }

        [Test]
        public void ProcessRecordsCreateHistoryThrowsExceptionUploadTest()
        {
            // Setup
            SetupMocks();
            Random random = new Random();
            List<CitiRecord> fakeCitiRecords = new List<CitiRecord>();
            CitiRecord fakeCitiRecord = fixture.Generate<CitiRecord>();
            fakeCitiRecords.Add(fakeCitiRecord);
            mockCitiService.Setup(f => f.GetRecords()).Returns(fakeCitiRecords);

            IsuImportHistory fakeIsuImportHisotry = fixture.Generate<IsuImportHistory>();
            fakeIsuImportHisotry.Id = random.Next(1, 10);
            mockCitiService.Setup(f => f.InsertImportHistory(fakeCitiRecord)).Returns(fakeIsuImportHisotry);

            History fakeHistory1 = null;
            mockLearnerWebServices.Setup(f => f.GetHistoryByCurriculaId(fakeCitiRecord)).Returns(fakeHistory1);

            History fakeHistory2 = null;
            mockCitiService.Setup(f => f.IsRecordVerified(fakeCitiRecord, out fakeHistory2)).Returns(false);

            string fakeUnivId = fixture.Generate<string>();
            mockCitiService.Setup(f => f.FindUser(fakeCitiRecord)).Returns(fakeUnivId);

            string fakeCourseId = fixture.Generate<string>();
            mockCitiService.Setup(f => f.FindCourse(fakeCitiRecord)).Returns(fakeCourseId);

            History fakeHistory3 = null;
            mockLearnerWebServices.Setup(f => f.GetHistoryByCitiIdCourseIdDate(fakeCitiRecord)).Returns(fakeHistory3);

            History fakeHistory4 = fixture.Generate<History>();
            mockLearnerWebServices.Setup(f => f.CreateHistoryRecord(fakeCitiRecord)).Throws(new Exception());
            //mockCitiService.Setup(f => f.InsertSingleHistoryRecord(fakeHistory4)).Verifiable();
            //mockLearnerWebServices.Setup(f => f.SetInsertedForImportRecord(fakeIsuImportHisotry.Id)).Verifiable();

            mockApplicationConfiguration.SetupGet(p => p.processType).Returns(ApplicationConfiguration.ProcessType.Upload);

            // Execute
            ITrainingService trainingService = new TrainingService(mockCitiService.Object, mockLearnerWebServices.Object, mockLogService.Object, mockReportingService.Object, mockApplicationConfiguration.Object, mockMailService.Object);
            trainingService.ProcessRecords();

            // Verify
            mockCitiService.Verify(f => f.GetRecords(), Times.Once);
            mockCitiService.Verify(f => f.UploadHistoryRecords(It.IsAny<List<History>>()), Times.Never);
            mockCitiService.Verify(f => f.InsertImportHistory(It.IsAny<CitiRecord>()), Times.Once);
            mockCitiService.Verify(f => f.IsRecordVerified(It.IsAny<CitiRecord>(), out fakeHistory2), Times.Once);
            mockCitiService.Verify(f => f.FindUser(It.IsAny<CitiRecord>()), Times.Once);
            mockCitiService.Verify(f => f.FindCourse(It.IsAny<CitiRecord>()), Times.Once);
            mockCitiService.Verify(f => f.InsertSingleHistoryRecord(It.IsAny<History>()), Times.Never);
            mockCitiService.VerifyNoOtherCalls();
            mockLearnerWebServices.Verify(f => f.GetHistoryByCurriculaId(It.IsAny<CitiRecord>()), Times.Once);
            mockLearnerWebServices.Verify(f => f.GetHistoryByCitiIdCourseIdDate(It.IsAny<CitiRecord>()), Times.Once);
            mockLearnerWebServices.Verify(f => f.CreateHistoryRecord(It.IsAny<CitiRecord>()), Times.Once);
            mockLearnerWebServices.Verify(f => f.CreateHistoryRecord(It.IsAny<CitiRecord>()), Times.Once);
            mockLearnerWebServices.Verify(f => f.SetInsertedForImportRecord(It.IsAny<int>()), Times.Never);
            mockLearnerWebServices.VerifyNoOtherCalls();
            mockMailService.Verify(f => f.SendMessages(), Times.Once);
            mockMailService.VerifyNoOtherCalls();
            mockLogService.Verify(f => f.GetCacheAndFlush(), Times.Once);
            mockReportingService.Verify(f => f.ReportSystemError(It.IsAny<SystemError>(), It.IsAny<List<string>>()), Times.Once);
            Mock.VerifyAll(mockCitiService);
            Mock.VerifyAll(mockLearnerWebServices);
        }

        [Test]
        public void ProcessRecordsInsertsingleHistoryThrowsExceptionUploadTest()
        {
            // Setup
            SetupMocks();
            Random random = new Random();
            List<CitiRecord> fakeCitiRecords = new List<CitiRecord>();
            CitiRecord fakeCitiRecord = fixture.Generate<CitiRecord>();
            fakeCitiRecords.Add(fakeCitiRecord);
            mockCitiService.Setup(f => f.GetRecords()).Returns(fakeCitiRecords);

            IsuImportHistory fakeIsuImportHisotry = fixture.Generate<IsuImportHistory>();
            fakeIsuImportHisotry.Id = random.Next(1, 10);
            mockCitiService.Setup(f => f.InsertImportHistory(fakeCitiRecord)).Returns(fakeIsuImportHisotry);

            History fakeHistory1 = null;
            mockLearnerWebServices.Setup(f => f.GetHistoryByCurriculaId(fakeCitiRecord)).Returns(fakeHistory1);

            History fakeHistory2 = null;
            mockCitiService.Setup(f => f.IsRecordVerified(fakeCitiRecord, out fakeHistory2)).Returns(false);

            string fakeUnivId = fixture.Generate<string>();
            mockCitiService.Setup(f => f.FindUser(fakeCitiRecord)).Returns(fakeUnivId);

            string fakeCourseId = fixture.Generate<string>();
            mockCitiService.Setup(f => f.FindCourse(fakeCitiRecord)).Returns(fakeCourseId);

            History fakeHistory3 = null;
            mockLearnerWebServices.Setup(f => f.GetHistoryByCitiIdCourseIdDate(fakeCitiRecord)).Returns(fakeHistory3);

            History fakeHistory4 = fixture.Generate<History>();
            mockLearnerWebServices.Setup(f => f.CreateHistoryRecord(fakeCitiRecord)).Returns(fakeHistory4);
            mockCitiService.Setup(f => f.InsertSingleHistoryRecord(fakeHistory4)).Throws(new Exception());
            //mockLearnerWebServices.Setup(f => f.SetInsertedForImportRecord(fakeIsuImportHisotry.Id)).Verifiable();

            mockApplicationConfiguration.SetupGet(p => p.processType).Returns(ApplicationConfiguration.ProcessType.Upload);

            // Execute
            ITrainingService trainingService = new TrainingService(mockCitiService.Object, mockLearnerWebServices.Object, mockLogService.Object, mockReportingService.Object, mockApplicationConfiguration.Object, mockMailService.Object);
            trainingService.ProcessRecords();

            // Verify
            mockCitiService.Verify(f => f.GetRecords(), Times.Once);
            mockCitiService.Verify(f => f.UploadHistoryRecords(It.IsAny<List<History>>()), Times.Once);
            mockCitiService.Verify(f => f.InsertImportHistory(It.IsAny<CitiRecord>()), Times.Once);
            mockCitiService.Verify(f => f.IsRecordVerified(It.IsAny<CitiRecord>(), out fakeHistory2), Times.Once);
            mockCitiService.Verify(f => f.FindUser(It.IsAny<CitiRecord>()), Times.Once);
            mockCitiService.Verify(f => f.FindCourse(It.IsAny<CitiRecord>()), Times.Once);
            mockCitiService.Verify(f => f.InsertSingleHistoryRecord(It.IsAny<History>()), Times.Once);
            mockCitiService.VerifyNoOtherCalls();
            mockLearnerWebServices.Verify(f => f.GetHistoryByCurriculaId(It.IsAny<CitiRecord>()), Times.Once);
            mockLearnerWebServices.Verify(f => f.GetHistoryByCitiIdCourseIdDate(It.IsAny<CitiRecord>()), Times.Once);
            mockLearnerWebServices.Verify(f => f.CreateHistoryRecord(It.IsAny<CitiRecord>()), Times.Once);
            mockLearnerWebServices.Verify(f => f.CreateHistoryRecord(It.IsAny<CitiRecord>()), Times.Once);
            mockLearnerWebServices.Verify(f => f.SetInsertedForImportRecord(It.IsAny<int>()), Times.Never);
            mockLearnerWebServices.VerifyNoOtherCalls();
            mockMailService.Verify(f => f.SendMessages(), Times.Once);
            mockMailService.VerifyNoOtherCalls();
            mockLogService.Verify(f => f.GetCacheAndFlush(), Times.Once);
            mockReportingService.Verify(f => f.ReportSystemError(It.IsAny<SystemError>(), It.IsAny<List<string>>()), Times.Once);
            Mock.VerifyAll(mockCitiService);
            Mock.VerifyAll(mockLearnerWebServices);
        }

        [Test]
        public void ProcessRecordsSetInsertedThrowsExceptionUploadTest()
        {
            // Setup
            SetupMocks();
            Random random = new Random();
            List<CitiRecord> fakeCitiRecords = new List<CitiRecord>();
            CitiRecord fakeCitiRecord = fixture.Generate<CitiRecord>();
            fakeCitiRecords.Add(fakeCitiRecord);
            mockCitiService.Setup(f => f.GetRecords()).Returns(fakeCitiRecords);

            IsuImportHistory fakeIsuImportHisotry = fixture.Generate<IsuImportHistory>();
            fakeIsuImportHisotry.Id = random.Next(1, 10);
            mockCitiService.Setup(f => f.InsertImportHistory(fakeCitiRecord)).Returns(fakeIsuImportHisotry);

            History fakeHistory1 = null;
            mockLearnerWebServices.Setup(f => f.GetHistoryByCurriculaId(fakeCitiRecord)).Returns(fakeHistory1);

            History fakeHistory2 = null;
            mockCitiService.Setup(f => f.IsRecordVerified(fakeCitiRecord, out fakeHistory2)).Returns(false);

            string fakeUnivId = fixture.Generate<string>();
            mockCitiService.Setup(f => f.FindUser(fakeCitiRecord)).Returns(fakeUnivId);

            string fakeCourseId = fixture.Generate<string>();
            mockCitiService.Setup(f => f.FindCourse(fakeCitiRecord)).Returns(fakeCourseId);

            History fakeHistory3 = null;
            mockLearnerWebServices.Setup(f => f.GetHistoryByCitiIdCourseIdDate(fakeCitiRecord)).Returns(fakeHistory3);

            History fakeHistory4 = fixture.Generate<History>();
            mockLearnerWebServices.Setup(f => f.CreateHistoryRecord(fakeCitiRecord)).Returns(fakeHistory4);
            mockCitiService.Setup(f => f.InsertSingleHistoryRecord(fakeHistory4)).Verifiable();
            mockLearnerWebServices.Setup(f => f.SetInsertedForImportRecord(fakeIsuImportHisotry.Id)).Throws(new Exception());

            mockApplicationConfiguration.SetupGet(p => p.processType).Returns(ApplicationConfiguration.ProcessType.Upload);

            // Execute
            ITrainingService trainingService = new TrainingService(mockCitiService.Object, mockLearnerWebServices.Object, mockLogService.Object, mockReportingService.Object, mockApplicationConfiguration.Object, mockMailService.Object);
            trainingService.ProcessRecords();

            // Verify
            mockCitiService.Verify(f => f.GetRecords(), Times.Once);
            mockCitiService.Verify(f => f.UploadHistoryRecords(It.IsAny<List<History>>()), Times.Once);
            mockCitiService.Verify(f => f.InsertImportHistory(It.IsAny<CitiRecord>()), Times.Once);
            mockCitiService.Verify(f => f.IsRecordVerified(It.IsAny<CitiRecord>(), out fakeHistory2), Times.Once);
            mockCitiService.Verify(f => f.FindUser(It.IsAny<CitiRecord>()), Times.Once);
            mockCitiService.Verify(f => f.FindCourse(It.IsAny<CitiRecord>()), Times.Once);
            mockCitiService.Verify(f => f.InsertSingleHistoryRecord(It.IsAny<History>()), Times.Once);
            mockCitiService.VerifyNoOtherCalls();
            mockLearnerWebServices.Verify(f => f.GetHistoryByCurriculaId(It.IsAny<CitiRecord>()), Times.Once);
            mockLearnerWebServices.Verify(f => f.GetHistoryByCitiIdCourseIdDate(It.IsAny<CitiRecord>()), Times.Once);
            mockLearnerWebServices.Verify(f => f.CreateHistoryRecord(It.IsAny<CitiRecord>()), Times.Once);
            mockLearnerWebServices.Verify(f => f.CreateHistoryRecord(It.IsAny<CitiRecord>()), Times.Once);
            mockLearnerWebServices.Verify(f => f.SetInsertedForImportRecord(It.IsAny<int>()), Times.Once);
            mockLearnerWebServices.VerifyNoOtherCalls();
            mockMailService.Verify(f => f.SendMessages(), Times.Once);
            mockMailService.VerifyNoOtherCalls();
            mockLogService.Verify(f => f.GetCacheAndFlush(), Times.Once);
            mockReportingService.Verify(f => f.ReportSystemError(It.IsAny<SystemError>(), It.IsAny<List<string>>()), Times.Once);
            Mock.VerifyAll(mockCitiService);
            Mock.VerifyAll(mockLearnerWebServices);
        }

        [Test]
        public void ProcessRecordsCreateHistoryInsertTest()
        {
            // Setup
            SetupMocks();
            Random random = new Random();
            List<CitiRecord> fakeCitiRecords = new List<CitiRecord>();
            CitiRecord fakeCitiRecord = fixture.Generate<CitiRecord>();
            fakeCitiRecords.Add(fakeCitiRecord);
            mockCitiService.Setup(f => f.GetRecords()).Returns(fakeCitiRecords);

            IsuImportHistory fakeIsuImportHisotry = fixture.Generate<IsuImportHistory>();
            fakeIsuImportHisotry.Id = random.Next(1, 10);
            mockCitiService.Setup(f => f.InsertImportHistory(fakeCitiRecord)).Returns(fakeIsuImportHisotry);

            History fakeHistory1 = null;
            mockLearnerWebServices.Setup(f => f.GetHistoryByCurriculaId(fakeCitiRecord)).Returns(fakeHistory1);

            History fakeHistory2 = null;
            mockCitiService.Setup(f => f.IsRecordVerified(fakeCitiRecord, out fakeHistory2)).Returns(false);

            string fakeUnivId = fixture.Generate<string>();
            mockCitiService.Setup(f => f.FindUser(fakeCitiRecord)).Returns(fakeUnivId);

            string fakeCourseId = fixture.Generate<string>();
            mockCitiService.Setup(f => f.FindCourse(fakeCitiRecord)).Returns(fakeCourseId);

            History fakeHistory3 = null;
            mockLearnerWebServices.Setup(f => f.GetHistoryByCitiIdCourseIdDate(fakeCitiRecord)).Returns(fakeHistory3);

            History fakeHistory4 = fixture.Generate<History>();
            mockLearnerWebServices.Setup(f => f.CreateHistoryRecord(fakeCitiRecord)).Returns(fakeHistory4);
            mockCitiService.Setup(f => f.InsertSingleHistoryRecord(fakeHistory4)).Verifiable();
            mockLearnerWebServices.Setup(f => f.SetInsertedForImportRecord(fakeIsuImportHisotry.Id)).Verifiable();

            mockApplicationConfiguration.SetupGet(p => p.processType).Returns(ApplicationConfiguration.ProcessType.Insert);

            // Execute
            ITrainingService trainingService = new TrainingService(mockCitiService.Object, mockLearnerWebServices.Object, mockLogService.Object, mockReportingService.Object, mockApplicationConfiguration.Object, mockMailService.Object);
            trainingService.ProcessRecords();

            // Verify
            mockCitiService.Verify(f => f.GetRecords(), Times.Once);
            mockCitiService.Verify(f => f.UploadHistoryRecords(It.IsAny<List<History>>()), Times.Never);
            mockCitiService.Verify(f => f.InsertImportHistory(It.IsAny<CitiRecord>()), Times.Once);
            mockCitiService.Verify(f => f.IsRecordVerified(It.IsAny<CitiRecord>(), out fakeHistory2), Times.Once);
            mockCitiService.Verify(f => f.FindUser(It.IsAny<CitiRecord>()), Times.Once);
            mockCitiService.Verify(f => f.FindCourse(It.IsAny<CitiRecord>()), Times.Once);
            mockCitiService.Verify(f => f.InsertSingleHistoryRecord(It.IsAny<History>()), Times.Once);
            mockCitiService.VerifyNoOtherCalls();
            mockLearnerWebServices.Verify(f => f.GetHistoryByCurriculaId(It.IsAny<CitiRecord>()), Times.Once);
            mockLearnerWebServices.Verify(f => f.GetHistoryByCitiIdCourseIdDate(It.IsAny<CitiRecord>()), Times.Once);
            mockLearnerWebServices.Verify(f => f.CreateHistoryRecord(It.IsAny<CitiRecord>()), Times.Once);
            mockLearnerWebServices.Verify(f => f.CreateHistoryRecord(It.IsAny<CitiRecord>()), Times.Once);
            mockLearnerWebServices.Verify(f => f.SetInsertedForImportRecord(It.IsAny<int>()), Times.Once);
            mockLearnerWebServices.VerifyNoOtherCalls();
            mockMailService.Verify(f => f.SendMessages(), Times.Once);
            mockMailService.VerifyNoOtherCalls();
            Mock.VerifyAll(mockCitiService);
            Mock.VerifyAll(mockLearnerWebServices);
        }

        [Test]
        public void ProcessRecordsErrorSendingMessagesTest()
        {
            // Setup
            SetupMocks();
            Random random = new Random();
            List<CitiRecord> fakeCitiRecords = new List<CitiRecord>();
            CitiRecord fakeCitiRecord = fixture.Generate<CitiRecord>();
            fakeCitiRecords.Add(fakeCitiRecord);
            mockCitiService.Setup(f => f.GetRecords()).Returns(fakeCitiRecords);

            IsuImportHistory fakeIsuImportHisotry = fixture.Generate<IsuImportHistory>();
            fakeIsuImportHisotry.Id = random.Next(1, 10);
            mockCitiService.Setup(f => f.InsertImportHistory(fakeCitiRecord)).Returns(fakeIsuImportHisotry);

            History fakeHistory1 = null;
            mockLearnerWebServices.Setup(f => f.GetHistoryByCurriculaId(fakeCitiRecord)).Returns(fakeHistory1);

            History fakeHistory2 = null;
            mockCitiService.Setup(f => f.IsRecordVerified(fakeCitiRecord, out fakeHistory2)).Returns(false);

            string fakeUnivId = fixture.Generate<string>();
            mockCitiService.Setup(f => f.FindUser(fakeCitiRecord)).Returns(fakeUnivId);

            string fakeCourseId = fixture.Generate<string>();
            mockCitiService.Setup(f => f.FindCourse(fakeCitiRecord)).Returns(fakeCourseId);

            History fakeHistory3 = null;
            mockLearnerWebServices.Setup(f => f.GetHistoryByCitiIdCourseIdDate(fakeCitiRecord)).Returns(fakeHistory3);

            History fakeHistory4 = fixture.Generate<History>();
            mockLearnerWebServices.Setup(f => f.CreateHistoryRecord(fakeCitiRecord)).Returns(fakeHistory4);
            mockCitiService.Setup(f => f.InsertSingleHistoryRecord(fakeHistory4)).Verifiable();
            mockLearnerWebServices.Setup(f => f.SetInsertedForImportRecord(fakeIsuImportHisotry.Id)).Verifiable();

            mockMailService.Setup(f => f.SendMessages()).Throws(new SendMailException("test"));

            mockApplicationConfiguration.SetupGet(p => p.processType).Returns(ApplicationConfiguration.ProcessType.Insert);

            // Execute
            ITrainingService trainingService = new TrainingService(mockCitiService.Object, mockLearnerWebServices.Object, mockLogService.Object, mockReportingService.Object, mockApplicationConfiguration.Object, mockMailService.Object);
            trainingService.ProcessRecords();

            // Verify
            mockCitiService.Verify(f => f.GetRecords(), Times.Once);
            mockCitiService.Verify(f => f.UploadHistoryRecords(It.IsAny<List<History>>()), Times.Never);
            mockCitiService.Verify(f => f.InsertImportHistory(It.IsAny<CitiRecord>()), Times.Once);
            mockCitiService.Verify(f => f.IsRecordVerified(It.IsAny<CitiRecord>(), out fakeHistory2), Times.Once);
            mockCitiService.Verify(f => f.FindUser(It.IsAny<CitiRecord>()), Times.Once);
            mockCitiService.Verify(f => f.FindCourse(It.IsAny<CitiRecord>()), Times.Once);
            mockCitiService.Verify(f => f.InsertSingleHistoryRecord(It.IsAny<History>()), Times.Once);
            mockCitiService.VerifyNoOtherCalls();
            mockLearnerWebServices.Verify(f => f.GetHistoryByCurriculaId(It.IsAny<CitiRecord>()), Times.Once);
            mockLearnerWebServices.Verify(f => f.GetHistoryByCitiIdCourseIdDate(It.IsAny<CitiRecord>()), Times.Once);
            mockLearnerWebServices.Verify(f => f.CreateHistoryRecord(It.IsAny<CitiRecord>()), Times.Once);
            mockLearnerWebServices.Verify(f => f.CreateHistoryRecord(It.IsAny<CitiRecord>()), Times.Once);
            mockLearnerWebServices.Verify(f => f.SetInsertedForImportRecord(It.IsAny<int>()), Times.Once);
            mockLearnerWebServices.VerifyNoOtherCalls();
            mockMailService.Verify(f => f.SendMessages(), Times.Once);
            mockMailService.VerifyNoOtherCalls();
            Mock.VerifyAll(mockCitiService);
            Mock.VerifyAll(mockLearnerWebServices);
        }

    }
}
