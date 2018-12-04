using System;
using System.Collections.Generic;

namespace CitiDownloader.models.entities
{
    public partial class History
    {
        public string DateTimeStamp { get; set; }
        public int? BatchNumber { get; set; }
        public int CurriculaId { get; set; }
        public string LearnerId { get; set; }
        public string CourseId { get; set; }
        public string ClassId { get; set; }
        public int? ClassKey { get; set; }
        public int? ClassSessionKey { get; set; }
        public int? ClassSessionTopicKey { get; set; }
        public string Title { get; set; }
        public string InternalVersion { get; set; }
        public string Status { get; set; }
        public DateTime? EnrollmentDate { get; set; }
        public int? Score { get; set; }
        public string CompletionStatusId { get; set; }
        public string HistoryTypeId { get; set; }
        public DateTime? StatusDate { get; set; }
        public string ActionCode { get; set; }
        public int? WaitlistPosition { get; set; }
        public DateTime? DateExpires { get; set; }
        public double? Duration { get; set; }
        public double? Cost { get; set; }
        public string Notes { get; set; }
        public string StatusReason { get; set; }
        public string Bookmark { get; set; }
        public int? Accesses { get; set; }
        public DateTime? LastAccessDt { get; set; }
        public string LastAccessIp { get; set; }
        public string Credit { get; set; }
        public double? Ceu { get; set; }
        public string TitleId { get; set; }
        public string DepartmentId { get; set; }
        public string CompetencyCode { get; set; }
        public double? LaborRate { get; set; }
        public bool? UserFlag1 { get; set; }
        public bool? UserFlag2 { get; set; }
        public bool? UserFlag3 { get; set; }
        public bool? UserFlag4 { get; set; }
        public bool? UserFlag5 { get; set; }
        public bool? UserFlag6 { get; set; }
        public string User1 { get; set; }
        public string User2 { get; set; }
        public string User3 { get; set; }
        public string User4 { get; set; }
        public string User5 { get; set; }
        public string User6 { get; set; }
        public DateTime? UserDate1 { get; set; }
        public DateTime? UserDate2 { get; set; }
        public DateTime? UserDate3 { get; set; }
        public DateTime? UserDate4 { get; set; }
        public DateTime? UserDate5 { get; set; }
        public DateTime? UserDate6 { get; set; }
        public string CoreTotalTime { get; set; }
        public string CourseTime { get; set; }
        public bool? AuthorizeRetake { get; set; }
        public string LibStatus { get; set; }
        public DateTime? LibDueDate { get; set; }
        public DateTime? LibReturnDate { get; set; }
        public int? ClassMealKey { get; set; }
        public bool? SignatureEntered { get; set; }
        public int? TestOutAttempts { get; set; }
        public int? PerformanceLevel { get; set; }
        public string ApproverLearnerId { get; set; }
        public int? FirstScore { get; set; }
        public int? LastScore { get; set; }
        public int? HighestScore { get; set; }
        public int? PassingScore { get; set; }
        public DateTime? RequestDate { get; set; }
        public int? PaymentMethod { get; set; }
        public string PaymentReference { get; set; }
        public double? PaymentAmount { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string TrainerId { get; set; }
        public string CourseUnitTypeId { get; set; }
        public int? LocationKey { get; set; }
        public string VenueId { get; set; }
        public int? PreTestScore { get; set; }
        public int? PostTestScore { get; set; }
    }
}
