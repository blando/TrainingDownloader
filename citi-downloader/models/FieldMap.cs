using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace TrainingDownloader.models
{
    public class FieldMap
    {
        public int? VendorUserId { get; set; }
        public int? FirstName { get; set; }
        public int? LastName { get; set; }
        public int? EmailAddress { get; set; }
        public int? RegistrationDate { get; set; }
        public int? VendorCourseName { get; set; }
        public int? StageNumber { get; set; }
        public int? StageDescription { get; set; }
        public int? CompletionReportNum { get; set; }
        public int? CompletionDate { get; set; }
        public int? Score { get; set; }
        public int? PassingScore { get; set; }
        public int? ExpirationDate { get; set; }
        public int? VendorCourseId { get; set; }

    }
}
