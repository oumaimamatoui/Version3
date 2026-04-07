using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace NeoEvaluation.API.DTOs
{
    public class PlanningDto
    {
        [Required]
        [JsonPropertyName("campagneId")]
        public Guid CampagneId { get; set; }

        [Required]
        [Range(1, 480)]
        [JsonPropertyName("dureeMinutes")]
        public int DureeMinutes { get; set; }

        [Required]
        [JsonPropertyName("dateOuverture")]
        public DateTime DateOuverture { get; set; }

        [JsonPropertyName("modeNotation")]
        public string ModeNotation { get; set; } = "IA";
    }
}