using System;
using System.Collections.Generic;

namespace CitiDownloader.models.entities
{
    public partial class IsuCitiLwTlHistory
    {
        public int LwCurriculaId { get; set; }
        public string CitiHistoryId { get; set; }
        public string TlHistoryId { get; set; }
        public DateTime? DateInserted { get; set; }
    }
}
