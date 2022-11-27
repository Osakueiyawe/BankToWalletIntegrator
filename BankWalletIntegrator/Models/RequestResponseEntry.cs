using System;

namespace BankWalletIntegrator.Models
{
    public class RequestResponseEntry
    {
        public string requestId { get; set; }
        public string requestContentType { get; set; }
        public string requestIpAddress { get; set; }
        public string requestMethod { get; set; }
        public string requestHeaders { get; set; }
        public DateTime requestTimestamp { get; set; }
        public string requestUri { get; set; }
        public string queryString { get; set; }
        public string responseContentBody { get; set; }
        public string responseContentType { get; set; }
        public string responseHeaders { get; set; }
        public DateTime responseTimestamp { get; set; }
        public int responseStatusCode { get; set; }
    }    
}
