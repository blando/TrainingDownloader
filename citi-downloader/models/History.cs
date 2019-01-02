using CitiDownloader.models.entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CitiDownloader.models.entities
{
    public partial class History
    {
        public string Enrollment_Date
        {
            get
            {
                return this.EnrollmentDate.HasValue ? RoundDateTime(this.EnrollmentDate).Value.ToString("yyyy-MM-dd HH:mm:ss") : "NULL";
            }
        }
        public string Status_Date
        {
            get
            {
                return this.StatusDate.HasValue ? RoundDateTime(this.StatusDate).Value.ToString("yyyy-MM-dd HH:mm:ss") : "NULL";
            }
        }
        public string Date_Time_Stamp
        {
            get
            {
                return GetDateTimeStamp();
            }
        }
        public string date_expires
        {
            get
            {
                return this.DateExpires.HasValue ? RoundDateTime(this.DateExpires).Value.ToString("yyyy-MM-dd HH:mm:ss") : "NULL";
            }
        }
        private bool IsValid { get; set; }
        private string NetId { get; set; }
        private string importId { get; set; }

        public override string ToString()
        {
            return String.Format(@"DateTimeStamp={1}{0}" +
                    "Learner_Id={2}{0}" +
                    "Course_id={3}{0}" +
                    "Title={4}{0}" +
                    "Status={5}{0}" +
                    "EnrollmentDate={6}{0}" +
                    "Score={7}{0}" +
                    "CompletionStatusId={8}{0}" +
                    "Status_Date={9}{0}" +
                    "date_expires={10}{0}" +
                    "PassingScore={11}{0}",
                Environment.NewLine, this.DateTimeStamp, this.LearnerId, this.CourseId, this.Title, this.Status, this.EnrollmentDate, this.Score.ToString(),
                this.CompletionStatusId, this.Status_Date, this.date_expires, this.PassingScore.ToString());
        }
        public string GetNetId()
        {
            return this.NetId;
        }
        public string GetDateTimeStamp()
        {
            return this.StatusDate.HasValue ? ((DateTime)this.StatusDate).ToString("yyMMddHHmmss") : DateTime.Now.ToString("yyMMddHHmmss");
        }

        public bool GetIsValid()
        {
            return this.IsValid;
        }

        public DateTime? GetDateExpires()
        {
            return this.DateExpires;
        }

        public DateTime? GetEnrollmentDate()
        {
            return this.EnrollmentDate;
        }

        public DateTime GetStatusDate()
        {
            if (this.StatusDate.HasValue)
            {
                return this.StatusDate.Value;
            }
            return DateTime.MinValue;
        }

        private DateTime? RoundDateTime(DateTime? dt)
        {
            if (dt == null)
            {
                return null;
            }

            DateTime dt1 = (DateTime)dt;

            if (dt1.Second > 29)
            {
                dt1 = dt1.AddMinutes(1);
            }
            dt1 = dt1.AddSeconds(dt1.Second * -1);

            return dt1;
        }
        public void SetImportId(string id)
        {
            this.importId = id;
        }
        public string GetImportId()
        {
            return this.importId;
        }


    }
}
