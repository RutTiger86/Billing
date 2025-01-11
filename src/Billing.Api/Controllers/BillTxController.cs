using Billing.Api.Models.Respons;
using Billing.Core.Interfaces;
using Billing.Protobuf.Core;
using Microsoft.AspNetCore.Mvc;

namespace Billing.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillTxController(IBillTxService billTxService,IBillService billService, ILogger<BillTxController> logger) : ControllerBase
    {
        private readonly ILogger<BillTxController> logger = logger;
        private readonly IBillTxService billTxService = billTxService;
        private readonly IBillService billService = billService;

        [HttpGet("issuetx")]
        public BaseResponse<long> IssueBillTx([FromQuery] BillTxTypes txTypes)
        {
            try
            {
                return new BaseResponse<long>
                {
                    Result = true,
                    ErrorCode = (int)BillingError.None,
                    Data = billTxService.IssueBillTx(txTypes)
                };
            }
            catch (Exception ex)
            {
                logger.LogError($"IssuBillTx Error : {ex.Message}");
                return new BaseResponse<long>
                {
                    Result = false,
                    ErrorCode = (int)BillingError.SystemError,
                    ErrorMessage = BillingError.SystemError.ToString(),
                    Data = -1
                };
            }
        }

        [HttpPost("completetx")]
        public BaseResponse<bool> CompleteBillTx([FromBody] long billTxId )
        {
            try
            {
                if(!billTxService.EndBillTx(billTxId))
                {
                    return new BaseResponse<bool>
                    {
                        Result = false,
                        ErrorCode = (int)BillingError.TxCompleteFailed,
                        ErrorMessage = BillingError.TxCompleteFailed.ToString(),
                        Data = false
                    };
                }


                return new BaseResponse<bool>
                {
                    Result = true,
                    ErrorCode = (int)BillingError.None,
                    Data = true
                };
            }
            catch (Exception ex)
            {
                logger.LogError($"IssuBillTx Error : {ex.Message}");
                return new BaseResponse<bool>
                {
                    Result = false,
                    ErrorCode = (int)BillingError.SystemError,
                    ErrorMessage = BillingError.SystemError.ToString(),
                    Data = false
                };
            }
        }

        [HttpPost("cancletx")]
        public async Task<BaseResponse<bool>> CancleBillTx([FromBody] long billTxId)
        {
            try
            {
                (bool cancleResult, BillingError billingError) = await billService.CanclePurchase(billTxId);
                if (!cancleResult)
                {
                    return new BaseResponse<bool>
                    {
                        Result = cancleResult,
                        ErrorCode = (int)billingError,
                        ErrorMessage = billingError.ToString(),
                        Data = false
                    };
                }
                
                if (!billTxService.CancleBillTx(billTxId))
                {
                    return new BaseResponse<bool>
                    {
                        Result = false,
                        ErrorCode = (int)BillingError.TxCompleteFailed,
                        ErrorMessage = BillingError.TxCompleteFailed.ToString(),
                        Data = false
                    };
                }


                return new BaseResponse<bool>
                {
                    Result = true,
                    ErrorCode = (int)BillingError.None,
                    Data = true
                };
            }
            catch (Exception ex)
            {
                logger.LogError($"IssuBillTx Error : {ex.Message}");
                return new BaseResponse<bool>
                {
                    Result = false,
                    ErrorCode = (int)BillingError.SystemError,
                    ErrorMessage = BillingError.SystemError.ToString(),
                    Data = false
                };
            }
        }


    }
}
