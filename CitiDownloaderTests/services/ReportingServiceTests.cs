using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
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
using System.Diagnostics;
using static CitiDownloader.services.LogService;

namespace CitiDownloaderTests.services
{
    [TestFixture]
    public class ReportingServiceTests
    {
        private Fixture fixture = new Fixture();

        [Test]
        public void ReportSystemErrorTest()
        {
            // Setup
            Random random = new Random();
            int itemsCount = random.Next(1, 20);

            // Execute
            IReportingService reportingService = new ReportingService();
            for (int i = 0; i < itemsCount; i++)
            {
                reportingService.ReportSystemError(fixture.Generate<SystemError>(), fixture.Generate<List<string>>());
            }

            // Verify
            Assert.That(reportingService.HasErrors());
            Assert.That(reportingService.GetSystemErrors().Count == itemsCount);
        }

        [Test]
        public void ReportUnknownCourseTest()
        {
            // Setup
            SystemError systemError = fixture.Generate<SystemError>();
            List<string> messages = fixture.Generate<List<string>>();
            Random random = new Random();
            int itemsCount = random.Next(1, 20);

            // Execute
            IReportingService reportingService = new ReportingService();
            for (int i = 0; i < itemsCount; i++)
            {
                reportingService.ReportUnknownCourse(fixture.Generate<CitiRecord>(), fixture.Generate<List<string>>());
            }

            // Verify
            Assert.That(reportingService.HasErrors());
            Assert.That(reportingService.GetUnknownCourses().Count == itemsCount);
        }

        [Test]
        public void ReportUnknownUsersTest()
        {
            // Setup
            SystemError systemError = fixture.Generate<SystemError>();
            List<string> messages = fixture.Generate<List<string>>();
            Random random = new Random();
            int itemsCount = random.Next(1, 20);

            // Execute
            IReportingService reportingService = new ReportingService();
            for (int i = 0; i < itemsCount; i++)
            {
                reportingService.ReportUnknownUser(fixture.Generate<CitiRecord>(), fixture.Generate<List<string>>());
            }

            // Verify
            Assert.That(reportingService.HasErrors());
            Assert.That(reportingService.GetUnknownUsers().Count == itemsCount);
        }

        [Test]
        public void ReportNoErrorsTest()
        {
            // Setup


            // Execute
            IReportingService reportingService = new ReportingService();


            // Verify
            Assert.That(!reportingService.HasErrors());
        }
    }
}
