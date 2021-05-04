using System.Collections.Generic;
using System.Threading.Tasks;
using Merwylan.ExampleApi.Persistence.Entities;

namespace Merwylan.ExampleApi.Audit
{
    public interface IAuditService
    {
        Task<AuditTrail> AddAuditTrailAsync(AuditPostModel model);
        IEnumerable<AuditGetModel> GetAuditModels(AuditSearchModel search);
    }
}