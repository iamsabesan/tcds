using CommonServices;
using Dawn.Frameworks.Helper;
using Dawn.Utilities.Extensions;
using Dawn.Utilities.i18N;
using EntitiesDTO.Admin;
using EntitiesDTO.Batch;
using Frameworks.Entities;
using Frameworks.Helper;
using iCloud.Admission.Repository;
using System.Data;
using Utilities.Extensions;

namespace iCloud.Admission.Services {
    public class BatchService {
        #region Fields
        private readonly SqlI18N _sqlI18N;
        private readonly IdentityInfo _userIdentityInfo;
        private string _connectionString;
        private readonly BatchRepository _batchRepository;
        private readonly CommonService _commonService;
        private readonly IDawnHttpClient _dawnHttpClient;
        #endregion
        #region Constructor
        public BatchService (SqlI18N sqlI18N, IHttpContextAccessor accessor, IDawnHttpClient dawnHttpClient) {
            _sqlI18N = sqlI18N;
            _userIdentityInfo = accessor.GetUserIdentityInfoDetails();
            _connectionString = accessor.GetConnectionString(dawnHttpClient);
            _commonService = new CommonService(dawnHttpClient);
            _batchRepository = new BatchRepository(_sqlI18N, _connectionString);
            _dawnHttpClient = dawnHttpClient;
        }
        #endregion
        #region Methods
        /// <summary>
        /// Create new batch
        /// </summary>
        /// <param createdBy="sakthivel"></param>
        /// <param createdDate="19-04-2023"></param>
        /// <param name="batch"></param>
        /// <returns></returns>
        public Guid CreateNewBatch (CreateBatch batch) {
            return _batchRepository.CreateNewBatch(batch, _userIdentityInfo.TenantBranchID, _userIdentityInfo.ContactID);
        }

        /// <summary>
        /// Get batch list
        /// </summary>
        /// <param createdBy="sakthivel"></param>
        /// <param createdDate="19-04-2023"></param>
        /// <returns></returns>
        public async Task<IEnumerable<NewBatchList>> GetBatchList () {
            List<NewBatchList> batchLists = new List<NewBatchList>();
            var batch = _batchRepository.GetBatchList(_userIdentityInfo.TenantBranchID);
            var course = await _commonService.GetCourseInfoList();
            var faculty = await _commonService.GetTenantUserList();
            //var batchIds = batch.Select(b => b.CourseBatchID).Distinct().ToList();
            var result = (from b in batch
                          join c in course on b.CourseID equals c.CourseID
                          join f in faculty on b.FacultyID equals f.ContactID
                          select new NewBatchList() {
                              CourseBatchID = b.CourseBatchID,
                              BatchNumber = b.BatchNumber,
                              CourseName = c.CourseName,
                              Duration = c.Duration,
                              FacultyID = b.FacultyID,
                              FacultyName = f.ContactName,
                              BatchType = b.BatchType,
                              Timing = b.Timing,
                              BatchTrack = b.BatchTrack,
                              StartDate = b.StartDate,
                              EndDate = b.EndDate,
                              IsActive = b.IsActive,
                              ShortName = c.ShortName
                          }).ToList();
            return result;
        }

        /// <summary>
        /// Get batch by id
        /// </summary>
        /// <param createdBy="sakthivel"></param>
        /// <param createdDate="19-04-2023"></param>
        /// <param name="courseBatchID"></param>
        /// <returns></returns>
        public async Task<BatchByID> GetBatchByID (Guid courseBatchID) {
            var batch = _batchRepository.GetBatchByID(courseBatchID);
            BatchByID byID = batch.Mapper<BatchByID>();
            var facultyList = await _commonService.GetTenantUserList();
            var faculty = facultyList.Where(x => x.ContactID == batch.FacultyID).FirstOrDefault();
            byID.FacultyName = faculty != null ? faculty.ContactName : "";
            return byID;
        }

