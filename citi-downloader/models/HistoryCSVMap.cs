using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using TrainingDownloader.models.entities;

namespace TrainingDownloader.models
{
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
}
