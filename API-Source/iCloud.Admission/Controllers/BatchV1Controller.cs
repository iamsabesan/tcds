using Dawn.Frameworks.Entities;
using Dawn.Frameworks.Helper;
using Dawn.Utilities.Enums;
using Dawn.Utilities.Exceptions;
using Dawn.Utilities.i18N;
using EntitiesDTO.Admin;
using EntitiesDTO.Batch;
using Frameworks.Helper;
using iCloud.Admission.Services;
using Microsoft.AspNetCore.Mvc;

namespace iCloud.Admission.Controllers {
    [ApiVersion("1.0")]
    [Route("api/v{v:apiVersion}/batch/")]
    [ApiController]
    public class BatchV1Controller : ControllerBase {
        #region Fields
        private readonly BatchService _batchService;
        #endregion
        #region Constructor
        public BatchV1Controller (SqlI18N sqlI18N, IHttpContextAccessor accessor, IDawnHttpClient dawnHttpClient) {
            _batchService = new BatchService(sqlI18N, accessor, dawnHttpClient);
        }
        #endregion
        #region Methods
        /// <summary>
        /// Create new batch
        /// </summary>
        /// <param name="batch"></param>
        /// <param createdBy="sakthivel"></param>
        /// <param createdDate="19-04-2023"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpPost("create")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public APIResult<Guid> CreateNewBatch (CreateBatch batch) {
            Guid result;
            try {
                result = _batchService.CreateNewBatch(batch);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return APIResult.Success(result, "Batch created successfully !", DawnStatusCode.Created);
        }

        /// <summary>
        /// Get batch list
        /// </summary>
        /// <param createdBy="sakthivel"></param>
        /// <param createdDate="19-04-2023"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("list")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<IEnumerable<NewBatchList>> GetBatchList () {
            IEnumerable<NewBatchList> batches;
            try {
                batches = await _batchService.GetBatchList();
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.NotExist);
            }
            return batches;
        }

        /// <summary>
        /// Get batch by id
        /// </summary>
        /// <param name="courseBatchID"></param>
        /// <param createdBy="sakthivel"></param>
        /// <param createdDate="19-04-2023"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("getbyid")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<BatchByID> GetBatchByID (Guid courseBatchID) {
            BatchByID batch;
            try {
                batch = await _batchService.GetBatchByID(courseBatchID);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.NotExist);
            }
            return batch;
        }

        /// <summary>
        /// Get batch info list
        /// </summary>
        /// <param createdBy="sakthivel"></param>
        /// <param createdDate="19-04-2023"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("basicinfo")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<IEnumerable<NewBatchInfo>> GetBatchBasicInfo () {
            IEnumerable<NewBatchInfo> batches;
            try {
                batches = await _batchService.GetBatchBasicInfo();
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.NotExist);
            }
            return batches;
        }
        /*New Batch info list for active exam only*/
        [HttpGet("basicinfo/intented")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<IEnumerable<NewBatchInfo>> GetBatchBasicInfoV1()
        {
            IEnumerable<NewBatchInfo> batches;
            try
            {
                batches = await _batchService.GetBatchBasicInfoV1();
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.NotExist);
            }
            return batches;
        }

        /// <summary>
        /// Get batch list by course id
        /// </summary>
        /// <param name="courseID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("bycourseid")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<List<BatchByCourseID>> GetBatchByCourseID (Guid courseID) {
            List<BatchByCourseID> batches;
            try {
                batches = await _batchService.GetBatchByCourseID(courseID);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.NotExist);
            }
            return batches;
        }

        /// <summary>
        /// Get batch list by course id
        /// </summary>
        /// <param name="courseID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("active/list")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<List<BatchByCourseID>> GetActiveBatchByCourseID (Guid courseID) {
            List<BatchByCourseID> batches;
            try {
                batches = await _batchService.GetActiveBatchByCourseID(courseID);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.NotExist);
            }
            return batches;
        }

        [HttpGet("faculty/list")]
        [DawnAuthorize("TenantUser", "TenantAdmin")]
        public async Task<List<TenantUserList>> GetFacultyList () {
            try {
                return await _batchService.GetFacultyList();
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }
        #endregion
    }
}
