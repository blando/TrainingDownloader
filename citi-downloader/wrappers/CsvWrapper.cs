using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using CitiDownloader.models;
using CitiDownloader.models.entities;
using CsvHelper;
using CsvHelper.Configuration;

namespace CitiDownloader.wrappers
{
    public class CsvWrapper : ICsvWrapper
    {

        private ApplicationConfiguration config;

        public CsvWrapper(ApplicationConfiguration config)
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
            string outputFile = config.FullOutputPath;
            Configuration csvConfig = new Configuration
            {
                QuoteAllFields = true,
                HasHeaderRecord = true,
                Delimiter = ","
            };

            using (CsvWriter csvWriter = new CsvWriter(new StreamWriter(outputFile), csvConfig))
            {
                csvWriter.WriteRecords<History>(histories);
            }
            return outputFile;
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
