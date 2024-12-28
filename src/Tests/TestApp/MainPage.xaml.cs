using Plugin.InAppBilling;
using System.Collections.ObjectModel;

namespace TestApp
{
    public partial class MainPage : ContentPage
    {
        int count = 0;
        public ObservableCollection<InAppBillingProduct> Items { get; } = new();
        public MainPage()
        {
            InitializeComponent();
        }

        private void OnCounterClicked(object sender, EventArgs e)
        {
            count++;

            if (count == 1)
                CounterBtn.Text = $"Clicked {count} time";
            else
                CounterBtn.Text = $"Clicked {count} times";

            SemanticScreenReader.Announce(CounterBtn.Text);
        }

        private async void ButtonConsumable_Clicked(object sender, EventArgs e)
        {
            var productId = "ruttiger.billing.item1";
            var billing = CrossInAppBilling.Current;
            try
            {
                var connected = await billing.ConnectAsync();
                if (!connected)
                {
                    //we are offline or can't connect, don't try to purchase
                    return ;
                }

                //check purchases
                var purchase = await billing.PurchaseAsync(productId, ItemType.InAppPurchaseConsumable);

                //possibility that a null came through.
                if (purchase == null)
                {
                    await DisplayAlert(string.Empty, "purchase fail", "OK");
                    //did not purchase
                }
                else if (purchase.State == PurchaseState.Purchased)
                {
                    // purchased, we can now consume the item or do it later
                    // here you may want to call your backend or process something in your app.

                    lbl_TrasactionID.Text = purchase.TransactionIdentifier;
                    //only required on Android & Windows    
                    var wasConsumed = await CrossInAppBilling.Current.ConsumePurchaseAsync(purchase.ProductId, purchase.TransactionIdentifier);

                    if (wasConsumed)
                    {
                        await DisplayAlert(string.Empty, "wasConsumed Success", "OK");
                    }
                    else
                    {
                        await DisplayAlert(string.Empty, "wasConsumed fail", "OK");
                    }
                }
            }
            catch (InAppBillingPurchaseException purchaseEx)
            {
                //Billing Exception handle this based on the type
                Console.WriteLine("Error: " + purchaseEx);
            }
            catch (Exception ex)
            {
                //Something else has gone wrong, log it
                Console.WriteLine("Issue connecting: " + ex);
            }
            finally
            {
                await billing.DisconnectAsync();
            }
        }

        private async void ButtonProductInfo_Clicked(object sender, EventArgs e)
        {
            try
            {
                var id = "ruttiger.billing.item1";
                await CrossInAppBilling.Current.ConnectAsync();
                var items = await CrossInAppBilling.Current.GetProductInfoAsync(ItemType.InAppPurchase, [id]);
                foreach (var item in items)
                    await DisplayAlert(string.Empty, "item :  " + item.ProductId + ", Name : " + item.Name, "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert(string.Empty, "Did not purchase: " + ex.Message, "OK");
                Console.WriteLine(ex);
            }
            finally
            {

                await CrossInAppBilling.Current.DisconnectAsync();
            }
        }

    }

}
