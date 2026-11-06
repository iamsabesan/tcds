using Dawn.Frameworks.Entities;
using Dawn.Frameworks.Helper;
using Dawn.Utilities.Enums;
using Dawn.Utilities.Exceptions;
using Dawn.Utilities.i18N;
using EntitiesDTO.Admin;
using EntitiesDTO.Material;
using EntitiesDTO.Payment;
using Frameworks.Entities;
using Frameworks.Helper;
using iCloud.Payment.Services;
using Microsoft.AspNetCore.Mvc;

namespace iCloud.Payment.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{v:apiVersion}/wallet/")]
    [ApiController]
    public class WalletV1Controller : ControllerBase
    {
        #region Fields
        private readonly WalletService _walletService;
        private readonly IdentityInfo _userIdentityInfo;
        #endregion
        #region Constructor
        public WalletV1Controller(SqlI18N sqlI18N, IHttpContextAccessor accessor, IDawnHttpClient dawnHttpClient)
        {
            _walletService = new WalletService(sqlI18N, accessor, dawnHttpClient);
            _userIdentityInfo = accessor.GetUserIdentityInfoDetails();
        }
        #endregion
        #region Methods
        /// <summary>
        /// Create wallet topup
        /// </summary>
        /// <param name="walletTopup"></param>
        /// <param createdBy="sakthivel"></param>
        /// <param createdDate="29-04-2023"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpPost("topup")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<APIResult<Guid>> CreateWalletTopup(NewWalletTopup walletTopup) {
            Guid walletID;
            try {
                walletID = await _walletService.CreateWalletTopup(walletTopup);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.NotExist);
            }
            return APIResult.Success(walletID, "Wallet topup request send successfully !", DawnStatusCode.Created);
        }

        /// <summary>
        /// Reject wallet topup request
        /// </summary>
        /// <param name="reject"></param>
        /// <param createdBy="sakthivel"></param>
        /// <param createdDate="29-04-2023"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpPost("reject")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<APIResult<Guid>> RejectWalletRequest(RejectWallet reject) {
            Guid walletID;
            try {
                walletID = await _walletService.RejectWalletRequest(reject);
            }
            catch(Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.NotExist);
            }
            return APIResult.Success(walletID, "Wallet topup request rejected !", DawnStatusCode.Created);
        }

        /// <summary>
        /// Approve wallet topup request
        /// </summary>
        /// <param name="approve"></param>
        /// <param createdBy="sakthivel"></param>
        /// <param createdDate="29-04-2023"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpPost("accept")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<APIResult<Guid>> ApproveWalletRequest(ApproveWalletRequest approve) {
            Guid walletID;
            try {
                walletID = await _walletService.ApproveWalletRequest(approve);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.NotExist);
            }
            return APIResult.Success(walletID, "Wallet request topup approved !", DawnStatusCode.Created);
        }

        /// <summary>
        /// Get wallet hostory
        /// </summary>
        /// <param name="tenantBranchID"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("history")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<GetNewWalletHistory> GetWalletHistory(string? tenantBranchID, DateTime fromDate, DateTime toDate) {
            GetNewWalletHistory walletHistory;
            try {
                Guid branchID = Guid.Empty;
                if(tenantBranchID == null) { 
                    branchID = _userIdentityInfo.TenantBranchID; 
                }
                else {
                    branchID = Guid.Parse(tenantBranchID);
                }
                walletHistory = await _walletService.GetWalletHistory(branchID, fromDate, toDate);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.NotExist);
            }
            return walletHistory;
        }

        /// <summary>
        /// Get franchisee wallet request list
        /// </summary>
        /// <param createdBy="sakthivel"></param>
        /// <param createdDate="02-05-2023"></param>
        /// <returns>
        /// Returns franchisee wallet request list
        /// </returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("list")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<IEnumerable<GetWalletRequestList>> GetWalletRequestList() {
            IEnumerable<GetWalletRequestList> requestLists;
            try {
                requestLists = await _walletService.GetWalletRequestList();
                return requestLists;
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.NotExist);
            }
        }

        /// <summary>
        /// Get franchisee wallet balance
        /// </summary>
        /// <param name="tenantBranchID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("walletbalance/{tenantBranchID}")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public decimal GetWalletBalance(Guid tenantBranchID) {
            decimal balance;
            try {
                balance = _walletService.GetWalletBalance(tenantBranchID);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return balance;
        }

        /// <summary>
        /// Get branch wallet transaction details
        /// </summary>
        /// <param name="tenantBranchID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("branch/transaction/{tenantBranchID}")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public List<Wallet> GetBranchWalletTrans(Guid tenantBranchID) {
            List<Wallet> trans;
            try {
                trans = _walletService.GetBranchWalletTrans(tenantBranchID);
            }
            catch(Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return trans;
        }

        /// <summary>
        /// Get wallet summary
        /// </summary>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("summary")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public WalletSummary GetWalletSummary() {
            WalletSummary wallet;
            try {
                wallet = _walletService.GetWalletSummary();
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return wallet;
        }

        /// <summary>
        /// Get payment id by using keyvalue
        /// </summary>
        /// Author : Sakthivel B
        /// CreatedDate : 21-06-2024
        /// <param name="keyValue"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("paymentid")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public GetPaymentID GetPaymentID(Guid keyValue) {
            GetPaymentID result;
            try {
                result = _walletService.GetPaymentID(keyValue);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return result;
        }

        [HttpGet("payment-details")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public List<FranchiseeToAdminPay> GetPaymentItems(Guid keyValue) {
            List<FranchiseeToAdminPay> result;
            try {
                result = _walletService.GetPaymentItems(keyValue);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return result;
        }
        
        ///// <summary>
        ///// Get branch wise admission fees list
        ///// </summary>
        ///// Author : Sakthivel B
        ///// <returns></returns>
        ///// <exception cref="DawnUserException"></exception>
        //[HttpGet("admission-fees/list")]
        //[DawnAuthorize("TenantAdmin", "TenantUser")]
        //public List<PurchaseAmount> GetAdmissionFeesList() {
        //    List<PurchaseAmount> result;
        //    try {
        //        result = _walletService.GetAdmissionFeesList();
        //        return result;
        //    }
        //    catch (Exception ex) {
        //        throw new DawnUserException(ex.Message, DawnStatusCode.Error);
        //    }
        //}

        [HttpPost("renewal/payment")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public APIResult<int> CreateRenewalPayment(WalletRenewalPayment payment) {
            int result = 0;
            try {
                result = _walletService.CreateRenewalPayment(payment);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return APIResult.Success(result, "", DawnStatusCode.Created);
        }
        [HttpPost("history-v1")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<List<GetNewWalletHistoryV1>> GetWalletHistoryV1(WalletHistory history)
        {
            List<GetNewWalletHistoryV1> walletHistory;
            try
            {
                walletHistory = await _walletService.GetWalletHistoryV1(history.TenantBranchID, history.Fromdate, history.Todate);
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.NotExist);
            }
            return walletHistory;
        }
        #endregion
    }
}
