using System.ComponentModel.DataAnnotations;

namespace BankWalletIntegrator.Models
{
    public class MerchantBaseRequest
    {
        [Required]
        public string merchantId { get; set; }
        [Required]
        public string requestId { get; set; }        
        [Required]
        public string requestType { get; set; }
    }
}
