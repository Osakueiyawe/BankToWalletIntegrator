using BankWalletIntegrator.Interfaces;
using BankWalletIntegrator.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace BankWalletIntegrator.Services
{
    public class PostingModelUtility:IPostingModelUtility
    {
        private string className = "PostingModelUtility";
        private readonly ILogger _logger;
        private readonly IBankToWalletMerchants _getFeeDetails;
        public PostingModelUtility(IBankToWalletMerchants getFeeDetails, ILogger logger)
        {
            _logger = logger;
            _getFeeDetails = getFeeDetails;
        }
        public async Task<List<MultipleTransactions>> GenerateAccountBalancePostingModel(Request request)
        {
            string methodName = "GenerateAccountBalancePostingModel";
            List<MultipleTransactions> postingModel = new List<MultipleTransactions>();
            try
            {
                _logger.Information($"{className} || {methodName} || About to generate account balance posting model for request id: {request.requestId}");
                long merchantId = Convert.ToInt64(request.merchantId);
                DataTable feeDetails = await _getFeeDetails.GetMerchantAccountBalanceFees(merchantId);
                if(feeDetails != null && feeDetails.Rows.Count > 0)
                {
                    _logger.Information($"{className} || {methodName} || Number of rows retrieved for merchant account balance fees for request id: {request.requestId} is {feeDetails.Rows.Count}");
                    foreach (DataRow feeDetail in feeDetails.Rows)
                    {
                        MultipleTransactions transaction = new MultipleTransactions();
                        transaction.Narration = feeDetail["FeeNarration"].ToString();
                        transaction.Amount = Convert.ToDecimal(feeDetail["FeeValue"]);
                        transaction.Currency = feeDetail["Currency"].ToString();
                        transaction.PartTranType = "D";
                        transaction.AccountNumber = request.accountNumber;
                        transaction.Narration2 = feeDetail["FeeNarration2"].ToString();
                        postingModel.Add(transaction);
                        MultipleTransactions transaction1 = new MultipleTransactions();
                        transaction1.Narration = feeDetail["FeeNarration"].ToString();
                        transaction1.Amount = Convert.ToDecimal(feeDetail["FeeValue"]);
                        transaction1.Currency = feeDetail["Currency"].ToString();
                        transaction1.PartTranType = "C";
                        transaction1.AccountNumber = feeDetail["FeeAccount"].ToString().Replace("{BranchCode}", request.braCode);
                        transaction1.Narration2 = feeDetail["FeeNarration2"].ToString();
                        postingModel.Add(transaction1);
                    }
                }
            }
            catch(Exception ex)
            {
                _logger.Error(ex, $"Error generating account balance posting model for request id: {request.requestId}");
            }
            return postingModel;
        }

        public async Task<List<MultipleTransactions>> GenerateMiniStatementPostingModel(Request request)
        {
            string methodName = "GenerateMiniStatementPostingModel";
            List<MultipleTransactions> postingModel = new List<MultipleTransactions>();
            try
            {
                _logger.Information($"{className} || {methodName} || About to generate account balance posting model for request id: {request.requestId}");
                long merchantId = Convert.ToInt64(request.merchantId);
                DataTable feeDetails = await _getFeeDetails.GetMerchantMiniStatementFees(merchantId);
                if (feeDetails != null && feeDetails.Rows.Count > 0)
                {
                    _logger.Information($"{className} || {methodName} || Number of rows retrieved for merchant ministatement fees for request id: {request.requestId} is {feeDetails.Rows.Count}");
                    foreach (DataRow feeDetail in feeDetails.Rows)
                    {
                        MultipleTransactions transaction = new MultipleTransactions();
                        transaction.Narration = feeDetail["FeeNarration"].ToString();
                        transaction.Amount = Convert.ToDecimal(feeDetail["FeeValue"]);
                        transaction.Currency = feeDetail["Currency"].ToString();
                        transaction.PartTranType = "D";
                        transaction.AccountNumber = request.accountNumber;
                        transaction.Narration2 = feeDetail["FeeNarration2"].ToString();
                        postingModel.Add(transaction);
                        MultipleTransactions transaction1 = new MultipleTransactions();
                        transaction1.Narration = feeDetail["FeeNarration"].ToString();
                        transaction1.Amount = Convert.ToDecimal(feeDetail["FeeValue"]);
                        transaction1.Currency = feeDetail["Currency"].ToString();
                        transaction1.PartTranType = "C";
                        transaction1.AccountNumber = feeDetail["FeeAccount"].ToString().Replace("{BranchCode}", request.braCode);
                        transaction1.Narration2 = feeDetail["FeeNarration2"].ToString();
                        postingModel.Add(transaction1);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error generating ministatement posting model for request id: {request.requestId}");
            }
            return postingModel;
        }

        public async Task<List<MultipleTransactions>> GenerateBankToWalletPostingModel(Request request)
        {
            string methodName = "GenerateBankToWalletPostingModel";
            List<MultipleTransactions> postingModel = new List<MultipleTransactions>();
            try
            {
                _logger.Information($"{className} || {methodName} || About to generate bank to wallet posting model for request id: {request.requestId}");
                long merchantId = Convert.ToInt64(request.merchantId);
                //For the main amount
                MultipleTransactions multipleTransactions1 = new MultipleTransactions();
                multipleTransactions1.AccountNumber = request.accountNumber;
                multipleTransactions1.Amount = Convert.ToDecimal(request.amount);
                multipleTransactions1.Narration = request.ExternalRef;
                multipleTransactions1.Currency = request.merchantCurrency;
                multipleTransactions1.PartTranType = "D";
                multipleTransactions1.Narration2 = request.ExternalRef;
                postingModel.Add(multipleTransactions1);
                MultipleTransactions multipleTransactions2 = new MultipleTransactions();
                multipleTransactions2.AccountNumber = request.merchantSuspenseAccount;
                multipleTransactions2.Amount = Convert.ToDecimal(request.amount);
                multipleTransactions2.Narration = request.ExternalRef;
                multipleTransactions2.Currency = request.merchantCurrency;
                multipleTransactions2.PartTranType = "C";
                multipleTransactions2.Narration2 = request.ExternalRef;
                postingModel.Add(multipleTransactions2);

                //For the fees
                DataTable feeDetails = await _getFeeDetails.GetMerchantBankToWalletFees(merchantId);
                if (feeDetails != null && feeDetails.Rows.Count > 0)
                {
                    _logger.Information($"{className} || {methodName} || Number of rows retrieved for merchant ministatement fees for request id: {request.requestId} is {feeDetails.Rows.Count}");
                    foreach (DataRow feeDetail in feeDetails.Rows)
                    {
                        MultipleTransactions transaction = new MultipleTransactions();
                        transaction.Narration = feeDetail["FeeNarration"].ToString();
                        transaction.Amount = Convert.ToString(feeDetail["IsPercentage"])== "True" ? Convert.ToDecimal(feeDetail["FeeValue"])*request.amount : Convert.ToDecimal(feeDetail["FeeValue"]);
                        transaction.Currency = Convert.ToString(feeDetail["Currency"]);
                        transaction.PartTranType = "D";
                        transaction.AccountNumber = request.accountNumber;
                        transaction.Narration2 = feeDetail["FeeNarration2"].ToString();
                        postingModel.Add(transaction);
                        MultipleTransactions transaction1 = new MultipleTransactions();
                        transaction1.Narration = feeDetail["FeeNarration"].ToString();
                        transaction1.Amount = Convert.ToString(feeDetail["IsPercentage"]) == "True" ? Convert.ToDecimal(feeDetail["FeeValue"]) * request.amount : Convert.ToDecimal(feeDetail["FeeValue"]);
                        transaction1.Currency = Convert.ToString(feeDetail["Currency"]);
                        transaction1.PartTranType = "C";
                        transaction1.AccountNumber = feeDetail["FeeAccount"].ToString().Replace("{BranchCode}", request.braCode);
                        transaction1.Narration2 = feeDetail["FeeNarration2"].ToString();
                        postingModel.Add(transaction1);
                    }
                }
            }
            catch(Exception ex)
            {
                _logger.Error(ex, $"Error generating bank to wallet for request id: {request.requestId}");
            }
            return postingModel;
        }

        public async Task<List<MultipleTransactions>> GenerateWalletToBankPostingModel(Request request)
        {
            string methodName = "GenerateWalletToBankPostingModel";
            List<MultipleTransactions> postingModel = new List<MultipleTransactions>();
            try
            {
                _logger.Information($"{className} || {methodName} || About to generate bank to wallet posting model for request id: {request.requestId}");
                long merchantId = Convert.ToInt64(request.merchantId);
                MultipleTransactions multipleTransactions1 = new MultipleTransactions();
                multipleTransactions1.AccountNumber = request.merchantSuspenseAccount;
                multipleTransactions1.Amount = Convert.ToDecimal(request.amount);
                multipleTransactions1.Narration = request.ExternalRef;
                multipleTransactions1.Currency = request.merchantCurrency;
                multipleTransactions1.PartTranType = "D";
                multipleTransactions1.Narration2 = request.ExternalRef;
                postingModel.Add(multipleTransactions1);
                MultipleTransactions multipleTransactions2 = new MultipleTransactions();
                multipleTransactions2.AccountNumber = request.accountNumber;
                multipleTransactions2.Amount = Convert.ToDecimal(request.amount);
                multipleTransactions2.Narration = request.ExternalRef;
                multipleTransactions2.Currency = request.merchantCurrency;
                multipleTransactions2.PartTranType = "C";
                multipleTransactions2.Narration2 = request.ExternalRef;
                postingModel.Add(multipleTransactions2);
                DataTable feeDetails = await _getFeeDetails.GetMerchantWalletToBankFees(merchantId);
                if (feeDetails != null && feeDetails.Rows.Count > 0)
                {
                    _logger.Information($"{className} || {methodName} || Number of rows retrieved for merchant ministatement fees for request id: {request.requestId} is {feeDetails.Rows.Count}");
                    foreach (DataRow feeDetail in feeDetails.Rows)
                    {
                        MultipleTransactions transaction = new MultipleTransactions();
                        transaction.Narration = feeDetail["FeeNarration"].ToString();
                        transaction.Amount = Convert.ToString(feeDetail["IsPercentage"]) == "True" ? Convert.ToDecimal(feeDetail["FeeValue"]) * request.amount : Convert.ToDecimal(feeDetail["FeeValue"]);
                        transaction.Currency = feeDetail["Currency"].ToString();
                        transaction.PartTranType = "D";
                        transaction.AccountNumber = request.accountNumber;
                        transaction.Narration2 = feeDetail["FeeNarration2"].ToString();
                        postingModel.Add(transaction);
                        MultipleTransactions transaction1 = new MultipleTransactions();
                        transaction1.Narration = feeDetail["FeeNarration"].ToString();
                        transaction1.Amount = Convert.ToString(feeDetail["IsPercentage"]) == "True" ? Convert.ToDecimal(feeDetail["FeeValue"]) * request.amount : Convert.ToDecimal(feeDetail["FeeValue"]);
                        transaction1.Currency = feeDetail["Currency"].ToString();
                        transaction1.PartTranType = "C";
                        transaction1.AccountNumber = feeDetail["FeeAccount"].ToString().Replace("{BranchCode}", request.braCode);
                        transaction1.Narration2 = feeDetail["FeeNarration2"].ToString();
                        postingModel.Add(transaction1);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error generating wallet to bank posting model for request id: {request.requestId}");
            }
            return postingModel;
        }
    }
}
