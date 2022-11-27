namespace BankWalletIntegrator.Models
{
    public class TransactionStatusRequest:BaseRequest
    {
        public string ClientReferenceId { get; set; }
    }
}
