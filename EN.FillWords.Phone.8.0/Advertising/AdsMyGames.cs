using FillWords.Phone._8._0.Helpers;
using FillWords.Phone._8._0.Resources;
using Microsoft.Phone.Tasks;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FillWords.Phone._8._0.Advertising
{
    public class AdsMyGames
    {
        IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
        List<AdsGame> AdsGames = new List<AdsGame>
        {
            new AdsGame
            {
                AppId = "0f892535-518b-4199-902c-711130e7c7bd",
                Icon = new Uri("/Images/games/mt-300.png", UriKind.RelativeOrAbsolute),
                Name = "Memory Training",
                TitleMessage = AppResources.AdsTitleMessage,
                Game = Game.ТренировкаПамяти,
                SupportLanguages = SupportLanguages.All
            },
            new AdsGame
            {
                AppId = "3533b1c7-ab70-41c2-b1ec-828e77e73d0d",
                Icon = new Uri("/Images/games/pm300.png", UriKind.RelativeOrAbsolute),
                Name = "Puzzles Mashi",
                TitleMessage = AppResources.AdsTitleMessage,
                Game = Game.ПазлыМаши,
                SupportLanguages = SupportLanguages.All
            },
            new AdsGame
            {
                AppId = "0363c883-6428-4bd6-b599-f11796c69cc0",
                Icon = new Uri("/Images/games/b_300.png", UriKind.RelativeOrAbsolute),
                Name = "Bottle",
                TitleMessage = AppResources.AdsTitleMessage,
                Game = Game.Бутылочка,
                SupportLanguages = SupportLanguages.All
            },
            new AdsGame
            {
                AppId = "2c1abff6-596d-45e4-8f25-fed9ae780b4b",
                Icon = new Uri("/Images/games/cc-300.png", UriKind.RelativeOrAbsolute),
                Name = "Collect Cat",
                TitleMessage = AppResources.AdsTitleMessage,
                Game = Game.СобериКота,
                SupportLanguages = SupportLanguages.All
            },
            new AdsGame
            {
                AppId = "4d4336f6-29a4-4128-935b-5885d67c79ad",
                Icon = new Uri("/Images/games/tp-300.png", UriKind.RelativeOrAbsolute),
                Name = "Talking Puzzle",
                TitleMessage = AppResources.AdsTitleMessage,
                Game = Game.ГоворящийПазл,
                SupportLanguages = SupportLanguages.All
            },
        };

        public void ShowAd(Action cancelAction)
        {
            var currentCulture = CultureInfo.CurrentCulture.ToString().Substring(0, 2).ToUpper();
            var supportGames = AdsGames.Where(x => x.SupportLanguages == SupportLanguages.All || Enum.GetName(typeof(SupportLanguages), x.SupportLanguages) == currentCulture).ToList();
            if (supportGames.Count > 0)
            {
                var rand = new Random();
                int selectIndex = rand.Next(0, supportGames.Count);
                var game = supportGames[selectIndex];
                string isShowAds = String.Format("isShowAds{0}", game.AppId);
                if (!settings.Contains(isShowAds))
                {
                    settings[isShowAds] = true;
                    settings.Save();
                    var image = new Image()
                    {
                        Source = new BitmapImage(game.Icon),
                        Width = 300,
                        Height = 300
                    };
                    image.Tap += delegate
                    {
                        OpenGameInStore(game.AppId);
                    };
                    var title = new TextBlock
                    {
                        Text = game.TitleMessage,
                        Foreground = new SolidColorBrush(Colors.White),
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Margin = new Thickness(5, 5, 5, 10),
                        TextWrapping = TextWrapping.Wrap
                    };
                    var gameName = new TextBlock
                    {
                        Text = game.Name,
                        Foreground = new SolidColorBrush(Colors.White),
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Margin = new Thickness(5, 5, 5, 20)
                    };
                    var body = new StackPanel
                    {
                        Orientation = System.Windows.Controls.Orientation.Vertical,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Children =
                    {
                        title,
                        image,
                        gameName
                    }
                    };
                    var popup = new PopupMessage
                    {
                        Body = body
                    };
                    popup.OnOkClick += delegate
                    {
                        OpenGameInStore(game.AppId);
                    };
                    popup.OnCancelClick += delegate
                    {
                        cancelAction.Invoke();
                    };
                    popup.Show("", true);
                }
            }
        }

        private void OpenGameInStore(string appId)
        {
            var marketplaceDetailTask = new MarketplaceDetailTask();
            marketplaceDetailTask.ContentIdentifier = appId;
            marketplaceDetailTask.ContentType = MarketplaceContentType.Applications;
            marketplaceDetailTask.Show();
        }
    }

    public class AdsGame
    {
        public string TitleMessage { get; set; }
        public Uri Icon { get; set; }
        public string Name { get; set; }
        public string AppId { get; set; }
        public Game Game { get; set; }
        public SupportLanguages SupportLanguages { get; set; }
    }

    public enum Game
    {
        CompetePeppers,
        Филлворды,
        RiempireParole,
        FillWords,
        ТренировкаПамяти,
        ГоворящийПазл,
        Бутылочка,
        ПазлыМаши,
        СобериКота
    }

    public enum SupportLanguages
    {
        All,
        EN,
        IT,
        RU
    }
}
