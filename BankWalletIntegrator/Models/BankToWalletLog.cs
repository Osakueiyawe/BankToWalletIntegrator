using System;

namespace BankWalletIntegrator.Models
{
    public class BankToWalletLog
    {
        public string requestId { get; set; }
        public string countryId { get; set; }
        public string externalRefNo { get; set; }
        public string accountNo { get; set; }
        public string finnacleTransRef { get; set; }
        public string responseCode { get; set; }
        public string responseMessage { get; set; }
        public DateTime transactionTime { get; set; }        
    }
}
