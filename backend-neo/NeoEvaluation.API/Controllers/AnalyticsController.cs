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
            // 1. Récupération de l'entreprise
            var entreprise = await _context.Entreprises.FirstOrDefaultAsync();
            if (entreprise == null) 
                return Ok(new { Insight = "Aucune entreprise detectee en base." });
            
            Guid entId = entreprise.Id;

            // 2. Statistiques KPI
            var talentsCount = await _context.Utilisateurs
                .CountAsync(u => u.EntrepriseId == entId && u.RoleNom == "Candidat");

            var testsEnCours = await _context.Evaluations
                .CountAsync(e => e.Candidature.Campagne.EntrepriseId == entId && e.Statut == StatutPassage.EN_COURS);

            var queryTermines = _context.Evaluations
                .Where(e => e.Candidature.Campagne.EntrepriseId == entId && e.Statut == StatutPassage.TERMINE);

            double taux = await queryTermines.AnyAsync() 
                ? Math.Round(await queryTermines.AverageAsync(e => e.ScorePourcentage), 1) 
                : 0;

            // 3. Activités récentes
            var activities = await _context.Evaluations
                .Include(e => e.Candidat)
                .Include(e => e.Candidature.Campagne)
                .Where(e => e.Candidature.Campagne.EntrepriseId == entId)
                .OrderByDescending(e => e.DateDebut)
                .Take(5)
                .Select(e => new {
                    User = e.Candidat != null ? (e.Candidat.Prenom + " " + e.Candidat.Nom) : "Anonyme",
                    Action = e.Statut == StatutPassage.TERMINE ? "a termine son evaluation" : "a rejoint une session",
                    Campagne = e.Candidature.Campagne.Nom,
                    Time = "Recent",
                    Color = e.Statut == StatutPassage.TERMINE ? "#10b981" : "#fbbf24"
                })
                .ToListAsync();

            // 4. Graphique : Nombre de candidatures par jour (7 derniers jours)
            var chartData = new List<object>();
            for (int i = 6; i >= 0; i--)
            {
                var dateTarget = DateTime.UtcNow.Date.AddDays(-i);
                
                // Compte des candidatures via PostuleLe
                var count = await _context.Candidatures
                    .CountAsync(c => c.Campagne.EntrepriseId == entId && c.PostuleLe.Date == dateTarget);
                
                chartData.Add(new { 
                    Day = dateTarget.ToString("ddd"), // lun., mar., etc.
                    Count = count 
                });
            }

            // 5. Réponse structurée pour Aura Talent Dashboard
            return Ok(new
            {
                Kpis = new {
                    TalentsActifs = talentsCount,
                    TauxReussite = taux + "%",
                    TestsEnCours = testsEnCours,
                    IaScore = "92.4/100"
                },
                RecentActivities = activities,
                ChartData = chartData,
                Insight = $"Aura IA : Analyse terminee pour {entreprise.Nom}. Vos flux sont optimises."
            });
        }
    }
}