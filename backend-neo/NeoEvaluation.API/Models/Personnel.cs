using System;
using System.Collections.Generic;

namespace NeoEvaluation.API.Models
{
// Dans Models/Personnel.cs
public class Personnel : Utilisateur
{
    public string IdEmploye { get; set; } = string.Empty;
    public string Departement { get; set; } = string.Empty;

    // ✅ Supprimé d'ici car déplacé dans Utilisateur.cs pour tous les rôles
    
    public ICollection<Campagne> CampagnesGerees { get; set; } = new List<Campagne>();
}
}