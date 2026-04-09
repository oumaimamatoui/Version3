using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NeoEvaluation.API.Data;
using NeoEvaluation.API.Models;
using System.Text.Json;

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

        // GET: api/Questions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetQuestions()
        {
            var questions = await _context.Questions
                .Include(q => q.Questionnaire)
                .ToListAsync();

            // Transformation pour correspondre au format attendu par le Front-end (Vue.js)
            var result = questions.Select(q => new
            {
                q.Id,
                q.Texte,
                q.Type,
                q.Poids,
                // On mappe le Titre du questionnaire vers 'categorie'
                Categorie = q.Questionnaire?.Titre ?? "Général",
                // On désérialise les réponses stockées en JSON dans BonneReponse
                Reponses = string.IsNullOrEmpty(q.BonneReponse) 
                           ? new List<object>() 
                           : TryDeserialize(q.BonneReponse)
            });

            return Ok(result);
        }

        // GET: api/Questions/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetQuestion(Guid id)
        {
            var q = await _context.Questions
                .Include(q => q.Questionnaire)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (q == null) return NotFound();

            return Ok(new
            {
                q.Id,
                q.Texte,
                q.Type,
                q.Poids,
                Categorie = q.Questionnaire?.Titre ?? "Général",
                Reponses = TryDeserialize(q.BonneReponse)
            });
        }

        // POST: api/Questions
        [HttpPost]
        public async Task<ActionResult<Question>> CreateQuestion([FromBody] dynamic data)
        {
            try 
            {
                // Extraction des données du JSON dynamique envoyé par Vue
                string texte = data.GetProperty("texte").GetString();
                int type = data.GetProperty("type").GetInt32();
                double poids = data.GetProperty("poids").GetDouble();
                string categoryName = data.GetProperty("categorie").GetString();
                
                // On sérialise le tableau de réponses en JSON pour le stocker dans BonneReponse
                string reponsesJson = data.GetProperty("reponses").ToString();

                // 1. Gérer la catégorie (Questionnaire)
                var questionnaire = await _context.Questionnaires
                    .FirstOrDefaultAsync(q => q.Titre == categoryName);

                if (questionnaire == null)
                {
                    questionnaire = new Questionnaire { Id = Guid.NewGuid(), Titre = categoryName, Description = "Auto-généré via Question Bank" };
                    _context.Questionnaires.Add(questionnaire);
                    await _context.SaveChangesAsync();
                }

                // 2. Créer la question
                var question = new Question
                {
                    Id = Guid.NewGuid(),
                    Texte = texte,
                    Type = type,
                    Poids = poids,
                    QuestionnaireId = questionnaire.Id,
                    BonneReponse = reponsesJson // Stockage format JSON
                };

                _context.Questions.Add(question);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetQuestion), new { id = question.Id }, question);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Erreur de formatage", details = ex.Message });
            }
        }

        // PUT: api/Questions/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateQuestion(Guid id, [FromBody] dynamic data)
        {
            var question = await _context.Questions.FindAsync(id);
            if (question == null) return NotFound();

            try
            {
                string categoryName = data.GetProperty("categorie").GetString();
                
                // Mise à jour de la catégorie
                var questionnaire = await _context.Questionnaires.FirstOrDefaultAsync(q => q.Titre == categoryName);
                if (questionnaire == null)
                {
                    questionnaire = new Questionnaire { Id = Guid.NewGuid(), Titre = categoryName };
                    _context.Questionnaires.Add(questionnaire);
                    await _context.SaveChangesAsync();
                }

                question.Texte = data.GetProperty("texte").GetString();
                question.Type = data.GetProperty("type").GetInt32();
                question.Poids = data.GetProperty("poids").GetDouble();
                question.QuestionnaireId = questionnaire.Id;
                question.BonneReponse = data.GetProperty("reponses").ToString();

                _context.Entry(question).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                
                return Ok(new { message = "Actif mis à jour" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/Questions/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuestion(Guid id)
        {
            var question = await _context.Questions.FindAsync(id);
            if (question == null) return NotFound();

            _context.Questions.Remove(question);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Supprimé" });
        }

        // Helper pour éviter les erreurs de désérialisation
        private object TryDeserialize(string json)
        {
            try {
                return JsonSerializer.Deserialize<object>(json);
            } catch {
                return new List<object>();
            }
        }
    }
}