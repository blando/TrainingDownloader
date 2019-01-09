using System;
using System.Collections.Generic;
using System.Text;
using TrainingDownloader.wrappers;
using TrainingDownloader.services;
using NUnit.Framework.Constraints;
using Moq;
using NUnit.Framework;
using SimpleFixture;
using TrainingDownloader.models;
using System.Net;
using TrainingDownloader.repositories;
using TrainingDownloader.models.entities;
using TrainingDownloader.exceptions;
using TrainingDownloader.services.interfaces;

namespace TrainingDownloaderTests.services
{
    [TestFixture]
    public class LearnerWebServiceTests
    {
        private Fixture fixture;
        private Mock<ILearnerWebRepository> mockLearnerWebRepository;
        private Mock<IVendorUserService> mockVendorUserService;

        public LearnerWebServiceTests()
        {
            this.fixture = new Fixture();
        }

        private void SetupMocks()
        {
            this.mockLearnerWebRepository = new Mock<ILearnerWebRepository>();
            this.mockVendorUserService = new Mock<IVendorUserService>();
        }

        [Test]
        public void CreateHistoryRecordSuccessfulTest()
        {
            // Setup
            SetupMocks();
            VendorRecord testRecord = fixture.Generate<VendorRecord>();

            // Execute
            ILearnerWebServices learnerWebServices = new LearnerWebService(mockLearnerWebRepository.Object, mockVendorUserService.Object);
            History response = learnerWebServices.CreateHistoryRecord(testRecord);

            // Verify
            Assert.That(response.LearnerId == testRecord.UnivId);
            Assert.That(response.CourseId == testRecord.LearnerWebCourseId);
            Assert.That(response.Title == testRecord.VendorCourseName);
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
            SetupMocks();
            string fakeGroupId = fixture.Generate<string>();
            string returnCourseId = fixture.Generate<string>();
            VendorRecord fakeVendorRecord = fixture.Generate<VendorRecord>();
            Courses returnCourse = fixture.Generate<Courses>();
            returnCourse.CourseId = returnCourseId;
            mockLearnerWebRepository.Setup(f => f.GetCourseByVendorCourseId(It.IsAny<string>())).Returns(returnCourse);

            // Execute
            ILearnerWebServices learnerWebServices = new LearnerWebService(mockLearnerWebRepository.Object, mockVendorUserService.Object);
            string response = learnerWebServices.FindCourseId(fakeVendorRecord);

            // Verify
            Assert.That(response == returnCourseId);
            mockLearnerWebRepository.Verify(f => f.GetCourseByVendorCourseId(It.IsAny<string>()), Times.Once);
            mockLearnerWebRepository.Verify(f => f.InsertCourse(It.IsAny<IsuVendorCourses>()), Times.Never);

        }

        [Test]
        public void FindCourseIdNotFoundInsertAndThrowUnknownCourseExceptionTest()
        {
            // Setup
            SetupMocks();
            string fakeGroupId = fixture.Generate<string>();
            string returnCourseId = fixture.Generate<string>();
            VendorRecord fakeVendorRecord = fixture.Generate<VendorRecord>();
            Courses returnCourse = null;
            mockLearnerWebRepository.Setup(f => f.GetCourseByVendorCourseId(It.IsAny<string>())).Returns(returnCourse);
            mockLearnerWebRepository.Setup(f => f.InsertCourse(It.IsAny<IsuVendorCourses>())).Verifiable();

            // Execute
            ILearnerWebServices learnerWebServices = new LearnerWebService(mockLearnerWebRepository.Object, mockVendorUserService.Object);
            ActualValueDelegate<object> testDelegate = () => learnerWebServices.FindCourseId(fakeVendorRecord);

            // Verify
            Assert.That(testDelegate, Throws.TypeOf<UnknownCourseException>());
            mockLearnerWebRepository.Verify(f => f.GetCourseByVendorCourseId(It.IsAny<string>()), Times.Once);
            mockLearnerWebRepository.Verify(f => f.InsertCourse(It.IsAny<IsuVendorCourses>()), Times.Once);
            Mock.VerifyAll(mockLearnerWebRepository);
        }

        [Test]
        public void FindCourseIdGetCourseThrowsExceptionTest()
        {
            // Setup
            SetupMocks();
            string fakeGroupId = fixture.Generate<string>();
            string returnCourseId = fixture.Generate<string>();
            VendorRecord fakeVendorRecord = fixture.Generate<VendorRecord>();
            Courses returnCourse = fixture.Generate<Courses>();
            returnCourse.CourseId = returnCourseId;
            mockLearnerWebRepository.Setup(f => f.GetCourseByVendorCourseId(It.IsAny<string>())).Throws(new Exception());

            // Execute
            ILearnerWebServices learnerWebServices = new LearnerWebService(mockLearnerWebRepository.Object, mockVendorUserService.Object);
            ActualValueDelegate<object> testDelegate = () => learnerWebServices.FindCourseId(fakeVendorRecord);

            // Verify
            Assert.That(testDelegate, Throws.TypeOf<Exception>());
            mockLearnerWebRepository.Verify(f => f.GetCourseByVendorCourseId(It.IsAny<string>()), Times.Once);
            mockLearnerWebRepository.Verify(f => f.InsertCourse(It.IsAny<IsuVendorCourses>()), Times.Never);

        }

        [Test]
        public void FindCourseIdNotFoundInsertThrowsExceptionTest()
        {
            // Setup
            SetupMocks();
            string fakeGroupId = fixture.Generate<string>();
            string returnCourseId = fixture.Generate<string>();
            VendorRecord fakeVendorRecord = fixture.Generate<VendorRecord>();
            Courses returnCourse = null;
            mockLearnerWebRepository.Setup(f => f.GetCourseByVendorCourseId(It.IsAny<string>())).Returns(returnCourse);
            mockLearnerWebRepository.Setup(f => f.InsertCourse(It.IsAny<IsuVendorCourses>())).Throws(new Exception());

            // Execute
            ILearnerWebServices learnerWebServices = new LearnerWebService(mockLearnerWebRepository.Object, mockVendorUserService.Object);
            ActualValueDelegate<object> testDelegate = () => learnerWebServices.FindCourseId(fakeVendorRecord);

            // Verify
            Assert.That(testDelegate, Throws.TypeOf<Exception>());
            mockLearnerWebRepository.Verify(f => f.GetCourseByVendorCourseId(It.IsAny<string>()), Times.Once);
            mockLearnerWebRepository.Verify(f => f.InsertCourse(It.IsAny<IsuVendorCourses>()), Times.Once);
        }

