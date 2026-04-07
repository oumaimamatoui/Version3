using System.ComponentModel.DataAnnotations;

namespace NeoEvaluation.API.Models
{
    public class Questionnaire
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string Titre { get; set; } = string.Empty; // Vérifiez que c'est bien 'Titre'

        public string Description { get; set; } = string.Empty; // Vérifiez que c'est bien 'Description'

        public DateTime CreeLe { get; set; } = DateTime.UtcNow;

        // Relation avec les questions
        public ICollection<Question> Questions { get; set; } = new List<Question>();
    }
}