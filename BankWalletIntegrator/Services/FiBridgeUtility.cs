using BankWalletIntegrator.Interfaces;
using BankWalletIntegrator.Models;
using Newtonsoft.Json;
using RestSharp;
using Serilog;
using System;
using System.Net;
using System.Threading.Tasks;

namespace BankWalletIntegrator.Services
{
    public class FiBridgeUtility<T>:IFiBridgeUtility<T>
    {
        private string className = "FiBridgeUtility";
        private readonly ILogger _logger;
        public FiBridgeUtility(ILogger logger)
        {
            _logger = logger;
        }
            
        public async Task<T> CallFIBridge(object payLoad, string url, string appId, string appKey, Request request)
        {
            string methodName = "CallFIBridge";
            T result = default(T);
            try
            {
                string param = JsonConvert.SerializeObject(payLoad);
                _logger.Information($"{className} || {methodName} || About to call FI Bridge url: {url} for request id {request.requestId}");
                var client = new RestClient(url);
                var webRequest = new RestRequest(Method.POST);
                webRequest.AddHeader("content-type", "application/json");
                webRequest.AddHeader("AppId", appId);
                webRequest.AddHeader("AppKey", appKey);
                webRequest.AddParameter("application/json", param, ParameterType.RequestBody);
                webRequest.RequestFormat = DataFormat.Json;
                _logger.Information($"{className} || {methodName} || About to call FI Bridge with payload: {param} for request id {request.requestId}");

                IRestResponse webResponse = client.Execute(webRequest);
                string responseContent = webResponse.Content;
                _logger.Information($"{className} || {methodName} || FI Bridge response content for request id {request.requestId} is {responseContent}");
                if (webResponse.StatusCode == HttpStatusCode.OK)
                {
                    result = JsonConvert.DeserializeObject<T>(responseContent);
                }
            }
            catch(Exception ex)
            {
                _logger.Error(ex, $"Error calling FI Bridge for request id: {request.requestId}");
            }
            return result;
        }
    }
}
