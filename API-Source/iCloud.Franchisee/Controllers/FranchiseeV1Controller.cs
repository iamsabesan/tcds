using Dawn.Frameworks.Entities;
using Dawn.Frameworks.Helper;
using Dawn.Utilities.Enums;
using Dawn.Utilities.Exceptions;
using Dawn.Utilities.i18N;
using EntitiesDTO.Admin;
using EntitiesDTO.Payment;
using EntitiesDTO.Utilities;
using Frameworks.Entities;
using Frameworks.Helper;
using iCloud.Franchisee.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using System.Text;

namespace iCloud.Franchisee.Controllers {
    [ApiVersion("1.0")]
    [Route("api/v{v:apiVersion}/franchisee/")]
    [ApiController]
    public class FranchiseeV1Controller : ControllerBase {
        #region Fields
        private readonly FranchiseeService _franchiseeService;
        private readonly IdentityInfo _userIdentityInfo;
        private readonly IWebHostEnvironment _environment;
        private readonly IFileProvider _fileProvider;
        #endregion
        #region Constructor
        public FranchiseeV1Controller (SqlI18N sqlI18N, IHttpContextAccessor accessor, IDawnHttpClient dawnHttpClient, IWebHostEnvironment environment, IFileProvider fileProvider = null) {
            _userIdentityInfo = accessor.GetUserIdentityInfoDetails();
            _franchiseeService = new FranchiseeService(sqlI18N, accessor, dawnHttpClient, environment);
            _environment = environment;
            _fileProvider = fileProvider;
        }
        #endregion
        #region Methodes
        /// <summary>
        /// Register new franchisee
        /// </summary>
        /// <param name="franchiseeRegister"></param>
        /// <param createdBy="sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpPost("register")]
        [DawnAuthorize("Guest", "TenantAdmin", "TenantUser")]
        public async Task<APIResult<Guid>> RegisterFranchisee ([FromForm] FranchiseeRegister franchiseeRegister) {
            Guid franchiseeRequestID;
            try {
                franchiseeRequestID = await _franchiseeService.RegisterFranchisee(franchiseeRegister);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return APIResult.Success(franchiseeRequestID, "Franchisee request send successfully", DawnStatusCode.Created);
        }

        /// <summary>
        /// Reject franchisee request
        /// </summary>
        /// <param name="franchiseeRejection"></param>
        /// <param createdBy="sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpPost("reject")]
        [DawnAuthorize("Guest", "TenantAdmin", "TenantUser")]
        public APIResult<string> RejectFranchisee (FranchiseeRejection franchiseeRejection) {
            string result;
            try {
                result = _franchiseeService.RejectFranchisee(franchiseeRejection);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return APIResult.Success("", result, DawnStatusCode.Created);
        }

        /// <summary>
        /// Activate franchisee
        /// </summary>
        /// <param name="franchisee"></param>
        /// <param createdBy="sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpPost("activate")]
        [DawnAuthorize("Guest", "TenantAdmin", "TenantUser")]
        public async Task<APIResult<Guid>> ActivateFranchisee (ActivateFranchisee franchisee) {
            Guid result;
            try {
                result = await _franchiseeService.ActivateFranchisee(franchisee);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return APIResult.Success(result, "Franchisee activated successfully", DawnStatusCode.Created);
        }

        /// <summary>
        /// Get franchisee inactive list
        /// </summary>
        /// <returns></returns>
        /// <param createdBy="sakthivel"></param>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("list/inactive")]
        [DawnAuthorize("Guest", "TenantAdmin", "TenantUser")]
        public async Task<IEnumerable<FranchiseeInfo>> GetFranchiseeList () {
            IEnumerable<FranchiseeInfo> getBranchInfos;
            try {
                getBranchInfos = await _franchiseeService.GetFranchiseeInfos();
                return getBranchInfos;
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }

        /// <summary>
        /// Get franchisee request status
        /// </summary>
        /// <param name="tenantBranchID"></param>
        /// <param createdBy="sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("requeststatus")]
        [DawnAuthorize("Guest", "TenantAdmin", "TenantUser")]
        public async Task<FranchiseeRequestInfo> GetFranchiseeRequestInfo (Guid franchiseeRequestID) {
            FranchiseeRequestInfo getBranchInfos;
            try {
                getBranchInfos = await _franchiseeService.GetFranchiseeRequestInfo(franchiseeRequestID);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message);
            }
            return getBranchInfos;
        }

        /// <summary>
        /// Get franchisee acrtive list
        /// </summary>
        /// <param createdBy="sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("list/active")]
        [DawnAuthorize("Guest", "TenantAdmin", "TenantUser")]
        public async Task<IEnumerable<FranchiseeActiveList>> GetActiveFranchiseeList () {
            IEnumerable<FranchiseeActiveList> getBranchInfos;
            try {
                getBranchInfos = await _franchiseeService.GetActiveFranchiseeList();
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return getBranchInfos;
        }

        /// <summary>
        /// Create franchisee registration payment
        /// </summary>
        /// <param name="payment"></param>
        /// <param createdBy="sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpPost("payment")]
        [DawnAuthorize("Guest", "TenantAdmin", "TenantUser")]
        public async Task<APIResult<string>> CreateFranchiseePayment (EntitiesDTO.Admin.Payment payment) {
            string result;
            try {
                result = await _franchiseeService.CreateFranchiseePayment(payment);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return APIResult.Success("", result, DawnStatusCode.Created);
        }

        /// <summary>
        /// Get franchisee profile by id
        /// </summary>
        /// <param name="tenantBranchID"></param>
        /// <param createdBy="sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("getbyid")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<FranchiseeProfileInfo> GetFranchiseeProfileByID (Guid tenantBranchID, string fromDate, string toDate) {
            FranchiseeProfileInfo getBranchInfos;
            try {
                getBranchInfos = await _franchiseeService.GetFranchiseeProfileByID(tenantBranchID, fromDate, toDate);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return getBranchInfos;
        }

        /// <summary>
        /// Get franchisee basic info list
        /// </summary>
        /// <param createdBy="sakthivel"></param>
        /// <param createdDate="29-04-2023"></param>
        /// <returns>
        /// Returns franchisee basic info list
        /// </returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("list/basicinfo")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<IEnumerable<GetFranchiseeBasicInfo>> GetFranchiseeBasicInfo () {
            IEnumerable<GetFranchiseeBasicInfo> getBranchInfos;
            try {
                getBranchInfos = await _franchiseeService.GetFranchiseeBasicInfo();
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return getBranchInfos;
        }

        /// <summary>
        /// Get franchisee codes list
        /// </summary>
        /// <param createdBy="sakthivel"></param>
        /// <param createdDate="02-05-2023"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("codes/list")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<IEnumerable<GetFranchiseeCode>> GetFranchiseeID () {
            IEnumerable<GetFranchiseeCode> franchiseeIDs;
            try {
                franchiseeIDs = await _franchiseeService.GetFranchiseeID();
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return franchiseeIDs;
        }

        /// <summary>
        /// Get franchisee profile info
        /// </summary>
        /// <param name="tenantBranchID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("profile/info")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<FranchiseeProfile> GetFranchiseeProfileInfo () {
            FranchiseeProfile franchiseeProfile;
            try {
                franchiseeProfile = await _franchiseeService.GetFranchiseeProfileInfo();
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return franchiseeProfile;
        }

        /// <summary>
        /// Create franchisee renewal
        /// </summary>
        /// <param name="payment"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpPost("approval/renewal/payment")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<APIResult<Guid>> FranchiseeApprovalRenewalPayment (RenewalPayment payment) {
            Guid result; 
            try {
                result = await _franchiseeService.FranchiseeApprovalRenewalPayment(payment);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return APIResult.Success(result, "Franchisee renewal payment successfull !", DawnStatusCode.Created);
        }

        /// <summary>
        /// Reject franchisee renewal request
        /// </summary>
        /// <param name="packageLogID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpPost("renewal/reject")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<APIResult<string>> RejectRenewalRequest (RejectRenewal reject) {
            string result;
            try {
                result = await _franchiseeService.RejectRenewalRequest(reject);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return APIResult.Success("", result, DawnStatusCode.Created);
        }

        /// <summary>
        /// Get franchisee renewal request list
        /// </summary>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("renewal/list")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<List<RenewalRequestList>> getRenewalRequestList () {
            List<RenewalRequestList> requestLists;
            try {
                requestLists = await _franchiseeService.getRenewalRequestList();
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return requestLists;
        }

        /// <summary>
        /// Approve franchisee renewal request
        /// </summary>
        /// <param name="packageLogID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpPost("renewal/approve")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public APIResult<Guid> ApproveRenewalRequest (Guid packageLogID) {
            Guid result;
            try {
                result = _franchiseeService.ApproveRenewalRequest(packageLogID);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return APIResult.Success(result, "Renewal request approved", DawnStatusCode.Created);
        }

        /// <summary>
        /// Lock franchisee
        /// </summary>
        /// <param name="tenantBranchID"></param>
        /// <param name="isActive"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("lock")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<APIResult<string>> LockFranchisee (Guid tenantBranchID, bool isActive) {
            string result;
            try {
                result = await _franchiseeService.LockFranchisee(tenantBranchID, isActive);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return APIResult.Success("", result, DawnStatusCode.Created);
        }

        [HttpGet("baseurl/{fileID}")]
        [DawnAuthorize("TenantAdmin", "TenantUser", "Guest")]
        public APIResult<string> GetImage64String (Guid fileID) {
            string result;
            try {
                var data = _franchiseeService.FileDownLoad(fileID);
                result = "data:image/png;base64," + Convert.ToBase64String(data.Content);
            }
            catch (Exception ex) {
                throw new Exception(ex.Message);
            }
            return APIResult.Success(result, "File retrived successfully", DawnStatusCode.Created);
        }
        [HttpGet("baseurllist")]
        [DawnAuthorize("TenantAdmin", "TenantUser", "Guest")]
        public APIResult<List<BaseUrlList>> GetImage64StringList(Guid franchiseeRequestID)
        {
            try
            {
                var result = _franchiseeService.FileDownLoadList(franchiseeRequestID);
                return APIResult.Success(result, "File retrived successfully", DawnStatusCode.Success);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            
        }

        /// <summary>
        /// Get renewal payment list by admin
        /// </summary>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("renewal/payment/list")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<List<RenewalPaymentList>> GetRenewalPaymentList () {
            List<RenewalPaymentList> renewalPayments;
            try {
                renewalPayments = await _franchiseeService.GetRenewalPaymentList();
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return renewalPayments;
        }

        /// <summary>
        /// Get franchisee whole period date
        /// </summary>
        /// <param name="tenantBranchID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("wholeperiod")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public string GetWholePeriodDate (string? tenantBranchID) {
            string result;
            try {
                Guid branchID = Guid.Empty;
                if (tenantBranchID == null) {
                    branchID = _userIdentityInfo.TenantBranchID;
                }
                else {
                    branchID = Guid.Parse(tenantBranchID);
                }
                result = _franchiseeService.GetWholePeriodDate(branchID);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return result;
        }

        /// <summary>
        /// Get admin dashboard
        /// </summary>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("admin/dashboard")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<AdminDashBoard> GetAdminDashboard () {
            AdminDashBoard dashBoard;
            try {
                dashBoard = await _franchiseeService.GetAdminDashboard();
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return dashBoard;
        }

        /// <summary>
        /// Get franchisee dashboard
        /// </summary>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("dashboard")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<DashboardByFranchisee> GetFranchiseeDashboard () {
            DashboardByFranchisee dashBoard;
            try {
                dashBoard = await _franchiseeService.GetFranchiseeDashboard();
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return dashBoard;
        }

        /// <summary>
        /// Reject paid request
        /// </summary>
        /// <param name="reject"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpPost("reject/paid/request")]
        [DawnAuthorize("TenantAdmin")]
        public APIResult<string> RejectPaidRequest (RejectPaidRequest reject) {
            string result;
            try {
                result = _franchiseeService.RejectPaidRequest(reject);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return APIResult.Success("", result, DawnStatusCode.Created);
        }

        [HttpPost("add-videos")]
        [DawnAuthorize("TenantAdmin")]
        public APIResult<string> AddVideo (AddVideos add) {
            string result;
            try {
                result = _franchiseeService.AddVideo(add);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return APIResult.Success("", result, DawnStatusCode.Created);
        }

        /// <summary>
        /// Edit mobile app video by guideVideoID
        /// </summary>
        /// <param name="mobile_app"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpPost("edit-video")]
        [DawnAuthorize("TenantAdmin")]
        public APIResult<string> EditVideo (EditVideo edit) {
            string result;
            try {
                result = _franchiseeService.EditVideo(edit);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return APIResult.Success("", result, DawnStatusCode.Created);
        }

        /// <summary>
        /// Delete the guide video using guidevideoID
        /// </summary>
        /// Author:Sakthivel B
        /// CreatedDate:12-06-2024
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("guide-video/delete/{guideVideoID}")]
        [DawnAuthorize("TenantAdmin")]
        public APIResult<string> DeleteGuideVideo (Guid guideVideoID) {
            string result;
            try {
                result = _franchiseeService.DeleteGuideVideo(guideVideoID);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return APIResult.Success("", result, DawnStatusCode.Created);
        }

        [HttpGet("guide-videos/{mobile_app}")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public GetVideos GetGuideVideos (string mobile_app) {
            GetVideos guideVideos;
            try {
                guideVideos = _franchiseeService.GetGuideVideos(mobile_app);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return guideVideos;
        }

        [HttpPost("mobile-course-details")]
        [DawnAuthorize("TenantAdmin", "TenantUser", "Guest")]
        public async Task<List<MblFranchisee>> GetMobileCourseDetails (MblFilter filter) {
            List<MblFranchisee> details;
            try {
                details = await _franchiseeService.GetFranchisees(filter);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return details;
        }

        [HttpGet("mobile-renewal/list")]
        [DawnAuthorize("TenantUser")]
        public async Task<List<MlbRenewalList>> MblRenewalList () {
            List<MlbRenewalList> renewalLists;
            try {
                renewalLists = await _franchiseeService.MblRenewalList();
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return renewalLists;
        }

        [HttpPost("state-level/rank")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<List<RankList>> GetStateFranchiseeRank (StateRankFilter filter) {
            try {
                return await _franchiseeService.GetStateFranchiseeRank(filter);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }

        [HttpPost("district-level/rank")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<List<RankList>> GetDistrictFranchiseeRank (DistrictRankFilter filter) {
            try {
                return await _franchiseeService.GetDistrictFranchiseeRank(filter);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }

        [HttpPost("state-level/achievers")]
        [DawnAuthorize("TenantAdmin", "TenantUser", "Guest")]
        public async Task<List<RankList>> GetStateFranchiseeAchiever (StateAchieversFilter filter) {
            try {
                return await _franchiseeService.GetStateFranchiseeAchiever(filter);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }

        [HttpPost("district-level/achievers")]
        [DawnAuthorize("TenantAdmin", "TenantUser", "Guest")]
        public async Task<List<RankList>> GetDistrictFranchiseeAchiever (DistrictAchieversFilter filter) {
            try {
                return await _franchiseeService.GetDistrictFranchiseeAchiever(filter);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }

        [HttpGet("rank-status")]
        [DawnAuthorize("TenantUser")]
        public async Task<FranchiseeRankStatus> GetFranchiseeRankStatus () {
            try {
                return await _franchiseeService.GetFranchiseeRankStatus();
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }

        [HttpPost("notice/create")]
        [DawnAuthorize("TenantUser")]
        public APIResult<string> CreateNoticeBoard (CreateNotice create) {
            string result;
            try {
                result = _franchiseeService.CreateNotice(create);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return APIResult.Success("", result, DawnStatusCode.Created);
        }

        [HttpGet("get-notice")]
        [DawnAuthorize("TenantUser")]
        public GetNotice GetNoticeBoard () {
            try {
                return _franchiseeService.GetNoticeBoard();
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }

        [HttpGet("help-support")]
        [DawnAuthorize("TenantUser", "Guest")]
        public HelpAndSupport GetHelpAndSupport () {
            try {
                return _franchiseeService.GetHelpAndSupport();
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }

        /// <summary>
        /// Upload student feedback
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpPost("feedback/upload")]
        [DawnAuthorize("TenantUser")]
        public async Task<APIResult<string>> UploadVideo ([FromForm] UploadStudentFeedback file) {
            string result;
            try {
                result = await _franchiseeService.UploadStudentFeedback(file);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return APIResult.Success("", result, DawnStatusCode.Created);
        }

        [HttpPost("feedback/approve")]
        [DawnAuthorize("TenantAdmin")]
        public APIResult<string> ApproveFeedBack (ApproveFeedback approve) {
            string result;
            try {
                result = _franchiseeService.ApproveFeedback(approve);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return APIResult.Success("", result, DawnStatusCode.Created);
        }

        [HttpPost("feedback/reject")]
        [DawnAuthorize("TenantAdmin")]
        public APIResult<string> RejectFeedback (RejectStudentFeedback reject) {
            string result;
            try {
                result = _franchiseeService.RejectFeedback(reject);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return APIResult.Success("", result, DawnStatusCode.Created);
        }

        [HttpGet("download/{fileName}")]
        [DawnAuthorize("TenantAdmin")]
        public IActionResult DownloadFileV1 (string fileName) {
            var filePath = Path.Combine(_environment.WebRootPath, "videos", fileName);
            if (!System.IO.File.Exists(filePath)) {
                return NotFound("File not found.");
            }
            var contentType = GetContentType(filePath);
            var fileContents = Encoding.UTF8.GetBytes(contentType);
            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return new FileContentResult(fileBytes, contentType) {
                FileDownloadName = fileName
            };
        }

        [HttpGet("student-feedback/list")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public List<GetStudentFeedback> GetFeedbackByAdmin () {
            List<GetStudentFeedback> list;
            try {
                list = _franchiseeService.GetFeedbackByAdmin();
                return list;
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }

        private string GetContentType (string path) {
            var types = new Dictionary<string, string> {
                { ".txt", "text/plain" },
                { ".pdf", "application/pdf" },
                { ".doc", "application/vnd.ms-word" },
                { ".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
                { ".xls", "application/vnd.ms-excel" },
                { ".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
                { ".png", "image/png" },
                { ".jpg", "image/jpeg" },
                { ".jpeg", "image/jpeg" },
                { ".gif", "image/gif" },
                { ".csv", "text/csv" },
                { ".mp4", "video/mp4" }
            };
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types.ContainsKey(ext) ? types[ext] : "application/octet-stream";
        }

        /// <summary>
        /// Get mobile apps
        /// </summary>
        /// Author:Sakthivel B
        /// CreatedDate:11-06-2024
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("mobile/apps")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public List<GetMobileApp> GetMobileApps () {
            try {
                return _franchiseeService.GetMobileApps();
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }

        /// <summary>
        /// Get mobile apps in show-in
        /// </summary>
        /// Author:Sakthivel B
        /// CreatedDate:11-06-2024
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("mobile/show-in")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public List<GetMobileApp> GetMobileShowIn () {
            try {
                return _franchiseeService.GetMobileShowIn();
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }


        /// <summary>
        /// Get guide video
        /// </summary>
        /// Author:Sakthivel B
        /// CreatedDate:12-06-2024
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("guide-video-list")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public List<GuideVideos> GetGuideVideos () {
            try {
                return _franchiseeService.GuideVideos();
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }

        /// <summary>
        /// Get franchisee feedback video
        /// </summary>
        /// Author:Sakthivel B
        /// CreatedDate:13-06-2024
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("guide-video/feedback/list")]
        [DawnAuthorize("Franchisee", "TenantUser")]
        public List<GuideVideos> GetFeedbackVideo () {
            try {
                return _franchiseeService.GetFeedbackVideo();
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }

        /// <summary>
        /// Get student welcome video
        /// </summary>
        /// Author:Sakthivel B
        /// CreatedDate:13-06-2024
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("guide-video/welcome/list")]
        [DawnAuthorize("Student", "TenantUser")]
        public List<GuideVideos> GetWelcomeVideo () {
            try {
                return _franchiseeService.GetWelcomeVideo();
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }

        /// <summary>
        /// Get student purpose video
        /// </summary>
        /// Author:Sakthivel B
        /// CreatedDate:13-06-2024
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("guide-video/purpose/list")]
        [DawnAuthorize("Student", "TenantUser")]
        public List<GuideVideos> GetPurposeVideo () {
            try {
                return _franchiseeService.GetPurposeVideo();
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }

        /// <summary>
        /// Get student feedback video by franchisee
        /// </summary>
        /// Author:Sakthivel B
        /// CreatedDate:13-06-2024
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("guide-video/student-feedback/list")]
        [DawnAuthorize("Student", "TenantUser")]
        public List<GuideVideos> GetStudentFeedbackVideo () {
            try {
                return _franchiseeService.GetStudentFeedbackVideo();
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }

        [HttpPost("image/upload")]
        [DawnAuthorize("TenantAdmin", "TenantUser", "Guest")]
        public async Task<APIResult<DocumentResponce>> ImageDocumentUpload ([FromForm] ImageDocument imgUpload) {
            try {
                return APIResult.Success(await _franchiseeService.ImageDocumentUpload(imgUpload),
                    "Document uploaded successfully !", DawnStatusCode.Success);
            }
            catch (DawnUserException ex) {
                throw new DawnUserException(ex.Message);
            }
        }

        [HttpGet("bank/list")]
        [DawnAuthorize("TenantAdmin", "TenantUser", "Guest")]
        public List<FranchiseeBanks> GetFranchiseeBanks (Guid franchiseeRequestID) {
            try {
                return _franchiseeService.GetFranchiseeBanks(franchiseeRequestID);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }

        [HttpGet("level/status")]
        [DawnAuthorize("TenantAdmin", "TenantUser", "Guest")]
        public string[] GetLevelStatus () {
            return _franchiseeService.GetLevelStatus();
        }

        [HttpGet("type/status")]
        [DawnAuthorize("TenantAdmin", "TenantUser", "Guest")]
        public string[] GetTypeStatus () {
            return _franchiseeService.GetTypeStatus();
        }

        [HttpGet("relationship/list")]
        [DawnAuthorize("TenantAdmin", "TenantUser", "Guest")]
        public IEnumerable<GetMobileApp> GetRelationship () {
            return _franchiseeService.GetRelationship();
        }

        [HttpPost("otp/verify")]
        [DawnAuthorize("TenantAdmin", "TenantUser", "Guest")]
        public async Task<APIResult<OTPVerifyResult>> OTPVerify (OTPVerify otpVerify) {
            try {
                OTPVerifyResult oTPVerify = await _franchiseeService.OTPVerify(otpVerify);
                return APIResult.Success(oTPVerify, oTPVerify.Message, oTPVerify.IsOTPVerified ? DawnStatusCode.Created : DawnStatusCode.Error);
            }
            catch (DawnUserException ex) {
                throw new DawnUserException(ex.Message);
            }
        }
        [HttpPost("email/otp/resend")]
        [DawnAuthorize("TenantAdmin", "TenantUser", "Guest")]
        public async Task<APIResult<OTPResult>> EmailOTPResend (EmailResendRequest resendEmail) {
            try {
                return APIResult.Success(await _franchiseeService.EmailOTPRequest(resendEmail.Email),
                    "OTP resend sent successfully !", DawnStatusCode.Success);
            }
            catch (DawnUserException ex) {
                throw new DawnUserException(ex.Message);
            }
        }

        [HttpPost("registration/otp/request")]
        [DawnAuthorize("TenantAdmin", "TenantUser", "Guest")]
        public async Task<APIResult<OTPResult>> RegistrationOTPRequest (RegistrationOTPRequest otpRequest) {
            try {
                return APIResult.Success(await _franchiseeService.RegistrationOTPRequest(otpRequest),
                    OTPMessage.Sent, DawnStatusCode.Success);
            }
            catch (DawnUserException ex) {
                throw new DawnUserException(ex.Message);
            }
        }

        [HttpGet("taluk/list")]
        [DawnAuthorize("TenantAdmin", "TenantUser", "Guest")]
        public IEnumerable<TalukList> GetTalukList (string district) {
            return _franchiseeService.GetTalukList(district);
        }

        [HttpGet("account/type")]
        [DawnAuthorize("TenantAdmin", "TenantUser", "Guest")]
        public string[] GetAccountType () {
            return _franchiseeService.GetAccountType();
        }

        [HttpGet("approved/certificate")]
        [DawnAuthorize("TenantAdmin", "TenantUser", "Guest")]
        public GetApprovalCertificate GetApprovalCertificate (Guid tenantBranchID) {
            try {
                return _franchiseeService.GetApprovalCertificate(tenantBranchID);
            }
            catch(Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }

        [HttpPost("edit/franchisee/code")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public APIResult<string> EditFranchiseeCode(EditFranchiseeCode edit) {
            try {
                return APIResult.Success("", _franchiseeService.EditFranchiseeCode(edit), DawnStatusCode.Created);
            }
            catch(Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }
        [HttpGet("state/list")]
        [DawnAuthorize("TenantAdmin", "TenantUser", "Guest")]
        public APIResult<IEnumerable<StateList>> GetStateList(string country)
        {
            try
            {
            return APIResult.Success(_franchiseeService.GetStateList(country), PlaceMessage.State, DawnStatusCode.Success);
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }
        [HttpGet("district/list")]
        [DawnAuthorize("TenantAdmin", "TenantUser", "Guest")]
        public APIResult<IEnumerable<DistrictList>> GetDistrictList(string state)
        {
            try
            {
            return APIResult.Success(_franchiseeService.GetDistrictList(state), PlaceMessage.District, DawnStatusCode.Success);
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }
        [HttpGet("courier/charges")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public APIResult<int> GetCourierCharges()
        {
            try
            {
                return APIResult.Success(_franchiseeService.GetCourierCharges(), PlaceMessage.Courier, DawnStatusCode.Success);
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }
        [HttpGet("id-card/charges")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public APIResult<int> GetIDCardCharges()
        {
            try
            {
                return APIResult.Success(_franchiseeService.GetIDCardCharges(), PlaceMessage.IdCard, DawnStatusCode.Success);
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }
        [HttpPost("id-card/charge/update")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public APIResult<string> ToUpdateIDCardCharges(int amount)
        {
            try
            {
                return APIResult.Success(_franchiseeService.UpdateIDCardCharges(amount), PlaceMessage.IdCardUpdate, DawnStatusCode.Success);
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }
        [HttpPost("courier/charge/update")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public APIResult<string> ToUpdateCourierCharges(int amount)
        {
            try
            {
                return APIResult.Success(_franchiseeService.UpdateCourierCharges(amount), PlaceMessage.CourierUpdate, DawnStatusCode.Success);
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }
        [HttpPost("idcard-courier/charge/update")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public APIResult<string> IDCardCourierChargeUpdate(ChargeUpdate amount)
        {
            try
            {
                return APIResult.Success(_franchiseeService.IDCardCourierChargeUpdate(amount), PlaceMessage.ChargeUpdate, DawnStatusCode.Success);
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }

        [HttpPost("edit/contact")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public APIResult<string> EditFranchiseePersonalDetails(EditFranchiseeCodeData edit)
        {
            try
            {
                return APIResult.Success("", _franchiseeService.EditFranchiseePersonalDetails(edit), DawnStatusCode.Created);
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }

        [HttpPost("edit")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public APIResult<string> EditFranchiseeDetails(EditFranchiseeCodeData edit)
        {
            try
            {
                return APIResult.Success("", _franchiseeService.EditFranchiseeDetails(edit), DawnStatusCode.Created);
            }
            catch (Exception ex)
            {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
        }
        #endregion
    }
}
