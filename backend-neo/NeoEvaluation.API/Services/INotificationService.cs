namespace NeoEvaluation.API.Services
{
    public class NotificationPayload
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Type { get; set; } = "info";   // info | success | warning | alert
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? Link { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Dictionary<string, object>? Meta { get; set; }
    }

    public interface INotificationService
    {
        Task NotifyTenantAsync(Guid tenantId, NotificationPayload payload);
        Task NotifyUserAsync(Guid userId, NotificationPayload payload);
        Task NotifyRoleAsync(string role, NotificationPayload payload);
        Task NotifyAllAsync(NotificationPayload payload);
    }
}