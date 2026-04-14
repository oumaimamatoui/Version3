using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NeoEvaluation.API.Data;
using NeoEvaluation.API.DTOs;
using NeoEvaluation.API.Models;
using NeoEvaluation.API.Services;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Google.Apis.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeoEvaluation.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public AuthController(AppDbContext context, IConfiguration configuration, IEmailService emailService)
        {
            _context = context;
            _configuration = configuration;
            _emailService = emailService;
        }

        // --- 1. LOGIN CLASSIQUE (Email / Mot de passe) ---
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (loginDto == null || string.IsNullOrEmpty(loginDto.Email))
                return BadRequest(new { message = "Données de connexion invalides." });

            // Nettoyage des entrées
            var email = loginDto.Email.Trim().ToLower();
            var password = loginDto.Password; // Ne pas utiliser Trim() ici pour autoriser les mots de passe avec espace

            Console.WriteLine($"[AUTH DEBUG] Tentative de connexion pour l'email: '{email}'");
            Console.WriteLine($"[AUTH DEBUG] Longueur du mot de passe saisi : {password?.Length ?? 0}");

            var allUsers = await _context.Utilisateurs.Where(u => u.Email.ToLower() == email).ToListAsync();
            Console.WriteLine($"[AUTH DEBUG] Tentative pour '{email}' - {allUsers.Count} comptes trouvés.");

            Utilisateur? user = null;
            foreach (var u in allUsers)
            {
                Console.WriteLine($"[AUTH DEBUG] Test du mot de passe sur le compte ID: {u.Id} (Type: {u.GetType().Name})...");
                
                if (string.IsNullOrEmpty(u.MotDePasseHash)) continue;

                try {
                    if (BCrypt.Net.BCrypt.Verify(password, u.MotDePasseHash))
                    {
                        user = u;
                        Console.WriteLine($"[AUTH DEBUG] SUCCÈS : Correspondance trouvée pour le compte {u.Id} !");
                        break;
                    }
                } catch (Exception ex) { 
                    Console.WriteLine($"[AUTH DEBUG] Erreur BCrypt sur le compte {u.Id} : {ex.Message}");
                }
            }
            
            if (user == null) 
            {
                Console.WriteLine($"[AUTH DEBUG] ÉCHEC : Aucun des {allUsers.Count} comptes ne correspond à ce mot de passe.");
                return Unauthorized(new { message = "Email ou mot de passe incorrect." });
            }

            if (!user.EstActif) 
            {
                Console.WriteLine($"[AUTH DEBUG] Compte trouvé mais INACTIF : {user.Id}");
                return BadRequest(new { message = "Votre compte est désactivé. Veuillez contacter l'administrateur." });
            }

            Console.WriteLine($"[AUTH DEBUG] Connexion RÉUSSIE pour '{email}'");
            return Ok(GenerateJwtResponse(user));
        }

        // --- 2. LOGIN VIA GOOGLE ---
        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginDto googleDto)
        {
            try 
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings() {
                    Audience = new List<string>() { _configuration["GoogleAuthSettings:ClientId"]! }
                };

                // Validation du Token Google
                var payload = await GoogleJsonWebSignature.ValidateAsync(googleDto.IdToken, settings);
                
                // Recherche de l'utilisateur par l'email renvoyé par Google
                var user = await _context.Utilisateurs.FirstOrDefaultAsync(u => u.Email == payload.Email.ToLower());

                if (user == null) 
                    return BadRequest(new { message = "Compte introuvable. Veuillez d'abord vous inscrire via le formulaire." });

                if (!user.EstActif) 
                    return BadRequest(new { message = "Votre compte est en attente d'approbation par le SuperAdmin." });

                return Ok(GenerateJwtResponse(user));
            }
            catch (Exception ex) 
            {
                return Unauthorized(new { message = "Échec de l'authentification Google : " + ex.Message });
            }
        }

        // --- 3. GÉNÉRATION DU TOKEN JWT ET RÉPONSE JSON ---
        private object GenerateJwtResponse(Utilisateur user) 
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var secretKey = _configuration["JwtSettings:SecretKey"];
            
            if (string.IsNullOrEmpty(secretKey))
                throw new Exception("La clé secrète JWT est absente de la configuration (appsettings.json).");

            var key = Encoding.UTF8.GetBytes(secretKey);

            // Gestion sécurisée du rôle
            string userRole = string.IsNullOrEmpty(user.RoleNom) ? "Candidat" : user.RoleNom;
            
            // Correction pour le frontend : Admin -> AdminEntreprise
            if (userRole == "Admin") userRole = "AdminEntreprise";

            // Préparation des Claims (Identité de l'utilisateur)
            var claims = new List<Claim> {
                new Claim("id", user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, userRole),
                new Claim("nom", user.NomComplet ?? "")
            };

            // AJOUT DES PRIVILÈGES (Utilise votre nouvelle List<string> Privileges)
            if (user.Privileges != null && user.Privileges.Any())
            {
                foreach (var privilege in user.Privileges)
                {
                    claims.Add(new Claim("privilege", privilege));
                }
            }

            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(1), // Le token expire après 24h
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            
            // Retourne un objet JSON propre pour le frontend
            return new { 
                Token = tokenHandler.WriteToken(token), 
                Nom = user.NomComplet, 
                Role = userRole,
                Email = user.Email,
                Photo = user.PhotoUrl,
                EntrepriseId = user.EntrepriseId
            };
        }

        // --- 4. MOT DE PASSE OUBLIÉ ---
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            Console.WriteLine($"\n[DEBUG] Requête ForgotPassword reçue pour : {dto.Email}");

            var user = await _context.Utilisateurs.FirstOrDefaultAsync(u => u.Email.ToLower() == dto.Email.ToLower());
            if (user == null)
            {
                Console.WriteLine($"[DEBUG] Utilisateur non trouvé pour l'email : {dto.Email}");
                return Ok(new { message = "Si cet email existe, un lien de réinitialisation a été envoyé." });
            }

            // Création d'un token de réinitialisation
            var resetToken = new TokensActivation
            {
                UtilisateurId = user.Id,
                Email = user.Email,
                Token = Guid.NewGuid(),
                DateExpiration = DateTime.UtcNow.AddHours(2),
                Utilise = false
            };

            _context.TokensActivation.Add(resetToken);
            await _context.SaveChangesAsync();

            // Envoi de l'email
            var resetLink = $"{_configuration["AppSettings:FrontendUrl"]}/reset-password?token={resetToken.Token}";

            // ✅ LOG CONSOLE POUR DÉVELOPPEMENT (au cas où l'email n'arrive pas)
            Console.WriteLine("\n--------------------------------------------------");
            Console.WriteLine($"[DEBUG] PASSWORD RESET LINK FOR: {user.Email}");
            Console.WriteLine($"LINK: {resetLink}");
            Console.WriteLine("--------------------------------------------------\n");

            var emailBody = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: auto; border: 1px solid #eee; padding: 20px;'>
                    <h2 style='color: #eab308;'>Réinitialisation de votre mot de passe</h2>
                    <p>Bonjour {user.Prenom},</p>
                    <p>Vous avez demandé la réinitialisation de votre mot de passe pour votre compte EvaluaTech.</p>
                    <p>Cliquez sur le bouton ci-dessous pour choisir un nouveau mot de passe :</p>
                    <div style='text-align: center; margin: 30px 0;'>
                        <a href='{resetLink}' style='background-color: #eab308; color: white; padding: 15px 25px; text-decoration: none; border-radius: 5px; font-weight: bold;'>Réinitialiser mon mot de passe</a>
                    </div>
                    <p>Ce lien est valable pendant 2 heures.</p>
                    <p>Si vous n'êtes pas à l'origine de cette demande, vous pouvez ignorer cet email.</p>
                    <hr style='border: 0; border-top: 1px solid #eee;'>
                    <p style='font-size: 12px; color: #999;'>L'équipe EvaluaTech</p>
                </div>";

            try
            {
                await _emailService.SendEmailAsync(user.Email, "Réinitialisation de mot de passe - EvaluaTech", emailBody);
            }
            catch (Exception ex)
            {
                // DEBUG : Retourne l'erreur complète pour savoir ce qui bloque
                return BadRequest(new { message = "Erreur lors de l'envoi de l'email : " + ex.Message });
            }

            return Ok(new { message = "Un lien de réinitialisation a été envoyé à votre adresse email." });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            if (!Guid.TryParse(dto.Token, out Guid tokenGuid))
                return BadRequest(new { message = "Token invalide." });

            var resetToken = await _context.TokensActivation.FirstOrDefaultAsync(t => t.Token == tokenGuid && !t.Utilise && t.DateExpiration > DateTime.UtcNow);

            if (resetToken == null)
                return BadRequest(new { message = "Le lien est invalide ou a expiré." });

            var user = await _context.Utilisateurs.FindAsync(resetToken.UtilisateurId);
            if (user == null)
                return BadRequest(new { message = "Utilisateur introuvable." });

            Console.WriteLine($"[RESET DEBUG] Réinitialisation pour : {user.Email}");
            Console.WriteLine($"[RESET DEBUG] Longueur du nouveau mot de passe reçu : {dto.NewPassword.Length}");

            // Mise à jour du mot de passe
            user.MotDePasseHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            resetToken.Utilise = true;

            await _context.SaveChangesAsync();
            Console.WriteLine($"[RESET DEBUG] SUCCÈS : Mot de passe mis à jour en base pour {user.Email}");

            return Ok(new { message = "Votre mot de passe a été réinitialisé avec succès." });
        }

        // --- 6. NETTOYAGE DES DOUBLONS (Temporaire) ---
        [HttpGet("cleanup-duplicates")]
        public async Task<IActionResult> CleanupDuplicates()
        {
            var duplicates = await _context.Utilisateurs
                .GroupBy(u => u.Email.ToLower())
                .Select(g => new { Email = g.Key, Count = g.Count() })
                .Where(g => g.Count > 1)
                .ToListAsync();

            int deletedCount = 0;
            foreach (var dup in duplicates)
            {
                var users = await _context.Utilisateurs.Where(u => u.Email.ToLower() == dup.Email).ToListAsync();
                
                // On cherche le compte à supprimer (Responsable/AdminEntreprise)
                var toDelete = users.FirstOrDefault(u => (u.RoleNom == "AdminEntreprise" || u.Nom == "VBN") && u.RoleNom != "SuperAdmin");
                // On s'assure qu'on garde le SuperAdmin
                var toKeep = users.FirstOrDefault(u => u.RoleNom == "SuperAdmin" || u.RoleNom == "Admin");

                if (toDelete != null && toKeep != null)
                {
                    _context.Utilisateurs.Remove(toDelete);
                    deletedCount++;
                }
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = $"{deletedCount} comptes en double ont été supprimés.", details = duplicates });
        }
    }
}