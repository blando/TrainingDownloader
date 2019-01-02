using CitiDownloader.exceptions;
using CitiDownloader.models.entities;
using CitiDownloader.repositories;
using CitiDownloader.services;
using CitiDownloader.wrappers;
using ISULogger;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using CitiDownloader.configurations;

namespace CitiDownloader
{
    class Program
    {
        
        static void Main(string[] args)
        {

            var serviceProvider = new ServiceCollection()
                .AddSingleton<ICitiDownloadService, CitiDownloadService>()
                .AddSingleton<IWebClientWrapper, WebClientWrapper>()
                .AddSingleton<ICitiService, CitiService>()
                .AddSingleton<ILearnerWebServices, LearnerWebService>()
                .AddSingleton<ITrainingService, TrainingService>()
                .AddSingleton<ICsvClient, CsvClient>()
                .AddSingleton<ISftpClient, SftpClient>()
                .AddSingleton<ILearnerWebRepository, LearnerWebRepository>()
                .AddSingleton<IReportingService, ReportingService>()
                .AddSingleton<ILogService, LogService>()
                .AddSingleton<IMailService, MailService>()
                .AddSingleton<IMailClient, MailClient>()
                .AddSingleton<IEventLogClient, EventLogClient>()
                .AddDbContext<LWEBIAStateContext>()
                .AddTransient<ApplicationConfiguration>(s => new AppConfig(args))
                .BuildServiceProvider();

            ITrainingService trainingService = serviceProvider.GetService<ITrainingService>();

            trainingService.ProcessRecords();
        }

    }
}
