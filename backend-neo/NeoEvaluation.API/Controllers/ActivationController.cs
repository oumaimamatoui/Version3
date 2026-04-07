using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NeoEvaluation.API.Data;
using NeoEvaluation.API.Models;

namespace NeoEvaluation.API.Controllers
{
    public class CompleteActivationDto {
        public Guid Token { get; set; }
        public string Password { get; set; } = string.Empty;
    }

    [Route("api/[controller]")]
    [ApiController]
    public class ActivationController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ActivationController(AppDbContext context) { _context = context; }

        [HttpGet("check/{token}")]
        public async Task<IActionResult> CheckToken(Guid token)
        {
            var t = await _context.TokensActivation.FirstOrDefaultAsync(x => x.Token == token && !x.Utilise && x.DateExpiration > DateTime.UtcNow);
            return Ok(new { valide = (t != null) });
        }

        [HttpPost("complete")]
        public async Task<IActionResult> Complete([FromBody] CompleteActivationDto dto)
        {
            var token = await _context.TokensActivation.FirstOrDefaultAsync(t => t.Token == dto.Token && !t.Utilise);
            if (token == null || token.DateExpiration < DateTime.UtcNow) 
                return BadRequest(new { message = "Lien invalide ou expiré." });

            // CAS CANDIDAT
            if (token.UtilisateurId != null && token.UtilisateurId != Guid.Empty)
            {
                var user = await _context.Utilisateurs.FindAsync(token.UtilisateurId);
                if (user == null) return NotFound(new { message = "Utilisateur non trouvé." });

                user.MotDePasseHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
                user.EstActif = true;
                token.Utilise = true;
                await _context.SaveChangesAsync();
                return Ok(new { message = "Candidat activé." });
            }

            // CAS ENTREPRISE
            var reg = await _context.InscriptionsEntreprises.FindAsync(token.IdInscription);
            if (reg == null) return BadRequest(new { message = "Inscription introuvable." });

            // ... (Reste de votre logique InscriptionsEntreprises) ...
            token.Utilise = true;
            reg.Statut = 3;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Entreprise activée." });
        }
    }
}