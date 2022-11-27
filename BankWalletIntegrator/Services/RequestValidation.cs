using BankWalletIntegrator.Interfaces;
using BankWalletIntegrator.Models;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Data;
using System.Threading.Tasks;

namespace BankWalletIntegrator.Services
{
    public class RequestValidation:IRequestValidation
    {
        string className = "RequestValidation";
        private readonly IConfiguration _configuration;
        private readonly IBankToWalletMerchants _bankToWalletMerchants;
        private readonly IFiBridgeUtility<WalletSubscriptionAccountResponse> _fiBridgeUtilityAccount;
        private readonly ILogger _logger;
        public RequestValidation(IConfiguration configuration, IBankToWalletMerchants bankToWalletMerchants, ILogger logger)
        {
            _configuration = configuration;
            _bankToWalletMerchants = bankToWalletMerchants;
            _logger = logger;
        }
        public async Task<string> VerifyRequest(Request request)
        {
            string methodName = "VerifyRequest";
            string response = "";
            try
            {
                _logger.Information($"{className} || {methodName} || About to verify request for request id: {request.requestId}");
                if (request == null)
                {
                    response = "Request is invalid";
                    return response;
                }                
                if(string.IsNullOrEmpty(request.requestId))
                {
                    response = "No Request Id found";
                    return response;
                }
                if(request.requestId.Length > 50)
                {
                    response = "Request Id is above maximum Length";
                    return response;
                }                
                _logger.Information($"{className} || {methodName} || About to get merchant details for request id: {request.requestId}");
                
                DataTable dt = await _bankToWalletMerchants.GetMerchantDetails(Convert.ToInt64(request.merchantId));
                if (dt == null || dt.Rows.Count <= 0)
                {                    
                    response = "Unable to retrieve Merchant details";
                    return response;
                }
                _logger.Information($"{className} || {methodName} || Number of merchant detail rows retrieved for request id: {request.requestId} is {dt.Rows.Count}");
                //string getAccountUrl = dt.Rows[0]["GetAccountUrl"].ToString();
                if (request.requestType == RequestTypes.bankToWallet || request.requestType == RequestTypes.walletToBank)
                {
                    if (request.amount <= 0)
                    {
                        response = "Request amount is invalid";
                        return response;
                    }
                    if(dt.Rows[0]["MinimumAmount"] != DBNull.Value)
                    {
                        if (request.amount < Convert.ToDecimal(dt.Rows[0]["MinimumAmount"]))
                        {
                            response = "Amount is below minimum value";
                            return response;
                        }
                    }
                    
                    if(dt.Rows[0]["MaximumAmount"] != DBNull.Value)
                    {
                        if (request.amount > Convert.ToDecimal(dt.Rows[0]["MaximumAmount"]))
                        {
                            response = "Amount is above maximum value";
                            return response;
                        }
                    }
                    
                }
                request.countryId = dt.Rows[0]["CountryId"].ToString();
                request.statementCount = Convert.ToInt32(dt.Rows[0]["StatementCount"]);
                request.merchantSuspenseAccount = Convert.ToString(dt.Rows[0]["MerchantSuspenseAccount"]);
                request.merchantCurrency = Convert.ToString(dt.Rows[0]["MerchantCurrency"]);
                if (string.IsNullOrEmpty(request.countryId))
                {
                    response = "Invalid country Id";
                    return response;
                }
                if(request.requestType != RequestTypes.transactionInquiry && request.requestType != RequestTypes.cancelTransaction)
                {
                    if (string.IsNullOrEmpty(request.accountNumber))
                    {
                        response = "Invalid Account Number";
                        return response;
                    }
                }
                
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error verifying request for request id: {request.requestId}");
            }
            return response;
        }

        
    }
}
