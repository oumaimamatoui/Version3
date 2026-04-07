using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NeoEvaluation.API.Data;
using NeoEvaluation.API.DTOs;
using NeoEvaluation.API.Models;
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

        public AuthController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // --- 1. LOGIN CLASSIQUE (Email / Mot de passe) ---
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (loginDto == null || string.IsNullOrEmpty(loginDto.Email))
                return BadRequest(new { message = "Données de connexion invalides." });

            // Nettoyage des entrées
            var email = loginDto.Email.Trim().ToLower();
            var password = loginDto.Password?.Trim();

            // Recherche de l'utilisateur (Utilisateur est abstract, on cherche dans le DbSet global)
            var user = await _context.Utilisateurs.FirstOrDefaultAsync(u => u.Email == email);
            
            if (user == null) 
                return Unauthorized(new { message = "Email incorrect ou compte inexistant." });

            // CAS SPÉCIAL : Admin de test (Hardcoded pour faciliter le développement)
            if (email == "admin@evaluatech.tn" && password == "Admin123")
            {
                return Ok(GenerateJwtResponse(user));
            }

            // Vérification du mot de passe via BCrypt pour les autres comptes
            if (string.IsNullOrEmpty(user.MotDePasseHash))
                return Unauthorized(new { message = "Ce compte n'a pas de mot de passe configuré (Connexion Google uniquement ?)." });

            bool isValid = BCrypt.Net.BCrypt.Verify(password, user.MotDePasseHash);

            if (!isValid) 
                return Unauthorized(new { message = "Mot de passe incorrect." });

            if (!user.EstActif) 
                return BadRequest(new { message = "Votre compte est désactivé. Veuillez contacter l'administrateur." });

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
                    return BadRequest(new { message = "Compte introuvable. Veuillez vous inscrire d'abord." });

                if (!user.EstActif) 
                    return BadRequest(new { message = "Votre compte est en attente d'approbation." });

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

            // Gestion sécurisée du rôle (Accès direct via la classe abstract Utilisateur)
            string userRole = string.IsNullOrEmpty(user.RoleNom) ? "Candidat" : user.RoleNom;

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
                Photo = user.PhotoUrl
            };
        }
    }
}