namespace EntitiesDTO.Utilities
{
    public static class TCDSServiceURL
    {
        
    }

    public static class DawnServiceURL
    {
        public static class TenantBranchService {
            public static string CreateBranchURL = "tenant-branch-service/tenantbranch/create";
            public static string GetHomeBranchDetails = "tenant-branch-service/tenantbranch/home-branch-detail";
            public static string TenantBranchContact = "tenant-branch-service/tenantbranch/tenant-branch-contact";
            public static string GetDocumentByContact = "tenant-branch-service/tenantbranch/getDocumentByContact?ContactID=";
            public static string RollBackTenantBranch = "tenant-branch-service/tenantbranch/remove-tenant-branch?TenantBranchID=";
            public static string GetActiveFranchisee = "tenant-branch-service/tenantbranch/active-franchisee-contact?=";
            public static string GetBranchName = "tenant-branch-service/tenantbranch/branch-name?tenantBranchID=";
            public static string GetBranchAndContactByID = "tenant-branch-service/tenantbranch/branch-contact-by-id?tenantBranchID=";
            public static string GetBranchDetails = "tenant-branch-service/tenantbranch/branch-detail?tenantBranchID=";
            public static string DocumentUpload = "/tenant-branch-service/tenantbranch/upload-document";
            public static string RollBackDocument= "tenant-branch-service/tenantbranch/delete-document?ContactID=";
            public static string BranchWiseContactInfo = "tenant-branch-service/tenantbranch/contact-details";
            public static string GetBranchWiseContactInfo = "tenant-branch-service/tenantbranch/contact-details";
            public static string GetAdminBranchID = "/tenant-branch-service/tenantbranch/home-branch-id?tenantID=";
        }

        public static class UserService {
            public static string CreateFranchisee = "user-service/user/create-franchisee";
            public static string CreateStudent = "user-service/v1/audit/user/create-student";
            public static string RollBackUser = "user-service/user/remove-user?UserID=";
            public static string GetContactID = "user-service/user/getContactID?UserID=";
            public static string GetUserID = "user-service/user/getUserID?ContactID=";
            public static string GetVendorList = "/user-service/user/vendor-list";
            public static string LockFranchisee = "/user-service/user/status/update";
            public static string GetTenantUserList = "/user-service/user/tenant-user-list";
            public static string IsEmailCheck = "/user-service/user/isEmailExist";
            public static string UserCreate = "/user-service/v1/audit/user/create-user";
            public static string IsMobileCheck = "/user-service/user/isMobileExist";
        }

        public class DocumentService
        {
            public static string UploadDocument = "/document-service/document/save";
            public static string RollbackDocument = "/document-service/document/remove";
            public static string Base64Document = "/document-service/document/get-base64";
            public static string ThumbnailDocument = "/document-service/document/thumbnail-base64";
            public static string InterimDocument = "/document-service/document/interim/save";
            public static string DocumentActivate = "/document-service/document/interim/activate";
            public static string GetDocumentID = "/document-service/document/get-document-id?PrimaryKeyID=";

            //Added on 27-03-2025
            public static string ProfileDocument = "/document-service/image/profile/image";
        }

        public static class ContactService {
            public static string CreateContact = "contact-service/contact/create";
            public static string AddressUpdate = "contact-service/contact/address-update";
            public static string RemoveContact = "contact-service/contact/remove-contact?contactID=";
            public static string RollBackContactAddress = "contact-service/contact/remove-contact-address?contactID=";
            public static string ContactInfo = "contact-service/contact/contact-info?contactID=";
            public static string GetAdminContactID = "contact-service/contact/admin-contact";
            public static string GetFranchiseeContactID = "/contact-service/contact/franchisee-admin?branchID=";
        }

        public static class AccountingService {
            public static string CustomerLedgerCreate = "accounting-service/CustomerLedgerCreate";
            public static string JournalPosting = "/accounting-service/TransactionJournalPosting";
            public static string FranchiseeToAdmin = "/accounting-service/transaction/different-branch";
            public static string AddBank = "/accounting-service/AddBankAccount";
            public static string GetAdminBank = "/accounting-service/admin/bank/ledger";
            public static string GetBranchBank = "/accounting-service/branch/bank/ledger?branchID=";
        }

        public static class InventoryService {
            public static string AllMaterialList = "icloud-inventory/v1/material/alllist";
            public static string GetExamDate = "icloud-inventory/v1/indent/examdate/";
            public static string GetExamIndentTransList = "icloud-inventory/v1/indent/examtrans/list";
            public static string ExamIndentTransIDs = "icloud-inventory/v1/indent/examindentids";
            public static string UpdateStudentTotalMark = "icloud-inventory/v1/indent/update/totalmark";
            public static string UpdateStudentEditedMark = "icloud-inventory/v1/indent/update/editedmark";
            public static string GetStudentCertificateIndentInfo = "icloud-inventory/v1/indent/certificate/info/";
            public static string GetCertificateList = "icloud-inventory/v1/indent/certificate/print/list";
            public static string GetExamIndentTransID = "icloud-inventory/v1/indent/examindenttransid/bybatchmappingid/";
            public static string GetBranchTotalPurchase = "icloud-inventory/v1/indent/branch/totalpurchase?tenantBranchID=";
            public static string GetStudentGrade = "icloud-inventory/v1/indent/grade/";
            public static string GetStudentSemesterCount = "icloud-inventory/v1/indent/student/semester/count/";
            public static string GetIndentSummaryByAdmin = "icloud-inventory/v1/indent/summary/byadmin";
            public static string GetPurchaseAmount = "icloud-inventory/v1/indent/purchase-amount/list";
            public static string GetIndentSummaryByFranchisee = "icloud-inventory/v1/indent/summary/byfranchisee";
            public static string GetLocalStockList = "icloud-inventory/v1/indent/localstock/list";
        }

