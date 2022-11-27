using BankWalletIntegrator.Interfaces;
using BankWalletIntegrator.Models;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankWalletIntegrator.Services
{
    public class FinnacleManager:IFinnacleManager
    {
        string className = "FinnacleManager";
        private readonly IFiBridgeUtility<FinnaclePostResponse> _fiBridge;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        public FinnacleManager(IFiBridgeUtility<FinnaclePostResponse> fiBridge, IConfiguration configuration, ILogger logger)
        {
            _fiBridge = fiBridge;
            _configuration = configuration;
            _logger = logger;
        }
        public async Task<FinnaclePostResponse> PostToFinnacle(List<MultipleTransactions> transactions, string countryId, string requestId, string clientReferenceId,Request request)
        {
            string methodName = "PostToFinnacle";
            FinnaclePostResponse response = new FinnaclePostResponse();
            try
            {
                _logger.Information($"{className} || {methodName} || About to post to finnacle for request id: {request.requestId}");
                string appId = _configuration.GetSection("FIBridgeAppId").Value;
                string appKey = _configuration.GetSection("FIBridgeAppKey").Value;
                string url = _configuration.GetSection("FIBridgeUrl").Value + "transaction/post-multiple-transactions";
                FinnaclePostMultipleTransactionsRequest fiPostRequest = new FinnaclePostMultipleTransactionsRequest
                {
                    RequestId = requestId,
                    CountryId = countryId,
                    ClientReferenceId = clientReferenceId,
                    Transactions = transactions
                };
                _logger.Information($"{className} || {methodName} || About to call FI Bridge for request id: {request.requestId}");
                response = await _fiBridge.CallFIBridge(fiPostRequest, url, appId, appKey, request);
            }
            catch(Exception ex)
            {
                _logger.Error(ex, $"{className} || {methodName} || Error posting to finnacle for request id: {request.requestId}");
            }
            return response;
        }
    }
}
