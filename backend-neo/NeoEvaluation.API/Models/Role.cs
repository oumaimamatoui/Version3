using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NeoEvaluation.API.Models
{
    public class Role : IMultiTenant
    {
        public Guid? EntrepriseId { get; set; }
        
        [Key]
        public Guid? Id { get; set; } = Guid.NewGuid();
        
        [Required]
        public string Nom { get; set; } = string.Empty;

        public string? Prenom { get; set; }
        public string? NomFamille { get; set; }
        public string? Email { get; set; }
        public string Description { get; set; } = string.Empty;
        
        public List<string> Permissions { get; set; } = new List<string>();
        
        public DateTime CreeLe { get; set; } = DateTime.UtcNow;

        [NotMapped]
        public int NombreMembres { get; set; } 
    }
}