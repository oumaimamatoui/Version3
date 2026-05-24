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

            return cvHistory.OrderByDescending(x => x.CreatedAt).ToList();
        }

        private async Task<CvAnalysisResult> CallFastApiCvAsync(IFormFile file, string jobDesc, string lang)
        {
            using var content = new MultipartFormDataContent();
            var fileContent = new StreamContent(file.OpenReadStream());
            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(file.ContentType ?? "application/octet-stream");
            content.Add(fileContent, "file", file.FileName);
            content.Add(new StringContent(jobDesc ?? ""), "job_description");
            content.Add(new StringContent(lang ?? "fr"), "lang");

            var resp = await _http.PostAsync("/ia/match-cv", content);
            var json = await resp.Content.ReadAsStringAsync();

            if (!resp.IsSuccessStatusCode && resp.StatusCode != System.Net.HttpStatusCode.UnprocessableEntity)
            {
                Console.WriteLine($"[AiService] FastAPI error {resp.StatusCode}: {json}");
                throw new Exception("FastAPI unreachable");
            }

            var result = JsonSerializer.Deserialize<CvAnalysisResult>(json, _jsonOpts) ?? new();
            return result;
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