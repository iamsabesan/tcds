using CommonServices;
using Dawn.Frameworks.Helper;
using Dawn.Utilities.Exceptions;
using Dawn.Utilities.i18N;
using EntitiesDTO.Material;
using Frameworks.Entities;
using Frameworks.Helper;
using iCloud.Inventory.Repository;

namespace iCloud.Inventory.Services {
    public class MaterialService {
        #region Fields
        private readonly SqlI18N _sqlI18N;
        private readonly IdentityInfo _userIdentityInfo;
        private string _connectionString;
        private readonly MaterialRepository _materialRepository;
        private readonly CommonService _commonService;
        #endregion
        #region Constructor
        public MaterialService (SqlI18N sqlI18N, IHttpContextAccessor accessor, IDawnHttpClient dawnHttpClient) {
            _sqlI18N = sqlI18N;
            _userIdentityInfo = accessor.GetUserIdentityInfoDetails();
            _connectionString = accessor.GetConnectionString(dawnHttpClient);
            _commonService = new CommonService(dawnHttpClient);
            _materialRepository = new MaterialRepository(_sqlI18N, _connectionString);
        }
        #endregion
        #region Methods
        /// <summary>
        /// Create material type list
        /// </summary>
        /// <param name="materialType"></param>
        /// <param createdBy="sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public Guid CreateMaterialType (CreateMaterialType materialType) {
            if (_materialRepository.CheckMaterialTypeExist(materialType.MaterialType))
                throw new DawnUserException("The material type already exists");
            Guid result = _materialRepository.CreateMaterialType(materialType, _userIdentityInfo.ContactID);
            return result;
            //return result > 0 ? "Material type created successfully !" : "Could not create material type !";
        }

        /// <summary>
        /// Get material type list
        /// </summary>
        /// <param createdBy="sakthivel"></param>
        /// <returns></returns>
        public IEnumerable<MaterialTypeList> GetMaterialTypeList () {
            return _materialRepository.GetMaterialTypeList();
        }

        /// <summary>
        /// Create material
        /// </summary>
        /// <param name="material"></param>
        /// <param createdBy="sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public Guid CreateMaterial (CreateMaterial material) {
            if (_materialRepository.CheckMaterialExist(material.MaterialName))
                throw new DawnUserException("The given material name already exists");
            Guid result = _materialRepository.CreateMaterial(material, _userIdentityInfo.ContactID);
            return result;
        }

        /// <summary>
        /// Get material list
        /// </summary>
        /// <param createdBy="sakthivel"></param>
        /// <returns></returns>
        public List<MaterialList> GetMaterialList (Guid materialTypeID) {
            return _materialRepository.GetMaterialList(materialTypeID);
        }

        /// <summary>
        /// Get all material list
        /// </summary>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        public IEnumerable<MaterialNames> GetAllMaterialList () {
            return _materialRepository.GetAllMaterialList();
        }

        /// <summary>
        /// Get material types with material
        /// </summary>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        public List<MaterialTypeWithMaterial> GetMaterialTypeWithMaterial () {
            var materialTypes = _materialRepository.GetMaterialTypeList();
            var material = _materialRepository.GetAllMaterialList();
            return materialTypes
                .Select(types => new MaterialTypeWithMaterial {
                    MaterialTypeID = types.MaterialTypeID,
                    MaterialType = types.MaterialType,
                    Materials = material
                        .Where(x => x.MaterialTypeID == types.MaterialTypeID)
                        .OrderBy(x => x.MaterialName)
                        .ToList()
                }).OrderBy(types => types.MaterialType)
                .ToList();
        }



        public string EditMaterial (EditMaterial material) {
            int result = _materialRepository.EditMaterial(material, _userIdentityInfo.ContactID);
            return result > 0 ? "Material updated successfullly." : throw new DawnUserException("Updation failed");
        }
        #endregion
    }
}
