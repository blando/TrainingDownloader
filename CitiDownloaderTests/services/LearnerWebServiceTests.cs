using System;
using System.Collections.Generic;
using System.Text;
using CitiDownloader.wrappers;
using CitiDownloader.services;
using NUnit.Framework.Constraints;
using Moq;
using NUnit.Framework;
using SimpleFixture;
using CitiDownloader.models;
using System.Net;
using CitiDownloader.repositories;
using CitiDownloader.models.entities;
using CitiDownloader.exceptions;

namespace CitiDownloaderTests.services
{
    [TestFixture]
    public class LearnerWebServiceTests
    {
        private Fixture fixture;

        public LearnerWebServiceTests()
        {
            this.fixture = new Fixture();
        }

        [Test]
        public void CreateHistoryRecordSuccessfulTest()
        {
            // Setup
            CitiRecord testRecord = fixture.Generate<CitiRecord>();
            Mock<ILearnerWebRepository> mockLearnerWebRepository = new Mock<ILearnerWebRepository>();

            // Execute
            ILearnerWebServices learnerWebServices = new LearnerWebService(mockLearnerWebRepository.Object);
            History response = learnerWebServices.CreateHistoryRecord(testRecord);

            // Verify
            Assert.That(response.LearnerId == testRecord.UnivId);
            Assert.That(response.CourseId == testRecord.CourseId);
            Assert.That(response.Title == testRecord.CourseName);
            Assert.That(response.Status == "f");
            Assert.That(response.EnrollmentDate == testRecord.RegistrationDate);
            Assert.That(response.Score == testRecord.Score);
            Assert.That(response.CompletionStatusId == "R");
            Assert.That(response.StatusDate == testRecord.CompletionDate);
            Assert.That(response.DateExpires == testRecord.ExpirationDate);
            Assert.That(response.PassingScore == testRecord.PassingScore);
        }

        [Test]
        public void FindCourseIdReturnCourseIdTest()
        {
            // Setup
            string fakeGroupId = fixture.Generate<string>();
            string returnCourseId = fixture.Generate<string>();
            CitiRecord fakeCitiRecord = fixture.Generate<CitiRecord>();
            Courses returnCourse = fixture.Generate<Courses>();
            returnCourse.CourseId = returnCourseId;
            Mock<ILearnerWebRepository> mockLearnerWebRepository = new Mock<ILearnerWebRepository>();
            mockLearnerWebRepository.Setup(f => f.GetCourseByCitiCourseId(It.IsAny<string>())).Returns(returnCourse);

            // Execute
            ILearnerWebServices learnerWebServices = new LearnerWebService(mockLearnerWebRepository.Object);
            string response = learnerWebServices.FindCourseId(fakeCitiRecord);

            // Verify
            Assert.That(response == returnCourseId);
            mockLearnerWebRepository.Verify(f => f.GetCourseByCitiCourseId(It.IsAny<string>()), Times.Once);
            mockLearnerWebRepository.Verify(f => f.InsertCourse(It.IsAny<IsuCitiLwCourses>()), Times.Never);

        }

        [Test]
        public void FindCourseIdNotFoundInsertAndReturnNullTest()
        {
            // Setup
            string fakeGroupId = fixture.Generate<string>();
            string returnCourseId = fixture.Generate<string>();
            CitiRecord fakeCitiRecord = fixture.Generate<CitiRecord>();
            Courses returnCourse = null;
            Mock<ILearnerWebRepository> mockLearnerWebRepository = new Mock<ILearnerWebRepository>();
            mockLearnerWebRepository.Setup(f => f.GetCourseByCitiCourseId(It.IsAny<string>())).Returns(returnCourse);
            mockLearnerWebRepository.Setup(f => f.InsertCourse(It.IsAny<IsuCitiLwCourses>())).Verifiable();

            // Execute
            ILearnerWebServices learnerWebServices = new LearnerWebService(mockLearnerWebRepository.Object);
            string response = learnerWebServices.FindCourseId(fakeCitiRecord);

            // Verify
            Assert.That(response == null);
            mockLearnerWebRepository.Verify(f => f.GetCourseByCitiCourseId(It.IsAny<string>()), Times.Once);
            mockLearnerWebRepository.Verify(f => f.InsertCourse(It.IsAny<IsuCitiLwCourses>()), Times.Once);
            Mock.VerifyAll(mockLearnerWebRepository);
        }

        [Test]
        public void FindCourseIdGetCourseThrowsExceptionTest()
        {
            // Setup
            string fakeGroupId = fixture.Generate<string>();
            string returnCourseId = fixture.Generate<string>();
            CitiRecord fakeCitiRecord = fixture.Generate<CitiRecord>();
            Courses returnCourse = fixture.Generate<Courses>();
            returnCourse.CourseId = returnCourseId;
            Mock<ILearnerWebRepository> mockLearnerWebRepository = new Mock<ILearnerWebRepository>();
            mockLearnerWebRepository.Setup(f => f.GetCourseByCitiCourseId(It.IsAny<string>())).Throws(new Exception());

            // Execute
            ILearnerWebServices learnerWebServices = new LearnerWebService(mockLearnerWebRepository.Object);
            ActualValueDelegate<object> testDelegate = () => learnerWebServices.FindCourseId(fakeCitiRecord);

            // Verify
            Assert.That(testDelegate, Throws.TypeOf<Exception>());
            mockLearnerWebRepository.Verify(f => f.GetCourseByCitiCourseId(It.IsAny<string>()), Times.Once);
            mockLearnerWebRepository.Verify(f => f.InsertCourse(It.IsAny<IsuCitiLwCourses>()), Times.Never);

        }

        [Test]
        public void FindCourseIdNotFoundInsertThrowsExceptionTest()
        {
            // Setup
            string fakeGroupId = fixture.Generate<string>();
            string returnCourseId = fixture.Generate<string>();
            CitiRecord fakeCitiRecord = fixture.Generate<CitiRecord>();
            Courses returnCourse = null;
            Mock<ILearnerWebRepository> mockLearnerWebRepository = new Mock<ILearnerWebRepository>();
            mockLearnerWebRepository.Setup(f => f.GetCourseByCitiCourseId(It.IsAny<string>())).Returns(returnCourse);
            mockLearnerWebRepository.Setup(f => f.InsertCourse(It.IsAny<IsuCitiLwCourses>())).Throws(new Exception());

            // Execute
            ILearnerWebServices learnerWebServices = new LearnerWebService(mockLearnerWebRepository.Object);
            ActualValueDelegate<object> testDelegate = () => learnerWebServices.FindCourseId(fakeCitiRecord);

            // Verify
            Assert.That(testDelegate, Throws.TypeOf<Exception>());
            mockLearnerWebRepository.Verify(f => f.GetCourseByCitiCourseId(It.IsAny<string>()), Times.Once);
            mockLearnerWebRepository.Verify(f => f.InsertCourse(It.IsAny<IsuCitiLwCourses>()), Times.Once);
        }

