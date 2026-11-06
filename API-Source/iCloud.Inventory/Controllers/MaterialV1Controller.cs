using Dawn.Frameworks.Entities;
using Dawn.Frameworks.Helper;
using Dawn.Utilities.Enums;
using Dawn.Utilities.Exceptions;
using Dawn.Utilities.i18N;
using EntitiesDTO.Material;
using Frameworks.Helper;
using iCloud.Inventory.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace iCloud.Inventory.Controllers {
    [ApiVersion("1.0")]
    [Route("api/v{v:apiVersion}/material/")]
    [ApiController]
    public class MaterialV1Controller : ControllerBase {
        #region Fields
        private readonly MaterialService _materialService;
        #endregion
        #region Constructor
        public MaterialV1Controller (SqlI18N sqlI18N, IHttpContextAccessor accessor, IDawnHttpClient dawnHttpClient) {
            _materialService = new MaterialService(sqlI18N, accessor, dawnHttpClient);
        }
        #endregion
        #region Methods
        /// <summary>
        /// Create material type
        /// </summary>
        /// <param name="materialType"></param>
        /// <param createdBy="sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpPost("createtype")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public APIResult<Guid> CreateMaterialType (CreateMaterialType materialType) {
            Guid result;
            try {
                result = _materialService.CreateMaterialType(materialType);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return APIResult.Success(result, "Material type created successfully !", DawnStatusCode.Created);
        }

        /// <summary>
        /// Get material type list
        /// </summary>
        /// <param createdBy="sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("type/list")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public IEnumerable<MaterialTypeList> GetMaterialTypeList () {
            IEnumerable<MaterialTypeList> result;
            try {
                result = _materialService.GetMaterialTypeList();
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return result;
        }

        /// <summary>
        /// Create material
        /// </summary>
        /// <param name="material"></param>
        /// <param createdBy="sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpPost("create")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public APIResult<Guid> CreateMaterial (CreateMaterial material) {
            Guid result;
            try {
                result = _materialService.CreateMaterial(material);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return APIResult.Success(result, "Material created successfully !", DawnStatusCode.Created);
        }


        /// <summary>
        /// Get material list
        /// </summary>
        /// <param createdBy="sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("list")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public List<MaterialList> GetMaterialList (Guid materialTypeID) {
            List<MaterialList> result;
            try {
                result = _materialService.GetMaterialList(materialTypeID);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return result;
        }

        /// <summary>
        /// Get all material list
        /// </summary>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("alllist")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public IEnumerable<MaterialNames> GetAllMaterialList () {
            IEnumerable<MaterialNames> result;
            try {
                result = _materialService.GetAllMaterialList();
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return result;
        }

        /// <summary>
        /// Get material types with materials
        /// </summary>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("types/materials")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public List<MaterialTypeWithMaterial> GetMaterialTypeWithMaterial () {
            List<MaterialTypeWithMaterial> typeWithMaterials;
            try {
                typeWithMaterials = _materialService.GetMaterialTypeWithMaterial();
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return typeWithMaterials;
        }

        [HttpPost("edit")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public APIResult<string> EditMaterial (EditMaterial material) {
            try {
                return APIResult.Success("", _materialService.EditMaterial(material), DawnStatusCode.Created);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }
        #endregion
    }
}
