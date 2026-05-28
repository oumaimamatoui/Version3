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
        private readonly INotificationService _notificationService;

        public CandidatesController(AppDbContext context, IEmailService emailService, ITenantService tenantService, INotificationService notificationService)
        {
            _context = context;
            _emailService = emailService;
            _tenantService = tenantService;
            _notificationService = notificationService;
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
                .Include(c => c.Evaluation)
                .Where(c => c.Campagne.EntrepriseId == tenantId)
                .OrderByDescending(c => c.PostuleLe)
                .Select(c => new {
                    id = c.CandidatId,
                    name = (c.Candidat.Prenom + " " + c.Candidat.Nom).Trim() == "" ? "Candidat" : (c.Candidat.Prenom + " " + c.Candidat.Nom).Trim(),
                    email = c.Candidat.Email,
                    group = c.Campagne.Nom,
                    score = c.Evaluation != null ? (int)Math.Round(c.Evaluation.ScorePourcentage) : 0,
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

                try {
                    await _notificationService.NotifyUserAsync(cand.Id, new NotificationPayload
                    {
                        Type = "info",
                        Title = "Nouveau test",
                        Message = $"Vous avez un nouveau test : \"{campagne.Nom}\"",
                        Link = "/login"
                    });
                } catch { }
            }

            // Notification en temps réel pour l'entreprise
            await _notificationService.NotifyTenantAsync(entId.Value, new NotificationPayload
            {
                Type = "info",
                Title = "Invitations envoyées",
                Message = $"{dto.Emails.Count} candidat(s) invité(s) à la campagne.",
                Link = "/candidates-list"
            });

            return Ok(new { message = "Invitations envoyées avec succès." });
        }

        // URL: DELETE http://localhost:5172/api/Candidates/{id}
        [HttpDelete("{id}")]
        [RequirePermission("inv_can")]
        public async Task<IActionResult> DeleteCandidate(Guid id)
        {
            var tenantId = _tenantService.GetTenantId();
            if (tenantId == null) return Forbid();

            // Trouve l'utilisateur qui a le rôle de candidat et appartient au même tenant
            var cand = await _context.Utilisateurs
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(u => u.Id == id && u.EntrepriseId == tenantId && u.RoleNom == "Candidat");

            if (cand == null) return NotFound(new { message = "Candidat introuvable." });

            // Trouver toutes les candidatures pour ce candidat appartenant à l'entreprise
            var candidatures = await _context.Candidatures
                .IgnoreQueryFilters()
                .Include(c => c.Evaluation)
                .Where(c => c.CandidatId == id && c.Campagne.EntrepriseId == tenantId)
                .ToListAsync();

            foreach (var c in candidatures)
            {
                if (c.Evaluation != null)
                {
                    // Supprimer le rapport s'il existe
                    var rapport = await _context.Rapports
                        .IgnoreQueryFilters()
                        .FirstOrDefaultAsync(r => r.EvaluationId == c.Evaluation.Id);
                    if (rapport != null) _context.Rapports.Remove(rapport);

                    // Supprimer les réponses s'il y en a
                    var reponses = await _context.Reponses
                        .IgnoreQueryFilters()
                        .Where(rp => rp.EvaluationId == c.Evaluation.Id)
                        .ToListAsync();
                    _context.Reponses.RemoveRange(reponses);

                    _context.Evaluations.Remove(c.Evaluation);
                }
                _context.Candidatures.Remove(c);
            }

            // Supprimer les jetons d'activation s'il y en a
            var tokens = await _context.TokensActivation
                .IgnoreQueryFilters()
                .Where(t => t.UtilisateurId == id)
                .ToListAsync();
            _context.TokensActivation.RemoveRange(tokens);

            // Supprimer l'utilisateur lui-même
            _context.Utilisateurs.Remove(cand);

            await _context.SaveChangesAsync();

            return Ok(new { message = "Candidat supprimé avec succès." });
        }

        // URL: POST http://localhost:5172/api/Candidates/{id}/resend
        [HttpPost("{id}/resend")]
        [RequirePermission("inv_can")]
        public async Task<IActionResult> ResendInvitation(Guid id)
        {
            var tenantId = _tenantService.GetTenantId();
            if (tenantId == null) return Forbid();

            var cand = await _context.Utilisateurs
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(u => u.Id == id && u.EntrepriseId == tenantId);

            if (cand == null) return NotFound(new { message = "Candidat introuvable." });

            // Trouver la dernière candidature et sa campagne pour ce candidat
            var candidature = await _context.Candidatures
                .IgnoreQueryFilters()
                .Include(c => c.Campagne)
                .Where(c => c.CandidatId == id && c.Campagne.EntrepriseId == tenantId)
                .OrderByDescending(c => c.PostuleLe)
                .FirstOrDefaultAsync();

            if (candidature == null) return BadRequest("Aucune candidature trouvée pour ce candidat.");

            string link = "http://localhost:5173/login";

            if (!cand.EstActif)
            {
                // Trouver ou recréer le token d'activation
                var token = await _context.TokensActivation
                    .IgnoreQueryFilters()
                    .Where(t => t.UtilisateurId == id && !t.Utilise && t.DateExpiration > DateTime.UtcNow)
                    .FirstOrDefaultAsync();

                if (token == null)
                {
                    token = new TokensActivation { 
                        Id = Guid.NewGuid(), Token = Guid.NewGuid(), UtilisateurId = cand.Id, 
                        Email = cand.Email, DateCreation = DateTime.UtcNow, 
                        DateExpiration = DateTime.UtcNow.AddDays(7), Utilise = false 
                    };
                    _context.TokensActivation.Add(token);
                    await _context.SaveChangesAsync();
                }

                link = $"http://localhost:5173/activate-role?token={token.Token}";
            }

            try {
                await _emailService.SendEmailAsync(cand.Email, $"Relance : {candidature.Campagne.Nom}", $"Vous avez une évaluation en attente. Lien : {link}");
                return Ok(new { message = "Invitation renvoyée avec succès." });
            } catch (Exception ex) {
                return StatusCode(500, new { message = $"Erreur lors de l'envoi de l'email: {ex.Message}" });
            }
        }
    }
}