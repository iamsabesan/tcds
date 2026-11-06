using CommonServices;
using Dawn.Frameworks.Helper;
using Dawn.Utilities.Exceptions;
using Dawn.Utilities.Extensions;
using Dawn.Utilities.i18N;
using EntitiesDTO.Admin;
using EntitiesDTO.Payment;
using EntitiesDTO.Utilities;
using Frameworks.Entities;
using Frameworks.Helper;
using iCloud.Payment.Repository;
using System.Data;
using Utilities.Extensions;

namespace iCloud.Payment.Services
{
    public class PaymentService
    {
        #region Fields
        private readonly SqlI18N _sqlI18N;
        private readonly IdentityInfo _userIdentityInfo;
        private string _connectionString;
        private readonly PaymentRepository _paymentRepository;
        private readonly WalletRepository _walletRepository;
        private readonly CommonService _commonService;
        #endregion
        #region Constructor
        public PaymentService(SqlI18N sqlI18N, IHttpContextAccessor accessor, IDawnHttpClient dawnHttpClient)
        {
            _sqlI18N = sqlI18N;
            _userIdentityInfo = accessor.GetUserIdentityInfoDetails();
            _connectionString = accessor.GetConnectionString(dawnHttpClient);
            _commonService = new CommonService(dawnHttpClient);
            _paymentRepository = new PaymentRepository(_sqlI18N, _connectionString);
            _walletRepository = new WalletRepository(_sqlI18N, _connectionString);
        }
        #endregion
        #region Methods
        /// <summary>
        /// Get paymode list
        /// </summary>
        /// <param createdBy="Sakthivel"></param>
        /// <param createdDate="13-04-2023"></param>
        /// <returns></returns>
        public List<GetPaymode> GetPaymodeList() {
            return _paymentRepository.GetPaymodeList();
        }

        /// <summary>
        /// Create franchisee registration payment
        /// </summary>
        /// <param name="createPayment"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <param createdDate="13-04-2023"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<Guid> CreatePayment(NewReceipt createPayment) {
            createPayment.TenantBranchID = createPayment.TenantBranchID;
            createPayment.ContactID = _userIdentityInfo.ContactID;
            createPayment.CreatedBy = createPayment.CreatedBy;
            DataTable paymentData = createPayment.PaymentItems.ToDataTable();
            Guid paymentID = _paymentRepository.CreatePayment(createPayment, paymentData);
            if (paymentID == Guid.Empty) {
                throw new DawnUserException("Payment process failed !");
            }
            return paymentID;
        }

        /// <summary>
        /// Rollback payment by keyvalue
        /// </summary>
        /// <param name="keyValue"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        public string RollBackPayment(Guid keyValue) {
            int result = _paymentRepository.RollBackPayment(keyValue);
            return result > 0 ? "Payment roll backed" : "Couldn't roll back payment";
        }

        public string RollBackPartPayment(Guid tenantBranchTransID) {
            int result = _paymentRepository.RollBackPartPayment(tenantBranchTransID);
            return result > 0 ? "Payment roll backed" : "Couldn't roll back payment";
        }

        /// <summary>
        /// Create student down payment
        /// </summary>
        /// <param name="payment"></param>
        /// <param createdBy="sakthivel"></param>
        /// <param createdDate="17-04-2023"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<Guid> CreateStudentDownPayment(DownPayment payment) {
            if (_walletRepository.GetWalletBalance(payment.TenantBranchID) < payment.WalletAmount)
                throw new DawnUserException("Not enogh wallet balance !");
            payment.TenantBranchID = _userIdentityInfo.TenantBranchID;
            payment.CreatedBy = _userIdentityInfo.ContactID;
            DataTable paymentData = payment.PaymentItems.ToDataTable();
            Guid result = _paymentRepository.CreateStudentDownPayment(payment, paymentData);
            if (result == Guid.Empty) {
                throw new DawnUserException("Could not make student down payment");
            }
            return result;
        }

        /// <summary>
        /// Create student part payment
        /// </summary>
        /// <param name="payment"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<Guid> CreateStudentPartPayment(PartPayment payment) {
            payment.TenantBranchID = _userIdentityInfo.TenantBranchID;
            payment.CreatedBy = _userIdentityInfo.ContactID;
            //payment.TransactionName = "PART PAYMENT";
            payment.TransactionName = "INSTALMENT FEES"; // Changed on 08/08/25
            DataTable paymentData = payment.PaymentItems.ToDataTable();
            Guid result = _paymentRepository.CreateStudentPartPayment(payment, paymentData);
            if (result == Guid.Empty) {
                throw new DawnUserException("Could not make student part payment");
            }
            return result;
        }

        /// <summary>
        /// Get franchisee payment details
        /// </summary>
        /// <param createdBy="sakthivel"></param>
        /// <param createdDate="18-04-2023"></param>
        /// <returns></returns>
        public DataSet GetPaymentDetail() {
            return _paymentRepository.GetPaymentDetails();
        }

        /// <summary>
        /// Get student payment details
        /// </summary>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        public List<StudentPaymentDetails> GetStudentPaymentDetails() {
            return _paymentRepository.GetStudentPaymentDetails(_userIdentityInfo.TenantBranchID);
        }

        /// <summary>
        /// Get student paid fees
        /// </summary>
        /// <param name="batchMappingID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        public decimal GetStudentPaidFees(Guid batchMappingID) {
            return _paymentRepository.GetStudentPaidFees(batchMappingID);
        }

        /// <summary>
        /// Get indent payment details
        /// </summary>
        /// <param name="branchMaterialIndentID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        public List<PaymentDetails> GetPaymentDetails(Guid keyValue) {
            return _paymentRepository.GetPaymentDetails(keyValue);
        }

        /// <summary>
        /// Reject paid request
        /// </summary>
        /// <param name="reject"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        public string RejectPaidRequest(RejectPayment reject) {
            int count = _paymentRepository.RejectPaidRequest(reject, _userIdentityInfo.ContactID);
            if(count > 0) {
                return "Payment rejected";
            }
            else {
                throw new DawnUserException("Could't reject payment");
            }
        }

        /// <summary>
        /// Get franchisee payment details
        /// </summary>
        /// <param name="keyValue"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        public List<Payments> GetFranchiseePayments(Guid keyValue) {
            return _paymentRepository.GetFranchiseePayments(keyValue);
        }

        public List<NewInvoiceMaster> GetFranchiseeInvoice() {
            List<NewInvoiceMaster> masters = new List<NewInvoiceMaster>();
            DataSet ds = _paymentRepository.GetFranchiseeInvoice(_userIdentityInfo.TenantBranchID);
            var invoiceMaster = ds.Tables[0].DataTablesToList<InvoiceMaster>();
            foreach(var invoice in invoiceMaster) {
                NewInvoiceMaster list = invoice.Mapper<NewInvoiceMaster>();
                list.PaymentDetails = ds.Tables[1].DataTablesToList<InvoiceTrans>().Where(x => x.TenantBranchTransID == invoice.TenantBranchTransID).ToList();
                masters.Add(list);
            }
            return masters;
        }

        public Guid GetPaymentTransID (Guid tenantBranchTransID, string transNo) {
            return _paymentRepository.GetPaymentTransID(tenantBranchTransID, transNo);
        }
        #endregion
    }
}
