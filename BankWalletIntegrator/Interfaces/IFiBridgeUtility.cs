using BankWalletIntegrator.Models;
using System.Threading.Tasks;

namespace BankWalletIntegrator.Interfaces
{
    public interface IFiBridgeUtility<T>
    {
        Task<T> CallFIBridge(object payLoad, string url, string appId, string appKey, Request request);
    }
}
