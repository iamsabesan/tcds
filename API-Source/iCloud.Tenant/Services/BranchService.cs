using CommonServices;
using Dawn.Frameworks.Helper;
using Dawn.Utilities.i18N;
using EntitiesDTO.Users;
using Frameworks.Entities;
using Frameworks.Helper;
using iCloud.Tenant.Repository;
using System.Data;

namespace iCloud.Tenant.Services
{
    public class BranchService
    {
        #region Fields
        private readonly SqlI18N _sqlI18N;
        private readonly IdentityInfo _userIdentityInfo;
        private string _connectionString;
        private readonly BranchRepository _branchRepository;
        private readonly CommonService _commonService;

        #endregion
        #region Constructor
        public BranchService(SqlI18N sqlI18N, IHttpContextAccessor accessor, IDawnHttpClient dawnHttpClient)
        {
            _sqlI18N = sqlI18N;
            _userIdentityInfo = accessor.GetUserIdentityInfoDetails();
            _connectionString = accessor.GetConnectionString(dawnHttpClient);
            _commonService = new CommonService(dawnHttpClient);
            _branchRepository = new BranchRepository(_sqlI18N, _connectionString);
        }
        #endregion
        #region Methods
        public IEnumerable<ActiveFranchiseeContact> ActiveFranchiseeContact()
        {
            return _branchRepository.ActiveFranchiseeContact();
        }

        public GetBranchName GetBranchName(string tenantBranchID)
        {
            return _branchRepository.GetBranchName(tenantBranchID);
        }

        public DataSet GetTenantBranchAndContact()
        {
            var result =  _branchRepository.GetTenantBranchAndContact();
            return result;
        }

        public DataSet GetTenantBranchAndConatctByID(Guid tenantBranchID)
        {
            if (!_branchRepository.CheckTenantBranchIsExists(tenantBranchID))
                throw new Exception("Invalid branch ID !");
            var result = _branchRepository.GetTenantBranchAndConatctByID(tenantBranchID);
            return result;
        }
        #endregion
    }
}
