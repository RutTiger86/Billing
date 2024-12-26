using Billing.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billing.Core.Interfaces
{
    public interface IBillTxService
    {
        public long IssueBillTx(BillTxTypes transactionType);

        public (bool, ValidateError) ValidateBillTx(long billTxId);

        public bool UpdateBillTxState(long billTxId, BillTxStatus status);

        public bool EndBillTx(long billTxId);

        public bool CancleBillTx(long billTxId);
    }
}
