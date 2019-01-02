using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using SimpleFixture;
using NUnit.Framework;
using CitiDownloader.configurations;
using CitiDownloader.exceptions;
using CitiDownloader.models.entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using CitiDownloader.repositories;

namespace CitiDownloaderTests.repositories
{
    [TestFixture]
    public class LearnerWebRepositoryTests
    {
        private Mock<LWEBIAStateContext> mockDb { get; set; }
        private Fixture fixture = new Fixture();

        private List<IsuImportHistory> fakeIsuImportHistory { get; set; }
        private List<IsuCitiLwLearners> fakeIsuCitiLwLearners { get; set; }
        private List<IsuCitiLwCourses> fakeIsuCitiLwCourses { get; set; }
        private List<Learners> fakeLearners { get; set; }
        private List<Courses> fakeCourses { get; set; }
        private List<History> fakeHistory { get; set; }

        public LearnerWebRepositoryTests()
        {
            //SetupDbSets();
        }

        private void SetupIsuImportHistory()
        {
            mockDb = new Mock<LWEBIAStateContext>();
            fakeIsuImportHistory = new List<IsuImportHistory>();
            for (int i = 0; i < 5; i++)
            {
                fakeIsuImportHistory.Add(fixture.Generate<IsuImportHistory>());
            }

            var mockIsuImportHistorySet = new Mock<DbSet<IsuImportHistory>>();
            mockIsuImportHistorySet.As<IQueryable<IsuImportHistory>>().Setup(m => m.Provider).Returns(fakeIsuImportHistory.AsQueryable().Provider);
            mockIsuImportHistorySet.As<IQueryable<IsuImportHistory>>().Setup(m => m.Expression).Returns(fakeIsuImportHistory.AsQueryable().Expression);
            mockIsuImportHistorySet.As<IQueryable<IsuImportHistory>>().Setup(m => m.ElementType).Returns(fakeIsuImportHistory.AsQueryable().ElementType);
            mockIsuImportHistorySet.As<IQueryable<IsuImportHistory>>().Setup(m => m.GetEnumerator()).Returns(fakeIsuImportHistory.AsQueryable().GetEnumerator());
            mockIsuImportHistorySet.Setup(f => f.Find(It.IsAny<int>())).Returns((object[] input) => fakeIsuImportHistory.SingleOrDefault(x => x.Id == Convert.ToInt32(input[0])));
            mockIsuImportHistorySet.Setup(f => f.Add(It.IsAny<IsuImportHistory>())).Callback((IsuImportHistory input) => fakeIsuImportHistory.Add(input));
            mockIsuImportHistorySet.Setup(f => f.Update(It.IsAny<IsuImportHistory>())).Callback((IsuImportHistory input) => fakeIsuImportHistory[fakeIsuImportHistory.FindIndex(i => i.CurriculaId == input.CurriculaId)] = input);

            mockDb.Setup(f => f.IsuImportHistory).Returns(mockIsuImportHistorySet.Object);
            mockDb.Setup(f => f.IsuCitiLwLearners).Returns(SetupEmptyIsuCitiLwLearners().Object);
        }

