using Dawn.Database;
using Dawn.Utilities.Extensions;
using Dawn.Utilities.i18N;
using EntitiesDTO.Material;
using EntitiesDTO.Student;
using Microsoft.Data.SqlClient;
using System.Data;

namespace iCloud.Inventory.Repository {
    public class IndentRepository : DawnDbClient {
        #region Fields
        private readonly SqlI18N _sqlI18N;
        #endregion
        #region Constructor
        public IndentRepository (SqlI18N SqlI18N, string ConnectionString) : base(ConnectionString) {
            _sqlI18N = SqlI18N;
        }
        #endregion
        #region Methods
        /// <summary>
        /// Create new delivery mode list
        /// </summary>
        /// <param name="deliveryMode"></param>
        /// <param name="actionBy"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public Guid CreateDeliveryMode (CreateDeliveryMode deliveryMode, Guid actionBy) {
            Guid result = Guid.Empty;
            string query = "uspCreateDeliveryMode";
            try {
                result = (Guid)ExecuteScalar(query, new {
                    DeliveryModeName = deliveryMode.DeliveryModeName,
                    CreatedBy = actionBy
                }, commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Get delivery mode list
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<DeliveryModeList> GetDeliveryModeList () {
            List<DeliveryModeList> deliveryModes = new List<DeliveryModeList>();
            string query = "SELECT DeliveryModeID,DeliveryModeName FROM dbo.DeliveryMode";
            try {
                deliveryModes = GetList<DeliveryModeList>(query, _sqlI18N);
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return deliveryModes;
        }

        /// <summary>
        /// Create new material indent
        /// </summary>
        /// <param name="materialIndent"></param>
        /// <param name="material"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public GetBranchIndentInfo CreateMaterialIndent (CreateMaterialIndent materialIndent, DataTable MaterialItem) {
            GetBranchIndentInfo indentInfo = new GetBranchIndentInfo();
            try {
                indentInfo = Get<GetBranchIndentInfo>("uspCreateMaterialIndent", _sqlI18N, new {
                    TenantBranchID = materialIndent.TenantBranchID,
                    MaterialItem = MaterialItem,
                    CreatedBy = materialIndent.CreatedBy,
                    TotalAmount = materialIndent.TotalAmount,
                    CourierAmount = materialIndent.CourierAmount
                }, commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return indentInfo;
        }

        public int RollbackMaterialIndent (Guid branchMaterialIndentID) {
            int count = 0;
            try {
                count = ExecuteScalar("uspRollbackMaterialIndent", new { BranchMaterialIndentID = branchMaterialIndentID }, commandType: System.Data.CommandType.StoredProcedure).ToInt();
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return count;
        }

        /// <summary>
        /// Accept material indent request
        /// </summary>
        /// <param name="MaterialIndent"></param>
        /// <param name="actionBy"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int ApproveMaterialIndent (DataTable MaterialIndent, Guid actionBy) {
            int result = 0;
            try {
                result = ExecuteScalar("uspApproveMaterialIndent", new {
                    UpdatedBy = actionBy,
                    MaterialIndent = MaterialIndent
                }, commandType: System.Data.CommandType.StoredProcedure).ToInt();
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Split material indent item quantity
        /// </summary>
        /// <param name="splitMaterial"></param>
        /// <param name="actionBy"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public Guid SplitMaterialIndent (SplitMaterialIndent splitMaterial, Guid actionBy) {
            Guid result = Guid.Empty;
            try {
                result = (Guid)ExecuteScalar("uspSplitMaterialIndent", new {
                    MaterialIndentTransID = splitMaterial.MaterialIndentTransID,
                    IndentQuantity = splitMaterial.IndentQuantity,
                    CreatedBy = actionBy
                }, commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return result;
        }

        public int GetIndentQuantity (Guid materialIndentTransID) {
            int result = 0;
            result = ExecuteScalar("SELECT IndentQuantity FROM dbo.BranchMaterialIndentTrans WHERE MaterialIndentTransID = @MaterialIndentTransID", new {
                MaterialIndentTransID = materialIndentTransID
            }).ToInt();
            return result;
        }

        /// <summary>
        /// Reject material indent request
        /// </summary>
        /// <param name="rejectIndent"></param>
        /// <param name="actionBy"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public Guid RejectMaterialIndent (RejectMaterialIndent rejectIndent, Guid actionBy) {
            Guid result = Guid.Empty;
            try {
                result = (Guid)ExecuteScalar("uspRejectMaterialIndent", new {
                    BranchMaterialIndentID = rejectIndent.BranchMaterialIndentID,
                    Reason = rejectIndent.Reason,
                    UpdatedBy = actionBy
                }, commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return result;
        }

        public bool CheckBranchMaterialIndentID (Guid branchMaterialIndentID) {
            bool result = false;
            result = ExecuteScalar("SELECT COUNT(*) FROM dbo.BranchMaterialIndent WHERE BranchMaterialIndentID = @BranchMaterialIndentID", new {
                BranchMaterialIndentID = branchMaterialIndentID
            }).ToBoolean();
            return result;
        }

        /// <summary>
        /// Get material despatch list
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public IEnumerable<GetMaterialDespatchList> GetMaterialDespatchList () {
            IEnumerable<GetMaterialDespatchList> despatchLists = new List<GetMaterialDespatchList>();
            string query = "SELECT TenantBranchID,BranchMaterialIndentID,MaterialIndentTransID,MaterialName,Quantity,Price,Total,ConfirmedDate FROM dbo.tfnGetMaterialDespatchList() ORDER BY ConfirmedDate DESC";
            try {
                despatchLists = GetList<GetMaterialDespatchList>(query, _sqlI18N);
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return despatchLists;
        }

        /// <summary>
        /// Despatch material item
        /// </summary>
        /// <param name="newDespatch"></param>
        /// <param name="actionBy"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int DespatchMaterialIndent (DataTable newDespatch, Guid actionBy) {
            int result = 0;
            try {
                result = ExecuteScalar("uspDespatchMaterialItem", new {
                    UpdatedBy = actionBy,
                    DespatchMaterial = newDespatch
                }, commandType: System.Data.CommandType.StoredProcedure).ToInt();
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return result;
        }

        public List<GetMaterialIndentIDs> GetMaterialIndentIDs (DataTable materialDespatch) {
            return GetList<GetMaterialIndentIDs>("uspGetMaterialIndentIDs", _sqlI18N, new {
                DespatchMaterial = materialDespatch
            }, commandType: System.Data.CommandType.StoredProcedure);
        }

        /// <summary>
        /// Get material receivable list
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<GetReceivableList> GetReceivableList (Guid tenentBranchID) {
            List<GetReceivableList> receivableLists = new List<GetReceivableList>();
            string query = "SELECT MaterialIndentTransID,IndentDate,IndentNumber,MaterialType,MaterialName,Total,DespatchDate,DeliveryMode,TrackingID,BranchMaterialIndentID FROM dbo.tfnGetMaterialReceivableList(@TenantBranchID) ORDER BY IndentDate DESC";
            try {
                receivableLists = GetList<GetReceivableList>(query, _sqlI18N, new { TenantBranchID = tenentBranchID });
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return receivableLists;
        }

        /// <summary>
        /// Receive material indent item
        /// </summary>
        /// <param name="MaterialIndent"></param>
        /// <param name="actionBy"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int ReceiveMaterialIndentItem (DataTable MaterialIndent, Guid actionBy) {
            int result = 0;
            try {
                result = ExecuteScalar("uspReceiveMaterialItem", new {
                    UpdatedBy = actionBy,
                    MaterialIndent = MaterialIndent
                }, commandType: System.Data.CommandType.StoredProcedure).ToInt();
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Get local stock list
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<LocalStockList> GetLocalStockList (Guid tenantBranchID) {
            List<LocalStockList> stockLists = new List<LocalStockList>();
            string query = "SELECT MaterialID,CurrentStock,MaterialName,MaterialType,Price FROM dbo.tfnGetLocalStockList(@TenantBranchID) ORDER BY MaterialName ASC";
            try {
                stockLists = GetList<LocalStockList>(query, _sqlI18N, new { TenantBranchID = tenantBranchID });
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return stockLists;
        }

        /// <summary>
        /// Create material purchase invoice
        /// </summary>
        /// <param name="purchase"></param>
        /// <param name="MaterialInwards"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public Guid CreatePurchaseInvoice (MaterialPurchase purchase, DataTable MaterialInwards, Guid actionBy) {
            Guid result = Guid.Empty;
            try {
                result = (Guid)ExecuteScalar("uspCreatePurchaseInvoice", new {
                    VendorID = purchase.VendorID,
                    InvoiceNo = purchase.InvoiceNo,
                    InvoiceDate = purchase.InvoiceDate,
                    MaterialInward = MaterialInwards,
                    RoundOff = purchase.RoundOff,
                    Notes = purchase.Notes,
                    CreatedBy = actionBy,
                    Discount = purchase.Discount,
                    GST = purchase.GST,
                    Amount = purchase.Amount
                }, commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Get material purchase invoice list by date
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public DataSet GetMaterialPurchaseInvoiceList (DateTime fromDate, DateTime toDate) {
            try {
                return GetDataSet("uspGetPurchaseInvoiceList", new {
                    FromDate = fromDate,
                    ToDate = toDate
                }, commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
        }

        public DataSet PrintSalseInvoice (Guid materialPurchaseID) {
            try {
                return GetDataSet("uspPrintSalseInvoice", new {
                    MaterialPurchaseID = materialPurchaseID
                }, commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Get admin current stock list
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public IEnumerable<GetCurrentStock> GetCurrentStockList () {
            IEnumerable<GetCurrentStock> stocks = new List<GetCurrentStock>();
            string query = "SELECT MaterialID,MaterialType,MaterialName,MinimumStock,Price,CurrentStock FROM dbo.tfnGetCurrentStockList() ORDER BY MaterialName ASC";
            try {
                stocks = GetList<GetCurrentStock>(query);
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return stocks;
        }

        public MaterialInward GetIndentTrans (Guid materialIndentTransID) {
            MaterialInward material = new MaterialInward();
            string query = "SELECT MaterialID,IndentQuantity AS Quantity FROM dbo.BranchMaterialIndentTrans WHERE MaterialIndentTransID = @MaterialIndentTransID AND IsDeleted = 0 AND Status = 'ORDERED'";
            try {
                material = Get<MaterialInward>(query, _sqlI18N, new { MaterialIndentTransID = materialIndentTransID });
                return material;
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
        }

        public DataSet GetMaterialFlowReport (string fromDate, string toDate, DataTable Materials) {
            try {
                return GetDataSet("uspGetMaterialFolwReport", new {
                    FromDate = fromDate,
                    ToDate = toDate,
                    Materials = Materials
                }, commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Get material indent list by admin
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public DataSet GetMaterialIndentList () {
            try {
                return GetDataSet("uspGetMaterialIndentByAdmin", commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Get material indent list by franchisee
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public DataSet GetMaterialIndentListByFranchisee (IndentFilter filter, Guid tenantBranchID) {
            try {
                return GetDataSet("uspGetMaterialIndentByFranchisee",new {
                    FromDate = filter.FromDate,
                    ToDate = filter.ToDate,
                    TenantBranchID = tenantBranchID,
                    Status = filter.Status
                }, commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Create new exam indent
        /// </summary>
        /// <param name="exam"></param>
        /// <param name="BatchMapping"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public ExamIndentResult CreateExamIndent (CreateExamIndent exam, DataTable BatchMapping) {
            ExamIndentResult result = new ExamIndentResult();
            try {
                result = Get<ExamIndentResult>("uspCreateExamIndent", _sqlI18N, new {
                    TenantBranchID = exam.TenantBranchID,
                    CreatedBy = exam.CreatedBy,
                    BatchMapping = BatchMapping,
                    TotalAmount = exam.TotalAmount,
                    CourierAmount = exam.CourierAmount
                }, commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return result;
        }

        public int RollbackExamIndent (Guid examIndentID) {
            int count = 0;
            try {
                count = ExecuteScalar("uspRollbackExamIndent", new { ExamIndentID = examIndentID }, commandType: System.Data.CommandType.StoredProcedure).ToInt();
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return count;
        }

        public bool CheckStudentBatchMappingID (Guid batchMappingID, int semester) {
            bool result = false;
            string query = "SELECT dbo.sfnCheckStudentBatchMappingID(@BatchMappingID,@Semester)";
            try {
                result = ExecuteScalar(query, new { BatchMappingID = batchMappingID, Semester = semester }).ToBoolean();
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return result;
        }

        public List<ExamIndentTransIDs> GetExamIndentTransIDs (Guid examIndentID) {
            List<ExamIndentTransIDs> indentTransIDs = new List<ExamIndentTransIDs>();
            string query = $"SELECT ExamIndentTransID,StudentBatchMapping AS BatchMappingID,Semester FROM dbo.ExamIndentTrans WHERE ExamIndentID=@ExamIndentID AND IsDeleted=0;";
            try {
                indentTransIDs = GetList<ExamIndentTransIDs>(query, _sqlI18N, new { ExamIndentID = examIndentID });
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return indentTransIDs;
        }

        /// <summary>
        /// Get exam indent list
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public DataSet GetExamIndentList () {
            try {
                return GetDataSet("uspGetExamIndent", commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
        }
        public List<ExamIndentTransDetail> GetExamIndentTransDetail(Guid examIndentID)
        {
            try{
                return GetList<ExamIndentTransDetail>("uspGetExamIndentTransDetail", _sqlI18N, new { ExamIndentID = examIndentID }, commandType: CommandType.StoredProcedure);
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Guid RejectExamIndent (RejectExamIndent rejectIndent, Guid actionBy) {
            Guid result = Guid.Empty;
            try {
                result = (Guid)ExecuteScalar("uspRejectExamIndent", new {
                    ExamIndentID = rejectIndent.ExamIndentID,
                    Reason = rejectIndent.Reason,
                    UpdatedBy = actionBy
                }, commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return result;
        }

        public bool CheckExamIndentID (Guid examIndentID) {
            bool result = false;
            result = ExecuteScalar("SELECT COUNT(*) FROM dbo.ExamIndent WHERE ExamIndentID = @ExamIndentID AND IsRejected = 0", new {
                ExamIndentID = examIndentID
            }).ToBoolean();
            return result;
        }

        /// <summary>
        /// Activate student for exam
        /// </summary>
        /// <param name="ExamIndent"></param>
        /// <param name="actionBy"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int ApproveExamIndent (DataTable ExamIndent, Guid actionBy) {
            int result = 0;
            try {
                result = ExecuteScalar("uspApproveExamIndent", new {
                    UpdatedBy = actionBy,
                    ExamIndent = ExamIndent
                }, commandType: System.Data.CommandType.StoredProcedure).ToInt();
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return result;
        }

        public DataSet GetExamIndentListByFranchisee (DateTime fromDate, DateTime toDate, Guid tenantBranchID) {
            try {
                return GetDataSet("uspGetExamIndentByFranchisee", new {
                    FromDate = fromDate,
                    ToDate = toDate,
                    tenantBranchID = tenantBranchID
                }, commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (Exception ex) {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Get exam indent tran id
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<GetExamIndentTransID> ExamIndentTransID (Guid tenantBranchID) {
            List<GetExamIndentTransID> transIDs = new List<GetExamIndentTransID>();
            string query = "SELECT ExamIndentTransID,StudentBatchMapping,Semester FROM dbo.tfnGetBranchWiseExamTransID(@TenantBranchID)";
            try {
                transIDs = GetList<GetExamIndentTransID>(query, _sqlI18N, new { TenantBranchID = tenantBranchID });
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return transIDs;
        }

        public int UpdateStudentTotalMark (DataTable updateStudentTotalMark, Guid actionBy) {
            try {
                return ExecuteScalar("uspUpdateStudentTotalMark", new { UpdateStudentTotalMark = updateStudentTotalMark, UpdatedBy = actionBy }, commandType: System.Data.CommandType.StoredProcedure).ToInt();
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
        }

        public CertificateIndentInfo GetStudentCertificateIndentInfo (Guid examIndentTransID) {
            string query = "SELECT ExamIndentTransID,Semester,StudentBatchMapping FROM dbo.tfnGetCertificateIndentInfo(@ExamIndentTransID)";
            try {
                return Get<CertificateIndentInfo>(query, _sqlI18N, new { ExamIndentTransID = examIndentTransID });
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
        }

        public bool CheckExamIndentTransID (Guid examIndentTransID) {
            bool result = false;
            try {
                result = ExecuteScalar("SELECT dbo.sfnCheckExamIndentID(@ExamIndentTransID)", new { ExamIndentTransID = examIndentTransID }).ToBoolean();
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Update student edited subject total mark
        /// </summary>
        /// <param name="totalMark"></param>
        /// <param name="actionBy"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int UpdateEditedMark (GetTotalMark totalMark, Guid actionBy) {
            int result = 0;
            try {
                result = ExecuteScalar("uspUpdateStudentEditedMark", new {
                    ExamIndentTransID = totalMark.ExamIndentTransID,
                    TotalMarks = totalMark.TotalMarks,
                    UpdatedBy = actionBy
                }, commandType: System.Data.CommandType.StoredProcedure).ToInt();
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Get certificate print list
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public DataSet GetCertificateList () {
            try {
                return GetDataSet("uspGetCertificateList", commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Get exam indent trans ID
        /// </summary>
        /// <param name="batchMappingID"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<ExamTransID> GetExamIndentTransID (Guid batchMappingID) {
            List<ExamTransID> examIndentTransID = new List<ExamTransID>();
            string query = "SELECT ExamIndentTransID,ExamDate,StudentBatchMapping,Semester FROM dbo.tfnGetExamIndentTransID(@BatchMappingID)";
            try {
                examIndentTransID = GetList<ExamTransID>(query, _sqlI18N, new { BatchMappingID = batchMappingID });
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return examIndentTransID;
        }

        public decimal GetBranchExamPurchase (Guid tenantBranchID, string fromDate, string toDate) {
            decimal result = 0;
            string query = "SELECT dbo.sfnExamTotalPurchase(@TenantBranchID,@FromDate,@ToDate)";
            try {
                result = ExecuteScalar(query, new { TenantBranchID = tenantBranchID, FromDate = fromDate, ToDate = toDate }).ToDecimal();
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return result;
        }

        public decimal GetBranchMaterialPurchase (Guid tenantBranchID, string fromDate, string toDate) {
            decimal result = 0;
            string query = "SELECT dbo.sfnMaterialTotalPurchase(@TenantBranchID,@FromDate,@ToDate)";
            try {
                result = ExecuteScalar(query, new { TenantBranchID = tenantBranchID, FromDate = fromDate, ToDate = toDate }).ToDecimal();
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Get student grade
        /// </summary>
        /// <param name="examIndentTransID"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public StudentGrade GetStudentGrade (Guid examIndentTransID) {
            StudentGrade student = new StudentGrade();
            string query = "SELECT BatchMappingID,GradeName,Semester,DateOfIssueI18 FROM dbo.tfnGetStudentGrad(@ExamIndentTransID)";
            try {
                student = Get<StudentGrade>(query, _sqlI18N, new { ExamIndentTransID = examIndentTransID });
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return student;
        }

        /// <summary>
        /// Get student semester count
        /// </summary>
        /// <param name="batchMappingID"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int GetStudentSemesterCount (Guid batchMappingID) {
            int semester = 0;
            string query = "SELECT dbo.sfnGetStudentSemesterCount(@BatchMappingID)";
            try {
                semester = ExecuteScalar(query, new {
                    BatchMappingID = batchMappingID,
                }).ToInt();
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return semester;
        }

        public IndentSummaryByAdmin GetIndentSummaryByAdmin () {
            IndentSummaryByAdmin indent = new IndentSummaryByAdmin();
            string query = "SELECT IndentMaterial,DespatchMaterial,IndentExam FROM dbo.tfnGetIndentSummaryByAdmin()";
            try {
                indent = Get<IndentSummaryByAdmin>(query, _sqlI18N);
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return indent;
        }

        public IndentSummaryByFranchisee GetIndentSummaryByFranchisee (Guid tenantBranchID) {
            IndentSummaryByFranchisee indent;
            string query = "SELECT MaterialPending,ExamPending FROM dbo.tfnGetIndentSummaryByFranchisee(@TenantBranchID)";
            try {
                indent = Get<IndentSummaryByFranchisee>(query, _sqlI18N, new { TenantBranchID = tenantBranchID });
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return indent;
        }

        public List<GetExamDate> GetExamDate (Guid examIndentID) {
            List<GetExamDate> examDate;
            string query = "SELECT StudentBatchMapping AS BatchMappingID,ExamDate FROM dbo.tfnGetExamDate(@ExamIndentID)";
            try {
                examDate = GetList<GetExamDate>(query, _sqlI18N, new { ExamIndentID = examIndentID });
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return examDate;
        }

        public List<ExamIndentTransInfo> GetExamIndentTransList () {
            List<ExamIndentTransInfo> list;
            string query = "SELECT ei.ExamIndentTransID,ei.StudentBatchMapping,ei.Semester,ei.ExamDate as ExamDateI18,ei.ObtainedMark AS TotalMark,ei.GradeName AS Grade FROM dbo.ExamIndentTrans ei";
            try {
                list = GetList<ExamIndentTransInfo>(query, _sqlI18N);
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return list;
        }

        public List<PurchaseAmount> GetPurchaseAmount () {
            List<PurchaseAmount> list;
            string query = "SELECT TenantBranchID,Total FROM dbo.tfnGetIndentAmount(@Year) ORDER BY Total DESC";
            try {
                list = GetList<PurchaseAmount>(query, _sqlI18N, new { Year = DateTime.Now.Year });
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return list;
        }



        public DataSet GetCertificateIndent (string fromDate, string toDate, string status, Guid tenantBranchID) {
            try {
                return GetDataSet("uspGetCertificateIndent", new {
                    FromDate = fromDate,
                    ToDate = toDate,
                    Status = status,
                    tenantBranchID = tenantBranchID
                }, commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
        }

        public int ApprovedIndent (Guid branchMaterialIndentID, Guid actionBy) {
            int result = 0;
            try {
                result = ExecuteScalar("uspApprovedMaterialIndent", new { BranchMaterialIndentID = branchMaterialIndentID, UpdatedBy = actionBy }, commandType: System.Data.CommandType.StoredProcedure).ToInt();
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return result;
        }

        public int ApprovedExamIndent (Guid examIndentID, Guid actionBy) {
            int result = 0;
            try {
                result = ExecuteScalar("uspApprovedExamIndent", new { ExamIndentID = examIndentID, UpdatedBy = actionBy }, commandType: System.Data.CommandType.StoredProcedure).ToInt();
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return result;
        }

        public Guid CheckMaterialDesPatched (Guid materialIndentID) {
            Guid indentID;
            indentID = (Guid)ExecuteScalar("uspCheckMaterialDespatch", new { MaterialIndentID = materialIndentID }, commandType: System.Data.CommandType.StoredProcedure);
            return indentID;
        }

        public Guid GetTenantBranchID(Guid indentID) {
            Guid tenantBranchID = (Guid)ExecuteScalar("SELECT TenantBranchID FROM dbo.BranchMaterialIndent WHERE BranchMaterialIndentID = @MaterialIndentID AND IsDeleted = 0", new { MaterialIndentID = indentID });
            return tenantBranchID;
        }

        public List<GetMaterialIndentInfo> GetMaterialIndentInfo(Guid indentID) {
            string query = "SELECT ROW_NUMBER() OVER (ORDER BY bm.SlNo) AS SlNo,SUM(bm.UnitPrice) AS Price,mt.MaterialType,m.MaterialName,SUM(bm.IndentQuantity) AS Quantity,SUM((bm.UnitPrice*bm.IndentQuantity)) AS TaxableValue,SUM((((bm.UnitPrice*(bm.CGST + bm.SGST))/100) * bm.IndentQuantity)) AS GSTPrice,SUM(bm.ItemTotal) AS Total FROM dbo.BranchMaterialIndentTrans bm INNER JOIN dbo.Material m ON m.MaterialID = bm.MaterialID INNER JOIN dbo.MaterialType mt ON mt.MaterialTypeID = m.MaterialTypeID WHERE bm.BranchMaterialIndentID = @BranchMaterialIndentID AND bm.IsDeleted = 0 GROUP BY mt.MaterialType,m.MaterialName,bm.SlNo";
            return GetList<GetMaterialIndentInfo>(query, _sqlI18N, new { BranchMaterialIndentID = indentID });
        }

        public InvoiceCreate GetGST(Guid indentID) {
            string query = "SELECT SUM(((UnitPrice*CGST)/100)*IndentQuantity) AS CGST,SUM(((UnitPrice*SGST)/100)*IndentQuantity) AS SGST FROM dbo.BranchMaterialIndentTrans WHERE BranchMaterialIndentID = @BranchMaterialIndentID";
            return Get<InvoiceCreate>(query, _sqlI18N, new { BranchMaterialIndentID = indentID });
        }

        public decimal GetIndentAmount(Guid indentID) {
            decimal amount = 0;
            amount = ExecuteScalar($"SELECT SUM(ItemTotal) FROM dbo.BranchMaterialIndentTrans WHERE BranchMaterialIndentID = @BranchMaterialIndentID", new { BranchMaterialIndentID = indentID }).ToDecimal();
            return amount;
        }
        public List<GetMaterialPurchaseDetails> GetMaterialIndents(Guid branchMaterialIndentID)
        {
            try
            {
                string query = "uspGetMaterialIndents";
                return GetList<GetMaterialPurchaseDetails>(query,_sqlI18N,new
                {
                     BranchMaterialIndentID = branchMaterialIndentID
                },commandType : CommandType.StoredProcedure);
            }
            catch (SqlException)
            {
                throw;
            }
        }
        public List<GetExamIndentDetails> GetExamIndentDetails(Guid examIndentID)
        {
            try
            {
                string query = @"SELECT a.ExamIndentID,a.TenantBranchID,b.ExamIndentTransID,b.StudentBatchMapping,a.TotalAmount + ISNULL(a.CourierAmount,0) as TotalAmount,ISNULL(a.CourierAmount,0) AS CourierAmount,b.ExamFees
                                 FROM dbo.ExamIndent a INNER JOIN dbo.ExamIndentTrans b ON b.ExamIndentID = a.ExamIndentID
                                 WHERE a.ExamIndentID = @ExamIndentID";
                return GetList<GetExamIndentDetails>(query, _sqlI18N, new
                {
                    ExamIndentID = examIndentID
                });
            }
            catch (SqlException)
            {
                throw;
            }
        }
        public IDCardIndented CreateIDCardIndent(DataTable batchMapIDs, Guid actionBy, Guid tenantBranchID, decimal courierAmount, decimal totalAmount,int idCardCharge)
        {
            try
            {
                return Get<IDCardIndented>("uspCreateIDCardIndent", _sqlI18N, new
                {
                    TenantBranchID = tenantBranchID,
                    CreatedBy = actionBy,
                    BatchMappingID = batchMapIDs,
                    TotalAmount = totalAmount,
                    CourierAmount = courierAmount,
                    IdCardCharge = idCardCharge
                }, commandType: CommandType.StoredProcedure);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public int RollbackIDCardIndent(Guid idCardIndentID)
        {
            int count = 0;
            try
            {
                count = ExecuteScalar("uspRollbackIDCardIndent", new { IDCardIndentID = idCardIndentID }, commandType: System.Data.CommandType.StoredProcedure).ToInt();
            }
            catch (SqlException)
            {
                throw;
            }
            return count;
        }
        public List<IDCardIndentList> GetIDCardIntentList(string fromDate,string toDate,string status,Guid tenantBranchID)
        {
            try
            {
                return GetList<IDCardIndentList>("uspIDCardIndentList", _sqlI18N, new
                {
                    FromDate = fromDate,
                    ToDate = toDate,
                    Status = status,
                    TenantBranchID = tenantBranchID
                }, commandType: CommandType.StoredProcedure);
            }
            catch (SqlException)
            {
                throw;
            }
        }
        public DataSet GetIDCardIndentDetails(Guid tenantBranchID,Guid idCardIndentID)
        {
            try
            {
                return GetDataSet("uspGetIDCardIndentDetails", new
                {
                    TenantBranchID = tenantBranchID,
                    IDCardIndentID = idCardIndentID
                }, commandType: CommandType.StoredProcedure);
            }
            catch (SqlException)
            {
                throw;
            }
        }
        public List<IDCardListByAdmin> GetIDCardIndentListByAdmin()
        {
            try
            {
                return GetList<IDCardListByAdmin>("uspIDCardIndentListByAdmin", _sqlI18N, commandType: CommandType.StoredProcedure);
            }
            catch (SqlException)
            {
                throw;
            }
        }
        public IDcardIndentNumber GetIDcardIndentNumber(Guid iDCardIndentID)
        {
            try
            {
                string query = @"SELECT a.IDCardIndentID,a.TenantBranchID, a.IndentNumber,a.CreatedDate AS IndentDateI18, a.IndentAmount,a.CourierAmount FROM dbo.IDCardIndent a
                            INNER JOIN dbo.IDCardIndentTrans b ON b.IDCardIndentID = a.IDCardIndentID
                            WHERE a.IDCardIndentID = @IDCardIndentID AND a.IsDeleted = 0";
                return Get<IDcardIndentNumber>(query, _sqlI18N, new
                {
                    IDCardIndentID = iDCardIndentID
                });
            }
            catch (SqlException)
            {
                throw;
            }
        }
         public Guid ApproveIDCardIndent(Guid iDCardIndentID,Guid actionBy)
        {
            try
            {
                return (Guid) ExecuteScalar("uspApproveIDCardIndent", new
                {
                    IDCardIndentID = iDCardIndentID,
                    ActionBy = actionBy
                },commandType: CommandType.StoredProcedure);
            }
            catch (SqlException)
            {
                throw;
            }
        }
        public Guid RejectIDCardIndent(Guid iDCardIndentID,string reason, Guid actionBy)
        {
            try
            {
                return (Guid)ExecuteScalar("uspRejectIDCardIndent", new
                {
                    IDCardIndentID = iDCardIndentID,
                    ActionBy = actionBy,
                    Reason = reason
                }, commandType: CommandType.StoredProcedure);
            }
            catch (SqlException)
            {
                throw;
            }
        }
        public List<IDCardIndentDetailsListForAdmin> IndentDetailsAdmin(Guid iDCardIndentID)
        {
            try
            {
                string query = "uspIDCardViewByAdmin";
                return GetList<IDCardIndentDetailsListForAdmin>(query, _sqlI18N, new
                {
                    IDCardIndentID = iDCardIndentID
                },commandType:CommandType.StoredProcedure);
            }
            catch (SqlException)
            {
                throw;
            }
        }   
        public int IDCardIndentPrinted(DataTable transIDS,Guid iDCardIndentID,Guid actionBy)
        {
            try
            {
                string query = "uspUpdateIDCardPrintStatus";
                return (int)ExecuteScalar(query, new
                {
                    IDCardIndentID = iDCardIndentID,
                    IDCardIndentTransID = transIDS,
                    ActionBy = actionBy
                },commandType:CommandType.StoredProcedure);
            }
            catch (SqlException)
            {
                throw;
            }
        }
        public List<DespatchList> GetDespatchList()
        {
            try
            {
                string query = "uspGetIDCardDespatchedList";
                return GetList<DespatchList>(query, _sqlI18N, commandType: CommandType.StoredProcedure);
            }
            catch (SqlException)
            {
                throw;
            }
        }
        public int DespatchIDCardIndent(DataTable despatch,Guid actionBy)
        {
            try
            {
                return (int)ExecuteScalar("uspDespatchIDCardIndent", new
                {
                    DespatchIndent = despatch,
                    ActionBy = actionBy
                }, commandType: CommandType.StoredProcedure);
            }
            catch (SqlException)
            {
                throw;
            }
        }
        public int ReceiveIDCardIndent(DataTable receive, Guid actionBy)
        {
            try
            {
                return (int)ExecuteScalar("uspReceiveIDCardIndent", new
                {
                    ActionBy = actionBy,
                    IDCardIndentTransID = receive
                }, commandType: CommandType.StoredProcedure);
            }
            catch (SqlException)
            {
                throw;
            }
        }
        public List<IDCardListByAdmin> GetIDCardIndentPrintList()
        {
            try
            {
                return GetList<IDCardListByAdmin>("uspIDCardIndentPrintList", _sqlI18N, commandType: CommandType.StoredProcedure);
            }
            catch (SqlException)
            {
                throw;
            }
        }
        #endregion
    }
}
