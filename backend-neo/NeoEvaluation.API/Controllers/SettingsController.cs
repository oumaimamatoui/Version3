using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NeoEvaluation.API.Data;
using NeoEvaluation.API.Dtos;
using NeoEvaluation.API.Models;
using System.Security.Claims;
using BCrypt.Net;

namespace NeoEvaluation.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettingsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SettingsController(AppDbContext context)
        {
            _context = context;
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
            var user = await _context.Utilisateurs.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return NotFound();

            var entrepriseNom = user.EntrepriseId.HasValue 
                ? await _context.Entreprises.Where(e => e.Id == user.EntrepriseId).Select(e => e.Nom).FirstOrDefaultAsync() 
                : null;

            return Ok(new UserProfileDto {
                Nom = user.Nom,
                Prenom = user.Prenom,
                Email = user.Email,
                PhotoUrl = user.PhotoUrl,
                Bio = user.Bio,
                JoinDate = user.CreeLe.ToString("MMMM yyyy"),
                ThemePreference = user.ThemePreference,
                EntrepriseNom = entrepriseNom
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
                var user = await _context.Utilisateurs.FindAsync(userId);
                
                if (user == null || user.EntrepriseId == null)
                    return Ok(new BrandingDto { CompanyName = "NeoEvaluation", Color = "#6366f1" });

                var entreprise = await _context.Entreprises.FindAsync(user.EntrepriseId);
                return Ok(new BrandingDto {
                    CompanyName = entreprise?.Nom ?? "NeoEvaluation",
                    Color = entreprise?.CouleurSignature ?? "#6366f1",
                    LogoUrl = entreprise?.LogoUrl
                });
            } catch {
                return Ok(new BrandingDto { CompanyName = "NeoEvaluation", Color = "#6366f1" });
            }
        }

        [HttpPost("update-branding")]
        public async Task<IActionResult> UpdateBranding([FromBody] BrandingUpdateDto dto)
        {
            var userId = GetCurrentUserId();
            var user = await _context.Utilisateurs.FindAsync(userId);
            
            if (user == null || user.EntrepriseId == null)
                return BadRequest("Action réservée aux entreprises.");

            var entreprise = await _context.Entreprises.FindAsync(user.EntrepriseId);
            if (entreprise == null) return NotFound();

            entreprise.Nom = dto.CompanyName;
            entreprise.CouleurSignature = dto.Color;

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
    }
}