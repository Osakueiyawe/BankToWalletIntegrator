using BankWalletIntegrator.Interfaces;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Http;
using BankWalletIntegrator.Models;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.Features;
using Newtonsoft.Json;
using System.Collections.Generic;
using Serilog;

namespace BankWalletIntegrator.Services
{
    public class MiddlewareService:IMiddlewareService
    {
        private readonly IMiddlewareRepo _middlewareRepo;
        private readonly ILogger _logger;
        public MiddlewareService(IMiddlewareRepo middlewareRepo, ILogger logger)
        {
            _middlewareRepo = middlewareRepo;
            _logger = logger;
        }
        public async Task<bool> CheckIpAllowed(string ip, string appId, string appKey)
        {
            bool result = false;
            try
            {
                DataTable dt = await _middlewareRepo.GetAllowedIPAddresses(appId, appKey);
                if(dt != null)
                {
                    if(dt.Rows.Count > 0)
                    {
                        string ipAddresses = dt.Rows[0]["AllowedIPAddress"].ToString();
                        string[] ips = ipAddresses.Split(",");
                        if(ips.Contains(ip))
                        {
                            result = true;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                _logger.Error(ex, "Error checking that IP exists");
            }
            return result;
        }

        public async Task<bool> CheckRequestIdExists(string requestId)
        {
            bool response = false;
            try
            {
                string res = await _middlewareRepo.GetRequestIdStatus(requestId);
                if(res == "1")
                {
                    response = true;
                }
            }
            catch(Exception ex)
            {
                _logger.Error(ex, $"Error checking that requestId {requestId} exists");
            }
            return response;
        }

        public async Task<RequestResponseEntry> CreateRequestDetails(HttpContext context)
        {            
            try
            {
                HttpRequest request = context.Request;
                string queryString = string.Empty;
                if (request.QueryString.ToString() != null)
                {
                    queryString = request.QueryString.ToString();
                }
                return new RequestResponseEntry
                {
                    requestContentType = request.ContentType.ToString(),
                    requestIpAddress = request.HttpContext.Features.Get<IHttpConnectionFeature>().RemoteIpAddress.ToString(),
                    requestMethod = request.Method,
                    requestHeaders = await SerializeHeaders(request.Headers),
                    requestTimestamp = DateTime.Now,
                    requestUri = request.GetDisplayUrl(),
                    queryString = queryString
                };
            }
            catch(Exception ex)
            {
                _logger.Error(ex, "Error creating Request details");
                return null;
            }            
        }

        public async Task<string> SerializeHeaders(IHeaderDictionary headers)
        {
            var dict = new Dictionary<string, string>();
            try
            {
                foreach (var item in headers.ToList())
                {
                    try
                    {
                        if (item.Value.Any())
                        {
                            var header = item.Value.Aggregate(String.Empty, (current, value) => current + (value + " "));
                            header = header.TrimEnd(" ".ToCharArray());
                            dict.Add(item.Key, header);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, "Error looping through headers");
                    }
                }
            }
            catch (Exception ex)
            {

            }          
            return JsonConvert.SerializeObject(dict, Formatting.Indented);
        }

        public async Task<string> GenerateXmlResponse(Response responseDetail)
        {
            string response = string.Empty;
            try
            {
                if(responseDetail == null)
                {
                    return response;
                }
                response = @"<response>
                            <requestId>{requestId}</requestId>
                            <merchantId>{merchantId}</merchantId>
                            <responseCode>{responseCode}</responseCode>
                            <responseMessage>{responseMessage}</responseMessage>
                             </response>";

                response = response.Replace("{requestId}",responseDetail.RequestId);
                response = response.Replace("{merchantId}", responseDetail.merchantId);
                response = response.Replace("{responseCode}", responseDetail.responseCode);
                response = response.Replace("{responseMessage}", responseDetail.responseMessage);
                response = response.Replace("\r\n", "");
            }
            catch (Exception ex)
            {
                return response;
            }
            return response;
        }
    }
}
