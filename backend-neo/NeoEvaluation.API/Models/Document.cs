using System;
using System.ComponentModel.DataAnnotations;

namespace NeoEvaluation.API.Models
{
    public class Document
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string NomFichier { get; set; } = string.Empty;

        [Required]
        public string Url { get; set; } = string.Empty;

        // Utilise l'Enum DocumentType (DESCRIPTION_POSTE, etc.)
        public DocumentType Type { get; set; }

        public DateTime TelechargeLe { get; set; } = DateTime.UtcNow;

        // Relation avec la Campagne
        public Guid CampagneId { get; set; }
        public Campagne Campagne { get; set; } = null!;
    }
}