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
            
            // Création d'un message complet (Plain Text + HTML)
            var msg = MailHelper.CreateSingleEmail(from, recipient, subject, body, body);
            
            // Désactiver le tracking pour éviter d'être bloqué par certains filtres/firewalls locaux
            msg.SetClickTracking(false, false);
            msg.SetOpenTracking(false);
            
            var response = await _client.SendEmailAsync(msg);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Body.ReadAsStringAsync();
                Console.WriteLine("\n❌ [SENDGRID REJECTED THE EMAIL]");
                Console.WriteLine($"STATUS: {response.StatusCode}");
                Console.WriteLine($"ERROR: {errorBody}");
                Console.WriteLine("CONSEIL: Vérifiez que l'email 'FromEmail' est bien 'Verified' dans votre dashboard SendGrid (Single Sender Verification).\n");
                
                throw new Exception($"SendGrid Error: {response.StatusCode} - {errorBody}");
            }
        }
    }
}
