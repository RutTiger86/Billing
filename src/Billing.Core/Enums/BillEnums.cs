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
        BILL_CREATE_DETAIL_FAILED,         // 구글 검증 에러 
        UNKNOWN                         // 알려지지 않은 상태
    }
}
