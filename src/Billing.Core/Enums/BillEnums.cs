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
        UNKNOWN                         // 알려지지 않은 상태
    }

    public enum BillProductType
    {
        CONSUMABLE = 0,
        NON_CONSUMABLE,
        SUBSCRIPTION
    }

    public enum SubScriptionState
    {
        SUBSCRIPTION_STATE_UNSPECIFIED,
        SUBSCRIPTION_STATE_PENDING,
        SUBSCRIPTION_STATE_ACTIVE,
        SUBSCRIPTION_STATE_PAUSED,
        SUBSCRIPTION_STATE_IN_GRACE_PERIOD,
        SUBSCRIPTION_STATE_ON_HOLD,
        SUBSCRIPTION_STATE_CANCELED,
        SUBSCRIPTION_STATE_EXPIRED,
        SUBSCRIPTION_STATE_PENDING_PURCHASE_CANCELED
    }

    public enum AcknowledgementState
    {
        ACKNOWLEDGEMENT_STATE_UNSPECIFIED,
        ACKNOWLEDGEMENT_STATE_PENDING,
        ACKNOWLEDGEMENT_STATE_ACKNOWLEDGED
    }

}
