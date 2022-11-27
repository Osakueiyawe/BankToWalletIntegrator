using BankWalletIntegrator.Interfaces;
using BankWalletIntegrator.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Newtonsoft.Json;
using Serilog;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace BankWalletIntegrator.Middleware
{
    public class RequestResponseMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        string className = "RequestResponseMiddleware";
        public RequestResponseMiddleware(RequestDelegate next, ILogger logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context, IMiddlewareService _middlewareService, IMiddlewareRepo _middlewareRepo)
        {
            string methodName = "Invoke";
            try
            {
                _logger.Information($"{className} || {methodName} || About to start middleware authentication");
                var requestText = string.Empty;
                var req = context.Request;
                req.EnableBuffering();
                //All Requests should inherit the MerchantBaseRequest Class
                using (StreamReader reader = new StreamReader(req.Body, Encoding.UTF8, true, 1024, true))
                {
                    requestText = await reader.ReadToEndAsync();                    
                    req.Body.Position = 0;                  
                }
                string requestId = "";
                string merchantId = "";
                bool contentTypeXml = req.Headers["Content-Type"].ToString().ToLower().Contains("xml");
                _logger.Information($"{className} || {methodName} || About to verify whether request is json or xml");
                if (contentTypeXml)
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(requestText);
                    XmlNodeList xnList = doc.GetElementsByTagName("mmHeaderInfo");
                    requestId = xnList[0]["requestId"]?.InnerText ?? "";
                    merchantId = xnList[0]["merchantId"]?.InnerText ?? "";
                }
                else
                {
                    dynamic requestJson = JsonConvert.DeserializeObject<dynamic>(requestText);
                    requestId = requestJson.requestId;
                    merchantId = requestJson.merchantId;
                }
                
                if(string.IsNullOrEmpty(requestId))
                {
                    _logger.Information($"{className} || {methodName} || No Request Id found");
                    context.Response.StatusCode = 400;                    
                    Response responseDetails = new Response
                    {
                        responseCode = "400",
                        responseMessage = "Request Id Missing",
                        RequestId = requestId,
                        merchantId = merchantId
                    };
                    if(contentTypeXml)
                    {
                        context.Response.ContentType = "application/xml";
                        string response = await _middlewareService.GenerateXmlResponse(responseDetails);
                        await context.Response.WriteAsync(response);
                        return;
                    }
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsJsonAsync(responseDetails);
                    return;
                }
                if(string.IsNullOrEmpty(merchantId))
                {
                    _logger.Information($"{className} || {methodName} || No Merchant Id found");
                    context.Response.StatusCode = 400;                    
                    Response responseDetails = new Response
                    {
                        responseCode = "400",
                        responseMessage = "Merchant Id Missing",
                        RequestId = requestId,
                        merchantId = merchantId
                    };
                    if (contentTypeXml)
                    {
                        context.Response.ContentType = "application/xml";
                        string response = await _middlewareService.GenerateXmlResponse(responseDetails);
                        await context.Response.WriteAsync(response);
                        return;
                    }
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsJsonAsync(responseDetails);
                    return;
                }
                context.Request.Headers.TryGetValue("AppId", out var appId);
                context.Request.Headers.TryGetValue("AppKey", out var appKey);
                string ip = context.Request.HttpContext.Features.Get<IHttpConnectionFeature>().RemoteIpAddress.ToString();
                _logger.Information($"{className} || {methodName} || Ip for requestId: {requestId} is {ip}");
                bool ipEsists = await _middlewareService.CheckIpAllowed(ip, appId.ToString(), appKey.ToString());
                if(!ipEsists)
                {
                    _logger.Information($"{className} || {methodName} || Ip for requestId: {requestId} is not permitted");
                    context.Response.StatusCode = 401;                    
                    Response responseDetails = new Response
                    {
                        responseCode = "700",
                        responseMessage = "Not permitted",
                        merchantId = merchantId,
                        RequestId = requestId
                    };
                    if (contentTypeXml)
                    {
                        context.Response.ContentType = "application/xml";
                        string response = await _middlewareService.GenerateXmlResponse(responseDetails);
                        await context.Response.WriteAsync(response);
                        return;
                    }
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsJsonAsync(responseDetails);
                    return;
                }
                bool requestIdExists = await _middlewareService.CheckRequestIdExists(requestId);
                if(requestIdExists)
                {
                    _logger.Information($"{className} || {methodName} || requestId: {requestId} already exists");
                    context.Response.StatusCode = 400;                    
                    Response responseDetails = new Response
                    {
                        responseCode = "400",
                        responseMessage = "Request Id already exists",
                        merchantId = merchantId,
                        RequestId = requestId
                    };
                    if (contentTypeXml)
                    {
                        context.Response.ContentType = "application/xml";
                        string response = await _middlewareService.GenerateXmlResponse(responseDetails);
                        await context.Response.WriteAsync(response);
                        return;
                    }
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsJsonAsync(responseDetails);
                    return;
                }
                RequestResponseEntry entry = await _middlewareService.CreateRequestDetails(context);
                if(entry == null)
                {
                    context.Response.StatusCode = 400;                    
                    Response responseDetails = new Response
                    {
                        responseCode = "400",
                        responseMessage = "Request could not be understood",
                        merchantId = merchantId,
                        RequestId = requestId
                    };
                    if (contentTypeXml)
                    {
                        context.Response.ContentType = "application/xml";
                        string response = await _middlewareService.GenerateXmlResponse(responseDetails);
                        await context.Response.WriteAsync(response);
                        return;
                    }
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsJsonAsync(responseDetails);
                    return;
                }
                var routeData = context.Request.Scheme + "://" + context.Request.Host + context.Request.Path;
                string operationName = string.Empty;
                string operationVersion = string.Empty;

                var operationDetails = routeData.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                operationName = operationDetails[operationDetails.Length - 1];//Get the last item in Url
                operationVersion = operationDetails[operationDetails.Length - 3];
                string responseBody = string.Empty;
                Stream originalBody = context.Response.Body;
                using (var memStream = new MemoryStream())
                {
                    context.Response.Body = memStream;

                    await _next(context);

                    memStream.Position = 0;
                    responseBody = new StreamReader(memStream).ReadToEnd();

                    memStream.Position = 0;
                    await memStream.CopyToAsync(originalBody);
                }
                entry.requestId = requestId;
                entry.responseContentBody = responseBody;
                entry.responseContentType = context.Response.ContentType;
                entry.responseHeaders = await _middlewareService.SerializeHeaders(context.Response.Headers);
                entry.responseTimestamp = DateTime.Now;
                entry.responseStatusCode = context.Response.StatusCode;
                _middlewareRepo.InsertRequestResponseEntries(entry);
            }
            catch (Exception ex)
            {

            }
        }
    }
}
