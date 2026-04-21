using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using NeoEvaluation.API.Data;
using NeoEvaluation.API.DTOs;
using NeoEvaluation.API.Models;
using NeoEvaluation.API.Services;

namespace NeoEvaluation.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Route de base : api/Candidates
    [Authorize]
    public class CandidatesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;
        private readonly ITenantService _tenantService;

        public CandidatesController(AppDbContext context, IEmailService emailService, ITenantService tenantService)
        {
            _context = context;
            _emailService = emailService;
            _tenantService = tenantService;
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
            var tenantId = _tenantService.GetTenantId();

            // On récupère les candidats qui ont une candidature dans l'entreprise actuelle
            var list = await _context.Candidatures
                .Include(c => c.Candidat)
                .Include(c => c.Campagne)
                .Where(c => c.Campagne.EntrepriseId == tenantId)
                .OrderByDescending(c => c.PostuleLe)
                .Select(c => new {
                    id = c.CandidatId,
                    name = (c.Candidat.Prenom + " " + c.Candidat.Nom).Trim() == "" ? "Candidat" : (c.Candidat.Prenom + " " + c.Candidat.Nom).Trim(),
                    email = c.Candidat.Email,
                    group = c.Campagne.Nom,
                    score = 0,
                    status = c.Statut.ToString()
                }).ToListAsync();

            return Ok(list);
        }

        // URL: POST http://localhost:5172/api/Candidates/bulk-invite
        [HttpPost("bulk-invite")]
        public async Task<IActionResult> BulkInvite([FromBody] BulkInviteDto dto)
        {
            if (dto.Emails == null || !dto.Emails.Any()) return BadRequest("Emails manquants.");
            var campagne = await _context.Campagnes.IgnoreQueryFilters()
                .FirstOrDefaultAsync(c => c.Id == dto.CampagneId);

            if (campagne == null) return BadRequest("Campagne introuvable.");

            var entId = _tenantService.GetTenantId();

            foreach (var email in dto.Emails)
            {
                // Vérifier si le candidat existe déjà (on ignore les filtres pour éviter les doublons d'email)
                var cand = await _context.Utilisateurs.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Email == email);
                
                if (cand == null) {
                    cand = new Utilisateur { 
                        Id = Guid.NewGuid(), 
                        Email = email, 
                        Nom = "Candidat", 
                        Prenom = email.Split('@')[0], 
                        RoleNom = "Candidat", 
                        EstActif = false, 
                        CreeLe = DateTime.UtcNow,
                        EntrepriseId = entId, // CRUCIAL : Lier à l'entreprise
                        Privileges = new List<string> { "AccèsExamen" } 
                    };
                    _context.Utilisateurs.Add(cand);
                }
                else if (cand.EntrepriseId == null)
                {
                    // Si le candidat existait sans entreprise, on le lie à celle-ci
                    cand.EntrepriseId = entId;
                }

                _context.Candidatures.Add(new Candidature { 
                    Id = Guid.NewGuid(), CandidatId = cand.Id, CampagneId = dto.CampagneId, 
                    PostuleLe = DateTime.UtcNow, Statut = ApplicationStatus.POSTULE 
                });

                string link = "http://localhost:5173/login"; // Par défaut, aller au login

                if (!cand.EstActif)
                {
                    var token = new TokensActivation { 
                        Id = Guid.NewGuid(), Token = Guid.NewGuid(), UtilisateurId = cand.Id, 
                        Email = email, DateCreation = DateTime.UtcNow, 
                        DateExpiration = DateTime.UtcNow.AddDays(7), Utilise = false 
                    };
                    _context.TokensActivation.Add(token);
                    await _context.SaveChangesAsync();
                    link = $"http://localhost:5173/activate-role?token={token.Token}";
                }
                else 
                {
                    await _context.SaveChangesAsync();
                }

                try {
                    await _emailService.SendEmailAsync(email, $"Invitation : {campagne.Nom}", $"Vous avez été assigné à une nouvelle évaluation. Connectez-vous ou activez votre compte via ce lien : {link}");
                } catch { }
            }
            return Ok(new { message = "Invitations envoyées avec succès." });
        }
    }
}