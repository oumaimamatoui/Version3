using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NeoEvaluation.API.Data;
using NeoEvaluation.API.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeoEvaluation.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionnairesController : ControllerBase
    {
        private readonly AppDbContext _context;
        public QuestionnairesController(AppDbContext context) { _context = context; }

        // 1. جلب الكل
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Questionnaire>>> Get() 
        {
            return await _context.Questionnaires.OrderByDescending(q => q.CreeLe).ToListAsync();
        }

        // 2. إنشاء استبيان جديد (حل مشكلة الـ 400)
        [HttpPost]
        public async Task<ActionResult<Questionnaire>> Post([FromBody] QuestionnaireCreateDto dto)
        {
            // التحقق من البيانات لمنع طلبات فارغة
            if (dto == null || string.IsNullOrWhiteSpace(dto.Titre))
            {
                return BadRequest(new { message = "Le titre du questionnaire est obligatoire." });
            }

            try 
            {
                var questionnaire = new Questionnaire
                {
                    Id = Guid.NewGuid(), // نولد الـ ID هنا في السيرفر
                    Titre = dto.Titre,
                    Description = dto.Description ?? "",
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

        // 3. حذف استبيان
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var q = await _context.Questionnaires.FindAsync(id);
            if (q == null) return NotFound();

            // حذف الأسئلة المرتبطة به أولاً لتجنب مشاكل الـ Foreign Key
            var linkedQuestions = _context.Questions.Where(x => x.QuestionnaireId == id);
            _context.Questions.RemoveRange(linkedQuestions);

            _context.Questionnaires.Remove(q);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Supprimé avec succès" });
        }
    }

    // الـ DTO المطلوب لاستقبال البيانات من Vue
    public class QuestionnaireCreateDto {
        public string Titre { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}