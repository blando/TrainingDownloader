using System.Collections.Generic;
using TrainingDownloader.wrappers;
using TrainingDownloader.services;
using Moq;
using NUnit.Framework;
using SimpleFixture;
using TrainingDownloader.models;
using TrainingDownloader.exceptions;
using TrainingDownloader.configurations;

namespace TrainingDownloaderTests.services
{
    [TestFixture]
    public class MailServiceTests
    {
        private Mock<IReportingService> mockReportingService { get; set; }
        private Mock<IMailClient> mockMailWrapper { get; set; }
        private ApplicationConfiguration applicationConfiguration { get; set; }
        private Fixture fixture { get; set; }

        private void SetupMocks()
        {
            fixture = new Fixture();
            mockReportingService = new Mock<IReportingService>();
            mockMailWrapper = new Mock<IMailClient>();
            applicationConfiguration = new ApplicationConfiguration();
        }

        [Test]
        public void SendMessagesCoursesMessagesOnlyTest()
        {
            // Setup
            SetupMocks();
            mockReportingService.Setup(f => f.HasErrors()).Returns(true);
            List<ReportMessage> courseReportMessages = fixture.Generate<List<ReportMessage>>();
            
            foreach (ReportMessage rp in courseReportMessages)
            {
                rp.attachedObject = fixture.Generate<VendorRecord>();
            }
            mockReportingService.Setup(f => f.GetUnknownCourses()).Returns(courseReportMessages).Verifiable();
            mockReportingService.Setup(f => f.GetUnknownUsers()).Returns(new List<ReportMessage>()).Verifiable();
            mockReportingService.Setup(f => f.GetSystemErrors()).Returns(new List<ReportMessage>()).Verifiable();

            string mailTo = fixture.Generate<string>();
            string mailSubject = fixture.Generate<string>();
            string mailFrom = fixture.Generate<string>();
            string sysMailTo = fixture.Generate<string>();
            string sysMailSubject = fixture.Generate<string>();
            applicationConfiguration.AdminMailToAddress = mailTo;
            applicationConfiguration.AdminMailSubject = mailSubject;
            applicationConfiguration.MailSenderAddress = mailFrom;
            applicationConfiguration.SysAdminMailSubject = sysMailSubject;
            applicationConfiguration.SysAdminMailToAddress = sysMailTo;

            mockMailWrapper.Setup(f => f.SendEmail(mailTo, mailFrom, mailSubject, It.IsAny<string>())).Verifiable();
            

            // Execute
            IMailService mailService = new MailService(mockReportingService.Object, mockMailWrapper.Object, applicationConfiguration);
            mailService.SendMessages();

            // Verify
            Mock.VerifyAll(mockReportingService);
            Mock.VerifyAll(mockMailWrapper);
            mockMailWrapper.Verify(f => f.SendEmail(mailTo, mailFrom, mailSubject, It.IsAny<string>()), Times.Once);
            mockMailWrapper.Verify(f => f.SendEmail(sysMailTo, It.IsAny<string>(), sysMailSubject, It.IsAny<string>()), Times.Never);
            mockReportingService.Verify(f => f.GetUnknownCourses(), Times.Once);
            mockReportingService.Verify(f => f.GetUnknownUsers(), Times.Once);
            mockReportingService.Verify(f => f.GetSystemErrors(), Times.Once);
        }

        [Test]
        public void SendMessagesUserMessagesOnlyTest()
        {
            // Setup
            SetupMocks();
            mockReportingService.Setup(f => f.HasErrors()).Returns(true);
            List<ReportMessage> userReportMessages = fixture.Generate<List<ReportMessage>>();

            foreach (ReportMessage rp in userReportMessages)
            {
                rp.attachedObject = fixture.Generate<VendorRecord>();
            }
            mockReportingService.Setup(f => f.GetUnknownCourses()).Returns(new List<ReportMessage>()).Verifiable();
            mockReportingService.Setup(f => f.GetUnknownUsers()).Returns(userReportMessages).Verifiable();
            mockReportingService.Setup(f => f.GetSystemErrors()).Returns(new List<ReportMessage>()).Verifiable();

            string mailTo = fixture.Generate<string>();
            string mailSubject = fixture.Generate<string>();
            string mailFrom = fixture.Generate<string>();
            string sysMailTo = fixture.Generate<string>();
            string sysMailSubject = fixture.Generate<string>();
            applicationConfiguration.AdminMailToAddress = mailTo;
            applicationConfiguration.AdminMailSubject = mailSubject;
            applicationConfiguration.MailSenderAddress = mailFrom;
            applicationConfiguration.SysAdminMailSubject = sysMailSubject;
            applicationConfiguration.SysAdminMailToAddress = sysMailTo;

            mockMailWrapper.Setup(f => f.SendEmail(mailTo, mailFrom, mailSubject, It.IsAny<string>())).Verifiable();


            // Execute
            IMailService mailService = new MailService(mockReportingService.Object, mockMailWrapper.Object, applicationConfiguration);
            mailService.SendMessages();

            // Verify
            Mock.VerifyAll(mockReportingService);
            Mock.VerifyAll(mockMailWrapper);
            mockMailWrapper.Verify(f => f.SendEmail(mailTo, mailFrom, mailSubject, It.IsAny<string>()), Times.Once);
            mockMailWrapper.Verify(f => f.SendEmail(sysMailTo, It.IsAny<string>(), sysMailSubject, It.IsAny<string>()), Times.Never);
            mockReportingService.Verify(f => f.GetUnknownCourses(), Times.Once);
            mockReportingService.Verify(f => f.GetUnknownUsers(), Times.Once);
            mockReportingService.Verify(f => f.GetSystemErrors(), Times.Once);
        }

