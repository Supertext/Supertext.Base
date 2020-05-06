using System.Threading.Tasks;

namespace Supertext.Base.Net.Mail
{
    public interface IMailService
    {
        Task SendAsync(EmailInfo mail);

        Task SendAsHtmlAsync(EmailInfo mail);
    }
}