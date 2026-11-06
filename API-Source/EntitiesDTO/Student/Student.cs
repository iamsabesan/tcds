using Dawn.Utilities.i18N;
using EntitiesDTO.Admin;
using EntitiesDTO.Payment;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Realtor.Utilities;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Utilities.Attributes;

namespace EntitiesDTO.Student
{
    public class GetQualification
    {
        public Guid QualificationID { get; set; }
        public string Qualification { get; set; } = string.Empty;
    }

    public class GetOccupation
    {
        public Guid OccupationID { get; set; }
        public string Occupation { get; set; } = string.Empty;
    }

    public class CustomerLedger
    {
        public Guid CustomerID { get; set; }
        public string CustomerName { get; set; } = string.Empty;
    }

    public class RegisterStudent
    {
        [Required(ErrorMessage = "Student name could not be empty")]
        public string StudentName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Gender could not be empty")]
        public string Gender { get; set; } = string.Empty;
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy:MM:dd}", ApplyFormatInEditMode = true)]
        public string DateOfBirth { get; set; } = string.Empty;
        [Required(ErrorMessage = "Phone code could not be empty")]
        [StringLength(7, ErrorMessage = "{0} value cannot exceed {1} characters")]
        public string PhoneCode { get; set; } = string.Empty;
        [Required(ErrorMessage = "Phone number could not be empty")]
        [StringLength(maximumLength: 15, MinimumLength = 8, ErrorMessage = "Please enter the valid phone number")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Please enter the valid phone number")]
        public string Phone { get; set; } = string.Empty;
        public string? AlternatePhoneCode { get; set; }
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Please enter the valid phone number")]
        public string? AlternatePhone { get; set; }
        //[Required(ErrorMessage = "Email could not be empty")]
        [RegularExpression("^[a-zA-Z0-9+_.-]+@[a-zA-Z0-9.-]+$", ErrorMessage = "Enter valid email address")]
        [StringLength(maximumLength: 255, ErrorMessage = "{0} value cannot exceed {1} characters")]
        public string? Email { get; set; }
        [Required(ErrorMessage = "Qualification could not be empty")]
        public string Qualification { get; set; } = string.Empty;
        [Required(ErrorMessage = "GuardianType could not be empty")]
        public string GuardianType { get; set; } = string.Empty;
        [Required(ErrorMessage = "FatherName could not be empty")]
        public string FatherName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Occupation could not be empty")]
        public string Occupation { get; set; } = string.Empty;
        public List<StudentAddress> Address { get; set; } = new List<StudentAddress>();
        [AllowedExtensions(new string[] { ".png", ".jpg", ".jpeg" })]
        [MaxFileSize(2, ErrorMessage = "The file size should be 2MB")]
        public List<IFormFile>? StudentImage { get; set; } = new List<IFormFile>();
        public string? ReasonToSelect { get; set; } = string.Empty;
        public string? ReferBy { get; set; } = string.Empty;
        public string? Purpose { get; set; } = string.Empty;
    }
    public class CreateStudentEnquiry
    {
        [IsGuidEmpty]
        public Guid TenantBranchID { get; set; }
        [IsGuidEmpty]
        public Guid CourseID { get; set; }
        [Required(ErrorMessage = "Student name could not be empty")]
        public string StudentName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Gender could not be empty")]
        public string Gender { get; set; } = string.Empty;
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy:MM:dd}", ApplyFormatInEditMode = true)]
        public string DateOfBirth { get; set; } = string.Empty;
        [Required(ErrorMessage = "Phone code could not be empty")]
        [StringLength(7, ErrorMessage = "{0} value cannot exceed {1} characters")]
        public string PhoneCode { get; set; } = string.Empty;
        [Required(ErrorMessage = "Phone number could not be empty")]
        [StringLength(maximumLength: 15, MinimumLength = 8, ErrorMessage = "Please enter the valid phone number")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Please enter the valid phone number")]
        public string Phone { get; set; } = string.Empty;
        public string? AlternatePhoneCode { get; set; }
        public string? AlternatePhone { get; set; }
        //[Required(ErrorMessage = "Email could not be empty")]
        [RegularExpression("^[a-zA-Z0-9+_.-]+@[a-zA-Z0-9.-]+$", ErrorMessage = "Enter valid email address")]
        [StringLength(maximumLength: 255, ErrorMessage = "{0} value cannot exceed {1} characters")]
        public string? Email { get; set; } = string.Empty;
        [Required(ErrorMessage = "Qualification could not be empty")]
        public string Qualification { get; set; } = string.Empty;
        [Required(ErrorMessage = "GuardianType could not be empty")]
        public string GuardianType { get; set; } = string.Empty;
        [Required(ErrorMessage = "GuardianName could not be empty")]
        public string GuardianName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Occupation could not be empty")]
        public string Occupation { get; set; } = string.Empty;
        public string ReasonToSelect { get; set; } = string.Empty;
        [IsGuidEmpty]
        public Guid DocumentID { get; set; }
        public List<StudentEnquiryAddress> Address { get; set; } = new List<StudentEnquiryAddress>();
        public Guid? EmailOTPTransID { get; set; }
        public Guid SMSOTPTransID { get; set; }
        public string? ReferBy { get; set; } = string.Empty;
        public string? Purpose { get; set; } = string.Empty;
    }

    public class StudentAddress
    {
        public Guid ContactAddressID { get; set; }
        public Guid ContactID { get; set; }
        [Required(ErrorMessage = "AddressType could not be empty")]
        public string AddressType { get; set; } = string.Empty;
        [Required(ErrorMessage = "Address1 could not be empty")]
        [StringLength(maximumLength: 200, ErrorMessage = "{0} value cannot exceed {1} characters")]
        public string Address1 { get; set; } = string.Empty;
        public string? Address2 { get; set; }
        [Required(ErrorMessage = "Country could not be empty")]
        [StringLength(maximumLength: 50, ErrorMessage = "{0} value cannot exceed {1} characters")]
        public string Country { get; set; } = string.Empty;
        [Required(ErrorMessage = "State could not be empty")]
        [StringLength(maximumLength: 50, ErrorMessage = "{0} value cannot exceed {1} characters")]
        public string State { get; set; } = string.Empty;
        [Required(ErrorMessage = "District could not be empty")]
        [StringLength(maximumLength: 50, ErrorMessage = "{0} value cannot exceed {1} characters")]
        public string District { get; set; } = string.Empty;
        [Required(ErrorMessage = "Area could not be empty")]
        [StringLength(maximumLength: 100, ErrorMessage = "{0} value cannot exceed {1} characters")]
        public string Area { get; set; } = string.Empty;
        [StringLength(maximumLength: 6, ErrorMessage = "Please enter the valid pincode")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Please enter the valid pincode")]
        public string Pincode { get; set; } = string.Empty;
        public string Taluk { get; set; } = string.Empty;
        public decimal Latitude { get; set; } 
        public decimal Longitude { get; set; }
    }

