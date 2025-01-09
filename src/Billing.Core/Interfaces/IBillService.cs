using Billing.Core.Enums;
using Billing.Core.Models;

namespace Billing.Core.Interfaces
{
    public interface IBillService
    {
        /// <summary>
        /// 구매 검증
        /// Point 구매, 충전의 경우 포인트 프로세스 진행
        /// </summary>
        Task<(bool Result, BillingError error)> PurchaseValidation(PurchaseInfo purchaseInfo);
        /// <summary>
        /// 구독 상태 확인
        /// </summary>
        Task<(SubScriptionState Statue, BillingError error)> SubScriptionStateValidation(long billTxID);
        /// <summary>
        /// 구매 취소 
        /// Point 구매, 충전의 경우 해당 포인트 이력 롤백 처리 
        /// </summary>
        Task<(bool Result, BillingError error)> CanclePurchase(long billTxID);
    }
}
