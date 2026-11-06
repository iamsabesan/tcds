using CommonServices;
using Dawn.Frameworks.Entities;
using Dawn.Frameworks.Helper;
using Dawn.Utilities.Exceptions;
using Dawn.Utilities.Extensions;
using Dawn.Utilities.i18N;
using EntitiesDTO.Admin;
using Frameworks.Entities;
using Frameworks.Helper;
using iCloud.Franchisee.Repository;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace iCloud.Franchisee.Services
{
    public class SubscriptionService
    {
        #region Fields
        private readonly SqlI18N _sqlI18N;
        private readonly IdentityInfo _userIdentityInfo;
        private string _connectionString;
        private readonly SubscriptionRepository _subscriptionRepository;
        private readonly FranchiseeRepository _franchiseeRepository;
        private readonly CommonService _commonService;
        private readonly string _privateKey;
        #endregion
        #region Constructor
        public SubscriptionService(SqlI18N sqlI18N, IHttpContextAccessor accessor, IDawnHttpClient dawnHttpClient, IOptions<DawnConfig> dawnConfig)
        {
            _sqlI18N = sqlI18N;
            _userIdentityInfo = accessor.GetUserIdentityInfoDetails();
            _connectionString = accessor.GetConnectionString(dawnHttpClient);
            _subscriptionRepository = new SubscriptionRepository(_sqlI18N, _connectionString);
            _franchiseeRepository = new FranchiseeRepository(_sqlI18N, _connectionString);
            _commonService = new CommonService(dawnHttpClient);
            _privateKey = dawnConfig.Value.Encryption.PrivateKey;
        }
        #endregion
        #region Methods
        /// <summary>
        /// Update subscription fees
        /// </summary>
        /// <param name="updatePackage"></param>
        /// <param createdBy="sakthivel"></param>
        /// <returns></returns>
        public string UpdatePackage(List<UpdatePackage> updatePackage)
        {
            if (updatePackage == null)
            {
                throw new DawnUserException("Invalid data.");
            }
            int result = _subscriptionRepository.UpdatePackage(updatePackage, _userIdentityInfo.ContactID);
            return result > 0 ? "package fees updated successfully!" : throw new DawnUserException("Could not update package fees !");
        }

        /// <summary>
        /// Approve franchisee request
        /// </summary>
        /// <param name="approve"></param>
        /// <param createdBy="sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public string ApproveFranchisee(ApproveFranchisee approve)
        {
            if (!_franchiseeRepository.CheckFranchiseeRequestID(approve.FranchiseeRequestID))
            {
                throw new DawnUserException("Invalid branch request id");
            }
            List<Packages> packages = approve.PackageItems;
            var packageItems = packages.ToDataTable();
            int result = _subscriptionRepository.ApproveFranchisee(approve, packageItems, _userIdentityInfo.ContactID);
            if (result == 0)
                throw new DawnUserException("Couldn't approve franchisee !");
            return "Franchisee approved successfully";
        }

        /// <summary>
        /// Get franchisee subscription info
        /// </summary>
        /// <param createdBy="sakthivel"></param>
        /// <param name="tenantBranchID"></param>
        /// <returns></returns>
        public SubscriptionInfo GetSubscriptionInfo()
        {
            SubscriptionInfo subscriptionInfo = new SubscriptionInfo();
            var subscription = _subscriptionRepository.GetSubscription(_userIdentityInfo.TenantBranchID.ToString());
            DateTime date = DateTime.Now;
            subscriptionInfo.ApprovalStartDate = (subscription.ApprovalStartDate).ToEncrypt(_privateKey);
            subscriptionInfo.ApprovalEndDate = (subscription.ApprovalEndDate).ToEncrypt(_privateKey);
            subscriptionInfo.ApprovalStatus = DateTime.Parse(subscription.ApprovalEndDate) > date ? "ACTIVE" : "EXPIRED";
            subscriptionInfo.SubscriptionStartDate = (subscription.SubscriptionStartDate).ToEncrypt(_privateKey);
            subscriptionInfo.SubscriptionEndDate = (subscription.SubscriptionEndDate).ToEncrypt(_privateKey);
            subscriptionInfo.SubscriptionStatus = DateTime.Parse(subscription.SubscriptionEndDate) > date ? "ACTIVE" : "EXPIRED";
            subscriptionInfo.PollingInterval = _subscriptionRepository.GetPollingInterval();
            return subscriptionInfo;
        }

        /// <summary>
        /// Get subscription plan
        /// </summary>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        public IEnumerable<SubscriptionPlan> GetSubscriptionPlan()
        {
            return _subscriptionRepository.GetSubscriptionPlan();
        }

        public List<GetRenewalPlanList> GetRenewalList()
        {
            return _subscriptionRepository.GetRenewalPlan();
        }

        public List<GetRenewalPlanListNew> GetRenewalPlanList()
        {
            return _subscriptionRepository.GetRenewalPlanList(_userIdentityInfo.TenantBranchID.ToString());
        }
        #endregion
    }
}
