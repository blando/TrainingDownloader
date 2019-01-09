using System;
using System.Collections.Generic;
using System.Text;
using TrainingDownloader.models.entities;
using System.Linq;
using TrainingDownloader.configurations;
using TrainingDownloader.exceptions;
using TrainingDownloader.models;

namespace TrainingDownloader.repositories
{
    public class LearnerWebRepository : ILearnerWebRepository
    {
        private LWEBIAStateContext db;
        private ApplicationConfiguration config { get; set; }

        // Tables to cache
        private List<IsuImportHistory> isuImportHistories;
        private List<VendorUser> vendorUsers;

        public LearnerWebRepository(LWEBIAStateContext db, ApplicationConfiguration config)
        {
            this.db = db;
            this.config = config;
            this.isuImportHistories = db.IsuImportHistory.ToList();

            switch(config.applicationType)
            {
                case CommandLineConfiguration.ApplicationType.Citi:
                    this.vendorUsers = db.IsuCitiLwLearners.ToList<VendorUser>();
                    break;
                case CommandLineConfiguration.ApplicationType.Aalas:
                    this.vendorUsers = db.IsuAalasLwLearners.ToList<VendorUser>();
                    break;
            }
        }

        public IsuImportHistory InsertAppTrainingRecordHistory(IsuImportHistory isuImportHistory)
        {
            IsuImportHistory existingIsuImportHistory = isuImportHistories.Where(h => h.VendorUserId == isuImportHistory.VendorUserId
                && h.VendorCourseId == isuImportHistory.VendorCourseId
                && h.CompletionDate == isuImportHistory.CompletionDate).FirstOrDefault();
            if (existingIsuImportHistory == null)
            {
                db.IsuImportHistory.Add(isuImportHistory);
                db.SaveChanges();
                isuImportHistories.Add(isuImportHistory);
                return isuImportHistory;
            }
            return existingIsuImportHistory;
        }

        public void UpdateVendorIdToLearner(Learners learner)
        {
            db.Learners.Update(learner);
            db.SaveChanges();
        }

        public int InsertCourse(IsuVendorCourses isuVendorCourse)
        {
            db.IsuCitiLwCourses.Add(isuVendorCourse);
            db.SaveChanges();
            return isuVendorCourse.Id;
        }

        public int InsertTrainingRecord(History history)
        {
            db.History.Add(history);
            db.SaveChanges();
            return history.CurriculaId;
        }

        public IsuImportHistory SetInsertedForImportRecord(int id)
        {
            IsuImportHistory isuImportHistory = db.IsuImportHistory.Find(id);
            isuImportHistory.Inserted = true;
            isuImportHistory.Verified = true;
            db.IsuImportHistory.Update(isuImportHistory);
            db.SaveChanges();
            return isuImportHistory;
        }

        public void UpdateAppTrainingRecordIsInserted(IsuImportHistory isuImportHistory)
        {
            isuImportHistory.Inserted = true;
            db.IsuImportHistory.Update(isuImportHistory);
            db.SaveChanges();
            isuImportHistories.Add(isuImportHistory);
        }

        public History GetHistoryRecordByCurriculaId(int curriculaId)
        {
            return db.History.Find(curriculaId);
        }

        public VendorUser GetVendorUser(string id)
        {
            return vendorUsers.Where(l => l.GetVendorLearnerId() == id).SingleOrDefault();
        }

        public Learners GetLearnerByEmail(string Email)
        {
            return db.Learners.Where(l => l.Email == Email).FirstOrDefault();
        }

        public Learners GetLearnerByNetId(string NetId)
        {
            return db.Learners.Where(l => l.UserId == NetId).FirstOrDefault();
        }

        public Learners GetLearnerByVendorId(string VendorUserId)
        {
            if (config.applicationType == CommandLineConfiguration.ApplicationType.Citi)
            {
                return db.Learners.Where(l => l.User4 == VendorUserId).FirstOrDefault();
            }
            else
            {
                return db.Learners.Where(l => l.User6 == VendorUserId).FirstOrDefault();
            }

        }

        public Courses GetCourseByVendorCourseId(string VendorCourseId)
        {
            return db.Courses.Where(c => c.User2 == VendorCourseId).FirstOrDefault();
        }

        public IsuImportHistory GetImportHistory(string VendorUserId, string VendorCourseId, DateTime CompletedDate)
        {
            return isuImportHistories.Where(h => h.VendorUserId == VendorUserId && h.VendorCourseId == VendorCourseId && h.CompletionDate == CompletedDate).FirstOrDefault();
        }

        public void UpdateOrInsertVendorUser(VendorUser vendorUser)
        {
            if (config.applicationType == CommandLineConfiguration.ApplicationType.Citi)
            {
                IsuCitiLwLearners isuCitiLwLearner = (IsuCitiLwLearners)vendorUser;

                IsuCitiLwLearners isuCitiLwLearnerCache = db.IsuCitiLwLearners.Find(isuCitiLwLearner.CitiLearnerId);
                if (isuCitiLwLearnerCache != null)
                {
                    isuCitiLwLearnerCache.LwLearnerId = isuCitiLwLearner.LwLearnerId;
                    isuCitiLwLearnerCache.Valid = isuCitiLwLearner.Valid;
                    isuCitiLwLearnerCache.DateUpdated = DateTime.Now;
                    db.IsuCitiLwLearners.Update(isuCitiLwLearnerCache);
                }
                else
                {
                    db.IsuCitiLwLearners.Add(isuCitiLwLearner);
                }
                db.SaveChanges();
                vendorUsers.Add(isuCitiLwLearner);
            }
            else if (config.applicationType == CommandLineConfiguration.ApplicationType.Aalas)
            {
                IsuAalasLwLearners isuAalasLwLearner = (IsuAalasLwLearners)vendorUser;

                IsuAalasLwLearners isuAalasLwLearnerCache = db.IsuAalasLwLearners.Find(isuAalasLwLearner.AalasLearnerId);
                if (isuAalasLwLearnerCache != null)
                {
                    isuAalasLwLearnerCache.LwLearnerId = isuAalasLwLearner.LwLearnerId;
                    isuAalasLwLearnerCache.Valid = isuAalasLwLearner.Valid;
                    isuAalasLwLearnerCache.DateUpdated = DateTime.Now;
                    db.IsuAalasLwLearners.Update(isuAalasLwLearnerCache);
                }
                else
                {
                    db.IsuAalasLwLearners.Add(isuAalasLwLearner);
                }
                db.SaveChanges();
                vendorUsers.Add(isuAalasLwLearner);
            }
            
        }

        public History GetHistoryRecordByLearnerCourseDate(string univId, string courseId, DateTime CompletedDate)
        {
            return db.History.Where(h => h.LearnerId == univId && h.CourseId == courseId && h.StatusDate == CompletedDate).FirstOrDefault();
        }

        public void UpdateImportHistoryWithCurriculaId(IsuImportHistory isuImportHistory)
        {
            db.IsuImportHistory.Update(isuImportHistory);
            db.SaveChanges();
        }

    }
}
