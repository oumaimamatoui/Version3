namespace NeoEvaluation.API.DTOs
{
    public class ActivateDto
    {
        public Guid Token { get; set; }

        // " = string.Empty;"
        public string Password { get; set; } = string.Empty;
    }
}