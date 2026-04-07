using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NeoEvaluation.API.Data;
using NeoEvaluation.API.DTOs;
using NeoEvaluation.API.Models;
using NeoEvaluation.API.Services;

namespace NeoEvaluation.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuperAdminController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;
        private readonly IAuditLogService _auditLogService;

        public SuperAdminController(AppDbContext context, IEmailService emailService, IAuditLogService auditLogService)
        {
            _context = context;
            _emailService = emailService;
            _auditLogService = auditLogService;
        }

        // --- DASHBOARD STATS (REAL DATA) ---
        [HttpGet("stats")]
        public async Task<ActionResult<SuperAdminStatsDto>> GetStats()
        {
            try {
                var stats = new SuperAdminStatsDto
                {
                    TotalEntreprises = await _context.Entreprises.CountAsync(e => e.EstActif),
                    TotalUtilisateurs = await _context.Utilisateurs.CountAsync(),
                    DemandesEnAttente = await _context.InscriptionsEntreprises.CountAsync(i => i.Statut == 0),
                    SessionsIARecentes = await _context.Evaluations.CountAsync(e => e.Statut == EvaluationStatus.EN_COURS)
                };
                return Ok(stats);
            } catch (Exception ex) {
                Console.WriteLine($"[STATS ERROR] {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }

        // --- PLATFORM USERS MANAGEMENT ---
        [HttpGet("users")]
        public async Task<ActionResult<List<PlatformUserDto>>> GetAllUsers()
        {
            try {
                // Version robuste pour éviter les erreurs 500 dues aux jointures ou aux champs NULL
                var users = await _context.Utilisateurs
                    .Select(u => new PlatformUserDto {
                        Id = u.Id,
                        Name = ((u.Prenom ?? "") + " " + (u.Nom ?? "")).Trim(),
                        Email = u.Email,
                        Org = u.Entreprise != null ? u.Entreprise.Nom : "Plateforme Neo",
                        Role = u.RoleNom,
                        IsActive = u.EstActif,
                        LastLogin = "Récemment"
                    })
                    .ToListAsync();

                return Ok(users);
            } catch (Exception ex) {
                // Log de secours si ça persiste
                Console.WriteLine($"[CRITICAL ERROR] GetAllUsers: {ex.Message}");
                return StatusCode(500, "Erreur interne lors de la récupération des utilisateurs.");
            }
        }

        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var user = await _context.Utilisateurs.FindAsync(id);
            if (user == null) return NotFound();

            _context.Utilisateurs.Remove(user);
            await _context.SaveChangesAsync();

            await _auditLogService.LogActionAsync("DELETE_USER", "SuperAdmin", $"Suppression de l'utilisateur : {user.Email}");
            return Ok(new { message = "Utilisateur supprimé" });
        }

        [HttpPost("users/{id}/toggle-status")]
        public async Task<IActionResult> ToggleUserStatus(Guid id)
        {
            var user = await _context.Utilisateurs.FindAsync(id);
            if (user == null) return NotFound();

            user.EstActif = !user.EstActif;
            await _context.SaveChangesAsync();

            await _auditLogService.LogActionAsync("TOGGLE_USER_STATUS", "SuperAdmin", $"Statut de {user.Email} changé en : {(user.EstActif ? "Actif" : "Inactif")}");

            return Ok(new { message = "Statut mis à jour", isActive = user.EstActif });
        }

        [HttpPost("invite-admin")]
        public async Task<IActionResult> InviteSuperAdmin([FromBody] InviteAdminDto dto)
        {
            var superAdminRole = await _context.Roles.FirstOrDefaultAsync(r => r.Nom == "SuperAdmin");
            if (superAdminRole == null) return BadRequest("Rôle SuperAdmin manquant.");

            // Créer une inscription fictive pour porter l'activation
            var reg = new InscriptionsEntreprise {
                Id = Guid.NewGuid(),
                NomEntreprise = "Administration Neo",
                NomResponsable = dto.Name,
                EmailResponsable = dto.Email,
                Statut = 1, // Immédiatement approuvé pour un admin
                CreeLe = DateTime.UtcNow
            };

            var token = new TokensActivation {
                IdInscription = reg.Id,
                Token = Guid.NewGuid(),
                DateExpiration = DateTime.UtcNow.AddHours(24),
                Utilise = false
            };

            _context.InscriptionsEntreprises.Add(reg);
            _context.TokensActivation.Add(token);
            await _context.SaveChangesAsync();

            await _auditLogService.LogActionAsync("INVITE_ADMIN", "SuperAdmin", $"Invitation d'un nouvel administrateur : {dto.Email}");

            var link = $"http://localhost:5173/definir-mot-de-passe?token={token.Token}";
            
            //  DEBUG TERMINAL (comme demandé)
            Console.WriteLine("\n--------------------------------------------------");
            Console.WriteLine($"[DEBUG] ADMIN INVITATION: {dto.Email}");
            Console.WriteLine($"LINK: {link}");
            Console.WriteLine("--------------------------------------------------\n");

            try {
                await _emailService.SendEmailAsync(dto.Email, "Invitation SuperAdmin - NeoEvaluation", $"Bonjour {dto.Name}, vous avez été invité à administrer la plateforme NeoEvaluation. Activez votre compte ici : {link}");
            } catch { }

            return Ok(new { message = "Invitation envoyée", token = token.Token });
        }

        // --- AUDIT LOGS ---
        [HttpGet("audit-logs")]
        public async Task<ActionResult<List<AuditLogEntry>>> GetAuditLogs()
        {
            return Ok(await _auditLogService.GetLogsAsync());
        }

        [HttpDelete("audit-logs")]
        public async Task<IActionResult> ClearAuditLogs()
        {
            await _auditLogService.ClearLogsAsync();
            return Ok();
        }

        // --- ORGANIZATION MANAGEMENT (REUSING InscriptionsEntreprises) ---
        [HttpGet("pending")]
        public async Task<ActionResult> GetPending()
        {
            var list = await _context.InscriptionsEntreprises
                .Where(i => i.Statut == 0)
                .OrderByDescending(i => i.CreeLe)
                .ToListAsync();
            return Ok(list);
        }

        [HttpPost("approve/{id}")]
        public async Task<IActionResult> Approve(Guid id)
        {
            var reg = await _context.InscriptionsEntreprises.FindAsync(id);
            if (reg == null) return NotFound();

            reg.Statut = 1;
            
            var token = new TokensActivation {
                IdInscription = reg.Id,
                Token = Guid.NewGuid(),
                DateExpiration = DateTime.UtcNow.AddHours(48),
                Utilise = false
            };

            _context.TokensActivation.Add(token);
            await _context.SaveChangesAsync();

            // Log action
            await _auditLogService.LogActionAsync("APPROVE_ORG", "SuperAdmin", $"Approbation de l'entreprise : {reg.NomEntreprise}");

            try {
                var link = $"http://localhost:5173/definir-mot-de-passe?token={token.Token}";
                
                // ✅ LOG CONSOLE POUR DÉVELOPPEMENT (si l'email tarde)
                Console.WriteLine("\n--------------------------------------------------");
                Console.WriteLine($"[DEBUG] ACTIVATION LINK FOR: {reg.EmailResponsable}");
                Console.WriteLine($"LINK: {link}");
                Console.WriteLine("--------------------------------------------------\n");

                await _emailService.SendEmailAsync(reg.EmailResponsable, "Compte Approuvé", 
                    $"Félicitations ! Votre compte NeoEvaluation a été approuvé. Cliquez ici pour définir votre mot de passe : {link}");
            } catch { }

            return Ok(new { message = "Entreprise approuvée avec succès" });
        }

        [HttpPost("reject/{id}")]
        public async Task<IActionResult> Reject(Guid id)
        {
            var reg = await _context.InscriptionsEntreprises.FindAsync(id);
            if (reg == null) return NotFound();
            
            reg.Statut = 2; 
            await _context.SaveChangesAsync();

            await _auditLogService.LogActionAsync("REJECT_ORG", "SuperAdmin", $"Refus de l'entreprise : {reg.NomEntreprise}");

            return Ok(new { message = "Demande refusée" });
        }

        [HttpPost("create-org")]
        public async Task<IActionResult> CreateOrg([FromBody] AdminCreateOrgDto dto)
        {
            var reg = new InscriptionsEntreprise {
                Id = Guid.NewGuid(),
                NomEntreprise = dto.Name,
                MatriculeFiscale = dto.MatriculeFiscale,
                NomResponsable = $"{dto.AdminFirstName} {dto.AdminLastName}",
                EmailResponsable = dto.AdminEmail,
                Statut = 1,
                CreeLe = DateTime.UtcNow
            };

            var token = new TokensActivation {
                IdInscription = reg.Id,
                Token = Guid.NewGuid(),
                DateExpiration = DateTime.UtcNow.AddHours(72),
                Utilise = false
            };

            _context.InscriptionsEntreprises.Add(reg);
            _context.TokensActivation.Add(token);
            await _context.SaveChangesAsync();

            await _auditLogService.LogActionAsync("CREATE_ORG", "SuperAdmin", $"Création manuelle de l'organisation : {dto.Name}");

            try {
                var link = $"http://localhost:5173/definir-mot-de-passe?token={token.Token}";

                // ✅ LOG CONSOLE POUR DÉVELOPPEMENT (si l'email tarde)
                Console.WriteLine("\n--------------------------------------------------");
                Console.WriteLine($"[DEBUG] MANUAL CREATION LINK FOR: {dto.AdminEmail}");
                Console.WriteLine($"LINK: {link}");
                Console.WriteLine("--------------------------------------------------\n");

                await _emailService.SendEmailAsync(dto.AdminEmail, "Accès Admin NeoEvaluation", $"Votre organisation a été créée. Définissez votre accès ici : {link}");
            } catch { }

            return Ok(new { message = "Organisation créée avec succès" });
        }
    }
}
