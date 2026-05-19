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
        private readonly INotificationService _notificationService;

        public SuperAdminController(AppDbContext context, IEmailService emailService, IAuditLogService auditLogService, INotificationService notificationService)
        {
            _context = context;
            _emailService = emailService;
            _auditLogService = auditLogService;
            _notificationService = notificationService;
        }

        // --- DASHBOARD STATS (REAL DATA) ---
        [HttpGet("stats")]
        public async Task<ActionResult<SuperAdminStatsDto>> GetStats()
        {
            try {
                // Initialisation des stats de base
                // Exclure SYSTEM_PLATFORM des comptes
                var stats = new SuperAdminStatsDto
                {
                    TotalEntreprises = await _context.Entreprises
                        .IgnoreQueryFilters()
                        .CountAsync(e => e.Nom != "SYSTEM_PLATFORM"),
                    TotalUtilisateurs = await _context.Utilisateurs
                        .IgnoreQueryFilters()
                        .CountAsync(u => u.RoleNom != "SuperAdmin"),
                    DemandesEnAttente = await _context.InscriptionsEntreprises.CountAsync(i => i.Statut == 0),
                    TotalTests = await _context.Evaluations.IgnoreQueryFilters().CountAsync()
                };

                // Calcul de la croissance des entreprises sur les 6 derniers mois
                var sixMonthsAgo = DateTime.UtcNow.AddMonths(-5);
                var startDate = new DateTime(sixMonthsAgo.Year, sixMonthsAgo.Month, 1, 0, 0, 0, DateTimeKind.Utc);
                
                var monthlyData = await _context.Entreprises
                    .IgnoreQueryFilters()
                    .Where(e => e.CreeLe >= startDate && e.Nom != "SYSTEM_PLATFORM")
                    .GroupBy(e => new { e.CreeLe.Year, e.CreeLe.Month })
                    .Select(g => new { 
                        Year = g.Key.Year, 
                        Month = g.Key.Month, 
                        Count = g.Count() 
                    })
                    .OrderBy(g => g.Year).ThenBy(g => g.Month)
                    .ToListAsync();

                for (int i = 0; i < 6; i++)
                {
                    var date = sixMonthsAgo.AddMonths(i);
                    var match = monthlyData.FirstOrDefault(d => d.Year == date.Year && d.Month == date.Month);
                    
                    stats.CroissanceStats.Add(new MonthlyGrowthDto {
                        Mois = date.ToString("MMM", System.Globalization.CultureInfo.InvariantCulture).ToUpper(),
                        Count = match?.Count ?? 0
                    });
                }

                // Récupération des abonnements récents réels depuis la base de données
                var recentOrgs = await _context.Entreprises
                    .IgnoreQueryFilters()
                    .Where(e => e.Nom != "SYSTEM_PLATFORM")
                    .OrderByDescending(e => e.CreeLe)
                    .Take(5)
                    .Select(e => new RecentTransactionDto
                    {
                        Id = e.Id,
                        Name = e.Nom,
                        Plan = e.Plan,
                        Date = e.CreeLe.ToString("dd MMM yyyy", System.Globalization.CultureInfo.InvariantCulture),
                        Price = e.Plan.ToLower() == "startup" ? 79.0 : 
                                (e.Plan.ToLower() == "business" || e.Plan.ToLower() == "business ia" ? 199.0 : 
                                (e.Plan.ToLower() == "enterprise" || e.Plan.ToLower() == "entreprise" || e.Plan.ToLower() == "enterprise ia" ? 499.0 : 0.0)),
                        Color = e.CouleurSignature ?? "#6366f1"
                    })
                    .ToListAsync();

                stats.RecentTransactions = recentOrgs;

                // Calcul dynamique des statistiques de plan d'abonnement réels
                stats.StartupCount    = await _context.Entreprises.IgnoreQueryFilters().CountAsync(e => e.Nom != "SYSTEM_PLATFORM" && e.Plan.ToLower() == "startup");
                stats.BusinessCount   = await _context.Entreprises.IgnoreQueryFilters().CountAsync(e => e.Nom != "SYSTEM_PLATFORM" && (e.Plan.ToLower() == "business" || e.Plan.ToLower() == "business ia"));
                stats.EnterpriseCount = await _context.Entreprises.IgnoreQueryFilters().CountAsync(e => e.Nom != "SYSTEM_PLATFORM" && (e.Plan.ToLower() == "enterprise" || e.Plan.ToLower() == "entreprise" || e.Plan.ToLower() == "enterprise ia"));
                stats.GratuitCount    = await _context.Entreprises.IgnoreQueryFilters().CountAsync(e => e.Nom != "SYSTEM_PLATFORM" && (e.Plan.ToLower() == "gratuit" || e.Plan.ToLower() == "starter" || string.IsNullOrEmpty(e.Plan)));
                stats.TotalRevenus    = (stats.StartupCount * 79.0) + (stats.BusinessCount * 199.0) + (stats.EnterpriseCount * 499.0);

                var now = DateTime.UtcNow;
                var sevenDaysAgo  = now.AddDays(-7);
                var thirtyDaysAgo = now.AddDays(-30);

                stats.TotalEntreprises7Days  = await _context.Entreprises.IgnoreQueryFilters().CountAsync(e => e.Nom != "SYSTEM_PLATFORM" && e.CreeLe >= sevenDaysAgo);
                stats.TotalUtilisateurs7Days = await _context.Utilisateurs.IgnoreQueryFilters().CountAsync(u => u.RoleNom != "SuperAdmin" && u.CreeLe >= sevenDaysAgo);
                stats.TotalTests7Days        = await _context.Evaluations.IgnoreQueryFilters().CountAsync(ev => ev.DateDebut != null && ev.DateDebut >= sevenDaysAgo);

                var startup7    = await _context.Entreprises.IgnoreQueryFilters().CountAsync(e => e.Nom != "SYSTEM_PLATFORM" && e.CreeLe >= sevenDaysAgo && e.Plan.ToLower() == "startup");
                var business7   = await _context.Entreprises.IgnoreQueryFilters().CountAsync(e => e.Nom != "SYSTEM_PLATFORM" && e.CreeLe >= sevenDaysAgo && (e.Plan.ToLower() == "business" || e.Plan.ToLower() == "business ia"));
                var enterprise7 = await _context.Entreprises.IgnoreQueryFilters().CountAsync(e => e.Nom != "SYSTEM_PLATFORM" && e.CreeLe >= sevenDaysAgo && (e.Plan.ToLower() == "enterprise" || e.Plan.ToLower() == "entreprise" || e.Plan.ToLower() == "enterprise ia"));
                stats.TotalRevenus7Days = (startup7 * 79.0) + (business7 * 199.0) + (enterprise7 * 499.0);

                stats.TotalEntreprises30Days  = await _context.Entreprises.IgnoreQueryFilters().CountAsync(e => e.Nom != "SYSTEM_PLATFORM" && e.CreeLe >= thirtyDaysAgo);
                stats.TotalUtilisateurs30Days = await _context.Utilisateurs.IgnoreQueryFilters().CountAsync(u => u.RoleNom != "SuperAdmin" && u.CreeLe >= thirtyDaysAgo);
                stats.TotalTests30Days        = await _context.Evaluations.IgnoreQueryFilters().CountAsync(ev => ev.DateDebut != null && ev.DateDebut >= thirtyDaysAgo);

                var startup30    = await _context.Entreprises.IgnoreQueryFilters().CountAsync(e => e.Nom != "SYSTEM_PLATFORM" && e.CreeLe >= thirtyDaysAgo && e.Plan.ToLower() == "startup");
                var business30   = await _context.Entreprises.IgnoreQueryFilters().CountAsync(e => e.Nom != "SYSTEM_PLATFORM" && e.CreeLe >= thirtyDaysAgo && (e.Plan.ToLower() == "business" || e.Plan.ToLower() == "business ia"));
                var enterprise30 = await _context.Entreprises.IgnoreQueryFilters().CountAsync(e => e.Nom != "SYSTEM_PLATFORM" && e.CreeLe >= thirtyDaysAgo && (e.Plan.ToLower() == "enterprise" || e.Plan.ToLower() == "entreprise" || e.Plan.ToLower() == "enterprise ia"));
                stats.TotalRevenus30Days = (startup30 * 79.0) + (business30 * 199.0) + (enterprise30 * 499.0);

                // Calcul dynamique des revenus mensuels réels sur les 6 derniers mois basés sur la date de création des entreprises
                stats.MonthlyRevenues = new List<double>();
                for (int i = 5; i >= 0; i--)
                {
                    var targetDate = now.AddMonths(-i);
                    var endOfTargetMonth = new DateTime(targetDate.Year, targetDate.Month, 1, 23, 59, 59, DateTimeKind.Utc).AddMonths(1).AddDays(-1);

                    var activeOrgsUpToMonth = await _context.Entreprises
                        .IgnoreQueryFilters()
                        .Where(e => e.Nom != "SYSTEM_PLATFORM" && e.CreeLe <= endOfTargetMonth)
                        .ToListAsync();

                    double revForMonth = 0;
                    foreach (var org in activeOrgsUpToMonth)
                    {
                        var planLower = org.Plan.ToLower();
                        if (planLower == "startup") revForMonth += 79.0;
                        else if (planLower == "business" || planLower == "business ia") revForMonth += 199.0;
                        else if (planLower == "enterprise" || planLower == "entreprise" || planLower == "enterprise ia") revForMonth += 499.0;
                    }
                    stats.MonthlyRevenues.Add(revForMonth);
                }

                // Vérification de l'intégration Gmail Système
                var systemOrg = await _context.Entreprises.IgnoreQueryFilters()
                    .FirstOrDefaultAsync(e => e.Nom == "SYSTEM_PLATFORM");
                    
                if (systemOrg != null) {
                    stats.IsGoogleConnected = !string.IsNullOrEmpty(systemOrg.GmailRefreshToken);
                    stats.ConnectedEmail = systemOrg.GmailEmail;
                }

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

            // Notification SuperAdmin
            await _notificationService.NotifyRoleAsync("SuperAdmin", new NotificationPayload
            {
                Type = "success",
                Title = "Entreprise approuvée",
                Message = $"\"{reg.NomEntreprise}\" a été activée sur la plateforme.",
                Link = "/super-admin"
            });

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
            } catch (Exception) {
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
                MatriculeFiscale = dto.MatriculeFiscale,
                Domaine = dto.Domaine,
                Secteur = dto.Industrie,
                SiteWeb = dto.SiteWeb,
                Ville = dto.Ville,
                Pays = dto.Pays,
                CodePostal = dto.CodePostal,
                Adresse = dto.Adresse,
                Description = dto.Description,
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
        // --- ORGANIZATIONS LIST (SuperAdmin only) ---
        [HttpGet("organizations")]
        public async Task<IActionResult> GetOrganizations([FromQuery] int page = 1, [FromQuery] int limit = 20, [FromQuery] string? search = null)
        {
            try {
                var query = _context.Entreprises
                    .IgnoreQueryFilters()
                    .Where(e => e.Nom != "SYSTEM_PLATFORM");

                if (!string.IsNullOrWhiteSpace(search))
                    query = query.Where(e =>
                        e.Nom.Contains(search) ||
                        (e.Domaine != null && e.Domaine.Contains(search)) ||
                        (e.Secteur != null && e.Secteur.Contains(search)));

                var total = await query.CountAsync();
                var data = await query
                    .OrderByDescending(e => e.CreeLe)
                    .Skip((page - 1) * limit)
                    .Take(limit)
                    .Select(e => new {
                        e.Id,
                        e.Nom,
                        e.Plan,
                        EstActif = e.AbonnementFin == null || e.AbonnementFin > DateTime.UtcNow,
                        e.CreeLe,
                        e.CouleurSignature,
                        e.Secteur,
                        e.Ville,
                        e.Pays,
                        e.Domaine,
                        e.SiteWeb,
                        e.AbonnementFin,
                        EmailAdmin = _context.Utilisateurs
                            .IgnoreQueryFilters()
                            .Where(u => u.EntrepriseId == e.Id && u.RoleNom == "AdminEntreprise")
                            .Select(u => u.Email)
                            .FirstOrDefault()
                    })
                    .ToListAsync();

                return Ok(new { total, page, limit, data });
            } catch (Exception ex) {
                Console.WriteLine($"[ERROR] GetOrganizations: {ex.Message}");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("organizations/{id}")]
        public async Task<IActionResult> DeleteOrg(Guid id)
        {
            var ent = await _context.Entreprises.IgnoreQueryFilters().FirstOrDefaultAsync(e => e.Id == id);
            if (ent == null) return NotFound();

            _context.Entreprises.Remove(ent);
            await _context.SaveChangesAsync();

            await _auditLogService.LogActionAsync("DELETE_ORG", "SuperAdmin", $"Suppression de l'organisation : {ent.Nom}");
            return Ok(new { message = "Organisation supprimée." });
        }
    }
}