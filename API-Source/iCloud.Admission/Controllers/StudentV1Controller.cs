using Dawn.Frameworks.Entities;
using Dawn.Frameworks.Helper;
using Dawn.Utilities.Enums;
using Dawn.Utilities.Exceptions;
using Dawn.Utilities.i18N;
using EntitiesDTO.Material;
using EntitiesDTO.OTP;
using EntitiesDTO.Payment;
using EntitiesDTO.Student;
using EntitiesDTO.Utilities;
using Frameworks.Helper;
using iCloud.Admission.Services;
using MassTransit.RabbitMqTransport.Topology;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace iCloud.Admission.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{v:apiVersion}/student/")]
    [ApiController]
    public class StudentV1Controller : ControllerBase
    {
        #region Fields
        private readonly StudentService _studentService;
        #endregion
        #region Constructor
        public StudentV1Controller(SqlI18N sqlI18N, IHttpContextAccessor accessor, IDawnHttpClient dawnHttpClient)
        {
            _studentService = new StudentService(sqlI18N, accessor, dawnHttpClient);
        }
        #endregion
        #region Methods
        /// <summary>
        /// Get qualification list
        /// </summary>
        /// <param createdBy="Sakthivel"></param>
        /// <returns>
        /// Returns qualification list
        /// </returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("qualification/list")]
        [DawnAuthorize("TenantAdmin", "TenantUser", "Guest")]
        public List<GetQualification> GetQualificationList()
        {
            List<GetQualification> qualifications;
            try
            {
                qualifications = _studentService.GetQualificationList();
                return qualifications;
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }

        /// <summary>
        /// Get occupation list
        /// </summary>
        /// <param createdBy="Sakthivel"></param>
        /// <returns>
        /// Returns occupation list
        /// </returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("occupation/list")]
        [DawnAuthorize("TenantAdmin", "TenantUser", "Guest")]
        public List<GetOccupation> GetOccupationList()
        {
            List<GetOccupation> qualifications;
            try
            {
                qualifications = _studentService.GetOccupationList();
                return qualifications;
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }

        /// <summary>
        /// Register new student
        /// </summary>
        /// <param name="student"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns>
        /// Returns student id
        /// </returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpPost("register")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<APIResult<Guid>> RegisterStudent([FromForm] RegisterStudent student)
        {
            Guid result;
            try
            {
                result = await _studentService.RegisterStudent(student);
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return APIResult.Success(result, "Student registered successfully", DawnStatusCode.Created);
        }
        /// <summary>
        /// Create student down payment
        /// </summary>
        /// <param name="courseMap"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns>
        /// Returns student info
        /// </returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpPost("downpayment")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<APIResult<string>> CreateStudentCourseMap(NewStudentCourseMap courseMap)
        {
            string result;
            try
            {
                result = await _studentService.CreateStudentCourseMap(courseMap);
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return APIResult.Success(result, "Student course mapping successfull", DawnStatusCode.Created);
        }

        /// <summary>
        /// Create student part payment
        /// </summary>
        /// <param name="partPayment"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpPost("partpayment")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<APIResult<string>> CreateStudentPartPayment(StudentPartPayment partPayment)
        {
            string result;
            try
            {
                result = await _studentService.CreateStudentpartPayment(partPayment);
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return APIResult.Success(result, "Student part payment processed successfull", DawnStatusCode.Created);
        }

        /// <summary>
        /// Get student alumni list
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="courseID"></param>
        /// <param name="courseBatchID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpPost("alumni/list")]
        [DawnAuthorize("TenantUser")]
        public async Task<List<GetAlumniStudent>> GetAlumniStudentList(WebStudentFilter filter)
        {
            List<GetAlumniStudent> getAlumnis;
            try
            {
                getAlumnis = await _studentService.GetAlumniStudentList(filter);
                return getAlumnis;
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }

        /// <summary>
        /// Get current student list
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="courseID"></param>
        /// <param name="courseBatchID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpPost("current/list")]
        [DawnAuthorize("TenantUser")]
        public async Task<List<GetAlumniStudent>> GetCurrentStudentList(WebStudentFilter filter)
        {
            List<GetAlumniStudent> getAlumnis;
            try
            {
                getAlumnis = await _studentService.GetCurrentStudentList(filter);
                return getAlumnis;
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }

        /// <summary>
        /// Get student basic info
        /// </summary>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("basicinfo/list")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<List<StudentBasicInfo>> GetStudentBasicList()
        {
            List<StudentBasicInfo> students;
            try
            {
                students = await _studentService.GetStudentBasicList();
                return students;
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }

        /// <summary>
        /// Get student basic info list by franchisee wise
        /// </summary>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("basicinfo/list/bybranch")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<List<StudentBasicInfo>> GetStudentByBranchWise()
        {
            List<StudentBasicInfo> students;
            try
            {
                students = await _studentService.GetStudentByBranchWise();
                return students;
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }

        /// <summary>
        /// Get student basic info
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("receipt/list")]
        [DawnAuthorize("TenantUser")]
        public async Task<List<StudentReceiptList>> GetStudentReceiptList(DateTime fromDate, DateTime toDate,string? nameOrPhone)
        {
            List<StudentReceiptList> students;
            try
            {
                students = await _studentService.GetStudentReceiptList(fromDate, toDate, nameOrPhone!);
                return students;
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }

        /// <summary>
        /// Print student receipt
        /// </summary>
        /// <param name="studentID"></param>
        /// <param name="paymentType"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("print/receipt")]
        [DawnAuthorize("TenantUser")]
        public async Task<StudentReceiptList> PrintStudentReceipt(Guid tenantBranchTransID)
        {
            StudentReceiptList students;
            try
            {
                students = await _studentService.PrintStudentReceipt(tenantBranchTransID);
                return students;
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }

        /// <summary>
        /// Get student out standing balance list
        /// </summary>
        /// <param name="batchMappingID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("receipt/outstandingbalance")]
        [DawnAuthorize("TenantUser")]
        public async Task<List<StudentBalanceList>> GetStudentOutStandingBalance(Guid batchMappingID,string? nameOrPhone,string? fromDate,string? toDate,string? status)
        {
            List<StudentBalanceList> balanceLists;
            try
            {
                balanceLists = await _studentService.GetStudentOutStandingBalance(batchMappingID, nameOrPhone!,fromDate!, toDate!,status!);
                return balanceLists;
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }

        /// <summary>
        /// Get student profile by id
        /// </summary>
        /// <param name="studentID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("getbyid")]
        [DawnAuthorize("TenantAdmin", "TenantUser", "Student")]
        public async Task<StudentProfile> GetStudentProfile(Guid contactID)
        {
            StudentProfile studentProfiles;
            try
            {
                studentProfiles = await _studentService.GetStudentProfile(contactID);
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return studentProfiles;
        }

        /// <summary>
        /// Get student course details
        /// </summary>
        /// <param name="batchMappingID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("course/detail")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<StudentCourse> GetStudentCourseDetail(Guid batchMappingID)
        {
            try
            {
                return await _studentService.GetStudentCourseDetail(batchMappingID);
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }

        /// <summary>
        /// Issue material to student
        /// </summary>
        /// <param name="material"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpPost("material/issue")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<APIResult<string>> IssueMaterial(IssueMaterial material)
        {
            string result;
            try
            {
                result = await _studentService.IssueMaterial(material);
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return APIResult.Success("", result, DawnStatusCode.Created);
        }

        /// <summary>
        /// Get material isued list
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("material/issued/list")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public IEnumerable<MaterialIssuedList> MaterialIssuedList(string fromDate, string toDate)
        {
            IEnumerable<MaterialIssuedList> issuedLists;
            try
            {
                issuedLists = _studentService.MaterialIssuedList(fromDate, toDate);
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return issuedLists;
        }

        /// <summary>
        /// Get material pending list
        /// </summary>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("material/pending/list")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<IEnumerable<MaterialPendingList>> GetMaterialPendingList()
        {
            IEnumerable<MaterialPendingList> pendingLists;
            try
            {
                pendingLists = await _studentService.GetMaterialPendingList();
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return pendingLists;
        }

        /// <summary>
        /// Get material count
        /// </summary>
        /// <param name="batchMappingID"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("material/count/{batchMappingID}")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public int GetMaterialCount(Guid batchMappingID)
        {
            int count;
            try
            {
                count = _studentService.GetMaterialCount(batchMappingID);
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return count;
        }

        /// <summary>
        /// Get issued material count
        /// </summary>
        /// <param name="batchMappingID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("issued/material/count/{batchMappingID}")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public int GetIssuedMaterialCount(Guid batchMappingID)
        {
            int count;
            try
            {
                count = _studentService.GetIssuedMaterialCount(batchMappingID);
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return count;
        }

        [HttpGet("material/issued/count/{materialID}")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public int GetMaterialIssuedCount(Guid materialID)
        {
            int count;
            try
            {
                count = _studentService.GetMaterialIssuedCount(materialID);
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return count;
        }

        /// <summary>
        /// Get student actual subject mark list
        /// </summary>
        /// <param name="batchMappingID"></param>
        /// <param name="courseID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("mark/info")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<IEnumerable<GetStudentMark>> GetStudentMarks(Guid courseBatchID)
        {
            IEnumerable<GetStudentMark> marks;
            try
            {
                marks = await _studentService.GetStudentMark(courseBatchID);
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return marks;
        }

        /// <summary>
        /// Certificate indent
        /// </summary>
        /// <param name="marks"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpPost("mark/update")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<APIResult<string>> CreateStudentMarkUpdate(List<UpdateStudentMark> marks)
        {
            string result;
            try
            {
                result = await _studentService.CreateStudentMarkUpdate(marks);
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return APIResult.Success("", result, DawnStatusCode.Created);
        }
        ///// <summary>
        ///// Create certificate and marksheet indent
        ///// </summary>
        ///// <param name="indents"></param>
        ///// <param createdBy="Sakthivel"></param>
        ///// <returns></returns>
        ///// <exception cref="DawnUserException"></exception>
        //[HttpPost("indent/certificate")]
        //[DawnAuthorize("TenantAdmin", "TenantUser")]
        //public APIResult<string> CreateCertificateAndMarkSheetIndent(List<CertificateIndent> indents) {
        //    string result;
        //    try {
        //        result = _studentService.CreateCertificateAndMarkSheetIndent(indents);
        //    }
        //    catch (Exception ex) {
        //        throw new DawnUserException(ex.Message, DawnStatusCode.Error);
        //    }
        //    return APIResult.Success("", result, DawnStatusCode.Created);
        //}

        /// <summary>
        /// Get student certificate indent list
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="status"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("indent/certificate/list")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public DataSet GetCertificateIndentList(string? fromDate, string? toDate, string? status)
        {
            try
            {
                DataSet data = _studentService.GetCertificateIndentList(fromDate ?? "", toDate ?? "", status ?? "");
                return data;
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }

        [HttpGet("certificate/issued/list")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public DataSet GetCertificateIssuedList(string fromDate, string toDate)
        {
            try
            {
                return _studentService.GetCertificateIssuedList(fromDate, toDate);
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }

        /// <summary>
        /// Get student course basic info
        /// </summary>
        /// <param name="batchMappingID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("course/info/{batchMappingID}")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public StudentCourseInfo GetStudentCourseInfo(Guid batchMappingID)
        {
            try
            {
                return _studentService.GetStudentCourseInfo(batchMappingID);
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }

        /// <summary>
        /// Edit student subject marks
        /// </summary>
        /// <param name="editMarks"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpPost("mark/edit")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<APIResult<string>> EditStudentMark(List<EditMark> editMarks)
        {
            string result;
            try
            {
                result = await _studentService.EditStudentMark(editMarks);
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return APIResult.Success("", result, DawnStatusCode.Created);
        }

        /// <summary>
        /// Get certificate and marksheet despatch list
        /// </summary>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("certificate/despatch/list")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<List<CertificateDespatchList>> GetCertificateDespatchList()
        {
            List<CertificateDespatchList> despatchLists;
            try
            {
                despatchLists = await _studentService.GetCertificateDespatchList();
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return despatchLists;
        }

        /// <summary>
        /// Despatch certificate and marksheet indent
        /// </summary>
        /// <param name="despatches"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpPost("certificate/despatch")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public APIResult<string> DespatchCertificateAndMarkSheet(List<DespatchCertificate> despatches)
        {
            string result;
            try
            {
                result = _studentService.DespatchCertificateAndMarkSheet(despatches);
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return APIResult.Success("", result, DawnStatusCode.Created);
        }

        /// <summary>
        /// Receive certificate and marksheet
        /// </summary>
        /// <param name="receive"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpPost("certificate/receive")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public APIResult<string> ReceiveCertificateAndMarksheet(List<ReceiveCertificate> receive)
        {
            string result;
            try
            {
                result = _studentService.ReceiveCertificateAndMarksheet(receive);
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return APIResult.Success("", result, DawnStatusCode.Created);
        }

        /// <summary>
        /// Get certificate print list
        /// </summary>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("certificate/print/list")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<List<NewCertificatePrintList>> GetCertificatePrintList()
        {
            List<NewCertificatePrintList> certificatePrintLists;
            try
            {
                certificatePrintLists = await _studentService.GetCertificatePrintList();
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return certificatePrintLists;
        }

        /// <summary>
        /// Get marksheet print list
        /// </summary>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("marksheet/print/list")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<List<NewMarkSheetPrintList>> GetMarkSheetPrintList()
        {
            List<NewMarkSheetPrintList> markSheetPrintLists;
            try
            {
                markSheetPrintLists = await _studentService.GetMarkSheetPrintList();
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return markSheetPrintLists;
        }

        /// <summary>
        /// Issue certificate and marksheet to student
        /// </summary>
        /// <param name="examIndentTransID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("marksheetandcertificate/issue")]
        [DawnAuthorize("TenantUser")]
        public async Task<APIResult<string>> IssueCertificateAndMarksheet(Guid certificateAndMarksheetID)
        {
            string result;
            try
            {
                result = await _studentService.IssueCertificateAndMarksheet(certificateAndMarksheetID);
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return APIResult.Success("", result, DawnStatusCode.Created);
        }

        /// <summary>
        /// Get material required list
        /// </summary>
        /// <param name="tenantBranchID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("material/required/list/{tenantBranchID}")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public List<MaterialRequiredList> MaterialRequiredList(Guid tenantBranchID)
        {
            List<MaterialRequiredList> materialRequiredLists;
            try
            {
                materialRequiredLists = _studentService.MaterialRequiredList(tenantBranchID);
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return materialRequiredLists;
        }

        /// <summary>
        /// Print certificate
        /// </summary>
        /// <param name="examIndentTransID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("certificate/print")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<CertificatePrint> CreateCertificatePrint(Guid examIndentTransID)
        {
            CertificatePrint certificatePrint;
            try
            {
                certificatePrint = await _studentService.CreateCertificatePrint(examIndentTransID);
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return certificatePrint;
        }

        /// <summary>
        /// Print marksheet
        /// </summary>
        /// <param name="examIndentTransID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("marksheet/print")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<MarksheetPrint> PrintMarksheet(Guid examIndentTransID)
        {
            MarksheetPrint marksheetPrint;
            try
            {
                marksheetPrint = await _studentService.PrintMarksheet(examIndentTransID);
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return marksheetPrint;
        }

        /// <summary>
        /// Get students by batch wise
        /// </summary>
        /// <param name="courseBatchID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("batch/wise/list/{courseBatchID}")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<List<NewStudentByBatch>> GetStudentByBatchWise(Guid courseBatchID)
        {
            List<NewStudentByBatch> studentByBatches;
            try
            {
                studentByBatches = await _studentService.GetStudentByBatchWise(courseBatchID);
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return studentByBatches;
        }

        /// <summary>
        /// Print certificate and marksheet
        /// </summary>
        /// <param name="examIndentTransID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpPost("print")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public APIResult<string> PrintCertificateAndMarksheet(List<GetCertificateID> print)
        {
            string result;
            try
            {
                result = _studentService.PrintCertificateAndMarksheet(print);
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return APIResult.Success("", result, DawnStatusCode.Created);
        }

        /// <summary>
        /// Print hall ticket
        /// </summary>
        /// <param name="examIndentTransID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("print/hallticket")]
        [DawnAuthorize("TenantAdmin","TenantUser")]
        public async Task<List<PrintHallTicket>> PrintHallTicket(Guid examIndentID)
        {
            List<PrintHallTicket> hallTicket;
            try
            {
                hallTicket = await _studentService.PrintHallTicket(examIndentID);
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return hallTicket;
        }

        [HttpPost("enquiry/create")]
        [DawnAuthorize("Guest", "TenantUser")]
        public async Task<APIResult<Guid>> CreateStudentEnquiry(CreateStudentEnquiry enquiry)
        {
            Guid studentEnquiryID;
            try
            {
                studentEnquiryID = await _studentService.CreateStudentEnquiry(enquiry);
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return APIResult.Success(studentEnquiryID, "Registration request send successfully", DawnStatusCode.Created);
        }

        [HttpGet("enquiry/list")]
        [DawnAuthorize("TenantUser")]
        public async Task<List<GetStudentEnquiry>> GetEnquiryList(DateTime fromDate, DateTime toDate,string? nameOrPhone)
        {
            List<GetStudentEnquiry> getStudents;
            try
            {
                getStudents = await _studentService.GetEnquiryList(fromDate, toDate, nameOrPhone ?? "");
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return getStudents;
        }

        [HttpGet("admission/report")]
        [DawnAuthorize("TenantUser")]
        public async Task<List<GetAlumniStudent>> GetAdmissionReport(string fromDate, string toDate)
        {
            List<GetAlumniStudent> getAlumnis;
            try
            {
                getAlumnis = await _studentService.GetAdmissionReport(fromDate, toDate);
                return getAlumnis;
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }

        [HttpPost("discontinued")]
        [DawnAuthorize("TenantUser")]
        public APIResult<string> Discontinued(CreateDiscontinued discontinued)
        {
            string result;
            try
            {
                result = _studentService.StudentDiscontinued(discontinued);
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return APIResult.Success("", result, DawnStatusCode.Created);
        }

        [HttpPost("material/issued-pending/list")]
        [DawnAuthorize("TenantUser")]
        public async Task<IEnumerable<NewMaterialPendingList>> GetMblMaterialPendingList(MaterialFilter filter)
        {
            IEnumerable<NewMaterialPendingList> pendingLists;
            try
            {
                pendingLists = await _studentService.GetMblMaterialPendingList(filter);
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return pendingLists;
        }

        [HttpPost("mobile/list")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<IEnumerable<StudentMblList>> GetStudentMblList(StudentFilter filter)
        {
            try
            {
                return await _studentService.GetStudentMblList(filter);
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }

        [HttpGet("admission/statistics")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public AdmissionStatistics GetAdmissionStatistics(int year)
        {
            try
            {
                return _studentService.GetAdmissionStatistics(year);
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }

        [HttpGet("admission/statistics/{date}")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public AdmissionStatisticsByDate GetAdmissionStatisticsByDate(DateTime date)
        {
            try
            {
                return _studentService.GetAdmissionStatisticsByDate(date);
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }

        [HttpGet("all/basicinfo/list/bybranch")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<List<StudentBasicInfo>> GetAllStudentByBranchWise()
        {
            List<StudentBasicInfo> students;
            try
            {
                students = await _studentService.GetAllStudentByBranchWise();
                return students;
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }

        [HttpGet("mbl/fees/balance")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<List<CourseDetail>> MblFeesBalanceList(Guid? batchMappingID)
        {
            try
            {
                return await _studentService.MblFeesBalanceList(batchMappingID);
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }

        [HttpGet("payment/history")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<List<MblPaymentHistory>> MblPaymentHistory(Guid batchMappingID)
        {
            try
            {
                return await _studentService.MblPaymentHistory(batchMappingID);
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }

        [HttpGet("status/dropdown")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public string[] GetStatusDropdown()
        {
            return _studentService.GetStatusDropdown();
        }
        [HttpGet("status/dropdown/web")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public string[] GetStatusDropdownWeb()
        {
            return _studentService.GetStatusDropdownWeb();
        }

        [HttpGet("certificate/marksheet/tracking")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<List<CertificateIndentTrack>> CertificateIndentTrack(string status,string? fromDate, string? toDate)
        {
            try
            {
                return await _studentService.CertificateIndentTrack(status,fromDate,toDate);
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }

        [HttpPost("enquiry/reject")]
        [DawnAuthorize("TenantUser")]
        public APIResult<string> RejectStudentEnquiry(RejectStudentEnquiry reject)
        {
            try
            {
                return APIResult.Success("", _studentService.RejectStudentEnquiry(reject), DawnStatusCode.Success);
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }

        [HttpGet("enquiry")]
        [DawnAuthorize("Guest")]
        public async Task<GetEnquiryByStudent> GetEnquiryByStudent(Guid studentEnquiryID)
        {
            try
            {
                return await _studentService.GetEnquiryByStudent(studentEnquiryID);
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }

        [HttpPost("edit")]
        [DawnAuthorize("TenantUser")]
        public async Task<APIResult<string>> EditStudent(EditStudent student)
        {
            try
            {
                return APIResult.Success("", await _studentService.EditStudent(student), DawnStatusCode.Success);
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }

        [HttpGet("reason/dropdown")]
        [DawnAuthorize("Guest", "TenantUser")]
        public IEnumerable<ReasonDropdown> GetReasonDropdown()
        {
            try
            {
                return _studentService.ReasonDropdown();
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }

        [HttpPost("enquiry/report")]
        [DawnAuthorize("TenantUser", "TenantUser")]
        public async Task<IEnumerable<StudentEnquiryReport>> GetEnquiryByStudent(EnquiryFilter filter)
        {
            try
            {
                return await _studentService.GetEnquiryReport(filter);
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }

        [HttpGet("enquiry/report/status")]
        [DawnAuthorize("TenantUser", "TenantUser")]
        public string[] ReportStatusDropdown()
        {
            return _studentService.ReportStatusDropdown();
        }

        [HttpGet("image")]
        [DawnAuthorize("TenantUser", "TenantUser", "Guest")]
        public async Task<List<DocumentURLDetails>> GetStudentImage(Guid contactID)
        {
            try
            {
                return await _studentService.GetStudentImage(contactID);
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }

        [HttpGet("id/cards")]
        [DawnAuthorize("TenantUser", "TenantUser", "Guest")]
        public async Task<List<GetStudentIDCard>> GetStudentIDCard(Guid courseBatchID)
        {
            try
            {
                return await _studentService.GetStudentIDCard(courseBatchID);
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }

        [HttpGet("purpose/dropdown")]
        [DawnAuthorize("Guest", "TenantUser")]
        public IEnumerable<DropdownDetails> GetPurposeDropdown()
        {
            try
            {
                return _studentService.GetPurposeDropdown();
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }

        [HttpGet("referby/dropdown")]
        [DawnAuthorize("Guest", "TenantUser")]
        public IEnumerable<DropdownDetails> GetReferByDropdown()
        {
            try
            {
                return _studentService.GetReferByDropdown();
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }
        /*Using this register method for student creation using mobile and also web */
        [HttpPost("register/phone")]
        [DawnAuthorize("TenantAdmin", "TenantUser", "Guest")]
        public async Task<APIResult<StudentRegisterResult>> RegisterStudentByPhone([FromForm] RegisterStudent student)
        {
            try
            {
                return APIResult.Success(await _studentService.RegisterStudentByPhone(student), "Student registered successfully", DawnStatusCode.Created);
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }
        [HttpGet("login/list")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<APIResult<List<StudentLoginList>>> GetStudentListByPhone(string phoneNumber)
        {
            try
            {
                return APIResult.Success(await _studentService.GetStudentListByPhone(phoneNumber),
                    "Student ContactID list retrieved successfully !!!", DawnStatusCode.Success);
            }
            catch (DawnUserException ex)
            {
                throw new DawnUserException(ex.Message);
            }
        }
        #region OTP For SMS
        [HttpPost("otp/request")]
        [DawnAuthorize("TenantAdmin", "TenantUser","Guest")]
        public async Task<APIResult<RegistrationOTPResult>> OTPRequest(RegistrationOTPRequest otpRequest)
        {
            try
            {
                return APIResult.Success(await _studentService.RegistrationOTPRequest(otpRequest),
                    OTPMessage.SMSOTPSent, DawnStatusCode.Success);
            }
            catch (DawnUserException ex)
            {
                throw new DawnUserException(ex.Message);
            }
        }
        [HttpPost("sms/otp/resend")]
        [DawnAuthorize("TenantAdmin", "TenantUser", "Guest")]
        public async Task<APIResult<OTPResult>> SMSOTPResend(SMSResendRequest otpResend)
        {
            try
            {
                return APIResult.Success(await _studentService.SMSOTPResend(otpResend),
                    OTPMessage.SMSReSent, DawnStatusCode.Success);
            }
            catch (DawnUserException ex)
            {
                throw new DawnUserException(ex.Message);
            }
        }
        [HttpPost("otp/verify")]
        [DawnAuthorize("TenantAdmin", "TenantUser", "Guest")]
        public async Task<APIResult<OTPVerifyResult>> OTPVerify(OTPVerify otpVerify)
        {
            try
            {
                OTPVerifyResult oTPVerify = await _studentService.OTPVerify(otpVerify);
                return APIResult.Success(oTPVerify, oTPVerify.Message, oTPVerify.IsOTPVerified ? DawnStatusCode.Created : DawnStatusCode.Error);
            }
            catch (DawnUserException ex)
            {
                throw new DawnUserException(ex.Message);
            }
        }
        [HttpPost("forgot/otp/request")]
        [DawnAuthorize("TenantAdmin", "TenantUser", "Guest")]
        public async Task<APIResult<RegistrationOTPResult>> ForgotOTPRequest(RegistrationOTPRequest otpRequest)
        {
            try
            {
                return APIResult.Success(await _studentService.ForgotOTPRequest(otpRequest),
                    OTPMessage.SMSOTPSent, DawnStatusCode.Success);
            }
            catch (DawnUserException ex)
            {
                throw new DawnUserException(ex.Message);
            }
        }
        #endregion
        [HttpPost("preview/mark/update")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<APIResult<List<MarkUpdatePreview>>> CreateStudentMarkUpdatePreview(PreviewMarksUpdate update)
        {
            try
            {
                var result = await _studentService.CreateStudentMarkUpdateNew(update.Marks, update.Preview);
                return APIResult.Success(result, "Certificate send for print successfully", DawnStatusCode.Created);
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }
        [HttpPost("certificate/preview")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<APIResult<List<CertificatePrint>>> PreviewCertificatePrint(List<PreviewCretificateMarksheet> examIndentTransID)
        {
            try
            {
                 var certificatePrint = await _studentService.PreviewCertificate(examIndentTransID);
                 return APIResult.Success(certificatePrint,"", DawnStatusCode.Success);
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }
        [HttpPost("marksheet/preview")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<APIResult<List<MarksheetPrint>>> PreviewMarksheet(List<PreviewCretificateMarksheet> examIndentTransID)
        {
            try
            {
               var marksheetPrint = await _studentService.PreviewMarksheet(examIndentTransID);
                return APIResult.Success(marksheetPrint, "", DawnStatusCode.Success);

            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }
        [HttpGet("reprint/student/list")]
        [DawnAuthorize("TenantUser")]
        public async Task<APIResult<List<GetReprintStudent>>> GetReprintingStudentList()
        {
            try
            {
                var studentList = await _studentService.GetReprintingStudent();
                return APIResult.Success(studentList, "", DawnStatusCode.Success);
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }
        [HttpGet("reprint/student/mark")]
        [DawnAuthorize("TenantUser")]
        //public async Task<List<StudentMarkReprint>> GetStudentMarkReprint(Guid batchMappingID, int semester)
        public async Task<APIResult<List<StudentMarkReprintResult>>> GetStudentMarkReprint(Guid batchMappingID, int semester)
        {
            try
            {
                var studentList = await _studentService.GetStudentMarkForReprint(batchMappingID,semester);
                return APIResult.Success(studentList,"", DawnStatusCode.Success);
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }
        [HttpPost("reprint/mark/update")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<APIResult<List<MarkUpdatePreview>>> CreateReprintStudentMarkUpdatePreview(PreviewMarksUpdate update)
        {
            try
            {
                var result = await _studentService.CreateReprintStudentMarkUpdateNew(update.Marks, update.Preview);
                return APIResult.Success(result, "Preview mark update successfully", DawnStatusCode.Created);
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }
        [HttpPost("reprint/marksheet/preview")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<APIResult<List<MarksheetPrint>>> RePrintPreviewMarksheet(List<PreviewCretificateMarksheet> examIndentTransID)
        {
            try
            {
                var marksheetPrint = await _studentService.RePrintPreviewMarksheet(examIndentTransID);
                return APIResult.Success(marksheetPrint, "", DawnStatusCode.Success);
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }
        [HttpPost("reprint/certificate/preview")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<APIResult<List<CertificatePrint>>> RePrintPreviewCertificate(List<PreviewCretificateMarksheet> examIndentTransID)
        {
            try
            {
                var certificatePrint = await _studentService.RePrintPreviewCertificate(examIndentTransID);
                return APIResult.Success(certificatePrint, "", DawnStatusCode.Success);
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }
        //public async Task<List<StudentMarkReprintResult>> CreateCertificateMarksheetIndentReprinting(Guid batchMappingID, int semester)
        //{
        //    try
        //    {
        //        var studentList = await _studentService.CreateCertificateIndentReprinting(batchMappingID, semester);
        //        return studentList;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new DawnUserException(ex.Message, DawnStatusCode.Error);
        //    }
        //}
        [HttpPost("id-cards/print")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<APIResult<List<GetStudentIDCard>>> GetStudentIDCardList(List<StudentIDCards> batchmapingIDs)
        {
            try
            {
                return APIResult.Success(await _studentService.GetStudentIDCardList(batchmapingIDs), "Student ID Card details retrived successfully", DawnStatusCode.Success);
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }
        [HttpPost("mark/edit/rerequest")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<APIResult<string>> EditStudentMarkRerequest(List<EditMark> editMarks)
        {
            string result;
            try
            {
                result = await _studentService.EditStudentMarkRerequest(editMarks);
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return APIResult.Success( result,"Marked edited successfully" ,DawnStatusCode.Created);
        }
        #endregion
    }
}
