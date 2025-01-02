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
                var (isValid, validationError) = billTxService.ValidateBillTx(validateRequest.BillTxId);
                if (!isValid)
                {
                    return HandleValidationFailure(response, validationError, validateRequest.BillTxId);
                }

                if (!billTxService.RegistPurchaseToken(validateRequest.BillTxId, validateRequest.PurchaseToken))
                {
                    logger.LogWarning($"[RegistPurchaseToken False] BillTxId: {validateRequest.BillTxId}");
                    return HandleSystemError(response, BillingError.SYSTEM_ERROR, "Failed to register purchase token.");
                }

                var (validationResult, validationServiceError) = await billService.PurchaseValidation(validateRequest.toPurchaseInfo());

                response.Result = validationResult;
                response.ErrorCode = (int)validationServiceError;
                response.ErrorMessage = validationServiceError.ToString();
            }
            catch (Exception ex)
            {
                logger.LogError($"ValidateBillTx Error: {ex.Message}");
                return HandleSystemError(response, BillingError.SYSTEM_ERROR, "Unexpected exception during validation.");
            }

            return response;
        }

        private BaseResponse<bool> HandleValidationFailure(BaseResponse<bool> response, BillingError error, long billTxId)
        {
            logger.LogWarning($"[ValidationFailed] BillTxId: {billTxId}, ValidateError: {error}");
            response.Result = false;
            response.ErrorCode = (int)error;
            response.ErrorMessage = error.ToString();
            return response;
        }

        private BaseResponse<bool> HandleSystemError(BaseResponse<bool> response, BillingError error, string errorMessage)
        {
            logger.LogWarning(errorMessage);
            response.Result = false;
            response.ErrorCode = (int)error;
            response.ErrorMessage = error.ToString();
            return response;
        }

    }
}