    public class StudentEnquiryAddress
    {
        [Required(ErrorMessage = "AddressType could not be empty")]
        public string AddressType { get; set; } = string.Empty;
        [Required(ErrorMessage = "Address1 could not be empty")]
        [StringLength(maximumLength: 200, ErrorMessage = "{0} value cannot exceed {1} characters")]
        public string Address1 { get; set; } = string.Empty;
        public string? Address2 { get; set; }
        [Required(ErrorMessage = "Area could not be empty")]
        [StringLength(maximumLength: 100, ErrorMessage = "{0} value cannot exceed {1} characters")]
        public string Area { get; set; } = string.Empty;
        [Required(ErrorMessage = "Country could not be empty")]
        [StringLength(maximumLength: 50, ErrorMessage = "{0} value cannot exceed {1} characters")]
        public string Country { get; set; } = string.Empty;
        [Required(ErrorMessage = "State could not be empty")]
        [StringLength(maximumLength: 50, ErrorMessage = "{0} value cannot exceed {1} characters")]
        public string State { get; set; } = string.Empty;
        [Required(ErrorMessage = "District could not be empty")]
        [StringLength(maximumLength: 50, ErrorMessage = "{0} value cannot exceed {1} characters")]
        public string District { get; set; } = string.Empty;
        [StringLength(maximumLength: 6, ErrorMessage = "Please enter the valid pincode")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Please enter the valid pincode")]
        public string Pincode { get; set; } = string.Empty;
        public string? Taluk { get; set; } = string.Empty;
    }

    public class CreateStudent
    {
        public Guid TenantBranchID { get; set; }
        public Guid ContactID { get; set; }
        public string Qualification { get; set; } = string.Empty;
        public string GuardianType { get; set; } = string.Empty;
        public string GuardianName { get; set; } = string.Empty;
        public string Occupation { get; set; } = string.Empty;
    }

    public class DocumentUpload
    {
        public List<IFormFile> files { get; set; } = new List<IFormFile>();
        public string DocumentName { get; set; } = String.Empty;
        public Guid ContactID { get; set; }
    }

    public class SaveDocument
    {
        [IsGuidEmpty]
        public Guid DocumentID { get; set; }
        [IsGuidEmpty]
        public Guid PrimaryKeyID { get; set; }
        [Required]
        [StringLength(50)]
        public string DocumentType { get; set; } = string.Empty;
        [Required]
        public IFormFile DocumentFile { get; set; }
    }

    public record DocumentRemove
    {
        [IsGuidEmpty]
        public Guid DocumentID { get; set; }
    }

    public class StudentCourseMap
    {
        [IsGuidEmpty]
        public Guid StudentID { get; set; }
        public Guid CourseBatchID { get; set; }
        public decimal Discount { get; set; } = 0;
        public decimal DownPayment { get; set; }
        public Guid CreatedBy { get; set; }
    }

    public class NewStudentCourseMap : StudentCourseMap
    {
        public List<PaymentItemsNew> PaymentItems { get; set; } = new List<PaymentItemsNew>();
        public string StudentType { get; set; } = string.Empty;
        public Guid? StudentEnquiryID { get; set; }
    }

    public class StudentPartPayment
    {
        [IsGuidEmpty]
        public Guid BatchMappingID { get; set; }
        public decimal TransactionAmount { get; set; }
        public List<PaymentItemsNew> PaymentItems { get; set; } = new List<PaymentItemsNew>();
    }

    public class GetBatchMap
    {
        public Guid BatchMappingID { get; set; }
        public decimal CourseFees { get; set; }
        public decimal AdmissionFees { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public string StudentID { get; set; } = string.Empty;
        public string ShortName { get; set; } = string.Empty;
    }

