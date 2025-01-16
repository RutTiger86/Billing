using Billing.Core.Exceptions;
using Billing.Core.Interfaces;
using Billing.Core.Models.DataBase;
using Billing.Protobuf.Core;
using Billing.Protobuf.Point;
using Billing.Protobuf.Product;
using Billing.Protobuf.Purchase;
using Microsoft.Extensions.Logging;

namespace Billing.Core.Services
{
    public class PointValidationService(IDataService dataService, ILogger<PointValidationService> logger) : IValidationService
    {
        private readonly ILogger<PointValidationService> logger = logger;
        private readonly IDataService dataService = dataService;


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public Task<bool> PruchaseSubscriptionsValidate(long billDetailId, PurchaseInfo purchaseInfo)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public async Task<bool> PurchaseProductValidate(long billDetailId, PurchaseInfo purchaseInfo)
        {
            try
            {

                dataService.UpdateBillTxStatus(purchaseInfo.BillTxId, BillTxStatus.PointSpendStart);

                List<Ledger> ledgers =  dataService.SelectLedger(purchaseInfo.AccountId);

                if(purchaseInfo.PointPurchases == null || purchaseInfo.PointPurchases.Count ==0)
                {
                    return false;
                }

                foreach (PointPurchase pointPurchase in purchaseInfo.PointPurchases)
                {
                    var pointLedger = ledgers.Where(p => p.Type == pointPurchase.PointType).FirstOrDefault();
                    if (pointLedger == null || pointLedger.Balance < pointPurchase.Amount)
                    {
                        return false;
                    }
                }

                var product = dataService.SelectProductInfoByProductKey(purchaseInfo.ProductKey);

                bool isError = false;

                foreach (PointPurchase pointPurchase in purchaseInfo.PointPurchases)
                {
                    var pointLedger = ledgers.Where(p => p.Type == pointPurchase.PointType).First(); 

                    PointHistory pointHistory = new()
                    {
                        AccountId = purchaseInfo.AccountId,
                        BillTxId = purchaseInfo.BillTxId,
                        PointOperationType = PointOperationType.Withdraw,
                        ProductId = product.Id,
                        PointType = pointPurchase.PointType,
                        BeforeBalance = pointLedger.Balance,
                        Amount = pointPurchase.Amount,
                    };

                    dataService.WithdrawLedger(purchaseInfo.AccountId, pointPurchase.PointType, pointPurchase.Amount);

                    var afterPointledger = dataService.SelectLedgerByPointType(purchaseInfo.AccountId, pointPurchase.PointType);

                    pointHistory.AfterBalance = afterPointledger.Balance;

                    dataService.InsertPointHistory(pointHistory);

                    if(afterPointledger.Balance < 0)
                    {
                        isError = true; 
                        logger.LogError($"Point Type {pointPurchase.PointType} Withdraw Error, Balance is minus");
                        break;
                    }
                }

                if(isError)
                {
                    PointRollBack(purchaseInfo.BillTxId);

                    dataService.UpdateBillTxStatus(purchaseInfo.BillTxId, BillTxStatus.PointSpendFailed);
                    return false;
                }

                dataService.UpdateBillTxStatus(purchaseInfo.BillTxId, BillTxStatus.PointSpendEnd);
                return true;
            }
            catch (Exception ex)
            {
                throw new BillingException(BillingError.PurchasePointValidateError, $"Error verifying purchase: {ex.Message}");
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public Task<SubScriptionState> SubscriptionsValidate(string purchaseToken)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 포인트 충전 프로세스 
        /// </summary>
        public bool ChargePointProcess(List<ProductItemInfo> products, PurchaseInfo purchaseInfo)
        {
            try
            {
                foreach (ProductItemInfo productItem in products)
                {
                    var pointLedgers = dataService.SelectLedgerByPointType(purchaseInfo.AccountId, productItem.PointType);
                    if (pointLedgers == null)
                    {
                        CreatePointLedger(purchaseInfo, productItem);
                        pointLedgers = dataService.SelectLedgerByPointType(purchaseInfo.AccountId, productItem.PointType);
                    }

                    PointHistory pointHistory = new()
                    {
                        AccountId = purchaseInfo.AccountId,
                        BillTxId = purchaseInfo.BillTxId,
                        PointOperationType = PointOperationType.Charge,
                        ProductId = productItem.ProductId,
                        PointType = productItem.PointType,
                        BeforeBalance = pointLedgers.Balance,
                        Amount = productItem.ItemVolume,
                    };

                    dataService.ChargeLedger(purchaseInfo.AccountId, productItem.PointType, productItem.ItemVolume);

                    pointLedgers = dataService.SelectLedgerByPointType(purchaseInfo.AccountId, productItem.PointType);
                    pointHistory.AfterBalance = pointLedgers.Balance;
                    dataService.InsertPointHistory(pointHistory);
                }

                dataService.UpdateBillTxStatus(purchaseInfo.BillTxId, BillTxStatus.PointChargeEnd);
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError($"ChargePointProcess Error - TxId : {purchaseInfo.BillTxId} , Message : {ex.Message}");
                dataService.UpdateBillTxStatus(purchaseInfo.BillTxId, BillTxStatus.PointChargeFailed);
                return false;
            }
        }

        /// <summary>
        /// 포인트 롤백 프로세스 
        /// </summary>
        public void PointRollBack(long billTxId)
        {
            var pointHistories = dataService.SelectPointHistories(billTxId);

            foreach (PointHistory pointHistory in pointHistories)
            {
                if (!pointHistory.IsRollBack)
                {
                    if (pointHistory.PointOperationType == PointOperationType.Charge)
                    {
                        if (dataService.ChargeRollBackLedger(pointHistory.Id, pointHistory.AccountId, pointHistory.PointType, pointHistory.Amount) <1)
                        { 
                            logger.LogError($"PointRollBack pointHistory ID {pointHistory.Id} is WithdrawRollBack Failed");
                        }
                    }
                    else
                    {
                        if (dataService.ChargeRollBackLedger(pointHistory.Id, pointHistory.AccountId, pointHistory.PointType, pointHistory.Amount)<1)
                        { 
                            logger.LogError($"PointRollBack pointHistory ID {pointHistory.Id} is ChargeRollBack Failed");
                        }
                    }
                }
                else
                {
                    logger.LogInformation($"PointRollBack pointHistory ID {pointHistory.Id} is Already Rollback");
                }

            }

        }

        /// <summary>
        /// 포인트 지갑 정보 생성 
        /// </summary>
        private bool CreatePointLedger(PurchaseInfo purchaseInfo, ProductItemInfo productItem)
        {
            Ledger ledger = new()
            {
                AccountId = purchaseInfo.AccountId,
                Balance = 0,
                Type = productItem.PointType,
            };
            dataService.InsertLedger(ledger);

            PointHistory pointHistory = new()
            {
                AccountId = purchaseInfo.AccountId,
                BillTxId = purchaseInfo.BillTxId,
                PointOperationType = PointOperationType.Create,
                ProductId = productItem.ProductId,
                PointType = productItem.PointType,
                AfterBalance = 0,
                BeforeBalance = 0,
                Amount = 0,
            };

            dataService.InsertPointHistory(pointHistory);

            return true;
        }

    }
}
