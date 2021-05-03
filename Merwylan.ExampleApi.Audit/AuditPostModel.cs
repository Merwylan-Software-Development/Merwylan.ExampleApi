namespace Merwylan.ExampleApi.Audit
{
    public class AuditPostModel
    {
        public string Method { get; set; }
        public string Endpoint { get; set; }
        public int StatusCode { get; set; }
        public bool IsSuccessful { get; set; }
        public object Request { get; set; }
    }
}
