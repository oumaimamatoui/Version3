using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NeoEvaluation.API.Models
{
    public class Campagne : IMultiTenant
    {
        [Key] 
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "Le nom est obligatoire")]
        public string Nom { get; set; } = string.Empty;

        public string? Description { get; set; }
        public DateTime DateDebut { get; set; } = DateTime.UtcNow;
        public DateTime DateFin { get; set; } = DateTime.UtcNow.AddDays(30);
        public StatutCampagne Statut { get; set; } = StatutCampagne.BROUILLON;
        public DateTime CreeLe { get; set; } = DateTime.UtcNow;
        public int DureeMinutes { get; set; } = 60;
        public string ModeNotation { get; set; } = "STRICT";

        // ✅ AJOUT ICI : Limite de candidats pour la campagne
        public int? MaxCandidats { get; set; }

        [Required]
        public Guid? EntrepriseId { get; set; }
        public Entreprise? Entreprise { get; set; }

        // Relations
        public ICollection<CampagneQuestionnaire> CampagneQuestionnaires { get; set; } = new List<CampagneQuestionnaire>();
        public virtual ICollection<Candidature> Candidatures { get; set; } = new List<Candidature>();

        // Relation: Un utilisateur planifie plusieurs campagnes
        public Guid? PlanificateurId { get; set; }
        public Utilisateur? Planificateur { get; set; }
    }
}