        [Test]
        public void SendMessagesSystemMessagesOnlyTest()
        {
            // Setup
            SetupMocks();
            mockReportingService.Setup(f => f.HasErrors()).Returns(true);
            List<ReportMessage> systemReportMessages = fixture.Generate<List<ReportMessage>>();
            foreach (ReportMessage rp in systemReportMessages)
            {
                rp.attachedObject = fixture.Generate<SystemError>();
            }

            mockReportingService.Setup(f => f.GetUnknownCourses()).Returns(new List<ReportMessage>()).Verifiable();
            mockReportingService.Setup(f => f.GetUnknownUsers()).Returns(new List<ReportMessage>()).Verifiable();
            mockReportingService.Setup(f => f.GetSystemErrors()).Returns(systemReportMessages).Verifiable();

            string mailTo = fixture.Generate<string>();
            string mailSubject = fixture.Generate<string>();
            string mailFrom = fixture.Generate<string>();
            string sysMailTo = fixture.Generate<string>();
            string sysMailSubject = fixture.Generate<string>();
            applicationConfiguration.AdminMailToAddress = mailTo;
            applicationConfiguration.AdminMailSubject = mailSubject;
            applicationConfiguration.MailSenderAddress = mailFrom;
            applicationConfiguration.SysAdminMailSubject = sysMailSubject;
            applicationConfiguration.SysAdminMailToAddress = sysMailTo;

            mockMailWrapper.Setup(f => f.SendEmail(sysMailTo, mailFrom, sysMailSubject, It.IsAny<string>())).Verifiable();

            // Execute
            IMailService mailService = new MailService(mockReportingService.Object, mockMailWrapper.Object,applicationConfiguration);
            mailService.SendMessages();

            // Verify
            Mock.VerifyAll(mockReportingService);
            Mock.VerifyAll(mockMailWrapper);
            mockMailWrapper.Verify(f => f.SendEmail(sysMailTo, mailFrom, sysMailSubject, It.IsAny<string>()), Times.Once);
            mockMailWrapper.Verify(f => f.SendEmail(mailTo, It.IsAny<string>(), mailSubject, It.IsAny<string>()), Times.Never);
            mockReportingService.Verify(f => f.GetUnknownCourses(), Times.Once);
            mockReportingService.Verify(f => f.GetUnknownUsers(), Times.Once);
            mockReportingService.Verify(f => f.GetSystemErrors(), Times.Once);
        }

        [Test]
        public void SendMessagesUserCourseAndSystemMessagesTest()
        {
            // Setup
            SetupMocks();
            mockReportingService.Setup(f => f.HasErrors()).Returns(true);
            List<ReportMessage> systemReportMessages = fixture.Generate<List<ReportMessage>>();
            foreach (ReportMessage rp in systemReportMessages)
            {
                rp.attachedObject = fixture.Generate<SystemError>();
            }

            List<ReportMessage> userReportMessages = fixture.Generate<List<ReportMessage>>();
            foreach(ReportMessage rp in userReportMessages)
            {
                rp.attachedObject = fixture.Generate<VendorRecord>();
            }

            List<ReportMessage> courseReportMessages = fixture.Generate<List<ReportMessage>>();
            foreach(ReportMessage rp in courseReportMessages)
            {
                rp.attachedObject = fixture.Generate<VendorRecord>();
            }

            mockReportingService.Setup(f => f.GetUnknownCourses()).Returns(courseReportMessages).Verifiable();
            mockReportingService.Setup(f => f.GetUnknownUsers()).Returns(userReportMessages).Verifiable();
            mockReportingService.Setup(f => f.GetSystemErrors()).Returns(systemReportMessages).Verifiable();

            string mailTo = fixture.Generate<string>();
            string mailSubject = fixture.Generate<string>();
            string mailFrom = fixture.Generate<string>();
            string sysMailTo = fixture.Generate<string>();
            string sysMailSubject = fixture.Generate<string>();
            applicationConfiguration.AdminMailToAddress = mailTo;
            applicationConfiguration.AdminMailSubject = mailSubject;
            applicationConfiguration.MailSenderAddress = mailFrom;
            applicationConfiguration.SysAdminMailSubject = sysMailSubject;
            applicationConfiguration.SysAdminMailToAddress = sysMailTo;

            mockMailWrapper.Setup(f => f.SendEmail(sysMailTo, mailFrom, sysMailSubject, It.IsAny<string>())).Verifiable();
            mockMailWrapper.Setup(f => f.SendEmail(mailTo, mailFrom, mailSubject, It.IsAny<string>())).Verifiable();

            // Execute
            IMailService mailService = new MailService(mockReportingService.Object, mockMailWrapper.Object, applicationConfiguration);
            mailService.SendMessages();

            // Verify
            Mock.VerifyAll(mockReportingService);
            Mock.VerifyAll(mockMailWrapper);
            mockMailWrapper.Verify(f => f.SendEmail(mailTo, It.IsAny<string>(), mailSubject, It.IsAny<string>()), Times.Once);
            mockMailWrapper.Verify(f => f.SendEmail(sysMailTo, It.IsAny<string>(), sysMailSubject, It.IsAny<string>()), Times.Once);
            mockReportingService.Verify(f => f.GetUnknownCourses(), Times.Once);
            mockReportingService.Verify(f => f.GetUnknownUsers(), Times.Once);
            mockReportingService.Verify(f => f.GetSystemErrors(), Times.Once);
        }

