using System;
using System.ComponentModel.DataAnnotations;

namespace NeoEvaluation.API.Models
{
    public class Candidature
    {
        [Key] public Guid Id { get; set; } = Guid.NewGuid();
        public ApplicationStatus Statut { get; set; } = ApplicationStatus.POSTULE;
        public DateTime PostuleLe { get; set; } = DateTime.UtcNow;

        public Guid CandidatId { get; set; }
        public Utilisateur Candidat { get; set; } = null!;

        public Guid CampagneId { get; set; }
        public Campagne Campagne { get; set; } = null!;

        public Evaluation? Evaluation { get; set; }
    }
}