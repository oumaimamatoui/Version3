using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NeoEvaluation.API.Services;

namespace NeoEvaluation.API.Controllers
{
    public class ContactFormDto
    {
        public string Nom { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Entreprise { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }

    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class ContactController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public ContactController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost]
        public async Task<IActionResult> SendContactEmail([FromBody] ContactFormDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nom) || string.IsNullOrWhiteSpace(dto.Email))
                return BadRequest(new { message = "Nom et email sont requis." });

            string subject = $"[Enterprise] Nouvelle demande de devis - {dto.Entreprise}";

            string body = $@"
                <div style='font-family: sans-serif; max-width: 600px; margin: auto; border: 1px solid #e2e8f0; border-radius: 16px; padding: 40px;'>
                    <div style='display:flex; align-items:center; gap:12px; margin-bottom:24px;'>
                        <div style='background:#e0f2fe; border-radius:12px; width:48px; height:48px; display:flex; align-items:center; justify-content:center;'>
                            <span style='font-size:24px;'>📋</span>
                        </div>
                        <div>
                            <span style='background:#e0f2fe; color:#0369a1; padding:3px 12px; border-radius:8px; font-size:11px; font-weight:800; text-transform:uppercase; letter-spacing:1px;'>Enterprise</span>
                            <h2 style='color:#0f172a; margin:6px 0 0;'>Nouvelle demande de contact</h2>
                        </div>
                    </div>

                    <table style='width:100%; border-collapse:collapse;'>
                        <tr style='border-bottom:1px solid #f1f5f9;'>
                            <td style='padding:12px 0; font-weight:700; color:#374151; width:140px;'>👤 Nom</td>
                            <td style='padding:12px 0; color:#0f172a;'>{dto.Nom}</td>
                        </tr>
                        <tr style='border-bottom:1px solid #f1f5f9;'>
                            <td style='padding:12px 0; font-weight:700; color:#374151;'>📧 Email</td>
                            <td style='padding:12px 0; color:#0f172a;'><a href='mailto:{dto.Email}' style='color:#0369a1;'>{dto.Email}</a></td>
                        </tr>
                        <tr style='border-bottom:1px solid #f1f5f9;'>
                            <td style='padding:12px 0; font-weight:700; color:#374151;'>🏢 Entreprise</td>
                            <td style='padding:12px 0; color:#0f172a;'>{dto.Entreprise}</td>
                        </tr>
                    </table>

                    <div style='margin-top:24px; background:#f8fafc; border-radius:12px; padding:20px;'>
                        <p style='font-weight:700; color:#374151; margin:0 0 10px;'>💬 Message :</p>
                        <p style='color:#475569; line-height:1.7; margin:0;'>{(string.IsNullOrWhiteSpace(dto.Message) ? "<em>Aucun message fourni.</em>" : dto.Message)}</p>
                    </div>

                    <p style='margin-top:24px; font-size:12px; color:#94a3b8; text-align:center;'>EvaluaTech Platform — Demande Enterprise reçue le {DateTime.Now:dd/MM/yyyy à HH:mm}</p>
                </div>";

            try
            {
                await _emailService.SendEmailAsync("contactevaluatech@gmail.com", subject, body);
                return Ok(new { message = "Message envoyé avec succès." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ContactController] Erreur envoi email: {ex.Message}");
                return StatusCode(500, new { message = "Erreur lors de l'envoi. Réessayez plus tard." });
            }
        }
    }
}
