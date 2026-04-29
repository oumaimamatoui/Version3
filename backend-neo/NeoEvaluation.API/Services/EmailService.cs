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
            
            // On utilise le bloc SmtpSettings du appsettings.json
            var smtpServer = _config["SmtpSettings:Host"] ?? "smtp.gmail.com";
            var smtpPort = int.Parse(_config["SmtpSettings:Port"] ?? "587");
            var smtpUser = _config["SmtpSettings:SmtpUser"] ?? throw new Exception("SmtpUser missing in SmtpSettings");
            var smtpPass = _config["SmtpSettings:SmtpPass"] ?? throw new Exception("SmtpPass missing in SmtpSettings");
            var enableSsl = bool.Parse(_config["SmtpSettings:EnableSsl"] ?? "true");

            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(senderName, senderEmail));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;

            var builder = new BodyBuilder { HtmlBody = body };
            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            // On utilise StartTls si le port est 587
            await smtp.ConnectAsync(smtpServer, smtpPort, enableSsl ? MailKit.Security.SecureSocketOptions.StartTls : MailKit.Security.SecureSocketOptions.None);
            await smtp.AuthenticateAsync(smtpUser, smtpPass);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}
