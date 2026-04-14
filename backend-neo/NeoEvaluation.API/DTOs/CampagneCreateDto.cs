using NeoEvaluation.API.Models;

public class CampagneCreateDto {
        public string Nom { get; set; } = string.Empty;
        public string? Description { get; set; }
        public StatutCampagne Statut { get; set; } = StatutCampagne.BROUILLON;
        public DateTime DateDebut { get; set; }
        public DateTime DateFin { get; set; }
        public Guid? QuestionnaireId { get; set; }
        public List<Guid>? SelectedCandidatesIds { get; set; }
    }