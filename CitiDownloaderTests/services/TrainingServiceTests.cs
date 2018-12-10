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
using CitiDownloader.repositories;
using CitiDownloader.models.entities;
using CitiDownloader.exceptions;
using static CitiDownloader.models.ProcessTypeEnum;

namespace CitiDownloaderTests.services
{
    [TestFixture]
    public class TrainingServiceTests
    {
        private Mock<ICitiService> mockCitiService;
        private Mock<ILearnerWebServices> mockLearnerWebServices;
        private Mock<ICsvWrapper> mockCsvWrapper;
        private Mock<ISftpWrapper> mockSftpWrapper;
        private Mock<ILog> mockLog;
        private Mock<IReportingService> mockReportingService;
        private Fixture fixture;

        public TrainingServiceTests()
        {
            fixture = new Fixture();
        }

        private void SetupMocks()
        {
            mockCitiService = new Mock<ICitiService>();
            mockLearnerWebServices = new Mock<ILearnerWebServices>();
            mockCsvWrapper = new Mock<ICsvWrapper>();
            mockSftpWrapper = new Mock<ISftpWrapper>();
            mockLog = new Mock<ILog>();
            mockReportingService = new Mock<IReportingService>();
        }

        [Test]
        public void InsertHistoryRecordsTest()
        {
            // Setup
            SetupMocks();

            List<History> fakeHistories = fixture.Generate<List<History>>();
            bool insertBoolResponse = true;
            mockLearnerWebServices.Setup(f => f.InsertHistory(It.IsAny<History>(), out insertBoolResponse)).Verifiable();

            // Execute
            ITrainingService trainingService = new TrainingService(mockCitiService.Object, mockLearnerWebServices.Object, mockCsvWrapper.Object, mockSftpWrapper.Object, mockLog.Object, mockReportingService.Object);
            trainingService.InsertHistoryRecords(fakeHistories);

            // Verify
            mockLearnerWebServices.Verify(f => f.InsertHistory(It.IsAny<History>(), out insertBoolResponse), Times.Exactly(fakeHistories.Count));
            Mock.VerifyAll(mockLearnerWebServices);
        }

        [Test]
        public void InsertHistoryRecordsThrowsExceptionTest()
        {
            // Setup
            SetupMocks();

            List<History> fakeHistories = fixture.Generate<List<History>>();
            bool insertBoolResponse = true;
            mockLearnerWebServices.Setup(f => f.InsertHistory(It.IsAny<History>(), out insertBoolResponse)).Throws(new Exception());

            // Execute
            ITrainingService trainingService = new TrainingService(mockCitiService.Object, mockLearnerWebServices.Object, mockCsvWrapper.Object, mockSftpWrapper.Object, mockLog.Object, mockReportingService.Object);

            // Execute & Verify
            Assert.Throws<Exception>(delegate { trainingService.InsertHistoryRecords(fakeHistories); });

            // Verify
            mockLearnerWebServices.Verify(f => f.InsertHistory(It.IsAny<History>(), out insertBoolResponse), Times.Once);
        }

        [Test]
        public void ProcessRecordsFullSuccessTest()
        {
            // Setup
            SetupMocks();

            ProcessType processType = ProcessType.Full;

            List<CitiRecord> fakeCitiRecords = fixture.Generate<List<CitiRecord>>();
            mockCitiService.Setup(f => f.GetFullRecords()).Returns(fakeCitiRecords);

            mockLearnerWebServices.Setup(f => f.InsertImportHistory(It.IsAny<CitiRecord>())).Verifiable();

            mockLearnerWebServices.Setup(f => f.IsVerified(It.IsAny<CitiRecord>())).Returns(false);

            mockLearnerWebServices.Setup(f => f.FindUser(It.IsAny<CitiRecord>())).Returns(fixture.Generate<string>());

            mockLearnerWebServices.Setup(f => f.FindCourseId(It.IsAny<CitiRecord>())).Returns(fixture.Generate<string>());

            mockLearnerWebServices.Setup(f => f.CreateHistoryRecord(It.IsAny<CitiRecord>())).Returns(fixture.Generate<History>());

            // Execute
            ITrainingService trainingService = new TrainingService(mockCitiService.Object, mockLearnerWebServices.Object, mockCsvWrapper.Object, mockSftpWrapper.Object, mockLog.Object, mockReportingService.Object);
            List<History> response = trainingService.ProcessRecords(processType);

            // Verify
            Assert.That(response.Count == fakeCitiRecords.Count);
            mockCitiService.Verify(f => f.GetFullRecords(), Times.Once);
            mockLearnerWebServices.Verify(f => f.InsertImportHistory(It.IsAny<CitiRecord>()), Times.Exactly(fakeCitiRecords.Count));
            mockLearnerWebServices.Verify(f => f.IsVerified(It.IsAny<CitiRecord>()), Times.Exactly(fakeCitiRecords.Count));
            mockLearnerWebServices.Verify(f => f.FindUser(It.IsAny<CitiRecord>()), Times.Exactly(fakeCitiRecords.Count));
            mockLearnerWebServices.Verify(f => f.FindCourseId(It.IsAny<CitiRecord>()), Times.Exactly(fakeCitiRecords.Count));
            mockLearnerWebServices.Verify(f => f.CreateHistoryRecord(It.IsAny<CitiRecord>()), Times.Exactly(fakeCitiRecords.Count));
            Mock.VerifyAll(mockLearnerWebServices);
        }

