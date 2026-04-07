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
            // Validation de l'ID pour éviter l'erreur 400 automatique
            if (string.IsNullOrEmpty(candidatureId) || candidatureId == "undefined" || !Guid.TryParse(candidatureId, out Guid guidId))
            {
                return BadRequest(new { message = "L'ID de candidature est invalide ou absent." });
            }

            // Récupération de la candidature et de toute la chaîne de relations
            var cand = await _context.Candidatures
                .Include(c => c.Campagne)
                    .ThenInclude(cp => cp.Questionnaire)
                        .ThenInclude(q => q.Questions)
                .Include(c => c.Evaluation)
                .FirstOrDefaultAsync(c => c.Id == guidId);

            if (cand == null) 
                return NotFound(new { message = "Candidature introuvable." });

            if (cand.Campagne?.Questionnaire == null)
                return BadRequest(new { message = "Aucun questionnaire n'est configuré pour cette campagne." });

            // Initialisation de l'évaluation
            if (cand.Evaluation == null)
            {
                cand.Evaluation = new Evaluation {
                    Id = Guid.NewGuid(),
                    CandidatureId = cand.Id,
                    Statut = EvaluationStatus.NON_COMMENCE,
                    LimiteTemps = 30 // 30 minutes
                };
                _context.Evaluations.Add(cand.Evaluation);
                await _context.SaveChangesAsync();
            }

            // Mapping vers le DTO (Sécurité : on ne renvoie pas les bonnes réponses)
            var setup = new ExamSetupDto {
                EvaluationId = cand.Evaluation.Id,
                CampagneNom = cand.Campagne.Nom,
                TempsLimite = cand.Evaluation.LimiteTemps * 60, // En secondes pour le frontend
                Questions = cand.Campagne.Questionnaire.Questions.Select(q => new QuestionItemDto {
                    Id = q.Id,
                    Texte = q.Texte.Split('|')[0], // On prend la question
                    Options = q.Texte.Contains("|") ? q.Texte.Split('|').Skip(1).ToList() : new List<string> { "Choix A", "Choix B" }
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
            eval.Statut = EvaluationStatus.EN_COURS;
            await _context.SaveChangesAsync();
            return Ok(new { status = "SYNCED" });
        }

        // --- 3. TERMINER : Score final ---
        [HttpPost("terminer/{evaluationId}")]
        public async Task<IActionResult> Terminer(Guid evaluationId)
        {
            var eval = await _context.Evaluations.FindAsync(evaluationId);
            if (eval == null) return NotFound();

            eval.Statut = EvaluationStatus.TERMINE;
            eval.Score = new Random().Next(12, 19); // Simulation score
            await _context.SaveChangesAsync();
            return Ok(new { score = eval.Score });
        }
    }
}