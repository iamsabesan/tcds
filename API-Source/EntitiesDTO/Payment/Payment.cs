using Dawn.Utilities.i18N;
using EntitiesDTO.Admin;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Realtor.Utilities;
using System.ComponentModel.DataAnnotations;
using Utilities.Attributes;

namespace EntitiesDTO.Payment
{
    public class GetPaymode
    {
        public Guid PaymodeID { get; set; }
        public string Paymode { get; set; } = string.Empty;
    }

    public class Receipt
    {
        public Guid TenantBranchID { get; set; }
        public Guid KeyValue { get; set; }
        public string KeyTable { get; set; } = string.Empty;
        public string KeyColumn { get; set; } = string.Empty;
        public string TransactionName { get; set; } = string.Empty;
        public List<NewPaymentItems> PaymentItems { get; set; } = new List<NewPaymentItems>();
        public Guid CreatedBy { get; set; }
        public string VoucherDate { get; set; } = string.Empty;
    }

    public class NewReceipt
    {
        public Guid TenantBranchID { get; set; }
        public Guid KeyValue { get; set; }
        public Guid ContactID { get; set; }
        public string KeyTable { get; set; } = string.Empty;
        public string KeyColumn { get; set; } = string.Empty;
        public string TransactionName { get; set; } = string.Empty;
        public List<NewPaymentItems> PaymentItems { get; set; } = new List<NewPaymentItems>();
        public Guid CreatedBy { get; set; }
        public string VoucherDate { get; set; } = string.Empty;
    }

    public class NewPaymentItems
    {
        public Guid PaymentModeID { get; set; }
        public string PayModeType { get; set; } = string.Empty;
        public string? BankName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public decimal? RoundOff { get; set; } = 0;
        public string? TransactionNo { get; set; } = string.Empty;
        public DateTime TransactionDate { get; set; }
        public decimal? BankCommission { get; set; } = 0;
        public string PaymentDetails { get; set; } = string.Empty;
    }

    public class PaymentItems
    {
        public Guid PaymentModeID { get; set; }
        public string? PayModeType { get; set; }
        public decimal PayModeValue { get; set; }
        public decimal Amount { get; set; }
        public string LedgerID { get; set; } = string.Empty;
        public string LedgerName { get; set; } = string.Empty;
        public string TransactionNo { get; set; } = string.Empty;
        public string TransactionDate { get; set; } = string.Empty;
        public Guid VoucherID { get; set; } = Guid.NewGuid();
    }

    public class PaymentItemsNew
    {
        public Guid PaymentModeID { get; set; }
        public string? PayModeType { get; set; }
        public decimal PayModeValue { get; set; }
        public decimal Amount { get; set; }
        public decimal? RoundOff { get; set; } = 0;
        public string TransactionNo { get; set; } = string.Empty;
        public DateTime TransactionDate { get; set; }
        public decimal? BankCommission { get; set; } = 0;
        public Guid LedgerID { get; set; }
        public string LedgerName { get; set; } = string.Empty;
        public Guid DocumentID { get; set; }
    }

    public class FranchiseeToAdminPay
    {
        public Guid PaymentModeID { get; set; }
        public string? PayModeType { get; set; }
        public decimal PayModeValue { get; set; }
        public decimal Amount { get; set; }
        public decimal? RoundOff { get; set; } = 0;
        public string TransactionNo { get; set; } = string.Empty;
        public DateTime TransactionDate { get; set; }
        public decimal? BankCommission { get; set; } = 0;
        public string FromLedgerName { get; set; } = string.Empty;
        public Guid FromLedgerID { get; set; } = Guid.NewGuid();
        public Guid ToLedgerID { get; set; }
        public string ToLedgerName { get; set; } = string.Empty;
        public Guid DocumentID { get; set; }
    }

    public class RejectWallet
    {
        public Guid WalletID { get; set; }
        public string Reason { get; set; } = string.Empty;
    }

    public class ApproveWalletRequest
    {
        public Guid WalletID { get; set; }
    }

    public class GetWalletHistory
    {
        [JsonIgnore]
        public DateI18 TransactionDateI18 { get; set; } = new DateI18();
        public string TransactionDate { get { return TransactionDateI18.ToString() ?? string.Empty; } set { TransactionDateI18 = value; } }
        public string Description { get; set; } = string.Empty;
        public decimal AmountIn { get; set; }
        public decimal AmountOut { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal Balance { get; set; }
    }

    public class GetNewWalletHistory
    {
        public GetWalletOpeningBalance OpeningBalance { get; set; } = new GetWalletOpeningBalance();
        public GetWalletBalance WalletBalance { get; set; } = new GetWalletBalance();
        public List<GetWalletHistory> WalletHistory { get; set; } = new List<GetWalletHistory>();
    }

