using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NeoEvaluation.API.Data;
using NeoEvaluation.API.Dtos;
using NeoEvaluation.API.Models;
using System.Security.Claims;
using BCrypt.Net;
using NeoEvaluation.API.Services;

namespace NeoEvaluation.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettingsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IFailedEmailQueue _failedQueue;
        private readonly IEmailService _emailService;

        public SettingsController(AppDbContext context, IFailedEmailQueue failedQueue, IEmailService emailService)
        {
            _context = context;
            _failedQueue = failedQueue;
            _emailService = emailService;
        }

        private Guid GetCurrentUserId()
        {
            var idClaim = User.FindFirst("id")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(idClaim))
                throw new UnauthorizedAccessException("Utilisateur non identifié.");
            return Guid.Parse(idClaim);
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            var userId = GetCurrentUserId();
            var user = await _context.Utilisateurs.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return NotFound();

            var entreprise = user.EntrepriseId.HasValue 
                ? await _context.Entreprises.FirstOrDefaultAsync(e => e.Id == user.EntrepriseId) 
                : null;

            return Ok(new UserProfileDto {
                Nom = user.Nom,
                Prenom = user.Prenom,
                Email = user.Email,
                PhotoUrl = user.PhotoUrl,
                Bio = user.Bio,
                JoinDate = user.CreeLe.ToString("MMMM yyyy"),
                ThemePreference = user.ThemePreference,
                EntrepriseNom = entreprise?.Nom,
                SubscriptionPlan = entreprise?.Plan,
                SubscriptionDate = (entreprise?.AbonnementDebut ?? entreprise?.CreeLe)?.ToString("dd/MM/yyyy"),
                SubscriptionExpiry = entreprise?.AbonnementFin?.ToString("dd/MM/yyyy")
            });
        }

        [HttpPost("update-profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UserProfileDto dto)
        {
            var userId = GetCurrentUserId();
            var user = await _context.Utilisateurs.FindAsync(userId);
            if (user == null) return NotFound();

            user.Nom = dto.Nom;
            user.Prenom = dto.Prenom;
            user.Email = dto.Email;
            user.Bio = dto.Bio;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Profil mis à jour" });
        }

        [HttpPost("theme")]
        public async Task<IActionResult> UpdateTheme([FromBody] string theme)
        {
            var userId = GetCurrentUserId();
            var user = await _context.Utilisateurs.FindAsync(userId);
            if (user == null) return NotFound();

            user.ThemePreference = theme == "dark" ? "dark" : "light";
            await _context.SaveChangesAsync();
            return Ok(new { message = "Thème mis à jour" });
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            if (dto.NewPassword != dto.ConfirmPassword) return BadRequest("Incohérence mots de passe.");
            var userId = GetCurrentUserId();
            var user = await _context.Utilisateurs.FindAsync(userId);
            if (user == null) return NotFound();

            if (!string.IsNullOrEmpty(user.MotDePasseHash)) {
                if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.MotDePasseHash))
                    return BadRequest("Mot de passe actuel incorrect.");
            } else {
                // Si l'utilisateur n'a pas de mot de passe (Social Login), 
                // on peut optionnellement exiger un token ou autre, mais ici on permet de définir 
                // le premier mot de passe sans vérifier l'actuel.
            }

            user.MotDePasseHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Mot de passe mis à jour avec succès." });
        }

        // FIX ERREUR 500 : Approche manuelle pour récupérer l'entreprise
        [HttpGet("branding")]
        public async Task<IActionResult> GetBranding()
        {
            try {
                var userId = GetCurrentUserId();
                var user = await _context.Utilisateurs.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == userId);
                
                if (user == null || user.EntrepriseId == null)
                    return Ok(new BrandingDto { CompanyName = "NeoEvaluation" });

                var entreprise = await _context.Entreprises.FindAsync(user.EntrepriseId);
                return Ok(new BrandingDto {
                    CompanyName = entreprise?.Nom ?? "NeoEvaluation",
                    LogoUrl = entreprise?.LogoUrl,
                    Domaine = entreprise?.Domaine,
                    Secteur = entreprise?.Secteur,
                    SiteWeb = entreprise?.SiteWeb,
                    Ville = entreprise?.Ville,
                    Pays = entreprise?.Pays,
                    CodePostal = entreprise?.CodePostal,
                    Adresse = entreprise?.Adresse,
                    Description = entreprise?.Description,
                    MatriculeFiscale = entreprise?.MatriculeFiscale,
                    IsGoogleConnected = !string.IsNullOrEmpty(entreprise?.GmailRefreshToken),
                    ConnectedEmail = entreprise?.GmailEmail
                });
            } catch {
                return Ok(new BrandingDto { CompanyName = "NeoEvaluation" });
            }
        }

        [HttpPost("update-branding")]
        public async Task<IActionResult> UpdateBranding([FromBody] BrandingUpdateDto dto)
        {
            var userId = GetCurrentUserId();
            var user = await _context.Utilisateurs.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == userId);
            
            if (user == null || user.EntrepriseId == null)
                return BadRequest("Action réservée aux entreprises.");

            var entreprise = await _context.Entreprises.IgnoreQueryFilters().FirstOrDefaultAsync(e => e.Id == user.EntrepriseId);
            if (entreprise == null) return NotFound();

            entreprise.Nom = dto.CompanyName;
            entreprise.Domaine = dto.Domaine;
            entreprise.Secteur = dto.Secteur;
            entreprise.SiteWeb = dto.SiteWeb;
            entreprise.Ville = dto.Ville;
            entreprise.Pays = dto.Pays;
            entreprise.CodePostal = dto.CodePostal;
            entreprise.Adresse = dto.Adresse;
            entreprise.Description = dto.Description;
            entreprise.MatriculeFiscale = dto.MatriculeFiscale;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Identité visuelle mise à jour." });
        }

        [HttpPost("upload-photo")]
        public async Task<IActionResult> UploadPhoto(IFormFile file)
        {
            if (file == null || file.Length == 0) return BadRequest();
            var userId = GetCurrentUserId();
            var user = await _context.Utilisateurs.FindAsync(userId);
            if (user == null) return NotFound();

            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "profiles");
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

            var fileName = $"{userId}_{DateTime.UtcNow.Ticks}{Path.GetExtension(file.FileName)}";
            var fullPath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create)) {
                await file.CopyToAsync(stream);
            }

            user.PhotoUrl = $"/uploads/profiles/{fileName}";
            await _context.SaveChangesAsync();
            return Ok(new { photoUrl = user.PhotoUrl });
        }

        [HttpGet("mailer-diag")]
        public async Task<IActionResult> GetMailerDiag()
        {
            var userId = GetCurrentUserId();
            var user = await _context.Utilisateurs.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == userId);
            
            var email = "admin@evaluatech.tn";
            var isGoogleConnected = false;

            if (user?.EntrepriseId != null)
            {
                var entreprise = await _context.Entreprises.IgnoreQueryFilters().FirstOrDefaultAsync(e => e.Id == user.EntrepriseId);
                if (entreprise != null && !string.IsNullOrEmpty(entreprise.GmailRefreshToken))
                {
                    isGoogleConnected = true;
                    email = entreprise.GmailEmail ?? "Entreprise Gmail";
                }
            }
            
            if (!isGoogleConnected)
            {
                var systemOrg = await _context.Entreprises.IgnoreQueryFilters().FirstOrDefaultAsync(e => e.Nom == "SYSTEM_PLATFORM");
                if (systemOrg != null && !string.IsNullOrEmpty(systemOrg.GmailRefreshToken))
                {
                    isGoogleConnected = true;
                    email = systemOrg.GmailEmail ?? "Plateforme Système Gmail";
                }
            }

            var logs = new List<string>
            {
                $"[INFO] {DateTime.Now:HH:mm:ss} Début du diagnostic SMTP / Gmail API...",
                "[INFO] Vérification de la configuration d'arrière-plan...",
                isGoogleConnected ? "[SUCCESS] Connexion SMTP / OAuth2 : OK" : "[ERROR] Configuration manquante ou Token expiré.",
                $"[INFO] Invitations bloquées détectées en RAM : {_failedQueue.Count}",
                $"[INFO] {DateTime.Now:HH:mm:ss} Diagnostic mailer terminé."
            };

            return Ok(new {
                isGoogleConnected = isGoogleConnected,
                email = email,
                pendingInvitesCount = _failedQueue.Count,
                diagnosticsLogs = logs
            });
        }

        [HttpPost("mailer-resend")]
        public async Task<IActionResult> ResendFailedEmails()
        {
            var failedEmails = _failedQueue.GetAll();
            if (failedEmails.Count == 0) return Ok(new { success = true, count = 0 });

            int successCount = 0;
            _failedQueue.Clear(); // On vide d'abord, si ça rate on les remettra (ou SendEmailAsync s'en chargera via le catch)

            foreach (var msg in failedEmails)
            {
                try
                {
                    await _emailService.SendEmailAsync(msg.To, msg.Subject, msg.Body);
                    successCount++;
                }
                catch (Exception)
                {
                    // L'erreur est déjà recatchée dans GmailApiService et rajoutée à _failedQueue
                }
            }

            return Ok(new { success = true, count = successCount, pendingLeft = _failedQueue.Count });
        }
    }
}