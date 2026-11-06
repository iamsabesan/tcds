using Dawn.Database;
using Dawn.Utilities.Extensions;
using Dawn.Utilities.i18N;
using EntitiesDTO.Admin;
using EntitiesDTO.Material;
using EntitiesDTO.Payment;
using Microsoft.Data.SqlClient;
using System.Data;

namespace iCloud.Payment.Repository {
    public class WalletRepository : DawnDbClient {
        #region Fields
        private readonly SqlI18N _sqlI18N;
        #endregion
        #region Constructor
        public WalletRepository (SqlI18N SqlI18N, string ConnectionString) : base(ConnectionString) {
            _sqlI18N = SqlI18N;
        }
        #endregion
        #region Methods
        /// <summary>
        /// Create wallet topup
        /// </summary>
        /// <param name="wallet"></param>
        /// <param name="paymentItems"></param>
        /// <param name="actionBy"></param>
        /// <param createdBy="sakthivel"></param>
        /// <param createdDate=""></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public Guid CreateWalletTopup (WalletTopup wallet, DataTable PaymentItems, Guid actionBy, Guid walletID) {
            Guid result;
            try {
                result = (Guid)ExecuteScalar("uspCreateWalletTopup", new {
                    wallet.TenantBranchID,
                    wallet.SenderID,
                    wallet.ReceiverID,
                    wallet.WalletAmount,
                    PaymentItems,
                    CreatedBy = actionBy,
                    wallet.ContactID,
                    wallet.TransactionType,
                    wallet.WalletOut,
                    wallet.Description,
                    wallet.Status,
                    wallet.TransactionName,
                    wallet.VoucherDate,
                    WalletID = walletID,
                }, commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Reject wallet topup request
        /// </summary>
        /// <param name="reject"></param>
        /// <param name="actionBy"></param>
        /// <param createdBy="sakthivel"></param>
        /// <param createdDate=""></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public Guid RejectWalletRequest (RejectWallet reject, Guid actionBy) {
            Guid result = Guid.Empty;
            try {
                result = (Guid)ExecuteScalar("uspRejectWalletRequest", new {
                    WalletID = reject.WalletID,
                    Reason = reject.Reason,
                    UpdatedBy = actionBy
                }, commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Approve wallet topup request
        /// </summary>
        /// <param name="approve"></param>
        /// <param name="actionBy"></param>
        /// <param createdBy="sakthivel"></param>
        /// <param createdDate=""></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public Guid ApproveWalletRequest (ApproveWalletRequest approve, Guid actionBy) {
            Guid result;
            try {
                result = (Guid)ExecuteScalar("uspApproveWalletRequest", new {
                    WalletID = approve.WalletID,
                    UpdatedBy = actionBy
                }, commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Get wallet history
        /// </summary>
        /// <param name="tenantBranchID"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param createdBy="sakthivel"></param>
        /// <param createdDate=""></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public DataSet GetWalletHistory (Guid tenantBranchID, DateTime fromDate, DateTime toDate) {
            try {
                return GetDataSet("uspGetWalletHistory", new {
                    TenantBranchID = tenantBranchID,
                    FromDate = fromDate,
                    ToDate = toDate
                }, commandType: CommandType.StoredProcedure);
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Check wallet id is exists
        /// </summary>
        /// <param name="walletID"></param>
        /// <param createdBy="sakthivel"></param>
        /// <param createdDate=""></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public bool CheckWalletIDExists (Guid walletID) {
            try {
                var result = ExecuteScalar("SELECT dbo.sfnCheckWalletIDExists(@WalletID)", new {
                    WalletID = walletID
                }).ToBoolean();
                return result;
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Get wallet balance
        /// </summary>
        /// <param name="tenantBranchID"></param>
        /// <param createdBy="sakthivel"></param>
        /// <param createdDate="29-04-2023"></param>
        /// <returns>
        /// Returns franchisee current wallet balance
        /// </returns>
        /// <exception cref="Exception"></exception>
        public decimal GetWalletBalance (Guid tenantBranchID) {
            decimal result = 0;
            try {
                result = (decimal)ExecuteScalar("SELECT dbo.sfnGetWalletBalance(@TenantBranchID)", new {
                    TenantBranchID = tenantBranchID
                });
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Get wallet payment details
        /// </summary>
        /// <param createdBy="sakthivel"></param>
        /// <param createdDate="29-04-2023"></param>
        /// <returns>
        /// Returns wallet payment details
        /// </returns>
        /// <exception cref="Exception"></exception>
        public DataSet GetWalletPaymentDetail () {
            try {
                return GetDataSet("uspWalletPaymentDetails", commandType: CommandType.StoredProcedure);
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Get branch wallet transaction detail
        /// </summary>
        /// <param name="tenantBranchID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<Wallet> GetBranchWalletTrans (Guid tenantBranchID) {
            List<Wallet> result = new List<Wallet>();
            try {
                result = GetList<Wallet>("uspGetBranchWalletTrans", _sqlI18N, new { TenantBranchID = tenantBranchID }, commandType: CommandType.StoredProcedure);
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Get wallet summary
        /// </summary>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public WalletSummary GetWalletSummary () {
            WalletSummary wallet = new WalletSummary();
            string query = "SELECT WalletBalance,WalletPending FROM dbo.tfnGetWalletSummaryByAdmin()";
            try {
                wallet = Get<WalletSummary>(query, _sqlI18N);
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return wallet;
        }

        public List<GetPaymentDetails> GetPaymentItems (Guid keyValue) {
            string query = "SELECT ISNULL(f.PaymentDetails,'') AS PaymentDetails FROM dbo.TenantBranchReceiptAndPayment rp INNER JOIN dbo.TenantBranchFinanceRegister f ON f.TenantBranchTransID = rp.TenantBranchTransID WHERE rp.IsCancelled = 0 AND rp.IsDeleted = 0 AND rp.KeyValue = @KeyValue";
            try {
                return GetList<GetPaymentDetails>(query, _sqlI18N, new { KeyValue = keyValue });
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
        }

        public GetPaymentID GetPaymentID (Guid keyValue) {
            string query = "SELECT TenantBranchID,TenantBranchTransID FROM dbo.TenantBranchReceiptAndPayment WHERE KeyValue = @KeyValue";
            try {
                return Get<GetPaymentID>(query, _sqlI18N, new { KeyValue = keyValue });
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
        }

        public List<PurchaseAmount> GetAdmissionFeesList (int year) {
            string query = "SELECT TenantBranchID,SUM(ISNULL(WalletOut, 0)) AS Total FROM dbo.Wallet WHERE WalletType = 'ADMISSION' AND Status = 'APPROVED' AND IsDeleted = 0 AND YEAR(CreatedDate) = @Year GROUP BY TenantBranchID;";
            try {
                return GetList<PurchaseAmount>(query, _sqlI18N, new { Year = year });
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
        }

        public decimal GetTopupAmount (Guid walletID) {
            return ExecuteScalar("SELECT (WalletIn+WalletOut) FROM dbo.Wallet WHERE WalletID = @WalletID", new { WalletID = walletID }).ToDecimal();
        }

        public int CreateRenewalPayment (WalletRenewalPayment payment) {
            int result = 0;
            try {
                result = ExecuteScalar("uspRenewalPayment", new {
                    WalletType = payment.WalletType,
                    TenantBranchID = payment.TenantBranchID,
                    SenderID = payment.SenderID,
                    ReceiverID = payment.ReceiverID,
                    WalletOut = payment.WalletOut,
                    Description = payment.Description,
                    CreatedBy = payment.CreatedBy
                }, commandType: System.Data.CommandType.StoredProcedure).ToInt();
                return result;
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
        }
        public string GetPayModeType (Guid payModeId)
        {
            try
            {
                string query = "SELECT PayMode FROM dbo.PayModeType WHERE PayModeID = @PayModeID AND IsDeleted = 0";
                return (string)ExecuteScalar(query, new
                {
                    PayModeID = payModeId
                });
            }
            catch (Exception)
            {

                throw;
            }
        }
        public DataSet GetWalletHistoryV1(DataTable tenantBranchIDs, DateTime fromDate, DateTime toDate)
        {
            try
            {
                return GetDataSet("uspGetWalletHistoryV1", new
                {
                    TenantBranchIDs = tenantBranchIDs,
                    FromDate = fromDate.Date,
                    ToDate = toDate.Date
                }, commandType: CommandType.StoredProcedure);
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion
    }
}
