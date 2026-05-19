// CONTENU DE AiDtos.cs
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
 
namespace NeoEvaluation.API.Models
{
    public class CvAnalysisRequest
    {
        public IFormFile File { get; set; } = null!;
        public string JobDescription { get; set; } = string.Empty;
        public string Lang { get; set; } = "fr";
        public Guid? CandidatId { get; set; }
    }
 
    public class CvAnalysisResult
    {
        public int Score { get; set; }
        public List<string> Points_Forts   { get; set; } = new();
        public List<string> Points_Faibles { get; set; } = new();
        public string Decision { get; set; } = string.Empty;
        public List<string> Conseils { get; set; } = new();
        public bool Is_Cv { get; set; }
        public CvAlert? Alert { get; set; }
        public int? SavedId { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class CvAlert
    {
        public string Title { get; set; } = "";
        public string Subtitle { get; set; } = "";
    }

    // Ajoutez ici LetterRequest et LetterResult si besoin
}