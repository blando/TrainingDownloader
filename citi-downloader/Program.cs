using CitiDownloader.models.entities;
using CitiDownloader.repositories;
using CitiDownloader.services;
using CitiDownloader.wrappers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace CitiDownloader
{
    class Program
    {
        
        static void Main(string[] args)
        {

            var serviceProvider = new ServiceCollection()
                .AddSingleton<ICitiDownloadService, CitiDownloadService>()
                .AddSingleton<IWebClientWrapper, WebClientWrapper>()
                .AddSingleton<ApplicationConfiguration, AppConfig>()
                .AddSingleton<ICitiService, CitiService>()
                .AddSingleton<ILearnerWebServices, LearnerWebService>()
                .AddSingleton<ITrainingService, TrainingService>()
                .AddSingleton<ICsvWrapper, CsvWrapper>()
                .AddSingleton<ISftpWrapper, SftpWrapper>()
                .AddSingleton<ILearnerWebRepository, LearnerWebRepository>()
                .AddDbContext<LWEBIAStateContext>()
                .BuildServiceProvider();

            ITrainingService trainingService = serviceProvider.GetService<ITrainingService>();

            List<string> arguments = new List<string>(args);
            List<History> histories = new List<History>();
            if (arguments.Contains("full"))
            {
                histories = trainingService.ProcessRecords(models.ProcessTypeEnum.ProcessType.Full);

            }
            else if (arguments.Contains("delta"))
            {
                histories = trainingService.ProcessRecords(models.ProcessTypeEnum.ProcessType.Incremental);
            }

            if (arguments.Contains("upload"))
            {
                trainingService.UploadHistoryRecords(histories);
            }
            if (arguments.Contains("insert"))
            {
                trainingService.InsertHistoryRecords(histories);
            }
        }

    }
}
