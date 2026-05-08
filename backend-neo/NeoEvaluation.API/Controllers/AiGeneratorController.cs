using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NeoEvaluation.API.Services;

namespace NeoEvaluation.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AiGeneratorController : ControllerBase
    {
        private readonly IQuotaService _quotaService;
        private readonly ITenantService _tenantService;

        public AiGeneratorController(IQuotaService quotaService, ITenantService tenantService)
        {
            _quotaService = quotaService;
            _tenantService = tenantService;
        }

        [HttpPost("generate-questions")]
        public async Task<IActionResult> Generate()
        {
            // 1. Récupérer l'ID de l'entreprise via le service de Tenant
            var entrepriseId = _tenantService.GetTenantId();
            if (entrepriseId == null) return Unauthorized(new { message = "Entreprise non identifiée." });

            // 2. CONSOMMER LE QUOTA (Vérifie si l'entreprise a encore des crédits aujourd'hui)
            var quota = await _quotaService.ConsumeQuotaAsync(entrepriseId.Value);

            if (!quota.Success)
            {
                // Retourne l'erreur 429 (Too Many Requests) si la limite est dépassée
                return StatusCode(429, new { message = quota.Message });
            }

            // 3. SI OK -> APPEL À TON SERVICE IA ICI (OpenAI, Anthropic, etc.)
            // var questions = await _openaiService.CreateQuestions(...);

            return Ok(new { 
                message = "Questions générées avec succès", 
                remaining = quota.Remaining 
            });
        }
    }
}