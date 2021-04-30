namespace Merwylan.ExampleApi.Mail
{
    public class MailConfig
    {
        public string Sender { get; set; }
        public string SenderPassword { get; set; }
        public int Port { get; set; }
        public string SmtpServer { get; set; }
        public string Recipients { get; set; }
    }
}
