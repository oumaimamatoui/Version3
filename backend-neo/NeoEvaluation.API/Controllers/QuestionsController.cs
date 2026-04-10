using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NeoEvaluation.API.Data;
using NeoEvaluation.API.Models;

namespace NeoEvaluation.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionsController : ControllerBase
    {
        private readonly AppDbContext _context;
        public QuestionsController(AppDbContext context) { _context = context; }

        [HttpGet]
        public async Task<IActionResult> GetQuestions()
        {
            return Ok(await _context.Questions.ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetQuestion(Guid id)
        {
            var q = await _context.Questions.FindAsync(id);
            return q == null ? NotFound() : Ok(q);
        }

        [HttpPost]
        public async Task<IActionResult> PostQuestion([FromBody] Question question)
        {
            // SI ERREUR 400 : Ce bloc renverra la liste exacte des champs qui posent problème
            if (!ModelState.IsValid)
            {
                var details = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest(new { Message = "Données invalides", Errors = details });
            }

            if (question.Id == Guid.Empty) question.Id = Guid.NewGuid();

            _context.Questions.Add(question);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetQuestion), new { id = question.Id }, question);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuestion(Guid id)
        {
            var q = await _context.Questions.FindAsync(id);
            if (q == null) return NotFound();
            _context.Questions.Remove(q);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}