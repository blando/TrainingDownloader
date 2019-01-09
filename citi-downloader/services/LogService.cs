using TrainingDownloader.models;
using TrainingDownloader.wrappers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Linq;
using TrainingDownloader.configurations;

namespace TrainingDownloader.services
{
    public class LogService : ILogService
    {
        private List<string> LogCache { get; set; }
        public bool CacheLogMessages { get; set; }
        private IEventLogClient eventLogClient { get; set; }
        private ApplicationConfiguration appConfig { get; set; }
        public enum EventType { Information = 0, Warning = 1, Error = 2, Debug = 3 };

        public LogService(IEventLogClient eventLogClient, ApplicationConfiguration appConfig)
        {
            this.eventLogClient = eventLogClient;
            this.appConfig = appConfig;
            this.LogCache = new List<string>();
            this.CacheLogMessages = true;
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
                eventLogClient.LogMessage(message, eventLogEntryType);

            if (CacheLogMessages)
            {
                LogCache.Add(string.Format("{0}\t{1}\t{2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), eventType.ToString(), message));
            }

            if (appConfig.verbose)
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
            return LogCache.ToList();
        }

        public List<string> GetCacheAndFlush()
        {
            List<string> temp = LogCache.ToList();
            LogCache.Clear();
            return temp;
        }
    }
}
