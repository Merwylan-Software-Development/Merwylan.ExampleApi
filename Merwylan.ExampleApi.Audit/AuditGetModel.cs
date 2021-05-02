using System;

namespace Merwylan.ExampleApi.Audit
{
    public class AuditGetModel
    {
        public Guid Id { get; set; }
        public DateTime Occurred { get; set; }
        public string Method { get; set; }
        public string Endpoint { get; set; }
        public int StatusCode { get; set; }
        public bool IsSuccessful { get; set; }
        public string Object { get; set; }
    }
}
