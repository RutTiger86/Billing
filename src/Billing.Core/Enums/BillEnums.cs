using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billing.Core.Enums
{
    public enum ValidateError
    {
        None = 0,           // 에러 없음 
        NotFound,           // 존재하지 않는 Tx
        AlreadyCompleted,   // 이미 완료된 Tx
        Canceled,           // 취소된 Tx
        Unknown             // 알려지지 않은 상태
    }
}
