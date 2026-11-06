using Dawn.Database;
using Dawn.Utilities.Extensions;
using Dawn.Utilities.i18N;
using EntitiesDTO.Admin;
using EntitiesDTO.Course;
using EntitiesDTO.Material;
using EntitiesDTO.Payment;
using EntitiesDTO.Student;
using Microsoft.Data.SqlClient;
using System.Data;

namespace CommonServices {
    public class CommonRepository : DawnDbClient {
        #region Fields
        private readonly SqlI18N _sqlI18N;
        #endregion
        #region Constructor
        public CommonRepository (SqlI18N SqlI18N, string ConnectionString) : base(ConnectionString) {
            _sqlI18N = SqlI18N;
        }
        #endregion
        #region Methods
        public List<PurchaseAmount> GetAdmissionFeesList (int year) {
            string query = "SELECT TenantBranchID,SUM(ISNULL(WalletOut, 0)) AS Total FROM dbo.Wallet WHERE WalletType = 'ADMISSION' AND Status = 'APPROVED' AND IsDeleted = 0 AND YEAR(CreatedDate) = @Year GROUP BY TenantBranchID;";
            try {
                return GetList<PurchaseAmount>(query, _sqlI18N, new { Year = year });
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
        }

        public List<PurchaseAmount> GetPurchaseAmount (int year) {
            List<PurchaseAmount> list;
            string query = "SELECT TenantBranchID,Total FROM dbo.tfnGetIndentAmount(@Year) ORDER BY Total DESC";
            try {
                list = GetList<PurchaseAmount>(query, _sqlI18N, new { Year = year });
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return list;
        }

        public List<StudentContacts> GetStudentContacts(Guid tenantBranchID) {
            string query = "SELECT ContactID,ContactName,Gender,PhoneCode,Phone FROM dbo.Contact WHERE IsDeleted = 0 AND TenantBranchID = @TenantBranchID";
            return GetList<StudentContacts>(query, _sqlI18N, new { TenantBranchID = tenantBranchID });
        }

        public InvoiceDetail GetInvoiceDetail(Guid indentID) {
            string query = "SELECT InvoiceID,IndentID,InvoiceNo,InvoiceDate AS InvoiceDateI18 FROM dbo.InvoiceMaster WHERE IndentID = @IndentID";
            return Get<InvoiceDetail>(query, _sqlI18N, new { IndentID = indentID });
        }

        public Guid InvoiceCreation (InvoiceCreate invoice, Guid actionBy) {
            Guid invoiceID = Guid.Empty;
            try {
                invoiceID = (Guid)ExecuteScalar("uspCreateInvoice", new {
                    IndentID = invoice.IndentID,
                    Address = invoice.Address,
                    CGST = invoice.CGST,
                    SGST = invoice.SGST,
                    IGST = invoice.IGST,
                    CreatedBy = actionBy
                }, commandType: System.Data.CommandType.StoredProcedure);
                return invoiceID;
            }
            catch(SqlException ex) {
                throw new Exception(ex.Message);
            }
        }

        public InvoiceDetail GetInvoiceByNumber (string indentNo) {
            string query = "SELECT InvoiceID,IndentID,InvoiceNo,InvoiceDate AS InvoiceDateI18,BillingAddress,ShippingAddress,CGST,SGST,IGST FROM dbo.InvoiceMaster WHERE InvoiceNo = @InvoiceNo";
            return Get<InvoiceDetail>(query, _sqlI18N, new { InvoiceNo = indentNo });
        }

        public GetBranchAddress GetBranchAddress(Guid tenantBranchID) {
            return Get<GetBranchAddress>("SELECT Address1,Address2,Area,District,State,Country,Pincode FROM dbo.FranchiseeEnquiryDetail WHERE TenantBranchID = @TenantBranchID AND IsDeleted = 0", _sqlI18N, new {
                TenantBranchID = tenantBranchID
            });
        }

        public GetBranchAddress GetHOAddress () {
            return Get<GetBranchAddress>("SELECT TOP 1 ca.Address1,ca.Address2,ca.Area,ca.District,ca.State,ca.Country,ca.Pincode FROM dbo.Contact c INNER JOIN dbo.UserProfile u ON u.ContactTypeID = c.ContactTypeID INNER JOIN dbo.ContactAddress ca ON ca.ContactID = c.ContactID WHERE u.ContactType = 'TenantAdmin' AND c.IsDeleted = 0", _sqlI18N);
        }

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

        public GetAdminContactInfo GetAdminContact () {
            string query = "SELECT c.PhoneCode,c.Phone,c.Email,'TCDS Computer Education' AS BranchName FROM dbo.Contact c INNER JOIN dbo.UserProfile u ON u.ContactTypeID = c.ContactTypeID WHERE u.ContactType = 'TenantAdmin'";
            return Get<GetAdminContactInfo>(query, _sqlI18N);
        }

        public GetBranchContactDetail BranchDetail (Guid tenantBranchID) {
            string query = "SELECT fe.TenantBranchID,fe.FranchiseeName AS BranchName,fe.ContactName,fe.Gender,CONCAT(fe.PhoneCode,fe.Phone) AS Phone,CONCAT(fe.AlternatePhoneCode,fe.AlternatePhone) AS AlternatePhone,fe.Email,fe.Address1,fe.Address2,fe.Country,fe.State,fe.District,fe.Area,fe.Pincode,fe.FranchiseeCode,fe.Taluk FROM dbo.FranchiseeEnquiryDetail AS fe WHERE fe.TenantBranchID = @TenantBranchID";
            return Get<GetBranchContactDetail>(query, _sqlI18N, new { TenantBranchID = tenantBranchID });
        }

        public int EditStudent(EditStudent student, Guid actionBy) {
            int result = 0;
            try {
                result = ExecuteScalar("uspEditStudent", new {
                    ContactID = student.ContactID,
                    ContactName = student.ContactName,
                    Gender = student.Gender,
                    DateOfBirth = student.DateOfBirth,
                    PhoneCode = student.PhoneCode,
                    Phone = student.Phone,
                    AlternatePhoneCode = student.AlternatePhoneCode,
                    AlternatePhone = student.AlternatePhone,
                    Email = student.Email,
                    EditStudentAddress = student.Address.ToDataTable(),
                    UpdatedBy = actionBy
                }, commandType: System.Data.CommandType.StoredProcedure).ToInt();
                return result;
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
        }

        public GetMobileNumber GetMobileNumber (Guid contactID) {
            string query = "SELECT PhoneCode,Phone,AlternatePhoneCode,AlternatePhone FROM dbo.Contact WHERE ContactID = @ContactID AND IsDeleted = 0";
            return Get<GetMobileNumber>(query, _sqlI18N, new { ContactID = contactID });
        }

        public string GetEmail (Guid contactID) {
            return ExecuteScalar("SELECT ISNULL(Email,'') FROM dbo.Contact WHERE ContactID = @ContactID AND IsDeleted = 0", new { ContactID = contactID }).ToString();
        }

        public string GetContactName (Guid contactID) {
            return ExecuteScalar("SELECT ContactName FROM dbo.Contact WHERE ContactID = @ContactID AND IsDeleted = 0", new { ContactID = contactID }).ToString();
        }

        public DataSet MblPaymentHistory(Guid batchMappingID) {
            return GetDataSet("uspGetStudentPaymentHistory", new { BatchMappingID = batchMappingID }, commandType: System.Data.CommandType.StoredProcedure);
        }

        public List<FeesBalanceCount> GetNetFees(Guid tenantBranchID) {
            string query = "SELECT sbm.BatchMappingID,sbm.NetCourseFees FROM dbo.StudentBatchMapping sbm INNER JOIN dbo.Students s ON s.StudentID = sbm.StudentID AND sbm.IsDeleted = 0 WHERE s.TenantBranchID = @TenantBranchID";
            return GetList<FeesBalanceCount>(query, _sqlI18N, new { TenantBranchID = tenantBranchID });
        }

        public List<FeesBalanceCount> GetPaidFees () {
            string query = "SELECT SUM(ISNULL(tf.TransactionAmount,0)) AS PaidFees, tb.KeyValue AS BatchMappingID FROM dbo.TenantBranchReceiptAndPayment tb INNER JOIN dbo.TenantBranchFinanceRegister tf ON tf.TenantBranchTransID = tb.TenantBranchTransID WHERE tb.KeyTable = 'StudentBatchMapping' AND tb.KeyColumn = 'BatchMappingID' AND tb.IsDeleted = 0 GROUP BY tb.KeyValue";
            return GetList<FeesBalanceCount>(query);
        }

        public string GetStudentPlace (Guid contactID) {
            return ExecuteScalar("SELECT ca.Area FROM dbo.ContactAddress ca INNER JOIN dbo.AddressType a ON a.AddressTypeID = ca.AddressTypeID WHERE a.AddressType = 'Communication Address' AND ca.IsDeleted = 0 AND ca.ContactID = @ContactID", new { ContactID = contactID }).ToString();
        }

        public List<StudentPaymentDetails> GetStudentPaymentDetails (Guid tenantBranchID, DateTime fromDate, DateTime toDate) {
            List<StudentPaymentDetails> paymentDetails = new List<StudentPaymentDetails>();
            string query = "SELECT TenantBranchTransID,ReceiptDateI18,ReceiptNo,Amount,PaymentType,PayMode,KeyValue,Summary,TransID FROM dbo.tfnGetStudentPaymentDetail(@TenantBranchID) WHERE CAST(VoucherDate AS DATE) BETWEEN @FromDate AND @ToDate ORDER BY VoucherDate DESC;";
            try {
                paymentDetails = GetList<StudentPaymentDetails>(query, _sqlI18N, new { TenantBranchID = tenantBranchID, FromDate = fromDate, ToDate = toDate });
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return paymentDetails;
        }
        public List<GetExamIndentTransID> ExamIndentTransID(Guid tenantBranchID)
        {
            List<GetExamIndentTransID> transIDs = new List<GetExamIndentTransID>();
            string query = "SELECT ExamIndentTransID,StudentBatchMapping,Semester FROM dbo.tfnGetBranchWiseExamTransIDIntented(@TenantBranchID)";
            try
            {
                transIDs = GetList<GetExamIndentTransID>(query, _sqlI18N, new { TenantBranchID = tenantBranchID });
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            return transIDs;
        }
        public decimal GetWalletBalance(Guid tenantBranchID)
        {
            decimal result = 0;
            try
            {
                result = (decimal)ExecuteScalar("SELECT dbo.sfnGetWalletBalance(@TenantBranchID)", new
                {
                    TenantBranchID = tenantBranchID
                });
            }
            catch (SqlException)
            {
                throw;
            }
            return result;
        }
        public Guid CreateWalletPayment(WalletPayment payment)
        {
            try
            {
                return (Guid)ExecuteScalar("uspCreateWalletPayment", new
                {
                    TenantBranchID = payment.TenantBranchID,
                    WalletAmount = payment.WalletAmount,
                    SenderID = payment.SenderID,
                    ReceiverID = payment.ReceiverID,
                    Description = payment.Description,
                    CreatedBy = payment.CreatedBy,
                    WalletType = payment.WalletType

                }, commandType: CommandType.StoredProcedure);
            }
            catch (SqlException)
            {
                throw;
            }
        }
        public int RollbackWallet(Guid walletPaymentID)
        {
            try
            {
                return (int)ExecuteScalar("uspRollbackWalletPayment", new
                {
                    WalletID = walletPaymentID
                }, commandType: CommandType.StoredProcedure);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public BranchContactDetailUpdated GetBranchDetails(Guid tenantBranchID)
        {
            try
            {
                return Get<BranchContactDetailUpdated>("uspGetBranchDetail",_sqlI18N, new
                {
                    TenantBranchID = tenantBranchID
                }, commandType: CommandType.StoredProcedure);
            }
            catch (SqlException)
            {
                throw;
            }
        }
        public List<MaterialIssuedList> GetMaterialIssuedList(string fromDate, string toDate, Guid tenantBranchID, Guid courseTypeID, string shortName)
        {
            try
            {
                return GetList<MaterialIssuedList>("uspGetMaterialIssuedList", _sqlI18N, new
                {
                    FromDate = fromDate,
                    ToDate = toDate,
                    TenantBranchID = tenantBranchID,
                    CourseTypeID = courseTypeID,
                    ShortName = shortName
                }, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public StudentCourseInfo GetStudentCourseInfo(Guid batchMappingID, Guid courseTypeId, string shortName)
        {
            string query = "SELECT StudentID,ContactID,BatchMappingID,RollNumber,Course,BatchNumber,BatchTime,FacultyID,ShortName,CourseTypeName,CourseTypeID FROM dbo.tfnGetStudentCertificateInfo(@BatchMappingID) WHERE (@ShortName = '' OR @ShortName IS NULL  OR ShortName = @ShortName) AND (@CourseTypeID = CAST(0x0 AS UNIQUEIDENTIFIER) OR CourseTypeID = @CourseTypeID)";
            try
            {
                return Get<StudentCourseInfo>(query, _sqlI18N, new { BatchMappingID = batchMappingID, CourseTypeID = courseTypeId, ShortName = shortName });
            }
            catch (SqlException)
            {
                throw;
            }
        }
        public List<Paymenttypes> GetPaymenttypes(Guid branchMaterialIndentID,string keyTable)
        {
            try
            {
                string query = @"SELECT a.VoucherNo,a.VoucherDate as VoucherDateI18,b.TransactionAmount,c.PayMode FROM dbo.TenantBranchReceiptAndPayment a 
                                 INNER JOIN dbo.TenantBranchFinanceRegister b ON b.TenantBranchTransID = a.TenantBranchTransID
                                 INNER JOIN dbo.PayModeType c ON c.PayModeID = b.PayModeID
                                 WHERE a.KeyValue = @BranchMaterialIndentID AND a.KeyTable = @KeyTable";
                return GetList<Paymenttypes>(query, _sqlI18N, new
                {
                    BranchMaterialIndentID = branchMaterialIndentID,
                    KeyTable = keyTable
                });
            }
            catch (SqlException)
            {
                throw;
            }
        }
        public List<GetCourse> GetCourseInfo(DataTable batchMappingIDs)
        {
            try
            {
                string query = "uspGetExamIndents";
                return GetList<GetCourse>(query, _sqlI18N, new
                {
                    BatchMappings = batchMappingIDs
                }, commandType: CommandType.StoredProcedure);
            }
            catch (SqlException)
            {
                throw;
            }
        }
        public GetFranchiseeAddress GetFranchiseeAddress(Guid tenantBranchID)
        {
            try
            {
                string query = "SELECT * FROM dbo.tfnGetFranchiseeAddress(@TenantBranchID)";
                return Get<GetFranchiseeAddress>(query, _sqlI18N, new
                {
                    TenantBranchID = tenantBranchID
                });
            }
            catch (SqlException)
            {
                throw;
            }
        }
        public AdminContact GetAdminContactInfo()
        {
            try
            {
                string query = @"SELECT * FROM dbo.tfnGetAdminContact()";
                return Get<AdminContact>(query, _sqlI18N);
            }
            catch (SqlException)
            {
                throw;
            }
        }
        public int UpdateStudentTotalMark(DataTable updateStudentTotalMark, Guid actionBy, bool preview)
        {
            try
            {
                return ExecuteScalar("uspUpdateStudentTotalMarkWithPreview", new 
                { 
                    UpdateStudentTotalMark = updateStudentTotalMark,
                    UpdatedBy = actionBy,
                    Preview = preview
                }, commandType: System.Data.CommandType.StoredProcedure).ToInt();
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public List<ExamTransID> GetExamIndentTransID(Guid batchMappingID,int semester)
        {
            List<ExamTransID> examIndentTransID = new List<ExamTransID>();
            string query = "SELECT ExamIndentTransID,ExamDate,StudentBatchMapping,Semester FROM dbo.tfnGetExamIndentTransID(@BatchMappingID) WHERE Semester = @Semester";
            try
            {
                examIndentTransID = GetList<ExamTransID>(query, _sqlI18N, new { BatchMappingID = batchMappingID, Semester = semester });
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            return examIndentTransID;
        }
        public ReprintCertificateIndent CreateReprintCertificateIndent(DataTable certificateAndMarkSheetIndent, Guid actionBy, Guid tenantBranchID, decimal courierAmount,decimal totalAmount)
        {
            try
            {
                return Get<ReprintCertificateIndent>("uspReprintCertificateAndMarkSheetIndent",_sqlI18N, new
                {
                    CertificateAndMarkSheetIndent = certificateAndMarkSheetIndent,
                    TenantBranchID = tenantBranchID,
                    CreatedBy = actionBy,
                    TotalAmount = totalAmount,
                    CourierAmount = courierAmount
                }, commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            
        }
        public int RollbackReprintIndent(Guid certificateIndentID)
        {
            int count = 0;
            try
            {
                count = ExecuteScalar("uspRollbackCertificateIndent", new { CertificateIndentID = certificateIndentID }, commandType: System.Data.CommandType.StoredProcedure).ToInt();
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            return count;
        }
        public string GetStudentGrade(decimal percent)//t)//int obtainedMark,int totalMark
        {
            string query = @"SELECT dbo.sfnGetStudentPreviewGrade(@ObtainPer) AS result";            
            return (string)ExecuteScalar(query,new
            {               
                ObtainPer = percent
            });    
        }
        public string GetPayModeType(Guid payModeId)
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
        public List<FeesBalance> GetFeesBalance(DataTable batchMappingIds)
        {
            try
            {
                return GetList<FeesBalance>("uspGetStudentPaidFees", _sqlI18N, new
                {
                    BatchMappingID = batchMappingIds
                }, commandType: CommandType.StoredProcedure);
            }
            catch (SqlException)
            {

                throw;
            }
        }
        public int GetIDCardCharges()
        {
            return ExecuteScalar("SELECT MinValue FROM dbo.AppConfiguration WHERE ConfigID = 'IDCARD_CHARGES' AND ConfigGroupID ='FRANCHISEE' AND IsDeleted = 0").ToInt();
        }
        public List<StudentInfo> GetStudentInfo(DataTable batchMapingIds)
        {
            List<StudentInfo> studentList = new List<StudentInfo>();
            try
            {
                string query = @"uspGetStudentCourseInfo";
                studentList = GetList<StudentInfo>(query, _sqlI18N, new
                {
                    BatchMappingID = batchMapingIds
                },commandType:CommandType.StoredProcedure);
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            return studentList;
        }
        public List<FranchiseeDetailsForIDCard> FranchiseeDetailsForIDCard(DataTable tenantBranchIds)
        {
            try
            {
                return GetList<FranchiseeDetailsForIDCard>("uspGetFranchiseDetails", _sqlI18N, new
                {
                    TenantBranchID = tenantBranchIds
                }, commandType: CommandType.StoredProcedure);
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public List<GetBranchContactDetail> GetBranchDetail(DataTable tenantbranchIds)
        {
            return GetList<GetBranchContactDetail>("uspGetAllBranchDetail", _sqlI18N, new
            {
                TenantBranchID = tenantbranchIds
            },commandType: CommandType.StoredProcedure);
        }
        public List<PaymentDetailsBulk> GetPaymentDetails(DataTable keyValue)
        {
            List<PaymentDetailsBulk> payments = new List<PaymentDetailsBulk>();
            string query = "uspGetPaymentDetailByKeyValues";

            try
            {
                payments = GetList<PaymentDetailsBulk>(query, _sqlI18N, new { KeyValue = keyValue },commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return payments;
        }
        public List<FranchiseDetails> FranchiseDetails(string district)
        {
            List<FranchiseDetails> details = new List<FranchiseDetails>();
            var dataSet= GetDataSet("uspFranchiseDetailsForWebsite", new {
                District = district
            }, commandType: CommandType.StoredProcedure);
            foreach (DataRow item in dataSet.Tables[0].Rows)
            {
                FranchiseDetails data = new FranchiseDetails
                {
                    FranchiseeName = item["FranchiseeName"]?.ToString() ?? string.Empty,
                    ContactName = item["ContactName"]?.ToString() ?? string.Empty,
                    Phone = item["Phone"]?.ToString() ?? string.Empty,
                    Address1 = item["Address1"]?.ToString() ?? string.Empty,
                    Address2 = item["Address2"]?.ToString() ?? string.Empty,
                    Area = item["Area"]?.ToString() ?? string.Empty,
                    Taluk = item["Taluk"]?.ToString() ?? string.Empty,
                    State = item["State"]?.ToString() ?? string.Empty,
                    District = item["District"]?.ToString() ?? string.Empty,
                    Pincode = item["Pincode"]?.ToString() ?? string.Empty,
                    FileName = item["FileName"]?.ToString() ?? string.Empty,
                    AttachmentContent = item["AttachmentContent"] == DBNull.Value ? null : (byte[])item["AttachmentContent"],
                    TenantBranchID = Guid.TryParse(item["TenantBranchID"]?.ToString(), out var guid) ? guid : Guid.Empty
            };
                details.Add(data);
            }
            return details;
        }
        //public List<CourseList> GetCourseListForWebsite(Guid tenantBranchID)
        //{
        //    try
        //    {
        //        return GetList<CourseList>("uspGetCourseListForWebsite",_sqlI18N, new
        //        {
        //            TenantBranchID = tenantBranchID
        //        }, commandType: System.Data.CommandType.StoredProcedure);
        //    }
        //    catch (SqlException ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }
        //}
        public DataSet GetCourseListForWebsite(Guid tenantBranchID)
        {
            try
            {
                return GetDataSet("uspGetCourseListForWebsite", new
                {
                    TenantBranchID = tenantBranchID
                }, commandType: CommandType.StoredProcedure);
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public List<VendorDetail> VendorDetails(DataTable branchOrVendorIDs)
        {
            try
            {
                return GetList<VendorDetail>("uspGetVEndorDetails", _sqlI18N, new
                {
                    BranchOrVendorIDs = branchOrVendorIDs
                }, commandType: CommandType.StoredProcedure);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public List<FranchiseeCodeDetails> GetFranchiseeCode(DataTable branchOrVendorIDs)
        {
            return GetList<FranchiseeCodeDetails>("uspGetFranchsisBranchDetails", _sqlI18N, new
            {
                BranchOrVendorIDs = branchOrVendorIDs
            }, commandType: CommandType.StoredProcedure);
        }
        public string GetOTPType(Guid otpTransID)
        {
            try
            {
                string query = "SELECT dbo.sfnGetOTPTypeByTRansID(@OTPTransID) as Result";
                return (string)ExecuteScalar(query, new
                {
                    OTPTransID = otpTransID
                }); 
            }
            catch (Exception)
            {
                throw;
            }
        }       
        #endregion
    }
}
