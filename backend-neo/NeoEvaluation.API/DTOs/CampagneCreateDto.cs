public class CampagneCreateDto {
        public string Nom { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Categorie { get; set; }
        public int DureeMinutes { get; set; }
        public int ScorePassage { get; set; }
        public int Statut { get; set; }
        public int MaxCandidats { get; set; }
        public DateTime DateDebut { get; set; }
        public DateTime DateFin { get; set; }
        public Guid QuestionnaireId { get; set; }
        public List<Guid>? SelectedCandidatesIds { get; set; }
    }