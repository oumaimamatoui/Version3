using System;

namespace NeoEvaluation.API.DTOs
{
    public class AuditLogEntry
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Action { get; set; } = string.Empty;
        public string Utilisateur { get; set; } = string.Empty; // Correspond à 'user' dans le service
        public string Details { get; set; } = string.Empty;
        public string Statut { get; set; } = "Succès";
        public DateTime Date { get; set; } = DateTime.UtcNow;
    }
}
