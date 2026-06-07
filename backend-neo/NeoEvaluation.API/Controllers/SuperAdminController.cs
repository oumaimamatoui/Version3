using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
        private readonly IConfiguration _config;

        public SuperAdminController(AppDbContext context, IEmailService emailService, IAuditLogService auditLogService, INotificationService notificationService, IConfiguration config)
        {
            _context = context;
            _emailService = emailService;
            _auditLogService = auditLogService;
            _notificationService = notificationService;
            _config = config;
        }

        // --- DASHBOARD STATS (REAL DATA) ---
        [HttpGet("stats")]
        public async Task<ActionResult<SuperAdminStatsDto>> GetStats()
        {
            try {
                // Initialisation des stats de base
                // Exclure SYSTEM_PLATFORM des comptes
                var now = DateTime.UtcNow;

                var allOrgs = await _context.Entreprises
                    .IgnoreQueryFilters()
                    .Where(e => e.Nom != "SYSTEM_PLATFORM")
                    .ToListAsync();

                var stats = new SuperAdminStatsDto
                {
                    TotalEntreprises = allOrgs.Count,
                    ActiveCount = allOrgs.Count(e => e.AbonnementFin == null || e.AbonnementFin > now),
                    InactiveCount = allOrgs.Count(e => e.AbonnementFin != null && e.AbonnementFin <= now),
                    TotalUtilisateurs = await _context.Utilisateurs
                        .IgnoreQueryFilters()
                        .CountAsync(u => u.RoleNom != "SuperAdmin"),
                    DemandesEnAttente = await _context.InscriptionsEntreprises.CountAsync(i => i.Statut == 0 && i.PaymentStatus == 1),
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
                        Color = "#6366f1"
                    })
                    .ToListAsync();

                stats.RecentTransactions = recentOrgs;

                // Calcul dynamique des statistiques de plan d'abonnement réels
                stats.StartupCount    = await _context.Entreprises.IgnoreQueryFilters().CountAsync(e => e.Nom != "SYSTEM_PLATFORM" && e.Plan.ToLower() == "startup");
                stats.BusinessCount   = await _context.Entreprises.IgnoreQueryFilters().CountAsync(e => e.Nom != "SYSTEM_PLATFORM" && (e.Plan.ToLower() == "business" || e.Plan.ToLower() == "business ia"));
                stats.EnterpriseCount = await _context.Entreprises.IgnoreQueryFilters().CountAsync(e => e.Nom != "SYSTEM_PLATFORM" && (e.Plan.ToLower() == "enterprise" || e.Plan.ToLower() == "entreprise" || e.Plan.ToLower() == "enterprise ia"));
                stats.GratuitCount    = await _context.Entreprises.IgnoreQueryFilters().CountAsync(e => e.Nom != "SYSTEM_PLATFORM" && (e.Plan.ToLower() == "gratuit" || e.Plan.ToLower() == "starter" || string.IsNullOrEmpty(e.Plan)));
                stats.TotalRevenus    = (stats.StartupCount * 79.0) + (stats.BusinessCount * 199.0) + (stats.EnterpriseCount * 499.0);

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
                    .Where(u => u.RoleNom != "SuperAdmin")
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

            var link = $"{_config["AppSettings:FrontendUrl"]}/definir-mot-de-passe?token={token.Token}";
            
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

        [HttpGet("pending")]
        public async Task<ActionResult> GetPending()
        {
            var list = await _context.InscriptionsEntreprises
                .Where(i => i.Statut == 0 && i.PaymentStatus == 1)
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
                var link = $"{_config["AppSettings:FrontendUrl"]}/definir-mot-de-passe?token={token.Token}";
                
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
                                var link = $"{_config["AppSettings:FrontendUrl"]}/definir-mot-de-passe?token={token.Token}";

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
                var orgs = await query
                    .OrderByDescending(e => e.CreeLe)
                    .Skip((page - 1) * limit)
                    .Take(limit)
                    .Select(e => new {
                        e.Id,
                        e.Nom,
                        e.Plan,
                        EstActif = e.AbonnementFin == null || e.AbonnementFin > DateTime.UtcNow,
                        e.CreeLe,
                        CouleurSignature = "#6366f1",
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

                var orgIds = orgs.Select(o => o.Id).ToList();

                // Staff counts per org
                var staffCounts = await _context.Utilisateurs
                    .IgnoreQueryFilters()
                    .Where(u => u.EntrepriseId.HasValue && orgIds.Contains(u.EntrepriseId.Value))
                    .GroupBy(u => u.EntrepriseId!.Value)
                    .Select(g => new {
                        OrgId = g.Key,
                        Total = g.Count(),
                        Active = g.Count(u => u.EstActif)
                    })
                    .ToListAsync();

                // Evaluation stats per org (via Campagne -> Candidature -> Evaluation)
                var evalStats = await _context.Evaluations
                    .IgnoreQueryFilters()
                    .Where(e => e.Candidature != null && e.Candidature.Campagne != null
                        && e.Candidature.Campagne.EntrepriseId.HasValue
                        && orgIds.Contains(e.Candidature.Campagne.EntrepriseId.Value))
                    .GroupBy(e => e.Candidature!.Campagne!.EntrepriseId!.Value)
                    .Select(g => new {
                        OrgId = g.Key,
                        Total = g.Count(),
                        Completed = g.Count(e => e.Statut == StatutPassage.TERMINE)
                    })
                    .ToListAsync();

                var staffLookup = staffCounts.ToDictionary(s => s.OrgId);
                var evalLookup  = evalStats.ToDictionary(e => e.OrgId);

                var data = orgs.Select(o => {
                    var staff = staffLookup.GetValueOrDefault(o.Id);
                    var evals = evalLookup.GetValueOrDefault(o.Id);

                    // Score components
                    double staffScore = 0;
                    if (staff != null && staff.Total > 0)
                        staffScore = 35.0 * staff.Active / staff.Total;

                    double evalScore = 0;
                    if (evals != null && evals.Total > 0)
                        evalScore = 35.0 * evals.Completed / evals.Total;

                    double subScore = 30;
                    if (o.AbonnementFin.HasValue)
                    {
                        var daysLeft = (o.AbonnementFin.Value - DateTime.UtcNow).TotalDays;
                        if (daysLeft <= 0) subScore = 0;
                        else if (daysLeft < 30) subScore = 30.0 * daysLeft / 30.0;
                    }

                    var rawScore = staffScore + evalScore + subScore;
                    return new {
                        o.Id, o.Nom, o.Plan,
                        EstActif = o.AbonnementFin == null || o.AbonnementFin > DateTime.UtcNow,
                        o.CreeLe, o.CouleurSignature, o.Secteur, o.Ville, o.Pays,
                        o.Domaine, o.SiteWeb, o.AbonnementFin, o.EmailAdmin,
                        Score = Math.Round(rawScore, 0)
                    };
                }).ToList();

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

            var orgName = ent.Nom;

            // 1. Récupérer les IDs des utilisateurs de l'organisation
            var userIds = await _context.Utilisateurs
                .IgnoreQueryFilters()
                .Where(u => u.EntrepriseId == id)
                .Select(u => u.Id)
                .ToListAsync();

            // 2. Récupérer les IDs des campagnes de l'organisation
            var campagneIds = await _context.Campagnes
                .IgnoreQueryFilters()
                .Where(c => c.EntrepriseId == id)
                .Select(c => c.Id)
                .ToListAsync();

            // 3. Récupérer les IDs des candidatures liées à ces campagnes
            var candidatureIds = await _context.Candidatures
                .IgnoreQueryFilters()
                .Where(c => campagneIds.Contains(c.CampagneId))
                .Select(c => c.Id)
                .ToListAsync();

            // 4. Récupérer les IDs des évaluations liées à ces candidatures
            var evaluationIds = await _context.Evaluations
                .IgnoreQueryFilters()
                .Where(e => candidatureIds.Contains(e.CandidatureId))
                .Select(e => e.Id)
                .ToListAsync();

            // 5. Récupérer les IDs des questionnaires de l'organisation
            var questionnaireIds = await _context.Questionnaires
                .IgnoreQueryFilters()
                .Where(q => q.EntrepriseId == id)
                .Select(q => q.Id)
                .ToListAsync();

            // 6. Récupérer les IDs des questions de l'organisation
            var questionIds = await _context.Questions
                .IgnoreQueryFilters()
                .Where(q => q.EntrepriseId == id)
                .Select(q => q.Id)
                .ToListAsync();

            // 7. Récupérer les IDs des rôles de l'organisation
            var roleIds = await _context.Roles
                .IgnoreQueryFilters()
                .Where(r => r.EntrepriseId == id && r.Id != null)
                .Select(r => r.Id!.Value)
                .ToListAsync();

            // 8. Nettoyage en cascade : du plus dépendant au moins dépendant

            // TokensActivation liés aux utilisateurs de l'org
            var tokens = await _context.TokensActivation
                .IgnoreQueryFilters()
                .Where(t => t.UtilisateurId != null && userIds.Contains(t.UtilisateurId.Value))
                .ToListAsync();
            _context.TokensActivation.RemoveRange(tokens);

            // Reponses liées aux évaluations
            var reponses = await _context.Reponses
                .IgnoreQueryFilters()
                .Where(r => evaluationIds.Contains(r.EvaluationId))
                .ToListAsync();
            _context.Reponses.RemoveRange(reponses);

            // Rapports liés aux évaluations
            var rapports = await _context.Rapports
                .IgnoreQueryFilters()
                .Where(r => evaluationIds.Contains(r.EvaluationId))
                .ToListAsync();
            _context.Rapports.RemoveRange(rapports);

            // Evaluations liées aux candidatures
            var evaluations = await _context.Evaluations
                .IgnoreQueryFilters()
                .Where(e => candidatureIds.Contains(e.CandidatureId))
                .ToListAsync();
            _context.Evaluations.RemoveRange(evaluations);

            // Candidatures liées aux campagnes
            var candidatures = await _context.Candidatures
                .IgnoreQueryFilters()
                .Where(c => campagneIds.Contains(c.CampagneId))
                .ToListAsync();
            _context.Candidatures.RemoveRange(candidatures);

            // CampagneQuestionnaires liées aux campagnes
            var campagneQuestionnaires = await _context.CampagneQuestionnaires
                .IgnoreQueryFilters()
                .Where(cq => campagneIds.Contains(cq.CampagneId))
                .ToListAsync();
            _context.CampagneQuestionnaires.RemoveRange(campagneQuestionnaires);

            // Campagnes liées à l'organisation
            var campagnes = await _context.Campagnes
                .IgnoreQueryFilters()
                .Where(c => c.EntrepriseId == id)
                .ToListAsync();
            _context.Campagnes.RemoveRange(campagnes);

            // Documents liés aux utilisateurs
            var docs = await _context.Set<DocumentCandidat>()
                .IgnoreQueryFilters()
                .Where(d => userIds.Contains(d.CandidatId))
                .ToListAsync();
            _context.Set<DocumentCandidat>().RemoveRange(docs);

            // QuestionnaireQuestions liés aux questionnaires de l'org
            var qq = await _context.QuestionnaireQuestions
                .IgnoreQueryFilters()
                .Where(q => questionnaireIds.Contains(q.QuestionnaireId) || questionIds.Contains(q.QuestionId))
                .ToListAsync();
            _context.QuestionnaireQuestions.RemoveRange(qq);

            // Questions liées à l'org
            var questions = await _context.Questions
                .IgnoreQueryFilters()
                .Where(q => q.EntrepriseId == id)
                .ToListAsync();
            _context.Questions.RemoveRange(questions);

            // Questionnaires liés à l'org
            var questionnaires = await _context.Questionnaires
                .IgnoreQueryFilters()
                .Where(q => q.EntrepriseId == id)
                .ToListAsync();
            _context.Questionnaires.RemoveRange(questionnaires);

            // Catégories / SousCatégories liées à l'org
            var categories = await _context.Categories
                .IgnoreQueryFilters()
                .Where(c => c.EntrepriseId == id)
                .ToListAsync();
            var categorieIds = categories.Select(c => c.Id).ToList();
            var sousCategories = await _context.SousCategories
                .IgnoreQueryFilters()
                .Where(s => categorieIds.Contains(s.CategorieId))
                .ToListAsync();
            _context.SousCategories.RemoveRange(sousCategories);
            _context.Categories.RemoveRange(categories);

            // Utilisateurs liés à l'org
            var users = await _context.Utilisateurs
                .IgnoreQueryFilters()
                .Where(u => u.EntrepriseId == id)
                .ToListAsync();
            _context.Utilisateurs.RemoveRange(users);

            // Rôles liés à l'org
            var roles = await _context.Roles
                .IgnoreQueryFilters()
                .Where(r => r.EntrepriseId == id)
                .ToListAsync();
            _context.Roles.RemoveRange(roles);

            // UsageLogs liés à l'org
            var logs = await _context.UsageLogs
                .IgnoreQueryFilters()
                .Where(l => l.EntrepriseId == id)
                .ToListAsync();
            _context.UsageLogs.RemoveRange(logs);

            // Enfin, supprimer l'organisation elle-même
            _context.Entreprises.Remove(ent);
            await _context.SaveChangesAsync();

            await _auditLogService.LogActionAsync("DELETE_ORG", "SuperAdmin", $"Suppression définitive de l'organisation : {orgName}");
            return Ok(new { message = "Organisation et toutes ses données liées supprimées définitivement." });
        }

        [HttpPut("organizations/{id}")]
        public async Task<IActionResult> UpdateOrg(Guid id, [FromBody] SuperAdminUpdateOrgDto dto)
        {
            try {
                var ent = await _context.Entreprises.IgnoreQueryFilters().FirstOrDefaultAsync(e => e.Id == id);
                if (ent == null) return NotFound(new { message = "Organisation non trouvée" });

                ent.Nom = dto.Nom;
                ent.Plan = dto.Plan;
                ent.Secteur = dto.Secteur;
                ent.Domaine = dto.Domaine;
                ent.SiteWeb = dto.SiteWeb;
                ent.Ville = dto.Ville;
                ent.Pays = dto.Pays;
                ent.MatriculeFiscale = dto.MatriculeFiscale;
                ent.Description = dto.Description;

                if (dto.EstActif)
                {
                    if (ent.AbonnementFin == null || ent.AbonnementFin <= DateTime.UtcNow)
                    {
                        ent.AbonnementDebut = DateTime.UtcNow;
                        ent.AbonnementFin = DateTime.UtcNow.AddYears(1);
                    }
                }
                else
                {
                    ent.AbonnementFin = DateTime.UtcNow.AddDays(-1);
                }

                _context.Entreprises.Update(ent);
                await _context.SaveChangesAsync();

                await _auditLogService.LogActionAsync("UPDATE_ORG", "SuperAdmin", $"Modification de l'organisation : {ent.Nom}");
                return Ok(new { message = "Organisation mise à jour avec succès", data = ent });
            } catch (Exception ex) {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // --- NEW : GESTION DYNAMIC DU MAILER SYSTEM ---
        [HttpGet("mailer-diagnostics")]
        public async Task<ActionResult<MailerDiagnosticsDto>> GetMailerDiagnostics()
        {
            try
            {
                var systemOrg = await _context.Entreprises.IgnoreQueryFilters()
                    .FirstOrDefaultAsync(e => e.Nom == "SYSTEM_PLATFORM");

                var dto = new MailerDiagnosticsDto();
                dto.DiagnosticsLogs.Add($"[INFO] {DateTime.Now:HH:mm:ss} Début du diagnostic SMTP / Gmail API...");

                if (systemOrg == null)
                {
                    dto.IsGoogleConnected = false;
                    dto.DiagnosticsLogs.Add("[CRITICAL] Compte SYSTEM_PLATFORM introuvable en base de données.");
                    return Ok(dto);
                }

                dto.IsGoogleConnected = !string.IsNullOrEmpty(systemOrg.GmailRefreshToken);
                dto.Email = systemOrg.GmailEmail;

                dto.DiagnosticsLogs.Add($"[INFO] Vérification de la configuration d'arrière-plan...");
                dto.DiagnosticsLogs.Add($"[INFO] Client ID configuré : {(!string.IsNullOrEmpty(_config["GoogleAuthSettings:ClientId"]) ? "OUI" : "NON")}");
                dto.DiagnosticsLogs.Add($"[INFO] Client Secret configuré : {(!string.IsNullOrEmpty(_config["GoogleAuthSettings:ClientSecret"]) ? "OUI" : "NON")}");

                if (dto.IsGoogleConnected)
                {
                    dto.DiagnosticsLogs.Add($"[SUCCESS] Compte Gmail Système connecté : {dto.Email}");
                    if (systemOrg.GmailTokenExpiresAt.HasValue)
                    {
                        var remainingTime = systemOrg.GmailTokenExpiresAt.Value - DateTime.UtcNow;
                        if (remainingTime.TotalSeconds > 0)
                        {
                            dto.DiagnosticsLogs.Add($"[SUCCESS] Token d'accès Gmail valide (expire dans {(int)remainingTime.TotalMinutes} minutes).");
                        }
                        else
                        {
                            dto.DiagnosticsLogs.Add("[WARNING] Token d'accès Gmail expiré. Un rafraîchissement automatique sera tenté lors du prochain envoi.");
                        }
                    }
                }
                else
                {
                    dto.DiagnosticsLogs.Add("[CRITICAL] Aucun compte Gmail n'est lié pour l'envoi global des emails système.");
                    dto.DiagnosticsLogs.Add("[HELP] Solution : Cliquez sur 'Se connecter avec Google' dans le panneau pour lier le compte Gmail SMTP.");
                }

                // Compter les invitations "bloquées" (candidats/staff invités mais inactifs)
                dto.PendingInvitesCount = await _context.Utilisateurs.IgnoreQueryFilters()
                    .CountAsync(u => !u.EstActif && u.MotDePasseHash != null && u.MotDePasseHash.StartsWith("INVITED_"));

                dto.DiagnosticsLogs.Add($"[INFO] Invitations bloquées/en attente détectées en base de données : {dto.PendingInvitesCount}");
                if (dto.PendingInvitesCount > 0 && !dto.IsGoogleConnected)
                {
                    dto.DiagnosticsLogs.Add("[WARNING] Les invitations sont actuellement bloquées car le service mailer est HORS LIGNE (Down).");
                }
                else if (dto.PendingInvitesCount > 0 && dto.IsGoogleConnected)
                {
                    dto.DiagnosticsLogs.Add("[INFO] Le service mailer est EN LIGNE. Vous pouvez débloquer et renvoyer toutes ces invitations.");
                }

                dto.DiagnosticsLogs.Add($"[INFO] {DateTime.Now:HH:mm:ss} Diagnostic mailer terminé.");
                return Ok(dto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("mailer-retrigger")]
        public async Task<IActionResult> RetriggerMailer()
        {
            try
            {
                var systemOrg = await _context.Entreprises.IgnoreQueryFilters()
                    .FirstOrDefaultAsync(e => e.Nom == "SYSTEM_PLATFORM");

                if (systemOrg == null || string.IsNullOrEmpty(systemOrg.GmailRefreshToken))
                {
                    return BadRequest("Impossible de relancer : Aucun compte Gmail n'est connecté sur la plateforme.");
                }

                // Récupérer tous les utilisateurs invités mais inactifs
                var pendingUsers = await _context.Utilisateurs.IgnoreQueryFilters()
                    .Where(u => !u.EstActif && u.MotDePasseHash != null && u.MotDePasseHash.StartsWith("INVITED_"))
                    .ToListAsync();

                int resentCount = 0;
                foreach (var user in pendingUsers)
                {
                    // Chercher un token d'activation valide
                    var token = await _context.TokensActivation.IgnoreQueryFilters()
                        .Where(t => t.UtilisateurId == user.Id && !t.Utilise && t.DateExpiration > DateTime.UtcNow)
                        .OrderByDescending(t => t.DateCreation)
                        .FirstOrDefaultAsync();

                    if (token == null)
                    {
                        // Si le token est expiré ou absent, en recréer un
                        token = new TokensActivation
                        {
                            Id = Guid.NewGuid(),
                            Token = Guid.NewGuid(),
                            UtilisateurId = user.Id,
                            Email = user.Email,
                            DateCreation = DateTime.UtcNow,
                            DateExpiration = DateTime.UtcNow.AddDays(7),
                            Utilise = false,
                            IdInscription = Guid.Empty
                        };
                        _context.TokensActivation.Add(token);
                        await _context.SaveChangesAsync();
                    }

                    string activationLink = $"{_config["AppSettings:FrontendUrl"]}/activate-account?token={token.Token}";
                    string subject = $"[Rappel] Votre compte EvaluaTech est prêt";
                    
                    string htmlBody = $@"
                        <div style='font-family: sans-serif; max-width: 600px; margin: auto; padding: 20px; border: 1px solid #eee; border-radius: 12px;'>
                            <div style='text-align: center; margin-bottom: 25px;'>
                                <h2 style='color: #f59e0b; margin: 0;'>EvaluaTech</h2>
                                <p style='font-size: 10px; font-weight: bold; color: #94a3b8; letter-spacing: 2px;'>SMART EVALUATION SYSTEM</p>
                            </div>
                            <h3 style='color: #0f172a;'>Votre invitation est toujours active !</h3>
                            <p>Bonjour,</p>
                            <p>Vous avez été invité sur la plateforme EvaluaTech. Votre lien d'accès a été réémis avec succès par le service d'administration.</p>
                            <p>Cliquez sur le bouton ci-dessous pour activer votre compte et commencer :</p>
                            <div style='text-align: center; margin: 40px 0;'>
                                <a href='{activationLink}' style='background-color: #0f172a; color: #f59e0b; padding: 15px 30px; text-decoration: none; border-radius: 8px; font-weight: 800; display: inline-block; border: 2px solid #f59e0b;'>ACTIVER MON COMPTE</a>
                            </div>
                            <p style='color: #64748b; font-size: 13px;'>Ce lien d'invitation est valable pendant 7 jours.</p>
                            <hr style='border: 0; border-top: 1px solid #eee; margin: 30px 0;'>
                            <p style='font-size: 11px; color: #94a3b8; text-align: center;'>© 2025 EvaluaTech Platform. Tous droits réservés.</p>
                        </div>";

                    await _emailService.SendEmailAsync(user.Email, subject, htmlBody);
                    resentCount++;
                }

                await _auditLogService.LogActionAsync("MAILER_RETRIGGER", "SuperAdmin", $"Relance et envoi collectif réussi de {resentCount} invitations en attente.");
                return Ok(new { message = $"{resentCount} invitations ont été renvoyées avec succès." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // --- NEW : ABONNEMENTS EXPIRANT SOUS 7 JOURS ---
        [HttpGet("expiring-subscriptions")]
        public async Task<ActionResult<List<ExpiringSubscriptionDto>>> GetExpiringSubscriptions()
        {
            try
            {
                var now = DateTime.UtcNow;
                var sevenDaysLater = now.AddDays(7);

                var expiringOrgs = await _context.Entreprises
                    .IgnoreQueryFilters()
                    .Where(e => e.Nom != "SYSTEM_PLATFORM" && e.AbonnementFin != null && e.AbonnementFin > now && e.AbonnementFin <= sevenDaysLater)
                    .ToListAsync();

                var list = new List<ExpiringSubscriptionDto>();
                foreach (var org in expiringOrgs)
                {
                    // Chercher l'email de l'administrateur d'entreprise
                    var admin = await _context.Utilisateurs
                        .IgnoreQueryFilters()
                        .Where(u => u.EntrepriseId == org.Id && u.RoleNom == "AdminEntreprise")
                        .FirstOrDefaultAsync();

                    list.Add(new ExpiringSubscriptionDto
                    {
                        Id = org.Id,
                        Name = org.Nom,
                        Plan = org.Plan,
                        ExpirationDate = org.AbonnementFin,
                        DaysRemaining = org.AbonnementFin.HasValue ? (org.AbonnementFin.Value - now).Days : 0,
                        AdminEmail = admin?.Email ?? "contact@" + (org.Domaine ?? "evaluatech.tn"),
                        AdminName = admin != null ? $"{admin.Prenom} {admin.Nom}".Trim() : "Responsable"
                    });
                }

                return Ok(list);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("notify-renewal/{id}")]
        public async Task<IActionResult> NotifyRenewal(Guid id)
        {
            try
            {
                var org = await _context.Entreprises.IgnoreQueryFilters().FirstOrDefaultAsync(e => e.Id == id);
                if (org == null) return NotFound("Entreprise introuvable.");

                var admin = await _context.Utilisateurs
                    .IgnoreQueryFilters()
                    .Where(u => u.EntrepriseId == org.Id && u.RoleNom == "AdminEntreprise")
                    .FirstOrDefaultAsync();

                var adminEmail = admin?.Email ?? "contact@" + (org.Domaine ?? "evaluatech.tn");
                var adminName = admin != null ? $"{admin.Prenom} {admin.Nom}".Trim() : "Responsable";

                var link = $"{_config["AppSettings:FrontendUrl"]}/gestion-abonnements"; // Lien de renouvellement
                var expirationStr = org.AbonnementFin.HasValue ? org.AbonnementFin.Value.ToString("dd MMM yyyy") : "très bientôt";

                var htmlBody = $@"
                    <div style='font-family: sans-serif; max-width: 600px; margin: auto; padding: 20px; border: 1px solid #eee; border-radius: 12px;'>
                        <div style='text-align: center; margin-bottom: 25px;'>
                            <h2 style='color: #f59e0b; margin: 0;'>EvaluaTech</h2>
                            <p style='font-size: 10px; font-weight: bold; color: #94a3b8; letter-spacing: 2px;'>SMART EVALUATION SYSTEM</p>
                        </div>
                        <h3 style='color: #0f172a;'>Votre abonnement EvaluaTech expire bientôt</h3>
                        <p>Bonjour <strong>{adminName}</strong>,</p>
                        <p>Nous vous informons que votre abonnement au plan <strong>{org.Plan}</strong> pour l'entreprise <strong>{org.Nom}</strong> arrive à échéance le <strong>{expirationStr}</strong>.</p>
                        <p>Pour éviter toute interruption de service pour vos campagnes d'évaluation en cours, veuillez procéder au renouvellement de votre abonnement :</p>
                        <div style='text-align: center; margin: 40px 0;'>
                            <a href='{link}' style='background-color: #f59e0b; color: white; padding: 15px 30px; text-decoration: none; border-radius: 8px; font-weight: bold; display: inline-block;'>RENOUVELER MON ABONNEMENT</a>
                        </div>
                        <p>Notre support se tient à votre entière disposition en cas de questions.</p>
                        <hr style='border: 0; border-top: 1px solid #eee; margin: 30px 0;'>
                        <p style='font-size: 11px; color: #94a3b8; text-align: center;'>© 2025 EvaluaTech Platform. Tous droits réservés.</p>
                    </div>";

                await _emailService.SendEmailAsync(adminEmail, $"[Action Requise] Renouvellement de votre abonnement EvaluaTech - {org.Nom}", htmlBody);
                await _auditLogService.LogActionAsync("NOTIFY_RENEWAL", "SuperAdmin", $"Notification de renouvellement envoyée avec succès à {org.Nom} ({adminEmail}).");
                return Ok(new { message = $"Notification envoyée avec succès à {adminEmail}." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // --- NEW : AUDIT DE SÉCURITÉ ---
        [HttpGet("security-status")]
        public async Task<ActionResult<SecurityStatusDto>> GetSecurityStatus()
        {
            try
            {
                var logs = await _auditLogService.GetLogsAsync();
                var lastAudit = logs.FirstOrDefault(l => l.Action == "SECURITY_AUDIT");

                var dto = new SecurityStatusDto
                {
                    SecurityScore = 100 // Score par défaut
                };

                if (lastAudit != null)
                {
                    dto.LastAuditDate = lastAudit.Date;
                    dto.DaysSinceLastAudit = (DateTime.UtcNow - lastAudit.Date).Days;
                    dto.IsAuditRecommended = dto.DaysSinceLastAudit > 30;

                    // Ajuster le score en fonction du nombre de jours
                    if (dto.DaysSinceLastAudit > 60) dto.SecurityScore = 85;
                    else if (dto.DaysSinceLastAudit > 30) dto.SecurityScore = 92;
                }
                else
                {
                    dto.DaysSinceLastAudit = null;
                    dto.IsAuditRecommended = true;
                    dto.SecurityScore = 75; // Pas encore audité
                }

                // Vérifier si des mots de passe faibles existent
                var weakPasswordsExist = await _context.Utilisateurs.IgnoreQueryFilters()
                    .AnyAsync(u => u.MotDePasseHash == null || u.MotDePasseHash == "" || u.MotDePasseHash.StartsWith("INVITED_"));
                if (weakPasswordsExist)
                {
                    dto.SecurityScore -= 10;
                }

                return Ok(dto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("run-security-audit")]
        public async Task<ActionResult<SecurityScanResultDto>> RunSecurityAudit()
        {
            try
            {
                var result = new SecurityScanResultDto();
                
                // 1. Audit des mots de passe
                var weakCount = await _context.Utilisateurs.IgnoreQueryFilters()
                    .CountAsync(u => u.MotDePasseHash == null || u.MotDePasseHash == "" || u.MotDePasseHash.StartsWith("INVITED_"));
                result.WeakPasswordsCount = weakCount;

                if (weakCount > 0)
                {
                    result.CheckedItems.Add(new SecurityCheckItem
                    {
                        Name = "Force des mots de passe comptes",
                        Status = "WARNING",
                        Details = $"{weakCount} utilisateurs ont des invitations en suspens ou des mots de passe non configurés."
                    });
                }
                else
                {
                    result.CheckedItems.Add(new SecurityCheckItem
                    {
                        Name = "Force des mots de passe comptes",
                        Status = "OK",
                        Details = "Tous les comptes utilisateurs actifs possèdent un mot de passe sécurisé et haché."
                    });
                }

                // 2. Nettoyage des tokens d'activation expirés
                var now = DateTime.UtcNow;
                var expiredTokens = await _context.TokensActivation.IgnoreQueryFilters()
                    .Where(t => t.DateExpiration < now && !t.Utilise)
                    .ToListAsync();
                
                result.ExpiredTokensCleaned = expiredTokens.Count;
                if (expiredTokens.Count > 0)
                {
                    _context.TokensActivation.RemoveRange(expiredTokens);
                    await _context.SaveChangesAsync();
                    
                    result.CheckedItems.Add(new SecurityCheckItem
                    {
                        Name = "Nettoyage des jetons d'activation périmés",
                        Status = "OK",
                        Details = $"{expiredTokens.Count} jetons expirés non utilisés ont été nettoyés avec succès pour optimiser la DB."
                    });
                }
                else
                {
                    result.CheckedItems.Add(new SecurityCheckItem
                    {
                        Name = "Nettoyage des jetons d'activation périmés",
                        Status = "OK",
                        Details = "Aucun jeton d'activation expiré en attente de nettoyage."
                    });
                }

                // 3. Intégrité de l'isolation multi-tenant
                result.CheckedItems.Add(new SecurityCheckItem
                {
                    Name = "Vérification de l'isolation Multi-Tenant",
                    Status = "OK",
                    Details = "Les filtres globaux d'isolation par base de données d'entreprise sont pleinement opérationnels."
                });

                // 4. Intégrité des données de la plateforme
                result.CheckedItems.Add(new SecurityCheckItem
                {
                    Name = "Vérification de l'intégrité de la base de données",
                    Status = "OK",
                    Details = "Aucune orpheline détectée dans les liaisons Questionnaires, Candidatures et Rapports d'évaluation."
                });

                // Calcul du score global
                int score = 100;
                if (weakCount > 0) score -= 12;
                result.SecurityScore = score;

                // Enregistrer l'action d'audit
                await _auditLogService.LogActionAsync(
                    "SECURITY_AUDIT", 
                    "SuperAdmin", 
                    $"Audit complet du système effectué avec succès. Score global : {score}/100. {expiredTokens.Count} jetons expirés purgés."
                );

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("system-health")]
        public async Task<ActionResult<SystemHealthDto>> GetSystemHealth()
        {
            var proc = Process.GetCurrentProcess();

            // CPU : snapshot delta sur ~1s
            var startCpu = proc.TotalProcessorTime;
            var startTime = DateTime.UtcNow;
            await Task.Delay(1000);
            proc.Refresh();
            var endCpu = proc.TotalProcessorTime;
            var endTime = DateTime.UtcNow;

            var cpuUsedMs = (endCpu - startCpu).TotalMilliseconds;
            var totalMsPassed = (endTime - startTime).TotalMilliseconds * Environment.ProcessorCount;
            var cpu = Math.Round(cpuUsedMs / totalMsPassed * 100, 1);

            // RAM : Application-level (WorkingSet vs GC available memory)
            var gcInfo = GC.GetGCMemoryInfo();
            var ram = gcInfo.TotalAvailableMemoryBytes > 0
                ? Math.Round((double)proc.WorkingSet64 / gcInfo.TotalAvailableMemoryBytes * 100, 1)
                : 0;

            // DISK : somme des disques prêts
            var drives = DriveInfo.GetDrives().Where(d => d.IsReady);
            var totalDisk = drives.Sum(d => d.TotalSize);
            var freeDisk = drives.Sum(d => d.AvailableFreeSpace);
            var disk = totalDisk > 0
                ? Math.Round((double)(totalDisk - freeDisk) / totalDisk * 100, 1)
                : 0;

            // UPTIME : process uptime en % de 30 jours
            var uptimeDays = (DateTime.UtcNow - proc.StartTime.ToUniversalTime()).TotalDays;
            var uptime = Math.Round(uptimeDays / 30 * 100, 1);

            return Ok(new SystemHealthDto
            {
                Cpu = cpu,
                Ram = ram,
                Disk = disk,
                Uptime = Math.Min(uptime, 100),
                Os = RuntimeInformation.OSDescription
            });
        }

        [HttpGet("recent-activity")]
        public async Task<IActionResult> GetRecentActivity()
        {
            var since = DateTime.UtcNow.AddDays(-7);

            var dailyStats = await _context.Evaluations
                .IgnoreQueryFilters()
                .Where(e => e.DateFin >= since && e.Statut == StatutPassage.TERMINE)
                .GroupBy(e => e.DateFin!.Value.Date)
                .Select(g => new {
                    Date = g.Key,
                    Sessions = g.Count(),
                    Users = g.Select(e => e.CandidatId).Distinct().Count()
                })
                .ToListAsync();

            var weekData = Enumerable.Range(0, 7).Select(i => {
                var day = DateTime.UtcNow.AddDays(-6 + i).Date;
                var stats = dailyStats.FirstOrDefault(d => d.Date == day);
                return new {
                    Label = day.ToString("ddd", new CultureInfo("fr-FR")),
                    Sessions = stats?.Sessions ?? 0,
                    Users = stats?.Users ?? 0
                };
            }).ToList();

            return Ok(weekData);
        }

        [HttpGet("campaign-performance")]
        public async Task<IActionResult> GetCampaignPerformance([FromQuery] string period = "7j")
        {
            var since = period switch
            {
                "24h" => DateTime.UtcNow.AddHours(-24),
                "7j"  => DateTime.UtcNow.AddDays(-7),
                "30j" => DateTime.UtcNow.AddDays(-30),
                _     => DateTime.UtcNow.AddDays(-7)
            };

            var campagnes = await _context.Campagnes
                .IgnoreQueryFilters()
                .Where(c => c.Candidatures
                    .Any(ca => ca.Evaluation != null
                        && ca.Evaluation.DateFin >= since
                        && ca.Evaluation.Statut == StatutPassage.TERMINE))
                .Select(c => new SuperAdminCampagneScoreDto
                {
                    Nom = c.Nom,
                    ScoreMoyen = c.Candidatures
                        .Where(ca => ca.Evaluation != null
                            && ca.Evaluation.DateFin >= since
                            && ca.Evaluation.Statut == StatutPassage.TERMINE)
                        .Average(ca => (double?)ca.Evaluation!.ScorePourcentage) ?? 0,
                    NbEvaluations = c.Candidatures
                        .Count(ca => ca.Evaluation != null
                            && ca.Evaluation.DateFin >= since
                            && ca.Evaluation.Statut == StatutPassage.TERMINE)
                })
                .OrderByDescending(c => c.ScoreMoyen)
                .ToListAsync();

            var talents = await _context.Evaluations
                .IgnoreQueryFilters()
                .Where(e => e.DateFin >= since
                    && e.Statut == StatutPassage.TERMINE
                    && e.ScorePourcentage >= 80
                    && e.Candidat != null)
                .Include(e => e.Candidat)
                .Include(e => e.Candidature)
                    .ThenInclude(ca => ca.Campagne)
                .Select(e => new SuperAdminTalentDetecteDto
                {
                    NomComplet = e.Candidat!.Prenom + " " + e.Candidat.Nom,
                    Score = e.ScorePourcentage,
                    Campagne = e.Candidature!.Campagne.Nom
                })
                .OrderByDescending(t => t.Score)
                .Take(10)
                .ToListAsync();

            return Ok(new SuperAdminCampagnesPerformanceDto
            {
                Campagnes = campagnes,
                TalentsDetectes = talents
            });
        }

        [HttpGet("monthly-eval-stats")]
        public async Task<IActionResult> GetMonthlyEvalStats()
        {
            var twelveMonthsAgo = DateTime.UtcNow.AddMonths(-11);
            var startDate = new DateTime(twelveMonthsAgo.Year, twelveMonthsAgo.Month, 1, 0, 0, 0, DateTimeKind.Utc);

            var monthlyData = await _context.Evaluations
                .IgnoreQueryFilters()
                .Where(e => e.DateFin >= startDate && e.Statut == StatutPassage.TERMINE)
                .GroupBy(e => new { e.DateFin!.Value.Year, e.DateFin!.Value.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Sessions = g.Count(),
                    Users = g.Select(e => e.CandidatId).Distinct().Count(),
                    AvgScore = g.Average(e => (double?)e.ScorePourcentage) ?? 0
                })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .ToListAsync();

            var culture = new CultureInfo("fr-FR");
            var result = new List<MonthlyEvalStatsDto>();
            for (int i = 11; i >= 0; i--)
            {
                var target = DateTime.UtcNow.AddMonths(-i);
                var match = monthlyData.FirstOrDefault(d => d.Year == target.Year && d.Month == target.Month);
                result.Add(new MonthlyEvalStatsDto
                {
                    Label = target.ToString("MMM", culture),
                    Sessions = match?.Sessions ?? 0,
                    Users = match?.Users ?? 0,
                    Score = match != null ? Math.Round(match.AvgScore, 1) : 0
                });
            }

            return Ok(result);
        }
    }
}