using System;
using System.Collections.Generic;

namespace NeoEvaluation.API.DTOs
{
    public class ExamSetupDto
    {
        public Guid EvaluationId { get; set; }
        public required string CampagneNom { get; set; }
        public int TempsLimite { get; set; }
        public required List<QuestionItemDto> Questions { get; set; }
    }

    public class QuestionItemDto
    {
        public Guid Id { get; set; }
        public required string Enonce { get; set; }
        public int Type { get; set; }
        public int Points { get; set; }
        public required List<string> Options { get; set; }
    }

    // C'EST CETTE CLASSE QUI MANQUE :
    public class SyncDto
    {
        public Guid EvaluationId { get; set; }
    }

    public class SubmitAnswerDto
    {
        public Guid EvaluationId { get; set; }
        public Guid QuestionId { get; set; }
        public string? TexteReponse { get; set; }
        public List<int>? SelectedIndexes { get; set; }
    }
}