using Dawn.Database;
using Dawn.Utilities.i18N;
using EntitiesDTO.Batch;
using EntitiesDTO.Course;
using Microsoft.Data.SqlClient;
using System.Data;

namespace iCloud.Admission.Repository {
    public class BatchRepository : DawnDbClient {
        #region Fields
        private readonly SqlI18N _sqlI18N;
        #endregion
        #region Constructor
        public BatchRepository (SqlI18N SqlI18N, string ConnectionString) : base(ConnectionString) {
            _sqlI18N = SqlI18N;
        }
        #endregion
        #region Methods
        /// <summary>
        /// Create new batch
        /// </summary>
        /// <param name="batch"></param>
        /// <param name="tenantBranchID"></param>
        /// <param name="actionBy"></param>
        /// <param createdBy="sakthivel"></param>
        /// <param createdDate="19-04-2023"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public Guid CreateNewBatch (CreateBatch batch, Guid tenantBranchID, Guid actionBy) {
            Guid result = Guid.Empty;
            try {
                result = (Guid)ExecuteScalar("uspCreateBatch", new {
                    CourseID = batch.CourseID,
                    BatchType = batch.BatchType,
                    FacultyID = batch.FacultyID,
                    StartDate = batch.StartDate,
                    EndDate = batch.EndDate,
                    StartTime = batch.StartTime,
                    EndTime = batch.EndTime,
                    Monday = batch.Monday,
                    Tuesday = batch.Tuesday,
                    Wednesday = batch.Wednesday,
                    Thursday = batch.Thursday,
                    Friday = batch.Friday,
                    Saturday = batch.Saturday,
                    Sunday = batch.Sunday,
                    BatchTrack = batch.BatchTrack,
                    CreatedBy = actionBy,
                    TenantBranchID = tenantBranchID
                }, commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return result;
        }


        /// <summary>
        /// Get batch list
        /// </summary>
        /// <param createdBy="sakthivel"></param>
        /// <param createdDate="19-04-2023"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public IEnumerable<BatchList> GetBatchList (Guid tenantBranchID) {
            IEnumerable<BatchList> batches = new List<BatchList>();
            try {
                string query = "SELECT CourseBatchID,BatchNumber,CourseID,FacultyID,BatchType,Timing,BatchTrack,StartDate,EndDate,IsActive FROM dbo.tfnGetBatchList(@TenantBranchID)";
                batches = GetList<BatchList>(query, _sqlI18N, new { TenantBranchID = tenantBranchID });
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
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
        /// <exception cref="Exception"></exception>
        public BatchByID GetBatchByID (Guid courseBatchID) {
            BatchByID batch = new BatchByID();
            try {
                string query = "SELECT CourseBatchID,BatchNumber,FacultyID,BatchType,StartDate,EndDate,Timing FROM dbo.tfnGetBatchByID(@CourseBatchID)";
                batch = Get<BatchByID>(query, _sqlI18N, new {
                    CourseBatchID = courseBatchID
                });
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return batch;
        }

        /// <summary>
        /// Get batch basic info
        /// </summary>
        /// <param createdBy="sakthivel"></param>
        /// <param createdDate="19-04-2023"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public IEnumerable<BatchInfo> GetBatchBasicInfo (Guid tenantBranchID) {
            IEnumerable<BatchInfo> batches = new List<BatchInfo>();
            try {
                string query = "SELECT CourseBatchID,BatchNumber,CourseID,FacultyID,Timing,BatchType FROM dbo.tfnGetBatchBasicInfo(@TenantBranchID)";
                batches = GetList<BatchInfo>(query, _sqlI18N, new { TenantBranchID = tenantBranchID });
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return batches;
        }

        /// <summary>
        /// Get batch list by course id
        /// </summary>
        /// <param name="courseID"></param>
        /// <param name="tenantBranchID"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<BatchByCourseID> GetBatchByCourseID (Guid courseID, Guid tenantBranchID) {
            string query = "SELECT CourseBatchID,BatchNumber,Timing,FacultyID,BatchType FROM dbo.tfnGetBatchByCourseID(@CourseID,@TenantBranchID)";
            try {
                return GetList<BatchByCourseID>(query, _sqlI18N, new { CourseID = courseID, TenantBranchID = tenantBranchID });
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
        }

        public List<BatchByCourseID> GetActiveBatchByCourseID (Guid courseID, Guid tenantBranchID) {
            string query = "SELECT CourseBatchID,BatchNumber,Timing,FacultyID,BatchType FROM dbo.tfnGetActiveBatchByCourseID(@CourseID,@TenantBranchID)";
            try {
                return GetList<BatchByCourseID>(query, _sqlI18N, new { CourseID = courseID, TenantBranchID = tenantBranchID });
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
        }
        public List<BatchMappingID> GetBatchMappingIDs()
        {
            try
            {
                //string query = "SELECT StudentBatchMapping,ExamIndentTransID FROM dbo.ExamIndentTrans WHERE status ='APPROVED' AND TotalMark IS NULL AND ObtainedMark IS NULL ";
                string query = "SELECT StudentBatchMapping,ExamIndentTransID FROM dbo.ExamIndentTrans WHERE status ='APPROVED' AND ((TotalMark IS NULL AND ObtainedMark IS NULL) OR IsFinalMark = 0) ";
                return GetList<BatchMappingID>(query);
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public List<CourseInfo> GetCourseBasicInfo(DataTable data)
        {
            try
            {
                return GetList<CourseInfo>("uspGetCourseBasicInfo",_sqlI18N,new
                {
                    BatchMapping = data
                },commandType:CommandType.StoredProcedure);
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion
    }
}
