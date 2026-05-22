using Microsoft.EntityFrameworkCore;
using NeoEvaluation.API.Data;

namespace NeoEvaluation.API.Services
{
    public class PlanResetService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<PlanResetService> _logger;

        public PlanResetService(IServiceScopeFactory scopeFactory, ILogger<PlanResetService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("[PlanResetService] Démarrage — vérification toutes les 24h");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ResetExpiredPlansAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[PlanResetService] Erreur lors du reset des plans");
                }

                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }

        private async Task ResetExpiredPlansAsync(CancellationToken ct)
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var expiredOrgs = await context.Entreprises
                .IgnoreQueryFilters()
                .Where(e =>
                    e.Nom != "SYSTEM_PLATFORM" &&
                    e.Plan != "Starter" &&
                    e.Plan != "Gratuit" &&
                    e.AbonnementFin != null &&
                    e.AbonnementFin <= DateTime.UtcNow)
                .ToListAsync(ct);

            foreach (var org in expiredOrgs)
            {
                _logger.LogInformation("[PlanResetService] Reset plan pour {Org} ({Plan} → Starter)", org.Nom, org.Plan);
                org.Plan = "Starter";
            }

            if (expiredOrgs.Count > 0)
            {
                await context.SaveChangesAsync(ct);
                _logger.LogInformation("[PlanResetService] {Count} organisation(s) remise(s) à Starter", expiredOrgs.Count);
            }
        }
    }
}
