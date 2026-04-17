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
                var list = await _context.Campagnes
                    .Include(c => c.CampagneQuestionnaires)
                        .ThenInclude(cq => cq.Questionnaire)
                    .Select(c => new {
                        c.Id,
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
    }
}