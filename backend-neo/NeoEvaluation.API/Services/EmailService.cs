using MailKit.Net.Smtp;
using MimeKit;

namespace NeoEvaluation.API.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var senderName = _config["EmailSettings:SenderName"] ?? "NeoEvaluation";
            var senderEmail = _config["EmailSettings:SenderEmail"] ?? "test@neoevaluation.com";
            var smtpServer = _config["EmailSettings:SmtpServer"] ?? "sandbox.smtp.mailtrap.io";
            var smtpPort = int.Parse(_config["EmailSettings:SmtpPort"] ?? "2525");
            var smtpUser = _config["EmailSettings:SmtpUser"] ?? throw new Exception("SmtpUser missing");
            var smtpPass = _config["EmailSettings:SmtpPass"] ?? throw new Exception("SmtpPass missing");

            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(senderName, senderEmail));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;

            var builder = new BodyBuilder { HtmlBody = body };
            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(smtpServer, smtpPort, false);
            await smtp.AuthenticateAsync(smtpUser, smtpPass);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}