        private void SetupIsuCitiLwLearners()
        {
            mockDb = new Mock<LWEBIAStateContext>();
            fakeIsuCitiLwLearners = new List<IsuCitiLwLearners>();

            for (int i = 0; i < 5; i++)
            {
                fakeIsuCitiLwLearners.Add(fixture.Generate<IsuCitiLwLearners>());
            }

            var mockIsuCitiLwLearnersSet = new Mock<DbSet<IsuCitiLwLearners>>();
            mockIsuCitiLwLearnersSet.As<IQueryable<IsuCitiLwLearners>>().Setup(m => m.Provider).Returns(fakeIsuCitiLwLearners.AsQueryable().Provider);
            mockIsuCitiLwLearnersSet.As<IQueryable<IsuCitiLwLearners>>().Setup(m => m.Expression).Returns(fakeIsuCitiLwLearners.AsQueryable().Expression);
            mockIsuCitiLwLearnersSet.As<IQueryable<IsuCitiLwLearners>>().Setup(m => m.ElementType).Returns(fakeIsuCitiLwLearners.AsQueryable().ElementType);
            mockIsuCitiLwLearnersSet.As<IQueryable<IsuCitiLwLearners>>().Setup(m => m.GetEnumerator()).Returns(fakeIsuCitiLwLearners.AsQueryable().GetEnumerator());
            mockIsuCitiLwLearnersSet.Setup(f => f.Find(It.IsAny<string>())).Returns((object[] input) => fakeIsuCitiLwLearners.SingleOrDefault(x => x.CitiLearnerId == Convert.ToString(input[0])));
            mockIsuCitiLwLearnersSet.Setup(f => f.Add(It.IsAny<IsuCitiLwLearners>())).Callback((IsuCitiLwLearners input) => fakeIsuCitiLwLearners.Add(input));
            mockIsuCitiLwLearnersSet.Setup(f => f.Update(It.IsAny<IsuCitiLwLearners>())).Callback((IsuCitiLwLearners input) => fakeIsuCitiLwLearners[fakeIsuCitiLwLearners.FindIndex(i => i.CitiLearnerId == input.CitiLearnerId)] = input);

            mockDb.Setup(f => f.IsuCitiLwLearners).Returns(mockIsuCitiLwLearnersSet.Object);
            mockDb.Setup(f => f.IsuImportHistory).Returns(SetupEmptyIsuImportHistory().Object);
        }

        private void SetupIsuCitiLwCourses()
        {
            mockDb = new Mock<LWEBIAStateContext>();
            fakeIsuCitiLwCourses = new List<IsuCitiLwCourses>();

            for (int i = 0; i < 5; i++)
            {
                fakeIsuCitiLwCourses.Add(fixture.Generate<IsuCitiLwCourses>());
            }

            var mockIsuCitiLwCoursesSet = new Mock<DbSet<IsuCitiLwCourses>>();
            mockIsuCitiLwCoursesSet.As<IQueryable<IsuCitiLwCourses>>().Setup(m => m.Provider).Returns(fakeIsuCitiLwCourses.AsQueryable().Provider);
            mockIsuCitiLwCoursesSet.As<IQueryable<IsuCitiLwCourses>>().Setup(m => m.Expression).Returns(fakeIsuCitiLwCourses.AsQueryable().Expression);
            mockIsuCitiLwCoursesSet.As<IQueryable<IsuCitiLwCourses>>().Setup(m => m.ElementType).Returns(fakeIsuCitiLwCourses.AsQueryable().ElementType);
            mockIsuCitiLwCoursesSet.As<IQueryable<IsuCitiLwCourses>>().Setup(m => m.GetEnumerator()).Returns(fakeIsuCitiLwCourses.AsQueryable().GetEnumerator());
            mockIsuCitiLwCoursesSet.Setup(f => f.Add(It.IsAny<IsuCitiLwCourses>())).Callback((IsuCitiLwCourses input) => fakeIsuCitiLwCourses.Add(input));

            mockDb.Setup(f => f.IsuCitiLwCourses).Returns(mockIsuCitiLwCoursesSet.Object);
            mockDb.Setup(f => f.IsuCitiLwLearners).Returns(SetupEmptyIsuCitiLwLearners().Object);
            mockDb.Setup(f => f.IsuImportHistory).Returns(SetupEmptyIsuImportHistory().Object);
        }

