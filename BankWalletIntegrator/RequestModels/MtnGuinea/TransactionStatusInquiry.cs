using BankWalletIntegrator.Models;

namespace BankWalletIntegrator.RequestModels.MtnGuinea
{
    public class TransactionStatusInquiry:MerchantBaseRequest
    {
        public string externalReference { get; set; }
    }
}
