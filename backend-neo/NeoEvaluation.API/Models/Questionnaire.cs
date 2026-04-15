using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NeoEvaluation.API.Models
{
    public class Questionnaire : IMultiTenant
    {
        public Guid? EntrepriseId { get; set; }
        
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string Titre { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
        public int DureeMinutes { get; set; } = 60;
        public DateTime? DebutDispo { get; set; }
        public DateTime? FinDispo { get; set; }
        public bool ReprisesAutorisees { get; set; } = false;
        public int MaxReprises { get; set; } = 1;
        public bool AntitricheActif { get; set; } = false;
        public bool RandomiserQuestions { get; set; } = false;
        public bool EstPublie { get; set; } = false;
        public DateTime CreeLe { get; set; } = DateTime.UtcNow;

        // Relations Many-to-Many via table de jointure
        public ICollection<QuestionnaireQuestion> QuestionnaireQuestions { get; set; } = new List<QuestionnaireQuestion>();
        public ICollection<CampagneQuestionnaire> CampagneQuestionnaires { get; set; } = new List<CampagneQuestionnaire>();

        // Relation: Un utilisateur crée plusieurs questionnaires
        public Guid? CreateurId { get; set; }
        public Utilisateur? Createur { get; set; }
    }
}