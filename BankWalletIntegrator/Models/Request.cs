namespace BankWalletIntegrator.Models
{
    public class Request
    {
        public string requestId { get; set; }
        public string countryId { get; set; }        
        public string merchantId { get; set; }
        public string accountNumber { get; set; }
        public string merchantSuspenseAccount { get; set; }
        public string merchantCurrency { get; set; }
        public string ExternalRef { get; set; }
        public decimal amount { get; set; }
        public string braCode { get; set; }
        public string msisdn { get; set; }
        public string requestType { get; set; }
        public int statementCount { get; set; }
    }
}
