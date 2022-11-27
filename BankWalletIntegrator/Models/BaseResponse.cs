namespace BankWalletIntegrator.Models
{
    public class BaseResponse
    {
        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public string RequestId { get; set; }
    }
}
