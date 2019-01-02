using CitiDownloader.models;
using CitiDownloader.models.entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CitiDownloader.repositories
{
    public interface ILearnerWebRepository
    {
        void UpdateCitiIdToLearner(Learners leanrers);
        int InsertCourse(IsuCitiLwCourses isuCitiLwCourse);
        int InsertTrainingRecord(History history);
        IsuImportHistory SetInsertedForImportRecord(int id);
        void UpdateAppTrainingRecordIsInserted(IsuImportHistory isuImportHistory);
        IsuImportHistory InsertAppTrainingRecordHistory(IsuImportHistory isuImportHistory);
        History GetHistoryRecordByCurriculaId(int curriculaId);
        History GetHistoryRecordByLearnerCourseDate(string univId, string courseId, DateTime CompletedDate);
        IsuCitiLwLearners GetIsuCitiLwLearner(string CitiId);
        Learners GetLearnerByEmail(string Email);
        Learners GetLearnerByNetId(string NetId);
        Learners GetLearnerByCitiId(string CitiId);
        Courses GetCourseByCitiCourseId(string CitiCourseId);
        IsuImportHistory GetImportHistory(string CitiId, string CitiCourseId, DateTime CompletedDate);
        void UpdateOrInsertISUCitiLwLearner(IsuCitiLwLearners isuCitiLwLearner);
        void UpdateImportHistoryWithCurriculaId(IsuImportHistory isuImportHistory);
    }
}
