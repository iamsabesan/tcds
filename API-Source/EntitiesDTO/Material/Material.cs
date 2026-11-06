using Dawn.Utilities.i18N;
using EntitiesDTO.Admin;
using EntitiesDTO.Payment;
using EntitiesDTO.Student;
using Newtonsoft.Json;
using Realtor.Utilities;

namespace EntitiesDTO.Material
{
    public class CreateMaterialType
    {
        public string MaterialType { get; set; } = string.Empty;
    }

    public class MaterialTypeList
    {
        public Guid MaterialTypeID { get; set; }
        public string MaterialType { get; set; } = string.Empty;
    }

    public class MaterialTypeWithMaterial
    {
        public Guid MaterialTypeID { get; set; }
        public string MaterialType { get; set; } = string.Empty;
        public List<MaterialNames> Materials { get; set; } = new List<MaterialNames>();
    }

    public class CreateMaterial
    {
        public Guid MaterialTypeID { get; set; }
        public string MaterialName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int MinimumStock { get; set; }
        public int OpeningStock { get; set; }
        public decimal SGST { get; set; }
        public decimal CGST { get; set; }
    }

    public class MaterialList
    {
        public Guid MaterialID { get; set; }
        public string MaterialName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int MinimumStock { get; set; }
        public int OpeningStock { get; set; }
        public decimal CGST { get; set; }
        public decimal SGST { get; set; }
        public int CurrentStock { get; set; } = 0;
    }

    public class MaterialNames
    {
        public Guid MaterialTypeID { get; set;}
        public Guid MaterialID { get; set;}
        public string MaterialType { get; set; } = string.Empty;
        public string MaterialName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal GST { get; set; }
    }

    public class CreateDeliveryMode
    {
        public string DeliveryModeName { get; set; } = string.Empty;
    }

    public class DeliveryModeList
    {
        public Guid DeliveryModeID { get; set; }
        public string DeliveryModeName { get; set; } = string.Empty;
    }

    public class CreateMaterialIndent
    {
        public Guid TenantBranchID { get; set; }
        public Guid CreatedBy { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal  CourierAmount { get; set; }
    }

    public class CreateNewMaterialIndent
    {
        public List<MaterialItem> MaterialItem { get; set; } = new List<MaterialItem>();
        public decimal TotalAmount { get; set; }
        public decimal? CourierAmount { get; set; } = 0;
        public List<FranchiseeToAdminPay> PaymentItems { get; set; } = new List<FranchiseeToAdminPay>();
    }

    public class MaterialItem
    {
        public Guid MaterialID { get; set;}
        public int Quantity { get; set; }
    }
    public class GetBranchIndentInfo
    {
        public Guid BranchMaterialIndentID { get; set; }
        public decimal TotalAmount { get; set; }
        public string IndentNumber { get; set; } = string.Empty;
    }

    public class ApproveMaterialIndent
    {
        public List<MaterialIndent> MaterialIndent { get; set; } = new List<MaterialIndent>();
    }

    public class ApproveExamIndent
    {
        public List<ExamIndent> ExamIndent { get; set; } = new List<ExamIndent>();
    }

    public class MaterialIndent
    {
        public Guid MaterialIndentTransID { get; set;}
    }

    public class ExamIndent
    {
        public Guid ExamIndentTransID { get; set; }
    }

    public class SplitMaterialIndent
    {
        public Guid MaterialIndentTransID { get; set; }
        public int IndentQuantity { get; set; }
    }

    public class RejectMaterialIndent
    {
        public Guid BranchMaterialIndentID { get; set; }
        public string Reason { get; set; } = string.Empty;
    }

    public class RejectExamIndent
    {
        public Guid ExamIndentID { get; set; }
        public string Reason { get; set; } = string.Empty;
    }

    public class GetMaterialDespatchList
    {
        public Guid TenantBranchID { get; set; }
        public Guid BranchMaterialIndentID { get; set; }
        public Guid MaterialIndentTransID { get; set; }
        public string MaterialName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }
        [JsonIgnore]
        public DateI18 ConfirmedDateI18 { get; set; } = new DateI18();
        public string ConfirmedDate { get { return ConfirmedDateI18.ToString() ?? string.Empty; } set { ConfirmedDateI18 = value; } }
    }

