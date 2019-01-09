using TrainingDownloader.models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using TrainingDownloader.configurations;

namespace TrainingDownloader.wrappers
{
    public class EventLogClient : IEventLogClient
    {
        private EventLog eventLog { get; set; }
        private ApplicationConfiguration config { get; set; }

        public EventLogClient(ApplicationConfiguration config)
        {
            this.config = config;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                eventLog = new EventLog();

                try
                {
                    if (!EventLog.SourceExists(config.EventLogName))
                    {
                        try
                        {
                            EventLog.CreateEventSource(config.EventLogName, config.EventLogName);
                        }
                        catch (Exception exception)
                        {
                            eventLog.Source = "Application";
                            eventLog.WriteEntry("An error occurred creating event log with name " + config.EventLogName + ": " + exception.Message, EventLogEntryType.Error);
                            return;
                        }
                    }

                    eventLog.Source = config.EventLogName;
                    eventLog.Log = config.EventLogName;
                }
                catch (Exception error)
                {
                    eventLog.Source = "Application";
                    eventLog.WriteEntry("An error occurred creating event log with name " + config.EventLogName + ": " + error.Message, EventLogEntryType.Error);
                }
            }
            else
            {

            }
        }

        public void LogMessage(string message, EventLogEntryType type)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                eventLog.WriteEntry(message, type);
            }
            else
            {
                WriteToTextFile(message, type);
            }
        }

        private void WriteToTextFile(string message, EventLogEntryType type)
        {
            using (StreamWriter file = new StreamWriter(config.LinuxTextLogName))
            {
                file.WriteLine(string.Format("{0}\t{1}\t{2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), type.ToString(), message));
            }
        }


    }
}
