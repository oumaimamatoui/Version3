using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NeoEvaluation.API.Data;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly AppDbContext _context;
    public DashboardController(AppDbContext context) => _context = context;
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
        // 1. Calcul des KPIs réels
        var evaluations = await _context.Evaluations.ToListAsync();
        
        int totalTests = evaluations.Count;
        double moyenne = totalTests > 0 ? evaluations.Average(e => e.ScoreTotal) : 0;
        int iaProcessed = totalTests;
        double tauxEchec = totalTests > 0 ? (double)evaluations.Count(e => e.ScoreTotal < 50) / totalTests * 100 : 0;

        // 2. Données de l'histogramme (Moyenne par Campagne)
        var chartData = await _context.Campagnes
            .Include(c => c.Candidatures)
                .ThenInclude(can => can.Evaluation)
            .Select(c => new {
                name = c.Nom.Length > 10 ? c.Nom.Substring(0, 10) : c.Nom,
                score = c.Candidatures.Any(can => can.Evaluation != null) 
                        ? (int)c.Candidatures.Where(can => can.Evaluation != null).Average(can => can.Evaluation!.ScoreTotal) 
                        : 0
            })
            .Take(5)
            .ToListAsync();

        // 3. Leaderboard (Meilleurs scores)
        var topPerformers = await _context.Evaluations
            .Include(e => e.Candidature)
                .ThenInclude(c => c.Candidat)
            .OrderByDescending(e => e.ScoreTotal)
            .Take(4)
            .Select(e => new {
                name = e.Candidature != null && e.Candidature.Candidat != null ? e.Candidature.Candidat.NomComplet : "Inconnu",
                test = e.Candidature != null && e.Candidature.Campagne != null ? e.Candidature.Campagne.Nom : "N/A",
                score = (int)e.ScoreTotal
            })
            .ToListAsync();

        return Ok(new {
            kpis = new { totalTests, moyenne, iaProcessed, tauxEchec },
            chart = chartData,
            leaders = topPerformers
        });
    }
}