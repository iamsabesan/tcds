using Dawn.Database;
using Dawn.Utilities.Extensions;
using Dawn.Utilities.i18N;
using EntitiesDTO.Admin;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Reflection;

namespace iCloud.Franchisee.Repository
{
    public class SubscriptionRepository : DawnDbClient
    {
        #region Fields
        private readonly SqlI18N _sqlI18N;
        #endregion
        #region Constructor
        public SubscriptionRepository(SqlI18N SqlI18N, string ConnectionString) : base(ConnectionString)
        {
            _sqlI18N = SqlI18N;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Update subscription fees
        /// </summary>
        /// <param name="update"></param>
        /// <param name="actionBy"></param>
        /// <param createdBy="sakthivel"></param>
        /// <returns></returns>
        public int UpdatePackage(List<UpdatePackage> update, Guid actionBy)
        {
            int result;
            try
            {
                result = ExecuteScalar("uspUpdateSubscription", new
                {
                    PackageUpdate = update.ToDataTable(),
                    UpdatedBy = actionBy
                }, commandType: System.Data.CommandType.StoredProcedure).ToInt();
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Approve franchisee request
        /// </summary>
        /// <param name="approve"></param>
        /// <param name="packages"></param>
        /// <param name="actionBy"></param>
        /// <param createdBy="sakthivel"></param>
        /// <returns></returns>
        public int ApproveFranchisee(ApproveFranchisee approve, DataTable packages, Guid actionBy)
        {
            int result = 0;
            try
            {
                result = ExecuteScalar("uspApproveFranchisee", new
                {
                    FranchiseeRequestID = approve.FranchiseeRequestID,
                    Packages = packages,
                    UpdatedBy = actionBy
                }, commandType: System.Data.CommandType.StoredProcedure).ToInt();
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Get franchisee subscription info
        /// </summary>
        /// <param name="tenantBranchID"></param>
        /// <param createdBy="sakthivel"></param>
        /// <returns></returns>
        public SubscriptionInfo GetSubscription(string tenantBranchID)
        {
            SubscriptionInfo subscription = new SubscriptionInfo();
            string query;
            try
            {
                query = "SELECT TenantBranchID,FranchiseeCode,FranchiseeName,ApprovalStartDate,ApprovalEndDate,SubscriptionStartDate,SubscriptionEndDate,Status FROM dbo.tfnGetSubscriptionInfo(@TenantBranchID)";
                subscription = Get<SubscriptionInfo>(query, _sqlI18N, new { TenantBranchID = tenantBranchID });
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            return subscription;
        }

        public int GetPollingInterval()
        {
            return ExecuteScalar("SELECT MinValue FROM dbo.AppConfiguration WHERE ConfigID = 'POLLING_INTERVAL' AND IsDeleted = 0").ToInt();
        }

        /// <summary>
        /// Get subscription plan
        /// </summary>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public IEnumerable<SubscriptionPlan> GetSubscriptionPlan()
        {
            IEnumerable<SubscriptionPlan> subscriptions = new List<SubscriptionPlan>();
            string query = "SELECT PackageID,PackageName,NetPackageFees AS Fees,IsActive FROM dbo.ApprovalAndSubscriptionPackage WHERE IsDeleted = 0";
            try
            {
                subscriptions = GetList<SubscriptionPlan>(query);
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            return subscriptions;
        }


        public List<GetRenewalPlanList> GetRenewalPlan()
        {
            List<GetRenewalPlanList> subscriptions = new List<GetRenewalPlanList>();
            string query = "SELECT PackageID,PackageType,PackageName,NetPackageFees,CONCAT(Duration,' ',Frequency) AS Duration FROM dbo.ApprovalAndSubscriptionPackage WHERE PackageType <> 'REGISTRATION' ORDER BY PackageFees";
            try
            {
                return subscriptions = GetList<GetRenewalPlanList>(query);
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<GetRenewalPlanListNew> GetRenewalPlanList(string tenantBranchID)
        {
            return GetList<GetRenewalPlanListNew>("uspGetRenewalPlanInfo", _sqlI18N, new
            {
                TenantBranchID = tenantBranchID
            }, commandType: CommandType.StoredProcedure);
        }
        #endregion
    }
}