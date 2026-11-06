using Dawn.Frameworks.Entities;
using Dawn.Frameworks.Helper;
using Dawn.Utilities.Enums;
using Dawn.Utilities.Exceptions;
using Dawn.Utilities.i18N;
using EntitiesDTO.Subject;
using Frameworks.Helper;
using iCloud.Academic.Services;
using Microsoft.AspNetCore.Mvc;

namespace iCloud.Academic.Controllers {
    [ApiVersion("1.0")]
    [Route("api/v{v:apiVersion}/subject/")]
    [ApiController]
    public class SubjectV1Controller : ControllerBase {
        #region Fields
        private readonly SubjectService _subjectService;
        #endregion
        #region Constructor
        public SubjectV1Controller (SqlI18N sqlI18N, IHttpContextAccessor accessor, IDawnHttpClient dawnHttpClient) {
            _subjectService = new SubjectService(sqlI18N, accessor, dawnHttpClient);
        }
        #endregion
        #region Methods
        /// <summary>
        /// Create new subject
        /// </summary>
        /// <param name="subject"></param>
        /// <param createdBy="sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpPost("create")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public APIResult<Guid> CreateSubject (CreateSubject subject) {
            Guid result;
            try {
                result = _subjectService.CreateSubject(subject);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return APIResult.Success(result, "Subject Created successfully !", DawnStatusCode.Created);
        }

        /// <summary>
        /// Get subject type list
        /// </summary>
        /// <param createdBy="sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("list")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<IEnumerable<GetSubjectList>> GetMaterialTypeList () {
            IEnumerable<GetSubjectList> result;
            try {
                result = await _subjectService.GetSubjectList();
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return result;
        }

        /// <summary>
        /// Edit subject
        /// </summary>
        /// <param name="subject"></param>
        /// <param createdBy="sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpPost("edit")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public APIResult<Guid> EditSubject (EditSubject subject) {
            Guid result;
            try {
                result = _subjectService.EditSubject(subject);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return APIResult.Success(result, "Subject updated successfully !", DawnStatusCode.Created);
        }

        /// <summary>
        /// Mapping videos to subject
        /// </summary>
        /// Author:Sakthivel B
        /// CreatedDate:04-06-2024
        /// <param name="map"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpPost("video-mapping")]
        [DawnAuthorize("TenantAdmin")]
        public APIResult<string> AddSubVideoMap (AddSubVideoMap map) {
            string result;
            try {
                result = _subjectService.AddSubVideoMap(map);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return APIResult.Success("", result, DawnStatusCode.Created);
        }

        /// <summary>
        /// Get subject mapping video list by subjectID
        /// </summary>
        /// Author:Sakthivel B
        /// CreatedDate:04-06-2024
        /// <param name="subjectID"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("video/list")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public List<GetSubjectVideoList> GetSubjectVideos (Guid subjectID) {
            List<GetSubjectVideoList> videoList;
            try {
                videoList = _subjectService.GetSubjectLists(subjectID);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return videoList;
        }

        /// <summary>
        /// Delete subject mapping video
        /// </summary>
        /// Author:Sakthivel B
        /// CreatedDate:04-06-2024
        /// <param name="delete"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpPost("video/delete")]
        [DawnAuthorize("TenantAdmin")]
        public APIResult<string> DeleteVideo (DeleteVideo delete) {
            string result;
            try {
                result = _subjectService.DeleteVideo(delete);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return APIResult.Success("", result, DawnStatusCode.Created);
        }

        /// <summary>
        /// Edit subject mapping video
        /// </summary>
        /// Author:Sakthivel B
        /// CreatedDate:04-06-2024
        /// <param name="edit"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpPost("video/edit")]
        [DawnAuthorize("TenantAdmin")]
        public APIResult<string> EditVideo (GetSubjectVideoList edit) {
            string result;
            try {
                result = _subjectService.EditVideo(edit);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return APIResult.Success("", result, DawnStatusCode.Created);
        }

        /// <summary>
        /// Get student mapped course list
        /// </summary>
        /// Author:Sakthivel B
        /// CreatedDate:10-06-2024
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("courseinfo")]
        [DawnAuthorize("TenantUser")]
        public List<GetCourse> GetCourse () {
            try {
                return _subjectService.GetCourse();
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }

        /// <summary>
        /// Get student subject theory video by courseID
        /// </summary>
        /// Author:Sakthivel B
        /// CreatedDate:10-06-2024
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("theory/video")]
        [DawnAuthorize("TenantUser")]
        public List<GetSubjectVideo> GetTheoryVideo (Guid courseID) {
            try {
                return _subjectService.SubjectTheoryVideo(courseID);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }

        /// <summary>
        /// Get student subject practical video by courseID
        /// </summary>
        /// Author:Sakthivel B
        /// CreatedDate:10-06-2024
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("practical/video")]
        [DawnAuthorize("TenantUser")]
        public List<GetSubjectVideo> GetPracticalVideo (Guid courseID) {
            try {
                return _subjectService.SubjectPracticalVideo(courseID);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }
        #endregion
    }
}
