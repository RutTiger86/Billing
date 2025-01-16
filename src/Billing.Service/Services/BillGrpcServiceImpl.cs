using Billing.Core.Interfaces;
using Billing.Protobuf.Core;
using Billing.Protobuf.Purchase;
using Billing.Protobuf.Service;
using Grpc.Core;

namespace Billing.Service.Services
{
    public class BillGrpcServiceImpl(IBillService billService, ILogger<BillGrpcServiceImpl> logger) : BillGrpcService.BillGrpcServiceBase
    {
        private readonly ILogger<BillGrpcServiceImpl> logger = logger;
        private readonly IBillService billService = billService;
        public override async Task<SubscriptionStateResponse> GetSubscriptionState(CommonBillTxRequest request, ServerCallContext context)
        {
            try
            {
                var (state, validationServiceError) = await billService.GetSubScriptionState(request.TransactionId);

                return new SubscriptionStateResponse
                {
                    BaseResponse = new BaseResponse
                    {
                        IsSuccessful = validationServiceError == BillingError.None,
                        Error = validationServiceError
                    },
                    SubScriptionState = state
                };
            }
            catch (Exception ex)
            {
                logger.LogError($"GetSubscriptionState Error : {ex.Message}");
                return new SubscriptionStateResponse
                {
                    BaseResponse = new BaseResponse
                    {
                        IsSuccessful = false,
                        Error = BillingError.SystemError
                    },
                    SubScriptionState = SubScriptionState.Unspecified
                };
            }
        }

        public override async Task<CommonResponse> PurchaseValidate(PurchaseInfo request, ServerCallContext context)
        {
            try
            {
                var (validationResult, validationServiceError) = await billService.PurchaseValidation(request);

                return new CommonResponse
                {
                    BaseResponse = new BaseResponse
                    {
                        IsSuccessful = validationResult,
                        Error = validationServiceError
                    },
                    Result = validationResult
                };
            }
            catch (Exception ex)
            {
                logger.LogError($"PurchaseValidate Error : {ex.Message}");
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
