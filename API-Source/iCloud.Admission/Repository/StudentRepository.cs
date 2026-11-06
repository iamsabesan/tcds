using Consul;
using Consul.Filtering;
using Dawn.Database;
using Dawn.Utilities.Extensions;
using Dawn.Utilities.i18N;
using Entities.Common;
using EntitiesDTO.Material;
using EntitiesDTO.Student;
using EntitiesDTO.Utilities;
using Microsoft.Data.SqlClient;
using System.Data;

namespace iCloud.Admission.Repository
{
    public class StudentRepository : DawnDbClient
    {
        #region Fields
        private readonly SqlI18N _sqlI18N;
        #endregion
        #region Constructor
        public StudentRepository(SqlI18N SqlI18N, string ConnectionString) : base(ConnectionString)
        {
            _sqlI18N = SqlI18N;
        }
        #endregion
        #region Methods
        /// <summary>
        /// Get student qualification list
        /// </summary>
        /// <param createdBy="sakthivel"></param>
        /// <param createdDate="03-05-2023"></param>
        /// <returns>
        /// Returns qualification list
        /// </returns>
        /// <exception cref="Exception"></exception>
        public List<GetQualification> GetQualificationList()
        {
            List<GetQualification> qualifications = new List<GetQualification>();
            string query = "SELECT QualificationID,Qualification FROM dbo.Qualification";
            try
            {
                qualifications = GetList<GetQualification>(query, _sqlI18N);
                return qualifications;
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Get occupation list
        /// </summary>
        /// <param createdBy="sakthivel"></param>
        /// <param createdDate="03-05-2023"></param>
        /// <returns>
        /// Returns occupation list
        /// </returns>
        /// <exception cref="Exception"></exception>
        public List<GetOccupation> GetOccupationList()
        {
            List<GetOccupation> occupations = new List<GetOccupation>();
            string query = "SELECT OccupationID,Occupation FROM dbo.Occupation ORDER BY SlNo ASC";
            try
            {
                occupations = GetList<GetOccupation>(query, _sqlI18N);
                return occupations;
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Register new student
        /// </summary>
        /// <param name="student"></param>
        /// <param name="actionBy"></param>
        /// <param createdBy="sakthivel"></param>
        /// <param createdDate="15-05-2023"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public Guid RegisterStudent(CreateStudent student, Guid actionBy, string password, string franchiseeNo)
        {
            Guid result = Guid.Empty;
            string query = "uspStudentRegister";
            try
            {
                result = (Guid)ExecuteScalar(query, new
                {
                    TenantBranchID = student.TenantBranchID,
                    ContactID = student.ContactID,
                    Qualification = student.Qualification,
                    GuardianType = student.GuardianType,
                    GuardianName = student.GuardianName,
                    Occupation = student.Occupation,
                    Password = password,
                    CreatedBy = actionBy,
                    FranchiseeNo = franchiseeNo
                }, commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }
        public StudentContactID RegisterStudentNew(RegisterStudent register,string password,string franchiseeNo, Guid branchID, Guid actionBy)
        {
            try
            {
                return Get<StudentContactID>("uspStudentRegister1", _sqlI18N, new
                {
                    TenantBranchID = branchID,
                    Qualification = register.Qualification,
                    GuardianType = register.GuardianType,
                    GuardianName = register.FatherName,
                    Occupation = register.Occupation,
                    Password = password,
                    CreatedBy = actionBy,
                    FranchiseeNo = franchiseeNo,
                    ReasonToSelect = register.ReasonToSelect,
                    ReferBy = register.ReferBy,
                    Purpose = register.Purpose
                },
                commandType: CommandType.StoredProcedure);
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Create student course mapping
        /// </summary>
        /// <param name="courseMap"></param>
        /// <param createdBy="sakthivel"></param>
        /// <param createdDate="17-05-2023"></param>
        /// <returns>
        /// Returns batch mapping id and course fees
        /// </returns>
        /// <exception cref="Exception"></exception>
        public GetBatchMap CreateStudentCourseMap(StudentCourseMap courseMap)
        {
            GetBatchMap batchMap = new GetBatchMap();
            string query = "uspCreateStudentCourseMap";
            try
            {
                batchMap = Get<GetBatchMap>(query, _sqlI18N, new
                {
                    StudentID = courseMap.StudentID,
                    CourseBatchID = courseMap.CourseBatchID,
                    Discount = courseMap.Discount,
                    DownPayment = courseMap.DownPayment,
                    CreatedBy = courseMap.CreatedBy,
                }, commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            return batchMap;
        }

        public Guid GetStudentContactID(Guid studentID)
        {
            Guid contactID = Guid.Empty;
            try
            {
                contactID = (Guid)ExecuteScalar("SELECT ContactID FROM dbo.Students WHERE StudentID = @StudentID AND IsDeleted = 0", new { StudentID = studentID });
                return contactID;
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Guid StudentContactID(Guid batchMappingID)
        {
            Guid contactID = Guid.Empty;
            try
            {
                contactID = (Guid)ExecuteScalar("SELECT DISTINCT s.ContactID FROM dbo.StudentBatchMapping sb INNER JOIN dbo.Students s ON s.StudentID = sb.StudentID WHERE sb.BatchMappingID = @BatchMappingID AND s.IsDeleted = 0", new { BatchMappingID = batchMappingID });
                return contactID;
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Rollback student course details
        /// </summary>
        /// <param name="studentID"></param>
        /// <param name="batchMappingID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int RollBackStudentCourseMap(Guid studentID, Guid batchMappingID)
        {
            int count = 0;
            try
            {
                count = ExecuteScalar("uspRollbackStudentCourseMap", new { StudentID = studentID, BatchMappingID = batchMappingID }, commandType: System.Data.CommandType.StoredProcedure).ToInt();
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            return count;
        }

        public int RollBackNewStudentCourseMap(Guid studentID, Guid batchMappingID)
        {
            int count = 0;
            try
            {
                count = ExecuteScalar("uspRollbackNewStudentCourseMap", new { StudentID = studentID, BatchMappingID = batchMappingID }, commandType: System.Data.CommandType.StoredProcedure).ToInt();
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            return count;
        }

        public int RollBackStudent(Guid studentID)
        {
            int count = 0;
            try
            {
                count = ExecuteScalar("uspRollbackStudent", new { StudentID = studentID }, commandType: System.Data.CommandType.StoredProcedure).ToInt();
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            return count;
        }

        /// <summary>
        /// Check student id is exists or not
        /// </summary>
        /// <param name="studentID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns>
        /// Returns true or false
        /// </returns>
        public bool CheckStudentIDIsExists(Guid studentID, Guid courseBatchID)
        {
            bool result = ExecuteScalar("SELECT dbo.sfnStudentIDExist(@StudentID,@CourseBatchID)", new
            {
                StudentID = studentID,
                CourseBatchID = courseBatchID
            }).ToBoolean();
            return result;
        }

        /// <summary>
        /// Get student alumnus list
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="courseID"></param>
        /// <param name="courseBatchID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<AlumniStudent> GetAlumniStudentList(WebStudentFilter filter, Guid tenantBranchID)
        {
            List<AlumniStudent> alumnis = new List<AlumniStudent>();
            try
            {
                string query = "SELECT ContactID,BatchMappingID,StudentID,RegisteredDate,RollNumber,Course,StartDate,EndDate,StartTime,EndTime,ShortName,CourseTypeName FROM dbo.tfnGetStudentAlumniList(@FromDate,@ToDate,@CourseID,@CourseBatchID,@TenantBranchID,@CourseTypeID) ORDER BY RegisteredDate DESC";
                alumnis = GetList<AlumniStudent>(query, _sqlI18N, new
                {
                    FromDate = filter.StartDate,
                    ToDate = filter.EndDate,
                    CourseID = filter.CourseID,
                    CourseBatchID = filter.CourseBatchID,
                    tenantBranchID = tenantBranchID,
                    CourseTypeID = filter.CourseTypeID
                });
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            return alumnis;
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
        /// <exception cref="Exception"></exception>
        public List<AlumniStudent> GetCurrentStudentList(WebStudentFilter filter, Guid tenantBranchID)
        {
            List<AlumniStudent> alumnis = new List<AlumniStudent>();
            try
            {
                string query = "SELECT ContactID,BatchMappingID,StudentID,RegisteredDate,RollNumber,Course,StartDate,EndDate,StartTime,EndTime,IsActive,Reason,ShortName,CourseTypeName FROM dbo.tfnGetCurrentStudentList(@FromDate,@ToDate,@CourseID,@CourseBatchID,@TenantBranchID,@CourseTypeID) ORDER BY RegisteredDate DESC";
                alumnis = GetList<AlumniStudent>(query, _sqlI18N, new
                {
                    FromDate = filter.StartDate,
                    ToDate = filter.EndDate,
                    CourseID = filter.CourseID,
                    CourseBatchID = filter.CourseBatchID,
                    tenantBranchID = tenantBranchID,
                    CourseTypeID = filter.CourseTypeID
                });
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            return alumnis;
        }

        /// <summary>
        /// Get student basic info list
        /// </summary>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<StudentInfo> GetStudentInfo()
        {
            List<StudentInfo> alumnis = new List<StudentInfo>();
            try
            {
                string query = "SELECT BatchMappingID,ContactID,StudentID,RollNumber,Course,BatchNumber,BatchTime,ShortName,CourseTypeName FROM dbo.tfnGetStudentbasicInfo()";
                alumnis = GetList<StudentInfo>(query, _sqlI18N);
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            return alumnis;
        }

        /// <summary>
        /// Get student by franchisee wise
        /// </summary>
        /// <param name="tenantBranchID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<StudentInfo> GetStudentByBranchWise(Guid tenantBranchID)
        {
            List<StudentInfo> alumnis = new List<StudentInfo>();
            try
            {
                string query = "SELECT ContactID,StudentID,RollNumber FROM dbo.tfnGetStudentBranchWise(@TenantBranchID)";
                alumnis = GetList<StudentInfo>(query, _sqlI18N, new { TenantBranchID = tenantBranchID });
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            return alumnis;
        }

        /// <summary>
        /// Get student receipt list
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<StudentInfos> GetStudentReceiptInfo(Guid tenantBranchID)
        {
            List<StudentInfos> alumnis = new List<StudentInfos>();
            try
            {
                string query = "SELECT ContactID,StudentID,RollNumber,Course,BatchNumber,BatchMappingID,ShortName,CourseTypeName FROM dbo.tfnGetStudentReceiptList(@TenantBranchID)";
                alumnis = GetList<StudentInfos>(query, _sqlI18N, new { TenantBranchID = tenantBranchID });
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            return alumnis;
        }

        /// <summary>
        /// Print student receipt
        /// </summary>
        /// <param name="tenantBranchID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<StudentInfos> PrintStudentReceipt(Guid tenantBranchID)
        {
            List<StudentInfos> alumnis = new List<StudentInfos>();
            try
            {
                string query = "SELECT ContactID,StudentID,RollNumber,Course,BatchNumber,BatchMappingID,ShortName,CourseTypeName FROM dbo.tfnPrintStudentReceiptList(@TenantBranchID)";
                alumnis = GetList<StudentInfos>(query, _sqlI18N, new { TenantBranchID = tenantBranchID });
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            return alumnis;
        }

        /// <summary>
        /// Get student outstanding balance list
        /// </summary>
        /// <param name="courseBatchID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<StudentsList> GetStudentBalanceList(Guid courseBatchID, Guid tenantBranchID, string fromDate, string toDate)
        {
            List<StudentsList> students = new List<StudentsList>();
            string query = "SELECT ContactID,StudentID,RollNumber,Course,StartTime,EndTime,TotalFees,BatchMappingID,ShortName,CourseTypeName,Status,StartDateI18,EndDateI18 FROM dbo.tfnGetStudentList(@CourseBatchID, @TenantBranchID, @FromDate, @ToDate)";
            try
            {
                students = GetList<StudentsList>(query, _sqlI18N, new { 
                    CourseBatchID = courseBatchID,
                    TenantBranchID = tenantBranchID,
                    FromDate = fromDate,
                    Todate = toDate
                    //Status = status
                });
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            return students;
        }

        /// <summary>
        /// Get student profile by id
        /// </summary>
        /// <param name="studentID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public DataSet GetStudentProfile(Guid studentID)
        {
            try
            {
                string query = "uspGetStudentProfile";
                return GetDataSet(query, new { StudentID = studentID }, commandType: CommandType.StoredProcedure);
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Guid GetStudentID(Guid contactID)
        {
            return (Guid)ExecuteScalar("SELECT DISTINCT StudentID FROM dbo.Students WHERE ContactID = @ContactID AND IsDeleted = 0", new { ContactID = contactID });
        }

        /// <summary>
        /// Issue material to student
        /// </summary>
        /// <param name="material"></param>
        /// <param name="actionBy"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int IssueMaterial(IssueMaterial material, Guid actionBy)
        {
            int result = 0;
            try
            {
                result = ExecuteScalar("uspIssueMaterial", new
                {
                    BatchMappingID = material.BatchMappingID,
                    MaterialID = material.MaterialID,
                    CreatedBy = actionBy
                }, commandType: System.Data.CommandType.StoredProcedure).ToInt();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Get material issued list
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public IEnumerable<MaterialIssuedList> GetMaterialIssuedList(string fromDate, string toDate, Guid tenantBranchID)
        {
            IEnumerable<MaterialIssuedList> issuedLists = new List<MaterialIssuedList>();
            string query = "SELECT StudentID,ContactID,CourseID,RollNumber,Course,MaterialID,IssuedDate,ShortName,CourseTypeName,CourseTypeID FROM dbo.tfnGetMaterialIssuedList(@FromDate,@ToDate,@TenantBranchID)";
            try
            {
                issuedLists = GetList<MaterialIssuedList>(query, _sqlI18N, new
                {
                    FromDate = fromDate,
                    ToDate = toDate,
                    TenantBranchID = tenantBranchID
                });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return issuedLists;
        }

        /// <summary>
        /// Get material pending liat
        /// </summary>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public IEnumerable<MaterialPendingList> GetMaterialPendingList(Guid tenantBranchID)
        {
            IEnumerable<MaterialPendingList> pendingLists = new List<MaterialPendingList>();
            string query = "SELECT StudentID,ContactID,BatchMappingID,RollNumber,Course,BatchTime,ShortName,CourseTypeName FROM dbo.tfnGetMaterialPendingList(@TenantBranchID)";
            try
            {
                pendingLists = GetList<MaterialPendingList>(query, _sqlI18N, new { TenantBranchID = tenantBranchID });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return pendingLists;
        }

        public IEnumerable<MaterialPendingList> GetMaterialIssuePending(Guid tenantBranchID, MaterialFilter filter)
        {
            IEnumerable<MaterialPendingList> pendingLists = new List<MaterialPendingList>();
            string query = "SELECT StudentID,ContactID,BatchMappingID,RollNumber,Course,BatchTime FROM dbo.tfnGetMaterialIssuedPending(@StudentID,@CourseID,@CourseBatchID,@TenantBranchID)";
            try
            {
                pendingLists = GetList<MaterialPendingList>(query, _sqlI18N, new
                {
                    TenantBranchID = tenantBranchID,
                    StudentID = filter.StudentID,
                    CourseID = filter.CourseID,
                    CourseBatchID = filter.CourseBatchID
                });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return pendingLists;
        }

        /// <summary>
        /// Get material count
        /// </summary>
        /// <param name="batchMappingID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int GetMaterialCount(Guid batchMappingID)
        {
            int count = 0;
            string query = "SELECT dbo.sfnGetMaterialCount(@BatchMappingID)";
            try
            {
                count = ExecuteScalar(query, new { BatchMappingID = batchMappingID }).ToInt();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return count;
        }

        /// <summary>
        /// Get material issued count
        /// </summary>
        /// <param name="batchMappingID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int GetIssuedMaterialCount(Guid batchMappingID)
        {
            int count = 0;
            string query = "SELECT dbo.sfnGetIssuedMaterialCount(@BatchMappingID)";
            try
            {
                count = ExecuteScalar(query, new { BatchMappingID = batchMappingID }).ToInt();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return count;
        }

        public int GetMaterialIssuedCount(Guid materialID, Guid tenantBranchID)
        {
            int count = 0;
            string query = "SELECT dbo.sfnGetMaterialIssuedCount(@MaterialID,@TenantBranchID)";
            try
            {
                count = ExecuteScalar(query, new { MaterialID = materialID, TenantBranchID = tenantBranchID }).ToInt();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return count;
        }

        /// <summary>
        /// Get subject list
        /// </summary>
        /// <param name="batchMappingID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public IEnumerable<GetSubjects> GetSubjectsList(Guid batchMappingID, int semester)
        {
            IEnumerable<GetSubjects> studentMarks = new List<GetSubjects>();
            string query = "SELECT SubjectID,MaterialID,StudentID FROM dbo.tfnGetMarkInfo(@BatchMappingID,@Semester)";
            try
            {
                studentMarks = GetList<GetSubjects>(query, _sqlI18N, new { BatchMappingID = batchMappingID, Semester = semester });
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            return studentMarks;
        }

        /// <summary>
        /// Get student mark list
        /// </summary>
        /// <param name="studentID"></param>
        /// <param name="subjectID"></param>
        /// <param name="materialID"></param>
        /// <param name="courseBatchID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<GetStudentMarkInfo> GetStudentMarkInfo(Guid studentID, Guid subjectID, Guid materialID, Guid courseBatchID)
        {
            List<GetStudentMarkInfo> studentMarks = new List<GetStudentMarkInfo>();
            string query = "SELECT StudentID,SubjectMappingID,RollNumber,ContactID,TheoryMarks,PracticalMarks,Semester,SubjectName FROM dbo.tfnGetStudentMarkInfo(@StudentID,@SubjectID,@MaterialID,@CourseBatchID)";
            try
            {
                studentMarks = GetList<GetStudentMarkInfo>(query, _sqlI18N, new { StudentID = studentID, SubjectID = subjectID, MaterialID = materialID, CourseBatchID = courseBatchID });
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            return studentMarks;
        }

        /// <summary>
        /// Update student subject marks
        /// </summary>
        /// <param name="UpdateMark"></param>
        /// <param name="actionBy"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<StudentTotalMarks> CreateStudentMarkUpdate(DataTable updateMark, Guid actionBy)
        {
            List<StudentTotalMarks> student = new List<StudentTotalMarks>();
            try
            {
                student = GetList<StudentTotalMarks>("uspCreateMarkUpdate", _sqlI18N, new
                {
                    CreatedBy = actionBy,
                    UpdateMark = updateMark
                }, commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            return student;
        }
        public List<StudentTotalMarks> CreateStudentMarkUpdateNew(DataTable updateMark, Guid actionBy, bool preview)
        {
            List<StudentTotalMarks> student = new List<StudentTotalMarks>();
            try
            {
                student = GetList<StudentTotalMarks>("uspCreateMarkUpdateWithPreview", _sqlI18N, new
                {
                    CreatedBy = actionBy,
                    UpdateMark = updateMark,
                    Preview = preview
                }, commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            return student;
        }
        public List<StudentTotalMarks> CreateReprintStudentMarkUpdateNew(DataTable updateMark, Guid actionBy, bool preview)
        {
            List<StudentTotalMarks> student = new List<StudentTotalMarks>();
            try
            {
                student = GetList<StudentTotalMarks>("uspCreateReprintMarkUpdate", _sqlI18N, new
                {
                    CreatedBy = actionBy,
                    UpdateMark = updateMark,
                    Preview = preview
                }, commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            return student;
        }
        public int GetPassMark()
        {
            int passMark = 0;
            passMark = ExecuteScalar("SELECT MinValue FROM dbo.AppConfiguration WHERE ConfigID = 'MARK' AND ConfigGroupID = 'STUDENT' AND IsDeleted = 0").ToInt();
            return passMark;
        }

        public List<ExamIndentTransIDs> GetExamIndentTransID(DataTable updateMark)
        {
            List<ExamIndentTransIDs> student = new List<ExamIndentTransIDs>();
            try
            {
                student = GetList<ExamIndentTransIDs>("uspGetExamIndentTransID", _sqlI18N, new
                {
                    UpdateMark = updateMark
                }, commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            return student;
        }

        public int CheckExamIndentTransIDExists(Guid examIndentTransID)
        {
            int result = 0;
            try
            {
                result = ExecuteScalar("SELECT COUNT(*) FROM dbo.CertificateAndMarksheet WHERE ExamIndentTransID = @ExamIndentTransID", new { ExamIndentTransID = examIndentTransID }).ToInt();
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Check whether student mark updated or not
        /// </summary>
        /// <param name="examIndentTransID"></param>
        /// <param name="subjectMappingID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public bool CheckStudentMarkUpdated(Guid examIndentTransID, Guid subjectMappingID)
        {
            bool result = false;
            string query = "SELECT dbo.sfnIsMarkUpdated(@ExamIndentTransID,@SubjectMappingID)";
            try
            {
                result = ExecuteScalar(query, new { ExamIndentTransID = examIndentTransID, SubjectMappingID = subjectMappingID }).ToBoolean();
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Create certificate and marksheet indent
        /// </summary>
        /// <param name="certificateAndMarkSheetIndent"></param>
        /// <param name="actionBy"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int CreateCertificateIndent(DataTable certificateAndMarkSheetIndent, Guid actionBy, Guid tenantBranchID)
        {
            int result = 0;
            try
            {
                result = ExecuteScalar("uspCertificateAndMarkSheetIndent", new
                {
                    CertificateAndMarkSheetIndent = certificateAndMarkSheetIndent,
                    TenantBranchID = tenantBranchID,
                    CreatedBy = actionBy
                }, commandType: System.Data.CommandType.StoredProcedure).ToInt();
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Get certificate indent list
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="status"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public DataSet GetCertificateIndent(string fromDate, string toDate, string status, Guid tenantBranchID)
        {
            try
            {
                return GetDataSet("uspGetCertificateIndent", new
                {
                    FromDate = fromDate,
                    ToDate = toDate,
                    Status = status,
                    tenantBranchID = tenantBranchID
                }, commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public DataSet GetCertificateIssuedList(string fromDate, string toDate, Guid tenantBranchID)
        {
            try
            {
                return GetDataSet("uspGetCertificateIssuedList", new
                {
                    FromDate = fromDate,
                    ToDate = toDate,
                    tenantBranchID = tenantBranchID
                }, commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Get student course basic info by batch id
        /// </summary>
        /// <param name="batchMappingID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public StudentCourseInfo GetStudentCourseInfo(Guid batchMappingID)
        {
            string query = "SELECT StudentID,ContactID,BatchMappingID,RollNumber,Course,BatchNumber,BatchTime,FacultyID,ShortName,CourseTypeName,CourseTypeID FROM dbo.tfnGetStudentCertificateInfo(@BatchMappingID)";
            try
            {
                return Get<StudentCourseInfo>(query, _sqlI18N, new { BatchMappingID = batchMappingID });
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Get student course info list
        /// </summary>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<StudentCourseInfo> GetStudentCourseInfoList()
        {
            string query = "SELECT StudentID,ContactID,BatchMappingID,RollNumber,Course,BatchNumber,BatchTime,FacultyID,CourseTypeName,ShortName FROM dbo.tfnGetCertificatePrint()";
            try
            {
                return GetList<StudentCourseInfo>(query, _sqlI18N);
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Edit student subject marks
        /// </summary>
        /// <param name="editMark"></param>
        /// <param name="actionBy"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public GetTotalMark EditStudentMark(DataTable editMark, Guid actionBy)
        {
            GetTotalMark getTotal = new GetTotalMark();
            try
            {
                getTotal = Get<GetTotalMark>("uspEditStudentMark", _sqlI18N, new
                {
                    EditMark = editMark,
                    UpdatedBy = actionBy
                }, commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            return getTotal;
        }

        /// <summary>
        /// Check examindentTransID is exists
        /// </summary>
        /// <param name="examIndentTransID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public bool CheckExamIndentTransID(Guid examIndentTransID)
        {
            bool result = false;
            string query = "SELECT dbo.sfnCheckExamIndentTransID(@ExamIndentTransID)";
            try
            {
                result = ExecuteScalar(query, new { ExamIndentTransID = examIndentTransID }).ToBoolean();
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }
        public bool CheckExamIndentTransIDForRePrint(Guid examIndentTransID)
        {
            bool result = false;
            string query = "SELECT dbo.sfnCheckExamIndentTransIDForRerequest(@ExamIndentTransID)";
            try
            {
                result = ExecuteScalar(query, new { ExamIndentTransID = examIndentTransID }).ToBoolean();
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        public int CheckMark(Guid examIndentTransID, Guid subjectMappingID, int theoryMark, int practicalMark)
        {
            int result = 0;
            try
            {
                return result = ExecuteScalar("SELECT COUNT(*) FROM dbo.StudentMarks WHERE SubjectMappingID = @SubjectMappingID AND ExamIndentTransID = @ExamIndentTransID AND (TheoryMark < @TheoryMark OR PracticalMark < @PracticalMark)", new
                {
                    SubjectMappingID = subjectMappingID,
                    ExamIndentTransID = examIndentTransID,
                    TheoryMark = theoryMark,
                    PracticalMark = practicalMark
                }).ToInt();
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Get certificate despatch list
        /// </summary>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<CertificateDespatchInfo> CertificateDespatchList()
        {
            List<CertificateDespatchInfo> despatchLists = new List<CertificateDespatchInfo>();
            string query = "SELECT ExamIndentTransID,TenantBranchID,Type,IndentNumber,IndentDate FROM dbo.tfnGetCertificateDespatchList()";
            try
            {
                despatchLists = GetList<CertificateDespatchInfo>(query, _sqlI18N);
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            return despatchLists;
        }

        /// <summary>
        /// Despatch certificate and marksheet indent
        /// </summary>
        /// <param name="despatchCertificate"></param>
        /// <param name="actionBy"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int DespatchCertificateAndMarkSheet(DataTable despatchCertificate, Guid actionBy)
        {
            int result = 0;
            try
            {
                result = ExecuteScalar("uspCertificateMarkSheetDespatch", new
                {
                    DespatchCertificate = despatchCertificate,
                    UpdatedBy = actionBy
                }, commandType: System.Data.CommandType.StoredProcedure).ToInt();
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Receive certificate and marksheet
        /// </summary>
        /// <param name="receiveCertificate"></param>
        /// <param name="actionBy"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int ReceiveCertificateAndMarksheet(DataTable receiveCertificate, Guid actionBy)
        {
            int result = 0;
            try
            {
                result = ExecuteScalar("uspCertificateMarkSheetReceive", new
                {
                    ReceiveCertificate = receiveCertificate,
                    UpdatedBy = actionBy
                }, commandType: System.Data.CommandType.StoredProcedure).ToInt();
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Get certificate print list
        /// </summary>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<CertificatePrintList> GetCertificatePrintList(string printType, Guid certificateIndentID)
        {
            List<CertificatePrintList> certificates = new List<CertificatePrintList>();
            string query = "SELECT ExamIndentTransID,CertificateAndMarksheetID,Type,Status FROM dbo.tfnGetCertificatePrintList(@PrintType, @CertificateIndentID)";
            try
            {
                certificates = GetList<CertificatePrintList>(query, _sqlI18N, new { PrintType = printType, CertificateIndentID = certificateIndentID });
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            return certificates;
        }

        public List<CertificateIndentinfo> GetCertificateIndentList(string printType)
        {
            List<CertificateIndentinfo> certificates = new List<CertificateIndentinfo>();
            string query = "SELECT CertificateIndentID,TenantBranchID,IndentNumber,IndentDate,TotalItem FROM dbo.tfnGetCertificateIndentList(@PrintType) ORDER BY IndentDate DESC";
            try
            {
                certificates = GetList<CertificateIndentinfo>(query, _sqlI18N, new { PrintType = printType });
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            return certificates;
        }

        /// <summary>
        /// Get Marksheet print list
        /// </summary>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<CertificatePrintList> GetMarkSheetPrintList()
        {
            List<CertificatePrintList> certificates = new List<CertificatePrintList>();
            string query = "SELECT ExamIndentTransID,Type,Status FROM dbo.tfnGetMarkSheetPrintList()";
            try
            {
                certificates = GetList<CertificatePrintList>(query, _sqlI18N);
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            return certificates;
        }

        /// <summary>
        /// Check certificate is printed or not
        /// </summary>
        /// <param name="examIndentTransID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public bool CheckCertificatePrinted(Guid examIndentTransID)
        {
            bool result = false;
            string query = "SELECT dbo.sfnCheckCertificatePrinted(@ExamIndentTransID)";
            try
            {
                result = ExecuteScalar(query, new { ExamIndentTransID = examIndentTransID }).ToBoolean();
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Check marksheet is printed or not
        /// </summary>
        /// <param name="examIndentTransID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public bool CheckMarksheetPrinted(Guid examIndentTransID)
        {
            bool result = false;
            string query = "SELECT dbo.sfnCheckMarksheetPrinted(@ExamIndentTransID)";
            try
            {
                result = ExecuteScalar(query, new { ExamIndentTransID = examIndentTransID }).ToBoolean();
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Check certificate print type
        /// </summary>
        /// <param name="examIndentTransID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public bool CheckPrintType(Guid examIndentTransID)
        {
            bool result = false;
            string query = "SELECT dbo.sfnCheckPrintType(@ExamIndentTransID)";
            try
            {
                result = ExecuteScalar(query, new { ExamIndentTransID = examIndentTransID }).ToBoolean();
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        public bool CheckIsMarkSheet(Guid examIndentTransID)
        {
            bool result = false;
            string query = "SELECT dbo.sfnCheckIsMarkSheet(@ExamIndentTransID)";
            try
            {
                result = ExecuteScalar(query, new { ExamIndentTransID = examIndentTransID }).ToBoolean();
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Get student obtained mark
        /// </summary>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<StudentObtainedMarks> GetStudentObtainedMarks()
        {
            List<StudentObtainedMarks> obtainedMarks = new List<StudentObtainedMarks>();
            string query = $"SELECT s.ExamIndentTransID,SUM(s.ObtainedTheoryMarks) AS TheoryMarks,SUM(s.ObtainedPracticalMarks) AS PracticalMarks\r\nFROM dbo.StudentMarks s GROUP BY s.ExamIndentTransID";
            try
            {
                obtainedMarks = GetList<StudentObtainedMarks>(query, _sqlI18N);
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            return obtainedMarks;
        }

        /// <summary>
        /// Get student course details
        /// </summary>
        /// <param name="batchMappingID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public DataSet GetStudentCourseDetail(Guid batchMappingID)
        {
            try
            {
                return GetDataSet("uspGetStudentCourseDetail", new { BatchMappingID = batchMappingID }, commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Get student certificate
        /// </summary>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<StudentMarksheetAndCertificate> GetStudentCertificate()
        {
            List<StudentMarksheetAndCertificate> certificates = new List<StudentMarksheetAndCertificate>();
            string query = "SELECT CertificateAndMarksheetID,ExamIndentTransID,IssuedDate,PrintType FROM dbo.tfnGetStudentCertificate()";
            try
            {
                certificates = GetList<StudentMarksheetAndCertificate>(query, _sqlI18N);
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            return certificates;
        }

        /// <summary>
        /// Get student marksheet
        /// </summary>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<StudentMarksheetAndCertificate> GetStudentMarksheet()
        {
            List<StudentMarksheetAndCertificate> certificates = new List<StudentMarksheetAndCertificate>();
            string query = "SELECT CertificateAndMarksheetID,ExamIndentTransID,IssuedDate,PrintType FROM dbo.tfnGetStudentMarksheet()";
            try
            {
                certificates = GetList<StudentMarksheetAndCertificate>(query, _sqlI18N);
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            return certificates;
        }

        /// <summary>
        /// Issue certificate and marksheet to student
        /// </summary>
        /// <param name="examIndentTransID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int IssueCertificateAndMarksheet(Guid certificateAndMarksheetID, Guid actionBy)
        {
            int result = 0;
            try
            {
                result = ExecuteScalar("uspIssueCertificateMarkSheet", new { CertificateAndMarksheetID = certificateAndMarksheetID, UpdatedBy = actionBy }, commandType: System.Data.CommandType.StoredProcedure).ToInt();
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Get material required list
        /// </summary>
        /// <param name="tenantBranchID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public DataSet MaterialRequiredList(Guid tenantBranchID)
        {
            try
            {
                return GetDataSet("uspGetMaterialRequiredList", new { TenantBranchID = tenantBranchID }, commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Print certificate
        /// </summary>
        /// <param name="batchMappingID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public NewCertificatePrint PrintCertificate(Guid batchMappingID)
        {
            NewCertificatePrint print = new NewCertificatePrint();
            string query = "SELECT ContactID,RollNumber,ShortName,Course,StartDateI18,EndDateI18,TenantBranchID,TotalSemesters,CourseTypeName FROM dbo.tfnCreateCertificatePrint(@BatchMappingID)";
            try
            {
                print = Get<NewCertificatePrint>(query, _sqlI18N, new { BatchMappingID = batchMappingID });
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            return print;
        }

        /// <summary>
        /// Print marksheet
        /// </summary>
        /// <param name="examIndentTransID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public DataSet PrintMarksheet(Guid examIndentTransID)
        {
            try
            {
                return GetDataSet("uspCreateMarksheetPrint", new { ExamIndentTransID = examIndentTransID }, commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public DataSet RePrintMarksheet(Guid examIndentTransID)
        {
            try
            {
                return GetDataSet("uspCreateReprintPreviewMarksheet", new { ExamIndentTransID = examIndentTransID }, commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// Get students by batch wise
        /// </summary>
        /// <param name="courseBatchID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<StudentByBatch> GetStudentByBatchWise(Guid courseBatchID)
        {
            List<StudentByBatch> students = new List<StudentByBatch>();
            string query = "SELECT BatchMappingID,ContactID,StudentID,RollNumber FROM dbo.tfnGetStudentByBatchWise(@CourseBatchID)";
            try
            {
                students = GetList<StudentByBatch>(query, _sqlI18N, new { CourseBatchID = courseBatchID });
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            return students;
        }

        /// <summary>
        /// Print certificate and marksheet
        /// </summary>
        /// <param name="examIndentTransID"></param>
        /// <param name="printType"></param>
        /// <param name="actionBy"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int PrintCertificateAndMarksheet(DataTable data, Guid actionBy)
        {
            int result = 0;
            try
            {
                result = ExecuteScalar("uspCreateMarksheetCertificatePrint", new { CertificatePrint = data, UpdatedBy = actionBy }, commandType: System.Data.CommandType.StoredProcedure).ToInt();
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Get student hallticket by batchmappingID
        /// </summary>
        /// <param name="batchMappingID"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public StudentHallTicket GetStudentHallTicket(Guid batchMappingID)
        {
            StudentHallTicket student;
            string query = "SELECT RollNumber,ContactID,BatchNumber,Course,ShortName,CourseTypeName FROM dbo.tfnPrintHallTicket(@BatchMappingID)";
            try
            {
                student = Get<StudentHallTicket>(query, _sqlI18N, new { BatchMappingID = batchMappingID });
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            return student;
        }

        public Guid CreateStudentEnquiry(CreateStudentEnquiry enquiry, DataTable data, Guid actionBy)
        {
            Guid studentEnquiryID = Guid.Empty;
            try
            {
                studentEnquiryID = (Guid)ExecuteScalar("uspStudentEnquiryRegister", new
                {
                    TenantBranchID = enquiry.TenantBranchID,
                    CourseID = enquiry.CourseID,
                    Qualification = enquiry.Qualification,
                    GuardianType = enquiry.GuardianType,
                    GuardianName = enquiry.GuardianName,
                    Occupation = enquiry.Occupation,
                    StudentName = enquiry.StudentName,
                    Gender = enquiry.Gender,
                    DateOfBirth = enquiry.DateOfBirth,
                    PhoneCode = enquiry.PhoneCode,
                    Phone = enquiry.Phone,
                    AlternatePhoneCode = enquiry.AlternatePhoneCode,
                    AlternatePhone = enquiry.AlternatePhone,
                    Email = enquiry.Email,
                    CreatedBy = actionBy,
                    ReasonToSelect = enquiry.ReasonToSelect,
                    StudentAddress = data,
                    ReferBy = enquiry.ReferBy,
                    Purpose = enquiry.Purpose
                }, commandType: System.Data.CommandType.StoredProcedure);
                return studentEnquiryID;
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool IsPhoneExists(string phone)
        {
            bool result = false;
            return result = ExecuteScalar("SELECT COUNT(*) FROM dbo.StudentEnquiry WHERE Phone = @Phone AND IsDeleted = 0", new { Phone = phone }).ToBoolean();
        }

        public bool IsEmailExists(string email)
        {
            bool result = false;
            return result = ExecuteScalar("SELECT COUNT(*) FROM dbo.StudentEnquiry WHERE Email = @Email AND IsDeleted = 0", new { Email = email }).ToBoolean();
        }

        public int UpdateStudentEnquiry(Guid studentID, Guid actionBy, Guid? enquiryID)
        {
            int result = 0;
            try
            {
                return result = Update(new
                {
                    StudentID = studentID,
                    Status = "REGISTERED"
                }, new { StudentEnquiryID = enquiryID }, "StudentEnquiry", actionBy.ToString());
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<GetStudentEnquiry> GetEnquiryList(Guid tenantBranchID, DateTime fromDate, DateTime toDate, string nameOrPhone)
        {
            List<GetStudentEnquiry> getStudents;
            string query = "SELECT StudentEnquiryID,TenantBranchID,CourseID,Course,StudentName,Gender,DateOfBirthI18,PhoneCode,Phone,AlternatePhoneCode,AlternatePhone,Email,Qualification," +
                           "GuardianType,GuardianName,Occupation,ReasonToSelect,Status,RequestedDate,ReferBy,Purpose,ShortName,CourseTypeName FROM dbo.tfnGetEnquiryList(@TenantBranchID,@FromDate,@ToDate,@NameOrPhone) ORDER BY RequestedDate ASC";
            try
            {
                return getStudents = GetList<GetStudentEnquiry>(query, _sqlI18N, new
                {
                    TenantBranchID = tenantBranchID,
                    FromDate = fromDate,
                    ToDate = toDate,
                    NameOrPhone = nameOrPhone
                });
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<GetEnquiryAddress> GetEnquiryAddress(Guid studentEnquiryID)
        {
            List<GetEnquiryAddress> getStudents;
            string query = "SELECT StudentAddressID,StudentEnquiryID,AddressType,Address1,Address2,Area,Country,State,District,Pincode,Taluk FROM dbo.tfnGetEnquiryAddress(@StudentEnquiryID)";
            try
            {
                return getStudents = GetList<GetEnquiryAddress>(query, _sqlI18N, new { StudentEnquiryID = studentEnquiryID });
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<AlumniStudent> GetAdmissionReport(string fromDate, string toDate, Guid tenantBranchID)
        {
            List<AlumniStudent> alumnis = new List<AlumniStudent>();
            try
            {
                string query = "SELECT ContactID,BatchMappingID,StudentID,RegisteredDate,RollNumber,Course,StartDate,EndDate,StartTime,EndTime FROM dbo.tfnGetAdmissionReport(@FromDate,@ToDate,@TenantBranchID)";
                alumnis = GetList<AlumniStudent>(query, _sqlI18N, new
                {
                    FromDate = fromDate,
                    ToDate = toDate,
                    tenantBranchID = tenantBranchID
                });
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            return alumnis;
        }

        public int StudentDiscontinued(CreateDiscontinued discontinued, Guid actionBy)
        {
            int result = 0;
            try
            {
                result = ExecuteScalar("uspStudentDiscontinued", new
                {
                    BatchMappingID = discontinued.BatchMappingID,
                    Reason = discontinued.Reason,
                    UpdatedBy = actionBy
                }, commandType: System.Data.CommandType.StoredProcedure).ToInt();
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        public int CheckBatchMapID(Guid batchMappingID)
        {
            int result = 0;
            return result = ExecuteScalar("SELECT COUNT(*) FROM dbo.StudentBatchMapping WHERE IsActive = 0 AND BatchMappingID = @BatchMappingID", new { BatchMappingID = batchMappingID }).ToInt();
        }

        public IEnumerable<StudentMblList> GetStudentMblList(StudentFilter filter, List<StudentContacts> students)
        {
            return GetList<StudentMblList>("uspGetStudentMblList", _sqlI18N, new
            {
                //StudentID = filter.StudentID,
                //FromDate = filter.FromDate,
                //ToDate = filter.ToDate,
                CourseID = filter.CourseID,
                //CourseType = filter.CourseType,
                CourseBatchID = filter.CourseBatchID,
                Status = filter.Status.ToConditionFormat(),
                //Gender = filter.Gender,
                Students = students.ToDataTable()
            }, commandType: System.Data.CommandType.StoredProcedure);
        }

        public DataSet GetAdmissionStatistics(int year, Guid tenantBranchID)
        {
            return GetDataSet("uspGetAdmissionStatistics", new
            {
                Year = year,
                tenantBranchID = tenantBranchID
            }, commandType: System.Data.CommandType.StoredProcedure);
        }

        public AdmissionStatisticsByDate GetAdmissionStatisticsByDate(DateTime date, Guid tenantBranchID)
        {
            string query = "SELECT Total,Studying,Completed,Discontinued FROM tfnAdmissionStatisticsByDate(@Date, @TenantBranchID)";
            return Get<AdmissionStatisticsByDate>(query, _sqlI18N, new
            {
                Date = date,
                TenantBranchID = tenantBranchID
            });
        }

        public List<StudentInfo> GetAllStudentByBranchWise(Guid tenantBranchID)
        {
            List<StudentInfo> alumnis = new List<StudentInfo>();
            try
            {
                string query = "SELECT s.StudentID,s.ContactID,sb.BatchMappingID,s.RollNumber,c.ShortName,c.FullName AS Course,cb.BatchNumber,CONCAT(CONVERT(VARCHAR, cb.StartTime, 24), ' - ', CONVERT(VARCHAR, cb.EndTime, 24)) AS BatchTime,cb.StartDate AS StartDateI18,cb.EndDate AS EndDateI18 FROM dbo.Students s INNER JOIN dbo.StudentBatchMapping sb ON sb.StudentID = s.StudentID AND sb.IsDeleted = 0 INNER JOIN dbo.CourseBatch cb ON cb.CourseBatchID = sb.CourseBatchID AND cb.IsDeleted = 0 INNER JOIN dbo.Courses c ON c.CourseID = cb.CourseID AND c.IsDeleted = 0 WHERE s.IsDeleted = 0 AND s.TenantBranchID = @TenantBranchID";
                alumnis = GetList<StudentInfo>(query, _sqlI18N, new { TenantBranchID = tenantBranchID });
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            return alumnis;
        }

        public List<CourseDetail> MblFeesBalanceList(Guid? batchMappingID, Guid tenantBranchID)
        {
            string query = "SELECT sm.CourseBatchID,c.CourseID,c.ShortName,c.FullName AS Course,ct.CourseTypeName AS CourseType,CONCAT(c.Duration, ' ', c.DurationType) AS Duration,sm.CourseFees,sm.Discount,sm.NetCourseFees AS TotalFees,sm.BatchMappingID,s.ContactID,cb.StartDate AS StartDateI18,cb.EndDate AS EndDateI18,s.RollNumber FROM dbo.StudentBatchMapping sm INNER JOIN dbo.CourseBatch cb ON cb.CourseBatchID = sm.CourseBatchID INNER JOIN dbo.Courses c ON c.CourseID = cb.CourseID INNER JOIN dbo.CourseType ct ON ct.CourseTypeID = c.CourseTypeID INNER JOIN dbo.Students s ON sm.StudentID = s.StudentID WHERE c.IsDeleted = 0 AND (@BatchMappingID IS NULL OR sm.BatchMappingID = @BatchMappingID) AND s.TenantBranchID = @TenantBranchID;";
            return GetList<CourseDetail>(query, _sqlI18N, new
            {
                BatchMappingID = batchMappingID,
                TenantBranchID = tenantBranchID
            });
        }

        public decimal GetStudentCourseFees(Guid batchMappingID)
        {
            decimal total = 0;
            return total = ExecuteScalar("SELECT NetCourseFees FROM dbo.StudentBatchMapping WHERE BatchMappingID = @BatchMappingID", new { BatchMappingID = batchMappingID }).ToDecimal();
        }

        public DataSet CertificateIndentTrack(Guid tenantBranchID, string status,string? fromDate,string? toDate)
        {
            return GetDataSet("uspCertificateIndentTrack", new { TenantBranchID = tenantBranchID, Status = status, FromDate = fromDate, ToDate = toDate }, commandType: System.Data.CommandType.StoredProcedure);
        }

        public decimal GetAdmissionFees(Guid courseBatchID)
        {
            string query = "SELECT c.AdmissionFees FROM dbo.CourseBatch cb INNER JOIN dbo.Courses c ON c.CourseID = cb.CourseID AND c.IsDeleted = 0 WHERE cb.IsDeleted = 0 AND cb.CourseBatchID = @CourseBatchID";
            return ExecuteScalar(query, new { CourseBatchID = courseBatchID }).ToDecimal();
        }

        public int UpdateStudentCompleted(Guid batchMappingID, Guid actionBy)
        {
            int result = 0;
            result = Update(new
            {
                IsCompleted = 1,
            }, new { BatchMappingID = batchMappingID }, "StudentBatchMapping", actionBy.ToString());
            return result;
        }

        public int RejectStudentEnquiry(RejectStudentEnquiry reject, Guid actionBy)
        {
            int result = 0;
            try
            {
                result = ExecuteScalar("uspStudentEnquiryReject", new
                {
                    StudentEnquiryID = reject.StudentEnquiryID,
                    Reason = reject.Reason,
                    UpdatedBy = actionBy,
                }, commandType: System.Data.CommandType.StoredProcedure).ToInt();
                return result;
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public GetEnquiryByStudent GetEnquiryByStudent(Guid studentEnquiryID)
        {
            string query = "SELECT StudentEnquiryID,TenantBranchID,CourseID,Course,StudentName,Gender,DateOfBirthI18,PhoneCode,Phone,AlternatePhoneCode,AlternatePhone,Email,Qualification,GuardianType,GuardianName,Occupation,ReasonToSelect,Status,IsRejected,RejectedDate,Reason,CourseTypeName,DurationType,Duration,TotalSemesters,CourseFees,RequestedDate,ShortName FROM dbo.tfnGetEnquiryByStudent(@StudentEnquiryID)";
            return Get<GetEnquiryByStudent>(query, _sqlI18N, new { StudentEnquiryID = studentEnquiryID });
        }

        public int EditStudent(EditStudent student, Guid actionBy)
        {
            int result = 0;
            try
            {
                result = ExecuteScalar("uspEditStudent", new
                {
                    ContactID = student.ContactID,
                    StudentID = student.StudentID,
                    Qualification = student.Qualification,
                    GuardianType = student.GuardianType,
                    GuardianName = student.GuardianName,
                    Occupation = student.Occupation,
                    UpdatedBy = actionBy
                }, commandType: System.Data.CommandType.StoredProcedure).ToInt();
                return result;
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public string GetPassword(Guid studentID)
        {
            return ExecuteScalar("SELECT ISNULL(Password,'') FROM dbo.Students WHERE StudentID = @StudentID AND IsDeleted = 0", new { StudentID = studentID }).ToString();
        }

        public IEnumerable<ReasonDropdown> ReasonDropdown()
        {
            return GetList<ReasonDropdown>("SELECT ReasonID,Reason FROM dbo.ReasonMaster WHERE IsDeleted = 0 ORDER BY Reason");
        }

        public IEnumerable<StudentEnquiryReport> GetEnquiryReport(Guid tenantBranchID, EnquiryFilter filter)
        {
            string query = "SELECT StudentEnquiryID,TenantBranchID,CourseID,Course,StudentName,Gender,DateOfBirthI18,PhoneCode,Phone,AlternatePhoneCode,AlternatePhone,Email,Qualification,GuardianType," +
                           "GuardianName,Occupation,ReasonToSelect,Status,IsRejected,RejectedDate,Reason,CourseTypeName,DurationType,Duration,TotalSemesters,CourseFees," +
                           "RequestedDate,RegisteredDate,ReferBy,Purpose,ShortName FROM dbo.tfnGetEnquiryReport(@TenantBranchID,@Status,@FromDate,@ToDate) ORDER BY RequestedDate";
            return GetList<StudentEnquiryReport>(query, _sqlI18N, new
            {
                TenantBranchID = tenantBranchID,
                Status = filter.Status,
                FromDate = filter.FromDate,
                ToDate = filter.ToDate,
            });
        }

        public List<GetStudentIDCard> GetStudentIDCard(Guid courseBatchID)
        {
            string query = "SELECT s.ContactID,s.RollNumber,c.ShortName AS Course,CONCAT(cb.StartTime, ' - ', cb.EndTime) AS Batch,CONCAT((FORMAT(cb.StartDate, 'MMM yyyy')), ' - ', (FORMAT(cb.EndDate, 'MMM yyyy'))) AS Period FROM dbo.CourseBatch cb INNER JOIN dbo.Courses c ON c.CourseID = cb.CourseID INNER JOIN dbo.StudentBatchMapping sbm ON sbm.CourseBatchID = cb.CourseBatchID INNER JOIN dbo.Students s ON s.StudentID = sbm.StudentID WHERE cb.CourseBatchID = @CourseBatchID AND cb.IsDeleted = 0";
            return GetList<GetStudentIDCard>(query, _sqlI18N, new
            {
                courseBatchID = courseBatchID
            });
        }

        public IEnumerable<DropdownDetails> GetPurposeDropdown()
        {
            return GetList<DropdownDetails>("SELECT PurposeID AS ID,Purpose AS Value FROM dbo.PurposeMaster WHERE IsDeleted = 0 ORDER BY Purpose");
        }

        public IEnumerable<DropdownDetails> GetReferByDropdown()
        {
            return GetList<DropdownDetails>("SELECT ReferenceConfigID AS ID,Reference AS Value FROM dbo.ReferenceConfig WHERE IsDeleted = 0 ORDER BY SortOrder");
        }
        public List<GetAuthRoleID> GetAuthRoleID(string primaryRole, string secondaryRole)
        {
            try
            {
            string SQL = "SELECT AuthRoleID FROM dbo.AuthRole WHERE AuthRole = @PrimaryRole UNION ALL SELECT AuthRoleID FROM dbo.AuthRole WHERE AuthRole = @SecondaryRole";
            return GetList<GetAuthRoleID>(SQL, _sqlI18N,new { PrimaryRole = primaryRole, SecondaryRole = secondaryRole });
            }
            catch (SqlException)
            {
                throw;
            }
        }
        public int RegisterStudentInContact(RegisterStudent student,Guid contactID, Guid tenantBranchID, Guid actionBy)
        {
            try
            {
                return ExecuteScalar("uspRegisterStudentInContact", new
                {
                    ContactID = contactID,
                    Name = student.StudentName,
                    PhoneCode = student.PhoneCode,
                    Phone = student.Phone,
                    AltPhoneCode = student.AlternatePhoneCode,
                    AltPhone = student.AlternatePhone,
                    Email = student.Email,
                    Gender = student.Gender,
                    DateOfBirth = student.DateOfBirth,
                    TenantBranchID = tenantBranchID,
                    ActionBy = actionBy
                }, commandType: CommandType.StoredProcedure).ToInt();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public int CheckStudentPhoneNumber(string phone)
        {
            try
            {
                string query = "SELECT COUNT(Phone) FROM dbo.Contact WHERE phone = @Phone AND IsDeleted = 0";
                return ExecuteScalar(query, new
                {
                    Phone = phone
                }).ToInt();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<StudentLoginList> GetStudentListByPhone(string phoneNumber)
        {
            try
            {
                string query = "SELECT ContactID, ContactName AS StudentName FROM dbo.Contact WHERE Phone =@Phone";
                return GetList<StudentLoginList>(query, _sqlI18N, new
                {
                    Phone = phoneNumber
                });
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void RollbackContact(Guid contactID)
        {
            try
            {
                ExecuteScalar("uspRollbackContactAndAddress", new
                {
                    ContactID = contactID
                }, commandType: CommandType.StoredProcedure);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void RollBackInStudent(Guid contactID)
        {
            try
            {
                string query = "DELETE FROM dbo.Students WHERE ContactID = @ContactID";
                ExecuteScalar(query, new
                {
                    ContactID = contactID
                });
            }
            catch (Exception)
            {
                throw;
            }
        }
        public string GetCourseName(/*Guid studentID*/ Guid courseID)
        {
            try
            {
                //string query = "SELECT c.FullName FROM dbo.Students s INNER JOIN dbo.StudentEnquiry e ON s.StudentID=e.StudentID INNER JOIN dbo.Courses c ON c.CourseID=e.CourseID WHERE e.StudentID=@StudentID";
                //string query = "SELECT FullName FROM dbo.Courses Where CourseID= @CourseID";
                string query = "SELECT ShortName FROM dbo.Courses Where CourseID= @CourseID";
                string result = (string)ExecuteScalar(query, new
                {
                    //StudentID = studentID
                    CourseID = courseID
                });
                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public ExamTransAndType GetExamTransID(Guid certificateAndMarksheetID)
        {
            try
            {
                string query = "SELECT ExamIndentTransID,PrintType FROM dbo.CertificateAndMarksheet WHERE CertificateAndMarksheetID = @CertificateAndMarksheetID";
                return Get< ExamTransAndType> (query,_sqlI18N, new
                {
                    CertificateAndMarksheetID = certificateAndMarksheetID
                });
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Guid GetStudentCertificateIndentInfo(Guid examTransID)
        {
            try
            {
                string query = "SELECT StudentBatchMapping FROM dbo.ExamIndentTrans WHERE ExamIndentTransID = @ExamIndentTransID";
                return (Guid)ExecuteScalar(query, new
                {
                    ExamIndentTransID = examTransID
                });
            }
            catch (Exception)
            {

                throw;
            }
        }
        public StudentCourseID GetStudentCourseID(Guid studentBatchMappingID)
        {
            try
            {
                string query = "SELECT c.ShortName,c.FullName,a.StudentID,d.ContactID  FROM dbo.StudentBatchMapping a INNER JOIN dbo.CourseBatch b ON b.CourseBatchID = a.CourseBatchID INNER JOIN dbo.Courses c ON c.CourseID = b.CourseID INNER JOIN dbo.Students d ON d.StudentID = a.StudentID WHERE a.BatchMappingID =@BatchMappingID";
                return Get<StudentCourseID>(query,_sqlI18N, new
                {
                    BatchMappingID = studentBatchMappingID
                });
            }
            catch (Exception)
            {
                throw;
            }
        }
        public StudentNamePhoneDetail GetStudentDetail(Guid contactID)
        {
            try
            {
                string query = "SELECT ContactName AS StudentName,Phone as PhoneNumber FROM dbo.Contact WHERE ContactID=@ContactID";
                return Get<StudentNamePhoneDetail>(query, _sqlI18N, new
                {
                    ContactID = contactID
                });
            }
            catch (Exception)
            {
                throw;
            }
        }
        public CourseSubject GetSubjectCourseName(IssueMaterial material)
        {
            try
            {
                return Get<CourseSubject>("uspGetSubjectCourseDetail",_sqlI18N, new
                {
                    MaterialID = material.MaterialID,
                    BatchMappingID = material.BatchMappingID
                }, commandType: CommandType.StoredProcedure);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool CreateStudentAddress(StudentAddress contactAddress, Guid actionByGuid)
        {
            return ExecuteScalar("uspContactAddressUpdation", new
            {
                ContactAddressID = contactAddress.ContactAddressID,
                ContactID = contactAddress.ContactID,
                AddressType = contactAddress.AddressType,
                Address1 = contactAddress.Address1,
                Address2 = contactAddress.Address2,
                Area = contactAddress.Area,
                Country = contactAddress.Country,
                State = contactAddress.State,
                District = contactAddress.District,
                Pincode = contactAddress.Pincode,
                Taluk = contactAddress.Taluk,
                Latitude = contactAddress.Latitude,
                Longitude = contactAddress.Longitude,
                Createdby = actionByGuid
            }, commandType: System.Data.CommandType.StoredProcedure).ToBoolean();
        }
        public List<SubjectList> GetSubjectsName(Guid batchMappingID)
        {
            try
            {
                return GetList<SubjectList>("SELECT SubjectName FROM dbo.tfnGetSubjectNames(@BatchMappingID)", _sqlI18N, new
                {
                    BatchMappingID = batchMappingID
                });
            }
            catch (Exception)
            {

                throw;
            }
        }
        public GetStudentCourseName GetContactIDCourse(Guid batchMappingID)
        {
            try
            {
                string query = "SELECT  s.ContactID,s.RollNumber,b.FullName FROM dbo.StudentBatchMapping sb INNER JOIN dbo.Students s ON s.StudentID = sb.StudentID INNER JOIN dbo.CourseBatch a ON a.CourseBatchID = sb.CourseBatchID INNER JOIN dbo.Courses b ON b.CourseID = a.CourseID WHERE sb.BatchMappingID = @BatchMappingID AND s.IsDeleted = 0";
                return Get<GetStudentCourseName>(query,_sqlI18N, new
                {
                    BatchMappingID = batchMappingID
                });
            }
            catch (Exception)
            {

                throw;
            }
        }
        public List<StudentCourseDetails> GetCompletedStudents(Guid tenantBranchID)
        {
            try
            {
                string query = "SELECT * FROM dbo.tfnGetCompletedStudentList(@TenantBranchID)";
                return GetList<StudentCourseDetails>(query, _sqlI18N, new
                {
                    TenantBranchID = tenantBranchID
                });
            }
            catch (SqlException)
            {
                throw;
            }
        }
        public List<StudentMarkReprint> GetStudentMarkForReprint(DataTable ids)
        {
            try
            {
                string query = "uspGetMarkListForReprint";
                return GetList<StudentMarkReprint>(query, _sqlI18N, new
                {
                    BatchMappingIDs = ids
                }, commandType: CommandType.StoredProcedure);
            }
            catch (SqlException)
            {
                throw;
            }
        }
        public List<GetStudentIDCard> GetStudentIDCardList(DataTable batchMapingIds)
        {
            try
            {
                string query = "uspGetStudentIDCardList";
                return GetList<GetStudentIDCard>(query, _sqlI18N, new
                {
                    BatchMapingIds = batchMapingIds
                }, commandType: CommandType.StoredProcedure);
            }
            catch (SqlException)
            {
                throw;
            }
        }   
        #endregion
    }
}