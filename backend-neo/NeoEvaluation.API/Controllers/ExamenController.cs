using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NeoEvaluation.API.Data;
using NeoEvaluation.API.Models;
using NeoEvaluation.API.DTOs;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace NeoEvaluation.API.Controllers
{
    [Authorize] // Sécurise tout le contrôleur (nécessite un Token JWT)
    [ApiController]
    [Route("api/[controller]")]
    public class ExamenController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ExamenController(AppDbContext context)
        {
            _context = context;
        }

        // ============================================================
        // 1. SETUP EXAMEN (LOBBY) - Récupère infos et questions
        // ============================================================
        [HttpGet("setup/{candidatureId}")]
        public async Task<IActionResult> GetSetup(string candidatureId)
        {
            if (!Guid.TryParse(candidatureId, out Guid guidId))
                return BadRequest(new { message = "ID Candidature invalide" });

            var cand = await _context.Candidatures
                .IgnoreQueryFilters()
                .Include(c => c.Evaluation)
                .Include(c => c.Campagne)
                    .ThenInclude(cp => cp!.CampagneQuestionnaires)
                    .ThenInclude(cq => cq.Questionnaire)
                .FirstOrDefaultAsync(c => c.Id == guidId);

            if (cand == null) return NotFound(new { message = "Candidature introuvable" });

            // Si l'évaluation n'existe pas encore, on la crée (Start)
            if (cand.Evaluation == null)
            {
                cand.Evaluation = new Evaluation
                {
                    Id = Guid.NewGuid(),
                    CandidatureId = cand.Id,
                    Statut = StatutPassage.EN_COURS,
                    DateDebut = DateTime.UtcNow
                };
                _context.Evaluations.Add(cand.Evaluation);
                await _context.SaveChangesAsync();
            }

            // Récupération de toutes les questions liées à la campagne
            var questionnaireIds = cand.Campagne?.CampagneQuestionnaires
                                    .Select(cq => cq.QuestionnaireId).ToList() ?? new List<Guid>();

            var questions = await _context.QuestionnaireQuestions
                .IgnoreQueryFilters()
                .Where(qq => questionnaireIds.Contains(qq.QuestionnaireId))
                .OrderBy(qq => qq.Ordre)
                .Select(qq => qq.Question)
                .ToListAsync();

            return Ok(new
            {
                EvaluationId = cand.Evaluation.Id,
                CampagneNom = cand.Campagne?.Nom ?? "Évaluation Technique",
                Statut = cand.Evaluation.Statut.ToString(),
                TempsLimite = (cand.Campagne?.DureeMinutes ?? 45) * 60,
                Questions = questions.Select(q => new
                {
                    q!.Id,
                    q.Enonce,
                    Type = q.Type.ToString(),
                    Options = q.Choix ?? new List<string>(),
                    q.Points
                })
            });
        }

        // ============================================================
        // 2. AUTO-SAVE (Sauvegarde chaque réponse en temps réel)
        // ============================================================
        [HttpPost("save-response")]
        public async Task<IActionResult> SaveResponse([FromBody] ReponseDto dto)
        {
            var rep = await _context.Reponses
                .FirstOrDefaultAsync(r => r.EvaluationId == dto.EvaluationId && r.QuestionId == dto.QuestionId);

            if (rep == null)
            {
                _context.Reponses.Add(new Reponse
                {
                    Id = Guid.NewGuid(),
                    EvaluationId = dto.EvaluationId,
                    QuestionId = dto.QuestionId,
                    Valeur = dto.Valeur,
                    TempsSecondes = dto.TempsSecondes,
                    SoumisLe = DateTime.UtcNow
                });
            }
            else
            {
                rep.Valeur = dto.Valeur;
                rep.TempsSecondes = dto.TempsSecondes;
                rep.SoumisLe = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return Ok();
        }

        // ============================================================
        // 3. TERMINER (Calcul du score final)
        // ============================================================
        [HttpPost("terminer/{evaluationId}")]
        public async Task<IActionResult> Terminer(Guid evaluationId)
        {
            var eval = await _context.Evaluations
                .Include(e => e.Reponses)
                .FirstOrDefaultAsync(e => e.Id == evaluationId);

            if (eval == null) return NotFound();

            float scoreObtenu = 0;
            float totalPointsPossibles = 0;

            foreach (var r in eval.Reponses)
            {
                var q = await _context.Questions.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == r.QuestionId);
                if (q == null) continue;

                float questionPoints = q.Points > 0 ? q.Points : 1;
                totalPointsPossibles += questionPoints;

                // Logique de correction simple
                string userVal = (r.Valeur ?? "").Trim().ToLower();
                string correctVal = (q.BonneReponse ?? "").Trim().ToLower();

                if (userVal == correctVal)
                {
                    scoreObtenu += questionPoints;
                }
            }

            eval.ScoreTotal = scoreObtenu;
            eval.ScorePourcentage = totalPointsPossibles > 0 ? (scoreObtenu / totalPointsPossibles) * 100 : 0;
            eval.Statut = StatutPassage.TERMINE;
            eval.DateFin = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(new { score = eval.ScoreTotal, pourcentage = (int)Math.Round(eval.ScorePourcentage) });
        }

        // ============================================================
        // 4. MON HISTORIQUE (Fix 401 et Données Vides)
        // ============================================================
  [HttpGet("mon-historique")]
public async Task<IActionResult> GetMonHistorique()
{
    // ON IGNORE LE FILTRE USERID POUR TESTER
    var historique = await _context.Candidatures
        .IgnoreQueryFilters()
        .Include(c => c.Campagne)
        .Include(c => c.Evaluation)
        // On enlève "c.CandidatId == userId" juste pour voir si n'importe quoi remonte
        .Where(c => c.Evaluation != null) 
        .Select(c => new {
            EvaluationId = c.Evaluation!.Id,
            Nom = c.Campagne != null ? c.Campagne.Nom : "Examen Test",
            DateFin = c.Evaluation.DateFin,
            Score = (int)Math.Round(c.Evaluation.ScorePourcentage)
        })
        .ToListAsync();

    return Ok(historique);
}

        // ============================================================
        // 5. RÉSULTATS DÉTAILLÉS (Pour ResultsView)
        // ============================================================
        [HttpGet("results/{evaluationId}")]
        public async Task<IActionResult> GetResults(Guid evaluationId)
        {
            var eval = await _context.Evaluations
                .Include(e => e.Candidature).ThenInclude(c => c!.Campagne)
                .Include(e => e.Reponses)
                .FirstOrDefaultAsync(e => e.Id == evaluationId);

            if (eval == null) return NotFound();

            var detailed = new List<object>();
            foreach (var r in eval.Reponses)
            {
                var q = await _context.Questions.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == r.QuestionId);
                if (q == null) continue;

                detailed.Add(new
                {
                    Name = q.Enonce, // Utilisé pour Chart.js ou liste
                    Score = (r.Valeur?.Trim().ToLower() == q.BonneReponse?.Trim().ToLower()) ? 100 : 0
                });
            }

            return Ok(new
            {
                CampaignName = eval.Candidature?.Campagne?.Nom ?? "Rapport d'évaluation",
                ScorePourcentage = eval.ScorePourcentage,
                Themes = detailed // Liste envoyée pour le Radar Chart
            });
        }
    }
}