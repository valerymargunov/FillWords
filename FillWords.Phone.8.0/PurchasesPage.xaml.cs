using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;
using Microsoft.Phone.Tasks;
using FillWords.Phone._8._0.Resources;
using Windows.ApplicationModel.Store;
using FillWords.Phone._8._0.Helpers;

namespace FillWords.Phone._8._0
{
    public partial class PurchasesPage : PhoneApplicationPage
    {
        IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;

        public PurchasesPage()
        {
            InitializeComponent();
            PopulateTopBarValues();
        }

        private void panelHints_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var border = sender as Border;
            string productName = string.Empty;
            switch (border.Name)
            {
                case "panelHints20":
                    {
                        StoreHelper.Donate("hints20", (string productId) =>
                        {
                            int countHints = (int)settings["countHints"];
                            settings["countHints"] = countHints + 20;
                            settings.Save();
                            countHintsText.Text = (countHints + 20).ToString();
                        }, ErrorCallback, true);
                    }
                    break;
                case "panelHints40":
                    {
                        StoreHelper.Donate("hints40", (string productId) =>
                        {
                            int countHints = (int)settings["countHints"];
                            settings["countHints"] = countHints + 40;
                            settings.Save();
                            countHintsText.Text = (countHints + 40).ToString();
                        }, ErrorCallback, true);
                    }
                    break;
                case "panelHints80":
                    {
                        StoreHelper.Donate("hints80", (string productId) =>
                        {
                            int countHints = (int)settings["countHints"];
                            settings["countHints"] = countHints + 80;
                            settings.Save();
                            countHintsText.Text = (countHints + 80).ToString();
                        }, ErrorCallback, true);
                    }
                    break;
                case "panelHints150":
                    {
                        StoreHelper.Donate("hints150", (string productId) =>
                        {
                            int countHints = (int)settings["countHints"];
                            settings["countHints"] = countHints + 150;
                            settings.Save();
                            countHintsText.Text = (countHints + 150).ToString();
                        }, ErrorCallback, true);
                    }
                    break;
                case "panelHintsAll":
                    {
                        StoreHelper.Donate("hintsAll", (string productId) =>
                        {
                            int countHints = (int)settings["countHints"];
                            settings["countHints"] = countHints + 900;
                            settings.Save();
                            countHintsText.Text = (countHints + 900).ToString();
                        }, ErrorCallback, true);
                    }
                    break;
                default:
                    {
                        StoreHelper.Donate("hints20", (string productId) =>
                        {
                            int countHints = (int)settings["countHints"];
                            settings["countHints"] = countHints + 20;
                            settings.Save();
                            countHintsText.Text = (countHints + 20).ToString();
                        }, ErrorCallback, true);
                    }
                    break;
            }
            //int countHintsAfterPurchase = (int)settings["countHints"];
            //countHintsText.Text = countHintsAfterPurchase.ToString();
            if (!settings.Contains("showAds"))
            {
                settings.Add("showAds", false);
                settings.Save();
            }
        }

        private void ErrorCallback(Exception ex)
        {
            var popup = new PopupMessage();
            popup.Show(AppResources.PurchaseError + ex.Message);
        }

        #region Top Menu
        private void PopulateTopBarValues()
        {
            #region count completed levels
            int lastLevelCompleted = (int)settings["lastLevelCompleted"];
            int countLevels = (int)settings["countLevels"];
            countLevelsCompleted.Text = string.Format("{0}/{1}", lastLevelCompleted + 1, countLevels);
            #endregion

            #region rating
            int rating = (int)settings["rating"];
            ratingText.Text = rating.ToString();
            #endregion

            #region count of hints
            int countHints = (int)settings["countHints"];
            countHintsText.Text = countHints.ToString();
            #endregion
        }
        #endregion

        #region Bottom Menu Buttons
        private void exit_Click(object sender, RoutedEventArgs e)
        {
            CheckEstimate();
        }

        private void logOut_Click(object sender, RoutedEventArgs e)
        {
            //LogOut();
        }

        private void estimate_Click(object sender, RoutedEventArgs e)
        {
            var marketplaceReviewTask = new MarketplaceReviewTask();
            marketplaceReviewTask.Show();
        }

        private void share_Click(object sender, RoutedEventArgs e)
        {
            var shareLinkTask = new ShareLinkTask();
            shareLinkTask.Title = AppResources.ApplicationTitle;
            shareLinkTask.LinkUri = new Uri(String.Format("http://www.windowsphone.com/s?appid={0}", CurrentApp.AppId.ToString()), UriKind.RelativeOrAbsolute);
            shareLinkTask.Message = AppResources.ShareMessage;
            shareLinkTask.Show();
        }

        private void byu_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/PurchasesPage.xaml", UriKind.Relative));
        }

        private void removeAds_Click(object sender, RoutedEventArgs e)
        {
#if FREE8
            StoreHelper.Donate("RemoveAds", (string productId) =>
            {
                if (!settings.Contains("productRemoveAds"))
                {
                    settings.Add("productRemoveAds", true);
                    settings.Save();
                }
            }, null);
#endif
        }

        private void allGames_Click(object sender, RoutedEventArgs e)
        {
            var MmrketplaceSearchTask = new MarketplaceSearchTask();
            MmrketplaceSearchTask.ContentType = MarketplaceContentType.Applications;
            MmrketplaceSearchTask.SearchTerms = "valery margunov";
            MmrketplaceSearchTask.Show();
        }

        private void CheckEstimate()
        {
            try
            {
                const string isEstimated = "isEstimated";
                if (!settings.Contains(isEstimated))
                {
                    var popup = new PopupMessage();
                    popup.OnOkClick += delegate
                    {
                        settings.Add(isEstimated, true);
                        settings.Save();
                        var marketplaceReviewTask = new MarketplaceReviewTask();
                        marketplaceReviewTask.Show();

                    };
                    popup.OnCancelClick += delegate
                    {
                        settings.Add(isEstimated, false);
                        settings.Save();
                    };
                    popup.OnClick += delegate
                    {
                        Exit();
                    };
                    popup.Show(AppResources.EstimateMessage, true);
                }
                else
                {
                    Exit();
                }
            }
            catch (Exception ex)
            {
                //var popup = new PopupMessage();
                //popup.Show(String.Format(AppResources.Exception, ex.Message));
            }
        }

        private void Exit()
        {
            Application.Current.Terminate();
        }
        #endregion

    }
}