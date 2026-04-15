using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NeoEvaluation.API.Data;
using NeoEvaluation.API.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using NeoEvaluation.API.Services;

namespace NeoEvaluation.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class QuestionnairesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ITenantService _tenantService;

        public QuestionnairesController(AppDbContext context, ITenantService tenantService) 
        { 
            _context = context; 
            _tenantService = tenantService;
        }

        // 1. Lister tous les questionnaires
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Questionnaire>>> Get() 
        {
            return await _context.Questionnaires.OrderByDescending(q => q.CreeLe).ToListAsync();
        }

        // 2. Créer un questionnaire
        [HttpPost]
        public async Task<ActionResult<Questionnaire>> Post([FromBody] QuestionnaireCreateDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Titre))
            {
                return BadRequest(new { message = "Le titre du questionnaire est obligatoire." });
            }

            try 
            {
                var questionnaire = new Questionnaire
                {
                    Id = Guid.NewGuid(),
                    EntrepriseId = _tenantService.GetTenantId(),
                    Titre = dto.Titre,
                    Description = dto.Description ?? "",
                    DureeMinutes = dto.DureeMinutes,
                    AntitricheActif = dto.AntitricheActif,
                    RandomiserQuestions = dto.RandomiserQuestions,
                    EstPublie = dto.EstPublie,
                    CreeLe = DateTime.UtcNow
                };

                _context.Questionnaires.Add(questionnaire);
                await _context.SaveChangesAsync();

                return Ok(questionnaire);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Erreur interne", details = ex.Message });
            }
        }

        // 3. Supprimer un questionnaire
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var q = await _context.Questionnaires.FindAsync(id);
            if (q == null) return NotFound();

            // Supprimer les liens M2M (QuestionnaireQuestion) avant de supprimer le questionnaire
            var linkedQQ = _context.QuestionnaireQuestions.Where(x => x.QuestionnaireId == id);
            _context.QuestionnaireQuestions.RemoveRange(linkedQQ);

            // Supprimer les liens M2M (CampagneQuestionnaire)
            var linkedCQ = _context.CampagneQuestionnaires.Where(x => x.QuestionnaireId == id);
            _context.CampagneQuestionnaires.RemoveRange(linkedCQ);

            _context.Questionnaires.Remove(q);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Supprimé avec succès" });
        }
    }

    // DTO pour la création
    public class QuestionnaireCreateDto {
        public string Titre { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int DureeMinutes { get; set; } = 60;
        public bool AntitricheActif { get; set; }
        public bool RandomiserQuestions { get; set; }
        public bool EstPublie { get; set; } = true;
    }
}