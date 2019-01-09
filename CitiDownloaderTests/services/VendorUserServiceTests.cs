using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using SimpleFixture;
using NUnit.Framework;
using TrainingDownloader.configurations;
using TrainingDownloader.exceptions;
using TrainingDownloader.models.entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using TrainingDownloader.repositories;
using TrainingDownloader.models;
using TrainingDownloader.services.interfaces;
using TrainingDownloader.services;

namespace TrainingDownloaderTests.services
{
    [TestFixture]
    public class VendorUserServiceTests
    {
        private ApplicationConfiguration applicationConfiguration;
        private Fixture fixture = new Fixture();

        private void SetupMocks()
        {
            applicationConfiguration = new ApplicationConfiguration();
        }

        [Test]
        public void CreateCitiVendorUserTest()
        {
            // Setup
            SetupMocks();
            VendorRecord vRecord = fixture.Generate<VendorRecord>();
            applicationConfiguration.applicationType = CommandLineConfiguration.ApplicationType.Citi;

            // Execute
            IVendorUserService vendorUserService = new VendorUserService(applicationConfiguration);
            VendorUser response = vendorUserService.CreateVendorUser(vRecord);

            // Verify
            IsuCitiLwLearners citiUser = (IsuCitiLwLearners)response;
            Assert.That(citiUser.CitiLastName == vRecord.LastName);
            Assert.That(citiUser.CitiLearnerId == vRecord.VendorUserId);
            Assert.That(citiUser.LwLearnerId == vRecord.UnivId);
            Assert.That(citiUser.Valid.Value);
                
        }

        [Test]
        public void CreateAalasVendorUserTest()
        {
            // Setup
            SetupMocks();
            VendorRecord vRecord = fixture.Generate<VendorRecord>();
            applicationConfiguration.applicationType = CommandLineConfiguration.ApplicationType.Aalas;

            // Execute
            IVendorUserService vendorUserService = new VendorUserService(applicationConfiguration);
            VendorUser response = vendorUserService.CreateVendorUser(vRecord);

            // Verify
            IsuAalasLwLearners aalasUser = (IsuAalasLwLearners)response;
            Assert.That(aalasUser.AalasLastName == vRecord.LastName);
            Assert.That(aalasUser.AalasLearnerId == vRecord.VendorUserId);
            Assert.That(aalasUser.LwLearnerId == vRecord.UnivId);
            Assert.That(aalasUser.Valid.Value);

        }
    }
}
