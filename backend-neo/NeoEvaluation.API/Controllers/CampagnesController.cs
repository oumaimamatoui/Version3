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

        public CampagnesController(AppDbContext context, ITenantService tenantService) 
        { 
            _context = context; 
            _tenantService = tenantService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetCampagnes()
        {
            try
            {
                var userRole = _tenantService.GetUserRole();
                var userId = _tenantService.GetUserId();

                // On inclut les questionnaires liés via la table CampagneQuestionnaires
                IQueryable<Campagne> query = _context.Campagnes
                    .Include(c => c.CampagneQuestionnaires)
                        .ThenInclude(cq => cq.Questionnaire)
                    .Include(c => c.Candidatures);

                // --- Logique Candidat ---
                if (userRole == "Candidat" && userId.HasValue)
                {
                    var currentUser = await _context.Utilisateurs.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == userId.Value);
                    if (currentUser == null) return Unauthorized();
                    var email = currentUser.Email.ToLower();

                    var campaignIds = await _context.Candidatures
                        .IgnoreQueryFilters()
                        .Where(c => c.Candidat != null && c.Candidat.Email.ToLower() == email)
                        .Select(c => c.CampagneId)
                        .ToListAsync();

                    query = query.IgnoreQueryFilters().Where(c => campaignIds.Contains(c.Id));
                }

                // --- Projection pour le Frontend ---
                var list = await query
                    .Select(c => new {
                        c.Id,
                        c.Nom,
                        c.Description,
                        // Convertir l'Enum en int pour que le CSS (status-1, etc.) fonctionne
                        Statut = (int)c.Statut, 
                        c.DateDebut,
                        c.DateFin,
                        c.DureeMinutes,
                        c.CreeLe,
                        // ✅ FIX "Non défini" : On aplatit le QuestionnaireId pour le JS
                        // On prend le premier questionnaire lié comme référence principale
                        QuestionnaireId = c.CampagneQuestionnaires
                                            .Select(cq => cq.QuestionnaireId)
                                            .FirstOrDefault(),
                        // On renvoie aussi le nombre actuel de candidats inscrits
                        NbCandidats = c.Candidatures.Count
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

        [HttpPost]
        public async Task<ActionResult<Campagne>> PostCampagne([FromBody] CampagneCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var entId = _tenantService.GetTenantId();
            if (entId == null) return Unauthorized("Entreprise non identifiée.");

            using var trans = await _context.Database.BeginTransactionAsync();
            try {
                var nouvelle = new Campagne {
                    Id = Guid.NewGuid(),
                    Nom = dto.Nom, 
                    Description = dto.Description,
                    // Conversion du int vers l'Enum StatutCampagne
                    Statut = (StatutCampagne)dto.Statut, 
                    EntrepriseId = entId.Value,
                    DateDebut = dto.DateDebut.ToUniversalTime(), 
                    DateFin = dto.DateFin.ToUniversalTime(),
                    DureeMinutes = dto.DureeMinutes,
                    ModeNotation = dto.ModeNotation ?? "STRICT",
                    CreeLe = DateTime.UtcNow
                };
                
                _context.Campagnes.Add(nouvelle);

                // ✅ Liaison avec le questionnaire (Table M2M)
                if (dto.QuestionnaireId.HasValue && dto.QuestionnaireId != Guid.Empty)
                {
                    _context.CampagneQuestionnaires.Add(new CampagneQuestionnaire {
                        CampagneId = nouvelle.Id,
                        QuestionnaireId = dto.QuestionnaireId.Value
                    });
                }

                // ✅ Ajout des candidatures
                if (dto.SelectedCandidatesIds != null) {
                    foreach (var candId in dto.SelectedCandidatesIds) {
                        _context.Candidatures.Add(new Candidature {
                            Id = Guid.NewGuid(), 
                            CampagneId = nouvelle.Id, 
                            CandidatId = candId
                        });
                    }
                }

                await _context.SaveChangesAsync();
                await trans.CommitAsync();
                
                return Ok(nouvelle);
            } catch (Exception ex) {
                await trans.RollbackAsync();
                return StatusCode(500, ex.Message);
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

            // Nettoyage des tables liées avant suppression de la campagne
            _context.CampagneQuestionnaires.RemoveRange(c.CampagneQuestionnaires);
            _context.Candidatures.RemoveRange(c.Candidatures);
            _context.Campagnes.Remove(c);

            await _context.SaveChangesAsync();
            return Ok(new { message = "Campagne supprimée." });
        }
    }
}