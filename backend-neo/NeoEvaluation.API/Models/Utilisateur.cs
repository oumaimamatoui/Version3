using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NeoEvaluation.API.Models
{
    public class Utilisateur : IMultiTenant
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
        public string ThemePreference { get; set; } = "light";

        public List<string> Privileges { get; set; } = new List<string>();

        // Fields for Personnel/Candidat
        public string? IdEmploye { get; set; }
        public string? Departement { get; set; }
        public string? Telephone { get; set; }
        public string? Adresse { get; set; }
        public bool? ProfilComplet { get; set; } = false;

        // Relations
        public virtual ICollection<Campagne> CampagnesGerees { get; set; } = new List<Campagne>();
        public virtual ICollection<Candidature> Candidatures { get; set; } = new List<Candidature>();
        public virtual ICollection<DocumentCandidat> Documents { get; set; } = new List<DocumentCandidat>();

        [NotMapped]
        public string NomComplet => $"{Prenom} {Nom}";
    }
}