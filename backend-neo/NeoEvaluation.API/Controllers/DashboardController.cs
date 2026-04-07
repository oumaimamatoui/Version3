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
    // 1. Logique pour générer le PDF (exemple avec une lib comme QuestPDF ou iText)
    // Ici, on crée un fichier vide ou un test pour vérifier que ça marche
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
        double moyenne = totalTests > 0 ? evaluations.Average(e => e.Score) : 0;
        int iaProcessed = totalTests; // Dans votre logique UML, tout est traité par l'IA
        double tauxEchec = totalTests > 0 ? (double)evaluations.Count(e => e.Score < 50) / totalTests * 100 : 0;

        // 2. Données de l'histogramme (Moyenne par Campagne)
        var chartData = await _context.Campagnes
            .Include(c => c.Candidatures)
                .ThenInclude(can => can.Evaluation)
            .Select(c => new {
                name = c.Nom.Length > 10 ? c.Nom.Substring(0, 10) : c.Nom,
                score = c.Candidatures.Any(can => can.Evaluation != null) 
                        ? (int)c.Candidatures.Average(can => can.Evaluation.Score) 
                        : 0
            })
            .Take(5)
            .ToListAsync();

        // 3. Leaderboard (Meilleurs scores)
        var topPerformers = await _context.Evaluations
            .Include(e => e.Candidature)
                .ThenInclude(c => c.Candidat)
            .OrderByDescending(e => e.Score)
            .Take(4)
            .Select(e => new {
                name = e.Candidature.Candidat.NomComplet,
                test = e.Candidature.Campagne.Nom,
                score = (int)e.Score
            })
            .ToListAsync();

        return Ok(new {
            kpis = new { totalTests, moyenne, iaProcessed, tauxEchec },
            chart = chartData,
            leaders = topPerformers
        });
    }
}