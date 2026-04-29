using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
//3azil les entrepriseId 
namespace NeoEvaluation.API.Services
{
    public class TenantService : ITenantService
    {
        private readonly HttpContext? _httpContext;

        public TenantService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContext = httpContextAccessor.HttpContext;
        }

        public Guid? GetTenantId()
        {
            if (_httpContext == null || _httpContext.User == null)
                return null;
        //claims li fih entrepriseId  m3lomat jya mn token w n7awlo l guid        

            var tenantClaim = _httpContext.User.Claims.FirstOrDefault(c => c.Type == "entrepriseId");
            
            if (tenantClaim != null && Guid.TryParse(tenantClaim.Value, out Guid tenantId))
            {
                return tenantId;
            }

            return null;
        }

        public Guid? GetUserId()
        {
            if (_httpContext?.User == null) return null;
            var idClaim = _httpContext.User.Claims.FirstOrDefault(c => c.Type == "id" || c.Type == System.Security.Claims.ClaimTypes.NameIdentifier);
            if (idClaim != null && Guid.TryParse(idClaim.Value, out Guid userId)) return userId;
            return null;
        }

        public string? GetUserRole()
        {
            if (_httpContext == null || _httpContext.User == null)
                return null;

            var roleClaim = _httpContext.User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role);
            return roleClaim?.Value;
        }
    }
}
