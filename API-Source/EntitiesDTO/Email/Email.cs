using Dawn.Utilities.i18N;
using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;
using Utilities.Attributes;

namespace EntitiesDTO.Email
{
    public class Document
    {
        public IFormFile Content { get; set; }
    }

    public class SupportDocumentItems
    {
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public byte[] Content { get; set; } = new byte[0];
    }

    public class CreateMail
    {
        public string Subject { get; set; } = string.Empty;
        public string MailContent { get; set; } = string.Empty;
        public List<ToMail> ToContactID { get; set; } = new List<ToMail>();
        [MaxFileSize(2, ErrorMessage = "The file size should be 5MB")]
        public List<IFormFile>? Attachment { get; set; } = new List<IFormFile>();
    }

    public class CreateNewMail
    {
        public Guid FromContactID { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string MailContent { get; set; } = string.Empty;
        public Guid CreatedBy { get; set; }
    }

    public class ToMail
    {
        public Guid ToContactID { get; set; }
    }

    public class GetMailBoxID
    {
        public Guid MailBoxID { get; set; }
    }

    public class InboxList
    {
        public Guid MailBoxID { get; set; }
        public Guid MailOutBoxID { get; set; }
        public Guid FromContactID { get; set; }
        //public DateTime Date { get; set; }
        public string Date { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }

    public class GetMailAttachments
    {
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public string ContentUrl { get; set; } = string.Empty;
    }

    public class MailInboxList
    {
        public Guid MailOutBoxID { get; set; }
        public string FranchiseeID { get; set; } = string.Empty;
        public string BranchName { get; set; } = string.Empty;
        public string Address1 { get; set; } = string.Empty;
        public string Address2 { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string Pincode { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public List<GetMailAttachments> Attachments { get; set; } = new List<GetMailAttachments>();
    }

    public class DeletedMailList
    {
        public Guid MailBoxID { get; set; }
        public Guid MailOutBoxID { get; set; }
        public Guid FromContactID { get; set; }
        public string Subject { get; set; } = string.Empty;
        public DateI18 DateI18 { get; set; } = new DateI18();
        public string Date { get { return DateI18.ToString() ?? string.Empty; } set { DateI18 = value; } }
    }

    public class NewDeletedMailList
    {
        public Guid MailBoxID { get; set; }
        public Guid MailOutBoxID { get; set; }
        public string FranchiseeID { get; set; } = string.Empty;
        public string BranchName { get; set; } = string.Empty;
        public string Address1 { get; set; } = string.Empty;
        public string Address2 { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string Pincode { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
    }

    public class SendList
    {
        public ContactInfos To { get; set; } = new ContactInfos();
        public Guid MailBoxID { get; set; }
        public string Date { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public List<MailAttachments> Attachments { get; set; } = new List<MailAttachments>();
    }

    public class ContactInfos
    {
        public string FranchiseeID { get; set; } = string.Empty;
        public string BranchName { get; set; } = string.Empty;
        public string Address1 { get; set; } = string.Empty;
        public string Address2 { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string Pincode { get; set; } = string.Empty;
    }

    public class MailInfo
    {
        public Guid MailBoxID { get; set; }
        [JsonIgnore]
        public DateI18 DateI18 { get; set; } = new DateI18();
        public string Date { get { return DateI18.ToString() ?? string.Empty; } set { DateI18 = value; } }
        public string Subject { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }

    public class ToInfo
    {
        public Guid MailBoxID { get; set; }
        public Guid ToContactID { get; set; }
    }

    public class MailAttachments
    {
        public Guid MailBoxID { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public string ContentUrl { get; set; } = string.Empty;
    }

    public class DownloadDocument
    {
        public string FileName { get; set; } = string.Empty;
        public string MIMEType { get; set; } = string.Empty;
        public byte[] Content { get; set; }
    }
}
