using Dawn.Frameworks.Entities;
using Dawn.Frameworks.Helper;
using Dawn.Utilities.Exceptions;
using Dawn.Utilities.i18N;
using EntitiesDTO.Admin;
using EntitiesDTO.Course;
using Frameworks.Helper;
using iCloud.Franchisee.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace iCloud.Franchisee.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{v:apiVersion}/website/")]
    [ApiController]
    public class WebsiteV1Controller : ControllerBase
    {
        #region Fields
        private readonly SqlI18N _sqlI18N;
        private readonly IHttpClientFactory _httpClientFactory;

        private readonly WebsiteService _websiteService;
        #endregion
        #region Constructor
        public WebsiteV1Controller(SqlI18N sqlI18N, IHttpContextAccessor accessor, IDawnHttpClient dawnHttpClient, IHttpClientFactory httpClientFactory)
        {
            _sqlI18N = sqlI18N;
            _httpClientFactory = httpClientFactory;
            _websiteService = new WebsiteService(_sqlI18N, dawnHttpClient, _httpClientFactory);
        }
        #endregion
        #region
        [HttpGet("franchisee")]
        public async Task<APIResult<List<FranchiseDetails>>> FranchiseDetails([FromQuery] string district, [FromQuery] string tenantId)
        {
            try
            {
                Guid originalGuid = Guid.ParseExact(tenantId, "N");
                return APIResult.Success(await _websiteService.FranchiseDetails(district, originalGuid));
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex);
            }
        }
        [HttpGet("course/list")]
        public async Task<APIResult<List<CourseList>>> FranchiseCourseDetails([FromQuery] Guid tenantBranchID, [FromQuery] Guid tenantId)
        {
            try
            {
                //Guid originalGuid = Guid.ParseExact(tenantId, "N");
                return APIResult.Success(await _websiteService.FranchiseCourseDetails(tenantBranchID, tenantId));
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex);
            }
        }

        #endregion
    }
}