        [Test]
        public void FindUserExistingGetVendorUserTest()
        {
            // Setup
            SetupMocks();
            string vendorUserId = fixture.Generate<string>();
            string univId = fixture.Generate<string>();
            VendorRecord fakeVendorRecord = fixture.Generate<VendorRecord>();
            fakeVendorRecord.VendorUserId = vendorUserId;
            IsuCitiLwLearners isuCitiLwLearners = fixture.Generate<IsuCitiLwLearners>();
            Learners fakeLearner = fixture.Generate<Learners>();
            fakeLearner.LearnerId = univId;
            mockLearnerWebRepository.Setup(f => f.GetLearnerByVendorId(vendorUserId)).Returns(fakeLearner);
            mockVendorUserService.Setup(f => f.CreateVendorUser(fakeVendorRecord)).Returns(isuCitiLwLearners);
            mockLearnerWebRepository.Setup(f => f.UpdateOrInsertVendorUser(It.IsAny<VendorUser>())).Verifiable();

            // Execute
            ILearnerWebServices learnerWebServices = new LearnerWebService(mockLearnerWebRepository.Object, mockVendorUserService.Object);
            string response = learnerWebServices.FindUser(fakeVendorRecord);

            // Verify
            Assert.That(response == univId);
            mockLearnerWebRepository.Verify(f => f.GetLearnerByVendorId(It.IsAny<string>()), Times.Once);
            mockLearnerWebRepository.Verify(f => f.GetLearnerByNetId(It.IsAny<string>()), Times.Never);
            mockLearnerWebRepository.Verify(f => f.GetLearnerByEmail(It.IsAny<string>()), Times.Never);
            mockLearnerWebRepository.Verify(f => f.UpdateOrInsertVendorUser(It.IsAny<IsuCitiLwLearners>()), Times.Once);
        }


        [Test]
        public void FindUserExistingGetLearnerByVendorIdThrowsExceptionTest()
        {
            // Setup
            string citiId = fixture.Generate<string>();
            string univId = fixture.Generate<string>();
            VendorRecord fakeVendorRecord = fixture.Generate<VendorRecord>();
            fakeVendorRecord.VendorUserId = citiId;
            Learners fakeLearner = fixture.Generate<Learners>();
            fakeLearner.LearnerId = univId;
            Mock<ILearnerWebRepository> mockLearnerWebRepository = new Mock<ILearnerWebRepository>();
            mockLearnerWebRepository.Setup(f => f.GetLearnerByVendorId(citiId)).Throws(new Exception());
            mockLearnerWebRepository.Setup(f => f.UpdateOrInsertVendorUser(It.IsAny<IsuCitiLwLearners>())).Verifiable();

            // Execute
            ILearnerWebServices learnerWebServices = new LearnerWebService(mockLearnerWebRepository.Object, mockVendorUserService.Object);
            ActualValueDelegate<object> testDelegate = () => learnerWebServices.FindUser(fakeVendorRecord);

            // Verify
            Assert.That(testDelegate, Throws.TypeOf<Exception>());
            mockLearnerWebRepository.Verify(f => f.GetLearnerByVendorId(It.IsAny<string>()), Times.Once);
            mockLearnerWebRepository.Verify(f => f.GetLearnerByNetId(It.IsAny<string>()), Times.Never);
            mockLearnerWebRepository.Verify(f => f.GetLearnerByEmail(It.IsAny<string>()), Times.Never);
            mockLearnerWebRepository.Verify(f => f.UpdateOrInsertVendorUser(It.IsAny<IsuCitiLwLearners>()), Times.Never);
        }

