using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NeoEvaluation.API.Models
{
    [Table("CvAnalyses")]
    public class CvAnalysis
    {
        [Key]
        public int Id { get; set; }
        public Guid? CandidatId { get; set; } // Type Guid pour correspondre aux Utilisateurs
        public string FileName { get; set; } = string.Empty;
        public string JobDescription { get; set; } = string.Empty;
        public string Lang { get; set; } = "fr";
        public int Score { get; set; }
        public string Decision { get; set; } = string.Empty;
        public bool IsCv { get; set; }
        public List<string> PointsForts { get; set; } = new();
        public List<string> PointsFaibles { get; set; } = new();
        public List<string> Conseils { get; set; } = new();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? CreatedByUserId { get; set; }
    }
}