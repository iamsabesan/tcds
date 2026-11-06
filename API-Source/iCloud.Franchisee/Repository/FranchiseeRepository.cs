using Dawn.Database;
using Dawn.Utilities.Exceptions;
using Dawn.Utilities.Extensions;
using Dawn.Utilities.Gateway;
using Dawn.Utilities.i18N;
using Entities.Common;
using EntitiesDTO.Admin;
using EntitiesDTO.Email;
using EntitiesDTO.Material;
using EntitiesDTO.Payment;
using EntitiesDTO.Student;
using EntitiesDTO.Utilities;
using MassTransit.Caching;
using Microsoft.Data.SqlClient;
using System.Data;

namespace iCloud.Franchisee.Repository {
    public class FranchiseeRepository : DawnDbClient {
        #region Fields
        private readonly SqlI18N _sqlI18N;
        #endregion
        #region Constructor
        public FranchiseeRepository (SqlI18N SqlI18N, string ConnectionString) : base(ConnectionString) {
            _sqlI18N = SqlI18N;
        }
        #endregion
        #region Methods
        ///// <summary>
        ///// Register franchisee
        ///// </summary>
        ///// <param name="franchisee"></param>
        ///// <param name="supprotDocuments"></param>
        ///// <param createdBy="sakthivel"></param>
        ///// <returns></returns>
        //public Guid RegisterFranchisee(FranchiseeRegisterDTO franchisee,DataTable supprotDocuments)
        //{
        //    Guid result = Guid.Empty;
        //    try
        //    {
        //        result = (Guid)ExecuteScalar("uspFranchiseeRegister", new
        //        {
        //            BranchID = franchisee.BranchID,
        //            ContactID = franchisee.ContactID,
        //            CreatedBy = franchisee.CreatedBy,
        //            Documents = supprotDocuments
        //        },
        //    commandType: System.Data.CommandType.StoredProcedure);
        //    }
        //    catch(SqlException ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }
        //    return result;
        //}