        [Test]
        public void FindUserValidGetLearnerByNetidTest()
        {
            // Setup
            SetupMocks();
            Mock<ILearnerWebRepository> mockLearnerWebRepository = new Mock<ILearnerWebRepository>();
            string citiId = fixture.Generate<string>();
            string netId = "username";
            string domain = "iastate.edu";
            string univId = fixture.Generate<string>();

            // Mock the first part and return null univId
            VendorRecord fakeVendorRecord = fixture.Generate<VendorRecord>();
            fakeVendorRecord.VendorUserId = citiId;
            fakeVendorRecord.EmailAddress = string.Format("{0}@{1}", netId, domain);

            // Mock the second part and return null univId
            Learners fakeLearner1 = null;
            mockLearnerWebRepository.Setup(f => f.GetLearnerByVendorId(citiId)).Returns(fakeLearner1);

            // Mock the third part and return an actual univId
            Learners fakeLearner2 = fixture.Generate<Learners>();
            fakeLearner2.LearnerId = univId;
            mockLearnerWebRepository.Setup(f => f.GetLearnerByNetId(netId)).Returns(fakeLearner2);

            // Mock Updating IsuCitiLWLearner
            IsuCitiLwLearners isuCitiLwLearners = fixture.Generate<IsuCitiLwLearners>();
            mockVendorUserService.Setup(f => f.CreateVendorUser(It.IsAny<VendorRecord>())).Returns(isuCitiLwLearners);
            mockLearnerWebRepository.Setup(f => f.UpdateOrInsertVendorUser(It.IsAny<VendorUser>())).Verifiable();

            // Execute
            ILearnerWebServices learnerWebServices = new LearnerWebService(mockLearnerWebRepository.Object, mockVendorUserService.Object);
            string response = learnerWebServices.FindUser(fakeVendorRecord);

            // Verify
            Assert.That(response == univId);
            mockLearnerWebRepository.Verify(f => f.GetLearnerByVendorId(It.IsAny<string>()), Times.Once);
            mockLearnerWebRepository.Verify(f => f.GetLearnerByNetId(It.IsAny<string>()), Times.Once);
            mockLearnerWebRepository.Verify(f => f.GetLearnerByEmail(It.IsAny<string>()), Times.Never);
            mockLearnerWebRepository.Verify(f => f.UpdateOrInsertVendorUser(It.IsAny<IsuCitiLwLearners>()), Times.Once);
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
            VendorRecord fakeVendorRecord = fixture.Generate<VendorRecord>();
            fakeVendorRecord.VendorUserId = citiId;
            fakeVendorRecord.EmailAddress = string.Format("{0}@{1}", netId, domain);

            // Mock the second part and return null univId
            Learners fakeLearner1 = null;
            mockLearnerWebRepository.Setup(f => f.GetLearnerByVendorId(citiId)).Returns(fakeLearner1);

            // Mock the third part and return an actual univId
            Learners fakeLearner2 = fixture.Generate<Learners>();
            fakeLearner2.LearnerId = univId;
            mockLearnerWebRepository.Setup(f => f.GetLearnerByNetId(netId)).Throws(new Exception());

            // Mock the call to update the cached record
            mockLearnerWebRepository.Setup(f => f.UpdateOrInsertVendorUser(It.IsAny<IsuCitiLwLearners>())).Verifiable();

            // Execute
            ILearnerWebServices learnerWebServices = new LearnerWebService(mockLearnerWebRepository.Object, mockVendorUserService.Object);
            ActualValueDelegate<object> testDelegate = () => learnerWebServices.FindUser(fakeVendorRecord);

            // Verify
            Assert.That(testDelegate, Throws.TypeOf<Exception>());
            mockLearnerWebRepository.Verify(f => f.GetLearnerByVendorId(It.IsAny<string>()), Times.Once);
            mockLearnerWebRepository.Verify(f => f.GetLearnerByNetId(It.IsAny<string>()), Times.Once);
            mockLearnerWebRepository.Verify(f => f.GetLearnerByEmail(It.IsAny<string>()), Times.Never);
            mockLearnerWebRepository.Verify(f => f.UpdateOrInsertVendorUser(It.IsAny<IsuCitiLwLearners>()), Times.Never);
        }

