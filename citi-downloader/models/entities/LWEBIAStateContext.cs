using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CitiDownloader.models.entities
{
    public partial class LWEBIAStateContext : DbContext
    {
        public LWEBIAStateContext()
        {
        }

        public LWEBIAStateContext(DbContextOptions<LWEBIAStateContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Courses> Courses { get; set; }
        public virtual DbSet<History> History { get; set; }
        public virtual DbSet<IsuCitiLwCourses> IsuCitiLwCourses { get; set; }
        public virtual DbSet<IsuCitiLwLearners> IsuCitiLwLearners { get; set; }
        public virtual DbSet<IsuCitiLwTlHistory> IsuCitiLwTlHistory { get; set; }
        public virtual DbSet<IsuImportHistory> IsuImportHistory { get; set; }
        public virtual DbSet<Learners> Learners { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=learnisudbprod.its.iastate.edu,1433;Database=LWEB-IAState;User ID=lweb_citi_user;Password=zPE#LB64J9uYBkiPTPSGKHZ;Trusted_Connection=True;");
            //optionsBuilder.UseSqlServer("Server=learnisudbprod.its.iastate.edu,1433;Database=LWEB-IAState-TEST;User ID=lweb_citi_user;Password=zPE#LB64J9uYBkiPTPSGKHZ;Trusted_Connection=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Courses>(entity =>
            {
                entity.HasKey(e => e.InternalId)
                    .ForSqlServerIsClustered(false);

                entity.ToTable("courses");

                entity.HasIndex(e => e.CourseId)
                    .HasName("IX_courses")
                    .IsUnique()
                    .ForSqlServerIsClustered();

                entity.HasIndex(e => e.User2)
                    .HasName("ISU_User2");

                entity.Property(e => e.AlternateCourseId)
                    .HasColumnName("AlternateCourseID")
                    .HasMaxLength(64);

                entity.Property(e => e.ArchiveMethod).HasMaxLength(16);

                entity.Property(e => e.Category)
                    .HasColumnName("category")
                    .HasMaxLength(64);

                entity.Property(e => e.CategoryName).HasMaxLength(128);

                entity.Property(e => e.CbtlaunchParameters)
                    .HasColumnName("CBTLaunchParameters")
                    .HasMaxLength(512);

                entity.Property(e => e.CbtlaunchPath)
                    .HasColumnName("CBTLaunchPath")
                    .HasMaxLength(255);

                entity.Property(e => e.CbtwinHeight).HasColumnName("CBTWinHeight");

                entity.Property(e => e.CbtwinOptions)
                    .HasColumnName("CBTWinOptions")
                    .HasMaxLength(200);

                entity.Property(e => e.CbtwinWidth).HasColumnName("CBTWinWidth");

                entity.Property(e => e.CertificateBackground).HasMaxLength(128);

                entity.Property(e => e.CertificateLogo).HasMaxLength(128);

                entity.Property(e => e.CertificateSignature).HasMaxLength(128);

                entity.Property(e => e.Ceu).HasColumnName("CEU");

                entity.Property(e => e.Cost).HasColumnName("cost");

                entity.Property(e => e.CostUm)
                    .HasColumnName("cost_um")
                    .HasMaxLength(8);

                entity.Property(e => e.CourseId)
                    .HasColumnName("Course_Id")
                    .HasMaxLength(64);

                entity.Property(e => e.CourseLaunchTypeId)
                    .HasColumnName("CourseLaunchTypeID")
                    .HasMaxLength(12);

                entity.Property(e => e.CourseTypeId)
                    .HasColumnName("CourseTypeID")
                    .HasMaxLength(12);

                entity.Property(e => e.CourseTypeName).HasMaxLength(64);

                entity.Property(e => e.CourseUnitTypeName).HasMaxLength(64);

                entity.Property(e => e.DateAdded)
                    .HasColumnName("date_added")
                    .HasColumnType("smalldatetime");

                entity.Property(e => e.DateEffectiveFrom).HasColumnType("smalldatetime");

                entity.Property(e => e.DateEffectiveTo).HasColumnType("smalldatetime");

                entity.Property(e => e.DebugLevel).HasColumnName("debug_level");

                entity.Property(e => e.Delivery)
                    .HasColumnName("delivery")
                    .HasMaxLength(32);

                entity.Property(e => e.Description).HasColumnType("ntext");

                entity.Property(e => e.DomainName).HasMaxLength(128);

                entity.Property(e => e.Duration).HasColumnName("duration");

                entity.Property(e => e.DurationUm)
                    .HasColumnName("duration_um")
                    .HasMaxLength(8);

                entity.Property(e => e.ECommerceCategoryId)
                    .HasColumnName("eCommerceCategoryID")
                    .HasMaxLength(64);

                entity.Property(e => e.ECommerceSubcategoryId)
                    .HasColumnName("eCommerceSubcategoryID")
                    .HasMaxLength(64);

                entity.Property(e => e.EnrollmentRuleId)
                    .HasColumnName("EnrollmentRuleID")
                    .HasMaxLength(8);

                entity.Property(e => e.ExpireDays).HasColumnName("expire_days");

                entity.Property(e => e.ExpiryBasisId).HasColumnName("ExpiryBasisID");

                entity.Property(e => e.InternalVersion).HasMaxLength(32);

                entity.Property(e => e.LocationName).HasMaxLength(128);

                entity.Property(e => e.Manager1Id)
                    .HasColumnName("Manager1ID")
                    .HasMaxLength(12);

                entity.Property(e => e.Manager2Id)
                    .HasColumnName("Manager2ID")
                    .HasMaxLength(12);

                entity.Property(e => e.Manager3Id)
                    .HasColumnName("Manager3ID")
                    .HasMaxLength(12);

                entity.Property(e => e.OtherInformation).HasColumnType("ntext");

                entity.Property(e => e.ParentCourseId)
                    .HasColumnName("ParentCourse_Id")
                    .HasMaxLength(64);

                entity.Property(e => e.PlainTextDescription).HasColumnType("ntext");

                entity.Property(e => e.PreReqs).HasMaxLength(250);

                entity.Property(e => e.ShortDescription).HasMaxLength(256);

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasMaxLength(1);

                entity.Property(e => e.Subcategory)
                    .HasColumnName("subcategory")
                    .HasMaxLength(64);

                entity.Property(e => e.SubcategoryName).HasMaxLength(128);

                entity.Property(e => e.Title)
                    .HasColumnName("title")
                    .HasMaxLength(256);

                entity.Property(e => e.TrainingAreaName).HasMaxLength(128);

                entity.Property(e => e.User1).HasMaxLength(64);

                entity.Property(e => e.User1Name).HasMaxLength(128);

                entity.Property(e => e.User2).HasMaxLength(64);

                entity.Property(e => e.User2Name).HasMaxLength(128);

                entity.Property(e => e.User3).HasMaxLength(64);

                entity.Property(e => e.User3Name).HasMaxLength(128);

                entity.Property(e => e.User4).HasMaxLength(64);

                entity.Property(e => e.User4Name).HasMaxLength(128);

                entity.Property(e => e.User5).HasMaxLength(64);

                entity.Property(e => e.User5Name).HasMaxLength(128);

                entity.Property(e => e.User6).HasMaxLength(64);

                entity.Property(e => e.User6Name).HasMaxLength(128);

                entity.Property(e => e.UserDate1).HasColumnType("smalldatetime");

                entity.Property(e => e.UserDate2).HasColumnType("smalldatetime");

                entity.Property(e => e.UserDate3).HasColumnType("smalldatetime");

                entity.Property(e => e.UserDate4).HasColumnType("smalldatetime");

                entity.Property(e => e.UserDate5).HasColumnType("smalldatetime");

                entity.Property(e => e.UserDate6).HasColumnType("smalldatetime");

                entity.Property(e => e.VendorContact).HasMaxLength(32);

                entity.Property(e => e.VendorId)
                    .HasColumnName("VendorID")
                    .HasMaxLength(32);
            });

            modelBuilder.Entity<History>(entity =>
            {
                entity.HasKey(e => e.CurriculaId);

                entity.ToTable("history");

                entity.HasIndex(e => new { e.ClassId, e.Status, e.CurriculaId })
                    .HasName("IX_History_1");

                entity.HasIndex(e => new { e.LearnerId, e.CourseId, e.StatusDate, e.Status, e.InternalVersion })
                    .HasName("IX_history");

                entity.Property(e => e.CurriculaId).HasColumnName("Curricula_Id");

                entity.Property(e => e.ActionCode).HasMaxLength(10);

                entity.Property(e => e.ApproverLearnerId)
                    .HasColumnName("ApproverLearnerID")
                    .HasMaxLength(12);

                entity.Property(e => e.Bookmark)
                    .HasColumnName("bookmark")
                    .HasMaxLength(50);

                entity.Property(e => e.Ceu).HasColumnName("CEU");

                entity.Property(e => e.ClassId)
                    .HasColumnName("Class_Id")
                    .HasMaxLength(22);

                entity.Property(e => e.CompetencyCode).HasMaxLength(64);

                entity.Property(e => e.CompletionStatusId)
                    .HasColumnName("CompletionStatusID")
                    .HasMaxLength(16);

                entity.Property(e => e.CoreTotalTime)
                    .HasColumnName("Core_Total_Time")
                    .HasMaxLength(16);

                entity.Property(e => e.CourseId)
                    .HasColumnName("Course_Id")
                    .HasMaxLength(64);

                entity.Property(e => e.CourseTime).HasMaxLength(16);

                entity.Property(e => e.CourseUnitTypeId)
                    .HasColumnName("CourseUnitTypeID")
                    .HasMaxLength(32);

                entity.Property(e => e.Credit)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.DateExpires)
                    .HasColumnName("date_expires")
                    .HasColumnType("smalldatetime");

                entity.Property(e => e.DateTimeStamp).HasMaxLength(12);

                entity.Property(e => e.DepartmentId)
                    .HasColumnName("DepartmentID")
                    .HasMaxLength(64);

                entity.Property(e => e.EnrollmentDate).HasColumnType("smalldatetime");

                entity.Property(e => e.HistoryTypeId)
                    .HasColumnName("HistoryTypeID")
                    .HasMaxLength(16);

                entity.Property(e => e.InternalVersion).HasMaxLength(32);

                entity.Property(e => e.LastAccessDt)
                    .HasColumnName("LastAccessDT")
                    .HasColumnType("smalldatetime");

                entity.Property(e => e.LastAccessIp)
                    .HasColumnName("LastAccessIP")
                    .HasMaxLength(50);

                entity.Property(e => e.LearnerId)
                    .HasColumnName("Learner_Id")
                    .HasMaxLength(12);

                entity.Property(e => e.LibDueDate).HasColumnType("smalldatetime");

                entity.Property(e => e.LibReturnDate).HasColumnType("smalldatetime");

                entity.Property(e => e.LibStatus).HasMaxLength(1);

                entity.Property(e => e.Notes)
                    .HasColumnName("notes")
                    .HasColumnType("ntext");

                entity.Property(e => e.PaymentDate).HasColumnType("smalldatetime");

                entity.Property(e => e.PaymentReference).HasMaxLength(32);

                entity.Property(e => e.RequestDate).HasColumnType("smalldatetime");

                entity.Property(e => e.Status).HasMaxLength(1);

                entity.Property(e => e.StatusDate)
                    .HasColumnName("Status_Date")
                    .HasColumnType("smalldatetime");

                entity.Property(e => e.StatusReason).HasMaxLength(256);

                entity.Property(e => e.Title).HasMaxLength(256);

                entity.Property(e => e.TitleId)
                    .HasColumnName("TitleID")
                    .HasMaxLength(64);

                entity.Property(e => e.TrainerId)
                    .HasColumnName("TrainerID")
                    .HasMaxLength(12);

                entity.Property(e => e.User1).HasMaxLength(64);

                entity.Property(e => e.User2).HasMaxLength(64);

                entity.Property(e => e.User3).HasMaxLength(64);

                entity.Property(e => e.User4).HasMaxLength(64);

                entity.Property(e => e.User5).HasMaxLength(64);

                entity.Property(e => e.User6).HasMaxLength(64);

                entity.Property(e => e.UserDate1).HasColumnType("smalldatetime");

                entity.Property(e => e.UserDate2).HasColumnType("smalldatetime");

                entity.Property(e => e.UserDate3).HasColumnType("smalldatetime");

                entity.Property(e => e.UserDate4).HasColumnType("smalldatetime");

                entity.Property(e => e.UserDate5).HasColumnType("smalldatetime");

                entity.Property(e => e.UserDate6).HasColumnType("smalldatetime");

                entity.Property(e => e.VenueId)
                    .HasColumnName("VenueID")
                    .HasMaxLength(64);
            });

            modelBuilder.Entity<IsuCitiLwCourses>(entity =>
            {
                entity.ToTable("ISU_Citi_LW_Courses");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CitiCourseGroup)
                    .HasColumnName("CITI_CourseGroup")
                    .HasMaxLength(255);

                entity.Property(e => e.CitiCourseId)
                    .IsRequired()
                    .HasColumnName("CITI_CourseId")
                    .HasMaxLength(64);

                entity.Property(e => e.CitiCourseName)
                    .HasColumnName("CITI_CourseName")
                    .HasMaxLength(255);

                entity.Property(e => e.DateCreated).HasColumnType("datetime");

                entity.Property(e => e.DateUpdated).HasColumnType("datetime");

                entity.Property(e => e.DependsOn).HasMaxLength(255);

                entity.Property(e => e.LwCourseId)
                    .HasColumnName("LW_CourseId")
                    .HasMaxLength(64);

                entity.Property(e => e.TlCourseId)
                    .HasColumnName("TL_CourseId")
                    .HasMaxLength(10);

                entity.Property(e => e.TlCourseIdParallel)
                    .HasColumnName("TL_CourseId_Parallel")
                    .HasMaxLength(10);

                entity.Property(e => e.TlCourseType)
                    .HasColumnName("TL_Course_Type")
                    .HasMaxLength(30);

                entity.Property(e => e.TlCourseTypeParallel)
                    .HasColumnName("TL_Course_Type_Parallel")
                    .HasMaxLength(30);
            });

            modelBuilder.Entity<IsuCitiLwLearners>(entity =>
            {
                entity.HasKey(e => e.CitiLearnerId);

                entity.ToTable("ISU_Citi_LW_Learners");

                entity.HasIndex(e => e.LwLearnerId)
                    .HasName("ISU_Index_LW_LearnerID");

                entity.Property(e => e.CitiLearnerId)
                    .HasColumnName("Citi_LearnerId")
                    .HasMaxLength(10)
                    .ValueGeneratedNever();

                entity.Property(e => e.CitiLastName)
                    .HasColumnName("Citi_LastName")
                    .HasMaxLength(50);

                entity.Property(e => e.DateCreated).HasColumnType("datetime");

                entity.Property(e => e.DateUpdated).HasColumnType("datetime");

                entity.Property(e => e.LwLearnerId)
                    .HasColumnName("LW_LearnerId")
                    .HasMaxLength(10);
            });

            modelBuilder.Entity<IsuCitiLwTlHistory>(entity =>
            {
                entity.HasKey(e => e.LwCurriculaId);

                entity.ToTable("ISU_Citi_LW_TL_History");

                entity.Property(e => e.LwCurriculaId)
                    .HasColumnName("LW_Curricula_Id")
                    .ValueGeneratedNever();

                entity.Property(e => e.CitiHistoryId)
                    .HasColumnName("CITI_History_Id")
                    .HasMaxLength(50);

                entity.Property(e => e.DateInserted)
                    .HasColumnName("dateInserted")
                    .HasColumnType("datetime");

                entity.Property(e => e.TlHistoryId)
                    .HasColumnName("TL_History_Id")
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<IsuImportHistory>(entity =>
            {
                entity.ToTable("ISU_Import_History");

                entity.Property(e => e.CitiCourseId)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.CitiId)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.CompletionDate).HasColumnType("smalldatetime");

                entity.Property(e => e.CompletionReportNum).HasMaxLength(50);

                entity.Property(e => e.CourseName).HasMaxLength(255);

                entity.Property(e => e.EmailAddress).HasMaxLength(100);

                entity.Property(e => e.EmployeeNumber).HasMaxLength(50);

                entity.Property(e => e.ExpirationDate).HasColumnType("smalldatetime");

                entity.Property(e => e.FirstName).HasMaxLength(50);

                entity.Property(e => e.GroupName).HasMaxLength(100);

                entity.Property(e => e.InstitutionalEmailAddress).HasMaxLength(100);

                entity.Property(e => e.LastName).HasMaxLength(50);

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.Property(e => e.RegistrationDate).HasColumnType("smalldatetime");

                entity.Property(e => e.StageDescription).HasMaxLength(50);

                entity.Property(e => e.UserName).HasMaxLength(50);

                entity.Property(e => e.CurriculaId).HasColumnName("curricula_id");

            });

            modelBuilder.Entity<Learners>(entity =>
            {
                entity.HasKey(e => e.InternalId)
                    .ForSqlServerIsClustered(false);

                entity.HasIndex(e => e.User4)
                    .HasName("ISU_Index_User4");

                entity.HasIndex(e => e.User6)
                    .HasName("ISU_Index_User6");

                entity.HasIndex(e => new { e.LastName, e.FirstName })
                    .HasName("IX_learners_LastName");

                entity.HasIndex(e => new { e.LearnerId, e.RecordStatusId })
                    .HasName("IX_Learners_2")
                    .IsUnique();

                entity.HasIndex(e => new { e.UserId, e.Pin })
                    .HasName("IX_Learners");

                entity.Property(e => e.AccountExpiryDate).HasColumnType("smalldatetime");

                entity.Property(e => e.AuthorizationCode).HasMaxLength(64);

                entity.Property(e => e.City)
                    .HasColumnName("city")
                    .HasMaxLength(64);

                entity.Property(e => e.CompanyName).HasMaxLength(128);

                entity.Property(e => e.Country).HasMaxLength(64);

                entity.Property(e => e.DepartmentId)
                    .HasColumnName("DepartmentID")
                    .HasMaxLength(64);

                entity.Property(e => e.DepartmentName).HasMaxLength(128);

                entity.Property(e => e.DivisionName).HasMaxLength(128);

                entity.Property(e => e.DomainName).HasMaxLength(128);

                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasMaxLength(128);

                entity.Property(e => e.EmploymentBasisId)
                    .HasColumnName("EmploymentBasisID")
                    .HasMaxLength(16);

                entity.Property(e => e.Ext)
                    .HasColumnName("ext")
                    .HasMaxLength(6);

                entity.Property(e => e.Fax)
                    .HasColumnName("fax")
                    .HasMaxLength(18);

                entity.Property(e => e.FirstName)
                    .HasColumnName("First_Name")
                    .HasMaxLength(32);

                entity.Property(e => e.ForgotPasswordDateTimeStamp).HasMaxLength(24);

                entity.Property(e => e.ForgotPasswordUid)
                    .HasColumnName("ForgotPasswordUID")
                    .HasMaxLength(32);

                entity.Property(e => e.GenderId)
                    .HasColumnName("GenderID")
                    .HasMaxLength(1);

                entity.Property(e => e.HireDate).HasColumnType("smalldatetime");

                entity.Property(e => e.InsertDate).HasColumnType("smalldatetime");

                entity.Property(e => e.InsertTypeId)
                    .HasColumnName("InsertTypeID")
                    .HasMaxLength(1);

                entity.Property(e => e.LanguageId)
                    .HasColumnName("LanguageID")
                    .HasMaxLength(2);

                entity.Property(e => e.LastActionDate).HasColumnType("smalldatetime");

                entity.Property(e => e.LastActionType).HasMaxLength(1);

                entity.Property(e => e.LastName)
                    .HasColumnName("Last_Name")
                    .HasMaxLength(40);

                entity.Property(e => e.LaunchDate).HasColumnType("smalldatetime");

                entity.Property(e => e.LearnerId)
                    .HasColumnName("Learner_Id")
                    .HasMaxLength(12);

                entity.Property(e => e.LearnerStatusId)
                    .HasColumnName("LearnerStatusID")
                    .HasMaxLength(16);

                entity.Property(e => e.LearnerTypeId)
                    .HasColumnName("LearnerTypeID")
                    .HasMaxLength(8);

                entity.Property(e => e.LearnerUnionId)
                    .HasColumnName("LearnerUnionID")
                    .HasMaxLength(16);

                entity.Property(e => e.LocationName).HasMaxLength(128);

                entity.Property(e => e.Manager2Id)
                    .HasColumnName("Manager2ID")
                    .HasMaxLength(12);

                entity.Property(e => e.Manager3Id)
                    .HasColumnName("Manager3ID")
                    .HasMaxLength(12);

                entity.Property(e => e.ManagerId)
                    .HasColumnName("ManagerID")
                    .HasMaxLength(12);

                entity.Property(e => e.MiddleInitial)
                    .HasColumnName("Middle_Initial")
                    .HasMaxLength(32);

                entity.Property(e => e.NameFull).HasMaxLength(128);

                entity.Property(e => e.NameReverse).HasMaxLength(128);

                entity.Property(e => e.NickName).HasMaxLength(32);

                entity.Property(e => e.Notes)
                    .HasColumnName("notes")
                    .HasColumnType("ntext");

                entity.Property(e => e.PasswordExpiryDate).HasColumnType("smalldatetime");

                entity.Property(e => e.Phone)
                    .HasColumnName("phone")
                    .HasMaxLength(18);

                entity.Property(e => e.Pin)
                    .HasColumnName("PIN")
                    .HasMaxLength(64);

                entity.Property(e => e.ProfilePicture).HasMaxLength(128);

                entity.Property(e => e.RecordStatusId)
                    .HasColumnName("RecordStatusID")
                    .HasMaxLength(1);

                entity.Property(e => e.RegionName).HasMaxLength(128);

                entity.Property(e => e.SalutationId)
                    .HasColumnName("SalutationID")
                    .HasMaxLength(8);

                entity.Property(e => e.SecurityAnswer).HasMaxLength(32);

                entity.Property(e => e.SecurityProfileId)
                    .HasColumnName("SecurityProfileID")
                    .HasMaxLength(12);

                entity.Property(e => e.SecurityQuestion).HasMaxLength(16);

                entity.Property(e => e.Signature).HasMaxLength(128);

                entity.Property(e => e.State)
                    .HasColumnName("state")
                    .HasMaxLength(32);

                entity.Property(e => e.Street1)
                    .HasColumnName("street1")
                    .HasMaxLength(64);

                entity.Property(e => e.Street2)
                    .HasColumnName("street2")
                    .HasMaxLength(64);

                entity.Property(e => e.TerminateDate).HasColumnType("smalldatetime");

                entity.Property(e => e.TimeZone).HasMaxLength(8);

                entity.Property(e => e.TitleId)
                    .HasColumnName("TitleID")
                    .HasMaxLength(64);

                entity.Property(e => e.TitleName).HasMaxLength(128);

                entity.Property(e => e.TrainerTypeId)
                    .HasColumnName("TrainerTypeID")
                    .HasMaxLength(1);

                entity.Property(e => e.UpdateDate).HasColumnType("smalldatetime");

                entity.Property(e => e.User1).HasMaxLength(64);

                entity.Property(e => e.User1Name).HasMaxLength(128);

                entity.Property(e => e.User2).HasMaxLength(64);

                entity.Property(e => e.User2Name).HasMaxLength(128);

                entity.Property(e => e.User3).HasMaxLength(64);

                entity.Property(e => e.User3Name).HasMaxLength(128);

                entity.Property(e => e.User4).HasMaxLength(64);

                entity.Property(e => e.User4Name).HasMaxLength(128);

                entity.Property(e => e.User5).HasMaxLength(64);

                entity.Property(e => e.User5Name).HasMaxLength(128);

                entity.Property(e => e.User6).HasMaxLength(64);

                entity.Property(e => e.User6Name).HasMaxLength(128);

                entity.Property(e => e.UserDate1).HasColumnType("smalldatetime");

                entity.Property(e => e.UserDate2).HasColumnType("smalldatetime");

                entity.Property(e => e.UserDate3).HasColumnType("smalldatetime");

                entity.Property(e => e.UserDate4).HasColumnType("smalldatetime");

                entity.Property(e => e.UserDate5).HasColumnType("smalldatetime");

                entity.Property(e => e.UserDate6).HasColumnType("smalldatetime");

                entity.Property(e => e.UserId)
                    .HasColumnName("UserID")
                    .HasMaxLength(128);

                entity.Property(e => e.Zip)
                    .HasColumnName("zip")
                    .HasMaxLength(24);
            });
        }
    }
}