    public class GetAlumniStudent
    {
        public Guid StudentID { get; set; }
        public Guid BatchMappingID { get; set; }
        public Guid ContactID { get; set; }
        public string RegisteredDate { get; set; } = string.Empty;
        public string RollNumber { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public string Course { get; set; } = string.Empty;
        public string StartDate { get; set; } = string.Empty;
        public string EndDate { get; set; } = string.Empty;
        public string StartTime { get; set; } = string.Empty;
        public string EndTime { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string AlternatePhone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string ShortName { get; set; } = string.Empty;
        public string CourseTypeName { get; set; } = string.Empty;
    }

    public class AlumniStudent
    {
        public Guid ContactID { get; set; }
        public Guid BatchMappingID { get; set; }
        public Guid StudentID { get; set; }
        [JsonIgnore]
        public DateI18 RegisteredDateI18 { get; set; } = new DateI18();
        public string RegisteredDate { get { return RegisteredDateI18.ToString() ?? string.Empty; } set { RegisteredDateI18 = value; } }
        public string RollNumber { get; set; } = string.Empty;
        public string Course { get; set; } = string.Empty;
        public string StartDate { get; set; } = string.Empty;
        public string EndDate { get; set; } = string.Empty;
        public string StartTime { get; set; } = string.Empty;
        public string EndTime { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string ShortName { get; set; } = string.Empty;
        public string CourseTypeName { get; set; } = string.Empty;
    }

    public class StudentBasicInfo
    {
        public Guid BatchMappingID { get; set; }
        public Guid StudentID { get; set; }
        public Guid ContactID { get; set; }
        public string RollNumber { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string AlternatePhone { get; set; } = string.Empty;
        public string Course { get; set; } = string.Empty;
        public string BatchNumber { get; set; } = string.Empty;
        public string BatchTime { get; set; } = string.Empty;
        public string StartDate { get; set; } = string.Empty;
        public string EndDate { get; set; } = string.Empty;
        public string ShortName { get; set; } = string.Empty;
        public string CourseTypeName { get; set; } = string.Empty;
    }

    public class StudentInfo
    {
        public Guid BatchMappingID { get; set; }
        public Guid ContactID { get; set; }
        public Guid StudentID { get; set; }
        public string RollNumber { get; set; } = string.Empty;
        public string Course { get; set; } = string.Empty;
        public string BatchNumber { get; set; } = string.Empty;
        public string BatchTime { get; set; } = string.Empty;
        [JsonIgnore]
        public DateI18 StartDateI18 { get; set; } = new DateI18();
        public string StartDate { get { return StartDateI18.ToString() ?? string.Empty; } set { StartDateI18 = value; } }
        [JsonIgnore]
        public DateI18 EndDateI18 { get; set; } = new DateI18();
        public string EndDate { get { return EndDateI18.ToString() ?? string.Empty; } set { EndDateI18 = value; } }
        public string ShortName { get; set; } = string.Empty;
        public string CourseTypeName { get; set; } = string.Empty;
    }

    public class StudentInfos
    {
        public Guid ContactID { get; set; }
        public Guid StudentID { get; set; }
        public string RollNumber { get; set; } = string.Empty;
        public string Course { get; set; } = string.Empty;
        public string BatchNumber { get; set; } = string.Empty;
        public Guid BatchMappingID { get; set; }
        public string ShortName { get; set; } = string.Empty;
        public string CourseTypeName { get; set; } = string.Empty;
    }

    public class StudentReceiptList
    {
        public Guid TenantBranchTransID { get; set; }
        public Guid StudentID { get; set; }
        public string ReceiptDate { get; set; } = string.Empty;
        public string ReceiptNo { get; set; } = string.Empty;
        public string RollNumber { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public string Course { get; set; } = string.Empty;
        public string PaymentType { get; set; } = string.Empty;
        public string Paymode { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string SumOfRupees { get; set; } = string.Empty;
        public string ShortName { get; set; } = string.Empty;
        public string CourseTypeName { get; set; } = string.Empty;
    }

    public class StudentBalanceList
    {
        public Guid StudentID { get; set; }
        public Guid BatchMappingID { get; set; }
        public Guid ContactID { get; set; }
        public string RollNumber { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public string Course { get; set; } = string.Empty;
        public string StartTime { get; set; } = string.Empty;
        public string EndTime { get; set; } = string.Empty;
        public decimal TotalFees { get; set; }
        public decimal PaidFees { get; set; }
        public decimal BalanceFees { get; set; }
        public string Status { get; set; } = string.Empty;
        public string ShortName { get; set; } = string.Empty;
        public string CourseTypeName { get; set; } = string.Empty;
        public string StartDate { get; set; } = string.Empty;
        public string EndDate { get; set; } = string.Empty;
        public string FeesStatus { get; set; } = string.Empty;
    }

    public class StudentsList
    {
        public Guid ContactID { get; set; }
        public Guid StudentID { get; set; }
        public Guid BatchMappingID { get; set; }
        public string RollNumber { get; set; } = string.Empty;
        public string Course { get; set; } = string.Empty;
        public string StartTime { get; set; } = string.Empty;
        public string EndTime { get; set; } = string.Empty;
        public decimal TotalFees { get; set; }
        public string Status { get; set; } = string.Empty;
        public string ShortName { get; set; } = string.Empty;
        public string CourseTypeName { get; set; } = string.Empty;
        [JsonIgnore]
        public DateI18 StartDateI18 { get; set; } = new DateI18();
        public string StartDate { get { return StartDateI18.ToString() ?? string.Empty; } set { StartDateI18 = value; } }
        [JsonIgnore]
        public DateI18 EndDateI18 { get; set; } = new DateI18();
        public string EndDate { get { return EndDateI18.ToString() ?? string.Empty; } set { EndDateI18 = value; } }
    }

    public class StudentProfile
    {
        public Guid ContactID { get; set; }
        public string RollNumber { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public string DateOfBirth { get; set; } = string.Empty;
        public string PhoneCode { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string AlternatePhoneCode { get; set; } = string.Empty;
        public string AlternatePhone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Qualification { get; set; } = string.Empty;
        public string GuardianType { get; set; } = string.Empty;
        public string FatherName { get; set; } = string.Empty;
        public string Occupation { get; set; } = string.Empty;
        public string ReasonToSelect { get; set; } = string.Empty;
        public string ReferBy { get; set; } = string.Empty;
        public string Purpose { get; set; } = string.Empty;
        public List<Address> Address { get; set; } = new List<Address>();
        public List<DocumentURLDetails> StudentImages { get; set; } = new List<DocumentURLDetails>();
        public List<CourseInfos> Courses { get; set; } = new List<CourseInfos>();
    }

    public class StudentTotalSem
    {
        public Guid ExamIndentTransID { get; set; }
        public string Semester { get; set; } = string.Empty;
    }

    public record DocumentURLDetails
    {
        public Guid ContactID { get; set; }
        public string ProfileBase64 { get; set; } = string.Empty;
        public string MIMEType { get; set; } = string.Empty;
    }

    public record PrimaryKeyIDs
    {
        public Guid ContatcID { get; set; }
    }

    public class StudentCourse
    {
        public CourseDetail Course { get; set; } = new CourseDetail();
        public BatchDetail Batch { get; set; } = new BatchDetail();
        public List<Materials> Materials { get; set; } = new List<Materials>();
        public List<StudentPaymentDetails> Payments { get; set; } = new List<StudentPaymentDetails>();
        public List<CertificateMarksheet> Marksheet { get; set; } = new List<CertificateMarksheet>();
        public CertificateMarksheet Certificate { get; set; }
    }

    public class Address
    {
        public Guid ContactID { get; set; }
        public Guid AddressTypeID { get; set; }
        public string AddressType { get; set; } = string.Empty;
        public string Address1 { get; set; } = string.Empty;
        public string Address2 { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string Pincode { get; set; } = string.Empty;
        public string Taluk { get; set; } = string.Empty;
    }

    public class CourseDetail
    {
        public Guid CourseBatchID { get; set; }
        public Guid CourseID { get; set; }
        public Guid BatchMappingID { get; set; }
        public Guid ContactID { get; set; }
        public string Course { get; set; } = string.Empty;
        public string CourseType { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public decimal CourseFees { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalFees { get; set; }
        public decimal PaidFees { get; set; }
        public decimal Balance { get; set; }
        public bool IsActive { get; set; }
        public bool IsCompleted { get; set; }
        public string Reason { get; set; } = string.Empty;
        public int TotalSemesters { get; set; }
        public string Status { get; set; } = string.Empty;
        [JsonIgnore]
        public DateI18 StartDateI18 { get; set; } = new DateI18();
        public string StartDate { get { return StartDateI18.ToString() ?? string.Empty; } set { StartDateI18 = value; } }
        [JsonIgnore]
        public DateI18 EndDateI18 { get; set; } = new DateI18();
        public string EndDate { get { return EndDateI18.ToString() ?? string.Empty; } set { EndDateI18 = value; } }
        public string RollNumber { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public string ShortName { get; set; } = string.Empty;
        public string Eligibility { get; set; } = string.Empty;
        public decimal DiscountAmount { get; set; }
    }

    public class BatchDetail
    {
        public Guid CourseBatchID { get; set; }
        public string BatchNumber { get; set; } = string.Empty;
        public string BatchType { get; set; } = string.Empty;
        public string BatchTrack { get; set; } = string.Empty;
        public Guid FacultyID { get; set; }
        public string FacultyName { get; set; } = string.Empty;
        public string StartDate { get; set; } = string.Empty;
        public string EndDate { get; set; } = string.Empty;
        public string StartTime { get; set; } = string.Empty;
        public string EndTime { get; set; } = string.Empty;
    }

    public class Materials
    {
        public Guid MaterialID { get; set; }
        public string MaterialName { get; set; } = string.Empty;
        public int Semester { get; set; }
        public string IssuedDate { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int TheoryMarks { get; set; }
        public int PracticalMarks { get; set; }
        public int TotalMarks { get; set; }
    }

    public class NewIssueMaterial
    {
        public Guid MaterialID { get; set; }
        public int Semester { get; set; }
        [JsonIgnore]
        public DateI18 IssuedDateI18 { get; set; } = new DateI18();
        public string IssuedDate { get { return IssuedDateI18.ToString() ?? string.Empty; } set { IssuedDateI18 = value; } }
        public int TheoryMarks { get; set; }
        public int PracticalMarks { get; set; }
        public int TotalMarks { get; set; }
    }

    public class CertificateMarksheet
    {
        public Guid ExamIndentTransID { get; set; }
        public Guid CertificateAndMarksheetID { get; set; }
        public string PrintType { get; set; } = string.Empty;
        public string Course { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public string IssuedDate { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string ShortName { get; set; } = string.Empty;
        public int Semester { get; set; }
    }

    public class StudentDetail
    {
        public Guid StudentID { get; set; }
        public Guid ContactID { get; set; }
        public string RollNumber { get; set; } = string.Empty;
        public string Qualification { get; set; } = string.Empty;
        public string GuardianType { get; set; } = string.Empty;
        public string FatherName { get; set; } = string.Empty;
        public string Occupation { get; set; } = string.Empty;
        public string ReasonToSelect { get; set; } = string.Empty;
        public string ReferBy { get; set; } = string.Empty;
        public string Purpose { get; set; } = string.Empty;
    }

    public class CourseInfos
    {
        public string FullName { get; set; } = string.Empty;
        public string ShortName { get; set; } = string.Empty;
        public Guid StudentID { get; set; }
        public Guid BatchMappingID { get; set; }

        //added on 06-03-2025
        public StudentCourse studentCourse { get; set; } = new StudentCourse();
        //added on 06-03-2025
    }

    public class IssueMaterial
    {
        public Guid BatchMappingID { get; set; }
        public Guid MaterialID { get; set; }
        public Guid StudentID { get; set; }
        public Guid ContactID { get; set; }
    }

    public class MaterialIssuedList
    {
        public Guid StudentID { get; set; }
        public Guid ContactID { get; set; }
        public Guid CourseID { get; set; }
        public string RollNumber { get; set; } = string.Empty;
        public string Course { get; set; } = string.Empty;
        public Guid MaterialID { get; set; }
        [JsonIgnore]
        public DateI18 IssuedDateI18 { get; set; } = new DateI18();
        public string IssuedDate { get { return IssuedDateI18; } set { IssuedDateI18 = value; } }
        public string ShortName { get; set; } = string.Empty;
        public string CourseTypeName { get; set; } = string.Empty;
        public Guid CourseTypeID { get; set; }
    }

    public class NewMaterialIssuedList
    {
        public Guid StudentID { get; set; }
        public string RollNumber { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public string Course { get; set; } = string.Empty;
        public string MaterialName { get; set; } = string.Empty;
        public string IssuedDate { get; set; } = string.Empty;
        public string ShortName { get; set; } = string.Empty;
        public string CourseTypeName { get; set; } = string.Empty;
        public Guid CourseTypeID { get; set; } 
    }

    public class MaterialPendingList
    {
        public Guid StudentID { get; set; }
        public Guid ContactID { get; set; }
        public Guid BatchMappingID { get; set; }
        public string RollNumber { get; set; } = string.Empty;
        public string Course { get; set; } = string.Empty;
        //public Guid MaterialID { get; set; }
        public string BatchTime { get; set; } = string.Empty;
        public string ShortName { get; set; } = string.Empty;
        public string CourseTypeName { get; set; } = string.Empty;
    }

    public class GetStudentMark
    {
        public Guid StudentID { get; set; }
        public Guid ExamIndentTransID { get; set; }
        public Guid ContactID { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public Guid BatchMappingID { get; set; }
        public Guid SubjectMappingID { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public string RollNumber { get; set; } = string.Empty;
        public Guid MaterialID { get; set; }
        public int Semester { get; set; }
        public int TheoryMarks { get; set; }
        public int PracticalMarks { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class GetSubjects
    {
        public Guid SubjectID { get; set; }
        public Guid MaterialID { get; set; }
        public Guid StudentID { get; set; }
    }

    public class GetStudentMarkInfo
    {
        public Guid StudentID { get; set; }
        public Guid SubjectMappingID { get; set; }
        public Guid ContactID { get; set; }
        public string RollNumber { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public int TheoryMarks { get; set; }
        public int PracticalMarks { get; set; }
        public int Semester { get; set; }
    }

    public class UpdateStudentMark
    {
        public Guid ExamIndentTransID { get; set; }
        public Guid SubjectMappingID { get; set; }
        public Guid BatchMappingID { get; set; }
        public int TheoryMarks { get; set; }
        public int PracticalMarks { get; set; }
        public int ObtainedTheoryMarks { get; set; }
        public int ObtainedPracticalMarks { get; set; }
        public int Semester { get; set; }
    }

    public class UpdateNewStudentMark : UpdateStudentMark
    {
        public string Result { get; set; } = string.Empty;
    }

    public class StudentTotalMarks
    {
        public Guid ExamIndentTransID { get; set; }
        public int TotalMark { get; set; }
        public int ObtainedMark { get; set; }
    }

    public class CertificateIndent
    {
        public Guid ExamIndentTransID { get; set; }
        public string PrintType { get; set; } = string.Empty;
    }

    public class StudentCourseInfo
    {
        public Guid StudentID { get; set; }
        public Guid ContactID { get; set; }
        public Guid BatchMappingID { get; set; }
        public string Course { get; set; } = string.Empty;
        public string RollNumber { get; set; } = string.Empty;
        public string BatchNumber { get; set; } = string.Empty;
        public string BatchTime { get; set; } = string.Empty;
        public Guid FacultyID { get; set; }
        public string ShortName { get; set; } = string.Empty;
        public string CourseTypeName { get; set; } = string.Empty;
        public Guid CourseTypeID { get; set; }
    }

    public class EditMark
    {
        public Guid ExamIndentTransID { get; set; }
        public Guid SubjectMappingID { get; set; }
        public int ObtainedTheoryMarks { get; set; }
        public int ObtainedPracticalMarks { get; set; }
    }

    public class GetTotalMark
    {
        public Guid ExamIndentTransID { get; set; }
        public int TotalMarks { get; set; }
    }

    public class CertificateDespatchInfo
    {
        public Guid ExamIndentTransID { get; set; }
        public Guid TenantBranchID { get; set; }
        public string Type { get; set; } = string.Empty;
        public string IndentNumber { get; set; } = string.Empty;
        [JsonIgnore]
        public DateI18 IndentDateI18 { get; set; } = new DateI18();
        public string IndentDate { get { return IndentDateI18; } set { IndentDateI18 = value; } }
    }

    public class CertificateDespatchList
    {
        public Guid TenantBranchID { get; set; }
        public Guid ExamIndentTransID { get; set; }
        public string IndentDate { get; set; } = string.Empty;
        public string IndentNumber { get; set; } = string.Empty;
        public string FranchiseeID { get; set; } = string.Empty;
        public string FranchiseeName { get; set; } = string.Empty;
        public string Address1 { get; set; } = string.Empty;
        public string? Address2 { get; set; }
        public string Country { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public string Pincode { get; set; } = string.Empty;
        public string StudentID { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public string Course { get; set; } = string.Empty;
        public string Semester { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Taluk { get; set; } = string.Empty;
    }

    public class DespatchCertificate
    {
        public Guid ExamIndentTransID { get; set; }
        public Guid DeliveryModeID { get; set; }
        public string TrackingID { get; set; } = string.Empty;
    }

    public class ReceiveCertificate
    {
        public Guid ExamIndentTransID { get; set; }
        public string PrintType { get; set; } = string.Empty;
    }

    public class ExamTransID
    {
        public Guid ExamIndentTransID { get; set; }
        public string ExamDate { get; set; } = string.Empty;
        public Guid StudentBatchMapping { get; set; }
        public int Semester { get; set; }
    }

    public class CertificatePrintList
    {
        public Guid ExamIndentTransID { get; set; }
        public Guid CertificateAndMarksheetID { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }

    public class NewCertificatePrintList
    {
        public Guid CertificateIndentID { get; set; }
        public string IndentDate { get; set; } = string.Empty;
        public string IndentNumber { get; set; } = string.Empty;
        public Guid TenantBranchID { get; set; }
        public string FranchiseeID { get; set; } = string.Empty;
        public string FranchiseeName { get; set; } = string.Empty;
        public string Address1 { get; set; } = string.Empty;
        public string? Address2 { get; set; }
        public string Country { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public string Pincode { get; set; } = string.Empty;
        public int Total { get; set; }
        public int Printed { get; set; }
        public int Pending { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Taluk { get; set; } = string.Empty;
        public List<StudentCertificateInfo> StudentInfo { get; set; } = new List<StudentCertificateInfo>();
    }

    public class StudentCertificateInfo
    {
        public Guid StudentID { get; set; }
        public Guid ExamIndentTransID { get; set; }
        public Guid CertificateAndMarksheetID { get; set; }
        public string RollNumber { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public string Course { get; set; } = string.Empty;
        public string Semester { get; set; } = string.Empty;
        public string ExamDate { get; set; } = string.Empty;
        public string Batch { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int ToatlMarks { get; set; }
        public string Grade { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string CourseTypeName { get; set; } = string.Empty;
        public string ShortName { get; set; } = string.Empty;
    }


    public class CertificateIndentinfo
    {
        public Guid CertificateIndentID { get; set; }
        public Guid TenantBranchID { get; set; }
        [JsonIgnore]
        public DateI18 IndentDateI18 { get; set; } = new DateI18();
        public string IndentDate { get { return IndentDateI18; } set { IndentDateI18 = value; } }
        public string IndentNumber { get; set; } = string.Empty;
        public int TotalItem { get; set; }
    }

    public class ExamIndentTransInfo
    {
        public Guid ExamIndentTransID { get; set; }
        public Guid StudentBatchMapping { get; set; }
        public string Semester { get; set; } = string.Empty;
        [JsonIgnore]
        public DateI18 ExamDateI18 { get; set; } = string.Empty;
        public string ExamDate { get { return ExamDateI18; } set { ExamDateI18 = value; } }
        public int TotalMark { get; set; }
        public string Grade { get; set; } = string.Empty;
    }

    public class NewMarkSheetPrintList
    {
        public Guid CertificateIndentID { get; set; }
        [JsonIgnore]
        public DateI18 IndentDateI18 { get; set; } = new DateI18();
        public string IndentDate { get { return IndentDateI18; } set { IndentDateI18 = value; } }
        public string IndentNumber { get; set; } = string.Empty;
        public Guid TenantBranchID { get; set; }
        public string FranchiseeID { get; set; } = string.Empty;
        public string FranchiseeName { get; set; } = string.Empty;
        public string Address1 { get; set; } = string.Empty;
        public string? Address2 { get; set; }
        public string Country { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public string Pincode { get; set; } = string.Empty;
        public int Total { get; set; }
        public int Printed { get; set; }
        public int Pending { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Taluk { get; set; } = string.Empty;
        public List<StudentMarkSheetInfo> StudentInfo { get; set; } = new List<StudentMarkSheetInfo>();
    }

    public class StudentMarkSheetInfo
    {
        public Guid StudentID { get; set; }
        public Guid ExamIndentTransID { get; set; }
        public Guid CertificateAndMarksheetID { get; set; }
        public string RollNumber { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public string Course { get; set; } = string.Empty;
        public string Semester { get; set; } = string.Empty;
        public string ExamDate { get; set; } = string.Empty;
        public string Batch { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int TheoryMarks { get; set; }
        public int PracticalMarks { get; set; }
        public string Grade { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string ShortName { get; set; } = string.Empty;
    }

    public class StudentObtainedMarks
    {
        public Guid ExamIndentTransID { get; set; }
        public int TheoryMarks { get; set; }
        public int PracticalMarks { get; set; }
    }

    public class StudentMarksheetAndCertificate
    {
        public Guid ExamIndentTransID { get; set; }
        public Guid CertificateAndMarksheetID { get; set; }
        [JsonIgnore]
        public DateI18 IssuedDateI18 { get; set; } = new DateI18();
        public string IssuedDate { get { return IssuedDateI18.ToString() ?? string.Empty; } set { IssuedDateI18 = value; } }
        public string PrintType { get; set; } = string.Empty;
    }

    public class MaterialRequiredList
    {
        public Guid MaterialID { get; set; }
        public int Total { get; set; }
    }

    public class CertificatePrint
    {
        public Guid BatchMappingID { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string RollNumber { get; set; } = string.Empty;
        public string ShortName { get; set; } = string.Empty;
        public string Course { get; set; } = string.Empty;
        public string StartDate { get; set; } = string.Empty;
        public string EndDate { get; set; } = string.Empty;
        public string BranchName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Grade { get; set; } = string.Empty;
        public string DateOfIssue { get; set; } = string.Empty;
        public string ProfileBase64 { get; set; } = string.Empty;
        public string MIMEType { get; set; } = string.Empty;
        public string CourseTypeName { get; set; } = string.Empty;
        public List<SubjectList> Subjects   { get; set; } = new List<SubjectList>();
    }
    public class SubjectList
    {
        public string SubjectName { get; set; } = string.Empty;
    }

    public class StudentGrade
    {
        public Guid BatchMappingID { get; set; }
        public string GradeName { get; set; } = string.Empty;
        public int Semester { get; set; }
        [JsonIgnore]
        public DateI18 DateOfIssueI18 { get; set; } = new DateI18();
        public string DateOfIssue { get { return DateOfIssueI18.ToString() ?? string.Empty; } set { DateOfIssueI18 = value; } }

    }

    public class NewCertificatePrint
    {
        public Guid ContactID { get; set; }
        public Guid TenantBranchID { get; set; }
        public string RollNumber { get; set; } = string.Empty;
        public string ShortName { get; set; } = string.Empty;
        public string Course { get; set; } = string.Empty;
        [JsonIgnore]
        public DateI18 StartDateI18 { get; set; } = new DateI18();
        public string StartDate { get { return StartDateI18.ToString() ?? string.Empty; } set { StartDateI18 = value; } }
        public string StartDateMonthYear { get { if (DateTime.TryParseExact(StartDateI18, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date)) { return date.ToString("MMMM yyyy", CultureInfo.InvariantCulture); } return string.Empty; } }
        [JsonIgnore]
        public DateI18 EndDateI18 { get; set; } = new DateI18();
        public string EndDate { get { return EndDateI18.ToString() ?? string.Empty; } set { EndDateI18 = value; } }
        public string EndDateMonthYear { get { if (DateTime.TryParseExact(EndDateI18, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date)) { return date.ToString("MMMM yyyy", CultureInfo.InvariantCulture); } return string.Empty; } }
        public int TotalSemesters { get; set; }
        public string CourseTypeName { get; set; } = string.Empty;
    }

    public class MarksheetPrint
    {
        public Guid BatchMappingID { get; set; }
        public string BranchName { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string RollNumber { get; set; } = string.Empty;
        public string ShortName { get; set; } = string.Empty;
        public string Course { get; set; } = string.Empty;
        public int Semester { get; set; }
        public string Date { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Taluk { get; set; } = string.Empty;
        public int TotalTheoryMark { get; set; }
        public int TotalPracticalMark { get; set; }
        public string Result { get; set; } = string.Empty;
        public string DateOfIssue { get; set; } = string.Empty;
        public string ProfileBase64 { get; set; } = string.Empty;
        public string SemestersResult { get; set; } = string.Empty;
        public string MIMEType { get; set; } = string.Empty;        public List<PrintSubject> Subjects { get; set; } = new List<PrintSubject>();
    }

    public class PrintSubject
    {
        public int SlNo { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public int TheoryMark { get; set; }
        public int PracticalMark { get; set; }
        public string Result { get; set; } = string.Empty;
    }

    public class SubjectMarks
    {
        public int TotalTheoryMarks { get; set; }
        public int TotalPracticalMarks { get; set; }
        public int TotalMark { get; set; }
        public int ObtainedTotalMarks { get; set; }
    }

    public class StudentByBatch
    {
        public Guid BatchMappingID { get; set; }
        public Guid ContactID { get; set; }
        public Guid StudentID { get; set; }
        public string RollNumber { get; set; } = string.Empty;
    }

    public class NewStudentByBatch
    {
        public Guid BatchMappingID { get; set; }
        public Guid StudentID { get; set; }
        public string ContactName { get; set; } = string.Empty;
        public string RollNumber { get; set; } = string.Empty;
        public int Semester { get; set; }
    }

    public class IndentSummaryByAdmin
    {
        public int IndentMaterial { get; set; }
        public int DespatchMaterial { get; set; }
        public int IndentExam { get; set; }
    }

    public class IndentSummaryByFranchisee
    {
        public int MaterialPending { get; set; }
        public int ExamPending { get; set; }
    }

    public class GetExamDate
    {
        public Guid BatchMappingID { get; set; }
        [JsonIgnore]
        public DateI18 ExamDateI18 { get; set; } = new DateI18();
        public string ExamDate { get { return ExamDateI18; } set { ExamDateI18 = value; } }
    }

    public class PrintHallTicket
    {
        public string RollNumber { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public string Course { get; set; } = string.Empty;
        public string BatchID { get; set; } = string.Empty;
        public string ExamDate { get; set; } = string.Empty;
        public string ProfileBase64 { get; set; } = string.Empty;
        public string MIMEType { get; set; } = string.Empty;
        public string ShortName { get; set; } = string.Empty;
        public string CourseTypeName { get; set; } = string.Empty;
    }

    public class StudentHallTicket
    {
        public string RollNumber { get; set; } = string.Empty;
        public Guid ContactID { get; set; }
        public string Course { get; set; } = string.Empty;
        public string BatchNumber { get; set; } = string.Empty;
        public string ShortName { get; set; } = string.Empty;
        public string CourseTypeName { get; set; } = string.Empty;
    }

    public class GetCertificateID
    {
        public Guid CertificateAndMarksheetID { get; set; }
    }

    public class GetStudentEnquiry
    {
        public Guid StudentEnquiryID { get; set; }
        public Guid TenantBranchID { get; set; }
        public Guid CourseID { get; set; }
        public string Course { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        [JsonIgnore]
        public DateI18 DateOfBirthI18 { get; set; } = new DateI18();
        public string DateOfBirth { get { return DateOfBirthI18.ToString() ?? string.Empty; } set { DateOfBirthI18 = value; } }
        public string PhoneCode { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? AlternatePhoneCode { get; set; }
        public string? AlternatePhone { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Qualification { get; set; } = string.Empty;
        public string GuardianType { get; set; } = string.Empty;
        public string GuardianName { get; set; } = string.Empty;
        public string Occupation { get; set; } = string.Empty;
        public string ReasonToSelect { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public Guid DocumentID { get; set; }
        [JsonIgnore]
        public DateI18 RequestedDateI18 { get; set; } = new DateI18();
        public string RequestedDate { get { return RequestedDateI18.ToString() ?? string.Empty; } set { RequestedDateI18 = value; } }
        public List<GetEnquiryAddress> Address { get; set; } = new List<GetEnquiryAddress>();
        public string ReferBy { get; set; } = string.Empty;
        public string Purpose { get; set; } = string.Empty;
        public string ShortName { get; set; } = string.Empty;
        public string CourseTypeName { get; set; } = string.Empty;
    }

    public class GetEnquiryAddress
    {
        public Guid StudentAddressID { get; set; }
        public Guid StudentEnquiryID { get; set; }
        public string AddressType { get; set; } = string.Empty;
        public string Address1 { get; set; } = string.Empty;
        public string? Address2 { get; set; }
        public string Area { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string Pincode { get; set; } = string.Empty;
        public string Taluk { get; set; } = string.Empty;
    }

    public class StudentFilter
    {
        //public Guid? StudentID { get; set; }
        //public DateTime? FromDate { get; set; }
        //public DateTime? ToDate { get; set; }
        public Guid? CourseID { get; set; }
        public Guid? CourseBatchID { get; set; }
        //public string? CourseType { get; set; }
        //public string? Gender { get; set; }
        public string? Status { get; set; }
    }

    public class StudentMblList
    {
        public Guid StudentID { get; set; }
        public Guid ContactID { get; set; }
        public string ContactName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string RollNumber { get; set; } = string.Empty;
        public string Course { get; set; } = string.Empty;
        [JsonIgnore]
        public DateI18 DateOfAdmissionI18 { get; set; } = new DateI18();
        public string DateOfAdmission { get { return DateOfAdmissionI18.ToString() ?? string.Empty; } set { DateOfAdmissionI18 = value; } }
        [JsonIgnore]
        public DateI18 FromDateI18 { get; set; } = new DateI18();
        public string FromDate { get { return FromDateI18.ToString() ?? string.Empty; } set { FromDateI18 = value; } }
        [JsonIgnore]
        public DateI18 ToDateI18 { get; set; } = new DateI18();
        public string ToDate { get { return ToDateI18.ToString() ?? string.Empty; } set { ToDateI18 = value; } }
    }

    public class StudentContacts
    {
        public Guid ContactID { get; set; }
        public string ContactName { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string PhoneCode { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
    }

    public class AdmissionStatistics
    {
        public int Total { get; set; }
        public int Studying { get; set; }
        public int Completed { get; set; }
        public int Discontinued { get; set; }
        public List<MonthlyStatistics> MonthlyWise { get; set; } = new List<MonthlyStatistics>();
    }

    public class MonthlyStatistics
    {
        public int MonthID { get; set; }
        public string MonthName { get; set; }
        public int Total { get; set; }
        public int Studying { get; set; }
        public int Completed { get; set; }
        public int Discontinued { get; set; }
    }

    public class AdmissionStatisticsByDate
    {
        public int Total { get; set; }
        public int Studying { get; set; }
        public int Completed { get; set; }
        public int Discontinued { get; set; }
    }

    public class MaterialFilter
    {
        public Guid? StudentID { get; set; }
        public Guid? CourseID { get; set; }
        public Guid? CourseBatchID { get; set; }
        public string Type { get; set; }
    }

    public class CertificateIndentTrack
    {
        public Guid CertificateIndentID { get; set; }
        public Guid TenantBranchID { get; set; }
        public string IndentNumber { get; set; } = string.Empty;
        [JsonIgnore]
        public DateI18 IndentDateI18 { get; set; } = new DateI18();
        public string IndentDate { get { return IndentDateI18.ToString() ?? string.Empty; } set { IndentDateI18 = value; } }
        public int TotalStudents { get; set; }
        public int OrderedMarksheet { get; set; }
        public int DespatchedMarksheet { get; set; }
        public int ReceivedMarksheet { get; set; }
        public int PendingMarksheet { get; set; }
        public int OrderedCertificate { get; set; }
        public int DespatchedCertificate { get; set; }
        public int ReceivedCertificate { get; set; }
        public int PendingCertificate { get; set; }
        public string Status { get; set; } = string.Empty;
        public List<CertificateIndentTrans> IndentTrans { get; set; } = new List<CertificateIndentTrans>();
    }

    public class CertificateIndentTrans
    {
        public Guid CertificateAndMarksheetID { get; set; }
        public Guid ExamIndentTransID { get; set; }
        public Guid CertificateIndentID { get; set; }
        public Guid ContactID { get; set; }
        public string PrintType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string DeliveryModeID { get; set; } = string.Empty;
        public string DeliveryMode { get; set; } = string.Empty;
        public string TrackingID { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public string RollNumber { get; set; } = string.Empty;
        public string Course { get; set; } = string.Empty;
        public string Semester { get; set; } = string.Empty;
        [JsonIgnore]
        public DateI18 DespatchDateI18 { get; set; } = new DateI18();
        public string DespatchDate { get { return DespatchDateI18.ToString() ?? string.Empty; } set { DespatchDateI18 = value; } }
        [JsonIgnore]
        public DateI18 ReceivedDateI18 { get; set; } = new DateI18();
        public string ReceivedDate { get { return ReceivedDateI18.ToString() ?? string.Empty; } set { ReceivedDateI18 = value; } }
        [JsonIgnore]
        public DateI18 IssuedDateI18 { get; set; } = new DateI18();
        public string IssuedDate { get { return IssuedDateI18.ToString() ?? string.Empty; } set { IssuedDateI18 = value; } }
    }

    public class WebStudentFilter
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public Guid? CourseID { get; set; }
        public Guid? CourseBatchID { get; set; }
        public Guid? CourseTypeID { get; set; }
    }

    public class CreateDiscontinued
    {
        [IsGuidEmpty]
        public Guid BatchMappingID { get; set; }
        [Required]
        public string Reason { get; set; }
    }

    public class RejectStudentEnquiry
    {
        [IsGuidEmpty]
        public Guid StudentEnquiryID { get; set; }
        [Required]
        public string Reason { get; set; }
    }

    public class GetEnquiryByStudent
    {
        public Guid StudentEnquiryID { get; set; }
        public Guid TenantBranchID { get; set; }
        public Guid CourseID { get; set; }
        public string Course { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        [JsonIgnore]
        public DateI18 DateOfBirthI18 { get; set; } = new DateI18();
        public string DateOfBirth { get { return DateOfBirthI18.ToString() ?? string.Empty; } set { DateOfBirthI18 = value; } }
        public string PhoneCode { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? AlternatePhoneCode { get; set; }
        public string? AlternatePhone { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Qualification { get; set; } = string.Empty;
        public string GuardianType { get; set; } = string.Empty;
        public string GuardianName { get; set; } = string.Empty;
        public string Occupation { get; set; } = string.Empty;
        public string ReasonToSelect { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public bool IsRejected { get; set; }
        [JsonIgnore]
        public DateI18 RejectedDateI18 { get; set; } = new DateI18();
        public string RejectedDate { get { return RejectedDateI18.ToString() ?? string.Empty; } set { RejectedDateI18 = value; } }
        public string Reason { get; set; } = string.Empty;
        public string CourseTypeName { get; set; } = string.Empty;
        public string DurationType { get; set; } = string.Empty;
        public int Duration { get; set; }
        public int TotalSemesters { get; set; }
        public decimal CourseFees { get; set; }
        public Guid DocumentID { get; set; }
        [JsonIgnore]
        public DateI18 RequestedDateI18 { get; set; } = new DateI18();
        public string RequestedDate { get { return RequestedDateI18.ToString() ?? string.Empty; } set { RequestedDateI18 = value; } }
        public string ShortName { get; set; } = string.Empty;
        public List<GetEnquiryAddress> Address { get; set; } = new List<GetEnquiryAddress>();
        public GetBranchContactDetail BranchDetail { get; set; } = new GetBranchContactDetail();
    }

    public class EditStudent
    {
        [IsGuidEmpty]
        public Guid ContactID { get; set; }
        [IsGuidEmpty]
        public Guid StudentID { get; set; }
        [Required]
        public string ContactName { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        public DateTime DateOfBirth { get; set; }
        [Required]
        public string PhoneCode { get; set; }
        [Required]
        public string Phone { get; set; }
        public string? AlternatePhoneCode { get; set; }
        public string? AlternatePhone { get; set; }
        //[Required]
        public string? Email { get; set; }
        [Required]
        public string Qualification { get; set; }
        [Required]
        public string GuardianType { get; set; }
        [Required]
        public string GuardianName { get; set; }
        [Required]
        public string Occupation { get; set; }
        public Guid DocumentID { get; set; }
        public List<EditAddress> Address { get; set; }
    }

    public class EditAddress
    {
        [IsGuidEmpty]
        public Guid ContactID { get; set; }
        [IsGuidEmpty]
        public Guid AddressTypeID { get; set; }
        [Required]
        public string Address1 { get; set; } = string.Empty;
        public string? Address2 { get; set; }
        [Required]
        public string Area { get; set; } = string.Empty;
        [Required]
        public string District { get; set; } = string.Empty;
        [Required]
        public string State { get; set; } = string.Empty;
        [Required]
        public string Country { get; set; } = string.Empty;
        [Required]
        public string Pincode { get; set; } = string.Empty;
        [Required]
        public string Taluk { get; set; } = string.Empty;
    }

    public class GetMobileNumber
    {
        public string PhoneCode { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string AlternatePhoneCode { get; set; } = string.Empty;
        public string AlternatePhone { get; set; } = string.Empty;
    }

    public class ReasonDropdown
    {
        public Guid ReasonID { get; set; }
        public string Reason { get; set; } = string.Empty;
    }

    public class MblPaymentHistory
    {
        [JsonIgnore]
        public DateI18 VoucherDateI18 { get; set; } = new DateI18();
        public string VoucherDate { get { return VoucherDateI18.ToString() ?? string.Empty; } set { VoucherDateI18 = value; } }
        public string ReceiptNo { get; set; } = string.Empty;
        public string PaymentType { get; set; } = string.Empty;
        public Guid TenantBranchTransID { get; set; }
        public decimal Amount { get; set; }
        public List<MblPaymentTrans> Paymodes { get; set; } = new List<MblPaymentTrans>();
    }

    public class MblPaymentTrans
    {
        public Guid TenantBranchTransID { get; set; }
        public Guid TransID { get; set; }
        public decimal Amount { get; set; }
        public string PayMode { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        [JsonIgnore]
        public DateI18 TransactionDateI18 { get; set; } = new DateI18();
        public string TransactionDate { get { return TransactionDateI18.ToString() ?? string.Empty; } set { TransactionDateI18 = value; } }
        public Guid DocumentID { get; set; }
    }

    public class EnquiryFilter
    {
        public string Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }

    public class StudentEnquiryReport
    {
        public Guid StudentEnquiryID { get; set; }
        public Guid TenantBranchID { get; set; }
        public Guid CourseID { get; set; }
        public string Course { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        [JsonIgnore]
        public DateI18 DateOfBirthI18 { get; set; } = new DateI18();
        public string DateOfBirth { get { return DateOfBirthI18.ToString() ?? string.Empty; } set { DateOfBirthI18 = value; } }
        public string PhoneCode { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? AlternatePhoneCode { get; set; }
        public string? AlternatePhone { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Qualification { get; set; } = string.Empty;
        public string GuardianType { get; set; } = string.Empty;
        public string GuardianName { get; set; } = string.Empty;
        public string Occupation { get; set; } = string.Empty;
        public string ReasonToSelect { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public bool IsRejected { get; set; }
        [JsonIgnore]
        public DateI18 RegisteredDateI18 { get; set; } = new DateI18();
        public string RegisteredDate { get { return RegisteredDateI18.ToString() ?? string.Empty; } set { RegisteredDateI18 = value; } }
        [JsonIgnore]
        public DateI18 RejectedDateI18 { get; set; } = new DateI18();
        public string RejectedDate { get { return RejectedDateI18.ToString() ?? string.Empty; } set { RejectedDateI18 = value; } }
        public string Reason { get; set; } = string.Empty;
        public string CourseTypeName { get; set; } = string.Empty;
        public string DurationType { get; set; } = string.Empty;
        public int Duration { get; set; }
        public int TotalSemesters { get; set; }
        public decimal CourseFees { get; set; }
        public Guid DocumentID { get; set; }
        [JsonIgnore]
        public DateI18 RequestedDateI18 { get; set; } = new DateI18();
        public string RequestedDate { get { return RequestedDateI18.ToString() ?? string.Empty; } set { RequestedDateI18 = value; } }
        public List<GetEnquiryAddress> Address { get; set; } = new List<GetEnquiryAddress>();
        public string ReferBy { get; set; } = string.Empty;
        public string Purpose { get; set; } = string.Empty;
        public string ShortName { get; set; } = string.Empty;
    }

    public class FeesBalanceCount
    {
        public Guid BatchMappingID { get; set; }
        public decimal NetCourseFees { get; set; }
        public decimal PaidFees { get; set; }
    }

    public class GetStudentIDCard
    {
        public Guid ContactID { get; set; }
        public Guid TenantBranchID { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string RollNumber { get; set; } = string.Empty;
        public string StudentPlace { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string FranchiseeID { get; set; } = string.Empty;
        public string FranchiseePlace { get; set; } = string.Empty;
        public string Course { get; set; } = string.Empty;
        public string Batch { get; set; } = string.Empty;
        public string Period { get; set; } = string.Empty;
        public string ProfileBase64 { get; set; } = string.Empty;
        public string MIMEType { get; set; } = string.Empty;
    }

    public class DropdownDetails
    {
        public Guid ID { get; set; }
        public string Value { get; set; } = string.Empty;
    }
    public class StudentContactID
    {
        public Guid StudentID { get; set; }
        public Guid ContactID { get; set; }

    }
    public class GetAuthRoleID
    {
        public int AuthRoleID { get; set; }
    }
    public class UserLoginCreate
    {
        public Guid ParentContactID { get; set; } = Guid.NewGuid();
        public Guid UserID { get; set; } = Guid.NewGuid();
        [StringLength(maximumLength: 255, ErrorMessage = "{0} value cannot exceed {1} characters")]
        [RegularExpression("^[a-zA-Z0-9+_.-]+@[a-zA-Z0-9.-]+$", ErrorMessage = "Enter valid email address")]
        public string? LoginName { get; set; } = string.Empty;
        [StringLength(7, ErrorMessage = "{0} value cannot exceed {1} characters")]
        public string? PhoneCode { get; set; } = string.Empty;
        [RegularExpression("^([0-9]{10})$", ErrorMessage = "Invalid Mobile Number.")]
        public string? Phone { get; set; } = string.Empty;
        [Required]
        //[StringLength(20, MinimumLength = 8, ErrorMessage = "{0} value between {2} and {1}")]
        //[RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[@#$%^&-+=()_]).{8,20}$",
        //ErrorMessage = "Password should contain minimum eight characters, at least one uppercase letter, one lowercase letter, one number and one special character from any of these @#$%^&-+=()_")]
        public string Password { get; set; } = string.Empty;
        [IsGuidEmpty]
        public Guid ContactID { get; set; }
        [IsGuidEmpty]
        public Guid TenantID { get; set; }
        public Guid TenantBranchID { get; set; }
        public int[] AuthRoleID { get; set; } = new int[] { };
        public string ContactCode { get; set; } = string.Empty;
    }
    public class StudentLoginList
    {
        public Guid ContactID { get; set; }
        public string StudentName { get; set; } = string.Empty;
    }
    public class StudentRegisterResult
    {
        public Guid ContactID { get; set; }
        public Guid StudentID { get; set; }
    }
    public class SMSQueue
    {
        public string Token { get; set; } = string.Empty;
        public string DestinationNumber { get; set; } = string.Empty;
        public string TemplateSysID { get; set; } = string.Empty;
        public List<string> MessageParameter { get; set; } = new List<string>();
    }
    public class MessageInfo
    {
        public string ProviderID { get; set; } = string.Empty;
        public string TemplateID { get; set; } = string.Empty;
        public string EntityID { get; set; } = string.Empty;
        public string SMSTemplate { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string DestinationNumber { get; set; } = string.Empty;
        public string MessageParameters { get; set; } = string.Empty;
    }
    public class PasswordSMS
    {
        public string StudentName { get; set; } = string.Empty;
        public string CourseFranchiseeName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;

    }
    public class PaymentSMS
    {
        public string StudentName { get; set; } = string.Empty;
        public string CourseShortName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string PaymentMode { get; set; } = string.Empty;
        public string PaymentType { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;

    }
    public class EnquirySMS
    {
        public string StudentName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public string FranchiseeName { get; set; } = string.Empty;
    }
    public class ExamTransAndType
    {
        public Guid ExamIndentTransID { get; set; }
        public string PrintType { get; set; }

    }
    public class StudentCourseID
    {
        public Guid StudentID { get; set; }
        public Guid ContactID { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string ShortName { get; set; } = string.Empty;
    }
    public class StudentNamePhoneDetail
    {
        public string StudentName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
    }
    public class CertificateMarksheetSMS
    {
        public string StudentName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string CourseShortName { get; set; } = string.Empty;
        public string TemplateSysID { get; set; } = string.Empty;
    }
    public class MaterialSMS
    {
        public string StudentName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string CourseShortName { get; set; } = string.Empty;
    }
    public class CourseSubject
    {
        public string CourseFullName { get; set; } = string.Empty;
        public string CourseShortName { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
    }
    public class GetStudentCourseName
    {
        public Guid ContactID { get; set; }
        public string RollNumber { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
    }
    public class MarkUpdatePreview
    {
            public Guid ExamIndentTransID { get; set; }
            public Guid BatchMappingID { get; set; }
            public string Course { get; set; } = string.Empty;
            public string StudentName { get; set; } = string.Empty;
            public string RollNumber { get; set; } = string.Empty;           
            public int Semester { get; set; }
            public string SemesterResult { get; set; } = string.Empty;

    }
    public class PreviewMarksUpdate
    {
        public List<UpdateStudentMark> Marks { get; set; } = new List<UpdateStudentMark>();
        public bool Preview { get; set; }
    }
    public class PreviewCretificateMarksheet
    {
        public Guid ExamIndentTransID { get; set; }
    }
    public class GetReprintStudent
    {
        public Guid StudentID { get; set; }
        public Guid ContactID { get; set; }
        public string RollNumber { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public List<GetCourseDetails> CourseDetails { get; set; } = new List<GetCourseDetails>();
    }
    public class GetCourseDetails
    {
        public Guid CourseBatchID { get; set; }
        public Guid CourseID { get; set; }
        public Guid BatchMappingID { get; set; }
        public string RollNumber { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public string ShortName { get; set; } = string.Empty;
        public string CourseType { get; set; } = string.Empty;
        public int TotalSemesters { get; set; }
        public bool IsCompleted { get; set; }
        public int ReprintCharges { get; set; }
        public int Semester { get; set; }
    }
    public class StudentCourseDetails
    {
        public Guid ContactID { get; set; }
        public Guid BatchMappingID { get; set; }
        public Guid CourseBatchID { get; set; }
        public Guid StudentID { get; set; }
        public string RollNumber { get; set; } = string.Empty;
        public Guid CourseID { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public string ShortName { get; set; } = string.Empty;
        public string CourseTypeName { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
        public int TotalSemesters { get; set; }
        public int ReprintCharges { get; set; }

    }
    public class StudentMarkReprint
    {
        public Guid StudentMarkID { get; set; }
        public Guid StudentID { get; set; }
        public string RollNumber { get; set; }
        public Guid ContactID { get; set; }
        public Guid ExamIndentTransID { get; set; }
        public Guid SubjectMappingID { get; set; }
        public Guid BatchMappingID { get; set; }
        public int TheoryMark { get; set; }
        public int PracticalMark { get; set; }
        public int ObtainedTheoryMarks { get; set; }
        public int ObtainedPracticalMarks { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
    }

    public class StudentMarkSubjectDetail
    {
        public Guid SubjectMappingID { get; set; }
        public Guid StudentMarkID { get; set; }
        public int TheoryMark { get; set; }
        public int PracticalMark { get; set; }
        public int ObtainedTheoryMarks { get; set; }
        public int ObtainedPracticalMarks { get; set; }
        public string SubjectName { get; set; } = string.Empty;
    }

    public class StudentMarkReprintResult
    {
        public Guid StudentID { get; set; }
        public string RollNumber { get; set; } = string.Empty;
        public Guid ContactID { get; set; }
        public Guid ExamIndentTransID { get; set; }
        public Guid BatchMappingID { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public int Semester { get; set; } 
        public List<StudentMarkSubjectDetail> Subjects { get; set; } = new List<StudentMarkSubjectDetail>();
    }
    public class ReprintCertificateIndent
    {
        public Guid CertificateIndentID { get; set; }
        public string IndentNumber { get; set; } = string.Empty;
    }
    public class FeesBalance
    {
        public Guid BatchMappingID { get; set; }
        public decimal Amount { get; set; }
    }
    public class StudentIDCards
    {
        public Guid BatchMappingID { get; set; }
    }
}
