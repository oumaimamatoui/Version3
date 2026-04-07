using SendGrid;
using SendGrid.Helpers.Mail;

namespace NeoEvaluation.API.Services
{
    public class SendGridEmailService : IEmailService
    {
        private readonly ISendGridClient _client;
        private readonly IConfiguration _config;

        public SendGridEmailService(ISendGridClient client, IConfiguration config)
        {
            _client = client;
            _config = config;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var fromEmail = _config["SendGridSettings:FromEmail"];
            var fromName = _config["SendGridSettings:FromName"];

            var from = new EmailAddress(fromEmail, fromName);
            var recipient = new EmailAddress(to);
            var msg = MailHelper.CreateSingleEmail(from, recipient, subject, null, body);
            
            var response = await _client.SendEmailAsync(msg);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Body.ReadAsStringAsync();
                throw new Exception($"SendGrid Error: {response.StatusCode} - {errorBody}");
            }
        }
    }
}
