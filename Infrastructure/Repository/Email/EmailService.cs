using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Application.ServicesInterfaces;
using Application.ServicesInterfaces.Email;
using Application.Settings;

namespace Infrastructure.Services
{
    internal class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(EmailSettings settings)
        {
            _settings = settings;
        }

        public async Task SendMessageAsync(string toEmail, string message)
        {
            var fromAddress = new MailAddress(_settings.FromAddress, _settings.DisplayName);
            var toAddress = new MailAddress(toEmail);

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_settings.FromAddress, _settings.AppPassword)
            };

            using var mail = new MailMessage(fromAddress, toAddress)
            {
                Subject = _settings.Subject,
                Body = message
            };

            await smtp.SendMailAsync(mail);
        }
    }
}