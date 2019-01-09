using System;
using System.Collections.Generic;

namespace TrainingDownloader.models.entities
{
    public partial class IsuImportHistory
    {
        public int Id { get; set; }
        public string VendorUserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public string CourseName { get; set; }
        public byte? StageNumber { get; set; }
        public string StageDescription { get; set; }
        public string CompletionReportNum { get; set; }
        public DateTime CompletionDate { get; set; }
        public int? Score { get; set; }
        public int? PassingScore { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string GroupName { get; set; }
        public string VendorCourseId { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string InstitutionalEmailAddress { get; set; }
        public string EmployeeNumber { get; set; }
        public bool? Verified { get; set; }
        public bool? IsValid { get; set; }
        public bool? Inserted { get; set; }
        public byte? Source { get; set; }
        public int? CurriculaId { get; set; }
    }
}
