﻿using Billing.Core.Enums;
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
    public class GoogleValidationService : IValidationService
    {
        private readonly ILogger<GoogleValidationService> logger;
        private readonly AndroidPublisherService service;
        private readonly IDataService dataService;

        private const string packageName = "com.ruttiger.testapp";
        private const string AppName = "BillTestApp";

        public GoogleValidationService(IDataService dataService, ILogger<GoogleValidationService> logger)
        {
            this.logger = logger;
            this.dataService = dataService;

            string serviceAccountJsonPath = Path.Combine(AppContext.BaseDirectory, "account", "billServiceAccount.json");

            GoogleCredential credential = GoogleCredential.FromFile(serviceAccountJsonPath)
                .CreateScoped("https://www.googleapis.com/auth/androidpublisher");

            service = new AndroidPublisherService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = AppName,
            });
        }

        public async Task<bool> PurchaseProductValidate(long billDetailId, PurchaseInfo purchaseInfo)
        {
            try
            {
                var request = service.Purchases.Products.Get(packageName, purchaseInfo.ProductKey, purchaseInfo.PurchaseToken);

                var responseTask = request.ExecuteAsync();

                dataService.UpdateBillTxStatus(purchaseInfo.BillTxId, BillTxStatus.IAP_RECEIPT_PENDING);

                var response = await responseTask;

                if (response != null)
                {
                    // 구매 상태 확인
                    // 0 : 구매완료 
                    // 1 : 구매취소 
                    // 2 : 구매 보류 
                    if (response.PurchaseState == 0)
                    {
                        //  구매 확인을 Client에서 진행할 수 있음 (아이템 지급후) 
                        //if (response.AcknowledgementState == 0)
                        //{
                        //    logger.LogInformation("Acknowledgement required. Processing...");
                        //    await ProductAcknowledge(purchaseInfo);
                        //}
                        //else
                        //{
                        //    logger.LogInformation("Purchase already acknowledged.");
                        //}

                        dataService.UpdateBillTxStatus(purchaseInfo.BillTxId, BillTxStatus.IAP_RECEIPT_VALID);
                        logger.LogInformation($"Purchase verified: {response.OrderId}");
                        return true;
                    }
                    else
                    {
                        dataService.UpdateBillTxStatus(purchaseInfo.BillTxId, BillTxStatus.IAP_RECEIPT_INVALID);
                        logger.LogWarning($"Purchase is not valid: {response.OrderId}");
                        return false;
                    }
                }
                else
                {
                    dataService.UpdateBillTxStatus(purchaseInfo.BillTxId, BillTxStatus.IAP_RECEIPT_INVALID);
                    logger.LogError($"Purchase valid response is Null  ProductId: {purchaseInfo.ProductKey}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new BillingException(BillingError.PURCHASE_GOOGLE_VALIDATE_ERROR, $"Error verifying purchase: {ex.Message}");
            }
        }

        public async Task<bool> PruchaseSubscriptionsValidate(long billDetailId, PurchaseInfo purchaseInfo)
        {
            try
            {
                var request = service.Purchases.Subscriptions.Get(packageName, purchaseInfo.ProductKey, purchaseInfo.PurchaseToken);

                var responseTask = request.ExecuteAsync();

                dataService.UpdateBillTxStatus(purchaseInfo.BillTxId, BillTxStatus.IAP_RECEIPT_PENDING);

                var response = await responseTask;

                if (response != null && response.PaymentState == 0)
                {
                    long currentTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    if (response.ExpiryTimeMillis < currentTime)
                    {
                        dataService.UpdateBillTxStatus(purchaseInfo.BillTxId, BillTxStatus.IAP_RECEIPT_VALID);
                        logger.LogInformation("Subscription has expired.");
                        return false;
                    }

                    //  구매 확인을 Client에서 진행할 수 있음 
                    //if (response.AcknowledgementState == 0)
                    //{
                    //    logger.LogInformation("Acknowledgement required. Processing...");
                    //    await SubscriptionsAcknowledge(purchaseInfo);
                    //}
                    //else
                    //{
                    //    logger.LogInformation("Subscription already acknowledged.");
                    //}

                    CreateSubScription(billDetailId, purchaseInfo, (long)response.ExpiryTimeMillis);

                    dataService.UpdateBillTxStatus(purchaseInfo.BillTxId, BillTxStatus.IAP_RECEIPT_VALID);
                    logger.LogInformation($"Purchase verified: {response.OrderId}");
                    return true;
                }
                else
                {
                    dataService.UpdateBillTxStatus(purchaseInfo.BillTxId, BillTxStatus.IAP_RECEIPT_INVALID);
                    logger.LogError($"Purchase valid response is Null  ProductId: {purchaseInfo.ProductKey}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new BillingException(BillingError.PURCHASE_GOOGLE_VALIDATE_ERROR, $"Error verifying purchase: {ex.Message}");
            }
        }

        private long CreateSubScription(long billDetailId, PurchaseInfo purchaseInfo, long expiryTimeMillis)
        {
            var product = dataService.SelectProduct(purchaseInfo.ProductKey);

            if (product == null)
            {
                return -1;
            }

            SubscriptionInfo subscription = new()
            {
                AccountId = purchaseInfo.AccountId,
                BillDetailId = billDetailId,
                BillTxId = purchaseInfo.BillTxId,
                ExpiryTimeMillis = expiryTimeMillis,
                IsExpired = false,
                State = SubScriptionState.SUBSCRIPTION_STATE_ACTIVE             
            };

            return dataService.InsertSubscriptionInfo(subscription);
        }


        private async Task ProductAcknowledge(PurchaseInfo purchaseInfo)
        {
            logger.LogInformation("Acknowledgement required. Processing...");
            var acknowledgeRequest = new ProductPurchasesAcknowledgeRequest
            {
                DeveloperPayload = purchaseInfo.BillTxId.ToString(),
            };

            var acknowledge = service.Purchases.Products.Acknowledge(acknowledgeRequest, packageName, purchaseInfo.ProductKey, purchaseInfo.PurchaseToken);
            await acknowledge.ExecuteAsync();
        }


        private async Task SubscriptionsAcknowledge(PurchaseInfo purchaseInfo)
        {
            logger.LogInformation("Acknowledgement required. Processing...");
            var acknowledgeRequest = new SubscriptionPurchasesAcknowledgeRequest
            {
                DeveloperPayload = purchaseInfo.BillTxId.ToString(),
            };

            var acknowledge = service.Purchases.Subscriptions.Acknowledge(acknowledgeRequest, packageName, purchaseInfo.ProductKey, purchaseInfo.PurchaseToken);
            await acknowledge.ExecuteAsync();
        }

        
        
        public async Task<SubScriptionState> SubscriptionsValidate(string purchaseToken)
        {
            try
            {
                var request = service.Purchases.Subscriptionsv2.Get(packageName,purchaseToken);

                var response = await request.ExecuteAsync();

                if (response != null && Enum.TryParse(response.SubscriptionState,out SubScriptionState state))
                {
                    return state;
                }
                else
                {
                    return SubScriptionState.SUBSCRIPTION_STATE_UNSPECIFIED;
                }
            }
            catch (Exception ex)
            {
                throw new BillingException(BillingError.PURCHASE_GOOGLE_VALIDATE_ERROR, $"Error verifying purchase: {ex.Message}");
            }
        }
    }
}
