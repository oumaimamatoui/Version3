using System;

namespace NeoEvaluation.API.Models
{
    public interface IMultiTenant
    {
        Guid? EntrepriseId { get; set; }
    }
}