        [Test]
        public void ProcessRecordsFullWithInvalidUserRecordsTest()
        {
            // Setup
            SetupMocks();

            Random random = new Random();

            ProcessType processType = ProcessType.Full;

            List<CitiRecord> fakeCitiRecords = fixture.Generate<List<CitiRecord>>();
            
            mockCitiService.Setup(f => f.GetFullRecords()).Returns(fakeCitiRecords);

            mockLearnerWebServices.Setup(f => f.InsertImportHistory(It.IsAny<CitiRecord>())).Verifiable();

            mockLearnerWebServices.Setup(f => f.IsVerified(It.IsAny<CitiRecord>())).Returns(false);

            mockLearnerWebServices.Setup(f => f.FindUser(It.IsAny<CitiRecord>())).Returns(fixture.Generate<string>());
            int invalidUserCount = random.Next(10);
            for (int i = 0; i < invalidUserCount; i++)
            {
                CitiRecord errorCitiRecord = fixture.Generate<CitiRecord>();
                fakeCitiRecords.Add(errorCitiRecord);
                mockLearnerWebServices.Setup(f => f.FindUser(errorCitiRecord)).Throws(new InvalidUserException("test"));
            }         

            mockLearnerWebServices.Setup(f => f.FindCourseId(It.IsAny<CitiRecord>())).Returns(fixture.Generate<string>());

            mockLearnerWebServices.Setup(f => f.CreateHistoryRecord(It.IsAny<CitiRecord>())).Returns(fixture.Generate<History>());

            // Execute
            ITrainingService trainingService = new TrainingService(mockCitiService.Object, mockLearnerWebServices.Object, mockCsvWrapper.Object, mockSftpWrapper.Object, mockLog.Object, mockReportingService.Object);
            List<History> response = trainingService.ProcessRecords(processType);

            // Verify
            Assert.That(response.Count == fakeCitiRecords.Count - invalidUserCount);
            mockCitiService.Verify(f => f.GetFullRecords(), Times.Once);
            mockLearnerWebServices.Verify(f => f.InsertImportHistory(It.IsAny<CitiRecord>()), Times.Exactly(fakeCitiRecords.Count));
            mockLearnerWebServices.Verify(f => f.IsVerified(It.IsAny<CitiRecord>()), Times.Exactly(fakeCitiRecords.Count));
            mockLearnerWebServices.Verify(f => f.FindUser(It.IsAny<CitiRecord>()), Times.Exactly(fakeCitiRecords.Count));
            mockLearnerWebServices.Verify(f => f.FindCourseId(It.IsAny<CitiRecord>()), Times.Exactly(fakeCitiRecords.Count - invalidUserCount));
            mockLearnerWebServices.Verify(f => f.CreateHistoryRecord(It.IsAny<CitiRecord>()), Times.Exactly(fakeCitiRecords.Count - invalidUserCount));
            Mock.VerifyAll(mockLearnerWebServices);
        }

