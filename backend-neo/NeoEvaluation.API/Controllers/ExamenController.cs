using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NeoEvaluation.API.Data;
using NeoEvaluation.API.Models;
using NeoEvaluation.API.DTOs;
using Microsoft.AspNetCore.Authorization;
using NeoEvaluation.API.Services;

namespace NeoEvaluation.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ExamenController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ITenantService _tenantService;
        private readonly IEmailService _emailService;
        private readonly AiService _aiService;

        public ExamenController(
            AppDbContext context,
            ITenantService tenantService,
            IEmailService emailService,
            AiService aiService)
        {
            _context = context;
            _tenantService = tenantService;
            _emailService = emailService;
            _aiService = aiService;
        }

        // ─────────────────────────────────────────────────────────────────
        // PARTIE CANDIDAT : PASSAGE DE L'EXAMEN
        // ─────────────────────────────────────────────────────────────────

        [HttpGet("info/{candidatureId}")]
        public async Task<IActionResult> GetInfo(Guid candidatureId)
        {
            var cand = await _context.Candidatures
                .IgnoreQueryFilters()
                .Include(c => c.Campagne)
                    .ThenInclude(cp => cp!.CampagneQuestionnaires)
                    .ThenInclude(cq => cq.Questionnaire)
                .FirstOrDefaultAsync(c => c.Id == candidatureId);

            if (cand == null) return NotFound(new { message = "Candidature introuvable" });

            var qIds = cand.Campagne?.CampagneQuestionnaires
                           .Select(cq => cq.QuestionnaireId).ToList() ?? new List<Guid>();

            var totalQuestions = await _context.QuestionnaireQuestions
                .IgnoreQueryFilters()
                .Where(qq => qIds.Contains(qq.QuestionnaireId))
                .CountAsync();

            var questionsWithTimer = await _context.QuestionnaireQuestions
                .IgnoreQueryFilters()
                .Include(qq => qq.Question)
                .Where(qq => qIds.Contains(qq.QuestionnaireId)
                          && qq.Question != null
                          && qq.Question.DureeSecondes > 0)
                .CountAsync();

            return Ok(new
            {
                Titre = cand.Campagne?.Nom ?? "Certification",
                TotalQuestions = totalQuestions,
                DureeMinutes = cand.Campagne?.DureeMinutes ?? 45,
                ScoreReussite = 70,
                Theme = cand.Campagne?.Nom ?? "Général",
                AnticheatEnabled = true,
                SendNotifications = true,
                QuestionsWithTimer = questionsWithTimer
            });
        }

        [HttpGet("setup/{candidatureId}")]
        public async Task<IActionResult> GetSetup(Guid candidatureId)
        {
            var cand = await _context.Candidatures
                .IgnoreQueryFilters()
                .Include(c => c.Evaluation)
                .Include(c => c.Campagne)
                    .ThenInclude(cp => cp!.CampagneQuestionnaires)
                    .ThenInclude(cq => cq.Questionnaire)
                .FirstOrDefaultAsync(c => c.Id == candidatureId);

            if (cand == null)
                return NotFound(new { message = "Candidature introuvable" });

            if (cand.Evaluation?.Statut == StatutPassage.TERMINE)
                return BadRequest(new { message = "Cette session a déjà été soumise." });

            if (cand.Evaluation == null)
            {
                cand.Evaluation = new Evaluation
                {
                    Id = Guid.NewGuid(),
                    CandidatureId = cand.Id,
                    Statut = StatutPassage.EN_COURS,
                    DateDebut = DateTime.UtcNow,
                    CandidatId = _tenantService.GetUserId()
                };
                _context.Evaluations.Add(cand.Evaluation);
                await _context.SaveChangesAsync();
            }

            var qIds = cand.Campagne?.CampagneQuestionnaires
                           .Select(cq => cq.QuestionnaireId).ToList() ?? new List<Guid>();

            var questions = await _context.QuestionnaireQuestions
                .IgnoreQueryFilters()
                .Include(qq => qq.Question)
                .Where(qq => qIds.Contains(qq.QuestionnaireId))
                .OrderBy(qq => qq.Ordre)
                .Select(qq => new
                {
                    Id = qq.Question != null ? qq.Question.Id : Guid.Empty,
                    Enonce = qq.Question != null ? qq.Question.Enonce : "",
                    Type = qq.Question != null ? qq.Question.Type.ToString() : "QCM",
                    Choix = qq.Question != null ? qq.Question.Choix : new List<string>(),
                    Points = (qq.Question != null && qq.Question.Points > 0) ? qq.Question.Points : 1,
                    DureeSecondes = qq.Question != null ? qq.Question.DureeSecondes : 0,
                    Theme = (qq.Question != null ? qq.Question.Theme : "Général") ?? "Général"
                })
                .ToListAsync();

            return Ok(new
            {
                EvaluationId = cand.Evaluation.Id,
                Titre = cand.Campagne?.Nom,
                TempsLimite = (cand.Campagne?.DureeMinutes ?? 45) * 60,
                Questions = questions,
                ScoreReussite = 70,
                AnticheatEnabled = true,
                SendNotifications = true
            });
        }

        [HttpPost("save-response")]
        public async Task<IActionResult> SaveResponse([FromBody] ReponseDto dto)
        {
            if (dto.EvaluationId == Guid.Empty || dto.QuestionId == Guid.Empty)
                return BadRequest(new { message = "EvaluationId et QuestionId sont requis." });

            var evaluation = await _context.Evaluations.FindAsync(dto.EvaluationId);
            if (evaluation == null)
                return NotFound(new { message = "Session introuvable." });
            if (evaluation.Statut == StatutPassage.TERMINE)
                return BadRequest(new { message = "Session déjà terminée." });

            var rep = await _context.Reponses.FirstOrDefaultAsync(
                r => r.EvaluationId == dto.EvaluationId && r.QuestionId == dto.QuestionId);

            if (rep == null)
            {
                _context.Reponses.Add(new Reponse
                {
                    Id = Guid.NewGuid(),
                    EvaluationId = dto.EvaluationId,
                    QuestionId = dto.QuestionId,
                    Valeur = dto.Valeur ?? string.Empty,
                    SoumisLe = DateTime.UtcNow
                });
            }
            else
            {
                rep.Valeur = dto.Valeur ?? string.Empty;
                rep.SoumisLe = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("terminer/{evaluationId}")]
        public async Task<IActionResult> Terminer(Guid evaluationId)
        {
            var eval = await _context.Evaluations
                .Include(e => e.Reponses)
                .FirstOrDefaultAsync(e => e.Id == evaluationId);

            if (eval == null) return NotFound(new { message = "Session introuvable." });
            if (eval.Statut == StatutPassage.TERMINE)
                return Ok(new { success = true, alreadyDone = true });

            var candidature = await _context.Candidatures
                .IgnoreQueryFilters()
                .Include(c => c.Campagne)
                    .ThenInclude(cp => cp!.CampagneQuestionnaires)
                .FirstOrDefaultAsync(c => c.Id == eval.CandidatureId);

            var qIds = candidature?.Campagne?.CampagneQuestionnaires
                           .Select(cq => cq.QuestionnaireId).ToList() ?? new List<Guid>();

            var questions = await _context.QuestionnaireQuestions
                .IgnoreQueryFilters()
                .Include(qq => qq.Question)
                .Where(qq => qIds.Contains(qq.QuestionnaireId))
                .Select(qq => qq.Question)
                .Where(q => q != null)
                .ToListAsync();

            var reponses = eval.Reponses.ToDictionary(r => r.QuestionId, r => r.Valeur ?? "");

            var candidat = eval.CandidatId.HasValue ? await _context.Utilisateurs.FindAsync(eval.CandidatId) : null;
            string candidatNom = candidat != null ? $"{candidat.Prenom} {candidat.Nom}" : "Candidat Anonyme";
            string examenNom = candidature?.Campagne?.Nom ?? "Examen";

            var reponsesDict = eval.Reponses.ToDictionary(r => r.QuestionId.ToString(), r => r.Valeur ?? "");

            var payload = new {
                questions = questions!.Select(q => new {
                    id = q.Id,
                    enonce = q.Enonce,
                    type = q.Type.ToString(),
                    choix = q.Choix,
                    bonneReponse = q.BonneReponse
                }),
                reponses = reponsesDict,
                candidatNom = candidatNom,
                examenNom = examenNom
            };

            bool aiSuccess = false;
            try 
            {
                var aiResponse = await _aiService.EvaluateExamAsync(payload);
                if (aiResponse.Status == "SUCCESS" && aiResponse.Evaluation != null) 
                {
                    eval.ScorePourcentage = aiResponse.Evaluation.ScorePourcentage;
                    eval.RapportFinalIA = aiResponse.Evaluation.RapportFinal;
                    eval.CorrectionIA = System.Text.Json.JsonSerializer.Serialize(aiResponse.Evaluation.Corrections);
                    aiSuccess = true;
                }
            } 
            catch (Exception) 
            {
                // Fallback to local
            }

            if (!aiSuccess)
            {
                var reponsesLocales = eval.Reponses.ToDictionary(r => r.QuestionId, r => r.Valeur ?? "");
                float totalObtenu = 0;
                float totalPossible = 0;

                foreach (var q in questions!)
                {
                    float pts = q.Points > 0 ? q.Points : 1;
                    totalPossible += pts;

                    if (reponsesLocales.TryGetValue(q.Id, out var rawUserVal))
                    {
                        string processedUserVal = rawUserVal;
                        
                        // Traduction des index (0;2) en texte (Option A;Option C)
                        if (q.Choix != null && q.Choix.Any() && !string.IsNullOrWhiteSpace(rawUserVal))
                        {
                            var parts = rawUserVal.Split(';', StringSplitOptions.RemoveEmptyEntries);
                            var mappedValues = parts
                                .Select(p => int.TryParse(p.Trim(), out int idx) ? idx : -1)
                                .Where(idx => idx >= 0 && idx < q.Choix.Count)
                                .Select(idx => q.Choix[idx].Trim())
                                .ToList();

                            if (mappedValues.Any())
                                processedUserVal = string.Join(";", mappedValues);
                        }

                        if (EvaluerReponse(processedUserVal, q.BonneReponse ?? "")) 
                            totalObtenu += pts;
                    }
                }

                eval.ScoreTotal = totalObtenu;
                eval.ScorePourcentage = totalPossible > 0 ? (totalObtenu / totalPossible) * 100f : 0f;
            }

            eval.Statut = StatutPassage.TERMINE;
            eval.DateFin = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(new { success = true, score = eval.ScorePourcentage, aiGraded = aiSuccess });
        }

        [HttpGet("results/{evaluationId}")]
        public async Task<IActionResult> GetResults(Guid evaluationId)
        {
            var eval = await _context.Evaluations
                .IgnoreQueryFilters()
                .Include(e => e.Reponses)
                .Include(e => e.Candidature)
                    .ThenInclude(c => c!.Campagne)
                        .ThenInclude(cp => cp!.CampagneQuestionnaires)
                .FirstOrDefaultAsync(e => e.Id == evaluationId);

            if (eval == null) return NotFound(new { message = "Session introuvable." });

            var qIds = eval.Candidature?.Campagne?.CampagneQuestionnaires
                           .Select(cq => cq.QuestionnaireId).ToList() ?? new List<Guid>();

            var questionnaireQuestions = await _context.QuestionnaireQuestions
                .IgnoreQueryFilters()
                .Where(qq => qIds.Contains(qq.QuestionnaireId))
                .OrderBy(qq => qq.Ordre)
                .ToListAsync();

            var questionIds = questionnaireQuestions.Select(qq => qq.QuestionId).ToList();

            var dbQuestions = await _context.Questions
                .IgnoreQueryFilters()
                .Where(q => questionIds.Contains(q.Id))
                .ToListAsync();

            var questions = questionnaireQuestions
                .Select(qq => dbQuestions.FirstOrDefault(q => q.Id == qq.QuestionId))
                .Where(q => q != null)
                .ToList();

            var reponses = eval.Reponses.ToDictionary(r => r.QuestionId, r => r.Valeur ?? "");
            var correction = new List<object>();

            var correctionIA = new List<AiCorrectionItem>();
            if (!string.IsNullOrWhiteSpace(eval.CorrectionIA))
            {
                try {
                    correctionIA = System.Text.Json.JsonSerializer.Deserialize<List<AiCorrectionItem>>(eval.CorrectionIA, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
                } catch {}
            }
            var aiDict = correctionIA.ToDictionary(c => c.QuestionId, c => c);

            foreach (var q in questions!)
            {
                if (aiDict.TryGetValue(q.Id.ToString(), out var aiItem))
                {
                    correction.Add(new
                    {
                        Enonce = q.Enonce,
                        UserAnswer = aiItem.CandidateAnswer,
                        CorrectAnswer = aiItem.CorrectAnswer,
                        IsCorrect = aiItem.IsCorrect,
                        Options = q.Choix ?? new List<string>(),
                        Theme = q.Theme ?? "Général",
                        Points = q.Points > 0 ? q.Points : 1,
                        Explication = aiItem.Explication
                    });
                }
                else
                {
                    string rawUserVal = reponses.TryGetValue(q.Id, out var v) ? v.Trim() : "";
                    
                    string displayUserVal = rawUserVal;
                    string processedForEval = rawUserVal;

                    if (q.Choix != null && q.Choix.Any() && !string.IsNullOrWhiteSpace(rawUserVal))
                    {
                        var parts = rawUserVal.Split(';', StringSplitOptions.RemoveEmptyEntries);
                        var texts = parts
                            .Select(p => int.TryParse(p.Trim(), out int idx) ? idx : -1)
                            .Where(idx => idx >= 0 && idx < q.Choix.Count)
                            .Select(idx => q.Choix[idx].Trim())
                            .ToList();

                        if (texts.Any())
                        {
                            displayUserVal = string.Join("; ", texts);
                            processedForEval = string.Join(";", texts);
                        }
                    }

                    bool isCorrect = EvaluerReponse(processedForEval, q.BonneReponse ?? "");

                    correction.Add(new
                    {
                        Enonce = q.Enonce,
                        UserAnswer = displayUserVal,
                        CorrectAnswer = q.BonneReponse ?? "",
                        IsCorrect = isCorrect,
                        Options = q.Choix ?? new List<string>(),
                        Theme = q.Theme ?? "Général",
                        Points = q.Points > 0 ? q.Points : 1,
                        Explication = !string.IsNullOrWhiteSpace(q.BonneReponse)
                                        ? $"La bonne réponse était : {q.BonneReponse}"
                                        : ""
                    });
                }
            }

            return Ok(new
            {
                Pourcentage = (int)Math.Round(eval.ScorePourcentage),
                ScoreTotal = eval.ScoreTotal,
                DetailedCorrection = correction,
                RapportFinalIA = eval.RapportFinalIA,
                Infractions = 0
            });
        }

        [HttpGet("historique")]
        public async Task<IActionResult> GetHistorique()
        {
            var userId = _tenantService.GetUserId();
            if (userId == null) return Unauthorized(new { message = "Utilisateur non identifié." });

            var historique = await _context.Evaluations
                .IgnoreQueryFilters()
                .Include(e => e.Candidature)
                    .ThenInclude(c => c!.Campagne)
                .Where(e => e.CandidatId == userId)
                .OrderByDescending(e => e.DateDebut)
                .Select(e => new
                {
                    e.Id,
                    TitreExamen = (e.Candidature != null && e.Candidature.Campagne != null) ? e.Candidature.Campagne.Nom : "Examen",
                    Date = e.DateDebut,
                    Score = Math.Round(e.ScorePourcentage, 1),
                    Statut = e.Statut.ToString(),
                    Resultat = e.ScorePourcentage >= 70 ? "Succès" : "Échec"
                })
                .ToListAsync();

            return Ok(historique);
        }

        // ─────────────────────────────────────────────────────────────────
        // PARTIE ADMINISTRATION / RECRUTEUR
        // ─────────────────────────────────────────────────────────────────

        [HttpGet("dashboard-stats")]
        public async Task<IActionResult> GetDashboardStats()
        {
            var tenantId = _tenantService.GetTenantId();

            var query = _context.Evaluations
                .IgnoreQueryFilters()
                .Include(e => e.Candidature)
                    .ThenInclude(c => c!.Campagne)
                .Where(e => e.Candidature != null && 
                            e.Candidature.Campagne != null && 
                            e.Candidature.Campagne.EntrepriseId == tenantId);

            var total = await query.CountAsync();
            var reussites = await query.CountAsync(e => e.ScorePourcentage >= 70 && e.Statut == StatutPassage.TERMINE);
            var enCours = await query.CountAsync(e => e.Statut == StatutPassage.EN_COURS);

            var scoresTermines = await query
                .Where(e => e.Statut == StatutPassage.TERMINE)
                .Select(e => e.ScorePourcentage)
                .ToListAsync();

            double moyenne = scoresTermines.Any() ? scoresTermines.Average() : 0;

            return Ok(new
            {
                TotalSessions = total,
                Reussites = reussites,
                EnCours = enCours,
                ScoreMoyen = Math.Round(moyenne, 1)
            });
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllEvaluations()
        {
            var tenantId = _tenantService.GetTenantId();

            var list = await _context.Evaluations
                .IgnoreQueryFilters()
                .Include(e => e.Candidature)
                    .ThenInclude(c => c!.Campagne)
                .Include(e => e.Candidat)
                .Where(e => e.Candidature != null && 
                            e.Candidature.Campagne != null && 
                            e.Candidature.Campagne.EntrepriseId == tenantId)
                .OrderByDescending(e => e.DateDebut)
                .Select(e => new
                {
                    e.Id,
                    TitreExamen = (e.Candidature != null && e.Candidature.Campagne != null) ? e.Candidature.Campagne.Nom : "N/A",
                    CandidatNom = (e.Candidat != null) ? (e.Candidat.Prenom + " " + e.Candidat.Nom) : "Candidat Anonyme",
                    Date = e.DateDebut,
                    Score = Math.Round(e.ScorePourcentage, 1),
                    Statut = e.Statut.ToString(),
                    Resultat = e.ScorePourcentage >= 70 ? "Succès" : "Échec"
                })
                .ToListAsync();

            return Ok(list);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvaluation(Guid id)
        {
            var eval = await _context.Evaluations.FindAsync(id);
            if (eval == null) return NotFound();

            var reponses = _context.Reponses.Where(r => r.EvaluationId == id);
            _context.Reponses.RemoveRange(reponses);

            _context.Evaluations.Remove(eval);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Évaluation supprimée avec succès." });
        }

        [HttpPost("notify-result")]
        public async Task<IActionResult> NotifyResult([FromBody] NotifyResultDto dto)
        {
            var eval = await _context.Evaluations
                .Include(e => e.Candidature)
                    .ThenInclude(c => c!.Campagne)
                .FirstOrDefaultAsync(e => e.Id == dto.EvaluationId);

            if (eval == null)
                return NotFound(new { message = "Évaluation introuvable." });

            var candidatId = eval.CandidatId;
            if (candidatId == null)
                return BadRequest(new { message = "ID Candidat manquant sur l'évaluation." });

            var candidatEmail = await _context.Utilisateurs
                .Where(u => u.Id == candidatId)
                .Select(u => u.Email)
                .FirstOrDefaultAsync();

            if (string.IsNullOrWhiteSpace(candidatEmail))
                return BadRequest(new { message = "Email du candidat introuvable." });

            var nomExamen = eval.Candidature?.Campagne?.Nom ?? "Examen";
            var statut = dto.Passed ? "VALIDÉE ✅" : "ÉCHOUÉE ❌";
            var sujet = $"[EvaluaTech] Résultat de votre session — {nomExamen}";

            var corps = $"""
                Bonjour,

                Votre session d'examen pour « {nomExamen} » est maintenant terminée.

                📊 Résultat : {statut}
                🎯 Score obtenu : {dto.Pourcentage}%
                🛡️ Score d'intégrité : {dto.IntegrityScore}%

                Cordialement,
                L'équipe EvaluaTech
                """;

            await _emailService.SendEmailAsync(candidatEmail, sujet, corps);

            return Ok(new { sent = true, to = candidatEmail });
        }

        [HttpGet("candidate-report/{candidateId}")]
        public async Task<IActionResult> GetCandidateReport(Guid candidateId)
        {
            var allSessions = await _context.Evaluations
                .Where(e => e.CandidatId == candidateId && e.Statut == StatutPassage.TERMINE)
                .OrderByDescending(e => e.DateFin)
                .ToListAsync();

            if (!allSessions.Any()) return NotFound("Aucune session terminée.");

            var lastEval = allSessions.First();
            
            var candidature = await _context.Candidatures
                .IgnoreQueryFilters()
                .Include(c => c.Campagne).ThenInclude(cp => cp!.CampagneQuestionnaires)
                .FirstOrDefaultAsync(c => c.Id == lastEval.CandidatureId);

            var qIds = candidature?.Campagne?.CampagneQuestionnaires.Select(cq => cq.QuestionnaireId).ToList();
            
            var questions = await _context.QuestionnaireQuestions
                .IgnoreQueryFilters()
                .Include(qq => qq.Question)
                .Where(qq => qIds!.Contains(qq.QuestionnaireId))
                .Select(qq => qq.Question)
                .Where(q => q != null)
                .ToListAsync();

            var reponses = await _context.Reponses.Where(r => r.EvaluationId == lastEval.Id)
                                         .ToDictionaryAsync(r => r.QuestionId, r => r.Valeur ?? "");

            var detailedCorrection = questions.Select(q => {
                string userRaw = reponses.GetValueOrDefault(q!.Id) ?? "";
                string processedUser = userRaw;
                if (q.Choix != null && q.Choix.Any() && !string.IsNullOrWhiteSpace(userRaw))
                {
                     var parts = userRaw.Split(';', StringSplitOptions.RemoveEmptyEntries);
                     var texts = parts.Select(p => int.TryParse(p.Trim(), out int idx) && idx >= 0 && idx < q.Choix.Count ? q.Choix[idx].Trim() : p.Trim());
                     processedUser = string.Join(";", texts);
                }

                return new {
                    Enonce = q.Enonce,
                    UserAnswer = processedUser,
                    CorrectAnswer = q.BonneReponse,
                    IsCorrect = EvaluerReponse(processedUser, q.BonneReponse ?? "")
                };
            }).ToList();

            var avgScore = allSessions.Average(e => e.ScorePourcentage);

            return Ok(new
            {
                FullName = "Candidat", 
                ScoreGlobal = Math.Round(avgScore),
                IntegrityScore = 100,
                IaVerdict = avgScore >= 70 ? "Profil technique solide." : "Besoins de formation identifiés.",
                DetailedCorrection = detailedCorrection,
                History = allSessions.Select(s => new {
                    Id = s.Id,
                    Date = s.DateFin,
                    Score = s.ScorePourcentage,
                    TitreExamen = candidature?.Campagne?.Nom ?? "Examen"
                })
            });
        }

        [HttpGet("historique-candidat/{candidateId}")]
        public async Task<IActionResult> GetHistoriqueCandidat(Guid candidateId)
        {
            var historique = await _context.Evaluations
                .IgnoreQueryFilters()
                .Include(e => e.Candidature)
                    .ThenInclude(c => c!.Campagne)
                .Where(e => e.CandidatId == candidateId)
                .OrderByDescending(e => e.DateDebut)
                .Select(e => new
                {
                    e.Id,
                    TitreExamen = (e.Candidature != null && e.Candidature.Campagne != null) 
                                  ? e.Candidature.Campagne.Nom : "Examen",
                    Date = e.DateDebut,
                    Score = Math.Round(e.ScorePourcentage, 1),
                    Statut = e.Statut.ToString(),
                    Resultat = e.ScorePourcentage >= 70 ? "Succès" : "Échec",
                    Infractions = 0 
                })
                .ToListAsync();

            return Ok(historique);
        }

        // ─────────────────────────────────────────────────────────────────
        // HELPER RÉPARÉ : COMPARAISON DE TEXTE ROBUSTE
        // ─────────────────────────────────────────────────────────────────

        private static bool EvaluerReponse(string userVal, string correctVal)
        {
            // Si une des valeurs est vide, c'est faux par défaut
            if (string.IsNullOrWhiteSpace(userVal) || string.IsNullOrWhiteSpace(correctVal)) 
                return false;

            // Découpage par point-virgule, suppression des espaces et mise en minuscule
            var u = userVal.Split(';', StringSplitOptions.RemoveEmptyEntries)
                           .Select(x => x.Trim().ToLower())
                           .OrderBy(x => x)
                           .ToList();

            var c = correctVal.Split(';', StringSplitOptions.RemoveEmptyEntries)
                           .Select(x => x.Trim().ToLower())
                           .OrderBy(x => x)
                           .ToList();

            // S'il n'y a plus de données après nettoyage
            if (!u.Any() || !c.Any()) return false;

            // Comparaison des deux listes triées
            return u.SequenceEqual(c);
        }
    }
}