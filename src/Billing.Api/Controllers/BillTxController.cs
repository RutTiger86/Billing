using Billing.Api.Models.Respons;
using Billing.Core.Enums;
using Billing.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Billing.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillTxController(IBillTxService billTxService, ILogger<BillTxController> logger) : ControllerBase
    {
        private readonly ILogger<BillTxController> logger = logger;
        private readonly IBillTxService billTxService = billTxService;

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


    }
}