        [Test]
        public void ProcessRecordsFullWithUnknownUserRecordsTest()
        {
            // Setup
            SetupMocks();

            Random random = new Random();

            ProcessType processType = ProcessType.Full;

            List<CitiRecord> fakeCitiRecords = fixture.Generate<List<CitiRecord>>();

            mockCitiService.Setup(f => f.GetFullRecords()).Returns(fakeCitiRecords);

            mockLearnerWebServices.Setup(f => f.InsertImportHistory(It.IsAny<CitiRecord>())).Verifiable();

            mockLearnerWebServices.Setup(f => f.IsVerified(It.IsAny<CitiRecord>())).Returns(false);

            mockLearnerWebServices.Setup(f => f.FindUser(It.IsAny<CitiRecord>())).Returns(fixture.Generate<string>());
            int unknownUserCount = random.Next(10);
            for (int i = 0; i < unknownUserCount; i++)
            {
                CitiRecord errorCitiRecord = fixture.Generate<CitiRecord>();
                fakeCitiRecords.Add(errorCitiRecord);
                mockLearnerWebServices.Setup(f => f.FindUser(errorCitiRecord)).Throws(new UnknownUserException("test"));
            }

            mockReportingService.Setup(f => f.ReportUnknownUser(It.IsAny<CitiRecord>(), It.IsAny<List<string>>())).Verifiable();

            mockLearnerWebServices.Setup(f => f.FindCourseId(It.IsAny<CitiRecord>())).Returns(fixture.Generate<string>());

            mockLearnerWebServices.Setup(f => f.CreateHistoryRecord(It.IsAny<CitiRecord>())).Returns(fixture.Generate<History>());

            // Execute
            ITrainingService trainingService = new TrainingService(mockCitiService.Object, mockLearnerWebServices.Object, mockCsvWrapper.Object, mockSftpWrapper.Object, mockLog.Object, mockReportingService.Object);
            List<History> response = trainingService.ProcessRecords(processType);

            // Verify
            Assert.That(response.Count == fakeCitiRecords.Count - unknownUserCount);
            mockCitiService.Verify(f => f.GetFullRecords(), Times.Once);
            mockLearnerWebServices.Verify(f => f.InsertImportHistory(It.IsAny<CitiRecord>()), Times.Exactly(fakeCitiRecords.Count));
            mockLearnerWebServices.Verify(f => f.IsVerified(It.IsAny<CitiRecord>()), Times.Exactly(fakeCitiRecords.Count));
            mockLearnerWebServices.Verify(f => f.FindUser(It.IsAny<CitiRecord>()), Times.Exactly(fakeCitiRecords.Count));
            mockLearnerWebServices.Verify(f => f.FindCourseId(It.IsAny<CitiRecord>()), Times.Exactly(fakeCitiRecords.Count - unknownUserCount));
            mockLearnerWebServices.Verify(f => f.CreateHistoryRecord(It.IsAny<CitiRecord>()), Times.Exactly(fakeCitiRecords.Count - unknownUserCount));
            mockReportingService.Verify(f => f.ReportUnknownUser(It.IsAny<CitiRecord>(), It.IsAny<List<string>>()), Times.Exactly(unknownUserCount));
            Mock.VerifyAll(mockLearnerWebServices);
        }

        [Test]
        public void ProcessRecordsFullWithUnknownCourseRecordsTest()
        {
            // Setup
            SetupMocks();

            Random random = new Random();

            ProcessType processType = ProcessType.Full;

            List<CitiRecord> fakeCitiRecords = fixture.Generate<List<CitiRecord>>();

            mockCitiService.Setup(f => f.GetFullRecords()).Returns(fakeCitiRecords);

            mockLearnerWebServices.Setup(f => f.InsertImportHistory(It.IsAny<CitiRecord>())).Verifiable();

            mockLearnerWebServices.Setup(f => f.IsVerified(It.IsAny<CitiRecord>())).Returns(false);

            mockLearnerWebServices.Setup(f => f.FindUser(It.IsAny<CitiRecord>())).Returns(fixture.Generate<string>());
            int unknownCourseCount = random.Next(10);
            mockLearnerWebServices.Setup(f => f.FindCourseId(It.IsAny<CitiRecord>())).Returns(fixture.Generate<string>());
            for (int i = 0; i < unknownCourseCount; i++)
            {
                CitiRecord errorCitiRecord = fixture.Generate<CitiRecord>();
                fakeCitiRecords.Add(errorCitiRecord);
                mockLearnerWebServices.Setup(f => f.FindCourseId(errorCitiRecord)).Throws(new UnknownCourseException("test"));
            }

            mockReportingService.Setup(f => f.ReportUnknownUser(It.IsAny<CitiRecord>(), It.IsAny<List<string>>())).Verifiable();   

            mockLearnerWebServices.Setup(f => f.CreateHistoryRecord(It.IsAny<CitiRecord>())).Returns(fixture.Generate<History>());

            // Execute
            ITrainingService trainingService = new TrainingService(mockCitiService.Object, mockLearnerWebServices.Object, mockCsvWrapper.Object, mockSftpWrapper.Object, mockLog.Object, mockReportingService.Object);
            List<History> response = trainingService.ProcessRecords(processType);

            // Verify
            Assert.That(response.Count == fakeCitiRecords.Count - unknownCourseCount);
            mockCitiService.Verify(f => f.GetFullRecords(), Times.Once);
            mockLearnerWebServices.Verify(f => f.InsertImportHistory(It.IsAny<CitiRecord>()), Times.Exactly(fakeCitiRecords.Count));
            mockLearnerWebServices.Verify(f => f.IsVerified(It.IsAny<CitiRecord>()), Times.Exactly(fakeCitiRecords.Count));
            mockLearnerWebServices.Verify(f => f.FindUser(It.IsAny<CitiRecord>()), Times.Exactly(fakeCitiRecords.Count));
            mockLearnerWebServices.Verify(f => f.FindCourseId(It.IsAny<CitiRecord>()), Times.Exactly(fakeCitiRecords.Count));
            mockLearnerWebServices.Verify(f => f.CreateHistoryRecord(It.IsAny<CitiRecord>()), Times.Exactly(fakeCitiRecords.Count - unknownCourseCount));
            mockReportingService.Verify(f => f.ReportUnknownCourse(It.IsAny<CitiRecord>(), It.IsAny<List<string>>()), Times.Exactly(unknownCourseCount));
            Mock.VerifyAll(mockLearnerWebServices);
        }

