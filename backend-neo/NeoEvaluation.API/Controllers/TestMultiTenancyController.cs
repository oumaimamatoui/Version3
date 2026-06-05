// Ce contrôleur de test a été désactivé pour la sécurité de la plateforme.
// Les routes de seed non authentifiées représentent un risque de sécurité en production.
using Microsoft.AspNetCore.Mvc;

namespace NeoEvaluation.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestMultiTenancyController : ControllerBase
    {
        [HttpPost("seed-test-data")]
        public IActionResult SeedTestData()
        {
            return StatusCode(410, new { message = "Cette route de test a été désactivée." });
        }
    }
}
