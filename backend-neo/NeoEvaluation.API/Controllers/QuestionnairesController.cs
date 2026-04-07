using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NeoEvaluation.API.Data;
using NeoEvaluation.API.Models;

namespace NeoEvaluation.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionnairesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public QuestionnairesController(AppDbContext context)
        {
            _context = context;
        }

        // READ: Liste tous les questionnaires avec leurs questions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Questionnaire>>> GetQuestionnaires()
        {
            return await _context.Questionnaires
                .Include(q => q.Questions) 
                .AsNoTracking() // Optimisation pour la lecture seule
                .ToListAsync();
        }

        // READ: Un questionnaire spécifique
        [HttpGet("{id}")]
        public async Task<ActionResult<Questionnaire>> GetQuestionnaire(Guid id)
        {
            var questionnaire = await _context.Questionnaires
                .Include(q => q.Questions)
                .AsNoTracking()
                .FirstOrDefaultAsync(q => q.Id == id);

            if (questionnaire == null) 
                return NotFound(new { message = "Architecture introuvable dans la base SQL." });

            return questionnaire;
        }

        // CREATE: Déploiement complet (Questionnaire + Questions imbriquées)
        [HttpPost]
        public async Task<ActionResult<Questionnaire>> CreateQuestionnaire([FromBody] Questionnaire questionnaire)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                // Initialisation du Questionnaire
                questionnaire.Id = Guid.NewGuid();
                questionnaire.CreeLe = DateTime.UtcNow;

                // On s'assure que chaque question est liée au questionnaire
                if (questionnaire.Questions != null && questionnaire.Questions.Any())
                {
                    foreach (var q in questionnaire.Questions)
                    {
                        q.Id = Guid.NewGuid();
                        q.QuestionnaireId = questionnaire.Id; // Liaison obligatoire
                    }
                }

                _context.Questionnaires.Add(questionnaire);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetQuestionnaire), new { id = questionnaire.Id }, questionnaire);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors de la transaction SQL", detail = ex.Message });
            }
        }

        // UPDATE: Mise à jour du système
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateQuestionnaire(Guid id, [FromBody] Questionnaire questionnaire)
        {
            if (id != questionnaire.Id) 
                return BadRequest(new { message = "ID mismatch" });

            _context.Entry(questionnaire).State = EntityState.Modified;

            // Si vous envoyez aussi les questions lors de l'Update, 
            // la logique de synchronisation EF Core plus complexe est requise ici.

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Questionnaires.AnyAsync(e => e.Id == id))
                    return NotFound();
                else throw;
            }

            return Ok(new { message = "Terminal mis à jour avec succès", data = questionnaire });
        }

        // DELETE: Suppression définitive
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuestionnaire(Guid id)
        {
            var questionnaire = await _context.Questionnaires
                .Include(q => q.Questions) // Charge les questions pour les supprimer proprement
                .FirstOrDefaultAsync(x => x.Id == id);

            if (questionnaire == null) return NotFound();

            _context.Questionnaires.Remove(questionnaire);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Banque d'examen et ses actifs supprimés définitivement." });
        }
    }
}