        [Test]
        public void SendMessagesNoMessageToSendTest()
        {
            // Setup
            SetupMocks();
            mockReportingService.Setup(f => f.HasErrors()).Returns(false);

            string mailTo = fixture.Generate<string>();
            string mailSubject = fixture.Generate<string>();
            string mailFrom = fixture.Generate<string>();
            string sysMailTo = fixture.Generate<string>();
            string sysMailSubject = fixture.Generate<string>();
            applicationConfiguration.AdminMailToAddress = mailTo;
            applicationConfiguration.AdminMailSubject = mailSubject;
            applicationConfiguration.MailSenderAddress = mailFrom;
            applicationConfiguration.SysAdminMailSubject = sysMailSubject;
            applicationConfiguration.SysAdminMailToAddress = sysMailTo;

            // Execute
            IMailService mailService = new MailService(mockReportingService.Object, mockMailWrapper.Object, applicationConfiguration);
            mailService.SendMessages();

            // Verify
            mockReportingService.Verify(f => f.GetUnknownCourses(), Times.Never);
            mockReportingService.Verify(f => f.GetUnknownUsers(), Times.Never);
            mockReportingService.Verify(f => f.GetSystemErrors(), Times.Never);
            mockMailWrapper.Verify(f => f.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void SendMessagesSendEmailThrowsExceptionTest()
        {
            // Setup
            SetupMocks();
            mockReportingService.Setup(f => f.HasErrors()).Returns(true);
            List<ReportMessage> courseReportMessages = fixture.Generate<List<ReportMessage>>();

            foreach (ReportMessage rp in courseReportMessages)
            {
                rp.attachedObject = fixture.Generate<VendorRecord>();
            }
            mockReportingService.Setup(f => f.GetUnknownCourses()).Returns(courseReportMessages).Verifiable();
            mockReportingService.Setup(f => f.GetUnknownUsers()).Returns(new List<ReportMessage>()).Verifiable();

            string mailTo = fixture.Generate<string>();
            string mailSubject = fixture.Generate<string>();
            string mailFrom = fixture.Generate<string>();
            string sysMailTo = fixture.Generate<string>();
            string sysMailSubject = fixture.Generate<string>();
            applicationConfiguration.AdminMailToAddress = mailTo;
            applicationConfiguration.AdminMailSubject = mailSubject;
            applicationConfiguration.MailSenderAddress = mailFrom;
            applicationConfiguration.SysAdminMailSubject = sysMailSubject;
            applicationConfiguration.SysAdminMailToAddress = sysMailTo;

            mockMailWrapper.Setup(f => f.SendEmail(mailTo, mailFrom, mailSubject, It.IsAny<string>())).Throws(new SendMailException("Unable to send email"));


            // Execute
            IMailService mailService = new MailService(mockReportingService.Object, mockMailWrapper.Object, applicationConfiguration);

            // Execute & Verify
            Assert.Throws<SendMailException>(delegate { mailService.SendMessages(); });

            // Verify
            Mock.VerifyAll(mockReportingService);
            mockMailWrapper.Verify(f => f.SendEmail(mailTo, mailFrom, mailSubject, It.IsAny<string>()), Times.Once);
            mockMailWrapper.Verify(f => f.SendEmail(sysMailTo, It.IsAny<string>(), sysMailSubject, It.IsAny<string>()), Times.Never);
            mockReportingService.Verify(f => f.GetUnknownCourses(), Times.Once);
            mockReportingService.Verify(f => f.GetUnknownUsers(), Times.Once);
            mockReportingService.Verify(f => f.GetSystemErrors(), Times.Never);
        }
    }
}
