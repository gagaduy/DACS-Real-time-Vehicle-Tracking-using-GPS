using Microsoft.Extensions.Options;
using DACS.Models;
using System.Net;
using System.Net.Mail;

namespace DACS.Services
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly MailSettings _mailSettings;

        public EmailService(ILogger<EmailService> logger, IOptions<MailSettings> mailSettings)
        {
            _logger = logger;
            _mailSettings = mailSettings.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            try
            {
                using var client = new SmtpClient(_mailSettings.SmtpServer, _mailSettings.Port)
                {
                    Credentials = new NetworkCredential(_mailSettings.SenderEmail, _mailSettings.AppPassword),
                    EnableSsl = true
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_mailSettings.SenderEmail, _mailSettings.SenderName),
                    Subject = subject,
                    Body = htmlMessage,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(email);

                await client.SendMailAsync(mailMessage);
                _logger.LogInformation("Email sent successfully to {Email}", email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email to {Email}", email);
                throw;
            }
        }
    }
}
