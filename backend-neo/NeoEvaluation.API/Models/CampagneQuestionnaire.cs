using System;

namespace NeoEvaluation.API.Models
{
    public class CampagneQuestionnaire
    {
        public Guid CampagneId { get; set; }
        public Campagne Campagne { get; set; } = null!;

        public Guid QuestionnaireId { get; set; }
        public Questionnaire Questionnaire { get; set; } = null!;
    }
}
