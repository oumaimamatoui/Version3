using System.ComponentModel.DataAnnotations;

namespace NeoEvaluation.API.DTOs
{
    public class CandidateTableDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Group { get; set; } = string.Empty;
        public int Score { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    public class BulkInviteDto
    {
        [Required] public Guid CampagneId { get; set; }
        [Required] public List<string> Emails { get; set; } = new();
    }
}