        private void SetupLearners()
        {
            mockDb = new Mock<LWEBIAStateContext>();
            fakeLearners = new List<Learners>();

            for (int i = 0; i < 5; i++)
            {
                fakeLearners.Add(fixture.Generate<Learners>());
            }

            var mockLearnersSet = new Mock<DbSet<Learners>>();
            mockLearnersSet.As<IQueryable<Learners>>().Setup(m => m.Provider).Returns(fakeLearners.AsQueryable().Provider);
            mockLearnersSet.As<IQueryable<Learners>>().Setup(m => m.Expression).Returns(fakeLearners.AsQueryable().Expression);
            mockLearnersSet.As<IQueryable<Learners>>().Setup(m => m.ElementType).Returns(fakeLearners.AsQueryable().ElementType);
            mockLearnersSet.As<IQueryable<Learners>>().Setup(m => m.GetEnumerator()).Returns(fakeLearners.AsQueryable().GetEnumerator());
            mockLearnersSet.Setup(f => f.Update(It.IsAny<Learners>())).Callback((Learners input) => fakeLearners[fakeLearners.FindIndex(i => i.LearnerId == input.LearnerId)] = input);

            mockDb.Setup(f => f.Learners).Returns(mockLearnersSet.Object);
            mockDb.Setup(f => f.IsuCitiLwLearners).Returns(SetupEmptyIsuCitiLwLearners().Object);
            mockDb.Setup(f => f.IsuImportHistory).Returns(SetupEmptyIsuImportHistory().Object);
        }

        private void SetupCourses()
        {
            mockDb = new Mock<LWEBIAStateContext>();
            fakeCourses = new List<Courses>();

            for (int i = 0; i < 5; i++)
            {
                fakeCourses.Add(fixture.Generate<Courses>());
            }

            var mockCoursesSet = new Mock<DbSet<Courses>>();
            mockCoursesSet.As<IQueryable<Courses>>().Setup(m => m.Provider).Returns(fakeCourses.AsQueryable().Provider);
            mockCoursesSet.As<IQueryable<Courses>>().Setup(m => m.Expression).Returns(fakeCourses.AsQueryable().Expression);
            mockCoursesSet.As<IQueryable<Courses>>().Setup(m => m.ElementType).Returns(fakeCourses.AsQueryable().ElementType);
            mockCoursesSet.As<IQueryable<Courses>>().Setup(m => m.GetEnumerator()).Returns(fakeCourses.AsQueryable().GetEnumerator());

            mockDb.Setup(f => f.Courses).Returns(mockCoursesSet.Object);
            mockDb.Setup(f => f.IsuCitiLwLearners).Returns(SetupEmptyIsuCitiLwLearners().Object);
            mockDb.Setup(f => f.IsuImportHistory).Returns(SetupEmptyIsuImportHistory().Object);
        }

        private void SetupHistory()
        {
            mockDb = new Mock<LWEBIAStateContext>();
            fakeHistory = new List<History>();

            for (int i = 0; i < 5; i++)
            {
                fakeHistory.Add(fixture.Generate<History>());
            }

            var mockHistorySet = new Mock<DbSet<History>>();
            mockHistorySet.As<IQueryable<History>>().Setup(m => m.Provider).Returns(fakeHistory.AsQueryable().Provider);
            mockHistorySet.As<IQueryable<History>>().Setup(m => m.Expression).Returns(fakeHistory.AsQueryable().Expression);
            mockHistorySet.As<IQueryable<History>>().Setup(m => m.ElementType).Returns(fakeHistory.AsQueryable().ElementType);
            mockHistorySet.As<IQueryable<History>>().Setup(m => m.GetEnumerator()).Returns(fakeHistory.AsQueryable().GetEnumerator());
            mockHistorySet.Setup(f => f.Find(It.IsAny<int>())).Returns((object[] input) => fakeHistory.SingleOrDefault(x => x.CurriculaId == Convert.ToInt32(input[0])));
            mockHistorySet.Setup(f => f.Add(It.IsAny<History>())).Callback((History input) => fakeHistory.Add(input));

            mockDb.Setup(f => f.History).Returns(mockHistorySet.Object);
            mockDb.Setup(f => f.IsuCitiLwLearners).Returns(SetupEmptyIsuCitiLwLearners().Object);
            mockDb.Setup(f => f.IsuImportHistory).Returns(SetupEmptyIsuImportHistory().Object);
        }

