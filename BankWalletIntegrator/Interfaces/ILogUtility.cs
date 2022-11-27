using BankWalletIntegrator.Models;
using System.Threading.Tasks;

namespace BankWalletIntegrator.Interfaces
{
    public interface ILogUtility
    {
        Task LogBankToWallet(BankToWalletLog logDetails);
        Task LogWalletToBank(WalletToBankLog logDetails);
    }
}
