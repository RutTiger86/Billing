using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billing.Core.Enums
{
    public enum BillingError
    {
        NONE = 0,           // 에러 없음 
        TX_NOTFFOUND,           // 존재하지 않는 Tx
        TX_ALREADY_COMPLETED,   // 이미 완료된 Tx
        TX_CANCLED,           // 취소된 Tx
        UNKNOWN             // 알려지지 않은 상태
    }
}
