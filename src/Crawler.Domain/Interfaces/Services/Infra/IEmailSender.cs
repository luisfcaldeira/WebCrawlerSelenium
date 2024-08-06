namespace Crawler.Domain.Interfaces.Services.Infra
{
    public interface IEmailSender
    {
        void SendEmail(string toEmail, string sender, string subject, string message, string planText);
    }
}
