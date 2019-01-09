using System;
using System.Collections.Generic;
using TrainingDownloader.wrappers;
using TrainingDownloader.services;
using Moq;
using NUnit.Framework;
using SimpleFixture;
using System.Diagnostics;
using static TrainingDownloader.services.LogService;
using TrainingDownloader.configurations;

namespace TrainingDownloaderTests.services
{
    [TestFixture]
    public class LogServiceTests
    {
        private Mock<IEventLogClient> mockEventLogClient;
        private ApplicationConfiguration applicationConfiguration;
        private void SetupMocks()
        {
            applicationConfiguration = new ApplicationConfiguration();
            mockEventLogClient = new Mock<IEventLogClient>();
        }

        [Test]
        public void LogMessageInformationTest()
        {
            // Setup
            SetupMocks();
            EventType eventType = EventType.Information;
            Fixture fixture = new Fixture();
            string message = fixture.Generate<string>();

            mockEventLogClient.Setup(f => f.LogMessage(It.IsAny<string>(), EventLogEntryType.Information)).Verifiable();

            // Execute
            ILogService logService = new LogService(mockEventLogClient.Object, applicationConfiguration);
            logService.LogMessage(message, eventType);

            // Verify
            Mock.VerifyAll(mockEventLogClient);
            mockEventLogClient.Verify(f => f.LogMessage(It.IsAny<string>(), It.IsAny<EventLogEntryType>()), Times.Once);
            Assert.That(logService.GetCache().Count == 1);
        }

        [Test]
        public void LogMessageWarningTest()
        {
            // Setup
            SetupMocks();
            EventType eventType = EventType.Warning;
            Fixture fixture = new Fixture();
            string message = fixture.Generate<string>();

            mockEventLogClient.Setup(f => f.LogMessage(It.IsAny<string>(), EventLogEntryType.Warning)).Verifiable();

            // Execute
            ILogService logService = new LogService(mockEventLogClient.Object, applicationConfiguration);
            logService.LogMessage(message, eventType);

            // Verify
            Mock.VerifyAll(mockEventLogClient);
            mockEventLogClient.Verify(f => f.LogMessage(It.IsAny<string>(), It.IsAny<EventLogEntryType>()), Times.Once);
            Assert.That(logService.GetCache().Count == 1);
        }

        [Test]
        public void LogMessageErrorTest()
        {
            // Setup
            SetupMocks();
            EventType eventType = EventType.Error;
            Fixture fixture = new Fixture();
            string message = fixture.Generate<string>();

            mockEventLogClient.Setup(f => f.LogMessage(It.IsAny<string>(), EventLogEntryType.Error)).Verifiable();

            // Execute
            ILogService logService = new LogService(mockEventLogClient.Object, applicationConfiguration);
            logService.LogMessage(message, eventType);

            // Verify
            Mock.VerifyAll(mockEventLogClient);
            mockEventLogClient.Verify(f => f.LogMessage(It.IsAny<string>(), It.IsAny<EventLogEntryType>()), Times.Once);
            Assert.That(logService.GetCache().Count == 1);
        }

        [Test]
        public void LogMessageErrorVerboseTest()
        {
            // Setup
            SetupMocks();
            EventType eventType = EventType.Error;
            Fixture fixture = new Fixture();
            string message = fixture.Generate<string>();
            applicationConfiguration.verbose = true;
            mockEventLogClient.Setup(f => f.LogMessage(It.IsAny<string>(), EventLogEntryType.Error)).Verifiable();

            // Execute
            ILogService logService = new LogService(mockEventLogClient.Object, applicationConfiguration);
            
            logService.LogMessage(message, eventType);

            // Verify
            Mock.VerifyAll(mockEventLogClient);
            mockEventLogClient.Verify(f => f.LogMessage(It.IsAny<string>(), It.IsAny<EventLogEntryType>()), Times.Once);
            Assert.That(logService.GetCache().Count == 1);
        }

