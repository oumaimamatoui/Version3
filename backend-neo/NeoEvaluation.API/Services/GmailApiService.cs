using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using NeoEvaluation.API.Data;
using NeoEvaluation.API.Models;

namespace NeoEvaluation.API.Services
{
    public class GmailApiService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly AppDbContext _context;
        private readonly ITenantService _tenantService;

        public GmailApiService(IConfiguration config, AppDbContext context, ITenantService tenantService)
        {
            _config = config;
            _context = context;
            _tenantService = tenantService;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var tenantId = _tenantService.GetTenantId();

            // 1. Tenter d'utiliser le Gmail de l'entreprise si on est dans un contexte entreprise
            if (tenantId != null)
            {
                var entreprise = await _context.Entreprises.IgnoreQueryFilters()
                    .FirstOrDefaultAsync(e => e.Id == tenantId.Value);
                
                if (entreprise != null && !string.IsNullOrEmpty(entreprise.GmailRefreshToken))
                {
                    Console.WriteLine($"[EMAIL STRATEGY] Enterprise context -> Using Company Gmail ({entreprise.Nom})");
                    await SendUsingGmailApi(entreprise, to, subject, body);
                    return; // Succès
                }
            }

            // 2. Fallback ou Contexte Système : Utiliser le Master Gmail de la plateforme
            var systemOrg = await _context.Entreprises.IgnoreQueryFilters()
                .FirstOrDefaultAsync(e => e.Nom == "SYSTEM_PLATFORM");

            if (systemOrg != null && !string.IsNullOrEmpty(systemOrg.GmailRefreshToken))
            {
                Console.WriteLine($"[EMAIL STRATEGY] Fallback -> Using Master Platform Gmail account");
                await SendUsingGmailApi(systemOrg, to, subject, body);
            }
            else
            {
                // Échec ultime si même le Master n'est pas là
                throw new InvalidOperationException("CRITICAL: Aucun compte Gmail n'est connecté (ni Entreprise, ni Système). Envoi impossible.");
            }
        }

        private async Task SendUsingGmailApi(Entreprise target, string to, string subject, string body)
        {
            var clientId = _config["GoogleAuthSettings:ClientId"];
            var clientSecret = _config["GoogleAuthSettings:ClientSecret"];

            var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets { ClientId = clientId, ClientSecret = clientSecret }
            });

            Console.WriteLine($"[DEBUG GMAIL] Début envoi à {to}");
            var tokenResponse = new TokenResponse
            {
                RefreshToken = target.GmailRefreshToken,
                AccessToken = target.GmailAccessToken,
                ExpiresInSeconds = target.GmailTokenExpiresAt.HasValue && target.GmailTokenExpiresAt > DateTime.UtcNow
                    ? (long?)(target.GmailTokenExpiresAt.Value - DateTime.UtcNow).TotalSeconds 
                    : 0,
                IssuedUtc = DateTime.UtcNow
            };
            Console.WriteLine($"[DEBUG GMAIL] ExpiresIn: {tokenResponse.ExpiresInSeconds}");

            var credential = new UserCredential(flow, "user", tokenResponse);

            // Refresh automatique si expiré
            if (credential.Token.IsStale)
            {
                await credential.RefreshTokenAsync(CancellationToken.None);
                
                target.GmailAccessToken = credential.Token.AccessToken;
                
                // Sécurité : Vérifier que ExpiresInSeconds est un nombre positif raisonnable
                long secondsToAdd = credential.Token.ExpiresInSeconds ?? 3600;
                Console.WriteLine($"[DEBUG GMAIL] Refresh réussi. Seconds to add: {secondsToAdd}");
                
                if (secondsToAdd < 0) secondsToAdd = 0;
                if (secondsToAdd > 31536000) secondsToAdd = 31536000; // Max 1 an

                target.GmailTokenExpiresAt = DateTime.UtcNow.AddSeconds(secondsToAdd);
                Console.WriteLine($"[DEBUG GMAIL] Nouvelle expiration: {target.GmailTokenExpiresAt}");
                
                // On utilise une instance de context fraîche ou on attache l'entité
                _context.Update(target);
                await _context.SaveChangesAsync();
                Console.WriteLine("[DEBUG GMAIL] Entreprise mise à jour en DB.");
            }

            var service = new GmailService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "EvaluaTech",
            });

            var mimeMessage = new MimeMessage();
            mimeMessage.From.Add(new MailboxAddress("EvaluaTech", target.GmailEmail ?? "noreply@evaluatech.tn"));
            mimeMessage.To.Add(MailboxAddress.Parse(to));
            mimeMessage.Subject = subject;
            mimeMessage.Body = new BodyBuilder { HtmlBody = body }.ToMessageBody();

            var msg = new Message();
            using (var ms = new MemoryStream())
            {
                mimeMessage.WriteTo(ms);
                msg.Raw = Convert.ToBase64String(ms.ToArray())
                    .Replace('+', '-')
                    .Replace('/', '_')
                    .Replace("=", "");
            }

            await service.Users.Messages.Send(msg, "me").ExecuteAsync();
        }
    }
}
