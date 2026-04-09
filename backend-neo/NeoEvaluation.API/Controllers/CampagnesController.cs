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
        // Utilisé pour les compteurs KPI du dashboard
        [HttpGet("stats")]
        public async Task<ActionResult> GetStats()
        {
            try 
            {
                var total = await _context.Campagnes.CountAsync();
                var actives = await _context.Campagnes.CountAsync(c => c.Statut == 1);
                var totalCandidats = await _context.Candidatures.CountAsync();
                
                // On peut aussi calculer la santé moyenne ou d'autres KPIs ici
                return Ok(new
                {
                    totalCampaigns = total,
                    activeCampaigns = actives,
                    totalCandidates = totalCandidats,
                    averageHealth = 94 // Valeur simulée comme dans le frontend
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de la récupération des statistiques : {ex.Message}");
            }
        }

        // GET: api/Campagnes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Campagne>>> GetCampagnes()
        {
            // On inclut le Questionnaire pour afficher le nom de la structure dans la liste
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

            if (campagne == null) return NotFound("Campagne introuvable.");
            return campagne;
        }

        // POST: api/Campagnes
        [HttpPost]
        public async Task<ActionResult<Campagne>> PostCampagne([FromBody] Campagne campagne)
        {
            if (campagne == null) return BadRequest("Les données de la campagne sont invalides.");

            // 1. Initialisation de l'ID si absent
            if (campagne.Id == null || campagne.Id == Guid.Empty)
                campagne.Id = Guid.NewGuid();

            // 2. Gestion des dates pour PostgreSQL (Obligatoire : UTC)
            campagne.DateDebut = DateTime.SpecifyKind(campagne.DateDebut, DateTimeKind.Utc);
            campagne.DateFin = DateTime.SpecifyKind(campagne.DateFin, DateTimeKind.Utc);

            // 3. Sécurité EntrepriseId 
            // Si le frontend n'envoie pas d'ID, on récupère la première entreprise par défaut
            if (campagne.EntrepriseId == Guid.Empty)
            {
                var defaultEnt = await _context.Entreprises.Select(e => e.Id).FirstOrDefaultAsync();
                if (defaultEnt == Guid.Empty)
                    return BadRequest("Impossible de créer une campagne sans entreprise valide en base.");
                
                campagne.EntrepriseId = defaultEnt;
            }

            // 4. Protection contre la double insertion
            // On s'assure que EF Core ne tente pas de créer un nouveau Questionnaire
            // s'il est déjà présent dans l'objet envoyé.
            campagne.Questionnaire = null; 

            try
            {
                _context.Campagnes.Add(campagne);
                await _context.SaveChangesAsync();
                
                // On recharge la campagne avec son questionnaire pour le retour au frontend
                var result = await _context.Campagnes
                    .Include(c => c.Questionnaire)
                    .FirstOrDefaultAsync(x => x.Id == campagne.Id);

                return CreatedAtAction(nameof(GetCampagne), new { id = campagne.Id }, result);
            }
            catch (Exception ex)
            {
                var errorMsg = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(500, $"Erreur lors de la création : {errorMsg}");
            }
        }

        // PUT: api/Campagnes/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCampagne(Guid id, [FromBody] Campagne campagne)
        {
            if (id != campagne.Id) return BadRequest("L'ID de l'URL ne correspond pas à l'ID du corps de la requête.");

            var campagneExistante = await _context.Campagnes.FindAsync(id);
            if (campagneExistante == null) return NotFound("Campagne introuvable.");

            // Mise à jour des champs
            campagneExistante.Nom = campagne.Nom;
            campagneExistante.Description = campagne.Description;
            campagneExistante.Categorie = campagne.Categorie;
            campagneExistante.QuestionnaireId = campagne.QuestionnaireId;
            campagneExistante.DureeMinutes = campagne.DureeMinutes;
            campagneExistante.ScorePassage = campagne.ScorePassage;
            campagneExistante.MaxCandidats = campagne.MaxCandidats;
            campagneExistante.Statut = campagne.Statut;

            // Mise à jour des dates en UTC
            campagneExistante.DateDebut = DateTime.SpecifyKind(campagne.DateDebut, DateTimeKind.Utc);
            campagneExistante.DateFin = DateTime.SpecifyKind(campagne.DateFin, DateTimeKind.Utc);

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CampagneExists(id)) return NotFound();
                else throw;
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de la modification : {ex.Message}");
            }
        }

        // DELETE: api/Campagnes/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCampagne(Guid id)
        {
            var campagne = await _context.Campagnes.FindAsync(id);
            if (campagne == null) return NotFound();

            // Vérification si des candidats sont déjà inscrits
            var countCandidats = await _context.Candidatures.CountAsync(c => c.CampagneId == id);
            if (countCandidats > 0)
            {
                return BadRequest($"Suppression impossible : {countCandidats} candidat(s) sont liés à cette campagne.");
            }

            try
            {
                _context.Campagnes.Remove(campagne);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de la suppression : {ex.Message}");
            }
        }

        private bool CampagneExists(Guid id)
        {
            return _context.Campagnes.Any(e => e.Id == id);
        }
    }
}