        /// <summary>
        /// Register franchisee
        /// </summary>
        /// <param name="franchisee"></param>
        /// <param name="supportDocuments"></param>
        /// <param name="actionBy"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public Guid RegisterFranchisee (FranchiseeRegisterDTO franchisee, DataTable supportDocuments, Guid actionBy, DataTable banks) {
            Guid result = Guid.Empty;
            try {
                result = (Guid)ExecuteScalar("uspCreateFranchisee", new {
                    CreatedBy = actionBy,
                    ContactName = franchisee.ContactName,
                    Email = franchisee.Email,
                    Gender = franchisee.Gender,
                    PhoneCode = franchisee.PhoneCode,
                    Phone = franchisee.Phone,
                    AlternatePhoneCode = franchisee.AlternatePhoneCode,
                    AlternatePhone = franchisee.AlternatePhone,
                    FranchiseeName = franchisee.FranchiseeName,
                    Address1 = franchisee.Address1,
                    Address2 = franchisee.Address2,
                    Area = franchisee.Area,
                    District = franchisee.District,
                    Taluk = franchisee.Taluk,
                    State = franchisee.State,
                    Country = franchisee.Country,
                    Pincode = franchisee.Pincode,
                    Latitude = franchisee.Latitude,
                    Longitude = franchisee.Longitude,
                    Documents = supportDocuments,
                    BankAccounts = banks
                },
            commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return result;
        }

        public Guid FranchiseeRerequest (Guid actionBy, string email, DataTable supportDocuments) {
            Guid result = Guid.Empty;
            try {
                result = (Guid)ExecuteScalar("uspCreateFranchiseeRerequest", new {
                    CreatedBy = actionBy,
                    Email = email,
                    Documents = supportDocuments
                },
            commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Check email is exists
        /// </summary>
        /// <param name="email"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public bool CheckEmailIsExists (string email) {
            bool result = false;
            string query = "SELECT dbo.sfnEmailExist(@Email)";
            try {
                result = ExecuteScalar(query, new { Email = email }).ToBoolean();
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Check franchisee is rejected
        /// </summary>
        /// <param name="email"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public bool CheckFranchiseeIsRejected (string email) {
            bool result = false;
            string query = "SELECT dbo.sfnFranchiseeIdRejected(@Email)";
            try {
                result = ExecuteScalar(query, new { Email = email }).ToBoolean();
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Check phone number is exists
        /// </summary>
        /// <param name="phone"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public bool CheckPhoneIsExists (string phone) {
            bool result = false;
            string query = "SELECT dbo.sfnIsPhoneExist(@Phone)";
            try {
                result = ExecuteScalar(query, new { Phone = phone }).ToBoolean();
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return result;
        }

        public bool CheckAccountNumberExists (string accountNumber) {
            bool isExists = false;
            isExists = ExecuteScalar("SELECT COUNT(*) FROM dbo.BankAccounts WHERE AccountNumber = @AccountNumber AND IsActive = 1", new { AccountNumber = accountNumber }).ToBoolean();
            return isExists;
        }

        /// <summary>
        /// Reject franchisee approval request
        /// </summary>
        /// <param name="rejection"></param>
        /// <param name="actionBy"></param>
        /// <param createdBy="sakthivel"></param>
        /// <returns></returns>
        public int RejectFranchisee (FranchiseeRejection rejection, Guid actionBy) {
            int result = 0;
            try {
                result = ExecuteScalar("uspRejectFranchisee", new {
                    FranchiseeRequestID = rejection.FranchiseeRequestID,
                    Reason = rejection.Reason,
                    UpdatedBy = actionBy
                }, commandType: System.Data.CommandType.StoredProcedure).ToInt();
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Create franchisee payment
        /// </summary>
        /// <param name="payment"></param>
        /// <param name="paymentItems"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public Guid FranchiseePayment (FranchiseePayment payment, DataTable paymentItems) {
            Guid franchiseePaymentID = Guid.Empty;
            try {
                franchiseePaymentID = (Guid)ExecuteScalar("uspCreateFranchiseePayment", new {
                    FranchiseeRequestID = payment.FranchiseeRequestID,
                    TransactionName = payment.TransactionName,
                    KeyTable = payment.KeyTable,
                    KeyColumn = payment.KeyColumn,
                    KeyValue = payment.KeyValue,
                    CreatedBy = payment.CreatedBy,
                    PaymentItems = paymentItems
                }, commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return franchiseePaymentID;
        }

        /// <summary>
        /// Get franchisee payment details
        /// </summary>
        /// <param name="franchiseeRequestID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<FranchiseePayments> GetFranchiseePayments (Guid franchiseeRequestID) {
            List<FranchiseePayments> franchiseePayments = new List<FranchiseePayments>();
            string query = "SELECT FranchiseeRequestID,TransactionName,PaymodeType,TransactionAmount,KeyTable,KeyColumn,KeyValue,VoucherDate AS VoucherDateI18,PayModeID,BankName,Amount,RoundOff,TransactionNo,TransactionDate,BankCommission,LedgerName,ToLedgerID,ToLedgerName FROM dbo.tfnGetFranchiseePayments(@FranchiseeRequestID)";
            try {
                franchiseePayments = GetList<FranchiseePayments>(query, _sqlI18N, new { FranchiseeRequestID = franchiseeRequestID });
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return franchiseePayments;
        }

        /// <summary>
        /// Get franchisee payment history
        /// </summary>
        /// <param name="packageLogID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<Payments> GetFranchiseePaymentHistory (Guid packageLogID) {
            List<Payments> payments = new List<Payments>();
            string query = "SELECT TransactionDate,Summary,Amount,IsCancelled,Reason,PayMode,CreatedDate,FranchiseePaymentTransID FROM dbo.tfnGetFranchiseePaymentHistory(@PackageLogID) ORDER BY CreatedDate DESC";
            try {
                payments = GetList<Payments>(query, _sqlI18N, new { PackageLogID = packageLogID });
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return payments;
        }

        /// <summary>
        /// Activate franchisee
        /// </summary>
        /// <param name="approveFranchisee"></param>
        /// <param name="actionBy"></param>
        /// <param createdBy="sakthivel"></param>
        /// <returns></returns>
        public Guid ActivateFranchisee (ActivateFranchisee approveFranchisee, Guid actionBy, Guid tenantBranchID, Guid contactID) {
            Guid result = Guid.Empty;
            try {
                result = (Guid)ExecuteScalar("uspActivateFranchisee", new {
                    FranchiseeRequestID = approveFranchisee.FranchiseeRequestID,
                    TenantBranchID = tenantBranchID,
                    UpdatedBy = actionBy,
                    ContactID = contactID
                }, commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Check tenant branch request ID exists or not
        /// </summary>
        /// <param name="tenantBranchRequestID"></param>
        /// <param createdBy="sakthivel"></param>
        /// <returns></returns>
        public bool CheckFranchiseeRequestID (Guid franchiseeRequestID) {
            bool result = false;
            string query;
            try {
                query = "SELECT dbo.sfnCheckFranchiseeRequestID(@FranchiseeRequestID)";
                result = (bool)ExecuteScalar(query, new { FranchiseeRequestID = franchiseeRequestID });
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Get franchisee details
        /// </summary>
        /// <param name="franchiseeRequestID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public FranchiseeDetails GetFranchiseeDetail (Guid franchiseeRequestID) {
            FranchiseeDetails franchisee;
            string query = "SELECT FranchiseeEnquiryID,FranchiseeName,Address1,Address2,Area,District,State,Country,Pincode,Latitude,Longitude,ContactName,Email,PhoneCode,Phone,AlternatePhoneCode,AlternatePhone,Gender FROM dbo.tfnGetFranchiseeDetail(@FranchiseeRequestID)";
            try {
                franchisee = Get<FranchiseeDetails>(query, _sqlI18N, new { FranchiseeRequestID = franchiseeRequestID });
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return franchisee;
        }

        /// <summary>
        /// Get franchisee request info
        /// </summary>
        /// <param createdBy="sakthivel"></param>
        /// <returns></returns>
        public DataSet GetFranchiseeRequestInfo (Guid franchiseeRequestID) {
            return GetDataSet("uspGetFranchiseeRequestInfo", new {
                FranchiseeRequestID = franchiseeRequestID
            }, commandType: CommandType.StoredProcedure);
        }

        /// <summary>
        /// Get active franchisee list
        /// </summary>
        /// <param createdBy="sakthivel"></param>
        /// <returns>
        /// Returns franchisee active list
        /// </returns>
        public IEnumerable<ActiveFranchisee> GetActiveFranchiseeList () {
            IEnumerable<ActiveFranchisee> franchisees = new List<ActiveFranchisee>();
            string query;
            try {
                query = "SELECT TenantBranchID,FranchiseeCode,ApprovalEndDate,SubscriptionEndDate,RegistrationDate,IsActive FROM dbo.tfnActiveFranchiseeList() ORDER BY RegistrationDate ASC";                
                franchisees = GetList<ActiveFranchisee>(query,_sqlI18N);
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return franchisees;
        }
        public List<ActiveFranchiseeContactV1> GetActiveFranchiseeListV1()
        {
            List<ActiveFranchiseeContactV1> franchisees = new List<ActiveFranchiseeContactV1>();
            string query;
            try
            {
                query = "uspGetActiveFranchiseeList";
                franchisees = GetList<ActiveFranchiseeContactV1>(query, _sqlI18N,commandType:CommandType.StoredProcedure);
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            return franchisees;
        }
        /// <summary>
        /// Get franchisee request status
        /// </summary>
        /// <param createdBy="sakthivel"></param>
        /// <returns>
        /// Returns franchisee request status
        /// </returns>
        public DataSet GetFranchiseeRequest () {
            try {
                return GetDataSet("uspGetFranchiseeRequest", commandType: CommandType.StoredProcedure);
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Create subscription log
        /// </summary>
        /// <param name="subscription"></param>
        /// <param name="actionBy"></param>
        /// <param createdBy="sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public SubscriptionLogInfo CreateSubscriptionLog (SubscriptionLog subscription, Guid actionBy) {
            SubscriptionLogInfo logInfos = new SubscriptionLogInfo();
            try {
                logInfos = Get<SubscriptionLogInfo>("uspSubscriptionLog", _sqlI18N, new {
                    FranchiseeRequestID = subscription.FranchiseeRequestID,
                    CreatedBy = actionBy
                }, commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return logInfos;
        }

        /// <summary>
        /// Rollback franchisee subscription log
        /// </summary>
        /// <param name="packageLogID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int RollBackSubscriptionLog (Guid packageLogID) {
            int result = 0;
            try {
                result = ExecuteScalar("uspRollbackSubscriptionLog", new { PackageLogID = packageLogID }, commandType: System.Data.CommandType.StoredProcedure).ToInt();
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Create franchisee renewal
        /// </summary>
        /// <param name="subscription"></param>
        /// <param name="actionBy"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public SubscriptionLogInfo RenewalSubscriptionLog (RenewalSubscriptionLog subscription, Guid actionBy) {
            SubscriptionLogInfo logInfos = new SubscriptionLogInfo();
            try {
                logInfos = Get<SubscriptionLogInfo>("uspRenewalSubscriptionLog", _sqlI18N, new {
                    TenantBranchID = subscription.TenantBranchID,
                    PackageID = subscription.PackageID,
                    CreatedBy = actionBy
                }, commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return logInfos;
        }

        public SubscriptionLogInfo WalletRenewalPayment (RenewalSubscriptionLog subscription, Guid actionBy) {
            SubscriptionLogInfo logInfos = new SubscriptionLogInfo();
            try {
                logInfos = Get<SubscriptionLogInfo>("uspWalletRenewalPayment", _sqlI18N, new {
                    TenantBranchID = subscription.TenantBranchID,
                    PackageID = subscription.PackageID,
                    CreatedBy = actionBy,
                    PaymentType = subscription.PaymentType
                }, commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return logInfos;
        }

        /// <summary>
        /// Get franchisee profile by id
        /// </summary>
        /// <param name="tenantBranchID"></param>
        /// <param createdBy="sakthivel"></param>
        /// <returns></returns>
        public DataSet GetFranchiseeProfileInfo (Guid tenantBranchID) {
            try {
                return GetDataSet("uspGetFranchiseeProfileInfo", new {
                    TenantBranchID = tenantBranchID
                }, commandType: CommandType.StoredProcedure);
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Get franchisee basic info
        /// </summary>
        /// <param createdBy="sakthivel"></param>
        /// <param createdDate="21-04-2023"></param>
        /// <returns>
        /// Returns franchisee code
        /// </returns>
        /// <exception cref="Exception"></exception>
        public List<GetFranchiseeCode> GetFranchiseeBasicInfo () {
            List<GetFranchiseeCode> franchiseeIDs = new List<GetFranchiseeCode>();
            try {
                franchiseeIDs = GetList<GetFranchiseeCode>("SELECT TenantBranchID,FranchiseeCode FROM dbo.tfnFranchiseeInfo()", _sqlI18N);
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return franchiseeIDs;
        }

        public List<BranchDetail> GetBranchDetails () {
            List<BranchDetail> branches = new List<BranchDetail>();
            try {
                string query = "SELECT fed.FranchiseeEnquiryID, fer.Status, fer.FranchiseeRequestID, ISNULL(fer.Reason, '') AS Reason, fed.FranchiseeCode, fed.FranchiseeName, fed.ContactName, fed.Gender, CONCAT(fed.PhoneCode, ' ', fed.Phone) AS Phone, CONCAT(fed.AlternatePhoneCode,' ',fed.AlternatePhone) AS AlternatePhone, fed.Email, fer.CreatedDate AS RequestDate, fer.UpdatedDate AS StatusDate FROM dbo.FranchiseeEnquiryDetail fed INNER JOIN dbo.FranchiseeEnquiryRequest fer ON fer.FranchiseeEnquiryID = fed.FranchiseeEnquiryID WHERE fed.Status <> 'ACTIVE' AND fed.IsDeleted = 0 ORDER BY RequestDate DESC;";
                branches = GetList<BranchDetail>(query, _sqlI18N);
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return branches;
        }

        /// <summary>
        /// Get approved package id
        /// </summary>
        /// <param createdBy="sakthivel"></param>
        /// <returns>
        /// Returns approved package ids and fees
        /// </returns>
        /// <exception cref="Exception"></exception>
        public List<ApprovalAndSubPackage> GetApprovalPackageID () {
            List<ApprovalAndSubPackage> approvals = new List<ApprovalAndSubPackage>();
            string query;
            try {
                query = "SELECT PackageID,PackageApprovedFees,PackageType FROM dbo.tfnApprovalPackageInfo()";
                approvals = GetList<ApprovalAndSubPackage>(query, _sqlI18N);
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return approvals;
        }

        /// <summary>
        /// Get franchisee code
        /// </summary>
        /// <param createdBy="sakthivel"></param>
        /// <param createdDate=""></param>
        /// <returns>
        /// Returns franchisee codes
        /// </returns>
        /// <exception cref="Exception"></exception>
        public IEnumerable<GetFranchiseeCode> GetFranchiseeID () {
            List<GetFranchiseeCode> franchiseeIDs = new List<GetFranchiseeCode>();
            string query = "SELECT TenantBranchID,FranchiseeCode,FranchiseeName,FranchiseeShortName FROM dbo.tfnFranchiseeInfo()";
            try {
                franchiseeIDs = GetList<GetFranchiseeCode>(query, _sqlI18N);
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return franchiseeIDs;
        }

        /// <summary>
        /// Reject franchisee renewal request
        /// </summary>
        /// <param name="packageLogID"></param>
        /// <param name="actionBy"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int RejectRenewalRequest (Guid packageLogID, string reason, Guid actionBy) {
            int result = 0;
            try {
                result = ExecuteScalar("uspRejectFranchiseeRenewal", new {
                    PackageLogID = packageLogID,
                    Reason = reason,
                    UpdatedBy = actionBy
                }, commandType: System.Data.CommandType.StoredProcedure).ToInt();
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Get franchisee renewal request list
        /// </summary>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<RenewalList> GetFranchiseeRenewalList () {
            List<RenewalList> renewalLists = new List<RenewalList>();
            string query = "SELECT RequestDate,TenantBranchID,PackageLogID,FranchiseeCode,PackageType,PackageName,Duration,Amount,SubscriptionStart,SubscriptionEnd,PaymentType FROM dbo.tfnGetRenewalRequestList()";
            try {
                renewalLists = GetList<RenewalList>(query, _sqlI18N);
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return renewalLists;
        }

        public List<PaymentDetails> PaymentDetails (Guid packageLogID) {
            string query = "SELECT CreatedDate AS TransactionDate,PaymentType AS Summary, PackageApprovedFees AS Amount FROM dbo.ApprovalAndSubscriptionLog WHERE PackageLogID = @PackageLogID";
            return GetList<PaymentDetails>(query, _sqlI18N, new { PackageLogID = packageLogID });
        }

        /// <summary>
        /// Approve franchisee renewal request
        /// </summary>
        /// <param name="packageLogID"></param>
        /// <param name="actionBy"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public Guid ApproveRenewalRequest (Guid packageLogID, Guid actionBy) {
            Guid result = Guid.Empty;
            try {
                result = (Guid)ExecuteScalar("uspApproveRenewal", new {
                    PackageLogID = packageLogID,
                    UpdatedBy = actionBy
                }, commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Lock franchisee
        /// </summary>
        /// <param name="tenantBranchID"></param>
        /// <param name="isActive"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int LockFranchisee (Guid tenantBranchID, bool isActive) {
            int result = 0;
            try {
                result = ExecuteScalar("uspLockFranchisee", new { TenantBranchID = tenantBranchID, IsActive = isActive }, commandType: System.Data.CommandType.StoredProcedure).ToInt();
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Get documents from database
        /// </summary>
        /// <param name="fileID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public DownloadDocument FileDownLoad (Guid fileID) {
            DownloadDocument result = new DownloadDocument();
            try {
                string query = "select FileName,FileExtensionType as MIMEType,AttachmentContent as Content from dbo.FranchiseeEnquiryDocument WHERE FranchiseeDocumentID=@DocumentID";
                var dataRow = GetDataRow(query, new { DocumentID = fileID });
                if (dataRow != null) {
                    result.Content = (byte[])dataRow["Content"];
                    result.MIMEType = (string)dataRow["MIMEType"];
                    result.FileName = (string)dataRow["FileName"];
                }
                return result;
            }
            catch (Exception ex) {
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Get franchisee renewal list
        /// </summary>
        /// <param name="tenantBranchID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<Renewal> GetFranchiseeRenewalList (Guid franchiseeEnquiryID) {
            List<Renewal> renewals = new List<Renewal>();
            try {
                renewals = GetList<Renewal>("uspGetFranchiseeRenewalList", _sqlI18N, new { FranchiseeEnquiryID = franchiseeEnquiryID }, commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return renewals;
        }

        /// <summary>
        /// Get franchisee renewal list by admin
        /// </summary>
        /// <returns></returns>
        /// <param createdBy="Sakthivel"></param>
        /// <exception cref="Exception"></exception>
        public List<NewRenewalList> GetRenewalListByAdmin () {
            List<NewRenewalList> renewalLists = new List<NewRenewalList>();
            string query = "SELECT TenantBranchID,PackageLogID,FranchiseeCode,PackageType,Amount,PaymentType FROM dbo.tfnRenewalPaymentList()";
            try {
                renewalLists = GetList<NewRenewalList>(query, _sqlI18N);
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return renewalLists;
        }

        /// <summary>
        /// Get package log id
        /// </summary>
        /// <param name="franchiseeEnquiryID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public Guid GetPackageLogID (Guid franchiseeEnquiryID) {
            Guid packageLogID = Guid.Empty;
            string query = "SELECT dbo.sfnGetPackageLogID(@FranchiseeEnquiryID)";
            try {
                packageLogID = (Guid)ExecuteScalar(query, new { FranchiseeEnquiryID = franchiseeEnquiryID });
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return packageLogID;
        }

        /// <summary>
        /// Get franchisee whole period date
        /// </summary>
        /// <param name="tenantBranchID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public string GetWholePeriod (Guid tenantBranchID) {
            string date = string.Empty;
            string query = "SELECT dbo.sfnGetWholePeriodDate(@TenantBranchID)";
            try {
                date = ExecuteScalar(query, new { TenantBranchID = tenantBranchID }).ToString();
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return date;
        }
        public GetWholePeriod GetWholePeriodDate(Guid tenantBranchID)
        {
            try
            {
                string query = "SELECT CreatedDate AS CreatedDateI18 FROM dbo.FranchiseeEnquiryDetail WHERE TenantBranchID = @TenantBranchID";
                return Get<GetWholePeriod>(query, _sqlI18N, new
                {
                    TenantBranchID = tenantBranchID
                });
            }
            catch (Exception)
            {

                throw;
            }
        }
        /// <summary>
        /// Get franchisee summary by admin
        /// </summary>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public FranchiseeSummaryByAdmin GetFranchiseeDashboardByAdmin () {
            FranchiseeSummaryByAdmin dashboard = new FranchiseeSummaryByAdmin();
            string query = "SELECT Pending,Active,Expired,Locked,Approval,Subscription FROM dbo.tfnGetFranchiseeSummary()";
            try {
                dashboard = Get<FranchiseeSummaryByAdmin>(query, _sqlI18N);
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return dashboard;
        }

        public DashboardByFranchisee SummaryByFranchisee (Guid tenantBranchID) {
            DashboardByFranchisee summary;
            string query = @"SELECT ApprovalStartDateI18,ApprovalEndDateI18,SubscriptionStartDateI18,SubscriptionEndDateI18,
                             ApprovalTotalDays,ApprovalCompletedDays,ApprovalRemainingDays,
                            SubscriptionTotalDays,SubscriptionCompletedDays,SubscriptionRemainingDays,
                            ContactTitle,ContactName,FranchiseeName,Area,Taluk,District
                            FROM dbo.tfnGetSummaryByFranchisee(@TenantBranchID)";            
            try {
                summary = Get<DashboardByFranchisee>(query, _sqlI18N, new { TenantBranchID = tenantBranchID });
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return summary;
        }

        /// <summary>
        /// Reject franchisee paid request
        /// </summary>
        /// <param name="branchRequestID"></param>
        /// <param name="actionBy"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int RejectPaidRequest (RejectPaidRequest reject, Guid actionBy) {
            int result = 0;
            try {
                result = ExecuteScalar("uspRejectPaidRequest", new { FranchiseeRequestID = reject.FranchiseeRequestID, PackageLogID = reject.PackageLogID, Reason = reject.Reason, UpdatedBy = actionBy }, commandType: System.Data.CommandType.StoredProcedure).ToInt();
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Check package log is is exists
        /// </summary>
        /// <param name="franchiseeRequestID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public Guid CheckPackageLogIDExists (Guid franchiseeRequestID) {
            Guid packageLogID = Guid.Empty;
            string query = "SELECT dbo.sfnCheckPackageLogIDExists(@FranchiseeRequestID)";
            try {
                packageLogID = (Guid)ExecuteScalar(query, new { FranchiseeRequestID = franchiseeRequestID });
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return packageLogID;
        }

        /// <summary>
        /// Get package log info
        /// </summary>
        /// <param name="franchiseeRequestID"></param>
        /// <param name="packageLogID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public SubscriptionLogInfo GetPackageLogInfo (Guid franchiseeRequestID, Guid packageLogID) {
            SubscriptionLogInfo result = new SubscriptionLogInfo();
            try {
                result = Get<SubscriptionLogInfo>("uspGetPackageLogInfo", _sqlI18N, new { FranchiseeRequestID = franchiseeRequestID, PackageLogID = packageLogID }, commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return result;
        }

        public List<GuideVideos> GetGuideVideos (string mobile_app) {
            List<GuideVideos> videos = new List<GuideVideos>();
            try {
                string query = "SELECT GuideVideoID,Title,MobileApp,ShowIn,SortOrder,LinkType,ContentUrl,TenantBranchID FROM dbo.GuideVideoMaster WHERE IsDeleted = 0 AND MobileApp = @MobileApp AND ShowIn = 'Demo' ORDER BY SortOrder";
                return GetList<GuideVideos>(query, _sqlI18N, new { MobileApp = mobile_app });
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
        }

        public int CheckShorOrderExists (AddVideos add) {
            int result = 0;
            result = ExecuteScalar("SELECT COUNT(*) FROM dbo.GuideVideoMaster WHERE MobileApp = @MobileApp AND SortOrder = @SortOrder AND ShowIn = @ShowIn AND IsDeleted = 0", new { MobileApp = add.MobileApp, SortOrder = add.SortOrder, ShowIn = add.ShowIn }).ToInt();
            return result;
        }

        public int AddVideo (AddVideos add, Guid actionBy) {
            int result = 0;
            try {
                result = ExecuteScalar("uspCreateVideo", new {
                    Title = add.Title,
                    MobileApp = add.MobileApp,
                    ShowIn = add.ShowIn,
                    LinkType = add.LinkType,
                    SortOrder = add.SortOrder,
                    ContentUrl = add.ContentUrl,
                    TenantBranchID = add.TenantBranchID,
                    CreatedBy = actionBy
                }, commandType: System.Data.CommandType.StoredProcedure).ToInt();
                return result;
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
        }

        public int GetSortOrder (Guid guideVideoID) {
            return ExecuteScalar("SELECT SortOrder FROM dbo.GuideVideoMaster WHERE GuideVideoID = @guideVideoID AND IsDeleted = 0", new { GuideVideoID = guideVideoID }).ToInt();
        }

        public bool CheckSortOrder (EditVideo edit) {
            bool result = false;
            //result = ExecuteScalar("SELECT COUNT(*) FROM dbo.GuideVideoMaster WHERE GuideVideoID = @GuideVideoID AND SortOrder = @SortOrder AND LinkType = @LinkType AND IsDeleted = 0 AND IsActive = 1", new
            //{
            //    GuideVideoID = edit.GuideVideoID,
            //    LinkType = edit.LinkType,
            //    SortOrder = edit.SortOrder
            //}).ToBoolean();
            result = ExecuteScalar("SELECT COUNT(*) FROM dbo.GuideVideoMaster WHERE MobileApp = @MobileApp AND SortOrder = @SortOrder AND ShowIn = @ShowIn AND IsDeleted = 0", new { MobileApp = edit.MobileApp, SortOrder = edit.SortOrder, ShowIn = edit.ShowIn }).ToBoolean();
            return result;
        }

        public int EditVideo (EditVideo edit, Guid actionBy) {
            int result = 0;
            try {
                result = ExecuteScalar("uspEditVideo", new {
                    GuideVideoID = edit.GuideVideoID,
                    Title = edit.Title,
                    MobileApp = edit.MobileApp,
                    ShowIn = edit.ShowIn,
                    SortOrder = edit.SortOrder,
                    LinkType = edit.LinkType,
                    ContentURL = edit.ContentUrl,
                    UpdatedBy = actionBy
                }, commandType: System.Data.CommandType.StoredProcedure).ToInt();
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return result;
        }

        public Guid GetContactID (Guid tenantBranchID) {
            Guid contactID = Guid.Empty;
            string query = "SELECT ContactID FROM dbo.FranchiseeEnquiryDetail WHERE TenantBranchID = @TenantBranchID AND IsDeleted = 0";
            try {
                contactID = (Guid)ExecuteScalar(query, new { TenantBranchID = tenantBranchID });
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return contactID;
        }

        public List<MblFranchisee> GetFranchisees (MblFilter filter) {
            List<MblFranchisee> franchisees;
            try {
                var param = new MblFilter() {
                    State = filter.State,
                    District = filter.District,
                    Taluk = filter.Taluk,
                };
                franchisees = GetList<MblFranchisee>("uspGetFranchisees", _sqlI18N, param, commandType: System.Data.CommandType.StoredProcedure);
                return franchisees;
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
        }

        public List<MlbRenewalList> MblRenewalList (Guid tenantBranchID) {
            string query = "SELECT al.PackageLogID,ap.PackageType,al.PackageApprovedFees AS Amount,al.Status,al.Reason,al.PaymentType FROM dbo.ApprovalAndSubscriptionLog al INNER JOIN dbo.FranchiseeEnquiryDetail fe ON fe.FranchiseeEnquiryID = al.FranchiseeEnquiryID INNER JOIN dbo.ApprovalAndSubscriptionPackage ap ON ap.PackageID = al.PackageID WHERE fe.TenantBranchID = @TenantBranchID AND ap.PackageType <> 'REGISTRATION' ORDER BY al.CreatedDate ASC";
            try {
                return GetList<MlbRenewalList>(query, _sqlI18N, new { TenantBranchID = tenantBranchID });
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
        }

        public List<Payments> PaymentList (Guid packageLogID) {
            List<Payments> payments = new List<Payments>();
            string query = "SELECT CreatedDate AS TransactionDate, PaymentType AS Summary, PackageApprovedFees AS Amount, 0 AS IsCancelled FROM dbo.ApprovalAndSubscriptionLog WHERE PackageLogID = @PackageLogID";
            return GetList<Payments>(query, _sqlI18N, new { PackageLogID = packageLogID });
        }

        public List<RankList> GetStateFranchiseeRank (DataTable data, StateRankFilter filter) {
            try {
                return GetList<RankList>("uspGetFranchiseeRank", _sqlI18N, new {
                    PurchaseAmount = data,
                    State = filter.State
                }, commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
        }

        public List<RankList> GetDistrictFranchiseeRank (DataTable data, DistrictRankFilter filter) {
            try {
                return GetList<RankList>("uspGetDistrictFranchiseeRank", _sqlI18N, new {
                    PurchaseAmount = data,
                    State = filter.State,
                    District = filter.District
                }, commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
        }

        public List<RankList> GetStateFranchiseeAchiever (DataTable data, StateAchieversFilter filter) {
            try {
                return GetList<RankList>("uspGetFranchiseeStateAchievers", _sqlI18N, new {
                    PurchaseAmount = data,
                    State = filter.State
                }, commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
        }

        public List<RankList> GetDistrictFranchiseeAchiever (DataTable data, DistrictAchieversFilter filter) {
            try {
                return GetList<RankList>("uspGetDistrictFranchiseeAchievers", _sqlI18N, new {
                    PurchaseAmount = data,
                    State = filter.State,
                    District = filter.District
                }, commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
        }

        public FranchiseeRankStatus GetFranchiseeRankStatus (List<PurchaseAmount> purchaseAmount, List<PurchaseAmount> admisstionamount, Guid tenantBranchID) {
            try {
                return Get<FranchiseeRankStatus>("uspGetFranchiseeRankStatus", _sqlI18N, new {
                    PurchaseAmount = purchaseAmount.ToDataTable(),
                    Admisstionamount = admisstionamount.ToDataTable(),
                    TenantBranchID = tenantBranchID
                }, commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
        }

        public int CreateNoticeBoard (CreateNotice create, Guid tenantBranchID, Guid actionBy) {
            int result = 0;
            try {
                result = ExecuteScalar("uspCreateNotice", new { TenantBranchID = tenantBranchID, Content = create.Content, CreatedBy = actionBy }, commandType: System.Data.CommandType.StoredProcedure).ToInt();
                return result;
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
        }

        public GetNotice GetNoticeBoard (Guid tenantBranchID) {
            try {
                return Get<GetNotice>("SELECT ContentID,Content FROM dbo.NoticeBoard WHERE TenantBranchID = @TenantBranchID AND IsDeleted = 0", _sqlI18N, new { TenantBranchID = tenantBranchID });
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
        }

        public HelpAndSupport GetHelpAndSupport (Guid tenantBranchID) {
            try {
                return Get<HelpAndSupport>("SELECT TenantBranchID,FranchiseeName,ContactName,PhoneCode,Phone,ISNULL(AlternatePhoneCode,'') AS AlternatePhoneCode,ISNULL(AlternatePhone,'') AS AlternatePhone,Email,Address1,ISNULL(Address2,'') AS Address2,Area,District,State,Country,Pincode FROM dbo.FranchiseeEnquiryDetail WHERE Status = 'ACTIVE' AND IsDeleted = 0 AND TenantBranchID = @TenantBranchID", _sqlI18N, new { TenantBranchID = tenantBranchID });
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
        }

        public int UploadStudentFeedback (DTOStudentFeedback data, Guid actionBy) {
            int result = 0;
            try {
                result = ExecuteScalar("uspUploadStudentFeedback", new {
                    Title = data.Title,
                    FileName = data.FileName,
                    TenantBranchID = data.TenantBranchID,
                    CreatedBy = actionBy
                }, commandType: System.Data.CommandType.StoredProcedure).ToInt();
                return result;
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
        }

        public int ApproveFeedback (ApproveFeedback approve, Guid actionBy, int sortOrder) {
            int result = 0;
            try {
                result = ExecuteScalar("uspApprovetudentFeedback", new {
                    Title = approve.Title,
                    TenantBranchID = approve.TenantBranchID,
                    CreatedBy = actionBy,
                    ContentUrl = approve.ContentUrl,
                    FeedbackID = approve.FeedbackID,
                    SortOrder = sortOrder
                }, commandType: System.Data.CommandType.StoredProcedure).ToInt();
                return result;
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
        }

        public int GetSortOrderNum (Guid tenantBranchID) {
            int result = 0;
            try {
                result = ExecuteScalar("SELECT COUNT(*) FROM dbo.GuideVideoMaster WHERE MobileApp = 'Student' AND ShowIn = 'Feedback' AND TenantBranchID = @TenantBranchID", new { TenantBranchID = tenantBranchID }).ToInt();
                return result;
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
        }

        public int GetNewSortOrderNum (Guid tenantBranchID) {
            int result = 0;
            try {
                result = ExecuteScalar("SELECT TOP 1 SortOrder FROM dbo.GuideVideoMaster WHERE MobileApp = 'Student' AND ShowIn = 'Feedback' AND TenantBranchID = @TenantBranchID ORDER BY SortOrder DESC;", new { TenantBranchID = tenantBranchID }).ToInt();
                return result;
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
        }

        public int RejectFeedback (RejectStudentFeedback reject, Guid actionBy) {
            int result = 0;
            try {
                result = ExecuteScalar("uspRejectStudentFeedback", new {
                    FeedbackID = reject.FeedbackID,
                    Reason = reject.Reason,
                    UpdatedBy = actionBy
                }, commandType: System.Data.CommandType.StoredProcedure).ToInt();
                return result;
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
        }

        public List<GetStudentFeedback> GetFeedbackByAdmin () {
            List<GetStudentFeedback> list = new List<GetStudentFeedback>();
            try {
                list = GetList<GetStudentFeedback>("SELECT FeedbackID,TenantBranchID,Franchisee,FranchiseeCode,Title,FileName,Status,RequestDate FROM dbo.tfnGetStudentFeedback() ORDER BY RequestDate DESC", _sqlI18N);
                return list;
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
        }

        public List<GetBanks> GetBanks (Guid franchiseeRequestID) {
            try {
                string query = "SELECT b.FranchiseeEnquiryID,b.AccountType,b.AccountName,b.AccountNumber,b.Bank,b.IFSC,b.Description FROM dbo.BankAccounts b INNER JOIN dbo.FranchiseeEnquiryRequest fr ON fr.FranchiseeEnquiryID = b.FranchiseeEnquiryID WHERE fr.FranchiseeRequestID = @FranchiseeRequestID AND b.IsDeleted = 0";
                return GetList<GetBanks>(query, _sqlI18N, new { FranchiseeRequestID = franchiseeRequestID });
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
        }

        public List<GetMobileApp> GetMobileApps (string configName) {
            List<GetMobileApp> list;
            try {
                list = GetList<GetMobileApp>("SELECT MinValue FROM dbo.AppConfiguration WHERE ConfigName = @configName", _sqlI18N, new { ConfigName = configName });
                return list;
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
        }

        public List<GuideVideos> GuideVideos () {
            try {
                return GetList<GuideVideos>("SELECT GuideVideoID,Franchisee,TenantBranchID,Title,MobileApp,ShowIn,SortOrder,LinkType,ContentUrl FROM dbo.tfnGetMobileApps() ORDER BY MobileApp,ShowIn,SortOrder", _sqlI18N);
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
        }

        public int DeleteGuideVideo (Guid guideVideoID, Guid actionBy) {
            int result = 0;
            try {
                result = ExecuteScalar("uspDeleteVideo", new { GuideVideoID = guideVideoID, UpdatedBy = actionBy }, commandType: System.Data.CommandType.StoredProcedure).ToInt();
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return result;
        }

        public List<GuideVideos> GetGuideVideo (string mobileApp, string showIn) {
            List<GuideVideos> list;
            try {
                list = GetList<GuideVideos>("SELECT GuideVideoID,TenantBranchID, Title, MobileApp, ShowIn,SortOrder, LinkType,ContentUrl FROM dbo.GuideVideoMaster WHERE MobileApp = @mobileApp AND ShowIn = @showIn AND IsDeleted = 0 AND IsActive = 1", _sqlI18N, new { MobileApp = mobileApp, ShowIn = showIn });
                return list;
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
        }

        public List<GuideVideos> GetStudentFeedback (Guid tenantBranchID, string mobileApp, string showIn) {
            List<GuideVideos> list;
            try {
                list = GetList<GuideVideos>("SELECT GuideVideoID,TenantBranchID, Title, MobileApp, ShowIn,SortOrder, LinkType,ContentUrl FROM dbo.GuideVideoMaster WHERE TenantBranchID = @tenantBranchID AND MobileApp = @mobileApp AND ShowIn = @showIn AND IsDeleted = 0 AND IsActive = 1", _sqlI18N, new { TenantBranchID = tenantBranchID, MobileApp = mobileApp, ShowIn = showIn });
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return list;
        }

        public string GetPackageType (Guid packageLogID) {
            string packageType = "";
            string query = "SELECT ap.PackageType FROM dbo.ApprovalAndSubscriptionLog al INNER JOIN dbo.ApprovalAndSubscriptionPackage ap ON ap.PackageID = al.PackageID WHERE al.PackageLogID = @PackageLogID";
            packageType = ExecuteScalar(query, new { PackageLogID = packageLogID }).ToString();
            return packageType;
        }

        public List<FranchiseeBanks> GetFranchiseeBanks(Guid franchiseeRequestID) {
            string query = "SELECT b.AccountID,b.AccountType,b.AccountName,b.AccountNumber,b.Bank,b.IFSC,b.BranchName,b.Description AS Notes FROM dbo.BankAccounts b INNER JOIN dbo.FranchiseeEnquiryRequest fr ON fr.FranchiseeEnquiryID = b.FranchiseeEnquiryID WHERE fr.FranchiseeRequestID = @FranchiseeRequestID";
            return GetList<FranchiseeBanks>(query, _sqlI18N, new { FranchiseeRequestID = franchiseeRequestID });
        }

        public IEnumerable<GetMobileApp> GetRelationship () {
            return GetList<GetMobileApp>("SELECT MinValue FROM dbo.AppConfiguration WHERE ConfigGroupID = 'RELATIONSHIP' AND IsDeleted = 0 ORDER BY Slno");
        }

        public DataSet FranchiseePaymentHistory(Guid packageLogID) {
            return GetDataSet("uspPaymentHistory", new {
                PackageLogID = packageLogID
            }, commandType: System.Data.CommandType.StoredProcedure);
        }

        public IEnumerable<TalukList> GetTalukList(string district) {
            return GetList<TalukList>("SELECT DISTINCT ISNULL(Taluk, '') AS Taluk FROM dbo.FranchiseeEnquiryDetail WHERE REPLACE(UPPER(District), ' ', '') = REPLACE(UPPER(@District), ' ', '') ORDER BY Taluk;", _sqlI18N, new { District = district });
        }

        public GetImages GetOwner(Guid tenantBranchID) {
            string query = "SELECT fed.FileName, fed.DocumentType AS DocumentName, fed.FileExtensionType AS DocumentType, CONCAT('baseurl/', fed.FranchiseeDocumentID) AS ContentUrl FROM dbo.FranchiseeEnquiryDocument fed INNER JOIN dbo.FranchiseeEnquiryRequest fer ON fer.FranchiseeRequestID = fed.FranchiseeRequestID INNER JOIN dbo.FranchiseeEnquiryDetail fd ON fd.FranchiseeEnquiryID = fer.FranchiseeEnquiryID WHERE fed.IsDeleted = 0 AND fer.IsDeleted = 0 AND fed.DocumentType = 'OwnerImage' AND fd.TenantBranchID = @TenantBranchID";
            return Get<GetImages>(query, _sqlI18N, new { TenantBranchID = tenantBranchID });
        }

        public GetApprovalCertificate GetApprovalCertificate(Guid tenantBranchID) {
            string query = "SELECT fed.FileName,fed.FileExtensionType AS MIMEType,fed.FranchiseeDocumentID AS DocumentID,fd.ContactName,fd.FranchiseeCode,fd.FranchiseeName,fd.Area,fd.District,fd.ApprovalStartDate AS ApprovedDateI18,CONCAT((FORMAT(fd.ApprovalStartDate, 'MMM-yyyy')), ' To ', (FORMAT(fd.ApprovalEndDate, 'MMM-yyyy'))) AS ValidPeriod,DATEADD(DAY, -1, fd.ApprovalEndDate) AS NextRenewalDateI18 FROM dbo.FranchiseeEnquiryDocument fed INNER JOIN dbo.FranchiseeEnquiryRequest fer ON fer.FranchiseeRequestID = fed.FranchiseeRequestID INNER JOIN dbo.FranchiseeEnquiryDetail fd ON fd.FranchiseeEnquiryID = fer.FranchiseeEnquiryID WHERE fed.IsDeleted = 0 AND fer.IsDeleted = 0 AND fed.DocumentType = 'OwnerImage' AND fd.TenantBranchID = @TenantBranchID;";
            return Get<GetApprovalCertificate>(query, _sqlI18N, new {
                tenantBranchID = tenantBranchID
            });
        }

        public Guid GetTransID(Guid franchiseePaymentID, string transNo) {
            return (Guid)ExecuteScalar("SELECT FranchiseePaymentTransID FROM dbo.FranchiseePaymentTrans WHERE FranchiseePaymentID = @FranchiseePaymentID AND TransactionNo = @TransactionNo", new {
                FranchiseePaymentID = franchiseePaymentID,
                TransactionNo = transNo
            });
        }

        public int EditFranchiseeCode(EditFranchiseeCode edit, Guid actionBy) {
            try {
                int count = Update(new {
                    FranchiseeCode = edit.FranchiseeCode
                }, new {
                    FranchiseeEnquiryID = edit.FranchiseeEnquiryID
                }, "FranchiseeEnquiryDetail", actionBy.ToString());
                return count;
            }
            catch(SqlException ex) {
                throw new Exception(ex.Message);
            }
        }

        public bool CheckCodeExists(EditFranchiseeCode edit) {
            bool isExists = false;
            isExists = ExecuteScalar("SELECT COUNT(*) FROM dbo.FranchiseeEnquiryDetail WHERE FranchiseeCode = @FranchiseeCode AND FranchiseeEnquiryID <> @FranchiseeEnquiryID", new { FranchiseeCode = edit.FranchiseeCode, FranchiseeEnquiryID = edit.FranchiseeEnquiryID }).ToBoolean();
            return isExists;
        }

        public string GetCenterName(Guid tenantBranchID)
        {
            string centerName = "";
            string query = "SELECT TenantBranchName AS CentreName FROM dbo.TenantBranch WHERE TenantBranchID=@TenantBranchID";
            return centerName = ExecuteScalar(query, new { TenantBranchID = tenantBranchID }).ToStringOrEmpty();
        }
        public IEnumerable<StateList> GetStateList(string country)
        {
            try
            {
            return GetList<StateList>("SELECT DISTINCT ISNULL(State, '') AS StateProvinceName FROM dbo.FranchiseeEnquiryDetail WHERE REPLACE(UPPER(Country), ' ', '') = REPLACE(UPPER(@Country), ' ', '') ORDER BY StateProvinceName;", _sqlI18N, new { Country = country });
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IEnumerable<DistrictList> GetDistrictList(string state)
        {
            try
            {
            return GetList<DistrictList>("SELECT DISTINCT ISNULL(District, '') AS DistrictProvinceName FROM dbo.FranchiseeEnquiryDetail WHERE REPLACE(UPPER(State), ' ', '') = REPLACE(UPPER(@State), ' ', '') ORDER BY DistrictProvinceName;", _sqlI18N, new { State = state });
            }
            catch (Exception)
            {
                throw;
            }
        }
        public int GetCourierCharges()
        {
            return ExecuteScalar("SELECT MinValue FROM dbo.AppConfiguration WHERE ConfigID = 'COURIER_CHARGES' AND ConfigGroupID ='FRANCHISEE' AND IsDeleted = 0").ToInt();
        }
        public int GetIDCardCharges()
        {
            return ExecuteScalar("SELECT MinValue FROM dbo.AppConfiguration WHERE ConfigID = 'IDCARD_CHARGES' AND ConfigGroupID ='FRANCHISEE' AND IsDeleted = 0").ToInt();
        }
        public AdminContact GetAdminContact()
        {
            try
            {
                string query = @"SELECT ContactTitle,ContactName,FranchiseeName,Area,Taluk,District,Phone,Pincode,Website FROM dbo.tfnGetAdminContact()";
                return Get<AdminContact>(query, _sqlI18N);
            }
            catch (SqlException)
            {
                throw;
            }
        }
        public ProfileImage GetOwnerImage(Guid tenantBranchID)
        {
            try
            {
                string query = "uspGetFranchiseeOwnerImage";
                return Get<ProfileImage>(query,_sqlI18N, new
                {
                    TenantBranchID = tenantBranchID
                },commandType: CommandType.StoredProcedure);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public int UpdateCharges(int amount,string configID)
        {
            try
            {
                string query = @"DECLARE @RowsAffected INT;
                                 UPDATE dbo.AppConfiguration SET MinValue = @MinValue ,MaxValue = @MaxValue , UpdatedDate = SYSDATETIMEOFFSET() 
                                 WHERE ConfigGroupID = 'FRANCHISEE' AND ConfigID= @ConfigID
                                 SET @RowsAffected = @@ROWCOUNT
                                 SELECT @RowsAffected;";
                return (int)ExecuteScalar(query, new
                {
                    MinValue = amount,
                    MaxValue = amount,
                    ConfigID = configID
                });
            }
            catch (Exception)
            {

                throw;
            }
        }
        public int IDCardCourierChargesUpdate(ChargeUpdate amount)
        {
            try
            {
                return (int)ExecuteScalar("uspUpdateCourierIDCardCharges", new
                {
                    IDCard = amount.IDCard,
                    Courier = amount.Courier
                },commandType: CommandType.StoredProcedure);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public DataSet FileDownLoadList(Guid franchiseeRequestID)
        {
            try
            {
                return GetDataSet("uspGetDocumentDataList", new
                {
                    FranchiseeRequestID = franchiseeRequestID
                }, commandType:CommandType.StoredProcedure);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public bool EditFranchiseeContactInfo(EditFranchiseeCodeData edit,Guid UpdatedBy)
        {
            try
            {
             var result  = ExecuteScalar("uspEditFranchiseePersonalDetails", new
                {
                    FranchiseeEnquiryID=edit.FranchiseeEnquiryID,
                    ContactName=edit.ContactName,
                    Gender=edit.Gender,
                    Phone=edit.Phone,
                    PhoneCode = edit.PhoneCode,
                    UpdatedBy=UpdatedBy
                }, commandType: CommandType.StoredProcedure).ToBoolean();
              return result ;
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public bool EditFranchiseeInfo(EditFranchiseeCodeData edit, Guid UpdatedBy)
        {
            try
            {
                var result= ExecuteScalar("uspEditFranchiseePersonalDetails", new
                {
                    FranchiseeEnquiryID= edit.FranchiseeEnquiryID,
                    FranchiseeCode=  edit.FranchiseeCode,
                    Taluk= edit.Taluk,
                    Address1 =  edit.Address1,
                    Address2 =  edit.Address2,
                    Area=edit.Area,
                    District= edit.District,
                    State=edit.State,
                    Country= edit.Country,
                    Pincode=edit.Pincode,
                    UpdatedBy =UpdatedBy
                }, commandType: CommandType.StoredProcedure).ToBoolean();
                return result;
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
        }

      
        #endregion
    }
}