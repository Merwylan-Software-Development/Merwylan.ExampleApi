using System.Collections.Generic;
using System.Threading.Tasks;

namespace Merwylan.ExampleApi.Audit
{
    public interface IAuditService
    {
        Task<bool> AddAuditTrailAsync(AuditPostModel model);
        IEnumerable<AuditGetModel> GetAuditModels(AuditSearchModel search);
    }
}