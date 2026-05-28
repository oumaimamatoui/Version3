using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NeoEvaluation.API.Services;
using System;
using System.Threading.Tasks;

namespace NeoEvaluation.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UsageController : ControllerBase
    {
        private readonly IUsageService _usageService;
        private readonly ITenantService _tenantService;

        public UsageController(IUsageService usageService, ITenantService tenantService)
        {
            _usageService = usageService;
            _tenantService = tenantService;
        }

        [HttpGet("status")]
        public async Task<IActionResult> GetStatus()
        {
            var enterpriseId = _tenantService.GetTenantId();
            if (enterpriseId == null) return Forbid();

            var status = await _usageService.GetUsageStatusAsync(enterpriseId.Value);
            return Ok(status);
        }

        [HttpPost("validate-action")]
        public async Task<IActionResult> ValidateAction()
        {
            var enterpriseId = _tenantService.GetTenantId();
            if (enterpriseId == null) return Forbid();

            var result = await _usageService.CheckAndIncrementUsageAsync(enterpriseId.Value);
            if (!result.Allowed)
            {
                var seconds = 0;
                if (result.Message.StartsWith("RETRY_IN_"))
                {
                    int.TryParse(result.Message.Replace("RETRY_IN_", ""), out seconds);
                }

                return StatusCode(403, new { 
                    error = "LIMIT_REACHED", 
                    message = result.Message,
                    retryAfterSeconds = seconds
                });
            }

            return Ok(new { message = "Action autorisée" });
        }

        [HttpGet("can-create-campaign")]
        public async Task<IActionResult> CanCreateCampaign()
        {
            var enterpriseId = _tenantService.GetTenantId();
            if (enterpriseId == null) return Forbid();

            var result = await _usageService.CheckCampaignLimitAsync(enterpriseId.Value);
            if (!result.Allowed)
            {
                var seconds = 0;
                if (result.Message.StartsWith("RETRY_IN_"))
                {
                    int.TryParse(result.Message.Replace("RETRY_IN_", ""), out seconds);
                }

                return StatusCode(403, new { 
                    error = "LIMIT_CAMPAIGN_REACHED", 
                    message = "Limite de 3 campagnes atteinte.",
                    retryAfterSeconds = seconds
                });
            }

            return Ok(new { message = "Création autorisée" });
        }
    }
}