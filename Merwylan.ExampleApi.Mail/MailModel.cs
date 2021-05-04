using System.Collections.Generic;

namespace Merwylan.ExampleApi.Mail
{
    public class MailModel
    {
        public string Message { get; set; }
        public string Subject { get; set; }
        public IList<string> AttachmentFilePaths { get; set; } = new List<string>();
        public IList<string> ExtraRecipients { get; set; } = new List<string>();
    }
}