using FillWords.Phone._8._0.DataAccess;
using FillWords.Phone._8._0.LogicGame;
using MSPToolkit.Encodings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FillWords.Phone._8._0.Repositories
{
    public class CategoryRepository
    {
        DataManager dataManager { get; set; }

        public CategoryRepository()
        {
            dataManager = new DataManager();
        }

        public IList<Category> GetCategories()
        {
            var categories = new List<Category>();
            var sCategories = dataManager.ReadLinesFromTextFile("Content/categories.txt", new Windows1251Encoding());
            foreach (var line in sCategories)
            {
                string[] parts = line.Split(' ');
                categories.Add(new Category 
                {
                    Title = parts[1],
                    ImageSource = new Uri(parts[2], UriKind.RelativeOrAbsolute),
                    StartLevelId = Int32.Parse(parts[0])
                });
            }
            return categories;
        }
    }
}