    public class MaterialDespatchList
    {
        public Guid TenantBranchID { get; set; }
        public Guid BranchMaterialIndentID { get; set; }
        public Guid MaterialIndentTransID { get; set; }
        public string MaterialName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }
        public string ConfirmedDate { get; set; } = string.Empty;
        public string FranchiseeID { get; set; } = string.Empty;
        public string FranchiseeName { get; set; } = string.Empty;
        public string Address1 { get; set; } = string.Empty;
        public string Address2 { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string Pincode { get; set; } = string.Empty;
        public string Taluk { get; set; } = string.Empty;
    }

    public class CreateDespatch
    {
        public Guid MaterialIndentTransID { get; set; }
        public Guid DeliveryModeID { get; set; }
        public string TrackingID { get; set; } = string.Empty;
    }

    public class NewDespatchMaterial
    {
        public List<CreateDespatch> DespatchMaterial { get; set; } = new List<CreateDespatch>();
    }

    public class GetReceivableList
    {
        public Guid MaterialIndentTransID { get; set; }
        public Guid BranchMaterialIndentID { get; set; }
        [JsonIgnore]
        public DateI18 IndentDateI18 { get; set; } = new DateI18();
        public string IndentDate { get { return IndentDateI18.ToString() ?? string.Empty; ; } set { IndentDateI18 = value; } }
        public string InvoiceNo { get; set; } = string.Empty;
        public string InvoiceDate { get; set; } = string.Empty;
        public string IndentNumber { get; set; } = string.Empty;
        public string MaterialType { get; set; } = string.Empty;
        public string MaterialName { get; set; } = string.Empty;
        public decimal Total { get; set; }
        [JsonIgnore]
        public DateI18 DespatchDateI18 { get; set; } = new DateI18();
        public string DespatchDate { get { return DespatchDateI18.ToString() ?? string.Empty; ; } set { DespatchDateI18 = value; } }
        public string DeliveryMode { get; set; } = string.Empty;
        public string TrackingID { get; set; } = string.Empty;
    }

    public class NewMaterialPendingList
    {
        public Guid StudentID { get; set; }
        public Guid BatchMappingID { get; set; }
        public Guid ContactID { get; set; }
        public string RollNumber { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public string Course { get; set; } = string.Empty;
        public string BatchTime { get; set; } = string.Empty;
        public int Total { get; set; }
        public int Issued { get; set; }
        public int Pending { get; set; }
        public string ShortName { get; set; } = string.Empty;
        public string CourseTypeName { get; set; } = string.Empty;
        public List<Materials> Materials { get; set; } = new List<Materials>();
    }

    public class LocalStockList
    {
        public Guid MaterialID { get; set; }
        public int CurrentStock { get; set; }
        public string MaterialType { get; set; } = string.Empty;
        public string MaterialName { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }

    public class NewMaterialRequiredList
    {
        public Guid MaterialID { get; set; }
        public Guid MaterialTypeID { get; set; }
        public string MaterialType { get; set; } = string.Empty;
        public string MaterialName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal GST { get; set; }
        public int ToIssue { get; set; }
        public int CurrentStock { get; set; }
        public int Required { get; set; }
    }

    public class MaterialPurchase
    {
        public Guid VendorID { get; set; }
        public string InvoiceNo { get; set; } = string.Empty;
        public DateTime InvoiceDate { get; set; }
        public List<MaterialInward> MaterialInwards { get; set; } = new List<MaterialInward>();
        public decimal Amount { get; set; }
        public decimal Discount { get; set; }
        public decimal GST { get; set; }
        public decimal RoundOff { get; set; } = 0;
        public string Notes { get; set; } = string.Empty;
    }

    public class MaterialInward
    {
        public Guid MaterialID { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal GST { get; set; } = 0;
    }

    public class PurchaseInvoiceList
    {
        public string BranchName { get; set; } = string.Empty;
        public BranchDetails BranchDetail { get; set; } = new BranchDetails();
        public MaterialPurchaseList MaterialPurchase { get; set; } = new MaterialPurchaseList();
        public List<MaterialPurchaseTransList> MaterialPurchaseTrans { get; set; } = new List<MaterialPurchaseTransList>();
    }

