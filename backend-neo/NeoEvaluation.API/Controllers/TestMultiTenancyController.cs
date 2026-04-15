using Microsoft.AspNetCore.Mvc;
using NeoEvaluation.API.Data;
using NeoEvaluation.API.Models;
using System;
using System.Threading.Tasks;

namespace NeoEvaluation.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestMultiTenancyController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TestMultiTenancyController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("seed-test-data")]
        public async Task<IActionResult> SeedTestData()
        {
            // 1. Création de l'Entreprise A
            var entrepriseA = new Entreprise 
            { 
                Id = Guid.NewGuid(), 
                Nom = "Alpha Tech", 
                Secteur = "Informatique"
            };
            _context.Entreprises.Add(entrepriseA);

            // Admin pour Entreprise A
            var adminA = new Personnel
            {
                Id = Guid.NewGuid(),
                Nom = "Alpha",
                Prenom = "Admin",
                Email = "admin@alphatech.com",
                MotDePasseHash = BCrypt.Net.BCrypt.HashPassword("Alpha123!"),
                RoleNom = "AdminEntreprise",
                EntrepriseId = entrepriseA.Id,
                EstActif = true
            };
            _context.Utilisateurs.Add(adminA);

            // Employé pour Entreprise A
            var staffA = new Personnel
            {
                Id = Guid.NewGuid(),
                Nom = "Alpha",
                Prenom = "Employe 1",
                Email = "employe1@alphatech.com",
                MotDePasseHash = BCrypt.Net.BCrypt.HashPassword("Alpha123!"),
                RoleNom = "Recruteur",
                EntrepriseId = entrepriseA.Id,
                EstActif = true
            };
            _context.Utilisateurs.Add(staffA);


            // 2. Création de l'Entreprise B
            var entrepriseB = new Entreprise 
            { 
                Id = Guid.NewGuid(), 
                Nom = "Beta Solutions", 
                Secteur = "Consulting"
            };
            _context.Entreprises.Add(entrepriseB);

            // Admin pour Entreprise B
            var adminB = new Personnel
            {
                Id = Guid.NewGuid(),
                Nom = "Beta",
                Prenom = "Admin",
                Email = "admin@betasolutions.com",
                MotDePasseHash = BCrypt.Net.BCrypt.HashPassword("Beta123!"),
                RoleNom = "AdminEntreprise",
                EntrepriseId = entrepriseB.Id,
                EstActif = true
            };
            _context.Utilisateurs.Add(adminB);

            // Employé pour Entreprise B
            var staffB = new Personnel
            {
                Id = Guid.NewGuid(),
                Nom = "Beta",
                Prenom = "Employe 1",
                Email = "employe1@betasolutions.com",
                MotDePasseHash = BCrypt.Net.BCrypt.HashPassword("Beta123!"),
                RoleNom = "Evaluateur",
                EntrepriseId = entrepriseB.Id,
                EstActif = true
            };
            _context.Utilisateurs.Add(staffB);

            await _context.SaveChangesAsync();

            return Ok(new { 
                Message = "Test data created successfully!",
                EntrepriseA = new { Nom = entrepriseA.Nom, AdminEmail = adminA.Email, Password = "Alpha123!" },
                EntrepriseB = new { Nom = entrepriseB.Nom, AdminEmail = adminB.Email, Password = "Beta123!" }
            });
        }
    }
}
