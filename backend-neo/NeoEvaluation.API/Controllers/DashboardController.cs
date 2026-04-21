using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using NeoEvaluation.API.Data;
using NeoEvaluation.API.Services;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ITenantService _tenantService;

    public DashboardController(AppDbContext context, ITenantService tenantService)
    {
        _context = context;
        _tenantService = tenantService;
    }
[HttpGet("download-report")]
public IActionResult DownloadReport()
{
    var fileContents = System.Text.Encoding.UTF8.GetBytes("Contenu du rapport PDF simulé");
    var contentType = "application/pdf";
    var fileName = "Rapport_Analytique.pdf";

    return File(fileContents, contentType, fileName);
}
    [HttpGet("global-stats")]
    public async Task<IActionResult> GetGlobalStats()
    {
        try {
            // 1. Calcul des KPIs réels (filtrés par entreprise via ITenantService dans DbContext)
            var totalTests = await _context.Evaluations.CountAsync();
            var totalCampagnes = await _context.Campagnes.CountAsync();
            var totalTalents = await _context.Utilisateurs.CountAsync(u => u.RoleNom == "Candidat");
            
            var evaluations = await _context.Evaluations.Select(e => e.ScoreTotal).ToListAsync();
            double moyenne = evaluations.Any() ? evaluations.Average() : 0;
            
            // 2. Données de l'histogramme (Performance par campagne)
            var rawChartData = await _context.Campagnes
                .OrderByDescending(c => c.CreeLe)
                .Take(5)
                .Select(c => new {
                    c.Nom,
                    Scores = c.Candidatures
                        .Where(can => can.Evaluation != null)
                        .Select(can => (float?)can.Evaluation!.ScoreTotal)
                })
                .ToListAsync();

            var chartData = rawChartData.Select(c => new {
                name = c.Nom.Length > 12 ? c.Nom.Substring(0, 10) + ".." : c.Nom,
                score = c.Scores.Any() ? c.Scores.Average() : 0
            }).ToList();

            // 3. Leaderboard (Meilleurs scores)
            var topPerformers = await _context.Evaluations
                .OrderByDescending(e => e.ScoreTotal)
                .Take(4)
                .Select(e => new {
                    name = e.Candidature != null && e.Candidature.Candidat != null 
                           ? e.Candidature.Candidat.Prenom + " " + e.Candidature.Candidat.Nom 
                           : "Inconnu",
                    test = e.Candidature != null && e.Candidature.Campagne != null ? e.Candidature.Campagne.Nom : "N/A",
                    score = (int)e.ScoreTotal
                })
                .ToListAsync();

            return Ok(new {
                kpis = new { 
                    totalTests, 
                    totalCampagnes,
                    totalTalents,
                    moyenne = Math.Round(moyenne, 1), 
                    iaProcessed = totalTests,
                    tauxSucces = moyenne 
                },
                chart = chartData,
                leaders = topPerformers
            });
        } catch (Exception ex) {
            Console.WriteLine($"[DASHBOARD ERROR] {ex.Message}");
            return StatusCode(500, ex.Message);
        }
    }
}