    public class GetWalletBalance
    {
        public decimal WalletBalance { get; set; }
    }
    public class GetWalletOpeningBalance
    {
        public decimal OpeningBalance { get; set; }
    }

    public class WalletTopup
    {
        public Guid TenantBranchID { get; set; }
        public Guid SenderID { get; set; }
        public Guid ReceiverID { get; set; }
        public decimal WalletAmount { get; set; }
        public List<NewPaymentItems> PaymentItems { get; set; } = new List<NewPaymentItems>();
        public Guid ContactID { get; set; }
        public string TransactionType { get; set; } = string.Empty;
        public decimal WalletOut { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string TransactionName { get; set; } = string.Empty;
        public string VoucherDate { get; set; } = string.Empty;
}

    public class NewWalletTopup
    {
        public decimal WalletAmount { get; set; }
        public List<FranchiseeToAdminPay> PaymentItems { get; set; } = new List<FranchiseeToAdminPay>();
    }

    public class GetWalletRequestList
    {
        public Guid WalletID { get; set; }
        [JsonIgnore]
        public DateI18 RequestDateI18 { get; set; } = new DateI18();
        public string Date { get { return RequestDateI18.ToString() ?? string.Empty; } set { RequestDateI18 = value; } }
        public decimal Amount { get; set; }
        public GetBranchContactDetail BranchDetail { get; set; } = new GetBranchContactDetail();
        public decimal WalletBalance { get; set; }
        public string BranchCode { get; set; } = string.Empty;
        public List<WalletPaymentDetail> PaymentDetail { get; set; } = new List<WalletPaymentDetail>();
    }

    public class WalletPaymentDetail
    {
        [JsonIgnore]
        public DateI18 TransactionDateI18 { get; set; } = new DateI18();
        public string TransactionDate { get { return TransactionDateI18.ToString() ?? string.Empty; } set { TransactionDateI18 = value; } }
        public string Summary { get; set; } = string.Empty;
        public string PayMode { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public Guid WalletID { get; set; }
        public Guid TransID { get; set; }
        public Guid DocumentID { get; set; }
    }

    public class GetWalletID
    {
        public Guid WalletID { get; set; }
        public Guid TenantBranchID { get; set; }
        [JsonIgnore]
        public DateI18 DateI18 { get; set; } = new DateI18();
        public string Date { get { return DateI18.ToString() ?? string.Empty; } set { DateI18 = value; } }
        public decimal Amount { get; set; }
    }

    public class DownPayment
    {
        public Guid TenantBranchID { get; set; }
        public List<NewPaymentItems> PaymentItems { get; set; } = new List<NewPaymentItems>();
        public Guid KeyValue { get; set; }
        public Guid CreatedBy { get; set; }
        public string TransactionName { get; set; } = string.Empty;
        public decimal WalletAmount { get; set; }
        public Guid SenderID { get; set; }
        public Guid ReceiverID { get; set; }
        public string Description { get; set; } = string.Empty;
    }

    public class PartPayment
    {
        public Guid TenantBranchID { get; set; }
        public List<NewPaymentItems> PaymentItems { get; set; } = new List<NewPaymentItems>();
        public Guid KeyValue { get; set; }
        public Guid CreatedBy { get; set; }
        public string TransactionName { get; set; } = string.Empty;
    }

    public class StudentPaymentDetails
    {
        public Guid TenantBranchTransID { get; set; }
        public Guid TransID { get; set; }
        [JsonIgnore]
        public DateI18 ReceiptDateI18 { get; set; } = new DateI18();
        public string ReceiptDate { get { return ReceiptDateI18.ToString() ?? string.Empty; } set { ReceiptDateI18 = value; } }
        public string ReceiptNo { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public decimal ProgressPaid { get; set; }
        public decimal Balance { get; set; }
        public string PaymentType { get; set; } = string.Empty;
        public string Paymode { get; set; } = string.Empty;
        public Guid KeyValue { get; set; }
        public string Summary { get; set; } = string.Empty;
        public Guid DocumentID { get; set; }
    }

