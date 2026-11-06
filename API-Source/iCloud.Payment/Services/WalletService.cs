using CommonServices;
using Dawn.Frameworks.Helper;
using Dawn.Utilities.Exceptions;
using Dawn.Utilities.Extensions;
using Dawn.Utilities.i18N;
using EntitiesDTO.Admin;
using EntitiesDTO.Material;
using EntitiesDTO.Payment;
using EntitiesDTO.Users;
using EntitiesDTO.Utilities;
using Frameworks.Entities;
using Frameworks.Helper;
using iCloud.Payment.Repository;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using Utilities.Extensions;

namespace iCloud.Payment.Services
{
    public class WalletService
    {
        #region Fields
        private readonly SqlI18N _sqlI18N;
        private readonly IdentityInfo _userIdentityInfo;
        private string _connectionString;
        private readonly WalletRepository _walletRepository;
        private readonly CommonService _commonService;
        private readonly IDawnHttpClient _dawnHttpClient;
        #endregion
        #region Constructor
        public WalletService(SqlI18N sqlI18N, IHttpContextAccessor accessor, IDawnHttpClient dawnHttpClient)
        {
            _sqlI18N = sqlI18N;
            _userIdentityInfo = accessor.GetUserIdentityInfoDetails();
            _connectionString = accessor.GetConnectionString(dawnHttpClient);
            //_connectionString = "Data Source=3.108.251.251;Initial Catalog=iCloud_Payment_98392E8C_A6FC_48D6_873D_5A6C7C514B94_TCDS;Persist Security Info=True;User ID=Identity_db ;Password=Ljgl5871$;TrustServerCertificate=Yes;";
            _commonService = new CommonService(dawnHttpClient);
            _walletRepository = new WalletRepository(_sqlI18N, _connectionString);
            _dawnHttpClient = dawnHttpClient;
        }
        #endregion
        #region Methods
        /// <summary>
        /// Create wallet topup
        /// </summary>
        /// <param name="newWallet"></param>
        /// <param createdBy="sakthivel"></param>
        /// <param createdDate="29-04-2023"></param>
        /// <returns>
        /// Returns wallet id
        /// </returns>
        /// <exception cref="Exception"></exception>
        public async Task<Guid> CreateWalletTopup(NewWalletTopup newWallet) {
            Guid contactID = _userIdentityInfo.ContactID;
            string receiverContactID = await _commonService.GetAdminContactID();
            string receiverBranchID = await _commonService.GetAdminBranchID(_userIdentityInfo.TenantID);
            var payModeIDs = newWallet.PaymentItems.Select(p => p.PaymentModeID);
            var payModeType = payModeIDs.Select(id => _walletRepository.GetPayModeType(id)).Where(type => !string.IsNullOrWhiteSpace(type)).Distinct().ToList();
            WalletTopup wallet = new WalletTopup();
            wallet.TenantBranchID = _userIdentityInfo.TenantBranchID;
            wallet.SenderID = contactID;
            wallet.ContactID = contactID;
            wallet.ReceiverID = Guid.Parse(receiverContactID);
            wallet.WalletAmount = newWallet.WalletAmount;
            wallet.TransactionType = "WALLET IN";
            wallet.WalletOut = 0;
            //wallet.Description = "WALLET TOPUP";
            wallet.Description = $"WALLET TOPUP ({string.Join(", ", payModeType)})";
            wallet.Status = "PENDING";
            string transactionName = "WALLET TOPUP";
            wallet.TransactionName = transactionName;
            //wallet.VoucherDate = DateTime.Now.Date.ToString().ToDateOnly();
            wallet.VoucherDate = DateTime.Now.Date.ToString("yyyy-MM-dd");
            List<NewPaymentItems> newItems = new List<NewPaymentItems>();
            foreach (var paymentItems in newWallet.PaymentItems) {
                FranchiseeToAdminPay paymentDetails = paymentItems.Mapper<FranchiseeToAdminPay>();
                var paymentDetail = JsonConvert.SerializeObject(paymentDetails);
                NewPaymentItems newPaymentItems = paymentItems.Mapper<NewPaymentItems>();
                newPaymentItems.BankName = paymentItems.FromLedgerName;
                newPaymentItems.TransactionDate = paymentItems.TransactionDate;
                newPaymentItems.PaymentDetails = paymentDetail;
                newItems.Add(newPaymentItems);
            }
            DataTable paymentItem = newItems.ToDataTable();
            Guid walletID = Guid.NewGuid();
            Guid result = _walletRepository.CreateWalletTopup(wallet, paymentItem, _userIdentityInfo.ContactID, walletID);
            TempDocumentActivate temp = new TempDocumentActivate();
            List<DocumentItem> documents = new List<DocumentItem>();
            foreach (var item in newWallet.PaymentItems) {
                if (item.DocumentID != Guid.Empty) {
                    string transID = await _commonService.GetPaymentTransID(result, item.TransactionNo);
                    documents.Add(new DocumentItem { DocumentID = item.DocumentID, EntityID = Guid.Parse(transID) });
                    temp.DocumentItem = documents;
                }
            }
            if(temp.DocumentItem != null) {
                var activateDocument = await _commonService.TempDocumentActivated(temp);
            }
            if (result == Guid.Empty)
                throw new DawnUserException("Wallet topup process failed !");
            bool posting = await _commonService.FranchiseeToAdmin(newWallet.PaymentItems, _userIdentityInfo.TenantBranchID, Guid.Parse(receiverBranchID), _userIdentityInfo.ContactID, Guid.Parse(receiverContactID), result, transactionName);
            return result;
        }

