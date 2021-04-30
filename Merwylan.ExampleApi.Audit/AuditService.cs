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
            var trails = _auditRepository.Get()
                .Where(x => search.Type == null || x.Type == search.Type)
                .Where(x => search.IsSuccessful == null || x.IsSuccessful == search.IsSuccessful)
                .Where(x => search.StatusCode == null || x.StatusCode == search.StatusCode)
                .Where(x => search.SearchDescription == null || x.Description.Contains(search.SearchDescription))
                .Where(x => search.StartDate == null || x.Occurred >= search.StartDate)
                .Where(x => search.EndDate == null || x.Occurred <= search.EndDate);

            var models = trails.Select(x => new AuditGetModel
            {
                Id = x.Id,
                Description = x.Description,
                Occurred = x.Occurred,
                Type = x.Type.ToString(CultureInfo.InvariantCulture),
                IsSuccessful = x.IsSuccessful,
                StatusCode = x.StatusCode,
                Object = x.Object
            });

            return models;
        }

        public async Task<bool> AddAuditTrailAsync(AuditPostModel model)
        {
            try
            {
                await _auditRepository.AddAuditTrailAsync(new AuditTrail
                {
                    Id = Guid.NewGuid(),
                    Description = model.Description,
                    Occurred = DateTime.Now,
                    Type = model.Type.ToString(),
                    Object = model.Object.SerializeCamelCase(),
                    IsSuccessful = model.IsSuccessful,
                    StatusCode = model.StatusCode
                });

                await _auditRepository.SaveAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
