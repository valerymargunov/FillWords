using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FillWords.Phone._8._0.LogicGame
{
    public class Dimension
    {
        public Dimension(int columns, int rows)
        {
            Columns = columns;
            Rows = rows;
        }
        public int Columns { get; set; }
        public int Rows { get; set; }
    }
}
