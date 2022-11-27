using BankWalletIntegrator.Models;
using System.Threading.Tasks;

namespace BankWalletIntegrator.Interfaces
{
    public interface IRequestValidation
    {
        Task<string> VerifyRequest(Request request);        
    }
}
