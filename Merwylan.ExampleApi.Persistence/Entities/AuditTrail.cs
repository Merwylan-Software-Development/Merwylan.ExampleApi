using System;
using System.ComponentModel.DataAnnotations;

namespace Merwylan.ExampleApi.Persistence.Entities
{
    public class AuditTrail
    {
        [Key]
        public Guid Id { get; set; }
        public DateTime Occurred { get; set; }
        public string Type { get; set; } = null!;
        public string? Description { get; set; }
        public int? StatusCode { get; set; }
        public bool? IsSuccessful { get; set; }
        public string? Object { get; set; } 
    }
}
