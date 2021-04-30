using System.Linq;
using System.Threading.Tasks;
using Merwylan.ExampleApi.Persistence.Entities;

namespace Merwylan.ExampleApi.Persistence
{
    public class AuditRepository : IAuditRepository
    {
        private readonly ExampleContext _context;

        public AuditRepository(ExampleContext context)
        {
            _context = context;
        }

        public IQueryable<AuditTrail> Get()
        {
            return _context.AuditTrails;
        }

        public async Task<AuditTrail> AddAuditTrailAsync(AuditTrail trail)
        {
            var insertedCompany = await _context.AuditTrails.AddAsync(trail);
            return insertedCompany.Entity;
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
