using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using SimpleFixture;
using NUnit.Framework;
using CitiDownloader.configurations;
using CitiDownloader.exceptions;

namespace CitiDownloaderTests.configurations
{
    [TestFixture]
    public class AppConfigTests
    {

        [Test]
        public void AppConfigFullUploadTest()
        {
            // Setup
            string[] args = new string[] { "full", "upload", "test-settings.json" };

            // Execute
            AppConfig appConfig = new AppConfig(args);

            // Verify
            Assert.That(appConfig.importType == ApplicationConfiguration.ImportType.Full);
            Assert.That(appConfig.processType == ApplicationConfiguration.ProcessType.Upload);
        }

        [Test]
        public void AppConfigDeltaUploadTest()
        {
            // Setup
            string[] args = new string[] { "delta", "upload", "test-settings.json" };

            // Execute
            AppConfig appConfig = new AppConfig(args);

            // Verify
            Assert.That(appConfig.importType == ApplicationConfiguration.ImportType.Incremental);
            Assert.That(appConfig.processType == ApplicationConfiguration.ProcessType.Upload);
        }

        [Test]
        public void AppConfigFullInsertTest()
        {
            // Setup
            string[] args = new string[] { "full", "insert", "test-settings.json" };

            // Execute
            AppConfig appConfig = new AppConfig(args);

            // Verify
            Assert.That(appConfig.importType == ApplicationConfiguration.ImportType.Full);
            Assert.That(appConfig.processType == ApplicationConfiguration.ProcessType.Insert);
            Assert.That(!appConfig.verbose);
        }

        [Test]
        public void AppConfigFullInsertVerboseTest()
        {
            // Setup
            string[] args = new string[] { "full", "insert", "test-settings.json", "verbose" };

            // Execute
            AppConfig appConfig = new AppConfig(args);

            // Verify
            Assert.That(appConfig.importType == ApplicationConfiguration.ImportType.Full);
            Assert.That(appConfig.processType == ApplicationConfiguration.ProcessType.Insert);
            Assert.That(appConfig.verbose);
        }

        [Test]
        public void AppConfigDeltaInsert()
        {
            // Setup
            string[] args = new string[] { "delta", "insert", "test-settings.json" };

            // Execute
            AppConfig appConfig = new AppConfig(args);

            // Verify
            Assert.That(appConfig.importType == ApplicationConfiguration.ImportType.Incremental);
            Assert.That(appConfig.processType == ApplicationConfiguration.ProcessType.Insert);
            Assert.That(appConfig.DownloadUrl == "https://test.com/incrementalFile");
            Assert.That(appConfig.SaveFilePath == string.Format("C:\\Downloads\\incrementalFile_{0}.csv", appConfig.dateTimeString));
            Assert.That(appConfig.OutputFilePath == string.Format("C:\\Downloads\\output_{0}.csv", appConfig.dateTimeString));
        }

        [Test]
        public void AppConfigParameterExceptionNoImportTest()
        {
            // Setup
            string[] args = new string[] { "fake", "insert", "test-settings.json" };

            // Execute & Verify    
            Assert.Throws<ParameterException>(delegate { AppConfig appConfig = new AppConfig(args); });

        }

        [Test]
        public void AppConfigParameterExceptionNoProcessTest()
        {
            // Setup
            string[] args = new string[] { "full", "fake", "test-settings.json" };

            // Execute & Verify    
            Assert.Throws<ParameterException>(delegate { AppConfig appConfig = new AppConfig(args); });

        }

        [Test]
        public void AppConfigParameterExceptionNoConfigFile()
        {
            // Setup
            string[] args = new string[] { "full", "fake", "test-settings-not-dot-json" };

            // Execute & Verify
            Assert.Throws<ParameterException>(delegate { AppConfig appConfig = new AppConfig(args); });
        }

        [Test]
        public void AppConfigVerifySettingsImportFileTest()
        {
            // Setup
            string[] args = new string[] { "delta", "insert", "test-settings.json" };

            // Execute
            AppConfig appConfig = new AppConfig(args);

            // Verify
            Assert.That(appConfig.SftpServer == "sftpserver.com");
            Assert.That(appConfig.SftpUserName == "user");
            Assert.That(appConfig.SftpPassword == "password");
            Assert.That(appConfig.SftpRemotePath == "/folder1/folder2");
            Assert.That(appConfig.SftpUploadFileName == string.Format("uploadFile_{0}.CSV", appConfig.dateTime.ToString("yyyyMMddHHmmss")));
            Assert.That(appConfig.AdminUrl == "https://admin.com/site/");
            Assert.That(appConfig.AdminMailToAddress == "receive@iastate.edu");
            Assert.That(appConfig.AdminMailSubject == "Admin Mail Subject");
            Assert.That(appConfig.SysAdminMailToAddress == "sys-receive@iastate.edu");
            Assert.That(appConfig.SysAdminMailSubject == "Sys Admin Mail Subject");
            Assert.That(appConfig.MailSenderAddress == "Sender <sender@iastate.edu>");
            Assert.That(appConfig.EventLogName == "Test-EventLog");
            Assert.That(appConfig.LinuxTextLogName == "/var/log/test.log");
            Assert.That(appConfig.DownloadUrl == "https://test.com/incrementalFile");
            Assert.That(appConfig.SaveFilePath == string.Format("{0}\\{1}", "C:\\Downloads", string.Format("incrementalFile_{0}.csv", string.Format(appConfig.dateTime.ToString("yyyyMMddHHmmss")))));
        }

        [Test]
        public void AppConfigVerifySettingsFullImportFileTest()
        {
            // Setup
            string[] args = new string[] { "full", "insert", "test-settings.json" };

            // Execute
            AppConfig appConfig = new AppConfig(args);

            // Verify
            Assert.That(appConfig.SftpServer == "sftpserver.com");
            Assert.That(appConfig.SftpUserName == "user");
            Assert.That(appConfig.SftpPassword == "password");
            Assert.That(appConfig.SftpRemotePath == "/folder1/folder2");
            Assert.That(appConfig.SftpUploadFileName == string.Format("uploadFile_{0}.CSV", appConfig.dateTime.ToString("yyyyMMddHHmmss")));
            Assert.That(appConfig.AdminUrl == "https://admin.com/site/");
            Assert.That(appConfig.AdminMailToAddress == "receive@iastate.edu");
            Assert.That(appConfig.AdminMailSubject == "Admin Mail Subject");
            Assert.That(appConfig.SysAdminMailToAddress == "sys-receive@iastate.edu");
            Assert.That(appConfig.SysAdminMailSubject == "Sys Admin Mail Subject");
            Assert.That(appConfig.MailSenderAddress == "Sender <sender@iastate.edu>");
            Assert.That(appConfig.EventLogName == "Test-EventLog");
            Assert.That(appConfig.LinuxTextLogName == "/var/log/test.log");
            Assert.That(appConfig.DownloadUrl == "https://test.com/fullFile");
            Assert.That(appConfig.SaveFilePath == string.Format("{0}\\{1}", "C:\\Downloads", string.Format("fullFile_{0}.csv", string.Format(appConfig.dateTime.ToString("yyyyMMddHHmmss")))));
        }


    }
}
