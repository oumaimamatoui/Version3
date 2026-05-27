using System.Text.Json.Serialization; // Indispensable pour JsonPropertyName

namespace NeoEvaluation.API.DTOs
{
    // On garde seulement le record (pas besoin de classe autour)
    public record GoogleLoginDto(
        [property: JsonPropertyName("idToken")] string IdToken
    );
}