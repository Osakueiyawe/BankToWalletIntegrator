using BankWalletIntegrator.Models;
using System.Threading.Tasks;

namespace BankWalletIntegrator.Interfaces
{
    public interface IAccountUtility
    {
        Task<AccountBalanceResponse> GetAccountBalance(Request request);
        Task<MiniStatementResponse> GetMiniStatement(Request request);
        Task<Response> BankToWalletTransfer(Request request);
        Task<Response> WalletToBankTransfer(Request request);
        Task<TransactionStatusResponse> TransferStatusInquiry(Request request);
    }
}
