using System;
using System.Collections.Generic;
using System.Text;
using TrainingDownloader.exceptions;
using TrainingDownloader.models;
using TrainingDownloader.models.entities;
using TrainingDownloader.repositories;
using System.Linq;
using ISULogger;
using TrainingDownloader.configurations;
using TrainingDownloader.services.interfaces;

namespace TrainingDownloader.services
{
    public class LearnerWebService : ILearnerWebServices
    {
        private ILearnerWebRepository learnerWebRepository;
        private IVendorUserService vendorUserService;

        public LearnerWebService(ILearnerWebRepository learnerWebRepository, IVendorUserService vendorUserService)
        {
            this.learnerWebRepository = learnerWebRepository;
            this.vendorUserService = vendorUserService;
        }

        public History CreateHistoryRecord(VendorRecord vRecord)
        {


            History history = new History
            {
                LearnerId = vRecord.UnivId,
                CourseId = vRecord.LearnerWebCourseId,
                Title = vRecord.VendorCourseName,
                Status = "f",
                EnrollmentDate = vRecord.RegistrationDate,
                Score = vRecord.Score,
                CompletionStatusId = "R",
                StatusDate = vRecord.CompletionDate,
                DateExpires = vRecord.ExpirationDate,
                PassingScore = vRecord.PassingScore,
                DateTimeStamp = vRecord.GetDateTimeStamp()
            };
            history.SetImportId(vRecord.ID);
            return history;
        }

        public string FindCourseId(VendorRecord vRecord)
        {
            Courses course = learnerWebRepository.GetCourseByVendorCourseId(vRecord.VendorCourseId);
            string CourseId = course != null ? course.CourseId : null;
            if (string.IsNullOrEmpty(CourseId))
            {
                learnerWebRepository.InsertCourse(new IsuVendorCourses
                {
                    VendorCourseId = vRecord.VendorCourseId,
                    Source = 1,
                    DateCreated = DateTime.Now,
                    DateUpdated = DateTime.Now
                });
                throw new UnknownCourseException(string.Format("Unknown course {0}", vRecord.VendorCourseId));
            }
            else
            {
                return CourseId;
            }
        }

        public string FindUser(VendorRecord vRecord)
        {
            Learners learner = learnerWebRepository.GetLearnerByVendorId(vRecord.VendorUserId);
            vRecord.UnivId = learner != null ? learner.LearnerId : null;
            if (string.IsNullOrEmpty(vRecord.UnivId))
            {
                if (IsValid(vRecord))
                {
                    if (!string.IsNullOrEmpty(vRecord.NetId))
                    {
                        learner = learnerWebRepository.GetLearnerByNetId(vRecord.NetId);
                        vRecord.UnivId = learner != null ? learner.LearnerId : null;
                    }

                    if (string.IsNullOrEmpty(vRecord.UnivId))
                    {
                        learner = learnerWebRepository.GetLearnerByEmail(vRecord.EmailAddress);
                        vRecord.UnivId = learner != null ? learner.LearnerId : null;

                        if (string.IsNullOrEmpty(vRecord.UnivId))
                        {
                            throw new UnknownUserException(string.Format("Unable to find user: CitiId={0}, EmailAddress={1}", vRecord.ID, vRecord.EmailAddress));
                        }
                    }
                }
                else
                {
                    throw new InvalidUserException(string.Format("User is set as invalid: CitiId={0}, EmailAddress={1}", vRecord.ID, vRecord.EmailAddress));
                }
            }

            if (!string.IsNullOrEmpty(vRecord.UnivId))
            {
                VendorUser vendorUser = vendorUserService.CreateVendorUser(vRecord);
                learnerWebRepository.UpdateOrInsertVendorUser(vendorUser);
            }

            return vRecord.UnivId;
        }

        public History GetHistoryByVendorIdCourseIdDate(VendorRecord vRecord)
        {
            History history = learnerWebRepository.GetHistoryRecordByLearnerCourseDate(vRecord.UnivId, vRecord.LearnerWebCourseId, vRecord.GetCompletionDate());
            if (history != null)
            {
                IsuImportHistory isuImportHistory = learnerWebRepository.GetImportHistory(vRecord.VendorUserId, vRecord.VendorCourseId, vRecord.GetCompletionDate());
                isuImportHistory.CurriculaId = history.CurriculaId;
                learnerWebRepository.UpdateImportHistoryWithCurriculaId(isuImportHistory);
            }

            return history;
        }

        public History GetHistoryByCurriculaId(VendorRecord vRecord)
        {
            IsuImportHistory isuImportHistory = learnerWebRepository.GetImportHistory(vRecord.VendorUserId, vRecord.VendorCourseId, vRecord.GetCompletionDate());
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

        public IsuImportHistory InsertImportHistory(VendorRecord vRecord)
        {
            IsuImportHistory isuImportHistory = new IsuImportHistory
            {
                VendorUserId = vRecord.VendorUserId,
                FirstName = vRecord.FirstName,
                LastName = vRecord.LastName,
                EmailAddress = vRecord.EmailAddress,
                RegistrationDate = vRecord.RegistrationDate,
                CourseName = vRecord.VendorCourseName,
                StageNumber = vRecord.StageNumber == null ? (byte?)null : byte.Parse(vRecord.StageNumber.ToString()),
                StageDescription = vRecord.StageDescription,
                CompletionReportNum = vRecord.CompletionReportNum,
                CompletionDate = vRecord.GetCompletionDate(),
                Score = vRecord.Score,
                PassingScore = vRecord.PassingScore,
                ExpirationDate = vRecord.ExpirationDate,
                VendorCourseId = vRecord.VendorCourseId,
                Name = vRecord.Name,
                UserName = vRecord.UserName,
                InstitutionalEmailAddress = vRecord.InstitutionalEmailAddress,
                EmployeeNumber = vRecord.EmployeeNumber,
                Verified = vRecord.Verified,
                IsValid = vRecord.IsValid,
                Inserted = false,
                Source = 1
            };

            return learnerWebRepository.InsertAppTrainingRecordHistory(isuImportHistory);
        }

        public bool IsValid(VendorRecord vRecord)
        {
            VendorUser vendorUser = learnerWebRepository.GetVendorUser(vRecord.VendorUserId);
            if (vendorUser == null || !vendorUser.Valid.HasValue)
            {
                return true;
            }
            return vendorUser.Valid.Value;
        }

        public bool IsVerified(VendorRecord vRecord)
        {
            IsuImportHistory isuImportHistory = learnerWebRepository.GetImportHistory(vRecord.VendorUserId, vRecord.VendorCourseId, vRecord.GetCompletionDate());

            if (isuImportHistory == null || !isuImportHistory.Verified.HasValue)
            {
                return false;
            }
            return isuImportHistory.Verified.Value;
        }

        public void UpdateImportHistoryWithCurriculaId(VendorRecord vRecord, History history)
        {
            IsuImportHistory isuImportHistory = learnerWebRepository.GetImportHistory(vRecord.VendorUserId, vRecord.VendorCourseId, vRecord.GetCompletionDate());
            isuImportHistory.CurriculaId = history.CurriculaId;
            learnerWebRepository.UpdateImportHistoryWithCurriculaId(isuImportHistory);
        }
    }
}
