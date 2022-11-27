namespace BankWalletIntegrator.Models
{
    public class MiniStatementRequest:BaseRequest
    {
        public string AccountNumber { get; set; }
        public string MobileNumber { get; set; }
        public bool SendSms { get; set; }
        public int TransactionsCount { get; set; }
    }
}