        [Test]
        public void FindUserExistingGetIsuCitiLwLearnerTest()
        {
            // Setup
            string citiId = fixture.Generate<string>();
            string univId = fixture.Generate<string>();
            CitiRecord fakeCitiRecord = fixture.Generate<CitiRecord>();
            fakeCitiRecord.CitiId = citiId;
            IsuCitiLwLearners fakeLearners = fixture.Generate<IsuCitiLwLearners>();
            fakeLearners.LwLearnerId = univId;
            Mock<ILearnerWebRepository> mockLearnerWebRepository = new Mock<ILearnerWebRepository>();
            mockLearnerWebRepository.Setup(f => f.GetIsuCitiLwLearner(citiId)).Returns(fakeLearners);

            // Execute
            ILearnerWebServices learnerWebServices = new LearnerWebService(mockLearnerWebRepository.Object);
            string response = learnerWebServices.FindUser(fakeCitiRecord);

            // Verify
            Assert.That(response == univId);
            mockLearnerWebRepository.Verify(f => f.GetIsuCitiLwLearner(It.IsAny<string>()), Times.Once);
            mockLearnerWebRepository.Verify(f => f.GetLearnerByCitiId(It.IsAny<string>()), Times.Never);
            mockLearnerWebRepository.Verify(f => f.GetLearnerByNetId(It.IsAny<string>()), Times.Never);
            mockLearnerWebRepository.Verify(f => f.GetLearnerByEmail(It.IsAny<string>()), Times.Never);
            mockLearnerWebRepository.Verify(f => f.UpdateOrInsertISUCitiLwLearner(It.IsAny<IsuCitiLwLearners>()), Times.Never);
        }

        [Test]
        public void FindUserGetIsuCitiLwLearnerThrowsExceptionTest()
        {
            // Setup
            string citiId = fixture.Generate<string>();
            string univId = fixture.Generate<string>();
            CitiRecord fakeCitiRecord = fixture.Generate<CitiRecord>();
            fakeCitiRecord.CitiId = citiId;
            IsuCitiLwLearners fakeLearners = fixture.Generate<IsuCitiLwLearners>();
            fakeLearners.LwLearnerId = univId;
            Mock<ILearnerWebRepository> mockLearnerWebRepository = new Mock<ILearnerWebRepository>();
            mockLearnerWebRepository.Setup(f => f.GetIsuCitiLwLearner(citiId)).Throws(new Exception());

            // Execute
            ILearnerWebServices learnerWebServices = new LearnerWebService(mockLearnerWebRepository.Object);
            ActualValueDelegate<object> testDelegate = () => learnerWebServices.FindUser(fakeCitiRecord);

            // Verify
            Assert.That(testDelegate, Throws.TypeOf<Exception>());
            mockLearnerWebRepository.Verify(f => f.GetIsuCitiLwLearner(It.IsAny<string>()), Times.Once);
            mockLearnerWebRepository.Verify(f => f.GetLearnerByCitiId(It.IsAny<string>()), Times.Never);
            mockLearnerWebRepository.Verify(f => f.GetLearnerByNetId(It.IsAny<string>()), Times.Never);
            mockLearnerWebRepository.Verify(f => f.GetLearnerByEmail(It.IsAny<string>()), Times.Never);
            mockLearnerWebRepository.Verify(f => f.UpdateOrInsertISUCitiLwLearner(It.IsAny<IsuCitiLwLearners>()), Times.Never);
        }

        [Test]
        public void FindUserExistingGetLearnerByCitiIdTest()
        {
            // Setup
            string citiId = fixture.Generate<string>();
            string univId = fixture.Generate<string>();
            CitiRecord fakeCitiRecord = fixture.Generate<CitiRecord>();
            fakeCitiRecord.CitiId = citiId;
            IsuCitiLwLearners fakeLearners = null;
            Learners fakeLearner = fixture.Generate<Learners>();
            fakeLearner.LearnerId = univId;
            Mock<ILearnerWebRepository> mockLearnerWebRepository = new Mock<ILearnerWebRepository>();
            mockLearnerWebRepository.Setup(f => f.GetIsuCitiLwLearner(citiId)).Returns(fakeLearners);
            mockLearnerWebRepository.Setup(f => f.GetLearnerByCitiId(citiId)).Returns(fakeLearner);
            mockLearnerWebRepository.Setup(f => f.UpdateOrInsertISUCitiLwLearner(It.IsAny<IsuCitiLwLearners>())).Verifiable();

            // Execute
            ILearnerWebServices learnerWebServices = new LearnerWebService(mockLearnerWebRepository.Object);
            string response = learnerWebServices.FindUser(fakeCitiRecord);

            // Verify
            Assert.That(response == univId);
            mockLearnerWebRepository.Verify(f => f.GetIsuCitiLwLearner(It.IsAny<string>()), Times.Once);
            mockLearnerWebRepository.Verify(f => f.GetLearnerByCitiId(It.IsAny<string>()), Times.Once);
            mockLearnerWebRepository.Verify(f => f.GetLearnerByNetId(It.IsAny<string>()), Times.Never);
            mockLearnerWebRepository.Verify(f => f.GetLearnerByEmail(It.IsAny<string>()), Times.Never);
            mockLearnerWebRepository.Verify(f => f.UpdateOrInsertISUCitiLwLearner(It.IsAny<IsuCitiLwLearners>()), Times.Once);
            Mock.VerifyAll(mockLearnerWebRepository);
        }

