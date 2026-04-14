using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NeoEvaluation.API.Data;
using NeoEvaluation.API.Models;
using NeoEvaluation.API.Services;
using NeoEvaluation.API.Dtos;

namespace NeoEvaluation.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvitationsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService; // Injection du service

        public InvitationsController(AppDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        [HttpPost("invite-candidates")]
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
                    var candidat = await _context.Candidats.FirstOrDefaultAsync(u => u.Email == email);
                    if (candidat == null)
                    {
                        candidat = new Candidat { 
                            Id = Guid.NewGuid(), 
                            Email = email, 
                            RoleNom = "Candidat", 
                            EstActif = false 
                        };
                        _context.Candidats.Add(candidat);
                    }

                    // 2. Créer la candidature si elle n'existe pas
                    var exists = await _context.Candidatures.AnyAsync(c => c.CandidatId == candidat.Id && c.CampagneId == request.CampagneId);
                    if (!exists)
                    {
                        _context.Candidatures.Add(new Candidature { 
                            Id = Guid.NewGuid(), 
                            CandidatId = candidat.Id, 
                            CampagneId = request.CampagneId,
                            Statut = ApplicationStatus.POSTULE 
                        });
                    }

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

                    // 4. Préparation du lien et de l'email
                    string activationLink = $"http://localhost:5173/activate-role?token={token.Token}";
                    
                    // ✅ DEBUG TERMINAL (Pour toi en VS Code)
                    Console.WriteLine("\n--------------------------------------------------");
                    Console.WriteLine($"[DEBUG] CANDIDATE TEST LINK: {email}");
                    Console.WriteLine($"CAMPAGNE: {campagne.Nom}");
                    Console.WriteLine($"LINK: {activationLink}");
                    Console.WriteLine("--------------------------------------------------\n");

                    // ✅ PROFESSIONAL HTML TEMPLATE
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

        [HttpPost("invite-staff")]
        public async Task<IActionResult> InviteStaff([FromBody] StaffInvitationDto request)
        {
            if (string.IsNullOrEmpty(request.Email))
                return BadRequest("L'adresse e-mail est obligatoire.");

            try 
            {
                // 1. Vérifier si l'utilisateur existe déjà
                var existingUser = await _context.Utilisateurs.FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower());
                if (existingUser != null)
                {
                    return BadRequest("Un utilisateur avec cet email existe déjà.");
                }

                // 2. Récupérer le rôle réel depuis la DB pour avoir son ID
                var dbRole = await _context.Roles.FirstOrDefaultAsync(r => r.Nom == request.Role);

                // 2.5 Auto-résolution de l'EntrepriseId si manquant (HACK pour éviter le re-login)
                var currentEntrepriseId = request.EntrepriseId;
                if (!currentEntrepriseId.HasValue || currentEntrepriseId == Guid.Empty)
                {
                    // On cherche l'entreprise de l'admin qui fait l'action (via son propre compte)
                    // Note: Dans un environnement PRO, on utiliserait User.Identity, ici on simplifie
                    var authAdmin = await _context.Utilisateurs.FirstOrDefaultAsync(u => u.RoleNom == "AdminEntreprise");
                    currentEntrepriseId = authAdmin?.EntrepriseId;
                }

                // 3. Créer le membre du personnel
                var personnel = new Personnel 
                { 
                    Id = Guid.NewGuid(), 
                    Email = request.Email.ToLower(),
                    Prenom = request.Prenom,
                    Nom = request.NomFamille,
                    RoleNom = request.Role, 
                    RoleId = dbRole?.Id,
                    EntrepriseId = currentEntrepriseId,
                    EstActif = false,
                    MotDePasseHash = "PENDING_INVITATION_" + Guid.NewGuid().ToString("N") // Satisfait le NOT NULL
                };
                
                _context.Utilisateurs.Add(personnel);

                // 4. Générer le Token d'activation
                var token = new TokensActivation {
                    Id = Guid.NewGuid(), 
                    Token = Guid.NewGuid(), 
                    UtilisateurId = personnel.Id,
                    Email = request.Email.ToLower(), 
                    DateCreation = DateTime.UtcNow,
                    DateExpiration = DateTime.UtcNow.AddDays(7), 
                    Utilise = false,
                    IdInscription = Guid.Empty // 🔥 CRITIQUE : Satisfait la contrainte NOT NULL de la DB
                };
                _context.TokensActivation.Add(token);

                await _context.SaveChangesAsync();

                // 5. Préparation du lien et de l'email
                string activationLink = $"http://localhost:5173/activate-role?token={token.Token}";
                
                Console.WriteLine("\n--------------------------------------------------");
                Console.WriteLine($"[DEBUG] STAFF INVITATION LINK: {request.Email}");
                Console.WriteLine($"ROLE: {request.Role}");
                Console.WriteLine($"LINK: {activationLink}");
                Console.WriteLine("--------------------------------------------------\n");

                string subject = $"Invitation : Rejoindre l'équipe EvaluaTech";
                string htmlBody = $@"
                    <div style='font-family: sans-serif; max-width: 600px; margin: auto; padding: 20px; border: 1px solid #eee; border-radius: 12px;'>
                        <div style='text-align: center; margin-bottom: 25px;'>
                            <h2 style='color: #f59e0b; margin: 0;'>EvaluaTech</h2>
                            <p style='font-size: 10px; font-weight: bold; color: #94a3b8; letter-spacing: 2px;'>SMART EVALUATION SYSTEM</p>
                        </div>
                        <h3 style='color: #0f172a;'>Bienvenue dans l'équipe !</h3>
                        <p>Bonjour {request.Prenom},</p>
                        <p>Vous avez été invité à rejoindre la plateforme EvaluaTech en tant que <strong>{request.Role}</strong>.</p>
                        <p>Cliquez sur le bouton ci-dessous pour activer votre compte et configurer votre mot de passe :</p>
                        <div style='text-align: center; margin: 40px 0;'>
                            <a href='{activationLink}' style='background-color: #0f172a; color: #f59e0b; padding: 15px 30px; text-decoration: none; border-radius: 8px; font-weight: 800; display: inline-block; border: 2px solid #f59e0b;'>ACTIVER MON COMPTE</a>
                        </div>
                        <p style='color: #64748b; font-size: 13px;'>Ce lien est valable pendant 7 jours.</p>
                        <hr style='border: 0; border-top: 1px solid #eee; margin: 30px 0;'>
                        <p style='font-size: 11px; color: #94a3b8; text-align: center;'>© 2025 EvaluaTech Platform. Tous droits réservés.</p>
                    </div>";

                await _emailService.SendEmailAsync(request.Email, subject, htmlBody);

                return Ok(new { message = "Invitation envoyée avec succès au membre du personnel." });
            }
            catch (Exception ex)
            {
                var fullMessage = ex.InnerException != null 
                    ? $"{ex.Message} --> {ex.InnerException.Message}" 
                    : ex.Message;
                return BadRequest(new { message = "Erreur lors de l'invitation : " + fullMessage });
            }
        }

        [HttpGet("campagnes")]
        public async Task<IActionResult> GetCampagnes() {
            // Fix: On renvoie "Nom" et "Id" clairement pour le select
            return Ok(await _context.Campagnes
                .OrderByDescending(c => c.CreeLe)
                .Select(c => new { c.Id, Nom = c.Nom })
                .ToListAsync());
        }
    }
}