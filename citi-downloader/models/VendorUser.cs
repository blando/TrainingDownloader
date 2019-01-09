using System;
using System.Collections.Generic;
using System.Text;

namespace TrainingDownloader.models
{
    public abstract class VendorUser
    {
        public string LwLearnerId { get; set; }
        public abstract string GetVendorLearnerId();
        public abstract string GetVendorLastName();
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public bool? Valid { get; set; }
    }
}
