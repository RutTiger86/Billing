using Billing.Core.Enums;
using Billing.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billing.Core.Interfaces
{
    public interface IDataService
    {
        public long InsertBillTx(BillTx billTx);
        public bool UpdateBillTxState(long billTxId, BillTxStatus status);
        public bool UpdateBillTxState(long billTxId, BillTxStatus status, bool isComplete);
        public bool UpdateBillTxState(long billTxId, bool isDeleted);
        public bool InsertBillTxDetails(BillDetail billDetail);
        public bool UpdateBillTxDetailsDetail(long billDetailId);
        public BillTx GetBillTx(long billTxId);
        public BillDetail GetBillDetails(long billTxId);
        public BillDetail GetBillDetail(long billDetailId);

    }
}
