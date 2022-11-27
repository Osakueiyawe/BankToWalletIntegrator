using BankWalletIntegrator.Integrations;
using BankWalletIntegrator.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace BankWalletIntegrator.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BankToWalletXmlController : ControllerBase
    {
        private readonly IOrangeSenegalIntegration _orangeSenegal;
        public BankToWalletXmlController(IOrangeSenegalIntegration orangeSenegal)
        {
            _orangeSenegal = orangeSenegal;
        }
        [HttpPost]
        public async Task<IActionResult> Process()
        {
            try
            {
                string response = string.Empty;
                string body = string.Empty;
                Request.EnableBuffering();
                using (StreamReader stream = new StreamReader(Request.Body, Encoding.UTF8, true, 1024, true))
                {
                    body = await stream.ReadToEndAsync();
                    Request.Body.Position = 0;
                }
                if (!string.IsNullOrEmpty(body))
                {
                    response = await _orangeSenegal.ProcessRequest(body);
                }
                if (!string.IsNullOrEmpty(response))
                {
                    return new ContentResult
                    {
                        Content = response,
                        ContentType = "application/xml",
                        StatusCode = 200
                    };                    
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return new ContentResult
                {
                    Content = "",
                    ContentType = "application/xml",
                    StatusCode = 500
                };
            }
        }
    }
}
