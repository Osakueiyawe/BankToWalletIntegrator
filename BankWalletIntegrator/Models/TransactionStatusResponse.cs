namespace BankWalletIntegrator.Models
{
    public class TransactionStatusResponse:Response
    {
        public string finacleTranId { get; set; }
        public string transactionReference { get; set; }
    }
}
