using CommonServices;
using Dawn.Frameworks.Helper;
using Dawn.Utilities.Exceptions;
using Dawn.Utilities.Extensions;
using Dawn.Utilities.i18N;
using EntitiesDTO.Admin;
using EntitiesDTO.Material;
using EntitiesDTO.Payment;
using EntitiesDTO.Student;
using EntitiesDTO.Utilities;
using Frameworks.Entities;
using Frameworks.Helper;
using iCloud.Inventory.Repository;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Globalization;
using System.Net.Http.Headers;
using Utilities.Extensions;

namespace iCloud.Inventory.Services {
    public class IndentService {
        #region Fields
        private readonly SqlI18N _sqlI18N;
        private readonly IdentityInfo _userIdentityInfo;
        private string _connectionString;
        private readonly IndentRepository _indentRepository;
        private readonly CommonService _commonService;
        private readonly IDawnHttpClient _dawnHttpClient;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;
        #endregion
        #region Constructor
        public IndentService (SqlI18N sqlI18N, IHttpContextAccessor accessor, IDawnHttpClient dawnHttpClient, IConfiguration configuration, IWebHostEnvironment environment) {
            _sqlI18N = sqlI18N;
            _userIdentityInfo = accessor.GetUserIdentityInfoDetails();
            _connectionString = accessor.GetConnectionString(dawnHttpClient);
            _commonService = new CommonService(dawnHttpClient);
            _indentRepository = new IndentRepository(_sqlI18N, _connectionString);
            _dawnHttpClient = dawnHttpClient;
            _configuration = configuration;
            _webHostEnvironment = environment;
        }
        #endregion
        #region Methods
        /// <summary>
        /// Create new delivery mode
        /// Author : Sakthivel B
        /// </summary>
        /// <param name="deliveryMode"></param>
        /// <returns></returns>
        public Guid CreateDeliveryMode (CreateDeliveryMode deliveryMode) {
            CreateDeliveryMode createDelivery = new CreateDeliveryMode();
            createDelivery.DeliveryModeName = deliveryMode.DeliveryModeName;
            return _indentRepository.CreateDeliveryMode(createDelivery, _userIdentityInfo.ContactID);
        }

        /// <summary>
        /// Get all delivery mode list
        /// </summary>
        /// Author : Sakthivel B
        /// <returns></returns>
        public List<DeliveryModeList> GetDeliveryModeList () {
            return _indentRepository.GetDeliveryModeList();
        }
        /// <summary>
        /// Create Material Indent
        /// </summary>
        /// Author : Sakthivel B
        /// <param name="materialIndent"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<Guid> CreateMaterialIndent (CreateNewMaterialIndent materialIndent) {
            decimal walletBalance;
            string walletPaymentID = String.Empty;
            var currentStock = GetCurrentStockList();
            foreach (var item in materialIndent.MaterialItem) {
                var stock = currentStock.Where(x => x.MaterialID == item.MaterialID).FirstOrDefault();
                if (stock.CurrentStock < item.Quantity)
                    throw new DawnUserException($"Insufficient {stock.MaterialName} stock in administration.");
            }
            string connectionString = await GetConnectionString(_userIdentityInfo.TenantID, "Payment");//wallet payment
            CommonRepository commonRepository = new CommonRepository(_sqlI18N, connectionString);
            //Check franchisee wallet balance
            var walletItems = materialIndent.PaymentItems.Where(p => p?.PayModeType.ToUpperCase() == "WALLET");
            var walletTotalAmount = walletItems.Sum(p => p.Amount);
            if (walletTotalAmount > 0)
            {
                walletBalance = commonRepository.GetWalletBalance(_userIdentityInfo.TenantBranchID);
                if (walletBalance < walletTotalAmount)
                    throw new DawnUserException("Insufficient wallet balance");
            }
            string receiverContactID = await _commonService.GetAdminContactID();
            string receiverBranchID = await _commonService.GetAdminBranchID(_userIdentityInfo.TenantID);
            CreateMaterialIndent indent = new CreateMaterialIndent();
            DataTable material = materialIndent.MaterialItem.ToDataTable();
            indent.TenantBranchID = _userIdentityInfo.TenantBranchID;
            indent.CreatedBy = _userIdentityInfo.ContactID;
            indent.TotalAmount = materialIndent.TotalAmount;
            indent.CourierAmount = materialIndent.CourierAmount ?? 0;
            var createIndent = _indentRepository.CreateMaterialIndent(indent, material);
            /*Get PaymentMode by id*/
            var payModeIDs = materialIndent.PaymentItems.Select(p => p.PaymentModeID).Distinct();
            var payModeType = payModeIDs.Select(id => commonRepository.GetPayModeType(id)).Where(type => !string.IsNullOrWhiteSpace(type)).ToList();

            List<NewPaymentItems> newItems = new List<NewPaymentItems>();

            foreach (var paymentItems in materialIndent.PaymentItems) {
                FranchiseeToAdminPay paymentDetails = paymentItems.Mapper<FranchiseeToAdminPay>();
                var paymentDetail = JsonConvert.SerializeObject(paymentDetails);
                NewPaymentItems newPaymentItems = paymentItems.Mapper<NewPaymentItems>();
                newPaymentItems.PaymentModeID = paymentItems.PaymentModeID;
                newPaymentItems.PayModeType = paymentItems.PayModeType;
                newPaymentItems.TransactionDate = paymentItems.TransactionDate;
                newPaymentItems.BankName = paymentItems.FromLedgerName;
                newPaymentItems.PaymentDetails = paymentDetail;
                newItems.Add(newPaymentItems);
                if (paymentItems?.PayModeType.ToUpperCase() == "WALLET")
                {
                    WalletPayment wallet = new WalletPayment();
                    Guid createdBy = _userIdentityInfo.ContactID;
                    wallet.TenantBranchID = _userIdentityInfo.TenantBranchID;
                    wallet.CreatedBy = createdBy;
                    wallet.SenderID = createdBy;
                    wallet.ReceiverID = Guid.Parse(receiverContactID);
                    wallet.WalletAmount = walletTotalAmount;
                    //wallet.Description = "Material Fees : " + walletTotalAmount + ", IndentNo : " + createIndent.IndentNumber ;
                    wallet.Description = $"Material Fees : {walletTotalAmount}, IndentNo : {createIndent.IndentNumber} ({string.Join(", ", payModeType)})";
                    wallet.WalletType = "MATERIAL INDENT";
                    walletPaymentID = commonRepository.CreateWalletPayment(wallet).ToString();
                }
            }
            Receipt receipt = new Receipt();
            receipt.TenantBranchID = _userIdentityInfo.TenantBranchID;
            receipt.KeyTable = "BranchMaterialIndent";
            receipt.KeyColumn = "BranchMaterialIndentID";
            receipt.KeyValue = createIndent.BranchMaterialIndentID;
            string transactionName = "MATERIAL FEES";
            receipt.TransactionName = transactionName;
            receipt.PaymentItems = newItems;
            receipt.CreatedBy = _userIdentityInfo.ContactID;
            //receipt.VoucherDate = DateTime.Now.Date.ToString().ToDateOnly();
            receipt.VoucherDate = DateTime.Now.ToString("yyyy-MM-dd");
            string payments = await _commonService.CreatePayment(receipt);

            TempDocumentActivate temp = new TempDocumentActivate();
            List<DocumentItem> documents = new List<DocumentItem>();
            foreach (var item in materialIndent.PaymentItems) {
                if (item.DocumentID != Guid.Empty) {
                    string transID = await _commonService.GetPaymentTransID(Guid.Parse(payments), item.TransactionNo);
                    documents.Add(new DocumentItem { DocumentID = item.DocumentID, EntityID = Guid.Parse(transID) });
                    temp.DocumentItem = documents;
                }
            }
            if (temp.DocumentItem != null) {
                var activateDocument = await _commonService.TempDocumentActivated(temp);
            }            
            if (string.IsNullOrEmpty(payments) || (walletTotalAmount > 0 && string.IsNullOrEmpty(walletPaymentID))) {
                int result = _indentRepository.RollbackMaterialIndent(createIndent.BranchMaterialIndentID);
                if(!string.IsNullOrEmpty(payments))
                {
                string rollbackPayment = await _commonService.RollBackPayment(createIndent.BranchMaterialIndentID); //Added 30/06
                }
                if (walletTotalAmount > 0 && !string.IsNullOrEmpty(walletPaymentID)) //Added 30/06
                {
                    int rollbackWallet = commonRepository.RollbackWallet(Guid.Parse(walletPaymentID));//Added 30/06
                }
                throw new DawnUserException("Couldn't create material indent");
            }
            bool accounting = await _commonService.FranchiseeToAdmin(materialIndent.PaymentItems, _userIdentityInfo.TenantBranchID, Guid.Parse(receiverBranchID), _userIdentityInfo.ContactID, Guid.Parse(receiverContactID), Guid.Parse(payments), transactionName);
            if (accounting == false) {
                string rollbackPayment = await _commonService.RollBackPayment(createIndent.BranchMaterialIndentID);
                int result = _indentRepository.RollbackMaterialIndent(createIndent.BranchMaterialIndentID);
                if (walletTotalAmount > 0)
                {
                    int rollbackWallet = commonRepository.RollbackWallet(Guid.Parse(walletPaymentID));//Added 30/06
                }
                throw new DawnUserException("Couldn't create material indent");
            }
            //return indent.TenantBranchID;
            return createIndent.BranchMaterialIndentID;
        }
        public async Task<IndentReceipt> MaterialIndentReceipt(Guid branchMaterialIndentID)
        {
            IndentReceipt indent = new IndentReceipt();
            var materialIndents = _indentRepository.GetMaterialIndents(branchMaterialIndentID);

            string _connectionString = await GetConnectionString(_userIdentityInfo.TenantID, "Franchisee");
            var commonRepository = new CommonRepository(_sqlI18N, _connectionString);
            var branchDetails = commonRepository.BranchDetail(materialIndents.FirstOrDefault().TenantBranchID);
            indent = branchDetails.Mapper<IndentReceipt>();

            string connTenant = await GetConnectionString(_userIdentityInfo.TenantID, "TenantBranch");
            var commonRepoTenant = new CommonRepository(_sqlI18N, connTenant);
            var adminDetails = commonRepoTenant.GetAdminContactInfo();            

            string connectionString = await GetConnectionString(_userIdentityInfo.TenantID, "Payment"); //To get payment mode details
            var commonRepo = new CommonRepository(_sqlI18N, connectionString);
            var payments = commonRepo.GetPaymenttypes(branchMaterialIndentID, "BranchMaterialIndent");
            var firstPayment = payments.FirstOrDefault();
            var materials = materialIndents.FirstOrDefault();
            indent.MaterialsDetails = materialIndents.Cast<GetMaterialPurchaseDetails>().ToList();
            indent.AdminDetails = adminDetails.Mapper<AdminContact>();
            indent.IndentType = "MATERIALS";
            indent.PayModes = string.Join(", ", payments.Select(p => p.PayMode));
            indent.TotalAmount = materials?.TotalAmount ?? 0;
            indent.AmountInRupess = (materials?.TotalAmount.ToDouble() ?? 0).ToAmountInRupees();
            indent.CourierAmount = materials?.CourierAmount ?? 0;
            indent.VoucherDate = firstPayment?.VoucherDate ?? "";
            indent.VoucherNo = firstPayment?.VoucherNo ?? "";
            return indent;
        }
        /// <summary>
        /// Accept material indent request
        /// </summary>
        /// Author : Sakthivel B
        /// <param name="indent"></param>
        /// <returns></returns>
        public string AcceptMaterialIndent (ApproveMaterialIndent indent) {
            var currentStock = GetCurrentStockList();
            foreach (var item in indent.MaterialIndent) {
                MaterialInward material = _indentRepository.GetIndentTrans(item.MaterialIndentTransID);
                var stock = currentStock.Where(x => x.MaterialID == material.MaterialID).FirstOrDefault();
                if (stock.CurrentStock == 0 || stock.CurrentStock < material.Quantity)
                    throw new DawnUserException("Not enough current stock.");
            }
            DataTable materialIndent = indent.MaterialIndent.ToDataTable();
            int acceptIndent = _indentRepository.ApproveMaterialIndent(materialIndent, _userIdentityInfo.ContactID);
            return acceptIndent > 0 ? "Material indent request accepted" : throw new DawnUserException("Could not accept the indent request !");
        }

        /// <summary>
        /// Split material indent
        /// </summary>
        /// Author : Sakthivel B
        /// <param name="splitMaterial"></param>
        /// <returns></returns>
        public Guid SplitMaterialIndent (SplitMaterialIndent splitMaterial) {
            if (_indentRepository.GetIndentQuantity(splitMaterial.MaterialIndentTransID) <= splitMaterial.IndentQuantity)
                throw new DawnUserException("Quantity count is greater than indent count");
            return _indentRepository.SplitMaterialIndent(splitMaterial, _userIdentityInfo.ContactID);
        }

