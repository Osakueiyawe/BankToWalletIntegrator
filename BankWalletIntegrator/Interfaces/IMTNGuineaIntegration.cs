using BankWalletIntegrator.Models;
using BankWalletIntegrator.RequestModels.MtnGuinea;
using System.Threading.Tasks;

namespace BankWalletIntegrator.Interfaces
{
    public interface IMTNGuineaIntegration
    {
        Task<AccountBalanceResponse> ProcessAccountBalanceRequest(GetAccountBalance request);
        Task<MiniStatementResponse> ProcessMiniStatementRequest(GetMiniStatement request);
        Task<Response> ProcessBankToWalletTransfer(BankToWallet request);
        Task<Response> ProcessWalletToBankTransfer(WalletToBank request);
        Task<TransactionStatusResponse> ProcessTransactionInquiry(TransactionStatusInquiry request);
    }
}
