namespace BankWalletIntegrator.Models
{
    public class FinnaclePostResponse:BaseResponse
    {
        public string FinacleTranId { get; set; }
        public string TransactionReference { get; set; }
    }
}
