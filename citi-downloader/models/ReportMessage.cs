using System;
using System.Collections.Generic;
using System.Text;

namespace CitiDownloader.models
{
    public class ReportMessage
    {
        public object attachedObject { get; set; }
        public List<string> messages { get; set; }
    }
}
