﻿using Billing.Api.Models.Respons;
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
                if(billTxService.EndBillTx(billTxId) < 1)
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
                logger.LogError($"CompleteBillTx Error : {ex.Message}");
                return new BaseResponse<bool>
                {
                    Result = false,
                    ErrorCode = (int)BillingError.SystemError,
                    ErrorMessage = BillingError.SystemError.ToString(),
                    Data = false
                };
            }
        }

        [HttpPost("canceltx")]
        public BaseResponse<bool> CancelBillTx([FromBody] long billTxId)
        {
            try
            {
                (bool cancelResult, BillingError billingError) = billService.CancelPurchase(billTxId);
                if (!cancelResult)
                {
                    return new BaseResponse<bool>
                    {
                        Result = cancelResult,
                        ErrorCode = (int)billingError,
                        ErrorMessage = billingError.ToString(),
                        Data = false
                    };
                }
                
                if (billTxService.CancelBillTx(billTxId) < 1)
                {
                    return new BaseResponse<bool>
                    {
                        Result = false,
                        ErrorCode = (int)BillingError.TxCanceled,
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
                logger.LogError($"CancelBillTx Error : {ex.Message}");
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
