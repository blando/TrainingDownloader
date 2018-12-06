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
        }

        [Test]
        public void InsertHistoryRecordsTest()
        {
            // Setup
            SetupMocks();

            List<History> fakeHistories = fixture.Generate<List<History>>();
            mockLearnerWebServices.Setup(f => f.InsertHistory(It.IsAny<History>())).Verifiable();

            // Execute
            ITrainingService trainingService = new TrainingService(mockCitiService.Object, mockLearnerWebServices.Object, mockCsvWrapper.Object, mockSftpWrapper.Object);
            trainingService.InsertHistoryRecords(fakeHistories);

            // Verify
            mockLearnerWebServices.Verify(f => f.InsertHistory(It.IsAny<History>()), Times.Exactly(fakeHistories.Count));
            Mock.VerifyAll(mockLearnerWebServices);
        }

        [Test]
        public void InsertHistoryRecordsThrowsExceptionTest()
        {
            // Setup
            SetupMocks();

            List<History> fakeHistories = fixture.Generate<List<History>>();
            mockLearnerWebServices.Setup(f => f.InsertHistory(It.IsAny<History>())).Throws(new Exception());

            // Execute
            ITrainingService trainingService = new TrainingService(mockCitiService.Object, mockLearnerWebServices.Object, mockCsvWrapper.Object, mockSftpWrapper.Object);

            // Execute & Verify
            Assert.Throws<Exception>(delegate { trainingService.InsertHistoryRecords(fakeHistories); });

            // Verify
            mockLearnerWebServices.Verify(f => f.InsertHistory(It.IsAny<History>()), Times.Once);
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
            ITrainingService trainingService = new TrainingService(mockCitiService.Object, mockLearnerWebServices.Object, mockCsvWrapper.Object, mockSftpWrapper.Object);
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
            ITrainingService trainingService = new TrainingService(mockCitiService.Object, mockLearnerWebServices.Object, mockCsvWrapper.Object, mockSftpWrapper.Object);
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


    }
}
