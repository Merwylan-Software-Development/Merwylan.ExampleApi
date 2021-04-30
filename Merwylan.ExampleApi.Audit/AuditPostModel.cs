namespace Merwylan.ExampleApi.Audit
{
    public class AuditPostModel
    {
        public AuditTypes Type { get; set; }
        public string Description { get; set; }
        public int? StatusCode { get; set; }
        public bool? IsSuccessful { get; set; }
        public object Object { get; set; }
    }
}
