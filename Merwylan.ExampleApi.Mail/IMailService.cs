﻿namespace Merwylan.ExampleApi.Mail
{
    public interface IMailService
    {
        void SendMail(MailModel model);

        void FireAndForgetMail(MailModel model);
    }
}