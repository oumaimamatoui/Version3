using System;
using System.ComponentModel.DataAnnotations;

namespace NeoEvaluation.API.Models
{
    public class Planning
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid CampagneId { get; set; }
        public Campagne Campagne { get; set; } = null!;

        public DateTime DateOuverture { get; set; }
        public int DureeMinutes { get; set; }
        public string ModeNotation { get; set; } = "IA"; // IA, Manuel, Mixte
    }
}