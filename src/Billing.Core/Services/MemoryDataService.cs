using Billing.Core.Extensions;
using Billing.Core.Interfaces;
using Billing.Core.Models.DataBase;
using Billing.Protobuf.Core;
using Billing.Protobuf.Point;
using Billing.Protobuf.Product;
using Microsoft.Extensions.Logging;
using System.Data;

namespace Billing.Core.Services
{
    public class MemoryDataService : IDataService
    {
        private ILogger<MemoryDataService> logger;

        private readonly DataSet memoryDataSet;

        // Todo : Net9 변경시 System.Threading.Lock 클래스 사용 
        private readonly object lockObj = new();

        public MemoryDataService(ILogger<MemoryDataService> logger)
        {
            this.logger = logger;
            memoryDataSet = new DataSet();
            InitMemoryDataSet();
        }

        /// <summary>
        /// 인메모리 테이블 생성 
        /// </summary>
        private void InitMemoryDataSet()
        {
            AddTable<BillTx>("Id");
            AddTable<BillDetail>("Id");
            AddTable<Product>("Id");
            AddTable<ProductItem>("Id");
            AddTable<Item>("Id");
            AddTable<SubscriptionInfo>("Id");
            AddTable<Ledger>("Id");
            AddTable<PointHistory>("Id");

            ProductDataSet();
        }

        /// <summary>
        /// 테스트 용 상품 정보 세팅 
        /// </summary>
        private void ProductDataSet()
        {
            DataTable itemTable = memoryDataSet.Tables[typeof(Item).Name] ?? throw new Exception("Item Table is null");

            DataRow newRow = itemTable.NewRow();
            newRow["Id"] = 1;
            newRow["ItemName"] = "Test Item 1";
            newRow["PointType"] = PointType.None;
            newRow["CreateDate"] = DateTime.Now;
            newRow["UpdateDate"] = DateTime.Now;

            itemTable.Rows.Add(newRow);

            newRow = itemTable.NewRow();
            newRow["Id"] = 2;
            newRow["ItemName"] = "Test Item 2";
            newRow["PointType"] = PointType.None;
            newRow["CreateDate"] = DateTime.Now;
            newRow["UpdateDate"] = DateTime.Now;

            itemTable.Rows.Add(newRow);

            newRow = itemTable.NewRow();
            newRow["Id"] = 3;
            newRow["ItemName"] = "Diamond";
            newRow["PointType"] = PointType.Paid;
            newRow["CreateDate"] = DateTime.Now;
            newRow["UpdateDate"] = DateTime.Now;

            itemTable.Rows.Add(newRow);

            DataTable productTable = memoryDataSet.Tables[typeof(Product).Name] ?? throw new Exception("Product Table is null");

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


            newRow = productTable.NewRow();
            newRow["Id"] = 3;
            newRow["ProductKey"] = "ruttiger.billing.item3";
            newRow["ProductName"] = "Diamond Package 3";
            newRow["IsUse"] = true;
            newRow["CreateDate"] = DateTime.Now;
            newRow["UpdateDate"] = DateTime.Now;

            productTable.Rows.Add(newRow);

            DataTable productItemTable = memoryDataSet.Tables[typeof(ProductItem).Name] ?? throw new Exception("Product Item Table is null");

            newRow = productItemTable.NewRow();
            newRow["Id"] = 1;
            newRow["ProductId"] = 1;
            newRow["Types"] = ProductTypes.CharacterItem;
            newRow["ItemId"] = 1;
            newRow["ItemVolume"] = 1;
            newRow["IsUse"] = true;
            newRow["CreateDate"] = DateTime.Now;
            newRow["UpdateDate"] = DateTime.Now;

            productItemTable.Rows.Add(newRow);

            newRow = productItemTable.NewRow();
            newRow["Id"] = 2;
            newRow["ProductId"] = 2;
            newRow["Types"] = ProductTypes.CharacterItem;
            newRow["ItemId"] = 1;
            newRow["ItemVolume"] = 1;
            newRow["IsUse"] = true;
            newRow["CreateDate"] = DateTime.Now;
            newRow["UpdateDate"] = DateTime.Now;

            productItemTable.Rows.Add(newRow);

            newRow = productItemTable.NewRow();
            newRow["Id"] = 3;
            newRow["ProductId"] = 2;
            newRow["Types"] = ProductTypes.CharacterItem;
            newRow["ItemId"] = 2;
            newRow["ItemVolume"] = 10;
            newRow["IsUse"] = true;
            newRow["CreateDate"] = DateTime.Now;
            newRow["UpdateDate"] = DateTime.Now;

            productItemTable.Rows.Add(newRow);

            newRow = productItemTable.NewRow();
            newRow["Id"] = 4;
            newRow["ProductId"] = 3;
            newRow["Types"] = ProductTypes.Point;
            newRow["ItemId"] = 3;
            newRow["ItemVolume"] = 50000;
            newRow["IsUse"] = true;
            newRow["CreateDate"] = DateTime.Now;
            newRow["UpdateDate"] = DateTime.Now;

            productItemTable.Rows.Add(newRow);

        }

