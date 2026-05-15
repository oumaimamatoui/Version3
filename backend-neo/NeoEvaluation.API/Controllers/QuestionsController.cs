using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using NeoEvaluation.API.Data;
using NeoEvaluation.API.Models;
using NeoEvaluation.API.Services;
using NeoEvaluation.API.Attributes;
using NeoEvaluation.API.DTOs;

namespace NeoEvaluation.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class QuestionsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ITenantService _tenantService;

        public QuestionsController(AppDbContext context, ITenantService tenantService)
        {
            _context = context;
            _tenantService = tenantService;
        }

        // ══════════════════════════════════════════════════════════════
        // GET /api/Questions
        // FIX: suppression du GroupBy(Enonce) qui écrasait les questions
        //      bilingues FR/EN. On retourne TOUTES les questions.
        // ══════════════════════════════════════════════════════════════
        [HttpGet]
        public async Task<IActionResult> GetQuestions()
        {
            try
            {
                var entId = _tenantService.GetTenantId();

                Console.WriteLine($"[DEBUG BANQUE] Accès par : {User.Identity?.Name} | Entreprise : {entId}");

                if (!entId.HasValue || entId == Guid.Empty)
                    return Unauthorized(new { message = "ID d'entreprise manquant dans votre session." });

                var questions = await _context.Questions
                    .Where(q => q.EntrepriseId == entId.Value)
                    .OrderByDescending(q => q.CreeLe)
                    .ToListAsync();

                return Ok(questions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur serveur", detail = ex.Message });
            }
        }

        // ══════════════════════════════════════════════════════════════
        // GET /api/Questions/{id}
        // ══════════════════════════════════════════════════════════════
        [HttpGet("{id}")]
        public async Task<IActionResult> GetQuestion(Guid id)
        {
            try
            {
                var entId = _tenantService.GetTenantId();

                var q = await _context.Questions
                    .FirstOrDefaultAsync(x => x.Id == id && x.EntrepriseId == entId);

                if (q == null)
                    return NotFound(new { message = "Question non trouvée ou accès refusé." });

                return Ok(q);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur serveur", detail = ex.Message });
            }
        }

        // ══════════════════════════════════════════════════════════════
        // GET /api/Questions/stats
        // Retourne les stats KPI utilisées par le frontend
        // ══════════════════════════════════════════════════════════════
        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            try
            {
                var entId = _tenantService.GetTenantId();
                if (!entId.HasValue)
                    return Unauthorized(new { message = "Session invalide." });

                var questions = await _context.Questions
                    .Where(q => q.EntrepriseId == entId.Value)
                    .ToListAsync();

                var stats = new
                {
                    Total      = questions.Count,
                    ParLangue  = questions
                                    .GroupBy(q => q.Langue ?? "fr")
                                    .ToDictionary(g => g.Key, g => g.Count()),
                    ParType    = questions
                                    .GroupBy(q => q.Type)
                                    .ToDictionary(g => g.Key.ToString(), g => g.Count()),
                    Difficiles = questions.Count(q => q.Points >= 4)
                };

                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur stats", detail = ex.Message });
            }
        }

        // ══════════════════════════════════════════════════════════════
        // GET /api/Questions/by-lang/{lang}
        // Filtre par langue côté serveur — évite de tout charger côté client
        // ══════════════════════════════════════════════════════════════
        [HttpGet("by-lang/{lang}")]
        public async Task<IActionResult> GetByLang(string lang)
        {
            try
            {
                var entId = _tenantService.GetTenantId();
                if (!entId.HasValue)
                    return Unauthorized(new { message = "Session invalide." });

                var validLangs = new[] { "fr", "en" };
                if (!validLangs.Contains(lang.ToLower()))
                    return BadRequest(new { message = "Langue invalide. Utiliser 'fr' ou 'en'." });

                var questions = await _context.Questions
                    .Where(q => q.EntrepriseId == entId.Value && q.Langue == lang.ToLower())
                    .OrderByDescending(q => q.CreeLe)
                    .ToListAsync();

                return Ok(questions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur", detail = ex.Message });
            }
        }

        // ══════════════════════════════════════════════════════════════
        // POST /api/Questions
        // FIX: ajout Langue + cast supprimé (enums dans le DTO)
        //      Theme et SousTheme garantis non-null
        // ══════════════════════════════════════════════════════════════
        [HttpPost]
        [RequirePermission("edit_bank")]
        public async Task<IActionResult> PostQuestion([FromBody] QuestionCreateDto dto)
        {
            try
            {
                var entId = _tenantService.GetTenantId();
                if (!entId.HasValue)
                    return Unauthorized(new { message = "Session invalide." });

                var question = new Question
                {
                    Id            = Guid.NewGuid(),
                    Enonce        = dto.Enonce?.Trim() ?? string.Empty,
                    Type          = dto.Type,
                    Niveau        = dto.Niveau,
                    Points        = dto.Points > 0 ? dto.Points : 1,
                    DureeSecondes = dto.DureeSecondes ?? 60,
                    Theme         = dto.Theme?.Trim() ?? string.Empty,
                    SousTheme     = dto.SousTheme?.Trim() ?? string.Empty,
                    Langue        = dto.Langue ?? "fr",
                    EntrepriseId  = entId.Value,
                    Choix         = dto.Choix ?? new List<string>(),
                    BonneReponse  = dto.BonneReponse ?? string.Empty,
                    CreeLe        = DateTime.UtcNow
                };

                _context.Questions.Add(question);

                // Liaison questionnaire optionnelle
                if (dto.QuestionnaireId.HasValue && dto.QuestionnaireId != Guid.Empty)
                {
                    var maxOrder = await _context.QuestionnaireQuestions
                        .Where(qq => qq.QuestionnaireId == dto.QuestionnaireId.Value)
                        .Select(qq => (int?)qq.Ordre)
                        .MaxAsync();

                    _context.QuestionnaireQuestions.Add(new QuestionnaireQuestion
                    {
                        QuestionnaireId = dto.QuestionnaireId.Value,
                        QuestionId      = question.Id,
                        Ordre           = (maxOrder ?? 0) + 1,
                        Ponderation     = dto.Points
                    });
                }

                await _context.SaveChangesAsync();
                return Ok(question);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors de la création", detail = ex.Message });
            }
        }

        // ══════════════════════════════════════════════════════════════
        // PUT /api/Questions/{id}
        // FIX: mise à jour Langue + Theme + SousTheme ajoutés
        //      cast supprimé (enums dans le DTO)
        // ══════════════════════════════════════════════════════════════
        [HttpPut("{id}")]
        [RequirePermission("edit_bank")]
        public async Task<IActionResult> PutQuestion(Guid id, [FromBody] QuestionCreateDto dto)
        {
            try
            {
                var entId = _tenantService.GetTenantId();

                var q = await _context.Questions
                    .FirstOrDefaultAsync(x => x.Id == id && x.EntrepriseId == entId);

                if (q == null)
                    return NotFound(new { message = "Question non trouvée ou accès refusé." });

                q.Enonce        = dto.Enonce?.Trim() ?? q.Enonce;
                q.Type          = dto.Type;
                q.Niveau        = dto.Niveau;
                q.Points        = dto.Points > 0 ? dto.Points : q.Points;
                q.DureeSecondes = dto.DureeSecondes ?? q.DureeSecondes;
                q.Theme         = dto.Theme?.Trim() ?? string.Empty;
                q.SousTheme     = dto.SousTheme?.Trim() ?? string.Empty;
                q.Langue        = dto.Langue ?? q.Langue;
                q.Choix         = dto.Choix ?? new List<string>();
                q.BonneReponse  = dto.BonneReponse ?? string.Empty;

                await _context.SaveChangesAsync();
                return Ok(q);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors de la mise à jour", detail = ex.Message });
            }
        }

        // ══════════════════════════════════════════════════════════════
        // DELETE /api/Questions/{id}
        // Nettoyage complet des liaisons avant suppression
        // ══════════════════════════════════════════════════════════════
        [HttpDelete("{id}")]
        [RequirePermission("edit_bank")]
        public async Task<IActionResult> DeleteQuestion(Guid id)
        {
            try
            {
                var entId = _tenantService.GetTenantId();

                var q = await _context.Questions
                    .Include(x => x.QuestionnaireQuestions)
                    .FirstOrDefaultAsync(x => x.Id == id && x.EntrepriseId == entId);

                if (q == null)
                    return NotFound(new { message = "Question non trouvée." });

                _context.QuestionnaireQuestions.RemoveRange(q.QuestionnaireQuestions);

                var reponses = await _context.Reponses
                    .Where(r => r.QuestionId == id)
                    .ToListAsync();
                _context.Reponses.RemoveRange(reponses);

                _context.Questions.Remove(q);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Question supprimée avec succès." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors de la suppression", detail = ex.Message });
            }
        }

        // ══════════════════════════════════════════════════════════════
        // POST /api/Questions/seed-demo
        // FIX: ajout Langue dans les questions de test
        //      deux questions créées — une FR et une EN
        // ══════════════════════════════════════════════════════════════
        [HttpPost("seed-demo")]
        public async Task<IActionResult> SeedDemo()
        {
            try
            {
                var entId = _tenantService.GetTenantId();
                if (!entId.HasValue)
                    return Unauthorized(new { message = "Session invalide." });

                var demos = new List<Question>
                {
                    new Question
                    {
                        Id           = Guid.NewGuid(),
                        Enonce       = "Qu'est-ce que la programmation orientée objet ?",
                        Type         = TypeQuestion.QCU,
                        Niveau       = NiveauComplexite.INTERMEDIAIRE,
                        Points       = 2,
                        Langue       = "fr",
                        EntrepriseId = entId.Value,
                        Theme        = "Informatique",
                        SousTheme    = "POO",
                        Choix        = new List<string>
                        {
                            "Un paradigme de programmation",
                            "Un langage",
                            "Un framework",
                            "Un système d'exploitation"
                        },
                        BonneReponse = "Un paradigme de programmation",
                        CreeLe       = DateTime.UtcNow
                    },
                    new Question
                    {
                        Id           = Guid.NewGuid(),
                        Enonce       = "What is object-oriented programming?",
                        Type         = TypeQuestion.QCU,
                        Niveau       = NiveauComplexite.INTERMEDIAIRE,
                        Points       = 2,
                        Langue       = "en",
                        EntrepriseId = entId.Value,
                        Theme        = "Informatique",
                        SousTheme    = "POO",
                        Choix        = new List<string>
                        {
                            "A programming paradigm",
                            "A language",
                            "A framework",
                            "An operating system"
                        },
                        BonneReponse = "A programming paradigm",
                        CreeLe       = DateTime.UtcNow
                    }
                };

                _context.Questions.AddRange(demos);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = $"{demos.Count} questions de démonstration créées.",
                    ids     = demos.Select(d => d.Id)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur seed", detail = ex.Message });
            }
        }
    }
}