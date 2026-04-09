using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NeoEvaluation.API.Models
{
    public class EntrepriseParSA
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        // Relation optionnelle vers l'entreprise finale
        public Guid? EntrepriseId { get; set; }
        [ForeignKey("EntrepriseId")]
        public virtual Entreprise? Entreprise { get; set; }

        // Détails de l'organisation
        public string? Domaine { get; set; }
        public string? Industrie { get; set; }
        public string? SiteWeb { get; set; }
        public string? Adresse { get; set; }
        public string? Ville { get; set; }
        public string? Pays { get; set; }
        public string? CodePostal { get; set; }
        public string? Description { get; set; }

        // Détails du responsable (Admin) saisis par le SuperAdmin
        public string? NomResponsable { get; set; }
        public string? PrenomResponsable { get; set; }
        public string? EmailResponsable { get; set; }
        public string? TelephoneResponsable { get; set; }

        public DateTime CreeLe { get; set; } = DateTime.UtcNow;
    }
}
