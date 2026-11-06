using Dawn.Frameworks.Entities;
using Dawn.Frameworks.Helper;
using Dawn.Utilities.Enums;
using Dawn.Utilities.Exceptions;
using Dawn.Utilities.i18N;
using EntitiesDTO.Material;
using EntitiesDTO.Student;
using Frameworks.Entities;
using Frameworks.Helper;
using iCloud.Inventory.Services;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Text;

namespace iCloud.Inventory.Controllers {
    [ApiVersion("1.0")]
    [Route("api/v{v:apiVersion}/indent/")]
    [ApiController]
    public class IndentV1Controller : ControllerBase {
        #region Fields
        private readonly IndentService _indentService;
        private readonly IdentityInfo _userIdentityInfo;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;
        #endregion
        #region Constructor
        public IndentV1Controller (SqlI18N sqlI18N, IHttpContextAccessor accessor, IDawnHttpClient dawnHttpClient, IConfiguration configuration, IWebHostEnvironment environment) {
            _indentService = new IndentService(sqlI18N, accessor, dawnHttpClient, configuration, environment);
            _userIdentityInfo = accessor.GetUserIdentityInfoDetails();
            _configuration = configuration;
            _environment = environment;
        }
        #endregion
        #region Methods
        /// <summary>
        /// Create delivery mode
        /// </summary>
        /// <param name="deliveryMode"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpPost("create/deliverymode")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public APIResult<Guid> CreateDeliveryMode (CreateDeliveryMode deliveryMode) {
            Guid deliveryModeID;
            try {
                deliveryModeID = _indentService.CreateDeliveryMode(deliveryMode);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return APIResult.Success(deliveryModeID, "Delivery Mode created successfully !", DawnStatusCode.Created);
        }

