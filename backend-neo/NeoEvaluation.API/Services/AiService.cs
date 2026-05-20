using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using NeoEvaluation.API.Models;
using NeoEvaluation.API.Data;

namespace NeoEvaluation.API.Services
{
    public class AiService
    {
        private readonly HttpClient _http;
        private readonly AppDbContext _db;
        private static readonly JsonSerializerOptions _jsonOpts = new() { PropertyNameCaseInsensitive = true };

        public AiService(HttpClient http, AppDbContext db, IConfiguration config)
        {
            _http = http;
            _db = db;
            var baseUrl = config["FastApi:BaseUrl"] ?? "http://127.0.0.1:8000";
            _http.BaseAddress = new Uri(baseUrl);
        }

        public async Task<CvAnalysisResult> AnalyzeCvAsync(IFormFile file, string jobDesc, string lang, Guid? candidatId, string? userId)
        {
            var result = await CallFastApiCvAsync(file, jobDesc, lang);
            if (!result.Is_Cv) return result;

            var entity = new CvAnalysis {
                CandidatId = candidatId,
                FileName = file.FileName,
                JobDescription = jobDesc,
                Lang = lang,
                Score = result.Score,
                Decision = result.Decision,
                IsCv = true,
                PointsForts = result.Points_Forts,
                PointsFaibles = result.Points_Faibles,
                Conseils = result.Conseils,
                CreatedByUserId = userId
            };

            _db.CvAnalyses.Add(entity);
            await _db.SaveChangesAsync();
            result.SavedId = entity.Id;
            return result;
        }

        public async Task<List<CvAnalysisResult>> GetCvHistoryAsync(Guid candidatId)
        {
            var cvHistory = await _db.CvAnalyses
                .Where(x => x.CandidatId == candidatId)
                .OrderByDescending(x => x.CreatedAt)
                .Select(e => new CvAnalysisResult {
                    Score = e.Score,
                    Decision = e.Decision,
                    Is_Cv = e.IsCv,
                    Points_Forts = e.PointsForts,
                    Points_Faibles = e.PointsFaibles,
                    Conseils = e.Conseils,
                    SavedId = e.Id,
                    CreatedAt = e.CreatedAt
                }).ToListAsync();

            // Récupérer les évaluations du candidat (tests passés)
            var evaluations = await _db.Evaluations
                .Include(e => e.Candidature)
                    .ThenInclude(c => c!.Campagne)
                .Where(e => (e.CandidatId == candidatId || e.Candidature.CandidatId == candidatId)
                            && (e.Statut == StatutPassage.TERMINE || e.RapportFinalIA != null))
                .OrderByDescending(e => e.DateFin)
                .ToListAsync();

            foreach (var eval in evaluations)
            {
                var pointsForts = new List<string>();
                var pointsFaibles = new List<string>();
                var conseils = new List<string>();

                // Génération de points forts/faibles dynamiques selon les scores par thèmes
                if (eval.ScoresParTheme != null && eval.ScoresParTheme.Count > 0)
                {
                    foreach (var theme in eval.ScoresParTheme)
                    {
                        if (theme.Value >= 80)
                            pointsForts.Add($"Rigueur et haute performance cognitive sur le thème : {theme.Key} ({Math.Round(theme.Value)}%)");
                        else if (theme.Value < 60)
                            pointsFaibles.Add($"Axe de vigilance managériale ou besoin d'accompagnement sur : {theme.Key} ({Math.Round(theme.Value)}%)");
                    }
                }

                // Fallback si pas de thèmes précis
                if (pointsForts.Count == 0)
                {
                    pointsForts.Add("Forte persévérance et capacité de concentration pendant le passage du test.");
                    pointsForts.Add("Structure de raisonnement solide démontrée par les réponses.");
                    pointsForts.Add("Bonne gestion du temps imparti sous pression d'évaluation.");
                }
                if (pointsFaibles.Count == 0)
                {
                    pointsFaibles.Add("Risque potentiel de doute ou d'indécision face aux questions ouvertes complexes.");
                    pointsFaibles.Add("Axe de progression sur l'application opérationnelle directe des concepts.");
                }

                // Conseils managériaux adaptés à son score
                var score = eval.ScorePourcentage;
                if (score >= 85)
                {
                    conseils.Add("Recommandation managériale : Profil à forte autonomie. Lui confier des responsabilités techniques claires dès son intégration.");
                    conseils.Add("Entretien prédictif : Valider son adéquation avec la culture d'entreprise et ses ambitions de leadership.");
                    conseils.Add("Onboarding idéal : Permettre une prise de contact rapide avec des challenges stimulants et peu de micro-management.");
                }
                else if (score >= 70)
                {
                    conseils.Add("Recommandation managériale : Profil collaboratif fiable. Structurer son intégration avec des objectifs progressifs.");
                    conseils.Add("Entretien prédictif : Creuser sa résistance au stress sur des projets à délais très serrés.");
                    conseils.Add("Onboarding idéal : Assurer un tutorat technique axé sur le partage des bonnes pratiques internes.");
                }
                else
                {
                    conseils.Add("Recommandation managériale : Profil nécessitant un encadrement régulier et rassurant.");
                    conseils.Add("Entretien prédictif : Evaluer sa motivation réelle d'apprentissage et sa réceptivité aux feedbacks constructifs.");
                    conseils.Add("Onboarding idéal : Planifier un programme de formation initiale rigoureux de 30 jours.");
                }

                string campagneTitre = eval.Candidature?.Campagne?.Nom ?? "Test standard";
                string rapportText = !string.IsNullOrWhiteSpace(eval.RapportFinalIA) 
                    ? eval.RapportFinalIA 
                    : $"Analyse prédictive des réponses au test '{campagneTitre}' (Score global : {Math.Round(eval.ScorePourcentage)}%).";

                cvHistory.Add(new CvAnalysisResult
                {
                    Score = (int)Math.Round(eval.ScorePourcentage),
                    Decision = rapportText,
                    Is_Cv = false,
                    Points_Forts = pointsForts,
                    Points_Faibles = pointsFaibles,
                    Conseils = conseils,
                    SavedId = null,
                    CreatedAt = eval.DateFin ?? eval.DateDebut ?? DateTime.UtcNow
                });
            }

            return cvHistory.OrderByDescending(x => x.CreatedAt).ToList();
        }

