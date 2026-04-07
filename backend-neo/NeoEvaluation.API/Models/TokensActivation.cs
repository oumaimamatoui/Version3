using System;
using System.ComponentModel.DataAnnotations;

namespace NeoEvaluation.API.Models
{
    public class TokensActivation
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid Token { get; set; } = Guid.NewGuid(); // REPASSE EN GUID

        // Champs requis pour vos autres contrôleurs (ne pas supprimer)
        public Guid? IdInscription { get; set; }
        public bool Utilise { get; set; } = false;
        public DateTime DateExpiration { get; set; } = DateTime.UtcNow.AddDays(7);

        // Champs ajoutés pour le nouveau système d'invitation
        public Guid? UtilisateurId { get; set; }
        public string? Email { get; set; }
        public DateTime DateCreation { get; set; } = DateTime.UtcNow;
    }
}