using CommonServices;
using Dawn.Frameworks.Helper;
using Dawn.Utilities.Exceptions;
using Dawn.Utilities.Extensions;
using Dawn.Utilities.i18N;
using EntitiesDTO.Admin;
using EntitiesDTO.Course;
using EntitiesDTO.Utilities;
using Frameworks.Entities;
using Frameworks.Helper;
using iCloud.Academic.Repository;
using System.Data;
using Utilities.Extensions;

namespace iCloud.Academic.Services {
    public class CourseService {
        #region Fields
        private readonly SqlI18N _sqlI18N;
        private readonly IdentityInfo _userIdentityInfo;
        private string _connectionString;
        private readonly CourseRepository _courseRepository;
        private readonly CommonService _commonService;
        private readonly IDawnHttpClient _dawnHttpClient;
        #endregion
        #region Constructor
        public CourseService (SqlI18N sqlI18N, IHttpContextAccessor accessor, IDawnHttpClient dawnHttpClient) {
            _sqlI18N = sqlI18N;
            _userIdentityInfo = accessor.GetUserIdentityInfoDetails();
            _connectionString = accessor.GetConnectionString(dawnHttpClient);
            _commonService = new CommonService(dawnHttpClient);
            _courseRepository = new CourseRepository(_sqlI18N, _connectionString);
            _dawnHttpClient = dawnHttpClient;
        }
        #endregion
        #region Methods
        /// <summary>
        /// Get course type list
        /// </summary>
        /// <param createdBy="sakthivel"></param>
        /// <param createdDate="18-04-2023"></param>
        /// <returns></returns>
        public async Task<IEnumerable<CourseTypeList>> GetCourseTypeList () {
            return _courseRepository.GetCourseTypeList();
        }

        /// <summary>
        /// Create new corse
        /// </summary>
        /// <param name="course"></param>
        /// <param createdBy="sakthivel"></param>
        /// <param createdDate="18-04-2023"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public Guid CreateCourse (CreateNewCourse course) {
            if (_courseRepository.CheckCourseExists(course.FullName))
                throw new Exception("The given course name is already exists !");
            CreateNewCourse newCourse = course.Mapper<CreateNewCourse>();
            List<CourseSubjectMap> subjectMaps = course.SubjectMapping;
            var courseSubjects = subjectMaps.ToDataTable();
            Guid result = _courseRepository.CreateCourse(newCourse, courseSubjects, _userIdentityInfo.ContactID);
            return result;
        }

        /// <summary>
        /// Get course list
        /// </summary>
        /// <param createdBy="sakthivel"></param>
        /// <param createdDate="18-04-2023"></param>
        /// <returns></returns>
        public IEnumerable<GetCourseList> GetCourseList () {
            List<GetCourseList> courseLists = new List<GetCourseList>();
            DataSet courses = _courseRepository.GetCourseList();
            var courseList = courses.Tables[0].DataTablesToList<CourseList>();
            var subjectMapping = courses.Tables[1].DataTablesToList<CourseSubjectMapList>();
            foreach (var item in courseList) {
                GetCourseList getCourse = item.Mapper<GetCourseList>();
                getCourse.SubjectMapping = courses.Tables[1].DataTablesToList<CourseSubjectMapList>().Where(x => x.CourseID == item.CourseID).ToList();
                courseLists.Add(getCourse);
            }
            return courseLists;
        }

        /// <summary>
        /// Get course info
        /// </summary>
        /// <param createdBy="sakthivel"></param>
        /// <param createdDate="18-04-2023"></param>
        /// <returns></returns>
        public IEnumerable<CourseInfo> GetCourseBasicInfo () {
            return _courseRepository.GetCourseBasicInfo();
        }

        /// <summary>
        /// Get course by id
        /// </summary>
        /// <param createdBy="sakthivel"></param>
        /// <param createdDate="18-04-2023"></param>
        /// <param name="courseID"></param>
        /// <returns></returns>
        public CourseByID GetCourseByID (Guid courseID) {
            var courseInfo = _courseRepository.GetCourseByID(courseID);
            CourseByID course = courseInfo.Mapper<CourseByID>();
            return course;
        }

        /// <summary>
        /// Get course info list
        /// </summary>
        /// <param createdBy="sakthivel"></param>
        /// <param createdDate="19-04-2023"></param>
        /// <returns></returns>
        public IEnumerable<CourseBaseInfo> GetCourseInfoList () {
            return _courseRepository.GetCourseInfoList();
        }

