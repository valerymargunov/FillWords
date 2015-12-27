using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using FillWords.Phone._8._0.Resources;
using FillWords.Phone._8._0.LogicGame;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using FillWords.Phone._8._0.Repositories;
using System.IO.IsolatedStorage;
using Microsoft.Phone.Tasks;
using Windows.ApplicationModel.Store;
using FillWords.Phone._8._0.Helpers;

namespace FillWords.Phone._8._0
{
    public partial class CategoryPage : PhoneApplicationPage
    {
        CategoryRepository categoryRepository { get; set; }
        IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;

        public CategoryPage()
        {
            InitializeComponent();
            categoryRepository = new CategoryRepository();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            var categories = categoryRepository.GetCategories();
            GenerateCategories(categories, rootPanel);
            PopulateTopBarValues();
        }

        private void GenerateCategories(IList<Category> categories, StackPanel RootStackPanel)
        {
            try
            {
                int i = 0;
                int lastLevelCompleted = (int)settings["lastLevelCompleted"];
                int countLevels = (int)settings["countLevels"];
                StackPanel tempStackPanel = null;
                RootStackPanel.Children.Clear();
                foreach (var cat in categories)
                {
                    var categoryImage = cat.ImageSource;
                    bool isCatCompleted = false;
                    if (i + 1 != categories.Count && categories[i + 1].StartLevelId <= lastLevelCompleted + 1 || countLevels == lastLevelCompleted + 1)
                    {
                        isCatCompleted = true;
                    }
                    i++;
                    var image = new Image
                    {
                        Source = new BitmapImage(categoryImage),
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Stretch,
                        Margin = new Thickness(5),
                        Width = 200,
                        Height = 200,
                        Stretch = Stretch.Fill,
                        Name = i.ToString()
                    };
                    var title = new TextBlock
                    {
                        VerticalAlignment = System.Windows.VerticalAlignment.Center,
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                        TextAlignment = System.Windows.TextAlignment.Center,
                        Foreground = new SolidColorBrush(Colors.White),
                        FontSize = 55,
                        FontWeight = FontWeights.Bold,
                        FontFamily = new FontFamily("Comic Sans MS"),
                        Text = cat.Title
                    };
                    var catPanel = new Grid
                    {
                        Margin = new Thickness(0, 0, 0, 0),
                        Tag = cat,
                        Opacity = lastLevelCompleted + 1 >= cat.StartLevelId ? 1 : 0.5
                    };
                    catPanel.Tap += SeattleTap;
                    catPanel.Children.Add(image);
                    catPanel.Children.Add(title);
                    //isCatCompleted = true;
                    if (isCatCompleted)
                    {
                        var trophy = new Image
                        {
                            //Source = new BitmapImage(new Uri("/Images/Trophy-100-yellow.png", UriKind.Relative)),
                            //Source = new BitmapImage(new Uri("/Images/Olympic Medal-100-yellow.png", UriKind.Relative)),
                            Source = new BitmapImage(new Uri("/Images/Medal-100-yellow.png", UriKind.Relative)),
                            Width = 55,
                            Height = 55
                        };
                        var trophyGrid = new Grid
                        {
                            HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                            VerticalAlignment = System.Windows.VerticalAlignment.Bottom,
                            Margin = new Thickness(10)
                        };
                        catPanel.Children.Add(trophyGrid);
                        trophyGrid.Children.Add(trophy);
                    }
                    
                    if (i == 1)
                    {
                        var stackPanel = new StackPanel
                        {
                            Orientation = System.Windows.Controls.Orientation.Horizontal,
                            Margin = new Thickness(0, 0, 0, 0)
                        };
                        stackPanel.Children.Add(catPanel);
                        RootStackPanel.Children.Add(stackPanel);
                        tempStackPanel = stackPanel;
                    }
                    else if (i % 2 != 0)
                    {
                        tempStackPanel.Children.Add(catPanel);
                    }
                    if (i % 2 == 0)
                    {
                        tempStackPanel.Children.Add(catPanel);
                        var stackPanel = new StackPanel
                        {
                            Orientation = System.Windows.Controls.Orientation.Horizontal,
                            Margin = new Thickness(0, 0, 0, 0)
                        };
                        RootStackPanel.Children.Add(stackPanel);
                        tempStackPanel = stackPanel;
                    }
                }
                categories = null;
            }
            catch(Exception ex) 
            {

            }
        }

        private void SeattleTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var grid = sender as Grid;
            var cat = grid.Tag as Category;
            int lastLevelCompleted = (int)settings["lastLevelCompleted"];
            if (lastLevelCompleted + 1 >= cat.StartLevelId)
            {
                NavigationService.Navigate(new Uri(string.Format("/GameViewPage.xaml?LevelId={0}", cat.StartLevelId), UriKind.RelativeOrAbsolute));
            }
            else
            {
                var popup = new PopupMessage();
                popup.Show(AppResources.CategoryIsNotAvailable);
            }
        }

        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Terminate();
        }

        private void RateReminder_TryReminderCompleted(object sender, AppPromo.RateReminderResult e)
        {
            if (e.Runs == RateReminder.RunsBeforeReminder && !e.RatingShown)
            {
                RateReminder.RunsBeforeReminder *= 2;
                RateReminder.ResetCounters();
            }
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
            NavigationService.Navigate(new Uri("/PremiumPage.xaml", UriKind.Relative));
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