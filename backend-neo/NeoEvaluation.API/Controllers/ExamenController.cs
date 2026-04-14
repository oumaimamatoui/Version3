using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NeoEvaluation.API.Data;
using NeoEvaluation.API.Models;
using NeoEvaluation.API.DTOs;

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

        // --- 1. SETUP : Charger l'examen ---
        [HttpGet("setup/{candidatureId}")]
        public async Task<IActionResult> GetSetup(string candidatureId)
        {
            if (string.IsNullOrEmpty(candidatureId) || candidatureId == "undefined" || !Guid.TryParse(candidatureId, out Guid guidId))
            {
                return BadRequest(new { message = "L'ID de candidature est invalide ou absent." });
            }

            // Récupération via les nouvelles relations M2M
            var cand = await _context.Candidatures
                .Include(c => c.Campagne)
                    .ThenInclude(cp => cp.CampagneQuestionnaires)
                        .ThenInclude(cq => cq.Questionnaire)
                            .ThenInclude(q => q.QuestionnaireQuestions)
                                .ThenInclude(qq => qq.Question)
                .Include(c => c.Evaluation)
                .FirstOrDefaultAsync(c => c.Id == guidId);

            if (cand == null) 
                return NotFound(new { message = "Candidature introuvable." });

            // Récupérer le premier questionnaire de la campagne
            var premierQuestionnaire = cand.Campagne?.CampagneQuestionnaires?.FirstOrDefault()?.Questionnaire;
            if (premierQuestionnaire == null)
                return BadRequest(new { message = "Aucun questionnaire n'est configuré pour cette campagne." });

            // Initialisation de l'évaluation
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

            // Récupérer les questions ordonnées via la table de jointure
            var questions = premierQuestionnaire.QuestionnaireQuestions
                .OrderBy(qq => qq.Ordre)
                .Select(qq => qq.Question)
                .ToList();

            var setup = new ExamSetupDto {
                EvaluationId = cand.Evaluation.Id,
                CampagneNom = cand.Campagne.Nom,
                TempsLimite = premierQuestionnaire.DureeMinutes * 60,
                Questions = questions.Select(q => new QuestionItemDto {
                    Id = q.Id,
                    Enonce = q.Enonce,
                    Options = q.Choix.Any() ? q.Choix : new List<string> { "Choix A", "Choix B" }
                }).ToList()
            };

            return Ok(setup);
        }

        // --- 2. SYNC : Sauvegarde auto ---
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

        // --- 3. TERMINER : Score final ---
        [HttpPost("terminer/{evaluationId}")]
        public async Task<IActionResult> Terminer(Guid evaluationId)
        {
            var eval = await _context.Evaluations.FindAsync(evaluationId);
            if (eval == null) return NotFound();

            eval.Statut = StatutPassage.TERMINE;
            eval.DateFin = DateTime.UtcNow;
            eval.ScoreTotal = new Random().Next(12, 19); // Simulation score
            await _context.SaveChangesAsync();
            return Ok(new { score = eval.ScoreTotal });
        }
    }
}