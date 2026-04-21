using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NeoEvaluation.API.Data;
using NeoEvaluation.API.Models;
using Microsoft.AspNetCore.Authorization;
using NeoEvaluation.API.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeoEvaluation.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class StaffController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ITenantService _tenantService;

        public StaffController(AppDbContext context, ITenantService tenantService)
        {
            _context = context;
            _tenantService = tenantService;
        }

        // 1. OBTENIR TOUT LE PERSONNEL
        [HttpGet]
        public async Task<ActionResult> GetStaff()
        {
            try
            {
                // -- DEBUG MULTI-TENANCY --
                var tenantId = _tenantService.GetTenantId();
                var userRole = _tenantService.GetUserRole();
                var isSuperAdminContext = _context.IsSuperAdmin;
                var currentTenantIdContext = _context.CurrentTenantId;
                Console.WriteLine("\n=== [DEBUG MULTI-TENANCY STAFF] ===");
                Console.WriteLine($"Service TenantId : {tenantId}");
                Console.WriteLine($"Service UserRole : {userRole}");
                Console.WriteLine($"DbContext IsSuperAdmin : {isSuperAdminContext}");
                Console.WriteLine($"DbContext CurrentTenantId : {currentTenantIdContext}");
                Console.WriteLine("=====================================\n");

                // Diagnostic : On compte tout d'abord TOUS les utilisateurs
                var totalAll = await _context.Utilisateurs.CountAsync();
                Console.WriteLine($"[DIAGNOSTIC] Total Utilisateurs Database: {totalAll}");

                var staff = await _context.Utilisateurs
                    .Where(u => u.RoleNom != "Candidat") // On prend TOUT sauf Candidat
                    .OrderByDescending(u => u.CreeLe)
                    .ToListAsync();

                Console.WriteLine($"[DIAGNOSTIC] Staff Trouvés (Non-Candidat): {staff.Count}");

                var result = staff.Select(u => new
                {
                    u.Id,
                    u.Prenom,
                    NomFamille = u.Nom,
                    u.Email,
                    PhotoUrl = string.IsNullOrEmpty(u.PhotoUrl) ? null : u.PhotoUrl,
                    RoleNom = u.RoleNom ?? "Membre",
                    u.EstActif,
                    u.CreeLe,
                    EntrepriseId = u.EntrepriseId
                }).ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERREUR] GetStaff: {ex.Message}");
                return StatusCode(500, new { message = "Erreur", error = ex.Message });
            }
        }

        // 2. STATISTIQUES
        [HttpGet("stats")]
        public async Task<ActionResult> GetStaffStats()
        {
            try
            {
                var total = await _context.Utilisateurs.CountAsync(u => u.RoleNom != "Candidat" && u.RoleNom != null);
                var actifs = await _context.Utilisateurs.CountAsync(u => u.RoleNom != "Candidat" && u.EstActif == true);
                return Ok(new { totalPersonnel = total, actifs = actifs, rolesPersonnalises = 3 });
            }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        // 3. TOGGLE STATUS
        [HttpPatch("{id}/toggle-status")]
        public async Task<IActionResult> ToggleStatus(Guid id)
        {
            var user = await _context.Utilisateurs.FindAsync(id);
            if (user == null) return NotFound();
            user.EstActif = !user.EstActif;
            await _context.SaveChangesAsync();
            return Ok(new { status = user.EstActif });
        }

        // 4. METTRE À JOUR UN MEMBRE
        // PUT: api/staff/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStaff(Guid id, [FromBody] Utilisateur updateDto)
        {
            try
            {
                var user = await _context.Utilisateurs.FindAsync(id);
                if (user == null) return NotFound(new { message = "Utilisateur non trouvé" });

                user.Prenom = updateDto.Prenom;
                user.Nom = updateDto.Nom; // Note: on utilise Nom dans la DB
                user.RoleNom = updateDto.RoleNom;
                // On peut aussi mettre à jour d'autres champs si nécessaire

                await _context.SaveChangesAsync();
                return Ok(new { message = "Mise à jour réussie" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur mise à jour", error = ex.Message });
            }
        }

        // 5. SUPPRIMER DÉFINITIVEMENT UN MEMBRE
        // DELETE: api/staff/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStaff(Guid id)
        {
            try
            {
                var user = await _context.Utilisateurs.FindAsync(id);
                if (user == null) return NotFound(new { message = "Utilisateur non trouvé" });

                // On supprime également les données liées si nécessaire (Tokens, etc)
                var relatedTokens = _context.TokensActivation.Where(t => t.UtilisateurId == id);
                _context.TokensActivation.RemoveRange(relatedTokens);

                _context.Utilisateurs.Remove(user);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Membre supprimé définitivement de la base de données" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors de la suppression", error = ex.Message });
            }
        }
    }
}