using Billing.Api.Extensions;
using Billing.Api.Models.Requests;
using Billing.Api.Models.Respons;
using Billing.Core.Interfaces;
using Billing.Protobuf.Core;
using Billing.Protobuf.Purchase;
using Microsoft.AspNetCore.Mvc;

namespace Billing.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillController(IBillTxService billTxService, IBillService billService, ILogger<BillController> logger) : ControllerBase
    {
        private readonly ILogger<BillController> logger = logger;
        private readonly IBillTxService billTxService = billTxService;
        private readonly IBillService billService = billService;

        [HttpPost("validate")]
        public async Task<BaseResponse<bool>> ValidateBillTx([FromBody] PurchaseInfo validateRequest)
        {
            var response = new BaseResponse<bool>
            {
                Result = true,
                ErrorCode = (int)BillingError.None,
            };

            try
            {
                var (validationResult, validationServiceError) = await billService.PurchaseValidation(validateRequest);

                response.Result = validationResult;
                response.ErrorCode = (int)validationServiceError;
                response.ErrorMessage = validationServiceError.ToString();
            }
            catch (Exception ex)
            {
                logger.LogError($"ValidateBillTx Error: {ex.Message}");
                response.Result = false;
                response.ErrorCode = (int)BillingError.SystemError;
                response.ErrorMessage = BillingError.SystemError.ToString();
                return response;
            }

            return response;
        }

        [HttpGet("subscription/state")]
        public async Task<BaseResponse<SubScriptionState>> GetSubscriptionState([FromQuery] long billTxId)
        {
            var response = new BaseResponse<SubScriptionState>
            {
                Result = true,
                ErrorCode = (int)BillingError.None,
            };

            try
            {
                var (state, validationServiceError) = await billService.SubScriptionStateValidation(billTxId);

                response.Result = true;
                response.ErrorCode = (int)validationServiceError;
                response.ErrorMessage = validationServiceError.ToString();
                response.Data = state;
            }
            catch (Exception ex)
            {
                logger.LogError($"ValidateBillTx Error: {ex.Message}");
                response.Result = false;
                response.ErrorCode = (int)BillingError.SystemError;
                response.ErrorMessage = BillingError.SystemError.ToString();
                return response;
            }

            return response;
        }

    }
}
