using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NeoEvaluation.API.Data;
using NeoEvaluation.API.Models;
using NeoEvaluation.API.DTOs;
using System.Security.Claims;

namespace NeoEvaluation.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExamenController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ExamenController(AppDbContext context) => _context = context;

        // ==========================================
        // 1. CHARGER L'EXAMEN (LOBBY / START)
        // ==========================================
        [HttpGet("setup/{candidatureId}")]
        public async Task<IActionResult> GetSetup(string candidatureId)
        {
            if (!Guid.TryParse(candidatureId, out Guid guidId))
                return BadRequest(new { message = "ID invalide" });

            var cand = await _context.Candidatures
                .IgnoreQueryFilters()
                .Include(c => c.Evaluation)
                .Include(c => c.Campagne).ThenInclude(cp => cp.CampagneQuestionnaires).ThenInclude(cq => cq.Questionnaire)
                .FirstOrDefaultAsync(c => c.Id == guidId);

            if (cand == null) return NotFound(new { message = "Candidature non trouvée" });

            // Initialiser l'évaluation si elle n'existe pas encore
            if (cand.Evaluation == null)
            {
                cand.Evaluation = new Evaluation {
                    Id = Guid.NewGuid(), 
                    CandidatureId = cand.Id,
                    Statut = StatutPassage.EN_COURS, 
                    DateDebut = DateTime.UtcNow
                };
                _context.Evaluations.Add(cand.Evaluation);
                await _context.SaveChangesAsync();
            }

            var questionnaireIds = cand.Campagne?.CampagneQuestionnaires.Select(cq => cq.QuestionnaireId).ToList() ?? new List<Guid>();
            
            var questions = await _context.QuestionnaireQuestions
                .IgnoreQueryFilters()
                .Where(qq => questionnaireIds.Contains(qq.QuestionnaireId))
                .OrderBy(qq => qq.Ordre)
                .Select(qq => qq.Question)
                .ToListAsync();

            return Ok(new {
                EvaluationId = cand.Evaluation.Id,
                CampagneNom = cand.Campagne?.Nom ?? "Examen",
                Statut = cand.Evaluation.Statut.ToString(),
                TempsLimite = (cand.Campagne?.DureeMinutes ?? 45) * 60,
                Questions = questions.Select(q => new {
                    Id = q.Id, 
                    Enonce = q.Enonce, 
                    Type = q.Type.ToString(),
                    Options = q.Choix ?? new List<string>(), 
                    Points = q.Points
                }).ToList()
            });
        }

        // ==========================================
        // 2. SAUVEGARDER UNE RÉPONSE (AUTO-SAVE)
        // ==========================================
        [HttpPost("save-response")]
        public async Task<IActionResult> SaveResponse([FromBody] ReponseDto dto)
        {
            var rep = await _context.Reponses.FirstOrDefaultAsync(r => r.EvaluationId == dto.EvaluationId && r.QuestionId == dto.QuestionId);
            
            if (rep == null) {
                _context.Reponses.Add(new Reponse { 
                    Id = Guid.NewGuid(), 
                    EvaluationId = dto.EvaluationId, 
                    QuestionId = dto.QuestionId, 
                    Valeur = dto.Valeur, 
                    TempsSecondes = dto.TempsSecondes,
                    SoumisLe = DateTime.UtcNow
                });
            } else {
                rep.Valeur = dto.Valeur;
                rep.TempsSecondes = dto.TempsSecondes;
                rep.SoumisLe = DateTime.UtcNow;
            }
            
            await _context.SaveChangesAsync();
            return Ok();
        }

        // ==========================================
        // 3. TERMINER L'EXAMEN ET CALCULER LE SCORE
        // ==========================================
        [HttpPost("terminer/{evaluationId}")]
        public async Task<IActionResult> Terminer(Guid evaluationId)
        {
            var eval = await _context.Evaluations
                .Include(e => e.Reponses)
                .FirstOrDefaultAsync(e => e.Id == evaluationId);

            if (eval == null) return NotFound();

            float scoreObtenu = 0;
            float totalPointsPossibles = 0;

            foreach(var r in eval.Reponses) {
                var q = await _context.Questions.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == r.QuestionId);
                if (q == null) continue;

                float questionPoints = q.Points > 0 ? q.Points : 1;
                totalPointsPossibles += questionPoints;

                bool isCorrect = false;
                string userVal = (r.Valeur ?? "").Trim().ToLower();
                string correctVal = (q.BonneReponse ?? "").Trim().ToLower();

                // Logique de validation QCU / QCM / VRAI_FAUX
                if (q.Type == TypeQuestion.QCU || q.Type == TypeQuestion.VRAI_FAUX) {
                    if (userVal == correctVal) isCorrect = true;
                    else if (int.TryParse(userVal, out int uIdx) && q.Choix != null && uIdx < q.Choix.Count) {
                        if (q.Choix[uIdx].Trim().ToLower() == correctVal) isCorrect = true;
                    }
                }
                else if (q.Type == TypeQuestion.QCM) {
                    var uList = userVal.Split(';').Select(s => s.Trim()).OrderBy(s => s);
                    var cList = correctVal.Split(';').Select(s => s.Trim()).OrderBy(s => s);
                    isCorrect = uList.SequenceEqual(cList);
                }

                if (isCorrect) scoreObtenu += questionPoints;
            }

            eval.ScoreTotal = scoreObtenu;
            eval.ScorePourcentage = totalPointsPossibles > 0 ? (scoreObtenu / totalPointsPossibles) * 100 : 0;
            eval.Statut = StatutPassage.TERMINE;
            eval.DateFin = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(new { score = eval.ScoreTotal, pourcentage = (int)Math.Round(eval.ScorePourcentage) });
        }

        // ==========================================
        // 4. HISTORIQUE (POUR DASHBOARD) - LA SOLUTION
        // ==========================================
        [HttpGet("mon-historique")]
        public async Task<IActionResult> GetMonHistorique()
        {
            // Note: Si vous avez une gestion d'utilisateurs, filtrez par CandidatId ici
            // var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var historique = await _context.Candidatures
                .Include(c => c.Campagne)
                .Include(c => c.Evaluation)
                .Where(c => c.Evaluation != null && c.Evaluation.Statut == StatutPassage.TERMINE)
                .Select(c => new {
                    Id = c.Id,
                    EvaluationId = c.Evaluation.Id,
                    Nom = c.Campagne.Nom,
                    DateFin = c.Evaluation.DateFin, 
                    DureeMinutes = c.Campagne.DureeMinutes,
                    Score = (int)Math.Round(c.Evaluation.ScorePourcentage)
                })
                .OrderByDescending(c => c.DateFin)
                .ToListAsync();

            return Ok(historique);
        }

        // ==========================================
        // 5. RÉSULTATS DÉTAILLÉS (CORRECTION)
        // ==========================================
        [HttpGet("results/{evaluationId}")]
        public async Task<IActionResult> GetResults(Guid evaluationId)
        {
            var eval = await _context.Evaluations
                .Include(e => e.Reponses)
                .FirstOrDefaultAsync(e => e.Id == evaluationId);

            if (eval == null) return NotFound();

            var detailed = new List<object>();
            foreach (var r in eval.Reponses)
            {
                var q = await _context.Questions.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == r.QuestionId);
                if (q == null) continue;

                string uVal = (r.Valeur ?? "").Trim().ToLower();
                string cVal = (q.BonneReponse ?? "").Trim().ToLower();
                
                // On réutilise la même logique hybride que dans Terminer()
                bool isCorrect = (uVal == cVal);
                if (!isCorrect && int.TryParse(uVal, out int idx) && q.Choix != null && idx < q.Choix.Count)
                    isCorrect = q.Choix[idx].Trim().ToLower() == cVal;

                detailed.Add(new {
                    Enonce = q.Enonce,
                    UserAnswer = r.Valeur,
                    CorrectAnswer = q.BonneReponse,
                    IsCorrect = isCorrect
                });
            }

            return Ok(new {
                ScoreTotal = eval.ScoreTotal,
                Pourcentage = (int)Math.Round(eval.ScorePourcentage),
                DetailedCorrection = detailed
            });
        }
    }
}