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

            return Ok(new UserProfileDto {
                Nom = user.Nom,
                Prenom = user.Prenom,
                Email = user.Email,
                PhotoUrl = user.PhotoUrl,
                JoinDate = user.CreeLe.ToString("MMMM yyyy")
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

            await _context.SaveChangesAsync();
            return Ok(new { message = "Profil mis à jour" });
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
            }

            user.MotDePasseHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Succès" });
        }

        // FIX ERREUR 500 : Approche manuelle pour récupérer l'entreprise
        [HttpGet("branding")]
        public async Task<IActionResult> GetBranding()
        {
            try {
                var userId = GetCurrentUserId();
                var user = await _context.Utilisateurs.FindAsync(userId);
                
                if (user == null || user.EntrepriseId == null)
                    return Ok(new BrandingDto { CompanyName = "EvaluaTech", Color = "#eab308" });

                var entreprise = await _context.Entreprises.FindAsync(user.EntrepriseId);
                return Ok(new BrandingDto {
                    CompanyName = entreprise?.Nom ?? "EvaluaTech",
                    Color = "#6366f1"
                });
            } catch {
                return Ok(new BrandingDto { CompanyName = "EvaluaTech", Color = "#eab308" });
            }
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