using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using NeoEvaluation.API.Data;
using NeoEvaluation.API.Models;
using NeoEvaluation.API.Services;

namespace NeoEvaluation.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "SuperAdmin,AdminEntreprise")]
    public class RolesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _config;
        private readonly ITenantService _tenantService;

        public RolesController(AppDbContext context, IEmailService emailService, IConfiguration config, ITenantService tenantService)
        {
            _context = context;
            _emailService = emailService;
            _config = config;
            _tenantService = tenantService;
        }

        // 1. RÉCUPÉRER TOUS LES RÔLES
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Role>>> GetRoles()
        {
            try 
            {
                // On utilise IgnoreQueryFilters pour être sûr de voir les rôles système si besoin
                var roles = await _context.Roles.ToListAsync();
                
                foreach (var r in roles)
                {
                    r.NombreMembres = await _context.Utilisateurs
                        .IgnoreQueryFilters()
                        .CountAsync(u => u.RoleId == r.Id);
                }
                
                return Ok(roles.OrderByDescending(r => r.CreeLe));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ROLES ERROR] {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }

        // 2. CRÉATION D'UN RÔLE + INVITATION D'UN PERSONNEL
        [HttpPost]
        public async Task<ActionResult<Role>> CreateRole(Role role)
        {
            // Utilisation d'une transaction pour garantir l'intégrité (Role + Utilisateur + Token)
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // A. Sauvegarde du Rôle
                role.Id ??= Guid.NewGuid();
                role.EntrepriseId = _tenantService.GetTenantId();
                role.CreeLe = DateTime.UtcNow;
                _context.Roles.Add(role);

                // B. Création de l'utilisateur (Classe concrète Personnel héritant de Utilisateur)
                var newUser = new Utilisateur 
                {
                    Id = Guid.NewGuid(),
                    EntrepriseId = role.EntrepriseId,
                    Email = role.Email ?? "",
                    Nom = role.NomFamille ?? "",
                    Prenom = role.Prenom ?? "",
                    RoleNom = role.Nom,
                    RoleId = role.Id,
                    EstActif = false, // Inactif jusqu'à l'activation du token
                    CreeLe = DateTime.UtcNow,
                    Privileges = role.Permissions ?? new List<string>() // Synchronisation des permissions
                };
                _context.Utilisateurs.Add(newUser);

                // C. Génération du Token d'activation
                // Note: On utilise "_context.TokensActivation" (singulier comme dans votre AppDbContext)
                var activationToken = new TokensActivation
                {
                    Id = Guid.NewGuid(),
                    Token = Guid.NewGuid(), // Le GUID pour l'URL
                    UtilisateurId = newUser.Id,
                    Email = newUser.Email,
                    DateCreation = DateTime.UtcNow,
                    DateExpiration = DateTime.UtcNow.AddDays(7),
                    Utilise = false
                };
                _context.TokensActivation.Add(activationToken);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // D. Envoi de l'email d'activation
                if (!string.IsNullOrEmpty(role.Email))
                {
                    string frontendUrl = _config["AppSettings:FrontendUrl"] ?? "http://localhost:5173";
                    string activationLink = $"{frontendUrl}/activate-role?token={activationToken.Token}";

                    string subject = $"[NeoEvaluation] Activation de votre accès : {role.Nom}";
                    string body = $@"
                        <div style='font-family: sans-serif; padding: 25px; border: 1px solid #e2e8f0; border-radius: 12px;'>
                            <h2 style='color: #0f172a;'>Bienvenue {role.Prenom} !</h2>
                            <p>Un profil administratif a été créé pour vous avec le rôle : <b>{role.Nom}</b>.</p>
                            <p>Veuillez cliquer sur le bouton ci-dessous pour activer votre compte et configurer votre mot de passe :</p>
                            <div style='text-align: center; margin: 30px 0;'>
                                <a href='{activationLink}' 
                                   style='background: #0f172a; color: white; padding: 12px 25px; text-decoration: none; border-radius: 8px; font-weight: bold; display: inline-block;'>
                                   Activer mes accès
                                </a>
                            </div>
                            <p style='font-size: 11px; color: #64748b; text-align: center;'>Ce lien expirera dans 7 jours.</p>
                        </div>";

                    await _emailService.SendEmailAsync(role.Email, subject, body);
                }

                return Ok(role);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"Erreur interne : {ex.Message}");
            }
        }

        // 3. STATISTIQUES
        [HttpGet("stats")]
        public async Task<ActionResult> GetStats()
        {
            var totalRoles = await _context.Roles.CountAsync();
            var totalMembres = await _context.Utilisateurs.CountAsync(u => u.RoleId != null);
            
            return Ok(new[] {
                new { label = "Total Rôles", value = totalRoles.ToString(), icon = "fa-solid fa-shield-halved", color = "#eab308", bg = "#fffbeb" },
                new { label = "Utilisateurs Staff", value = totalMembres.ToString(), icon = "fa-solid fa-user-group", color = "#10b981", bg = "#ecfdf5" },
                new { label = "Sécurité", value = "Haute", icon = "fa-solid fa-lock", color = "#f59e0b", bg = "#fff7ed" }
            });
        }

        // 4. SUPPRESSION
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(Guid id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null) return NotFound();

            // Empêcher la suppression si le rôle est utilisé
            bool isUsed = await _context.Utilisateurs.AnyAsync(u => u.RoleId == id);
            if (isUsed) return BadRequest(new { message = "Ce rôle est actuellement attribué à des utilisateurs." });

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // 5. MISE À JOUR
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRole(Guid id, Role role)
        {
            var existing = await _context.Roles.FindAsync(id);
            if (existing == null) return NotFound();

            existing.Nom = role.Nom;
            existing.Prenom = role.Prenom;
            existing.NomFamille = role.NomFamille;
            existing.Email = role.Email;
            existing.Description = role.Description;
            existing.Permissions = role.Permissions;

            // Optionnel : Mettre à jour les privilèges des utilisateurs qui possèdent ce rôle
            var usersToUpdate = await _context.Utilisateurs.Where(u => u.RoleId == id).ToListAsync();
            foreach (var user in usersToUpdate)
            {
                user.Privileges = role.Permissions;
                user.RoleNom = role.Nom;
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}