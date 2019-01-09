using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using TrainingDownloader.models;
using TrainingDownloader.models.entities;
using CsvHelper;
using CsvHelper.Configuration;
using TrainingDownloader.configurations;

namespace TrainingDownloader.wrappers
{
    public class CsvClient : ICsvClient
    {

        private ApplicationConfiguration config;

        public CsvClient(ApplicationConfiguration config)
        {
            this.config = config;
        }
        public List<VendorRecord> GetVendorRecords(string file)
        {
            string fileContents = File.ReadAllText(file);
            using (CsvReader csvReader = new CsvReader(new StringReader(fileContents)))
            {
                csvReader.Configuration.Delimiter = ",";
                csvReader.Configuration.HeaderValidated = null;
                csvReader.Configuration.HasHeaderRecord = true;
                csvReader.Configuration.MissingFieldFound = null;
                csvReader.Configuration.RegisterClassMap(new VendorCSVMap(config.FieldMaps));
                return csvReader.GetRecords<VendorRecord>().ToList();
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
          
}
