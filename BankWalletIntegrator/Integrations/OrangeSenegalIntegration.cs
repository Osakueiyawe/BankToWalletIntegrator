using BankWalletIntegrator.Interfaces;
using BankWalletIntegrator.Models;
using BankWalletIntegrator.RequestModels;
using BankWalletIntegrator.RequestModels.MtnGuinea;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Xml;

namespace BankWalletIntegrator.Integrations
{
    public class OrangeSenegalIntegration:IOrangeSenegalIntegration
    {
        private readonly IBankToWalletMerchants _merchantdetails;
        private readonly IMTNGuineaIntegration _processRequest;
        private readonly IApiUtility<OrangeSuscriberAccountResponse> _apiUtility;
        private readonly IConfiguration _config;
        private readonly ILogger _logger;
        public OrangeSenegalIntegration(IBankToWalletMerchants merchantDetails, IMTNGuineaIntegration processRequest, IApiUtility<OrangeSuscriberAccountResponse> apiUtility, IConfiguration config, ILogger logger)
        {
            _merchantdetails = merchantDetails;
            _processRequest = processRequest;
            _apiUtility = apiUtility;
            _config = config;
            _logger = logger;
        }
        public async Task<string> ProcessRequest(string xmlRequest)
        {
            string response = "";
            try
            {
                                
                if(xmlRequest.Contains("GETACCBAL"))
                {
                    GetAccountBalance accountBalanceRequest = await ConvertAccountBalanceXmlToRequestObject(xmlRequest);        
                    AccountBalanceResponse accountBalanceResponse = await _processRequest.ProcessAccountBalanceRequest(accountBalanceRequest);
                    response = await GenerateAcctBalXmlResponse(accountBalanceResponse);
                }
                else if(xmlRequest.Contains("W2A"))
                {
                    WalletToBank walletToBankRequest = await ConvertW2AXmlToRequestObject(xmlRequest);
                    Response walletToBankResponse = await _processRequest.ProcessWalletToBankTransfer(walletToBankRequest);
                    response = await GenerateW2AXmlResponse(walletToBankResponse);
                }
                else if(xmlRequest.Contains("A2W"))
                {
                    BankToWallet bankToWalletRequest = await ConvertA2WXmlToRequestObject(xmlRequest);
                    Response bankToWalletResponse = await _processRequest.ProcessBankToWalletTransfer(bankToWalletRequest);
                    response = await GenerateW2AXmlResponse(bankToWalletResponse);
                }
                else if(xmlRequest.Contains("GETMINI"))
                {
                    GetMiniStatement miniStatementRequest = await ConvertMiniStatementXmlToRequestObject(xmlRequest);
                    MiniStatementResponse miniStatementResponse = await _processRequest.ProcessMiniStatementRequest(miniStatementRequest);
                    response = await GenerateMiniStatementXmlResponse(miniStatementResponse);
                }
                else if(xmlRequest.Contains("TRANINQ"))
                {
                    TransactionStatusInquiry transactionStatusRequest = await ConvertTransInqXmlToRequestObject(xmlRequest);
                    TransactionStatusResponse transactionStatusResponse = await _processRequest.ProcessTransactionInquiry(transactionStatusRequest);
                    response = await GenerateTransInqXmlResponse(transactionStatusResponse);
                }
                else if(xmlRequest.Contains("CANCELTRAN"))
                {

                }
                else
                {

                }
            }
            catch (Exception ex)
            {

            }
            return response;
        }
        
        private async Task<GetAccountBalance> ConvertAccountBalanceXmlToRequestObject(string xml)
        {
            GetAccountBalance response = new GetAccountBalance();
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);

                //get a list of the childnodes that contain request object information using the name of their parent nodes
                XmlNodeList xnList1 = doc.GetElementsByTagName("AccountBalanceInquiryRequest");
                XmlNodeList xnList2 = doc.GetElementsByTagName("mmHeaderInfo");

