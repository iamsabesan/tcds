using Dawn.Frameworks.Entities;
using Dawn.Frameworks.Helper;
using Dawn.Utilities.Enums;
using Dawn.Utilities.Exceptions;
using Dawn.Utilities.i18N;
using EntitiesDTO.Users;
using Frameworks.Entities;
using Frameworks.Helper;
using iCloud.Tenant.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace iCloud.Tenant.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{v:apiVersion}/icloud-branch/")]
    [ApiController]
    public class BranchV1Controller : ControllerBase
    {
        #region Fields
        private readonly SqlI18N _sqlI18N;
        private readonly IdentityInfo _userIdentityInfo;
        private readonly BranchService _branchService;
        #endregion
        #region Constructor
        public BranchV1Controller(SqlI18N sqlI18N, IHttpContextAccessor accessor, IDawnHttpClient dawnHttpClient)
        {
            _sqlI18N = sqlI18N;
            _userIdentityInfo = accessor.GetUserIdentityInfoDetails();
            _branchService = new BranchService(sqlI18N, accessor, dawnHttpClient);
        }
        #endregion
        #region Methods
        [HttpGet("active-franchisee-contact")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public IEnumerable<ActiveFranchiseeContact> ActiveFranchiseeContact()
        {
            IEnumerable<ActiveFranchiseeContact> getBranchContacts = new List<ActiveFranchiseeContact>();
            try
            {
                getBranchContacts = _branchService.ActiveFranchiseeContact();
            }
            catch (DawnException ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return getBranchContacts;
        }

        [HttpGet("branch-name")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public GetBranchName GetBranchName(string tenantBranchID)
        {
            GetBranchName getBranchName= new GetBranchName();
            try
            {
                getBranchName = _branchService.GetBranchName(tenantBranchID);
            }
            catch (DawnException ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return getBranchName;
        }

        [HttpGet("tenant-branch-contact")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public DataSet GetTenantBranchAndConatct()
        {
            DataSet result;
            try
            {
                 result = _branchService.GetTenantBranchAndContact();
            }
            catch (DawnException ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return result;
        }


        [HttpGet("branch-contact-by-id")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public DataSet GetTenantBranchAndConatctByID(Guid tenantBranchID)
        {
            DataSet result;
            try
            {
                 result = _branchService.GetTenantBranchAndConatctByID(tenantBranchID);
            }
            catch (DawnException ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return result;
        }
        #endregion
    }
}
