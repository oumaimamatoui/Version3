using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;

namespace NeoEvaluation.API.Attributes
{
    public class RequirePermissionAttribute : TypeFilterAttribute
    {
        public RequirePermissionAttribute(string permission) : base(typeof(RequirePermissionFilter))
        {
            Arguments = new object[] { permission };
        }
    }

    public class RequirePermissionFilter : IAuthorizationFilter
    {
        private readonly string _permission;

        public RequirePermissionFilter(string permission)
        {
            _permission = permission;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
            if (user == null || !user.Identity!.IsAuthenticated)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var userRole = user.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role)?.Value;
            
            // Les SuperAdmins et AdminEntreprise ont accès à tout par défaut
            if (userRole == "SuperAdmin" || userRole == "AdminEntreprise")
            {
                return;
            }

            // Vérifier si l'utilisateur possède la permission spécifique dans ses privilèges
            var hasPermission = user.Claims.Any(c => c.Type == "privilege" && c.Value == _permission);
            if (!hasPermission)
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