        private Mock<DbSet<IsuCitiLwLearners>> SetupEmptyIsuCitiLwLearners()
        {
            var mockIsuCitiLwLearnersSet = new Mock<DbSet<IsuCitiLwLearners>>();
            fakeIsuCitiLwLearners = new List<IsuCitiLwLearners>();
            mockIsuCitiLwLearnersSet.As<IQueryable<IsuCitiLwLearners>>().Setup(m => m.Provider).Returns(fakeIsuCitiLwLearners.AsQueryable().Provider);
            mockIsuCitiLwLearnersSet.As<IQueryable<IsuCitiLwLearners>>().Setup(m => m.Expression).Returns(fakeIsuCitiLwLearners.AsQueryable().Expression);
            mockIsuCitiLwLearnersSet.As<IQueryable<IsuCitiLwLearners>>().Setup(m => m.ElementType).Returns(fakeIsuCitiLwLearners.AsQueryable().ElementType);
            mockIsuCitiLwLearnersSet.As<IQueryable<IsuCitiLwLearners>>().Setup(m => m.GetEnumerator()).Returns(fakeIsuCitiLwLearners.AsQueryable().GetEnumerator());

            return mockIsuCitiLwLearnersSet;
        }

        private Mock<DbSet<IsuImportHistory>> SetupEmptyIsuImportHistory()
        {
            var mockIsuImportHistorySet = new Mock<DbSet<IsuImportHistory>>();
            fakeIsuImportHistory = new List<IsuImportHistory>();
            mockIsuImportHistorySet.As<IQueryable<IsuImportHistory>>().Setup(m => m.Provider).Returns(fakeIsuImportHistory.AsQueryable().Provider);
            mockIsuImportHistorySet.As<IQueryable<IsuImportHistory>>().Setup(m => m.Expression).Returns(fakeIsuImportHistory.AsQueryable().Expression);
            mockIsuImportHistorySet.As<IQueryable<IsuImportHistory>>().Setup(m => m.ElementType).Returns(fakeIsuImportHistory.AsQueryable().ElementType);
            mockIsuImportHistorySet.As<IQueryable<IsuImportHistory>>().Setup(m => m.GetEnumerator()).Returns(fakeIsuImportHistory.AsQueryable().GetEnumerator());

            return mockIsuImportHistorySet;
        }

        [Test]
        public void InsertAppTrainingRecordHistoryTest()
        {
            // Setup
            SetupIsuImportHistory();
            IsuImportHistory testRecord = fakeIsuImportHistory[0];

            // Execute
            ILearnerWebRepository learnerWebRepository = new LearnerWebRepository(mockDb.Object);
            IsuImportHistory response = learnerWebRepository.InsertAppTrainingRecordHistory(testRecord);

            // Verify
            Assert.That(response == testRecord);
        }

        [Test]
        public void InsertAppTrainingRecordHistoryNotExistInCacheTest()
        {
            // Setup
            SetupIsuImportHistory();
            IsuImportHistory testRecord = fixture.Generate<IsuImportHistory>();

            // Execute
            ILearnerWebRepository learnerWebRepository = new LearnerWebRepository(mockDb.Object);
            IsuImportHistory response = learnerWebRepository.InsertAppTrainingRecordHistory(testRecord);

            // Verify
            Assert.That(response == testRecord);
            Assert.That(fakeIsuImportHistory.Contains(testRecord));
        }

        [Test]
        public void UpdateCitiIdToLearnerTest()
        {
            // Setup
            SetupLearners();
            Learners sourceLearner = fakeLearners[0];
            Learners updatedLearner = sourceLearner;
            string newUser4 = fixture.Generate<string>();
            updatedLearner.User4 = newUser4;

            // Execute
            ILearnerWebRepository learnerWebRepository = new LearnerWebRepository(mockDb.Object);
            learnerWebRepository.UpdateCitiIdToLearner(updatedLearner);

            // Verify
            Assert.That(fakeLearners[0].User4 == newUser4);
        }

        [Test]
        public void InsertCourseTest()
        {
            // Setup
            SetupIsuCitiLwCourses();
            IsuCitiLwCourses fakeCourse = fixture.Generate<IsuCitiLwCourses>();

            // Execute
            ILearnerWebRepository learnerWebRepository = new LearnerWebRepository(mockDb.Object);
            int response = learnerWebRepository.InsertCourse(fakeCourse);

            // Verify
            Assert.That(response == fakeCourse.Id);
            Assert.That(fakeIsuCitiLwCourses.Contains(fakeCourse));
            mockDb.Verify(f => f.SaveChanges(), Times.Once);
        }

