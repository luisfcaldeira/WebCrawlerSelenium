using Azure;
using Azure.Communication.Email;
using Crawler.Domain.Interfaces.Services.Infra;

namespace Crawler.Services.Infra
{
    public class EmailSender : IEmailSender
    {
        private string connectionString;

        public EmailSender(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public void SendEmail(string toEmail, string sender, string subject, string message, string planText)
        {
            var emailClient = new EmailClient(connectionString);

            EmailSendOperation emailSendOperation = emailClient.Send(
                WaitUntil.Started,
                senderAddress: sender,
                recipientAddress: toEmail,
                subject: subject,
                htmlContent: message,
                plainTextContent: planText);

        }
    }
}
