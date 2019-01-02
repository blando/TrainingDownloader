using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using CitiDownloader.models;
using CitiDownloader.models.entities;
using CsvHelper;
using CsvHelper.Configuration;
using CitiDownloader.configurations;

namespace CitiDownloader.wrappers
{
    public class CsvClient : ICsvClient
    {

        private ApplicationConfiguration config;

        public CsvClient(ApplicationConfiguration config)
        {
            this.config = config;
        }
        public List<CitiRecord> GetCitiRecords(string file)
        {
            string fileContents = File.ReadAllText(file);
            using (CsvReader csvReader = new CsvReader(new StringReader(fileContents)))
            {
                csvReader.Configuration.RegisterClassMap<CitiRecordCSVMap>();
                csvReader.Configuration.Delimiter = ",";
                csvReader.Configuration.HeaderValidated = null;
                csvReader.Configuration.HasHeaderRecord = true;
                csvReader.Configuration.MissingFieldFound = null;
                return csvReader.GetRecords<CitiRecord>().ToList();
            }
        }

        public string WriteHistoryRecordsToFile(List<History> histories)
        {
            string outputFile = config.OutputFilePath;
            Configuration csvConfig = new Configuration
            {
                QuoteAllFields = true,
                HasHeaderRecord = false,
                Delimiter = ","
            };

            using (CsvWriter csvWriter = new CsvWriter(new StreamWriter(outputFile), csvConfig))
            {
                csvWriter.Configuration.RegisterClassMap<HistoryCSVMap>();
                csvWriter.WriteRecords<History>(histories);
            }
            return outputFile;
        }
    }

    public sealed class HistoryCSVMap : ClassMap<History>
    {
        public HistoryCSVMap()
        {
            Map(m => m.Date_Time_Stamp).Index(0);
            Map(m => m.LearnerId).Index(1);
            Map(m => m.CourseId).Index(2);
            Map(m => m.Title).Index(3);
            Map(m => m.Status).Index(4);
            Map(m => m.Enrollment_Date).Index(5);
            Map(m => m.Score).Index(6);
            Map(m => m.CompletionStatusId).Index(7);
            Map(m => m.Status_Date).Index(8);
            Map(m => m.date_expires).Index(9);
            Map(m => m.PassingScore).Index(10);
        }
    }

    public sealed class CitiRecordCSVMap : ClassMap<CitiRecord>
    {
        public CitiRecordCSVMap()
        {
            Map(m => m.CitiId).Index(0);
            Map(m => m.FirstName).Index(1);
            Map(m => m.LastName).Index(2);
            Map(m => m.EmailAddress).Index(3);
            Map(m => m.RegistrationDate).Index(4);
            Map(m => m.CourseName).Index(5);
            Map(m => m.Group).Index(6);
            Map(m => m.StageNumber).Index(7);
            Map(m => m.StageDescription).Index(8);
            Map(m => m.CompletionReportNum).Index(9);
            Map(m => m.CompletionDate).Index(10);
            Map(m => m.Score).Index(11);
            Map(m => m.PassingScore).Index(12);
            Map(m => m.ExpirationDate).Index(13);
            Map(m => m.GroupId).Index(14);
        }
    }
}
