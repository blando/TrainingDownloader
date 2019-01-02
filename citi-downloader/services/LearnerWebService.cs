using System;
using System.Collections.Generic;
using System.Text;
using CitiDownloader.exceptions;
using CitiDownloader.models;
using CitiDownloader.models.entities;
using CitiDownloader.repositories;
using System.Linq;
using ISULogger;

namespace CitiDownloader.services
{
    public class LearnerWebService : ILearnerWebServices
    {
        private ILearnerWebRepository learnerWebRepository;

        public LearnerWebService(ILearnerWebRepository learnerWebRepository)
        {
            this.learnerWebRepository = learnerWebRepository;
        }

        public History CreateHistoryRecord(CitiRecord citiRecord)
        {


            History history = new History
            {
                LearnerId = citiRecord.UnivId,
                CourseId = citiRecord.CourseId,
                Title = citiRecord.CourseName,
                Status = "f",
                EnrollmentDate = citiRecord.RegistrationDate,
                Score = citiRecord.Score,
                CompletionStatusId = "R",
                StatusDate = citiRecord.CompletionDate,
                DateExpires = citiRecord.ExpirationDate,
                PassingScore = citiRecord.PassingScore,
                DateTimeStamp = citiRecord.GetDateTimeStamp()
            };
            history.SetImportId(citiRecord.ID);
            return history;
        }

        public string FindCourseId(CitiRecord citiRecord)
        {
            Courses course = learnerWebRepository.GetCourseByCitiCourseId(citiRecord.CitiCourseId);
            string CourseId = course != null ? course.CourseId : null;
            if (string.IsNullOrEmpty(CourseId))
            {
                learnerWebRepository.InsertCourse(new IsuCitiLwCourses
                {
                    CitiCourseId = citiRecord.CitiCourseId,
                    Source = 1,
                    DateCreated = DateTime.Now,
                    DateUpdated = DateTime.Now
                });
                throw new UnknownCourseException(string.Format("Unknown course {0}", citiRecord.CitiCourseId));
            }
            else
            {
                return CourseId;
            }
        }

        public string FindUser(CitiRecord citiRecord)
        {
            Learners learner = learnerWebRepository.GetLearnerByCitiId(citiRecord.CitiId);
            string univId = learner != null ? learner.LearnerId : null;
            if (string.IsNullOrEmpty(univId))
            {
                if (IsValid(citiRecord))
                {
                    if (!string.IsNullOrEmpty(citiRecord.NetId))
                    {
                        learner = learnerWebRepository.GetLearnerByNetId(citiRecord.NetId);
                        univId = learner != null ? learner.LearnerId : null;
                    }

                    if (string.IsNullOrEmpty(univId))
                    {
                        learner = learnerWebRepository.GetLearnerByEmail(citiRecord.EmailAddress);
                        univId = learner != null ? learner.LearnerId : null;

                        if (string.IsNullOrEmpty(univId))
                        {
                            throw new UnknownUserException(string.Format("Unable to find user: CitiId={0}, EmailAddress={1}", citiRecord.ID, citiRecord.EmailAddress));
                        }
                    }
                }
                else
                {
                    throw new InvalidUserException(string.Format("User is set as invalid: CitiId={0}, EmailAddress={1}", citiRecord.ID, citiRecord.EmailAddress));
                }
            }

            if (!string.IsNullOrEmpty(univId))
            {
                learnerWebRepository.UpdateOrInsertISUCitiLwLearner(new IsuCitiLwLearners
                {
                    LwLearnerId = univId,
                    Valid = true,
                    CitiLastName = citiRecord.LastName,
                    CitiLearnerId = citiRecord.CitiId,
                    DateUpdated = DateTime.Now,
                    DateCreated = DateTime.Now
                });
            }

            return univId;
        }

        public History GetHistoryByCitiIdCourseIdDate(CitiRecord citiRecord)
        {
            History history = learnerWebRepository.GetHistoryRecordByLearnerCourseDate(citiRecord.UnivId, citiRecord.CourseId, citiRecord.GetCompletionDate());
            if (history != null)
            {
                IsuImportHistory isuImportHistory = learnerWebRepository.GetImportHistory(citiRecord.CitiId, citiRecord.CitiCourseId, citiRecord.GetCompletionDate());
                isuImportHistory.CurriculaId = history.CurriculaId;
                learnerWebRepository.UpdateImportHistoryWithCurriculaId(isuImportHistory);
            }

            return history;
        }

