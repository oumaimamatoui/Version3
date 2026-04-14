using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NeoEvaluation.API.Models
{
    public abstract class Utilisateur
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string Email { get; set; } = string.Empty;

        public string MotDePasseHash { get; set; } = string.Empty;
        public string Nom { get; set; } = string.Empty;
        public string Prenom { get; set; } = string.Empty;
        public string RoleNom { get; set; } = string.Empty;
        public bool EstActif { get; set; } = true;

        public Guid? RoleId { get; set; }
        public Role? Role { get; set; }

        public Guid? EntrepriseId { get; set; } 
        public Entreprise? Entreprise { get; set; }

        public DateTime CreeLe { get; set; } = DateTime.UtcNow;
        public DateTime? DerniereConnexion { get; set; }
        public string? PhotoUrl { get; set; }
        public string? Bio { get; set; }

        public List<string> Privileges { get; set; } = new List<string>();

        [NotMapped]
        public string NomComplet => $"{Prenom} {Nom}";
    }
}