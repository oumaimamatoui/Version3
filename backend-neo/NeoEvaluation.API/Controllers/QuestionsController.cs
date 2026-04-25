using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using NeoEvaluation.API.Data;
using NeoEvaluation.API.Models;
using NeoEvaluation.API.Services;

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
                
            return Ok(questions.GroupBy(q => q.Enonce).Select(g => g.First()).ToList());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetQuestion(Guid id)
        {
            var q = await _context.Questions.FindAsync(id);
            return q == null ? NotFound() : Ok(q);
        }

        [HttpPost]
        public async Task<IActionResult> PostQuestion([FromBody] NeoEvaluation.API.DTOs.QuestionCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                var details = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest(new { Message = "Données invalides", Errors = details });
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Création de la Question
                Console.WriteLine($"[DEBUG CREATE] Enonce: {dto.Enonce}");
                Console.WriteLine($"[DEBUG CREATE] Options received in DTO: {dto.Choix?.Count ?? 0}");

                var question = new Question
                {
                    Id = Guid.NewGuid(),
                    EntrepriseId = _tenantService.GetTenantId(),
                    Enonce = dto.Enonce,
                    Type = dto.Type,
                    Niveau = dto.Niveau,
                    Points = dto.Points,
                    DureeSecondes = dto.DureeSecondes,
                    Theme = dto.Theme,
                    SousTheme = dto.SousTheme,
                    Choix = new List<string>(dto.Choix ?? new List<string>()),
                    BonneReponse = dto.BonneReponse ?? string.Empty,
                    Prerequis = dto.Prerequis ?? new List<string>()
                };

                Console.WriteLine($"[DEBUG CREATE] Question object Choix count: {question.Choix.Count}");

                _context.Questions.Add(question);

                // 2. Liaison avec le Questionnaire via la table de jointure (si renseigné)
                if (dto.QuestionnaireId.HasValue && dto.QuestionnaireId.Value != Guid.Empty)
                {
                    // Vérifier si le questionnaire existe
                    var questionnaireExiste = await _context.Questionnaires.AnyAsync(q => q.Id == dto.QuestionnaireId.Value);
                    if (!questionnaireExiste)
                    {
                        return NotFound(new { message = "Le questionnaire spécifié n'existe pas." });
                    }

                    var liaison = new QuestionnaireQuestion
                    {
                        QuestionnaireId = dto.QuestionnaireId.Value,
                        QuestionId = question.Id,
                        Ordre = dto.Ordre,
                        Ponderation = dto.Ponderation,
                        EstObligatoire = dto.EstObligatoire
                    };
                    
                    _context.QuestionnaireQuestions.Add(liaison);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return CreatedAtAction(nameof(GetQuestion), new { id = question.Id }, question);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { message = "Erreur lors de la création de la question", details = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutQuestion(Guid id, [FromBody] NeoEvaluation.API.DTOs.QuestionCreateDto dto)
        {
            var q = await _context.Questions.FindAsync(id);
            if (q == null) return NotFound();

            Console.WriteLine($"[DEBUG] Updating Question {id}. Options received: {dto.Choix?.Count ?? 0}");
            if (dto.Choix != null) {
                foreach(var c in dto.Choix) Console.WriteLine($" - Option: {c}");
            }

            q.Enonce = dto.Enonce;
            q.Type = dto.Type;
            q.Niveau = dto.Niveau;
            q.Points = dto.Points;
            q.DureeSecondes = dto.DureeSecondes;
            q.Theme = dto.Theme;
            q.SousTheme = dto.SousTheme;
            q.Choix = new List<string>(dto.Choix ?? new List<string>());
            q.BonneReponse = dto.BonneReponse;

            await _context.SaveChangesAsync();
            return Ok(q);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuestion(Guid id)
        {
            var q = await _context.Questions
                .Include(x => x.QuestionnaireQuestions)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (q == null) return NotFound();

            // 1. Supprimer d'abord les liaisons dans la table de jointure
            if (q.QuestionnaireQuestions.Any())
            {
                _context.QuestionnaireQuestions.RemoveRange(q.QuestionnaireQuestions);
            }

            // 2. Supprimer la question elle-même
            _context.Questions.Remove(q);

            await _context.SaveChangesAsync();
            return Ok(new { message = "Question et ses liaisons supprimées avec succès." });
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