        /// <summary>
        /// Mapping coures to tenant branch
        /// </summary>
        /// <param name="courseMapping"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        public string CreateCourseMapping (CourseMapping courseMapping) {
            CourseMapping course = new CourseMapping();
            course.TenantBranchID = courseMapping.TenantBranchID;
            DataTable courses = courseMapping.Courses.ToDataTable();
            var courseList = _courseRepository.CreateCourseMapping(courseMapping, courses, _userIdentityInfo.ContactID);
            return courseList > 0 ? "Course Mapping Successfull !" : "Could not mapping course !";
        }

        /// <summary>
        /// Get course mapping list
        /// </summary>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        //public async Task<IEnumerable<CourseMappingList>> GetCourseMappingList()
        //{
        //    List<CourseMappingList> list = new List<CourseMappingList>();
        //    var course = _courseRepository.GetCourseMappingList();
        //    var franchisee = await _commonService.GetFranchiseeID();
        //    //var branchIds = franchisee.Select(b => b.TenantBranchID).ToList().ToDataTable();
        //    DataTable branchIds = new DataTable();
        //    branchIds.Columns.Add("TenantBranchID", typeof(Guid));
        //    foreach (var id in franchisee.Select(b => b.TenantBranchID))
        //    {
        //        branchIds.Rows.Add(id);
        //    }
        //    string tenantConn = await GetConnectionString(_userIdentityInfo.TenantID, "TenantBranch");
        //    var commonRepository = new CommonRepository(_sqlI18N, tenantConn);
        //    var branchDetails = commonRepository.GetBranchDetail(branchIds);
        //    foreach (var item in franchisee)
        //    {
        //        //var branch = await _commonService.GetBranchDetails(item.TenantBranchID);
        //        var branch = branchDetails.FirstOrDefault(x => x.TenantBranchID == item.TenantBranchID);
        //        if (branch != null)
        //        {
        //            CourseMappingList mappingList = branch.Mapper<CourseMappingList>();
        //            mappingList.FranchiseeName = branch.BranchName;
        //            mappingList.FranchiseeID = item.FranchiseeCode;
        //            mappingList.Courses = course.Where(x => x.TenantBranchID == branch.TenantBranchID).ToList();
        //            list.Add(mappingList);
        //        }
        //    }
        //    return list;
        //}
        public async Task<IEnumerable<CourseMappingList>> GetCourseMappingList()
        {
            var allCourses = _courseRepository.GetCourseMappingList();
            var franchisees = await _commonService.GetFranchiseeID();
            if (franchisees == null || !franchisees.Any())
            {
                return new List<CourseMappingList>();
            }
            DataTable branchIdsTable = new DataTable();
            branchIdsTable.Columns.Add("TenantBranchID", typeof(Guid));
            foreach (var item in franchisees)
            {
                branchIdsTable.Rows.Add(item.TenantBranchID);
            }
            string franchiseeConn = await GetConnectionString(_userIdentityInfo.TenantID, "Franchisee");
            var commonRepo = new CommonRepository(_sqlI18N, franchiseeConn);
            var branchTalukList = commonRepo.GetFranchiseeCode(branchIdsTable);
            string tenantConn = await GetConnectionString(_userIdentityInfo.TenantID, "TenantBranch");
            using (var commonRepository = new CommonRepository(_sqlI18N, tenantConn))
            {
                var branchDetailsList = commonRepository.GetBranchDetail(branchIdsTable);
                // Create a Dictionary for Branches (Instant ID lookup)
                // Key: TenantBranchID, Value: Branch Object
                var branchDict = branchDetailsList.ToDictionary(b => b.TenantBranchID);
                // Create a Lookup for Courses (Instant List retrieval)
                // Key: TenantBranchID, Value: List of Courses
                var coursesLookup = allCourses.ToLookup(c => c.TenantBranchID);
                var list = new List<CourseMappingList>(franchisees.Count());
                foreach (var item in franchisees)
                {
                    // Use TryGetValue (O(1) complexity) instead of FirstOrDefault (O(N) complexity)
                    if (branchDict.TryGetValue(item.TenantBranchID, out var branch))
                    {
                        var mappingList = branch.Mapper<CourseMappingList>();
                        mappingList.FranchiseeName = branch.BranchName;
                        mappingList.FranchiseeID = item.FranchiseeCode;
                        mappingList.Courses = coursesLookup[item.TenantBranchID].ToList();
                        mappingList.Taluk = branchTalukList.FirstOrDefault(x => x.TenantBranchID == item.TenantBranchID) ?.Taluk ?? string.Empty;
                        list.Add(mappingList);
                    }
                }
                return list;
            }
        }
        /// <summary>
        /// Get course list by franchisee
        /// </summary>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        public List<GetCourseList> GetCourseListByFranchisee (Guid? courseTypeID) {
            List<GetCourseList> getCourseLists = new List<GetCourseList>();
            DataSet courseDataSet = _courseRepository.GetCourseListByFranchisee(_userIdentityInfo.TenantBranchID, courseTypeID);
            var courses = courseDataSet.Tables[0].DataTablesToList<CourseList>();
            foreach (var course in courses) {
                GetCourseList courseList = course.Mapper<GetCourseList>();
                courseList.SubjectMapping = courseDataSet.Tables[1].DataTablesToList<CourseSubjectMapList>().Where(x => x.CourseID == course.CourseID).ToList();
                getCourseLists.Add(courseList);
            }
            return getCourseLists;
        }

        /// <summary>
        /// Get course basic info list by franchisee
        /// </summary>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        public List<CourseList> GetCourseBasicInfoListByFranchisee (Guid? courseTypeID) {
            DataSet courseDataSet = _courseRepository.GetCourseListByFranchisee(_userIdentityInfo.TenantBranchID, courseTypeID);
            List<CourseList> courseLists = courseDataSet.Tables[0].DataTablesToList<CourseList>();
            return courseLists;
        }

        /// <summary>
        /// Get total semester of course
        /// </summary>
        /// <param name="batchMappingID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        public int GetCourseSemesterCount (Guid batchMappingID) {
            return _courseRepository.GetCourseSemesterCount(batchMappingID);
        }

        /// <summary>
        /// Get course summary by admin
        /// </summary>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        public CourseSummaryByAdmin GetCourseSummaryByAdmin () {
            return _courseRepository.GetCourseSummaryByAdmin();
        }

        /// <summary>
        /// Get course summary by franchisee
        /// </summary>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        public CourseSummaryByFranchisee GetCourseSummaryByFranchisee () {
            return _courseRepository.GetCourseSummaryByFranchisee(_userIdentityInfo.TenantBranchID);
        }

        public List<MblCourse> GetMblCourseList (Guid tenantBranchID) {
            List<MblCourse> mblCourses = new List<MblCourse>();
            var courses = _courseRepository.GetMblCourse(tenantBranchID);
            foreach(var course in courses) {
                course.Subjects = _courseRepository.SubjectsOfCourse(course.CourseID);
                mblCourses.Add(course);
            }
            return mblCourses;
        }

        public async Task<FranchiseeMblDashboard> GetFranchiseeMblDash () {
            var courseInfo = _courseRepository.GetFranchiseeMblDash(_userIdentityInfo.TenantBranchID);
            FranchiseeMblDashboard dashboard = courseInfo.Mapper<FranchiseeMblDashboard>();
            TimeZoneInfo indianTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            DateTime indianTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, indianTimeZone);
            dashboard.Date = indianTime.ToString("dd-MM-yyyy hh:mm:ss tt");
            dashboard.WalletBalance = await _commonService.GetWalletBalance(_userIdentityInfo.TenantBranchID);
            var rankStatus = await _commonService.GetFranchiseeRankStatus();
            dashboard.DistrictLvlRank = rankStatus.DistrictLvlRank;
            dashboard.DistrictLvlOutOff = rankStatus.DistrictLvlOutOff;
            dashboard.StateLvlRank = rankStatus.StateLvlRank;
            dashboard.StateLvlOutOff = rankStatus.StateLvlOutOff;
            dashboard.DistrictLvlAchievers = rankStatus.DistrictLvlAchievers;
            dashboard.StateLvlAchievers = rankStatus.StateLvlAchievers;
            return dashboard;
        }

        public Guid EditCourse (EditCourse course) {
            Guid result = _courseRepository.EditCourse(course, _userIdentityInfo.ContactID);
            if (result == Guid.Empty)
                throw new DawnUserException("Updation failed.");
            return result;
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
        #endregion
    }
}