        private void AddTable<T>(string idName) where T : class
        {
            DataTable table = new(typeof(T).Name);

            foreach (var prop in typeof(T).GetProperties())
            {
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }

            if (table.Columns.Contains(idName))
            {
                table.PrimaryKey = [table.Columns[idName]];
            }

            memoryDataSet.Tables.Add(table);
        }

        public List<Ledger> SelectLedger(long accountId)
        {
            var ledgerTable = memoryDataSet.Tables[typeof(Ledger).Name];
            if (ledgerTable == null)
            {
                return null;
            }

            return ledgerTable.AsEnumerable()
                .Where(row => row.Field<long>("AccountId") == accountId)
                .Select(row => new Ledger
                {
                    Id = row.Field<long>("Id"),
                    AccountId = row.Field<long>("AccountId"),
                    Type = (PointType)row.Field<int>("Type"),
                    Balance = row.Field<long>("Balance"),
                    CreateDate = row.Field<DateTime>("CreateDate"),
                    UpdateDate = row.Field<DateTime>("UpdateDate")
                }).ToList();
        }

        public Ledger SelectLedgerByPointType(long accountId, PointType pointType)
        {
            var ledgerTable = memoryDataSet.Tables[typeof(Ledger).Name];
            if (ledgerTable == null)
            {
                return null;
            }

            return ledgerTable.AsEnumerable()
                .Where(row => row.Field<long>("AccountId") == accountId && row.Field<int>("PointType") == (int)pointType)
                .Select(row => new Ledger
                {
                    Id = row.Field<long>("Id"),
                    AccountId = row.Field<long>("AccountId"),
                    Type = (PointType)row.Field<int>("Type"),
                    Balance = row.Field<long>("Balance"),
                    CreateDate = row.Field<DateTime>("CreateDate"),
                    UpdateDate = row.Field<DateTime>("UpdateDate")
                })
                .FirstOrDefault();
        }


        public BillTx SelectBillTx(long billTxId)
        {
            var billTxTable = memoryDataSet.Tables[typeof(BillTx).Name];
            if (billTxTable == null)
            {
                return null;
            }

            return billTxTable.AsEnumerable().FirstOrDefault(p => p.Field<long>("Id") == billTxId)?.ToClass<BillTx>();
        }

        public SubscriptionInfo SelectSubscriptionInfo(long billTxId)
        {
            var subscriptionInfoTable = memoryDataSet.Tables[typeof(SubscriptionInfo).Name];
            if (subscriptionInfoTable == null)
            {
                return null;
            }

            return subscriptionInfoTable.AsEnumerable().FirstOrDefault(p => p.Field<long>("Id") == billTxId)?.ToClass<SubscriptionInfo>();
        }
        
