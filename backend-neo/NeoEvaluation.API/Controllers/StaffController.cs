using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NeoEvaluation.API.Data;
using NeoEvaluation.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeoEvaluation.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StaffController : ControllerBase
    {
        private readonly AppDbContext _context;

        public StaffController(AppDbContext context)
        {
            _context = context;
        }

        // 1. OBTENIR TOUT LE PERSONNEL (Exclut les candidats)
        // GET: api/staff
        [HttpGet]
        public async Task<ActionResult> GetStaff()
        {
            try
            {
                // Note : On filtre pour ne prendre que les AdminEntreprise, SuperAdmin, etc.
                // On se base sur la colonne RoleNom ou UserType vue dans votre PostgreSQL
                var staff = await _context.Utilisateurs
                    .Where(u => u.RoleNom != "Candidat" && u.RoleNom != null && u.RoleNom != "")
                    .Select(u => new
                    {
                        u.Id,
                        // Utilisation du NomComplet calculé ou concaténation manuelle pour sécurité
                        NomComplet = (u.Prenom ?? "") + " " + (u.Nom ?? ""),
                        u.Email,
                        u.PhotoUrl,
                        RoleNom = u.RoleNom ?? "Membre",
                        u.EstActif,
                        u.CreeLe,
                        // Assurer que Privileges n'est jamais null pour le Frontend
                        Privileges = u.Privileges ?? new List<string>()
                    })
                    .OrderByDescending(u => u.CreeLe)
                    .ToListAsync();

                return Ok(staff);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors de la récupération du personnel", error = ex.Message });
            }
        }

        // 2. OBTENIR LES STATISTIQUES DU PERSONNEL
        // GET: api/staff/stats
        [HttpGet("stats")]
        public async Task<ActionResult> GetStaffStats()
        {
            try
            {
                // On compte uniquement les membres qui ne sont pas des candidats
                var total = await _context.Utilisateurs
                    .CountAsync(u => u.RoleNom != "Candidat" && u.RoleNom != null && u.RoleNom != "");

                var actifs = await _context.Utilisateurs
                    .CountAsync(u => u.RoleNom != "Candidat" && u.RoleNom != null && u.RoleNom != "" && u.EstActif == true);

                // Nombre de rôles différents créés dans l'organisation
                var rolesCount = await _context.Roles.CountAsync();

                return Ok(new
                {
                    totalPersonnel = total,
                    actifs = actifs,
                    rolesPersonnalises = rolesCount
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors du calcul des statistiques", error = ex.Message });
            }
        }

        // 3. CHANGER LE STATUT (ACTIF / INACTIF)
        // PATCH: api/staff/{id}/toggle-status
        [HttpPatch("{id}/toggle-status")]
        public async Task<IActionResult> ToggleStatus(Guid id)
        {
            try
            {
                var user = await _context.Utilisateurs.FindAsync(id);
                
                if (user == null)
                    return NotFound(new { message = "Utilisateur non trouvé" });

                // Inversion du booléen
                user.EstActif = !user.EstActif;

                _context.Entry(user).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok(new { 
                    id = user.Id, 
                    status = user.EstActif, 
                    message = $"Statut mis à jour : {(user.EstActif ? "Actif" : "Inactif")}" 
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors de la mise à jour du statut", error = ex.Message });
            }
        }
    }
}