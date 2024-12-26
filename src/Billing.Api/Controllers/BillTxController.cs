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
        IBillTxService billTxService;

        public BillTxController(IBillTxService billTxService) 
        {
            this.billTxService = billTxService;
        }

        [HttpGet]
        public long IssueBillTx([FromQuery]BillTxTypes txTypes)
        {
            return billTxService.IssueBillTx(txTypes);
        }

    }
}
