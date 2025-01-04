using Billing.Core.Enums;
using Billing.Core.Extensions;
using Billing.Core.Interfaces;
using Billing.Core.Models.DataBase;
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
            AddTable<ProductItem>("Id");
            AddTable<Item>("Id");
            AddTable<SubscriptionInfo>("Id");

            ProductDataSet();
        }

        private void ProductDataSet()
        {
            DataTable itemTable = memoryDataSet.Tables[typeof(Item).Name];

            DataRow newRow = itemTable.NewRow();
            newRow["Id"] = 1;
            newRow["ItemName"] = "Test Item 1";
            newRow["CreateDate"] = DateTime.Now;
            newRow["UpdateDate"] = DateTime.Now;

            itemTable.Rows.Add(newRow);

            newRow = itemTable.NewRow();
            newRow["Id"] = 2;
            newRow["ItemName"] = "Test Item 2";
            newRow["CreateDate"] = DateTime.Now;
            newRow["UpdateDate"] = DateTime.Now;

            itemTable.Rows.Add(newRow);

            DataTable productTable = memoryDataSet.Tables[typeof(Product).Name];

            newRow = productTable.NewRow();
            newRow["Id"] = 1;
            newRow["ProductKey"] = "ruttiger.billing.item1";
            newRow["ProductName"] = "Test Package 1";
            newRow["IsUse"] = true;
            newRow["CreateDate"] = DateTime.Now;
            newRow["UpdateDate"] = DateTime.Now;

            productTable.Rows.Add(newRow);

            newRow = productTable.NewRow();
            newRow["Id"] = 2;
            newRow["ProductKey"] = "ruttiger.billing.item2";
            newRow["ProductName"] = "Test Package 2";
            newRow["IsUse"] = true;
            newRow["CreateDate"] = DateTime.Now;
            newRow["UpdateDate"] = DateTime.Now;

            productTable.Rows.Add(newRow);


            DataTable productItemTable = memoryDataSet.Tables[typeof(ProductItem).Name];

            newRow = productItemTable.NewRow();
            newRow["Id"] = 1;
            newRow["ProductId"] = 1;
            newRow["Types"] = ProductTypes.CHARACTER_ITEM;
            newRow["ItemId"] = 1;
            newRow["ItemVolume"] = 1;
            newRow["IsUse"] = true;
            newRow["CreateDate"] = DateTime.Now;
            newRow["UpdateDate"] = DateTime.Now;

            productItemTable.Rows.Add(newRow);

            newRow = productItemTable.NewRow();
            newRow["Id"] = 2;
            newRow["ProductId"] = 2;
            newRow["Types"] = ProductTypes.CHARACTER_ITEM;
            newRow["ItemId"] = 1;
            newRow["ItemVolume"] = 1;
            newRow["IsUse"] = true;
            newRow["CreateDate"] = DateTime.Now;
            newRow["UpdateDate"] = DateTime.Now;

            productItemTable.Rows.Add(newRow);

            newRow = productItemTable.NewRow();
            newRow["Id"] = 3;
            newRow["ProductId"] = 2;
            newRow["Types"] = ProductTypes.CHARACTER_ITEM;
            newRow["ItemId"] = 2;
            newRow["ItemVolume"] = 10;
            newRow["IsUse"] = true;
            newRow["CreateDate"] = DateTime.Now;
            newRow["UpdateDate"] = DateTime.Now;

            productItemTable.Rows.Add(newRow);

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


        public BillDetail GetBillDetail(long billDetailId)
        {
            throw new NotImplementedException();
        }

        public List<BillDetail> GetBillDetails(long billTxId)
        {
            throw new NotImplementedException();
        }


        public BillTx GetBillTx(long billTxId, bool isDeleted = false)
        {
            DataTable billTxTable = memoryDataSet.Tables[typeof(BillTx).Name];
            return billTxTable.AsEnumerable().Where(p => p.Field<bool>("IsDeleted") == isDeleted)
                .FirstOrDefault(p => p.Field<long>("Id") == billTxId)?.ToClass<BillTx>();
        }

        public SubscriptionInfo GetSubscriptionInfo(long billTxId)
        {
            DataTable subscriptionInfoTable = memoryDataSet.Tables[typeof(SubscriptionInfo).Name];
            return subscriptionInfoTable.AsEnumerable().FirstOrDefault(p => p.Field<long>("Id") == billTxId)?.ToClass<SubscriptionInfo>();
        }


        public long InsertBillTx(BillTx billTx)
        {
            lock (lockObj)
            {
                DateTime now = DateTime.UtcNow;
                DataTable billTxTable = memoryDataSet.Tables[typeof(BillTx).Name];

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

        public long InsertBillDetail(BillDetail billDetail)
        {
            lock (lockObj)
            {
                DateTime now = DateTime.UtcNow;
                DataTable billDetailTable = memoryDataSet.Tables[typeof(BillDetail).Name];

                long maxId = billDetailTable.Rows.Count > 0
                    ? billDetailTable.AsEnumerable().Max(row => row.Field<long>("Id"))
                    : 0;

                billDetail.Id = maxId + 1;
                billDetail.CreateDate = now;
                billDetail.UpdateDate = now;
                billDetailTable.AddRow<BillDetail>(billDetail);

                return billDetail.Id;
            }
        }

        public long InsertSubscriptionInfo(SubscriptionInfo subscriptionInfo)
        {
            lock (lockObj)
            {
                DateTime now = DateTime.UtcNow;
                DataTable subscriptionInfoTable = memoryDataSet.Tables[typeof(SubscriptionInfo).Name];

                long maxId = subscriptionInfoTable.Rows.Count > 0
                    ? subscriptionInfoTable.AsEnumerable().Max(row => row.Field<long>("Id"))
                    : 0;

                subscriptionInfo.Id = maxId + 1;
                subscriptionInfo.CreateDate = now;
                subscriptionInfo.UpdateDate = now;
                subscriptionInfoTable.AddRow<SubscriptionInfo>(subscriptionInfo);

                return subscriptionInfo.Id;
            }
        }

        public bool UpdateBillDetail(long billDetailId, BillTxStatus status)
        {
            lock (lockObj)
            {
                DataTable billDetailTable = memoryDataSet.Tables[typeof(BillDetail).Name];
                if (billDetailTable == null)
                {
                    return false;
                }

                var billDetailRow = billDetailTable.AsEnumerable()
                    .FirstOrDefault(p => p.Field<long>("Id") == billDetailId);

                if (billDetailRow == null)
                {
                    return false; 
                }

                billDetailRow["Status"] = status;
                billDetailRow["UpdateDate"] = DateTime.Now; 

                return true; 
            }
        }

        public bool CompleteBillDetail(long billTxId)
        {
            lock (lockObj)
            {
                DataTable billDetailTable = memoryDataSet.Tables[typeof(BillDetail).Name];
                if (billDetailTable == null)
                {
                    return false;
                }

                var billDetailRows = billDetailTable.AsEnumerable().Where(p=> p.Field<long>("BillTxId") == billTxId)
                    .ToList();

                if (billDetailRows == null || billDetailRows.Count == 0)
                {
                    return false;
                }

                foreach (var row in billDetailRows)
                {
                    row["Status"] = BillTxStatus.COMPLETED;
                    row["UpdateDate"] = DateTime.Now;
                }

                return true;
            }
        }

        public bool ExpireSubscription(long subscriptionId)
        {
            lock (lockObj)
            {
                DataTable subscriptionInfoTable = memoryDataSet.Tables[typeof(SubscriptionInfo).Name];
                if (subscriptionInfoTable == null)
                {
                    return false;
                }

                var billDetailRow = subscriptionInfoTable.AsEnumerable()
                    .FirstOrDefault(p => p.Field<long>("Id") == subscriptionId);

                if (billDetailRow == null)
                {
                    return false;
                }

                billDetailRow["State"] = SubScriptionState.SUBSCRIPTION_STATE_EXPIRED;
                billDetailRow["IsExpired"] = true;
                billDetailRow["UpdateDate"] = DateTime.Now;

                return true;
            }

        }


        public bool UpdateBillTx(long billTxId, bool isComplete)
        {
            lock (lockObj)
            {
                DataTable billTxTable = memoryDataSet.Tables[typeof(BillTx).Name];
                if (billTxTable == null)
                {
                    return false;
                }

                var billTxRow = billTxTable.AsEnumerable()
                    .FirstOrDefault(p => p.Field<long>("Id") == billTxId);

                if (billTxRow != null)
                {
                    billTxRow["UpdateDate"] = DateTime.Now;
                    billTxRow["IsCompleted"] = isComplete;
                    return true;
                }
                else
                {
                    return false; 
                }
            }
        }


        public bool DeleteBillTx(long billTxId, bool isDeleted)
        {
            lock (lockObj)
            {
                DataTable billTxTable = memoryDataSet.Tables[typeof(BillTx).Name];
                if (billTxTable == null)
                {
                    return false; 
                }

                var billTxRow = billTxTable.AsEnumerable()
                    .FirstOrDefault(p => p.Field<long>("Id") == billTxId);

                if (billTxRow != null)
                {                    
                    billTxRow["UpdateDate"] = DateTime.Now;
                    billTxRow["IsDeleted"] = isDeleted;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public bool UpdateBillTx(long billTxId, string PurchaseToken)
        {
            lock (lockObj)
            {
                DataTable billTxTable = memoryDataSet.Tables[typeof(BillTx).Name];
                if (billTxTable == null)
                {
                    return false; 
                }

                var billTxRow = billTxTable.AsEnumerable()
                    .FirstOrDefault(p => p.Field<long>("Id") == billTxId);

                if (billTxRow != null)
                {
                    billTxRow["UpdateDate"] = DateTime.Now;
                    billTxRow["PurchaseToken"] = PurchaseToken;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public Product GetProduct(string productKey, bool isUse = true)
        {
            var productTable = memoryDataSet.Tables[typeof(Product).Name];
            if (productTable == null || string.IsNullOrEmpty(productKey))
            {
                return null;
            }

            var dataRow = productTable.AsEnumerable()
                .FirstOrDefault(row =>
                    row.Field<bool>("IsUse") == isUse &&
                    row.Field<string>("ProductKey") == productKey);

            return dataRow == null ? null : new Product
            {
                Id = dataRow.Field<long>("Id"),
                ProductKey = dataRow.Field<string>("ProductKey"),
                ProductName = dataRow.Field<string>("ProductName"),
                IsUse = dataRow.Field<bool>("IsUse"),
                CreateDate = dataRow.Field<DateTime>("CreateDate"),
                UpdateDate = dataRow.Field<DateTime>("UpdateDate")
            };
        }
    }
}
