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

            // Splitting logic pour obtenir un prénom par défaut si possible
            var fullName = dto.NomResponsable ?? "Responsable";
            var parts = fullName.Trim().Split(' ');
            var prenom = parts.Length > 1 ? parts[0] : "Responsable";
            var nom = parts.Length > 1 ? string.Join(" ", parts.Skip(1)) : fullName;

            var registration = new InscriptionsEntreprise
            {
                NomEntreprise = dto.NomEntreprise,
                NomResponsable = nom,
                PrenomResponsable = prenom,
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
