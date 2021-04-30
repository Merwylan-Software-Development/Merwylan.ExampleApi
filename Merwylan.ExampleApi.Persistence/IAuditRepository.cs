using System.Linq;
using System.Threading.Tasks;
using Merwylan.ExampleApi.Persistence.Entities;

namespace Merwylan.ExampleApi.Persistence
{
    public interface IAuditRepository
    {
        Task<AuditTrail> AddAuditTrailAsync(AuditTrail trail);
        IQueryable<AuditTrail> Get();
        Task SaveAsync();
    }
}