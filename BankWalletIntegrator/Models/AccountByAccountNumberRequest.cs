namespace BankWalletIntegrator.Models
{
    public class AccountByAccountNumberRequest:BaseRequest
    {
        public string AccountNumber { get; set; }
    }
}