        [Test]
        public void FindUserValidGetLearnerByNetidWwithNonIastateEmailTest()
        {
            // Setup
            SetupMocks();
            Mock<ILearnerWebRepository> mockLearnerWebRepository = new Mock<ILearnerWebRepository>();
            string citiId = fixture.Generate<string>();
            string netId = "username";
            string domain = "notiastate.edu";
            string univId = fixture.Generate<string>();

            // Mock the first part and return null univId
            VendorRecord fakeVendorRecord = fixture.Generate<VendorRecord>();
            fakeVendorRecord.VendorUserId = citiId;
            fakeVendorRecord.EmailAddress = string.Format("{0}@{1}", netId, domain);
            Learners fakeLearner1 = null;
            mockLearnerWebRepository.Setup(f => f.GetLearnerByVendorId(citiId)).Returns(fakeLearner1);

            // Mock IsValid
            IsuCitiLwLearners isuCitiLwLearners = fixture.Generate<IsuCitiLwLearners>();
            isuCitiLwLearners.Valid = true;
            mockLearnerWebRepository.Setup(f => f.GetVendorUser(fakeVendorRecord.VendorUserId)).Returns(isuCitiLwLearners);

            // NetId will return null

            // Mock the fourth part and return a univId
            Learners fakeLearner2 = fixture.Generate<Learners>();
            fakeLearner2.LearnerId = univId;
            mockLearnerWebRepository.Setup(f => f.GetLearnerByEmail(fakeVendorRecord.EmailAddress)).Returns(fakeLearner2);

            // Mock the call to update the cached record
            mockLearnerWebRepository.Setup(f => f.UpdateOrInsertVendorUser(It.IsAny<IsuCitiLwLearners>())).Verifiable();

            // Execute
            ILearnerWebServices learnerWebServices = new LearnerWebService(mockLearnerWebRepository.Object, mockVendorUserService.Object);
            string response = learnerWebServices.FindUser(fakeVendorRecord);

            // Verify
            Assert.That(response == univId);
            mockLearnerWebRepository.Verify(f => f.GetLearnerByVendorId(It.IsAny<string>()), Times.Once);
            mockLearnerWebRepository.Verify(f => f.GetLearnerByNetId(It.IsAny<string>()), Times.Never);
            mockLearnerWebRepository.Verify(f => f.GetLearnerByEmail(It.IsAny<string>()), Times.Once);
            mockLearnerWebRepository.Verify(f => f.UpdateOrInsertVendorUser(It.IsAny<IsuCitiLwLearners>()), Times.Once);
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
            VendorRecord fakeVendorRecord = fixture.Generate<VendorRecord>();
            fakeVendorRecord.VendorUserId = citiId;
            fakeVendorRecord.EmailAddress = string.Format("{0}@{1}", netId, domain);

            // Mock the second part and return null univId
            Learners fakeLearner1 = null;
            mockLearnerWebRepository.Setup(f => f.GetLearnerByVendorId(citiId)).Returns(fakeLearner1);

            // Mock the third part and return null univId
            Learners fakeLearner2 = null;
            mockLearnerWebRepository.Setup(f => f.GetLearnerByNetId(netId)).Returns(fakeLearner2);

            // Mock the fourth part and return an actual univId
            Learners fakeLearner3 = fixture.Generate<Learners>();
            fakeLearner3.LearnerId = univId;
            mockLearnerWebRepository.Setup(f => f.GetLearnerByEmail(fakeVendorRecord.EmailAddress)).Returns(fakeLearner3);

            // Mock the call to update the cached record
            mockLearnerWebRepository.Setup(f => f.UpdateOrInsertVendorUser(It.IsAny<IsuCitiLwLearners>())).Verifiable();

            // Execute
            ILearnerWebServices learnerWebServices = new LearnerWebService(mockLearnerWebRepository.Object, mockVendorUserService.Object);
            string response = learnerWebServices.FindUser(fakeVendorRecord);

            // Verify
            Assert.That(response == univId);
            mockLearnerWebRepository.Verify(f => f.GetVendorUser(It.IsAny<string>()), Times.Once);
            mockLearnerWebRepository.Verify(f => f.GetLearnerByVendorId(It.IsAny<string>()), Times.Once);
            mockLearnerWebRepository.Verify(f => f.GetLearnerByNetId(It.IsAny<string>()), Times.Once);
            mockLearnerWebRepository.Verify(f => f.GetLearnerByEmail(It.IsAny<string>()), Times.Once);
            mockLearnerWebRepository.Verify(f => f.UpdateOrInsertVendorUser(It.IsAny<IsuCitiLwLearners>()), Times.Once);
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
            VendorRecord fakeVendorRecord = fixture.Generate<VendorRecord>();
            fakeVendorRecord.VendorUserId = citiId;
            fakeVendorRecord.EmailAddress = string.Format("{0}@{1}", netId, domain);

            // Mock the second part and return null univId
            Learners fakeLearner1 = null;
            mockLearnerWebRepository.Setup(f => f.GetLearnerByVendorId(citiId)).Returns(fakeLearner1);

            // Mock the third part and return null univId
            Learners fakeLearner2 = null;
            mockLearnerWebRepository.Setup(f => f.GetLearnerByNetId(netId)).Returns(fakeLearner2);

            // Mock the fourth part and return an actual univId
            Learners fakeLearner3 = null;
            mockLearnerWebRepository.Setup(f => f.GetLearnerByEmail(fakeVendorRecord.EmailAddress)).Returns(fakeLearner3);

            // Execute
            ILearnerWebServices learnerWebServices = new LearnerWebService(mockLearnerWebRepository.Object, mockVendorUserService.Object);
            ActualValueDelegate<object> testDelegate = () => learnerWebServices.FindUser(fakeVendorRecord);

            // Verify
            Assert.That(testDelegate, Throws.TypeOf<UnknownUserException>());
            mockLearnerWebRepository.Verify(f => f.GetLearnerByVendorId(It.IsAny<string>()), Times.Once);
            mockLearnerWebRepository.Verify(f => f.GetLearnerByNetId(It.IsAny<string>()), Times.Once);
            mockLearnerWebRepository.Verify(f => f.GetLearnerByEmail(It.IsAny<string>()), Times.Once);
            mockLearnerWebRepository.Verify(f => f.UpdateOrInsertVendorUser(It.IsAny<IsuCitiLwLearners>()), Times.Never);

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
            VendorRecord fakeVendorRecord = fixture.Generate<VendorRecord>();
            fakeVendorRecord.VendorUserId = citiId;
            fakeVendorRecord.EmailAddress = string.Format("{0}@{1}", netId, domain);
            IsuCitiLwLearners fakeIsuCitiLwLearners = new IsuCitiLwLearners();
            fakeIsuCitiLwLearners.LwLearnerId = null;
            fakeIsuCitiLwLearners.Valid = false;
            mockLearnerWebRepository.Setup(f => f.GetVendorUser(citiId)).Returns(fakeIsuCitiLwLearners);

            // Mock the second part and return null univId
            Learners fakeLearner1 = null;
            mockLearnerWebRepository.Setup(f => f.GetLearnerByVendorId(citiId)).Returns(fakeLearner1);

            // Execute
            ILearnerWebServices learnerWebServices = new LearnerWebService(mockLearnerWebRepository.Object, mockVendorUserService.Object);
            ActualValueDelegate<object> testDelegate = () => learnerWebServices.FindUser(fakeVendorRecord);

            // Verify
            Assert.That(testDelegate, Throws.TypeOf<InvalidUserException>());
            mockLearnerWebRepository.Verify(f => f.GetVendorUser(It.IsAny<string>()), Times.Once);
            mockLearnerWebRepository.Verify(f => f.GetLearnerByVendorId(It.IsAny<string>()), Times.Once);
            mockLearnerWebRepository.Verify(f => f.GetLearnerByNetId(It.IsAny<string>()), Times.Never);
            mockLearnerWebRepository.Verify(f => f.GetLearnerByEmail(It.IsAny<string>()), Times.Never);
            mockLearnerWebRepository.Verify(f => f.UpdateOrInsertVendorUser(It.IsAny<IsuCitiLwLearners>()), Times.Never);

        }

        [Test]
        public void GetHistoryByCitiIdCourseIdDateTest()
        {
            // Setup
            SetupMocks();
            VendorRecord VendorRecord = fixture.Generate<VendorRecord>();
            History fakeHistory = fixture.Generate<History>();
            mockLearnerWebRepository.Setup(f => f.GetHistoryRecordByLearnerCourseDate(VendorRecord.UnivId, VendorRecord.LearnerWebCourseId, VendorRecord.GetCompletionDate())).Returns(fakeHistory);

            IsuImportHistory fakeIsuImportHistory = fixture.Generate<IsuImportHistory>();
            mockLearnerWebRepository.Setup(f => f.GetImportHistory(VendorRecord.VendorUserId, VendorRecord.VendorCourseId, VendorRecord.GetCompletionDate())).Returns(fakeIsuImportHistory);
            mockLearnerWebRepository.Setup(f => f.UpdateImportHistoryWithCurriculaId(fakeIsuImportHistory)).Verifiable();

            // Execute
            ILearnerWebServices learnerWebServices = new LearnerWebService(mockLearnerWebRepository.Object, mockVendorUserService.Object);
            History response = learnerWebServices.GetHistoryByVendorIdCourseIdDate(VendorRecord);

            // Verify
            Assert.That(response == fakeHistory);
            mockLearnerWebRepository.Verify(f => f.GetHistoryRecordByLearnerCourseDate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()), Times.Once);
            mockLearnerWebRepository.Verify(f => f.GetImportHistory(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()), Times.Once);
            mockLearnerWebRepository.Verify(f => f.UpdateImportHistoryWithCurriculaId(It.IsAny<IsuImportHistory>()), Times.Once);
            mockLearnerWebRepository.VerifyNoOtherCalls();
            Mock.VerifyAll(mockLearnerWebRepository);
        }


