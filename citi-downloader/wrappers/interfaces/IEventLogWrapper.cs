using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace CitiDownloader.wrappers
{
    public interface IEventLogWrapper
    {
        void LogMessage(string message, EventLogEntryType type);
    }
}