        [Test]
        public void ProcessRecordsGetFullRecordsThrowsExceptionTest()
        {
            // Setup
            SetupMocks();

            ProcessType processType = ProcessType.Full;

            mockCitiService.Setup(f => f.GetFullRecords()).Throws(new Exception());
            mockReportingService.Setup(f => f.ReportSystemError(It.IsAny<SystemError>(), It.IsAny<List<string>>())).Verifiable();

            // Execute
            ITrainingService trainingService = new TrainingService(mockCitiService.Object, mockLearnerWebServices.Object, mockCsvWrapper.Object, mockSftpWrapper.Object, mockLog.Object, mockReportingService.Object);
            List<History> response = trainingService.ProcessRecords(processType);

            // Verify
            Assert.That(response == null);
            mockCitiService.Verify(f => f.GetFullRecords(), Times.Once);
            mockReportingService.Verify(f => f.ReportSystemError(It.IsAny<SystemError>(), It.IsAny<List<string>>()), Times.Once);
            Mock.VerifyAll(mockReportingService);
        }

        [Test]
        public void ProcessRecordsGetIncrementalRecordsThrowsExceptionTest()
        {
            // Setup
            SetupMocks();

            ProcessType processType = ProcessType.Incremental;

            mockCitiService.Setup(f => f.GetIncrementalRecords()).Throws(new Exception());
            mockReportingService.Setup(f => f.ReportSystemError(It.IsAny<SystemError>(), It.IsAny<List<string>>())).Verifiable();

            // Execute
            ITrainingService trainingService = new TrainingService(mockCitiService.Object, mockLearnerWebServices.Object, mockCsvWrapper.Object, mockSftpWrapper.Object, mockLog.Object, mockReportingService.Object);
            List<History> response = trainingService.ProcessRecords(processType);

            // Verify
            Assert.That(response == null);
            mockCitiService.Verify(f => f.GetIncrementalRecords(), Times.Once);
            mockReportingService.Verify(f => f.ReportSystemError(It.IsAny<SystemError>(), It.IsAny<List<string>>()), Times.Once);
            Mock.VerifyAll(mockReportingService);
        }

        [Test]
        public void ProcessRecordsInsertImportHistoryThrowsExceptionTest()
        {
            // Setup
            SetupMocks();

            ProcessType processType = ProcessType.Full;
            Random random = new Random();
            List<CitiRecord> fakeCitiRecords = fixture.Generate<List<CitiRecord>>();
            CitiRecord errorCitiRecord = fixture.Generate<CitiRecord>();
            mockLearnerWebServices.Setup(f => f.InsertImportHistory(errorCitiRecord)).Throws(new Exception());
            fakeCitiRecords.Add(errorCitiRecord);

            mockCitiService.Setup(f => f.GetFullRecords()).Returns(fakeCitiRecords);
            mockReportingService.Setup(f => f.ReportSystemError(It.IsAny<SystemError>(), It.IsAny<List<string>>())).Verifiable();

            // Execute
            ITrainingService trainingService = new TrainingService(mockCitiService.Object, mockLearnerWebServices.Object, mockCsvWrapper.Object, mockSftpWrapper.Object, mockLog.Object, mockReportingService.Object);
            List<History> response = trainingService.ProcessRecords(processType);

            // Verify
            Assert.That(response == null);
            mockCitiService.Verify(f => f.GetFullRecords(), Times.Once);
            mockReportingService.Verify(f => f.ReportSystemError(It.IsAny<SystemError>(), It.IsAny<List<string>>()), Times.Once);
            Mock.VerifyAll(mockReportingService);
        }
    }
}
