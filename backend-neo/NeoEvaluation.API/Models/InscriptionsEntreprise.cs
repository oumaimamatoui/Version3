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
    public string? TelephoneResponsable { get; set; } // Nouveau
    
    public string? MatriculeFiscale { get; set; }

    public int Statut { get; set; } = 0;
    public DateTime CreeLe { get; set; } = DateTime.UtcNow;
}
}