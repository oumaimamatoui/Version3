using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NeoEvaluation.API.Models
{
    // Modèle pour la Catégorie Parente (ex: Backend)
    public class Categorie
    {
        [Key]
        public Guid Id { get; set; }

        public Guid EntrepriseId { get; set; } // Pour le Multi-tenant

        [Required]
        [StringLength(100)]
        public string Nom { get; set; } = string.Empty;

        // Relation : Une catégorie a plusieurs sous-catégories
        public virtual ICollection<SousCategorie> SousCategories { get; set; } = new List<SousCategorie>();
    }

    // Modèle pour la Sous-Catégorie (ex: Node.js)
    public class SousCategorie
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nom { get; set; } = string.Empty;

        // Clé étrangère vers la catégorie parente
        public Guid CategorieId { get; set; }

        [ForeignKey("CategorieId")]
        public virtual Categorie? Categorie { get; set; }
    }
}