using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media;
using FillWords.Phone._8._0.Repositories;
using FillWords.Phone._8._0.LogicGame;
using System.Windows.Input;
using System.IO.IsolatedStorage;
using Microsoft.Phone.Tasks;
using FillWords.Phone._8._0.Resources;
using Windows.ApplicationModel.Store;
using FillWords.Phone._8._0.Helpers;

namespace FillWords.Phone._8._0
{
    public partial class GameViewPage : PhoneApplicationPage
    {
        GameViewRepository gameViewRepository { get; set; }
        WordRepository wordRepository { get; set; }
        int LevelId = 0;
        bool IsManipulationStarted { get; set; }
        bool IsManipulationCompleted { get; set; }
        Brush GridLetterBrush { get; set; }
        Brush FillGridLetterBrush { get; set; }
        Dimension Dimension { get; set; }
        string TempCurrentWord { get; set; }
        List<string> TempListCurrentLetter { get; set; }
        List<string> ListCurrentWords { get; set; }
        int CountWords { get; set; }
        IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
        int CountLevels { get; set; }
        MediaElement LevelCompleted { get; set; }
        List<Coordinate> ListHints { get; set; }
        int HintsEnumerator { get; set; }

        public GameViewPage()
        {
            InitializeComponent();
            gameViewRepository = new GameViewRepository();
            wordRepository = new WordRepository();
            GridLetterBrush = new SolidColorBrush(Color.FromArgb(255, 194, 11, 138));
            FillGridLetterBrush = new SolidColorBrush(Color.FromArgb(255, 1, 135, 197));
            TempCurrentWord = string.Empty;
            TempCurrentWordColor = new List<Color>();
            TempListCurrentLetter = new List<string>();
            ListCurrentWords = new List<string>();
            CountLevels = (int)settings["countLevels"];
            LevelCompleted = new MediaElement
            {
                Source = new Uri("/Audio/endlevel.wav", UriKind.RelativeOrAbsolute),
                AutoPlay = false
            };
            LayoutRoot.Children.Add(LevelCompleted);
            ListHints = new List<Coordinate>();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            PopulateWords();
            PopulateTopBarValues();
        }

        private void PopulateWords()
        {
            var rand = new Random();
            var gameView = gameViewRepository.GetGameView(LevelId);
            Dimension = gameView.Dimension;
            GenerateLevels(Dimension, rootPanel);
            int currentWord = 0;
            var existsColors = new List<Color>();
            foreach (var item in gameView.WordsLocation)//идем по словам
            {
                int currentLetter = 0;
                Color currentColor;
                var words = item.Matrix.Split(',');
                do
                {
                    currentColor = ColorsHelper.DictColors[rand.Next(0, ColorsHelper.DictColors.Count)];
                }
                while (existsColors.Contains(currentColor));
                existsColors.Add(currentColor);
                var word = wordRepository.GetWords(LevelId)[currentWord];
                ListCurrentWords.Add(word.Content);
                foreach (var letter in words)//идем по слову
                {
                    var cells = letter.Split(':');
                    var letterBlock = FindName(string.Format("letter{0}{1}", cells[0], cells[1])) as TextBlock;
                    ListHints.Add(new Coordinate(Int32.Parse(cells[0].ToString()), Int32.Parse(cells[1].ToString())));
                    var gridLetter = letterBlock.Parent as Grid;
                    //gridLetter.Background = new SolidColorBrush(currentColor);
                    gridLetter.Tag = new SolidColorBrush(currentColor);
                    letterBlock.Text = word.Content[currentLetter].ToString().ToUpper();
                    currentLetter++;
                }
                currentWord++;
            }
        }

