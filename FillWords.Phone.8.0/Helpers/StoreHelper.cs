using FillWords.Phone._8._0.Resources;
using Microsoft.Phone.Net.NetworkInformation;
using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Store;
using Store = Windows.ApplicationModel.Store;

namespace FillWords.Phone._8._0.Helpers
{
    public class StoreHelper
    {
        public static bool IsProductActive(string productId)
        {
            try
            {
                if (NetworkInterface.GetIsNetworkAvailable())
                {
                    return Store.CurrentApp.LicenseInformation.ProductLicenses[productId].IsActive ? true : false;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public static async void Donate(string productId, Action<string> success, Action<Exception> error, bool isConsurable = false)
        {
            try
            {
                var popupMessage = new PopupMessage();
                if (NetworkInterface.GetIsNetworkAvailable())
                {
                    //success.Invoke(productId);//TODO:remove after testing
                    var listing = await CurrentApp.LoadListingInformationAsync();
                    var myProduct = listing.ProductListings.FirstOrDefault(p => p.Value.ProductId == productId);
                    if (isConsurable)
                    {
                        await CurrentApp.RequestProductPurchaseAsync(productId, false);
                        var productLicenses = CurrentApp.LicenseInformation.ProductLicenses;
                        ProductLicense tokenLicense = productLicenses[productId];
                        if (tokenLicense.IsActive)
                        {
                            CurrentApp.ReportProductFulfillment(productId);
                            success.Invoke(productId);
                        }
                    }
                    else if (!isConsurable && !CurrentApp.LicenseInformation.ProductLicenses[myProduct.Value.ProductId].IsActive)
                    {
                        await CurrentApp.RequestProductPurchaseAsync(productId, false);
                        var productLicenses = CurrentApp.LicenseInformation.ProductLicenses;
                        ProductLicense tokenLicense = productLicenses[productId];
                        if (tokenLicense.IsActive)
                        {
                            success.Invoke(productId);
                        }
                    }
                    else
                    {
                        popupMessage.Show(AppResources.ProductExitsts);
                    }
                }
                else
                {
                    popupMessage.Show(AppResources.NoInternet);
                }
            }
            catch (Exception ex)
            {
                error.Invoke(ex);
            }
        }
    }
}
