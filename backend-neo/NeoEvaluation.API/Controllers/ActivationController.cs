using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NeoEvaluation.API.Data;
using NeoEvaluation.API.Models;

namespace NeoEvaluation.API.Controllers
{
    public class CompleteActivationDto {
        public Guid Token { get; set; }
        public string Password { get; set; } = string.Empty;
    }

    [Route("api/[controller]")]
    [ApiController]
    public class ActivationController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ActivationController(AppDbContext context) { _context = context; }

        [HttpGet("check/{token}")]
        public async Task<IActionResult> CheckToken(Guid token)
        {
            var t = await _context.TokensActivation.FirstOrDefaultAsync(x => x.Token == token && !x.Utilise && x.DateExpiration > DateTime.UtcNow);
            return Ok(new { valide = (t != null) });
        }

        [HttpPost("complete")]
        public async Task<IActionResult> Complete([FromBody] CompleteActivationDto dto)
        {
            Console.WriteLine($"[ACTIVATION DEBUG] Tentative de finalisation pour le token: {dto.Token}");
            var token = await _context.TokensActivation.FirstOrDefaultAsync(t => t.Token == dto.Token && !t.Utilise);
            
            if (token == null) {
                Console.WriteLine("[ACTIVATION DEBUG] Token introuvable ou déjà utilisé.");
                return BadRequest(new { message = "Lien invalide ou déjà utilisé." });
            }

            if (token.DateExpiration < DateTime.UtcNow) {
                Console.WriteLine("[ACTIVATION DEBUG] Token expiré.");
                return BadRequest(new { message = "Lien expiré." });
            }

            // CAS CANDIDAT
            if (token.UtilisateurId != null && token.UtilisateurId != Guid.Empty)
            {
                Console.WriteLine($"[ACTIVATION DEBUG] Mode: CANDIDAT (User: {token.UtilisateurId})");
                var user = await _context.Utilisateurs.FindAsync(token.UtilisateurId);
                if (user == null) return NotFound(new { message = "Utilisateur non trouvé." });

                user.MotDePasseHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
                user.EstActif = true;
                token.Utilise = true;
                await _context.SaveChangesAsync();
                Console.WriteLine("[ACTIVATION DEBUG] Candidat activé avec succès.");
                return Ok(new { message = "Candidat activé." });
            }

            Console.WriteLine($"[ACTIVATION DEBUG] Mode: ENTREPRISE (Inscription: {token.IdInscription})");
            // CAS ENTREPRISE / SUPERADMIN
            var reg = await _context.InscriptionsEntreprises.FindAsync(token.IdInscription);
            if (reg == null) {
                Console.WriteLine("[ACTIVATION DEBUG] Inscription introuvable dans la DB.");
                return BadRequest(new { message = "Inscription introuvable." });
            }

            // --- CAS PARTICULIER : SUPER ADMIN (MASTER) ---
            if (reg.NomEntreprise == "Administration Neo")
            {
                Console.WriteLine("[ACTIVATION DEBUG] Détection d'un SUPER ADMIN.");
                
                var superUser = new SuperAdmin {
                    Id = Guid.NewGuid(),
                    Nom = reg.NomResponsable,
                    Prenom = reg.PrenomResponsable ?? "Master",
                    Email = reg.EmailResponsable,
                    MotDePasseHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                    RoleNom = "SuperAdmin",
                    EstActif = true,
                    Privileges = new List<string> { "ALL" }
                };

                var roleSA = await _context.Roles.FirstOrDefaultAsync(r => r.Nom == "SuperAdmin");
                if (roleSA != null) superUser.RoleId = roleSA.Id;

                _context.Utilisateurs.Add(superUser);
                token.Utilise = true;
                reg.Statut = 3;
                await _context.SaveChangesAsync();
                
                return Ok(new { message = "Master Admin activé." });
            }

            // --- CAS CLASSIQUE : ENTREPRISE ---
            // 1. Création de l'Entreprise (Clean)
            var entreprise = new Entreprise {
                Id = Guid.NewGuid(),
                Nom = reg.NomEntreprise,
                MatriculeFiscale = reg.MatriculeFiscale,
                AbonnementFin = DateTime.UtcNow.AddYears(1), // 1 an d'essai gratuit
                Plan = "Gratuit"
            };
            _context.Entreprises.Add(entreprise);

            // 2. Liaison avec EntrepriseParSA (Métadonnées du SuperAdmin)
            var detailsSA = await _context.EntrepriseParSA
                .FirstOrDefaultAsync(d => d.EmailResponsable == reg.EmailResponsable && d.EntrepriseId == null);
            
            if (detailsSA != null)
            {
                detailsSA.EntrepriseId = entreprise.Id;
            }

            // 3. Préparation de l'Admin (Personnel)
            var targetNom = reg.NomResponsable;
            var targetPrenom = reg.PrenomResponsable ?? "";

            if (string.IsNullOrWhiteSpace(targetPrenom) && reg.NomResponsable.Contains(" "))
            {
                var parts = reg.NomResponsable.Split(' ');
                targetNom = String.Join(" ", parts.Skip(1));
                targetPrenom = parts[0];
            }

            // 2. Création de l'Administrateur (Personnel)
            var adminUser = new Personnel {
                Id = Guid.NewGuid(),
                Nom = targetNom,
                Prenom = targetPrenom,
                Email = reg.EmailResponsable,
                MotDePasseHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                EntrepriseId = entreprise.Id,
                RoleNom = "AdminEntreprise",
                EstActif = true,
                IdEmploye = "ADMIN-01",
                Privileges = new List<string> { "ALL" }
            };

            // Lier au rôle AdminEntreprise s'il existe
            var roleAdmin = await _context.Roles.FirstOrDefaultAsync(r => r.Nom == "AdminEntreprise");
            if (roleAdmin == null) {
                roleAdmin = await _context.Roles.FirstOrDefaultAsync(r => r.Nom == "Admin");
            }
            if (roleAdmin != null) {
                adminUser.RoleId = roleAdmin.Id;
            }

            _context.Personnels.Add(adminUser);

            token.Utilise = true;
            reg.Statut = 3; // "Activé / Terminé"
            await _context.SaveChangesAsync();
            return Ok(new { message = "Entreprise activée." });
        }
    }
}