using Billing.Core.Enums;
using Billing.Core.Interfaces;
using Billing.Core.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billing.Core.Services
{
    public class MemoryDataService : IDataService
    {
        private ILogger<MemoryDataService> logger;

        private readonly DataSet memoryDataSet;

        public MemoryDataService(ILogger<MemoryDataService> logger)
        {
            this.logger = logger;
            memoryDataSet = new DataSet();
            InitMemoryDataSet();
        }
        
        private void InitMemoryDataSet()
        {
            AddTable<BillTx>("Id");
        }

        public void AddTable<T>(string IdName) where T : class
        {
            var tableName = typeof(T).Name;
            if (memoryDataSet.Tables.Contains(tableName)) throw new Exception($"Table {tableName} already exists.");

            DataTable table = new DataTable(tableName);

            foreach (var prop in typeof(T).GetProperties())
            {
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }

            if (table.Columns.Contains(IdName))
            {
                table.PrimaryKey = new[] { table.Columns[IdName] };
            }

            memoryDataSet.Tables.Add(table);
        }


        public bool DeleteBillTxDetail(long BillTxId)
        {
            throw new NotImplementedException();
        }

        public BillDetail GetBillDetail(long BillDetailId)
        {
            throw new NotImplementedException();
        }

        public BillDetail GetBillDetails(long BillTxId)
        {
            throw new NotImplementedException();
        }

        public BillTx GetBillTx(long BillTxId)
        {
            throw new NotImplementedException();
        }

        public long InsertBillTx(BillTx BillTx)
        {
            throw new NotImplementedException();
        }

        public bool InsertBillTxDetails(BillDetail billDetail)
        {
            throw new NotImplementedException();
        }

        public bool UpdateBillTxDetailsDetail(long BillDetailId)
        {
            throw new NotImplementedException();
        }

        public bool UpdateBillTxState(long BillTxId, BillTxStatus status)
        {
            throw new NotImplementedException();
        }

        public bool UpdateBillTxState(long BillTxId, BillTxStatus status, bool IsComplete)
        {
            throw new NotImplementedException();
        }

        public bool UpdateBillTxState(long BillTxId, bool IsDeleted)
        {
            throw new NotImplementedException();
        }
    }
}
