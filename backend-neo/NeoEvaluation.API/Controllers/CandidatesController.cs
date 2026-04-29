using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using NeoEvaluation.API.Data;
using NeoEvaluation.API.DTOs;
using NeoEvaluation.API.Models;
using NeoEvaluation.API.Services;
using NeoEvaluation.API.Attributes;

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
        [RequirePermission("view_can")]
        public async Task<IActionResult> GetCampagnes()
        {
            var list = await _context.Campagnes
                .Select(c => new { id = c.Id, titre = c.Nom })
                .ToListAsync();
            return Ok(list);
        }

        // URL: GET http://localhost:5172/api/Candidates
        [HttpGet]
        [RequirePermission("view_can")]
        public async Task<ActionResult> GetCandidates()
        {
            var tenantId = _tenantService.GetTenantId();

            var query = await _context.Candidatures
                .IgnoreQueryFilters()
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

            var distinctList = query.GroupBy(x => x.email)
                                    .Select(g => g.First())
                                    .ToList();

            return Ok(distinctList);
        }

        // NOUVEAU - URL: GET http://localhost:5172/api/Candidates/{id}
        [HttpGet("{id}")]
        [RequirePermission("view_can")]
        public async Task<IActionResult> GetCandidateById(Guid id)
        {
            var tenantId = _tenantService.GetTenantId();

            var candidature = await _context.Candidatures
                .IgnoreQueryFilters()
                .Include(c => c.Candidat)
                .Include(c => c.Campagne)
                .Where(c => c.CandidatId == id && c.Campagne.EntrepriseId == tenantId)
                .FirstOrDefaultAsync();

            if (candidature == null) return NotFound(new { message = "Candidat introuvable" });

            // Projection pour correspondre aux besoins du Frontend
            var result = new
            {
                id = candidature.CandidatId,
                fullName = (candidature.Candidat.Prenom + " " + candidature.Candidat.Nom).Trim(),
                email = candidature.Candidat.Email,
                campaignName = candidature.Campagne.Nom,
                scoreGlobal = 78, // À lier à votre table de résultats plus tard
                iaVerdict = "L'analyse montre un profil technique solide avec une bonne capacité d'adaptation aux environnements agiles.",
                skills = new List<object>
                {
                    new { name = "Technique", val = 85 },
                    new { name = "Soft Skills", val = 70 },
                    new { name = "Logique", val = 90 }
                }
            };

            return Ok(result);
        }

        // URL: POST http://localhost:5172/api/Candidates/bulk-invite
        [HttpPost("bulk-invite")]
        [RequirePermission("inv_can")]
        public async Task<IActionResult> BulkInvite([FromBody] BulkInviteDto dto)
        {
            if (dto.Emails == null || !dto.Emails.Any()) return BadRequest("Emails manquants.");
            var campagne = await _context.Campagnes.IgnoreQueryFilters()
                .FirstOrDefaultAsync(c => c.Id == dto.CampagneId);

            if (campagne == null) return BadRequest("Campagne introuvable.");

            var entId = _tenantService.GetTenantId();

            foreach (var email in dto.Emails)
            {
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
                        EntrepriseId = entId, 
                        Privileges = new List<string> { "AccèsExamen" } 
                    };
                    _context.Utilisateurs.Add(cand);
                }
                else if (cand.EntrepriseId == null)
                {
                    cand.EntrepriseId = entId;
                }

                _context.Candidatures.Add(new Candidature { 
                    Id = Guid.NewGuid(), CandidatId = cand.Id, CampagneId = dto.CampagneId, 
                    PostuleLe = DateTime.UtcNow, Statut = ApplicationStatus.POSTULE 
                });

                string link = "http://localhost:5173/login";

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
                    await _emailService.SendEmailAsync(email, $"Invitation : {campagne.Nom}", $"Vous avez été assigné à une nouvelle évaluation. Lien : {link}");
                } catch { }
            }
            return Ok(new { message = "Invitations envoyées avec succès." });
        }
    }
}