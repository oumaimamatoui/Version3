namespace NeoEvaluation.API.DTOs
{
    public class ActivateDto
    {
        public Guid Token { get; set; }

        public string Password { get; set; } = string.Empty;
    }
}