using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NeoEvaluation.API.Data;
using NeoEvaluation.API.DTOs;
using NeoEvaluation.API.Models;
using NeoEvaluation.API.Services;
using Stripe;
using Stripe.Checkout;
using Microsoft.EntityFrameworkCore;

namespace NeoEvaluation.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;
        private readonly ITenantService _tenantService;
        private readonly ILogger<PaymentsController> _logger;

        public PaymentsController(IConfiguration configuration, AppDbContext context, ITenantService tenantService, ILogger<PaymentsController> logger)
        {
            _configuration = configuration;
            _context = context;
            _tenantService = tenantService;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpPost("create-checkout-session")]
        public async Task<ActionResult<CheckoutSessionResponse>> CreateCheckoutSession([FromBody] CreateCheckoutSessionRequest request)
        {
            var enterpriseId = _tenantService.GetTenantId() ?? Guid.Parse("00000000-0000-0000-0000-000000000001");
            
            var domain = _configuration["AppSettings:FrontendUrl"] ?? "http://localhost:5173";

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(request.Price * 100), // En centimes
                            Currency = "eur", // Ou "tnd" si supporté par votre compte Stripe
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = request.PlanName,
                            },
                            Recurring = new SessionLineItemPriceDataRecurringOptions
                            {
                                Interval = "month",
                            },
                        },
                        Quantity = 1,
                    },
                },
                Mode = "subscription",
                SuccessUrl = domain + "/payment/success?session_id={CHECKOUT_SESSION_ID}",
                CancelUrl = domain + "/payment/cancel",
                Metadata = new Dictionary<string, string>
                {
                    { "EntrepriseId", enterpriseId.ToString() },
                    { "PlanName", request.PlanName }
                }
            };

            // Ajout de la période d'essai de 14 jours pour le plan Business IA ou EvaluaTech Go
            if (request.PlanName.Contains("Business IA", StringComparison.OrdinalIgnoreCase) || request.PlanName.Contains("EvaluaTech Go", StringComparison.OrdinalIgnoreCase))
            {
                options.SubscriptionData = new SessionSubscriptionDataOptions
                {
                    TrialPeriodDays = 14
                };
            }

            var secretKey = _configuration["Stripe:SecretKey"];
            if (string.IsNullOrEmpty(secretKey))
            {
                _logger.LogError("Stripe SecretKey is missing in configuration.");
                return StatusCode(500, "Configuration error: Stripe SecretKey is missing.");
            }

            var stripeClient = new StripeClient(secretKey);
            var service = new SessionService(stripeClient);
            Session session = await service.CreateAsync(options);

            return Ok(new CheckoutSessionResponse
            {
                SessionId = session.Id,
                PublicKey = _configuration["Stripe:PublishableKey"] ?? "",
                Url = session.Url
            });
        }

        [AllowAnonymous]
        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            try
            {
                var stripeEvent = EventUtility.ConstructEvent(json,
                    Request.Headers["Stripe-Signature"], _configuration["Stripe:WebhookSecret"]);

                // Handle the event
                if (stripeEvent.Type == EventTypes.CheckoutSessionCompleted)
                {
                    if (stripeEvent.Data.Object is Session session)
                    {
                        await HandleSubscriptionSuccess(session);
                    }
                }

                return Ok();
            }
            catch (StripeException e)
            {
                _logger.LogError(e, "Stripe Webhook Error");
                return BadRequest();
            }
        }

        private async Task HandleSubscriptionSuccess(Session session)
        {
            if (session?.Metadata == null || !session.Metadata.ContainsKey("EntrepriseId")) return;

            var enterpriseIdStr = session.Metadata["EntrepriseId"];
            var planName = session.Metadata.ContainsKey("PlanName") ? session.Metadata["PlanName"] : "Business IA";

            if (Guid.TryParse(enterpriseIdStr, out Guid enterpriseId))
            {
                var enterprise = await _context.Entreprises.FindAsync(enterpriseId);
                if (enterprise != null)
                {
                    enterprise.Plan = planName;
                    // Mise à jour de la date d'expiration (simplifié)
                    var hasTrial = planName.Contains("Business IA") || planName.Contains("EvaluaTech Go");
                    enterprise.AbonnementFin = DateTime.UtcNow.AddDays(30 + (hasTrial ? 14 : 0));
                    
                    // Reset des limites lors du passage au payant
                    enterprise.UsageCount = 0;
                    enterprise.IsUsageSuspended = false;
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"Abonnement activé pour l'entreprise {enterpriseId} : {planName}");
                }
            }
        }

        [HttpGet("confirm-session")]
        public async Task<IActionResult> ConfirmSession(string session_id)
        {
            var secretKey = _configuration["Stripe:SecretKey"];
            var stripeClient = new StripeClient(secretKey);
            var service = new SessionService(stripeClient);
            
            try 
            {
                var session = await service.GetAsync(session_id);
                if (session.PaymentStatus == "paid")
                {
                    await HandleSubscriptionSuccess(session);
                    return Ok(new { message = "Plan mis à jour avec succès" });
                }
                return BadRequest("Le paiement n'a pas été confirmé.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}