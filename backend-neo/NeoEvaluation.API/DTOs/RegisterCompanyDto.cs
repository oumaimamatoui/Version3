using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace NeoEvaluation.API.DTOs
{
    public class RegisterCompanyDto
    {
        [Required]
        [JsonPropertyName("nomEntreprise")] // Force la lecture du camelCase JS
        public string NomEntreprise { get; set; } = string.Empty;

        [Required]
        [JsonPropertyName("nomResponsable")]
        public string NomResponsable { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [JsonPropertyName("emailResponsable")]
        public string EmailResponsable { get; set; } = string.Empty;

        [JsonPropertyName("matriculeFiscale")]
        public string? MatriculeFiscale { get; set; }
    }
}