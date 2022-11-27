using BankWalletIntegrator.Models;
using System.ComponentModel.DataAnnotations;

namespace BankWalletIntegrator.RequestModels.MtnGuinea
{
    public class WalletToBank:MerchantBaseRequest
    {
        public decimal amount { get; set; }
        public string externalReference { get; set; }
        [Required]
        public string customerMsisdn { get; set; }
    }
}
