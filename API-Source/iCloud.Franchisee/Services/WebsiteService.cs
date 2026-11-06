using CommonServices;
using Dawn.Utilities.Extensions;
using Dawn.Utilities.i18N;
using EntitiesDTO.Admin;
using EntitiesDTO.Course;
using EntitiesDTO.Utilities;
using Frameworks.Helper;
using System.Data;

namespace iCloud.Franchisee.Services
{
    public class WebsiteService
    {
        private readonly SqlI18N _sqlI18N;
        private readonly IDawnHttpClient _dawnHttpClient;
        private readonly IHttpClientFactory _httpClientFactory;
        public WebsiteService(SqlI18N sqlI18N, IDawnHttpClient dawnHttpClient, IHttpClientFactory httpClientFactory)
        {
            _sqlI18N = sqlI18N;
            _httpClientFactory = httpClientFactory;
            _dawnHttpClient = dawnHttpClient;           
        }
        #region Methods
        public async Task<List<FranchiseDetails>> FranchiseDetails(string district, Guid tenantId)
        {
            string conString = await GetConnectionString(tenantId, "Franchisee");
            var commonRepository = new CommonRepository(_sqlI18N, conString);
            var franchiseDetails = commonRepository.FranchiseDetails(district);
            return franchiseDetails ;
        }
        public async Task<List<CourseList>> FranchiseCourseDetails(Guid tenantBranchID, Guid tenantId)
        {
            List<CourseList> courses = new List<CourseList>();
            List<CourseSubjectMapList> subjects = new List<CourseSubjectMapList>();
            string conString = await GetConnectionString(tenantId, "Academic");
            var commonRepository = new CommonRepository(_sqlI18N, conString);
            //var courseDetails = commonRepository.GetCourseListForWebsite(tenantBranchID);
            DataSet courseDataset = commonRepository.GetCourseListForWebsite(tenantBranchID);
            courses = courseDataset.Tables[0].DataTablesToList<CourseList>();
            //subjects = courseDataset.Tables[1].DataTablesToList<CourseSubjectMapList>();
            foreach(var result in courses)
            {
                CourseSubjectMapList subject = new CourseSubjectMapList();
                result.SubjectListDetails = courseDataset.Tables[1].DataTablesToList<CourseSubjectMapList>().Where(c => c.CourseID == result.CourseID).ToList();
            }
            return courses;
        }
        #region Get Connection String
        private async Task<string> GetConnectionString(Guid tenantId, string serviceName)
        {
            TenantServiceInfo tenantServiceInfo = new()
            {
                ServiceName = serviceName,
                TenantID = tenantId
            };
            dynamic dbAdminAPIResult = await _dawnHttpClient.PostAsync("dbadmin/get-tenant-constr-on-reg", tenantServiceInfo, false);
            string constring = dbAdminAPIResult.Code == 200 ? dbAdminAPIResult.Data.ToString() : "";
            return constring.ToDecrypt();
        }
        #endregion
        #endregion
    }
}
