using System.Threading.Tasks;

namespace FunctionApp5.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