        [Test]
        public void FindUserExistingGetLearnerByCitiIdThrowsExceptionTest()
        {
            // Setup
            string citiId = fixture.Generate<string>();
            string univId = fixture.Generate<string>();
            CitiRecord fakeCitiRecord = fixture.Generate<CitiRecord>();
            fakeCitiRecord.CitiId = citiId;
            IsuCitiLwLearners fakeLearners = null;
            Learners fakeLearner = fixture.Generate<Learners>();
            fakeLearner.LearnerId = univId;
            Mock<ILearnerWebRepository> mockLearnerWebRepository = new Mock<ILearnerWebRepository>();
            mockLearnerWebRepository.Setup(f => f.GetIsuCitiLwLearner(citiId)).Returns(fakeLearners);
            mockLearnerWebRepository.Setup(f => f.GetLearnerByCitiId(citiId)).Throws(new Exception());

            // Execute
            ILearnerWebServices learnerWebServices = new LearnerWebService(mockLearnerWebRepository.Object);
            ActualValueDelegate<object> testDelegate = () => learnerWebServices.FindUser(fakeCitiRecord);

            // Verify
            Assert.That(testDelegate, Throws.TypeOf<Exception>());
            mockLearnerWebRepository.Verify(f => f.GetIsuCitiLwLearner(It.IsAny<string>()), Times.Once);
            mockLearnerWebRepository.Verify(f => f.GetLearnerByCitiId(It.IsAny<string>()), Times.Once);
            mockLearnerWebRepository.Verify(f => f.GetLearnerByNetId(It.IsAny<string>()), Times.Never);
            mockLearnerWebRepository.Verify(f => f.GetLearnerByEmail(It.IsAny<string>()), Times.Never);
            mockLearnerWebRepository.Verify(f => f.UpdateOrInsertISUCitiLwLearner(It.IsAny<IsuCitiLwLearners>()), Times.Never);
        }

        [Test]
        public void FindUserValidGetLearnerByNetidTest()
        {
            // Setup
            Mock<ILearnerWebRepository> mockLearnerWebRepository = new Mock<ILearnerWebRepository>();
            string citiId = fixture.Generate<string>();
            string netId = "username";
            string domain = "iastate.edu";
            string univId = fixture.Generate<string>();

                // Mock the first part and return null univId
            CitiRecord fakeCitiRecord = fixture.Generate<CitiRecord>();
            fakeCitiRecord.CitiId = citiId;
            fakeCitiRecord.EmailAddress = string.Format("{0}@{1}", netId, domain);
            IsuCitiLwLearners fakeIsuCitiLwLearners = new IsuCitiLwLearners();
            fakeIsuCitiLwLearners.LwLearnerId = null;
            fakeIsuCitiLwLearners.Valid = true;
            mockLearnerWebRepository.Setup(f => f.GetIsuCitiLwLearner(citiId)).Returns(fakeIsuCitiLwLearners);

                // Mock the second part and return null univId
            Learners fakeLearner1 = null;
            mockLearnerWebRepository.Setup(f => f.GetLearnerByCitiId(citiId)).Returns(fakeLearner1);

                // Mock the third part and return an actual univId
            Learners fakeLearner2 = fixture.Generate<Learners>();
            fakeLearner2.LearnerId = univId;
            mockLearnerWebRepository.Setup(f => f.GetLearnerByNetId(netId)).Returns(fakeLearner2);

                // Mock the call to update the cached record
            mockLearnerWebRepository.Setup(f => f.UpdateOrInsertISUCitiLwLearner(It.IsAny<IsuCitiLwLearners>())).Verifiable();

            // Execute
            ILearnerWebServices learnerWebServices = new LearnerWebService(mockLearnerWebRepository.Object);
            string response = learnerWebServices.FindUser(fakeCitiRecord);

            // Verify
            Assert.That(response == univId);
            mockLearnerWebRepository.Verify(f => f.GetIsuCitiLwLearner(It.IsAny<string>()), Times.Exactly(2));
            mockLearnerWebRepository.Verify(f => f.GetLearnerByCitiId(It.IsAny<string>()), Times.Once);
            mockLearnerWebRepository.Verify(f => f.GetLearnerByNetId(It.IsAny<string>()), Times.Once);
            mockLearnerWebRepository.Verify(f => f.GetLearnerByEmail(It.IsAny<string>()), Times.Never);
            mockLearnerWebRepository.Verify(f => f.UpdateOrInsertISUCitiLwLearner(It.IsAny<IsuCitiLwLearners>()), Times.Once);
            Mock.VerifyAll(mockLearnerWebRepository);
        }

        [Test]
        public void FindUserValidGetLearnerByNetidThrowsExceptionTest()
        {
            // Setup
            Mock<ILearnerWebRepository> mockLearnerWebRepository = new Mock<ILearnerWebRepository>();
            string citiId = fixture.Generate<string>();
            string netId = "username";
            string domain = "iastate.edu";
            string univId = fixture.Generate<string>();

            // Mock the first part and return null univId
            CitiRecord fakeCitiRecord = fixture.Generate<CitiRecord>();
            fakeCitiRecord.CitiId = citiId;
            fakeCitiRecord.EmailAddress = string.Format("{0}@{1}", netId, domain);
            IsuCitiLwLearners fakeIsuCitiLwLearners = new IsuCitiLwLearners();
            fakeIsuCitiLwLearners.LwLearnerId = null;
            fakeIsuCitiLwLearners.Valid = true;
            mockLearnerWebRepository.Setup(f => f.GetIsuCitiLwLearner(citiId)).Returns(fakeIsuCitiLwLearners);

            // Mock the second part and return null univId
            Learners fakeLearner1 = null;
            mockLearnerWebRepository.Setup(f => f.GetLearnerByCitiId(citiId)).Returns(fakeLearner1);

            // Mock the third part and return an actual univId
            Learners fakeLearner2 = fixture.Generate<Learners>();
            fakeLearner2.LearnerId = univId;
            mockLearnerWebRepository.Setup(f => f.GetLearnerByNetId(netId)).Throws(new Exception());

            // Mock the call to update the cached record
            mockLearnerWebRepository.Setup(f => f.UpdateOrInsertISUCitiLwLearner(It.IsAny<IsuCitiLwLearners>())).Verifiable();

            // Execute
            ILearnerWebServices learnerWebServices = new LearnerWebService(mockLearnerWebRepository.Object);
            ActualValueDelegate<object> testDelegate = () => learnerWebServices.FindUser(fakeCitiRecord);

            // Verify
            Assert.That(testDelegate, Throws.TypeOf<Exception>());
            mockLearnerWebRepository.Verify(f => f.GetIsuCitiLwLearner(It.IsAny<string>()), Times.Exactly(2));
            mockLearnerWebRepository.Verify(f => f.GetLearnerByCitiId(It.IsAny<string>()), Times.Once);
            mockLearnerWebRepository.Verify(f => f.GetLearnerByNetId(It.IsAny<string>()), Times.Once);
            mockLearnerWebRepository.Verify(f => f.GetLearnerByEmail(It.IsAny<string>()), Times.Never);
            mockLearnerWebRepository.Verify(f => f.UpdateOrInsertISUCitiLwLearner(It.IsAny<IsuCitiLwLearners>()), Times.Never);
        }

