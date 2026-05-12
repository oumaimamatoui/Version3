using System;

namespace NeoEvaluation.API.Models
{
    public class UsageLog : IMultiTenant
    {
        public Guid Id { get; set; }
        public Guid? EntrepriseId { get; set; }
        public DateTime Date { get; set; }
        public string Feature { get; set; } = string.Empty; // "AI_GENERATION", "EXPORT_PDF", etc.
        public string? Metadata { get; set; }
    }
}