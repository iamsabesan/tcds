using Dawn.Frameworks.Entities;
using Dawn.Frameworks.Helper;
using Dawn.Utilities.Enums;
using Dawn.Utilities.Exceptions;
using Dawn.Utilities.i18N;
using EntitiesDTO.Admin;
using Frameworks.Helper;
using iCloud.Franchisee.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace TCDS.Franchisee.Controllers {
    [ApiVersion("1.0")]
    [Route("api/v{v:apiVersion}/subscription/")]
    [ApiController]
    public class SubscriptionV1Controller : ControllerBase {
        #region Fields
        private readonly SubscriptionService _subscriptionService;
        private readonly string _privateKey;
        #endregion
        #region Constructor
        public SubscriptionV1Controller (SqlI18N sqlI18N, IHttpContextAccessor accessor, IDawnHttpClient dawnHttpClient, IOptions<DawnConfig> dawnConfig) {
            _subscriptionService = new SubscriptionService(sqlI18N, accessor, dawnHttpClient, dawnConfig);
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
        /// <exception cref="DawnUserException"></exception>
        [HttpPost("update/subscriptionfees")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public APIResult<string> SubscriptionfeesUpdation (List<UpdatePackage> updatePackage) {
            string result;
            try {
                result = _subscriptionService.UpdatePackage(updatePackage);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.NotExist);
            }
            return APIResult.Success("", result, DawnStatusCode.Created);
        }

        /// <summary>
        /// Approve franchisee request
        /// </summary>
        /// <param name="franchisee"></param>
        /// <param createdBy="sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpPost("update/franchiseefees")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public APIResult<string> ApproveFranchisee (ApproveFranchisee franchisee) {
            string result;
            try {
                result = _subscriptionService.ApproveFranchisee(franchisee);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.NotExist);
            }
            return APIResult.Success("", result, DawnStatusCode.Created);
        }

        /// <summary>
        /// Get franchisee subscription info by id
        /// </summary>
        /// <param name="tenantBranchID"></param>
        /// <param createdBy="sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("basicinfo")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public SubscriptionInfo GetSubscriptionInfo () {
            SubscriptionInfo subscription;
            try {
                subscription = _subscriptionService.GetSubscriptionInfo();
            }
            catch (DawnException ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return subscription;
        }

        /// <summary>
        /// Get subscription list
        /// </summary>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("list")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public IEnumerable<SubscriptionPlan> GetSubscriptionPlans () {
            IEnumerable<SubscriptionPlan> subscriptions;
            try {
                subscriptions = _subscriptionService.GetSubscriptionPlan();
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return subscriptions;
        }

        [HttpGet("renewal/plan/list")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public List<GetRenewalPlanList> GetRenewalPlans () {
            List<GetRenewalPlanList> subscriptions;
            try {
                subscriptions = _subscriptionService.GetRenewalList();
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return subscriptions;
        }

        [HttpGet("renewal/plan/detaillist")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public List<GetRenewalPlanListNew> GetRenewalPlanList()
        {
            List<GetRenewalPlanListNew> subscriptions;
            try
            {
                subscriptions = _subscriptionService.GetRenewalPlanList();
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return subscriptions;
        }
        #endregion
    }
}