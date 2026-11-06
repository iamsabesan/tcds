using Dawn.Database;
using Dawn.Utilities.Extensions;
using Dawn.Utilities.i18N;
using EntitiesDTO.News;
using Microsoft.Data.SqlClient;

namespace News.Repository
{
    public class NewsRepository : DawnDbClient
    {
        #region Fields
        private readonly SqlI18N _sqlI18N;
        #endregion
        #region Constructor
        public NewsRepository(SqlI18N SqlI18N, string ConnectionString) : base(ConnectionString)
        {
            _sqlI18N = SqlI18N;
        }
        #endregion
        #region Methods
        /// <summary>
        /// Create news
        /// </summary>
        /// <param name="news"></param>
        /// <param name="actionBy"></param>
        /// <param CreatedBy="Sakthivel"></param>
        /// <param CreatedDate="16-06-2023"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public Guid CreateNews(CreateNews news, Guid actionBy) {
            Guid result = Guid.Empty;
            try {
                result = (Guid)ExecuteScalar("uspCreateNews", new
                {
                    Title = news.Title,
                    NewsContent = news.NewsContent,
                    ShowOnStartUp = news.ShowOnStartUp,
                    CreatedBy = actionBy
                }, commandType: System.Data.CommandType.StoredProcedure);
            }
            catch(SqlException ex) {
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Edit news
        /// </summary>
        /// <param name="news"></param>
        /// <param name="actionBy"></param>
        /// <param CreatedBy="Sakthivel"></param>
        /// <param CreatedDate="16-06-2023"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int EditNews(EditNews news, Guid actionBy) {
            int result = 0;
            try {
                result = ExecuteScalar("uspEditNews", new
                {
                    NewsListID = news.NewsListID,
                    Title = news.Title,
                    NewsContent = news.NewsContent,
                    ShowOnStartUp = news.ShowOnStartUp,
                    UpdatedBy = actionBy
                }, commandType: System.Data.CommandType.StoredProcedure).ToInt();
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Get news list
        /// </summary>
        /// <param CreatedBy="Sakthivel"></param>
        /// <param CreatedDate="16-06-2023"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<GetNewsList> GetNewsList() {
            List<GetNewsList> newsLists = new List<GetNewsList>();
            string query = "SELECT NewsListID,CreatedDate AS Date,Title,NewsContent,ShowOnStartUp FROM dbo.NewsList WHERE IsDeleted = 0 ORDER BY SlNo DESC";
            try {
                newsLists = GetList<GetNewsList>(query,_sqlI18N);
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return newsLists;
        }

        /// <summary>
        /// Get news by franchisee
        /// </summary>
        /// <param CreatedBy="Sakthivel"></param>
        /// <param CreatedDate="16-06-2023"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public NewsByFranchisee GetNewsByFrancisee() {
            NewsByFranchisee news = new NewsByFranchisee();
            string query = "SELECT NewsListID,Title,NewsContent FROM dbo.tfnGetNews()";
            try {
                news = Get<NewsByFranchisee>(query,_sqlI18N);
            }
            catch(Exception ex) {
                throw new Exception(ex.Message);
            }
            return news;
        }
        #endregion
    }
}
