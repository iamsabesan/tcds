using Dawn.Database;
using Dawn.Utilities.Extensions;
using Dawn.Utilities.i18N;
using EntitiesDTO.Admin;
using EntitiesDTO.Course;
using EntitiesDTO.Subject;
using Microsoft.Data.SqlClient;
using System.Data;

namespace iCloud.Academic.Repository
{
    public class CourseRepository : DawnDbClient
    {
        #region Fields
        private readonly SqlI18N _sqlI18N;
        #endregion
        #region Constructor
        public CourseRepository(SqlI18N SqlI18N, string ConnectionString) : base(ConnectionString)
        {
            _sqlI18N = SqlI18N;
        }
        #endregion
        #region Methods
        /// <summary>
        /// Get course type list
        /// </summary>
        /// <param createdBy="sakthivel"></param>
        /// <param createdDate="18-04-2023"></param>
        /// <returns></returns>
        public IEnumerable<CourseTypeList> GetCourseTypeList()
        {
            List<CourseTypeList> courseTypes = new List<CourseTypeList>();
            try
            {
                string query = "SELECT CourseTypeID,CourseType FROM dbo.tfnGetCourseTypeList()";
                courseTypes = GetList<CourseTypeList>(query, _sqlI18N);
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            return courseTypes;
        }

        /// <summary>
        /// Create new course
        /// </summary>
        /// <param name="course"></param>
        /// <param name="subjectMap"></param>
        /// <param name="actionBy"></param>
        /// <param createdBy="sakthivel"></param>
        /// <param createdDate="18-04-2023"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public Guid CreateCourse(CreateNewCourse course, DataTable subjectMap, Guid actionBy)
        {
            Guid result = Guid.Empty;
            try
            {
                result = (Guid)ExecuteScalar("uspCreateCourse", new
                {
                    ShortName = course.ShortName,
                    FullName = course.FullName,
                    CertificateName = course.CertificateName,
                    CourseType = course.CourseType,
                    Duration = course.Duration,
                    DurationType = course.DurationType,
                    TotalSemester = course.TotalSemester,
                    CourseFees = course.CourseFees,
                    ExamFees = course.ExamFees,
                    AdmissionFees = course.AdmissionFees,
                    CreatedBy = actionBy,
                    SubjectMapping = subjectMap,
                    Eligibility = course.Eligibility
                }, commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Check course name exists or not
        /// </summary>
        /// <param name="fullName"></param>
        /// <param createdBy="sakthivel"></param>
        /// <param createdDate="18-04-2023"></param>
        /// <returns></returns>
        public bool CheckCourseExists(string fullName)
        {
            bool result;
            string query;
            try
            {
                query = "SELECT dbo.sfnCheckCourseExists(@FullName)";
                result = ExecuteScalar(query, new { FullName = fullName }).ToBoolean();
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Get all course list
        /// </summary>
        /// <param createdBy="sakthivel"></param>
        /// <param createdDate="18-04-2023"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public DataSet GetCourseList()
        {
            DataSet courseLists = new DataSet();
            try
            {
                courseLists = GetDataSet("uspGetCourseList", commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            return courseLists;
        }


        /// <summary>
        /// Get course basic info list
        /// </summary>
        /// <param createdBy="sakthivel"></param>
        /// <param createdDate="18-04-2023"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<CourseInfo> GetCourseBasicInfo()
        {
            List<CourseInfo> courseInfos = new List<CourseInfo>();
            try
            {
                string query = "SELECT CourseID,CourseName,ShortName,CourseType,Duration,Semester FROM dbo.tfnGetCourseBasicInfoList()";
                courseInfos = GetList<CourseInfo>(query, _sqlI18N);
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            return courseInfos;
        }

        /// <summary>
        /// Get course by id
        /// </summary>
        /// <param name="courseID"></param>
        /// <param createdBy="sakthivel"></param>
        /// <param createdDate="18-04-2023"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public CourseByID GetCourseByID(Guid courseID)
        {
            CourseByID course = new CourseByID();
            try
            {
                string query = "SELECT CourseID,ShortName,CourseName,Duration,CourseType,CourseFees,AdmissionFees FROM dbo.tfnGetCourseByID(@CourseID)";
                course = Get<CourseByID>(query, _sqlI18N, new
                {
                    CourseID = courseID
                });
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            return course;
        }

        /// <summary>
        /// Get course info list
        /// </summary>
        /// <param createdBy="sakthivel"></param>
        /// <param createdDate="19-04-2023"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public IEnumerable<CourseBaseInfo> GetCourseInfoList()
        {
            IEnumerable<CourseBaseInfo> courseInfos = new List<CourseBaseInfo>();
            try
            {
                string query = "SELECT CourseID,CourseName,Duration,ShortName FROM dbo.tfnGetCourseInfoList()";
                courseInfos = GetList<CourseBaseInfo>(query, _sqlI18N);
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            return courseInfos;
        }

        /// <summary>
        /// Mapping coures to tenant branch
        /// </summary>
        /// <param name="courseMapping"></param>
        /// <param name="courses"></param>
        /// <param name="actionBy"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int CreateCourseMapping(CourseMapping courseMapping, DataTable courses, Guid actionBy)
        {
            int result = 0;
            try
            {
                result = ExecuteScalar("uspCreateCourseMapping", new
                {
                    TenantBranchID = courseMapping.TenantBranchID,
                    CreatedBy = actionBy,
                    CourseMapping = courses
                }, commandType: System.Data.CommandType.StoredProcedure).ToInt();
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Get course mapping list
        /// </summary>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<GetCourseMappingList> GetCourseMappingList()
        {
            List<GetCourseMappingList> mappingLists = new List<GetCourseMappingList>();
            string query = "SELECT CourseID,TenantBranchID,CourseName,CourseType,Duration,Semester,ShortName FROM dbo.tfnGetCourseMappingList()";
            try
            {
                mappingLists = GetList<GetCourseMappingList>(query, _sqlI18N);
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            return mappingLists;
        }

        /// <summary>
        /// Get course list by franchisee
        /// </summary>
        /// <param name="tenantBranchID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public DataSet GetCourseListByFranchisee(Guid tenantBranchID, Guid? courseTypeID)
        {
            try
            {
                return GetDataSet("uspGetCourseListByFranchisee", new
                {
                    TenantBranchID = tenantBranchID,
                    CourseTypeID = courseTypeID
                }, commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Get total semester of course
        /// </summary>
        /// <param name="batchMappingID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int GetCourseSemesterCount(Guid batchMappingID)
        {
            int semester = 0;
            string query = "SELECT dbo.sfnGetTotalSemester(@BatchMappingID)";
            try
            {
                semester = ExecuteScalar(query, new { BatchMappingID = batchMappingID }).ToInt();
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            return semester;
        }

        /// <summary>
        /// Get course summary by admin
        /// </summary>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public CourseSummaryByAdmin GetCourseSummaryByAdmin()
        {
            CourseSummaryByAdmin course = new CourseSummaryByAdmin();
            string query = "SELECT CourseTotal,PrintingCertificate,PrintingMarksheet,DespatchCertificate,DespatchMarksheet FROM dbo.tfnGetCourseSummaryByAdmin()";
            try
            {
                course = Get<CourseSummaryByAdmin>(query, _sqlI18N);
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            return course;
        }

        /// <summary>
        /// Get course summary by franchisee
        /// </summary>
        /// <param name="tenantBranchID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public CourseSummaryByFranchisee GetCourseSummaryByFranchisee(Guid tenantBranchID)
        {
            CourseSummaryByFranchisee course = new CourseSummaryByFranchisee();
            string query = "SELECT Courses,AlumnusStudent,CurrentStudent,DiscontinuedStudent,CompletedBatch,ActiveBatch,CertficatePending,MarksheetPending FROM dbo.tfnGetCourseSummaryByFranchisee(@TenantBranchID)";
            try
            {
                course = Get<CourseSummaryByFranchisee>(query, _sqlI18N, new { TenantBranchID = tenantBranchID });
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            return course;
        }

        public List<MblCourse> GetMblCourse(Guid tenantBranchID)
        {
            List<MblCourse> courses;
            try
            {
                courses = GetList<MblCourse>("SELECT CourseID,CourseTypeName,Course,Duration,CourseFees,TotalSemesters,ShortName,Eligibility  FROM dbo.tfnGetMblCourse(@TenantBranchID) ORDER BY Course ASC", _sqlI18N, new { TenantBranchID = tenantBranchID });
                return courses;
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public FranchiseeMblDashboard GetFranchiseeMblDash(Guid tenantBranchID)
        {
            try
            {
                return Get<FranchiseeMblDashboard>("uspGetFranchiseeMblDashboard", _sqlI18N, new { TenantBranchID = tenantBranchID }, commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Guid EditCourse(EditCourse course, Guid actionBy)
        {
            Guid result = Guid.Empty;
            try
            {
                result = (Guid)ExecuteScalar("uspEditCourse", new
                {
                    CourseID = course.CourseID,
                    ShortName = course.ShortName,
                    FullName = course.FullName,
                    CertificateName = course.CertificateName,
                    CourseType = course.CourseType,
                    Duration = course.Duration,
                    DurationType = course.DurationType,
                    TotalSemester = course.TotalSemester,
                    CourseFees = course.CourseFees,
                    ExamFees = course.ExamFees,
                    AdmissionFees = course.AdmissionFees,
                    CreatedBy = actionBy,
                    Eligibility = course.Eligibility
                }, commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        public List<SubjectsOfCourse> SubjectsOfCourse(Guid courseID)
        {
            return GetList<SubjectsOfCourse>("SELECT cs.Semester,s.SubjectName,s.SubjectID,cs.TheoryMarks,cs.PracticalMarks,cs.TotalMarks FROM dbo.CourseSubjectMapping cs INNER JOIN dbo.Subjects s ON s.SubjectID = cs.SubjectID WHERE cs.CourseID = @CourseID ORDER BY cs.SlNo, cs.Semester", _sqlI18N, new { CourseID = courseID });
        }
        #endregion
    }
}
