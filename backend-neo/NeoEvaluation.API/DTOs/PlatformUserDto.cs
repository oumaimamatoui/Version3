using System;

namespace NeoEvaluation.API.DTOs
{
    public class PlatformUserDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Org { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string LastLogin { get; set; } = "Hors ligne";
        public bool IsActive { get; set; } = true;
    }
}
