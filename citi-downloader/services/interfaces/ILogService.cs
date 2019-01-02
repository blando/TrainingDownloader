using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using ISULogger;
using static CitiDownloader.services.LogService;

namespace CitiDownloader.services
{
    public interface ILogService
    {
        void LogMessage(string message, EventType eventType);
        void FlushCache();
        List<string> GetCacheAndFlush();
        List<string> GetCache();
    }
}