        /// <summary>
        /// Reject wallet topup request
        /// </summary>
        /// <param name="reject"></param>
        /// <param createdBy="sakthivel"></param>
        /// <param createdDate="02-05-2023"></param>
        /// <returns>
        /// Returns wallet id
        /// </returns>
        /// <exception cref="Exception"></exception>
        public async Task<Guid> RejectWalletRequest(RejectWallet reject) {
            if (!_walletRepository.CheckWalletIDExists(reject.WalletID))
                throw new DawnUserException("Invalid wallet id");
            GetPaymentID paymentID = GetPaymentID(reject.WalletID);
            string receiverContactID = await _commonService.GetFranchiseeContactID(paymentID.TenantBranchID);
            string giverContactID = await _commonService.GetAdminContactID();
            Guid giverBranchID = _userIdentityInfo.TenantBranchID;
            string transactionName = "WALLET TOPUP REJECTED";
            List<FranchiseeToAdminPay> payments = GetPaymentItems(reject.WalletID);
            bool posting = await _commonService.FranchiseeToAdminReEntry(payments, giverBranchID, paymentID.TenantBranchID, Guid.Parse(giverContactID), Guid.Parse(receiverContactID), paymentID.TenantBranchTransID, transactionName);
            return _walletRepository.RejectWalletRequest(reject,_userIdentityInfo.ContactID);
        }

        /// <summary>
        /// Approve wallet topup request
        /// </summary>
        /// <param name="approve"></param>
        /// <param createdBy="sakthivel"></param>
        /// <param createdDate="02-05-2023"></param>
        /// <returns>
        /// Returns wallet id
        /// </returns>
        /// <exception cref="Exception"></exception>
        public async Task<Guid> ApproveWalletRequest(ApproveWalletRequest approve) {
            if (!_walletRepository.CheckWalletIDExists(approve.WalletID))
                throw new DawnUserException("Invalid wallet id");
            GetPaymentID paymentID = GetPaymentID(approve.WalletID);
            string receiverContactID = await _commonService.GetFranchiseeContactID(paymentID.TenantBranchID);
            string giverContactID = await _commonService.GetAdminContactID();
            Guid giverBranchID = _userIdentityInfo.TenantBranchID;
            string transactionName = "WALLET TOPUP APPROVED";
            NewPaymentDetails details = new NewPaymentDetails();
            var branchBanks = await _commonService.GetBranchBankList(paymentID.TenantBranchID);
            var adminBanks = await _commonService.GetAdminBankList();
            var branchBank = branchBanks.Where(x => x.LedgerName.ToConditionFormat() == "WALLETBANK").FirstOrDefault();
            var adminBank = adminBanks.Where(x => x.LedgerName.ToConditionFormat() == "WALLETBANK").FirstOrDefault();
            decimal topupAmount = _walletRepository.GetTopupAmount(approve.WalletID);
            details.PaymentModeID = Guid.Empty;
            details.PayModeType = "";
            details.PayModeValue = topupAmount;
            details.Amount = topupAmount;
            details.FromLedgerID = adminBank.LedgerID;
            details.FromLedgerName = adminBank.LedgerName;
            details.ToLedgerID = branchBank.LedgerID;
            details.ToLedgerName = branchBank.LedgerName;
            details.TransactionDate = DateTime.Now.ToString();
            bool posting = await _commonService.WalletToWalletEntry(details, giverBranchID, paymentID.TenantBranchID, Guid.Parse(giverContactID), Guid.Parse(receiverContactID), paymentID.TenantBranchTransID, transactionName);
            return _walletRepository.ApproveWalletRequest(approve, _userIdentityInfo.ContactID);
        }

