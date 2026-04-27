using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using NeoEvaluation.API.Data;
using NeoEvaluation.API.Models;
using NeoEvaluation.API.Services;
using NeoEvaluation.API.Attributes;

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

        [HttpGet]
        public async Task<IActionResult> GetQuestions()
        {
            var questions = await _context.Questions
                .OrderByDescending(q => q.CreeLe)
                .ToListAsync();
                
            var result = questions
                .GroupBy(q => q.Enonce)
                .Select(g => g.OrderByDescending(q => q.Choix?.Count ?? 0).ThenByDescending(q => q.CreeLe).First())
                .ToList();
                
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetQuestion(Guid id)
        {
            var q = await _context.Questions.FindAsync(id);
            return q == null ? NotFound() : Ok(q);
        }

        [HttpGet("ByQuestionnaire/{questionnaireId}")]
        public async Task<IActionResult> GetQuestionsByQuestionnaire(Guid questionnaireId)
        {
            var questions = await _context.QuestionnaireQuestions
                .Where(qq => qq.QuestionnaireId == questionnaireId)
                .OrderBy(qq => qq.Ordre)
                .Select(qq => qq.Question)
                .ToListAsync();

            return Ok(questions);
        }

        [HttpPost("seed-demo")]
        public async Task<IActionResult> SeedDemo()
        {
            var entId = _tenantService.GetTenantId();
            if (!entId.HasValue) return Unauthorized();

            var q = new Question {
                Id = Guid.NewGuid(),
                Enonce = "Quelle est la capitale de la Tunisie ?",
                Type = TypeQuestion.QCU,
                Points = 2,
                EntrepriseId = entId.Value,
                Choix = new List<string> { "Tunis", "Sousse", "Sfax", "Bizerte" },
                BonneReponse = "Tunis",
                CreeLe = DateTime.UtcNow
            };

            _context.Questions.Add(q);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Question démo créée !", questionId = q.Id });
        }

        [HttpPost]
        [RequirePermission("edit_bank")]
        public async Task<IActionResult> PostQuestion([FromBody] NeoEvaluation.API.DTOs.QuestionCreateDto dto)
        {
            try {
                var entId = _tenantService.GetTenantId();
                if (!entId.HasValue) return Unauthorized();

                // DEBUG POUR VOIR SI LE FRONTEND ENVOIE BIEN LES CHOIX
                Console.WriteLine($"[DEBUG POST] Enonce: {dto.Enonce} | Choix Count: {dto.Choix?.Count ?? 0}");

                var question = new Question {
                    Id = Guid.NewGuid(),
                    Enonce = dto.Enonce,
                    Type = dto.Type,
                    Niveau = dto.Niveau,
                    Points = dto.Points,
                    DureeSecondes = dto.DureeSecondes ?? 60,
                    Theme = dto.Theme,
                    SousTheme = dto.SousTheme,
                    EntrepriseId = entId.Value,
                    Choix = dto.Choix ?? new List<string>(),
                    BonneReponse = dto.BonneReponse ?? string.Empty,
                    CreeLe = DateTime.UtcNow
                };

                _context.Questions.Add(question);

                // ✅ FIX: Créer la liaison avec le questionnaire si on nous l'envoie !
                if (dto.QuestionnaireId.HasValue && dto.QuestionnaireId != Guid.Empty)
                {
                    // Calculer le prochain ordre de manière compatible avec EF Core
                    var maxOrder = await _context.QuestionnaireQuestions
                        .Where(qq => qq.QuestionnaireId == dto.QuestionnaireId.Value)
                        .Select(qq => (int?)qq.Ordre)
                        .MaxAsync();
                        
                    var nextOrder = (maxOrder ?? 0) + 1;

                    _context.QuestionnaireQuestions.Add(new QuestionnaireQuestion {
                        QuestionnaireId = dto.QuestionnaireId.Value,
                        QuestionId = question.Id,
                        Ordre = nextOrder,
                        Ponderation = dto.Points
                    });
                }

                await _context.SaveChangesAsync();
                return Ok(question);
            } catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        [HttpPut("{id}")]
        [RequirePermission("edit_bank")]
        public async Task<IActionResult> PutQuestion(Guid id, [FromBody] NeoEvaluation.API.DTOs.QuestionCreateDto dto)
        {
            var q = await _context.Questions.FindAsync(id);
            if (q == null) return NotFound();

            Console.WriteLine($"[DEBUG PUT] ID: {id} | Choix Count: {dto.Choix?.Count ?? 0}");

            q.Enonce = dto.Enonce;
            q.Type = dto.Type;
            q.Niveau = dto.Niveau;
            q.Points = dto.Points;
            q.DureeSecondes = dto.DureeSecondes ?? 60;
            q.Theme = dto.Theme;
            q.SousTheme = dto.SousTheme;
            q.Choix = dto.Choix ?? new List<string>();
            q.BonneReponse = dto.BonneReponse;

            await _context.SaveChangesAsync();
            return Ok(q);
        }

        [HttpDelete("{id}")]
        [RequirePermission("edit_bank")]
        public async Task<IActionResult> DeleteQuestion(Guid id)
        {
            var q = await _context.Questions
                .IgnoreQueryFilters()
                .Include(x => x.QuestionnaireQuestions)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (q == null) return NotFound();

            // 1. Supprimer les liaisons dans la table de jointure
            if (q.QuestionnaireQuestions.Any())
            {
                _context.QuestionnaireQuestions.RemoveRange(q.QuestionnaireQuestions);
            }

            // 2. Supprimer les réponses des candidats (très important pour ne pas bloquer)
            var reponses = await _context.Reponses.IgnoreQueryFilters().Where(r => r.QuestionId == id).ToListAsync();
            if (reponses.Any())
            {
                _context.Reponses.RemoveRange(reponses);
            }

            // 3. Supprimer la question elle-même
            _context.Questions.Remove(q);

            await _context.SaveChangesAsync();
            return Ok(new { message = "Question et ses liaisons/réponses supprimées avec succès." });
        }

        // --- GESTION DES CATÉGORIES ---

        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories()
        {
            var themes = await _context.Questions
                .Where(q => q.Theme != null && q.Theme != "")
                .Select(q => q.Theme)
                .Distinct()
                .ToListAsync();
                
            return Ok(themes);
        }

        [HttpPost("categories/rename")]
        [RequirePermission("edit_bank")]
        public async Task<IActionResult> RenameCategory([FromBody] NeoEvaluation.API.DTOs.CategoryRenameDto dto)
        {
            var questions = await _context.Questions
                .Where(q => q.Theme == dto.OldName)
                .ToListAsync();

            foreach (var q in questions)
            {
                q.Theme = dto.NewName;
            }

            await _context.SaveChangesAsync();
            return Ok(new { Message = $"Renommage effectué sur {questions.Count} questions.", NewName = dto.NewName });
        }

        [HttpDelete("categories/{name}")]
        [RequirePermission("edit_bank")]
        public async Task<IActionResult> DeleteCategory(string name)
        {
            var questions = await _context.Questions
                .Where(q => q.Theme == name)
                .ToListAsync();

            foreach (var q in questions)
            {
                q.Theme = null;
            }

            await _context.SaveChangesAsync();
            return Ok(new { Message = $"Catégorie retirée de {questions.Count} questions." });
        }
    }
}