using Dawn.Frameworks.Entities;
using Dawn.Frameworks.Helper;
using Dawn.Utilities.Enums;
using Dawn.Utilities.Exceptions;
using Dawn.Utilities.i18N;
using EntitiesDTO.Admin;
using EntitiesDTO.Payment;
using Frameworks.Entities;
using Frameworks.Helper;
using iCloud.Payment.Services;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Utilities.Extensions;

namespace iCloud.Payment.Controllers {
    [ApiVersion("1.0")]
    [Route("api/v{v:apiVersion}/payment/")]
    [ApiController]
    public class PaymentV1Controller : ControllerBase {
        #region Fields
        private readonly IdentityInfo _userIdentityInfo;
        private readonly PaymentService _paymentService;
        #endregion
        #region Constructor
        public PaymentV1Controller (SqlI18N sqlI18N, IHttpContextAccessor accessor, IDawnHttpClient dawnHttpClient) {
            _userIdentityInfo = accessor.GetUserIdentityInfoDetails();
            _paymentService = new PaymentService(sqlI18N, accessor, dawnHttpClient);
        }
        #endregion
        #region Methods
        /// <summary>
        /// Get paymode list
        /// </summary>
        /// <param createdBy="Sakthivel"></param>
        /// <param createdDate="13-04-2023"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("list/paymode")]
        [DawnAuthorize("TenantAdmin", "TenantUser", "Guest")]
        public List<GetPaymode> GetPaymodeList () {
            List<GetPaymode> paymodes;
            try {
                paymodes = _paymentService.GetPaymodeList();
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return paymodes;
        }

        /// <summary>
        /// Create franchisee registration payment
        /// </summary>
        /// <param name="createPayment"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <param createdDate="13-04-2023"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpPost("create")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<Guid> CreatePayment (Receipt createPayment) {
            Guid paymentID;
            try {
                NewReceipt newReceipt = createPayment.Mapper<NewReceipt>();
                paymentID = await _paymentService.CreatePayment(newReceipt);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.NotExist);
            }
            return paymentID;
        }

        /// <summary>
        /// Rollback payment by keyvalue
        /// </summary>
        /// <param name="keyValue"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("rollback/payment/{keyValue}")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public APIResult<string> RollBackPayment (Guid keyValue) {
            string result;
            try {
                result = _paymentService.RollBackPayment(keyValue);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.NotExist);
            }
            return APIResult.Success("", result, DawnStatusCode.Created);
        }

        [HttpGet("rollback/part-payment/{tenantBranchTransID}")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public APIResult<string> RollBackPartPayment (Guid tenantBranchTransID) {
            string result;
            try {
                result = _paymentService.RollBackPartPayment(tenantBranchTransID);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.NotExist);
            }
            return APIResult.Success("", result, DawnStatusCode.Created);
        }

        /// <summary>
        /// Create student downpayment
        /// </summary>
        /// <param name="payment"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpPost("downpayment")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<APIResult<Guid>> CreateStudentDownPayment (DownPayment payment) {
            Guid result;
            try {
                result = await _paymentService.CreateStudentDownPayment(payment);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.NotExist);
            }
            return APIResult.Success(result, "Student down payment processed successfully !", DawnStatusCode.Created);
        }

        /// <summary>
        /// Create student part payment
        /// </summary>
        /// <param name="payment"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpPost("partpayment")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<APIResult<Guid>> CreateStudentPartPayment (PartPayment payment) {
            Guid transID = Guid.NewGuid();
            try {
                transID = await _paymentService.CreateStudentPartPayment(payment);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return APIResult.Success(transID, "Student part payment processed successfully !", DawnStatusCode.Created);
        }

        /// <summary>
        /// Get payment details
        /// </summary>
        /// <param createdBy="Sakthivel"></param>
        /// <param createdDate="18-04-2023"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("paymentdetails")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public DataSet GetPaymentDetail () {
            DataSet paymodes;
            try {
                paymodes = _paymentService.GetPaymentDetail();
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return paymodes;
        }

        /// <summary>
        /// Get student payment details
        /// </summary>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("studentpayment-detail")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public List<StudentPaymentDetails> GetStudentPaymentDetails () {
            List<StudentPaymentDetails> studentPaymentDetails;
            try {
                studentPaymentDetails = _paymentService.GetStudentPaymentDetails();
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return studentPaymentDetails;
        }

        /// <summary>
        /// Get student paid fees
        /// </summary>
        /// <param name="batchMappingID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("paidfees")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public decimal GetStudentPaidFees (Guid batchMappingID) {
            decimal result;
            try {
                result = _paymentService.GetStudentPaidFees(batchMappingID);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return result;
        }

        /// <summary>
        /// Get indent payment details
        /// </summary>
        /// <param name="branchMaterialIndentID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("paymentdetails/{keyValue}")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public List<PaymentDetails> GetPaymentDetails (Guid keyValue) {
            List<PaymentDetails> payments;
            try {
                payments = _paymentService.GetPaymentDetails(keyValue);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return payments;
        }

        /// <summary>
        /// Reject paid request
        /// </summary>
        /// <param name="packageLogID"></param>
        /// <param name="reason"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpPost("reject/paid/request")]
        [DawnAuthorize("TenantAdmin")]
        public APIResult<string> RejectPaidRequest (RejectPayment reject) {
            string result;
            try {
                result = _paymentService.RejectPaidRequest(reject);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return APIResult.Success("", result, DawnStatusCode.Created);
        }

        /// <summary>
        /// Get franchisee payment details
        /// </summary>
        /// <param name="keyValue"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("franchisee/payments/{keyValue}")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public List<Payments> GetFranchiseePayments (Guid keyValue) {
            List<Payments> payments;
            try {
                payments = _paymentService.GetFranchiseePayments(keyValue);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return payments;
        }
        /// <summary>
        /// Get franchisee invoice list
        /// </summary>
        /// Author : Sakthivel B
        /// CreatedDate : 20-06-2024
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("franchisee/invoice")]
        [DawnAuthorize("TenantUser")]
        public List<NewInvoiceMaster> GetFranchiseeInvoice () {
            try {
                return _paymentService.GetFranchiseeInvoice();
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }

        [HttpGet("trans-id")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public Guid GetPaymentTransID (Guid tenantBranchTransID, string transNo) {
            try {
                return _paymentService.GetPaymentTransID(tenantBranchTransID, transNo);
            }
            catch(Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }
        #endregion
    }
}
