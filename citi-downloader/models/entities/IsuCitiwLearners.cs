using System;
using System.Collections.Generic;

namespace TrainingDownloader.models.entities
{
    public partial class IsuCitiLwLearners : VendorUser
    {
        public string CitiLearnerId { get; set; }
        public string CitiLastName { get; set; }
        public override string GetVendorLearnerId()
        {
            return this.CitiLearnerId;
        }
        public override string GetVendorLastName()
        {
            return this.CitiLastName;
        }
    }
}
