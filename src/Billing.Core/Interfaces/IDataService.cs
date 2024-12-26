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
        public long InsertBillTx(BillTx BillTx);
        public bool UpdateBillTxState(long BillTxId, BillTxStatus status);
        public bool UpdateBillTxState(long BillTxId, BillTxStatus status, bool IsComplete);
        public bool UpdateBillTxState(long BillTxId, bool IsDeleted);
        public bool DeleteBillTxDetail(long BillTxId);
        public bool InsertBillTxDetails(BillDetail billDetail);
        public bool UpdateBillTxDetailsDetail(long BillDetailId);
        public BillTx GetBillTx(long BillTxId);
        public BillDetail GetBillDetails(long BillTxId);
        public BillDetail GetBillDetail(long BillDetailId);

    }
}
