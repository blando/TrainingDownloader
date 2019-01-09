using System;
using System.Collections.Generic;
using System.Text;

namespace TrainingDownloader.models.entities
{
    public partial class IsuAalasLwLearners : VendorUser
    {
        public string AalasLearnerId { get; set; }
        public string AalasLastName { get; set; }
        public override string GetVendorLearnerId()
        {
            return this.AalasLearnerId;
        }
        public override string GetVendorLastName()
        {
            return this.AalasLastName;
        }
    }
}