        /// <summary>
        /// Get delivery mode list
        /// </summary>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("deliverymode/list")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public List<DeliveryModeList> GetDeliveryModeList () {
            List<DeliveryModeList> deliveryModes;
            try {
                deliveryModes = _indentService.GetDeliveryModeList();
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.NotExist);
            }
            return deliveryModes;
        }

        /// <summary>
        /// Create new material indent
        /// </summary>
        /// <param name="materialIndent"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpPost("material/new")]
        [DawnAuthorize("TenantUser")]
        public async Task<APIResult<Guid>> CreateMaterialIndent (CreateNewMaterialIndent materialIndent) {
            Guid tenantBranchID;
            try {
                tenantBranchID = await _indentService.CreateMaterialIndent(materialIndent);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return APIResult.Success(tenantBranchID, "Material indent request sent successfully !", DawnStatusCode.Created);
        }
        [HttpGet("material/receipt")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<APIResult<IndentReceipt>> MaterialIndentReceipt(Guid branchMaterialIndentID)
        {
            try
            {
                return APIResult.Success( await _indentService.MaterialIndentReceipt(branchMaterialIndentID), "Material Indent Receipt details fetched successfully", DawnStatusCode.Success);
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }
        /// <summary>
        /// Accept material indent request
        /// </summary>
        /// <param name="indent"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpPost("material/accept")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public APIResult<string> AcceptMaterialIndent (ApproveMaterialIndent indent) {
            string result;
            try {
                result = _indentService.AcceptMaterialIndent(indent);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return APIResult.Success("", result, DawnStatusCode.Created);
        }

        /// <summary>
        /// Split material indent
        /// </summary>
        /// <param name="indent"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpPost("material/split")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public APIResult<Guid> SplitMaterialIndent (SplitMaterialIndent indent) {
            Guid result;
            try {
                result = _indentService.SplitMaterialIndent(indent);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return APIResult.Success(result, "Material indent splited successfully", DawnStatusCode.Created);
        }

        /// <summary>
        /// Reject material indent request
        /// </summary>
        /// <param name="indent"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpPost("material/reject")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<APIResult<Guid>> RejectMaterialIndent (RejectMaterialIndent indent) {
            Guid result;
            try {
                result = await _indentService.RejectMaterialIndent(indent);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return APIResult.Success(result, "Material indent request rejected !", DawnStatusCode.Created);
        }

        /// <summary>
        /// Get material despatch list
        /// </summary>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("despatch/list")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<IEnumerable<MaterialDespatchList>> GetMaterialDespatchList () {
            IEnumerable<MaterialDespatchList> despatchLists;
            try {
                despatchLists = await _indentService.GetMaterialDespatchList();
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return despatchLists;
        }

        /// <summary>
        /// Despatch material item
        /// </summary>
        /// <param name="despatchMaterial"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpPost("material/despatch/new")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<APIResult<string>> DespatchMaterialItem (NewDespatchMaterial despatchMaterial) {
            string result;
            try {
                result = await _indentService.DespatchMaterialItem(despatchMaterial);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return APIResult.Success("", result, DawnStatusCode.Created);
        }

        /// <summary>
        /// Get material receivable list
        /// </summary>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("material/receivable/list")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<List<GetReceivableList>> GetMaterialReceivableList () {
            List<GetReceivableList> receivableLists;
            try {
                receivableLists = await _indentService.GetMaterialReceivableList();
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return receivableLists;
        }

        /// <summary>
        /// Get material received list
        /// </summary>
        /// <param name="indent"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpPost("material/received")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public APIResult<string> ReceiveMaterialIndentItem (ApproveMaterialIndent indent) {
            string result;
            try {
                result = _indentService.ReceiveMaterialIndentItem(indent);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return APIResult.Success("", result, DawnStatusCode.Created);
        }

        /// <summary>
        /// Get material issued list
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("material/issued/list")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<IEnumerable<NewMaterialIssuedList>> GetMaterialIssuedList (string? fromDate, string? toDate, string? nameOrPhone, Guid courseTypeID, string? shortName) {
            IEnumerable<NewMaterialIssuedList> materialIssuedLists;
            try {
                materialIssuedLists = await _indentService.GetMaterialIssuedList(fromDate!, toDate!, nameOrPhone!, courseTypeID, shortName!);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return materialIssuedLists;
        }

        /// <summary>
        /// Get issued material list
        /// </summary>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("issued/material/list")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<IEnumerable<NewMaterialPendingList>> GetMaterialPendingList () {
            IEnumerable<NewMaterialPendingList> pendingLists;
            try {
                pendingLists = await _indentService.GetMaterialPendingList();
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return pendingLists;
        }

        /// <summary>
        /// Get local stock list
        /// </summary>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("localstock/list")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<List<LocalStockList>> GetLocalStockList () {
            List<LocalStockList> localStocks;
            try {
                localStocks = await _indentService.GetLocalStockList();
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return localStocks;
        }

        /// <summary>
        /// Get branch wise material required list
        /// </summary>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("material/required/list")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<List<NewMaterialRequiredList>> GetMaterialRequiredList () {
            List<NewMaterialRequiredList> materialRequiredLists;
            try {
                materialRequiredLists = await _indentService.GetMaterialRequiredList();
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return materialRequiredLists;
        }

        /// <summary>
        /// Create material purchase invoice
        /// </summary>
        /// <param name="purchase"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpPost("material/purchase/invoice")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public APIResult<Guid> CreateMaterialPurchaseInvoice (MaterialPurchase purchase) {
            Guid result;
            try {
                result = _indentService.CreateMaterialPurchaseInvoice(purchase);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return APIResult.Success(result, "Purchase invoice created successfully !", DawnStatusCode.Created);
        }

        /// <summary>
        /// Get material purchase invoice list by date
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("purchase/invoice/list")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<List<PurchaseInvoiceList>> GetMaterialPurchaseInvoiceList (DateTime fromDate, DateTime toDate) {
            List<PurchaseInvoiceList> invoiceLists;
            try {
                invoiceLists = await _indentService.GetMaterialPurchaseInvoiceList(fromDate, toDate);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return invoiceLists;
        }

        /// <summary>
        /// Print purchase invoice
        /// </summary>
        /// <param name="materialPurchaseID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("print/purchase/invoice")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<PurchaseInvoiceList> PrintSalseInvoice (Guid materialPurchaseID) {
            PurchaseInvoiceList printInvoice;
            try {
                printInvoice = await _indentService.PrintSalseInvoice(materialPurchaseID);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return printInvoice;
        }

        /// <summary>
        /// Get admin current stock list
        /// </summary>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("currentstock/list")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public IEnumerable<GetCurrentStock> GetCurrentStockList () {
            IEnumerable<GetCurrentStock> stocks;
            try {
                stocks = _indentService.GetCurrentStockList();
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return stocks;
        }

        /// <summary>
        /// Get material flow report
        /// </summary>
        /// <param name="flowReport"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        [HttpPost("material/flow/report")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<List<MaterialFolwReport>> GetMaterialFlowReport (FlowReportByFilter flowReport) {
            List<MaterialFolwReport> reports;
            try {
                reports = await _indentService.GetMaterialFlowReport(flowReport);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return reports;
        }

        /// <summary>
        /// Get material indent list by admin
        /// </summary>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("material/list")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<List<MaterialIndentList>> GetMaterialIndentList () {
            List<MaterialIndentList> list;
            try {
                list = await _indentService.GetMaterialIndentList();
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return list;
        }

        /// <summary>
        /// Get material indent list by franchisee
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpPost("material/listbyfranchisee")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<List<MaterialIndentListByFranchisee>> GetMaterialIndentListByFranchisee (IndentFilter filter) {
            List<MaterialIndentListByFranchisee> list;
            try {
                list = await _indentService.GetMaterialIndentListByFranchisee(filter);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return list;
        }

        /// <summary>
        /// Create new exam indent
        /// </summary>
        /// <param name="examIndent"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpPost("exam/create")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<APIResult<Guid>> CreateExamIndent (NewExamIndent examIndent) {
            Guid tenantBranchID;
            try {
                tenantBranchID = await _indentService.CreateExamIndent(examIndent);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return APIResult.Success(tenantBranchID, "Exam indent request send successfully !", DawnStatusCode.Created);
        }
        [HttpGet("exam/receipt")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<APIResult<IndentReceipt>> ExamIndentReceipt(Guid examIndentID)
        {
            try
            {
                return APIResult.Success(await _indentService.ExamIndentReceipt(examIndentID), "Exam Indent Receipt details fetched successfully", DawnStatusCode.Success);
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }
        /// <summary>
        /// Get exam indent request list by admin
        /// </summary>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("exam/list")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<List<GetExamIndent>> GetExamIndentList () {
            List<GetExamIndent> examIndents;
            try {
                examIndents = await _indentService.GetExamIndentList();
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return examIndents;
        }

        [HttpGet("exam/details")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<APIResult<GetExamIndentDetail>> ExamIndentDetailsByAdmin(Guid examIndentID)
        {
            try
            {
                return APIResult.Success(await _indentService.ExamIndentDetailsByAdmin(examIndentID), "Exam IndentTrans details fetched successfully", DawnStatusCode.Success);
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }

        /// <summary>
        /// Reject exam indent request
        /// </summary>
        /// <param name="indent"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpPost("exam/reject")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<APIResult<Guid>> RejectExamIndent (RejectExamIndent indent) {
            Guid result;
            try {
                result = await _indentService.RejectExamIndent(indent);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return APIResult.Success(result, "Exam indent request rejected !", DawnStatusCode.Created);
        }

        /// <summary>
        /// Activate student for exam
        /// </summary>
        /// <param name="indent"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpPost("exam/activate")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public APIResult<string> AcceptExamIndent (ApproveExamIndent indent) {
            string result;
            try {
                result = _indentService.AcceptExamIndent(indent);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return APIResult.Success("", result, DawnStatusCode.Created);
        }

        /// <summary>
        /// Get exam indent request list by franchisee
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("exam/list/byfranchisee")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<List<GetExamIndentByFranchisee>> GetExamIndentListByFranchisee (DateTime fromDate, DateTime toDate) {
            List<GetExamIndentByFranchisee> getExamIndents;
            try {
                getExamIndents = await _indentService.GetExamIndentListByFranchisee(fromDate, toDate);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return getExamIndents;
        }

        /// <summary>
        /// Get exam indent trans id list
        /// </summary>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("examindentids")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public List<GetExamIndentTransID> ExamIndentTransID () {
            try {
                return _indentService.ExamIndentTransID();
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }

        /// <summary>
        /// Update student total mark
        /// </summary>
        /// <param name="totalMarks"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpPost("update/totalmark")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public APIResult<string> UpdateStudentTotalMark (List<StudentTotalMarks> totalMarks) {
            string result;
            try {
                result = _indentService.UpdateStudentTotalMark(totalMarks);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return APIResult.Success("", result, DawnStatusCode.Created);
        }

        /// <summary>
        /// Get certificate indent list
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="status"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("certificate/list")]
        [DawnAuthorize("TenantUser")]
        public async Task<List<CertificateIndentList>> GetCertificateIndentList (string? fromDate, string? toDate, string? status,string? nameOrPhone,Guid courseTypeId,string? shortName) {
            List<CertificateIndentList> result;
            try {
                result = await _indentService.GetCertificateIndentList(fromDate ?? "", toDate ?? "", status ?? "",nameOrPhone??"",courseTypeId, shortName??"");
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return result;
        }

        [HttpGet("certificate/issued/list")]
        [DawnAuthorize("TenantUser")]
        public async Task<List<CertificateIssuedList>> GetCertificateIssuedList (string fromDate, string toDate) {
            List<CertificateIssuedList> result;
            try {
                result = await _indentService.GetCertificateIssuedList(fromDate, toDate);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return result;
        }

        /// <summary>
        /// Get certificate info
        /// </summary>
        /// <param name="examIndentTransID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("certificate/info/{examIndentTransID}")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public CertificateIndentInfo GetStudentCertificateIndentInfo (Guid examIndentTransID) {
            CertificateIndentInfo result;
            try {
                result = _indentService.GetStudentCertificateIndentInfo(examIndentTransID);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return result;
        }

        /// <summary>
        /// Update student edited subject total mark
        /// </summary>
        /// <param name="totalMark"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpPost("update/editedmark")]
        [DawnAuthorize("TenantUser")]
        public APIResult<int> UpdateStudentEditedMark (GetTotalMark totalMark) {
            int result;
            try {
                result = _indentService.UpdateEditedMark(totalMark);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return APIResult.Success(result, "Mark updated successfully !", DawnStatusCode.Created);
        }

        /// <summary>
        /// Get certificate print list
        /// </summary>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("certificate/print/list")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public DataSet GetCertificateList () {
            try {
                return _indentService.GetCertificateList();
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }

        /// <summary>
        /// Get exam indent trans ID by batchmapping ID
        /// </summary>
        /// <param name="batchMappingID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("examindenttransid/bybatchmappingid/{batchMappingID}")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public List<ExamTransID> GetExamIndentTransID (Guid batchMappingID) {
            List<ExamTransID> receives;
            try {
                receives = _indentService.GetExamIndentTransID(batchMappingID);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return receives;
        }

        /// <summary>
        /// Get Branch Total Purchase
        /// </summary>
        /// <param name="tenantBranchID"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("branch/totalpurchase")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public GetTotalPurchase GetBranchTotalPurchase (Guid tenantBranchID, string fromDate, string toDate) {
            GetTotalPurchase totalPurchase;
            try {
                totalPurchase = _indentService.GetBranchTotalPurchase(tenantBranchID, fromDate, toDate);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return totalPurchase;
        }

        /// <summary>
        /// Get student grade
        /// </summary>
        /// <param name="examIndentTransID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("grade/{examIndentTransID}")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public StudentGrade GetStudentGrade (Guid examIndentTransID) {
            StudentGrade student;
            try {
                student = _indentService.GetStudentGrade(examIndentTransID);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return student;
        }

        /// <summary>
        /// Get student semester count
        /// </summary>
        /// <param name="batchMappingID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("student/semester/count/{batchMappingID}")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public int GetStudentSemesterCount (Guid batchMappingID) {
            int semester;
            try {
                semester = _indentService.GetStudentSemesterCount(batchMappingID);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return semester;
        }

        /// <summary>
        /// Get indent summary by admin
        /// </summary>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("summary/byadmin")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public IndentSummaryByAdmin GetIndentSummaryByAdmin () {
            IndentSummaryByAdmin indent;
            try {
                indent = _indentService.GetIndentSummaryByAdmin();
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return indent;
        }

        /// <summary>
        /// Get indent summary by franchisee
        /// </summary>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("summary/byfranchisee")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public IndentSummaryByFranchisee GetIndentSummaryByFranchisee () {
            IndentSummaryByFranchisee indent;
            try {
                indent = _indentService.GetIndentSummaryByFranchisee();
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return indent;
        }

        /// <summary>
        /// Get student exam date
        /// </summary>
        /// <param name="examIndentID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("examdate/{examIndentID}")]
        [DawnAuthorize("TenantUser")]
        public List<GetExamDate> GetExamDate (Guid examIndentID) {
            List<GetExamDate> examDate;
            try {
                examDate = _indentService.GetExamDate(examIndentID);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return examDate;
        }

        [HttpGet("examtrans/list")]
        [DawnAuthorize("TenantUser", "TenantAdmin", "Franchisee")]
        public List<ExamIndentTransInfo> GetExamIndentTransList () {
            List<ExamIndentTransInfo> examDate;
            try {
                examDate = _indentService.GetExamIndentTransList();
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return examDate;
        }

        [HttpGet("purchase-amount/list")]
        [DawnAuthorize("TenantUser", "Franchisee")]
        public List<PurchaseAmount> GetPurchaseList () {
            List<PurchaseAmount> amounts;
            try {
                amounts = _indentService.GetPurchaseAmount();
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return amounts;
        }

        /// <summary>
        /// Material Indent Status Update using branchMaterialIndentID
        /// </summary>
        /// <param name="branchMaterialIndentID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("material/approved")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public APIResult<string> ApprovedMaterialIndent (Guid branchMaterialIndentID) {
            string result;
            try {
                result = _indentService.ApprovedMaterialIndent(branchMaterialIndentID);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return APIResult.Success("", result, DawnStatusCode.Created);
        }

        /// <summary>
        /// Exam Indent Status Update using examIndentID
        /// </summary>
        /// Author:Sakthivel B
        /// CreatedDate:20-06-2024
        /// <returns></returns>
        /// <param name="branchMaterialIndentID"></param>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("exam-indent/approved")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public APIResult<string> ApprovedExamIndent (Guid examIndentID) {
            string result;
            try {
                result = _indentService.ApprovedExamIndent(examIndentID);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return APIResult.Success<string>("", result, DawnStatusCode.Created);
        }

        [HttpGet("status/dropdown")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public string[] GetStatusDropdown () {
            return _indentService.GetStatusDropdown();
        }

        [HttpGet("material/invoice")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<GetMaterialInvoice> GetMaterialInvoice (string invoiceNo) {
            try {
                return await _indentService.GetMaterialInvoice(invoiceNo);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }

        [HttpGet("invoice/download")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<IActionResult> CreateAndDownloadReportDocument (string invoiceNo) {
            try {
                var reportName = "Material_Invoice";
                var parameters = new {
                    invoiceNo = invoiceNo,
                    Authorization = _userIdentityInfo.BearerToken
                };
                var format = "PDF";
                await _indentService.CreateAndDownloadReportDocument(reportName, parameters, format);
                reportName = $"{reportName}.pdf";
                var filePath = Path.Combine(_environment.WebRootPath, "downloads", reportName);
                var contentType = GetContentType(filePath);
                if (!System.IO.File.Exists(filePath)) {
                    return NotFound("File not found.");
                }
                var fileContents = Encoding.UTF8.GetBytes(contentType);
                var fileBytes = System.IO.File.ReadAllBytes(filePath);
                return new FileContentResult(fileBytes, contentType) {
                    FileDownloadName = reportName
                };
            }
            catch (Exception ex) {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet("invoice/download/new")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<IActionResult> CreateAndDownloadReportDocumentNew(string branchMaterialIndentID)
        {
            try
            {
                var reportName = "Material_Indent_Receipt";
                var parameters = new
                {
                    branchMaterialIndentID = branchMaterialIndentID,
                    Authorization = _userIdentityInfo.BearerToken
                };
                var format = "PDF";
                await _indentService.CreateAndDownloadReportDocument(reportName, parameters, format);
                reportName = $"{reportName}.pdf";
                var filePath = Path.Combine(_environment.WebRootPath, "downloads", reportName);
                var contentType = GetContentType(filePath);
                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound("File not found.");
                }
                var fileContents = Encoding.UTF8.GetBytes(contentType);
                var fileBytes = System.IO.File.ReadAllBytes(filePath);
                return new FileContentResult(fileBytes, contentType)
                {
                    FileDownloadName = reportName
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpPost("reprint/certificate-marksheet")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<APIResult<Guid>> CreateCertificateMarksheetIndentReprinting(ReprintIndent reprint)
        {
            try
            {
                var result = await _indentService.CreateCertificateIndentReprintingUpdated(reprint);
                return APIResult.Success(result,"Reprint indent created successfull", DawnStatusCode.Created);
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }
        #region IDCard
        [HttpPost("id-card")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<APIResult<Guid>> CreateIDCardIndent(IDCardIndent idCard)
        {
            try
            {
                var result = await _indentService.CreateIDCardIndent(idCard);
                return APIResult.Success(result, "IdCard indent created successfull", DawnStatusCode.Created);
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }
        [HttpGet("id-card/status/dropdown")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public string[] GetIDCardStatusDropdown()
        {
            return _indentService.GetIDCardStatusDropdown();
        }
        [HttpGet("id-card/list")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public APIResult<List<IDCardIndentList>> GetIDCardIntentListByFranchise(string? fromDate, string? toDate, string? status)
        {
            try
            {
                return APIResult.Success(_indentService.GetIDCardIntentList(fromDate ?? "", toDate ?? "", status?? ""),
                    "ID Card indent details fetched successfully", DawnStatusCode.Success);
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }
        [HttpPost("id-card/details")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<APIResult<IDCardIndentDetails>> GetIDCardIndentDetails(IndentID id)
        {
            try
            {
                return APIResult.Success(await _indentService.GetIDCardIndentDetails(id.IDCardIndentID),
                    "ID Card indent details fetched successfully", DawnStatusCode.Success);
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }
        [HttpGet("id-card/list-by/admin")]
        [DawnAuthorize("TenantAdmin")]
        public async Task<APIResult<List<IDCardListByAdmin>>> GetIDCardIndentListByAdmin()
        {
            try
            {
                return APIResult.Success(await _indentService.GetIDCardIndentListByAdmin(),
                    "ID Card indent details fetched successfully", DawnStatusCode.Success);
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }
        [HttpGet("id-card/view-by/admin")]
        [DawnAuthorize("TenantAdmin")]
        public async Task<APIResult<IDcardIndentNumber>> ViewIDCardPaymentDetailsByAdmin(Guid iDCardIndentID)
        {
            try
            {
                return APIResult.Success(await _indentService.ViewIDCardPaymentDetailsByAdmin(iDCardIndentID),
                    "", DawnStatusCode.Success);
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }
        [HttpPost("id-card/approve")]
        [DawnAuthorize("TenantAdmin")]
        public APIResult<Guid> ApproveIDCardIndent(IndentID id)
        {
            try
            {
                return APIResult.Success(_indentService.ApproveIDCardIndent(id.IDCardIndentID),
                    "IDCard Indent Approved Successfully", DawnStatusCode.Success);
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }
        [HttpPost("id-card/reject")]
        [DawnAuthorize("TenantAdmin")]
        public APIResult<Guid> RejectIDCardIndent(Guid iDCardIndentID,string reason)
        {
            try
            {
                return APIResult.Success(_indentService.RejectIDCardIndent(iDCardIndentID,reason),
                    "IDCard Indent Rejected Successfully", DawnStatusCode.Success);
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }
        [HttpPost("id-card/print/status")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public APIResult<string> IDCardIndentPrinted(IDCardPrinted print)
        {
            try
            {
                return APIResult.Success(_indentService.IDCardIndentPrinted(print),
                    "Print Status Updated Successfully", DawnStatusCode.Success);
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }
        [HttpGet("id-card/despatch/list")]
        [DawnAuthorize("TenantAdmin")]
        public async Task<APIResult<List<DespatchList>>> GetDespatchList()
        {
            try
            {
                return APIResult.Success(await _indentService.GetDespatchList(),
                    "To Despatch Indent details fetched successfully", DawnStatusCode.Success);
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }
        [HttpPost("id-card/despatch")]
        [DawnAuthorize("TenantAdmin")]
        public APIResult<string> DespatchIDCardIndent(NewDespatchIDCard newDespatch)
        {
            try
            {
                return APIResult.Success(_indentService.DespatchIDCardIndent(newDespatch), "Indent Despatched Successfully", DawnStatusCode.Success);
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error); 
            }
        }
        [HttpPost("id-card/receive")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public APIResult<string> ReceiveIDCardIndent(List<IndentTransID> receive)
        {
            try
            {
                return APIResult.Success(_indentService.ReceiveIDCardIndent(receive), "Indent Received Successfully", DawnStatusCode.Success);
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }
        [HttpGet("id-card/print/list")]
        [DawnAuthorize("TenantAdmin")]
        public async Task<APIResult<List<IDCardListByAdmin>>> GetIDCardIndentPrintList()
        {
            try
            {
                return APIResult.Success(await _indentService.GetIDCardIndentPrintList(),
                    "Indent print details fetched successfully", DawnStatusCode.Success);
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }
        #endregion
        private string GetContentType (string path) {
            var types = new Dictionary<string, string> {
                { ".txt", "text/plain" },
                { ".pdf", "application/pdf" },
                { ".doc", "application/vnd.ms-word" },
                { ".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
                { ".xls", "application/vnd.ms-excel" },
                { ".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
                { ".png", "image/png" },
                { ".jpg", "image/jpeg" },
                { ".jpeg", "image/jpeg" },
                { ".gif", "image/gif" },
                { ".csv", "text/csv" },
                { ".mp4", "video/mp4" }
            };
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types.ContainsKey(ext) ? types[ext] : "application/octet-stream";
        }
        #endregion
    }
}
