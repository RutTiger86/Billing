using Billing.Api.Models.Respons;
using Billing.Core.Enums;
using Billing.Core.Interfaces;
using Billing.Core.Models.DataBase;
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
                    ErrorCode = (int)BillingError.NONE,
                    Data = billTxService.IssueBillTx(txTypes)
                };
            }
            catch (Exception ex)
            {
                logger.LogError($"IssuBillTx Error : {ex.Message}");
                return new BaseResponse<long>
                {
                    Result = false,
                    ErrorCode = (int)BillingError.SYSTEM_ERROR,
                    ErrorMessage = BillingError.SYSTEM_ERROR.ToString(),
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
                        ErrorCode = (int)BillingError.TX_COMPLETE_FAILED,
                        ErrorMessage = BillingError.TX_COMPLETE_FAILED.ToString(),
                        Data = false
                    };
                }


                return new BaseResponse<bool>
                {
                    Result = true,
                    ErrorCode = (int)BillingError.NONE,
                    Data = true
                };
            }
            catch (Exception ex)
            {
                logger.LogError($"IssuBillTx Error : {ex.Message}");
                return new BaseResponse<bool>
                {
                    Result = false,
                    ErrorCode = (int)BillingError.SYSTEM_ERROR,
                    ErrorMessage = BillingError.SYSTEM_ERROR.ToString(),
                    Data = false
                };
            }
        }


    }
}
