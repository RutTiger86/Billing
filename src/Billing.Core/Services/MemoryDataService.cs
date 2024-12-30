using Billing.Core.Enums;
using Billing.Core.Extensions;
using Billing.Core.Interfaces;
using Billing.Core.Models;
using Microsoft.Extensions.Logging;
using System.Data;

namespace Billing.Core.Services
{
    public class MemoryDataService : IDataService
    {
        private ILogger<MemoryDataService> logger;

        private readonly DataSet memoryDataSet;

        private readonly object lockObj = new();
        public MemoryDataService(ILogger<MemoryDataService> logger)
        {
            this.logger = logger;
            memoryDataSet = new DataSet();
            InitMemoryDataSet();
        }
        
        private void InitMemoryDataSet()
        {
            AddTable<BillTx>("Id");
            AddTable<BillDetail>("Id");
            AddTable<Product>("Id");
            AddTable<ProductDetail>("Id");
            AddTable<Item>("Id");
        }

        private void AddTable<T>(string idName) where T : class
        {
            var tableName = typeof(T).Name;

            DataTable table = new (tableName);

            foreach (var prop in typeof(T).GetProperties())
            {
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }

            if (table.Columns.Contains(idName))
            {
                table.PrimaryKey = new[] { table.Columns[idName] };
            }

            memoryDataSet.Tables.Add(table);
        }

        private BillTx SelectBillTx(long billTxId, bool isDeleted  = false)
        {
            DataTable billTxTable = memoryDataSet.Tables[typeof(BillTx).Name];
            return billTxTable.AsEnumerable().Where(p=> p.Field<bool>("IsDeleted") == isDeleted)
                .FirstOrDefault(p => p.Field<long>("Id") == billTxId)?.ToClass<BillTx>();
        }


        public BillDetail GetBillDetail(long billDetailId)
        {
            throw new NotImplementedException();
        }

        public BillDetail GetBillDetails(long billTxId)
        {
            throw new NotImplementedException();
        }


        public BillTx GetBillTx(long billTxId, bool isDeleted = false)
        {
            var billTx = SelectBillTx(billTxId, isDeleted);
            return billTx.DeepCopy<BillTx>();
        }
        

        public long InsertBillTx(BillTx billTx)
        {
            lock (lockObj)
            {
                DateTime now = DateTime.UtcNow;
                DataTable billTxTable = memoryDataSet.Tables[typeof(BillTx).Name];
                // 행이 없는 경우 ID를 1로 설정
                long maxId = billTxTable.Rows.Count > 0
                    ? billTxTable.AsEnumerable().Max(row => row.Field<long>("Id"))
                    : 0;

                billTx.Id = maxId + 1;
                billTx.CreateDate = now;
                billTx.UpdateDate = now;
                billTxTable.AddRow<BillTx>(billTx);
                return billTx.Id;
            }
        }

        public bool InsertBillTxDetails(BillDetail billDetail)
        {
            throw new NotImplementedException();
        }

        public bool UpdateBillTxDetailsDetail(long billDetailId)
        {
            throw new NotImplementedException();
        }

        public bool UpdateBillTxState(long billTxId, BillTxStatus status)
        {
            lock (lockObj)
            {

                var billTx = SelectBillTx(billTxId);

                if (billTx != null)
                {
                    billTx.UpdateDate = DateTime.Now;
                    billTx.Status = status;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool UpdateBillTxState(long billTxId, BillTxStatus status, bool isComplete)
        {
            lock (lockObj)
            {
                var billTx = SelectBillTx(billTxId);

                if (billTx != null)
                {
                    billTx.UpdateDate = DateTime.Now;
                    billTx.Status = status;
                    billTx.IsCompleted = isComplete;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool UpdateBillTxState(long billTxId, bool isDeleted)
        {
            lock (lockObj)
            {
                var billTx = SelectBillTx(billTxId);

                if (billTx != null)
                {
                    billTx.UpdateDate = DateTime.Now;
                    billTx.IsDeleted = isDeleted;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
