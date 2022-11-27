using BankWalletIntegrator.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankWalletIntegrator.Interfaces
{
    public interface IPostingModelUtility
    {
        Task<List<MultipleTransactions>> GenerateMiniStatementPostingModel(Request request);
        Task<List<MultipleTransactions>> GenerateAccountBalancePostingModel(Request request);
        Task<List<MultipleTransactions>> GenerateBankToWalletPostingModel(Request request);
        Task<List<MultipleTransactions>> GenerateWalletToBankPostingModel(Request request);
    }
}
