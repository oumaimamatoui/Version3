using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using NeoEvaluation.API.Services;
using NeoEvaluation.API.Models;

namespace NeoEvaluation.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AiController : ControllerBase
    {
        private readonly AiService _aiService;
        public AiController(AiService aiService) => _aiService = aiService;

        [HttpPost("analyze-cv")]
        public async Task<IActionResult> AnalyzeCv([FromForm] CvAnalysisRequest req)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _aiService.AnalyzeCvAsync(req.File, req.JobDescription, req.Lang, req.CandidatId, userId);
            return Ok(result);
        }

        [HttpGet("cv-history/{candidatId:guid}")]
        public async Task<IActionResult> GetCvHistory(Guid candidatId)
        {
            var history = await _aiService.GetCvHistoryAsync(candidatId);
            return Ok(history);
        }
    }
}