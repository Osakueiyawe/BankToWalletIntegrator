namespace BankWalletIntegrator.Models
{
    public class AccountBalanceResponse:Response
    {
        public string availableBalance { get; set; }
        public string bookBalance { get; set; }
        public string msisdn { get; set; }
    }
}
