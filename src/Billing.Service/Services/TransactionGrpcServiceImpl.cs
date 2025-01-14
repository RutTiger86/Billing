using Billing.Core.Interfaces;
using Billing.Protobuf.Core;
using Billing.Protobuf.Service;
using Grpc.Core;

namespace Billing.Service.Services
{
    public  class TransactionGrpcServiceImpl(IBillService billService,IBillTxService billTxService, ILogger<TransactionGrpcServiceImpl> logger) : TransactionGrpcService.TransactionGrpcServiceBase
    {
        private readonly ILogger<TransactionGrpcServiceImpl> logger = logger;
        private readonly IBillTxService billTxService = billTxService;
        private readonly IBillService billService = billService;

        public override Task<IssueBillTxResponse> IssueBillTx(IssueBillTxRequest request, ServerCallContext context)
        {
            try
            {
                return Task.FromResult(new IssueBillTxResponse
                {
                    BaseResponse = new BaseResponse
                    {
                        IsSuccessful = true,
                        Error = BillingError.None
                    },
                    TransactionId = billTxService.IssueBillTx(request.BillTxType)
                });
            }
            catch (Exception ex)
            {
                logger.LogError($"IssueBillTx Error : {ex.Message}");
                return Task.FromResult(new IssueBillTxResponse
                {
                    BaseResponse = new BaseResponse
                    {
                        IsSuccessful = false,
                        Error = BillingError.SystemError
                    },
                    TransactionId = -1
                });
            }
        }

        public override Task<CommonResponse> CompleteBillTx(CommonBillTxRequest request, ServerCallContext context)
        {
            try
            {
                return Task.FromResult(new CommonResponse
                {
                    BaseResponse = new BaseResponse
                    {
                        IsSuccessful = true,
                        Error = BillingError.None
                    },
                    Result = billTxService.EndBillTx(request.TransactionId)
                });
            }
            catch (Exception ex)
            {
                logger.LogError($"CompleteBillTx Error : {ex.Message}");
                return Task.FromResult(new CommonResponse
                {
                    BaseResponse = new BaseResponse
                    {
                        IsSuccessful = false,
                        Error = BillingError.SystemError
                    },
                    Result = false
                });
            }
        }

        public override async Task<CommonResponse> CancelBillTx(CommonBillTxRequest request, ServerCallContext context)
        {
            try
            {
                (bool cancelResult, BillingError billingError) = await billService.CancelPurchase(request.TransactionId);

                if (!cancelResult)
                {
                    logger.LogError($"CancelBillTx CancelPurchase Error : TxId( {request.TransactionId} ), {billingError}");
                    return new CommonResponse
                    {
                        BaseResponse = new BaseResponse
                        {
                            IsSuccessful = false,
                            Error = billingError
                        },
                        Result = false
                    };
                }

                return new CommonResponse
                {
                    BaseResponse = new BaseResponse
                    {
                        IsSuccessful = true,
                        Error = BillingError.None
                    },
                    Result = billTxService.CancelBillTx(request.TransactionId)
                };
            }
            catch (Exception ex)
            {
                logger.LogError($"CancelBillTx Error : {ex.Message}");
                return new CommonResponse
                {
                    BaseResponse = new BaseResponse
                    {
                        IsSuccessful = false,
                        Error = BillingError.SystemError
                    },
                    Result = false
                };
            }
        }
    }
}