        public History GetHistoryByCurriculaId(CitiRecord citiRecord)
        {
            IsuImportHistory isuImportHistory = learnerWebRepository.GetImportHistory(citiRecord.CitiId, citiRecord.CitiCourseId, citiRecord.GetCompletionDate());
            if (isuImportHistory == null || !isuImportHistory.CurriculaId.HasValue)
            {
                return null;
            }

            History history = learnerWebRepository.GetHistoryRecordByCurriculaId(isuImportHistory.CurriculaId.Value);
            return history;
            
        }

        public int InsertHistory(History history, out bool inserted)
        {
            History historyRecord = learnerWebRepository.GetHistoryRecordByLearnerCourseDate(history.LearnerId, history.CourseId, history.GetStatusDate());
            int curriculaId;
            if (historyRecord == null)
            {
                curriculaId = learnerWebRepository.InsertTrainingRecord(history);
                inserted = true;
                return curriculaId; ;
            }
            curriculaId = historyRecord.CurriculaId;

            inserted = false;

            return curriculaId;
        }

        public IsuImportHistory SetInsertedForImportRecord(int id)
        {
            return learnerWebRepository.SetInsertedForImportRecord(id);
        }

        public IsuImportHistory InsertImportHistory(CitiRecord citiRecord)
        {
            IsuImportHistory isuImportHistory = new IsuImportHistory
            {
                CitiId = citiRecord.CitiId,
                FirstName = citiRecord.FirstName,
                LastName = citiRecord.LastName,
                EmailAddress = citiRecord.EmailAddress,
                RegistrationDate = citiRecord.RegistrationDate,
                CourseName = citiRecord.CourseName,
                StageNumber = byte.Parse(citiRecord.StageNumber.ToString()),
                StageDescription = citiRecord.StageDescription,
                CompletionReportNum = citiRecord.CompletionReportNum,
                CompletionDate = citiRecord.GetCompletionDate(),
                Score = citiRecord.Score,
                PassingScore = citiRecord.PassingScore,
                ExpirationDate = citiRecord.ExpirationDate,
                GroupName = citiRecord.Group,
                CitiCourseId = citiRecord.GroupId,
                Name = citiRecord.Name,
                UserName = citiRecord.UserName,
                InstitutionalEmailAddress = citiRecord.InstitutionalEmailAddress,
                EmployeeNumber = citiRecord.EmployeeNumber,
                Verified = citiRecord.Verified,
                IsValid = citiRecord.IsValid,
                Inserted = false,
                Source = 1
            };

            return learnerWebRepository.InsertAppTrainingRecordHistory(isuImportHistory);
        }

        public bool IsValid(CitiRecord citiRecord)
        {
            IsuCitiLwLearners isuCitiLwLearners = learnerWebRepository.GetIsuCitiLwLearner(citiRecord.CitiId);
            if (isuCitiLwLearners == null || !isuCitiLwLearners.Valid.HasValue)
            {
                return true;
            }
            return isuCitiLwLearners.Valid.Value;
        }

        public bool IsVerified(CitiRecord citiRecord)
        {
            IsuImportHistory isuImportHistory = learnerWebRepository.GetImportHistory(citiRecord.CitiId, citiRecord.GroupId, citiRecord.GetCompletionDate());

            if (isuImportHistory == null || !isuImportHistory.Verified.HasValue)
            {
                return false;
            }
            return isuImportHistory.Verified.Value;
        }

        public void UpdateImportHistoryWithCurriculaId(CitiRecord citiRecord, History history)
        {
            IsuImportHistory isuImportHistory = learnerWebRepository.GetImportHistory(citiRecord.CitiId, citiRecord.GroupId, citiRecord.GetCompletionDate());
            isuImportHistory.CurriculaId = history.CurriculaId;
            learnerWebRepository.UpdateImportHistoryWithCurriculaId(isuImportHistory);
        }
    }
}
