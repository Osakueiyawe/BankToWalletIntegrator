using BankWalletIntegrator.Interfaces;
using BankWalletIntegrator.Models;
using BankWalletIntegrator.RequestModels.MtnGuinea;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using StatusCodes = BankWalletIntegrator.Models.StatusCodes;

namespace BankWalletIntegrator.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BankToWalletJsonController : ControllerBase
    {
        private readonly IMTNGuineaIntegration _mtnGuinea;
        public BankToWalletJsonController(IMTNGuineaIntegration mtnGuinea)
        {
            _mtnGuinea = mtnGuinea;
        }
        [HttpPost]
        public async Task<IActionResult> GetAccountBalance(GetAccountBalance request)
        {
            try
            {
                if(ModelState.IsValid)
                {
                    AccountBalanceResponse accountBalance = await _mtnGuinea.ProcessAccountBalanceRequest(request);
                    if(accountBalance == null)
                    {
                        return NotFound();
                    }
                    if(accountBalance.responseCode == StatusCodes.validationError)
                    {
                        return BadRequest(accountBalance);
                    }
                    return Ok(accountBalance);
                }
                else
                {
                    return BadRequest(request);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(request);
            }
                
        }

        [HttpPost]
        public async Task<IActionResult> GetMiniStatement(GetMiniStatement request)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    MiniStatementResponse miniStatement = await _mtnGuinea.ProcessMiniStatementRequest(request);
                    if (miniStatement == null)
                    {
                        return NotFound();
                    }
                    if(miniStatement.responseCode == StatusCodes.technicalError)
                    {
                        return NotFound(request);
                    }
                    if (miniStatement.responseCode == StatusCodes.validationError)
                    {
                        return BadRequest(miniStatement);
                    }
                    return Ok(miniStatement);
                }
                else
                {
                    return BadRequest(request);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(request);
            }

        }

        [HttpPost]
        public async Task<IActionResult> BankToWallet(BankToWallet request)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Response miniStatement = await _mtnGuinea.ProcessBankToWalletTransfer(request);
                    if (miniStatement == null)
                    {
                        return NotFound();
                    }
                    if (miniStatement.responseCode == StatusCodes.technicalError)
                    {
                        return NotFound(request);
                    }
                    if (miniStatement.responseCode == StatusCodes.validationError)
                    {
                        return BadRequest(miniStatement);
                    }
                    return Ok(miniStatement);
                }
                else
                {
                    return BadRequest(request);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(request);
            }

        }

        [HttpPost]
        public async Task<IActionResult> WalletToBank(WalletToBank request)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Response miniStatement = await _mtnGuinea.ProcessWalletToBankTransfer(request);
                    if (miniStatement == null)
                    {
                        return NotFound();
                    }
                    if (miniStatement.responseCode == StatusCodes.technicalError)
                    {
                        return NotFound(request);
                    }
                    if (miniStatement.responseCode == StatusCodes.validationError)
                    {
                        return BadRequest(miniStatement);
                    }
                    return Ok(miniStatement);
                }
                else
                {
                    return BadRequest(request);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(request);
            }

        }

        [HttpPost]
        public async Task<IActionResult> TransactionStatus(TransactionStatusInquiry request)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Response miniStatement = await _mtnGuinea.ProcessTransactionInquiry(request);
                    if (miniStatement == null)
                    {
                        return NotFound();
                    }
                    if (miniStatement.responseCode == StatusCodes.technicalError)
                    {
                        return NotFound(request);
                    }
                    if (miniStatement.responseCode == StatusCodes.validationError)
                    {
                        return BadRequest(miniStatement);
                    }
                    return Ok(miniStatement);
                }
                else
                {
                    return BadRequest(request);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(request);
            }

        }
    }
}