        [Test]
        public void GetHistoryByCitiIdCourseIdDateNullTest()
        {
            // Setup
            SetupMocks();
            VendorRecord VendorRecord = fixture.Generate<VendorRecord>();
            History fakeHistory = null;
            mockLearnerWebRepository.Setup(f => f.GetHistoryRecordByLearnerCourseDate(VendorRecord.UnivId, VendorRecord.LearnerWebCourseId, VendorRecord.GetCompletionDate())).Returns(fakeHistory);

            // Execute
            ILearnerWebServices learnerWebServices = new LearnerWebService(mockLearnerWebRepository.Object, mockVendorUserService.Object);
            History response = learnerWebServices.GetHistoryByVendorIdCourseIdDate(VendorRecord);

            // Verify
            Assert.That(response == fakeHistory);
            mockLearnerWebRepository.Verify(f => f.GetHistoryRecordByLearnerCourseDate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()), Times.Once);
            mockLearnerWebRepository.Verify(f => f.GetImportHistory(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()), Times.Never);
            mockLearnerWebRepository.Verify(f => f.UpdateImportHistoryWithCurriculaId(It.IsAny<IsuImportHistory>()), Times.Never);
            mockLearnerWebRepository.VerifyNoOtherCalls();
            Mock.VerifyAll(mockLearnerWebRepository);
        }



        [Test]
        public void GetHistoryByCurriculaIdTest()
        {
            // Setup
            int curriculaId = fixture.Generate<int>();

            VendorRecord fakeVendorRecord = fixture.Generate<VendorRecord>();
            IsuImportHistory returnIsuImportHistory = fixture.Generate<IsuImportHistory>();
            returnIsuImportHistory.CurriculaId = curriculaId;

            Mock<ILearnerWebRepository> mockLearnerWebRepository = new Mock<ILearnerWebRepository>();
            mockLearnerWebRepository.Setup(f => f.GetImportHistory(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>())).Returns(returnIsuImportHistory);

            History returnHistory = fixture.Generate<History>();
            mockLearnerWebRepository.Setup(f => f.GetHistoryRecordByCurriculaId(curriculaId)).Returns(returnHistory);

            // Execute
            ILearnerWebServices learnerWebServices = new LearnerWebService(mockLearnerWebRepository.Object, mockVendorUserService.Object);
            History response = learnerWebServices.GetHistoryByCurriculaId(fakeVendorRecord);

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

            VendorRecord fakeVendorRecord = fixture.Generate<VendorRecord>();
            IsuImportHistory returnIsuImportHistory = fixture.Generate<IsuImportHistory>();
            returnIsuImportHistory.CurriculaId = curriculaId;

            Mock<ILearnerWebRepository> mockLearnerWebRepository = new Mock<ILearnerWebRepository>();
            mockLearnerWebRepository.Setup(f => f.GetImportHistory(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>())).Returns(returnIsuImportHistory);

            History returnHistory = fixture.Generate<History>();
            mockLearnerWebRepository.Setup(f => f.GetHistoryRecordByCurriculaId(curriculaId)).Throws(new Exception());

            // Execute
            ILearnerWebServices learnerWebServices = new LearnerWebService(mockLearnerWebRepository.Object, mockVendorUserService.Object);
            ActualValueDelegate<object> testDelegate = () => learnerWebServices.GetHistoryByCurriculaId(fakeVendorRecord);

            // Verify
            Assert.That(testDelegate, Throws.TypeOf<Exception>());
            mockLearnerWebRepository.Verify(f => f.GetImportHistory(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()), Times.Once);
            mockLearnerWebRepository.Verify(f => f.GetHistoryRecordByCurriculaId(It.IsAny<int>()), Times.Once);
        }

        [Test]
        public void GetHistoryByCurriculaIdNullValueCurriculaIdTest()
        {
            // Setup
            int? curriculaId = null;

            VendorRecord fakeVendorRecord = fixture.Generate<VendorRecord>();
            IsuImportHistory returnIsuImportHistory = fixture.Generate<IsuImportHistory>();
            returnIsuImportHistory.CurriculaId = curriculaId;

            Mock<ILearnerWebRepository> mockLearnerWebRepository = new Mock<ILearnerWebRepository>();
            mockLearnerWebRepository.Setup(f => f.GetImportHistory(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>())).Returns(returnIsuImportHistory);

            // Execute
            ILearnerWebServices learnerWebServices = new LearnerWebService(mockLearnerWebRepository.Object, mockVendorUserService.Object);
            History response = learnerWebServices.GetHistoryByCurriculaId(fakeVendorRecord);

            // Verify
            Assert.That(response == null);
            mockLearnerWebRepository.Verify(f => f.GetImportHistory(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()), Times.Once);
            mockLearnerWebRepository.Verify(f => f.GetHistoryRecordByCurriculaId(It.IsAny<int>()), Times.Never);
        }

        [Test]
        public void GetHistoryByCurriculaIdNullIsuImportHistoryTest()
        {
            // Setup

            VendorRecord fakeVendorRecord = fixture.Generate<VendorRecord>();
            IsuImportHistory returnIsuImportHistory = null;

            Mock<ILearnerWebRepository> mockLearnerWebRepository = new Mock<ILearnerWebRepository>();
            mockLearnerWebRepository.Setup(f => f.GetImportHistory(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>())).Returns(returnIsuImportHistory);

            // Execute
            ILearnerWebServices learnerWebServices = new LearnerWebService(mockLearnerWebRepository.Object, mockVendorUserService.Object);
            History response = learnerWebServices.GetHistoryByCurriculaId(fakeVendorRecord);

            // Verify
            Assert.That(response == null);
            mockLearnerWebRepository.Verify(f => f.GetImportHistory(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()), Times.Once);
            mockLearnerWebRepository.Verify(f => f.GetHistoryRecordByCurriculaId(It.IsAny<int>()), Times.Never);
        }

        [Test]
        public void GetHistoryByCurriculaIdGetImportHistoryThrowsExceptionTest()
        {
            // Setup

            VendorRecord fakeVendorRecord = fixture.Generate<VendorRecord>();
            IsuImportHistory returnIsuImportHistory = fixture.Generate<IsuImportHistory>();

            Mock<ILearnerWebRepository> mockLearnerWebRepository = new Mock<ILearnerWebRepository>();
            mockLearnerWebRepository.Setup(f => f.GetImportHistory(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>())).Throws(new Exception());

            // Execute
            ILearnerWebServices learnerWebServices = new LearnerWebService(mockLearnerWebRepository.Object, mockVendorUserService.Object);
            ActualValueDelegate<object> testDelegate = () => learnerWebServices.GetHistoryByCurriculaId(fakeVendorRecord);

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
            ILearnerWebServices learnerWebServices = new LearnerWebService(mockLearnerWebRepository.Object, mockVendorUserService.Object);
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
            ILearnerWebServices learnerWebServices = new LearnerWebService(mockLearnerWebRepository.Object, mockVendorUserService.Object);
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
            ILearnerWebServices learnerWebServices = new LearnerWebService(mockLearnerWebRepository.Object, mockVendorUserService.Object);

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
            ILearnerWebServices learnerWebServices = new LearnerWebService(mockLearnerWebRepository.Object, mockVendorUserService.Object);

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
            SetupMocks();
            VendorRecord fakeVendorRecord = fixture.Generate<VendorRecord>();
            fakeVendorRecord.StageNumber = fixture.Generate<Byte>();
            IsuImportHistory isuImportHistory = new IsuImportHistory()
            {
                VendorUserId = fakeVendorRecord.VendorUserId,
                FirstName = fakeVendorRecord.FirstName,
                LastName = fakeVendorRecord.LastName,
                EmailAddress = fakeVendorRecord.EmailAddress,
                RegistrationDate = fakeVendorRecord.RegistrationDate,
                CourseName = fakeVendorRecord.VendorCourseName,
                StageNumber = byte.Parse(fakeVendorRecord.StageNumber.ToString()),
                StageDescription = fakeVendorRecord.StageDescription,
                CompletionReportNum = fakeVendorRecord.CompletionReportNum,
                CompletionDate = fakeVendorRecord.GetCompletionDate(),
                Score = fakeVendorRecord.Score,
                PassingScore = fakeVendorRecord.PassingScore,
                ExpirationDate = fakeVendorRecord.ExpirationDate,
                VendorCourseId = fakeVendorRecord.VendorCourseId,
                Name = fakeVendorRecord.Name,
                UserName = fakeVendorRecord.UserName,
                InstitutionalEmailAddress = fakeVendorRecord.InstitutionalEmailAddress,
                EmployeeNumber = fakeVendorRecord.EmployeeNumber,
                Verified = fakeVendorRecord.Verified,
                IsValid = fakeVendorRecord.IsValid,
                Inserted = false,
                Source = 1
            };
            
            mockLearnerWebRepository.Setup(f => f.InsertAppTrainingRecordHistory(It.IsAny<IsuImportHistory>())).Returns(isuImportHistory).Verifiable();

            // Execute
            ILearnerWebServices learnerWebServices = new LearnerWebService(mockLearnerWebRepository.Object, mockVendorUserService.Object);
            IsuImportHistory response = learnerWebServices.InsertImportHistory(fakeVendorRecord);

            // Verify
            Assert.That(response.VendorUserId == fakeVendorRecord.VendorUserId);
            Assert.That(response.FirstName == fakeVendorRecord.FirstName);
            Assert.That(response.LastName == fakeVendorRecord.LastName);
            Assert.That(response.EmailAddress == fakeVendorRecord.EmailAddress);
            Assert.That(response.RegistrationDate == fakeVendorRecord.RegistrationDate);
            Assert.That(response.CourseName == fakeVendorRecord.VendorCourseName);
            Assert.That(response.StageNumber == byte.Parse(fakeVendorRecord.StageNumber.ToString()));
            Assert.That(response.StageDescription == fakeVendorRecord.StageDescription);
            Assert.That(response.CompletionReportNum == fakeVendorRecord.CompletionReportNum);
            Assert.That(response.CompletionDate == fakeVendorRecord.GetCompletionDate());
            Assert.That(response.Score == fakeVendorRecord.Score);
            Assert.That(response.PassingScore == fakeVendorRecord.PassingScore);
            Assert.That(response.ExpirationDate == fakeVendorRecord.ExpirationDate);
            Assert.That(response.VendorCourseId == fakeVendorRecord.VendorCourseId);
            Assert.That(response.Name == fakeVendorRecord.Name);
            Assert.That(response.UserName == fakeVendorRecord.UserName);
            Assert.That(response.InstitutionalEmailAddress == fakeVendorRecord.InstitutionalEmailAddress);
            Assert.That(response.EmployeeNumber == fakeVendorRecord.EmployeeNumber);
            Assert.That(response.Verified == fakeVendorRecord.Verified);
            Assert.That(response.IsValid == fakeVendorRecord.IsValid);
            Assert.That(response.Inserted == false);
            Assert.That(response.Source == 1);
            Mock.VerifyAll(mockLearnerWebRepository);
            mockLearnerWebRepository.Verify(f => f.InsertAppTrainingRecordHistory(It.IsAny<IsuImportHistory>()), Times.Once);
        }

        [Test]
        public void InsertImportHistoryThrowsExceptionTest()
        {
            // Setup
            VendorRecord fakeVendorRecord = fixture.Generate<VendorRecord>();
            fakeVendorRecord.StageNumber = fixture.Generate<Byte>();

            Mock<ILearnerWebRepository> mockLearnerWebRepository = new Mock<ILearnerWebRepository>();
            mockLearnerWebRepository.Setup(f => f.InsertAppTrainingRecordHistory(It.IsAny<IsuImportHistory>())).Throws(new Exception());

            // Execute
            ILearnerWebServices learnerWebServices = new LearnerWebService(mockLearnerWebRepository.Object, mockVendorUserService.Object);
            ActualValueDelegate<object> testDelegate = () => learnerWebServices.InsertImportHistory(fakeVendorRecord);

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
            VendorRecord fakeVendorRecord = fixture.Generate<VendorRecord>();
            fakeVendorRecord.VendorUserId = citiId;
            IsuCitiLwLearners returnIsuCitiLwLearners = fixture.Generate<IsuCitiLwLearners>();
            returnIsuCitiLwLearners.Valid = expectedResponse;

            Mock<ILearnerWebRepository> mockLearnerWebRepository = new Mock<ILearnerWebRepository>();
            mockLearnerWebRepository.Setup(f => f.GetVendorUser(citiId)).Returns(returnIsuCitiLwLearners);

            // Execute
            ILearnerWebServices learnerWebServices = new LearnerWebService(mockLearnerWebRepository.Object, mockVendorUserService.Object);
            bool response = learnerWebServices.IsValid(fakeVendorRecord);

            // Verify
            Assert.That(response == expectedResponse);
            mockLearnerWebRepository.Verify(f => f.GetVendorUser(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void IsValidTrueValueTest()
        {
            // Setup
            string citiId = fixture.Generate<string>();
            bool expectedResponse = true;
            VendorRecord fakeVendorRecord = fixture.Generate<VendorRecord>();
            fakeVendorRecord.VendorUserId = citiId;
            IsuCitiLwLearners returnIsuCitiLwLearners = fixture.Generate<IsuCitiLwLearners>();
            returnIsuCitiLwLearners.Valid = expectedResponse;

            Mock<ILearnerWebRepository> mockLearnerWebRepository = new Mock<ILearnerWebRepository>();
            mockLearnerWebRepository.Setup(f => f.GetVendorUser(citiId)).Returns(returnIsuCitiLwLearners);

            // Execute
            ILearnerWebServices learnerWebServices = new LearnerWebService(mockLearnerWebRepository.Object, mockVendorUserService.Object);
            bool response = learnerWebServices.IsValid(fakeVendorRecord);

            // Verify
            Assert.That(response == expectedResponse);
            mockLearnerWebRepository.Verify(f => f.GetVendorUser(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void IsValidFalseValueTest()
        {
            // Setup
            string citiId = fixture.Generate<string>();
            bool expectedResponse = false;
            VendorRecord fakeVendorRecord = fixture.Generate<VendorRecord>();
            fakeVendorRecord.VendorUserId = citiId;
            IsuCitiLwLearners returnIsuCitiLwLearners = fixture.Generate<IsuCitiLwLearners>();
            returnIsuCitiLwLearners.Valid = expectedResponse;

            Mock<ILearnerWebRepository> mockLearnerWebRepository = new Mock<ILearnerWebRepository>();
            mockLearnerWebRepository.Setup(f => f.GetVendorUser(citiId)).Returns(returnIsuCitiLwLearners);

            // Execute
            ILearnerWebServices learnerWebServices = new LearnerWebService(mockLearnerWebRepository.Object, mockVendorUserService.Object);
            bool response = learnerWebServices.IsValid(fakeVendorRecord);

            // Verify
            Assert.That(response == expectedResponse);
            mockLearnerWebRepository.Verify(f => f.GetVendorUser(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void IsValidNullValueTest()
        {
            // Setup
            string citiId = fixture.Generate<string>();
            bool? expectedResponse = null;
            VendorRecord fakeVendorRecord = fixture.Generate<VendorRecord>();
            fakeVendorRecord.VendorUserId = citiId;
            IsuCitiLwLearners returnIsuCitiLwLearners = fixture.Generate<IsuCitiLwLearners>();
            returnIsuCitiLwLearners.Valid = expectedResponse;

            Mock<ILearnerWebRepository> mockLearnerWebRepository = new Mock<ILearnerWebRepository>();
            mockLearnerWebRepository.Setup(f => f.GetVendorUser(citiId)).Returns(returnIsuCitiLwLearners);

            // Execute
            ILearnerWebServices learnerWebServices = new LearnerWebService(mockLearnerWebRepository.Object, mockVendorUserService.Object);
            bool response = learnerWebServices.IsValid(fakeVendorRecord);

            // Verify
            Assert.That(response == true);
            mockLearnerWebRepository.Verify(f => f.GetVendorUser(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void IsValidGetVendorUserThrowsExceptionTest()
        {
            // Setup
            string citiId = fixture.Generate<string>();
            bool? expectedResponse = null;
            VendorRecord fakeVendorRecord = fixture.Generate<VendorRecord>();
            fakeVendorRecord.VendorUserId = citiId;
            IsuCitiLwLearners returnIsuCitiLwLearners = fixture.Generate<IsuCitiLwLearners>();
            returnIsuCitiLwLearners.Valid = expectedResponse;

            Mock<ILearnerWebRepository> mockLearnerWebRepository = new Mock<ILearnerWebRepository>();
            mockLearnerWebRepository.Setup(f => f.GetVendorUser(citiId)).Throws(new Exception());

            // Execute
            ILearnerWebServices learnerWebServices = new LearnerWebService(mockLearnerWebRepository.Object, mockVendorUserService.Object);
            ActualValueDelegate<object> testDelegate = () => learnerWebServices.IsValid(fakeVendorRecord);

            // Verify
            Assert.That(testDelegate, Throws.TypeOf<Exception>());
            mockLearnerWebRepository.Verify(f => f.GetVendorUser(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void IsVerifiedRandomValueTest()
        {
            // Setup
            string citiId = fixture.Generate<string>();
            bool expectedResponse = fixture.Generate<bool>();
            VendorRecord fakeVendorRecord = fixture.Generate<VendorRecord>();
            fakeVendorRecord.VendorUserId = citiId;
            IsuImportHistory returnIsuImportHistory = fixture.Generate<IsuImportHistory>();
            returnIsuImportHistory.Verified = expectedResponse;

            Mock<ILearnerWebRepository> mockLearnerWebRepository = new Mock<ILearnerWebRepository>();
            mockLearnerWebRepository.Setup(f => f.GetImportHistory(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>())).Returns(returnIsuImportHistory);

            // Execute
            ILearnerWebServices learnerWebServices = new LearnerWebService(mockLearnerWebRepository.Object, mockVendorUserService.Object);
            bool response = learnerWebServices.IsVerified(fakeVendorRecord);

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
            VendorRecord fakeVendorRecord = fixture.Generate<VendorRecord>();
            fakeVendorRecord.VendorUserId = citiId;
            IsuImportHistory returnIsuImportHistory = fixture.Generate<IsuImportHistory>();
            returnIsuImportHistory.Verified = expectedResponse;

            Mock<ILearnerWebRepository> mockLearnerWebRepository = new Mock<ILearnerWebRepository>();
            mockLearnerWebRepository.Setup(f => f.GetImportHistory(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>())).Returns(returnIsuImportHistory);

            // Execute
            ILearnerWebServices learnerWebServices = new LearnerWebService(mockLearnerWebRepository.Object, mockVendorUserService.Object);
            bool response = learnerWebServices.IsVerified(fakeVendorRecord);

            // Verify
            Assert.That(response == false);
            mockLearnerWebRepository.Verify(f => f.GetImportHistory(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()), Times.Once);
        }

        [Test]
        public void IsVerifiedNullTest()
        {
            // Setup
            string citiId = fixture.Generate<string>();
            bool? expectedResponse = null;
            VendorRecord fakeVendorRecord = fixture.Generate<VendorRecord>();
            fakeVendorRecord.VendorUserId = citiId;
            IsuImportHistory returnIsuImportHistory = null;

            Mock<ILearnerWebRepository> mockLearnerWebRepository = new Mock<ILearnerWebRepository>();
            mockLearnerWebRepository.Setup(f => f.GetImportHistory(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>())).Returns(returnIsuImportHistory);

            // Execute
            ILearnerWebServices learnerWebServices = new LearnerWebService(mockLearnerWebRepository.Object, mockVendorUserService.Object);
            bool response = learnerWebServices.IsVerified(fakeVendorRecord);

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
            VendorRecord fakeVendorRecord = fixture.Generate<VendorRecord>();
            fakeVendorRecord.VendorUserId = citiId;
            IsuImportHistory returnIsuImportHistory = fixture.Generate<IsuImportHistory>();
            returnIsuImportHistory.Verified = expectedResponse;

            Mock<ILearnerWebRepository> mockLearnerWebRepository = new Mock<ILearnerWebRepository>();
            mockLearnerWebRepository.Setup(f => f.GetImportHistory(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>())).Returns(returnIsuImportHistory);

            // Execute
            ILearnerWebServices learnerWebServices = new LearnerWebService(mockLearnerWebRepository.Object, mockVendorUserService.Object);
            bool response = learnerWebServices.IsVerified(fakeVendorRecord);

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
            VendorRecord fakeVendorRecord = fixture.Generate<VendorRecord>();
            fakeVendorRecord.VendorUserId = citiId;
            IsuImportHistory returnIsuImportHistory = fixture.Generate<IsuImportHistory>();
            returnIsuImportHistory.Verified = expectedResponse;

            Mock<ILearnerWebRepository> mockLearnerWebRepository = new Mock<ILearnerWebRepository>();
            mockLearnerWebRepository.Setup(f => f.GetImportHistory(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>())).Returns(returnIsuImportHistory);

            // Execute
            ILearnerWebServices learnerWebServices = new LearnerWebService(mockLearnerWebRepository.Object, mockVendorUserService.Object);
            bool response = learnerWebServices.IsVerified(fakeVendorRecord);

            // Verify
            Assert.That(response == false);
            mockLearnerWebRepository.Verify(f => f.GetImportHistory(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()), Times.Once);
        }

        [Test]
        public void SetInsertedForImportRecordTest()
        {
            // Setup
            int fakeId = fixture.Generate<int>();
            Mock<ILearnerWebRepository> mockLearnerWebRepository = new Mock<ILearnerWebRepository>();
            mockLearnerWebRepository.Setup(f => f.SetInsertedForImportRecord(fakeId)).Verifiable();

            // Execute
            ILearnerWebServices learnerWebServices = new LearnerWebService(mockLearnerWebRepository.Object, mockVendorUserService.Object);
            learnerWebServices.SetInsertedForImportRecord(fakeId);

            // Verify
            mockLearnerWebRepository.Verify(f => f.SetInsertedForImportRecord(It.IsAny<int>()), Times.Once);
            mockLearnerWebRepository.VerifyNoOtherCalls();
            Mock.VerifyAll(mockLearnerWebRepository);
        }

        [Test]
        public void UpdateImportHistoryWithCurriculaIdTest()
        {
            // Setup
            VendorRecord fakeVendorRecord = fixture.Generate<VendorRecord>();
            History fakeHistory = fixture.Generate<History>();
            Mock<ILearnerWebRepository> mockLearnerWebRepository = new Mock<ILearnerWebRepository>();
            IsuImportHistory fakeIsuImportHistory = fixture.Generate<IsuImportHistory>();
            IsuImportHistory fakeIsuImportHistory2 = fakeIsuImportHistory;
            fakeIsuImportHistory2.CurriculaId = fakeHistory.CurriculaId;
            mockLearnerWebRepository.Setup(f => f.GetImportHistory(fakeVendorRecord.VendorUserId, fakeVendorRecord.VendorCourseId, fakeVendorRecord.GetCompletionDate())).Returns(fakeIsuImportHistory);
            mockLearnerWebRepository.Setup(f => f.UpdateImportHistoryWithCurriculaId(fakeIsuImportHistory2)).Verifiable();

            // Execute
            ILearnerWebServices learnerWebServices = new LearnerWebService(mockLearnerWebRepository.Object, mockVendorUserService.Object);
            learnerWebServices.UpdateImportHistoryWithCurriculaId(fakeVendorRecord, fakeHistory);

            // Verify
            mockLearnerWebRepository.Verify(f => f.GetImportHistory(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()), Times.Once);
            mockLearnerWebRepository.Verify(f => f.UpdateImportHistoryWithCurriculaId(It.IsAny<IsuImportHistory>()), Times.Once);
            mockLearnerWebRepository.VerifyNoOtherCalls();
        }
    }
}
