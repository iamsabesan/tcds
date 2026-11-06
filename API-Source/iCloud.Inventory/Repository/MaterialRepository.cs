using Dawn.Database;
using Dawn.Utilities.Extensions;
using Dawn.Utilities.i18N;
using EntitiesDTO.Material;
using Microsoft.Data.SqlClient;

namespace iCloud.Inventory.Repository
{
    public class MaterialRepository : DawnDbClient
    {
        #region Fields
        private readonly SqlI18N _sqlI18N;
        #endregion
        #region Constructor
        public MaterialRepository(SqlI18N SqlI18N, string ConnectionString) : base(ConnectionString)
        {
            _sqlI18N = SqlI18N;
        }
        #endregion
        #region Methods
        /// <summary>
        /// Create material type
        /// </summary>
        /// <param name="materialType"></param>
        /// <param name="actionBy"></param>
        /// <param createdBy="sakthivel"></param>
        /// <returns></returns>
        public Guid CreateMaterialType(CreateMaterialType materialType, Guid actionBy)
        {
            Guid result = Guid.Empty;
            try
            {
                result = (Guid)ExecuteScalar("uspCreateMaterialType", new
                {
                    MaterialType = materialType.MaterialType,
                    CreatedBy = actionBy
                }, commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Check material type exists or not
        /// </summary>
        /// <param name="materialType"></param>
        /// <param createdBy="sakthivel"></param>
        /// <returns></returns>
        public bool CheckMaterialTypeExist(string materialType)
        {
            string query;
            bool result;
            try
            {
                query = "SELECT dbo.sfnMaterialTypeExist(@MaterialType)";
                result = ExecuteScalar(query, new { MaterialType = materialType }).ToBoolean();
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Get material type list
        /// </summary>
        /// <param createdBy="sakthivel"></param>
        /// <returns></returns>
        public IEnumerable<MaterialTypeList> GetMaterialTypeList()
        {
            IEnumerable<MaterialTypeList> materialTypes = new List<MaterialTypeList>();
            string query;
            try
            {
                query = "SELECT MaterialTypeID,MaterialType FROM dbo.tfnGetMaterialTypeList() ORDER BY CASE WHEN MaterialType= 'Others' THEN 1 ELSE 0 END, MaterialType ASC";
                materialTypes = GetList<MaterialTypeList>(query, _sqlI18N);
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            return materialTypes;
        }

        /// <summary>
        /// Create material
        /// </summary>
        /// <param name="material"></param>
        /// <param name="actionBy"></param>
        /// <param createdBy="sakthivel"></param>
        /// <returns></returns>
        public Guid CreateMaterial(CreateMaterial material, Guid actionBy)
        {
            Guid result = Guid.Empty;
            try
            {
                result = (Guid)ExecuteScalar("uspCreateMaterial", new
                {
                    MaterialTypeID = material.MaterialTypeID,
                    MaterialName = material.MaterialName,
                    Price = material.Price,
                    MinimumStock = material.MinimumStock,
                    OpeningStock = material.OpeningStock,
                    SGST = material.SGST,
                    CGST = material.CGST,
                    CreatedBy = actionBy
                }, commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Check material exists
        /// </summary>
        /// <param name="materialName"></param>
        /// <param createdBy="sakthivel"></param>
        /// <returns></returns>
        public bool CheckMaterialExist(string materialName)
        {
            bool result;
            string query;
            try
            {
                query = "SELECT dbo.sfnMaterialExist(@MaterialName)";
                result = ExecuteScalar(query, new { MaterialName = materialName }).ToBoolean();
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Get material list
        /// </summary>
        /// <param createdBy="sakthivel"></param>
        /// <returns></returns>
        public List<MaterialList> GetMaterialList(Guid materialTypeID)
        {
            List<MaterialList> materials = new List<MaterialList>();
            string query;
            try
            {
                //query = "SELECT MaterialID,MaterialName,Price,MinimumStock,OpeningStock,CGST,SGST FROM dbo.Material WHERE MaterialTypeID = @MaterialTypeID AND IsDeleted = 0 ORDER BY MaterialName ASC";
                query = "SELECT m.MaterialID,m.MaterialName,m.Price,m.MinimumStock,m.OpeningStock,m.CGST,m.SGST,tfn.CurrentStock FROM dbo.Material m " +
                        "JOIN dbo.tfnGetCurrentStockList() tfn ON tfn.MaterialID = m.MaterialID WHERE m.IsDeleted = 0 AND m.MaterialTypeID=@MaterialTypeID ORDER BY m.MaterialName ASC";
                materials = GetList<MaterialList>(query, _sqlI18N, new
                {
                    MaterialTypeID = materialTypeID
                });
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            return materials;
        }

        /// <summary>
        /// Get all material list
        /// </summary>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public IEnumerable<MaterialNames> GetAllMaterialList()
        {
            IEnumerable<MaterialNames> materials = new List<MaterialNames>();
            string query;
            try
            {
                query = "SELECT MaterialTypeID,MaterialType,MaterialID,MaterialName,Price,GST FROM dbo.tfnGetAllMaterialList() ORDER BY MaterialName ASC";
                materials = GetList<MaterialNames>(query, _sqlI18N);
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            return materials;
        }

        public int EditMaterial(EditMaterial material, Guid actionBy)
        {
            int result = 0;
            try
            {
                result = ExecuteScalar("uspEditMaterial", new
                {
                    MaterialID = material.MaterialID,
                    MaterialTypeID = material.MaterialTypeID,
                    MaterialName = material.MaterialName,
                    Price = material.Price,
                    MinimumStock = material.MinimumStock,
                    OpeningStock = material.OpeningStock,
                    SGST = material.SGST,
                    CGST = material.CGST,
                    CreatedBy = actionBy
                }, commandType: System.Data.CommandType.StoredProcedure).ToInt();
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }
        #endregion
    }
}
