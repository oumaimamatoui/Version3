using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NeoEvaluation.API.Data;
using NeoEvaluation.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeoEvaluation.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CampagnesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CampagnesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Campagnes/stats
        [HttpGet("stats")]
        public async Task<ActionResult> GetStats()
        {
            var total = await _context.Campagnes.CountAsync();
            var active = await _context.Campagnes.CountAsync(c => c.Statut == 1);
            var questionnaires = await _context.Questionnaires.CountAsync();
            var capacite = await _context.Campagnes.SumAsync(c => (int?)c.MaxCandidats) ?? 0;

            return Ok(new
            {
                totalCampaigns = total,
                activeCampaigns = active,
                totalTests = questionnaires,
                totalCapacity = capacite
            });
        }

        // GET: api/Campagnes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Campagne>>> GetCampagnes()
        {
            return await _context.Campagnes
                .Include(c => c.Questionnaire)
                .OrderByDescending(c => c.DateDebut)
                .ToListAsync();
        }

        // GET: api/Campagnes/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Campagne>> GetCampagne(Guid id)
        {
            var campagne = await _context.Campagnes
                .Include(c => c.Questionnaire)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (campagne == null) return NotFound();
            return campagne;
        }

        // POST: api/Campagnes
        [HttpPost]
        public async Task<ActionResult<Campagne>> PostCampagne([FromBody] Campagne campagne)
        {
            if (campagne == null) return BadRequest("Données invalides.");

            // 1. Initialisation de l'ID
            campagne.Id = Guid.NewGuid();

            // 2. CORRECTION POSTGRESQL (CRITIQUE) : Forcer les dates en UTC
            // PostgreSQL refuse les dates "Unspecified" envoyées par le frontend
            campagne.DateDebut = DateTime.SpecifyKind(campagne.DateDebut, DateTimeKind.Utc);
            campagne.DateFin = DateTime.SpecifyKind(campagne.DateFin, DateTimeKind.Utc);

            // 3. SÉCURITÉ ENTREPRISE : Récupérer une entreprise réelle si l'ID est vide
            if (campagne.EntrepriseId == Guid.Empty)
            {
                var premiereEntreprise = await _context.Entreprises.Select(e => e.Id).FirstOrDefaultAsync();
                if (premiereEntreprise == Guid.Empty)
                    return BadRequest("Erreur : Aucune entreprise n'existe dans la base de données.");
                
                campagne.EntrepriseId = premiereEntreprise;
            }

            // 4. Nettoyage de l'objet Questionnaire pour éviter les conflits d'insertion
            campagne.Questionnaire = null;

            try
            {
                _context.Campagnes.Add(campagne);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetCampagne), new { id = campagne.Id }, campagne);
            }
            catch (Exception ex)
            {
                // Capture le message d'erreur précis pour le débogage
                var message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return StatusCode(500, $"Erreur base de données : {message}");
            }
        }

        // PUT: api/Campagnes/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCampagne(Guid id, [FromBody] Campagne campagne)
        {
            if (id != campagne.Id) return BadRequest("L'ID ne correspond pas.");

            var existing = await _context.Campagnes.FindAsync(id);
            if (existing == null) return NotFound();

            // Mise à jour des propriétés
            existing.Nom = campagne.Nom;
            existing.Description = campagne.Description;
            existing.QuestionnaireId = campagne.QuestionnaireId;
            existing.DureeMinutes = campagne.DureeMinutes;
            existing.ScorePassage = campagne.ScorePassage;
            existing.MaxCandidats = campagne.MaxCandidats;
            existing.Statut = campagne.Statut;

            // CORRECTION POSTGRESQL : Toujours forcer UTC lors de la modification
            existing.DateDebut = DateTime.SpecifyKind(campagne.DateDebut, DateTimeKind.Utc);
            existing.DateFin = DateTime.SpecifyKind(campagne.DateFin, DateTimeKind.Utc);

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                var message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return StatusCode(500, $"Erreur lors de la mise à jour : {message}");
            }
        }

        // DELETE: api/Campagnes/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCampagne(Guid id)
        {
            var campagne = await _context.Campagnes.FindAsync(id);
            if (campagne == null) return NotFound();

            // Sécurité : Vérifier s'il y a des candidatures liées
            var hasCandidatures = await _context.Candidatures.AnyAsync(c => c.CampagneId == id);
            if (hasCandidatures)
                return BadRequest("Action refusée : cette campagne possède des candidats.");

            _context.Campagnes.Remove(campagne);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CampagneExists(Guid id)
        {
            return _context.Campagnes.Any(e => e.Id == id);
        }
    }
}