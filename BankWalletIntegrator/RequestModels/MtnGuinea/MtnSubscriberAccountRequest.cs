using BankWalletIntegrator.Models;

namespace BankWalletIntegrator.ResponseModels.MtnGuinea
{
    public class MtnSubscriberAccountRequest: BaseRequest
    {
        public string AccountAlias { get; set; }
    }    
}
