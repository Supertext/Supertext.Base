namespace Supertext.Base.Net.Mail
{
    public interface IMailService
    {
        void Send(EmailInfo mail);

        void SendAsHtml(EmailInfo mail);
    }
}