        private async Task<CvAnalysisResult> CallFastApiCvAsync(IFormFile file, string jobDesc, string lang)
        {
            using var content = new MultipartFormDataContent();
            var fileContent = new StreamContent(file.OpenReadStream());
            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(file.ContentType ?? "application/octet-stream");
            content.Add(fileContent, "file", file.FileName);
            content.Add(new StringContent(jobDesc), "job_description");
            content.Add(new StringContent(lang), "lang");

            var resp = await _http.PostAsync("/ia/match-cv", content);
            if (!resp.IsSuccessStatusCode && resp.StatusCode != System.Net.HttpStatusCode.UnprocessableEntity)
            {
                throw new Exception("FastAPI unreachable");
            }

            var json = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<CvAnalysisResult>(json, _jsonOpts) ?? new();
        }

        public async Task<AiEvaluationResponse> EvaluateExamAsync(object payload)
        {
            var content = new StringContent(JsonSerializer.Serialize(payload), System.Text.Encoding.UTF8, "application/json");
            var resp = await _http.PostAsync("/ia/evaluate-exam", content);
            if (!resp.IsSuccessStatusCode) throw new Exception("AI Engine unreachable");

            var json = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<AiEvaluationResponse>(json, _jsonOpts) ?? new AiEvaluationResponse();
        }

        public async Task<string> GetRecommendationsAsync(object payload)
        {
            var content = new StringContent(JsonSerializer.Serialize(payload), System.Text.Encoding.UTF8, "application/json");
            var resp = await _http.PostAsync("/ia/recommendations", content);
            if (!resp.IsSuccessStatusCode) throw new Exception("AI Engine unreachable");

            return await resp.Content.ReadAsStringAsync();
        }
    }

    public class AiEvaluationResponse {
        public string Status { get; set; } = string.Empty;
        public AiEvaluationData? Evaluation { get; set; }
    }
    public class AiEvaluationData {
        public float ScorePourcentage { get; set; }
        public string RapportFinal { get; set; } = string.Empty;
        public List<AiCorrectionItem> Corrections { get; set; } = new();
    }
    public class AiCorrectionItem {
        public string QuestionId { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
        public string CandidateAnswer { get; set; } = string.Empty;
        public string CorrectAnswer { get; set; } = string.Empty;
        public string Explication { get; set; } = string.Empty;
    }
}