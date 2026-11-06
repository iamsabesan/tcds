using Dawn.Database;
using Dawn.Utilities.Extensions;
using Dawn.Utilities.i18N;
using EntitiesDTO.Subject;
using Microsoft.Data.SqlClient;
using System.Data;

namespace iCloud.Academic.Repository {
    public class SubjectRepository : DawnDbClient {
        #region Fields
        private readonly SqlI18N _sqlI18N;
        #endregion
        #region Constructor
        public SubjectRepository (SqlI18N SqlI18N, string ConnectionString) : base(ConnectionString) {
            _sqlI18N = SqlI18N;
        }
        #endregion
        #region Methods
        /// <summary>
        /// Create new subject
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="material"></param>
        /// <param name="actionBy"></param>
        /// <param createdBy="sakthivel"></param>
        /// <returns></returns>
        public Guid CreateSubject (CreateSubject subject, DataTable material, Guid actionBy) {
            Guid result = Guid.Empty;
            try {
                result = (Guid)ExecuteScalar("uspCreateSubject", new {
                    SubjectName = subject.SubjectName,
                    SubjectMaterial = material,
                    CreatedBy = actionBy
                }, commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Get subject list
        /// </summary>
        /// <param createdBy="sakthivel"></param>
        /// <returns></returns>
        public DataSet GetSubjetsList () {
            try {
                return GetDataSet("uspGetSubjectList", commandType: CommandType.StoredProcedure);
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Edit existing subject
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="material"></param>
        /// <param name="actionBy"></param>
        /// <param createdBy="sakthivel"></param>
        /// <returns></returns>
        public Guid EditSubject (Guid subjectID,string subjectName, string subjects, Guid actionBy) {
            Guid result;
            try {
                result = (Guid)ExecuteScalar("uspEditSubject", new {
                    SubjectID = subjectID,
                    SubjectMaterial = subjects,
                    UpdatedBy = actionBy,
                    SubjectName = subjectName
                }, commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Check subject id is existing or not
        /// </summary>
        /// <param name="subjectID"></param>
        /// <param createdBy="sakthivel"></param>
        /// <returns></returns>
        public bool CheckSubjectIDExists (Guid subjectID) {
            string query;
            bool result;
            try {
                query = "SELECT dbo.sfnSubjectIDExist(@SubjectID)";
                result = ExecuteScalar(query, new { SubjectID = subjectID }).ToBoolean();
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Check subject exists or not
        /// </summary>
        /// <param name="subjectName"></param>
        /// <param createdBy="sakthivel"></param>
        /// <returns></returns>
        public bool CheckSubjectExists (string subjectName) {
            string query;
            bool result;
            try {
                query = "SELECT dbo.sfnSubjectExist(@SubjectName)";
                result = ExecuteScalar(query, new { SubjectName = subjectName }).ToBoolean();
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return result;
        }

        public int AddSubVideoMap (AddSubVideoMap map, Guid actionBy) {
            int result = 0;
            try {
                result = ExecuteScalar("uspSubVideoMap", new {
                    SubjectID = map.SubjectID,
                    Title = map.Title,
                    VideoType = map.VideoType,
                    ContentUrl = map.ContentUrl,
                    SortOrder = map.SortOrder,
                    CreatedBy = actionBy
                }, commandType: System.Data.CommandType.StoredProcedure).ToInt();
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return result;
        }

        public bool CheckSortOrderExists (AddSubVideoMap map) {
            bool result = false;
            result = ExecuteScalar("SELECT COUNT(*) FROM dbo.SubjectVideoMapping WHERE SubjectID = @SubjectID AND VideoType = @VideoType AND SortOrder = @SortOrder AND IsDeleted = 0", new {
                SubjectID = map.SubjectID,
                VideoType = map.VideoType,
                SortOrder = map.SortOrder
            }).ToBoolean();
            return result;
        }

        public List<GetSubjectVideoList> GetSubjectLists (Guid subjectID) {
            List<GetSubjectVideoList> getSubjects = new List<GetSubjectVideoList>();
            string query = "SELECT SubVideoMappingID,SubjectID,Title,VideoType,ContentUrl,SortOrder FROM dbo.SubjectVideoMapping WHERE SubjectID = @SubjectID AND IsDeleted = 0 ORDER BY SortOrder ASC";
            try {
                getSubjects = GetList<GetSubjectVideoList>(query, _sqlI18N, new { SubjectID = subjectID });
                return getSubjects;
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
        }

        public int DeleteVideo (DeleteVideo delete, Guid actionBy) {
            int result = 0;
            try {
                result = ExecuteScalar("uspDeleteVideo", new { SubVideoMappingID = delete.SubVideoMappingID, UpdatedBy = actionBy }, commandType: System.Data.CommandType.StoredProcedure).ToInt();
                return result;
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
        }

        public int EditVideo (GetSubjectVideoList edit, Guid actionBy) {
            int result = 0;
            try {
                result = ExecuteScalar("uspEditSubVideoMap", new {
                    SubVideoMappingID = edit.SubVideoMappingID,
                    SubjectID = edit.SubjectID,
                    Title = edit.Title,
                    VideoType = edit.VideoType,
                    ContentUrl = edit.ContentUrl,
                    SortOrder = edit.SortOrder,
                    UpdatedBy = actionBy
                }, commandType: System.Data.CommandType.StoredProcedure).ToInt();
                return result;
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
        }

        public int GetSortOrder (Guid subVideoMappingID) {
            int sortOrder = ExecuteScalar("SELECT SortOrder FROM dbo.SubjectVideoMapping WHERE SubVideoMappingID = @SubVideoMappingID AND IsDeleted = 0", new { SubVideoMappingID = subVideoMappingID }).ToInt();
            return sortOrder;
        }

        public List<GetCourse> GetCourseLists (Guid contactID) {
            List<GetCourse> courses;
            try {
                string query = "SELECT CourseID, ShortName, FullName FROM dbo.tfnGetStudentCourseInfo(@ContactID)";
                courses = GetList<GetCourse>(query, _sqlI18N, new { ContactID = contactID });
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return courses;
        }

        public List<GetSubjectVideo> GetSubjectVideo (Guid courseID, string videoType) {
            List<GetSubjectVideo> subjectVideos;
            try {
                subjectVideos = GetList<GetSubjectVideo>("uspGetSubjectVideo", _sqlI18N, new { CourseID = courseID, VideoType = videoType }, commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return subjectVideos;
        }
        #endregion
    }
}
