using Billing.Protobuf.Core;
using Billing.Protobuf.Purchase;

namespace Billing.Core.Interfaces
{
    public interface IValidationService
    {
        /// <summary>
        /// 구매 검증 
        /// IAP의 경우 각 Market API를 통해 결재 영수증 검증
        /// Point의 경우 계정 충전 Point 대비 소모 체크 및 소모 진행 
        /// Web의 경우 PG 사의 결재 영수증 검증 
        /// </summary>
        public Task<bool> PurchaseProductValidate(long billDetailId, PurchaseInfo purchaseInfo);
        /// <summary>
        /// 구독 검증
        /// IAP의 경우 각 Market API를 통해 결재 영수증 검증
        /// Point의 경우 계정 충전 Point 대비 소모 체크 및 소모 진행 
        /// Web의 경우 PG 사의 결재 영수증 검증 
        /// </summary>
        public Task<bool> PruchaseSubscriptionsValidate(long billDetailId, PurchaseInfo purchaseInfo);
        /// <summary>
        /// 구독 상태 확인
        /// </summary>
        public Task<SubScriptionState> SubscriptionsValidate(string purchaseToken);
    }
}
