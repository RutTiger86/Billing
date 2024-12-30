using Billing.Api.Enums;
using Billing.Api.Middlewares;
using Billing.Api.Models.Respons;
using Billing.Core.Enums;
using Billing.Core.Interfaces;
using Billing.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace Billing.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillTxController : ControllerBase
    {
        private ILogger<BillTxController> logger;
        private IBillTxService billTxService;

        public BillTxController(IBillTxService billTxService, ILogger<BillTxController> logger) 
        {
            this.billTxService = billTxService;
            this.logger = logger;
        }

        [HttpGet]
        public BaseResponse<long> IssueBillTx([FromQuery]BillTxTypes txTypes)
        {
            BaseResponse<long> response = new BaseResponse<long>
            {
                Result = true,
                ErrorCode = (int)BillingErrorCode.SUCESS,
                Data = billTxService.IssueBillTx(txTypes)

            };
            return response;
        }

        
    }
}
