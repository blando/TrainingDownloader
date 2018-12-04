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


    }
}
