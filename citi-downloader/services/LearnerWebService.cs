using System;
using System.Collections.Generic;
using System.Text;
using CitiDownloader.exceptions;
using CitiDownloader.models;
using CitiDownloader.models.entities;
using CitiDownloader.repositories;
using System.Linq;

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
            return new History
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
                PassingScore = citiRecord.PassingScore
            };
        }

        public string FindCourseId(CitiRecord citiRecord)
        {
            Courses course = learnerWebRepository.GetCourseByCitiCourseId(citiRecord.GroupId);
            string CourseId = course != null ? course.CourseId : null;
            if (string.IsNullOrEmpty(CourseId))
            {
                learnerWebRepository.InsertCourse(new IsuCitiLwCourses
                {
                    CitiCourseId = citiRecord.GroupId,
                    Source = 1,
                    DateCreated = DateTime.Now,
                    DateUpdated = DateTime.Now
                });
                return null;
            }
            else
            {
                return CourseId;
            }
        }

        public string FindUser(CitiRecord citiRecord)
        {
            IsuCitiLwLearners isuCitiLwLearner = learnerWebRepository.GetIsuCitiLwLearner(citiRecord.CitiId);
            string univId = isuCitiLwLearner != null ? isuCitiLwLearner.LwLearnerId : null;
            if (!string.IsNullOrEmpty(univId))
            {
                return univId;
            }

            Learners learner = learnerWebRepository.GetLearnerByCitiId(citiRecord.CitiId);
            univId = learner != null ? learner.LearnerId : null;
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
                    CitiLearnerId = citiRecord.CitiId,
                    LwLearnerId = univId,
                    DateCreated = DateTime.Now,
                    DateUpdated = DateTime.Now,
                    Valid = true,
                    CitiLastName = citiRecord.LastName
                });
            }

            return univId;
        }

        public History GetHistoryByCurriculaId(CitiRecord citiRecord)
        {
            IsuImportHistory isuImportHistory = learnerWebRepository.GetImportHistory(citiRecord.CitiId, citiRecord.GroupId, citiRecord.GetCompletionDate());
            if (isuImportHistory == null || isuImportHistory.CurriculaId == 0)
            {
                return null;
            }

            History history = learnerWebRepository.GetHistoryRecordByCurriculaId(isuImportHistory.CurriculaId);
            return history;
        }

        public void InsertHistory(History history)
        {
            History historyRecord = learnerWebRepository.GetHistoryRecordByLearnerCourseDate(history.LearnerId, history.CourseId, history.GetStatusDate());
            if (historyRecord == null)
            {
                learnerWebRepository.InsertTrainingRecord(history);
            }
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

            learnerWebRepository.InsertAppTrainingRecordHistory(isuImportHistory);

            return isuImportHistory;
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

        
    }
}
