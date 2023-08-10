using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marble.Core
{
    public struct Cell
    {
        public Cell(int row, int column)
        {
            Row = row;
            Column = column;
        }

        public int Row { get; set; }
        public int Column { get; set; }
    }

    public class Marble
    {
        public Marble(int number, Cell cell)
        {
            Number = number;
            Cell = cell;
        }

        public int Number { get; }
        public Cell Cell { get; set; }
    }

    public class Hole
    {
        public Hole(int number, Cell cell)
        {
            Number = number;
            Cell = cell;
        }

        public int Number { get; }
        public Cell Cell { get; set; }
    }

    public class Wall
    {
        public Wall(Cell firstCell, Cell secondCell)
        {
            FirstCell = firstCell;
            SecondCell = secondCell;
            Direction = firstCell.Row == secondCell.Row ? WallDirection.Vertical : WallDirection.Horizontal;
        }

        public Cell FirstCell { get; }
        public Cell SecondCell { get; }
        public WallDirection Direction { get; }
    }

    public enum WallDirection
    {
        Vertical,
        Horizontal
    }

    public enum GameAction
    {
        Left,
        Right,
        Up,
        Down
    }
}
