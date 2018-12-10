using System;
using System.Collections.Generic;
using System.Text;

namespace CitiDownloader.models
{
    public class SystemError
    {
        public string message { get; set; }
        public object attachedObject { get; set; }
        public Type objectType { get; set; }
    }
}
