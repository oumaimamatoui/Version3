using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NeoEvaluation.API.Models
{
    public class Evaluation
    {
        [Key] public Guid Id { get; set; } = Guid.NewGuid();
        public double Score { get; set; }
        public EvaluationStatus Statut { get; set; } = EvaluationStatus.NON_COMMENCE;
        public int LimiteTemps { get; set; }

        public Guid CandidatureId { get; set; }
        public Candidature Candidature { get; set; } = null!;

        public ICollection<Reponse> Reponses { get; set; } = new List<Reponse>();
    }
}