        private void GenerateLevels(Dimension dimension, StackPanel RootStackPanel)
        {
            try
            {
                StackPanel tempStackPanel = null;
                RootStackPanel.Children.Clear();
                for (int i = 0; i < dimension.Columns; i++)
                {
                    for (int j = 0; j < dimension.Rows; j++)
                    {
                        double borderLetterSize = Dimension.Columns >= 5 ? LayoutRoot.ActualWidth / Dimension.Columns - 10 : 100;
                        double letterFontSize = Dimension.Columns >= 5 ? 55 * borderLetterSize / 100 : 55;
                        //double borderLetterSize = Dimension.Columns >= 5 ? LayoutRoot.ActualWidth / Dimension.Columns - 10 : 150;
                        //double letterFontSize = Dimension.Columns >= 5 ? 55 * borderLetterSize / 100 : 85;
                        var letter = new TextBlock
                        {
                            Name = string.Format("letter{1}{0}", i.ToString(), j.ToString()),

                            VerticalAlignment = System.Windows.VerticalAlignment.Center,
                            HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                            TextAlignment = System.Windows.TextAlignment.Center,
                            Foreground = new SolidColorBrush(Colors.White),//TODO:commented for generating the category images
                            //Foreground = GridLetterBrush,
                            FontSize = letterFontSize,
                            FontWeight = FontWeights.Bold,
                            FontFamily = new FontFamily("Comic Sans MS")
                        };

                        var borderLetter = new Border
                        {
                            Width = borderLetterSize,
                            Height = borderLetterSize,
                            BorderThickness = new Thickness(2),
                            BorderBrush = new SolidColorBrush(Colors.White),
                            //BorderBrush = new SolidColorBrush(Color.FromArgb(255, 255, 242, 0)),
                            CornerRadius = new CornerRadius(5),
                            Margin = new Thickness(2)
                        };
                        var gridLetter = new Grid
                        {
                            Background = GridLetterBrush
                        };
                        gridLetter.MouseEnter += new MouseEventHandler(LetterManipulation);
                        gridLetter.MouseLeave += new MouseEventHandler(LetterManipulation);
                        gridLetter.Children.Add(letter);
                        borderLetter.Child = gridLetter;
                        if (j == 0)
                        {
                            var stackPanel = new StackPanel
                            {
                                Orientation = System.Windows.Controls.Orientation.Horizontal,
                                Margin = new Thickness(0, 0, 0, 0)
                            };
                            stackPanel.Children.Add(borderLetter);
                            RootStackPanel.Children.Add(stackPanel);
                            tempStackPanel = stackPanel;
                        }
                        else
                        {
                            tempStackPanel.Children.Add(borderLetter);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }



        //private void ResetGridLettersBrush()
        //{
        //    for (int i = 0; i < Dimension; i++)
        //    {
        //        for (int j = 0; j < Dimension; j++)
        //        {
        //            var letterBlock = FindName(string.Format("letter{0}{1}", i, j)) as TextBlock;
        //            var gridLetter = letterBlock.Parent as Grid;
        //            gridLetter.Background = GridLetterBrush;
        //        }
        //    }
        //}

        private void LetterManipulation(object sender, MouseEventArgs e)
        {
            if (IsManipulationStarted)
            {
                var gridLetter = sender as Grid;
                if (gridLetter.Background == GridLetterBrush)
                {
                    gridLetter.Background = FillGridLetterBrush;
                    var letterBlock = gridLetter.Children[0] as TextBlock;
                    if (!TempListCurrentLetter.Contains(letterBlock.Name))
                    {
                        TempCurrentWord += letterBlock.Text.ToLower();
                        TempListCurrentLetter.Add(letterBlock.Name);
                        TempCurrentWordColor.Add((gridLetter.Tag as SolidColorBrush).Color);
                    }
                }
            }
        }
        private List<Color> TempCurrentWordColor { get; set; }
        protected override void OnManipulationStarted(ManipulationStartedEventArgs e)
        {
            base.OnManipulationStarted(e);
            IsManipulationCompleted = false;
            IsManipulationStarted = true;
        }

        protected override void OnManipulationCompleted(ManipulationCompletedEventArgs e)
        {
            base.OnManipulationCompleted(e);
            IsManipulationStarted = false;
            IsManipulationCompleted = true;
            if (ListCurrentWords.Contains(TempCurrentWord) && TempCurrentWordColor.Where(x => x == TempCurrentWordColor[0]).Count() == TempCurrentWordColor.Count)
            {
                CountWords++;
                foreach (var letterName in TempListCurrentLetter)
                {
                    var letter = FindName(letterName) as TextBlock;
                    letter.Foreground = new SolidColorBrush(Colors.White);
                    var gridLetter = letter.Parent as Grid;
                    gridLetter.Background = (SolidColorBrush)gridLetter.Tag;
                }
                if (CountWords == ListCurrentWords.Count)
                {
                    LevelCompleted.MediaEnded += delegate
                    {
                        if ((int)settings["lastLevelCompleted"] < LevelId)
                        {
                            int rating = Dimension.Columns * Dimension.Rows;
                            settings["rating"] = (int)settings["rating"] + rating;
                            settings.Save();
                        }
                        if (CountLevels == LevelId + 1)
                        {
                            settings["lastLevelCompleted"] = LevelId > (int)settings["lastLevelCompleted"] ? LevelId : (int)settings["lastLevelCompleted"];
                            settings.Save();
                            PopulateTopBarValues();
                            var popup = new PopupMessage();
                            popup.Show(AppResources.GameCompleted);
                        }
                        else
                        {
                            settings["lastLevelCompleted"] = LevelId > (int)settings["lastLevelCompleted"] ? LevelId : (int)settings["lastLevelCompleted"];
                            settings.Save();
                            NavigationService.Navigate(new Uri(string.Format("/GameViewPage.xaml?LevelId={0}", LevelId + 1), UriKind.RelativeOrAbsolute));
                        }
                    };
                    LevelCompleted.Play();
                }
            }
            else if (ListCurrentWords.Contains(TempCurrentWord) && TempCurrentWordColor.Where(x => x == TempCurrentWordColor[0]).Count() != TempCurrentWordColor.Count)
            {
                var popup = new PopupMessage();
                popup.Show(AppResources.DifferentPathWord);
                foreach (var letterName in TempListCurrentLetter)
                {
                    var letter = FindName(letterName) as TextBlock;
                    var gridLetter = letter.Parent as Grid;
                    gridLetter.Background = GridLetterBrush;
                }
            }
            else
            {
                foreach (var letterName in TempListCurrentLetter)
                {
                    var letter = FindName(letterName) as TextBlock;
                    var gridLetter = letter.Parent as Grid;
                    gridLetter.Background = GridLetterBrush;
                }
            }
            TempCurrentWord = string.Empty;
            TempListCurrentLetter.Clear();
            TempCurrentWordColor.Clear();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            string sLevelId = string.Empty;
            NavigationContext.QueryString.TryGetValue("LevelId", out sLevelId);
            if (!Int32.TryParse(sLevelId, out LevelId))
                LevelId = 0;
        }

        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            NavigationService.Navigate(new Uri("/CategoryPage.xaml", UriKind.RelativeOrAbsolute));
        }

        private void countHintsPanel_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            int countHints = (int)settings["countHints"];
            if (countHints > 0)
            {
                if (HintsEnumerator != ListHints.Count)
                {
                    TextBlock letter = null;
                    Grid gridLetter = null;
                    do
                    {
                        letter = FindName(string.Format("letter{0}{1}", ListHints[HintsEnumerator].Column, ListHints[HintsEnumerator].Row)) as TextBlock;
                        gridLetter = letter.Parent as Grid;
                        HintsEnumerator++;
                    }
                    while (gridLetter.Background == (SolidColorBrush)gridLetter.Tag);
                    letter.Foreground = new SolidColorBrush(Color.FromArgb(255, 139, 197, 1));                 
                    countHintsText.Text = (countHints - 1).ToString();
                    settings["countHints"] = countHints - 1;
                    settings.Save();
                }
            }
            else
            {
                NavigationService.Navigate(new Uri("/PurchasesPage.xaml", UriKind.RelativeOrAbsolute));
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

        #region Menu Buttons
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