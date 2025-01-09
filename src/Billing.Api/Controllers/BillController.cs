using Billing.Api.Extensions;
using Billing.Api.Models.Requests;
using Billing.Api.Models.Respons;
using Billing.Core.Enums;
using Billing.Core.Interfaces;
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
        public async Task<BaseResponse<bool>> ValidateBillTx([FromBody] ValidateBillTxRequest validateRequest)
        {
            var response = new BaseResponse<bool>
            {
                Result = true,
                ErrorCode = (int)BillingError.NONE,
            };

            try
            {
                var (validationResult, validationServiceError) = await billService.PurchaseValidation(validateRequest.toPurchaseInfo());

                response.Result = validationResult;
                response.ErrorCode = (int)validationServiceError;
                response.ErrorMessage = validationServiceError.ToString();
            }
            catch (Exception ex)
            {
                logger.LogError($"ValidateBillTx Error: {ex.Message}");
                response.Result = false;
                response.ErrorCode = (int)BillingError.SYSTEM_ERROR;
                response.ErrorMessage = BillingError.SYSTEM_ERROR.ToString();
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
                ErrorCode = (int)BillingError.NONE,
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
                response.ErrorCode = (int)BillingError.SYSTEM_ERROR;
                response.ErrorMessage = BillingError.SYSTEM_ERROR.ToString();
                return response;
            }

            return response;
        }

    }
}
