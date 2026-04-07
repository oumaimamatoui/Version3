using System;
using System.ComponentModel.DataAnnotations;

namespace NeoEvaluation.API.Models
{
    public class Invitation
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string Token { get; set; } = Guid.NewGuid().ToString();

        public string Statut { get; set; } = "Envoyé"; // Envoyé, Activé, Expiré

        public DateTime DateEnvoi { get; set; } = DateTime.UtcNow;

        public Guid CampagneId { get; set; }
    }
}