using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Kaffee.Settings;

namespace Kaffee.Services.Email
{
    public class GmailEmailService : IEmailService
    {
        private readonly GmailSettings _gmailSettings;

        public GmailEmailService(GmailSettings _gmailSettings)
        {
            this._gmailSettings = _gmailSettings;
        }

        public async Task SendEmail(MailAddress address, string subject, Dictionary<string, string> fields)
        {
            var template = await EmailTemplateService.GetTemplate(Models.EmailTemplate.Welcome, fields);

            var fromAddress = new MailAddress(_gmailSettings.Username, "Kaffee");
            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Credentials = new NetworkCredential(fromAddress.Address, _gmailSettings.Password),
                Timeout = 20000
            };
            using (var message = new MailMessage(fromAddress, address)
                {
                    Subject = subject,
                    Body = template,
                    IsBodyHtml = true
                }
            )
            {
                smtp.Send(message);
            }
        }
    }
}