        [Test]
        public void FindUserValidGetLearnerByNetidWwithNonIastateEmailTest()
        {
            // Setup
            Mock<ILearnerWebRepository> mockLearnerWebRepository = new Mock<ILearnerWebRepository>();
            string citiId = fixture.Generate<string>();
            string netId = "username";
            string domain = "notiastate.edu";
            string univId = fixture.Generate<string>();

            // Mock the first part and return null univId
            CitiRecord fakeCitiRecord = fixture.Generate<CitiRecord>();
            fakeCitiRecord.CitiId = citiId;
            fakeCitiRecord.EmailAddress = string.Format("{0}@{1}", netId, domain);
            IsuCitiLwLearners fakeIsuCitiLwLearners = new IsuCitiLwLearners();
            fakeIsuCitiLwLearners.LwLearnerId = null;
            fakeIsuCitiLwLearners.Valid = true;
            mockLearnerWebRepository.Setup(f => f.GetIsuCitiLwLearner(citiId)).Returns(fakeIsuCitiLwLearners);

            // Mock the second part and return null univId
            Learners fakeLearner1 = null;
            mockLearnerWebRepository.Setup(f => f.GetLearnerByCitiId(citiId)).Returns(fakeLearner1);

            // NetId will return null

            // Mock the fourth part and return a univId
            Learners fakeLearner2 = fixture.Generate<Learners>();
            fakeLearner2.LearnerId = univId;
            mockLearnerWebRepository.Setup(f => f.GetLearnerByEmail(fakeCitiRecord.EmailAddress)).Returns(fakeLearner2);

            // Mock the call to update the cached record
            mockLearnerWebRepository.Setup(f => f.UpdateOrInsertISUCitiLwLearner(It.IsAny<IsuCitiLwLearners>())).Verifiable();

            // Execute
            ILearnerWebServices learnerWebServices = new LearnerWebService(mockLearnerWebRepository.Object);
            string response = learnerWebServices.FindUser(fakeCitiRecord);

            // Verify
            Assert.That(response == univId);
            mockLearnerWebRepository.Verify(f => f.GetIsuCitiLwLearner(It.IsAny<string>()), Times.Exactly(2));
            mockLearnerWebRepository.Verify(f => f.GetLearnerByCitiId(It.IsAny<string>()), Times.Once);
            mockLearnerWebRepository.Verify(f => f.GetLearnerByNetId(It.IsAny<string>()), Times.Never);
            mockLearnerWebRepository.Verify(f => f.GetLearnerByEmail(It.IsAny<string>()), Times.Once);
            mockLearnerWebRepository.Verify(f => f.UpdateOrInsertISUCitiLwLearner(It.IsAny<IsuCitiLwLearners>()), Times.Once);
            Mock.VerifyAll(mockLearnerWebRepository);
        }

        [Test]
        public void FindUserValidGetLearnerByEmailTest()
        {
            // Setup
            Mock<ILearnerWebRepository> mockLearnerWebRepository = new Mock<ILearnerWebRepository>();
            string citiId = fixture.Generate<string>();
            string netId = "username";
            string domain = "iastate.edu";
            string univId = fixture.Generate<string>();

            // Mock the first part and return null univId
            CitiRecord fakeCitiRecord = fixture.Generate<CitiRecord>();
            fakeCitiRecord.CitiId = citiId;
            fakeCitiRecord.EmailAddress = string.Format("{0}@{1}", netId, domain);
            IsuCitiLwLearners fakeIsuCitiLwLearners = new IsuCitiLwLearners();
            fakeIsuCitiLwLearners.LwLearnerId = null;
            fakeIsuCitiLwLearners.Valid = true;
            mockLearnerWebRepository.Setup(f => f.GetIsuCitiLwLearner(citiId)).Returns(fakeIsuCitiLwLearners);

            // Mock the second part and return null univId
            Learners fakeLearner1 = null;
            mockLearnerWebRepository.Setup(f => f.GetLearnerByCitiId(citiId)).Returns(fakeLearner1);

            // Mock the third part and return null univId
            Learners fakeLearner2 = null;
            mockLearnerWebRepository.Setup(f => f.GetLearnerByNetId(netId)).Returns(fakeLearner2);

            // Mock the fourth part and return an actual univId
            Learners fakeLearner3 = fixture.Generate<Learners>();
            fakeLearner3.LearnerId = univId;
            mockLearnerWebRepository.Setup(f => f.GetLearnerByEmail(fakeCitiRecord.EmailAddress)).Returns(fakeLearner3);

            // Mock the call to update the cached record
            mockLearnerWebRepository.Setup(f => f.UpdateOrInsertISUCitiLwLearner(It.IsAny<IsuCitiLwLearners>())).Verifiable();

            // Execute
            ILearnerWebServices learnerWebServices = new LearnerWebService(mockLearnerWebRepository.Object);
            string response = learnerWebServices.FindUser(fakeCitiRecord);

            // Verify
            Assert.That(response == univId);
            mockLearnerWebRepository.Verify(f => f.GetIsuCitiLwLearner(It.IsAny<string>()), Times.Exactly(2));
            mockLearnerWebRepository.Verify(f => f.GetLearnerByCitiId(It.IsAny<string>()), Times.Once);
            mockLearnerWebRepository.Verify(f => f.GetLearnerByNetId(It.IsAny<string>()), Times.Once);
            mockLearnerWebRepository.Verify(f => f.GetLearnerByEmail(It.IsAny<string>()), Times.Once);
            mockLearnerWebRepository.Verify(f => f.UpdateOrInsertISUCitiLwLearner(It.IsAny<IsuCitiLwLearners>()), Times.Once);
            Mock.VerifyAll(mockLearnerWebRepository);
        }

