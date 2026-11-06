using Dawn.Database;
using Dawn.Utilities.Extensions;
using Dawn.Utilities.i18N;
using EntitiesDTO.Admin;
using EntitiesDTO.Users;
using System.Data;

namespace iCloud.Tenant.Repository
{
    public class BranchRepository: DawnDbClient
    {
        #region Fields
        private readonly SqlI18N _sqlI18N;
        #endregion
        #region Constructor
        public BranchRepository(SqlI18N SqlI18N, string ConnectionString) : base(ConnectionString)
        {
            _sqlI18N = SqlI18N;
        }
        #endregion
        #region Methods
        public IEnumerable<ActiveFranchiseeContact> ActiveFranchiseeContact()
        {
            IEnumerable<ActiveFranchiseeContact> franchiseeContacts = new List<ActiveFranchiseeContact>();
            string query = "SELECT TenantBranchName as BranchName,TenantBranchID,ContactID,ContactName,Phone,AlternatePhone,Email,Area,District,State FROM dbo.tfnBranchContact()";
            franchiseeContacts = GetList<ActiveFranchiseeContact>(query, _sqlI18N);
            return franchiseeContacts;
        }

        public GetBranchName GetBranchName(string tenantBranchID)
        {
            string query = "SELECT TenantBranchID,TenantBranchName as BranchName FROM dbo.tfnGetBranchName(@TenantBranchID)";
            var result = Get<GetBranchName>(query, _sqlI18N, new { TenantBranchID = tenantBranchID });
            return result;
        }

        public DataSet GetTenantBranchAndContact()
        {
            return GetDataSet("uspGetContactAndBranch", commandType: CommandType.StoredProcedure);
        }

        public DataSet GetTenantBranchAndConatctByID(Guid tenantBranchID)
        {
            return GetDataSet("uspGetContactAndBranchByID", new
            {
                TenantBranchID = tenantBranchID
            }, commandType: CommandType.StoredProcedure);
        }

        public bool CheckTenantBranchIsExists(Guid tenantBranchID)
        {
            return ExecuteScalar("SELECT dbo.sfnTenantBranchIsExist(@TenantBranchID)", new
            {
                TenantBranchID = tenantBranchID
            }).ToBoolean();
        }
        #endregion
    }
}
