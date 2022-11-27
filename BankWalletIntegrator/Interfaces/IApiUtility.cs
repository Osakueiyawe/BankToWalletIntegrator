using BankWalletIntegrator.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankWalletIntegrator.Interfaces
{
    public interface IApiUtility<T>
    {
        Task<T> GetAccountNumberFromApi(object payLoad, string url, Dictionary<string, string> headers, Request request);
    }
}
