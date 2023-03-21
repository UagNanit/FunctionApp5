using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using FunctionApp5.Services;

namespace FunctionApp5.Repository
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            var client = new SmtpClient("smtp.sendgrid.net", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("{secret}", "{secret}")
            };

            return client.SendMailAsync(
                new MailMessage(from: "{secret}",
                                to: email,
                                subject,
                                message
                                ));
        }
    }
}

