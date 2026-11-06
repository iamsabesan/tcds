using Dawn.Frameworks.Entities;
using Dawn.Frameworks.Helper;
using Dawn.Utilities.Enums;
using Dawn.Utilities.Exceptions;
using Dawn.Utilities.i18N;
using EntitiesDTO.Admin;
using EntitiesDTO.Course;
using Frameworks.Helper;
using iCloud.Academic.Services;
using Microsoft.AspNetCore.Mvc;

namespace iCloud.Academic.Controllers {
    [ApiVersion("1.0")]
    [Route("api/v{v:apiVersion}/course/")]
    [ApiController]
    public class CourseV1Controller : ControllerBase {
        #region Fields
        private readonly CourseService _courseService;
        #endregion
        #region Constructor
        public CourseV1Controller (SqlI18N sqlI18N, IHttpContextAccessor accessor, IDawnHttpClient dawnHttpClient) {
            _courseService = new CourseService(sqlI18N, accessor, dawnHttpClient);
        }
        #endregion
        #region Methods
        /// <summary>
        /// Get course type list
        /// </summary>
        /// <param createdBy="sakthivel"></param>
        /// <param createdDate="18-04-2023"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("type/list")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<IEnumerable<CourseTypeList>> GetCourseTypeList () {
            IEnumerable<CourseTypeList> result;
            try {
                result = await _courseService.GetCourseTypeList();
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return result;
        }

        /// <summary>
        /// Create new subject
        /// </summary>
        /// <param name="newCourse"></param>
        /// <param createdBy="sakthivel"></param>
        /// <param createdDate="18-04-2023"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpPost("create")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public APIResult<Guid> CreateCourse (CreateNewCourse newCourse) {
            Guid result;
            try {
                result = _courseService.CreateCourse(newCourse);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return APIResult.Success(result, "Course created successfully !", DawnStatusCode.Created);
        }

        /// <summary>
        /// Get course list
        /// </summary>
        /// <param createdBy="sakthivel"></param>
        /// <param createdDate="18-04-2023"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("list")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public IEnumerable<GetCourseList> GetCourseList () {
            IEnumerable<GetCourseList> getCourseLists;
            try {
                getCourseLists = _courseService.GetCourseList();
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return getCourseLists;
        }

        /// <summary>
        /// Get course basic info list
        /// </summary>
        /// <param createdBy="sakthivel"></param>
        /// <param createdDate="18-04-2023"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("list/basicinfo")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public IEnumerable<CourseInfo> GetCourseBasicInfo () {
            IEnumerable<CourseInfo> result;
            try {
                result = _courseService.GetCourseBasicInfo();
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return result;
        }


        /// <summary>
        /// Get course by id
        /// </summary>
        /// <param name="courseID"></param>
        /// <param createdBy="sakthivel"></param>
        /// <param createdDate="18-04-2023"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("getbyid")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public CourseByID GetCourseByID (Guid courseID) {
            CourseByID course;
            try {
                course = _courseService.GetCourseByID(courseID);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return course;
        }

        /// <summary>
        /// Get course info list
        /// </summary>
        /// <param createdBy="sakthivel"></param>
        /// <param createdDate="19-04-2023"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("list/info")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public IEnumerable<CourseBaseInfo> GetCourseInfoList () {
            IEnumerable<CourseBaseInfo> result;
            try {
                result = _courseService.GetCourseInfoList();
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return result;
        }

        /// <summary>
        /// Mapping coures to tenant branch
        /// </summary>
        /// <param name="courseMapping"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpPost("mapping")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public APIResult<string> CreateCourseMapping (CourseMapping courseMapping) {
            string result;
            try {
                result = _courseService.CreateCourseMapping(courseMapping);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return APIResult.Success("", result, DawnStatusCode.Created);
        }

        /// <summary>
        /// Get course mapping list
        /// </summary>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("mapping/list")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<IEnumerable<CourseMappingList>> GetCourseMappingList () {
            IEnumerable<CourseMappingList> result;
            try {
                result = await _courseService.GetCourseMappingList();
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return result;
        }

        /// <summary>
        /// Get course list by franchisee
        /// </summary>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("list/byfranchisee")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public List<GetCourseList> GetCourseListByFranchisee (Guid? courseTypeID) {
            List<GetCourseList> getCourseLists;
            try {
                getCourseLists = _courseService.GetCourseListByFranchisee(courseTypeID);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return getCourseLists;
        }

        /// <summary>
        /// Get course info list by franchisee
        /// </summary>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("info/list/byfranchisee")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public List<CourseList> CourseInfoListByFranchisee (Guid? courseTypeID) {
            List<CourseList> courses;
            try {
                courses = _courseService.GetCourseBasicInfoListByFranchisee(courseTypeID);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return courses;
        }

        /// <summary>
        /// Get total semester of course
        /// </summary>
        /// <param name="batchMappingID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("semester/count/{batchMappingID}")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public int GetCourseSemesterCount (Guid batchMappingID) {
            int semester;
            try {
                semester = _courseService.GetCourseSemesterCount(batchMappingID);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return semester;
        }

        /// <summary>
        /// Get course summary by admin
        /// </summary>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("summary/byadmin")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public CourseSummaryByAdmin GetCourseSummaryByAdmin () {
            CourseSummaryByAdmin course;
            try {
                course = _courseService.GetCourseSummaryByAdmin();
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return course;
        }

        /// <summary>
        /// Get course summary by franchisee
        /// </summary>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("summary/byfranchisee")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public CourseSummaryByFranchisee GetCourseSummaryByFranchisee () {
            CourseSummaryByFranchisee course;
            try {
                course = _courseService.GetCourseSummaryByFranchisee();
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return course;
        }

        [HttpGet("mobile-courses/{tenantBranchID}")]
        [DawnAuthorize("TenantAdmin", "TenantUser", "Guest")]
        public List<MblCourse> GetMblCourses (Guid tenantBranchID) {
            List<MblCourse> courses;
            try {
                courses = _courseService.GetMblCourseList(tenantBranchID);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return courses;
        }

        [HttpGet("mobile/dashboard")]
        [DawnAuthorize("TenantUser")]
        public async Task<FranchiseeMblDashboard> GetFranchiseeDash () {
            FranchiseeMblDashboard franchisee;
            try {
                franchisee = await _courseService.GetFranchiseeMblDash();
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return franchisee;
        }

        [HttpPost("edit")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public APIResult<Guid> EditCourse (EditCourse newCourse) {
            Guid result;
            try {
                result = _courseService.EditCourse(newCourse);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return APIResult.Success(result, "Course updated successfully !", DawnStatusCode.Created);
        }
        #endregion
    }
}
