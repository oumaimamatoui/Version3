using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NeoEvaluation.API.Data;
using NeoEvaluation.API.DTOs;
using NeoEvaluation.API.Models;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeoEvaluation.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public RegistrationController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // POST /api/registration
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterCompanyDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Splitting logic pour obtenir un prénom par défaut si possible
            var fullName = dto.NomResponsable ?? "Responsable";
            var parts = fullName.Trim().Split(' ');
            var prenom = parts.Length > 1 ? parts[0] : "Responsable";
            var nom = parts.Length > 1 ? string.Join(" ", parts.Skip(1)) : fullName;

            var chosenPlan = string.IsNullOrEmpty(dto.Plan) ? "Starter" : dto.Plan;
            var isFreePlan = chosenPlan.Equals("Starter", StringComparison.OrdinalIgnoreCase) || 
                             chosenPlan.Equals("Gratuit", StringComparison.OrdinalIgnoreCase);

            var registration = new InscriptionsEntreprise
            {
                NomEntreprise = dto.NomEntreprise,
                NomResponsable = nom,
                PrenomResponsable = prenom,
                EmailResponsable = dto.EmailResponsable,
                MatriculeFiscale = dto.MatriculeFiscale,
                Plan = chosenPlan,
                Statut = 0, // En attente
                PaymentStatus = isFreePlan ? 1 : 0 // Plan Gratuit est considéré payé immédiatement
            };

            _context.InscriptionsEntreprises.Add(registration);
            await _context.SaveChangesAsync();

            if (isFreePlan)
            {
                return Ok(new { Message = "Demande d'inscription enregistrée avec succès." });
            }

            // Plan Payant: Création de la Checkout Session de Stripe
            try
            {
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
                                UnitAmount = 9900, // 99.00 EUR (DT)
                                Currency = "eur", // Devise Stripe compatible
                                ProductData = new SessionLineItemPriceDataProductDataOptions
                                {
                                    Name = chosenPlan,
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
                        { "RegistrationId", registration.Id.ToString() },
                        { "PlanName", chosenPlan }
                    }
                };

                var service = new SessionService();
                Session session = await service.CreateAsync(options);

                registration.StripeSessionId = session.Id;
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    stripeUrl = session.Url,
                    registrationId = registration.Id
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Erreur lors de la création de la session Stripe: " + ex.Message });
            }
        }
    }
}