        /// <summary>
        /// Get wallet request history
        /// </summary>
        /// <param name="tenantBranchID"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param createdBy="sakthivel"></param>
        /// <param createdDate="02-05-2023"></param>
        /// <returns>
        /// Returns franchisee wallet request history
        /// </returns>
        public async Task<GetNewWalletHistory> GetWalletHistory(Guid tenantBranchID,DateTime fromDate,DateTime toDate) {
            GetNewWalletHistory walletHistory = new GetNewWalletHistory();
            DataSet walletDataSet = _walletRepository.GetWalletHistory(tenantBranchID, fromDate, toDate);
            walletHistory.OpeningBalance = walletDataSet.Tables[0].DataTablesToItem<GetWalletOpeningBalance>();
            walletHistory.WalletBalance = walletDataSet.Tables[1].DataTablesToItem<GetWalletBalance>();
            walletHistory.WalletHistory = walletDataSet.Tables[2].DataTablesToList<GetWalletHistory>();
            return walletHistory;
        }

        /// <summary>
        /// Get wallet request list
        /// </summary>
        /// <param createdBy="sakthivel"></param>
        /// <param createdDate="02-05-2023"></param>
        /// <returns>
        /// Returns franchisee wallet request list
        /// </returns>
        public async Task<IEnumerable<GetWalletRequestList>> GetWalletRequestList() {
            List<GetWalletRequestList> walletRequestList = new List<GetWalletRequestList>();
            var franchiseeIds = await _commonService.GetFranchiseeID();
            DataSet walletDataSet = _walletRepository.GetWalletPaymentDetail();
            var getWalletID = walletDataSet.Tables[0].DataTablesToList<GetWalletID>();
            var payments = walletDataSet.Tables[1].DataTablesToList<WalletPaymentDetail>();
            DataTable branchIdsTable = new DataTable();
            branchIdsTable.Columns.Add("TenantBranchID", typeof(Guid));
            foreach (var item in getWalletID)
            {
                branchIdsTable.Rows.Add(item.TenantBranchID);
            }
            string franchiseeConn = await GetConnectionString(_userIdentityInfo.TenantID, "Franchisee");
            var commonRepo = new CommonRepository(_sqlI18N, franchiseeConn);
            var branchDetailsList = commonRepo.GetBranchDetail(branchIdsTable);
            foreach (var wallet in getWalletID) {
                GetFranchiseeCode franchiseeCode = new GetFranchiseeCode();
                franchiseeCode = franchiseeIds.Where(x => x.TenantBranchID == wallet.TenantBranchID).FirstOrDefault();
                if (franchiseeCode != null) {
                    GetWalletRequestList requestList = new GetWalletRequestList();
                    requestList.WalletID = wallet.WalletID;
                    requestList.Date = wallet.Date;
                    requestList.BranchCode = franchiseeCode.FranchiseeCode;
                    requestList.Amount = wallet.Amount;
                    requestList.WalletBalance = _walletRepository.GetWalletBalance(wallet.TenantBranchID);
                    //requestList.BranchDetail = await _commonService.GetBranchDetails(wallet.TenantBranchID);
                    requestList.BranchDetail = branchDetailsList.Where(a=> a.TenantBranchID == wallet.TenantBranchID).FirstOrDefault()?? new GetBranchContactDetail();
                    requestList.PaymentDetail = await GetWalletPaymentDetail(wallet.WalletID, payments);
                    walletRequestList.Add(requestList);
                }
            }
            return walletRequestList;
        }

        public async Task<List<WalletPaymentDetail>> GetWalletPaymentDetail(Guid walletID, List<WalletPaymentDetail> details) {
            List<WalletPaymentDetail> wallet = new List<WalletPaymentDetail>();
            var payments = details.Where(x => x.WalletID == walletID).ToList();
            foreach (WalletPaymentDetail walletPaymentDetail in payments) {
                WalletPaymentDetail paymentDetail = new WalletPaymentDetail();
                paymentDetail.TransactionDate = walletPaymentDetail.TransactionDate;
                paymentDetail.Summary = walletPaymentDetail.Summary;
                paymentDetail.PayMode = walletPaymentDetail.PayMode;
                paymentDetail.Amount = walletPaymentDetail.Amount;
                paymentDetail.WalletID = walletPaymentDetail.WalletID;
                var document = await _commonService.GetDocumentID(walletPaymentDetail.TransID, walletPaymentDetail.PayMode);
                paymentDetail.DocumentID = Guid.Parse(document);
                wallet.Add(paymentDetail);
            }
            return wallet;
        }

