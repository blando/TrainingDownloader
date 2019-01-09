using TrainingDownloader.models;
using TrainingDownloader.models.entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace TrainingDownloader.repositories
{
    public interface ILearnerWebRepository
    {
        void UpdateVendorIdToLearner(Learners leanrers);
        int InsertCourse(IsuVendorCourses isuVendorCourse);
        int InsertTrainingRecord(History history);
        IsuImportHistory SetInsertedForImportRecord(int id);
        void UpdateAppTrainingRecordIsInserted(IsuImportHistory isuImportHistory);
        IsuImportHistory InsertAppTrainingRecordHistory(IsuImportHistory isuImportHistory);
        History GetHistoryRecordByCurriculaId(int curriculaId);
        History GetHistoryRecordByLearnerCourseDate(string univId, string courseId, DateTime CompletedDate);
        VendorUser GetVendorUser(string id);
        Learners GetLearnerByEmail(string Email);
        Learners GetLearnerByNetId(string NetId);
        Learners GetLearnerByVendorId(string VendorUserId);
        Courses GetCourseByVendorCourseId(string VendorCourseId);
        IsuImportHistory GetImportHistory(string VendorId, string VendorCourseId, DateTime CompletedDate);
        void UpdateOrInsertVendorUser(VendorUser vendorUser);
        void UpdateImportHistoryWithCurriculaId(IsuImportHistory isuImportHistory);
    }
}
