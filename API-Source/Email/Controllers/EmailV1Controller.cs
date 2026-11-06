using Dawn.Frameworks.Entities;
using Dawn.Frameworks.Helper;
using Dawn.Utilities.Enums;
using Dawn.Utilities.Exceptions;
using Dawn.Utilities.i18N;
using Email.Services;
using EntitiesDTO.Email;
using EntitiesDTO.Users;
using Frameworks.Entities;
using Frameworks.Helper;
using Microsoft.AspNetCore.Mvc;

namespace Email.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{v:apiVersion}/email/")]
    [ApiController]
    public class EmailV1Controller : ControllerBase
    {
        #region Fields
        private readonly SqlI18N _sqlI18N;
        private readonly IdentityInfo _userIdentityInfo;
        private readonly EmailService _emailService;
        #endregion
        #region Constructor
        public EmailV1Controller(SqlI18N sqlI18N, IHttpContextAccessor accessor, IDawnHttpClient dawnHttpClient)
        {
            _sqlI18N = sqlI18N;
            _userIdentityInfo = accessor.GetUserIdentityInfoDetails();
            _emailService = new EmailService(sqlI18N, accessor, dawnHttpClient);
        }
        #endregion
        #region Methods
        /// <summary>
        /// Create mail
        /// </summary>
        /// <param name="mail"></param>
        /// <param CreatedBy="Sakthivel"></param>
        /// <param CreatedDate="16-06-2023"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpPost("create")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<APIResult<Guid>> CreateMail([FromForm]CreateMail mail) {
            Guid result;
            try {
                result = await _emailService.CreateMail(mail);
            }
            catch(Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return APIResult.Success(result, "Mail send successfully !", DawnStatusCode.Created);
        }

        /// <summary>
        /// Get mail inbox list
        /// </summary>
        /// <param createdBy="sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("inbox/list")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<List<MailInboxList>> GetMailInboxList() {
            List<MailInboxList> inboxLists;
            try {
                inboxLists = await _emailService.GetMailInboxList();
            }
            catch(Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return inboxLists;
        }

        /// <summary>
        /// Delete mail
        /// </summary>
        /// <param name="mailBoxID"></param>
        /// <param createdBy="sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("delete")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public APIResult<string> DeleteMail(Guid mailOutBoxID) {
            string result;
            try {
                result = _emailService.DeleteMail(mailOutBoxID);
            }
            catch(Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return APIResult.Success("", result, DawnStatusCode.Created);
        }

        /// <summary>
        /// Rollback mail
        /// </summary>
        /// <param name="mailBoxID"></param>
        /// <param createdBy="sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("rollback")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public APIResult<string> RollBackEmail(Guid mailOutBoxID) {
            string result;
            try {
                result = _emailService.RollBackEmail(mailOutBoxID);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return APIResult.Success("", result, DawnStatusCode.Created);
        }

        /// <summary>
        /// Read mail
        /// </summary>
        /// <param name="mailBoxID"></param>
        /// <param createdBy="sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("read")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public APIResult<string> ReadMail(Guid mailOutBoxID) {
            string result;
            try {
                result = _emailService.ReadEmail(mailOutBoxID);
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return APIResult.Success(result, "Mail read successfully", DawnStatusCode.Created);
        }

        /// <summary>
        /// Get deleted mail list
        /// </summary>
        /// <param createdBy="sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("delete/list")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<List<NewDeletedMailList>> GetDeletedMailList() {
            List<NewDeletedMailList> mailLists = new List<NewDeletedMailList>();
            try {
                mailLists = await _emailService.GetDeletedMailList();
            }
            catch( Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return mailLists;
        }

        /// <summary>
        /// Get mail send list
        /// </summary>
        /// <param createdBy="sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("send/list")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<List<SendList>> GetSendList() {
            List<SendList> sendLists = new List<SendList>();
            try {
                sendLists = await _emailService.GetSendList();
            }
            catch(Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return sendLists;
        }


        [HttpGet("baseurl/{fileID}")]
        public APIResult<string> GetImage64String(Guid fileID) {
            string result = string.Empty;
            try {
                var data =  _emailService.FileDownLoad(fileID);

                result = "data:image/png;base64," + Convert.ToBase64String(data.Content);
            }
            catch (Exception ex) {
                throw new Exception(ex.Message);
            }
            return APIResult.Success(result, "File retrived successfully", DawnStatusCode.Created);
        }

        /// <summary>
        /// Get branch wise contact info
        /// </summary>
        /// <param createdBy="sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("branch/wise/contact")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public async Task<IEnumerable<GetNewBranchWiseContactInfo>> GetBranchWiseContactInfo() {
            IEnumerable<GetNewBranchWiseContactInfo> contact = new List<GetNewBranchWiseContactInfo>();
            try {
                contact = await _emailService.GetBranchWiseContactInfo();
            }
            catch (Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return contact;
        }

        /// <summary>
        /// Get unread mail count
        /// </summary>
        /// <param createdBy="sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="DawnUserException"></exception>
        [HttpGet("count")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public int GetMailCount() {
            int count;
            try {
                count = _emailService.GetMailCount();
            }
            catch(Exception ex) {
                throw new DawnUserException(ex.Message, DawnStatusCode.Error);
            }
            return count;
        }

        /// <summary>
        /// Download image
        /// </summary>
        /// <param name="fileID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpGet("download/{fileID}")]
        [DawnAuthorize("TenantAdmin", "TenantUser")]
        public FileContentResult DownloadFile(Guid fileID) {
            try {
                var data =  _emailService.FileDownLoad(fileID);

                return File(data.Content, data.MIMEType, data.FileName);
            }
            catch (Exception ex) {
                throw new Exception(ex.Message);
            }
        }
        #endregion
    }
}
