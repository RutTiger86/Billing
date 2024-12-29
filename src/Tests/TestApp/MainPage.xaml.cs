using Plugin.InAppBilling;
using System.Collections.ObjectModel;
using Microsoft.Maui.ApplicationModel;

namespace TestApp
{
    public partial class MainPage : ContentPage
    {
        public ObservableCollection<InAppBillingProduct> Items { get; } = new();
        public MainPage()
        {
            InitializeComponent();
            lbl_VersionInfo.Text = AppInfo.VersionString;
        }

        private async void ButtonConsumable_Clicked(object sender, EventArgs e)
        {
            var productId = ent_ProductId.Text;
            var billing = CrossInAppBilling.Current;
            try
            {
                var connected = await billing.ConnectAsync();
                if (!connected)
                {
                    lbl_Result.Text = $"CrossInAppBilling.Current Connected Fail";
                    return;
                }

                //check purchases
                var purchase = await billing.PurchaseAsync(productId, ItemType.InAppPurchaseConsumable);

                if (purchase == null)
                {
                    lbl_Result.Text = "Purchase Fail (Is Null)";
                }
                else if (purchase.State == PurchaseState.Purchased)
                {
                    ent_TrasactionID.Text = purchase.TransactionIdentifier;
                    lbl_Result.Text = "Purchase Success";
                }
                else
                {
                    ent_TrasactionID.Text = purchase.TransactionIdentifier;
                    lbl_Result.Text = $"Purchase Issue!!  pid : {purchase.ProductId} , State : {purchase.State}, TxId : {purchase.TransactionIdentifier}  ";

                }
            }
            catch (InAppBillingPurchaseException purchaseEx)
            {
                lbl_Result.Text = $"Error : {purchaseEx}";
            }
            catch (Exception ex)
            {
                lbl_Result.Text = $"Issue Connecting : {ex} ";
            }
            finally
            {
                await billing.DisconnectAsync();
            }
        }

        private async void ButtonConsumed_Clicked(object sender, EventArgs e)
        {
            var productId = ent_ProductId.Text;
            var transactionID = ent_TrasactionID.Text;
            var billing = CrossInAppBilling.Current;
            try
            {
                var connected = await billing.ConnectAsync();
                if (!connected)
                {
                    //we are offline or can't connect, don't try to purchase
                    return;
                }

                //only required on Android & Windows    
                var wasConsumed = await billing.ConsumePurchaseAsync(productId, transactionID);

                if (wasConsumed)
                {

                    lbl_Result.Text = "WasConsumed Success";
                }
                else
                {
                    lbl_Result.Text = "WasConsumed Fail";
                }

            }
            catch (InAppBillingPurchaseException purchaseEx)
            {
                lbl_Result.Text = $"Error : {purchaseEx}";
            }
            catch (Exception ex)
            {
                lbl_Result.Text = $"Issue connecting : {ex}";
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
                var id = ent_ProductId.Text;
                await CrossInAppBilling.Current.ConnectAsync();
                Items.Clear();
                var items = await CrossInAppBilling.Current.GetProductInfoAsync(ItemType.InAppPurchaseConsumable, [id]);
                foreach (var item in items)
                    await DisplayAlert(string.Empty, "item :  " + item.ProductId + ", Name : " + item.Name, "OK");
            }
            catch (Exception ex)
            {
                lbl_Result.Text = $"ProductInfo Exception : {ex.Message}";
            }
            finally
            {

                await CrossInAppBilling.Current.DisconnectAsync();
            }
        }

        private async void ButtonCopyTxID_Clicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ent_TrasactionID.Text))
            {
                await Clipboard.SetTextAsync(ent_TrasactionID.Text);
                await DisplayAlert("Copied", "Transaction ID copied to clipboard.", "OK");
            }
            else
            {
                await DisplayAlert("Error", "Transaction ID is empty.", "OK");
            }
        }

    }

}
