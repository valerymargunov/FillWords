using FillWords.Phone._8._0.Helpers;
using GoogleAds;
using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace FillWords.Phone._8._0.Advertising
{
    public class AdsMob
    {
        IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
        private InterstitialAd interstitialAd;
        bool IsForceTesting = false;

        public void AddAds(Grid grid, string adUnitID)
        {
            try
            {
                const string product = "productRemoveAds";
                const string productHint = "showAds";
                if (!settings.Contains(product) && !settings.Contains(productHint))
                {
                    AdView bannerAd = new AdView
                    {
                        Format = AdFormats.Banner,
                        AdUnitID = adUnitID
                    };
                    AdRequest adRequest = new AdRequest();
                    adRequest.ForceTesting = IsForceTesting;
                    if (grid != null)
                    {
                        grid.Children.Add(bannerAd);
                        bannerAd.LoadAd(adRequest);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        public void AddInterstitialAd(string adUnitID)
        {
            try
            {
                const string product = "productRemoveAds";
                const string productHint = "showAds";
                if (!settings.Contains(product) && !settings.Contains(productHint))
                {
                    interstitialAd = new InterstitialAd(adUnitID);
                    AdRequest adRequest = new AdRequest();
                    adRequest.ForceTesting = IsForceTesting;
                    interstitialAd.DismissingOverlay += OnDismissingOverlay;
                    interstitialAd.ReceivedAd += OnAdReceived;
                    interstitialAd.LoadAd(adRequest);
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void OnDismissingOverlay(object sender, AdEventArgs e)
        {
            //CheckIsPaid();
        }

        private void OnAdReceived(object sender, AdEventArgs e)
        {
            try
            {
                interstitialAd.ShowAd();
            }
            catch (Exception ex)
            {

            }
        }

        private void CheckIsPaid()
        {
            try
            {
                const string isPaid = "isPaid";
                if (!settings.Contains(isPaid))
                {
                    StoreHelper.Donate("RemoveAds", (string productId) =>
                    {
                        if (!settings.Contains("productRemoveAds"))
                        {
                            settings.Add("productRemoveAds", true);
                            settings.Save();
                        }
                    }, null);
                    settings[isPaid] = true;
                    settings.Save();
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
