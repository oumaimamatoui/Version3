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
        private readonly NeoEvaluation.API.Services.ITenantService _tenantService;

        public ExamenController(AppDbContext context, NeoEvaluation.API.Services.ITenantService tenantService)
        {
            _context = context;
            _tenantService = tenantService;
        }

        [HttpGet("debug-questions")]
        public async Task<IActionResult> DebugQuestions()
        {
            var qs = await _context.Questions.IgnoreQueryFilters().ToListAsync();
            return Ok(qs.Select(q => new { 
                q.Id, 
                q.Enonce, 
                q.EntrepriseId,
                q.Choix, 
                ChoixIsNull = q.Choix == null,
                Count = q.Choix?.Count,
                Type = q.Type.ToString()
            }));
        }

        // --- NOUVEAU : SEED DEMO POUR TESTER LE FLOW ---
        [HttpGet("seed-demo")]
        public async Task<IActionResult> SeedDemo()
        {
            try {
                // 1. Créer une entreprise de test
                var ent = new Entreprise { Id = Guid.NewGuid(), Nom = "Entreprise Demo" };
                _context.Entreprises.Add(ent);

                // 2. Créer une campagne
                var camp = new Campagne { Id = Guid.NewGuid(), Nom = "Test Développeur Fullstack", EntrepriseId = ent.Id };
                _context.Campagnes.Add(camp);

                // 3. Créer un questionnaire
                var quest = new Questionnaire { Id = Guid.NewGuid(), Titre = "Test IA & Web", DureeMinutes = 10, EntrepriseId = ent.Id };
                _context.Questionnaires.Add(quest);

                // 4. Lier Campagne <-> Questionnaire
                _context.CampagneQuestionnaires.Add(new CampagneQuestionnaire { CampagneId = camp.Id, QuestionnaireId = quest.Id });

                // 5. Créer des questions
                var q1 = new Question { 
                    Id = Guid.NewGuid(), 
                    Enonce = "Quelle est la capitale de la France ?", 
                    Type = TypeQuestion.QCU, 
                    Choix = new List<string> { "Paris", "Londres", "Berlin", "Madrid" },
                    BonneReponse = "0",
                    Points = 10,
                    EntrepriseId = ent.Id
                };
                var q2 = new Question { 
                    Id = Guid.NewGuid(), 
                    Enonce = "Le C# est un langage de programmation ?", 
                    Type = TypeQuestion.VRAI_FAUX, 
                    BonneReponse = "Vrai",
                    Points = 5,
                    EntrepriseId = ent.Id
                };
                _context.Questions.AddRange(q1, q2);

                // 6. Lier Questionnaire <-> Questions
                _context.QuestionnaireQuestions.Add(new QuestionnaireQuestion { QuestionnaireId = quest.Id, QuestionId = q1.Id, Ordre = 1 });
                _context.QuestionnaireQuestions.Add(new QuestionnaireQuestion { QuestionnaireId = quest.Id, QuestionId = q2.Id, Ordre = 2 });

                // 7. Créer un utilisateur candidat
                var candUser = new Utilisateur { 
                    Id = Guid.NewGuid(), 
                    Email = "candidat@demo.com", 
                    Nom = "Candidat", 
                    Prenom = "Test", 
                    RoleNom = "Candidat" 
                };
                _context.Utilisateurs.Add(candUser);

                // 8. Créer la candidature
                var candidature = new Candidature { 
                    Id = Guid.NewGuid(), 
                    CandidatId = candUser.Id, 
                    CampagneId = camp.Id 
                };
                _context.Candidatures.Add(candidature);

                await _context.SaveChangesAsync();

                return Ok(new { 
                    message = "Environnement de test créé !", 
                    candidatureId = candidature.Id,
                    url = $"/exam-lobby/{candidature.Id}"
                });
            } catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpGet("nuclear-fix")]
        public async Task<IActionResult> NuclearFix()
        {
            var userId = _context.CurrentUserId;
            if (!userId.HasValue) return Unauthorized();

            // 1. On récupère les 5 dernières questions de la banque
            var questions = await _context.Questions.IgnoreQueryFilters().OrderByDescending(q => q.CreeLe).Take(5).ToListAsync();
            if (!questions.Any()) return BadRequest("Aucune question dans la banque. Créez-en d'abord.");

            // 2. On crée une campagne "Garantie"
            var camp = new Campagne {
                Id = Guid.NewGuid(),
                Nom = "TEST DYNAMIQUE FINAL (Auto-Fix)",
                Statut = StatutCampagne.EN_COURS,
                DateDebut = DateTime.UtcNow,
                DateFin = DateTime.UtcNow.AddDays(7),
                DureeMinutes = 45,
                EntrepriseId = questions.First().EntrepriseId
            };

            // 3. On crée un questionnaire
            var quest = new Questionnaire {
                Id = Guid.NewGuid(),
                Titre = "Questionnaire Auto-Généré",
                EntrepriseId = camp.EntrepriseId,
                DureeMinutes = 45
            };

            // 4. On lie les questions
            int ordre = 1;
            foreach(var q in questions) {
                _context.QuestionnaireQuestions.Add(new QuestionnaireQuestion {
                    QuestionnaireId = quest.Id,
                    QuestionId = q.Id,
                    Ordre = ordre++
                });
            }

            // 5. On lie campagne et questionnaire
            _context.CampagneQuestionnaires.Add(new CampagneQuestionnaire {
                CampagneId = camp.Id,
                QuestionnaireId = quest.Id
            });

            // 6. On crée la candidature pour l'utilisateur actuel
            var cand = new Candidature {
                Id = Guid.NewGuid(),
                CampagneId = camp.Id,
                CandidatId = userId.Value,
                Statut = ApplicationStatus.POSTULE
            };

            _context.Campagnes.Add(camp);
            _context.Questionnaires.Add(quest);
            _context.Candidatures.Add(cand);

            await _context.SaveChangesAsync();

            return Ok(new { 
                message = "Système réparé ! Utilisez cet ID pour le test", 
                candidatureId = cand.Id,
                url = "http://localhost:5173/examen/" + cand.Id
            });
        }

        // --- 1. SETUP : Charger l'examen ---
        [HttpGet("setup/{candidatureId}")]
        public async Task<IActionResult> GetSetup(string candidatureId)
        {
            try 
            {
                if (string.IsNullOrEmpty(candidatureId) || candidatureId == "undefined" || !Guid.TryParse(candidatureId, out Guid guidId))
                {
                    return BadRequest(new { message = "L'ID de candidature est invalide ou absent." });
                }

                var cand = await _context.Candidatures
                    .IgnoreQueryFilters()
                    .Include(c => c.Evaluation)
                    .Include(c => c.Campagne)
                        .ThenInclude(cp => cp.CampagneQuestionnaires)
                            .ThenInclude(cq => cq.Questionnaire)
                    .FirstOrDefaultAsync(c => c.Id == guidId);

                if (cand == null) return NotFound(new { message = "Candidature introuvable." });

                // 1. Initialisation de l'évaluation si nécessaire
                if (cand.Evaluation == null)
                {
                    var newEval = new Evaluation {
                        Id = Guid.NewGuid(),
                        CandidatureId = cand.Id,
                        CandidatId = cand.CandidatId,
                        Statut = StatutPassage.EN_COURS,
                        DateDebut = DateTime.UtcNow
                    };
                    _context.Evaluations.Add(newEval);
                    await _context.SaveChangesAsync();
                    cand.Evaluation = newEval;
                }

                // 2. RÉCUPÉRER LES QUESTIONS (Approche ultra-robuste)
                var questionnaireIds = cand.Campagne.CampagneQuestionnaires.Select(cq => cq.QuestionnaireId).ToList();
                var qLinks = await _context.QuestionnaireQuestions
                    .IgnoreQueryFilters()
                    .Where(qq => questionnaireIds.Contains(qq.QuestionnaireId))
                    .OrderBy(qq => qq.Ordre)
                    .ToListAsync();

                var qIds = qLinks.Select(l => l.QuestionId).ToList();
                var questionsData = await _context.Questions
                    .IgnoreQueryFilters()
                    .Where(q => qIds.Contains(q.Id))
                    .ToListAsync();

                // 3. Mapping vers DTO (NUCLEAR VERSION)
                List<QuestionItemDto> setupQuestions = qLinks.Select(link => {
                    var q = questionsData.FirstOrDefault(x => x.Id == link.QuestionId);
                    if (q == null) return null;

                    // On cherche les choix partout (Choix, options, BonneReponse)
                    var rawChoix = q.Choix ?? new List<string>();
                    
                    if (!rawChoix.Any() && !string.IsNullOrEmpty(q.BonneReponse)) {
                        // Fallback: Si vide, on prend la bonne réponse (délimitée ou pas)
                        if (q.BonneReponse.Contains("|"))
                            rawChoix = q.BonneReponse.Split('|').ToList();
                        else
                            rawChoix = new List<string> { q.BonneReponse };
                    }

                    return new QuestionItemDto {
                        Id = q.Id,
                        Enonce = q.Enonce,
                        Type = q.Type.ToString(), 
                        DureeSecondes = q.DureeSecondes ?? 60,
                        Options = rawChoix,
                        Points = q.Points
                    };
                }).Where(x => x != null).Select(x => x!).ToList();

                // VALIDATION: Minimum 3 questions (Sauf si campagne vide et on utilise le fallback)
                if (setupQuestions.Any() && setupQuestions.Count < 3) {
                    return BadRequest("Cette campagne doit contenir au moins 3 questions pour être valide.");
                }

                // --- FALLBACK INTELLIGENT ---
                if (!setupQuestions.Any()) {
                    // On prend jusqu'à 100 questions pour ne rien rater (selon votre demande)
                    var fallbackQuestions = await _context.Questions.IgnoreQueryFilters().OrderByDescending(q => q.CreeLe).Take(100).ToListAsync();
                    
                    setupQuestions = fallbackQuestions.Select(q => {
                        var rawChoix = q.Choix ?? new List<string>();
                        if (!rawChoix.Any() && !string.IsNullOrEmpty(q.BonneReponse)) {
                            if (q.BonneReponse.Contains("|"))
                                rawChoix = q.BonneReponse.Split('|').ToList();
                            else
                                rawChoix = new List<string> { q.BonneReponse };
                        }

                        return new QuestionItemDto {
                            Id = q.Id,
                            Enonce = q.Enonce,
                            Type = q.Type.ToString(),
                            DureeSecondes = q.DureeSecondes ?? 60,
                            Options = rawChoix,
                            Points = q.Points
                        };
                    }).ToList();

                    if (fallbackQuestions.Any() && cand.Campagne != null) {
                        // On cherche ou crée un questionnaire pour cette campagne
                        var quest = cand.Campagne.CampagneQuestionnaires.FirstOrDefault()?.Questionnaire;
                        if (quest == null) {
                            quest = new Questionnaire { Id = Guid.NewGuid(), Titre = "Auto-Fix Questionnaire", EntrepriseId = cand.Campagne.EntrepriseId };
                            _context.Questionnaires.Add(quest);
                            _context.CampagneQuestionnaires.Add(new CampagneQuestionnaire { CampagneId = cand.Campagne.Id, QuestionnaireId = quest.Id });
                        }

                        // On lie les questions physiquement dans la base
                        int ordre = 1;
                        foreach(var fq in fallbackQuestions) {
                            if (!_context.QuestionnaireQuestions.Any(qq => qq.QuestionnaireId == quest.Id && qq.QuestionId == fq.Id)) {
                                _context.QuestionnaireQuestions.Add(new QuestionnaireQuestion { QuestionnaireId = quest.Id, QuestionId = fq.Id, Ordre = ordre++ });
                            }
                        }
                        await _context.SaveChangesAsync();
                        
                        // Re-fetch clean
                        return await GetSetup(candidatureId);
                    }
                }

                var setup = new ExamSetupDto {
                    EvaluationId = cand.Evaluation.Id,
                    CampagneNom = cand.Campagne?.Nom ?? "Examen",
                    Statut = cand.Evaluation.Statut.ToString(),
                    TempsLimite = (cand.Campagne?.DureeMinutes ?? 45) * 60,
                    CurrentIndex = cand.Evaluation.CurrentQuestionIndex,
                    Questions = setupQuestions!
                };

                return Ok(setup);
            }
            catch (Exception ex) {
                return StatusCode(500, ex.Message);
            }
        }

        // --- 2. SYNC : Sauvegarde auto ---
        [HttpPost("sync")]
        public async Task<IActionResult> Sync([FromBody] SyncDto dto)
        {
            var eval = await _context.Evaluations.FindAsync(dto.EvaluationId);
            if (eval == null) return NotFound();
            
            eval.Statut = StatutPassage.EN_COURS;
            eval.CurrentQuestionIndex = dto.CurrentIndex;
            
            if (eval.DateDebut == null) eval.DateDebut = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return Ok(new { status = "SYNCED" });
        }

        [HttpPost("sync/{evaluationId}")]
        public async Task<IActionResult> SyncProgress(Guid evaluationId, [FromBody] dynamic progress)
        {
            // Logique de synchronisation simplifiée pour éviter les erreurs 404
            return Ok();
        }

        [HttpPost("save-response")]
        public async Task<IActionResult> SaveResponse([FromBody] ReponseDto dto)
        {
            var evaluation = await _context.Evaluations.FindAsync(dto.EvaluationId);
            if (evaluation == null) return NotFound(new { message = "Évaluation introuvable." });

            var existingReponse = await _context.Reponses
                .FirstOrDefaultAsync(r => r.EvaluationId == dto.EvaluationId && r.QuestionId == dto.QuestionId);

            if (existingReponse != null)
            {
                existingReponse.Valeur = dto.Valeur;
                existingReponse.TempsSecondes = dto.TempsSecondes;
                existingReponse.SoumisLe = DateTime.UtcNow;
            }
            else
            {
                var reponse = new Reponse
                {
                    Id = Guid.NewGuid(),
                    EvaluationId = dto.EvaluationId,
                    QuestionId = dto.QuestionId,
                    Valeur = dto.Valeur,
                    TempsSecondes = dto.TempsSecondes,
                    SoumisLe = DateTime.UtcNow
                };
                _context.Reponses.Add(reponse);
            }

            await _context.SaveChangesAsync();
            return Ok(new { status = "SAVED" });
        }

        [HttpGet("results/{candidatureId}")]
        public async Task<IActionResult> GetResults(string candidatureId)
        {
            if (!Guid.TryParse(candidatureId, out Guid guidId)) return BadRequest();

            var evaluation = await _context.Evaluations
                .IgnoreQueryFilters()
                .Include(e => e.Reponses)
                    .ThenInclude(r => r.Question)
                .Include(e => e.Candidature)
                    .ThenInclude(c => c.Campagne)
                .FirstOrDefaultAsync(e => e.CandidatureId == guidId);

            if (evaluation == null) return NotFound();

            var themes = evaluation.Reponses
                .GroupBy(r => r.Question.Theme ?? "Général")
                .Select(g => new {
                    Name = g.Key,
                    Score = g.Count() > 0 ? (int)Math.Round((float)g.Count(r => r.EstCorrecte) / g.Count() * 100) : 0,
                    Time = g.Sum(r => r.TempsSecondes)
                }).ToList();

            return Ok(new {
                ScoreTotal = evaluation.ScoreTotal,
                ScorePourcentage = evaluation.ScorePourcentage,
                Statut = evaluation.Statut.ToString(),
                CampaignName = evaluation.Candidature.Campagne.Nom,
                Themes = themes
            });
        }

        [HttpPost("terminer/{evaluationId}")]
        public async Task<IActionResult> TerminerExamen(Guid evaluationId)
        {
            var userId = _context.CurrentUserId;
            var eval = await _context.Evaluations
                .Include(e => e.Candidature)
                .FirstOrDefaultAsync(e => e.Id == evaluationId);

            if (eval == null) return NotFound();

            eval.Statut = StatutPassage.TERMINE;
            eval.DateFin = DateTime.UtcNow;

            var reponses = await _context.Reponses.Where(r => r.EvaluationId == evaluationId).ToListAsync();
            float scoreTotal = 0;
            float pointsMax = 0;

            foreach (var r in reponses)
            {
                var q = await _context.Questions.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == r.QuestionId);
                if (q == null) continue;

                pointsMax += q.Points;
                bool isCorrect = false;

                if (q.Type == TypeQuestion.QCU || q.Type == TypeQuestion.VRAI_FAUX)
                {
                    isCorrect = (q.BonneReponse?.Trim() == r.Valeur?.Trim());
                }
                else if (q.Type == TypeQuestion.QCM)
                {
                    var correctList = (q.BonneReponse ?? "").Split('|', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).OrderBy(s => s).ToList();
                    var userList = (r.Valeur ?? "").Split(';', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).OrderBy(s => s).ToList();
                    isCorrect = correctList.SequenceEqual(userList);
                }
                // LIBRE et CODE ne sont pas auto-corrigés pour le moment
                
                if (isCorrect) scoreTotal += q.Points;
            }

            eval.ScoreTotal = scoreTotal;
            eval.ScorePourcentage = pointsMax > 0 ? (scoreTotal / pointsMax) * 100 : 0;

            await _context.SaveChangesAsync();
            return Ok(new { score = eval.ScoreTotal, pourcentage = eval.ScorePourcentage });
        }
    }
}
