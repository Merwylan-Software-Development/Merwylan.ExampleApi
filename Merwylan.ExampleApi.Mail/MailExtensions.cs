using System.Collections.Generic;
using System.Net.Mail;

namespace Merwylan.ExampleApi.Mail
{
    public static class MailExtensions
    {
        public static bool TryAdd(this ICollection<Attachment> attachments, Attachment attachment)
        {
            try
            {
                attachments.Add(attachment);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
