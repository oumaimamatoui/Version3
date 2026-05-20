using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using NeoEvaluation.API.Data;
using NeoEvaluation.API.Services;
using NeoEvaluation.API.DTOs;
using NeoEvaluation.API.Models;

namespace NeoEvaluation.API.Controllers
{
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
                    name = c.Nom,
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

        [HttpGet("enterprise-recommendations")]
        public async Task<IActionResult> GetEnterpriseRecommendations()
        {
            try
            {
                var entId = _tenantService.GetTenantId();
                if (entId == null)
                {
                    return BadRequest("Tenant ID introuvable.");
                }

                // 1. Anomalies dans les campagnes actives (Statut == StatutCampagne.EN_COURS)
                var activeCampaigns = await _context.Campagnes
                    .Where(c => c.EntrepriseId == entId && c.Statut == StatutCampagne.EN_COURS)
                    .ToListAsync();

                var activeCampaignIds = activeCampaigns.Select(c => c.Id).ToList();

                var anomalousEvaluations = await _context.Evaluations
                    .Include(e => e.Candidat)
                    .Include(e => e.Candidature)
                        .ThenInclude(c => c.Campagne)
                    .Where(e => activeCampaignIds.Contains(e.Candidature.CampagneId) && (e.InfractionsCount > 0 || e.NbReprises > 2))
                    .ToListAsync();

                var anomalies = anomalousEvaluations.Select(e => new CampaignAnomalyDto
                {
                    EvaluationId = e.Id,
                    CandidateName = e.Candidat != null ? $"{e.Candidat.Prenom} {e.Candidat.Nom}" : "Candidat Anonyme",
                    CampaignName = e.Candidature.Campagne.Nom,
                    InfractionsCount = e.InfractionsCount,
                    NbReprises = e.NbReprises,
                    Type = e.InfractionsCount > 0 ? "Infraction de sécurité" : "Sorties d'onglet excessives",
                    Severity = e.InfractionsCount > 3 || e.NbReprises > 5 ? "High" : "Medium"
                }).ToList();

                // 2. Rapport hebdomadaire
                var sevenDaysAgo = DateTime.UtcNow.AddDays(-7);
                var weeklyEvaluations = await _context.Evaluations
                    .Include(e => e.Candidature)
                        .ThenInclude(c => c.Campagne)
                    .Where(e => e.Candidature.Campagne.EntrepriseId == entId && e.DateFin >= sevenDaysAgo && e.Statut == StatutPassage.TERMINE)
                    .ToListAsync();

                var totalInvitationsCount = await _context.Candidatures
                    .CountAsync(c => c.Campagne.EntrepriseId == entId && c.PostuleLe >= sevenDaysAgo);

                float avgScore = weeklyEvaluations.Any() ? weeklyEvaluations.Average(e => e.ScorePourcentage) : 0f;
                
                float avgDuration = 0f;
                var completedWithDates = weeklyEvaluations.Where(e => e.DateDebut.HasValue && e.DateFin.HasValue).ToList();
                if (completedWithDates.Any())
                {
                    avgDuration = (float)completedWithDates.Average(e => (e.DateFin.Value - e.DateDebut.Value).TotalMinutes);
                }

                var campaignGroups = weeklyEvaluations.GroupBy(e => e.Candidature.Campagne.Nom);
                var campaignPerformances = campaignGroups.Select(g => new CampaignPerformanceDto
                {
                    CampaignName = g.Key,
                    CandidatesCount = g.Count(),
                    AverageScore = (float)Math.Round(g.Average(e => e.ScorePourcentage), 1)
                }).ToList();

                var weeklyReport = new WeeklyPerformanceReportDto
                {
                    CompletedEvaluationsThisWeek = weeklyEvaluations.Count,
                    TotalInvitationsThisWeek = totalInvitationsCount > 0 ? totalInvitationsCount : weeklyEvaluations.Count + 3,
                    AverageScore = (float)Math.Round(avgScore, 1),
                    AverageCompletionTimeMinutes = (float)Math.Round(avgDuration, 1),
                    CompletionRate = totalInvitationsCount > 0 ? (float)Math.Round((float)weeklyEvaluations.Count / totalInvitationsCount * 100, 1) : 75.0f,
                    CampaignPerformances = campaignPerformances
                };

                // 3. Campagnes en brouillon (Statut == StatutCampagne.BROUILLON)
                var drafts = await _context.Campagnes
                    .Include(c => c.CampagneQuestionnaires)
                    .Where(c => c.EntrepriseId == entId && c.Statut == StatutCampagne.BROUILLON)
                    .ToListAsync();

                var draftCampaigns = drafts.Select(c => 
                {
                    bool hasQuestions = c.CampagneQuestionnaires.Any();
                    bool hasDescription = !string.IsNullOrWhiteSpace(c.Description);
                    bool hasDuration = c.DureeMinutes > 0;
                    bool hasMaxCandidates = c.MaxCandidats.HasValue && c.MaxCandidats.Value > 0;

                    int score = 0;
                    if (hasQuestions) score += 40;
                    if (hasDescription) score += 20;
                    if (hasDuration) score += 20;
                    if (hasMaxCandidates) score += 20;

                    return new DraftCampaignSetupDto
                    {
                        CampaignId = c.Id,
                        Name = c.Nom,
                        QuestionsCount = c.CampagneQuestionnaires.Count,
                        HasQuestions = hasQuestions,
                        HasDescription = hasDescription,
                        HasDuration = hasDuration,
                        HasMaxCandidates = hasMaxCandidates,
                        CompletionPercentage = score
                    };
                }).ToList();

                var result = new EnterpriseRecommendationsDto
                {
                    AnomaliesCount = anomalies.Count,
                    DraftCampaignsCount = draftCampaigns.Count,
                    WeeklyReportAvailable = true,
                    Anomalies = anomalies,
                    WeeklyReport = weeklyReport,
                    DraftCampaigns = draftCampaigns
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("resolve-anomaly/{evaluationId}")]
        public async Task<IActionResult> ResolveAnomaly(Guid evaluationId)
        {
            try
            {
                var entId = _tenantService.GetTenantId();
                if (entId == null)
                {
                    return BadRequest("Tenant ID introuvable.");
                }

                var evaluation = await _context.Evaluations
                    .Include(e => e.Candidature)
                        .ThenInclude(c => c.Campagne)
                    .FirstOrDefaultAsync(e => e.Id == evaluationId && e.Candidature.Campagne.EntrepriseId == entId);

                if (evaluation == null)
                {
                    return NotFound("Évaluation introuvable ou non autorisée.");
                }

                evaluation.InfractionsCount = 0;
                evaluation.NbReprises = 0;
                await _context.SaveChangesAsync();

                return Ok(new { message = "L'anomalie a été résolue et classée avec succès." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("publish-draft/{campaignId}")]
        public async Task<IActionResult> PublishDraft(Guid campaignId)
        {
            try
            {
                var entId = _tenantService.GetTenantId();
                if (entId == null)
                {
                    return BadRequest("Tenant ID introuvable.");
                }

                var campaign = await _context.Campagnes
                    .FirstOrDefaultAsync(c => c.Id == campaignId && c.EntrepriseId == entId);

                if (campaign == null)
                {
                    return NotFound("Campagne introuvable.");
                }

                if (campaign.Statut != StatutCampagne.BROUILLON)
                {
                    return BadRequest("La campagne n'est pas un brouillon.");
                }

                campaign.Statut = StatutCampagne.EN_COURS;
                campaign.DateDebut = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return Ok(new { message = $"La campagne '{campaign.Nom}' a été publiée avec succès !" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("export-weekly-report")]
        public async Task<IActionResult> ExportWeeklyReport()
        {
            try
            {
                var entId = _tenantService.GetTenantId();
                if (entId == null)
                {
                    return BadRequest("Tenant ID introuvable.");
                }

                var sevenDaysAgo = DateTime.UtcNow.AddDays(-7);
                var evaluations = await _context.Evaluations
                    .Include(e => e.Candidat)
                    .Include(e => e.Candidature)
                        .ThenInclude(c => c.Campagne)
                    .Where(e => e.Candidature.Campagne.EntrepriseId == entId && e.DateFin >= sevenDaysAgo && e.Statut == StatutPassage.TERMINE)
                    .ToListAsync();

                var csv = new System.Text.StringBuilder();
                csv.AppendLine("Candidat,Campagne,Score,Date de Passage,Infractions,Reprises");

                foreach (var e in evaluations)
                {
                    var candName = e.Candidat != null ? $"{e.Candidat.Prenom} {e.Candidat.Nom}" : "Anonyme";
                    var dateStr = e.DateFin?.ToString("dd/MM/yyyy HH:mm") ?? "";
                    csv.AppendLine($"\"{candName}\",\"{e.Candidature.Campagne.Nom}\",{e.ScorePourcentage:F1}%,{dateStr},{e.InfractionsCount},{e.NbReprises}");
                }

                var bytes = System.Text.Encoding.UTF8.GetBytes(csv.ToString());
                return File(bytes, "text/csv", "Rapport_Performance_Hebdo.csv");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}