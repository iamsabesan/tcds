using Dawn.Frameworks.Entities;
using Dawn.Frameworks.Helper;
using Dawn.Utilities.Enums;
using Dawn.Utilities.Exceptions;
using Dawn.Utilities.i18N;
using Entities.Tenant;
using EntitiesDTO.News;
using Frameworks.Entities;
using Frameworks.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using News.Services;

namespace News.Controller
{
    [ApiVersion("1.0")]
    [Route("api/v{v:apiVersion}/news/")]
    [ApiController]
    public class NewsV1Controller : ControllerBase
    {
        #region Fields
        private readonly SqlI18N _sqlI18N;
        private readonly IdentityInfo _userIdentityInfo;
        private readonly NewsService _newsService;
        #endregion
        #region Constructor
        public NewsV1Controller(SqlI18N sqlI18N, IHttpContextAccessor accessor, IDawnHttpClient dawnHttpClient)
        {
            _sqlI18N = sqlI18N;
            _userIdentityInfo = accessor.GetUserIdentityInfoDetails();
            _newsService = new NewsService(sqlI18N, accessor, dawnHttpClient);
        }
        #endregion
        #region Methods
        /// <summary>
        /// Create news
        /// </summary>
        /// <param name="news"></param>
        /// <param CreatedBy="Sakthivel"></param>
        /// <param CreatedDate="16-06-2023"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpPost("create")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public APIResult<Guid> CreateNews(CreateNews news) {
            Guid result;
            try {
                result = _newsService.CreateNews(news);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return APIResult.Success(result, "News created successfully !", DawnStatusCode.Created);
        }

        /// <summary>
        /// Edit news
        /// </summary>
        /// <param name="news"></param>
        /// <param CreatedBy="Sakthivel"></param>
        /// <param CreatedDate="16-06-2023"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpPost("edit")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public APIResult<string> EditNews(EditNews news) {
            string result;
            try {
                result = _newsService.EditNews(news);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return APIResult.Success("", result, DawnStatusCode.Created);
        }

        /// <summary>
        /// Get news list
        /// </summary>
        /// <param CreatedBy="Sakthivel"></param>
        /// <param CreatedDate="16-06-2023"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("list")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public List<GetNewsList> GetNewsList() {
            List<GetNewsList> newsLists = new List<GetNewsList>();
            try {
                newsLists = _newsService.GetNewsList();
            }
            catch(Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return newsLists;
        }

        /// <summary>
        /// Get news by franchisee
        /// </summary>
        /// <param CreatedBy="Sakthivel"></param>
        /// <param CreatedDate="16-06-2023"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("byfranchisee")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public NewsByFranchisee GetNewsByFranchisee() {
            NewsByFranchisee news = new NewsByFranchisee();
            try {
                news = _newsService.GetNewsByFranchisee();
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return news;
        }
        #endregion
    }
}
