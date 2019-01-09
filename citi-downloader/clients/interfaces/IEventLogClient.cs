using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace TrainingDownloader.wrappers
{
    public interface IEventLogClient
    {
        void LogMessage(string message, EventLogEntryType type);
    }
}
