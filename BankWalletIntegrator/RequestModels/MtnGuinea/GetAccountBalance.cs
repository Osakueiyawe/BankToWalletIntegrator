

using BankWalletIntegrator.Models;
using System.ComponentModel.DataAnnotations;

namespace BankWalletIntegrator.RequestModels.MtnGuinea
{
    public class GetAccountBalance:MerchantBaseRequest
    {
        [Required]
        public string customerMsisdn { get; set; }
    }
}
