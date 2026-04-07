using Microsoft.AspNetCore.Mvc;
using NeoEvaluation.API.Data;
using NeoEvaluation.API.DTOs;
using NeoEvaluation.API.Models;

namespace NeoEvaluation.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RegistrationController(AppDbContext context)
        {
            _context = context;
        }

        // POST /api/registration
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterCompanyDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var registration = new InscriptionsEntreprise
            {
                NomEntreprise = dto.NomEntreprise,
                NomResponsable = dto.NomResponsable,
                EmailResponsable = dto.EmailResponsable,
                MatriculeFiscale = dto.MatriculeFiscale,
                Statut = 0 // En attente
            };

            _context.InscriptionsEntreprises.Add(registration);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Demande d'inscription enregistrée avec succès." });
        }
    }
}