    public class PaymentDetails
    {
        [JsonIgnore]
        public DateI18 TransactionDateI18 { get; set; } = new DateI18();
        public string TransactionDate { get { return TransactionDateI18.ToString() ?? string.Empty; } set { TransactionDateI18 = value; } }
        public string Summary { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string PayMode { get; set; } = string.Empty;
        public Guid DocumentID { get; set; }
        public Guid TransID { get; set; }
    }
    public class PaymentDetailsBulk
    {
        [JsonIgnore]
        public DateI18 TransactionDateI18 { get; set; } = new DateI18();
        public string TransactionDate { get { return TransactionDateI18.ToString() ?? string.Empty; } set { TransactionDateI18 = value; } }
        public string Summary { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string PayMode { get; set; } = string.Empty;
        public Guid DocumentID { get; set; }
        public Guid TransID { get; set; }
        public Guid KeyValue { get; set; }
    }
    public class WalletSummary
    {
        public int WalletPending { get; set; }
        public decimal WalletBalance { get; set; }
    }

    public class RejectPayment
    {
        public Guid PackageLogID { get; set; }
        public string Reason { get; set; } = string.Empty;
    }

    public class InvoiceMaster
    {
        public Guid TenantBranchID { get; set; }
        public Guid TenantBranchTransID { get; set; }
        public string VoucherNo { get; set; } = string.Empty;
        public Guid KeyValue { get; set; }
        public string VoucherDate { get; set; } = string.Empty;
        public string TransactionName { get; set; } = string.Empty;
    }

    public class NewInvoiceMaster : InvoiceMaster
    {
        public List<InvoiceTrans> PaymentDetails { get; set; } = new List<InvoiceTrans>();
    }

    public class InvoiceTrans
    {
        public Guid TenantBranchTransID { get; set;}
        public string PayMode { get; set; } = string.Empty;
        public string BankName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public decimal RoundOff { get; set; }
        public decimal BankCommission { get; set; }
        public string TransactionDate { get; set; } = string.Empty;
        public string TransactionNo { get; set; } = string.Empty;
    }

    public class GetPaymentDetails
    {
        public string PaymentDetails { get; set; } = string.Empty;
    }

    public class GetPaymentID
    {
        public Guid TenantBranchID { get; set; }
        public Guid TenantBranchTransID { get; set; }
    }

    public class GetAdmissionFeesList
    {
        public Guid TenantBranchID { get; set; }
        public decimal Amount { get; set; }
    }

    public class WalletRenewalPayment
    {
        public string WalletType { get; set; } = string.Empty;
        public Guid TenantBranchID { get; set; }
        public Guid SenderID { get; set; }
        public Guid ReceiverID { get; set; }
        public decimal WalletOut { get; set; }
        public string Description { get; set; } = string.Empty;
        public Guid CreatedBy { get; set; }
    }

    public class InvoiceDetail {
        public Guid InvoiceID { get; set; }
        public Guid IndentID { get; set; }
        public string InvoiceNo { get; set; } = string.Empty;
        public string BillingAddress { get; set; } = string.Empty;
        public string ShippingAddress { get; set; } = string.Empty;
        [JsonIgnore]
        public DateI18 InvoiceDateI18 { get; set; } = new DateI18();
        public string InvoiceDate { get { return InvoiceDateI18.ToString() ?? string.Empty; } set { InvoiceDateI18 = value; } }
        public decimal CGST { get; set; }
        public decimal SGST { get; set; }
        public decimal IGST { get; set; }
    }

    public class ImageDocument {
        [Required]
        [AllowedExtensions(new string[] { ".png", ".jpg", ".jpeg", ".pdf" })]
        [MaxFileSize(2, ErrorMessage = "The maximum upload size allowed is 2MB")]
        public IFormFile DocumentFile { get; set; }
        [Required]
        public string EntityName { get; set; }
    }

    public class DocumentResponce {
        public Guid DocumentID { get; set; }
        public string DocumentType { get; set; }
        public string FileName { get; set; }
        public string MIMEType { get; set; }
        public string FileExtension { get; set; }
        public bool IsActivated { get; set; }
    }

    public class TempDocumentActivate {
        [Required]
        public List<DocumentItem> DocumentItem { get; set; }
    }

    public class DocumentItem {
        [IsGuidEmpty]
        public Guid DocumentID { get; set; }

        [IsGuidEmpty]
        public Guid EntityID { get; set; }
    }
    public class WalletHistory
    {
        public List<Guid> TenantBranchID { get; set; } = new List<Guid>();
        public DateTime Fromdate { get; set; }
        public DateTime Todate { get; set; }
    }
    public class GetWalletHistoryV1: GetWalletHistory
    {
        public Guid TenantBranchID { get; set; }
    }
    public class GetNewWalletHistoryV1
    {
        public Guid TenantBranchID { get; set; }
        public GetBranchContactDetail BranchDetail { get; set; } = new GetBranchContactDetail();
        public List<GetWalletHistoryV1> WalletHistory { get; set; } = new List<GetWalletHistoryV1>();
    }
}
