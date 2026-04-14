using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NeoEvaluation.API.Data;
using NeoEvaluation.API.Models;

namespace NeoEvaluation.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CampagnesController : ControllerBase
    {
        private readonly AppDbContext _context;
        public CampagnesController(AppDbContext context) { _context = context; }

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

            var entId = await _context.Entreprises.Select(e => e.Id).FirstOrDefaultAsync();
            Guid newCampagneId = Guid.NewGuid();

            using var trans = await _context.Database.BeginTransactionAsync();
            try {
                var nouvelle = new Campagne {
                    Id = newCampagneId,
                    Nom = dto.Nom, 
                    Description = dto.Description,
                    Statut = dto.Statut,
                    EntrepriseId = entId,
                    DateDebut = dto.DateDebut.ToUniversalTime(), 
                    DateFin = dto.DateFin.ToUniversalTime()
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
    }
}