        [Test]
        public void FindUserUnknownUserExceptionTest()
        {
            // Setup
            Mock<ILearnerWebRepository> mockLearnerWebRepository = new Mock<ILearnerWebRepository>();
            string citiId = fixture.Generate<string>();
            string netId = "username";
            string domain = "iastate.edu";
            string univId = fixture.Generate<string>();

            // Mock the first part and return null univId
            CitiRecord fakeCitiRecord = fixture.Generate<CitiRecord>();
            fakeCitiRecord.CitiId = citiId;
            fakeCitiRecord.EmailAddress = string.Format("{0}@{1}", netId, domain);
            IsuCitiLwLearners fakeIsuCitiLwLearners = new IsuCitiLwLearners();
            fakeIsuCitiLwLearners.LwLearnerId = null;
            fakeIsuCitiLwLearners.Valid = true;
            mockLearnerWebRepository.Setup(f => f.GetIsuCitiLwLearner(citiId)).Returns(fakeIsuCitiLwLearners);

            // Mock the second part and return null univId
            Learners fakeLearner1 = null;
            mockLearnerWebRepository.Setup(f => f.GetLearnerByCitiId(citiId)).Returns(fakeLearner1);

            // Mock the third part and return null univId
            Learners fakeLearner2 = null;
            mockLearnerWebRepository.Setup(f => f.GetLearnerByNetId(netId)).Returns(fakeLearner2);

            // Mock the fourth part and return an actual univId
            Learners fakeLearner3 = null;
            mockLearnerWebRepository.Setup(f => f.GetLearnerByEmail(fakeCitiRecord.EmailAddress)).Returns(fakeLearner3);

            // Execute
            ILearnerWebServices learnerWebServices = new LearnerWebService(mockLearnerWebRepository.Object);
            ActualValueDelegate<object> testDelegate = () => learnerWebServices.FindUser(fakeCitiRecord);

            // Verify
            Assert.That(testDelegate, Throws.TypeOf<UnknownUserException>());
            mockLearnerWebRepository.Verify(f => f.GetIsuCitiLwLearner(It.IsAny<string>()), Times.Exactly(2));
            mockLearnerWebRepository.Verify(f => f.GetLearnerByCitiId(It.IsAny<string>()), Times.Once);
            mockLearnerWebRepository.Verify(f => f.GetLearnerByNetId(It.IsAny<string>()), Times.Once);
            mockLearnerWebRepository.Verify(f => f.GetLearnerByEmail(It.IsAny<string>()), Times.Once);
            mockLearnerWebRepository.Verify(f => f.UpdateOrInsertISUCitiLwLearner(It.IsAny<IsuCitiLwLearners>()), Times.Never);

        }

        [Test]
        public void FindUserInvalidUserExceptionTest()
        {
            // Setup
            Mock<ILearnerWebRepository> mockLearnerWebRepository = new Mock<ILearnerWebRepository>();
            string citiId = fixture.Generate<string>();
            string netId = "username";
            string domain = "iastate.edu";
            string univId = fixture.Generate<string>();

            // Mock the first part and return null univId
            CitiRecord fakeCitiRecord = fixture.Generate<CitiRecord>();
            fakeCitiRecord.CitiId = citiId;
            fakeCitiRecord.EmailAddress = string.Format("{0}@{1}", netId, domain);
            IsuCitiLwLearners fakeIsuCitiLwLearners = new IsuCitiLwLearners();
            fakeIsuCitiLwLearners.LwLearnerId = null;
            fakeIsuCitiLwLearners.Valid = false;
            mockLearnerWebRepository.Setup(f => f.GetIsuCitiLwLearner(citiId)).Returns(fakeIsuCitiLwLearners);

            // Mock the second part and return null univId
            Learners fakeLearner1 = null;
            mockLearnerWebRepository.Setup(f => f.GetLearnerByCitiId(citiId)).Returns(fakeLearner1);

            // Execute
            ILearnerWebServices learnerWebServices = new LearnerWebService(mockLearnerWebRepository.Object);
            ActualValueDelegate<object> testDelegate = () => learnerWebServices.FindUser(fakeCitiRecord);

            // Verify
            Assert.That(testDelegate, Throws.TypeOf<InvalidUserException>());
            mockLearnerWebRepository.Verify(f => f.GetIsuCitiLwLearner(It.IsAny<string>()), Times.Exactly(2));
            mockLearnerWebRepository.Verify(f => f.GetLearnerByCitiId(It.IsAny<string>()), Times.Once);
            mockLearnerWebRepository.Verify(f => f.GetLearnerByNetId(It.IsAny<string>()), Times.Never);
            mockLearnerWebRepository.Verify(f => f.GetLearnerByEmail(It.IsAny<string>()), Times.Never);
            mockLearnerWebRepository.Verify(f => f.UpdateOrInsertISUCitiLwLearner(It.IsAny<IsuCitiLwLearners>()), Times.Never);

        }

        [Test]
        public void GetHistoryByCurriculaIdTest()
        {
            // Setup
            int curriculaId = fixture.Generate<int>();

            CitiRecord fakeCitiRecord = fixture.Generate<CitiRecord>();
            IsuImportHistory returnIsuImportHistory = fixture.Generate<IsuImportHistory>();
            returnIsuImportHistory.CurriculaId = curriculaId;

            Mock<ILearnerWebRepository> mockLearnerWebRepository = new Mock<ILearnerWebRepository>();
            mockLearnerWebRepository.Setup(f => f.GetImportHistory(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>())).Returns(returnIsuImportHistory);

            History returnHistory = fixture.Generate<History>();
            mockLearnerWebRepository.Setup(f => f.GetHistoryRecordByCurriculaId(curriculaId)).Returns(returnHistory);

            // Execute
            ILearnerWebServices learnerWebServices = new LearnerWebService(mockLearnerWebRepository.Object);
            History response = learnerWebServices.GetHistoryByCurriculaId(fakeCitiRecord);

            // Verify
            Assert.That(response == returnHistory);
            mockLearnerWebRepository.Verify(f => f.GetImportHistory(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()), Times.Once);
            mockLearnerWebRepository.Verify(f => f.GetHistoryRecordByCurriculaId(It.IsAny<int>()), Times.Once);
        }

        [Test]
        public void GetHistoryByCurriculaIdGetHistoryRecordByCurriculaIdThrowsExceptionTest()
        {
            // Setup
            int curriculaId = fixture.Generate<int>();

            CitiRecord fakeCitiRecord = fixture.Generate<CitiRecord>();
            IsuImportHistory returnIsuImportHistory = fixture.Generate<IsuImportHistory>();
            returnIsuImportHistory.CurriculaId = curriculaId;

            Mock<ILearnerWebRepository> mockLearnerWebRepository = new Mock<ILearnerWebRepository>();
            mockLearnerWebRepository.Setup(f => f.GetImportHistory(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>())).Returns(returnIsuImportHistory);

            History returnHistory = fixture.Generate<History>();
            mockLearnerWebRepository.Setup(f => f.GetHistoryRecordByCurriculaId(curriculaId)).Throws(new Exception());

            // Execute
            ILearnerWebServices learnerWebServices = new LearnerWebService(mockLearnerWebRepository.Object);
            ActualValueDelegate<object> testDelegate = () => learnerWebServices.GetHistoryByCurriculaId(fakeCitiRecord);

            // Verify
            Assert.That(testDelegate, Throws.TypeOf<Exception>());
            mockLearnerWebRepository.Verify(f => f.GetImportHistory(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()), Times.Once);
            mockLearnerWebRepository.Verify(f => f.GetHistoryRecordByCurriculaId(It.IsAny<int>()), Times.Once);
        }

