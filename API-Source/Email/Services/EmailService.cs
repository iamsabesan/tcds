using CommonServices;
using Dawn.Frameworks.Helper;
using Dawn.Utilities.i18N;
using Email.Repository;
using EntitiesDTO.Email;
using Frameworks.Entities;
using Frameworks.Helper;
using System.Data;
using EntitiesDTO.Utilities;
using EntitiesDTO.Admin;
using EntitiesDTO.Student;
using EntitiesDTO.Users;
using Dawn.Utilities.Extensions;
using Dawn.Utilities.Exceptions;

namespace Email.Services
{
    public class EmailService
    {
        #region Fields
        private readonly SqlI18N _sqlI18N;
        private readonly IdentityInfo _userIdentityInfo;
        private string _connectionString;
        private readonly EmailRepository _emailRepository;
        private readonly CommonService _commonService;
        private readonly IDawnHttpClient _dawnHttpClient;

        #endregion
        #region Constructor
        public EmailService(SqlI18N sqlI18N, IHttpContextAccessor accessor, IDawnHttpClient dawnHttpClient)
        {
            _sqlI18N = sqlI18N;
            _userIdentityInfo = accessor.GetUserIdentityInfoDetails();
            _connectionString = accessor.GetConnectionString(dawnHttpClient);
            //_connectionString = "Data Source=3.108.251.251;Initial Catalog=InternalEmail_Service_98392E8C_A6FC_48D6_873D_5A6C7C514B94_TCDS;Persist Security Info=True;User ID=Identity_db ;Password=Ljgl5871$;TrustServerCertificate=Yes;";
            _commonService = new CommonService(dawnHttpClient);
            _emailRepository = new EmailRepository(_sqlI18N, _connectionString);
            _dawnHttpClient = dawnHttpClient;
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
        public async Task<Guid> CreateMail(CreateMail mail) {
            Guid contactID = _userIdentityInfo.ContactID;
            CreateNewMail newMail = new CreateNewMail();
            newMail.FromContactID = contactID;
            newMail.Subject = mail.Subject;
            newMail.MailContent = mail.MailContent;
            newMail.CreatedBy = contactID;
            DataTable toContact = mail.ToContactID.ToDataTable();
            Guid mailBoxID = _emailRepository.CreateMail(newMail, toContact);
            var supportDocuments = await DocumentToDataTable(mail.Attachment ?? new List<IFormFile>());
            //var attachment = _emailRepository.MailAttachment(mailBoxID, supportDocuments, contactID);
            if (supportDocuments != null)
            {
                _emailRepository.MailAttachment(mailBoxID, supportDocuments, contactID);
            }
            //if(attachment == 0)
            //{
            //    int result = _emailRepository.RollBackEmail(mailBoxID);
            //    throw new Exception("Couldn't send mail !");
            //}
            return mailBoxID;
        }

        private async Task<DataTable> DocumentToDataTable(List<IFormFile> document) {
            List<SupportDocumentItems> supportDocuments = new List<SupportDocumentItems>();
            foreach (IFormFile item in document) {
                using (var ms = new MemoryStream()) {
                    await item.CopyToAsync(ms);
                    var fileBytes = ms.ToArray();
                    supportDocuments.Add(new SupportDocumentItems {
                        FileName = item.FileName,
                        ContentType = item.ContentType,
                        Content = fileBytes
                    });
                }
            }
            return supportDocuments.ToDataTable();
        }

        /// <summary>
        /// Get mail inbox list
        /// </summary>
        /// <param createdBy="sakthivel"></param>
        /// <returns></returns>
        public async Task<List<MailInboxList>> GetMailInboxList() {
            List<MailInboxList> inboxLists = new List<MailInboxList>();
            var inbox = _emailRepository.GetInboxList(_userIdentityInfo.ContactID);
            DataSet branchDataSet = await _commonService.GetTenantBranchAndContact();
            var franchiseeIDs = await _commonService.GetFranchiseeID();
            foreach(var item in inbox) {
                var attachments = _emailRepository.GetMailAttachmentList(item.MailBoxID);
                var branch = branchDataSet.Tables[0].DataTablesToList<FranchiseeBaseInfo>().Where(x => x.ContactID == item.FromContactID).FirstOrDefault();
                var address = branchDataSet.Tables[1].DataTablesToList<Address>().Where(x => x.ContactID == item.FromContactID).FirstOrDefault();
                var franchiseeID = franchiseeIDs.Where(x => x.TenantBranchID == branch.TenantBranchID).FirstOrDefault();
                MailInboxList inboxList = new MailInboxList();
                inboxList.MailOutBoxID = item.MailOutBoxID;
                inboxList.FranchiseeID = franchiseeID != null ? franchiseeID.FranchiseeCode : "";
                inboxList.BranchName = branch.TenantBranchName;
                inboxList.Address1 = address.Address1;
                inboxList.Address2 = address.Address2;
                inboxList.Area = address.Area;
                inboxList.District = address.District;
                inboxList.State = address.State;
                inboxList.Country = address.Country;
                inboxList.Pincode = address.Pincode;
                inboxList.Date = item.Date.ToString();
                inboxList.Subject = item.Subject;
                inboxList.Content = item.Content;
                inboxList.Status = item.Status;
                inboxList.Attachments = attachments;
                inboxLists.Add(inboxList);
            }
            return inboxLists;
        }

        /// <summary>
        /// Delete mail
        /// </summary>
        /// <param name="mailBoxID"></param>
        /// <param createdBy="sakthivel"></param>
        /// <returns></returns>
        public string DeleteMail(Guid mailOutBoxID) {
            int result = _emailRepository.DeleteMail(mailOutBoxID);
            if (result == 0)
                throw new DawnUserException("Couldn't delete mail !");
            return "Mail deleted";
        }

        /// <summary>
        /// Rollback mail
        /// </summary>
        /// <param name="mailBoxID"></param>
        /// <param createdBy="sakthivel"></param>
        /// <returns></returns>
        public string RollBackEmail(Guid mailOutBoxID) {
            int result = _emailRepository.RollBackEmail(mailOutBoxID);
            return result > 0 ? "Mail rollbacked successfully" : "Couldn't rollback mail !";
        }

        /// <summary>
        /// Read mail
        /// </summary>
        /// <param name="mailBoxID"></param>
        /// <param createdBy="sakthivel"></param>
        /// <returns></returns>
        public string ReadEmail(Guid mailOutBoxID) {
            int result = _emailRepository.ReadEmail(mailOutBoxID);
            return string.Empty;
        }

        /// <summary>
        /// Get deleted mail list
        /// </summary>
        /// <param createdBy="sakthivel"></param>
        /// <returns></returns>
        public async Task<List<NewDeletedMailList>> GetDeletedMailList() {
            List<NewDeletedMailList> deletedMailLists = new List<NewDeletedMailList>();
            DataSet branchDataSet = await _commonService.GetTenantBranchAndContact();
            var franchiseeIDs = await _commonService.GetFranchiseeID();
            var deletedList = _emailRepository.DeletedMailList(_userIdentityInfo.ContactID);
            foreach(var mail in deletedList) {
                var branch = branchDataSet.Tables[0].DataTablesToList<FranchiseeBaseInfo>().Where(x => x.ContactID == mail.FromContactID).FirstOrDefault();
                var address = branchDataSet.Tables[1].DataTablesToList<Address>().Where(x => x.ContactID == mail.FromContactID).FirstOrDefault();
                var franchiseeID = franchiseeIDs.Where(x => x.TenantBranchID == branch.TenantBranchID).FirstOrDefault();
                NewDeletedMailList mailList = new NewDeletedMailList();
                mailList.MailBoxID = mail.MailBoxID;
                mailList.MailOutBoxID = mail.MailOutBoxID;
                mailList.FranchiseeID = franchiseeID != null ? franchiseeID.FranchiseeCode : "";
                mailList.BranchName = branch.TenantBranchName;
                mailList.Address1 = address.Address1;
                mailList.Address2 = address.Address2;
                mailList.Area = address.Area;
                mailList.District = address.District;
                mailList.State = address.State;
                mailList.Country = address.Country;
                mailList.Pincode = address.Pincode;
                mailList.Date = mail.Date;//.ToString(_userIdentityInfo.DateTimeFormat);
                mailList.Subject = mail.Subject;
                deletedMailLists.Add(mailList);
            }
            return deletedMailLists;
        }

        /// <summary>
        /// Get mail send list
        /// </summary>
        /// <param createdBy="sakthivel"></param>
        /// <returns></returns>
        public async Task<List<SendList>> GetSendList() {
            List<SendList> sendLists = new List<SendList>();
            DataSet emailDataSet = _emailRepository.GetSendList(_userIdentityInfo.ContactID);
            var mailInfo = emailDataSet.Tables[0].DataTablesToList<MailInfo>();
            DataSet branchDataSet = await _commonService.GetTenantBranchAndContact();
            var franchiseeIDs = await _commonService.GetFranchiseeID();
            foreach (var mail in mailInfo) {
                var toContact = emailDataSet.Tables[1].DataTablesToList<ToInfo>().
                    Where(x => x.MailBoxID == mail.MailBoxID).ToList();
                var ducuments = emailDataSet.Tables[2].DataTablesToList<MailAttachments>().
                    Where(x => x.MailBoxID == mail.MailBoxID).ToList();
                foreach(var item in toContact) {
                    var branch = branchDataSet.Tables[0].DataTablesToList<FranchiseeBaseInfo>().
                        Where(x => x.ContactID == item.ToContactID).FirstOrDefault();
                    var address = branchDataSet.Tables[1].DataTablesToList<Address>().
                        Where(x => x.ContactID == item.ToContactID).FirstOrDefault();
                    var franchiseeID = franchiseeIDs.Where(x => x.TenantBranchID == branch.TenantBranchID).FirstOrDefault();
                    ContactInfos contact = new ContactInfos();
                    contact.FranchiseeID = franchiseeID.FranchiseeCode;
                    contact.BranchName = branch.TenantBranchName;
                    contact.Address1 = address.Address1;
                    contact.Address2 = address.Address2;
                    contact.Area = address.Area;
                    contact.District = address.District;
                    contact.State = address.State;
                    contact.Country = address.Country;
                    contact.Pincode = address.Pincode;
                    SendList send = new SendList();
                    send.To = contact;
                    send.MailBoxID = mail.MailBoxID;
                    send.Date = mail.Date;
                    send.Subject = mail.Subject;
                    send.Content = mail.Content;
                    send.Attachments = ducuments;
                    sendLists.Add(send);
                }
            }
            return sendLists;
        }

        /// <summary>
        /// Get documents from database
        /// </summary>
        /// <param name="fileId"></param>
        /// <param createdBy="sakthivel"></param>
        /// <returns></returns>
        public DownloadDocument FileDownLoad(Guid fileId) {
            return _emailRepository.FileDownLoad(fileId);
        }

        /// <summary>
        /// Get branch wise contact info
        /// </summary>
        /// <param createdBy="sakthivel"></param>
        /// <returns></returns>
        public async Task<IEnumerable<GetNewBranchWiseContactInfo>> GetBranchWiseContactInfo() {
            var contact = await _commonService.GetBranchWiseContactInfo();
            var franchisee = await _commonService.GetFranchiseeID();
            DataTable branchIdsTable = new DataTable();
            branchIdsTable.Columns.Add("TenantBranchID", typeof(Guid));
            foreach (var item in franchisee)
            {
                branchIdsTable.Rows.Add(item.TenantBranchID);
            }
            string franchiseeConn = await GetConnectionString(_userIdentityInfo.TenantID, "Franchisee");
            var commonRepo = new CommonRepository(_sqlI18N, franchiseeConn);
            var branchTalukList = commonRepo.GetFranchiseeCode(branchIdsTable);
            var contactInfo = (from c in contact
                               join f in franchisee on c.TenantBranchID equals f.TenantBranchID
                               select new GetNewBranchWiseContactInfo() {
                                   TenantBranchID = c.TenantBranchID,
                                   FranchiseeID = f.FranchiseeCode,
                                   TenantBranchName = c.TenantBranchName,
                                   ContactID = c.ContactID,
                                   ContactName = c.ContactName,
                                   AddressTypeID = c.AddressTypeID,
                                   AddressType = c.AddressType,
                                   Address1 = c.Address1,
                                   Address2 = c.Address2,
                                   Area = c.Area,
                                   District = c.District,
                                   State = c.State,
                                   Country = c.Country,
                                   Pincode = c.Pincode,
                                   Taluk = branchTalukList.FirstOrDefault(x => x.TenantBranchID == c.TenantBranchID)?.Taluk ?? string.Empty
        }).ToList();
            return contactInfo;
        }

        /// <summary>
        /// Get unread mail count
        /// </summary>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        public int GetMailCount() {
            return _emailRepository.GetMailCount(_userIdentityInfo.ContactID);
        }
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
    }
}
