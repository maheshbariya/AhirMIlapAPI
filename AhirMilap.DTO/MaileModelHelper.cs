namespace AhirMilap.DTO
{
    public class MaileModelHelper
    {
        public MaileModelHelper(string to, string cc, string subject, string message, bool isBodyHtml, string attachment, string attachmentName)
        {
            To = to;
            CC = cc;
            Subject = subject;
            Message = message;
            IsBodyHtml = isBodyHtml;
            Attachment = attachment;
            AttachmentName = attachmentName;
        }
        public string To
        {
            get;
        } 
        public string CC
        {
            get;
        }
        public string Subject
        {
            get;
        }
        public string Message
        {
            get;
        }
        public bool IsBodyHtml
        {
            get;
        }
        public string Attachment
        {
            get;
        }
        public string AttachmentName
        {
            get;
        }
    } 
}