        /// <summary>
        /// Reject material indent request list
        /// </summary>
        /// Author : Sakthivel B
        /// <param name="indent"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<Guid> RejectMaterialIndent (RejectMaterialIndent indent) {
            if (!_indentRepository.CheckBranchMaterialIndentID(indent.BranchMaterialIndentID))
                throw new DawnUserException("Invalid BranchMaterialIndentID !");
            GetPaymentID paymentID = await _commonService.GetPaymentID(indent.BranchMaterialIndentID);
            List<FranchiseeToAdminPay> payments = await _commonService.GetPaymentItems(indent.BranchMaterialIndentID);
            Guid giverBranchID = _userIdentityInfo.TenantBranchID;
            string receiverContactID = await _commonService.GetFranchiseeContactID(paymentID.TenantBranchID);
            string giverContactID = await _commonService.GetAdminContactID();
            string transactionName = "MATERIAL FEES REJECTED";
            RejectPayment payment = new RejectPayment();
            payment.PackageLogID = indent.BranchMaterialIndentID;
            payment.Reason = indent.Reason;
            var rejectPayment = await _commonService.RejectPaidRequest(payment);
            bool posting = await _commonService.FranchiseeToAdminReEntry(payments, giverBranchID, paymentID.TenantBranchID, Guid.Parse(giverContactID), Guid.Parse(receiverContactID), paymentID.TenantBranchTransID, transactionName);
            return _indentRepository.RejectMaterialIndent(indent, _userIdentityInfo.ContactID);
        }

        /// <summary>
        /// Get material despatch list
        /// </summary>
        /// Author : Sakthivel B
        /// <returns></returns>
        public async Task<List<MaterialDespatchList>> GetMaterialDespatchList () {
            List<MaterialDespatchList> despatchLists = new List<MaterialDespatchList>();
            var franchisee = await _commonService.GetFranchiseeID();
            var indent = _indentRepository.GetMaterialDespatchList();
            DataTable branchIdsTable = new DataTable();
            branchIdsTable.Columns.Add("TenantBranchID", typeof(Guid));
            foreach (var item in indent)
            {
                branchIdsTable.Rows.Add(item.TenantBranchID);
            }
            string franchiseeConn = await GetConnectionString(_userIdentityInfo.TenantID, "Franchisee");
            var commonRepo = new CommonRepository(_sqlI18N, franchiseeConn);
            var branchDetailsList = commonRepo.GetBranchDetail(branchIdsTable);
            foreach (var item in indent) {
                //var branch = await _commonService.GetBranchDetails(item.TenantBranchID);
                var branch = branchDetailsList.Where(x=> x.TenantBranchID == item.TenantBranchID).FirstOrDefault();
                var branchCode = franchisee.Where(x => x.TenantBranchID == item.TenantBranchID).FirstOrDefault();
                MaterialDespatchList despatch = new MaterialDespatchList();
                despatch.BranchMaterialIndentID = item.BranchMaterialIndentID;
                despatch.MaterialIndentTransID = item.MaterialIndentTransID;
                despatch.TenantBranchID = item.TenantBranchID;
                despatch.MaterialName = item.MaterialName;
                despatch.Quantity = item.Quantity;
                despatch.Price = item.Price;
                despatch.Total = item.Total;
                despatch.ConfirmedDate = item.ConfirmedDate;
                despatch.FranchiseeID = branchCode != null ? branchCode.FranchiseeCode : "";
                despatch.FranchiseeName = branch.BranchName;
                despatch.Address1 = branch.Address1;
                despatch.Address2 = branch.Address2 != null ? branch.Address2 : "";
                despatch.Area = branch.Area;
                despatch.District = branch.District;
                despatch.State = branch.State;
                despatch.Country = branch.Country;
                despatch.Pincode = branch.Pincode;
                despatch.Taluk = branch.Taluk;
                despatchLists.Add(despatch);
            }
            return despatchLists;
        }

        /// <summary>
        /// Despatch material item
        /// </summary>
        /// Author : Sakthivel B
        /// <param name="newDespatch"></param>
        /// <returns></returns>
        public async Task<string> DespatchMaterialItem (NewDespatchMaterial newDespatch) {
            DataTable materialDespatch = newDespatch.DespatchMaterial.ToDataTable();
            int result = _indentRepository.DespatchMaterialIndent(materialDespatch, _userIdentityInfo.ContactID);
            if (result > 0) {
                List<GetMaterialIndentIDs> materialIndents = _indentRepository.GetMaterialIndentIDs(materialDespatch);
                foreach (var indent in materialIndents) {
                    Guid materialIndentID = _indentRepository.CheckMaterialDesPatched(indent.MaterialIndentID);
                    if (materialIndentID != Guid.Empty) {
                        var invoice = await GetInvoiceDetail(materialIndentID);
                        string connectionString = await GetConnectionString(_userIdentityInfo.TenantID, "Payment");
                        CommonRepository commonRepository = new CommonRepository(_sqlI18N, connectionString);
                        Guid invoiceID = commonRepository.InvoiceCreation(invoice, _userIdentityInfo.ContactID);
                    }
                }
            }
            string response = result > 0 ? "Material despatched successfully" : throw new DawnUserException("Could not despatch material !");
            return response;
        }

        private async Task<InvoiceCreate> GetInvoiceDetail (Guid materialIndentID) {
            Guid tenantBranchID = _indentRepository.GetTenantBranchID(materialIndentID);
            string _connectionString = await GetConnectionString(_userIdentityInfo.TenantID, "Franchisee");
            CommonRepository commonRepository = new CommonRepository(_sqlI18N, _connectionString);
            var address = commonRepository.GetBranchAddress(tenantBranchID);
            var adminAddress = await GetHOAddress();
            var getGST = _indentRepository.GetGST(materialIndentID);
            var invoice = new InvoiceCreate {
                IndentID = materialIndentID,
                CGST = address.State.ToConditionFormat() == adminAddress.State.ToConditionFormat() ? 0 : getGST.CGST,
                SGST = address.State.ToConditionFormat() == adminAddress.State.ToConditionFormat() ? 0 : getGST.SGST,
                IGST = address.State.ToConditionFormat() == adminAddress.State.ToConditionFormat() ? (getGST.CGST + getGST.SGST) : 0,
                Address = JsonConvert.SerializeObject(address)
            };
            return invoice;
        }


        public async Task<List<GetReceivableList>> GetMaterialReceivableList () {
            var list = _indentRepository.GetReceivableList(_userIdentityInfo.TenantBranchID);
            // Use Task.WhenAll to process each item concurrently and create receivable lists.
            var receivableTasks = list.Select(async item => {
                string connectionString = await GetConnectionString(_userIdentityInfo.TenantID, "Payment");
                CommonRepository commonRepository = new CommonRepository(_sqlI18N, connectionString);
                var invoice = commonRepository.GetInvoiceDetail(item.BranchMaterialIndentID);
                return new GetReceivableList {
                    // Assuming item has the same properties as GetReceivableList
                    MaterialIndentTransID = item.MaterialIndentTransID,
                    BranchMaterialIndentID = item.BranchMaterialIndentID,
                    IndentDate = item.IndentDate,
                    InvoiceNo = invoice?.InvoiceNo ?? string.Empty,
                    InvoiceDate = invoice?.InvoiceDate ?? string.Empty,
                    IndentNumber = item.IndentNumber,
                    MaterialType = item.MaterialType,
                    MaterialName = item.MaterialName,
                    Total = item.Total,
                    DespatchDate = item.DespatchDate,
                    DeliveryMode = item.DeliveryMode,
                    TrackingID = item.TrackingID,
                };
            });
            var receivableLists = await Task.WhenAll(receivableTasks);
            return receivableLists.ToList();
        }


        /// <summary>
        /// Receive material indent item
        /// </summary>
        /// Author : Sakthivel B
        /// <param name="indent"></param>
        /// <returns></returns>
        public string ReceiveMaterialIndentItem (ApproveMaterialIndent indent) {
            DataTable materialIndent = indent.MaterialIndent.ToDataTable();
            int acceptIndent = _indentRepository.ReceiveMaterialIndentItem(materialIndent, _userIdentityInfo.ContactID);
            return acceptIndent > 0 ? "Material item received successfully" : throw new DawnUserException("Could not receive material item !");
        }

        /// <summary>
        /// Get material issued list
        /// </summary>
        /// Author : Sakthivel B
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public async Task<IEnumerable<NewMaterialIssuedList>> GetMaterialIssuedList (string fromDate, string toDate,string nameOrPhone, Guid courseTypeID, string shortName) {
            var material = await _commonService.GetMaterialNameList();
            //var contact = await _commonService.ContactInfo(string.Empty);
            string connectionString = await GetConnectionString(_userIdentityInfo.TenantID, "TenantBranch");
            var commonRepository = new CommonRepository(_sqlI18N, connectionString);
            var contact = commonRepository.GetStudentContacts(_userIdentityInfo.TenantBranchID);
            string connAcademic = await GetConnectionString(_userIdentityInfo.TenantID, "Academic");
            var commonRepo = new CommonRepository(_sqlI18N, connAcademic);
            var student = commonRepo.GetMaterialIssuedList(fromDate, toDate, _userIdentityInfo.TenantBranchID, courseTypeID, shortName);
            var issuedList = (from s in student
                              join c in contact on s.ContactID equals c.ContactID
                              join m in material on s.MaterialID equals m.MaterialID
                              where string.IsNullOrEmpty(nameOrPhone) || c.ContactName.Contains(nameOrPhone, StringComparison.OrdinalIgnoreCase) ||
                                                                         c.Phone.Contains(nameOrPhone, StringComparison.OrdinalIgnoreCase)
                              select new NewMaterialIssuedList() {
                                  StudentID = s.StudentID,
                                  StudentName = c.ContactName,
                                  RollNumber = s.RollNumber,
                                  Course = s.Course,
                                  MaterialName = m.MaterialName,
                                  IssuedDate = s.IssuedDate,
                                  ShortName = s.ShortName,
                                  CourseTypeName = s.CourseTypeName,
                                  CourseTypeID = s.CourseTypeID
                              }).ToList();
            return issuedList;
        }

        /// <summary>
        /// Get material pending list
        /// </summary>
        /// Author : Sakthivel B
        /// <returns></returns>
        public async Task<IEnumerable<NewMaterialPendingList>> GetMaterialPendingList () {
            List<NewMaterialPendingList> pendingLists = new List<NewMaterialPendingList>();
            IEnumerable<MaterialPendingList> student = await _commonService.GetMaterialPendingList();
            var contactInfo = await _commonService.ContactInfo(string.Empty);
            foreach (var item in student) {
                var contact = contactInfo.Where(x => x.ContactID == item.ContactID).FirstOrDefault();
                NewMaterialPendingList pendingList = new NewMaterialPendingList();
                int total = await _commonService.GetMaterialCount(item.BatchMappingID);
                int issued = await _commonService.GetIssuedMaterialCount(item.BatchMappingID);
                pendingList.StudentID = item.StudentID;
                pendingList.BatchMappingID = item.BatchMappingID;
                pendingList.ContactID = item.ContactID;
                pendingList.StudentName = contact != null ? contact.ContactName : "";
                pendingList.RollNumber = item.RollNumber;
                pendingList.Course = item.Course;
                pendingList.ShortName = item.ShortName;
                pendingList.CourseTypeName = item.CourseTypeName;
                pendingList.BatchTime = item.BatchTime;
                pendingList.Total = total;
                pendingList.Issued = issued;
                pendingList.Pending = pendingList.Total - pendingList.Issued;
                if (total != issued)
                    pendingLists.Add(pendingList);
            }
            return pendingLists;
        }

        /// <summary>
        /// Get local stock list
        /// </summary>
        /// Author : Sakthivel B
        /// <returns></returns>
        public async Task<List<LocalStockList>> GetLocalStockList () {
            List<LocalStockList> stockLists = new List<LocalStockList>();
            var stock = _indentRepository.GetLocalStockList(_userIdentityInfo.TenantBranchID);
            foreach (var item in stock) {
                LocalStockList stockList = new LocalStockList();
                stockList.MaterialID = item.MaterialID;
                stockList.MaterialType = item.MaterialType;
                stockList.MaterialName = item.MaterialName;
                stockList.Price = item.Price;
                stockList.CurrentStock = item.CurrentStock - (await _commonService.GetMaterialIssuedCount(item.MaterialID));
                stockLists.Add(stockList);
            }
            return stockLists;
        }

        /// <summary>
        /// Get material required list
        /// </summary>
        /// Author : Sakthivel B
        /// <returns></returns>
        public async Task<List<NewMaterialRequiredList>> GetMaterialRequiredList () {
            List<NewMaterialRequiredList> materialRequiredLists = new List<NewMaterialRequiredList>();
            List<LocalStockList> currentStock = await GetLocalStockList();
            var requiredList = await _commonService.MaterialRequiredList(_userIdentityInfo.TenantBranchID);
            var materialNames = await _commonService.GetMaterialNameList();
            foreach (var item in requiredList) {
                var stock = currentStock.Where(x => x.MaterialID == item.MaterialID).FirstOrDefault();
                int totalStock = stock != null ? stock.CurrentStock : 0;
                var materialName = materialNames.Where(x => x.MaterialID == item.MaterialID).FirstOrDefault();
                NewMaterialRequiredList material = new NewMaterialRequiredList();
                if (item.Total > totalStock) {
                    material.MaterialID = item.MaterialID;
                    material.MaterialTypeID = materialName.MaterialTypeID;
                    material.MaterialType = materialName.MaterialType;
                    material.MaterialName = materialName.MaterialName;
                    material.Price = materialName.Price;
                    material.GST = materialName.GST;
                    material.ToIssue = item.Total;
                    material.CurrentStock = totalStock;
                    material.Required = item.Total - totalStock;
                    materialRequiredLists.Add(material);
                }
            }
            return materialRequiredLists;
        }

        /// <summary>
        /// Create new material purchase invoice
        /// </summary>
        /// Author : Sakthivel B
        /// <param name="purchase"></param>
        /// <returns></returns>
        public Guid CreateMaterialPurchaseInvoice (MaterialPurchase purchase) {
            if (purchase == null)
                throw new DawnUserException("Invalid data !");
            DataTable materials = purchase.MaterialInwards.ToDataTable();
            return _indentRepository.CreatePurchaseInvoice(purchase, materials, _userIdentityInfo.ContactID);
        }

        /// <summary>
        /// Get material purchase invoice list by date
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public async Task<List<PurchaseInvoiceList>> GetMaterialPurchaseInvoiceList (DateTime fromDate, DateTime toDate) {
            List<PurchaseInvoiceList> purchaseInvoiceLists = new List<PurchaseInvoiceList>();
            DataSet purchaseDataSet = _indentRepository.GetMaterialPurchaseInvoiceList(fromDate, toDate);
            var materialPurchaseList = purchaseDataSet.Tables[0].DataTablesToList<MaterialPurchaseList>();
            var materialPurchaseTrans = purchaseDataSet.Tables[1].DataTablesToList<MaterialPurchaseTransList>();
            var branchInfo = await _commonService.GetBranchDetails(_userIdentityInfo.TenantBranchID);
            BranchDetails branch = branchInfo.Mapper<BranchDetails>();
            var vendorList = await _commonService.GetVendorList();
            foreach (var item in materialPurchaseList) {
                PurchaseInvoiceList invoiceList = new PurchaseInvoiceList();
                var vendor = vendorList.Where(x => x.ContactID == item.VendorID).FirstOrDefault();
                var materialPurchase = materialPurchaseList.Where(x => x.MaterialPurchaseID == item.MaterialPurchaseID).FirstOrDefault();
                MaterialPurchaseList purchase = materialPurchase.Mapper<MaterialPurchaseList>();
                purchase.VendorName = vendor != null ? vendor.CompanyName : "";
                invoiceList.BranchDetail = branch;
                invoiceList.MaterialPurchase = purchase;
                invoiceList.MaterialPurchaseTrans = materialPurchaseTrans.Where(x => x.MaterialPurchaseID == item.MaterialPurchaseID).ToList();
                purchaseInvoiceLists.Add(invoiceList);
            }
            return purchaseInvoiceLists;
        }

        public async Task<PurchaseInvoiceList> PrintSalseInvoice (Guid materialPurchaseID) {
            DataSet purchaseDataSet = _indentRepository.PrintSalseInvoice(materialPurchaseID);
            var materialPurchase = purchaseDataSet.Tables[0].DataTablesToItem<MaterialPurchaseList>();
            var materialPurchaseTrans = purchaseDataSet.Tables[1].DataTablesToList<MaterialPurchaseTransList>();
            var branchInfo = await _commonService.GetBranchDetails(_userIdentityInfo.TenantBranchID);
            BranchDetails branch = branchInfo.Mapper<BranchDetails>();
            var vendorList = await _commonService.GetVendorList();

            int slNo = 1;
            List<MaterialPurchaseTransList> transLists = new List<MaterialPurchaseTransList>();
            var purchaseTrans = materialPurchaseTrans.Where(x => x.MaterialPurchaseID == materialPurchase.MaterialPurchaseID).ToList();
            foreach (var item in purchaseTrans) {
                MaterialPurchaseTransList material = item.Mapper<MaterialPurchaseTransList>();
                material.SlNo = slNo;
                transLists.Add(material);
                slNo++;
            }

            PurchaseInvoiceList invoiceList = new PurchaseInvoiceList();
            var vendor = vendorList.Where(x => x.ContactID == materialPurchase.VendorID).FirstOrDefault();
            MaterialPurchaseList purchase = materialPurchase.Mapper<MaterialPurchaseList>();
            purchase.VendorName = vendor != null ? vendor.CompanyName : "";
            invoiceList.BranchName = _userIdentityInfo.TenantBranchName;
            invoiceList.BranchDetail = branch;
            invoiceList.MaterialPurchase = purchase;
            invoiceList.MaterialPurchaseTrans = transLists;
            return invoiceList;
        }

        /// <summary>
        /// Get admin current stock list
        /// </summary>
        /// <returns></returns>
        public IEnumerable<GetCurrentStock> GetCurrentStockList () {
            return _indentRepository.GetCurrentStockList();
        }

        /// <summary>
        /// Get material flow report
        /// </summary>
        /// <param name="flowReport"></param>
        /// <returns></returns>
        public async Task<List<MaterialFolwReport>> GetMaterialFlowReport (FlowReportByFilter flowReport) {
            List<MaterialFolwReport> reports = new List<MaterialFolwReport>();
            DataTable materials = flowReport.Materials.ToDataTable();
            DataSet materialDataSet = _indentRepository.GetMaterialFlowReport(flowReport.FromDate ?? "", flowReport.ToDate ?? "", materials);
            var materialPurchase = materialDataSet.Tables[0].DataTablesToList<MaterialFolwReport>();//branch
            var materialSalse = materialDataSet.Tables[1].DataTablesToList<MaterialFolwReport>();//Vendor
            DataTable branchOrVendorIDs = new DataTable();
            branchOrVendorIDs.Columns.Add("BranchOrVendorID", typeof(Guid));
            var ids = materialPurchase
                .Select(x => x.BranchOrVendorID)
                .Concat(materialSalse.Select(x => x.BranchOrVendorID))
                .Distinct();
            foreach (var id in ids)
            {
                branchOrVendorIDs.Rows.Add(id);
            }
            string connTenant = await GetConnectionString(_userIdentityInfo.TenantID, "TenantBranch");
            var commonRepoTenant = new CommonRepository(_sqlI18N, connTenant);
            var vendorDetails = commonRepoTenant.VendorDetails(branchOrVendorIDs);

            string connFrances = await GetConnectionString(_userIdentityInfo.TenantID, "Franchisee");
            var commonRepository = new CommonRepository(_sqlI18N, connFrances);
            var branchDetails = commonRepository.GetFranchiseeCode(branchOrVendorIDs);

            foreach (var material in materialPurchase) {
                MaterialFolwReport materialFolw = new MaterialFolwReport();
                materialFolw.InvoiceDate = material.InvoiceDate;
                materialFolw.InvoiceNo = material.InvoiceNo;
                materialFolw.MaterialName = material.MaterialName;
                materialFolw.PurchaseQuantity = material.PurchaseQuantity;
                materialFolw.PurchaseAmount = material.PurchaseAmount;
                materialFolw.SalesQuantity = material.SalesQuantity;
                materialFolw.SalesAmount = material.SalesAmount;
                materialFolw.VendorOrFranchisee = branchDetails.Where(x => x.TenantBranchID == material.BranchOrVendorID).Select(x => x.FranchiseeCode).FirstOrDefault() ?? "";
                reports.Add(materialFolw);
            }
            foreach (var material in materialSalse) {
                MaterialFolwReport materialFolw = new MaterialFolwReport();
                materialFolw.InvoiceDate = material.InvoiceDate;
                materialFolw.InvoiceNo = material.InvoiceNo;
                materialFolw.MaterialName = material.MaterialName;
                materialFolw.PurchaseQuantity = material.PurchaseQuantity;
                materialFolw.PurchaseAmount = material.PurchaseAmount;
                materialFolw.SalesQuantity = material.SalesQuantity;
                materialFolw.SalesAmount = material.SalesAmount;
                materialFolw.VendorOrFranchisee = vendorDetails.Where(x => x.VendorId == material.BranchOrVendorID).Select(x => x.VendorName).FirstOrDefault() ?? "";
                reports.Add(materialFolw);
            }
            return reports;
        }

        /// <summary>
        /// Get material indent list by admin
        /// </summary>
        /// <returns></returns>
        public async Task<List<MaterialIndentList>> GetMaterialIndentList () {
            List<MaterialIndentList> indentLists = new List<MaterialIndentList>();
            DataSet indentDataSet = _indentRepository.GetMaterialIndentList();
            var materialIndent = indentDataSet.Tables[0].DataTablesToList<GetMaterialIndent>();
            var materialIndentTrans = indentDataSet.Tables[1].DataTablesToList<GetMaterialPurchaseTrans>();
            //var materialIndentIds = materialIndent.Select(id => new { KeyValue = id.BranchMaterialIndentID }).ToList().ToDataTable();
            //var payments = await GetPaymentDetailsByBulk(materialIndentIds);

            string _connectionString = await GetConnectionString(_userIdentityInfo.TenantID, "Payment");
            CommonRepository commonRepository = new CommonRepository(_sqlI18N, _connectionString);

            var franchiseeID = await _commonService.GetFranchiseeID();

            DataTable branchIdsTable = new DataTable();
            branchIdsTable.Columns.Add("TenantBranchID", typeof(Guid));
            foreach (var item in materialIndent)
            {
                branchIdsTable.Rows.Add(item.TenantBranchID);
            }
            string franchiseeConn = await GetConnectionString(_userIdentityInfo.TenantID, "Franchisee");
            var commonRepo = new CommonRepository(_sqlI18N, franchiseeConn);
            var branchDetailsList = commonRepo.GetBranchDetail(branchIdsTable);

            foreach (var material in materialIndent) {
                GetFranchiseeCode franchiseeCode = new GetFranchiseeCode();
                franchiseeCode = franchiseeID.Where(x => x.TenantBranchID == material.TenantBranchID).FirstOrDefault()!;
                //var branchInfo = await _commonService.GetBranchDetails(material.TenantBranchID);
                var branchInfo = branchDetailsList.Where(x=> x.TenantBranchID == material.TenantBranchID).FirstOrDefault();
                BranchDetails branch = branchInfo.Mapper<BranchDetails>();
                GetNewMaterialIndent getNewMaterial = new GetNewMaterialIndent();
                getNewMaterial.IndentDate = material.IndentDate;
                getNewMaterial.IndentNo = material.IndentNo;
                getNewMaterial.BranchMaterialIndentID = material.BranchMaterialIndentID;
                getNewMaterial.TenantBranchID = material.TenantBranchID;
                getNewMaterial.TotalItem = material.TotalItem;
                getNewMaterial.Ordered = material.Ordered;
                getNewMaterial.Despatched = material.Despatched;
                getNewMaterial.Pending = material.Pending;
                getNewMaterial.Status = material.Status;
                getNewMaterial.CourierAmount = material.CourierAmount;

                var invoice = commonRepository.GetInvoiceDetail(material.BranchMaterialIndentID);
                MaterialIndentList indentList = new MaterialIndentList();
                indentList.FranchiseeCode = franchiseeCode != null ? franchiseeCode.FranchiseeCode : "";
                indentList.FranchiseeName = franchiseeCode.FranchiseeName??"";
                indentList.IndentDetails = getNewMaterial;
                indentList.IndentDetails.InvoiceNo = invoice != null ? invoice.InvoiceNo : string.Empty;
                indentList.IndentDetails.InvoiceDate = invoice != null ? invoice.InvoiceDate : string.Empty;
                indentList.IndentTransDetail = materialIndentTrans.Where(x => x.BranchMaterialIndentID == material.BranchMaterialIndentID).ToList();
                indentList.BranchDetail = branch;
                indentList.PaymentDetails = await GetPaymentDetails(material.BranchMaterialIndentID);
                indentLists.Add(indentList);
            }
            return indentLists;
        }


        /// <summary>
        /// Get material Indent list by franchisee
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public async Task<List<MaterialIndentListByFranchisee>> GetMaterialIndentListByFranchisee (IndentFilter filter) {
            List<MaterialIndentListByFranchisee> indentListByFranchisee = new List<MaterialIndentListByFranchisee>();
            DataSet indentDataSet = _indentRepository.GetMaterialIndentListByFranchisee(filter, _userIdentityInfo.TenantBranchID);
            var indentDetail = indentDataSet.Tables[0].DataTablesToList<MaterialIndentByFranchisee>();
            foreach (var indent in indentDetail)
            {
                var indentTrans = indentDataSet.Tables[1].DataTablesToList<MaterialIndentTransByFranchisee>().Where(x => x.BranchMaterialIndentID == indent.BranchMaterialIndentID).ToList();
                MaterialIndentByFranchisee material = indent.Mapper<MaterialIndentByFranchisee>();
                //material.IndentDate = indent.IndentDate;
                if (indent.Ordered == indent.Pending)
                {
                    material.Status = "PENDING";
                }
                else if (indent.Pending != 0 && indent.Pending < indent.Ordered)
                {
                    material.Status = "PARTIAL PENDING";
                }
                else if (indent.Ordered == indent.Despatched)
                {
                    material.Status = "DISPATCHED";
                }
                else if (indent.Ordered == indent.Received)
                {
                    material.Status = "RECEIVED";
                }
                else if (indent.Ordered == indent.Rejected)
                {
                    material.Status = "REJECTED";
                }
                string _connectionString = await GetConnectionString(_userIdentityInfo.TenantID, "Payment");
                var commonRepository = new CommonRepository(_sqlI18N, _connectionString);
                var invoice = commonRepository.GetInvoiceDetail(indent.BranchMaterialIndentID);
                MaterialIndentListByFranchisee listByFranchisee = new MaterialIndentListByFranchisee();
                listByFranchisee.Indent = material;
                listByFranchisee.Indent.InvoiceNo = invoice?.InvoiceNo ?? string.Empty;
                listByFranchisee.Indent.InvoiceDate = invoice?.InvoiceDate ?? string.Empty;
                //listByFranchisee.IndentTrans = indentTrans;
                listByFranchisee.IndentTrans = MaterialIndentTrans(indentTrans);
                listByFranchisee.PaymentDetails = await GetPaymentDetails(indent.BranchMaterialIndentID);
                indentListByFranchisee.Add(listByFranchisee);
            }
            return indentListByFranchisee;


            /*  //var indentDetails = indentDataSet.Tables[0].DataTablesToList<MaterialIndentByFranchisee>();
            //var indentTransList = indentDataSet.Tables[1].DataTablesToList<MaterialIndentTransByFranchisee>();

            //var connectionString = await GetConnectionString(_userIdentityInfo.TenantID, "Payment");
            //var commonRepository = new CommonRepository(_sqlI18N, connectionString);

            //var result = await Task.WhenAll(indentDetails.Select(async indent =>
            //{
            //    var indentTrans = indentTransList
            //        .Where(x => x.BranchMaterialIndentID == indent.BranchMaterialIndentID)
            //        .ToList();

            //    var material = indent.Mapper<MaterialIndentByFranchisee>();
            //    material.IndentDate = indent.IndentDate;
            //    material.Status = GetStatus(indent);

            //    var invoice = commonRepository.GetInvoiceDetail(indent.BranchMaterialIndentID);

            //    material.InvoiceNo = invoice?.InvoiceNo ?? string.Empty;
            //    material.InvoiceDate = invoice?.InvoiceDate ?? string.Empty;
            //    return new MaterialIndentListByFranchisee
            //    {
            //        Indent = material,
            //        IndentTrans = MaterialIndentTrans(indentTrans),
            //        PaymentDetails = await GetPaymentDetails(indent.BranchMaterialIndentID)
            //    };
            //}));

            //return result.ToList();       */

        }
          /*  //private string GetStatus(MaterialIndentByFranchisee indent)
        //{
        //    if (indent.Ordered == indent.Pending)
        //        return "PENDING";
        //    if (indent.Pending != 0 && indent.Pending < indent.Ordered)
        //        return "PARTIAL PENDING";
        //    if (indent.Ordered == indent.Despatched)
        //        return "DISPATCHED";
        //    if (indent.Ordered == indent.Received)
        //        return "RECEIVED";

        //    return string.Empty;
        //} */
        public List<MaterialIndentTransByFranchisee> MaterialIndentTrans(List<MaterialIndentTransByFranchisee> trans) {
            List<MaterialIndentTransByFranchisee> materialIndentTrans = new List<MaterialIndentTransByFranchisee>();
            var deliveryModes = _indentRepository.GetDeliveryModeList();
            foreach (var item in trans)
            {
                var deliverymode = deliveryModes.Where(x => x.DeliveryModeID.ToString().ToUpper() == item.DeliveryModeID.ToUpper()).FirstOrDefault();
                item.DeliveryMode = deliverymode?.DeliveryModeName ?? string.Empty;
                materialIndentTrans.Add(item);
            }
            return materialIndentTrans;
        }


        /// <summary>
        /// Create new exam indent
        /// </summary>
        /// <param name="examIndent"></param>
        /// <returns></returns>
        public async Task<Guid> CreateExamIndent (NewExamIndent examIndent) {
            decimal walletBalance;
            string walletPaymentID = String.Empty;
            if (examIndent == null)
                throw new DawnUserException("Invalid data !");
            //Check franchisee wallet balance
            string connectionString = await GetConnectionString(_userIdentityInfo.TenantID, "Payment");//wallet payment
            CommonRepository commonRepository = new CommonRepository(_sqlI18N, connectionString);
            var walletItems = examIndent.PaymentItems.Where(p => p?.PayModeType.ToUpperCase() == "WALLET");
            var walletTotalAmount = walletItems.Sum(p => p.Amount);
            if (walletTotalAmount > 0)
            {                
                walletBalance = commonRepository.GetWalletBalance(_userIdentityInfo.TenantBranchID);
                if (walletBalance < walletTotalAmount)
                    throw new DawnUserException("Insufficient wallet balance");
            }            
            string receiverContactID = await _commonService.GetAdminContactID();
            string receiverBranchID = await _commonService.GetAdminBranchID(_userIdentityInfo.TenantID);
            CreateExamIndent indent = new CreateExamIndent();
            indent.TenantBranchID = _userIdentityInfo.TenantBranchID;
            indent.CreatedBy = _userIdentityInfo.ContactID;
            DataTable batchMapping = examIndent.BatchMapping.ToDataTable();
            indent.TotalAmount = examIndent.TotalAmount;
            indent.CourierAmount = examIndent.CourierAmount ?? 0;
            foreach (var item in examIndent.BatchMapping)
            {
                if (_indentRepository.CheckStudentBatchMappingID(item.BatchMappingID, item.Semester))
                    throw new DawnUserException("The exam indent already created for this student !");
            }
            var createIndent = _indentRepository.CreateExamIndent(indent, batchMapping);
            var indentTransID = _indentRepository.GetExamIndentTransIDs(createIndent.ExamIndentID);
            List<NewPaymentItems> newItems = new List<NewPaymentItems>();
            /*Get PaymentMode by id*/
            var payModeTypes = examIndent.PaymentItems.Select(p => p.PayModeType).Distinct().ToList();

            foreach (var paymentItems in examIndent.PaymentItems)
            {
                FranchiseeToAdminPay paymentDetails = paymentItems.Mapper<FranchiseeToAdminPay>();
                var paymentDetail = JsonConvert.SerializeObject(paymentDetails);
                NewPaymentItems newPaymentItems = paymentItems.Mapper<NewPaymentItems>();
                newPaymentItems.PaymentModeID = paymentItems.PaymentModeID;
                newPaymentItems.PayModeType = paymentItems.PayModeType;
                newPaymentItems.TransactionDate = paymentItems.TransactionDate;
                newPaymentItems.BankName = paymentItems.FromLedgerName;
                newPaymentItems.PaymentDetails = paymentDetail;
                newItems.Add(newPaymentItems);
                /*Create wallet payment */
                if (paymentItems?.PayModeType.ToUpperCase() == "WALLET")
                {
                    WalletPayment wallet = new WalletPayment();
                    Guid createdBy = _userIdentityInfo.ContactID;
                    wallet.TenantBranchID = _userIdentityInfo.TenantBranchID;
                    wallet.CreatedBy = createdBy;
                    wallet.SenderID = createdBy;
                    wallet.ReceiverID = Guid.Parse(receiverContactID);
                    wallet.WalletAmount = walletTotalAmount;
                    wallet.Description = $"Exam Fees : {walletTotalAmount}, IndentNo : {createIndent.IndentNumber} ({string.Join(", ", payModeTypes)})";
                    wallet.WalletType = "EXAM INDENT";                    
                    walletPaymentID = commonRepository.CreateWalletPayment(wallet).ToString();
                }
            }
            Receipt receipt = new Receipt();
            receipt.TenantBranchID = _userIdentityInfo.TenantBranchID;
            receipt.KeyTable = "ExamIndent";
            receipt.KeyColumn = "ExamIndentID";
            receipt.KeyValue = createIndent.ExamIndentID;
            string transactionName = "EXAM FEES";
            receipt.TransactionName = transactionName;
            receipt.PaymentItems = newItems;
            receipt.CreatedBy = _userIdentityInfo.ContactID;
            //receipt.VoucherDate = DateTime.Now.Date.ToString().ToDateOnly();
            receipt.VoucherDate = DateTime.Now.ToString("yyyy-MM-dd");
            string payments = await _commonService.CreatePayment(receipt);
            TempDocumentActivate temp = new TempDocumentActivate();
            List<DocumentItem> documents = new List<DocumentItem>();
            foreach (var item in examIndent.PaymentItems)
            {
                if (item.DocumentID != Guid.Empty)
                {
                    string transID = await _commonService.GetPaymentTransID(Guid.Parse(payments), item.TransactionNo);
                    documents.Add(new DocumentItem { DocumentID = item.DocumentID, EntityID = Guid.Parse(transID) });
                    temp.DocumentItem = documents;
                }
            }
            if (temp.DocumentItem != null)
            {
                var activateDocument = await _commonService.TempDocumentActivated(temp);
            }
            if (string.IsNullOrEmpty(payments) || (walletTotalAmount > 0 && string.IsNullOrEmpty(walletPaymentID)))
            {
                //Rollback all above transaction
                int result = _indentRepository.RollbackExamIndent(createIndent.ExamIndentID);
                if (!string.IsNullOrEmpty(payments))
                {
                    string rollbackPayment = await _commonService.RollBackPayment(createIndent.ExamIndentID); //Added 30/06
                }
                if (walletTotalAmount > 0 && !string.IsNullOrEmpty(walletPaymentID))//Added 30/06
                {
                    int rollbackWallet = commonRepository.RollbackWallet(Guid.Parse(walletPaymentID));//Added 30/06
                }
                throw new DawnUserException("Couldn't create exam indent");
            }
            bool accounting = await _commonService.FranchiseeToAdmin(examIndent.PaymentItems, _userIdentityInfo.TenantBranchID, Guid.Parse(receiverBranchID), _userIdentityInfo.ContactID, Guid.Parse(receiverContactID), Guid.Parse(payments), transactionName);
            if (accounting == false)
            {
                var rollbackPayment = await _commonService.RollBackPayment(createIndent.ExamIndentID);
                int result = _indentRepository.RollbackExamIndent(createIndent.ExamIndentID);
                if (walletTotalAmount > 0)
                {
                    int rollbackWallet = commonRepository.RollbackWallet(Guid.Parse(walletPaymentID));//Added 30/06
                }
                throw new DawnUserException("Couldn't create exam indent");
            }
            //return indent.TenantBranchID;
            return createIndent.ExamIndentID;
        }
        public async Task<IndentReceipt> ExamIndentReceipt(Guid examIndentID)
        {
            IndentReceipt indent = new IndentReceipt();

            var examIndents = _indentRepository.GetExamIndentDetails(examIndentID);
            var exam = examIndents.FirstOrDefault();
            if (exam == null) throw new DawnUserException("Invalid Indent");
            var batchMappingIDs = examIndents.Select(e => new
            {
                e.StudentBatchMapping,
                e.ExamIndentID
            }).ToList().ToDataTable();

            string connAcademic = await GetConnectionString(_userIdentityInfo.TenantID, "Academic");
            var commonRepo = new CommonRepository(_sqlI18N, connAcademic);
            var courseDetails = commonRepo.GetCourseInfo(batchMappingIDs);

            string _connectionString = await GetConnectionString(_userIdentityInfo.TenantID, "Franchisee");
            var commonRepository = new CommonRepository(_sqlI18N, _connectionString);
            var branchDetails = commonRepository.BranchDetail(examIndents.FirstOrDefault().TenantBranchID);
            indent = branchDetails.Mapper<IndentReceipt>();

            string connectionString = await GetConnectionString(_userIdentityInfo.TenantID, "Payment"); //To get payment mode details
            var commonRepoPayment = new CommonRepository(_sqlI18N, connectionString);
            var payments = commonRepoPayment.GetPaymenttypes(examIndentID, "ExamIndent");
            var firstPayment = payments.FirstOrDefault();

            string connTenant = await GetConnectionString(_userIdentityInfo.TenantID, "TenantBranch");
            var commonRepoTenant = new CommonRepository(_sqlI18N, connTenant);
            var adminDetails = commonRepoTenant.GetAdminContactInfo();

            indent.AdminDetails = adminDetails.Mapper<AdminContact>();
            indent.ExamDetails = courseDetails.Cast<GetCourse>().ToList();
            indent.IndentType = "EXAM & CERTIFICATE";
            indent.PayModes = string.Join(", ", payments.Select(p => p.PayMode));
            indent.TotalAmount = exam?.TotalAmount ?? 0;
            indent.AmountInRupess = (exam?.TotalAmount.ToDouble() ?? 0).ToAmountInRupees();
            indent.CourierAmount = exam?.CourierAmount ?? 0;
            indent.VoucherDate = firstPayment?.VoucherDate ?? "";
            indent.VoucherNo = firstPayment?.VoucherNo ?? "";
            return indent;
        }     
        /// <summary>
        /// Get exam indent list by admin
        /// </summary>
        /// <returns></returns>
        public async Task<List<GetExamIndent>> GetExamIndentList () {
            List<GetExamIndent> getExamIndents = new List<GetExamIndent>();
            DataSet examDataSet = _indentRepository.GetExamIndentList();
            var examIndent = examDataSet.Tables[0].DataTablesToList<ExamIndentDetail>();
            var examIndentTrans = examDataSet.Tables[1].DataTablesToList<ExamIndentTransDetail>();
            var franchiseeID = await _commonService.GetFranchiseeID();
            DataTable branchIdsTable = new DataTable();
            branchIdsTable.Columns.Add("TenantBranchID", typeof(Guid));
            foreach (var item in examIndent)
            {
                branchIdsTable.Rows.Add(item.TenantBranchID);
            }
            string franchiseeConn = await GetConnectionString(_userIdentityInfo.TenantID, "Franchisee");
            var commonRepo = new CommonRepository(_sqlI18N, franchiseeConn);
            var branchDetailsList = commonRepo.GetBranchDetail(branchIdsTable);
            foreach (var exam in examIndent) {
                GetFranchiseeCode franchiseeCode = new GetFranchiseeCode();
                franchiseeCode = franchiseeID.FirstOrDefault(x => x.TenantBranchID == exam.TenantBranchID) ?? new GetFranchiseeCode();
                //var branchInfo = await _commonService.GetBranchDetails(exam.TenantBranchID);
                var branchInfo = branchDetailsList.Where(x => x.TenantBranchID == exam.TenantBranchID).FirstOrDefault();
                BranchDetails branch = branchInfo.Mapper<BranchDetails>();

                /*The below commented fuctionality are splited by ExamIndentDetailsByAdmin method*/

                //var newExamIndent = examIndentTrans.Where(x => x.ExamIndentID == exam.ExamIndentID).ToList();
                //var students = await _commonService.GetStudentBasicInfo();
                //List<NewExamIndentTransDetail> transDetails = new List<NewExamIndentTransDetail>();
                //foreach (var trans in newExamIndent)
                //{
                //    var student = students.Where(x => x.BatchMappingID == trans.StudentBatchMapping).FirstOrDefault();
                //    if (student != null)
                //    {
                //        NewExamIndentTransDetail detail = new NewExamIndentTransDetail();
                //        detail.ExamIndentID = trans.ExamIndentID;
                //        detail.StudentBatchMapping = trans.StudentBatchMapping;
                //        detail.ExamIndentTransID = trans.ExamIndentTransID;
                //        detail.Semester = trans.Semester + "/" + (await _commonService.GetCourseSemesterCount(trans.StudentBatchMapping));
                //        detail.ExamFees = trans.ExamFees;
                //        detail.Status = trans.Status;
                //        detail.StudentID = student != null ? student.RollNumber : "";
                //        detail.StudentName = student.StudentName;
                //        detail.Course = student.Course;
                //        detail.ActivateDate = trans.ActivateDate != "" ? trans.ActivateDate : "";
                //        detail.BatchTime = student.BatchTime;
                //        detail.ShortName = student.ShortName;
                //        detail.CourseTypeName = student.CourseTypeName;
                //        transDetails.Add(detail);
                //    }
                //}

                GetExamIndent getExamIndent = new GetExamIndent();
                getExamIndent.FranchiseeCode = franchiseeCode != null ? franchiseeCode.FranchiseeCode : "";
                getExamIndent.FranchiseeName = franchiseeCode.FranchiseeName??"";
                getExamIndent.BranchDetail = branch;
                getExamIndent.ExamIndentDetail = exam;

                //string _connectionString = await GetConnectionString(_userIdentityInfo.TenantID, "Payment");
                //CommonRepository commonRepository = new CommonRepository(_sqlI18N, _connectionString);
                //var invoice = commonRepository.GetInvoiceDetail(exam.ExamIndentID);
                //getExamIndent.ExamIndentDetail.InvoiceNo = invoice != null ? invoice.InvoiceNo : string.Empty;
                //getExamIndent.ExamIndentDetail.InvoiceDate = invoice != null ? invoice.InvoiceDate : string.Empty;
                //getExamIndent.ExamIndentTransDetails = transDetails;
                //getExamIndent.PaymentDetails = await GetPaymentDetails(exam.ExamIndentID);

                getExamIndents.Add(getExamIndent);
            }
            return getExamIndents;
        }
        public async Task<GetExamIndentDetail> ExamIndentDetailsByAdmin(Guid examIndentID)
        {
            var examIndentTrans = _indentRepository.GetExamIndentTransDetail(examIndentID);
            var students = await _commonService.GetStudentBasicInfo();
            List<NewExamIndentTransDetail> transDetails = new List<NewExamIndentTransDetail>();
            foreach (var trans in examIndentTrans)
            {
                var student = students.Where(x => x.BatchMappingID == trans.StudentBatchMapping).FirstOrDefault();
                if (student != null)
                {
                    NewExamIndentTransDetail detail = new NewExamIndentTransDetail();
                    detail.ExamIndentID = trans.ExamIndentID;
                    detail.StudentBatchMapping = trans.StudentBatchMapping;
                    detail.ExamIndentTransID = trans.ExamIndentTransID;
                    detail.Semester = trans.Semester + "/" + (await _commonService.GetCourseSemesterCount(trans.StudentBatchMapping));
                    detail.ExamFees = trans.ExamFees;
                    detail.Status = trans.Status;
                    detail.StudentID = student != null ? student.RollNumber : "";
                    detail.StudentName = student.StudentName;
                    detail.Course = student.Course;
                    detail.ActivateDate = trans.ActivateDate != "" ? trans.ActivateDate : "";
                    detail.BatchTime = student.BatchTime;
                    detail.ShortName = student.ShortName;
                    detail.CourseTypeName = student.CourseTypeName;
                    transDetails.Add(detail);
                }
            }
            string _connectionString = await GetConnectionString(_userIdentityInfo.TenantID, "Payment");
            CommonRepository commonRepository = new CommonRepository(_sqlI18N, _connectionString);
            var invoice = commonRepository.GetInvoiceDetail(examIndentID);

            GetExamIndentDetail getExamIndent = new GetExamIndentDetail();
            getExamIndent.ExamIndentTransDetails = transDetails;
            getExamIndent.PaymentDetails = await GetPaymentDetails(examIndentID);
            return getExamIndent;
        }
        /// <summary>
        /// Reject exam indent
        /// </summary>
        /// <param name="indent"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<Guid> RejectExamIndent (RejectExamIndent indent) {
            if (!_indentRepository.CheckExamIndentID(indent.ExamIndentID))
                throw new Exception("Invalid ExamIndentID !");
            GetPaymentID paymentID = await _commonService.GetPaymentID(indent.ExamIndentID);
            List<FranchiseeToAdminPay> payments = await _commonService.GetPaymentItems(indent.ExamIndentID);
            Guid giverBranchID = _userIdentityInfo.TenantBranchID;
            string receiverContactID = await _commonService.GetFranchiseeContactID(paymentID.TenantBranchID);
            string giverContactID = await _commonService.GetAdminContactID();
            string transactionName = "EXAM FEES REJECTED";
            RejectPayment payment = new RejectPayment();
            payment.PackageLogID = indent.ExamIndentID;
            payment.Reason = indent.Reason;
            var rejectPayment = await _commonService.RejectPaidRequest(payment);
            bool posting = await _commonService.FranchiseeToAdminReEntry(payments, giverBranchID, paymentID.TenantBranchID, Guid.Parse(giverContactID), Guid.Parse(receiverContactID), paymentID.TenantBranchTransID, transactionName);
            return _indentRepository.RejectExamIndent(indent, _userIdentityInfo.ContactID);

        }

        /// <summary>
        /// Activate student for exam
        /// </summary>
        /// <param name="indent"></param>
        /// <returns></returns>
        public string AcceptExamIndent (ApproveExamIndent indent) {
            DataTable examIndent = indent.ExamIndent.ToDataTable();
            int acceptIndent = _indentRepository.ApproveExamIndent(examIndent, _userIdentityInfo.ContactID);
            return acceptIndent > 0 ? "Exam indent request accepted" : throw new DawnUserException("Could not accept the indent request !");
        }

        /// <summary>
        /// Get exam indent request list by franchisee
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public async Task<List<GetExamIndentByFranchisee>> GetExamIndentListByFranchisee (DateTime fromDate, DateTime toDate) {
            List<GetExamIndentByFranchisee> getExamIndents = new List<GetExamIndentByFranchisee>();
            DataSet indentDataSet = _indentRepository.GetExamIndentListByFranchisee(fromDate, toDate, _userIdentityInfo.TenantBranchID);
            var indentDetail = indentDataSet.Tables[0].DataTablesToList<NewExamIndentDetail>();
            var indentTransDetail = indentDataSet.Tables[1].DataTablesToList<ExamIndentTransDetail>();
            foreach (var item in indentDetail) {
                var indentTrans = indentTransDetail.Where(x => x.ExamIndentID == item.ExamIndentID).ToList();
                var students = await _commonService.GetStudentBasicInfo();
                List<NewExamIndentTransDetail> transDetails = new List<NewExamIndentTransDetail>();
                foreach (var trans in indentTrans) {
                    var student = students.Where(x => x.BatchMappingID == trans.StudentBatchMapping).FirstOrDefault();
                    if (student != null)
                    {
                    NewExamIndentTransDetail detail = new NewExamIndentTransDetail();
                    detail.ExamIndentID = trans.ExamIndentID;
                    detail.StudentBatchMapping = trans.StudentBatchMapping;
                    detail.ExamIndentTransID = trans.ExamIndentTransID;
                    detail.Semester = trans.Semester + "/" + (await _commonService.GetCourseSemesterCount(trans.StudentBatchMapping));
                    detail.ExamFees = trans.ExamFees;
                    detail.Status = trans.Status;
                    detail.StudentID = student.RollNumber;
                    detail.StudentName = student.StudentName;
                    detail.Course = student.Course;
                    detail.ActivateDate = trans.ActivateDate;
                    detail.BatchTime = student.BatchTime;
                    detail.ShortName = student.ShortName;
                    detail.CourseTypeName = student.CourseTypeName;
                    transDetails.Add(detail);
                    }
                }
                NewExamIndentDetail newExamIndent = new NewExamIndentDetail();
                string _connectionString = await GetConnectionString(_userIdentityInfo.TenantID, "Payment");
                CommonRepository commonRepository = new CommonRepository(_sqlI18N, _connectionString);
                var invoice = commonRepository.GetInvoiceDetail(item.ExamIndentID);
                newExamIndent.InvoiceNo = invoice != null ? invoice.InvoiceNo : string.Empty;
                newExamIndent.InvoiceDate = invoice != null ? invoice.InvoiceDate : string.Empty;
                newExamIndent.ExamIndentID = item.ExamIndentID;
                newExamIndent.TenantBranchID = item.TenantBranchID;
                newExamIndent.IndentDate = item.IndentDate;
                newExamIndent.IndentNumber = item.IndentNumber;
                newExamIndent.TotalAmount = item.TotalAmount;
                newExamIndent.TotalItem = item.TotalItem;
                newExamIndent.ActiveItem = item.ActiveItem;
                newExamIndent.InactiveItem = item.TotalItem - item.ActiveItem;
                newExamIndent.IsRejected = item.IsRejected;
                newExamIndent.Reason = item.Reason;
                newExamIndent.CourierAmount = item.CourierAmount;
                if (newExamIndent.IsRejected == true) {
                    newExamIndent.Status = "REJECTED";
                }
                else {
                    newExamIndent.Status = newExamIndent.InactiveItem == 0 ? "APPROVED" : "PENDING";
                }
                var distinctStudent = transDetails.GroupBy(x => x.ExamIndentTransID).Select(y => y.First()).ToList();
                GetExamIndentByFranchisee indentByFranchisee = new GetExamIndentByFranchisee();
                indentByFranchisee.ExamIndentDetail = newExamIndent;
                indentByFranchisee.ExamIndentTransDetails = distinctStudent;
                indentByFranchisee.PaymentDetails = await GetPaymentDetails(item.ExamIndentID);
                getExamIndents.Add(indentByFranchisee);
            }
            return getExamIndents;
        }

        /// <summary>
        /// Get exam indent trans id list
        /// </summary>
        /// <returns></returns>
        public List<GetExamIndentTransID> ExamIndentTransID () {
            return _indentRepository.ExamIndentTransID(_userIdentityInfo.TenantBranchID);
        }

        public string UpdateStudentTotalMark (List<StudentTotalMarks> totalMarks) {
            DataTable marks = totalMarks.ToDataTable();
            int result = _indentRepository.UpdateStudentTotalMark(marks, _userIdentityInfo.ContactID);
            return result > 0 ? "Mark updated successfully" : throw new DawnUserException("Couldn't update mark");
        }

        /// <summary>
        /// Get certificate indent list
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public async Task<List<CertificateIndentList>> GetCertificateIndentList (string fromDate, string toDate, string status, string nameOrPhone, Guid courseTypeId, string shortName)
        {
            string _connectionString = await GetConnectionString(_userIdentityInfo.TenantID);
            using IndentRepository indentRepository = new IndentRepository(_sqlI18N, _connectionString);
            var commRepo = new CommonRepository(_sqlI18N, _connectionString);
            List<CertificateIndentList> certificateIndentLists = new List<CertificateIndentList>();
            DataSet certificateDataSet = indentRepository.GetCertificateIndent(fromDate, toDate, status, _userIdentityInfo.TenantBranchID);
            var certificateIndent = certificateDataSet.Tables[0].DataTablesToList<GetCertificateIndentList>();
            var subjectMarks = certificateDataSet.Tables[1].DataTablesToList<SubjectMarkList>();
            var delivrymodeList = _indentRepository.GetDeliveryModeList();
            //var contactInfo = await _commonService.ContactInfo(string.Empty);
            string connectionString = await GetConnectionString(_userIdentityInfo.TenantID, "TenantBranch");
            var commonRepository = new CommonRepository(_sqlI18N, connectionString);
            var contactInfo = commonRepository.GetStudentContacts(_userIdentityInfo.TenantBranchID);
            foreach (var item in certificateIndent) {
                if (_indentRepository.CheckExamIndentTransID(item.ExamIndentTransID)) {
                    var examIndent = _indentRepository.GetStudentCertificateIndentInfo(item.ExamIndentTransID);
                    //var courseInfo = await _commonService.GetStudentCourseInfo(examIndent.StudentBatchMapping);
                    var courseInfo = commRepo.GetStudentCourseInfo(examIndent.StudentBatchMapping, courseTypeId, shortName);
                    if (courseInfo is null) continue;
                    //if (courseTypeId != Guid.Empty && courseInfo.CourseTypeID != courseTypeId)
                    //    continue;
                    //if (!string.IsNullOrWhiteSpace(shortName) && !courseInfo.ShortName.Contains(shortName, StringComparison.OrdinalIgnoreCase))
                    //    continue;
                    var contact = contactInfo.Where(x => x.ContactID == courseInfo.ContactID).FirstOrDefault();
                    if (!string.IsNullOrWhiteSpace(nameOrPhone))
                    {
                        bool nameMatches = contact?.ContactName?.Contains(nameOrPhone, StringComparison.OrdinalIgnoreCase) == true;
                        bool phoneMatches = contact?.Phone?.Contains(nameOrPhone, StringComparison.OrdinalIgnoreCase) == true;
                        if (!nameMatches && !phoneMatches) continue;
                    }
                    var deliverymode = delivrymodeList.Where(x => x.DeliveryModeID.ToString().ToUpper() == item.DeliveryModeID).FirstOrDefault();
                    CertificateIndentList indentList = new CertificateIndentList();
                    indentList.ExamIndentTransID = item.ExamIndentTransID;
                    indentList.IndentNumber = item.IndentNumber;
                    indentList.IndentDate = item.IndentDate;
                    indentList.DespatchDate = item.DespatchDate;
                    indentList.ReceivedDate = item.ReceivedDate;
                    indentList.IssuedDate = item.IssuedDate;
                    indentList.RollNumber = courseInfo.RollNumber;
                    indentList.StudentName = contact != null ? contact.ContactName : "";
                    indentList.Course = courseInfo.Course;
                    indentList.BatchNumber = courseInfo.BatchNumber;
                    indentList.BatchTiming = courseInfo.BatchTime;
                    indentList.Semester = examIndent.Semester + "/" + (await _commonService.GetCourseSemesterCount(examIndent.StudentBatchMapping));
                    indentList.Type = item.Type;
                    indentList.DeliveryMode = deliverymode != null ? deliverymode.DeliveryModeName : "";
                    indentList.TrackingID = item.TrackingID;
                    indentList.Status = item.Status;
                    indentList.FacultyName = await GetFacultyName(courseInfo.FacultyID);
                    indentList.MarkInfo = subjectMarks.Where(x => x.ExamIndentTransID == item.ExamIndentTransID).ToList();
                    indentList.ShortName = courseInfo.ShortName;
                    indentList.CourseTypeName = courseInfo.CourseTypeName;
                    indentList.CourseTypeID = courseInfo.CourseTypeID;
                    certificateIndentLists.Add(indentList);
                }
            }
            return certificateIndentLists;
        }
        private async Task<string> GetConnectionString (Guid tenantId) {
            TenantServiceInfo tenantServiceInfo = new() {
                ServiceName = "Academic",
                TenantID = tenantId
            };
            dynamic dbAdminAPIResult = await _dawnHttpClient.PostAsync("dbadmin/get-tenant-constr-on-reg", tenantServiceInfo, false);
            string constring = dbAdminAPIResult.Code == 200 ? dbAdminAPIResult.Data.ToString() : "";
            return constring.ToDecrypt();
        }

        public async Task<string> GetFacultyName (Guid facultyID) {
            var facultyList = await _commonService.GetTenantUserList();
            var faculty = facultyList.Where(x => x.ContactID == facultyID).FirstOrDefault();
            string name = faculty != null ? faculty.ContactName : string.Empty;
            return name;
        }

        public async Task<List<CertificateIssuedList>> GetCertificateIssuedList (string fromDate, string toDate) {
            List<CertificateIssuedList> certificateIndentLists = new List<CertificateIssuedList>();
            DataSet certificateDataSet = await _commonService.GetCertificateIssuedList(fromDate, toDate);
            var certificateIndent = certificateDataSet.Tables[0].DataTablesToList<GetCertificateIndentList>();
            var subjectMarks = certificateDataSet.Tables[1].DataTablesToList<SubjectMarkList>();
            var delivrymodeList = _indentRepository.GetDeliveryModeList();
            foreach (var item in certificateIndent) {
                if (_indentRepository.CheckExamIndentTransID(item.ExamIndentTransID)) {
                    var examIndent = _indentRepository.GetStudentCertificateIndentInfo(item.ExamIndentTransID);
                    var courseInfo = await _commonService.GetStudentCourseInfo(examIndent.StudentBatchMapping);
                    var contactInfo = await _commonService.ContactInfo(string.Empty);
                    var contact = contactInfo.Where(x => x.ContactID == courseInfo.ContactID).FirstOrDefault();
                    var deliverymode = delivrymodeList.Where(x => x.DeliveryModeID.ToString() == item.DeliveryModeID).FirstOrDefault();
                    CertificateIssuedList indentList = new CertificateIssuedList();
                    indentList.ExamIndentTransID = item.ExamIndentTransID;
                    indentList.IndentNumber = item.IndentNumber;
                    indentList.IndentDate = item.IndentDate;
                    indentList.IssuedDate = item.IssuedDate;
                    indentList.RollNumber = courseInfo.RollNumber;
                    indentList.StudentName = contact != null ? contact.ContactName : "";
                    indentList.Course = courseInfo.Course;
                    indentList.BatchNumber = courseInfo.BatchNumber;
                    indentList.BatchTiming = courseInfo.BatchTime;
                    indentList.Semester = examIndent.Semester + "/" + (await _commonService.GetCourseSemesterCount(examIndent.StudentBatchMapping));
                    indentList.Type = item.Type;
                    indentList.Status = item.Status;
                    indentList.FacultyName = await GetFacultyName(courseInfo.FacultyID);
                    certificateIndentLists.Add(indentList);
                }
            }
            return certificateIndentLists;
        }

        public CertificateIndentInfo GetStudentCertificateIndentInfo (Guid examIndentTransID) {
            return _indentRepository.GetStudentCertificateIndentInfo(examIndentTransID);
        }

        public int UpdateEditedMark (GetTotalMark totalMark) {
            return _indentRepository.UpdateEditedMark(totalMark, _userIdentityInfo.ContactID);
        }

        public DataSet GetCertificateList () {
            return _indentRepository.GetCertificateList();
        }

        public List<ExamTransID> GetExamIndentTransID (Guid batchMappingID) {
            return _indentRepository.GetExamIndentTransID(batchMappingID);
        }

        public GetTotalPurchase GetBranchTotalPurchase (Guid tenantBranchID, string fromDate, string toDate) {
            GetTotalPurchase totalPurchase = new GetTotalPurchase();
            decimal material = _indentRepository.GetBranchMaterialPurchase(tenantBranchID, fromDate, toDate);
            decimal exam = _indentRepository.GetBranchExamPurchase(tenantBranchID, fromDate, toDate);
            totalPurchase.Exam = exam;
            totalPurchase.Books = material;
            return totalPurchase;
        }

        public StudentGrade GetStudentGrade (Guid examIndentTransID) {
            return _indentRepository.GetStudentGrade(examIndentTransID);
        }

        public int GetStudentSemesterCount (Guid batchMappingID) {
            return _indentRepository.GetStudentSemesterCount(batchMappingID);
        }

        public IndentSummaryByAdmin GetIndentSummaryByAdmin () {
            return _indentRepository.GetIndentSummaryByAdmin();
        }

        public IndentSummaryByFranchisee GetIndentSummaryByFranchisee () {
            return _indentRepository.GetIndentSummaryByFranchisee(_userIdentityInfo.TenantBranchID);
        }

        public List<GetExamDate> GetExamDate (Guid examIndentID) {
            return _indentRepository.GetExamDate(examIndentID);
        }

        public List<ExamIndentTransInfo> GetExamIndentTransList () {
            return _indentRepository.GetExamIndentTransList();
        }

        public List<PurchaseAmount> GetPurchaseAmount () {
            return _indentRepository.GetPurchaseAmount();
        }

        public string ApprovedMaterialIndent (Guid branchMaterialIndentID) {
            var result = _indentRepository.ApprovedIndent(branchMaterialIndentID, _userIdentityInfo.ContactID);
            return result > 0 ? "Material Indent Approved Successfully" : throw new DawnUserException("Could not approved material indent");
        }

        public string ApprovedExamIndent (Guid examIndentID) {
            var result = _indentRepository.ApprovedExamIndent(examIndentID, _userIdentityInfo.ContactID);
            return result > 0 ? "Exam Indent Approved Successfully" : throw new DawnUserException("Could not approved exam indent");
        }

        public async Task<List<PaymentDetails>> GetPaymentDetails (Guid indentID) {
            List<PaymentDetails> paymentDetails = new List<PaymentDetails>();
            var payments = await _commonService.GetPaymentDetails(indentID);
            foreach (var item in payments) {
                PaymentDetails payment = new PaymentDetails();
                payment.TransactionDate = item.TransactionDate;
                payment.Amount = item.Amount;
                payment.Summary = item.Summary;
                payment.PayMode = item.PayMode;
                var document = await _commonService.GetDocumentID(item.TransID, item.PayMode);
                payment.DocumentID = Guid.Parse(document);
                paymentDetails.Add(payment);
            }
            return paymentDetails;
        }
        public async Task<List<PaymentDetailsBulk>> GetPaymentDetailsByBulk(DataTable keyValues)
        {
            string _connectionString = await GetConnectionString(_userIdentityInfo.TenantID, "Payment");
            CommonRepository commonRepository = new CommonRepository(_sqlI18N, _connectionString);
            var payments = commonRepository.GetPaymentDetails(keyValues);
            List<PaymentDetailsBulk> paymentDetails = new List<PaymentDetailsBulk>();
            //var payments = await _commonService.GetPaymentDetails(indentID);
            foreach (var item in payments)
            {
                PaymentDetailsBulk payment = new PaymentDetailsBulk();
                payment.TransactionDate = item.TransactionDate;
                payment.Amount = item.Amount;
                payment.Summary = item.Summary;
                payment.PayMode = item.PayMode;
                var document = await _commonService.GetDocumentID(item.TransID, item.PayMode);
                payment.DocumentID = Guid.Parse(document);
                payment.KeyValue = item.KeyValue;
                paymentDetails.Add(payment);
            }
            return paymentDetails;
        }

        private async Task<string> GetConnectionString (Guid tenantId, string serviceName) {
            TenantServiceInfo tenantServiceInfo = new() {
                ServiceName = serviceName,
                TenantID = tenantId
            };
            dynamic dbAdminAPIResult = await _dawnHttpClient.PostAsync("dbadmin/get-tenant-constr-on-reg", tenantServiceInfo, false);
            string constring = dbAdminAPIResult.Code == 200 ? dbAdminAPIResult.Data.ToString() : "";
            return constring.ToDecrypt();
        }

        public string[] GetStatusDropdown () {
            string[] level = new string[] { "ALL", "PENDING", "DISPATCHED", "RECEIVED" };
            return level;
        }

        public async Task<GetMaterialInvoice> GetMaterialInvoice (string invoiceNo) {
            GetMaterialInvoice invoiceDetail = new GetMaterialInvoice();
            string _connectionString = await GetConnectionString(_userIdentityInfo.TenantID, "Payment");
            CommonRepository commonRepository = new CommonRepository(_sqlI18N, _connectionString);
            var invoice = commonRepository.GetInvoiceByNumber(invoiceNo);
            var adminContact = await GetAdminContactInfo();
            decimal totalAmount = _indentRepository.GetIndentAmount(invoice.IndentID);
            invoiceDetail.InvoiceNo = invoice.InvoiceNo;
            invoiceDetail.InvoiceDate = invoice.InvoiceDate;
            invoiceDetail.CGST = invoice.CGST;
            invoiceDetail.SGST = invoice.SGST;
            invoiceDetail.IGST = invoice.IGST;
            invoiceDetail.TotalAmount = totalAmount;
            invoiceDetail.Email = adminContact.Email;
            invoiceDetail.PhoneCode = adminContact.PhoneCode;
            invoiceDetail.Phone = adminContact.Phone;
            invoiceDetail.BranchName = adminContact.BranchName;
            invoiceDetail.TotalInWords = ConvertToRupees(totalAmount);
            invoiceDetail.BillingTo = DeserializeAddress(invoice.BillingAddress);
            invoiceDetail.ShippingTo = DeserializeAddress(invoice.ShippingAddress);
            invoiceDetail.AdminAddress = await GetHOAddress();
            invoiceDetail.IndentDetails = _indentRepository.GetMaterialIndentInfo(invoice.IndentID);
            return invoiceDetail;
        }

        //public async Task<Guid> CreateCertificateIndentReprinting(ReprintIndent reprint)
        //{

        //    decimal walletBalance;
        //    string walletPaymentID = String.Empty;
        //    if (reprint == null)
        //        throw new DawnUserException("Invalid data !");
        //    //Check franchisee wallet balance
        //    string connectionString = await GetConnectionString(_userIdentityInfo.TenantID, "Payment");//wallet payment
        //    CommonRepository commonRepository = new CommonRepository(_sqlI18N, connectionString);
        //    var walletItems = reprint.PaymentItems.Where(p => p?.PayModeType.ToUpperCase() == "WALLET");
        //    var walletTotalAmount = walletItems.Sum(p => p.Amount);
        //    if (walletTotalAmount > 0)
        //    {
        //        walletBalance = commonRepository.GetWalletBalance(_userIdentityInfo.TenantBranchID);
        //        if (walletBalance < walletTotalAmount)
        //            throw new DawnUserException("Insufficient wallet balance");
        //    }
        //    string receiverContactID = await _commonService.GetAdminContactID();
        //    string receiverBranchID = await _commonService.GetAdminBranchID(_userIdentityInfo.TenantID);

        //    DataTable dataList = reprint.Reprint.ToDataTable();
        //    string _connectionString = await GetConnectionString(_userIdentityInfo.TenantID, "Academic");
        //    CommonRepository commonRepo = new CommonRepository(_sqlI18N, _connectionString);
        //    var certificateIndent = commonRepo.CreateReprintCertificateIndent(dataList, _userIdentityInfo.ContactID, _userIdentityInfo.TenantBranchID,reprint.CourierAmount,reprint.TotalAmount);
        //    if(string.IsNullOrEmpty(certificateIndent.IndentNumber)) throw new DawnUserException("Couldn't send indent request !");


        //    List<NewPaymentItems> newItems = new List<NewPaymentItems>();
        //    foreach (var paymentItems in reprint.PaymentItems)
        //    {
        //        FranchiseeToAdminPay paymentDetails = paymentItems.Mapper<FranchiseeToAdminPay>();
        //        var paymentDetail = JsonConvert.SerializeObject(paymentDetails);
        //        NewPaymentItems newPaymentItems = paymentItems.Mapper<NewPaymentItems>();
        //        newPaymentItems.PaymentModeID = paymentItems.PaymentModeID;
        //        newPaymentItems.PayModeType = paymentItems.PayModeType;
        //        newPaymentItems.TransactionDate = paymentItems.TransactionDate;
        //        newPaymentItems.BankName = paymentItems.FromLedgerName;
        //        newPaymentItems.PaymentDetails = paymentDetail;
        //        newItems.Add(newPaymentItems);
        //        /*Create wallet payment */
        //        if (paymentItems?.PayModeType.ToUpperCase() == "WALLET")
        //        {
        //            WalletPayment wallet = new WalletPayment();
        //            Guid createdBy = _userIdentityInfo.ContactID;
        //            wallet.TenantBranchID = _userIdentityInfo.TenantBranchID;
        //            wallet.CreatedBy = createdBy;
        //            wallet.SenderID = createdBy;
        //            wallet.ReceiverID = Guid.Parse(receiverContactID);
        //            wallet.WalletAmount = walletTotalAmount;
        //            wallet.Description = "Reprint Fees : " + walletTotalAmount + ", CertificateIndentNo :" + certificateIndent.IndentNumber;
        //            wallet.WalletType = "REPRINT INDENT";
        //            walletPaymentID = commonRepository.CreateWalletPayment(wallet).ToString();
        //        }
        //    }
        //    Receipt receipt = new Receipt();
        //    receipt.TenantBranchID = _userIdentityInfo.TenantBranchID;
        //    receipt.KeyTable = "CertificateAndMarksheetIndent";
        //    receipt.KeyColumn = "CertificateIndentID";
        //    receipt.KeyValue = certificateIndent.CertificateIndentID;
        //    string transactionName = "REPRINT FEES";
        //    receipt.TransactionName = transactionName;
        //    receipt.PaymentItems = newItems;
        //    receipt.CreatedBy = _userIdentityInfo.ContactID;
        //    receipt.VoucherDate = DateTime.Now.ToString("yyyy-MM-dd");
        //    string payments = await _commonService.CreatePayment(receipt);
        //    TempDocumentActivate temp = new TempDocumentActivate();
        //    List<DocumentItem> documents = new List<DocumentItem>();
        //    foreach (var item in reprint.PaymentItems)
        //    {
        //        if (item.DocumentID != Guid.Empty)
        //        {
        //            string transID = await _commonService.GetPaymentTransID(Guid.Parse(payments), item.TransactionNo);
        //            documents.Add(new DocumentItem { DocumentID = item.DocumentID, EntityID = Guid.Parse(transID) });
        //            temp.DocumentItem = documents;
        //        }
        //    }
        //    if (temp.DocumentItem != null)
        //    {
        //        var activateDocument = await _commonService.TempDocumentActivated(temp);
        //    }
        //    if (string.IsNullOrEmpty(payments) || (walletTotalAmount > 0 && string.IsNullOrEmpty(walletPaymentID)))
        //    {
        //        //Rollback all above transaction
        //        int result = commonRepo.RollbackReprintIndent(certificateIndent.CertificateIndentID);
        //        if (!string.IsNullOrEmpty(payments))
        //        {
        //            string rollbackPayment = await _commonService.RollBackPayment(certificateIndent.CertificateIndentID); //Added 30/06
        //        }
        //        if (walletTotalAmount > 0 && !string.IsNullOrEmpty(walletPaymentID))//Added 30/06
        //        {
        //            int rollbackWallet = commonRepository.RollbackWallet(Guid.Parse(walletPaymentID));//Added 30/06
        //        }
        //        throw new DawnUserException("Couldn't create reprint indent");
        //    }
        //    bool accounting = await _commonService.FranchiseeToAdmin(reprint.PaymentItems, _userIdentityInfo.TenantBranchID, Guid.Parse(receiverBranchID), _userIdentityInfo.ContactID, Guid.Parse(receiverContactID), Guid.Parse(payments), transactionName);
        //    if (accounting == false)
        //    {
        //        var rollbackPayment = await _commonService.RollBackPayment(certificateIndent.CertificateIndentID);
        //        int result = commonRepo.RollbackReprintIndent(certificateIndent.CertificateIndentID);
        //        if (walletTotalAmount > 0)
        //        {
        //            int rollbackWallet = commonRepository.RollbackWallet(Guid.Parse(walletPaymentID));//Added 30/06
        //        }
        //        throw new DawnUserException("Couldn't create reprint indent");
        //    }
        //    return certificateIndent.CertificateIndentID;
        //}
        #region Reprint
        public async Task<Guid> CreateCertificateIndentReprintingUpdated(ReprintIndent reprint)
        {
            if (reprint == null)
                throw new DawnUserException("Invalid data!");

            decimal walletTotalAmount = CalculateWalletTotal(reprint.PaymentItems);
            string receiverContactID = await _commonService.GetAdminContactID();
            string receiverBranchID = await _commonService.GetAdminBranchID(_userIdentityInfo.TenantID);

            string paymentConnection = await GetConnectionString(_userIdentityInfo.TenantID, "Payment");
            string academicConnection = await GetConnectionString(_userIdentityInfo.TenantID, "Academic");

            CommonRepository paymentRepo = new CommonRepository(_sqlI18N, paymentConnection);
            CommonRepository academicRepo = new CommonRepository(_sqlI18N, academicConnection);


            var payModeIDs = reprint.PaymentItems.Select(p => p.PaymentModeID).Distinct();
            var payModeType = payModeIDs.Select(id => paymentRepo.GetPayModeType(id)).Where(type => !string.IsNullOrWhiteSpace(type)).ToList();

            string walletPaymentID = null;
            string paymentID = null;
            Guid indentID = Guid.Empty;

            try
            {
                if (walletTotalAmount > 0)
                {
                    decimal walletBalance = paymentRepo.GetWalletBalance(_userIdentityInfo.TenantBranchID);
                    if (walletBalance < walletTotalAmount)
                        throw new DawnUserException("Insufficient wallet balance");
                }
                DataTable indentTable = reprint.Reprint.ToDataTable();
                var indent = academicRepo.CreateReprintCertificateIndent(indentTable, _userIdentityInfo.ContactID, _userIdentityInfo.TenantBranchID, reprint.CourierAmount, reprint.TotalAmount);
                if (string.IsNullOrEmpty(indent.IndentNumber)) throw new DawnUserException("Couldn't send indent request!");
                indentID = indent.CertificateIndentID;

                List<NewPaymentItems> newPaymentItems = BuildNewPaymentItems(reprint.PaymentItems);

                if (walletTotalAmount > 0)
                {
                    string description = $"Reprint Fees : {walletTotalAmount}, CertificateIndentNo : {indent.IndentNumber} ({string.Join(", ", payModeType)})";
                    string walletType = "REPRINT INDENT";
                    walletPaymentID = CreateWalletTransaction(paymentRepo, walletTotalAmount, receiverContactID, description, walletType);
                }

                string transactionName = "REPRINT FEES";
                paymentID = await CreateReceiptAndPayments(newPaymentItems, indentID, transactionName, "CertificateAndMarksheetIndent", "CertificateIndentID");

                await TryActivateDocuments(reprint.PaymentItems, paymentID);

                if (string.IsNullOrEmpty(paymentID) || (walletTotalAmount > 0 && string.IsNullOrEmpty(walletPaymentID)))
                    throw new Exception("Incomplete payment or wallet transaction");

                bool accountingSuccess = await _commonService.FranchiseeToAdmin(
                    reprint.PaymentItems,
                    _userIdentityInfo.TenantBranchID,
                    Guid.Parse(receiverBranchID),
                    _userIdentityInfo.ContactID,
                    Guid.Parse(receiverContactID),
                    Guid.Parse(paymentID),
                    transactionName);

                if (!accountingSuccess)
                    throw new Exception("Accounting transaction failed");

                return indentID;
            }
            catch (Exception)
            {
                if (indentID != Guid.Empty) academicRepo.RollbackReprintIndent(indentID);
                if (!string.IsNullOrEmpty(paymentID)) await _commonService.RollBackPayment(indentID);
                if (!string.IsNullOrEmpty(walletPaymentID)) paymentRepo.RollbackWallet(Guid.Parse(walletPaymentID));
                throw new DawnUserException("Couldn't create reprint indent. Please try again.");
            }
        }
        private decimal CalculateWalletTotal(List<FranchiseeToAdminPay> paymentItems)
        {
            return paymentItems?.Where(p => p?.PayModeType?.ToUpper() == "WALLET")?.Sum(p => p.Amount) ?? 0;
        }
        private List<NewPaymentItems> BuildNewPaymentItems(List<FranchiseeToAdminPay> items)
        {
            return items.Select(item =>
            {
                var newItem = item.Mapper<NewPaymentItems>();
                newItem.PaymentModeID = item.PaymentModeID;
                newItem.PayModeType = item.PayModeType;
                newItem.TransactionDate = item.TransactionDate;
                newItem.BankName = item.FromLedgerName;
                newItem.PaymentDetails = JsonConvert.SerializeObject(item.Mapper<FranchiseeToAdminPay>());
                return newItem;
            }).ToList();
        }
        private string CreateWalletTransaction(CommonRepository repo, decimal walletTotalAmount, string receiverID,string description,string walletType)
        {
            var wallet = new WalletPayment
            {
                TenantBranchID = _userIdentityInfo.TenantBranchID,
                CreatedBy = _userIdentityInfo.ContactID,
                SenderID = _userIdentityInfo.ContactID,
                ReceiverID = Guid.Parse(receiverID),
                WalletAmount = walletTotalAmount,
                Description = description,
                WalletType = walletType
            };
            return repo.CreateWalletPayment(wallet).ToString();
        }
        private async Task<string> CreateReceiptAndPayments(List<NewPaymentItems> items, Guid indentID, string transactionName,string keyTable,string keyColumn)
        {
            var receipt = new Receipt
            {
                TenantBranchID = _userIdentityInfo.TenantBranchID,
                KeyTable = keyTable,
                KeyColumn = keyColumn,
                KeyValue = indentID,
                TransactionName = transactionName,
                PaymentItems = items,
                CreatedBy = _userIdentityInfo.ContactID,
                VoucherDate = DateTime.Now.ToString("yyyy-MM-dd")
            };
            return await _commonService.CreatePayment(receipt);
        }
        private async Task TryActivateDocuments(List<FranchiseeToAdminPay> items, string paymentID)
        {
            var docItems = new List<DocumentItem>();

            foreach (var item in items)
            {
                if (item.DocumentID != Guid.Empty)
                {
                    string transID = await _commonService.GetPaymentTransID(Guid.Parse(paymentID), item.TransactionNo);
                    docItems.Add(new DocumentItem
                    {
                        DocumentID = item.DocumentID,
                        EntityID = Guid.Parse(transID)
                    });
                }
            }
            if (docItems.Any())
            {
                await _commonService.TempDocumentActivated(new TempDocumentActivate { DocumentItem = docItems });
            }
        }
        private async Task RollbackIndent(Guid indentID, string paymentID, string walletPaymentID, CommonRepository academicRepo, CommonRepository paymentRepo)
        {
            await _commonService.RollBackPayment(indentID);
            academicRepo.RollbackReprintIndent(indentID);
            if (!string.IsNullOrEmpty(walletPaymentID))
            {
                paymentRepo.RollbackWallet(Guid.Parse(walletPaymentID));
            }
        }
        #endregion
        #region IDCard
        public async Task<Guid> CreateIDCardIndent(IDCardIndent idCard)
        {
            if (idCard == null)
                throw new DawnUserException("Invalid data!");

            decimal walletTotalAmount = CalculateWalletTotal(idCard.PaymentItems);
            string receiverContactID = await _commonService.GetAdminContactID();
            string receiverBranchID = await _commonService.GetAdminBranchID(_userIdentityInfo.TenantID);

            string paymentConnection = await GetConnectionString(_userIdentityInfo.TenantID, "Payment");
            string franchiseeConnection = await GetConnectionString(_userIdentityInfo.TenantID, "Franchisee");
            CommonRepository paymentRepo = new CommonRepository(_sqlI18N, paymentConnection);
            CommonRepository franchiseeRepo = new CommonRepository(_sqlI18N, franchiseeConnection);
            
            var payModeIDs = idCard.PaymentItems.Select(p => p.PaymentModeID).Distinct();
            var payModeType = payModeIDs.Select(id => paymentRepo.GetPayModeType(id)).Where(type => !string.IsNullOrWhiteSpace(type)).ToList();

            string walletPaymentID = null;
            string paymentID = null;
            Guid indentID = Guid.Empty;

            try
            {
                if (walletTotalAmount > 0)
                {
                    decimal walletBalance = paymentRepo.GetWalletBalance(_userIdentityInfo.TenantBranchID);
                    if (walletBalance < walletTotalAmount)
                        throw new DawnUserException("Insufficient wallet balance");
                }
                /*Get id card charges*/
                int idCardCharge = franchiseeRepo.GetIDCardCharges();
                var batchMapIDs = idCard.IDCard.Select(a => new { a.BatchMappingID}).ToList().ToDataTable();
                var indent = _indentRepository.CreateIDCardIndent(batchMapIDs, _userIdentityInfo.ContactID, _userIdentityInfo.TenantBranchID, idCard.CourierAmount, idCard.TotalAmount, idCardCharge);
                if (string.IsNullOrEmpty(indent.IndentNumber)) throw new DawnUserException("Couldn't send indent request!");
                indentID = indent.IDCardIndentID;

                List<NewPaymentItems> newPaymentItems = BuildNewPaymentItems(idCard.PaymentItems);

                if (walletTotalAmount > 0)
                {
                    string description = $"IDCard Fees : {walletTotalAmount}, IDCardIndentNo : {indent.IndentNumber} ({string.Join(", ", payModeType)})";
                    string walletType = "IDCARD INDENT";
                    walletPaymentID = CreateWalletTransaction(paymentRepo, walletTotalAmount, receiverContactID, description, walletType);
                }

                string transactionName = "IDCARD FEES";
                paymentID = await CreateReceiptAndPayments(newPaymentItems, indentID, transactionName, "IDCardIndent", "IDCardIndentID");

                await TryActivateDocuments(idCard.PaymentItems, paymentID);

                if (string.IsNullOrEmpty(paymentID) || (walletTotalAmount > 0 && string.IsNullOrEmpty(walletPaymentID)))
                    throw new Exception("Incomplete payment or wallet transaction");

                bool accountingSuccess = await _commonService.FranchiseeToAdmin(
                    idCard.PaymentItems,
                    _userIdentityInfo.TenantBranchID,
                    Guid.Parse(receiverBranchID),
                    _userIdentityInfo.ContactID,
                    Guid.Parse(receiverContactID),
                    Guid.Parse(paymentID),
                    transactionName);

                if (!accountingSuccess)
                    throw new Exception("Accounting transaction failed");
                return indentID;
            }
            catch (Exception ex)
            {
                if (indentID != Guid.Empty) _indentRepository.RollbackIDCardIndent(indentID);
                if (!string.IsNullOrEmpty(paymentID)) await _commonService.RollBackPayment(indentID);
                if (!string.IsNullOrEmpty(walletPaymentID)) paymentRepo.RollbackWallet(Guid.Parse(walletPaymentID));
                throw new DawnUserException("Couldn't create id card indent. Please try again.");
                //throw new Exception(ex.Message);
            }           
        }
        public string[] GetIDCardStatusDropdown()
        {
            string[] level = new string[] { "ALL", "PENDING", "APPROVED", "DESPATCHED", "RECEIVED","REJECTED" };
            return level;
        }
        public List<IDCardIndentList> GetIDCardIntentList(string fromDate, string toDate, string status)
        {
            var indentList = _indentRepository.GetIDCardIntentList(fromDate,toDate,status,_userIdentityInfo.TenantBranchID);
            return indentList;
        }
        public async Task<IDCardIndentDetails> GetIDCardIndentDetails(Guid idCardIndentID)
        {
            var details = new IDCardIndentDetails();
            DataSet indentDataSet = _indentRepository.GetIDCardIndentDetails(_userIdentityInfo.TenantBranchID, idCardIndentID);
            details = indentDataSet.Tables[0].DataTablesToItem<IDCardIndentDetails>();
            var detailsList = indentDataSet.Tables[1].DataTablesToList<IDCardIndentDetailsList>();
            var batchMapingIds = detailsList.Select(a => new { a.BatchMappingID, ExamIndentTransID = Guid.Empty}).ToList().ToDataTable();//here ExamIndentTransID is nothing dummy declatation for SQL USER DEFINED TABLE DATATYPE

            string _connectionString = await GetConnectionString(_userIdentityInfo.TenantID, "Academic");
            var commonRepository = new CommonRepository(_sqlI18N, _connectionString);
            var studentInfos = commonRepository.GetStudentInfo(batchMapingIds);
            var contact = await _commonService.ContactInfo(string.Empty);
            var contactName = contact.ToDictionary(c=>c.ContactID,c=>c.ContactName);
            var contactPhone = contact.ToDictionary(c => c.ContactID, c => c.Phone);

            foreach (var studentInfo in studentInfos)
            {
                var info = new IDCardStudentInfo();
                info.IDCardIndentTransID = detailsList.Where(a => a.BatchMappingID == studentInfo.BatchMappingID).Select(a => a.IDCardIndentTransID).FirstOrDefault();
                info.BatchMappingID = studentInfo.BatchMappingID;
                info.StudentID = studentInfo.StudentID;
                info.ContactID = studentInfo.ContactID;
                info.StudentName = contactName.GetValueOrDefault(studentInfo.ContactID) ?? "";
                info.Phone = contactPhone.GetValueOrDefault(studentInfo.ContactID) ?? "";
                info.BatchNumber = studentInfo.BatchNumber;
                info.BatchTime = studentInfo.BatchTime;
                info.RollNumber = studentInfo.RollNumber;
                info.Course = studentInfo.Course;
                info.ShortName = studentInfo.ShortName;
                info.CourseTypeName = studentInfo.CourseTypeName;
                info.Status = detailsList.Where(a => a.BatchMappingID == studentInfo.BatchMappingID).Select(a => a.Status).FirstOrDefault() ?? "";

                details.StudentInfo.Add(info);
            }
            details.PaymentDetails = await GetPaymentDetails(idCardIndentID);
            return details;
        }
        public async Task<List<IDCardListByAdmin>> GetIDCardIndentListByAdmin()
        {
            var detailsAdmin =new List<IDCardListByAdmin>();
            var indentList = _indentRepository.GetIDCardIndentListByAdmin();
            var tenantBranchIds = indentList.Select(a => new { a.TenantBranchID }).Distinct().ToList().ToDataTable();

            string _connectionString = await GetConnectionString(_userIdentityInfo.TenantID, "Franchisee");
            var commonRepository = new CommonRepository(_sqlI18N, _connectionString);
            var branchDetails = commonRepository.FranchiseeDetailsForIDCard(tenantBranchIds);
            foreach(var details in indentList)
            {
                IDCardListByAdmin id = new IDCardListByAdmin();
                id = details.Mapper<IDCardListByAdmin>();
                var branch = branchDetails.FirstOrDefault(a => a.TenantBranchID == details.TenantBranchID);
                id.franchiseeDetails = branch.Mapper<FranchiseeDetailsForIDCard>();
                detailsAdmin.Add(id);
            }
            return detailsAdmin;
        }
        public async Task<IDcardIndentNumber> ViewIDCardPaymentDetailsByAdmin(Guid iDCardIndentID)
        {
            var indent = new IDcardIndentNumber();
            indent = _indentRepository.GetIDcardIndentNumber(iDCardIndentID);

            var indentList = _indentRepository.IndentDetailsAdmin(iDCardIndentID);
            var batchMapingIds = indentList.Select(a => new { a.BatchMappingID, ExamIndentTransID = Guid.Empty }).ToList().ToDataTable();//here ExamIndentTransID is nothing dummy declatation for SQL USER DEFINED TABLE DATATYPE

            string connectionString = await GetConnectionString(_userIdentityInfo.TenantID, "Academic");
            var academicRepository = new CommonRepository(_sqlI18N, connectionString);
            var studentInfos = academicRepository.GetStudentInfo(batchMapingIds);
            var contact = await _commonService.ContactInfo(string.Empty);
            var contactName = contact.ToDictionary(c => c.ContactID, c => c.ContactName);
            var contactPhone = contact.ToDictionary(c => c.ContactID, c => c.Phone);

            foreach (var studentInfo in studentInfos)
            {
                var info = new IDCardStudentInfoUpdated();
                info.IDCardIndentTransID = indentList.Where(a => a.BatchMappingID == studentInfo.BatchMappingID).Select(a => a.IDCardIndentTransID).FirstOrDefault();
                info.BatchMappingID = studentInfo.BatchMappingID;
                info.StudentID = studentInfo.StudentID;
                info.ContactID = studentInfo.ContactID;
                info.StudentName = contactName.GetValueOrDefault(studentInfo.ContactID) ?? "";
                info.Phone = contactPhone.GetValueOrDefault(studentInfo.ContactID) ?? "";
                info.BatchNumber = studentInfo.BatchNumber;
                info.BatchTime = studentInfo.BatchTime;
                info.RollNumber = studentInfo.RollNumber;
                info.Course = studentInfo.Course;
                info.ShortName = studentInfo.ShortName;
                info.CourseTypeName = studentInfo.CourseTypeName;
                info.Status = indentList.Where(a => a.BatchMappingID == studentInfo.BatchMappingID).Select(a => a.Status).FirstOrDefault() ?? "";
                info.IDCardFees = indentList.Where(a => a.BatchMappingID == studentInfo.BatchMappingID).Select(a => a.IDCardFees).FirstOrDefault();

                indent.StudentInfo.Add(info);
            }

            string _connectionString = await GetConnectionString(_userIdentityInfo.TenantID, "Franchisee");
            var commonRepository = new CommonRepository(_sqlI18N, _connectionString);
            var branchDetails = commonRepository.BranchDetail(indent.TenantBranchID);

            indent.franchiseeDetails = branchDetails;
            indent.PaymentDetails = await GetPaymentDetails(iDCardIndentID);
            return indent;
        }
        public Guid ApproveIDCardIndent(Guid iDCardIndentID)
        {
            var approve = _indentRepository.ApproveIDCardIndent(iDCardIndentID,_userIdentityInfo.ContactID);
            return approve;
        }
        public Guid RejectIDCardIndent(Guid iDCardIndentID, string reason)
        {
            var reject = _indentRepository.RejectIDCardIndent(iDCardIndentID, reason,_userIdentityInfo.ContactID);
            return reject;
        }
        public string IDCardIndentPrinted(IDCardPrinted print)
        {
            var transIDs = print.TransIDs.Select(a => new { a.IDCardIndentTransID }).ToList().ToDataTable();
            var printUpdate = _indentRepository.IDCardIndentPrinted(transIDs,print.IDCardIndentID,_userIdentityInfo.ContactID);
            return printUpdate > 0 ? "Print Status Updated Successfully" : throw new Exception("Something went wrong");
        }

        public async Task<List<DespatchList>> GetDespatchList()
        {
            var despatchList = new List<DespatchList>();
            var indentList = _indentRepository.GetDespatchList();
            var tenantBranchIds = indentList.Select(a => new { a.TenantBranchID }).Distinct().ToList().ToDataTable();

            string _connectionString = await GetConnectionString(_userIdentityInfo.TenantID, "Franchisee");
            var commonRepository = new CommonRepository(_sqlI18N, _connectionString);
            var branchDetails = commonRepository.FranchiseeDetailsForIDCard(tenantBranchIds);
            foreach (var details in indentList)
            {
                DespatchList id = new DespatchList();
                id = details.Mapper<DespatchList>();
                var branch = branchDetails.FirstOrDefault(a => a.TenantBranchID == details.TenantBranchID);
                id.franchiseeDetails = branch.Mapper<FranchiseeDetailsForIDCard>();
                despatchList.Add(id);
            }
            return despatchList;
        }
        public string DespatchIDCardIndent(NewDespatchIDCard newDespatch)
        {

            var despatch = _indentRepository.DespatchIDCardIndent(newDespatch.DespatchMaterial.ToDataTable(),_userIdentityInfo.ContactID);
            return despatch > 0 ? "Indent Despatched Successfully" : throw new Exception("Something went wrong");
        }
        public string ReceiveIDCardIndent(List<IndentTransID> receive)
        {

            var received = _indentRepository.ReceiveIDCardIndent(receive.ToDataTable(), _userIdentityInfo.ContactID);
            return received > 0 ? "Indent Received Successfully" : throw new Exception("Something went wrong");
        }
        public async Task<List<IDCardListByAdmin>> GetIDCardIndentPrintList()
        {
            var detailsAdmin = new List<IDCardListByAdmin>();
            var indentList = _indentRepository.GetIDCardIndentPrintList();
            var tenantBranchIds = indentList.Select(a => new { a.TenantBranchID }).Distinct().ToList().ToDataTable();

            string _connectionString = await GetConnectionString(_userIdentityInfo.TenantID, "Franchisee");
            var commonRepository = new CommonRepository(_sqlI18N, _connectionString);
            var branchDetails = commonRepository.FranchiseeDetailsForIDCard(tenantBranchIds);
            foreach (var details in indentList)
            {
                IDCardListByAdmin id = new IDCardListByAdmin();
                id = details.Mapper<IDCardListByAdmin>();
                var branch = branchDetails.FirstOrDefault(a => a.TenantBranchID == details.TenantBranchID);
                id.franchiseeDetails = branch.Mapper<FranchiseeDetailsForIDCard>();
                detailsAdmin.Add(id);
            }
            return detailsAdmin;
        }
        #endregion
        private GetBranchAddress DeserializeAddress (string address) {
            return JsonConvert.DeserializeObject<GetBranchAddress>(address);
        }

        private async Task<GetBranchAddress> GetHOAddress () {
            string _connectionString = await GetConnectionString(_userIdentityInfo.TenantID, "TenantBranch");
            CommonRepository commonRepository = new CommonRepository(_sqlI18N, _connectionString);
            var address = commonRepository.GetHOAddress();
            return address;
        }

        private async Task<GetAdminContactInfo> GetAdminContactInfo () {
            string _connectionString = await GetConnectionString(_userIdentityInfo.TenantID, "TenantBranch");
            CommonRepository commonRepository = new CommonRepository(_sqlI18N, _connectionString);
            var contact = commonRepository.GetAdminContact();
            return contact;
        }

        private static readonly string[] Units = { "", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine" };
        private static readonly string[] Teens = { "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" };
        private static readonly string[] Tens = { "", "", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };
        private static readonly string[] Thousands = { "", "Thousand", "Lakh", "Crore" };
        public static string ConvertToRupees (decimal amount) {
            if (amount == 0)
                return "Zero Rupees Only";
            string result = ConvertToWords((long)amount);
            return result + " Rupees Only";
        }
        private static string ConvertToWords (long number) {
            if (number == 0)
                return "";
            string words = "";
            if (number / 10000000 > 0) {
                words += ConvertToWords(number / 10000000) + " Crore ";
                number %= 10000000;
            }
            if (number / 100000 > 0) {
                words += ConvertToWords(number / 100000) + " Lakh ";
                number %= 100000;
            }
            if (number / 1000 > 0) {
                words += ConvertToWords(number / 1000) + " Thousand ";
                number %= 1000;
            }
            if (number / 100 > 0) {
                words += ConvertToWords(number / 100) + " Hundred ";
                number %= 100;
            }
            if (number > 0) {
                if (words != "")
                    words += "and ";

                if (number < 10)
                    words += Units[number];
                else if (number < 20)
                    words += Teens[number - 10];
                else {
                    words += Tens[number / 10];
                    if ((number % 10) > 0)
                        words += " " + Units[number % 10];
                }
            }
            return words.Trim();
        }

        public async Task CreateAndDownloadReportDocument (string reportName, object parameters, string format) {
            using (var client = new HttpClient()) {
                client.BaseAddress = new Uri(_configuration["DawnConfig:ReportServer:ReportServerApiUrl"]);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                // Authenticate via OAuth 2.0 Password Grant.
                var authToken = GetAuthenticationToken(client, _configuration["DawnConfig:ReportServer:Username"], _configuration["DawnConfig:ReportServer:Password"]);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
                // List all reports.
                var reports = await GetAllReports(client, _configuration["DawnConfig:ReportServer:ReportServerApiUrl"]);
                // Render the desired report.
                var report = reports.Find(r => r["Name"].Equals(reportName))["Id"];
                //WriteToLog("Create document");
                var documentUrl = await CreateDocument(client, _configuration["DawnConfig:ReportServer:ReportServerApiUrl"], format, report, parameters);
                // Download the created document.
                var downloadPath = DownloadDocumentToWwwroot(documentUrl, reportName, false, _webHostEnvironment);
            }
        }

        private string GetAuthenticationToken (HttpClient client, string usernameInput, string passwordInput) {
            var data = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("username", usernameInput),
                new KeyValuePair<string, string>("password", passwordInput)
            });
            // POST Token
            var response = client.PostAsync(_configuration["DawnConfig:ReportServer:ReportServerUrl"] + "Token", data).Result;
            response.EnsureSuccessStatusCode();
            var result = response.Content.ReadAsStringAsync().Result;
            var token = JObject.Parse(result).SelectToken("access_token").ToString();
            return token;
        }

        private async Task<List<Dictionary<string, string>>> GetAllReports (HttpClient client, string apiUrl) {
            // GET api/reportserver/reports
            var response = await client.GetAsync(apiUrl + "reports");
            if (response.IsSuccessStatusCode) {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(content);
            }
            return null;
        }

        private async Task<string> CreateDocument (HttpClient client, string apiUrl, string format, string reportId, object parameterValues) {
            var data = new { ReportId = reportId, Format = format, ParameterValues = parameterValues };
            // POST api/reportserver/documents
            var response = await client.PostAsJsonAsync(apiUrl + "documents", data);
            response.EnsureSuccessStatusCode();
            var documentUrl = response.Headers.Location;
            return documentUrl.ToString();
        }


        private string DownloadDocumentToWwwroot (string url, string reportName, bool asAttachment, IWebHostEnvironment env) {
            // Define the file path in the wwwroot folder
            var wwwrootPath = Path.Combine(env.WebRootPath, "downloads");
            // Ensure the folder exists
            if (!Directory.Exists(wwwrootPath)) {
                Directory.CreateDirectory(wwwrootPath);
            }
            // Define file name and path
            var fileName = $"{reportName}.pdf";
            var filePath = Path.Combine(wwwrootPath, fileName);
            if (System.IO.File.Exists(filePath)) {
                System.IO.File.Delete(filePath);
            }
            var queryString = asAttachment ? "?content-disposition=attachment" : "";
            // Download the file
            using (var webClient = new System.Net.WebClient()) {
                webClient.DownloadFile(url + queryString, filePath);
            }
            return $"/downloads/{fileName}";
        }
        #endregion
    }
}