using System.ComponentModel.DataAnnotations;
namespace NeoEvaluation.API.DTOs {
    public class CreateCheckoutSessionRequest {
        [Required] public string PlanName { get; set; } = string.Empty;
        [Required] public decimal Price { get; set; }
    }
    public class CheckoutSessionResponse {
        public string SessionId { get; set; } = string.Empty;
        public string PublicKey { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
    }
}