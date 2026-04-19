using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NeoEvaluation.API.Data;
using NeoEvaluation.API.Models;

namespace NeoEvaluation.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CandidaturesController : ControllerBase
    {
        private readonly AppDbContext _context;
        public CandidaturesController(AppDbContext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Candidature>>> Lister() 
            => await _context.Candidatures
                .Include(c => c.Candidat)
                .Include(c => c.Campagne)
                .ToListAsync();

        [HttpGet("mes-tests")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> GetMesTests()
        {
            var userIdClaim = User.FindFirst("id")?.Value ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
                return Unauthorized();

            var mesCandidatures = await _context.Candidatures
                .Include(c => c.Campagne)
                .Include(c => c.Evaluation)
                .Where(c => c.CandidatId == userId)
                .Select(c => new {
                    candidatureId = c.Id,
                    campagneNom = c.Campagne.Nom,
                    statut = c.Evaluation != null ? c.Evaluation.Statut.ToString() : c.Statut.ToString(),
                    datePostule = c.PostuleLe,
                    scoreTotal = c.Evaluation != null ? c.Evaluation.ScoreTotal : 0
                })
                .ToListAsync();

            return Ok(mesCandidatures);
        }

        [HttpPost("postuler")]
        public async Task<IActionResult> Postuler([FromBody] Candidature candidature)
        {
            candidature.Id = Guid.NewGuid();
            candidature.PostuleLe = DateTime.UtcNow;
            candidature.Statut = ApplicationStatus.POSTULE;

            // Création automatique de l'évaluation liée (Diagramme 1:1)
            candidature.Evaluation = new Evaluation 
            { 
                Id = Guid.NewGuid(), 
                Statut = StatutPassage.NON_COMMENCE,
                ScoreTotal = 0 
            };

            _context.Candidatures.Add(candidature);
            await _context.SaveChangesAsync();
            return Ok(new { Message = "Candidature enregistrée avec succès.", Id = candidature.Id });
        }
    }
}