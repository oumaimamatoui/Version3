using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NeoEvaluation.API.Models
{
    public class Entreprise
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string Nom { get; set; } = string.Empty;

        public string? Secteur { get; set; }
        public string Plan { get; set; } = "Gratuit";
        public DateTime? AbonnementFin { get; set; }
        public string Langue { get; set; } = "fr";
        public DateTime CreeLe { get; set; } = DateTime.UtcNow;

        // Champs existants conservés (pas dans le diagramme mais utiles)
        public string? MatriculeFiscale { get; set; }
        public string CouleurSignature { get; set; } = "#6366f1";
        public string? LogoUrl { get; set; }

        // Méthode du diagramme: estActif() basé sur la date d'abonnement
        [NotMapped]
        public bool EstActif => AbonnementFin == null || AbonnementFin > DateTime.UtcNow;

        // Relation optionnelle vers les détails du SuperAdmin

        // Relations
        public ICollection<Utilisateur> Staff { get; set; } = new List<Utilisateur>();
        public ICollection<Campagne> Campagnes { get; set; } = new List<Campagne>();
    }
}