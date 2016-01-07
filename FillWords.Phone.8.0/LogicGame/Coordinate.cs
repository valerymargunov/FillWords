using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FillWords.Phone._8._0.LogicGame
{
    public class Coordinate
    {
        public Coordinate(int column, int row)
        {
            Column = column;
            Row = row;
        }
        public int Column { get; set; }
        public int Row { get; set; }
    }
}
