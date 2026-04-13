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
            return await _context.Campagnes
                .Include(c => c.Questionnaire)
                .OrderByDescending(c => c.DateDebut)
                .Select(c => new {
                    c.Id, c.Nom, c.Description, c.Categorie, c.Statut,
                    c.DateDebut, c.DateFin, c.MaxCandidats, c.ScorePassage,
                    c.DureeMinutes, c.QuestionnaireId,
                    QuestionnaireTitre = c.Questionnaire != null ? c.Questionnaire.Titre : "Non lié"
                }).ToListAsync();
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
                    Nom = dto.Nom, Description = dto.Description, Categorie = dto.Categorie,
                    Statut = dto.Statut, MaxCandidats = dto.MaxCandidats,
                    DureeMinutes = dto.DureeMinutes, ScorePassage = dto.ScorePassage,
                    QuestionnaireId = dto.QuestionnaireId, EntrepriseId = entId,
                    DateDebut = dto.DateDebut.ToUniversalTime(), DateFin = dto.DateFin.ToUniversalTime()
                };
                _context.Campagnes.Add(nouvelle);

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