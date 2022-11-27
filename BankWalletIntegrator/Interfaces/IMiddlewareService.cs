using BankWalletIntegrator.Models;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace BankWalletIntegrator.Interfaces
{
    public interface IMiddlewareService
    {
        Task<bool> CheckIpAllowed(string ip, string appId, string appKey);
        Task<bool> CheckRequestIdExists(string requestId);
        Task<RequestResponseEntry> CreateRequestDetails(HttpContext context);
        Task<string> SerializeHeaders(IHeaderDictionary headers);
        Task<string> GenerateXmlResponse(Response responseDetail);
    }
}
