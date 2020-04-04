using System.Threading.Tasks;

namespace Supertext.Base.Net.Mail
{
    public interface IMailService
    {
        Task SendAsync(EmailInfo mail);

        Task SendAsHtml(EmailInfo mail);
    }
}