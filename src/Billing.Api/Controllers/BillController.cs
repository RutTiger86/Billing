using Microsoft.AspNetCore.Mvc;
using Billing.Core.Enums;
using Billing.Core.Interfaces;
using Billing.Core.Services;
using Billing.Api.Models.Respons;
using Billing.Api.Enums;
using Billing.Core.Exceptions;
using Billing.Api.Models.Requests;

namespace Billing.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillController : ControllerBase
    {
        private ILogger<BillController> logger;
        private IBillTxService billTxService;
        private GoogleBillingService googlBillServices;

        public BillController(IBillTxService billTxService, ILogger<BillController> logger, GoogleBillingService googlBillServices)
        {
            this.billTxService = billTxService;
            this.logger = logger;
            this.googlBillServices = googlBillServices;
        }

        [HttpPost]
        public async Task<BaseResponse<bool>> ValidateBillTx([FromBody] ValidateBillTxRequest validateRequest)
        {

            BaseResponse<bool> response = new()
            {
                Result = true,
                ErrorCode = (int)BillingErrorCode.SUCESS,
            };

            try
            {
                (bool result, BillingError error) = billTxService.ValidateBillTx(validateRequest.BillTxId);

                if (result)
                {
                    var billType =  billTxService.GetBillTxType(validateRequest.BillTxId);
                    IBillingService billingService = null;

                    switch (billType)
                    {
                        case BillTxTypes.IAP_Google:
                            billingService = googlBillServices;
                            break;
                        case BillTxTypes.WEB_PG:
                            break;
                        case BillTxTypes.IAP_IOS:
                            break;
                        case BillTxTypes.POINT:
                            break;
                        default:
                            billingService = null;
                            break;
                    }

                    if (billingService != null)
                    {
                        bool purcaseResult = await billingService.ValidatePurchaseAsync(validateRequest.ProductId, validateRequest.PurchaseToken);

                        if(!purcaseResult)
                        {
                            response.Result = false;
                            response.ErrorCode = (int)BillingErrorCode.PURCHASE_VALIDATION_FAILED;
                        }
                    }
                    else
                    {
                        response.Result = false;
                        response.ErrorCode = (int)BillingErrorCode.TX_UNVERIFIABLE_TYPE;
                    }

                }
                else
                {
                    logger.LogWarning($"[ValidataionFaild] BillTxId : {validateRequest.BillTxId}, Result : {result}, ValidateError : {error}");
                    response.Result = result;
                    response.ErrorCode = (int)BillingErrorCode.TX_VALIDATION_FAILED;
                }
            }
            catch (BillingException ex)
            {

            }
            catch (Exception)
            {

            }

            return response;
        }
    }
}
