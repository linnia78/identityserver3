using Microsoft.AspNet.Identity;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace IdentityServer.Library.Service
{
    public class EmailService : IIdentityMessageService
    {
        public async Task SendAsync(IdentityMessage message)
        {

            var apiKey = ConfigurationManager.AppSettings.Get("SendgridApiKey");
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(ConfigurationManager.AppSettings.Get("EmailFrom"), "IndentityServer");
            var subject = message.Subject;
            var to = new EmailAddress(message.Destination);
            //var plainTextContent = "and easy to do anywhere, even with C#";
            var htmlContent = message.Body;
            var msg = MailHelper.CreateSingleEmail(from, to, subject, null, htmlContent);
            var response = await client.SendEmailAsync(msg);
        }
    }
}