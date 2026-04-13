// Models/Question.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace NeoEvaluation.API.Models
{
    public class Question
    {
        [Key] 
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required] 
        public string Texte { get; set; } = string.Empty;

        public int Type { get; set; } 

        public double Poids { get; set; }

        public string BonneReponse { get; set; } = string.Empty;

        // تم التغيير هنا: أزلنا [Required] وجعلنا الحقل يقبل Null
        // هذا يحل مشكلة الـ 500 عند جلب الأسئلة التي ليس لها استبيان
        public Guid? QuestionnaireId { get; set; } 

        [JsonIgnore] 
        [ForeignKey("QuestionnaireId")]
        public Questionnaire? Questionnaire { get; set; }
    }
}