using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NeoEvaluation.API.Data;
using NeoEvaluation.API.DTOs;
using NeoEvaluation.API.Models;
using NeoEvaluation.API.Services;

namespace NeoEvaluation.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Route de base : api/Candidates
    public class CandidatesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;

        public CandidatesController(AppDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // URL: GET http://localhost:5172/api/Candidates/campagnes
        [HttpGet("campagnes")]
        public async Task<IActionResult> GetCampagnes()
        {
            var list = await _context.Campagnes
                .Select(c => new { id = c.Id, titre = c.Nom })
                .ToListAsync();
            return Ok(list);
        }

        // URL: GET http://localhost:5172/api/Candidates
        [HttpGet]
        public async Task<ActionResult> GetCandidates()
        {
            var list = await _context.Set<Candidat>()
                .OrderByDescending(u => u.CreeLe)
                .Select(u => new {
                    id = u.Id,
                    name = (u.Prenom + " " + u.Nom).Trim(),
                    email = u.Email,
                    group = _context.Candidatures
                        .Where(cad => cad.CandidatId == u.Id)
                        .OrderByDescending(cad => cad.PostuleLe)
                        .Select(cad => cad.Campagne.Nom)
                        .FirstOrDefault() ?? "N/A",
                    score = 0,
                    status = _context.Candidatures
                        .Where(cad => cad.CandidatId == u.Id)
                        .OrderByDescending(cad => cad.PostuleLe)
                        .Select(cad => cad.Statut.ToString())
                        .FirstOrDefault() ?? "INACTIF"
                }).ToListAsync();
            return Ok(list);
        }

        // URL: POST http://localhost:5172/api/Candidates/bulk-invite
        [HttpPost("bulk-invite")]
        public async Task<IActionResult> BulkInvite([FromBody] BulkInviteDto dto)
        {
            if (dto.Emails == null || !dto.Emails.Any()) return BadRequest("Emails manquants.");
            var campagne = await _context.Campagnes.FindAsync(dto.CampagneId);
            if (campagne == null) return BadRequest("Campagne introuvable.");

            foreach (var email in dto.Emails)
            {
                var cand = await _context.Set<Candidat>().FirstOrDefaultAsync(u => u.Email == email);
                if (cand == null) {
                    cand = new Candidat { 
                        Id = Guid.NewGuid(), Email = email, Nom = "Candidat", 
                        Prenom = email.Split('@')[0], RoleNom = "Candidat", 
                        EstActif = false, CreeLe = DateTime.UtcNow,
                        Privileges = new List<string> { "AccèsExamen" } 
                    };
                    _context.Set<Candidat>().Add(cand);
                }

                _context.Candidatures.Add(new Candidature { 
                    Id = Guid.NewGuid(), CandidatId = cand.Id, CampagneId = dto.CampagneId, 
                    PostuleLe = DateTime.UtcNow, Statut = ApplicationStatus.POSTULE 
                });

                var token = new TokensActivation { 
                    Id = Guid.NewGuid(), Token = Guid.NewGuid(), UtilisateurId = cand.Id, 
                    Email = email, DateCreation = DateTime.UtcNow, 
                    DateExpiration = DateTime.UtcNow.AddDays(7), Utilise = false 
                };
                _context.TokensActivation.Add(token);
                await _context.SaveChangesAsync();

                try {
                    string link = $"http://localhost:5173/activer?token={token.Token}";
                    await _emailService.SendEmailAsync(email, $"Invitation : {campagne.Nom}", $"Lien : {link}");
                } catch { }
            }
            return Ok(new { message = "Invitations envoyées avec succès." });
        }
    }
}