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
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;
        private readonly ILogger<PaymentsController> _logger;

        public PaymentsController(IConfiguration configuration, AppDbContext context, ILogger<PaymentsController> logger)
        {
            _configuration = configuration;
            _context = context;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpPost("create-checkout-session")]
        public async Task<ActionResult<CheckoutSessionResponse>> CreateCheckoutSession([FromBody] CreateCheckoutSessionRequest request, [FromServices] ITenantService tenantService)
        {
            var enterpriseId = tenantService.GetTenantId() ?? Guid.Parse("00000000-0000-0000-0000-000000000001");
            
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
                            UnitAmount = (long)(request.Price * 100), 
                            Currency = "eur", 
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

            var service = new SessionService();
            Session session = await service.CreateAsync(options);

            return Ok(new CheckoutSessionResponse
            {
                SessionId = session.Id,
                PublicKey = _configuration["Stripe:PublishableKey"] ?? "",
                Url = session.Url
            });
        }

        [AllowAnonymous]
        [HttpPost("confirm-session")]
        public async Task<IActionResult> ConfirmSession([FromQuery] string sessionId)
        {
            try
            {
                var service = new SessionService();
                var session = await service.GetAsync(sessionId);

                if (session.PaymentStatus == "paid")
                {
                    await HandleSubscriptionSuccess(session);
                    return Ok(new { message = "Plan mis à jour avec succès" });
                }
                return BadRequest("La session n'est pas payée.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [AllowAnonymous]
        [HttpGet("session-invoice/{sessionId}")]
        public async Task<IActionResult> GetSessionInvoice(string sessionId)
        {
            try
            {
                var service = new SessionService();
                var session = await service.GetAsync(sessionId, new SessionGetOptions
                {
                    Expand = new List<string> { "invoice", "subscription" }
                });

                if (session?.Invoice == null) return NotFound("Facture non trouvée.");

                return Ok(new { 
                    invoiceUrl = session.Invoice.InvoicePdf,
                    hostedUrl = session.Invoice.HostedInvoiceUrl
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
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
                    enterprise.AbonnementDebut ??= DateTime.UtcNow;
                    enterprise.AbonnementFin = (enterprise.AbonnementFin ?? DateTime.UtcNow).AddDays(30);
                    await _context.SaveChangesAsync();
                }
            }
        }
    }
}