   public class CampagneCreateDto
    {
        public string Nom { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Statut { get; set; }
        public Guid? QuestionnaireId { get; set; }
        public DateTime DateDebut { get; set; }
        public DateTime DateFin { get; set; }
        public int DureeMinutes { get; set; }
        
        // ✅ Propriété ajoutée pour mapper le champ du formulaire Vue.js
        public int? MaxCandidats { get; set; } 
        
        public string? ModeNotation { get; set; }
        public List<Guid>? SelectedCandidatesIds { get; set; }
    }