                //extract all the contents and pass into payload 
                response.customerMsisdn = xnList1[0]["accountAlias"]?.InnerText ?? "";                
                response.requestId = xnList2[0]["requestId"]?.InnerText ?? "";                
                response.requestType = xnList2[0]["requestType"].InnerText;
                response.merchantId = xnList2[0]["merchantId"]?.InnerText ?? "";                
            }
            catch (Exception ex)
            {

            }
            return response;
        }

        private async Task<GetMiniStatement> ConvertMiniStatementXmlToRequestObject(string xml)
        {
            GetMiniStatement response = new GetMiniStatement();
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);

                //get a list of the childnodes that contain request object information using the name of their parent nodes
                XmlNodeList xnList1 = doc.GetElementsByTagName("AccountBalanceInquiryRequest");
                XmlNodeList xnList2 = doc.GetElementsByTagName("mmHeaderInfo");

                //extract all the contents and pass into payload 
                response.customerMsisdn = xnList1[0]["accountAlias"]?.InnerText ?? "";
                response.requestId = xnList2[0]["requestId"]?.InnerText ?? "";
                response.requestType = xnList2[0]["requestType"].InnerText;
                response.merchantId = xnList2[0]["merchantId"]?.InnerText ?? "";                
            }
            catch(Exception ex)
            {

            }
            return response;
        }

        private async Task<WalletToBank> ConvertW2AXmlToRequestObject(string xml)
        {
            WalletToBank response = new WalletToBank();
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);

                //get a list of the childnodes that contain request object information using the name of their parent nodes
                XmlNodeList xnList1 = doc.GetElementsByTagName("MobileTransferRequest");
                XmlNodeList xnList2 = doc.GetElementsByTagName("mmHeaderInfo");
                
                //extract all the contents and pass into payload 
                response.customerMsisdn = xnList1[0]["accountAlias"]?.InnerText ?? "";
                response.requestId = xnList2[0]["requestId"]?.InnerText ?? "";
                response.requestType = xnList2[0]["requestType"].InnerText;
                response.merchantId = xnList2[0]["merchantId"]?.InnerText ?? "";
                response.externalReference = xnList1[0]["externalRefNo"]?.InnerText ?? "";
            }
            catch (Exception ex)
            {

            }
            return response;
        }

        private async Task<BankToWallet> ConvertA2WXmlToRequestObject(string xml)
        {
            BankToWallet response = new BankToWallet();
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);

                //get a list of the childnodes that contain request object information using the name of their parent nodes
                XmlNodeList xnList1 = doc.GetElementsByTagName("MobileTransferRequest");
                XmlNodeList xnList2 = doc.GetElementsByTagName("mmHeaderInfo");

                //extract all the contents and pass into payload 
                response.customerMsisdn = xnList1[0]["accountAlias"]?.InnerText ?? "";
                response.requestId = xnList2[0]["requestId"]?.InnerText ?? "";
                response.requestType = xnList2[0]["requestType"].InnerText;
                response.merchantId = xnList2[0]["merchantId"]?.InnerText ?? "";
                response.externalReference = xnList1[0]["externalRefNo"]?.InnerText ?? "";
            }
            catch (Exception ex)
            {

            }
            return response;
        }

        private async Task<TransactionStatusInquiry> ConvertTransInqXmlToRequestObject(string xml)
        {
            TransactionStatusInquiry response = new TransactionStatusInquiry();
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);

                //get a list of the childnodes that contain request object information using the name of their parent nodes
                XmlNodeList xnList1 = doc.GetElementsByTagName("TranRequestInfo");
                XmlNodeList xnList2 = doc.GetElementsByTagName("mmHeaderInfo");

                //extract all the contents and pass into payload                 
                response.requestId = xnList2[0]["requestId"]?.InnerText ?? "";
                response.requestType = xnList2[0]["requestType"].InnerText;
                response.merchantId = xnList2[0]["merchantId"]?.InnerText ?? "";
                response.externalReference = xnList1[0]["externalRefNo"]?.InnerText ?? "";
            }
            catch (Exception ex)
            {

            }
            return response;
        }

        private async Task<string> GenerateAcctBalXmlResponse(AccountBalanceResponse accountBalanceResponse)
        {
            string response = "";
            try
            {
                response = @"<?xml version=""1.0"" encoding=""UTF-8""?>
                            <S:Envelope xmlns:S=""http://schemas.xmlsoap.org/soap/envelope/"">    
                            <S:Body>       
                            <ns2:GetAccountBalanceResponse xmlns:ns2=""http://b2w.banktowallet.com/b2w"">          
                            <return>             
                            <mmHeaderInfo>                                                    
                            <requestId>{requestId}</requestId>
                            <merchantId>{merchantId}</merchantId>
                            <responseCode>{responseCode}</responseCode>                
                            <responseMessage>{responseMessage}</responseMessage>             
                            </mmHeaderInfo>             
                            <accountAlias>{accountAlias}</accountAlias>                                                  
                            <availableBalance>{availableBalance}</availableBalance>             
                            <currentBalance>{currentBalance}</currentBalance>          
                            </return>       
                            </ns2:GetAccountBalanceResponse>    
                            </S:Body> 
                            </S:Envelope>";
                response = response.Replace("{requestId}",accountBalanceResponse.RequestId);
                response = response.Replace("{merchantId}", accountBalanceResponse.merchantId);
                response = response.Replace("{responseCode}", accountBalanceResponse.responseCode);
                response = response.Replace("{responseMessage}", accountBalanceResponse.responseMessage);
                response = response.Replace("{accountAlias}", accountBalanceResponse.msisdn);
                response = response.Replace("{availableBalance}", accountBalanceResponse.availableBalance);
                response = response.Replace("{currentBalance}", accountBalanceResponse.bookBalance);
            }
            catch(Exception ex)
            {

            }
            return response;
        }

        private async Task<string> GenerateMiniStatementXmlResponse(MiniStatementResponse miniStatementResponse)
        {
            string response = "";
            try
            {
                response = @"<?xml version=""1.0"" encoding=""UTF-8""?> <soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">    
                            <soap:Body>
                            <ns2:GetMiniStatementResponse xmlns:ns2 = ""http://b2w.banktowallet.com/b2w"" >
                            <return>
                            <mmHeaderInfo>                           
                            <requestId>{requestId}</requestId> 
                            <merchantId>{merchantId}</merchantId>
                            <responseCode>{responseCode}</responseCode>
                            <responseMessage>{responseMessage}</responseMessage>
                            </mmHeaderInfo>
                            {transactionList}
                            </return>
                            </ns2:GetMiniStatementResponse >
                            </soap:Body>
                            </soap:Envelope>";
                response = response.Replace("{requestId}", miniStatementResponse.RequestId);
                response = response.Replace("{merchantId}", miniStatementResponse.merchantId);
                response = response.Replace("{responseCode}", miniStatementResponse.responseCode);
                response = response.Replace("{responseMessage}", miniStatementResponse.responseMessage);
                string transactions = await GenerateTransactionList(miniStatementResponse.LastNTransactions);
                response = response.Replace("{transactionList}", transactions);
            }
            catch (Exception ex)
            {

            }
            return response;
        }

        private async Task<string> GenerateTransactionList(List<Statement> LastNTransactions)
        {
            string response = "";
            try
            {
                response = @"<TransactionList>
                            <tranRefNo>{tranRefNo}</tranRefNo>
                            <tranDate>{tranDate}</tranDate>
                            <tranType/>
                            <ccy/>
                            <crDr>{crDr}</crDr>
                            <amount>{amount}</amount>
                            <narration/>
                            </TransactionList>";
                if (LastNTransactions == null)
                {
                    return null;
                }
                if(LastNTransactions.Count == 0)
                {
                    return null;
                }
                foreach(Statement LastNTransaction in LastNTransactions)
                {
                    try
                    {
                        response = response.Replace("{tranRefNo}",LastNTransaction.TranId);
                        response = response.Replace("{tranDate}", LastNTransaction.TranId);
                        response = response.Replace("{crDr}", LastNTransaction.TranId);
                        response = response.Replace("{amount}", LastNTransaction.TranId);
                    }
                    catch(Exception ex)
                    {

                    }
                }
            }
            catch(Exception ex)
            {

            }
            return response;
        }
        private async Task<string> GenerateW2AXmlResponse(Response w2AResponse)
        {
            string response = "";
            try
            {
                response = @"<?xml version=""1.0"" encoding=""UTF-8""?>
                            <S:Envelope xmlns:S=""http://schemas.xmlsoap.org/soap/envelope/"">    
                            <S:Body>       
                            <ns2:WalletToAccountTransferResponse xmlns:ns2=""http://b2w.banktowallet.com/b2w"">          
                            <return>             
                            <mmHeaderInfo>                                            
                            <requestId>{requestId}</requestId>
                            <merchantId>{merchantId}</merchantId>
                            <responseCode>{responseCode}</responseCode>                
                            <responseMessage>{responseMessage}</responseMessage>             
                            </mmHeaderInfo>                                                                  
                            </return>       
                            </ns2:WalletToAccountTransferResponse>    
                            </S:Body> 
                            </S:Envelope>";
                response = response.Replace("{requestId}",w2AResponse.RequestId);
                response = response.Replace("{merchanttId}", w2AResponse.merchantId);
                response = response.Replace("{responseCode}", w2AResponse.responseCode);
                response = response.Replace("{responseMessage}", w2AResponse.responseMessage);                
            }
            catch (Exception ex)
            {

            }
            return response;
        }

        private async Task<string> GenerateTransInqXmlResponse(TransactionStatusResponse transInqResponse)
        {
            string response = "";
            try
            {
                response = @"<?xml version=""1.0"" encoding=""UTF-8""?>
                            <S:Envelope xmlns:S=""http://schemas.xmlsoap.org/soap/envelope/"">    
                            <S:Body>       
                            <ns2:TransferStatusInquiryResponse xmlns:ns2=""http://b2w.banktowallet.com/b2w"">          
                            <return>             
                            <mmHeaderInfo>                                            
                            <requestId>{requestId}</requestId>
                            <merchantId>{merchantId}</merchantId>                                          
                            <responseCode>{responseCode}</responseCode>                
                            <responseMessage>{responseMessage}</responseMessage>             
                            </mmHeaderInfo>
                            </return>       
                            </ns2:TransferStatusInquiryResponse>    
                            </S:Body> 
                            </S:Envelope>";

                response = response.Replace("{requestId}", transInqResponse.RequestId);
                response = response.Replace("{merchantId}", transInqResponse.merchantId);
                response = response.Replace("{responseCode}", transInqResponse.responseCode);
                response = response.Replace("{responseMessage}", transInqResponse.responseMessage);
            }
            catch (Exception ex)
            {

            }
            return response;
        }
    }
}
