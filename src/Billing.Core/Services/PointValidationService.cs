using Billing.Core.Enums;
using Billing.Core.Exceptions;
using Billing.Core.Interfaces;
using Billing.Core.Models;
using Billing.Core.Models.DataBase;
using Google.Apis.AndroidPublisher.v3;
using Google.Apis.AndroidPublisher.v3.Data;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Requests;
using Google.Apis.Services;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using static Google.Apis.Requests.BatchRequest;

namespace Billing.Core.Services
{
    public class PointValidationService(IDataService dataService, ILogger<PointValidationService> logger) : IValidationService
    {
        private readonly ILogger<PointValidationService> logger = logger;
        private readonly IDataService dataService = dataService;

        public Task<bool> PruchaseSubscriptionsValidate(long billDetailId, PurchaseInfo purchaseInfo)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> PurchaseProductValidate(long billDetailId, PurchaseInfo purchaseInfo)
        {
            try
            {
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
                        PointOperationType = PointOperationType.WITHDRAW,
                        ProductId = product.Id,
                        PointType = pointPurchase.PointType,
                        BeforeBalance = pointLedger.Balance,
                        Amount = pointPurchase.Amount,
                    };

                    dataService.Withdrawledger(purchaseInfo.AccountId, pointPurchase.PointType, pointPurchase.Amount);

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
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new BillingException(BillingError.PURCHASE_GOOGLE_VALIDATE_ERROR, $"Error verifying purchase: {ex.Message}");
            }
        }

        public Task<SubScriptionState> SubscriptionsValidate(string purchaseToken)
        {
            throw new NotImplementedException();
        }

        public void PointRollBack(long billTxId)
        {
            var pointHistories =  dataService.SelectPointHistories(billTxId);

            foreach (PointHistory pointHistory in pointHistories)
            {
                if(!pointHistory.IsRollBack)
                {
                    if (pointHistory.PointOperationType == PointOperationType.CHARGE)
                    {
                        if (dataService.Withdrawledger(pointHistory.AccountId, pointHistory.PointType, pointHistory.Amount))
                        {
                            dataService.UpdatePointHistoryIsRollBack(pointHistory.Id);
                        }
                        else
                        {
                            logger.LogError($"PointRollBack pointHistory ID {pointHistory.Id} is WithdrawRollBack Failed");
                        }
                    }
                    else
                    {
                        if (dataService.ChargeLedger(pointHistory.AccountId, pointHistory.PointType, pointHistory.Amount))
                        {
                            dataService.UpdatePointHistoryIsRollBack(pointHistory.Id);
                        }
                        else
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
    }
}
