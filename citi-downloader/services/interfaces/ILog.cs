using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using ISULogger;
using static CitiDownloader.services.Log;

namespace CitiDownloader.services
{
    public interface ILog
    {
        void LogMessage(string message, EventType eventType);
        void FlushCache();
        List<string> GetCacheAndFlush();
        List<string> GetCache();
    }
}
