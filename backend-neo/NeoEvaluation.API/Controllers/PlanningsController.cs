using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NeoEvaluation.API.Data;
using NeoEvaluation.API.Models;
using NeoEvaluation.API.DTOs;

namespace NeoEvaluation.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlanningsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PlanningsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Programmer([FromBody] PlanningDto dto)
        {
            if (dto == null) return BadRequest(new { message = "Le corps de la requête est vide." });

            // 1. Vérification d'existence
            var campagneExistante = await _context.Campagnes.AnyAsync(c => c.Id == dto.CampagneId);
            if (!campagneExistante) return NotFound(new { message = "Campagne non trouvée." });

            // 2. Gestion de la date UTC (Crucial pour Postgres)
            DateTime startAt = DateTime.SpecifyKind(dto.DateOuverture, DateTimeKind.Utc);

            // 3. Logique Upsert
            var planning = await _context.Plannings
                .FirstOrDefaultAsync(p => p.CampagneId == dto.CampagneId);

            if (planning != null)
            {
                planning.DateOuverture = startAt;
                planning.DureeMinutes = dto.DureeMinutes;
                planning.ModeNotation = dto.ModeNotation;
                _context.Plannings.Update(planning);
            }
            else
            {
                planning = new Planning
                {
                    Id = Guid.NewGuid(),
                    CampagneId = dto.CampagneId,
                    DateOuverture = startAt,
                    DureeMinutes = dto.DureeMinutes,
                    ModeNotation = dto.ModeNotation
                };
                _context.Plannings.Add(planning);
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Planning mis à jour.", data = planning });
        }

        [HttpGet("calendrier")]
        public async Task<IActionResult> GetCalendrier()
        {
            // Erreur 500 résolue ici : On récupère les données simples, 
            // et on calcule les dates de fin côté Serveur (Memory) et non BDD.
            var results = await _context.Plannings
                .Include(p => p.Campagne)
                .OrderBy(p => p.DateOuverture)
                .ToListAsync();

            var data = results.Select(p => new
            {
                id = p.Id,
                titre = p.Campagne?.Nom ?? "Inconnu",
                debut = p.DateOuverture,
                // Le calcul de la fin se fait ici après le ToListAsync()
                fin = p.DateOuverture.AddMinutes(p.DureeMinutes), 
                mode = p.ModeNotation
            });

            return Ok(data);
        }
    }
}