        [Test]
        public void LogMessageDebugTest()
        {
            // Setup
            SetupMocks();
            EventType eventType = EventType.Debug;
            Fixture fixture = new Fixture();
            string message = fixture.Generate<string>();

            // Execute
            ILogService logService = new LogService(mockEventLogClient.Object, applicationConfiguration);
            logService.LogMessage(message, eventType);

            // Verify
            Mock.VerifyAll(mockEventLogClient);
            mockEventLogClient.Verify(f => f.LogMessage(It.IsAny<string>(), It.IsAny<EventLogEntryType>()), Times.Never);
            Assert.That(logService.GetCache().Count == 1);
        }

        [Test]
        public void LogMessageLogMessageThrowsExceptionTest()
        {
            // Setup
            SetupMocks();
            Fixture fixture = new Fixture();
            string message = fixture.Generate<string>();

            mockEventLogClient.Setup(f => f.LogMessage(It.IsAny<string>(), It.IsAny<EventLogEntryType>())).Throws(new Exception());

            // Execute
            ILogService logService = new LogService(mockEventLogClient.Object, applicationConfiguration);

            // Execute & Verify
            Assert.Throws<Exception>(delegate { logService.LogMessage(message, It.IsAny<EventType>()); });

            // Verify
            Mock.VerifyAll(mockEventLogClient);
            mockEventLogClient.Verify(f => f.LogMessage(It.IsAny<string>(), It.IsAny<EventLogEntryType>()), Times.Once);
            Assert.That(logService.GetCache().Count == 0);
        }

        [Test]
        public void FlushCacheTest()
        {
            // Setup
            SetupMocks();
            mockEventLogClient.Setup(f => f.LogMessage(It.IsAny<string>(), It.IsAny<EventLogEntryType>())).Verifiable();
            Fixture fixture = new Fixture();
            Random random = new Random();
            int itemsCount = random.Next(1, 20);
            string message = fixture.Generate<string>();

            // Execute
            ILogService logService = new LogService(mockEventLogClient.Object, applicationConfiguration);
            for (int i = 0; i < itemsCount; i++)
            {
                logService.LogMessage(fixture.Generate<string>(), fixture.Generate<EventType>());
            }

            List<string> preMessages = logService.GetCache();
            logService.FlushCache();
            List<string> postMesages = logService.GetCache();

            // Verify
            Assert.That(preMessages.Count == itemsCount);
            Assert.That(postMesages.Count == 0);
            Mock.VerifyAll(mockEventLogClient);
        }

        [Test]
        public void GetCacheTest()
        {
            // Setup
            SetupMocks();
            Fixture fixture = new Fixture();
            Random random = new Random();
            int itemsCount = random.Next(1, 20);
            string message = fixture.Generate<string>();

            // Execute
            ILogService logService = new LogService(mockEventLogClient.Object, applicationConfiguration);
            for (int i = 0; i < itemsCount; i++)
            {
                logService.LogMessage(fixture.Generate<string>(), fixture.Generate<EventType>());
            }

            List<string> messages = logService.GetCache();

            // Verify
            Assert.That(messages.Count == itemsCount);
            Mock.VerifyAll(mockEventLogClient);
        }

        [Test]
        public void GetCacheAndFlushTest()
        {
            // Setup
            SetupMocks();
            Fixture fixture = new Fixture();
            Random random = new Random();
            int itemsCount = random.Next(1, 20);
            string message = fixture.Generate<string>();

            // Execute
            ILogService logService = new LogService(mockEventLogClient.Object, applicationConfiguration);
            for (int i = 0; i < itemsCount; i++)
            {
                logService.LogMessage(fixture.Generate<string>(), fixture.Generate<EventType>());
            }

            List<string> preMessages = logService.GetCache();
            List<string> returnMessages = logService.GetCacheAndFlush();
            List<string> postMesages = logService.GetCache();

            // Verify
            Assert.That(preMessages.Count == itemsCount);
            Assert.That(returnMessages.Count == itemsCount);
            Assert.That(postMesages.Count == 0);
            Mock.VerifyAll(mockEventLogClient);
        }
    }
}