        [Test]
        public void GetHistoryByCurriculaIdZeroValueCurriculaIdTest()
        {
            // Setup
            int curriculaId = 0;

            CitiRecord fakeCitiRecord = fixture.Generate<CitiRecord>();
            IsuImportHistory returnIsuImportHistory = fixture.Generate<IsuImportHistory>();
            returnIsuImportHistory.CurriculaId = curriculaId;

            Mock<ILearnerWebRepository> mockLearnerWebRepository = new Mock<ILearnerWebRepository>();
            mockLearnerWebRepository.Setup(f => f.GetImportHistory(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>())).Returns(returnIsuImportHistory);

            // Execute
            ILearnerWebServices learnerWebServices = new LearnerWebService(mockLearnerWebRepository.Object);
            History response = learnerWebServices.GetHistoryByCurriculaId(fakeCitiRecord);

            // Verify
            Assert.That(response == null);
            mockLearnerWebRepository.Verify(f => f.GetImportHistory(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()), Times.Once);
            mockLearnerWebRepository.Verify(f => f.GetHistoryRecordByCurriculaId(It.IsAny<int>()), Times.Never);
        }

        [Test]
        public void GetHistoryByCurriculaIdGetImportHistoryThrowsExceptionTest()
        {
            // Setup

            CitiRecord fakeCitiRecord = fixture.Generate<CitiRecord>();
            IsuImportHistory returnIsuImportHistory = fixture.Generate<IsuImportHistory>();

            Mock<ILearnerWebRepository> mockLearnerWebRepository = new Mock<ILearnerWebRepository>();
            mockLearnerWebRepository.Setup(f => f.GetImportHistory(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>())).Throws(new Exception());

            // Execute
            ILearnerWebServices learnerWebServices = new LearnerWebService(mockLearnerWebRepository.Object);
            ActualValueDelegate<object> testDelegate = () => learnerWebServices.GetHistoryByCurriculaId(fakeCitiRecord);

            // Verify
            Assert.That(testDelegate, Throws.TypeOf<Exception>());
            mockLearnerWebRepository.Verify(f => f.GetImportHistory(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()), Times.Once);
            mockLearnerWebRepository.Verify(f => f.GetHistoryRecordByCurriculaId(It.IsAny<int>()), Times.Never);
        }

        [Test]
        public void InsertHistoryNoExistingTest()
        {
            // Setup
            History returnHistory = null;
            History fakeHistory = fixture.Generate<History>();

            Mock<ILearnerWebRepository> mockLearnerWebRepository = new Mock<ILearnerWebRepository>();
            mockLearnerWebRepository.Setup(f => f.GetHistoryRecordByLearnerCourseDate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>())).Returns(returnHistory);
            mockLearnerWebRepository.Setup(f => f.InsertTrainingRecord(fakeHistory)).Verifiable();

            // Execute
            ILearnerWebServices learnerWebServices = new LearnerWebService(mockLearnerWebRepository.Object);
            bool response;
            learnerWebServices.InsertHistory(fakeHistory, out response);

