using System;
using System.Collections.Generic;

namespace CitiDownloader.models.entities
{
    public partial class IsuCitiLwLearners
    {
        public string LwLearnerId { get; set; }
        public string CitiLearnerId { get; set; }
        public string CitiLastName { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public bool? Valid { get; set; }
    }
}