        public static class PaymentService {
            public static string CreatePayment = "icloud-payment/v1/payment/create";
            public static string RollBackPayment = "icloud-payment/v1/payment/rollback/payment/";
            public static string RollBackPartPayment = "icloud-payment/v1/payment/rollback/part-payment/";
            public static string StudentDownPayment = "icloud-payment/v1/payment/downpayment";
            public static string StudentPartPayment = "icloud-payment/v1/payment/partpayment";
            public static string GetStudentPaymentDetails = "icloud-payment/v1/payment/studentpayment-detail";
            public static string GetStudentPaidFees = "icloud-payment/v1/payment/paidfees?batchMappingID=";
            public static string GetWalletBalance = "icloud-payment/v1/wallet/walletbalance/";
            public static string GetPaymentDetails = "icloud-payment/v1/payment/paymentdetails/";
            public static string GetBranchWalletTrans = "icloud-payment/v1/wallet/branch/transaction/";
            public static string GetWalletSummary = "icloud-payment/v1/wallet/summary";
            public static string RejectPaidRequest = "icloud-payment/v1/payment/reject/paid/request";
            public static string GetFranchiseePayments = "icloud-payment/v1/payment/franchisee/payments/";
            public static string GetPaymentID = "icloud-payment/v1/wallet/paymentid?keyValue=";
            public static string GetPaymentItems = "icloud-payment/v1/wallet/payment-details?keyValue=";
            public static string GetAdmissionFeesList = "icloud-payment/v1/wallet/admission-fees/list";
            public static string WalletRenewalPayment = "icloud-payment/v1/wallet/renewal/payment";
            public static string GetPaymentTransID = "icloud-payment/v1/payment/trans-id?tenantBranchTransID=";

        }

        public static class AcademicService {
            public static string GetCourseInfoList = "icloud-academic/v1/course/list/info";
            public static string GetCourseBasicInfo = "icloud-academic/v1/course/list/basicinfo";
            public static string GetCourseSemesterCount = "icloud-academic/v1/course/semester/count/";
            public static string GetCourseSummaryByAdmin = "icloud-academic/v1/course/summary/byadmin";
            public static string GetCourseSummaryByFranchisee = "icloud-academic/v1/course/summary/byfranchisee";
            public static string GetMblCourseList = "icloud-academic/v1/course/mobile-courses/";
        }

        public static class FranchiseeService {
            public static string GetFranchiseeCodeList = "icloud-franchisee/v1/franchisee/codes/list";
            public static string GetFranchiseeRankStatus = "icloud-franchisee/v1/franchisee/rank-status";
        }

        public static class AdmissionService {
            public static string MaterialIssuedList = "icloud-admission/v1/student/material/issued/list?fromDate=";
            public static string GetMaterialPendingList = "icloud-admission/v1/student/material/pending/list";
            public static string GetMaterialCount = "icloud-admission/v1/student/material/count/";
            public static string GetIssuedMaterialCount = "icloud-admission/v1/student/issued/material/count/";
            public static string GetMaterialIssuedCount = "icloud-admission/v1/student/material/issued/count/";
            public static string GetStudentBasicInfo = "icloud-admission/v1/student/basicinfo/list";
            public static string CreateCertificateAndMarkSheetIndent = "icloud-admission/v1/student/indent/certificate";
            public static string GetCertificateIndentList = "icloud-admission/v1/student/indent/certificate/list?fromDate=";
            public static string GetCertificateIssuedList = "icloud-admission/v1/student/certificate/issued/list?fromDate=";
            public static string GetStudentCourseInfo = "icloud-admission/v1/student/course/info/";
            public static string MaterialRequiredList = "icloud-admission/v1/student/material/required/list/";
        }

        public static class EmailService
        {
            public static string GetMailCount = "email/v1/email/count";
        }

        public static class SMSServiceURL {
            public static string VerifyOTP = "/sms-service/sms/VerifyOTP";
            public static string IsOTPVerified = "/sms-service/sms/IsOTPVerified";
            public static string IsVerifiedCheck = "/sms-service/sms/otp/is-verified/check";
            public static string OTPGenerate = "/sms-service/sms/GenerateOTP";
            public static string StudentPasswordSMS = "/sms-service/sms/SendCompasSMS";
        }

        public static class EmailServiceURL {
            public static string OTPGenerate = "/email-service/email/otp-send";
            public static string EmailSend = "/email-service/email/send";
        }
    }
}