        [Test]
        public void InsertTrainingRecordTest()
        {
            // Setup
            SetupHistory();
            History history = fixture.Generate<History>();

            // Execute
            ILearnerWebRepository learnerWebRepository = new LearnerWebRepository(mockDb.Object);
            int response = learnerWebRepository.InsertTrainingRecord(history);

            // Verify
            Assert.That(response == history.CurriculaId);
            Assert.That(fakeHistory.Contains(history));
            mockDb.Verify(f => f.SaveChanges(), Times.Once);
        }

        [Test]
        public void SetInsertedForImportRecordTest()
        {
            // Setup
            SetupIsuImportHistory();
            fakeIsuImportHistory[0].Inserted = false;
            fakeIsuImportHistory[0].Verified = false;
            IsuImportHistory isuImportHistory = fakeIsuImportHistory[0];

            // Execute
            ILearnerWebRepository learnerWebRepository = new LearnerWebRepository(mockDb.Object);
            IsuImportHistory response = learnerWebRepository.SetInsertedForImportRecord(isuImportHistory.Id);

            // Verify
            Assert.That(response.Inserted.Value);
            Assert.That(response.Verified.Value);
            Assert.That(fakeIsuImportHistory[0].Inserted.Value);
            Assert.That(fakeIsuImportHistory[0].Verified.Value);
        }

        [Test]
        public void UpdateAppTrainingRecordIsInsertedTest()
        {
            // Setup
            SetupIsuImportHistory();
            fakeIsuImportHistory[0].Inserted = false;
            IsuImportHistory history = fakeIsuImportHistory[0];

            // Execute
            ILearnerWebRepository learnerWebRepository = new LearnerWebRepository(mockDb.Object);
            learnerWebRepository.UpdateAppTrainingRecordIsInserted(history);

            // Verify
            Assert.That(fakeIsuImportHistory[0].Inserted.Value);
        }

        [Test]
        public void GetHistoryRecordByCurriculaIdTest()
        {
            // Setup
            SetupHistory();
            History testRecord = fakeHistory[0];
            int id = testRecord.CurriculaId;

            // Execute
            ILearnerWebRepository learnerWebRepository = new LearnerWebRepository(mockDb.Object);
            History response = learnerWebRepository.GetHistoryRecordByCurriculaId(id);

            // Verify
            Assert.That(response == testRecord);
        }

        [Test]
        public void GetIsuCitiLwLearnerTest()
        {
            // Setup
            SetupIsuCitiLwLearners();
            string citiId = fakeIsuCitiLwLearners[0].CitiLearnerId;

            // Execute
            ILearnerWebRepository learnerWebRepository = new LearnerWebRepository(mockDb.Object);
            IsuCitiLwLearners response = learnerWebRepository.GetIsuCitiLwLearner(citiId);

            // Verify
            Assert.That(response == fakeIsuCitiLwLearners[0]);
        }

        [Test]
        public void GetLearnerByEmailTest()
        {
            // Setup
            SetupLearners();
            string email = fakeLearners[0].Email;

            // Execute
            ILearnerWebRepository learnerWebRepository = new LearnerWebRepository(mockDb.Object);
            Learners response = learnerWebRepository.GetLearnerByEmail(email);

            // Verify
            Assert.That(response == fakeLearners[0]);
        }

        [Test]
        public void GetLearnerByNetIdTest()
        {
            // Setup
            SetupLearners();
            string netId = fakeLearners[0].UserId;

            // Execute
            ILearnerWebRepository learnerWebRepository = new LearnerWebRepository(mockDb.Object);
            Learners response = learnerWebRepository.GetLearnerByNetId(netId);

            // Verify
            Assert.That(response == fakeLearners[0]);
        }
       
        [Test]
        public void GetLearnerByCitiIdTest()
        {
            // Setup
            SetupLearners();
            string citiId = fakeLearners[0].User4;

            // Execute
            ILearnerWebRepository learnerWebRepository = new LearnerWebRepository(mockDb.Object);
            Learners response = learnerWebRepository.GetLearnerByCitiId(citiId);

            // Verify
            Assert.That(response == fakeLearners[0]);
        }

