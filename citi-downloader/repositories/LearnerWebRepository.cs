using System;
using System.Collections.Generic;
using System.Text;
using CitiDownloader.models.entities;
using System.Linq;

namespace CitiDownloader.repositories
{
    public class LearnerWebRepository : ILearnerWebRepository
    {
        private LWEBIAStateContext db;

        // Tables to cache
        private List<IsuImportHistory> isuImportHistories;
        private List<IsuCitiLwLearners> isuCitiLwLearners;

        public LearnerWebRepository(LWEBIAStateContext db)
        {
            this.db = db;
            this.isuImportHistories = db.IsuImportHistory.ToList();
            this.isuCitiLwLearners = db.IsuCitiLwLearners.ToList();
        }

        public IsuImportHistory InsertAppTrainingRecordHistory(IsuImportHistory isuImportHistory)
        {
            IsuImportHistory existingIsuImportHistory = isuImportHistories.Where(h => h.CitiId == isuImportHistory.CitiId
                && h.CitiCourseId == isuImportHistory.CitiCourseId
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

        public void UpdateCitiIdToLearner(Learners learner)
        {
            db.Learners.Update(learner);
            db.SaveChanges();
        }

        public int InsertCourse(IsuCitiLwCourses isuCitiLwCourse)
        {
            db.IsuCitiLwCourses.Add(isuCitiLwCourse);
            db.SaveChanges();
            return isuCitiLwCourse.Id;
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

        public IsuCitiLwLearners GetIsuCitiLwLearner(string CitiId)
        {
            return isuCitiLwLearners.Where(l => l.CitiLearnerId == CitiId).SingleOrDefault();
        }

        public Learners GetLearnerByEmail(string Email)
        {
            return db.Learners.Where(l => l.Email == Email).FirstOrDefault();
        }

        public Learners GetLearnerByNetId(string NetId)
        {
            return db.Learners.Where(l => l.UserId == NetId).FirstOrDefault();
        }

        public Learners GetLearnerByCitiId(string CitiId)
        {
            return db.Learners.Where(l => l.User4 == CitiId).FirstOrDefault();
        }

        public Courses GetCourseByCitiCourseId(string CitiCourseId)
        {
            return db.Courses.Where(c => c.User2 == CitiCourseId).FirstOrDefault();
        }

        public IsuImportHistory GetImportHistory(string CitiId, string CitiCourseId, DateTime CompletedDate)
        {
            return isuImportHistories.Where(h => h.CitiId == CitiId && h.CitiCourseId == CitiCourseId && h.CompletionDate == CompletedDate).FirstOrDefault();
        }

        public void UpdateOrInsertISUCitiLwLearner(IsuCitiLwLearners isuCitiLwLearner)
        {
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
            isuCitiLwLearners.Add(isuCitiLwLearner);
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
