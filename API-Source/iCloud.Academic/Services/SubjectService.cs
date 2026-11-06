using CommonServices;
using Dawn.Frameworks.Helper;
using Dawn.Utilities.Exceptions;
using Dawn.Utilities.Extensions;
using Dawn.Utilities.i18N;
using EntitiesDTO.Subject;
using EntitiesDTO.Utilities;
using Frameworks.Entities;
using Frameworks.Helper;
using iCloud.Academic.Repository;
using Newtonsoft.Json;
using System.Data;
using Utilities.Extensions;

namespace iCloud.Academic.Services {
    public class SubjectService {
        #region Fields
        private readonly SqlI18N _sqlI18N;
        private readonly IdentityInfo _userIdentityInfo;
        private string _connectionString;
        private readonly SubjectRepository _subjectRepository;
        private readonly CommonService _commonService;
        #endregion
        #region Constructor
        public SubjectService (SqlI18N sqlI18N, IHttpContextAccessor accessor, IDawnHttpClient dawnHttpClient) {
            _sqlI18N = sqlI18N;
            _userIdentityInfo = accessor.GetUserIdentityInfoDetails();
            _connectionString = accessor.GetConnectionString(dawnHttpClient);
            _commonService = new CommonService(dawnHttpClient);
            _subjectRepository = new SubjectRepository(_sqlI18N, _connectionString);
        }
        #endregion
        #region Methods
        /// <summary>
        /// Create new subject
        /// </summary>
        /// <param name="subject"></param>
        /// <param createdBy="sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public Guid CreateSubject (CreateSubject subject) {
            if (_subjectRepository.CheckSubjectExists(subject.SubjectName))
                throw new Exception("The given subject name is already exists !");
            List<GetMaterial> materials = subject.SubjectMaterial;
            var subjectMaterial = materials.ToDataTable();
            Guid result = _subjectRepository.CreateSubject(subject, subjectMaterial, _userIdentityInfo.ContactID);
            return result;
        }

        /// <summary>
        /// Get subject list
        /// </summary>
        /// <param createdBy="sakthivel"></param>
        /// <returns></returns>
        public async Task<List<GetSubjectList>> GetSubjectList () {
            List<GetSubjectList> getSubjectLists = new List<GetSubjectList>();
            var resultSet = _subjectRepository.GetSubjetsList();
            var materialNameList = await _commonService.GetMaterialNameList();
            List<SubjectResult> subjectResults = resultSet.Tables[0].DataTablesToList<SubjectResult>();
            List<MaterialResult> materialResult = resultSet.Tables[1].DataTablesToList<MaterialResult>();
            foreach (var result in subjectResults) {
                getSubjectLists.Add(new GetSubjectList {
                    SubjectID = result.SubjectID,
                    SubjectName = result.SubjectName,
                    Materials = (from DataRow row in resultSet.Tables[1].Select("SubjectID='" + result.SubjectID.ToString() + "'")
                                 select new Material() {
                                     MaterialID = (Guid)row["MaterialID"],
                                     MaterialName = materialNameList.Where(x => x.MaterialID == (Guid)row["MaterialID"]).Select(y => y.MaterialName).SingleOrDefault()
                                 }).ToList()
                });
            }
            return getSubjectLists;
        }

        /// <summary>
        /// Edit existing subject
        /// </summary>
        /// <param name="subject"></param>
        /// <param createdBy="sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public Guid EditSubject (EditSubject subject) {
            if (!_subjectRepository.CheckSubjectIDExists(subject.SubjectID))
                throw new Exception("The given subjectID doesn't exists");
            List<GetMaterial> materials = subject.SubjectMaterial;
            Model model = new Model();
            model.SubjectMaterial = subject.SubjectMaterial;
            var demo = JsonConvert.SerializeObject(model);
            var subjectMaterial = materials.ToDataTable();
            Guid result = _subjectRepository.EditSubject(subject.SubjectID,subject.SubjectName, demo, _userIdentityInfo.ContactID);
            return result;
        }

        public string AddSubVideoMap (AddSubVideoMap map) {
            if (_subjectRepository.CheckSortOrderExists(map))
                throw new DawnUserException("The sortorder already exists");
            int count = _subjectRepository.AddSubVideoMap(map, _userIdentityInfo.ContactID);
            if (count > 0) {
                return "Video uploaded successfully";
            }
            else {
                throw new DawnUserException("Couldn't upload video");
            }
        }

        public List<GetSubjectVideoList> GetSubjectLists (Guid subjectID) {
            return _subjectRepository.GetSubjectLists(subjectID);
        }

        public string DeleteVideo (DeleteVideo delete) {
            int result = _subjectRepository.DeleteVideo(delete, _userIdentityInfo.ContactID);
            if (result > 0) {
                return "Video deleted";
            }
            else {
                throw new DawnUserException("Couldn't delete");
            }
        }

        public string EditVideo (GetSubjectVideoList edit) {
            AddSubVideoMap map = edit.Mapper<AddSubVideoMap>();
            int sortOrder = _subjectRepository.GetSortOrder(edit.SubVideoMappingID);
            if (_subjectRepository.CheckSortOrderExists(map) && edit.SortOrder != sortOrder)
                throw new DawnUserException("The sortorder already exists");
            int count = _subjectRepository.EditVideo(edit, _userIdentityInfo.ContactID);
            if (count > 0) {
                return "Updated successfully";
            }
            else {
                throw new DawnUserException("Couldn't update");
            }
        }

        public List<GetCourse> GetCourse () {
            return _subjectRepository.GetCourseLists(_userIdentityInfo.ContactID);
        }

        public List<GetSubjectVideo> SubjectTheoryVideo (Guid courseID) {
            return _subjectRepository.GetSubjectVideo(courseID, "Theory");
        }

        public List<GetSubjectVideo> SubjectPracticalVideo (Guid courseID) {
            return _subjectRepository.GetSubjectVideo(courseID, "Practical");
        }
        #endregion
    }
}
