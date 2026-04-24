using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using NeoEvaluation.API.Data;
using NeoEvaluation.API.Models;
using NeoEvaluation.API.Services;

namespace NeoEvaluation.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CampagnesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ITenantService _tenantService;
        private readonly IEmailService _emailService;

        public CampagnesController(AppDbContext context, ITenantService tenantService, IEmailService emailService) 
        { 
            _context = context; 
            _tenantService = tenantService;
            _emailService = emailService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetCampagnes()
        {
            try
            {
                var userRole = _tenantService.GetUserRole();
                var userId = _tenantService.GetUserId();

                IQueryable<Campagne> query = _context.Campagnes
                    .Include(c => c.CampagneQuestionnaires)
                        .ThenInclude(cq => cq.Questionnaire);

                // Si c'est un candidat, on cherche par son EMAIL (en minuscules)
                if (userRole == "Candidat" && userId.HasValue)
                {
                    var currentUser = await _context.Utilisateurs.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == userId.Value);
                    if (currentUser == null) return Unauthorized();

                    var email = currentUser.Email.ToLower();

                    // 1. Récupérer tous les IDs de campagnes où cet email est inscrit
                    var campaignIds = await _context.Candidatures
                        .IgnoreQueryFilters()
                        .Include(c => c.Candidat)
                        .Where(c => c.Candidat != null && c.Candidat.Email.ToLower() == email)
                        .Select(c => c.CampagneId)
                        .ToListAsync();

                    // 2. Récupérer les détails de ces campagnes
                    var campaignsForUser = await _context.Campagnes
                        .IgnoreQueryFilters()
                        .Include(c => c.CampagneQuestionnaires)
                            .ThenInclude(cq => cq.Questionnaire)
                        .Where(c => campaignIds.Contains(c.Id))
                        .ToListAsync();

                    var result = campaignsForUser.Select(c => new {
                        c.Id,
                        CandidatureId = _context.Candidatures
                            .IgnoreQueryFilters()
                            .Where(cand => cand.CampagneId == c.Id && cand.Candidat != null && cand.Candidat.Email.ToLower() == email)
                            .Select(cand => cand.Id)
                            .FirstOrDefault(),
                        c.Nom,
                        c.Description,
                        c.Statut,
                        c.DateDebut,
                        c.DateFin,
                        c.CreeLe,
                        c.DureeMinutes,
                        c.ModeNotation,
                        Questionnaires = c.CampagneQuestionnaires.Select(cq => new {
                            Id = cq.Questionnaire?.Id ?? Guid.Empty,
                            Titre = cq.Questionnaire?.Titre ?? "Sans Titre",
                            DureeMinutes = cq.Questionnaire?.DureeMinutes ?? 60
                        })
                    }).OrderByDescending(c => c.DateDebut).ToList();

                    return Ok(result);
                }

                var list = await query
                    .Select(c => new {
                        c.Id,
                        // On cherche le CandidatureId pour ce candidat précis
                        CandidatureId = userRole == "Candidat" ? _context.Candidatures
                            .Where(cand => cand.CampagneId == c.Id && cand.CandidatId == userId.Value)
                            .Select(cand => cand.Id)
                            .FirstOrDefault() : Guid.Empty,
                        c.Nom,
                        c.Description,
                        c.Statut,
                        c.DateDebut,
                        c.DateFin,
                        c.CreeLe,
                        c.DureeMinutes,
                        c.ModeNotation,
                        Questionnaires = c.CampagneQuestionnaires.Select(cq => new {
                            Id = cq.Questionnaire != null ? cq.Questionnaire.Id : Guid.Empty,
                            Titre = cq.Questionnaire != null ? cq.Questionnaire.Titre : "Sans Titre",
                            DureeMinutes = cq.Questionnaire != null ? cq.Questionnaire.DureeMinutes : 60,
                            AntitricheActif = cq.Questionnaire != null ? cq.Questionnaire.AntitricheActif : false,
                            RandomiserQuestions = cq.Questionnaire != null ? cq.Questionnaire.RandomiserQuestions : false,
                            Description = cq.Questionnaire != null ? cq.Questionnaire.Description : ""
                        })
                    })
                    .OrderByDescending(c => c.DateDebut)
                    .ToListAsync();

                return Ok(list);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur interne", details = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Campagne>> GetCampagne(Guid id)
        {
            var c = await _context.Campagnes.FindAsync(id);
            return c == null ? NotFound() : c;
        }

        [HttpPost]
        public async Task<ActionResult<Campagne>> PostCampagne([FromBody] CampagneCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var entId = _tenantService.GetTenantId();
            if (entId == null || entId == Guid.Empty)
                return Unauthorized("Impossible d'identifier votre entreprise.");

            Guid newCampagneId = Guid.NewGuid();

            using var trans = await _context.Database.BeginTransactionAsync();
            try {
                var nouvelle = new Campagne {
                    Id = newCampagneId,
                    Nom = dto.Nom, 
                    Description = dto.Description,
                    Statut = dto.Statut,
                    EntrepriseId = entId.Value,
                    DateDebut = dto.DateDebut.ToUniversalTime(), 
                    DateFin = dto.DateFin.ToUniversalTime(),
                    DureeMinutes = dto.DureeMinutes,
                    ModeNotation = dto.ModeNotation
                };
                _context.Campagnes.Add(nouvelle);

                // Lier le questionnaire via la table M2M
                if (dto.QuestionnaireId.HasValue)
                {
                    _context.CampagneQuestionnaires.Add(new CampagneQuestionnaire {
                        CampagneId = newCampagneId,
                        QuestionnaireId = dto.QuestionnaireId.Value
                    });
                }

                if (dto.SelectedCandidatesIds != null) {
                    foreach (var candId in dto.SelectedCandidatesIds) {
                        _context.Candidatures.Add(new Candidature {
                            Id = Guid.NewGuid(), CampagneId = newCampagneId, CandidatId = candId
                        });
                    }
                }
                await _context.SaveChangesAsync();
                await trans.CommitAsync();
                return CreatedAtAction(nameof(GetCampagne), new { id = nouvelle.Id }, nouvelle);
            } catch (Exception ex) {
                await trans.RollbackAsync();
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{id}/planning")]
        public async Task<IActionResult> PutPlanning(Guid id, [FromBody] dynamic dto)
        {
            var campagne = await _context.Campagnes.FindAsync(id);
            if (campagne == null) return NotFound();

            try {
                // On extrait les valeurs dynamiquement (plus simple pour le DTO partiel)
                campagne.DureeMinutes = (int)dto.dureeMinutes;
                campagne.DateDebut = ((DateTime)dto.dateOuverture).ToUniversalTime();
                campagne.ModeNotation = (string)dto.modeNotation;

                await _context.SaveChangesAsync();
                return Ok(new { message = "Planning mis à jour." });
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCampagne(Guid id)
        {
            var c = await _context.Campagnes
                .Include(x => x.CampagneQuestionnaires)
                .Include(x => x.Candidatures)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (c == null) return NotFound();

            // 1. Supprimer les liaisons questionnaires
            if (c.CampagneQuestionnaires.Any())
            {
                _context.CampagneQuestionnaires.RemoveRange(c.CampagneQuestionnaires);
            }

            // 2. Supprimer les candidatures liées
            if (c.Candidatures.Any())
            {
                _context.Candidatures.RemoveRange(c.Candidatures);
            }

            // 3. Supprimer la campagne
            _context.Campagnes.Remove(c);

            await _context.SaveChangesAsync();
            return Ok(new { message = "Campagne supprimée avec succès." });
        }
    }
}