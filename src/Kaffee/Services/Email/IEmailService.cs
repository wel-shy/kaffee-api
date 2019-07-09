using System.Collections.Generic;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Kaffee.Services.Email
{
    public interface IEmailService
    {
        Task SendEmail(MailAddress address, string subject, Dictionary<string, string> content);
    }
}