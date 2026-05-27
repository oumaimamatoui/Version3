using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace NeoEvaluation.API.DTOs
{
    public record LoginDto(
        [property: JsonPropertyName("email")] string Email, 
        [property: JsonPropertyName("password")] string Password
    );
}