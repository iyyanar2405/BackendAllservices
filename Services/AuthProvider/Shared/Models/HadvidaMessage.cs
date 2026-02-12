using Hadvida.EmailService.Models;
using System.ComponentModel.DataAnnotations;

namespace Hadvida.EmailService.Models
{
    public class HadvidaMessage
    {
        /// <summary>
        /// List of "To" Emails
        /// </summary>
        public int? SiteId { get; set; }
        /// <summary>
        /// Message type Email is default if not supplied
        /// 0=Email, 1=Mobile, 2=Notification
        /// </summary>
        public MessageType MessageType { get; set; } = MessageType.Email;
        /// <summary>
        /// List of "To" Emails
        /// </summary>
        public List<string>? To { get; set; }
        /// <summary>
        /// List of Emails to be in CC
        /// </summary>
        public List<string>? Cc { get; set; }
        /// <summary>
        /// List of Emails to be in BCC
        /// </summary>
        public List<string>? Bcc { get; set; }
        /// <summary>
        /// From Email
        /// </summary>
        public string From { get; set; } = "no-reply@Hadvida.com";
        /// <summary>
        /// From Name
        /// </summary>
        public string? FromName { get; set; } = "Hadvida";
        /// <summary>
        /// Reply To Email
        /// </summary>
        public string? ReplyTo { get; set; }
        /// <summary>
        /// Subject of the Email
        /// </summary>
        public string? Subject { get; set; }
        /// <summary>
        /// HTML Template to be sent 
        /// </summary>
        public int? MessageTemplateId { get; set; }
        /// <summary>
        /// Pass a HTML Body thats not currently in the template table
        /// </summary>
        public string? MessageBody { get; set; }
        /// <summary>
        /// List of Text Replacements
        /// The values will be replaced with the given keys in the provided HTML Template
        /// </summary>
        public Dictionary<string, string>? TemplateReplacements { get; set; }
        /// <summary>
        /// Will the messages be sent Individually or as group
        /// </summary>
        public bool SendEachUserSeparateEmails { get; set; } = false;
        /// <summary>
        /// Will we add the support user to the group email
        /// </summary>
        public bool AddSupport { get; set; } = false;
        /// <summary>
        /// will allow a companywide generic email to be sent across sites.
        /// </summary>
        public bool isMultiSite { get; set; } = false;
    }
}
