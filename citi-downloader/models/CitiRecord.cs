using HtmlTags;
using System;
using System.Collections.Generic;
using System.Text;

namespace CitiDownloader.models
{
    public class CitiRecord
    {
        public string ID { get; set; }
        public string _courseID { get; set; }
        public string UnivId { get; set; }
        public string CitiId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string NetId {
            get
            {
                if (this.EmailAddress != null && this.EmailAddress.Contains("@iastate.edu"))
                {
                    return this.EmailAddress.Split('@')[0];
                }
                else if (this.InstitutionalEmailAddress != null && this.InstitutionalEmailAddress.Contains("@iastate.edu"))
                {
                    return this.InstitutionalEmailAddress.Split('@')[0];
                }
                else
                {
                    return null;
                }
            }
        }
        public string UserId { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public string CourseName { get; set; }
        public string CourseId { get; set; }
        public string CompletionReportNum { get; set; }
        public DateTime? CompletionDate { get; set; }
        public int? Score { get; set; }
        public int? PassingScore { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string CitiCourseId
        {
            get
            {
                if (this.StageNumber.HasValue && this.StageNumber.Value != 1)
                {
                    return string.Format("{0}-{1}", this.GroupId, this.StageNumber);
                }

                return this.GroupId;
            }
        }
        public string GroupId { get; set; }
        public string Group { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string InstitutionalEmailAddress { get; set; }
        public string EmployeeNumber { get; set; }
        public int? StageNumber { get; set; }
        public string StageDescription { get; set; }
        public bool IsValid { get; set; }
        public bool Verified { get; set; }
        public string EntryString { get; set; }

        public DateTime GetCompletionDate()
        {
            return FormatNullableDate(this.CompletionDate);
        }
        public string GetCompletionDateString()
        {
            return GetCompletionDate().ToString("yyyy-MM-dd HH:mm:ss");
        }
        public DateTime GetRegistrationDate()
        {
            return FormatNullableDate(this.RegistrationDate);
        }
        public string GetRegistrationDateString()
        {
            return GetRegistrationDate().ToString("yyyy-MM-dd HH:mm:ss");
        }
        public DateTime GetExpirationDate()
        {
            return FormatNullableDate(this.ExpirationDate);
        }
        public string GetExpirationDateString()
        {
            return GetExpirationDate().ToString("yyyy-MM-dd HH:mm:ss");
        }
        public string GetDateTimeStamp()
        {
            return this.CompletionDate.HasValue ? this.CompletionDate.Value.ToString("yyMMddHHmmss") : DateTime.Now.ToString("yyMMddHHmmss");
        }

        private DateTime FormatNullableDate(DateTime? nullableDateTime)
        {
            if (!nullableDateTime.HasValue)
            {
                return DateTime.MinValue;
            }

            DateTime dateTime = (DateTime)nullableDateTime;

            if (dateTime.Second > 29)
            {
                dateTime = dateTime.AddMinutes(1);
            }
            dateTime = dateTime.AddSeconds(dateTime.Second * -1);

            return dateTime;
        }

        public string GetScore()
        {
            return this.Score.HasValue ? Convert.ToString(this.Score) : null;
        }

        public string ToTableString(string url)
        {
            return (string.Format("<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td><td>{5}</td><td>{6}</td><td>{7}</td><td>{8}</td><td>{9}</td><td>{10}</td><td>{11}</td><td>{12}</td><td><a href='{13}courses/details/{14}'>View</a></td></tr>",
                        this.CitiId, this.FirstName, this.LastName, this.EmailAddress, this.NetId, this.RegistrationDate, this.CourseName, this.CitiCourseId, this.Group, this.Score, this.CompletionDate, this.ExpirationDate, this.InstitutionalEmailAddress, url, this.ID));
        }

        public static Action<TableRowTag> GetTableHeaderRow()
        {
            return row =>
            {
                row.Header("CitiId");
                row.Header("First Name");
                row.Header("Last Name");
                row.Header("Email Address");
                row.Header("NetId");
                row.Header("Registration Date");
                row.Header("Course Name");
                row.Header("Citi Course ID");
                row.Header("Group");
                row.Header("Score");
                row.Header("Completion Date");
                row.Header("Expiration Date");
                row.Header("Institutional Email Address");
                row.Header("Link");
            };
        }

        public Action<TableRowTag> ToTableRow()
        {
            return row =>
            {
                row.Cell(this.CitiId);
                row.Cell(this.FirstName);
                row.Cell(this.LastName);
                row.Cell(this.EmailAddress);
                row.Cell(this.NetId);
                row.Cell(GetRegistrationDate().ToString("yyyy-MM-dd HH:mm:ss"));
                row.Cell(this.CourseName);
                row.Cell(this.CitiCourseId);
                row.Cell(this.Group);
                row.Cell(GetScore());
                row.Cell(GetCompletionDate().ToString("yyyy-MM-dd HH:mm:ss"));
                row.Cell(GetExpirationDate().ToString("yyyy-MM-dd HH:mm:ss"));
                row.Cell(this.InstitutionalEmailAddress);
                row.Cell("_link_");
            };
        }
    }
}
