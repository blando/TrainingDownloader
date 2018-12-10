using CitiDownloader.models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace CitiDownloader.wrappers
{
    public class EventLogWrapper : IEventLogWrapper
    {
        private EventLog eventLog { get; set; }
        private ApplicationConfiguration config { get; set; }

        public EventLogWrapper(ApplicationConfiguration config)
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

            }
        }

    }
}
