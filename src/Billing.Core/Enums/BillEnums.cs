using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billing.Core.Enums
{
    public enum BillingError
    {
        NONE = 0,                       // 에러 없음 
        SYSTEM_ERROR,                   // 시스템 에러 
        TX_NOTFFOUND,                   // 존재하지 않는 Tx
        TX_ALREADY_COMPLETED,           // 이미 완료된 Tx
        TX_CANCLED,                     // 취소된 Tx
        TX_ALREADY_INPROGRESS,          // 지급 진행중인 Tx
        TX_UNVERIFIABLE_TYPE,           // 검증 불가 타입 
        PURCHASE_VALIDATION_FAILED,     // 구매 검증 실패 
        PURCHASE_GOOGLE_VALIDATE_ERROR, // 구글 검증 에러 
        BILL_CREATE_DETAIL_FAILED,      // 빌링 정보 생성 에러 
        BILL_COMPLETE_DETAIL_FAILED,    // 빌링 정보 완료 에러 
        TX_COMPLETE_FAILED,             // Tx 완료 에러 
        SUBSCRIPTION_NOTFFOUND,                   // 존재하지 않는 Tx
        UNKNOWN                         // 알려지지 않은 상태
    }


    public enum BillDetailTypes
    {
        CONSUMABLE,
        NON_CONSUMABLE,
        SUBSCRIPTION_AUTO,
        SOBSCRIPTION_NON_AUTO,
        CHARGE,
        REFUND
    }

    public enum BillProductType
    {
        CONSUMABLE = 0,
        NON_CONSUMABLE,
        SUBSCRIPTION,
    }

    public enum SubScriptionState
    {
        /// <summary>
        /// 지정되지 않은 정기 결제 상태입니다.
        /// </summary>
        SUBSCRIPTION_STATE_UNSPECIFIED,
        /// <summary>
        /// 정기 결제가 생성되었지만 가입하는 동안 결제를 기다리고 있습니다. 이 상태에서는 모든 항목이 결제를 기다리고 있습니다.
        /// </summary>
        SUBSCRIPTION_STATE_PENDING,
        /// <summary>
        /// 구독이 활성 상태입니다. - (1) 정기 결제가 자동 갱신 요금제인 경우 하나 이상의 항목이 autoRenewEnabled이고 만료되지 않았습니다. - (2) 정기 결제가 선불 요금제인 경우 하나 이상의 항목이 만료되지 않았습니다.
        /// </summary>
        SUBSCRIPTION_STATE_ACTIVE,
        /// <summary>
        /// 정기 결제가 일시중지되었습니다. 이 상태는 정기 결제가 자동 갱신 요금제인 경우에만 제공됩니다. 이 상태에서는 모든 항목이 일시중지된 상태입니다.
        /// </summary>
        SUBSCRIPTION_STATE_PAUSED,
        /// <summary>
        /// 정기 결제가 유예 기간입니다. 이 상태는 정기 결제가 자동 갱신 요금제인 경우에만 제공됩니다. 이 상태에서는 모든 항목이 유예 기간입니다.
        /// </summary>
        SUBSCRIPTION_STATE_IN_GRACE_PERIOD,
        /// <summary>
        /// 정기 결제가 보류 중 (일시중지됨)입니다. 이 상태는 정기 결제가 자동 갱신 요금제인 경우에만 제공됩니다. 이 상태에서는 모든 항목이 보류 중입니다.
        /// </summary>
        SUBSCRIPTION_STATE_ON_HOLD,
        /// <summary>
        /// 정기 결제가 취소되었지만 아직 만료되지 않았습니다. 이 상태는 정기 결제가 자동 갱신 요금제인 경우에만 제공됩니다. 모든 항목에서 autoRenewEnabled가 false로 설정되어 있습니다.
        /// </summary>
        SUBSCRIPTION_STATE_CANCELED,
        /// <summary>
        /// 정기 결제가 만료되었습니다. 모든 항목의 expiryTime이 지났습니다.
        /// </summary>
        SUBSCRIPTION_STATE_EXPIRED,
        /// <summary>
        /// 정기 결제의 대기 중인 거래가 취소되었습니다. 이 대기 중인 구매가 기존 정기 결제에 관한 것이라면 linkedPurchaseToken을 사용하여 해당 정기 결제의 현재 상태를 가져옵니다.
        /// </summary>
        SUBSCRIPTION_STATE_PENDING_PURCHASE_CANCELED
    }

    public enum AcknowledgementState
    {
        ACKNOWLEDGEMENT_STATE_UNSPECIFIED,
        ACKNOWLEDGEMENT_STATE_PENDING,
        ACKNOWLEDGEMENT_STATE_ACKNOWLEDGED
    }

}
