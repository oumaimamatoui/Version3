using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using NeoEvaluation.API.Data;
using NeoEvaluation.API.Models;
using NeoEvaluation.API.Services;
using NeoEvaluation.API.Attributes;

namespace NeoEvaluation.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
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

        [HttpGet]
        [RequirePermission("view_rol")]
        public async Task<ActionResult> GetRoles()
        {
            var tenantId = _tenantService.GetTenantId();
            var roles = await _context.Roles
                .Where(r => r.EntrepriseId == tenantId && r.Nom != "SuperAdmin")
                .OrderByDescending(r => r.CreeLe)
                .ToListAsync();

            var result = new List<object>();
            foreach (var r in roles)
            {
                var count = await _context.Utilisateurs.CountAsync(u => u.RoleId == r.Id);
                result.Add(new { 
                    r.Id, r.Nom, r.Description, 
                    Permissions = r.Permissions ?? new List<string>(), 
                    r.CreeLe, NombreMembres = count, r.Email, r.Prenom, r.NomFamille 
                });
            }
            return Ok(result);
        }

        [HttpGet("stats")]
        [RequirePermission("view_rol")]
        public async Task<ActionResult> GetStats()
        {
            var tenantId = _tenantService.GetTenantId();
            var totalRoles = await _context.Roles.CountAsync(r => r.EntrepriseId == tenantId && r.Nom != "SuperAdmin");
            var totalStaff = await _context.Utilisateurs.CountAsync(u => u.EntrepriseId == tenantId && u.RoleNom != "Candidat");
            
            return Ok(new[] {
                new { label = "Rôles Déployés", value = totalRoles.ToString(), icon = "fa-solid fa-shield-halved", color = "#fbbf24", bg = "#fffbeb" },
                new { label = "Membres Staff", value = totalStaff.ToString(), icon = "fa-solid fa-user-shield", color = "#3b82f6", bg = "#eff6ff" },
                new { label = "Sécurité Système", value = "ACTIF", icon = "fa-solid fa-vault", color = "#10b981", bg = "#ecfdf5" }
            });
        }

        [HttpPost]
        [RequirePermission("add_rol")]
        public async Task<ActionResult> CreateRole([FromBody] Role role)
        {
            var tenantId = _tenantService.GetTenantId();
            
            // Vérification existence
            if (await _context.Roles.AnyAsync(r => r.EntrepriseId == tenantId && r.Nom.ToLower() == role.Nom.ToLower()))
                return BadRequest(new { message = "Ce nom de rôle est déjà utilisé dans votre organisation." });

            string activationToken = null;

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                role.Id = Guid.NewGuid();
                role.EntrepriseId = tenantId;
                role.CreeLe = DateTime.UtcNow;
                _context.Roles.Add(role);

                if (!string.IsNullOrEmpty(role.Email))
                {
                    if (await _context.Utilisateurs.AnyAsync(u => u.Email == role.Email))
                        return BadRequest(new { message = "Cette adresse email est déjà rattachée à un compte." });

                    var newUser = new Utilisateur {
                        Id = Guid.NewGuid(), EntrepriseId = tenantId, Email = role.Email,
                        Nom = role.NomFamille ?? "", Prenom = role.Prenom ?? "",
                        RoleNom = role.Nom, RoleId = role.Id, EstActif = false, CreeLe = DateTime.UtcNow,
                        Privileges = role.Permissions ?? new List<string>()
                    };
                    _context.Utilisateurs.Add(newUser);

                    var token = new TokensActivation {
                        Id = Guid.NewGuid(), Token = Guid.NewGuid(), UtilisateurId = newUser.Id,
                        Email = newUser.Email, DateCreation = DateTime.UtcNow, DateExpiration = DateTime.UtcNow.AddDays(7), Utilise = false
                    };
                    _context.TokensActivation.Add(token);
                    activationToken = token.Token.ToString();
                    Console.WriteLine($"\n========================================");
                    Console.WriteLine($"[NOUVEAU RÔLE] Rôle : {role.Nom}");
                    Console.WriteLine($"[EMAIL]        : {role.Email}");
                    Console.WriteLine($"[TOKEN]        : {activationToken}");
                    Console.WriteLine($"[LIEN]         : {_config["AppSettings:FrontendUrl"] ?? "http://localhost:5173"}/activate-account?token={activationToken}");
                    Console.WriteLine($"========================================\n");
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // ENVOI D'EMAIL : On attend la fin de l'envoi pour éviter le crash "Reader is closed"
                if (!string.IsNullOrEmpty(role.Email) && activationToken != null)
                {
                    try {
                        await SendActivationEmail(role, activationToken);
                    } catch (Exception ex) {
                        // Log de l'erreur mais le rôle est créé
                        Console.WriteLine($"[AVERTISSEMENT EMAIL] : {ex.Message}");
                    }
                }

                return CreatedAtAction(nameof(GetRoles), new { id = role.Id }, role);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { message = "Erreur technique lors du déploiement.", details = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [RequirePermission("add_rol")]
        public async Task<IActionResult> UpdateRole(Guid id, [FromBody] Role roleUpdate)
        {
            var existingRole = await _context.Roles.FindAsync(id);
            if (existingRole == null) return NotFound();

            existingRole.Nom = roleUpdate.Nom;
            existingRole.Description = roleUpdate.Description;
            existingRole.Permissions = roleUpdate.Permissions;

            var usersToUpdate = await _context.Utilisateurs.Where(u => u.RoleId == id).ToListAsync();
            foreach (var user in usersToUpdate) {
                user.Privileges = roleUpdate.Permissions;
                user.RoleNom = roleUpdate.Nom;
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [RequirePermission("add_rol")]
        public async Task<IActionResult> DeleteRole(Guid id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null) return NotFound();

            if (await _context.Utilisateurs.AnyAsync(u => u.RoleId == id))
                return BadRequest(new { message = "Impossible de révoquer un rôle assigné à des membres." });

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private async Task SendActivationEmail(Role role, string token)
        {
            string frontendUrl = _config["AppSettings:FrontendUrl"] ?? "http://localhost:5173";
            string link = $"{frontendUrl}/activate-account?token={token}";
            string subject = $"[SÉCURITÉ] Accréditation de votre accès : {role.Nom}";
            
            string body = $@"
                <div style='font-family: sans-serif; max-width: 600px; margin: auto; border: 1px solid #e2e8f0; border-radius: 16px; padding: 40px;'>
                    <h2 style='color: #0f172a;'>Activation de compte</h2>
                    <p>Un nouvel accès a été configuré pour vous avec le rôle : <b>{role.Nom}</b></p>
                    <a href='{link}' style='display: inline-block; background: #0f172a; color: #ffffff; padding: 15px 25px; text-decoration: none; border-radius: 8px; font-weight: bold;'>
                        ACTIVER MON ACCÈS
                    </a>
                </div>";

            await _emailService.SendEmailAsync(role.Email, subject, body);
        }
    }
}