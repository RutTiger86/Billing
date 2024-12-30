using Billing.Core.Interfaces;
using Google.Apis.AndroidPublisher.v3;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billing.Core.Services
{
    public class GoogleBillingService : IBillingService
    {
        private readonly IDataService dataService;
        private readonly ILogger<GoogleBillingService> logger;
        private readonly AndroidPublisherService service;

        private const string packageName = "com.ruttiger.testapp";
        private const string AppName = "BillTestApp";

        public GoogleBillingService(IDataService dataService, ILogger<GoogleBillingService> logger)
        {
            this.dataService = dataService;
            this.logger = logger;

            // 프로세스 실행 경로에 account 폴더 경로를 붙임
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

                // 구매 상태 확인
                // 0 : 구매완료 
                // 1 : 구매취소 
                // 2 : 구매 보류 
                if (response.PurchaseState == 0)
                {
                    Console.WriteLine($"Purchase verified: {response.OrderId}");
                    return true;
                }
                else
                {
                    Console.WriteLine("Purchase is not valid.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error verifying purchase: {ex.Message}");
                return false;
            }
        }
    }
}
