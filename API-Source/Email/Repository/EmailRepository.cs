using Dawn.Database;
using Dawn.Utilities.Extensions;
using Dawn.Utilities.i18N;
using EntitiesDTO.Email;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Email.Repository {
    public class EmailRepository : DawnDbClient {
        #region Fields
        private readonly SqlI18N _sqlI18N;
        #endregion
        #region Constructor
        public EmailRepository (SqlI18N SqlI18N, string ConnectionString) : base(ConnectionString) {
            _sqlI18N = SqlI18N;
        }
        #endregion
        #region Methods
        /// <summary>
        /// Create mail
        /// </summary>
        /// <param name="mail"></param>
        /// <param name="toContact"></param>
        /// <param CreatedBy="Sakthivel"></param>
        /// <param CreatedDate="16-06-2023"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public Guid CreateMail (CreateNewMail mail, DataTable toContact) {
            Guid mailBoxID = Guid.Empty;
            try {
                mailBoxID = (Guid)ExecuteScalar("uspCreateEmail", new {
                    FromContactID = mail.FromContactID,
                    Subject = mail.Subject,
                    MailContent = mail.MailContent,
                    CreatedBy = mail.CreatedBy,
                    ToContact = toContact
                }, commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return mailBoxID;
        }

        public int MailAttachment (Guid mailBoxID, DataTable supportDocumentItems, Guid actionBy) {
            int result = 0;
            try {
                result = ExecuteScalar("uspMailAttachments", new {
                    mailBoxID = mailBoxID,
                    SupportDocumentItems = supportDocumentItems,
                    CreatedBy = actionBy
                }, commandType: System.Data.CommandType.StoredProcedure).ToInt();
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Rollback mail by id
        /// </summary>
        /// <param name="mailBoxID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int RollBackMail (Guid mailBoxID) {
            int result = 0;
            try {
                result = ExecuteScalar("uspRollbackEmail", new { MailBoxID = mailBoxID }, commandType: System.Data.CommandType.StoredProcedure).ToInt();
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Get mail inbox list
        /// </summary>
        /// <param name="toContactID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<InboxList> GetInboxList (Guid toContactID) {
            List<InboxList> inboxList = new List<InboxList>();
            try {
                inboxList = GetList<InboxList>("uspGetInboxLIst", _sqlI18N, new { ToContactID = toContactID }, commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return inboxList;
        }

        /// <summary>
        /// Get mail attachments
        /// </summary>
        /// <param name="mailBoxID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<GetMailAttachments> GetMailAttachmentList (Guid mailBoxID) {
            List<GetMailAttachments> getMail = new List<GetMailAttachments>();
            string query = "SELECT FileName,ContentType,ContentUrl FROM dbo.tfnGetMailAttachments(@MailBoxID)";
            try {
                getMail = GetList<GetMailAttachments>(query, _sqlI18N, new { MailBoxID = mailBoxID });
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return getMail;
        }

        /// <summary>
        /// Delete mail
        /// </summary>
        /// <param name="mailBoxID"></param>
        /// <param name="actionBy"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int DeleteMail (Guid mailOutBoxID) {
            int result = 0;
            try {
                result = ExecuteScalar("uspDeleteEmail", new { MailOutBoxID = mailOutBoxID }, commandType: System.Data.CommandType.StoredProcedure).ToInt();
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Rollback mail
        /// </summary>
        /// <param name="mailBoxID"></param>
        /// <param name="actionBy"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int RollBackEmail (Guid mailOutBoxID) {
            int result = 0;
            try {
                result = ExecuteScalar("uspRollBackEmail", new { MailOutBoxID = mailOutBoxID }, commandType: System.Data.CommandType.StoredProcedure).ToInt();
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Read Email
        /// </summary>
        /// <param name="mailBoxID"></param>
        /// <param name="actionBy"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int ReadEmail (Guid mailOutBoxID) {
            int result = 0;
            try {
                result = ExecuteScalar("uspReadEmail", new { MailOutBoxID = mailOutBoxID }, commandType: System.Data.CommandType.StoredProcedure).ToInt();
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Get deleted mail list
        /// </summary>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<DeletedMailList> DeletedMailList (Guid toContactID) {
            List<DeletedMailList> mailLists = new List<DeletedMailList>();
            try {
                mailLists = GetList<DeletedMailList>("uspGetDeletedMailList", _sqlI18N, new { ToContactID = toContactID }, commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return mailLists;
        }

        /// <summary>
        /// Get mail send list
        /// </summary>
        /// <param name="fromContactID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public DataSet GetSendList (Guid fromContactID) {
            try {
                return GetDataSet("uspSendboxList", new { FromContactID = fromContactID }, commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Get documents from database
        /// </summary>
        /// <param name="fileID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public DownloadDocument FileDownLoad (Guid fileID) {
            DownloadDocument result = new DownloadDocument();
            try {
                string query = "select FileName,FileExtensionType as MIMEType,AttachmentContent as Content from dbo.MailAttachment WHERE AttachmentID=@DocumentID";
                var dataRow = GetDataRow(query, new { DocumentID = fileID });
                if (dataRow != null) {
                    result.Content = (byte[])dataRow["Content"];
                    result.MIMEType = (string)dataRow["MIMEType"];
                    result.FileName = (string)dataRow["FileName"];
                }
                return result;
            }
            catch (Exception ex) {
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Get unread mail count
        /// </summary>
        /// <param name="contactID"></param>
        /// <param createdBy="Sakthivel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int GetMailCount (Guid contactID) {
            int count = 0;
            string query = "SELECT dbo.sfnGetNewMailCount(@ContactID)";
            try {
                count = ExecuteScalar(query, new { ContactID = contactID }).ToInt();
            }
            catch (SqlException ex) {
                throw new Exception(ex.Message);
            }
            return count;
        }
        #endregion
    }
}
