using BankWalletIntegrator.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankWalletIntegrator.Interfaces
{
    public interface IFinnacleManager
    {
        Task<FinnaclePostResponse> PostToFinnacle(List<MultipleTransactions> transactions, string countryId, string requestId, string clientReferenceId, Request request);
    }
}
