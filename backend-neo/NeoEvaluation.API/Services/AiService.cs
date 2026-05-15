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
            return await _db.CvAnalyses
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
            if (!resp.IsSuccessStatusCode) throw new Exception("FastAPI unreachable");

            var json = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<CvAnalysisResult>(json, _jsonOpts) ?? new();
        }
    }
}