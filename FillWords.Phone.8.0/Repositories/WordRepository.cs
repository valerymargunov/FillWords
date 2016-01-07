using FillWords.Phone._8._0.DataAccess;
using FillWords.Phone._8._0.LogicGame;
using MSPToolkit.Encodings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FillWords.Phone._8._0.Repositories
{
    public class WordRepository
    {
        DataManager dataManager { get; set; }

        public WordRepository()
        {
            dataManager = new DataManager();
        }

        private IList<Word> GetWords()
        {
            var words = new List<Word>();
            var sWords = dataManager.ReadLinesFromTextFile("Content/words.txt", new Windows1251Encoding());
            foreach (var line in sWords)
            {
                string[] parts = line.Split(' ');
                words.Add(new Word 
                {
                    LevelId = Int32.Parse(parts[0]),
                    Content = parts[1]
                });
            }
            return words;
        }

        public IList<Word> GetWords(int levelId)
        {
            var word = GetWords().Where(x => x.LevelId == levelId).ToList();
            return word;
        }
    }
}
