using FillWords.Phone._8._0.DataAccess;
using FillWords.Phone._8._0.LogicGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FillWords.Phone._8._0.Repositories
{
    public class GameViewRepository
    {
        DataManager dataManager { get; set; }

        public GameViewRepository()
        {
            dataManager = new DataManager();
        }

        private IList<GameView> GetGameViews()
        {
            var gameViews = new List<GameView>();
            var sGameViews = dataManager.ReadLinesFromTextFile("Content/gameviews.txt", Encoding.UTF8);
            foreach (var line in sGameViews)
            {
                string[] parts = line.Split(' ');
                string[] matrix = parts[1].Split(';');
                var listWordMatrix = new List<WordMatrix>();
                foreach (var item in matrix)//массив слов
                {
                    listWordMatrix.Add(
                        new WordMatrix
                        {
                            Matrix = item
                        });
                }
                var dimension = parts[0].Split(',');
                gameViews.Add(
                    new GameView
                    {
                        Dimension = new Dimension(Int32.Parse(dimension[0]), Int32.Parse(dimension[1])),
                        WordsLocation = listWordMatrix
                    });
            }
            return gameViews;
        }

        public GameView GetGameView(int levelId)
        {
            var gameView = GetGameViews()[levelId];
            return gameView;
        }

        public int GetCountGameViews()
        {
            var sGameViews = dataManager.ReadLinesFromTextFile("Content/gameviews.txt", Encoding.UTF8);
            return sGameViews.Count;
        }
    }
}
