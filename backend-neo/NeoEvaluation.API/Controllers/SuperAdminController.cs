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
                    TotalEntreprises = await _context.Entreprises.CountAsync(e => e.AbonnementFin == null || e.AbonnementFin > DateTime.UtcNow),
                    TotalUtilisateurs = await _context.Utilisateurs.CountAsync(),
                    DemandesEnAttente = await _context.InscriptionsEntreprises.CountAsync(i => i.Statut == 0),
                    SessionsIARecentes = await _context.Evaluations.CountAsync(e => e.Statut == StatutPassage.EN_COURS)
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
                        Org = u.EntrepriseId != null 
                              ? _context.Entreprises.Where(e => e.Id == u.EntrepriseId).Select(e => e.Nom).FirstOrDefault() ?? "Plateforme Neo"
                              : "Plateforme Neo",
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
                PrenomResponsable = "Admin", // Valeur par défaut pour une invitation
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
            
            //  PROFESSIONAL HTML TEMPLATE Email Button
            var htmlBody = $@"
                <div style='font-family: sans-serif; max-width: 600px; margin: auto; padding: 20px; border: 1px solid #eee; border-radius: 10px;'>
                    <div style='text-align: center; margin-bottom: 20px;'>
                        <h2 style='color: #f59e0b;'>NeoEvaluation</h2>
                    </div>
                    <p>Bonjour <strong>{dto.Name}</strong>,</p>
                    <p>Vous avez été invité à administrer la plateforme <strong>NeoEvaluation</strong>. Pour activer votre accès, veuillez cliquer sur le bouton ci-dessous :</p>
                    <div style='text-align: center; margin: 30px 0;'>
                        <a href='{link}' style='background-color: #f59e0b; color: white; padding: 12px 25px; text-decoration: none; border-radius: 5px; font-weight: bold; display: inline-block;'>ACTIVER MON COMPTE</a>
                    </div>
                    <p style='color: #666; font-size: 12px;'>Si le bouton ne fonctionne pas, copiez ce lien : <br> {link}</p>
                    <hr style='border: 0; border-top: 1px solid #eee; margin: 20px 0;'>
                    <p style='font-size: 11px; color: #999; text-align: center;'>&copy; 2025 NeoEvaluation - Smart Evaluation System</p>
                </div>";

            //  DEBUG TERMINAL (Fallback)
            Console.WriteLine("\n--------------------------------------------------");
            Console.WriteLine($"[DEBUG] ADMIN INVITATION: {dto.Email}");
            Console.WriteLine($"LINK: {link}");
            Console.WriteLine("--------------------------------------------------\n");

            try {
                await _emailService.SendEmailAsync(dto.Email, "Invitation SuperAdmin - NeoEvaluation", htmlBody);
                return Ok(new { message = "Invitation envoyée", token = token.Token });
            } catch (Exception ex) {
                return BadRequest(new { message = "Erreur d'envoi d'email : " + ex.Message });
            }

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
                
                // ✅ PROFESSIONAL HTML TEMPLATE
                var htmlBody = $@"
                    <div style='font-family: sans-serif; max-width: 600px; margin: auto; padding: 20px; border: 1px solid #eee; border-radius: 10px;'>
                        <div style='text-align: center; margin-bottom: 20px;'>
                            <h2 style='color: #f59e0b;'>NeoEvaluation</h2>
                        </div>
                        <h3 style='color: #0f172a;'>Félicitations !</h3>
                        <p>Votre compte <strong>NeoEvaluation</strong> a été approuvé avec succès.</p>
                        <p>Cliquez sur le bouton ci-dessous pour définir votre mot de passe et accéder à votre tableau de bord :</p>
                        <div style='text-align: center; margin: 30px 0;'>
                            <a href='{link}' style='background-color: #f59e0b; color: white; padding: 12px 25px; text-decoration: none; border-radius: 5px; font-weight: bold; display: inline-block;'>DÉFINIR MON MOT DE PASSE</a>
                        </div>
                        <p style='color: #666; font-size: 12px;'>Ce lien expirera dans 48 heures.</p>
                        <hr style='border: 0; border-top: 1px solid #eee; margin: 20px 0;'>
                        <p style='font-size: 11px; color: #999; text-align: center;'>&copy; 2025 NeoEvaluation - Smart Evaluation System</p>
                    </div>";

                // ✅ DEBUG TERMINAL (Fallback)
                Console.WriteLine("\n--------------------------------------------------");
                Console.WriteLine($"[DEBUG] ACTIVATION LINK FOR: {reg.EmailResponsable}");
                Console.WriteLine($"LINK: {link}");
                Console.WriteLine("--------------------------------------------------\n");

                await _emailService.SendEmailAsync(reg.EmailResponsable, "Compte Approuvé - NeoEvaluation", htmlBody);
                
                return Ok(new { message = "Entreprise approuvée et email envoyé." });
            } catch (Exception ex) {
            }

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
            // 1. Création de l'inscription (pour le flux d'activation)
            var reg = new InscriptionsEntreprise {
                Id = Guid.NewGuid(),
                NomEntreprise = dto.Name,
                NomResponsable = dto.AdminLastName,
                PrenomResponsable = dto.AdminFirstName,
                EmailResponsable = dto.AdminEmail,
                TelephoneResponsable = dto.AdminPhone,
                MatriculeFiscale = dto.MatriculeFiscale,
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

                // ✅ PROFESSIONAL HTML TEMPLATE
                var htmlBody = $@"
                    <div style='font-family: sans-serif; max-width: 600px; margin: auto; padding: 20px; border: 1px solid #eee; border-radius: 10px;'>
                        <div style='text-align: center; margin-bottom: 20px;'>
                            <h2 style='color: #f59e0b;'>NeoEvaluation</h2>
                        </div>
                        <h3 style='color: #0f172a;'>Bienvenue parmi nous !</h3>
                        <p>Une organisation a été créée pour vous sur la plateforme <strong>NeoEvaluation</strong>.</p>
                        <p>Veuillez cliquer sur le bouton ci-dessous pour finaliser la création de votre accès :</p>
                        <div style='text-align: center; margin: 30px 0;'>
                            <a href='{link}' style='background-color: #f59e0b; color: white; padding: 12px 25px; text-decoration: none; border-radius: 5px; font-weight: bold; display: inline-block;'>ACCÉDER À MON COMPTE</a>
                        </div>
                        <hr style='border: 0; border-top: 1px solid #eee; margin: 20px 0;'>
                        <p style='font-size: 11px; color: #999; text-align: center;'>&copy; 2025 NeoEvaluation - Smart Evaluation System</p>
                    </div>";

                // ✅ DEBUG TERMINAL (Fallback)
                Console.WriteLine("\n--------------------------------------------------");
                Console.WriteLine($"[DEBUG] MANUAL CREATION LINK FOR: {dto.AdminEmail}");
                Console.WriteLine($"LINK: {link}");
                Console.WriteLine("--------------------------------------------------\n");

                await _emailService.SendEmailAsync(dto.AdminEmail, "Accès Admin NeoEvaluation", htmlBody);
                return Ok(new { message = "Organisation créée avec succès" });
            } catch (Exception ex) {
                return BadRequest(new { message = "Erreur d'envoi d'email de création : " + ex.Message });
            }
        }
    }
}
