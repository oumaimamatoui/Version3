using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using NeoEvaluation.API.Data;
using NeoEvaluation.API.Services;

namespace NeoEvaluation.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("global-stats")]
        public async Task<IActionResult> GetGlobalStats()
        {
            try 
            {
                // 1. KPIs : Calculs simples et robustes
                var totalTests = await _context.Evaluations.CountAsync();
                var totalCampagnes = await _context.Campagnes.CountAsync();
                
                // On utilise ToLower() pour ignorer la casse du rôle
                var totalTalents = await _context.Utilisateurs
                    .CountAsync(u => u.RoleNom.ToLower() == "candidat");
                
                var scores = await _context.Evaluations.Select(e => (double)e.ScoreTotal).ToListAsync();
                double moyenne = scores.Any() ? Math.Round(scores.Average(), 1) : 0;

                // 2. Histogramme : Performance par campagne
                var chartDataRaw = await _context.Campagnes
                    .OrderByDescending(c => c.Id)
                    .Take(5)
                    .Select(c => new {
                        c.Nom,
                        Moyenne = _context.Evaluations
                            .Where(e => e.Candidature.CampagneId == c.Id)
                            .Select(e => (double?)e.ScoreTotal)
                            .Average() ?? 0
                    })
                    .ToListAsync();

                var chart = chartDataRaw.Select(c => new {
                    name = c.Nom.Length > 12 ? c.Nom.Substring(0, 10) + ".." : c.Nom,
                    score = Math.Round(c.Moyenne, 1)
                }).ToList();

                // 3. Leaderboard & 4. Résultats récents
                // On récupère tout en une fois avec les jointures nécessaires
                var evaluationsQuery = await _context.Evaluations
                    .Include(e => e.Candidature)
                        .ThenInclude(can => can.Candidat)
                    .Include(e => e.Candidature)
                        .ThenInclude(can => can.Campagne)
                    .OrderByDescending(e => e.Id)
                    .Take(10) // On en prend assez pour les leaders et le tableau
                    .ToListAsync();

                // Formatage des leaders (les 4 meilleurs)
                var leaders = evaluationsQuery
                    .OrderByDescending(e => e.ScoreTotal)
                    .Take(4)
                    .Select(e => new {
                        name = e.Candidature?.Candidat != null ? $"{e.Candidature.Candidat.Prenom} {e.Candidature.Candidat.Nom}" : "Anonyme",
                        test = e.Candidature?.Campagne?.Nom ?? "Évaluation",
                        score = (int)e.ScoreTotal
                    }).ToList();

                // Formatage des résultats récents (tableau du bas)
                var recentResults = evaluationsQuery.Select(e => new {
                    id = e.Id,
                    candidateId = e.Candidature?.CandidatId,
                    candidateName = e.Candidature?.Candidat != null ? $"{e.Candidature.Candidat.Prenom} {e.Candidature.Candidat.Nom}" : "Candidat",
                    testName = e.Candidature?.Campagne?.Nom ?? "Test",
                    date = DateTime.Now.ToString("dd MMM yyyy"), // Remplacez par e.DateCreation si disponible
                    score = (int)e.ScoreTotal,
                    integrity = 100 // Valeur par défaut
                }).ToList();

                return Ok(new {
                    kpis = new { 
                        totalTests, 
                        totalCampagnes,
                        totalTalents,
                        moyenne, 
                        iaProcessed = totalTests,
                        tauxEchec = Math.Max(0, 100 - (int)moyenne) 
                    },
                    chart = chart,
                    leaders = leaders,
                    recentResults = recentResults
                });
            } 
            catch (Exception ex) 
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}