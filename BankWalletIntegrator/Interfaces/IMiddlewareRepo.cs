using BankWalletIntegrator.Models;
using System.Data;
using System.Threading.Tasks;

namespace BankWalletIntegrator.Interfaces
{
    public interface IMiddlewareRepo
    {
        Task<DataTable> GetAllowedIPAddresses(string appId, string appKey);
        Task<string> GetRequestIdStatus(string requestId);
        Task InsertRequestResponseEntries(RequestResponseEntry entry);
    }
}
