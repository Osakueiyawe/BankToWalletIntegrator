using System.Threading.Tasks;

namespace BankWalletIntegrator.Interfaces
{
    public interface IOrangeSenegalIntegration
    {
        Task<string> ProcessRequest(string xmlRequest);
    }
}
