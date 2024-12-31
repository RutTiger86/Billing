﻿using Billing.Core.Enums;
using Billing.Core.Exceptions;
using Billing.Core.Interfaces;
using Google.Apis.AndroidPublisher.v3;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Microsoft.Extensions.Logging;

namespace Billing.Core.Services
{
    public class GoogleValidationService : IValidationService
    {
        private readonly ILogger<GoogleValidationService> logger;
        private readonly AndroidPublisherService service;

        private const string packageName = "com.ruttiger.testapp";
        private const string AppName = "BillTestApp";

        public GoogleValidationService(ILogger<GoogleValidationService> logger)
        {
            this.logger = logger;

            string serviceAccountJsonPath = Path.Combine(AppContext.BaseDirectory, "account", "billServiceAccount.json");

            GoogleCredential credential = GoogleCredential.FromFile(serviceAccountJsonPath)
                .CreateScoped("https://www.googleapis.com/auth/androidpublisher");

            service = new AndroidPublisherService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = AppName,
            });
        }

        public  async Task<bool> ValidatePurchaseAsync(string productId, string purchaseToken)
        {
            try
            {
                var request = service.Purchases.Products.Get(packageName, productId, purchaseToken);
                var response = await request.ExecuteAsync();

                if (response != null)
                {
                    // 구매 상태 확인
                    // 0 : 구매완료 
                    // 1 : 구매취소 
                    // 2 : 구매 보류 
                    if (response.PurchaseState == 0)
                    {
                        logger.LogInformation($"Purchase verified: {response.OrderId}");
                        return true;
                    }
                    else
                    {
                        logger.LogWarning($"Purchase is not valid: {response.OrderId}");
                        return false;
                    }
                }
                else
                {
                    logger.LogError($"Purchase valid response is Null  ProductId: {productId}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new BillingException(BillingError.PURCHASE_GOOGLE_VALIDATE_ERROR, $"Error verifying purchase: {ex.Message}");
            }
        }
    }
}