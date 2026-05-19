using Microsoft.EntityFrameworkCore;
using NeoEvaluation.API.Data;
using NeoEvaluation.API.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NeoEvaluation.API.Services
{
    public interface IUsageService
    {
        Task<(bool Allowed, string Message)> CheckAndIncrementUsageAsync(Guid enterpriseId, int questionCount = 0);
        Task<(bool Allowed, string Message)> CheckCampaignLimitAsync(Guid enterpriseId);
        Task<UsageStatusDto> GetUsageStatusAsync(Guid enterpriseId);
    }

    public class UsageService : IUsageService
    {
        private readonly AppDbContext _context;

        public UsageService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<(bool Allowed, string Message)> CheckAndIncrementUsageAsync(Guid enterpriseId, int questionCount = 0)
        {
            var ent = await _context.Entreprises.IgnoreQueryFilters().FirstOrDefaultAsync(e => e.Id == enterpriseId);
            if (ent == null) return (false, "Entreprise introuvable.");

            // ✅ LIMITE 1: Max 100 questions par génération
            if (questionCount > 100)
            {
                return (false, "MAX_QUESTIONS_EXCEEDED");
            }

            // ✅ LIMITE 2: Starter ou Abonnement Expiré -> 3 fois par 24h
            bool isStarter = string.Equals(ent.Plan, "Starter", StringComparison.OrdinalIgnoreCase);
            bool isExpired = ent.AbonnementFin.HasValue && ent.AbonnementFin.Value < DateTime.UtcNow;

            if (isStarter || isExpired) 
            {
                var rollingLimit = DateTime.UtcNow.AddHours(-24);
                var usageCount = await _context.UsageLogs.IgnoreQueryFilters()
                    .CountAsync(u => u.EntrepriseId == enterpriseId && u.Date > rollingLimit && u.Feature == "AI_GENERATION");

                Console.WriteLine($"[USAGE] Entreprise: {ent.Nom}, Plan: {ent.Plan}, Usage (24h): {usageCount}/3, Questions: {questionCount}");

                if (usageCount >= 3)
                {
                    var newestUsage = await _context.UsageLogs.IgnoreQueryFilters()
                        .Where(u => u.EntrepriseId == enterpriseId && u.Date > rollingLimit && u.Feature == "AI_GENERATION")
                        .OrderByDescending(u => u.Date)
                        .FirstOrDefaultAsync();

                    if (newestUsage != null)
                    {
                        var remaining = TimeSpan.FromHours(24) - (DateTime.UtcNow - newestUsage.Date);
                        return (false, $"RETRY_IN_{(int)remaining.TotalSeconds}");
                    }
                    return (false, "DAILY_LIMIT_REACHED");
                }

                // Enregistrer l'usage
                _context.UsageLogs.Add(new UsageLog
                {
                    Id = Guid.NewGuid(),
                    EntrepriseId = enterpriseId,
                    Date = DateTime.UtcNow,
                    Feature = "AI_GENERATION"
                });

                await _context.SaveChangesAsync();
            }

            return (true, "");
        }

        public async Task<(bool Allowed, string Message)> CheckCampaignLimitAsync(Guid enterpriseId)
        {
            var ent = await _context.Entreprises.IgnoreQueryFilters().FirstOrDefaultAsync(e => e.Id == enterpriseId);
            if (ent == null) return (false, "Entreprise introuvable.");

            bool isStarter = string.Equals(ent.Plan, "Starter", StringComparison.OrdinalIgnoreCase);
            bool isExpired = ent.AbonnementFin.HasValue && ent.AbonnementFin.Value < DateTime.UtcNow;

            if (isStarter || isExpired) 
            {
                var campaignCount = await _context.Campagnes.IgnoreQueryFilters().CountAsync(c => c.EntrepriseId == enterpriseId);
                
                if (campaignCount > 0 && campaignCount % 3 == 0)
                {
                    var lastCampaign = await _context.Campagnes.IgnoreQueryFilters()
                        .Where(c => c.EntrepriseId == enterpriseId)
                        .OrderByDescending(c => c.CreeLe)
                        .FirstOrDefaultAsync();

                    if (lastCampaign != null)
                    {
                        var createdDate = lastCampaign.CreeLe.Kind == DateTimeKind.Unspecified 
                            ? DateTime.SpecifyKind(lastCampaign.CreeLe, DateTimeKind.Utc) 
                            : lastCampaign.CreeLe.ToUniversalTime();

                        var timeSinceLast = DateTime.UtcNow - createdDate;
                        var remaining = TimeSpan.FromHours(24) - timeSinceLast;

                        if (remaining.TotalSeconds > 0)
                        {
                            return (false, $"RETRY_IN_{(int)remaining.TotalSeconds}");
                        }
                    }
                }
            }

            return (true, "");
        }

        public async Task<UsageStatusDto> GetUsageStatusAsync(Guid enterpriseId)
        {
            var ent = await _context.Entreprises.IgnoreQueryFilters().FirstOrDefaultAsync(e => e.Id == enterpriseId);
            if (ent == null) return new UsageStatusDto();

            var rollingLimit = DateTime.UtcNow.AddHours(-24);
            var currentUsage = await _context.UsageLogs.IgnoreQueryFilters()
                .CountAsync(u => u.EntrepriseId == enterpriseId && u.Date > rollingLimit && u.Feature == "AI_GENERATION");

            bool isStarter = string.Equals(ent.Plan, "Starter", StringComparison.OrdinalIgnoreCase);
            bool isExpired = ent.AbonnementFin.HasValue && ent.AbonnementFin.Value < DateTime.UtcNow;

            int daysRemaining = 0;
            if (ent.AbonnementFin.HasValue && !isExpired)
            {
                daysRemaining = (int)(ent.AbonnementFin.Value - DateTime.UtcNow).TotalDays;
                if (daysRemaining < 0) daysRemaining = 0;
            }

            return new UsageStatusDto
            {
                Current = currentUsage,
                Max = (isStarter || isExpired) ? 3 : 999,
                Plan = isExpired ? "Expiré (Starter)" : (ent.Plan ?? "Starter"),
                MaxQuestions = 100,
                DaysRemaining = daysRemaining
            };
        }
    }

    public class UsageStatusDto
    {
        public int Current { get; set; }
        public int Max { get; set; }
        public int MaxQuestions { get; set; }
        public string Plan { get; set; }
        public int DaysRemaining { get; set; }
    }
}