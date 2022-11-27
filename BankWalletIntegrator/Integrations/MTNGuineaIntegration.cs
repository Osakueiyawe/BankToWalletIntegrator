using BankWalletIntegrator.Interfaces;
using BankWalletIntegrator.Models;
using BankWalletIntegrator.RequestModels.MtnGuinea;
using BankWalletIntegrator.ResponseModels.MtnGuinea;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace BankWalletIntegrator.Integrations
{
    public class MTNGuineaIntegration:IMTNGuineaIntegration
    {
        private readonly IAccountUtility _accountUtility;
        private readonly IApiUtility<MtnSubscriberAccountResponse> _apiUtility;
        private readonly IConfiguration _config;
        private readonly IBankToWalletMerchants _merchantDetails;
        public MTNGuineaIntegration(IAccountUtility accountUtility, IApiUtility<MtnSubscriberAccountResponse> apiUtility, IConfiguration config, IBankToWalletMerchants merchantDetails)
        {
            _accountUtility = accountUtility;
            _apiUtility = apiUtility;
            _config = config;
            _merchantDetails = merchantDetails;
        }
        public async Task<AccountBalanceResponse> ProcessAccountBalanceRequest(GetAccountBalance request)
        {
            AccountBalanceResponse response = new AccountBalanceResponse();
            try
            {
                if(request == null)
                {
                    response.responseCode = StatusCodes.validationError;
                    response.responseMessage = "Invalid Request";
                    return response;
                }                         
                              
                Request accountBalanceRequest = new Request
                {
                    requestId = request.requestId,
                    merchantId = request.merchantId,
                    msisdn = request.customerMsisdn,
                    requestType = request.requestType                    
                };
                accountBalanceRequest.accountNumber = await GetAccountNumberFromAccountAlias(accountBalanceRequest);
                if(string.IsNullOrEmpty(accountBalanceRequest.accountNumber))
                {
                    response.responseCode = StatusCodes.validationError;
                    response.responseMessage = "Unable to match customer's msisdn to an account";
                    return response;
                }
                if(request.requestType == RequestTypes.accountBalance)
                {
                    response = await _accountUtility.GetAccountBalance(accountBalanceRequest);
                }                
                else
                {
                    response.responseCode = StatusCodes.validationError;
                    response.responseMessage = "Invalid Request Type";
                    return response;
                }
            }
            catch(Exception ex)
            {

            }
            return response;
        }

        public async Task<MiniStatementResponse> ProcessMiniStatementRequest(GetMiniStatement request)
        {
            MiniStatementResponse response = new MiniStatementResponse();
            try
            {
                if (request == null)
                {
                    response.responseCode = StatusCodes.validationError;
                    response.responseMessage = "Invalid Request";
                    return response;
                }
                Request miniStatementRequest = new Request
                {
                    requestId = request.requestId,
                    merchantId = request.merchantId,
                    msisdn = request.customerMsisdn,
                    requestType = request.requestType
                };
                miniStatementRequest.accountNumber = await GetAccountNumberFromAccountAlias(miniStatementRequest);
                if (string.IsNullOrEmpty(miniStatementRequest.accountNumber))
                {
                    response.responseCode = StatusCodes.validationError;
                    response.responseMessage = "Unable to match customer's msisdn to an account";
                    return response;
                }
                if (request.requestType == RequestTypes.miniStatement)
                {
                    response = await _accountUtility.GetMiniStatement(miniStatementRequest);
                }
                else
                {
                    response.responseCode = StatusCodes.validationError;
                    response.responseMessage = "Invalid Request Type";
                    return response;
                }
            }
            catch (Exception ex)
            {

            }
            return response;
        }

        public async Task<Response> ProcessBankToWalletTransfer(BankToWallet request)
        {
            Response response = new Response();
            try
            {
                if (request == null)
                {
                    response.responseCode = StatusCodes.validationError;
                    response.responseMessage = "Invalid Request";
                    return response;
                }
                response.RequestId = request.requestId;
                response.merchantId = request.merchantId;
                Request bankToWalletRequest = new Request
                {
                    requestId = request.requestId,
                    merchantId = request.merchantId,
                    msisdn = request.customerMsisdn,
                    requestType = request.requestType,
                    amount = request.amount,
                    ExternalRef = request.externalReference
                };
                bankToWalletRequest.accountNumber = await GetAccountNumberFromAccountAlias(bankToWalletRequest);
                if (string.IsNullOrEmpty(bankToWalletRequest.accountNumber))
                {
                    response.responseCode = StatusCodes.validationError;
                    response.responseMessage = "Unable to match customer's msisdn to an account";
                    return response;
                }
                if (request.requestType == RequestTypes.bankToWallet)
                {
                    response = await _accountUtility.BankToWalletTransfer(bankToWalletRequest);
                    response.RequestId = request.requestId;
                    response.merchantId = request.merchantId;
                }
                else
                {
                    response.responseCode = StatusCodes.validationError;
                    response.responseMessage = "Invalid Request Type";
                    return response;
                }
            }
            catch (Exception ex)
            {

            }
            return response;
        }

        public async Task<Response> ProcessWalletToBankTransfer(WalletToBank request)
        {
            Response response = new Response();
            try
            {
                if (request == null)
                {
                    response.responseCode = StatusCodes.validationError;
                    response.responseMessage = "Invalid Request";
                    return response;
                }
                response.RequestId = request.requestId;
                response.merchantId = request.merchantId;
                Request bankToWalletRequest = new Request
                {
                    requestId = request.requestId,
                    merchantId = request.merchantId,
                    msisdn = request.customerMsisdn,
                    requestType = request.requestType,
                    amount = request.amount,
                    ExternalRef = request.externalReference
                };
                bankToWalletRequest.accountNumber = await GetAccountNumberFromAccountAlias(bankToWalletRequest);
                if (string.IsNullOrEmpty(bankToWalletRequest.accountNumber))
                {
                    response.responseCode = StatusCodes.validationError;
                    response.responseMessage = "Unable to match customer's msisdn to an account";
                    return response;
                }
                if (request.requestType == RequestTypes.walletToBank)
                {
                    response = await _accountUtility.WalletToBankTransfer(bankToWalletRequest);                    
                }
                else
                {
                    response.responseCode = StatusCodes.validationError;
                    response.responseMessage = "Invalid Request Type";
                    return response;
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                response.RequestId = request.requestId;
                response.merchantId = request.merchantId;
            }
            return response;
        }

        public async Task<TransactionStatusResponse> ProcessTransactionInquiry(TransactionStatusInquiry request)
        {
            TransactionStatusResponse response = new TransactionStatusResponse();
            try
            {
                if (request == null)
                {
                    response.responseCode = StatusCodes.validationError;
                    response.responseMessage = "Invalid Request";
                    return response;
                }                
                Request transactionInquiryRequest = new Request
                {
                    requestId = request.requestId,
                    merchantId = request.merchantId,                    
                    requestType = request.requestType,                    
                    ExternalRef = request.externalReference
                };                
                if (request.requestType == RequestTypes.transactionInquiry)
                {
                    response = await _accountUtility.TransferStatusInquiry(transactionInquiryRequest);                    
                }
                else
                {
                    response.responseCode = StatusCodes.validationError;
                    response.responseMessage = "Invalid Request Type";
                    return response;
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                response.RequestId = request.requestId;
                response.merchantId = request.merchantId;
            }
            return response;
        }
        private async Task<string> GetAccountNumberFromAccountAlias(Request request)
        {
            string accountNumber = string.Empty;
            string url = "";
            try
            {
                DataTable merchant = await _merchantDetails.GetMerchantDetails(Convert.ToInt64(request.merchantId));
                if (merchant.Rows.Count > 0)
                {
                    request.countryId = merchant.Rows[0]["CountryId"].ToString();
                    url = merchant.Rows[0]["GetAccountUrl"].ToString();
                }
                MtnSubscriberAccountRequest orangeRequest = new MtnSubscriberAccountRequest
                {
                    RequestId = request.requestId,
                    CountryId = request.countryId,
                    AccountAlias = request.msisdn,
                };                
                Dictionary<string, string> headers = new Dictionary<string, string>();
                MtnSubscriberAccountResponse apiResponse = await _apiUtility.GetAccountNumberFromApi(orangeRequest, url, headers, request);
                if(apiResponse != null)
                {
                    if(apiResponse.responseCode == StatusCodes.success)
                    {
                        accountNumber = apiResponse.AccountNumber;
                    }
                }
                
            }
            catch (Exception ex)
            {

            }
            return accountNumber;
        }
    }
}