            // Verify
            Assert.That(response == true);
            Mock.VerifyAll(mockLearnerWebRepository);
            mockLearnerWebRepository.Verify(f => f.GetHistoryRecordByLearnerCourseDate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()), Times.Once);
            mockLearnerWebRepository.Verify(f => f.InsertTrainingRecord(It.IsAny<History>()), Times.Once);
        }

        [Test]
        public void InsertHistoryWithExistingTest()
        {
            // Setup
            History returnHistory = fixture.Generate<History>();
            History fakeHistory = fixture.Generate<History>();

            Mock<ILearnerWebRepository> mockLearnerWebRepository = new Mock<ILearnerWebRepository>();
            mockLearnerWebRepository.Setup(f => f.GetHistoryRecordByLearnerCourseDate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>())).Returns(returnHistory);

            // Execute
            ILearnerWebServices learnerWebServices = new LearnerWebService(mockLearnerWebRepository.Object);
            bool response;
            learnerWebServices.InsertHistory(fakeHistory, out response);

            // Verify
            Assert.That(response == false);
            Mock.VerifyAll(mockLearnerWebRepository);
            mockLearnerWebRepository.Verify(f => f.GetHistoryRecordByLearnerCourseDate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()), Times.Once);
            mockLearnerWebRepository.Verify(f => f.InsertTrainingRecord(It.IsAny<History>()), Times.Never);
        }

        [Test]
        public void InsertHistoryWithExistingThrowsExceptionTest()
        {
            // Setup
            History fakeHistory = fixture.Generate<History>();

            Mock<ILearnerWebRepository> mockLearnerWebRepository = new Mock<ILearnerWebRepository>();
            mockLearnerWebRepository.Setup(f => f.GetHistoryRecordByLearnerCourseDate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>())).Throws(new Exception());

            Exception exception = new Exception();

            // Execute
            ILearnerWebServices learnerWebServices = new LearnerWebService(mockLearnerWebRepository.Object);

            // Execute & Verify
            bool response;
            Assert.Throws<Exception>(delegate { learnerWebServices.InsertHistory(fakeHistory, out response); });

            // Verify
            mockLearnerWebRepository.Verify(f => f.GetHistoryRecordByLearnerCourseDate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()), Times.Once);
            mockLearnerWebRepository.Verify(f => f.InsertTrainingRecord(It.IsAny<History>()), Times.Never);
        }

        [Test]
        public void InsertHistoryWithNonExistingThrowsExceptionTest()
        {
            // Setup
            History fakeHistory = fixture.Generate<History>();
            History returnHistory = null;

            Mock<ILearnerWebRepository> mockLearnerWebRepository = new Mock<ILearnerWebRepository>();
            mockLearnerWebRepository.Setup(f => f.GetHistoryRecordByLearnerCourseDate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>())).Returns(returnHistory);
            mockLearnerWebRepository.Setup(f => f.InsertTrainingRecord(fakeHistory)).Throws(new Exception());

            Exception exception = new Exception();

            // Execute
            ILearnerWebServices learnerWebServices = new LearnerWebService(mockLearnerWebRepository.Object);

            // Excecute & Verify
            bool response;
            Assert.Throws<Exception>(delegate { learnerWebServices.InsertHistory(fakeHistory, out response); });

            // Verify
            mockLearnerWebRepository.Verify(f => f.GetHistoryRecordByLearnerCourseDate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()), Times.Once);
            mockLearnerWebRepository.Verify(f => f.InsertTrainingRecord(It.IsAny<History>()), Times.Once);
        }

        [Test]
        public void InsertImportHistoryTest()
        {
            // Setup
            CitiRecord fakeCitiRecord = fixture.Generate<CitiRecord>();
            fakeCitiRecord.StageNumber = fixture.Generate<Byte>();

            Mock<ILearnerWebRepository> mockLearnerWebRepository = new Mock<ILearnerWebRepository>();
            mockLearnerWebRepository.Setup(f => f.InsertAppTrainingRecordHistory(It.IsAny<IsuImportHistory>())).Verifiable();

            // Execute
            ILearnerWebServices learnerWebServices = new LearnerWebService(mockLearnerWebRepository.Object);
            IsuImportHistory response = learnerWebServices.InsertImportHistory(fakeCitiRecord);

            // Verify
            Assert.That(response.CitiId == fakeCitiRecord.CitiId);
            Assert.That(response.FirstName == fakeCitiRecord.FirstName);
            Assert.That(response.LastName == fakeCitiRecord.LastName);
            Assert.That(response.EmailAddress == fakeCitiRecord.EmailAddress);
            Assert.That(response.RegistrationDate == fakeCitiRecord.RegistrationDate);
            Assert.That(response.CourseName == fakeCitiRecord.CourseName);
            Assert.That(response.StageNumber == byte.Parse(fakeCitiRecord.StageNumber.ToString()));
            Assert.That(response.StageDescription == fakeCitiRecord.StageDescription);
            Assert.That(response.CompletionReportNum == fakeCitiRecord.CompletionReportNum);
            Assert.That(response.CompletionDate == fakeCitiRecord.GetCompletionDate());
            Assert.That(response.Score == fakeCitiRecord.Score);
            Assert.That(response.PassingScore == fakeCitiRecord.PassingScore);
            Assert.That(response.ExpirationDate == fakeCitiRecord.ExpirationDate);
            Assert.That(response.GroupName == fakeCitiRecord.Group);
            Assert.That(response.CitiCourseId == fakeCitiRecord.GroupId);
            Assert.That(response.Name == fakeCitiRecord.Name);
            Assert.That(response.UserName == fakeCitiRecord.UserName);
            Assert.That(response.InstitutionalEmailAddress == fakeCitiRecord.InstitutionalEmailAddress);
            Assert.That(response.EmployeeNumber == fakeCitiRecord.EmployeeNumber);
            Assert.That(response.Verified == fakeCitiRecord.Verified);
            Assert.That(response.IsValid == fakeCitiRecord.IsValid);
            Assert.That(response.IsValid == fakeCitiRecord.IsValid);
            Assert.That(response.Inserted == false);
            Assert.That(response.Source == 1);
            mockLearnerWebRepository.Verify(f => f.InsertAppTrainingRecordHistory(It.IsAny<IsuImportHistory>()), Times.Once);
        }

        [Test]
        public void InsertImportHistoryThrowsExceptionTest()
        {
            // Setup
            CitiRecord fakeCitiRecord = fixture.Generate<CitiRecord>();
            fakeCitiRecord.StageNumber = fixture.Generate<Byte>();

            Mock<ILearnerWebRepository> mockLearnerWebRepository = new Mock<ILearnerWebRepository>();
            mockLearnerWebRepository.Setup(f => f.InsertAppTrainingRecordHistory(It.IsAny<IsuImportHistory>())).Throws(new Exception());

            // Execute
            ILearnerWebServices learnerWebServices = new LearnerWebService(mockLearnerWebRepository.Object);
            ActualValueDelegate<object> testDelegate = () => learnerWebServices.InsertImportHistory(fakeCitiRecord);

            // Verify
            Assert.That(testDelegate, Throws.TypeOf<Exception>());
            mockLearnerWebRepository.Verify(f => f.InsertAppTrainingRecordHistory(It.IsAny<IsuImportHistory>()), Times.Once);
        }

        [Test]
        public void IsValidRandomValueTest()
        {
            // Setup
            string citiId = fixture.Generate<string>();
            bool expectedResponse = fixture.Generate<bool>();
            CitiRecord fakeCitiRecord = fixture.Generate<CitiRecord>();
            fakeCitiRecord.CitiId = citiId;
            IsuCitiLwLearners returnIsuCitiLwLearners = fixture.Generate<IsuCitiLwLearners>();
            returnIsuCitiLwLearners.Valid = expectedResponse;

            Mock<ILearnerWebRepository> mockLearnerWebRepository = new Mock<ILearnerWebRepository>();
            mockLearnerWebRepository.Setup(f => f.GetIsuCitiLwLearner(citiId)).Returns(returnIsuCitiLwLearners);

            // Execute
            ILearnerWebServices learnerWebServices = new LearnerWebService(mockLearnerWebRepository.Object);
            bool response = learnerWebServices.IsValid(fakeCitiRecord);

            // Verify
            Assert.That(response == expectedResponse);
            mockLearnerWebRepository.Verify(f => f.GetIsuCitiLwLearner(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void IsValidTrueValueTest()
        {
            // Setup
            string citiId = fixture.Generate<string>();
            bool expectedResponse = true;
            CitiRecord fakeCitiRecord = fixture.Generate<CitiRecord>();
            fakeCitiRecord.CitiId = citiId;
            IsuCitiLwLearners returnIsuCitiLwLearners = fixture.Generate<IsuCitiLwLearners>();
            returnIsuCitiLwLearners.Valid = expectedResponse;

            Mock<ILearnerWebRepository> mockLearnerWebRepository = new Mock<ILearnerWebRepository>();
            mockLearnerWebRepository.Setup(f => f.GetIsuCitiLwLearner(citiId)).Returns(returnIsuCitiLwLearners);

            // Execute
            ILearnerWebServices learnerWebServices = new LearnerWebService(mockLearnerWebRepository.Object);
            bool response = learnerWebServices.IsValid(fakeCitiRecord);

            // Verify
            Assert.That(response == expectedResponse);
            mockLearnerWebRepository.Verify(f => f.GetIsuCitiLwLearner(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void IsValidFalseValueTest()
        {
            // Setup
            string citiId = fixture.Generate<string>();
            bool expectedResponse = false;
            CitiRecord fakeCitiRecord = fixture.Generate<CitiRecord>();
            fakeCitiRecord.CitiId = citiId;
            IsuCitiLwLearners returnIsuCitiLwLearners = fixture.Generate<IsuCitiLwLearners>();
            returnIsuCitiLwLearners.Valid = expectedResponse;

            Mock<ILearnerWebRepository> mockLearnerWebRepository = new Mock<ILearnerWebRepository>();
            mockLearnerWebRepository.Setup(f => f.GetIsuCitiLwLearner(citiId)).Returns(returnIsuCitiLwLearners);

            // Execute
            ILearnerWebServices learnerWebServices = new LearnerWebService(mockLearnerWebRepository.Object);
            bool response = learnerWebServices.IsValid(fakeCitiRecord);

            // Verify
            Assert.That(response == expectedResponse);
            mockLearnerWebRepository.Verify(f => f.GetIsuCitiLwLearner(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void IsValidNullValueTest()
        {
            // Setup
            string citiId = fixture.Generate<string>();
            bool? expectedResponse = null;
            CitiRecord fakeCitiRecord = fixture.Generate<CitiRecord>();
            fakeCitiRecord.CitiId = citiId;
            IsuCitiLwLearners returnIsuCitiLwLearners = fixture.Generate<IsuCitiLwLearners>();
            returnIsuCitiLwLearners.Valid = expectedResponse;

            Mock<ILearnerWebRepository> mockLearnerWebRepository = new Mock<ILearnerWebRepository>();
            mockLearnerWebRepository.Setup(f => f.GetIsuCitiLwLearner(citiId)).Returns(returnIsuCitiLwLearners);

            // Execute
            ILearnerWebServices learnerWebServices = new LearnerWebService(mockLearnerWebRepository.Object);
            bool response = learnerWebServices.IsValid(fakeCitiRecord);

            // Verify
            Assert.That(response == true);
            mockLearnerWebRepository.Verify(f => f.GetIsuCitiLwLearner(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void IsValidGetIsuCitiLwLearnerThrowsExceptionTest()
        {
            // Setup
            string citiId = fixture.Generate<string>();
            bool? expectedResponse = null;
            CitiRecord fakeCitiRecord = fixture.Generate<CitiRecord>();
            fakeCitiRecord.CitiId = citiId;
            IsuCitiLwLearners returnIsuCitiLwLearners = fixture.Generate<IsuCitiLwLearners>();
            returnIsuCitiLwLearners.Valid = expectedResponse;

            Mock<ILearnerWebRepository> mockLearnerWebRepository = new Mock<ILearnerWebRepository>();
            mockLearnerWebRepository.Setup(f => f.GetIsuCitiLwLearner(citiId)).Throws(new Exception());

            // Execute
            ILearnerWebServices learnerWebServices = new LearnerWebService(mockLearnerWebRepository.Object);
            ActualValueDelegate<object> testDelegate = () => learnerWebServices.IsValid(fakeCitiRecord);

            // Verify
            Assert.That(testDelegate, Throws.TypeOf<Exception>());
            mockLearnerWebRepository.Verify(f => f.GetIsuCitiLwLearner(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void IsVerifiedRandomValueTest()
        {
            // Setup
            string citiId = fixture.Generate<string>();
            bool expectedResponse = fixture.Generate<bool>();
            CitiRecord fakeCitiRecord = fixture.Generate<CitiRecord>();
            fakeCitiRecord.CitiId = citiId;
            IsuImportHistory returnIsuImportHistory = fixture.Generate<IsuImportHistory>();
            returnIsuImportHistory.Verified = expectedResponse;

            Mock<ILearnerWebRepository> mockLearnerWebRepository = new Mock<ILearnerWebRepository>();
            mockLearnerWebRepository.Setup(f => f.GetImportHistory(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>())).Returns(returnIsuImportHistory);

            // Execute
            ILearnerWebServices learnerWebServices = new LearnerWebService(mockLearnerWebRepository.Object);
            bool response = learnerWebServices.IsVerified(fakeCitiRecord);

            // Verify
            Assert.That(response == expectedResponse);
            mockLearnerWebRepository.Verify(f => f.GetImportHistory(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()), Times.Once);
        }

        [Test]
        public void IsVerifiedNullValueTest()
        {
            // Setup
            string citiId = fixture.Generate<string>();
            bool? expectedResponse = null;
            CitiRecord fakeCitiRecord = fixture.Generate<CitiRecord>();
            fakeCitiRecord.CitiId = citiId;
            IsuImportHistory returnIsuImportHistory = fixture.Generate<IsuImportHistory>();
            returnIsuImportHistory.Verified = expectedResponse;

            Mock<ILearnerWebRepository> mockLearnerWebRepository = new Mock<ILearnerWebRepository>();
            mockLearnerWebRepository.Setup(f => f.GetImportHistory(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>())).Returns(returnIsuImportHistory);

            // Execute
            ILearnerWebServices learnerWebServices = new LearnerWebService(mockLearnerWebRepository.Object);
            bool response = learnerWebServices.IsVerified(fakeCitiRecord);

            // Verify
            Assert.That(response == false);
            mockLearnerWebRepository.Verify(f => f.GetImportHistory(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()), Times.Once);
        }

        [Test]
        public void IsVerifiedTrueValueTest()
        {
            // Setup
            string citiId = fixture.Generate<string>();
            bool? expectedResponse = true;
            CitiRecord fakeCitiRecord = fixture.Generate<CitiRecord>();
            fakeCitiRecord.CitiId = citiId;
            IsuImportHistory returnIsuImportHistory = fixture.Generate<IsuImportHistory>();
            returnIsuImportHistory.Verified = expectedResponse;

            Mock<ILearnerWebRepository> mockLearnerWebRepository = new Mock<ILearnerWebRepository>();
            mockLearnerWebRepository.Setup(f => f.GetImportHistory(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>())).Returns(returnIsuImportHistory);

            // Execute
            ILearnerWebServices learnerWebServices = new LearnerWebService(mockLearnerWebRepository.Object);
            bool response = learnerWebServices.IsVerified(fakeCitiRecord);

            // Verify
            Assert.That(response == true);
            mockLearnerWebRepository.Verify(f => f.GetImportHistory(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()), Times.Once);
        }

        [Test]
        public void IsVerifiedFalseValueTest()
        {
            // Setup
            string citiId = fixture.Generate<string>();
            bool? expectedResponse = false;
            CitiRecord fakeCitiRecord = fixture.Generate<CitiRecord>();
            fakeCitiRecord.CitiId = citiId;
            IsuImportHistory returnIsuImportHistory = fixture.Generate<IsuImportHistory>();
            returnIsuImportHistory.Verified = expectedResponse;

            Mock<ILearnerWebRepository> mockLearnerWebRepository = new Mock<ILearnerWebRepository>();
            mockLearnerWebRepository.Setup(f => f.GetImportHistory(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>())).Returns(returnIsuImportHistory);

            // Execute
            ILearnerWebServices learnerWebServices = new LearnerWebService(mockLearnerWebRepository.Object);
            bool response = learnerWebServices.IsVerified(fakeCitiRecord);

            // Verify
            Assert.That(response == false);
            mockLearnerWebRepository.Verify(f => f.GetImportHistory(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()), Times.Once);
        }
    }
}
