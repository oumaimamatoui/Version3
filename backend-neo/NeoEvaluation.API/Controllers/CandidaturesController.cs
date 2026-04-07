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
                Statut = EvaluationStatus.NON_COMMENCE,
                Score = 0 
            };

            _context.Candidatures.Add(candidature);
            await _context.SaveChangesAsync();
            return Ok(new { Message = "Candidature enregistrée avec succès.", Id = candidature.Id });
        }
    }
}