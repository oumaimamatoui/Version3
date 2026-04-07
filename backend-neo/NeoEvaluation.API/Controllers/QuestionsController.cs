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

        public QuestionsController(AppDbContext context)
        {
            _context = context;
        }

        // READ: Liste toutes les questions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Question>>> GetQuestions()
        {
            return await _context.Questions.ToListAsync();
        }

        // READ: Une seule question par ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Question>> GetQuestion(Guid id)
        {
            var question = await _context.Questions.FindAsync(id);
            if (question == null) return NotFound(new { message = "Question introuvable" });
            return question;
        }

        // CREATE: Ajouter une nouvelle question
        [HttpPost]
        public async Task<ActionResult<Question>> CreateQuestion(Question question)
        {
            // Sécurité si l'ID envoyé est vide
            if (question.Id == Guid.Empty)
            {
                question.Id = Guid.NewGuid();
            }

            // Vérifier si le questionnaire parent existe
            var questionnaireExists = await _context.Questionnaires.AnyAsync(q => q.Id == question.QuestionnaireId);
            if (!questionnaireExists) 
                return BadRequest(new { message = "Le questionnaire associé n'existe pas." });

            _context.Questions.Add(question);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetQuestion), new { id = question.Id }, question);
        }

        // UPDATE: Modifier une question
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateQuestion(Guid id, Question question)
        {
            if (id != question.Id) return BadRequest(new { message = "ID non correspondant" });

            _context.Entry(question).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Questions.AnyAsync(e => e.Id == id))
                    return NotFound(new { message = "Question introuvable pour mise à jour" });
                else throw;
            }

            return Ok(new { message = "Question mise à jour avec succès", data = question });
        }

        // DELETE: Supprimer une question
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuestion(Guid id)
        {
            var question = await _context.Questions.FindAsync(id);
            if (question == null) return NotFound(new { message = "Question introuvable" });

            _context.Questions.Remove(question);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Question supprimée du terminal" });
        }
    }
}