        public long InsertBillTx(BillTx billTx)
        {
            lock (lockObj)
            {
                DateTime now = DateTime.UtcNow;

                var billTxTable = memoryDataSet.Tables[typeof(BillTx).Name];
                if(billTxTable == null)
                {
                    return -1;
                }

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

                var billDetailTable = memoryDataSet.Tables[typeof(BillDetail).Name];
                if (billDetailTable == null)
                {
                    return -1;
                }

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

                var subscriptionInfoTable = memoryDataSet.Tables[typeof(SubscriptionInfo).Name];
                if (subscriptionInfoTable == null)
                {
                    return -1;
                }

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

        public long InsertLedger(Ledger ledger)
        {
            lock (lockObj)
            {
                DateTime now = DateTime.UtcNow;

                var ledgerTable = memoryDataSet.Tables[typeof(Ledger).Name];
                if (ledgerTable == null)
                {
                    return -1;
                }

                long maxId = ledgerTable.Rows.Count > 0
                    ? ledgerTable.AsEnumerable().Max(row => row.Field<long>("Id"))
                    : 0;

                ledger.Id = maxId + 1;
                ledger.CreateDate = now;
                ledger.UpdateDate = now;
                ledgerTable.AddRow<Ledger>(ledger);
                return ledger.Id;
            }
        }

        public long InsertPointHistory(PointHistory pointHistory)
        {
            DateTime now = DateTime.UtcNow;

            var pointHistoryTable = memoryDataSet.Tables[typeof(PointHistory).Name];
            if (pointHistoryTable == null)
            {
                return -1;
            }

            long maxId = pointHistoryTable.Rows.Count > 0
                ? pointHistoryTable.AsEnumerable().Max(row => row.Field<long>("Id"))
                : 0;

            pointHistory.Id = maxId + 1;
            pointHistory.CreateDate = now;
            pointHistory.UpdateDate = now;
            pointHistoryTable.AddRow<PointHistory>(pointHistory);
            return pointHistory.Id;
        }

        public int UpdateBillTxStatus(long billTxId, BillTxStatus status, bool IsDone = false)
        {
            lock (lockObj)
            {
                var billTxTable = memoryDataSet.Tables[typeof(BillTx).Name];
                if (billTxTable == null)
                {
                    return -1;
                }

                var billTxRow = billTxTable.AsEnumerable()
                    .FirstOrDefault(p => p.Field<long>("Id") == billTxId);

                if (billTxRow == null)
                {
                    return -1;
                }

                billTxRow["Status"] = status;
                billTxRow["UpdateDate"] = DateTime.Now;
                if (IsDone)
                    billTxRow["IsDone"] = IsDone;

                return 1;
            }
        }

        public int BillTxValidateStart(long billTxId)
        {
            lock (lockObj)
            {
                var billTxTable = memoryDataSet.Tables[typeof(BillTx).Name];
                if (billTxTable == null)
                {
                    return -1;
                }

                var billTxRow = billTxTable.AsEnumerable()
                    .FirstOrDefault(p => p.Field<long>("Id") == billTxId && p.Field<int>("Status") == (int)BillTxStatus.Initiated);

                if (billTxRow == null)
                {
                    return -1;
                }

                billTxRow["Status"] = BillTxStatus.ValidateStart;
                billTxRow["UpdateDate"] = DateTime.Now;

                return 1;
            }
        }

        public int ExpireSubscription(long subscriptionId)
        {
            lock (lockObj)
            {
                var subscriptionInfoTable = memoryDataSet.Tables[typeof(SubscriptionInfo).Name];
                if (subscriptionInfoTable == null)
                {
                    return -1;
                }

                var subscriptionInfoRow = subscriptionInfoTable.AsEnumerable()
                    .FirstOrDefault(p => p.Field<long>("Id") == subscriptionId);

                if (subscriptionInfoRow == null)
                {
                    return -1;
                }

                subscriptionInfoRow["State"] = SubScriptionState.Expired;
                subscriptionInfoRow["IsExpired"] = true;
                subscriptionInfoRow["UpdateDate"] = DateTime.Now;

                return 1;
            }

        }
        
        public int UpdateBillTxToken(long billTxId, string PurchaseToken)
        {
            lock (lockObj)
            {
                var billTxTable = memoryDataSet.Tables[typeof(BillTx).Name];
                if (billTxTable == null)
                {
                    return -1;
                }

                var billTxRow = billTxTable.AsEnumerable()
                    .FirstOrDefault(p => p.Field<long>("Id") == billTxId);

                if (billTxRow != null)
                {
                    billTxRow["UpdateDate"] = DateTime.Now;
                    billTxRow["PurchaseToken"] = PurchaseToken;
                    return 1;
                }
                else
                {
                    return -1;
                }
            }
        }

        public Product SelectProduct(string productKey, bool isUse = true)
        {
            var productTable = memoryDataSet.Tables[typeof(Product).Name];
            if (productTable == null || string.IsNullOrEmpty(productKey))
            {
                return null;
            }

            var productRow = productTable.AsEnumerable()
                .FirstOrDefault(row =>
                    row.Field<bool>("IsUse") == isUse &&
                    row.Field<string>("ProductKey") == productKey);

            return productRow == null ? null : new Product
            {
                Id = productRow.Field<long>("Id"),
                ProductKey = productRow.Field<string>("ProductKey")??String.Empty,
                ProductName = productRow.Field<string>("ProductName") ?? String.Empty,
                Price = productRow.Field<int>("Price"),
                IsUse = productRow.Field<bool>("IsUse"),
                CreateDate = productRow.Field<DateTime>("CreateDate"),
                UpdateDate = productRow.Field<DateTime>("UpdateDate")
            };
        }

        public ProductInfo SelectProductInfoByProductKey(string productKey)
        {
            var productTable = memoryDataSet.Tables[nameof(Product)];
            var productItemTable = memoryDataSet.Tables[nameof(ProductItem)];
            var itemTable = memoryDataSet.Tables[nameof(Item)];

            var query = from product in productTable.AsEnumerable() where product.Field<string>("ProductKey") == productKey
                        join productItem in productItemTable.AsEnumerable() on product.Field<long>("Id") equals productItem.Field<long>("ProductId")
                        join item in itemTable.AsEnumerable() on productItem.Field<long>("ItemId") equals item.Field<long>("Id")
                        group productItem by new
                        {
                            ProductId = product.Field<long>("Id"),
                            ProductKey = product.Field<string>("ProductKey"),
                            ProductName = product.Field<string>("ProductName"),
                            IsUse = product.Field<bool>("IsUse"),
                        } into productGroup
                        select new
                        {
                            ProductInfo = new ProductInfo
                            {
                                Id = productGroup.Key.ProductId,
                                ProductKey = productGroup.Key.ProductKey,
                                ProductName = productGroup.Key.ProductName,
                                IsUse = productGroup.Key.IsUse
                            },
                            Items = productGroup.Select(pg => new ProductItemInfo
                            {
                                Id = pg.Field<long>("Id"),
                                ProductId = pg.Field<long>("ProductId"),
                                ProductType = (ProductTypes)pg.Field<int>("Types"),
                                ItemId = pg.Field<long>("ItemId"),
                                ItemVolume = pg.Field<int>("ItemVolume"),
                                IsUse = pg.Field<bool>("IsUse"),
                                ItemName = pg.Field<string>("ItemName"),
                                PointType = (PointType)pg.Field<int>("PointType")
                            }).ToList()
                        };

            var result = query.FirstOrDefault();
            result?.ProductInfo.Items.AddRange(result.Items);

            return result?.ProductInfo;
        }

        public int ChargeLedger(long accountId, PointType pointType, long amount)
        {
            lock (lockObj)
            {
                var ledgerTable = memoryDataSet.Tables[nameof(Ledger)];
                if (ledgerTable == null)
                {
                    return -1;
                }

                var ledgerRow = ledgerTable.AsEnumerable()
                    .FirstOrDefault(p => p.Field<long>("AccountId") == accountId && p.Field<int>("PointType") == (int)pointType);

                if (ledgerRow == null)
                {
                    return -1;
                }

                ledgerRow["UpdateDate"] = DateTime.Now;
                ledgerRow["Balance"] = (long)ledgerRow["Balance"] + amount;
            }

            return 1;
        }


        public int ChargeRollBackLedger(long pointHistoryId, long accountId, PointType pointType, long amount)
        {
            lock (lockObj)
            {
                var ledgerTable = memoryDataSet.Tables[nameof(Ledger)];
                var pointHistoryTable = memoryDataSet.Tables[typeof(PointHistory).Name];
                if (pointHistoryTable == null)
                {
                    return -1;
                }
                if (ledgerTable == null)
                {
                    return -1;
                }


                var pointHistoryRow = pointHistoryTable.AsEnumerable().FirstOrDefault(p => p.Field<long>("Id") == pointHistoryId);
                var ledgerRow = ledgerTable.AsEnumerable()
                    .FirstOrDefault(p => p.Field<long>("AccountId") == accountId && p.Field<int>("PointType") == (int)pointType);
                if (pointHistoryRow == null)
                {
                    return -1;
                }
                if (ledgerRow == null)
                {
                    return -1;
                }

                pointHistoryRow["IsRollBack"] = true;
                pointHistoryRow["UpdateDate"] = DateTime.Now;

                ledgerRow["UpdateDate"] = DateTime.Now;
                ledgerRow["Balance"] = (long)ledgerRow["Balance"] - amount;


                ledgerRow = ledgerTable.AsEnumerable()
                  .FirstOrDefault(p => p.Field<long>("AccountId") == accountId && p.Field<int>("PointType") == (int)pointType);

                if((long)ledgerRow["Balance"] < 0 )
                {
                    pointHistoryRow["IsRollBack"] = false;
                    pointHistoryRow["UpdateDate"] = DateTime.Now;

                    ledgerRow["UpdateDate"] = DateTime.Now;
                    ledgerRow["Balance"] = (long)ledgerRow["Balance"] + amount;

                    return -1;
                }

            }

            return 1;
        }

        public int WithdrawLedger(long accountId, PointType pointType, long amount)
        {
            lock (lockObj)
            {
                var ledgerTable = memoryDataSet.Tables[typeof(Ledger).Name];
                if (ledgerTable == null)
                {
                    return -1;
                }

                var ledgerRow = ledgerTable.AsEnumerable()
                    .FirstOrDefault(row => row.Field<long>("AccountId") == accountId && row.Field<int>("PointType") == (int)pointType);
                if (ledgerRow == null)
                {
                    return -1;
                }

                ledgerRow["UpdateDate"] = DateTime.Now;
                ledgerRow["Balance"] = (long)ledgerRow["Balance"] - amount;

                return 1;
            }
        }

        public int WithdrawRollBackLedger(long pointHistoryId, long accountId, PointType pointType, long amount)
        {
            lock (lockObj)
            {
                var ledgerTable = memoryDataSet.Tables[typeof(Ledger).Name];
                var pointHistoryTable = memoryDataSet.Tables[typeof(PointHistory).Name];
                if (pointHistoryTable == null)
                {
                    return -1;
                }
                if (ledgerTable == null)
                {
                    return -1;
                }

                var pointHistoryRow = pointHistoryTable.AsEnumerable().FirstOrDefault(p => p.Field<long>("Id") == pointHistoryId);
                var ledgerRow = ledgerTable.AsEnumerable()
                    .FirstOrDefault(row => row.Field<long>("AccountId") == accountId && row.Field<int>("PointType") == (int)pointType);
                if (pointHistoryRow == null)
                {
                    return -1;
                }
                if (ledgerRow == null)
                {
                    return -1;
                }

                pointHistoryRow["IsRollBack"] = true;
                pointHistoryRow["UpdateDate"] = DateTime.Now;               
                ledgerRow["UpdateDate"] = DateTime.Now;
                ledgerRow["Balance"] = (long)ledgerRow["Balance"] + amount;

                return 1;
            }
        }

        public List<PointHistory> SelectPointHistories(long billTxId)
        {
            lock (lockObj)
            {
                var pointHistoryTable = memoryDataSet.Tables[typeof(PointHistory).Name];

                return pointHistoryTable.AsEnumerable()
                    .Where(row => row.Field<long>("BillTxId") == billTxId)
                    .Select(row => new PointHistory
                    {
                        Id = row.Field<long>("Id"),
                        BillTxId = row.Field<long>("BillTxId"),
                        ProductId = row.Field<long>("ProductId"),
                        PointOperationType = (PointOperationType)row.Field<int>("PointOperationType"),
                        AccountId = row.Field<long>("AccountId"),
                        PointType = (PointType)row.Field<int>("PointType"),
                        BeforeBalance = row.Field<long>("BeforeBalance"),
                        Amount = row.Field<long>("Amount"),
                        AfterBalance = row.Field<long>("AfterBalance"),
                        IsRollBack = row.Field<bool>("IsRollBack"),
                        CreateDate = row.Field<DateTime>("CreateDate"),
                    })
                    .ToList();
            }
        }
    }
}
