using System;
using System.ComponentModel.DataAnnotations;

namespace Merwylan.ExampleApi.Persistence.Entities
{
    public class AuditTrail
    {
        [Key]
        public Guid Id { get; set; }
        public DateTime Occurred { get; set; }
        public string Method { get; set; }
        public string Endpoint { get; set; } = null!;
        public int StatusCode { get; set; }
        public bool IsSuccessful { get; set; }
        public string? Request { get; set; } 
    }
}
