using System;
using System.Collections.Generic;
using System.Text;
using TrainingDownloader.configurations;
using TrainingDownloader.exceptions;
using TrainingDownloader.models;
using TrainingDownloader.models.entities;
using TrainingDownloader.services.interfaces;

namespace TrainingDownloader.services
{
    public class VendorUserService : IVendorUserService
    {
        private ApplicationConfiguration config;

        public VendorUserService(ApplicationConfiguration config)
        {
            this.config = config;
        }

        public VendorUser CreateVendorUser(VendorRecord vRecord)
        {
            switch(config.applicationType)
            {
                case CommandLineConfiguration.ApplicationType.Citi:
                    return new IsuCitiLwLearners
                    {
                        LwLearnerId = vRecord.UnivId,
                        Valid = true,
                        CitiLastName = vRecord.LastName,
                        CitiLearnerId = vRecord.VendorUserId,
                        DateUpdated = DateTime.Now,
                        DateCreated = DateTime.Now
                    };
                case CommandLineConfiguration.ApplicationType.Aalas:
                    return new IsuAalasLwLearners
                    {
                        LwLearnerId = vRecord.UnivId,
                        Valid = true,
                        AalasLastName = vRecord.LastName,
                        AalasLearnerId = vRecord.VendorUserId,
                        DateUpdated = DateTime.Now,
                        DateCreated = DateTime.Now
                    };
            }

            return null;
        }
    }
}
