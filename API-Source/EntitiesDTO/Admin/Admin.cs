using Dawn.Utilities.i18N;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using EntitiesDTO.Payment;
using System.ComponentModel.DataAnnotations;
using Utilities.Attributes;
using Realtor.Utilities;
using EntitiesDTO.Course;

namespace EntitiesDTO.Admin
{
    public class FranchiseeRegister
    {
        [Required(ErrorMessage = "Contact name could not be empty")]
        public string ContactName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Gender could not be empty")]
        public string Gender { get; set; } = string.Empty;
        [Required(ErrorMessage = "Phone code could not be empty")]
        [StringLength(7, ErrorMessage = "{0} value cannot exceed {1} characters")]
        public string PhoneCode { get; set; } = string.Empty;
        [Required(ErrorMessage = "Phone number could not be empty")]
        [StringLength(maximumLength: 15, MinimumLength = 8, ErrorMessage = "Please enter the valid phone number")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Please enter the valid phone number")]
        public string Phone { get; set; } = string.Empty;
        public string? AlternatePhoneCode { get; set; } = string.Empty;
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Please enter the valid phone number")]
        public string? AlternatePhone { get; set; } = string.Empty;
        [Required(ErrorMessage = "Email could not be empty")]
        [RegularExpression("^[a-zA-Z0-9+_.-]+@[a-zA-Z0-9.-]+$", ErrorMessage = "Enter valid email address")]
        [StringLength(maximumLength: 255, ErrorMessage = "{0} value cannot exceed {1} characters")]
        public string Email { get; set; } = string.Empty;
        [Required(ErrorMessage = "Franchisee name could not be empty")]
        public string FranchiseeName { get; set; } = string.Empty;
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
        [Required]
        public string Taluk { get; set; } = string.Empty;
        [StringLength(maximumLength: 6, ErrorMessage = "Please enter the valid pincode")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Please enter the valid pincode")]
        public string Pincode { get; set; } = string.Empty;
        [AllowedExtensions(new string[] { ".png", ".jpg", ".jpeg" })]
        [MaxFileSize(2, ErrorMessage = "The file size should be 2MB")]
        public IFormFile OwnerImage { get; set; }
        [AllowedExtensions(new string[] { ".png", ".jpg", ".jpeg" })]
        [MaxFileSize(2, ErrorMessage = "The file size should be 2MB")]
        public List<IFormFile> OutletImage { get; set; }
        [AllowedExtensions(new string[] { ".png", ".jpg", ".jpeg" })]
        [MaxFileSize(2, ErrorMessage = "The file size should be 2MB")]
        public List<IFormFile> LaboratoryImage { get; set; }
        [AllowedExtensions(new string[] { ".png", ".jpg", ".jpeg" })]
        [MaxFileSize(2, ErrorMessage = "The file size should be 2MB")]
        public List<IFormFile> ClassRoomImage { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public List<BankAccount> BankAccount { get; set; } = new List<BankAccount>();
        public Guid EmailOTPTransID { get; set; }
    }

    public class BankAccount
    {
        public string AccountType { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public string AccountNumber { get; set; } = string.Empty;
        public string Bank { get; set; } = string.Empty;
        public string? IFSC { get; set; }
        public string? Description { get; set; }
        public string? BranchName { get; set; }
    }

    public class FranchiseeRegisterDTO
    {
        public string ContactName { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string PhoneCode { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string AlternatePhoneCode { get; set; } = string.Empty;
        public string AlternatePhone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FranchiseeName { get; set; } = string.Empty;
        public string Address1 { get; set; } = string.Empty;
        public string? Address2 { get; set; }
        public string Area { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string Taluk { get; set; } = string.Empty;
        public string Pincode { get; set; } = string.Empty;
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    }

    public class SupportDocument
    {
        public string DocumentType { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public byte[] Content { get; set; } = new byte[0];
        public string ContentType { get; set; } = string.Empty;
    }
    public class Images
    {
        public string DocumentType { get; set; } = string.Empty;
        public IFormFile Content { get; set; }
    }


    public class FranchiseeRejection
    {
        [IsGuidEmpty]
        public Guid FranchiseeRequestID { get; set; }
        public string Reason { get; set; } = string.Empty;
    }

    public class FranchiseePayment
    {
        public Guid FranchiseeRequestID { get; set; }
        public Guid KeyValue { get; set; }
        public string KeyTable { get; set; } = string.Empty;
        public string KeyColumn { get; set; } = string.Empty;
        public string TransactionName { get; set; } = string.Empty;
        public Guid CreatedBy { get; set; }
    }

    public class FranchiseePayments
    {
        public Guid FranchiseeRequestID { get; set; }
        public string TransactionName { get; set; } = string.Empty;
        public string PaymodeType { get; set; } = string.Empty;
        public string KeyTable { get; set; } = string.Empty;
        public string KeyColumn { get; set; } = string.Empty;
        public Guid KeyValue { get; set; }
        public decimal TransactionAmount { get; set; }
        [JsonIgnore]
        public DateI18 VoucherDateI18 { get; set; } = new DateI18();
        public string VoucherDate { get { return VoucherDateI18; } set { VoucherDateI18 = value; } }
        public Guid PaymodeID { get; set; }
        public string BankName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public decimal RoundOff { get; set; }
        public string TransactionNo { get; set; } = string.Empty;
        public string TransactionDate { get; set; } = string.Empty;
        public decimal BankCommission { get; set; }
        public string LedgerName { get; set; } = string.Empty;
        public Guid ToLedgerID { get; set; }
        public string ToLedgerName { get; set; } = string.Empty;
    }

    public class ActivateFranchisee
    {
        [IsGuidEmpty]
        public Guid FranchiseeRequestID { get; set; }
    }

    public class FranchiseeDetails
    {
        public Guid FranchiseeEnquiryID { get; set; }
        public string FranchiseeName { get; set; } = string.Empty;
        public string Address1 { get; set; } = string.Empty;
        public string Address2 { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string Pincode { get; set; } = string.Empty;
        public string Latitude { get; set; } = string.Empty;
        public string Longitude { get; set; } = string.Empty;
        public string ContactName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneCode { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string AlternatePhoneCode { get; set; } = string.Empty;
        public string AlternatePhone { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
    }


    public class AddressInfo
    {
        public Guid ContactID { get; set; }
        public Guid AddressTypeID { get; set; }
        public string AddressType { get; set; } = string.Empty;
        public string Address1 { get; set; } = string.Empty;
        public string Address2 { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string Pincode { get; set; } = string.Empty;
        public string Latitude { get; set; } = string.Empty;
        public string Longitude { get; set; } = string.Empty;
        // added on 19-05-2025
        public string Taluk { get; set; } = string.Empty;
    }

    public class FranchiseeAddress
    {
        public Guid FranchiseeRequestID { get; set; }
        public string Address1 { get; set; } = string.Empty;
        public string Address2 { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string Pincode { get; set; } = string.Empty;
        public string Latitude { get; set; } = string.Empty;
        public string Longitude { get; set; } = string.Empty;
        public string Taluk { get; set; } = string.Empty;
    }

    public class FranchiseeImg
    {
        public Guid   FranchiseeRequestID { get; set; }
        public string DocumentName { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string DocumentType { get; set; } = string.Empty;
        public string ContentUrl { get; set; } = string.Empty;
        [JsonIgnoreAttribute]
        public byte[]? AttachmentContent { get; set; } = Array.Empty<byte>();
        public string ThumbnailData { get; set; } = string.Empty;
    }

    public class FranchiseeInfo
    {
        public Guid FranchiseeEnquiryID { get; set; }
        public Guid FranchiseeRequestID { get; set; }
        public Guid PackageLogID { get; set; } = Guid.Empty;
        public string FranchiseeCode { get; set; } = string.Empty;
        public string FranchiseeName { get; set; } = string.Empty;
        public string ContactName { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string AlternatePhone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string RequestDate { get; set; } = string.Empty;
        public string StatusDate { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public List<FranchiseeAddress> Address { get; set; } = new List<FranchiseeAddress>();
        public List<FeesDetail> FeesDetails { get; set; } = new List<FeesDetail>();
        public List<Payments> PaymentDetails { get; set; } = new List<Payments>();
        public List<FranchiseeImg> FranchiseeImgs { get; set; } = new List<FranchiseeImg>();
    }

    public class FranchiseeRequestInfo
    {
        public Guid FranchiseeRequestID { get; set; }
        public Guid PackageLogID { get; set; }
        public string FranchiseeCode { get; set; } = string.Empty;
        public string BranchName { get; set; } = string.Empty;
        public string ContactName { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string AlternatePhone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public string RequestDate { get; set; } = string.Empty;
        public List<FranchiseeAddress> Address { get; set; } = new List<FranchiseeAddress>();
        public List<FeesDetail> FeesDetails { get; set; } = new List<FeesDetail>();
        public List<FranchiseeImg> FranchiseeImgs { get; set; } = new List<FranchiseeImg>();
        public List<PaymentHistoryMaster> Payments { get; set; } = new List<PaymentHistoryMaster>();
        public List<FranchiseeBanks> Banks { get; set; } = new List<FranchiseeBanks>();
    }

    public class Payments
    {
        [JsonIgnore]
        public DateI18 TransactionDateI18 { get; set; } = new DateI18();
        public string TransactionDate { get { return TransactionDateI18.ToString() ?? string.Empty; } set { TransactionDateI18 = value; } }
        public string Summary { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public bool IsCancelled { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string PayMode { get; set; } = string.Empty;
        public Guid DocumentID { get; set; }
        public Guid FranchiseePaymentTransID { get; set; }
    }

    public class FranchiseeProfile
    {
        public Guid FranchiseeRequestID { get; set; }
        public string FranchiseeCode { get; set; } = string.Empty;
        public string BranchName { get; set; } = string.Empty;
        public Guid ContactID { get; set; }
        public string ContactName { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string AlternatePhone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public decimal WalletBalance { get; set; }
        public string ApprovalStartDate { get; set; } = string.Empty;
        public string ApprovalEndDate { get; set; } = string.Empty;
        public string SubscriptionStartDate { get; set; } = string.Empty;
        public string SubscriptionEndDate { get; set; } = string.Empty;
        public List<AddressInfo> Address { get; set; } = new List<AddressInfo>();
        public List<FranchiseeImg> FranchiseeImgs { get; set; } = new List<FranchiseeImg>();
    }

    public class FranchiseeProfileInfo
    {
        public Guid FranchiseeRequestID { get; set; }
        public string FranchiseeCode { get; set; } = string.Empty;
        public string FranchiseeName { get; set; } = string.Empty;
        public Guid ContactID { get; set; }
        public string ContactName { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string AlternatePhone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string ApprovalStartDate { get; set; } = string.Empty;
        public string ApprovalEndDate { get; set; } = string.Empty;
        public string SubscriptionStartDate { get; set; } = string.Empty;
        public string SubscriptionEndDate { get; set; } = string.Empty;
        public List<AddressInfo> Address { get; set; } = new List<AddressInfo>();
        public List<FranchiseeImg> FranchiseeImgs { get; set; } = new List<FranchiseeImg>();
        public Purchase TotalPurchases { get; set; } = new Purchase();
        public List<Renewal> RenewalTrans { get; set; } = new List<Renewal>();
        public List<Wallet> WalletTrans { get; set; } = new List<Wallet>();
    }

    public class Purchase
    {
        public decimal Books { get; set; }
        public decimal ExamIndent { get; set; }
        public decimal Others { get; set; }
    }

    public class Renewal
    {
        public Guid PackageID { get; set; }
        [JsonIgnore]
        public DateI18 PaidDateI18 { get; set; } = new DateI18();
        public string PaidDate { get { return PaidDateI18.ToString() ?? string.Empty; } set { PaidDateI18 = value; } }
        public string PackageType { get; set; } = string.Empty;
        public decimal Fees { get; set; }
        public string Duration { get; set; } = string.Empty;
        [JsonIgnore]
        public DateI18 PackageStartDateI18 { get; set; } = new DateI18();
        public string PackageStartDate { get { return PackageStartDateI18.ToString() ?? string.Empty; } set { PackageStartDateI18 = value; } }
        [JsonIgnore]
        public DateI18 PackageEndDateI18 { get; set; } = new DateI18();
        public string PackageEndDate { get { return PackageEndDateI18.ToString() ?? string.Empty; } set { PackageEndDateI18 = value; } }
    }

    public class Wallet
    {
        [JsonIgnore]
        public DateI18 PaidDateI18 { get; set; } = new DateI18();
        public string PaidDate { get { return PaidDateI18; } set { PaidDateI18 = value; } }
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public decimal Balance { get; set; }
    }

    public class UpdatePackage
    {
        public Guid PackageID { get; set; }
        public decimal Fees { get; set; }
        public bool IsActive { get; set; }
    }

    public class BranchDetail
    {
        public Guid FranchiseeEnquiryID { get; set; }
        public Guid FranchiseeRequestID { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public string FranchiseeCode { get; set; } = string.Empty;
        public string FranchiseeName { get; set; } = string.Empty;
        public string ContactName { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string AlternatePhone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        [JsonIgnore]
        public DateI18 RequestDateI18 { get; set; } = new DateI18();
        public string RequestDate { get { return RequestDateI18.ToString() ?? string.Empty; } set { RequestDateI18 = value; } }
        [JsonIgnore]
        public DateI18 StatusDateI18 { get; set; } = new DateI18();
        public string StatusDate { get { return StatusDateI18.ToString() ?? string.Empty; } set { StatusDateI18 = value; } }
    }

    public class BranchDetail2
    {
        public Guid FranchiseeEnquiryID { get; set; }
        public Guid PackageLogID { get; set; }
        public Guid FranchiseeRequestID { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public string FranchiseeCode { get; set; } = string.Empty;
        public string FranchiseeName { get; set; } = string.Empty;
        public string ContactName { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string AlternatePhone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        [JsonIgnore]
        public DateI18 RequestDateI18 { get; set; } = new DateI18();
        public string RequestDate { get { return RequestDateI18.ToString() ?? string.Empty; } set { RequestDateI18 = value; } }
    }

    public class BranchDetail1
    {
        public Guid FranchiseeRequestID { get; set; }
        public Guid FranchiseeEnquiryID { get; set; }
        public Guid TenantBranchID { get; set; }
        public string FranchiseeCode { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        [JsonIgnore]
        public DateI18 ApprovalStartDateI18 { get; set; } = new DateI18();
        public string ApprovalStartDate { get { return ApprovalStartDateI18; } set { ApprovalStartDateI18 = value; } }
        [JsonIgnore]
        public DateI18 ApprovalEndDateI18 { get; set; } = new DateI18();
        public string ApprovalEndDate { get { return ApprovalEndDateI18; } set { ApprovalEndDateI18 = value; } }
        [JsonIgnore]
        public DateI18 SubscriptionStartDateI18 { get; set; } = new DateI18();
        public string SubscriptionStartDate { get { return SubscriptionStartDateI18; } set { SubscriptionStartDateI18 = value; } }
        [JsonIgnore]
        public DateI18 SubscriptionEndDateI18 { get; set; } = new DateI18();
        public string SubscriptionEndDate { get { return SubscriptionEndDateI18; } set { SubscriptionEndDateI18 = value; } }
        //added on 19-05-2025
        [JsonIgnore]
        public string Taluk { get; set; } = string.Empty;
    }

    public class FranchiseeBaseInfo
    {
        public string TenantBranchName { get; set; } = string.Empty;
        public Guid ContactID { get; set; }
        public Guid TenantBranchID { get; set; }
        public string ContactName { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string AlternatePhone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string DateOfBirth { get; set; } = string.Empty;
    }

    public class ApproveFranchisee
    {
        public Guid FranchiseeRequestID { get; set; }
        public List<Packages> PackageItems { get; set; } = new List<Packages>();
    }

    public class Packages
    {
        public Guid PackageID { get; set; }
        public decimal NewFees { get; set; }
    }

    public class FeesDetail
    {
        public Guid FranchiseeEnquiryID { get; set; }
        public Guid PackageID { get; set; }
        public string Duration { get; set; } = string.Empty;
        public string SubscriptionDuration { get; set; } = string.Empty;
        public decimal ActualFees { get; set; }
        public decimal NewFees { get; set; }
    }

    public class FranchiseeActiveList
    {
        public Guid TenantBranchID { get; set; }
        public string FranchiseeCode { get; set; } = string.Empty;
        public string BranchName { get; set; } = string.Empty;
        public string ContactName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string AlternatePhone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string ApprovalEndDate { get; set; } = string.Empty;
        public string SubscriptionEndDate { get; set; } = string.Empty;
        public string RegistrationDate { get; set; } = string.Empty;
        public string SubscriptionStatus { get; set; } = string.Empty;
        public bool Status { get; set; }
        public string Taluk { get; set; } = string.Empty;
    }

    public class ActiveFranchisee
    {
        public Guid TenantBranchID { get; set; }
        public string FranchiseeCode { get; set; } = string.Empty;
        //public DateTime ApprovalEndDate { get; set; }
        //public DateTime SubscriptionEndDate { get; set; }
        //public DateTime RegistrationDate { get; set; }
        [JsonIgnore]
        public DateI18 ApprovalEndDateI18 { get; set; } = new DateI18();
        public string ApprovalEndDate { get { return ApprovalEndDateI18; } set { ApprovalEndDateI18 = value; } }
        [JsonIgnore]
        public DateI18 SubscriptionEndDateI18 { get; set; } = new DateI18();
        public string SubscriptionEndDate { get { return SubscriptionEndDateI18; } set { SubscriptionEndDateI18 = value; } }
        [JsonIgnore]
        public DateI18 RegistrationDateI18 { get; set; } = new DateI18();
        public string RegistrationDate { get { return RegistrationDateI18; } set { RegistrationDateI18 = value; } }

        public bool IsActive { get; set; }
    }

    public class SubscriptionInfo
    {
        [JsonIgnore]
        public DateI18 ApprovalStartDateI18 { get; set; } = new DateI18();
        public string ApprovalStartDate { get { return ApprovalStartDateI18; } set { ApprovalStartDateI18 = value; } }
        [JsonIgnore]
        public DateI18 ApprovalEndDateI18 { get; set; } = new DateI18();
        public string ApprovalEndDate { get { return ApprovalEndDateI18; } set { ApprovalEndDateI18 = value; } }
        public string ApprovalStatus { get; set; } = string.Empty;
        [JsonIgnore]
        public DateI18 SubscriptionStartDateI18 { get; set; } = new DateI18();
        public string SubscriptionStartDate { get { return SubscriptionStartDateI18; } set { SubscriptionStartDateI18 = value; } }
        [JsonIgnore]
        public DateI18 SubscriptionEndDateI18 { get; set; } = new DateI18();
        public string SubscriptionEndDate { get { return SubscriptionEndDateI18; } set { SubscriptionEndDateI18 = value; } }
        public string SubscriptionStatus { get; set; } = string.Empty;
        public int PollingInterval { get; set; }
    }

    public class SubscriptionPlan
    {
        public Guid PackageID { get; set; }
        public string PackageName { get; set; } = string.Empty;
        public decimal Fees { get; set; }
        public bool IsActive { get; set; }
    }

    public class SubscriptionLog
    {
        public Guid FranchiseeRequestID { get; set; }
    }

    public class RenewalSubscriptionLog
    {
        public Guid TenantBranchID { get; set; }
        public Guid PackageID { get; set; }
        public string PaymentType { get; set; } = string.Empty;
    }

    public class Payment
    {
        [IsGuidEmpty]
        public Guid FranchiseeRequestID { get; set; }
        public decimal TransactionAmount { get; set; }
        public List<FranchiseeToAdminPay> PaymentItems { get; set; } = new List<FranchiseeToAdminPay>();
    }

    public class JournalPosting
    {
        public string ReceiptType { get; set; } = string.Empty;
        public Guid GiverContactID { get; set; }
        public decimal TransactionAmount { get; set; }
        public Guid PaymentID { get; set; }
    }

    public class RenewalPayment
    {
        public Guid PackageID { get; set; }
        public decimal TransactionAmount { get; set; }
        public string PaymentType { get; set; } = string.Empty;
        public List<FranchiseeToAdminPay> PaymentItems { get; set; } = new List<FranchiseeToAdminPay>();
    }

    public class SubscriptionLogInfo
    {
        public Guid PackageLogID { get; set; }
        public Guid PackageID { get; set; }
        public Guid FranchiseeEnquiryID { get; set; }
        public decimal NewFees { get; set; }
        public string PackageType { get; set; } = string.Empty;
        [JsonIgnore]
        public DateI18 PackageStartDateI18 { get; set; } = new DateI18();
        public string PackageStartDate { get { return PackageStartDateI18; } set { PackageStartDateI18 = value; } }
        [JsonIgnore]
        public DateI18 PackageEndDateI18 { get; set; } = new DateI18();
        public string PackageEndDate { get { return PackageEndDateI18; } set { PackageEndDateI18 = value; } }
    }


    public class FranchiseeBasicInfo
    {
        public Guid TenantBranchID { get; set; }
        public string FranchiseeID { get; set; } = string.Empty;
        public string BranchName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
    }

    public class GetFranchiseeBasicInfo
    {
        public Guid TenantBranchID { get; set; }
        public string FranchiseeCode { get; set; } = string.Empty;
        public string BranchName { get; set; } = string.Empty;
        public string ContactName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public Guid AddressTypeID { get; set; }
        public string AddressType { get; set; } = string.Empty;
        public string Address1 { get; set; } = string.Empty;
        public string? Address2 { get; set; }
        public string Country { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public string Pincode { get; set; } = string.Empty;
        public string Taluk { get; set; } = string.Empty;
    }



    public class ApprovalAndSubPackage
    {
        public Guid PackageID { get; set; }
        public string PackageType { get; set; } = string.Empty;
        public decimal PackageApprovedFees { get; set; }
    }

    public class GetFranchiseeCode
    {
        public Guid TenantBranchID { get; set; }
        public string FranchiseeCode { get; set; } = string.Empty;
        public string FranchiseeName { get; set; } = string.Empty;
        public string FranchiseeShortName { get; set; } = string.Empty;
    }

    public class GetBranchContactDetail
    {
        public Guid TenantBranchID { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public string ContactName { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string AlternatePhone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public Guid AddressTypeID { get; set; }
        public string AddressType { get; set; } = string.Empty;
        public string Address1 { get; set; } = string.Empty;
        public string? Address2 { get; set; }
        public string Country { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public string Pincode { get; set; } = string.Empty;
        public string Taluk { get; set; } = string.Empty;
        public string FranchiseeCode { get; set; } = string.Empty;
    }

    public class RenewalList
    {
        [JsonIgnore]
        public DateI18 RequestDateI18 { get; set; } = new DateI18();
        public string RequestDate { get { return RequestDateI18; } set { RequestDateI18 = value; } }
        public Guid TenantBranchID { get; set; }
        public Guid PackageLogID { get; set; }
        public string FranchiseeCode { get; set; } = string.Empty;
        public string PackageType { get; set; } = string.Empty;
        public string PackageName { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string SubscriptionPeroid { get; set; } = string.Empty;
        [JsonIgnore]
        public DateI18 SubscriptionStartI18 { get; set; } = new DateI18();
        public string SubscriptionStart { get { return SubscriptionStartI18.ToString() ?? string.Empty; } set { SubscriptionStartI18 = value; } }
        [JsonIgnore]
        public DateI18 SubscriptionEndI18 { get; set; } = new DateI18();
        public string SubscriptionEnd { get { return SubscriptionEndI18.ToString() ?? string.Empty; } set { SubscriptionEndI18 = value; } }
        public string PaymentType { get; set; } = string.Empty;
    }

    public class RenewalRequestList
    {
        public string RequestDate { get; set; } = string.Empty;
        public Guid TenantBranchID { get; set; }
        public Guid PackageLogID { get; set; }
        public string FranchiseeCode { get; set; } = string.Empty;
        public string FranchiseeName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string AlternatePhone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address1 { get; set; } = string.Empty;
        public string? Address2 { get; set; }
        public string Country { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public string Pincode { get; set; } = string.Empty;
        public string PackageType { get; set; } = string.Empty;
        public string PackageName { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string SubscriptionPeroid { get; set; } = string.Empty;
        public string Taluk { get; set; } = string.Empty;
        public List<PaymentDetails> PaymentDetails { get; set; } = new List<PaymentDetails>();
    }

    public class ContactInfo
    {
        public Guid TenantBranchID { get; set; }
        public Guid ContactID { get; set; }
        public string ContactName { get; set; } = string.Empty;
        public string TenantBranchName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string AlternatePhone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string DateOfBirth { get; set; } = string.Empty;
    }

    public class RenewalPaymentList
    {
        public Guid TenantBranchID { get; set; }
        public Guid PackageLogID { get; set; }
        public string FranchiseeCode { get; set; } = string.Empty;
        public string FranchiseeName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string AlternatePhone { get; set; } = string.Empty;
        public string Address1 { get; set; } = string.Empty;
        public string? Address2 { get; set; }
        public string Country { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public string Pincode { get; set; } = string.Empty;
        public string PackageType { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public List<PaymentDetails> PaymentDetails { get; set; } = new List<PaymentDetails>();
    }

    public class NewRenewalList
    {
        public Guid TenantBranchID { get; set; }
        public Guid PackageLogID { get; set; }
        public string FranchiseeCode { get; set; } = string.Empty;
        public string PackageType { get; set; } = string.Empty;
        public string PaymentType { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }

    public class AdminDashBoard
    {
        public int FranchiseePending { get; set; }
        public int FranchiseeActive { get; set; }
        public int FranchiseeExpired { get; set; }
        public int FranchiseeLocked { get; set; }
        public int WalletPending { get; set; }
        public decimal WalletBalance { get; set; }
        public int MailPending { get; set; }
        public int IndentMaterial { get; set; }
        public int IndentExam { get; set; }
        public int DespatchMaterial { get; set; }
        public int DespatchCertificate { get; set; }
        public int DespatchMarksheet { get; set; }
        public int PrintingCertificate { get; set; }
        public int PrintingMarksheet { get; set; }
        public int RenewalApproval { get; set; }
        public int RenewalSubscription { get; set; }
        public int CourseTotal { get; set; }
        public string ContactTitle { get; set; } = string.Empty;
        public string ContactName { get; set; } = string.Empty;
        public string FranchiseeName { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public string Taluk { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public string Day { get; set; } = string.Empty;
    }

    public class FranchiseeSummaryByAdmin
    {
        public int Pending { get; set; }
        public int Active { get; set; }
        public int Expired { get; set; }
        public int Locked { get; set; }
        public int Approval { get; set; }
        public int Subscription { get; set; }
    }

    public class DashboardByFranchisee
    {
        [JsonIgnore]
        public DateI18 ApprovalStartDateI18 { get; set; } = new DateI18();
        public string ApprovalStartDate { get { return ApprovalStartDateI18; } set { ApprovalStartDateI18 = value; } }
        [JsonIgnore]
        public DateI18 ApprovalEndDateI18 { get; set; } = new DateI18();
        public string ApprovalEndDate { get { return ApprovalEndDateI18; } set { ApprovalEndDateI18 = value; } }
        [JsonIgnore]
        public DateI18 SubscriptionStartDateI18 { get; set; } = new DateI18();
        public string SubscriptionStartDate { get { return SubscriptionStartDateI18; } set { SubscriptionStartDateI18 = value; } }
        [JsonIgnore]
        public DateI18 SubscriptionEndDateI18 { get; set; } = new DateI18();
        public string SubscriptionEndDate { get { return SubscriptionEndDateI18; } set { SubscriptionEndDateI18 = value; } }
        public int ApprovalTotalDays { get; set; }
        public int ApprovalCompletedDays { get; set; }
        public int ApprovalRemainingDays { get; set; }
        public int SubscriptionTotalDays { get; set; }
        public int SubscriptionCompletedDays { get; set; }
        public int SubscriptionRemainingDays { get; set; }
        public int Courses { get; set; }
        public int Inbox { get; set; }
        public decimal WalletBalance { get; set; } = 0;
        public int ActiveBatch { get; set; }
        public int CompletedBatch { get; set; }
        public int AlumniStudent { get; set; }
        public int CurrentStudent { get; set; }
        public int DiscontinuedStudent { get; set; }
        public decimal FeesBalance { get; set; } = 0;
        public int MaterialPending { get; set; }
        public int ExamPending { get; set; }
        public int MarksheetPending { get; set; }
        public int CertificatePending { get; set; }
        public string ContactTitle { get; set; } = string.Empty;
        public string ContactName { get; set; } = string.Empty;
        public string FranchiseeName { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public string Taluk { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public string Day { get; set; } = string.Empty;
        public AdminContact AdminDetails { get; set; } = new AdminContact();
        public ProfileImage OwnerImage { get; set; } = new ProfileImage();
    }
    

    public class TransactionJournalPosting
    {
        public string JournalDate { get; set; } = string.Empty;
        public decimal TransactionAmount { get; set; }
        public string? TransactionCurrency { get; set; }
        public decimal? ConversionRate { get; set; }
        public string Narration { get; set; } = string.Empty;
        public Guid GiverContactID { get; set; }
        public Guid ReceiverContactID { get; set; }
        public List<DepositDetails>? DepositDetails { get; set; }
    }

    public class DepositDetails
    {
        public decimal DepositAmount { get; set; }
        public string DepositTo { get; set; } = string.Empty;
        public Guid PaymentID { get; set; } = Guid.Empty;
        public string PaymentDetail { get; set; } = string.Empty;
        public Guid? VoucherID { get; set; } = Guid.Empty;
    }

    public class RejectPaidRequest
    {
        [IsGuidEmpty]
        public Guid FranchiseeRequestID { get; set; }
        public Guid PackageLogID { get; set; }
        public string Reason { get; set; } = string.Empty;
    }

    public class RejectRenewal
    {
        [IsGuidEmpty]
        public Guid PackageLogID { get; set; }
        public string Reason { get; set; } = string.Empty;
    }

    public class GuideVideos
    {
        public Guid GuideVideoID { get; set; } = Guid.Empty;
        public Guid TenantBranchID { get; set; }
        public string Franchisee { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string MobileApp { get; set; } = string.Empty;
        public string ShowIn { get; set; } = string.Empty;
        public int SortOrder { get; set; } = 0;
        public string LinkType { get; set; } = string.Empty;
        public string ContentUrl { get; set; } = string.Empty;
    }

    public class AddVideos
    {
        public Guid? TenantBranchID { get; set; }
        public string Title { get; set; } = string.Empty;
        public string MobileApp { get; set; } = string.Empty;
        public string ShowIn { get; set; } = string.Empty;
        public int SortOrder { get; set; }
        public string LinkType { get; set; } = string.Empty;
        public string ContentUrl { get; set; } = string.Empty;
    }

    public class EditVideo : AddVideos
    {
        public Guid GuideVideoID { get; set; }
    }

    public class GetVideos
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<GuideVideos> Videos { get; set; } = new List<GuideVideos>();
    }

    public record ActiveDeactive
    {
        [IsGuidEmpty]
        public Guid ContactID { get; set; }
        public bool Status { get; set; }
    }

    public class MblFranchisee
    {
        public Guid TenantBranchID { get; set; }
        public string FranchiseeCode { get; set; } = string.Empty;
        public string FranchiseeName { get; set; } = string.Empty;
        public string ContactName { get; set; } = string.Empty;
        public string Address1 { get; set; } = string.Empty;
        public string Address2 { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string PhoneCode { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string AlternatePhoneCode { get; set; } = string.Empty;
        public string AlternatePhone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string Taluk { get; set; } = string.Empty;
        public string Pincode { get; set; } = string.Empty;
        public string Latitude { get; set; } = string.Empty;
        public string Longitude { get; set; } = string.Empty;
        public GetImages OwnerImage { get; set; } = new GetImages();
        public List<MblCourse> Courses { get; set; } = new List<MblCourse>();
    }

    public class MblFilter
    {
        public string State { get; set; } = string.Empty;
        public string? District { get; set; }
        public string? Taluk { get; set; }
    }

    public class GetRenewalPlanList
    {
        public Guid PackageID { get; set; }
        public string PackageType { get; set; } = string.Empty;
        public string PackageName { get; set; } = string.Empty;
        public decimal NetPackageFees { get; set; }
        public string Duration { get; set; } = string.Empty;
    }

    public class FranchiseeMblDashboard
    {
        public decimal WalletBalance { get; set; }
        public string Date { get; set; } = string.Empty;
        public int NoOfCourse { get; set; }
        public int Enquiry { get; set; }
        public int Admission { get; set; }
        public int Pending { get; set; }
        public int Closed { get; set; }
        public int Direct { get; set; }
        public int TotalStrength { get; set; }
        public int StudyingOffline { get; set; }
        public int StudyingOnline { get; set; }
        public int StudyingTotal { get; set; }
        public int CompletedOffline { get; set; }
        public int CompletedOnline { get; set; }
        public int CompletedTotal { get; set; }
        public int DiscontinuedOffline { get; set; }
        public int DiscontinuedOnline { get; set; }
        public int DiscontinuedTotal { get; set; }
        public int DistrictLvlRank { get; set; } = 0;
        public int DistrictLvlOutOff { get; set; } = 0;
        public int StateLvlRank { get; set; } = 0;
        public int StateLvlOutOff { get; set; } = 0;
        public int DistrictLvlAchievers { get; set; } = 0;
        public int StateLvlAchievers { get; set; } = 0;
    }

    public class MlbRenewalList
    {
        public Guid PackageLogID { get; set; }
        public string PackageType { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public string PaymentType { get; set; } = string.Empty;
        public List<Payments> Payments { get; set; } = new List<Payments>();
    }

    public class FranchiseeRankList
    {
        public Guid TenantBranchID { get; set; }
        public string State { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string Taluk { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public int Rank { get; set; }
    }

    public class FranchiseeRankStatus
    {
        public int StateLvlOutOff { get; set; }
        public int DistrictLvlOutOff { get; set; }
        public int DistrictLvlRank { get; set; }
        public int StateLvlRank { get; set; }
        public int DistrictLvlAchievers { get; set; }
        public int StateLvlAchievers { get; set; }
    }

    public record TenantUserList
    {
        public Guid UserID { get; set; }
        public Guid ContactID { get; set; }
        public string ContactName { get; set; } = string.Empty;
        public string DepartmentID { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string[] AuthRole { get; set; } = new string[0];
        public string JobTitle { get; set; } = string.Empty;
        public Guid TenantBranchID { get; set; }
        public string TenantBranch { get; set; } = string.Empty;
        public string PhoneCode { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string Employment { get; set; } = string.Empty;
    }

    public class CreateNotice
    {
        public string Content { get; set; } = string.Empty;
    }

    public class GetNotice
    {
        public Guid ContentID { get; set; }
        public string Content { get; set; } = string.Empty;
    }

    public class HelpAndSupport
    {
        public Guid TenantBranchID { get; set; }
        public string FranchiseeName { get; set; } = string.Empty;
        public string ContactName { get; set; } = string.Empty;
        public string PhoneCode { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string AlternatePhoneCode { get; set; } = string.Empty;
        public string AlternatePhone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address1 { get; set; } = string.Empty;
        public string Address2 { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string Pincode { get; set; } = string.Empty;
    }

    public class UploadStudentFeedback
    {
        public string Title { get; set; } = string.Empty;
        public IFormFile File { get; set; }
    }

    public class DTOStudentFeedback : UploadStudentFeedback
    {
        public string FileName { get; set; } = string.Empty;
        public Guid TenantBranchID { get; set; }
    }

    public class RejectStudentFeedback
    {
        [IsGuidEmpty]
        public Guid FeedbackID { get; set; }
        [Required]
        public string Reason { get; set; } = string.Empty;
        [Required]
        public string FileName { get; set; } = string.Empty;
    }

    public class GetStudentFeedback
    {
        public Guid FeedbackID { get; set; }
        public Guid TenantBranchID { get; set; }
        public string FranchiseeCode { get; set; } = string.Empty;
        public string Franchisee { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        [JsonIgnore]
        public DateI18 RequestDateI18 { get; set; } = new DateI18();
        public string RequestDate { get { return RequestDateI18.ToString() ?? string.Empty; } set { RequestDateI18 = value; } }
    }

    public class ApproveFeedback
    {
        [IsGuidEmpty]
        public Guid FeedbackID { get; set; }
        [IsGuidEmpty]
        public Guid TenantBranchID { get; set; }
        public string Title { get; set; } = string.Empty;
        [Required]
        public string ContentUrl { get; set; } = string.Empty;
        [Required]
        public string FileName { get; set; } = string.Empty;
    }

    public record BranchToAdmin
    {
        public Guid PaymentVoucherID { get; set; } = Guid.NewGuid();
        public Guid ReceiptVoucherID { get; set; } = Guid.NewGuid();
        public Guid GiverBranchID { get; set; }
        public Guid ReceiverBranchID { get; set; }
        public decimal BaseAmount { get; set; }
        public string Narration { get; set; } = string.Empty;
        public Guid GiverContactID { get; set; }
        public Guid ReceiverContactID { get; set; }
        public decimal TransactionAmount { get; set; }
        public string? TransactionCurrency { get; set; } = string.Empty;
        public decimal? ConversionRate { get; set; }
        public Guid PaymentID { get; set; }
        public string PaymentDetail { get; set; } = string.Empty;
        public Guid FromLedgerID { get; set; }
        public Guid ToLedgerID { get; set; }
    }

    public class GetBanks
    {
        public Guid FranchiseeEnquiryID { get; set; }
        public string AccountType { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public string AccountNumber { get; set; } = string.Empty;
        public string Bank { get; set; } = string.Empty;
        public string IFSC { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class AddBankAccount
    {
        public Guid AccountID { get; set; }
        public string AccountType { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public string AccountNumber { get; set; } = string.Empty;
        public string Bank { get; set; } = string.Empty;
        public string? IFSC { get; set; }
        public decimal OpeningBalance { get; set; }
        public string IncreasedBy { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string JournalDate { get; set; } = string.Empty;
        public Guid AdminContactID { get; set; }
        public Guid BranchID { get; set; }
    }
    public class GetMobileApp
    {
        public string MinValue { get; set; } = string.Empty;
    }

    public class Ledger
    {
        public int SlNo { get; set; } = 0;
        public Guid LedgerID { get; set; }
        public string LedgerName { get; set; } = string.Empty;
        public string ParentLedgerID { get; set; } = string.Empty;
        public string CategoryID { get; set; } = string.Empty;
        public bool IsLedgerGroup { get; set; }
        public bool AllowCustomSubLedger { get; set; }
    }

    public class NewPaymentDetails
    {
        public Guid PaymentModeID { get; set; }
        public string? PayModeType { get; set; }
        public decimal PayModeValue { get; set; }
        public decimal Amount { get; set; }
        public Guid FromLedgerID { get; set; }
        public string FromLedgerName { get; set; } = string.Empty;
        public Guid ToLedgerID { get; set; }
        public string ToLedgerName { get; set; } = string.Empty;
        public string TransactionNo { get; set; } = string.Empty;
        public string TransactionDate { get; set; } = string.Empty;
        public Guid VoucherID { get; set; } = Guid.NewGuid();
    }

    public record BranchToBranch
    {
        public Guid VoucherID { get; set; } = Guid.NewGuid();
        public Guid BranchID { get; set; }
        public decimal BaseAmount { get; set; }
        public string Narration { get; set; } = string.Empty;
        public Guid GiverContactID { get; set; }
        public Guid ReceiverContactID { get; set; }
        public decimal TransactionAmount { get; set; }
        public string? TransactionCurrency { get; set; } = string.Empty;
        public decimal? ConversionRate { get; set; }
        public Guid PaymentID { get; set; }
        public string PaymentDetail { get; set; } = string.Empty;
        public Guid LedgerID { get; set; }
        public string JournalType { get; set; } = string.Empty;
    }

    public class RankList
    {
        public Guid TenantBranchID { get; set; }
        public Guid ContactID { get; set; }
        public string ContactName { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public string Taluk { get; set; } = string.Empty;
        public int TotalCount { get; set; }
        public int Rank { get; set; }
        public string DocumentName { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string DocumentType { get; set; } = string.Empty;
        public string ContentUrl { get; set; } = string.Empty;
        public int StateCount { get; set; } 
        public string FranchiseeCode { get; set; } = string.Empty;
        public string CentreName { get; set; } = string.Empty;
        public int PurchaseRank { get; set; } = 0;
        public string DistrictRank { get; set; } = string.Empty;
    }

    public class StateRankFilter
    {
        [Required]
        public int Year { get; set; }
        [Required]
        public string State { get; set; }
        [Required]
        public string Type { get; set; }
    }

    public class DistrictRankFilter
    {
        [Required]
        public int Year { get; set; }
        [Required]
        public string State { get; set; }
        [Required]
        public string District { get; set; }
        [Required]
        public string Type { get; set; }
    }

    public class StateAchieversFilter
    {
        [Required]
        public int Year { get; set; }
        [Required]
        public string State { get; set; }
    }

    public class DistrictAchieversFilter
    {
        [Required]
        public int Year { get; set; }
        [Required]
        public string State { get; set; }
        [Required]
        public string District { get; set; }
    }

    public record TenantServiceInfo
    {
        public Guid TenantID { get; set; }
        public string ServiceName { get; set; } = string.Empty;
    }

    public class FranchiseeBanks
    {
        public Guid AccountID { get; set; }
        public string AccountType { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public string AccountNumber { get; set; } = string.Empty;
        public string Bank { get; set; } = string.Empty;
        public string IFSC { get; set; } = string.Empty;
        public string BranchName { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
    }

    public class GetImages
    {
        public string DocumentName { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string DocumentType { get; set; } = string.Empty;
        public string ContentUrl { get; set; } = string.Empty;
    }

    public class GetAdminContactInfo
    {
        public string PhoneCode { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string BranchName { get; set; } = string.Empty;
    }

    public class VerifyOTP
    {
        public Guid OTPTransID { get; set; }
        public string OTPCode { get; set; }
        public string OTPType { get; set; }
    }

    public class OTPVerifyResult
    {
        public bool IsOTPVerified { get; set; }
        public bool IsEmailVerified { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public string Message { get; set; }
    }

    public class OTPResult
    {
        public Guid OTPTransID { get; set; }
        public int ValidityTime { get; set; }
    }

    public class EmailResendRequest
    {
        [Required]
        [RegularExpression("^[a-zA-Z0-9+_.-]+@[a-zA-Z0-9.-]+$",
            ErrorMessage = "Enter valid email address")]
        [StringLength(maximumLength: 255, ErrorMessage = "{0} value cannot exceed {1} characters")]
        public string Email { get; set; }
    }

    public class OTPVerify
    {
        [IsGuidEmpty]
        public Guid EmailOTPTransID { get; set; }
        [Required]
        public string EmailOTPCode { get; set; }
    }

    public class EmailOTPRequest
    {
        public string MailAddress { get; set; } = string.Empty;
        public string OTPType { get; set; } = string.Empty;
        public List<string> MessageParameter { get; set; } = new List<string>();
    }

    public class EmailOTPSentResult
    {
        public Guid OTPTransID { get; set; }
        public string EmailAddress { get; set; } = string.Empty;
        public int ValidityTime { get; set; }
    }

    public class RegistrationOTPResult
    {
        public Guid EmailOTPTransID { get; set; }
        public int EmailValidityTime { get; set; }
    }

    public class RegistrationOTPRequest
    {
        [Required]
        [RegularExpression("^[a-zA-Z0-9+_.-]+@[a-zA-Z0-9.-]+$",
                ErrorMessage = "Enter valid email address")]
        [StringLength(maximumLength: 255, ErrorMessage = "{0} value cannot exceed {1} characters")]
        public string Email { get; set; }
    }

    public class IsEmailCheck
    {
        public string Email { get; set; }
    }

    public class EmailSendRequest
    {
        public string MailAddress { get; set; } = string.Empty;
        public string EmailType { get; set; } = string.Empty;
        public List<string> MessageParameter { get; set; } = new List<string>();
    }

    public class PaymentHistoryMaster
    {
        public Guid FranchiseePaymentID { get; set; }
        public string ReceiptNo { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        [JsonIgnore]
        public DateI18 VoucherDateI18 { get; set; } = new DateI18();
        public string VoucherDate { get { return VoucherDateI18?.ToString() ?? string.Empty; } set { VoucherDateI18 = value; } }
        [JsonIgnore]
        public DateI18 RejectedDateI18 { get; set; } = new DateI18();
        public string RejectedDate { get { return RejectedDateI18?.ToString() ?? string.Empty; } set { RejectedDateI18 = value; } }
        public decimal Amount { get; set; }
        public List<PaymentHistoryTrans> Payments { get; set; } = new List<PaymentHistoryTrans>();
    }

    public class PaymentHistoryTrans
    {
        public Guid FranchiseePaymentID { get; set; }
        public Guid FranchiseePaymentTransID { get; set; }
        public string PayMode { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        [JsonIgnore]
        public DateI18 TransactionDateI18 { get; set; } = new DateI18();
        public string TransactionDate { get { return TransactionDateI18.ToString() ?? string.Empty; } set { TransactionDateI18 = value; } }
        public decimal Amount { get; set; }
        public Guid DocumentID { get; set; }
    }

    public class TalukList
    {
        public string Taluk { get; set; } = string.Empty;
    }

    public class GetApprovalCertificate
    {
        public string ContactName { get; set; } = string.Empty;
        public string FranchiseeName { get; set; } = string.Empty;
        public string FranchiseeCode { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        [JsonIgnore]
        public DateI18 ApprovedDateI18 { get; set; } = new DateI18();
        public string ApprovedDate { get { return ApprovedDateI18.ToString() ?? string.Empty; } set { ApprovedDateI18 = value; } }
        public string ValidPeriod { get; set; } = string.Empty;
        [JsonIgnore]
        public DateI18 NextRenewalDateI18 { get; set; } = new DateI18();
        public string NextRenewalDate { get { return NextRenewalDateI18.ToString() ?? string.Empty; } set { NextRenewalDateI18 = value; } }
        public Guid DocumentID { get; set; }
        public string ContentUrl { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string MIMEType { get; set; } = string.Empty;
    }

    public class EditFranchiseeCode
    {
        [IsGuidEmpty]
        public Guid FranchiseeEnquiryID { get; set; }
        [Required]
        public string FranchiseeCode { get; set; }
    }

    public class GetRenewalPlanListNew
    {
        public Guid PackageID { get; set; }
        public string PackageType { get; set; } = string.Empty;
        public string PackageName { get; set; } = string.Empty;
        public decimal NetPackageFees { get; set; }
        public string Duration { get; set; } = string.Empty;
        [JsonIgnore]
        public DateI18 ApprovalStartDateI18 { get; set; } = new DateI18();
        public string ApprovalStartDate { get { return ApprovalStartDateI18; } set { ApprovalStartDateI18 = value; } }
        [JsonIgnore]
        public DateI18 ApprovalEndDateI18 { get; set; } = new DateI18();
        public string ApprovalEndDate { get { return ApprovalEndDateI18; } set { ApprovalEndDateI18 = value; } }
        public string ApprovalStatus { get; set; } = string.Empty;
        [JsonIgnore]
        public DateI18 SubscriptionStartDateI18 { get; set; } = new DateI18();
        public string SubscriptionStartDate { get { return SubscriptionStartDateI18; } set { SubscriptionStartDateI18 = value; } }
        [JsonIgnore]
        public DateI18 SubscriptionEndDateI18 { get; set; } = new DateI18();
        public string SubscriptionEndDate { get { return SubscriptionEndDateI18; } set { SubscriptionEndDateI18 = value; } }
    }

    public class StateList
    {
        public string StateProvinceName { get; set; } = string.Empty;
    }
    public class DistrictList
    {
        public string DistrictProvinceName { get; set; } = string.Empty;
    }
    public class FranchiseeList
    {
        public string FranchiseeCode { get; set; } = string.Empty;
        public string ContactName { get; set; } = string.Empty;
        public string PhoneCode { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FranchiseeName { get; set; } = string.Empty;
        public string Taluk { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public Guid ContactID { get; set; } = Guid.Empty;
        public string ImageString { get; set; } = string.Empty;
    }

    public record DocumentBase64Details
    {
        public Guid DocumentID { get; set; }
        public Guid PrimaryKeyID { get; set; }
        public string DocumentType { get; set; } = string.Empty;
        public string MIMEType { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string FileExtension { get; set; } = string.Empty;
        public string ProfileBase64 { get; set; } = string.Empty;
        public string ThumbnailBase64 { get; set; } = string.Empty;
    }
    public record EmailCheck
    {
        public string Email { get; set; } = string.Empty;
    }
    //public class StateList
    //{
    //    public string State { get; set; } = string.Empty;
    //}
    //public class DistrictList
    //{
    //    public string District { get; set; } = string.Empty;
    //}
    public class BranchContactDetailUpdated   //Additional with taluk
    {
        public Guid TenantBranchID { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public string ContactName { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string AlternatePhone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public Guid AddressTypeID { get; set; }
        public string AddressType { get; set; } = string.Empty;
        public string Address1 { get; set; } = string.Empty;
        public string? Address2 { get; set; }
        public string Country { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public string Pincode { get; set; } = string.Empty;
        public string Taluk { get; set; } = string.Empty;
    }
    public class Paymenttypes
    {
        public string VoucherNo { get; set; } = string.Empty;
        public DateI18 VoucherDateI18 { get; set; } = new DateI18();
        public string VoucherDate { get { return VoucherDateI18.ToString() ?? string.Empty; } set { VoucherDateI18 = value; } }
        public int TransactionAmount { get; set; }
        public string PayMode { get; set; } = string.Empty;
    }
    public class GetCourse
    {
        //public Guid BatchMappingID { get; set; }
        public Guid CourseID { get; set; }
        public string ShortName { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public string CourseTypeName { get; set; } = string.Empty;
        public decimal ExamFees { get; set; }
        public int Quantity { get; set; }
        public int TotalFees { get; set; }
        public int SlNo { get; set; }
    }
    public class AdminContact
    {
        public string ContactTitle { get; set; } = string.Empty;
        public string ContactName { get; set; } = string.Empty;
        public string FranchiseeName { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public string Taluk { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string Address1 { get; set; } = string.Empty;
        public string Address2 { get; set; } = string.Empty;
        public string Pincode { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Website { get; set; } = string.Empty;
    }
    public class GetFranchiseeAddress
    {
        public string FranchiseeCode { get; set; } = string.Empty;
        public string FranchiseeName { get; set; } = string.Empty;
        public string ContactName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address1 { get; set; } = string.Empty;
        public string Address2 { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string Pincode { get; set; } = string.Empty;
        public string Website { get; set; } = string.Empty;
    }
    public class ProfileImage
    {
        public Guid ContactID { get; set; }
        public string ContactName { get; set; } = string.Empty;
        public string DocumentName { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string DocumentType { get; set; } = string.Empty;
        public string ContentUrl { get; set; } = string.Empty;
    }
    public class GetWholePeriod
    {
        public DateI18 CreatedDateI18 { get; set; } = new DateI18();
        public string CreatedDate { get { return CreatedDateI18.ToString() ?? string.Empty; } set { CreatedDateI18 = value; } }
    }
    public class FranchiseeDetailsForIDCard
    {
        public Guid TenantBranchID { get; set; }
        public string FranchiseeCode { get; set; } = string.Empty;
        public string FranchiseeName { get; set; } = string.Empty;
        public string ContactName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address1 { get; set; } = string.Empty;
        public string Address2 { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public string Taluk { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string Pincode { get; set; } = string.Empty;
    }
    public class ChargeUpdate
    {
        public int Courier { get; set; }
        public int IDCard { get; set; }
    }
    /*For TCDS WEBSITE*/
    public class FranchiseDetails
    {
        public string FranchiseeName { get; set; } = string.Empty;
        public string ContactName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address1 { get; set; } = string.Empty;
        public string Address2 { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public string Taluk { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string Pincode { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public byte[]? AttachmentContent { get; set; } = Array.Empty<byte>();
        public Guid TenantBranchID { get; set; }
    }
    public class BaseUrlData
    {
        public Guid fileId { get; set; }
    }
    public class BaseUrlList
    {
        public string FileName { get; set; } = string.Empty;
        public string MIMEType { get; set; } = string.Empty;
        public byte[]? Content { get; set; } = Array.Empty<byte>();
    }
    public class VendorDetail
    {
        public Guid VendorId { get; set; }
        public string VendorName { get; set; } = string.Empty;
    }
    public class FranchiseeCodeDetails
    {
        public Guid TenantBranchID { get; set; }
        public string FranchiseeCode { get; set; } = string.Empty;
        public string FranchiseeName { get; set; } = string.Empty;
        public string Taluk { get; set; } = string.Empty;
    }
    public class ActiveFranchiseeContactV1
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
        public string Taluk { get; set; } = string.Empty;
    }

    public class EditFranchiseeCodeData
    {
        public Guid FranchiseeEnquiryID { get; set; }

        public string? ContactName { get; set; }
        public string? Gender { get; set; }


        [StringLength(maximumLength: 15, MinimumLength = 8, ErrorMessage = "Please enter the valid phone number")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Please enter the valid phone number")]
        public string? Phone { get; set; }


        [StringLength(7, ErrorMessage = "{0} value cannot exceed {1} characters")]
        public string? PhoneCode { get; set; } 


        public string? FranchiseeCode { get; set; }

        [StringLength(maximumLength: 200, ErrorMessage = "{0} value cannot exceed {1} characters")]
        public string? Address1 { get; set; }

        [StringLength(maximumLength: 200, ErrorMessage = "{0} value cannot exceed {1} characters")]
        public string? Address2 { get; set; }

        [StringLength(maximumLength: 100, ErrorMessage = "{0} value cannot exceed {1} characters")]
        public string? Area { get; set; }


        [StringLength(maximumLength: 50, ErrorMessage = "{0} value cannot exceed {1} characters")]
        public string? Country { get; set; } 

 
        [StringLength(maximumLength: 50, ErrorMessage = "{0} value cannot exceed {1} characters")]
        public string? State { get; set; } 

      
        [StringLength(maximumLength: 50, ErrorMessage = "{0} value cannot exceed {1} characters")]
        public string? District { get; set; } 


        public string? Taluk { get; set; } 


        [StringLength(maximumLength: 6, ErrorMessage = "Please enter the valid pincode")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Please enter the valid pincode")]
        public string? Pincode { get; set; } 
    }



}
