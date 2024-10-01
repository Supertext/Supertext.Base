using System.Threading;
using System.Threading.Tasks;

namespace Supertext.Base.Net.Mail
{
    public interface IMailService
    {
        Task SendAsync(EmailInfo mail, CancellationToken ct = default);

        Task SendAsHtmlAsync(EmailInfo mail, CancellationToken ct = default);

        Task SendUsingTemplateAsync(EmailInfoTemplates mailInfo, CancellationToken ct = default);
    }
}