using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Merwylan.ExampleApi.Persistence;
using System.Linq;
using Merwylan.ExampleApi.Persistence.Entities;
using Merwylan.ExampleApi.Shared.Extensions;

namespace Merwylan.ExampleApi.Audit
{
    public class AuditService : IAuditService
    {
        private readonly IAuditRepository _auditRepository;

        public AuditService(IAuditRepository auditRepository)
        {
            _auditRepository = auditRepository;
        }

        public IEnumerable<AuditGetModel> GetAuditModels(AuditSearchModel search)
        {
            return _auditRepository.Get()
                .Where(x => search.Method == null || x.Method == search.Method)
                .Where(x => search.Endpoint == null || x.Endpoint == search.Endpoint)
                .Where(x => search.IsSuccessful == null || x.IsSuccessful == search.IsSuccessful)
                .Where(x => search.StatusCode == null || x.StatusCode == search.StatusCode)
                .Where(x => search.StartDate == null || x.Occurred >= search.StartDate)
                .Where(x => search.EndDate == null || x.Occurred <= search.EndDate)
                .Select(x => new AuditGetModel
                {
                    Id = x.Id,
                    Occurred = x.Occurred,
                    Method = x.Method,
                    Endpoint = x.Endpoint,
                    IsSuccessful = x.IsSuccessful,
                    StatusCode = x.StatusCode,
                    Object = x.Request
                });
        }

        public async Task<AuditTrail> AddAuditTrailAsync(AuditPostModel model)
        {
            var addedTrail = await _auditRepository.AddAuditTrailAsync(new AuditTrail
                {
                    Id = Guid.NewGuid(),
                    Occurred = DateTime.Now,
                    Method = model.Method,
                    Endpoint = model.Endpoint,
                    Request = model.Request?.SerializeCamelCase(),
                    IsSuccessful = model.IsSuccessful,
                    StatusCode = model.StatusCode
                });

                await _auditRepository.SaveAsync();
                return addedTrail;
        }
    }
}
