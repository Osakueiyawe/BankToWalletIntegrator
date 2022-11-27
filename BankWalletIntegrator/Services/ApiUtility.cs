using BankWalletIntegrator.Interfaces;
using BankWalletIntegrator.Models;
using Newtonsoft.Json;
using RestSharp;
using Serilog;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace BankWalletIntegrator.Services
{
    public class ApiUtility<T>:IApiUtility<T>
    {
        string className = "ApiUtility";
        private readonly ILogger _logger;
        public ApiUtility(ILogger logger)
        {
            _logger = logger;
        }
        public async Task<T> GetAccountNumberFromApi(object payLoad, string url, Dictionary<string, string> headers, Request request)
        {
            string methodName = "GetAccountNumberFromApi";
            T response = default(T);
            try
            {
                string param = JsonConvert.SerializeObject(payLoad);
                _logger.Information($"{className} || {methodName} || About to get account number from url: {url} for request id {request.requestId}");
                var client = new RestClient(url);
                var webRequest = new RestRequest(Method.POST);
                webRequest.AddHeader("content-type", "application/json");
                foreach(var header in headers)
                {
                    webRequest.AddHeader(header.Key, header.Value);
                }                
                webRequest.AddParameter("application/json", param, ParameterType.RequestBody);
                webRequest.RequestFormat = DataFormat.Json;
                _logger.Information($"{className} || {methodName} || About to get account number from api with payload: {param} for request id {request.requestId}");

                IRestResponse webResponse = client.Execute(webRequest);
                string responseContent = webResponse.Content;
                _logger.Information($"{className} || {methodName} || Account number response content for request id {request.requestId} is {responseContent}");
                if (webResponse.StatusCode == HttpStatusCode.OK)
                {
                    response = JsonConvert.DeserializeObject<T>(responseContent);
                }
            }
            catch(Exception ex)
            {
                _logger.Error(ex, $"Error getting account number for request id {request.requestId}");
            }
            return response;
        }
    }
}
