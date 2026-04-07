using System;
using System.ComponentModel.DataAnnotations;

namespace NeoEvaluation.API.Models
{
    public class DocumentCandidat
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string NomFichier { get; set; } = string.Empty;

        [Required]
        public string Url { get; set; } = string.Empty;

        // Utilise l'Enum CandidateDocType (CV, LETTRE_DE_MOTIVATION, etc.)
        public CandidateDocType Type { get; set; }

        public bool EstVerifie { get; set; } = false;

        // Relation avec le Candidat
        public Guid CandidatId { get; set; }
        public Candidat Candidat { get; set; } = null!;
    }
}