using System;
using System.Text.Json.Serialization; // Indispensable pour JsonPropertyName

namespace NeoEvaluation.API.DTOs
{
    public record CompleteActivationDto(
        [property: JsonPropertyName("token")] Guid Token, 
        [property: JsonPropertyName("password")] string Password
    );
}