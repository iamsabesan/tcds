using CommonServices;
using Dawn.Frameworks.Helper;
using Dawn.Utilities.i18N;
using EntitiesDTO.News;
using Frameworks.Entities;
using Frameworks.Helper;
using News.Repository;

namespace News.Services
{
    public class NewsService
    {
        #region Fields
        private readonly SqlI18N _sqlI18N;
        private readonly IdentityInfo _userIdentityInfo;
        private string _connectionString;
        private readonly NewsRepository _newsRepository;
        private readonly CommonService _commonService;

        #endregion
        #region Constructor
        public NewsService(SqlI18N sqlI18N, IHttpContextAccessor accessor, IDawnHttpClient dawnHttpClient)
        {
            _sqlI18N = sqlI18N;
            _userIdentityInfo = accessor.GetUserIdentityInfoDetails();
            _connectionString = accessor.GetConnectionString(dawnHttpClient);
            _commonService = new CommonService(dawnHttpClient);
            _newsRepository = new NewsRepository(_sqlI18N, _connectionString);
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
        public Guid CreateNews(CreateNews news) {
            return _newsRepository.CreateNews(news, _userIdentityInfo.ContactID);
        }

        /// <summary>
        /// Edit news
        /// </summary>
        /// <param name="news"></param>
        /// <param CreatedBy="Sakthivel"></param>
        /// <param CreatedDate="16-06-2023"></param>
        /// <returns></returns>
        public string EditNews(EditNews news) {
            int result = _newsRepository.EditNews(news, _userIdentityInfo.ContactID);
            return result > 0 ? "News edited successfully !" : "Couldn't update news";
        }

        /// <summary>
        /// Get news list
        /// </summary>
        /// <param CreatedBy="Sakthivel"></param>
        /// <param CreatedDate="16-06-2023"></param>
        /// <returns></returns>
        public List<GetNewsList> GetNewsList() {
            return _newsRepository.GetNewsList();
        }

        /// <summary>
        /// Get news by franchisee
        /// </summary>
        /// <param CreatedBy="Sakthivel"></param>
        /// <param CreatedDate="16-06-2023"></param>
        /// <returns></returns>
        public NewsByFranchisee GetNewsByFranchisee() {
            return _newsRepository.GetNewsByFrancisee();
        }
        #endregion
    }
}
