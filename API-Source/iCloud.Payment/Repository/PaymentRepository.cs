using Dawn.Database;
using Dawn.Utilities.Extensions;
using Dawn.Utilities.i18N;
using EntitiesDTO.Admin;
using EntitiesDTO.Payment;
using Microsoft.Data.SqlClient;
using System.Data;

namespace iCloud.Payment.Repository
{
    public class PaymentRepository : DawnDbClient
    {
        #region Fields
        private readonly SqlI18N _sqlI18N;
        #endregion
        #region Constructor
        public PaymentRepository(SqlI18N SqlI18N, string ConnectionString) : base(ConnectionString)
        {
            _sqlI18N = SqlI18N;
        }
        #endregion
        #region Methods
        /// <summary>
        /// Get all paymode list
        /// </summary>
        /// <returns></returns>
        /// <param createdBy="Sakthivel"></param>
        /// <param createdDate="13-04-2023"></param>
        /// <exception cref="Exception"></exception>
        public List<GetPaymode> GetPaymodeList()
        {
            List<GetPaymode> getPaymodes = new List<GetPaymode>();
            string query;
            try
            {
                query = "SELECT PayModeID,PayMode FROM tfnGetPaymode()";
                getPaymodes = GetList<GetPaymode>(query, _sqlI18N);
            }
            catch(SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            return getPaymodes;
        }

        /// <summary>
        /// Create franchisee registration payment
        /// </summary>
        /// <param name="receipt"></param>
        /// <param name="paymentItems"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <param createdDate="13-04-2023"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public Guid CreatePayment(NewReceipt receipt, DataTable PaymentItems) {
            Guid result = Guid.Empty;
            try {
                result = (Guid)ExecuteScalar("uspCreateFranchiseePayment", new {
                   receipt.TenantBranchID,receipt.TransactionName,receipt.KeyTable,receipt.KeyColumn,
                   receipt.KeyValue,
                    PaymentItems,
                    receipt.CreatedBy,receipt.ContactID,receipt.VoucherDate
                }, commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Roll back payment
        /// </summary>
        /// <param name="keyValue"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int RollBackPayment(Guid keyValue) {
            int result = 0;
            try {
                result = ExecuteScalar("uspRollBackPayment", new { KeyValue = keyValue }, commandType: System.Data.CommandType.StoredProcedure).ToInt();
            }
            catch(SqlException ex) {
                throw new Exception(ex.Message);
            }
            return result;
        }

        public int RollBackPartPayment(Guid tenantBranchTransID) {
            int result = 0;
            try {
                result = ExecuteScalar("uspRollBackPartPayment", new { TenantBranchTransID = tenantBranchTransID }, commandType: System.Data.CommandType.StoredProcedure).ToInt();
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return result;
        }


        /// <summary>
        /// Create student down payment
        /// </summary>
        /// <param name="payment"></param>
        /// <param name="PaymentItems"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns>
        /// Returns tenant branch id
        /// </returns>
        /// <exception cref="Exception"></exception>
        public Guid CreateStudentDownPayment(DownPayment payment, DataTable PaymentItems) {
            Guid result = Guid.Empty;
            try {
                result = (Guid)ExecuteScalar("uspCreateStudentDownPayment", new {
                    payment.TenantBranchID,
                    PaymentItems,
                    payment.KeyValue,
                    payment.CreatedBy,
                    payment.TransactionName,
                    payment.WalletAmount,
                    payment.SenderID,
                    payment.ReceiverID,
                    payment.Description
                }, commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Create student part payment
        /// </summary>
        /// <param name="payment"></param>
        /// <param name="PaymentItems"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public Guid CreateStudentPartPayment(PartPayment payment, DataTable PaymentItems) {
            Guid result = Guid.Empty;
            try {
                result = (Guid)ExecuteScalar("uspCreateStudentPartPayment", new {
                    payment.TenantBranchID,
                    PaymentItems,
                    payment.KeyValue,
                    payment.CreatedBy,
                    payment.TransactionName
                }, commandType: System.Data.CommandType.StoredProcedure);
            }
            catch(SqlException ex) {
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Get franchisee payment details
        /// </summary>
        /// <param createdBy="Sakthivel"></param>
        /// <param createdDate="18-04-2023"></param>
        /// <returns></returns>
        public DataSet GetPaymentDetails()
        {
            try
            {
                string query = "uspPaymentDetails";
                return GetDataSet(query, commandType: CommandType.StoredProcedure);
            }
            catch(SqlException ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Get student payment details
        /// </summary>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<StudentPaymentDetails> GetStudentPaymentDetails(Guid tenantBranchID)
        {
            List<StudentPaymentDetails> paymentDetails = new List<StudentPaymentDetails>();
            string query = "SELECT TenantBranchTransID,ReceiptDateI18,ReceiptNo,Amount,PaymentType,PayMode,KeyValue,Summary,TransID FROM dbo.tfnGetStudentPaymentDetail(@TenantBranchID) ORDER BY VoucherDate ASC";
            try
            {
                paymentDetails = GetList<StudentPaymentDetails>(query, _sqlI18N, new { TenantBranchID = tenantBranchID });
            }
            catch(SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            return paymentDetails;
        }

        /// <summary>
        /// Get student paid fees
        /// </summary>
        /// <param name="batchMappingID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public decimal GetStudentPaidFees(Guid batchMappingID)
        {
            decimal result = 0;
            try
            {
                result = ExecuteScalar("SELECT dbo.sfnGetStudentPaidFees(@BatchMappingID)", new {BatchMappingID = batchMappingID}).ToDecimal();
            }
            catch (SqlException ex) 
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Get indent payment details
        /// </summary>
        /// <param name="branchMaterialIndentID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<PaymentDetails> GetPaymentDetails(Guid keyValue)
        {
            List<PaymentDetails> payments = new List<PaymentDetails>();
            string query = "SELECT TransactionDate,Summary,Amount,PayMode,TransID FROM dbo.tfnGetPaymentDetail(@KeyValue)";

            try
            {
                payments = GetList<PaymentDetails>(query, _sqlI18N, new { KeyValue = keyValue });
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return payments;
        }

        /// <summary>
        /// Reject paid request
        /// </summary>
        /// <param name="packageLogID"></param>
        /// <param name="reason"></param>
        /// <param name="actionBy"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int RejectPaidRequest(RejectPayment reject, Guid actionBy) {
            int result = 0;
            try {
                result = ExecuteScalar("uspRejectPaidRequest", new {PackageLogID = reject.PackageLogID, Reason = reject.Reason, UpdatedBy = actionBy }, commandType: System.Data.CommandType.StoredProcedure).ToInt();
            }
            catch( SqlException ex) {
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Get franchisee payments
        /// </summary>
        /// <param name="keyValue"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<Payments> GetFranchiseePayments(Guid keyValue) {
            List<Payments> payments = new List<Payments>();
            string query = "SELECT TransactionDate,Summary,Amount,IsCancelled,Reason FROM dbo.tfnGetFranchiseePaymentDetail(@KeyValue)";
            try {
                payments = GetList<Payments>(query, _sqlI18N, new { KeyValue = keyValue });
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return payments;
        }

        public DataSet GetFranchiseeInvoice(Guid tenantBranchID) {
            try {
                return GetDataSet("uspGetFranchiseeInvoice", new { TenantBranchID = tenantBranchID },
                    commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
        }

        public Guid GetPaymentTransID(Guid tenantBranchTransID, string transNo) {
            return (Guid) ExecuteScalar("SELECT TOP 1 TenantBranchFinanceRegisterID FROM dbo.TenantBranchFinanceRegister WHERE TenantBranchTransID = @TenantBranchTransID AND TransactionNo = @TransactionNo", new { TenantBranchTransID = tenantBranchTransID, TransactionNo = transNo });
        }
        #endregion
    }
}
