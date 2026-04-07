using System;
using System.Collections.Generic;

namespace NeoEvaluation.API.Models
{
    public class Candidat : Utilisateur
    {
        public string Telephone { get; set; } = string.Empty;
        public string Adresse { get; set; } = string.Empty;
        public bool ProfilComplet { get; set; } = false;

        // Relations
        public ICollection<Candidature> Candidatures { get; set; } = new List<Candidature>();
        public ICollection<DocumentCandidat> Documents { get; set; } = new List<DocumentCandidat>();
    }
}