using System;

namespace NeoEvaluation.API.Services
{
    public interface ITenantService
    {
        Guid? GetTenantId();
        Guid? GetUserId();
        string? GetUserRole();
    }
}
