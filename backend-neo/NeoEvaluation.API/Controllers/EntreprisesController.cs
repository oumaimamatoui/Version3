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
    public class EntreprisesController : ControllerBase
    {
        private readonly AppDbContext _context;
        public EntreprisesController(AppDbContext context) => _context = context;

        [HttpGet] // Lire tout
        public async Task<ActionResult<IEnumerable<Entreprise>>> Lister() 
            => await _context.Entreprises.ToListAsync();

        [HttpGet("{id}")] // Lire un seul
        public async Task<ActionResult<Entreprise>> Detail(Guid id)
        {
            var ent = await _context.Entreprises.FindAsync(id);
            return ent == null ? NotFound() : ent;
        }

        [HttpPost] // Créer
        public async Task<ActionResult<Entreprise>> Creer(Entreprise entreprise)
        {
            entreprise.Id = Guid.NewGuid();
            _context.Entreprises.Add(entreprise);
            await _context.SaveChangesAsync();
            return Ok(entreprise);
        }

        [HttpDelete("{id}")] // Supprimer
        public async Task<IActionResult> Supprimer(Guid id)
        {
            var ent = await _context.Entreprises.FindAsync(id);
            if (ent == null) return NotFound();
            _context.Entreprises.Remove(ent);
            await _context.SaveChangesAsync();
            return Ok("Entreprise supprimée.");
        }
    }
}