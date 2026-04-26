using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NeoEvaluation.API.Data;
using NeoEvaluation.API.Models;
using NeoEvaluation.API.DTOs; // Utilise le namespace des DTOs

namespace NeoEvaluation.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExamenController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ExamenController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("setup/{candidatureId}")]
        public async Task<IActionResult> GetSetup(string candidatureId)
        {
            if (string.IsNullOrEmpty(candidatureId) || candidatureId == "undefined" || !Guid.TryParse(candidatureId, out Guid guidId))
            {
                return BadRequest(new { message = "L'ID de candidature est invalide." });
            }

            var cand = await _context.Candidatures
                .Include(c => c.Campagne)
                    .ThenInclude(cp => cp.CampagneQuestionnaires)
                        .ThenInclude(cq => cq.Questionnaire)
                            .ThenInclude(q => q.QuestionnaireQuestions)
                                .ThenInclude(qq => qq.Question)
                .Include(c => c.Evaluation)
                .AsSplitQuery() 
                .FirstOrDefaultAsync(c => c.Id == guidId);

            if (cand == null) 
                return NotFound(new { message = "Candidature introuvable." });

            var questionnaire = cand.Campagne?.CampagneQuestionnaires?.FirstOrDefault()?.Questionnaire;
            
            if (questionnaire == null)
                return BadRequest(new { message = "Aucun questionnaire configuré." });

            if (cand.Evaluation == null)
            {
                cand.Evaluation = new Evaluation {
                    Id = Guid.NewGuid(),
                    CandidatureId = cand.Id,
                    Statut = StatutPassage.NON_COMMENCE,
                };
                _context.Evaluations.Add(cand.Evaluation);
                await _context.SaveChangesAsync();
            }

            var questionsOrdonnees = questionnaire.QuestionnaireQuestions
                .OrderBy(qq => qq.Ordre)
                .Select(qq => qq.Question)
                .ToList();

            var setup = new ExamSetupDto {
                EvaluationId = cand.Evaluation.Id,
                CampagneNom = cand.Campagne?.Nom ?? "Évaluation",
                TempsLimite = (questionnaire.DureeMinutes > 0 ? questionnaire.DureeMinutes : 30) * 60,
                Questions = questionsOrdonnees.Select(q => new QuestionItemDto {
                    Id = q.Id,
                    Enonce = q.Enonce,
                    Type = (int)q.Type, 
                    Points = q.Points,
                    Options = q.Choix ?? new List<string> { "Option A", "Option B" }
                }).ToList()
            };

            return Ok(setup);
        }

        [HttpPost("sync")]
        public async Task<IActionResult> Sync([FromBody] SyncDto dto)
        {
            var eval = await _context.Evaluations.FindAsync(dto.EvaluationId);
            if (eval == null) return NotFound();
            eval.Statut = StatutPassage.EN_COURS;
            if (eval.DateDebut == null) eval.DateDebut = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return Ok(new { status = "SYNCED" });
        }

        [HttpPost("terminer/{evaluationId}")]
        public async Task<IActionResult> Terminer(Guid evaluationId)
        {
            var eval = await _context.Evaluations.FindAsync(evaluationId);
            if (eval == null) return NotFound();
            eval.Statut = StatutPassage.TERMINE;
            eval.DateFin = DateTime.UtcNow;
            eval.ScoreTotal = new Random().Next(10, 20); 
            await _context.SaveChangesAsync();
            return Ok(new { score = eval.ScoreTotal });
        }
    }
}