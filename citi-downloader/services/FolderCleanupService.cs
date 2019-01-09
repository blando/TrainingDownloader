using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TrainingDownloader.configurations;
using TrainingDownloader.services.interfaces;

namespace TrainingDownloader.services
{
    public class FolderCleanupService : IFolderCleanupService
    {
        private ApplicationConfiguration applicationConfiguration;
        private LogService logService;

        public FolderCleanupService(ApplicationConfiguration applicationConfiguration)
        {
            this.applicationConfiguration = applicationConfiguration;
        }

        public void CleanUpDataDirectory()
        {
            try
            {
                foreach (string file in Directory.GetFiles(applicationConfiguration.SaveFilePath))
                {
                    try
                    {
                        if (File.GetCreationTime(file).AddDays(7) < DateTime.Now)
                        {
                            File.Delete(file);
                        }
                    }
                    catch (Exception err)
                    {
                        logService.LogMessage(String.Format("An error occurred deleting file: {0}", err.Message), LogService.EventType.Error);
                    }
                }
            }
            catch (Exception e)
            {
                logService.LogMessage(String.Format("An error occurred retrieving files from AALAS save path: {0}", e.Message), LogService.EventType.Error);
            }
        }
    }
}
