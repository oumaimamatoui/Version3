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
        public async Task<IActionResult> GetGlobalStats([FromQuery] string period = "7d")
        {
            try 
            {
                var entId = _tenantService.GetTenantId();
                var userId = _tenantService.GetUserId();
                var userRole = _tenantService.GetUserRole();
                var isCandidat = userRole?.Equals("Candidat", StringComparison.OrdinalIgnoreCase) == true;

                var since = period switch
                {
                    "24h" => DateTime.UtcNow.AddHours(-24),
                    "30d" => DateTime.UtcNow.AddDays(-30),
                    _     => DateTime.UtcNow.AddDays(-7),
                };

                int totalTests, totalCampagnes, totalTalents, completionRate;
                double moyenne;
                List<object> chart = new();
                List<object> leaders = new();
                List<object> recentResults = new();

                if (isCandidat && userId.HasValue)
                {
                    // ── CANDIDAT : voit uniquement ses propres stats ──
                    totalTests = await _context.Evaluations
                        .CountAsync(e => e.Candidature.CandidatId == userId);

                    totalCampagnes = await _context.Candidatures
                        .CountAsync(c => c.CandidatId == userId);

                    totalTalents = 1;

                    var scores = await _context.Evaluations
                        .Where(e => e.Candidature.CandidatId == userId)
                        .Select(e => (double)e.ScorePourcentage)
                        .ToListAsync();
                    moyenne = scores.Any() ? Math.Round(scores.Average(), 1) : 0;

                    var completedEvals = await _context.Evaluations
                        .CountAsync(e => e.Candidature.CandidatId == userId
                                      && e.Statut == StatutPassage.TERMINE);
                    completionRate = totalTests > 0 ? (int)Math.Round((double)completedEvals / totalTests * 100) : 0;

                    var chartRaw = await _context.Candidatures
                        .Where(c => c.CandidatId == userId)
                        .Include(c => c.Campagne)
                        .Select(c => new {
                            name = c.Campagne != null ? c.Campagne.Nom : "Campagne",
                            score = _context.Evaluations
                                .Where(e => e.CandidatureId == c.Id)
                                .Select(e => (double?)e.ScorePourcentage)
                                .FirstOrDefault() ?? 0.0
                        })
                        .ToListAsync();
                    chart = chartRaw.Select(c => (object)c).ToList();

                    var candidatesEvals = await _context.Evaluations
                        .Include(e => e.Candidature)
                            .ThenInclude(can => can.Campagne)
                        .Where(e => e.Candidature.CandidatId == userId)
                        .OrderByDescending(e => e.Id)
                        .Take(10)
                        .ToListAsync();

                    leaders = candidatesEvals
                        .OrderByDescending(e => e.ScorePourcentage)
                        .Take(4)
                        .Select(e => (object)new {
                            name = "Moi",
                            test = e.Candidature?.Campagne?.Nom ?? "Évaluation",
                            score = (int)e.ScorePourcentage
                        }).ToList();

                    recentResults = candidatesEvals.Select(e => (object)new {
                        id = e.Id,
                        candidateId = e.Candidature?.CandidatId,
                        candidateName = "Moi",
                        testName = e.Candidature?.Campagne?.Nom ?? "Test",
                        date = DateTime.Now.ToString("dd MMM yyyy"),
                        score = (int)e.ScorePourcentage,
                        integrity = 100
                    }).ToList();
                }
                else
                {
                    // ── ADMIN / ÉVALUATEUR / RH : voit les stats de son entreprise ──
                    var usersQuery = _context.Utilisateurs.IgnoreQueryFilters();
                    var evalsQuery = _context.Evaluations.IgnoreQueryFilters();
                    var campagnesQuery = _context.Campagnes.IgnoreQueryFilters();
                    totalTests = await evalsQuery
                        .CountAsync(e => e.Candidature.Campagne.EntrepriseId == entId);

                    totalCampagnes = await campagnesQuery
                        .CountAsync(c => c.EntrepriseId == entId);

                    totalTalents = await usersQuery
                        .CountAsync(u => u.RoleNom.ToLower() == "candidat"
                                      && u.EntrepriseId == entId);

                    var scoresByCandidat = await evalsQuery
                        .Where(e => e.Candidature.Campagne.EntrepriseId == entId)
                        .GroupBy(e => e.Candidature.CandidatId)
                        .Select(g => new {
                            CandidatId = g.Key,
                            AvgScore = g.Average(e => (double)e.ScorePourcentage)
                        })
                        .ToListAsync();
                    moyenne = scoresByCandidat.Any()
                        ? Math.Round(scoresByCandidat.Average(x => x.AvgScore), 1)
                        : 0;

                    var completedEvals = await evalsQuery
                        .CountAsync(e => e.Candidature.Campagne.EntrepriseId == entId
                                      && e.Statut == StatutPassage.TERMINE);
                    completionRate = totalTests > 0 ? (int)Math.Round((double)completedEvals / totalTests * 100) : 0;

                    // Histogramme (filtré par période)
                    var chartDataRaw = await campagnesQuery
                        .Where(c => c.EntrepriseId == entId)
                        .OrderByDescending(c => c.Id)
                        .Take(5)
                        .Select(c => new {
                            c.Nom,
                            Moyenne = evalsQuery
                                .Where(e => e.Candidature.CampagneId == c.Id
                                         && e.DateFin >= since)
                                .Average(e => (double?)e.ScorePourcentage)
                        })
                        .ToListAsync();

                    chart = chartDataRaw.Select(c => (object)new {
                        name = c.Nom,
                        score = c.Moyenne.HasValue ? Math.Round(c.Moyenne.Value, 1) : (double?)null
                    }).ToList();

                    // Leaderboard & Résultats récents
                    var evaluationsQuery = await evalsQuery
                        .Include(e => e.Candidature)
                            .ThenInclude(can => can.Candidat)
                        .Include(e => e.Candidature)
                            .ThenInclude(can => can.Campagne)
                        .Where(e => e.Candidature.Campagne.EntrepriseId == entId)
                        .OrderByDescending(e => e.Id)
                        .Take(10)
                        .ToListAsync();

                    leaders = evaluationsQuery
                        .OrderByDescending(e => e.ScorePourcentage)
                        .Take(4)
                        .Select(e => (object)new {
                            name = e.Candidature?.Candidat != null ? $"{e.Candidature.Candidat.Prenom} {e.Candidature.Candidat.Nom}" : "Anonyme",
                            test = e.Candidature?.Campagne?.Nom ?? "Évaluation",
                            score = (int)e.ScorePourcentage
                        }).ToList();

                    recentResults = evaluationsQuery.Select(e => (object)new {
                        id = e.Id,
                        candidateId = (object?)e.Candidature?.CandidatId,
                        candidateName = e.Candidature?.Candidat != null ? $"{e.Candidature.Candidat.Prenom} {e.Candidature.Candidat.Nom}" : "Candidat",
                        testName = e.Candidature?.Campagne?.Nom ?? "Test",
                        date = DateTime.Now.ToString("dd MMM yyyy"),
                        score = (int)e.ScorePourcentage,
                        integrity = 100
                    }).ToList();
                }

                return Ok(new {
                    kpis = new { 
                        totalTests, 
                        totalCampagnes,
                        totalTalents,
                        moyenne, 
                        completionRate,
                        iaProcessed = totalTests,
                        tauxEchec = Math.Max(0, 100 - (int)moyenne) 
                    },
                    chart,
                    leaders,
                    recentResults
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
                    TotalInvitationsThisWeek = totalInvitationsCount,
                    AverageScore = (float)Math.Round(avgScore, 1),
                    AverageCompletionTimeMinutes = (float)Math.Round(avgDuration, 1),
                    CompletionRate = totalInvitationsCount > 0 ? (float)Math.Round((float)weeklyEvaluations.Count / totalInvitationsCount * 100, 1) : 0f,
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

                return Ok(new { 
                    campaign = new {
                        campaign.Id,
                        campaign.Nom,
                        campaign.Description,
                        Statut = (int)campaign.Statut,
                        campaign.DateDebut,
                        campaign.DateFin,
                        campaign.DureeMinutes,
                        campaign.MaxCandidats
                    },
                    message = $"La campagne '{campaign.Nom}' a été publiée avec succès !"
                });
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
                    return BadRequest(new { message = "Tenant ID introuvable." });
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
                    var campNom = e.Candidature?.Campagne?.Nom ?? "Inconnue";
                    var candName = e.Candidat != null ? $"{e.Candidat.Prenom} {e.Candidat.Nom}" : "Anonyme";
                    var dateStr = e.DateFin?.ToString("dd/MM/yyyy HH:mm") ?? "";
                    csv.AppendLine($"\"{candName}\",\"{campNom}\",{e.ScorePourcentage:F1}%,{dateStr},{e.InfractionsCount},{e.NbReprises}");
                }

                var preamble = System.Text.Encoding.UTF8.GetPreamble();
                var csvBytes = System.Text.Encoding.UTF8.GetBytes(csv.ToString());
                var bytes = preamble.Concat(csvBytes).ToArray();
                return File(bytes, "text/csv; charset=utf-8", "Rapport_Performance_Hebdo.csv");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CSV EXPORT ERROR] {ex.Message}");
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}