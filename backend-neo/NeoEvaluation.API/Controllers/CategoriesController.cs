using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using NeoEvaluation.API.Data;
using NeoEvaluation.API.Models;
using NeoEvaluation.API.Services;

namespace NeoEvaluation.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ITenantService _tenantService;

        public CategoriesController(AppDbContext context, ITenantService tenantService)
        {
            _context = context;
            _tenantService = tenantService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            // Correction ici : récupération du Guid (en forçant la valeur si non null)
            var tenantId = _tenantService.GetTenantId() ?? Guid.Empty;

            var categories = await _context.Categories
                .Where(c => c.EntrepriseId == tenantId)
                .Include(c => c.SousCategories) 
                .OrderBy(c => c.Nom)
                .ToListAsync();

            return Ok(categories);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CategoryRequest dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nom))
                return BadRequest("Le nom est obligatoire.");

            var category = new Categorie
            {
                Id = Guid.NewGuid(),
                // Correction ici : .Value car EntrepriseId dans le modèle n'est pas nullable
                EntrepriseId = _tenantService.GetTenantId() ?? Guid.Empty, 
                Nom = dto.Nom
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return Ok(category);
        }

        [HttpPost("{id}/sub")]
        public async Task<IActionResult> CreateSubCategory(Guid id, [FromBody] CategoryRequest dto)
        {
            var parentExists = await _context.Categories.AnyAsync(c => c.Id == id);
            if (!parentExists) return NotFound();

            var subCategory = new SousCategorie
            {
                Id = Guid.NewGuid(),
                CategorieId = id,
                Nom = dto.Nom
            };

            _context.SousCategories.Add(subCategory);
            await _context.SaveChangesAsync();
            return Ok(subCategory);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var cat = await _context.Categories.FindAsync(id);
            if (cat == null) return NotFound();

            _context.Categories.Remove(cat);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }

    public class CategoryRequest { public string Nom { get; set; } = string.Empty; }
}