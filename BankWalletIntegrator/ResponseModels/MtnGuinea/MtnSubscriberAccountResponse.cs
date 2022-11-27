using BankWalletIntegrator.Models;

namespace BankWalletIntegrator.ResponseModels.MtnGuinea
{
    public class MtnSubscriberAccountResponse:Response
    {
        public string AccountNumber { get; set; }
        public string MobileNumber { get; set; }
    }    
}
