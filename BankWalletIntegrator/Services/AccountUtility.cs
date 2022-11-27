using BankWalletIntegrator.Interfaces;
using BankWalletIntegrator.Models;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace BankWalletIntegrator.Services
{
    public class AccountUtility:IAccountUtility
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IRequestValidation _requestValidation;
        private readonly IBankToWalletMerchants _bankToWalletMerchants;
        private readonly IFiBridgeUtility<AccountByAccountNumberResponse> _fiBridgeUtilityAccountDetails;
        private readonly IFiBridgeUtility<MiniStatementResponse> _fiBridgeUtilityMiniStatement;
        private readonly IFiBridgeUtility<TransactionStatusResponse> _transactionStatus;
        private readonly IPostingModelUtility _postingModelUtility;
        private readonly IFinnacleManager _finnacleManager;
        private readonly ILogUtility _logUtility;
        private string className = "AccountUtility";
        public AccountUtility(ILogger logger,IConfiguration configuration, IRequestValidation requestValidation, IBankToWalletMerchants bankToWalletMerchants, IFiBridgeUtility<AccountByAccountNumberResponse> fiBridgeUtilityAccountDetails, IFiBridgeUtility<MiniStatementResponse> fiBridgeUtilityMiniStatement, IPostingModelUtility postingModelUtility, IFinnacleManager finnacleManager, ILogUtility logUtility, IFiBridgeUtility<TransactionStatusResponse> transactionStatus)
        {
            _logger = logger;
            _configuration = configuration;
            _requestValidation = requestValidation;
            _bankToWalletMerchants = bankToWalletMerchants;
            _fiBridgeUtilityAccountDetails = fiBridgeUtilityAccountDetails;
            _fiBridgeUtilityMiniStatement = fiBridgeUtilityMiniStatement;
            _postingModelUtility = postingModelUtility;
            _finnacleManager = finnacleManager;
            _logUtility = logUtility;
            _transactionStatus = transactionStatus;
        }
        public async Task<AccountBalanceResponse> GetAccountBalance(Request request)
        {
            string methodName = "GetAccountBalance";
            AccountBalanceResponse response = new AccountBalanceResponse();
            try
            {                
                _logger.Information($"{className} || {methodName} || About to get account balance for request id {request.requestId}");
                string appId = _configuration.GetSection("FIBridgeAppId").Value;
                string appKey = _configuration.GetSection("FIBridgeAppKey").Value;
                _logger.Information($"{className} || {methodName} || About to verify request for request id {request.requestId}");
                string requestValidationResult = await _requestValidation.VerifyRequest(request);
                _logger.Information($"{className} || {methodName} || Result of request verification for {request.requestId} is {requestValidationResult}");
                if (!string.IsNullOrEmpty(requestValidationResult))
                {
                    response.responseCode = StatusCodes.validationError;
                    response.responseMessage = requestValidationResult;
                    return response;
                }                
                
                AccountByAccountNumberRequest accountDetailRequest = new AccountByAccountNumberRequest
                {
                    RequestId = Guid.NewGuid().ToString("n"),
                    CountryId = request.countryId,
                    AccountNumber = request.accountNumber
                };
                string getAccountUrl = _configuration.GetSection("FIBridgeUrl").Value + "account/get-account-details";
                _logger.Information($"{className} || {methodName} || About to retrieve account details for request id {request.requestId} using url: {getAccountUrl}");
                AccountByAccountNumberResponse accountDetails = await _fiBridgeUtilityAccountDetails.CallFIBridge(accountDetailRequest,getAccountUrl,appId,appKey,request);
                if(accountDetails == null)
                {
                    response.responseCode = StatusCodes.technicalError;
                    response.responseMessage = "Unable to retrieve account Details";
                    return response;
                }
                if(accountDetails.ResponseCode != "00")
                {
                    response.responseCode = StatusCodes.technicalError;
                    response.responseMessage = accountDetails.ResponseMessage;
                    return response;
                }
                _logger.Information($"{className} || {methodName} || About to generate account balance posting model for request id {request.requestId}");
                List<MultipleTransactions> postingModel = await _postingModelUtility.GenerateAccountBalancePostingModel(request);
                _logger.Information($"{className} || {methodName} || Number of transactions retrieved for request id {request.requestId} is {postingModel.Count}");
                if (postingModel == null || postingModel.Count <= 0)
                {
                    response.responseCode= StatusCodes.technicalError;
                    response.responseMessage = "Unable to generate posting model";
                    return response;
                }
                string clientRefId = string.IsNullOrEmpty(request.ExternalRef) ? Guid.NewGuid().ToString("n") : request.ExternalRef;
                _logger.Information($"{className} || {methodName} || About to finnacle for request id {request.requestId}");
                FinnaclePostResponse postResponse = await _finnacleManager.PostToFinnacle(postingModel, request.countryId, Guid.NewGuid().ToString("n"), clientRefId, request);
                _logger.Information($"{className} || {methodName} || Finnacle response code for request id {request.requestId} is {postResponse.ResponseCode}");
                if (postResponse != null && postResponse.ResponseCode != "00")
                {
                    response.responseCode = StatusCodes.finnacleError;
                    response.responseMessage = "Finnacle Posting Error";
                    return response;
                }
                response.responseCode = StatusCodes.success;
                response.responseMessage = "Account Balance retrieval successful";                
                response.availableBalance = accountDetails.AvailableBalance.ToString();
                response.bookBalance = accountDetails.BookBalance.ToString();
            }
            catch(Exception ex)
            {
                _logger.Error(ex, $"{className} || {methodName} || Error getting account details for request id: {request.requestId}");
            }
            finally
            {
                response.RequestId = request.requestId;
                response.merchantId = request.merchantId;
                response.msisdn = request.msisdn;
            }
            return response;
        }

        public async Task<MiniStatementResponse> GetMiniStatement(Request request)
        {
            string methodName = "GetMiniStatement";
            MiniStatementResponse response = new MiniStatementResponse();
            try
            {                
                _logger.Information($"{className} || {methodName} || About to get ministatement for request id {request.requestId}");
                string appId = _configuration.GetSection("StatementAppId").Value;
                string appKey = _configuration.GetSection("StatementAppKey").Value;
                _logger.Information($"{className} || {methodName} || About to verify request for request id {request.requestId}");
                string requestValidationResult = await _requestValidation.VerifyRequest(request);
                _logger.Information($"{className} || {methodName} || Verification result for request id {request.requestId} is {requestValidationResult}");
                if (!string.IsNullOrEmpty(requestValidationResult))
                {
                    response.responseCode = StatusCodes.validationError;
                    response.responseMessage = requestValidationResult;
                    return response;
                }

                _logger.Information($"{className} || {methodName} || About to generate ministatement posting model for request id {request.requestId}");
                List<MultipleTransactions> postingModel = await _postingModelUtility.GenerateMiniStatementPostingModel(request);
                _logger.Information($"{className} || {methodName} || Number of generated transactions for ministatement posting model for request id {request.requestId} is {postingModel.Count}");
                if (postingModel == null || postingModel.Count <= 0)
                {
                    response.responseCode = StatusCodes.technicalError;
                    response.responseMessage = "Unable to generate posting model";
                    return response;
                }
                string clientRefId = string.IsNullOrEmpty(request.ExternalRef) ? Guid.NewGuid().ToString("n") : request.ExternalRef;
                _logger.Information($"{className} || {methodName} || About to post to finnacle for request id {request.requestId}");
                FinnaclePostResponse postResponse = await _finnacleManager.PostToFinnacle(postingModel, request.countryId, Guid.NewGuid().ToString("n"), clientRefId, request);
                _logger.Information($"{className} || {methodName} || Finnacle post response for request id {request.requestId} is {postResponse.ResponseCode}");
                if (postResponse != null && postResponse.ResponseCode != "00")
                {
                    response.responseCode = StatusCodes.finnacleError;
                    response.responseMessage = "Finnacle Posting Error";
                    return response;
                }
                string getminiStatementUrl = _configuration.GetSection("StatementUrl").Value + "statement/last-N-transactions";
                var ministatementRequest = new MiniStatementRequest
                {
                    CountryId = request.countryId,
                    RequestId = Guid.NewGuid().ToString(),//SequenceService.GenerateGuid(),
                    AccountNumber = request.accountNumber,
                    SendSms = false,
                    TransactionsCount = request.statementCount
                };
                _logger.Information($"{className} || {methodName} || About to get ministatement for request id {request.requestId} from url: {getminiStatementUrl}");
                response = await _fiBridgeUtilityMiniStatement.CallFIBridge(ministatementRequest, getminiStatementUrl, appId, appKey, request);                
            }
            catch(Exception ex)
            {
                _logger.Error(ex, $"{className} || {methodName} || Error generating ministatement for request id: {request.requestId}");
            }
            finally
            {
                response.RequestId = request.requestId;
                response.merchantId = request.merchantId;
            }
            return response;
        }

        public async Task<Response> BankToWalletTransfer(Request request)
        {
            string methodName = "BankToWalletTransfer";
            Response response = new Response();
            try
            {
                _logger.Information($"{className} || {methodName} || About to do bank to wallet transfer for request id {request.requestId}");
                string appId = _configuration.GetSection("FIBridgeAppId").Value;
                string appKey = _configuration.GetSection("FIBridgeAppKey").Value;
                _logger.Information($"{className} || {methodName} || About to verify request for request id {request.requestId}");
                string requestValidationResult = await _requestValidation.VerifyRequest(request);
                _logger.Information($"{className} || {methodName} || Verification result for request id {request.requestId} is {requestValidationResult}");
                if (!string.IsNullOrEmpty(requestValidationResult))
                {
                    response.responseCode = StatusCodes.validationError;
                    response.responseMessage = requestValidationResult;
                    return response;
                }
                _logger.Information($"{className} || {methodName} || About to generate bank to wallet posting model for request id {request.requestId}");
                List<MultipleTransactions> multipleTransactions = await _postingModelUtility.GenerateBankToWalletPostingModel(request);
                _logger.Information($"{className} || {methodName} || Number of generate transactions for request id {request.requestId} is {multipleTransactions.Count}");
                _logger.Information($"{className} || {methodName} || About to post to finnacle for request id {request.requestId}");
                FinnaclePostResponse finnacleResponse = await _finnacleManager.PostToFinnacle(multipleTransactions,request.countryId,Guid.NewGuid().ToString(),request.ExternalRef,request);
                if (finnacleResponse == null)
                {
                    response.responseCode = StatusCodes.finnacleError;
                    response.responseMessage = "Error posting to finnacle";
                    return response;
                }
                _logger.Information($"{className} || {methodName} || Finnacle response code for request id {request.requestId} is {finnacleResponse.ResponseCode}");
                if (finnacleResponse.ResponseCode != "00")
                {
                    response.responseCode = StatusCodes.finnacleError;
                    response.responseMessage = finnacleResponse.ResponseMessage;
                    return response;
                }
                response.responseCode = StatusCodes.success;
                response.responseMessage = $"Successful with finnacle reference Id {finnacleResponse.TransactionReference}";
                BankToWalletLog bankToWalletLog = new BankToWalletLog
                {
                    requestId = request.requestId,
                    countryId = request.countryId,
                    externalRefNo = request.ExternalRef,
                    accountNo = request.accountNumber,
                    finnacleTransRef = finnacleResponse.TransactionReference,
                    responseCode = response.responseCode,
                    responseMessage = response.responseMessage,
                    transactionTime = DateTime.Now
                };
                _logUtility.LogBankToWallet(bankToWalletLog);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"{className} || {methodName} || Error with bank to wallet transfer for request id: {request.requestId}");
            }
            return response;
        }

        public async Task<Response> WalletToBankTransfer(Request request)
        {
            string methodName = "WalletToBankTransfer";
            Response response = new Response();
            try
            {
                _logger.Information($"{className} || {methodName} || About to do wallet to bank transfer for request id {request.requestId}");
                string appId = _configuration.GetSection("FIBridgeAppId").Value;
                string appKey = _configuration.GetSection("FIBridgeAppKey").Value;
                _logger.Information($"{className} || {methodName} || About to verify request for request id {request.requestId}");
                string requestValidationResult = await _requestValidation.VerifyRequest(request);
                _logger.Information($"{className} || {methodName} || Verification result for request id {request.requestId} is {requestValidationResult}");
                if (!string.IsNullOrEmpty(requestValidationResult))
                {
                    response.responseCode = StatusCodes.validationError;
                    response.responseMessage = requestValidationResult;
                    return response;
                }
                _logger.Information($"{className} || {methodName} || About to generate wallet to bank posting model for request id {request.requestId}");
                List<MultipleTransactions> multipleTransactions = await _postingModelUtility.GenerateWalletToBankPostingModel(request);
                _logger.Information($"{className} || {methodName} || Number of transactions generated for request id {request.requestId} is {multipleTransactions.Count}");
                _logger.Information($"{className} || {methodName} || About to post to finnacle for request id {request.requestId}");
                FinnaclePostResponse finnacleResponse = await _finnacleManager.PostToFinnacle(multipleTransactions, request.countryId, request.requestId, request.ExternalRef, request);
                if (finnacleResponse == null)
                {
                    response.responseCode = StatusCodes.finnacleError;
                    response.responseMessage = "Error posting to finnacle";
                    return response;
                }
                _logger.Information($"{className} || {methodName} || Finnacle response code for request id {request.requestId} is {finnacleResponse.ResponseCode}");
                if (finnacleResponse.ResponseCode != "00")
                {
                    response.responseCode = StatusCodes.finnacleError;
                    response.responseMessage = finnacleResponse.ResponseMessage;
                    return response;
                }
                response.responseCode = StatusCodes.success;
                response.responseMessage = $"Successful with finnacle reference Id {finnacleResponse.TransactionReference}";
                WalletToBankLog walletToBankLog = new WalletToBankLog
                {
                    requestId = request.requestId,
                    countryId = request.countryId,
                    externalRefNo = request.ExternalRef,
                    accountNo = request.accountNumber,
                    finnacleTransRef = finnacleResponse.TransactionReference,
                    responseCode = response.responseCode,
                    responseMessage = response.responseMessage,
                    transactionTime = DateTime.Now
                };
                _logUtility.LogWalletToBank(walletToBankLog);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"{className} || {methodName} || Error with Wallet to bank transfer for request id {request.requestId}");
            }
            return response;
        }

        public async Task<TransactionStatusResponse> TransferStatusInquiry(Request request)
        {
            string methodName = "TransferStatusInquiry";
            TransactionStatusResponse response = new TransactionStatusResponse();
            try
            {
                _logger.Information($"{className} || {methodName} || About to do transfer status inquiry for request id {request.requestId}");
                string appId = _configuration.GetSection("FIBridgeAppId").Value;
                string appKey = _configuration.GetSection("FIBridgeAppKey").Value;
                _logger.Information($"{className} || {methodName} || About to verify request for request id {request.requestId}");
                string requestValidationResult = await _requestValidation.VerifyRequest(request);
                _logger.Information($"{className} || {methodName} || Verification result for request id {request.requestId} is {requestValidationResult}");
                if (!string.IsNullOrEmpty(requestValidationResult))
                {
                    response.responseCode = StatusCodes.validationError;
                    response.responseMessage = requestValidationResult;
                    return response;
                }
                TransactionStatusRequest transactionStatusRequest = new TransactionStatusRequest
                {
                    RequestId = request.requestId,
                    CountryId = request.countryId,
                    ClientReferenceId = request.ExternalRef
                };
                string url = _configuration.GetSection("FIBridgeUrl").Value + "transaction/transaction-status-query";
                _logger.Information($"{className} || {methodName} || About to make transaction inquiry for request id {request.requestId} with url: {url}");
                response = await _transactionStatus.CallFIBridge(transactionStatusRequest,url, appId, appKey, request);
                response.merchantId = request.merchantId;
                if(response == null)
                {
                    response.responseCode = StatusCodes.finnacleError;
                    response.responseMessage = "Could not retrieve details from finnacle";
                    return response;
                }
                if(response.responseCode != "00")
                {
                    response.responseCode = StatusCodes.finnacleError;                    
                    return response;
                }

            }
            catch(Exception ex)
            {
                _logger.Error(ex, $"{className} || {methodName} || Error with transfer inquiry for request id: {request.requestId}");
            }
            return response;
        }
    }
}
