﻿using Billing.Api.Enums;
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
                ErrorCode = (int)BillingErrorCode.Success,
                Data = billTxService.IssueBillTx(txTypes)

            };
            return response;
        }

        [HttpPost]
        public BaseResponse<bool> ValidateBillTx([FromBody] long billTxId)
        {

            BaseResponse<bool> response = new BaseResponse<bool>()
            {
                Result = true,
                ErrorCode= (int)BillingErrorCode.Success,
            };

            (bool result , ValidateError error ) = billTxService.ValidateBillTx(billTxId);

            if (!result)
            {

                logger.LogWarning($"[ValidataionFaild] BillTxId : {billTxId}, Result : {result}, ValidateError : {error}")
                response.Result = result;
                response.ErrorCode = (int)BillingErrorCode.TX_VALIDATION_FAILED;
            }

            return response;
        }
    }
}
