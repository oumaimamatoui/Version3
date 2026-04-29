using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using NeoEvaluation.API.Data;
using NeoEvaluation.API.Models;
using Microsoft.EntityFrameworkCore;

namespace NeoEvaluation.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class GoogleAuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly AppDbContext _context;

        public GoogleAuthController(IConfiguration config, AppDbContext context)
        {
            _config = config;
            _context = context;
        }

        [HttpGet("auth-url")]
        public IActionResult GetAuthUrl()
        {
            var clientId = _config["GoogleAuthSettings:ClientId"];
            var redirectUri = _config["GoogleAuthSettings:RedirectUri"];
            
            // On demande l'accès pour envoyer des emails + mode offline pour avoir le Refresh Token
            var scopes = new[] { 
                "https://www.googleapis.com/auth/gmail.send", 
                "https://www.googleapis.com/auth/userinfo.email" 
            };

            var url = $"https://accounts.google.com/o/oauth2/v2/auth?" +
                      $"client_id={clientId}&" +
                      $"redirect_uri={redirectUri}&" +
                      $"response_type=code&" +
                      $"scope={string.Join(" ", scopes)}&" +
                      $"access_type=offline&" +
                      $"prompt=consent";

            return Ok(new { url });
        }

        [HttpPost("callback")]
        public async Task<IActionResult> Callback([FromBody] GoogleCallbackDto dto)
        {
            try
            {
                var clientId = _config["GoogleAuthSettings:ClientId"];
                var clientSecret = _config["GoogleAuthSettings:ClientSecret"];
                var redirectUri = _config["GoogleAuthSettings:RedirectUri"];

                var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
                {
                    ClientSecrets = new ClientSecrets { ClientId = clientId, ClientSecret = clientSecret }
                });

                TokenResponse token = await flow.ExchangeCodeForTokenAsync("user", dto.Code, redirectUri, CancellationToken.None);

                // Récupérer l'utilisateur actuel
                var userEmail = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Email || c.Type == "email")?.Value;
                var user = await _context.Utilisateurs.Include(u => u.Entreprise).FirstOrDefaultAsync(u => u.Email == userEmail);

                if (user == null)
                    return BadRequest(new { message = "Utilisateur non trouvé." });

                if (user.RoleNom == "SuperAdmin")
                {
                    // Pour le SuperAdmin, on peut stocker dans une entreprise spéciale "Système" ou dans les settings
                    // Ici on va créer une ligne "Système" dans Entreprise si elle n'existe pas pour stocker le Gmail Global
                    var systemOrg = await _context.Entreprises.IgnoreQueryFilters().FirstOrDefaultAsync(e => e.Nom == "SYSTEM_PLATFORM");
                    if (systemOrg == null)
                    {
                        systemOrg = new Entreprise { Nom = "SYSTEM_PLATFORM", Plan = "System" };
                        _context.Entreprises.Add(systemOrg);
                    }
                    
                    systemOrg.GmailRefreshToken = token.RefreshToken ?? systemOrg.GmailRefreshToken;
                    systemOrg.GmailAccessToken = token.AccessToken;
                    systemOrg.GmailTokenExpiresAt = DateTime.UtcNow.AddSeconds(token.ExpiresInSeconds ?? 3600);
                    systemOrg.GmailEmail = user.Email; // Optionnel
                }
                else if (user.Entreprise != null)
                {
                    // Sauvegarder dans la table Entreprise de l'utilisateur
                    var entreprise = user.Entreprise;
                    entreprise.GmailRefreshToken = token.RefreshToken ?? entreprise.GmailRefreshToken;
                    entreprise.GmailAccessToken = token.AccessToken;
                    entreprise.GmailTokenExpiresAt = DateTime.UtcNow.AddSeconds(token.ExpiresInSeconds ?? 3600);
                    entreprise.GmailEmail = user.Email;
                }
                else
                {
                    return BadRequest(new { message = "Impossible d'associer le compte Gmail (Ni SuperAdmin, ni Entreprise)." });
                }
                
                await _context.SaveChangesAsync();

                return Ok(new { message = "Compte Gmail connecté avec succès !" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Erreur OAuth : " + ex.Message });
            }
        }
    }

    public class GoogleCallbackDto
    {
        public string Code { get; set; } = string.Empty;
    }
}
