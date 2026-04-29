using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NeoEvaluation.API.Models;
using NeoEvaluation.API.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeoEvaluation.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnalyticsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AnalyticsController(AppDbContext context) => _context = context;
       
        [HttpGet("overview")]
        public async Task<IActionResult> GetOverview()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var roleClaim = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            var entIdClaim = User.FindFirst("entrepriseId")?.Value;

            if (string.IsNullOrEmpty(roleClaim)) return Unauthorized();

            // CAS 1 : CANDIDAT
            if (roleClaim == "Candidat" && Guid.TryParse(userIdClaim, out Guid userId))
            {
                var testsEnAttente = await _context.Evaluations
                    .CountAsync(e => e.CandidatId == userId && (e.Statut == StatutPassage.EN_COURS || e.Statut == StatutPassage.NON_COMMENCE));

                var termines = await _context.Evaluations
                    .Where(e => e.CandidatId == userId && e.Statut == StatutPassage.TERMINE)
                    .ToListAsync();

                double avgScore = termines.Any() ? Math.Round(termines.Average(e => e.ScorePourcentage), 1) : 0;

                return Ok(new
                {
                    Kpis = new {
                        TalentsActifs = 1,
                        TauxReussite = avgScore + "%",
                        TestsEnCours = testsEnAttente,
                        IaScore = avgScore > 0 ? $"{avgScore}/100" : "0/100"
                    },
                    RecentActivities = termines.OrderByDescending(e => e.DateFin).Take(5).Select(e => new {
                        User = "Vous",
                        Action = "avez terminé l'évaluation",
                        Campagne = "Session terminée",
                        Time = e.DateFin?.ToString("dd/MM"),
                        Color = "#10b981"
                    }),
                    Insight = avgScore > 0 ? "Vos performances sont stables." : "Prêt pour votre premier test ?"
                });
            }

            // CAS 2 : ADMIN ENTREPRISE
            if (Guid.TryParse(entIdClaim, out Guid entId))
            {
                var talentsCount = await _context.Utilisateurs
                    .CountAsync(u => u.EntrepriseId == entId && u.RoleNom == "Candidat");

                var testsEnCours = await _context.Evaluations
                    .CountAsync(e => e.Candidature.Campagne.EntrepriseId == entId && e.Statut == StatutPassage.EN_COURS);

                var queryTermines = _context.Evaluations
                    .Where(e => e.Candidature.Campagne.EntrepriseId == entId && e.Statut == StatutPassage.TERMINE);

                double taux = await queryTermines.AnyAsync() 
                    ? Math.Round(await queryTermines.AverageAsync(e => e.ScorePourcentage), 1) 
                    : 0;

                var activities = await _context.Evaluations
                    .Include(e => e.Candidat)
                    .Include(e => e.Candidature.Campagne)
                    .Where(e => e.Candidature.Campagne.EntrepriseId == entId)
                    .OrderByDescending(e => e.DateDebut)
                    .Take(5)
                    .Select(e => new {
                        User = e.Candidat != null ? (e.Candidat.Prenom + " " + e.Candidat.Nom) : "Anonyme",
                        Action = e.Statut == StatutPassage.TERMINE ? "a terminé son évaluation" : "a rejoint une session",
                        Campagne = e.Candidature.Campagne.Nom,
                        Time = "Récent",
                        Color = e.Statut == StatutPassage.TERMINE ? "#10b981" : "#fbbf24"
                    })
                    .ToListAsync();

                return Ok(new
                {
                    Kpis = new {
                        TalentsActifs = talentsCount,
                        TauxReussite = taux + "%",
                        TestsEnCours = testsEnCours,
                        IaScore = talentsCount > 0 ? "Calculé" : "0/100"
                    },
                    RecentActivities = activities,
                    Insight = $"Analyse terminée. Flux optimisés pour votre organisation."
                });
            }

            return Ok(new { Insight = "Accès restreint." });
        }

    }
}