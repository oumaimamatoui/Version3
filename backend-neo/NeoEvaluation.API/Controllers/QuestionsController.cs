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

        // ==========================================
        // 1. RÉCUPÉRER LES QUESTIONS (FILTRÉES PAR ENTREPRISE)
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> GetQuestions()
        {
            try 
            {
                var entId = _tenantService.GetTenantId();
                
                // LOG DE DEBUG : Vérifiez votre terminal backend
                Console.WriteLine($"[DEBUG BANQUE] Accès par : {User.Identity?.Name} | Entreprise : {entId}");

                if (!entId.HasValue || entId == Guid.Empty) 
                {
                    return Unauthorized(new { message = "ID d'entreprise manquant dans votre session." });
                }

                // Récupération filtrée
                var questions = await _context.Questions
                    .Where(q => q.EntrepriseId == entId.Value)
                    .OrderByDescending(q => q.CreeLe)
                    .ToListAsync();
                
                // Logique pour éviter les doublons d'énoncés (optionnel selon votre besoin)
                var result = questions
                    .GroupBy(q => q.Enonce)
                    .Select(g => g.OrderByDescending(q => q.Choix?.Count ?? 0).ThenByDescending(q => q.CreeLe).First())
                    .ToList();
                
                return Ok(result);
            }
            catch (Exception ex) 
            {
                return StatusCode(500, new { message = "Erreur serveur", detail = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetQuestion(Guid id)
        {
            var entId = _tenantService.GetTenantId();
            var q = await _context.Questions
                .FirstOrDefaultAsync(x => x.Id == id && x.EntrepriseId == entId);
            
            return q == null ? NotFound("Question non trouvée ou accès refusé.") : Ok(q);
        }

        // ==========================================
        // 2. CRÉATION (FORCE L'ENTREPRISE DE L'UTILISATEUR)
        // ==========================================
        [HttpPost]
        [RequirePermission("edit_bank")]
        public async Task<IActionResult> PostQuestion([FromBody] NeoEvaluation.API.DTOs.QuestionCreateDto dto)
        {
            try {
                var entId = _tenantService.GetTenantId();
                if (!entId.HasValue) return Unauthorized();

                var question = new Question {
                    Id = Guid.NewGuid(),
                    Enonce = dto.Enonce,
                    Type = dto.Type,
                    Niveau = dto.Niveau,
                    Points = dto.Points,
                    DureeSecondes = dto.DureeSecondes ?? 60,
                    Theme = dto.Theme,
                    SousTheme = dto.SousTheme,
                    EntrepriseId = entId.Value, // <--- On force l'ID ici
                    Choix = dto.Choix ?? new List<string>(),
                    BonneReponse = dto.BonneReponse ?? string.Empty,
                    CreeLe = DateTime.UtcNow
                };

                _context.Questions.Add(question);

                // Liaison questionnaire si nécessaire
                if (dto.QuestionnaireId.HasValue && dto.QuestionnaireId != Guid.Empty)
                {
                    var maxOrder = await _context.QuestionnaireQuestions
                        .Where(qq => qq.QuestionnaireId == dto.QuestionnaireId.Value)
                        .Select(qq => (int?)qq.Ordre).MaxAsync();
                        
                    _context.QuestionnaireQuestions.Add(new QuestionnaireQuestion {
                        QuestionnaireId = dto.QuestionnaireId.Value,
                        QuestionId = question.Id,
                        Ordre = (maxOrder ?? 0) + 1,
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
            var entId = _tenantService.GetTenantId();
            var q = await _context.Questions.FirstOrDefaultAsync(x => x.Id == id && x.EntrepriseId == entId);
            
            if (q == null) return NotFound("Question non trouvée.");

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
            var entId = _tenantService.GetTenantId();
            var q = await _context.Questions
                .Include(x => x.QuestionnaireQuestions)
                .FirstOrDefaultAsync(x => x.Id == id && x.EntrepriseId == entId);

            if (q == null) return NotFound();

            // Nettoyage des liaisons
            _context.QuestionnaireQuestions.RemoveRange(q.QuestionnaireQuestions);
            
            var reponses = await _context.Reponses.Where(r => r.QuestionId == id).ToListAsync();
            _context.Reponses.RemoveRange(reponses);

            _context.Questions.Remove(q);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Supprimé." });
        }

        // ==========================================
        // 3. CATÉGORIES (FILTRÉES)
        // ==========================================
        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories()
        {
            var entId = _tenantService.GetTenantId();
            if (!entId.HasValue) return Unauthorized();

            var themes = await _context.Questions
                .Where(q => q.EntrepriseId == entId.Value && !string.IsNullOrEmpty(q.Theme))
                .Select(q => q.Theme)
                .Distinct()
                .ToListAsync();
                
            return Ok(themes);
        }

        // ==========================================
        // 4. SEED DEMO (POUR TESTER SI ÇA APPARAIT)
        // ==========================================
        [HttpPost("seed-demo")]
        public async Task<IActionResult> SeedDemo()
        {
            var entId = _tenantService.GetTenantId();
            if (!entId.HasValue) return Unauthorized();

            var q = new Question {
                Id = Guid.NewGuid(),
                Enonce = "Question Test pour mon Entreprise",
                Type = TypeQuestion.QCU,
                Points = 5,
                EntrepriseId = entId.Value, // On s'assure qu'elle appartient à l'utilisateur
                Theme = "General",
                Choix = new List<string> { "Choix 1", "Choix 2" },
                BonneReponse = "Choix 1",
                CreeLe = DateTime.UtcNow
            };

            _context.Questions.Add(q);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Question de test créée !", id = q.Id });
        }
    }
}