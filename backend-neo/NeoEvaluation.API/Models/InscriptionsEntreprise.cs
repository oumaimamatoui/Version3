using System;
using System.ComponentModel.DataAnnotations;

namespace NeoEvaluation.API.Models
{
public class InscriptionsEntreprise
{
    [Key]
    public Guid Id { get; set; }

    public required string NomEntreprise { get; set; }
    public required string NomResponsable { get; set; }
    public required string PrenomResponsable { get; set; } // Nouveau
    public required string EmailResponsable { get; set; }
    
    public string? MatriculeFiscale { get; set; }

    // Nouveaux champs d'organisation récupérés lors de l'inscription
    public string? Domaine { get; set; }
    public string? Secteur { get; set; }
    public string? SiteWeb { get; set; }
    public string? Ville { get; set; }
    public string? Pays { get; set; }
    public string? CodePostal { get; set; }
    public string? Adresse { get; set; }
    public string? Description { get; set; }

    public string? Plan { get; set; }

    public int Statut { get; set; } = 0;
    public DateTime CreeLe { get; set; } = DateTime.UtcNow;
}
}