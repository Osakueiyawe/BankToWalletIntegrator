using System.Data;
using System.Threading.Tasks;

namespace BankWalletIntegrator.Interfaces
{
    public interface IBankToWalletMerchants
    {
        Task<DataTable> GetMerchantDetails(long merchantId);
        Task<DataTable> GetMerchantAccountBalanceFees(long merchantId);
        Task<DataTable> GetMerchantMiniStatementFees(long merchantId);
        Task<DataTable> GetMerchantBankToWalletFees(long merchantId);
        Task<DataTable> GetMerchantWalletToBankFees(long merchantId);
    }
}
