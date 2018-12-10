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
            if (this.CompletionDate == null)
            {
                return DateTime.MinValue;
            }

            DateTime dt1 = (DateTime)this.CompletionDate;

            if (dt1.Second > 29)
            {
                dt1 = dt1.AddMinutes(1);
            }
            dt1 = dt1.AddSeconds(dt1.Second * -1);

            return dt1;
        }

        public string ToTableString(string url)
        {
            return (string.Format("<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td><td>{5}</td><td>{6}</td><td>{7}</td><td>{8}</td><td>{9}</td><td>{10}</td><td>{11}</td><td>{12}</td><td><a href='{13}courses/details/{14}'>View</a></td></tr>",
                        this.CitiId, this.FirstName, this.LastName, this.EmailAddress, this.NetId, this.RegistrationDate, this.CourseName, this.GroupId, this.Group, this.Score, this.CompletionDate, this.ExpirationDate, this.InstitutionalEmailAddress, url, this._courseID));
        }
    }
}