        /// <summary>
        /// Get wallet balance
        /// </summary>
        /// <param name="tenantBranchID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        public decimal GetWalletBalance(Guid tenantBranchID) {
            return _walletRepository.GetWalletBalance(tenantBranchID);
        }

        /// <summary>
        /// Get branch wallet transaction detail
        /// </summary>
        /// <param name="tenantBranchID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        public List<Wallet> GetBranchWalletTrans(Guid tenantBranchID) {
            return _walletRepository.GetBranchWalletTrans(tenantBranchID);
        }

        /// <summary>
        /// Get wallet summary
        /// </summary>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        public WalletSummary GetWalletSummary() {
            return _walletRepository.GetWalletSummary();
        }

        public List<FranchiseeToAdminPay> GetPaymentItems(Guid keyValue) {
            List<FranchiseeToAdminPay> list = new List<FranchiseeToAdminPay>();
            var payments = _walletRepository.GetPaymentItems(keyValue);
            foreach (var payment in payments) {
                var branchData = JsonConvert.DeserializeObject<FranchiseeToAdminPay>(payment.PaymentDetails);
                list.Add(branchData);
            }
            return list;
        }

        public GetPaymentID GetPaymentID(Guid keyValue) {
            return _walletRepository.GetPaymentID(keyValue);
        }

        //public List<PurchaseAmount> GetAdmissionFeesList(int year) {
        //    return _walletRepository.GetAdmissionFeesList(year);
        //}

        public int CreateRenewalPayment(WalletRenewalPayment payment) {
            return _walletRepository.CreateRenewalPayment(payment);
        }
        private async Task<string> GetConnectionString(Guid tenantId, string serviceName)
        {
            TenantServiceInfo tenantServiceInfo = new()
            {
                ServiceName = serviceName,
                TenantID = tenantId
            };
            dynamic dbAdminAPIResult = await _dawnHttpClient.PostAsync("dbadmin/get-tenant-constr-on-reg", tenantServiceInfo, false);
            string constring = dbAdminAPIResult.Code == 200 ? dbAdminAPIResult.Data.ToString() : "";
            return constring.ToDecrypt();
        }
        public async Task<List<GetNewWalletHistoryV1>> GetWalletHistoryV1(List<Guid> tenantBranchIDs,DateTime fromDate,DateTime toDate)
        {
            var result = new List<GetNewWalletHistoryV1>();
            var ids = CreateTenantBranchIDTable(tenantBranchIDs);
            DataSet walletDataSet = _walletRepository.GetWalletHistoryV1(ids, fromDate,toDate);
            //var openingBalances = walletDataSet.Tables[0].DataTablesToList<GetWalletOpeningBalance>();
            //var walletBalances = walletDataSet.Tables[1].DataTablesToList<GetWalletBalance>();
            var walletHistories = walletDataSet.Tables[0].DataTablesToList<GetWalletHistoryV1>();
            var resultIds = walletHistories.Select(x => x.TenantBranchID).Distinct().ToList();
            var branchids = CreateTenantBranchIDTable(resultIds);
            string franchiseeConn = await GetConnectionString(_userIdentityInfo.TenantID, "Franchisee");
            var commonRepo = new CommonRepository(_sqlI18N, franchiseeConn);
            var branchDetailsList = commonRepo.GetBranchDetail(branchids);
            foreach (var tenantId in resultIds)
            {
                result.Add(new GetNewWalletHistoryV1
                {
                    TenantBranchID = tenantId,               
                    WalletHistory = walletHistories.Where(x => x.TenantBranchID == tenantId).ToList(),
                    BranchDetail = branchDetailsList.Where(a => a.TenantBranchID == tenantId).FirstOrDefault() ?? new GetBranchContactDetail()
            });
            }
            return result;
        }
        private DataTable CreateTenantBranchIDTable(List<Guid> tenantBranchIDs)
        {
            var table = new DataTable();
            table.Columns.Add("TenantBranchID", typeof(Guid));
            foreach (var id in tenantBranchIDs)
                table.Rows.Add(id);
            return table;
        }
        #endregion
    }
}
