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

                    // 3. Générer le Token d'activation
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

                    // Sauvegarde en base avant l'envoi de l'email
                    await _context.SaveChangesAsync();

                    // 4. Préparation et envoi de l'email via le service
                    string activationLink = $"http://localhost:5173/activer?token={token.Token}";
                    
                    string subject = $"Invitation : {campagne.Nom}";
                    string htmlBody = $@"
                        <div style='font-family: Arial, sans-serif; border: 1px solid #eee; padding: 20px;'>
                            <h2 style='color: #f59e0b;'>Invitation EvaluaTech</h2>
                            <p>Bonjour,</p>
                            <p>Vous avez été invité à passer une évaluation pour la campagne : <strong>{campagne.Nom}</strong>.</p>
                            <p>Pour commencer, veuillez activer votre compte et définir votre mot de passe en cliquant sur le bouton ci-dessous :</p>
                            <div style='text-align: center; margin: 30px 0;'>
                                <a href='{activationLink}' style='background-color: #f59e0b; color: white; padding: 12px 25px; text-decoration: none; border-radius: 5px; font-weight: bold;'>Activer mon compte</a>
                            </div>
                            <p style='color: #888; font-size: 12px;'>Ce lien est valable pendant 7 jours.</p>
                        </div>";

                    await _emailService.SendEmailAsync(email, subject, htmlBody);
                    sentCount++;
                }
                catch (Exception ex)
                {
                    // On log l'erreur mais on continue la boucle pour les autres candidats
                    Console.WriteLine($"Erreur lors de l'envoi à {email}: {ex.Message}");
                    errorCount++;
                }
            }

            return Ok(new { 
                message = $"{sentCount} invitations envoyées avec succès.",
                errors = errorCount > 0 ? $"{errorCount} échecs d'envoi." : null
            });
        }

        [HttpGet("campagnes")]
        public async Task<IActionResult> GetCampagnes() {
            return Ok(await _context.Campagnes.Select(c => new { c.Id, Titre = c.Nom }).ToListAsync());
        }
    }
}