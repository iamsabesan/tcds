using Newtonsoft.Json;
using Realtor.Utilities;
using System.ComponentModel.DataAnnotations;
using Systemic.Stringdata.Validator;

namespace EntitiesDTO.Users
{
    public class ContactModel
    {
        [StringLength(25, ErrorMessage = "{0} value cannot exceed {1} characters")]
        public string ContactName { get; set; } = string.Empty;
        [Required]
        [StringLength(7, ErrorMessage = "{0} value cannot exceed {1} characters")]
        public string PhoneCode { get; set; } = string.Empty;
        [StringLength(10, MinimumLength = 10, ErrorMessage = "{0} value should be {1}")]
        public string Phone { get; set; } = string.Empty;
        [StringLength(7, ErrorMessage = "{0} value cannot exceed {1} characters")]
        public string? AlternatePhoneCode { get; set; }
        [StringLength(10, MinimumLength = 10, ErrorMessage = "{0} value should be {1}")]
        public string? AlternatePhone { get; set; }
        public string Gender { get; set; } = string.Empty;
        [Required]
        [RegularExpression("^[a-zA-Z0-9+_.-]+@[a-zA-Z0-9.-]+$",
                ErrorMessage = "Enter valid email address")]
        [StringLength(maximumLength: 255, ErrorMessage = "{0} value cannot exceed {1} characters")]
        public string Email { get; set; } = string.Empty;
        [Required]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "{0} value between {2} and {1}")]
        [RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[@#$%^&-+=()]).{8,20}$",
        ErrorMessage = "Password should contain minimum eight characters, at least one uppercase letter, one lowercase letter, one number and one special character from any of these @#$%^&-+=()")]
        public string Password { get; set; } = string.Empty;
        public DateTime? DateOfBirth { get; set; }
        public string? ContactType { get; set; }
        public Guid TenantBranchID { get; set; }
        public Guid TenantID { get; set; }
        public int[] AuthRoleID { get; set; } = new int[] { };
        public Guid ReportToParentID { get; set; } = Guid.Empty;
    }

    public record BaseContactUserWrite
    {
        [StringLength(25, ErrorMessage = "{0} value cannot exceed {1} characters")]
        public string ContactName { get; set; } = String.Empty;
        [Required]
        [RegularExpression("^[a-zA-Z0-9+_.-]+@[a-zA-Z0-9.-]+$", ErrorMessage = "Enter valid email address")]
        [StringLength(maximumLength: 255, ErrorMessage = "{0} value cannot exceed {1} characters")]
        public string Email { get; set; } = String.Empty;
        public string? ContactType { get; set; }
        [IsGuidEmpty]
        public Guid TenantBranchID { get; set; }
        [Required]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "{0} value between {2} and {1}")]
        [RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[@#$%^&-+=()]).{8,20}$",
        ErrorMessage = "Password should contain minimum eight characters, at least one uppercase letter, one lowercase letter, one number and one special character from any of these @#$%^&-+=()")]
        public string Password { get; set; } = string.Empty;
        public int[] AuthRoleID { get; set; } = new int[] { };
        [Required]
        [StringLength(7, ErrorMessage = "{0} value cannot exceed {1} characters")]
        public string PhoneCode { get; set; } = string.Empty;
        [PhoneNumber(true, 10, "Phone")]
        public string Phone { get; set; } = String.Empty;
        public string? AlternatePhoneCode { get; set; } = string.Empty;
        [PhoneNumber(false, 10, "Alternate Phone")]
        public string? AlternatePhone { get; set; }
        public Guid TenantID { get; set; }
        [Required]
        public string Gender { get; set; } = string.Empty;
        [JsonProperty("DepartmentID")]
        public Guid DeptGroupID { get; set; }
        public DateTime? DateOfBirth { get; set; }
    }

    public record TenantContactUserWrite : BaseContactUserWrite
    {
        public Guid? ReportToParentID { get; set; }
    }

    public record ContactCreate
    {
        public Guid ContactID { get; set; }
        [Required]
        [StringLength(25, ErrorMessage = "{0} value cannot exceed {1} characters")]
        public string ContactName { get; set; } = String.Empty;
        [Required]
        [RegularExpression("^[a-zA-Z0-9+_.-]+@[a-zA-Z0-9.-]+$", ErrorMessage = "Enter valid email address")]
        [StringLength(maximumLength: 255, ErrorMessage = "{0} value cannot exceed {1} characters")]
        public string Email { get; set; } = String.Empty;
        public string ContactType { get; set; } = String.Empty;
        public Guid TenantBranchID { get; set; }
        public Guid TenantID { get; set; }
        public Guid? ReportToParentID { get; set; }
        [Required]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "{0} value should be {1}")]
        public string Phone { get; set; } = String.Empty;
        [StringLength(10, MinimumLength = 10, ErrorMessage = "{0} value should be {1}")]
        public string? AlternatePhone { get; set; }
        [Required]
        [StringLength(7, ErrorMessage = "{0} value cannot exceed {1} characters")]
        public string PhoneCode { get; set; } = string.Empty;
        public string? AlternatePhoneCode { get; set; }
        [Required]
        public string Gender { get; set; } = string.Empty;
        [JsonProperty("DepartmentID")]
        public Guid DeptGroupID { get; set; } = Guid.Empty;
        public DateTime? DateOfBirth { get; set; }
    }

    public class BranchCreate
    {
        public string TenantBranchName { get; set; } = string.Empty;
        // Branch Address Info
        public string Address1 { get; set; } = string.Empty;
        public string? Address2 { get; set; }
        public string Area { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public int Pincode { get; set; }

        // Culture Info
        public int CountryID { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string CurrencyCode { get; set; } = string.Empty;
        public string CurrencySymbol { get; set; } = string.Empty;
        public string DateTimeFormat { get; set; } = string.Empty;
        public string DateFormat { get; set; } = string.Empty;
        public string TimeFormat { get; set; } = string.Empty;
        public string LanguageCulture { get; set; } = string.Empty;
        public int TimeZoneID { get; set; }

        // Accounting info
        //public string JournalDate { get; set; } = DateTime.UtcNow.ToString("dd-MM-yyyy");
        public DateTime JournalDate { get; set; } = DateTime.UtcNow;
        public decimal OpeningBalance { get; set; } = 0;
        //public SupportDocuments? logo { get; set; } = new SupportDocuments();
        public string? logo { get; set; }
    }


    public class SupportDocuments
    {
        public Guid DocumentID { get; set; }
        public Guid TenantBranchID { get; set; }
        public string DocumentType { get; set; } = String.Empty;
        public string FileName { get; set; } = String.Empty;
        public byte[] DocumentData { get; set; }
        public string MIMEType { get; set; } = String.Empty;
        public Guid? ContactID { get; set; }
    }


    public class GetHomeBranchDetails
    {
        public int CountryID { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string CurrencyCode { get; set; } = string.Empty;
        public string CurrencySymbol { get; set; } = string.Empty;
        public string DateTimeFormat { get; set; } = string.Empty;
        public string DateFormat { get; set; } = string.Empty;
        public string TimeFormat { get; set; } = string.Empty;
        public string LanguageCulture { get; set; } = string.Empty;
        public int TimeZoneID { get; set; }
    }

    public class ContactAddress
    {
        public Guid ContactAddressID { get; set; }
        public Guid ContactID { get; set; }
        public string AddressType { get; set; } = string.Empty;
        public string Address1 { get; set; } = string.Empty;
        public string? Address2 { get; set; }
        public string Country { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public int Pincode { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    }

    public class ActiveFranchiseeContact
    {
        public Guid TenantBranchID { get; set; }
        public Guid ContactID { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public string ContactName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string AlternatePhone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
    }

    public class GetBranchName
    {
        public Guid TenantBranchID { get; set; }
        public string BranchName { get; set; } = string.Empty;
    }

    public class GetBranchWiseContactInfo
    {
        public Guid TenantBranchID { get; set; }
        public string TenantBranchName { get; set; } = string.Empty;
        public Guid ContactID { get; set; }
        public string ContactName { get; set; } = string.Empty;
        public Guid AddressTypeID { get; set; }
        public string AddressType { get; set; } = string.Empty;
        public string Address1 { get; set; } = string.Empty;
        public string? Address2 { get; set; }
        public string Country { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public string Taluk { get; set; } = string.Empty;
        public int Pincode { get; set; }
    }

    public class GetNewBranchWiseContactInfo : GetBranchWiseContactInfo
    {
        public string FranchiseeID { get; set; } = string.Empty;
    }
}
