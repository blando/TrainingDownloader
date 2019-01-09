using TrainingDownloader.exceptions;
using TrainingDownloader.models.entities;
using TrainingDownloader.repositories;
using TrainingDownloader.services;
using TrainingDownloader.wrappers;
using ISULogger;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using TrainingDownloader.configurations;
using TrainingDownloader.services.interfaces;

namespace TrainingDownloader
{
    class Program
    {
        
        static void Main(string[] args)
        {

            CommandLineConfiguration config = new CommandLineConfiguration(args);

            var serviceProvider = new ServiceCollection()
                .AddSingleton<IVendorDownloadService, VendorDownloadService>()
                .AddSingleton<IWebClientWrapper, WebClientWrapper>()
                .AddSingleton<IVendorService, VendorService>()
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
                .AddSingleton<IVendorUserService, VendorUserService>()
                .AddSingleton<IFolderCleanupService, FolderCleanupService>()
                .AddSingleton(config.applicationConfiguration)
                .AddDbContext<LWEBIAStateContext>()
                .BuildServiceProvider();

            // Process Training Records
            serviceProvider.GetService<ITrainingService>().ProcessRecords();
            // Cleanup Download Folder
            serviceProvider.GetService<IFolderCleanupService>().CleanUpDataDirectory();
        }

    }
}
