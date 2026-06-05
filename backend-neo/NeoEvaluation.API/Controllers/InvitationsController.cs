using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using NeoEvaluation.API.Data;
using NeoEvaluation.API.Models;
using NeoEvaluation.API.Services;
using NeoEvaluation.API.Dtos;
using NeoEvaluation.API.Attributes;

namespace NeoEvaluation.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class InvitationsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService; // Injection du service
        private readonly ITenantService _tenantService;

        public InvitationsController(AppDbContext context, IEmailService emailService, ITenantService tenantService)
        {
            _context = context;
            _emailService = emailService;
            _tenantService = tenantService;
        }

        [HttpPost("invite-candidates")]
        [RequirePermission("inv_can")]
        public async Task<IActionResult> InviteCandidates([FromBody] InvitationRequestDto request)
        {
            if (request.Emails == null || !request.Emails.Any())
                return BadRequest("La liste d'adresses e-mail est vide.");

            var campagne = await _context.Campagnes.FindAsync(request.CampagneId);
            if (campagne == null) return NotFound("Campagne d'évaluation introuvable.");

            int sentCount = 0;
            int errorCount = 0;

            foreach (var email in request.Emails)
            {
                try 
                {
                    // 1. Gérer l'utilisateur (Candidat)
                    var currentEntId = _tenantService.GetTenantId();
                    var candidat = await _context.Utilisateurs.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Email == email);
                    
                    if (candidat == null)
                    {
                        candidat = new Utilisateur { 
                            Id = Guid.NewGuid(), 
                            Email = email, 
                            RoleNom = "Candidat", 
                            EstActif = false,
                            EntrepriseId = currentEntId
                        };
                        _context.Utilisateurs.Add(candidat);
                    }
                    else if (candidat.EntrepriseId == null)
                    {
                        candidat.EntrepriseId = currentEntId;
                    }

                    // 2. Créer la candidature si elle n'existe pas
                    var exists = await _context.Candidatures.IgnoreQueryFilters()
                        .AnyAsync(c => c.CandidatId == candidat.Id && c.CampagneId == request.CampagneId);
                    if (!exists)
                    {
                        _context.Candidatures.Add(new Candidature { 
                            Id = Guid.NewGuid(), 
                            CandidatId = candidat.Id, 
                            CampagneId = request.CampagneId,
                            Statut = ApplicationStatus.POSTULE 
                        });
                    }

                    // 3. Sauvegarder les modifications (Candidature, Utilisateur, etc.)
                    await _context.SaveChangesAsync();

                    string activationLink;

                    if (candidat.EstActif)
                    {
                        // Le candidat est déjà enregistré et actif. Pas besoin de recréer de mot de passe.
                        activationLink = "http://localhost:5173/login";
                    }
                    else
                    {
                        // 3. Générer le Token d'activation (pour le test)
                        var token = new TokensActivation {
                            Id = Guid.NewGuid(), 
                            Token = Guid.NewGuid(), 
                            UtilisateurId = candidat.Id,
                            Email = email, 
                            DateCreation = DateTime.UtcNow,
                            DateExpiration = DateTime.UtcNow.AddDays(7), 
                            Utilise = false
                        };
                        _context.TokensActivation.Add(token);
                        await _context.SaveChangesAsync();

                        // 4. Préparation du lien
                        activationLink = $"http://localhost:5173/activate-account?token={token.Token}";
                    }
                    
                    //  DEBUG TERMINAL (Pour toi en VS Code)
                    Console.WriteLine("\n--------------------------------------------------");
                    Console.WriteLine($"[DEBUG] CANDIDATE TEST LINK: {email}");
                    Console.WriteLine($"CAMPAGNE: {campagne.Nom}");
                    Console.WriteLine($"LINK: {activationLink}");
                    Console.WriteLine("--------------------------------------------------\n");

                    //  PROFESSIONAL HTML TEMPLATE
                    string subject = $"Invitation : Evaluation {campagne.Nom}";
                    string htmlBody = $@"
                        <div style='font-family: sans-serif; max-width: 600px; margin: auto; padding: 20px; border: 1px solid #eee; border-radius: 12px;'>
                            <div style='text-align: center; margin-bottom: 25px;'>
                                <h2 style='color: #f59e0b; margin: 0;'>EvaluaTech</h2>
                                <p style='font-size: 10px; font-weight: bold; color: #94a3b8; letter-spacing: 2px;'>SMART EVALUATION SYSTEM</p>
                            </div>
                            <h3 style='color: #0f172a;'>Invitation à une évaluation</h3>
                            <p>Bonjour,</p>
                            <p>Vous avez été invité à passer une évaluation en ligne pour la campagne : <strong>{campagne.Nom}</strong>.</p>
                            <p>Cliquez sur le bouton ci-dessous pour accéder à votre espace de test et commencer l'examen :</p>
                            <div style='text-align: center; margin: 40px 0;'>
                                <a href='{activationLink}' style='background-color: #0f172a; color: #f59e0b; padding: 15px 30px; text-decoration: none; border-radius: 8px; font-weight: 800; display: inline-block; border: 2px solid #f59e0b;'>COMMENCER L'ÉVALUATION</a>
                            </div>
                            <p style='color: #64748b; font-size: 13px;'>Note : Assurez-vous d'être dans un endroit calme avec une connexion stable.</p>
                            <hr style='border: 0; border-top: 1px solid #eee; margin: 30px 0;'>
                            <p style='font-size: 11px; color: #94a3b8; text-align: center;'>© 2025 EvaluaTech Platform. Tous droits réservés.</p>
                        </div>";

                    await _emailService.SendEmailAsync(email, subject, htmlBody);
                    sentCount++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] Envoi échoué à {email}: {ex.Message}");
                    errorCount++;
                }
            }

            return Ok(new { 
                message = $"{sentCount} invitations envoyées.",
                errors = errorCount
            });
        }


        [HttpGet("campagnes")]
        public async Task<IActionResult> GetCampagnes() {
            var now = DateTime.UtcNow;
            return Ok(await _context.Campagnes
                .Where(c => c.DateFin > now && (c.Statut == StatutCampagne.EN_COURS || c.Statut == StatutCampagne.PLANIFIEE))
                .OrderByDescending(c => c.CreeLe)
                .Select(c => new { c.Id, Nom = c.Nom })
                .ToListAsync());
        }

        [HttpGet("success-rate")]
        public async Task<IActionResult> GetSuccessRate()
        {
            var entId = _tenantService.GetTenantId();
            if (entId == null) return BadRequest(new { message = "Tenant ID introuvable." });

            var sentCount = await _context.Candidatures
                .CountAsync(c => c.Campagne.EntrepriseId == entId);

            var completedCount = await _context.Evaluations
                .CountAsync(e => e.Candidature.Campagne.EntrepriseId == entId
                              && e.Statut == StatutPassage.TERMINE);

            var successRate = sentCount > 0
                ? (int)Math.Round((double)completedCount / sentCount * 100)
                : 0;

            return Ok(new { sentCount, completedCount, successRate });
        }
    }
}