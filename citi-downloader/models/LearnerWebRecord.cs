using CitiDownloader.models.entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CitiDownloader.models.entities
{
    public partial class History
    {
        public string Learner_Id { get; set; }
        public string Course_Id { get; set; }
        private DateTime? _EnrollmentDate { get; set; }
        private DateTime? _Status_Date { get; set; }
        public string Status_Date
        {
            get
            {
                return this._Status_Date == null ? "NULL" : this._Status_Date.Value.ToString("yyyy-MM-dd HH:mm:00");
            }
        }
        private DateTime? _date_expires { get; set; }
        public string date_expires
        {
            get
            {
                return this._date_expires == null ? "NULL" : this._date_expires.Value.ToString("yyyy-MM-dd HH:mm:00");
            }
        }
        private bool IsValid { get; set; }
        private string NetId { get; set; }


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
                Environment.NewLine, this.DateTimeStamp, this.Learner_Id, this.Course_Id, this.Title, this.Status, this.EnrollmentDate, this.Score.ToString(),
                this.CompletionStatusId, this.Status_Date, this.date_expires, this.PassingScore.ToString());
        }
        public string GetNetId()
        {
            return this.NetId;
        }
        public string GetDateTimeStamp()
        {
            return ((DateTime)this._Status_Date).ToString("yyMMddHHmmss");
        }

        public bool GetIsValid()
        {
            return this.IsValid;
        }

        public DateTime? GetDateExpires()
        {
            return this._date_expires;
        }

        public DateTime? GetEnrollmentDate()
        {
            return this._EnrollmentDate;
        }

        public DateTime GetStatusDate()
        {
            if (this._Status_Date.HasValue)
            {
                return this._Status_Date.Value;
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
    }
}