    public record VendorList
    {
        public Guid ContactID { get; set; }
        public Guid VendorID { get; set; }
        public string ContactName { get; set; } = string.Empty;
        public string TenantBranch { get; set; } = string.Empty;
        public string PhoneCode { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string VendorType { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string PanNumber { get; set; } = string.Empty;
        public decimal TDS { get; set; }
        public string SourceSupply { get; set; } = string.Empty;
        public string GSTTreatmentName { get; set; } = string.Empty;
        public Guid GSTTreatmentID { get; set; }
    }

    public class MaterialPurchaseTransList
    {
        public int SlNo { get; set; }
        public Guid MaterialPurchaseID { get; set; }
        public string MaterialName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Amount { get; set; }
        public decimal GST { get; set; }
        public decimal GSTAmount { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class MaterialPurchaseList
    {
        public Guid MaterialPurchaseID { get; set; }
        public Guid VendorID { get; set; }
        public string VendorName { get; set; } = string.Empty;
        public string InvoiceNo { get; set; } = string.Empty;
        public string InvoiceDate { get; set; } = string.Empty;
        public decimal GST { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Discount { get; set; }
        public decimal AfterDiscount { get; set; }
        public decimal RoundOff { get; set; }
        public string Notes { get; set; } = string.Empty;
    }

    public class BranchDetails
    {
        public string ContactName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string AlternatePhone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address1 { get; set; } = string.Empty;
        public string Address2 { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string Pincode { get; set; } = string.Empty;
        public string Taluk { get; set; } = string.Empty;
    }

    public class GetCurrentStock
    {
        public Guid MaterialID { get; set; }
        public string MaterialType { get; set; } = string.Empty;
        public string MaterialName { get; set; } = string.Empty;
        public int MinimumStock { get; set; }
        public decimal Price { get; set; }
        public int CurrentStock { get; set; }
    }

    public class MaterialRequired
    {
        public Guid MaterialID { get; set; }
        public string MaterialName { get; set; } = string.Empty;
        public int ToIssue { get; set; }
        public int CurrentStock { get; set; }
        public int Required { get; set; }
    }

    public class MaterialFolwReport
    {
        [JsonIgnore]
        public DateI18 InvoiceDateI18 { get; set; } = new DateI18();
        public string InvoiceDate { get { return InvoiceDateI18.ToString() ?? string.Empty; } set { InvoiceDateI18 = value; } }
        public string InvoiceNo { get; set; } = string.Empty;
        public string VendorOrFranchisee { get; set; } = string.Empty;
        public string MaterialName { get; set; } = string.Empty;
        public int PurchaseQuantity { get; set; }
        public decimal PurchaseAmount { get; set; }
        public int SalesQuantity { get; set; }
        public decimal SalesAmount { get; set; }
        public Guid BranchOrVendorID { get; set; }
    }

    public class FlowReportByFilter
    {
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
        public List<MaterialIDs> Materials { get; set; } = new List<MaterialIDs>();
    }

    public class MaterialIDs
    {
        public Guid MaterialID { get; set; }
    }

    public class MaterialIndentList
    {
        public string FranchiseeCode { get; set; } = string.Empty;
        public string FranchiseeName { get; set; } = string.Empty;
        public GetNewMaterialIndent IndentDetails { get; set; } = new GetNewMaterialIndent();
        public List<GetMaterialPurchaseTrans> IndentTransDetail { get; set; } = new List<GetMaterialPurchaseTrans>();
        public BranchDetails BranchDetail { get; set; } = new BranchDetails();
        public List<PaymentDetails> PaymentDetails { get; set; } = new List<PaymentDetails>();
    }

    public class GetMaterialIndent
    {
        [JsonIgnore]
        public DateI18 IndentDateI18 { get; set; } = new DateI18();
        public string IndentDate { get { return IndentDateI18.ToString() ?? string.Empty; } set { IndentDateI18 = value; } }
        public string IndentNo { get; set; } = string.Empty;
        public Guid BranchMaterialIndentID { get; set; }
        public Guid TenantBranchID { get; set; }
        public int TotalItem { get; set; }
        public int Ordered { get; set; }
        public int Despatched { get; set; }
        public int Pending { get; set; }
        public string Status { get; set; } = string.Empty;
        public int CourierAmount { get; set; }
    }

    public class GetNewMaterialIndent
    {
        public string IndentDate { get; set; } = string.Empty;
        public string IndentNo { get; set; } = string.Empty;
        public string InvoiceNo { get; set; } = string.Empty;
        public string InvoiceDate { get; set; } = string.Empty;
        public Guid BranchMaterialIndentID { get; set; }
        public Guid TenantBranchID { get; set; }
        public int TotalItem { get; set; }
        public int Ordered { get; set; }
        public int Despatched { get; set; }
        public int Pending { get; set; }
        public string Status { get; set; } = string.Empty;
        public int CourierAmount { get; set; }
    }

    public class GetMaterialPurchaseTrans
    {
        public Guid BranchMaterialIndentID { get; set; }
        public Guid MaterialIndentTransID { get; set; }
        public string MaterialType { get; set; } = string.Empty;
        public string MaterialName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal Total { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class CreateExamIndent
    {
        public Guid TenantBranchID { get; set; }
        public Guid CreatedBy { get; set; }
        public List<BatchMapping> BatchMapping { get; set; } = new List<BatchMapping>();
        public decimal TotalAmount { get; set; }
        public decimal CourierAmount { get; set; }
    }

    public class ExamIndentTransIDs
    {
        public Guid ExamIndentTransID { get; set; }
        public Guid BatchMappingID { get; set; }
        public int Semester { get; set; }
    }

    public class NewExamIndent
    {
        public List<BatchMapping> BatchMapping { get; set; } = new List<BatchMapping>();
        public decimal TotalAmount { get; set; }
        public decimal? CourierAmount { get; set; } = 0;
        public List<FranchiseeToAdminPay> PaymentItems { get; set; } = new List<FranchiseeToAdminPay>();
    }

    public class BatchMapping
    {
        public Guid BatchMappingID { get; set; }
        public decimal ExamFees { get; set; }
        public int Semester { get; set; }
        public string ExamDate { get; set; } = string.Empty;
    }

    public class ExamIndentResult
    {
        public Guid ExamIndentID { get; set; }
        public decimal TotalAmount { get; set; }
        public string IndentNumber { get; set; } = string.Empty;
    }

    public class GetExamIndent
    {
        public string FranchiseeCode { get; set; } = string.Empty;
        public string FranchiseeName { get; set; } = string.Empty;
        public BranchDetails BranchDetail { get; set; } = new BranchDetails();
        public ExamIndentDetail ExamIndentDetail { get; set; } = new ExamIndentDetail();
        //List<NewExamIndentTransDetail> ExamIndentTransDetails { get; set; } = new List<NewExamIndentTransDetail>();
        //public List<PaymentDetails> PaymentDetails { get; set; } = new List<PaymentDetails>();
    }
    public class GetExamIndentDetail
    {
        public List<NewExamIndentTransDetail> ExamIndentTransDetails { get; set; } = new List<NewExamIndentTransDetail>();
        public List<PaymentDetails> PaymentDetails { get; set; } = new();
    }
    public class ExamIndentDetail
    {
        public Guid ExamIndentID { get; set; }
        public Guid TenantBranchID { get; set; }
        [JsonIgnore]
        public DateI18 IndentDateI18 { get; set; } = new DateI18();
        public string IndentDate { get { return IndentDateI18.ToString() ?? string.Empty; } set { IndentDateI18 = value; } }
        public string IndentNumber { get; set; } = string.Empty;
        public string InvoiceNo { get; set; } = string.Empty;
        public string InvoiceDate { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public int TotalItem { get; set; }
        public int ActiveItem { get; set; }
        public int InactiveItem { get; set; }
        public string Status { get; set; } = string.Empty;
        public int CourierAmount { get; set; }
    }

    public class NewExamIndentDetail : ExamIndentDetail
    {
        public bool IsRejected { get; set; }
        public string Reason { get; set; } = string.Empty;
    }

    public class ExamIndentTransDetail
    {
        public Guid ExamIndentID { get; set; }
        public Guid ExamIndentTransID { get; set; }
        public Guid StudentBatchMapping { get; set; }
        public int Semester { get; set; }
        public decimal ExamFees { get; set; }
        public string Status { get; set; } = string.Empty;
        [JsonIgnore]
        public DateI18 ActivateDateI18 { get; set; } = new DateI18();
        public string ActivateDate { get { return ActivateDateI18 ?? string.Empty; } set { ActivateDateI18 = value; } }
    }

    public class NewExamIndentTransDetail
    {
        public Guid ExamIndentID { get; set; }
        public Guid ExamIndentTransID { get; set; }
        public Guid StudentBatchMapping { get; set; }
        public string Semester { get; set; } = string.Empty;
        public decimal ExamFees { get; set; }
        public string Status { get; set; } = string.Empty;
        public string ActivateDate { get; set; } = string.Empty;
        public string StudentID { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public string Course { get; set; } = string.Empty;
        public string BatchTime { get; set; } = string.Empty;
        public string ShortName { get; set; } = string.Empty;
        public string CourseTypeName { get; set; } = string.Empty;
    }

    public class GetExamIndentByFranchisee
    {
        public NewExamIndentDetail ExamIndentDetail { get; set; } = new NewExamIndentDetail();
        public List<NewExamIndentTransDetail> ExamIndentTransDetails { get; set; } = new List<NewExamIndentTransDetail>();
        public List<PaymentDetails> PaymentDetails { get; set; } = new List<PaymentDetails>();
    }

    public class GetExamIndentTransID
    {
        public Guid ExamIndentTransID { get; set; }
        public Guid StudentBatchMapping { get; set; }
        public int Semester { get; set; }
    }

    public class CertificateIndentInfo
    {
        public Guid ExamIndentTransID { get; set; }
        public int Semester { get; set; }
        public Guid StudentBatchMapping { get; set; }
    }

    public class CertificateIndentList
    {
        public Guid ExamIndentTransID { get; set; }
        public string IndentNumber { get; set; } = string.Empty;
        public string IndentDate { get; set; } = string.Empty;
        public string DespatchDate { get; set; } = string.Empty;
        public string ReceivedDate { get; set; } = string.Empty;
        public string IssuedDate { get; set; } = string.Empty;
        public string RollNumber { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public string Course { get; set; } = string.Empty;
        public string BatchNumber { get; set; } = string.Empty;
        public string BatchTiming { get; set; } = string.Empty;
        public string Semester { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string DeliveryMode { get; set; } = string.Empty;
        public string TrackingID { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string FacultyName { get; set; } = string.Empty;
        public string ShortName { get; set; } = string.Empty;
        public string CourseTypeName { get; set; } = string.Empty;
        public Guid CourseTypeID { get; set; }
        public List<SubjectMarkList> MarkInfo { get; set; } = new List<SubjectMarkList>();
    }

    public class CertificateIssuedList
    {
        public Guid ExamIndentTransID { get; set; }
        public string IndentNumber { get; set; } = string.Empty;
        public string IndentDate { get; set; } = string.Empty;
        public string IssuedDate { get; set; } = string.Empty;
        public string RollNumber { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public string Course { get; set; } = string.Empty;
        public string BatchNumber { get; set; } = string.Empty;
        public string BatchTiming { get; set; } = string.Empty;
        public string Semester { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string FacultyName { get; set; } = string.Empty;
    }

    public class SubjectMarkList
    {
        public Guid ExamIndentTransID { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public Guid SubjectMappingID { get; set; }
        public int TheoryMarks { get; set; }
        public int PracticalMarks { get; set; }
        public int ObtainedTheoryMarks { get; set; }
        public int ObtainedPracticalMarks { get; set; }
    }

    public class GetCertificateIndentList
    {
        public Guid CertificateIndentID { get; set; }
        [JsonIgnore]
        public DateI18 DespatchDateI18 { get; set; } = new DateI18();
        public string DespatchDate { get { return DespatchDateI18.ToString() ?? string.Empty; } set { DespatchDateI18 = value; } }
        [JsonIgnore]
        public DateI18 IndentDateI18 { get; set; } = new DateI18();
        public string IndentDate { get { return IndentDateI18.ToString() ?? string.Empty; } set { IndentDateI18 = value; } }
        public string IndentNumber { get; set; } = string.Empty;
        public Guid ExamIndentTransID { get; set; }
        [JsonIgnore]
        public DateI18 ReceivedDateI18 { get; set; } = new DateI18();
        public string ReceivedDate { get { return ReceivedDateI18.ToString() ?? string.Empty; } set { ReceivedDateI18 = value; } }
        [JsonIgnore]
        public DateI18 IssuedDateI18 { get; set; } = new DateI18();
        public string IssuedDate { get { return IssuedDateI18.ToString() ?? string.Empty; } set { IssuedDateI18 = value; } }
        public string Type { get; set; } = string.Empty;
        public string DeliveryModeID { get; set; } = string.Empty;
        public string TrackingID { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }

    public class MaterialIndentByFranchisee
    {
        public Guid BranchMaterialIndentID { get; set; }
        public string IndentNumber { get; set; } = string.Empty;
        [JsonIgnore]
        public DateI18 IndentDateI18 { get; set; } = new DateI18();
        public string IndentDate { get { return IndentDateI18.ToString() ?? string.Empty; } set { IndentDateI18 = value; } }
        public string InvoiceNo { get; set; } = string.Empty;
        public string InvoiceDate { get; set; } = string.Empty;
        public int Total { get; set; }
        public int Ordered { get; set; }
        public int Received { get; set; }
        public int Despatched { get; set; }
        public int Pending { get; set; }
        public decimal Amount { get; set; }
        public int CourierAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public int Rejected { get; set; }
    }

    public class MaterialIndentTransByFranchisee
    {
        public Guid BranchMaterialIndentID { get; set; }
        public Guid MaterialIndentTransID { get; set; }
        public string DeliveryModeID { get; set; }
        public string MaterialType { get; set; } = string.Empty;
        public string DeliveryMode { get; set; } = string.Empty;
        public string TrackingID { get; set; } = string.Empty;
        public string MaterialName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal GST { get; set; }
        public decimal GSTPrice { get; set; }
        public decimal Total { get; set; }
        [JsonIgnore]
        public DateI18 DespatchDateI18 { get; set; } = new DateI18();
        public string DespatchDate { get { return DespatchDateI18.ToString() ?? string.Empty; } set { DespatchDateI18 = value; } }
        [JsonIgnore]
        public DateI18 ReceivedDateI18 { get; set; } = new DateI18();
        public string ReceivedDate { get { return ReceivedDateI18.ToString() ?? string.Empty; } set { ReceivedDateI18 = value; } }
        public string Status { get; set; } = string.Empty;
    }
    public class MaterialIndentListByFranchisee
    {
        public MaterialIndentByFranchisee Indent { get; set; } = new MaterialIndentByFranchisee();
        public List<MaterialIndentTransByFranchisee> IndentTrans { get; set; } = new List<MaterialIndentTransByFranchisee>();
        public List<PaymentDetails> PaymentDetails { get; set; } = new List<PaymentDetails>();
    }

    public class GetTotalPurchase
    {
        public decimal Books { get; set; }
        public decimal Exam { get; set; }
    }

    public class PurchaseAmount
    {
        public Guid TenantBranchID { get; set; }
        public decimal Total { get; set; }
    }

    public class IndentFilter {
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
        public string Status { get; set; }
    }

    public class GetMaterialIndentIDs {
        public Guid MaterialIndentID { get; set; }
    }

    public class GetMaterialInvoice {
        public string InvoiceNo { get; set; } = string.Empty;
        [JsonIgnore]
        public DateI18 InvoiceDateI18 { get; set; } = new DateI18();
        public string InvoiceDate { get { return InvoiceDateI18.ToString() ?? string.Empty; } set { InvoiceDateI18 = value; } }
        public decimal CGST { get; set; }
        public decimal SGST { get; set; }
        public decimal IGST { get; set; }
        public decimal TotalAmount { get; set; }
        public string TotalInWords { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneCode { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string BranchName { get; set; } = string.Empty;
        public GetBranchAddress BillingTo { get; set; } = new GetBranchAddress();
        public GetBranchAddress ShippingTo { get; set; } = new GetBranchAddress();
        public GetBranchAddress AdminAddress { get; set; } = new GetBranchAddress();
        public List<GetMaterialIndentInfo> IndentDetails { get; set; } = new List<GetMaterialIndentInfo>();
    }

    public class GetMaterialIndentInfo {
        public int SlNo { get; set; }
        public string MaterialType { get; set; } = string.Empty;
        public string MaterialName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal TaxableValue { get; set; }
        public decimal GSTPrice { get; set; }
        public decimal Total { get; set; }
    }

    public class InvoicePaymentDetails {
        public string PayMode { get; set; } = string.Empty;
        public string TransactionNo { get; set; } = string.Empty;
        [JsonIgnore]
        public DateI18 TransactionDateI18 { get; set; } = new DateI18();
        public string TransactionDate { get { return TransactionDateI18.ToString() ?? string.Empty; } set { TransactionDateI18 = value; } }
        public string BankName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }

    public class GetBranchAddress {
        public string Address1 { get; set; } = string.Empty;
        public string Address2 { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string Pincode { get; set; } = string.Empty;
    }

    public class InvoiceCreate {
        public Guid IndentID { get; set; }
        public string Address { get; set; } = string.Empty;
        public decimal CGST { get; set; }
        public decimal SGST { get; set; }
        public decimal IGST { get; set; }
    }

    public class EditMaterial {
        public Guid MaterialID { get; set; }
        public Guid MaterialTypeID { get; set; }
        public string MaterialName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int MinimumStock { get; set; }
        public int OpeningStock { get; set; }
        public decimal SGST { get; set; }
        public decimal CGST { get; set; }
    }
    public class WalletPayment
    {
        public Guid TenantBranchID { get; set; }
        public Guid CreatedBy { get; set; }
        public decimal WalletAmount { get; set; }
        public Guid SenderID { get; set; }
        public Guid ReceiverID { get; set; }
        public string Description { get; set; } = string.Empty;
        public string WalletType { get; set; } = string.Empty;
    }
    public class IndentReceipt
    {
        public string FranchiseeCode { get; set; } = string.Empty;
        public string BranchName { get; set; } = string.Empty;
        public string Address1 { get; set; } = string.Empty;
        public string Address2 { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public string Taluk { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string IndentType { get; set; } = string.Empty;
        public string PayModes { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string AmountInRupess { get; set; } = string.Empty;
        public string VoucherNo { get; set; } = string.Empty;
        public string VoucherDate { get; set; } = string.Empty;
        public int CourierAmount { get; set; }
        [JsonProperty(Order = 15)]
        public List<GetCourse> ExamDetails = new List<GetCourse>();
        [JsonProperty(Order = 16)]
        public List<GetMaterialPurchaseDetails> MaterialsDetails = new List<GetMaterialPurchaseDetails>();
        [JsonProperty(Order = 17)]
        public AdminContact AdminDetails { get; set; } = new AdminContact();
    }
    public class GetMaterialPurchaseDetails
    {
        public Guid BranchMaterialIndentID { get; set; }
        public Guid MaterialIndentTransID { get; set; }
        public Guid TenantBranchID { get; set; }
        public string MaterialType { get; set; } = string.Empty;
        public string MaterialName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal MaterialAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public int CGST { get; set; }
        public int SGST { get; set; }
        public int CourierAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public int SlNo { get; set; }
    }
    public class GetExamIndentDetails
    {
        public Guid ExamIndentID { get; set; }
        public Guid TenantBranchID { get; set; }
        public Guid ExamIndentTransID { get; set; }
        public Guid StudentBatchMapping { get; set; }
        public decimal TotalAmount { get; set; }
        public int CourierAmount { get; set; }
        public decimal ExamFees { get; set; }
    }
    public class ReprintIndent
    {
        public List<CertificateIndent> Reprint { get; set; } = new List<CertificateIndent>();
        public decimal TotalAmount { get; set; }
        public decimal CourierAmount { get; set; } = 0;
        public List<FranchiseeToAdminPay> PaymentItems { get; set; } = new List<FranchiseeToAdminPay>();
    }
    public class IDCard
    {
        public Guid BatchMappingID { get; set; }
    }
    public class IDCardIndent
    {
        public List<IDCard> IDCard { get; set; } = new List<IDCard>();
        public decimal TotalAmount { get; set; }
        public decimal CourierAmount { get; set; } = 0;
        public List<FranchiseeToAdminPay> PaymentItems { get; set; } = new List<FranchiseeToAdminPay>();
    }
    public class IDCardIndented
    {
        public Guid IDCardIndentID { get; set; }
        public string IndentNumber { get; set; } = string.Empty;
        public string TotalAmount { get; set; } = string.Empty;

    }
    public class IDCardIndentList
    {
        public int Slno { get; set; }
        public Guid IDCardIndentID { get; set; }    
        public string IndentNumber { get; set; } = string.Empty;
        public int TotalIndent { get; set; }
        public int Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        public int Pending { get; set; }
        public int Printed { get; set; }
        public int Despatched { get; set; }
        public int Rejected { get; set; }
        public int Received { get; set; }
        [JsonIgnore]
        public DateI18 IndentDateI18 { get; set; } = new DateI18();
        public string IndentDate { get { return IndentDateI18.ToString() ?? string.Empty; } set { IndentDateI18 = value; } }
    }
    public class IDCardIndentDetails
    {
        public Guid IDCardIndentID { get; set; }
        public string IndentNumber { get; set; } = string.Empty;
        public int TotalIndent { get; set; }
        public int IndentAmount { get; set; }
        public int CourierAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        [JsonIgnore]
        public DateI18 IndentDateI18 { get; set; } = new DateI18();
        public string IndentDate { get { return IndentDateI18.ToString() ?? string.Empty; } set { IndentDateI18 = value; } }
        public List<IDCardStudentInfo> StudentInfo { get; set; } = new List<IDCardStudentInfo>();
        public List<PaymentDetails> PaymentDetails { get; set; } = new List<PaymentDetails>();
    }
    public class IndentID
    {
        [IsGuidEmpty]
        public Guid IDCardIndentID { get; set; }
    }
    public class IndentTransID
    {
        public Guid IDCardIndentTransID { get; set; }
    }
    public class IDCardIndentDetailsList
    {
        public Guid IDCardIndentTransID { get; set; }
        public Guid IDCardIndentID { get; set; }
        public Guid BatchMappingID { get; set; }
        public string Status { get; set; } = string.Empty;
    }
    public class IDCardStudentInfo
    {
        public Guid IDCardIndentTransID { get; set; }
        public Guid BatchMappingID { get; set; }
        public Guid ContactID { get; set; }
        public Guid StudentID { get; set; }
        public string RollNumber { get; set; } = string.Empty;
        public string Course { get; set; } = string.Empty;
        public string BatchNumber { get; set; } = string.Empty;
        public string BatchTime { get; set; } = string.Empty;
        public string ShortName { get; set; } = string.Empty;
        public string CourseTypeName { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
    public class IDCardStudentInfoUpdated : IDCardStudentInfo
    {
        public int IDCardFees { get; set; }
    }
    public class IDCardListByAdmin
    {
        public Guid IDCardIndentID { get; set; }
        public Guid TenantBranchID { get; set; }
        public string IndentNumber { get; set; } = string.Empty;
        public int TotalIndent { get; set; }
        public int Amount { get; set; }
        public int IndentAmount { get; set; }
        public int CourierAmount { get; set; }
        [JsonIgnore]
        public DateI18 IndentDateI18 { get; set; } = new DateI18();
        public string IndentDate { get { return IndentDateI18.ToString() ?? string.Empty; } set { IndentDateI18 = value; } }
        public int Pending { get; set; }
        public int Printed { get; set; }
        public int Despatched { get; set; }
        public int Rejected { get; set; }
        public int Received { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool IsApproved { get; set; }
        public FranchiseeDetailsForIDCard franchiseeDetails { get; set; } = new FranchiseeDetailsForIDCard();
    }
    public class IDcardIndentNumber
    {
        public Guid IDCardIndentID { get; set; }
        public Guid TenantBranchID { get; set; }
        public string IndentNumber { get; set; } = string.Empty;
        public int IndentAmount { get; set; }
        public int CourierAmount { get; set; }
        [JsonIgnore]
        public DateI18 IndentDateI18 { get; set; } = new DateI18();
        public string IndentDate { get { return IndentDateI18.ToString() ?? string.Empty; } set { IndentDateI18 = value; } }
        public List<IDCardStudentInfoUpdated> StudentInfo { get; set; } = new List<IDCardStudentInfoUpdated>();
        public List<PaymentDetails> PaymentDetails { get; set; } = new List<PaymentDetails>();
        public GetBranchContactDetail franchiseeDetails { get; set; } = new GetBranchContactDetail();
    }
    public class IDCardStudentInfoMore: IDCardStudentInfo
    {
        public int IDCardFees { get; set; }
    }
    public class IDCardIndentDetailsListForAdmin
    {
        public Guid IDCardIndentTransID { get; set; }
        public Guid IDCardIndentID { get; set; }
        public Guid BatchMappingID { get; set; }
        public string Status { get; set; } = string.Empty;
        public string IndentNumber { get; set; } = string.Empty;
        [JsonIgnore]
        public DateI18 IndentDateI18 { get; set; } = new DateI18();
        public string IndentDate { get { return IndentDateI18.ToString() ?? string.Empty; } set { IndentDateI18 = value; } }
        public int IDCardFees { get; set; }

    }
    public class IDCardPrinted
    {
        public Guid IDCardIndentID { get; set; }
        public List<IndentTransID> TransIDs { get; set; } = new List<IndentTransID>();
    }
    public class DespatchList
    {
        public Guid IDCardIndentID { get; set; }
        public Guid TenantBranchID { get; set; }
        public string IndentNumber { get; set; } = string.Empty;
        public int IndentFees { get; set; }
        public int TotalIndent { get; set; }
        public int Printed { get; set; }
        [JsonIgnore]
        public DateI18 IndentDateI18 { get; set; } = new DateI18();
        public string IndentDate { get { return IndentDateI18.ToString() ?? string.Empty; } set { IndentDateI18 = value; } }
        [JsonIgnore]
        public DateI18 PrintedDateI18 { get; set; } = new DateI18();
        public string PrintedDate { get { return PrintedDateI18.ToString() ?? string.Empty; } set { PrintedDateI18 = value; } }
        public FranchiseeDetailsForIDCard franchiseeDetails { get; set; } = new FranchiseeDetailsForIDCard();
    }
    public class DespatchIDCard
    {
        public Guid IDCardIndentID { get; set; }
        public Guid DeliveryModeID { get; set; }
        public string TrackingID { get; set; } = string.Empty;
    }

    public class NewDespatchIDCard
    {
        public List<DespatchIDCard> DespatchMaterial { get; set; } = new List<DespatchIDCard>();
    }
}