        /// <summary>
        /// Get batch basic info list
        /// </summary>
        /// <param createdBy="sakthivel"></param>
        /// <param createdDate="19-04-2023"></param>
        /// <returns></returns>
        public async Task<IEnumerable<NewBatchInfo>> GetBatchBasicInfo()
        {
            var batch = _batchRepository.GetBatchBasicInfo(_userIdentityInfo.TenantBranchID);
            var course = await _commonService.GetCourseBasicInfo();
            var faculty = await _commonService.GetTenantUserList();
            var result = (from b in batch
                          join c in course on b.CourseID equals c.CourseID
                          join f in faculty on b.FacultyID equals f.ContactID
                          select new NewBatchInfo()
                          {
                              CourseBatchID = b.CourseBatchID,
                              BatchNumber = b.BatchNumber,
                              CourseName = c.CourseName,
                              FacultyID = b.FacultyID,
                              FacultyName = f.ContactName,
                              Timing = b.Timing,
                              BatchType = b.BatchType,
                              ShortName = c.ShortName
                          }).ToList();
            return result;
        }
        public async Task<IEnumerable<NewBatchInfo>> GetBatchBasicInfoV1()
        {
            var batch = _batchRepository.GetBatchBasicInfo(_userIdentityInfo.TenantBranchID);
            var faculty = await _commonService.GetTenantUserList();

            /*Get student batchmapingIDs from active Exam indent*/
            string _connectionString = await GetConnectionString(_userIdentityInfo.TenantID, "Inventory");
            using BatchRepository batchRepository = new BatchRepository(_sqlI18N, _connectionString);
            List<BatchMappingID> studentBatchMappingID = batchRepository.GetBatchMappingIDs();

            DataTable ids = studentBatchMappingID.ToDataTable();
            var course = _batchRepository.GetCourseBasicInfo(ids);

            var result = (from b in batch
                          join c in course on b.CourseID equals c.CourseID
                          join f in faculty on b.FacultyID equals f.ContactID
                          select new NewBatchInfo()
                          {
                              CourseBatchID = b.CourseBatchID,
                              BatchNumber = b.BatchNumber,
                              CourseName = c.CourseName,
                              FacultyID = b.FacultyID,
                              FacultyName = f.ContactName,
                              Timing = b.Timing,
                              BatchType = b.BatchType,
                              ShortName = c.ShortName,
                              CourseTypeName = c.CourseType
                          }).ToList();
            return result;
        }
        /// <summary>
        /// Get batch list by course id
        /// </summary>
        /// <param name="courseID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        public async Task<List<BatchByCourseID>> GetBatchByCourseID (Guid courseID) {
            List<BatchByCourseID> batches = new List<BatchByCourseID>();
            var batchs = _batchRepository.GetBatchByCourseID(courseID, _userIdentityInfo.TenantBranchID);
            foreach (var item in batchs) {
                BatchByCourseID batch = item.Mapper<BatchByCourseID>();
                var facultyList = await _commonService.GetTenantUserList();
                var faculty = facultyList.Where(x => x.ContactID == batch.FacultyID).FirstOrDefault();
                batch.FacultyName = faculty != null ? faculty.ContactName : "";
                batches.Add(batch);
            }
            return batches;
        }

        public async Task<List<BatchByCourseID>> GetActiveBatchByCourseID (Guid courseID) {
            List<BatchByCourseID> batches = new List<BatchByCourseID>();
            var batchs = _batchRepository.GetActiveBatchByCourseID(courseID, _userIdentityInfo.TenantBranchID);
            foreach (var item in batchs) {
                BatchByCourseID batch = item.Mapper<BatchByCourseID>();
                var facultyList = await _commonService.GetTenantUserList();
                var faculty = facultyList.Where(x => x.ContactID == batch.FacultyID).FirstOrDefault();
                batch.FacultyName = faculty != null ? faculty.ContactName : "";
                batches.Add(batch);
            }
            return batches;
        }

        public async Task<List<TenantUserList>> GetFacultyList () {
            List<TenantUserList> tenantUserLists = new List<TenantUserList>();
            var employees = await _commonService.GetTenantUserList();
            foreach (var employee in employees) {
                if (employee.TenantBranchID == _userIdentityInfo.TenantBranchID) {
                    TenantUserList list = employee.Mapper<TenantUserList>();
                    tenantUserLists.Add(list);
                }
            }
            return tenantUserLists;
        }
        #region GetConnection String
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
        #endregion

    }
}
