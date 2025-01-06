﻿using Billing.Core.Enums;
using Billing.Core.Extensions;
using Billing.Core.Interfaces;
using Billing.Core.Models;
using Billing.Core.Models.DataBase;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Runtime.InteropServices;

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
            AddTable<Ledger>("Id");

            ProductDataSet();
        }

        private void ProductDataSet()
        {
            DataTable itemTable = memoryDataSet.Tables[typeof(Item).Name];

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


            newRow = productTable.NewRow();
            newRow["Id"] = 3;
            newRow["ProductKey"] = "ruttiger.billing.item3";
            newRow["ProductName"] = "Diamond Package 3";
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

            newRow = productItemTable.NewRow();
            newRow["Id"] = 4;
            newRow["ProductId"] = 3;
            newRow["Types"] = ProductTypes.POINT;
            newRow["ItemId"] = 3;
            newRow["ItemVolume"] = 50000;
            newRow["IsUse"] = true;
            newRow["CreateDate"] = DateTime.Now;
            newRow["UpdateDate"] = DateTime.Now;

            productItemTable.Rows.Add(newRow);

        }

        private void AddTable<T>(string idName) where T : class
        {
            var tableName = typeof(T).Name;

            DataTable table = new(tableName);

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


        public BillDetail SelectBillDetail(long billDetailId)
        {
            throw new NotImplementedException();
        }

        public List<BillDetail> SelectBillDetails(long billTxId)
        {
            throw new NotImplementedException();
        }

        public List<Ledger> SelectLedger(long accountId)
        {
            DataTable ledgerTable = memoryDataSet.Tables[typeof(Ledger).Name];
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
                })
                .ToList();
        }


        public BillTx SelectBillTx(long billTxId, bool isDeleted = false)
        {
            DataTable billTxTable = memoryDataSet.Tables[typeof(BillTx).Name];
            return billTxTable.AsEnumerable().Where(p => p.Field<bool>("IsDeleted") == isDeleted)
                .FirstOrDefault(p => p.Field<long>("Id") == billTxId)?.ToClass<BillTx>();
        }

        public SubscriptionInfo SelectSubscriptionInfo(long billTxId)
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

        public long InsertLedger(Ledger ledger)
        {
            lock (lockObj)
            {
                DateTime now = DateTime.UtcNow;
                DataTable ledgerTable = memoryDataSet.Tables[typeof(Ledger).Name];

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

        public bool UpdateBillTxStatus(long billTxId, BillTxStatus status)
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

                if (billTxRow == null)
                {
                    return false;
                }

                billTxRow["Status"] = status;
                billTxRow["UpdateDate"] = DateTime.Now;

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


        public bool CompleteBillTx(long billTxId)
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
                    billTxRow["Status"] = BillTxStatus.COMPLETED;
                    billTxRow["UpdateDate"] = DateTime.Now;
                    billTxRow["IsCompleted"] = true;
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
        public bool UpdateBillTxToken(long billTxId, string PurchaseToken)
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

        public Product SelectProduct(string productKey, bool isUse = true)
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

        public ProductInfo SelectProductInfoByProductKey(string productKey)
        {
            var productTable = memoryDataSet.Tables[nameof(Product)];
            var productItemTable = memoryDataSet.Tables[nameof(ProductItem)];
            var itemTable = memoryDataSet.Tables[nameof(Item)];

            // Query the dataset
            var query = from product in productTable.AsEnumerable()
                        where product.Field<string>("ProductKey") == productKey
                        join productItem in productItemTable.AsEnumerable()
                        on product.Field<long>("Id") equals productItem.Field<long>("ProductId")
                        join item in itemTable.AsEnumerable()
                        on productItem.Field<long>("ItemId") equals item.Field<long>("Id")
                        group productItem by new
                        {
                            ProductId = product.Field<long>("Id"),
                            ProductKey = product.Field<string>("ProductKey"),
                            ProductName = product.Field<string>("ProductName"),
                            IsUse = product.Field<bool>("IsUse"),
                            ItemName = item.Field<string>("ItemName"),
                            PointType = item.Field<int>("PointType")

                        } into productGroup
                        select new ProductInfo
                        {
                            Id = productGroup.Key.ProductId,
                            ProductKey = productGroup.Key.ProductKey,
                            ProductName = productGroup.Key.ProductName,
                            IsUse = productGroup.Key.IsUse,
                            Items = productGroup.Select(pg => new ProductItemInfo
                            {
                                Id = pg.Field<long>("Id"),
                                ProductId = pg.Field<long>("ProductId"),
                                Types = (ProductTypes)pg.Field<int>("Types"),
                                ItemId = pg.Field<long>("ItemId"),
                                ItemVolume = pg.Field<int>("ItemVolume"),
                                IsUse = pg.Field<bool>("IsUse"),
                                ItemName = pg.Field<string>("ItemName"),
                                PointType = (PointType)pg.Field<int>("PointType")
                            }).ToList()
                        };

            return query.FirstOrDefault();
        }

        public bool ChargeLedger(long accountId, PointType pointType, long amount)
        {
            var ledgerTable = memoryDataSet.Tables[nameof(Ledger)];

            var dataRow = ledgerTable.AsEnumerable()
                .Where(p => p.Field<long>("AccountId") == accountId && p.Field<int>("PointType") == (int)pointType)
                .FirstOrDefault();

            if (dataRow == null)
            {
                Ledger ledger = new()
                {
                    AccountId = accountId,
                    Balance = 0,
                    Type = pointType,
                };

                InsertLedger(ledger);

                dataRow = ledgerTable.AsEnumerable()
                .Where(p => p.Field<long>("AccountId") == accountId && p.Field<int>("PointType") == (int)pointType)
                .FirstOrDefault();
            }

            dataRow["UpdateDate"] = DateTime.Now;
            dataRow["Balance"] = (long)dataRow["Balance"] + amount;

            return true;
        }
    }
}
