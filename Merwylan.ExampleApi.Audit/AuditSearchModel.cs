﻿using System;

namespace Merwylan.ExampleApi.Audit
{
    public class AuditSearchModel
    {
        public string Method { get; set; }
        public string Endpoint { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? StatusCode { get; set; }
        public bool? IsSuccessful { get; set; }
    }
}
