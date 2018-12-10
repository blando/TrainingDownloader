using CitiDownloader.models;
using CitiDownloader.wrappers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace CitiDownloader.services
{
    public class Log : ILog
    {
        private List<string> LogCache { get; set; }
        public bool CacheLogMessages { get; set; }
        public bool Verbose { get; set; }
        private IEventLogWrapper eventLogWrapper { get; set; }
        public enum EventType { Information = 0, Warning = 1, Error = 2, Debug = 3 };

        public Log(IEventLogWrapper eventLogWrapper)
        {
            this.eventLogWrapper = eventLogWrapper;
            
        }

        public void LogMessage(string message, EventType eventType)
        {
            bool WriteToEventLog = true;
            EventLogEntryType eventLogEntryType = EventLogEntryType.Information;
            switch (eventType)
            {
                case EventType.Information:
                    eventLogEntryType = EventLogEntryType.Information;
                    break;
                case EventType.Warning:
                    eventLogEntryType = EventLogEntryType.Warning;
                    break;
                case EventType.Error:
                    eventLogEntryType = EventLogEntryType.Error;
                    break;
                case EventType.Debug:
                    WriteToEventLog = false;
                    break;
            }
            if (WriteToEventLog)
                eventLogWrapper.LogMessage(message, eventLogEntryType);

            if (CacheLogMessages)
            {
                LogCache.Add(string.Format("{0}\t{1}\t{2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), eventType.ToString(), message));
            }

            if (Verbose)
            {
                Console.WriteLine(string.Format("{0}\t{1}\t{2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), eventType.ToString(), message));
            }
        }

        public void FlushCache()
        {
            LogCache.Clear();
        }

        public List<string> GetCache()
        {
            return LogCache;
        }

        public List<string> GetCacheAndFlush()
        {
            List<string> temp = LogCache;
            LogCache.Clear();
            return temp;
        }
    }
}
