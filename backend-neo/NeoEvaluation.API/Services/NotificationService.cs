using Microsoft.AspNetCore.SignalR;
using NeoEvaluation.API.Hubs;

namespace NeoEvaluation.API.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IHubContext<NotificationHub> _hub;

        public NotificationService(IHubContext<NotificationHub> hub)
        {
            _hub = hub;
        }

        public async Task NotifyTenantAsync(Guid tenantId, NotificationPayload payload)
        {
            await _hub.Clients
                .Group($"tenant_{tenantId}")
                .SendAsync("ReceiveNotification", payload);
        }

        public async Task NotifyUserAsync(Guid userId, NotificationPayload payload)
        {
            await _hub.Clients
                .Group($"user_{userId}")
                .SendAsync("ReceiveNotification", payload);
        }

        public async Task NotifyRoleAsync(string role, NotificationPayload payload)
        {
            await _hub.Clients
                .Group($"role_{role}")
                .SendAsync("ReceiveNotification", payload);
        }

        public async Task NotifyAllAsync(NotificationPayload payload)
        {
            await _hub.Clients
                .All
                .SendAsync("ReceiveNotification", payload);
        }
    }
}