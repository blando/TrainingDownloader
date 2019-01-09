using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace TrainingDownloader.models
{
    public sealed class VendorCSVMap : ClassMap<VendorRecord>
    {
        public VendorCSVMap(FieldMap fieldMap)
        {

            if (fieldMap.VendorUserId.HasValue)
                Map(m => m.VendorUserId).Index(fieldMap.VendorUserId.Value);

            if (fieldMap.FirstName.HasValue)
                Map(m => m.FirstName).Index(fieldMap.FirstName.Value);

            if (fieldMap.LastName.HasValue)
                Map(m => m.LastName).Index(fieldMap.LastName.Value);

            if (fieldMap.EmailAddress.HasValue)
                Map(m => m.EmailAddress).Index(fieldMap.EmailAddress.Value);

            if (fieldMap.RegistrationDate.HasValue)
                Map(m => m.RegistrationDate).Index(fieldMap.RegistrationDate.Value);

            if (fieldMap.VendorCourseName.HasValue)
                Map(m => m.VendorCourseName).Index(fieldMap.VendorCourseName.Value);

            if (fieldMap.StageNumber.HasValue)
                Map(m => m.StageNumber).Index(fieldMap.StageNumber.Value);

            if (fieldMap.StageDescription.HasValue)
                Map(m => m.StageDescription).Index(fieldMap.StageDescription.Value);

            if (fieldMap.CompletionReportNum.HasValue)
                Map(m => m.CompletionReportNum).Index(fieldMap.CompletionReportNum.Value);

            if (fieldMap.CompletionDate.HasValue)
                Map(m => m.CompletionDate).Index(fieldMap.CompletionDate.Value);

            if (fieldMap.Score.HasValue)
                Map(m => m.Score).Index(fieldMap.Score.Value);

            if (fieldMap.PassingScore.HasValue)
                Map(m => m.PassingScore).Index(fieldMap.PassingScore.Value);

            if (fieldMap.ExpirationDate.HasValue)
                Map(m => m.ExpirationDate).Index(fieldMap.ExpirationDate.Value);

            if (fieldMap.VendorCourseId.HasValue)
                Map(m => m.VendorCourseId).Index(fieldMap.VendorCourseId.Value);
        }
    }
}
