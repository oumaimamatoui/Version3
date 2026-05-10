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
        Task<(bool Allowed, string Message)> CheckAndIncrementUsageAsync(Guid enterpriseId);
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

        public async Task<(bool Allowed, string Message)> CheckAndIncrementUsageAsync(Guid enterpriseId)
        {
            var ent = await _context.Entreprises.IgnoreQueryFilters().FirstOrDefaultAsync(e => e.Id == enterpriseId);
            if (ent == null) return (false, "Entreprise introuvable.");

            // ✅ LIMITE : Starter ou Gratuit -> 2 par 24h
            if (string.Equals(ent.Plan, "Starter", StringComparison.OrdinalIgnoreCase) || 
                string.Equals(ent.Plan, "Gratuit", StringComparison.OrdinalIgnoreCase)) 
            {
                var rollingLimit = DateTime.UtcNow.AddHours(-24);
                var usageCount = await _context.UsageLogs.IgnoreQueryFilters()
                    .CountAsync(u => u.EntrepriseId == enterpriseId && u.Date > rollingLimit && u.Feature == "AI_GENERATION");

                Console.WriteLine($"[USAGE] Entreprise: {ent.Nom}, Plan: {ent.Plan}, Usage (24h): {usageCount}/5");

                if (usageCount >= 5)
                {
                    // Trouver la plus ancienne génération dans les dernières 24h pour calculer le reset
                    var oldestUsage = await _context.UsageLogs.IgnoreQueryFilters()
                        .Where(u => u.EntrepriseId == enterpriseId && u.Date > rollingLimit && u.Feature == "AI_GENERATION")
                        .OrderBy(u => u.Date)
                        .FirstOrDefaultAsync();

                    if (oldestUsage != null)
                    {
                        var remaining = TimeSpan.FromHours(24) - (DateTime.UtcNow - oldestUsage.Date);
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

            if (string.Equals(ent.Plan, "Starter", StringComparison.OrdinalIgnoreCase) || 
                string.Equals(ent.Plan, "Gratuit", StringComparison.OrdinalIgnoreCase)) 
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

            return new UsageStatusDto
            {
                Current = currentUsage,
                Max = (string.Equals(ent.Plan, "Starter", StringComparison.OrdinalIgnoreCase) || string.Equals(ent.Plan, "Gratuit", StringComparison.OrdinalIgnoreCase)) ? 5 : 999,
                Plan = ent.Plan ?? "Starter"
            };
        }
    }

    public class UsageStatusDto
    {
        public int Current { get; set; }
        public int Max { get; set; }
        public string Plan { get; set; }
    }
}