        [Test]
        public void GetCourseByCitiCourseIdTest()
        {
            // Setup
            SetupCourses();
            string citiCourseId = fakeCourses[0].User2;

            // Execute
            ILearnerWebRepository learnerWebRepository = new LearnerWebRepository(mockDb.Object);
            Courses response = learnerWebRepository.GetCourseByCitiCourseId(citiCourseId);

            // Verify
            Assert.That(response == fakeCourses[0]);
        }

        [Test]
        public void GetImportHistoryTest()
        {
            // Setup
            SetupIsuImportHistory();
            string citiId = fakeIsuImportHistory[0].CitiId;
            string citiCourseId = fakeIsuImportHistory[0].CitiCourseId;
            DateTime dateTime = fakeIsuImportHistory[0].CompletionDate;

            // Execute
            ILearnerWebRepository learnerWebRepository = new LearnerWebRepository(mockDb.Object);
            IsuImportHistory response = learnerWebRepository.GetImportHistory(citiId, citiCourseId, dateTime);

            // Verify
            Assert.That(response == fakeIsuImportHistory[0]);
        }

        [Test]
        public void UpdateOrInsertIsuCitiLwLearnerUpdateTest()
        {
            // Setup
            SetupIsuCitiLwLearners();
            bool newValid = false;
            string newLwLearnerId = "MyTestLwLearnerId";
            DateTime newDateTime = DateTime.Now.AddYears(-10);
            IsuCitiLwLearners learner = fakeIsuCitiLwLearners[0];
            learner.Valid = newValid;
            learner.LwLearnerId = newLwLearnerId;
            learner.DateUpdated = newDateTime;

            // Execute
            ILearnerWebRepository learnerWebRepository = new LearnerWebRepository(mockDb.Object);
            learnerWebRepository.UpdateOrInsertISUCitiLwLearner(learner);

            // Verify
            Assert.That(fakeIsuCitiLwLearners[0].Valid == newValid);
            Assert.That(fakeIsuCitiLwLearners[0].LwLearnerId == newLwLearnerId);
            Assert.That(fakeIsuCitiLwLearners[0].DateUpdated != newDateTime);
        }

        [Test]
        public void UpdateOrInsertIsuCitiLwLearnerInsertTest()
        {
            // Setup
            SetupIsuCitiLwLearners();
            IsuCitiLwLearners newLearner = fixture.Generate<IsuCitiLwLearners>();

            // Execute
            ILearnerWebRepository learnerWebRepository = new LearnerWebRepository(mockDb.Object);
            learnerWebRepository.UpdateOrInsertISUCitiLwLearner(newLearner);

            // Verify
            Assert.That(fakeIsuCitiLwLearners[fakeIsuCitiLwLearners.Count - 1] == newLearner);
        }

        [Test]
        public void GetHistoryRecordByLearnerCourseDateTest()
        {
            // Setup
            SetupHistory();
            string univId = fakeHistory[0].LearnerId;
            string courseId = fakeHistory[0].CourseId;
            DateTime dateTime = fakeHistory[0].StatusDate.Value;

            // Execute
            ILearnerWebRepository learnerWebRepository = new LearnerWebRepository(mockDb.Object);
            History response = learnerWebRepository.GetHistoryRecordByLearnerCourseDate(univId, courseId, dateTime);

            // Verify
            Assert.That(response == fakeHistory[0]);
        }

        [Test]
        public void UpdateImportHistoryWithCurriculaIdTest()
        {
            // Setup
            SetupIsuImportHistory();
            IsuImportHistory isuImportHistory = fakeIsuImportHistory[0];
            int curriculaId = fixture.Generate<int>();
            isuImportHistory.CurriculaId = curriculaId;

            // Execute
            ILearnerWebRepository learnerWebRepository = new LearnerWebRepository(mockDb.Object);
            learnerWebRepository.UpdateImportHistoryWithCurriculaId(isuImportHistory);

            // Verify
            Assert.That(fakeIsuImportHistory[0].CurriculaId == curriculaId);
        }
    }
}
