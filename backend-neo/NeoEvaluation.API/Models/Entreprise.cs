using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NeoEvaluation.API.Models
{
    public class Entreprise
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string Nom { get; set; } = string.Empty;

        public string Plan { get; set; } = "Gratuit";
        public string? MatriculeFiscale { get; set; }
        public DateTime CreeLe { get; set; } = DateTime.UtcNow;

        public bool EstActif { get; set; } = true;

        // Branding & Personnalisation
        public string CouleurSignature { get; set; } = "#6366f1";
        public string? LogoUrl { get; set; }

        // Relation optionnelle vers les détails du SuperAdmin
        public virtual EntrepriseParSA? DetailsSA { get; set; }

        // Relations
        public ICollection<Personnel> Staff { get; set; } = new List<Personnel>();
        public ICollection<Campagne> Campagnes { get; set; } = new List<Campagne>();
    }
}