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
using TrainingDownloader.configurations;
using TrainingDownloader.models.entities;
using TrainingDownloader.exceptions;

namespace TrainingDownloaderTests.services
{
    [TestFixture]
    public class TrainingServiceTests
    {
        private Mock<IVendorService> mockCitiService;
        private Mock<ILearnerWebServices> mockLearnerWebServices;
        private Mock<ILogService> mockLogService;
        private Mock<IReportingService> mockReportingService;
        private ApplicationConfiguration applicationConfiguration;
        private Mock<IMailService> mockMailService;
        private Fixture fixture;

        public TrainingServiceTests()
        {
            fixture = new Fixture();
        }

        private void SetupMocks()
        {
            mockCitiService = new Mock<IVendorService>();
            mockLearnerWebServices = new Mock<ILearnerWebServices>();
            mockLogService = new Mock<ILogService>();
            mockMailService = new Mock<IMailService>();
            mockReportingService = new Mock<IReportingService>();
            applicationConfiguration = new ApplicationConfiguration();
        }

        private VendorRecord CreateValidFakeRecord()
        {
            VendorRecord vendorRecord = fixture.Generate<VendorRecord>();
            vendorRecord.Score = 80;
            vendorRecord.PassingScore = 80;
            return vendorRecord;
        }


        [Test]
        public void ProcessRecordsNullTest()
        {
            // Setup
            SetupMocks();
            List<VendorRecord> fakeVendorRecords = null;
            mockCitiService.Setup(f => f.GetRecords()).Returns(fakeVendorRecords);

            // Execute
            ITrainingService trainingService = new TrainingService(mockCitiService.Object, mockLearnerWebServices.Object, mockLogService.Object, mockReportingService.Object, applicationConfiguration, mockMailService.Object);
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
            List<VendorRecord> fakeVendorRecords = new List<VendorRecord>();
            mockCitiService.Setup(f => f.GetRecords()).Returns(fakeVendorRecords);

            // Execute
            ITrainingService trainingService = new TrainingService(mockCitiService.Object, mockLearnerWebServices.Object, mockLogService.Object, mockReportingService.Object, applicationConfiguration, mockMailService.Object);
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
            List<VendorRecord> fakeVendorRecords = new List<VendorRecord>();
            VendorRecord fakeVendorRecord = CreateValidFakeRecord();
            fakeVendorRecords.Add(fakeVendorRecord);
            mockCitiService.Setup(f => f.GetRecords()).Returns(fakeVendorRecords);

            IsuImportHistory fakeIsuImportHisotry = fixture.Generate<IsuImportHistory>();
            fakeIsuImportHisotry.Id = 0;
            mockCitiService.Setup(f => f.InsertImportHistory(fakeVendorRecord)).Returns(fakeIsuImportHisotry);

            // Execute
            ITrainingService trainingService = new TrainingService(mockCitiService.Object, mockLearnerWebServices.Object, mockLogService.Object, mockReportingService.Object, applicationConfiguration, mockMailService.Object);
            trainingService.ProcessRecords();

            // Verify
            mockCitiService.Verify(f => f.GetRecords(), Times.Once);
            mockCitiService.Verify(f => f.InsertImportHistory(It.IsAny<VendorRecord>()), Times.Once);
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
            List<VendorRecord> fakeVendorRecords = new List<VendorRecord>();
            VendorRecord fakeVendorRecord = CreateValidFakeRecord();
            fakeVendorRecords.Add(fakeVendorRecord);
            mockCitiService.Setup(f => f.GetRecords()).Returns(fakeVendorRecords);

            IsuImportHistory fakeIsuImportHisotry = fixture.Generate<IsuImportHistory>();
            fakeIsuImportHisotry.Id = random.Next(1,10);
            mockCitiService.Setup(f => f.InsertImportHistory(fakeVendorRecord)).Returns(fakeIsuImportHisotry);

            History fakeHistory = fixture.Generate<History>();
            mockLearnerWebServices.Setup(f => f.GetHistoryByCurriculaId(fakeVendorRecord)).Returns(fakeHistory);
            applicationConfiguration.processType = CommandLineConfiguration.ProcessType.Upload;

            // Execute
            ITrainingService trainingService = new TrainingService(mockCitiService.Object, mockLearnerWebServices.Object, mockLogService.Object, mockReportingService.Object, applicationConfiguration, mockMailService.Object);
            trainingService.ProcessRecords();

            // Verify
            mockCitiService.Verify(f => f.GetRecords(), Times.Once);
            mockCitiService.Verify(f => f.UploadHistoryRecords(It.IsAny<List<History>>()), Times.Once);
            mockCitiService.Verify(f => f.InsertImportHistory(It.IsAny<VendorRecord>()), Times.Once);
            mockCitiService.VerifyNoOtherCalls();
            mockLearnerWebServices.Verify(f => f.GetHistoryByCurriculaId(It.IsAny<VendorRecord>()), Times.Once);
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
            List<VendorRecord> fakeVendorRecords = new List<VendorRecord>();
            VendorRecord fakeVendorRecord = CreateValidFakeRecord();
            fakeVendorRecords.Add(fakeVendorRecord);
            mockCitiService.Setup(f => f.GetRecords()).Returns(fakeVendorRecords);

            IsuImportHistory fakeIsuImportHisotry = fixture.Generate<IsuImportHistory>();
            fakeIsuImportHisotry.Id = random.Next(1, 10);
            mockCitiService.Setup(f => f.InsertImportHistory(fakeVendorRecord)).Returns(fakeIsuImportHisotry);

            History fakeHistory = fixture.Generate<History>();
            mockLearnerWebServices.Setup(f => f.GetHistoryByCurriculaId(fakeVendorRecord)).Returns(fakeHistory);
            applicationConfiguration.processType = CommandLineConfiguration.ProcessType.Insert;

            // Execute
            ITrainingService trainingService = new TrainingService(mockCitiService.Object, mockLearnerWebServices.Object, mockLogService.Object, mockReportingService.Object, applicationConfiguration, mockMailService.Object);
            trainingService.ProcessRecords();

            // Verify
            mockCitiService.Verify(f => f.GetRecords(), Times.Once);
            mockCitiService.Verify(f => f.UploadHistoryRecords(It.IsAny<List<History>>()), Times.Never);
            mockCitiService.Verify(f => f.InsertImportHistory(It.IsAny<VendorRecord>()), Times.Once);
            mockCitiService.VerifyNoOtherCalls();
            mockLearnerWebServices.Verify(f => f.GetHistoryByCurriculaId(It.IsAny<VendorRecord>()), Times.Once);
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
            List<VendorRecord> fakeVendorRecords = new List<VendorRecord>();
            VendorRecord fakeVendorRecord = CreateValidFakeRecord();
            fakeVendorRecords.Add(fakeVendorRecord);
            mockCitiService.Setup(f => f.GetRecords()).Returns(fakeVendorRecords);

            IsuImportHistory fakeIsuImportHisotry = fixture.Generate<IsuImportHistory>();
            fakeIsuImportHisotry.Id = random.Next(1, 10);
            mockCitiService.Setup(f => f.InsertImportHistory(fakeVendorRecord)).Returns(fakeIsuImportHisotry);

            History fakeHistory1 = null;
            mockLearnerWebServices.Setup(f => f.GetHistoryByCurriculaId(fakeVendorRecord)).Returns(fakeHistory1);

            History fakeHistory2 = fixture.Generate<History>();
            mockCitiService.Setup(f => f.IsRecordVerified(fakeVendorRecord, out fakeHistory2)).Returns(true);

            applicationConfiguration.processType = CommandLineConfiguration.ProcessType.Upload;

            // Execute
            ITrainingService trainingService = new TrainingService(mockCitiService.Object, mockLearnerWebServices.Object, mockLogService.Object, mockReportingService.Object, applicationConfiguration, mockMailService.Object);
            trainingService.ProcessRecords();

            // Verify
            mockCitiService.Verify(f => f.GetRecords(), Times.Once);
            mockCitiService.Verify(f => f.UploadHistoryRecords(It.IsAny<List<History>>()), Times.Once);
            mockCitiService.Verify(f => f.InsertImportHistory(It.IsAny<VendorRecord>()), Times.Once);
            mockCitiService.Verify(f => f.IsRecordVerified(It.IsAny<VendorRecord>(), out fakeHistory2), Times.Once);
            mockCitiService.VerifyNoOtherCalls();
            mockLearnerWebServices.Verify(f => f.GetHistoryByCurriculaId(It.IsAny<VendorRecord>()), Times.Once);
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
            List<VendorRecord> fakeVendorRecords = new List<VendorRecord>();
            VendorRecord fakeVendorRecord = CreateValidFakeRecord();
            fakeVendorRecords.Add(fakeVendorRecord);
            mockCitiService.Setup(f => f.GetRecords()).Returns(fakeVendorRecords);

            IsuImportHistory fakeIsuImportHisotry = fixture.Generate<IsuImportHistory>();
            fakeIsuImportHisotry.Id = random.Next(1, 10);
            mockCitiService.Setup(f => f.InsertImportHistory(fakeVendorRecord)).Returns(fakeIsuImportHisotry);

            History fakeHistory1 = null;
            mockLearnerWebServices.Setup(f => f.GetHistoryByCurriculaId(fakeVendorRecord)).Returns(fakeHistory1);

            History fakeHistory2 = fixture.Generate<History>();
            mockCitiService.Setup(f => f.IsRecordVerified(fakeVendorRecord, out fakeHistory2)).Returns(true);

            applicationConfiguration.processType = CommandLineConfiguration.ProcessType.Insert;

            // Execute
            ITrainingService trainingService = new TrainingService(mockCitiService.Object, mockLearnerWebServices.Object, mockLogService.Object, mockReportingService.Object, applicationConfiguration, mockMailService.Object);
            trainingService.ProcessRecords();

            // Verify
            mockCitiService.Verify(f => f.GetRecords(), Times.Once);
            mockCitiService.Verify(f => f.UploadHistoryRecords(It.IsAny<List<History>>()), Times.Never);
            mockCitiService.Verify(f => f.InsertImportHistory(It.IsAny<VendorRecord>()), Times.Once);
            mockCitiService.Verify(f => f.IsRecordVerified(It.IsAny<VendorRecord>(), out fakeHistory2), Times.Once);
            mockCitiService.VerifyNoOtherCalls();
            mockLearnerWebServices.Verify(f => f.GetHistoryByCurriculaId(It.IsAny<VendorRecord>()), Times.Once);
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
            List<VendorRecord> fakeVendorRecords = new List<VendorRecord>();
            VendorRecord fakeVendorRecord = CreateValidFakeRecord();
            fakeVendorRecords.Add(fakeVendorRecord);
            mockCitiService.Setup(f => f.GetRecords()).Returns(fakeVendorRecords);

            IsuImportHistory fakeIsuImportHisotry = fixture.Generate<IsuImportHistory>();
            fakeIsuImportHisotry.Id = random.Next(1, 10);
            mockCitiService.Setup(f => f.InsertImportHistory(fakeVendorRecord)).Returns(fakeIsuImportHisotry);

            History fakeHistory1 = null;
            mockLearnerWebServices.Setup(f => f.GetHistoryByCurriculaId(fakeVendorRecord)).Returns(fakeHistory1);

            History fakeHistory2 = null;
            mockCitiService.Setup(f => f.IsRecordVerified(fakeVendorRecord, out fakeHistory2)).Returns(false);

            string fakeUnivId = null;
            mockCitiService.Setup(f => f.FindUser(fakeVendorRecord)).Returns(fakeUnivId);

            //mockApplicationConfiguration.SetupGet(p => p.processType).Returns(CommandLineConfiguration.ProcessType.Insert);

            // Execute
            ITrainingService trainingService = new TrainingService(mockCitiService.Object, mockLearnerWebServices.Object, mockLogService.Object, mockReportingService.Object, applicationConfiguration, mockMailService.Object);
            trainingService.ProcessRecords();

            // Verify
            mockCitiService.Verify(f => f.GetRecords(), Times.Once);
            mockCitiService.Verify(f => f.UploadHistoryRecords(It.IsAny<List<History>>()), Times.Never);
            mockCitiService.Verify(f => f.InsertImportHistory(It.IsAny<VendorRecord>()), Times.Once);
            mockCitiService.Verify(f => f.IsRecordVerified(It.IsAny<VendorRecord>(), out fakeHistory2), Times.Once);
            mockCitiService.Verify(f => f.FindUser(It.IsAny<VendorRecord>()), Times.Once);
            mockCitiService.VerifyNoOtherCalls();
            mockLearnerWebServices.Verify(f => f.GetHistoryByCurriculaId(It.IsAny<VendorRecord>()), Times.Once);
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
            List<VendorRecord> fakeVendorRecords = new List<VendorRecord>();
            VendorRecord fakeVendorRecord = CreateValidFakeRecord();
            fakeVendorRecords.Add(fakeVendorRecord);
            mockCitiService.Setup(f => f.GetRecords()).Returns(fakeVendorRecords);

            IsuImportHistory fakeIsuImportHisotry = fixture.Generate<IsuImportHistory>();
            fakeIsuImportHisotry.Id = random.Next(1, 10);
            mockCitiService.Setup(f => f.InsertImportHistory(fakeVendorRecord)).Returns(fakeIsuImportHisotry);

            History fakeHistory1 = null;
            mockLearnerWebServices.Setup(f => f.GetHistoryByCurriculaId(fakeVendorRecord)).Returns(fakeHistory1);

            History fakeHistory2 = null;
            mockCitiService.Setup(f => f.IsRecordVerified(fakeVendorRecord, out fakeHistory2)).Returns(false);

            string fakeUnivId = fixture.Generate<string>();
            mockCitiService.Setup(f => f.FindUser(fakeVendorRecord)).Returns(fakeUnivId);

            string fakeCourseId = null;
            mockCitiService.Setup(f => f.FindCourse(fakeVendorRecord)).Returns(fakeCourseId);

            //mockApplicationConfiguration.SetupGet(p => p.processType).Returns(CommandLineConfiguration.ProcessType.Insert);

            // Execute
            ITrainingService trainingService = new TrainingService(mockCitiService.Object, mockLearnerWebServices.Object, mockLogService.Object, mockReportingService.Object, applicationConfiguration, mockMailService.Object);
            trainingService.ProcessRecords();

            // Verify
            mockCitiService.Verify(f => f.GetRecords(), Times.Once);
            mockCitiService.Verify(f => f.UploadHistoryRecords(It.IsAny<List<History>>()), Times.Never);
            mockCitiService.Verify(f => f.InsertImportHistory(It.IsAny<VendorRecord>()), Times.Once);
            mockCitiService.Verify(f => f.IsRecordVerified(It.IsAny<VendorRecord>(), out fakeHistory2), Times.Once);
            mockCitiService.Verify(f => f.FindUser(It.IsAny<VendorRecord>()), Times.Once);
            mockCitiService.Verify(f => f.FindCourse(It.IsAny<VendorRecord>()), Times.Once);
            mockCitiService.VerifyNoOtherCalls();
            mockLearnerWebServices.Verify(f => f.GetHistoryByCurriculaId(It.IsAny<VendorRecord>()), Times.Once);
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
            List<VendorRecord> fakeVendorRecords = new List<VendorRecord>();
            VendorRecord fakeVendorRecord = CreateValidFakeRecord();
            fakeVendorRecords.Add(fakeVendorRecord);
            mockCitiService.Setup(f => f.GetRecords()).Returns(fakeVendorRecords);

            IsuImportHistory fakeIsuImportHisotry = fixture.Generate<IsuImportHistory>();
            fakeIsuImportHisotry.Id = random.Next(1, 10);
            mockCitiService.Setup(f => f.InsertImportHistory(fakeVendorRecord)).Returns(fakeIsuImportHisotry);

            History fakeHistory1 = null;
            mockLearnerWebServices.Setup(f => f.GetHistoryByCurriculaId(fakeVendorRecord)).Returns(fakeHistory1);

            History fakeHistory2 = null;
            mockCitiService.Setup(f => f.IsRecordVerified(fakeVendorRecord, out fakeHistory2)).Returns(false);

            string fakeUnivId = fixture.Generate<string>();
            mockCitiService.Setup(f => f.FindUser(fakeVendorRecord)).Returns(fakeUnivId);

            string fakeCourseId = fixture.Generate<string>();
            mockCitiService.Setup(f => f.FindCourse(fakeVendorRecord)).Returns(fakeCourseId);

            History fakeHistory3 = fixture.Generate<History>();
            mockLearnerWebServices.Setup(f => f.GetHistoryByVendorIdCourseIdDate(fakeVendorRecord)).Returns(fakeHistory3);

            applicationConfiguration.processType = CommandLineConfiguration.ProcessType.Insert;

            // Execute
            ITrainingService trainingService = new TrainingService(mockCitiService.Object, mockLearnerWebServices.Object, mockLogService.Object, mockReportingService.Object, applicationConfiguration, mockMailService.Object);
            trainingService.ProcessRecords();

            // Verify
            mockCitiService.Verify(f => f.GetRecords(), Times.Once);
            mockCitiService.Verify(f => f.UploadHistoryRecords(It.IsAny<List<History>>()), Times.Never);
            mockCitiService.Verify(f => f.InsertImportHistory(It.IsAny<VendorRecord>()), Times.Once);
            mockCitiService.Verify(f => f.IsRecordVerified(It.IsAny<VendorRecord>(), out fakeHistory2), Times.Once);
            mockCitiService.Verify(f => f.FindUser(It.IsAny<VendorRecord>()), Times.Once);
            mockCitiService.Verify(f => f.FindCourse(It.IsAny<VendorRecord>()), Times.Once);
            mockCitiService.VerifyNoOtherCalls();
            mockLearnerWebServices.Verify(f => f.GetHistoryByCurriculaId(It.IsAny<VendorRecord>()), Times.Once);
            mockLearnerWebServices.Verify(f => f.GetHistoryByVendorIdCourseIdDate(It.IsAny<VendorRecord>()), Times.Once);
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
            List<VendorRecord> fakeVendorRecords = new List<VendorRecord>();
            VendorRecord fakeVendorRecord = CreateValidFakeRecord();
            fakeVendorRecords.Add(fakeVendorRecord);
            mockCitiService.Setup(f => f.GetRecords()).Returns(fakeVendorRecords);

            IsuImportHistory fakeIsuImportHisotry = fixture.Generate<IsuImportHistory>();
            fakeIsuImportHisotry.Id = random.Next(1, 10);
            mockCitiService.Setup(f => f.InsertImportHistory(fakeVendorRecord)).Returns(fakeIsuImportHisotry);

            History fakeHistory1 = null;
            mockLearnerWebServices.Setup(f => f.GetHistoryByCurriculaId(fakeVendorRecord)).Returns(fakeHistory1);

            History fakeHistory2 = null;
            mockCitiService.Setup(f => f.IsRecordVerified(fakeVendorRecord, out fakeHistory2)).Returns(false);

            string fakeUnivId = fixture.Generate<string>();
            mockCitiService.Setup(f => f.FindUser(fakeVendorRecord)).Returns(fakeUnivId);

            string fakeCourseId = fixture.Generate<string>();
            mockCitiService.Setup(f => f.FindCourse(fakeVendorRecord)).Returns(fakeCourseId);

            History fakeHistory3 = fixture.Generate<History>();
            mockLearnerWebServices.Setup(f => f.GetHistoryByVendorIdCourseIdDate(fakeVendorRecord)).Returns(fakeHistory3);

            applicationConfiguration.processType = CommandLineConfiguration.ProcessType.Upload;

            // Execute
            ITrainingService trainingService = new TrainingService(mockCitiService.Object, mockLearnerWebServices.Object, mockLogService.Object, mockReportingService.Object, applicationConfiguration, mockMailService.Object);
            trainingService.ProcessRecords();

            // Verify
            mockCitiService.Verify(f => f.GetRecords(), Times.Once);
            mockCitiService.Verify(f => f.UploadHistoryRecords(It.IsAny<List<History>>()), Times.Once);
            mockCitiService.Verify(f => f.InsertImportHistory(It.IsAny<VendorRecord>()), Times.Once);
            mockCitiService.Verify(f => f.IsRecordVerified(It.IsAny<VendorRecord>(), out fakeHistory2), Times.Once);
            mockCitiService.Verify(f => f.FindUser(It.IsAny<VendorRecord>()), Times.Once);
            mockCitiService.Verify(f => f.FindCourse(It.IsAny<VendorRecord>()), Times.Once);
            mockCitiService.VerifyNoOtherCalls();
            mockLearnerWebServices.Verify(f => f.GetHistoryByCurriculaId(It.IsAny<VendorRecord>()), Times.Once);
            mockLearnerWebServices.Verify(f => f.GetHistoryByVendorIdCourseIdDate(It.IsAny<VendorRecord>()), Times.Once);
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
            List<VendorRecord> fakeVendorRecords = new List<VendorRecord>();
            VendorRecord fakeVendorRecord = CreateValidFakeRecord();
            fakeVendorRecords.Add(fakeVendorRecord);
            mockCitiService.Setup(f => f.GetRecords()).Returns(fakeVendorRecords);

            IsuImportHistory fakeIsuImportHisotry = fixture.Generate<IsuImportHistory>();
            fakeIsuImportHisotry.Id = random.Next(1, 10);
            mockCitiService.Setup(f => f.InsertImportHistory(fakeVendorRecord)).Returns(fakeIsuImportHisotry);

            History fakeHistory1 = null;
            mockLearnerWebServices.Setup(f => f.GetHistoryByCurriculaId(fakeVendorRecord)).Returns(fakeHistory1);

            History fakeHistory2 = null;
            mockCitiService.Setup(f => f.IsRecordVerified(fakeVendorRecord, out fakeHistory2)).Returns(false);

            string fakeUnivId = fixture.Generate<string>();
            mockCitiService.Setup(f => f.FindUser(fakeVendorRecord)).Returns(fakeUnivId);

            string fakeCourseId = fixture.Generate<string>();
            mockCitiService.Setup(f => f.FindCourse(fakeVendorRecord)).Returns(fakeCourseId);

            History fakeHistory3 = null;
            mockLearnerWebServices.Setup(f => f.GetHistoryByVendorIdCourseIdDate(fakeVendorRecord)).Returns(fakeHistory3);

            History fakeHistory4 = fixture.Generate<History>();
            mockLearnerWebServices.Setup(f => f.CreateHistoryRecord(fakeVendorRecord)).Returns(fakeHistory4);
            mockCitiService.Setup(f => f.InsertSingleHistoryRecord(fakeHistory4)).Verifiable();
            mockLearnerWebServices.Setup(f => f.SetInsertedForImportRecord(fakeIsuImportHisotry.Id)).Verifiable();

            applicationConfiguration.processType = CommandLineConfiguration.ProcessType.Upload;

            // Execute
            ITrainingService trainingService = new TrainingService(mockCitiService.Object, mockLearnerWebServices.Object, mockLogService.Object, mockReportingService.Object, applicationConfiguration, mockMailService.Object);
            trainingService.ProcessRecords();

            // Verify
            mockCitiService.Verify(f => f.GetRecords(), Times.Once);
            mockCitiService.Verify(f => f.UploadHistoryRecords(It.IsAny<List<History>>()), Times.Once);
            mockCitiService.Verify(f => f.InsertImportHistory(It.IsAny<VendorRecord>()), Times.Once);
            mockCitiService.Verify(f => f.IsRecordVerified(It.IsAny<VendorRecord>(), out fakeHistory2), Times.Once);
            mockCitiService.Verify(f => f.FindUser(It.IsAny<VendorRecord>()), Times.Once);
            mockCitiService.Verify(f => f.FindCourse(It.IsAny<VendorRecord>()), Times.Once);
            mockCitiService.Verify(f => f.InsertSingleHistoryRecord(It.IsAny<History>()), Times.Once);
            mockCitiService.VerifyNoOtherCalls();
            mockLearnerWebServices.Verify(f => f.GetHistoryByCurriculaId(It.IsAny<VendorRecord>()), Times.Once);
            mockLearnerWebServices.Verify(f => f.GetHistoryByVendorIdCourseIdDate(It.IsAny<VendorRecord>()), Times.Once);
            mockLearnerWebServices.Verify(f => f.CreateHistoryRecord(It.IsAny<VendorRecord>()), Times.Once);
            mockLearnerWebServices.Verify(f => f.CreateHistoryRecord(It.IsAny<VendorRecord>()), Times.Once);
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
            List<VendorRecord> fakeVendorRecords = new List<VendorRecord>();
            VendorRecord fakeVendorRecord = CreateValidFakeRecord();
            fakeVendorRecords.Add(fakeVendorRecord);
            mockCitiService.Setup(f => f.GetRecords()).Returns(fakeVendorRecords);

            IsuImportHistory fakeIsuImportHisotry = fixture.Generate<IsuImportHistory>();
            fakeIsuImportHisotry.Id = random.Next(1, 10);
            mockCitiService.Setup(f => f.InsertImportHistory(fakeVendorRecord)).Returns(fakeIsuImportHisotry);

            History fakeHistory1 = null;
            mockLearnerWebServices.Setup(f => f.GetHistoryByCurriculaId(fakeVendorRecord)).Returns(fakeHistory1);

            History fakeHistory2 = null;
            mockCitiService.Setup(f => f.IsRecordVerified(fakeVendorRecord, out fakeHistory2)).Returns(false);

            string fakeUnivId = fixture.Generate<string>();
            mockCitiService.Setup(f => f.FindUser(fakeVendorRecord)).Returns(fakeUnivId);

            string fakeCourseId = fixture.Generate<string>();
            mockCitiService.Setup(f => f.FindCourse(fakeVendorRecord)).Returns(fakeCourseId);

            History fakeHistory3 = null;
            mockLearnerWebServices.Setup(f => f.GetHistoryByVendorIdCourseIdDate(fakeVendorRecord)).Returns(fakeHistory3);

            History fakeHistory4 = fixture.Generate<History>();
            mockLearnerWebServices.Setup(f => f.CreateHistoryRecord(fakeVendorRecord)).Throws(new Exception());
            //mockCitiService.Setup(f => f.InsertSingleHistoryRecord(fakeHistory4)).Verifiable();
            //mockLearnerWebServices.Setup(f => f.SetInsertedForImportRecord(fakeIsuImportHisotry.Id)).Verifiable();

            applicationConfiguration.processType = CommandLineConfiguration.ProcessType.Upload;

            // Execute
            ITrainingService trainingService = new TrainingService(mockCitiService.Object, mockLearnerWebServices.Object, mockLogService.Object, mockReportingService.Object, applicationConfiguration, mockMailService.Object);
            trainingService.ProcessRecords();

            // Verify
            mockCitiService.Verify(f => f.GetRecords(), Times.Once);
            mockCitiService.Verify(f => f.UploadHistoryRecords(It.IsAny<List<History>>()), Times.Never);
            mockCitiService.Verify(f => f.InsertImportHistory(It.IsAny<VendorRecord>()), Times.Once);
            mockCitiService.Verify(f => f.IsRecordVerified(It.IsAny<VendorRecord>(), out fakeHistory2), Times.Once);
            mockCitiService.Verify(f => f.FindUser(It.IsAny<VendorRecord>()), Times.Once);
            mockCitiService.Verify(f => f.FindCourse(It.IsAny<VendorRecord>()), Times.Once);
            mockCitiService.Verify(f => f.InsertSingleHistoryRecord(It.IsAny<History>()), Times.Never);
            mockCitiService.VerifyNoOtherCalls();
            mockLearnerWebServices.Verify(f => f.GetHistoryByCurriculaId(It.IsAny<VendorRecord>()), Times.Once);
            mockLearnerWebServices.Verify(f => f.GetHistoryByVendorIdCourseIdDate(It.IsAny<VendorRecord>()), Times.Once);
            mockLearnerWebServices.Verify(f => f.CreateHistoryRecord(It.IsAny<VendorRecord>()), Times.Once);
            mockLearnerWebServices.Verify(f => f.CreateHistoryRecord(It.IsAny<VendorRecord>()), Times.Once);
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
            List<VendorRecord> fakeVendorRecords = new List<VendorRecord>();
            VendorRecord fakeVendorRecord = CreateValidFakeRecord();
            fakeVendorRecords.Add(fakeVendorRecord);
            mockCitiService.Setup(f => f.GetRecords()).Returns(fakeVendorRecords);

            IsuImportHistory fakeIsuImportHisotry = fixture.Generate<IsuImportHistory>();
            fakeIsuImportHisotry.Id = random.Next(1, 10);
            mockCitiService.Setup(f => f.InsertImportHistory(fakeVendorRecord)).Returns(fakeIsuImportHisotry);

            History fakeHistory1 = null;
            mockLearnerWebServices.Setup(f => f.GetHistoryByCurriculaId(fakeVendorRecord)).Returns(fakeHistory1);

            History fakeHistory2 = null;
            mockCitiService.Setup(f => f.IsRecordVerified(fakeVendorRecord, out fakeHistory2)).Returns(false);

            string fakeUnivId = fixture.Generate<string>();
            mockCitiService.Setup(f => f.FindUser(fakeVendorRecord)).Returns(fakeUnivId);

            string fakeCourseId = fixture.Generate<string>();
            mockCitiService.Setup(f => f.FindCourse(fakeVendorRecord)).Returns(fakeCourseId);

            History fakeHistory3 = null;
            mockLearnerWebServices.Setup(f => f.GetHistoryByVendorIdCourseIdDate(fakeVendorRecord)).Returns(fakeHistory3);

            History fakeHistory4 = fixture.Generate<History>();
            mockLearnerWebServices.Setup(f => f.CreateHistoryRecord(fakeVendorRecord)).Returns(fakeHistory4);
            mockCitiService.Setup(f => f.InsertSingleHistoryRecord(fakeHistory4)).Throws(new Exception());
            //mockLearnerWebServices.Setup(f => f.SetInsertedForImportRecord(fakeIsuImportHisotry.Id)).Verifiable();

            applicationConfiguration.processType = CommandLineConfiguration.ProcessType.Upload;

            // Execute
            ITrainingService trainingService = new TrainingService(mockCitiService.Object, mockLearnerWebServices.Object, mockLogService.Object, mockReportingService.Object, applicationConfiguration, mockMailService.Object);
            trainingService.ProcessRecords();

            // Verify
            mockCitiService.Verify(f => f.GetRecords(), Times.Once);
            mockCitiService.Verify(f => f.UploadHistoryRecords(It.IsAny<List<History>>()), Times.Once);
            mockCitiService.Verify(f => f.InsertImportHistory(It.IsAny<VendorRecord>()), Times.Once);
            mockCitiService.Verify(f => f.IsRecordVerified(It.IsAny<VendorRecord>(), out fakeHistory2), Times.Once);
            mockCitiService.Verify(f => f.FindUser(It.IsAny<VendorRecord>()), Times.Once);
            mockCitiService.Verify(f => f.FindCourse(It.IsAny<VendorRecord>()), Times.Once);
            mockCitiService.Verify(f => f.InsertSingleHistoryRecord(It.IsAny<History>()), Times.Once);
            mockCitiService.VerifyNoOtherCalls();
            mockLearnerWebServices.Verify(f => f.GetHistoryByCurriculaId(It.IsAny<VendorRecord>()), Times.Once);
            mockLearnerWebServices.Verify(f => f.GetHistoryByVendorIdCourseIdDate(It.IsAny<VendorRecord>()), Times.Once);
            mockLearnerWebServices.Verify(f => f.CreateHistoryRecord(It.IsAny<VendorRecord>()), Times.Once);
            mockLearnerWebServices.Verify(f => f.CreateHistoryRecord(It.IsAny<VendorRecord>()), Times.Once);
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
            List<VendorRecord> fakeVendorRecords = new List<VendorRecord>();
            VendorRecord fakeVendorRecord = CreateValidFakeRecord();
            fakeVendorRecords.Add(fakeVendorRecord);
            mockCitiService.Setup(f => f.GetRecords()).Returns(fakeVendorRecords);

            IsuImportHistory fakeIsuImportHisotry = fixture.Generate<IsuImportHistory>();
            fakeIsuImportHisotry.Id = random.Next(1, 10);
            mockCitiService.Setup(f => f.InsertImportHistory(fakeVendorRecord)).Returns(fakeIsuImportHisotry);

            History fakeHistory1 = null;
            mockLearnerWebServices.Setup(f => f.GetHistoryByCurriculaId(fakeVendorRecord)).Returns(fakeHistory1);

            History fakeHistory2 = null;
            mockCitiService.Setup(f => f.IsRecordVerified(fakeVendorRecord, out fakeHistory2)).Returns(false);

            string fakeUnivId = fixture.Generate<string>();
            mockCitiService.Setup(f => f.FindUser(fakeVendorRecord)).Returns(fakeUnivId);

            string fakeCourseId = fixture.Generate<string>();
            mockCitiService.Setup(f => f.FindCourse(fakeVendorRecord)).Returns(fakeCourseId);

            History fakeHistory3 = null;
            mockLearnerWebServices.Setup(f => f.GetHistoryByVendorIdCourseIdDate(fakeVendorRecord)).Returns(fakeHistory3);

            History fakeHistory4 = fixture.Generate<History>();
            mockLearnerWebServices.Setup(f => f.CreateHistoryRecord(fakeVendorRecord)).Returns(fakeHistory4);
            mockCitiService.Setup(f => f.InsertSingleHistoryRecord(fakeHistory4)).Verifiable();
            mockLearnerWebServices.Setup(f => f.SetInsertedForImportRecord(fakeIsuImportHisotry.Id)).Throws(new Exception());

            applicationConfiguration.processType = CommandLineConfiguration.ProcessType.Upload;

            // Execute
            ITrainingService trainingService = new TrainingService(mockCitiService.Object, mockLearnerWebServices.Object, mockLogService.Object, mockReportingService.Object, applicationConfiguration, mockMailService.Object);
            trainingService.ProcessRecords();

            // Verify
            mockCitiService.Verify(f => f.GetRecords(), Times.Once);
            mockCitiService.Verify(f => f.UploadHistoryRecords(It.IsAny<List<History>>()), Times.Once);
            mockCitiService.Verify(f => f.InsertImportHistory(It.IsAny<VendorRecord>()), Times.Once);
            mockCitiService.Verify(f => f.IsRecordVerified(It.IsAny<VendorRecord>(), out fakeHistory2), Times.Once);
            mockCitiService.Verify(f => f.FindUser(It.IsAny<VendorRecord>()), Times.Once);
            mockCitiService.Verify(f => f.FindCourse(It.IsAny<VendorRecord>()), Times.Once);
            mockCitiService.Verify(f => f.InsertSingleHistoryRecord(It.IsAny<History>()), Times.Once);
            mockCitiService.VerifyNoOtherCalls();
            mockLearnerWebServices.Verify(f => f.GetHistoryByCurriculaId(It.IsAny<VendorRecord>()), Times.Once);
            mockLearnerWebServices.Verify(f => f.GetHistoryByVendorIdCourseIdDate(It.IsAny<VendorRecord>()), Times.Once);
            mockLearnerWebServices.Verify(f => f.CreateHistoryRecord(It.IsAny<VendorRecord>()), Times.Once);
            mockLearnerWebServices.Verify(f => f.CreateHistoryRecord(It.IsAny<VendorRecord>()), Times.Once);
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
            List<VendorRecord> fakeVendorRecords = new List<VendorRecord>();
            VendorRecord fakeVendorRecord = CreateValidFakeRecord();
            fakeVendorRecords.Add(fakeVendorRecord);
            mockCitiService.Setup(f => f.GetRecords()).Returns(fakeVendorRecords);

            IsuImportHistory fakeIsuImportHisotry = fixture.Generate<IsuImportHistory>();
            fakeIsuImportHisotry.Id = random.Next(1, 10);
            mockCitiService.Setup(f => f.InsertImportHistory(fakeVendorRecord)).Returns(fakeIsuImportHisotry);

            History fakeHistory1 = null;
            mockLearnerWebServices.Setup(f => f.GetHistoryByCurriculaId(fakeVendorRecord)).Returns(fakeHistory1);

            History fakeHistory2 = null;
            mockCitiService.Setup(f => f.IsRecordVerified(fakeVendorRecord, out fakeHistory2)).Returns(false);

            string fakeUnivId = fixture.Generate<string>();
            mockCitiService.Setup(f => f.FindUser(fakeVendorRecord)).Returns(fakeUnivId);

            string fakeCourseId = fixture.Generate<string>();
            mockCitiService.Setup(f => f.FindCourse(fakeVendorRecord)).Returns(fakeCourseId);

            History fakeHistory3 = null;
            mockLearnerWebServices.Setup(f => f.GetHistoryByVendorIdCourseIdDate(fakeVendorRecord)).Returns(fakeHistory3);

            History fakeHistory4 = fixture.Generate<History>();
            mockLearnerWebServices.Setup(f => f.CreateHistoryRecord(fakeVendorRecord)).Returns(fakeHistory4);
            mockCitiService.Setup(f => f.InsertSingleHistoryRecord(fakeHistory4)).Verifiable();
            mockLearnerWebServices.Setup(f => f.SetInsertedForImportRecord(fakeIsuImportHisotry.Id)).Verifiable();

            applicationConfiguration.processType = CommandLineConfiguration.ProcessType.Insert;

            // Execute
            ITrainingService trainingService = new TrainingService(mockCitiService.Object, mockLearnerWebServices.Object, mockLogService.Object, mockReportingService.Object, applicationConfiguration, mockMailService.Object);
            trainingService.ProcessRecords();

            // Verify
            mockCitiService.Verify(f => f.GetRecords(), Times.Once);
            mockCitiService.Verify(f => f.UploadHistoryRecords(It.IsAny<List<History>>()), Times.Never);
            mockCitiService.Verify(f => f.InsertImportHistory(It.IsAny<VendorRecord>()), Times.Once);
            mockCitiService.Verify(f => f.IsRecordVerified(It.IsAny<VendorRecord>(), out fakeHistory2), Times.Once);
            mockCitiService.Verify(f => f.FindUser(It.IsAny<VendorRecord>()), Times.Once);
            mockCitiService.Verify(f => f.FindCourse(It.IsAny<VendorRecord>()), Times.Once);
            mockCitiService.Verify(f => f.InsertSingleHistoryRecord(It.IsAny<History>()), Times.Once);
            mockCitiService.VerifyNoOtherCalls();
            mockLearnerWebServices.Verify(f => f.GetHistoryByCurriculaId(It.IsAny<VendorRecord>()), Times.Once);
            mockLearnerWebServices.Verify(f => f.GetHistoryByVendorIdCourseIdDate(It.IsAny<VendorRecord>()), Times.Once);
            mockLearnerWebServices.Verify(f => f.CreateHistoryRecord(It.IsAny<VendorRecord>()), Times.Once);
            mockLearnerWebServices.Verify(f => f.CreateHistoryRecord(It.IsAny<VendorRecord>()), Times.Once);
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
            List<VendorRecord> fakeVendorRecords = new List<VendorRecord>();
            VendorRecord fakeVendorRecord = CreateValidFakeRecord();
            fakeVendorRecords.Add(fakeVendorRecord);
            mockCitiService.Setup(f => f.GetRecords()).Returns(fakeVendorRecords);

            IsuImportHistory fakeIsuImportHisotry = fixture.Generate<IsuImportHistory>();
            fakeIsuImportHisotry.Id = random.Next(1, 10);
            mockCitiService.Setup(f => f.InsertImportHistory(fakeVendorRecord)).Returns(fakeIsuImportHisotry);

            History fakeHistory1 = null;
            mockLearnerWebServices.Setup(f => f.GetHistoryByCurriculaId(fakeVendorRecord)).Returns(fakeHistory1);

            History fakeHistory2 = null;
            mockCitiService.Setup(f => f.IsRecordVerified(fakeVendorRecord, out fakeHistory2)).Returns(false);

            string fakeUnivId = fixture.Generate<string>();
            mockCitiService.Setup(f => f.FindUser(fakeVendorRecord)).Returns(fakeUnivId);

            string fakeCourseId = fixture.Generate<string>();
            mockCitiService.Setup(f => f.FindCourse(fakeVendorRecord)).Returns(fakeCourseId);

            History fakeHistory3 = null;
            mockLearnerWebServices.Setup(f => f.GetHistoryByVendorIdCourseIdDate(fakeVendorRecord)).Returns(fakeHistory3);

            History fakeHistory4 = fixture.Generate<History>();
            mockLearnerWebServices.Setup(f => f.CreateHistoryRecord(fakeVendorRecord)).Returns(fakeHistory4);
            mockCitiService.Setup(f => f.InsertSingleHistoryRecord(fakeHistory4)).Verifiable();
            mockLearnerWebServices.Setup(f => f.SetInsertedForImportRecord(fakeIsuImportHisotry.Id)).Verifiable();

            mockMailService.Setup(f => f.SendMessages()).Throws(new SendMailException("test"));

            applicationConfiguration.processType = CommandLineConfiguration.ProcessType.Insert;

            // Execute
            ITrainingService trainingService = new TrainingService(mockCitiService.Object, mockLearnerWebServices.Object, mockLogService.Object, mockReportingService.Object, applicationConfiguration, mockMailService.Object);
            trainingService.ProcessRecords();

            // Verify
            mockCitiService.Verify(f => f.GetRecords(), Times.Once);
            mockCitiService.Verify(f => f.UploadHistoryRecords(It.IsAny<List<History>>()), Times.Never);
            mockCitiService.Verify(f => f.InsertImportHistory(It.IsAny<VendorRecord>()), Times.Once);
            mockCitiService.Verify(f => f.IsRecordVerified(It.IsAny<VendorRecord>(), out fakeHistory2), Times.Once);
            mockCitiService.Verify(f => f.FindUser(It.IsAny<VendorRecord>()), Times.Once);
            mockCitiService.Verify(f => f.FindCourse(It.IsAny<VendorRecord>()), Times.Once);
            mockCitiService.Verify(f => f.InsertSingleHistoryRecord(It.IsAny<History>()), Times.Once);
            mockCitiService.VerifyNoOtherCalls();
            mockLearnerWebServices.Verify(f => f.GetHistoryByCurriculaId(It.IsAny<VendorRecord>()), Times.Once);
            mockLearnerWebServices.Verify(f => f.GetHistoryByVendorIdCourseIdDate(It.IsAny<VendorRecord>()), Times.Once);
            mockLearnerWebServices.Verify(f => f.CreateHistoryRecord(It.IsAny<VendorRecord>()), Times.Once);
            mockLearnerWebServices.Verify(f => f.CreateHistoryRecord(It.IsAny<VendorRecord>()), Times.Once);
            mockLearnerWebServices.Verify(f => f.SetInsertedForImportRecord(It.IsAny<int>()), Times.Once);
            mockLearnerWebServices.VerifyNoOtherCalls();
            mockMailService.Verify(f => f.SendMessages(), Times.Once);
            mockMailService.VerifyNoOtherCalls();
            Mock.VerifyAll(mockCitiService);
            Mock.VerifyAll(mockLearnerWebServices);
        }

    }
}
