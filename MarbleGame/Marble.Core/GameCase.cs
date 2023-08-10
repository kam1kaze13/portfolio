using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marble.Core
{
    public class GameCase
    {
        private int gameNumber;
        private int boardSize;
        private Marble[] marbles;
        private Hole[] holes;
        private Wall[] walls;

        public GameCase (int gameNumber, int boardSize, Marble[] marbles, Hole[] holes, Wall[] walls)
        {
            this.gameNumber = gameNumber;
            this.boardSize = boardSize;
            this.marbles = marbles;
            this.holes = holes;
            this.walls = walls;
        }

        public int HolesCount
        {
            get { return this.holes.Length; }
        }

        public int Move(GameAction action)
        {
            var newMarbles = new List<Marble>();

            switch (action)
            {
                case GameAction.Up:
                    {
                        var sortedMarbles = this.marbles.OrderBy(i => i.Cell.Row).ToList();

                        foreach (var marble in sortedMarbles)
                        {
                            Marble newMarble = null;

                            var marblesOnLine = sortedMarbles.Where(i => i.Cell.Column == marble.Cell.Column & i.Cell.Row < marble.Cell.Row);
                            var holesOnLine = this.holes.Where(i => i.Cell.Column == marble.Cell.Column & i.Cell.Row < marble.Cell.Row);
                            var wallOnLine = this.walls.Where(i => i.Direction == WallDirection.Horizontal & 
                                                                i.FirstCell.Column == marble.Cell.Column & i.SecondCell.Row <= marble.Cell.Row);

                            var marblesOnLineCount = marblesOnLine.Count();
                            var holesOnLineCount = holesOnLine.Count();
                            var wallsOnLineCount = wallOnLine.Count();

                            if (holesOnLineCount + wallsOnLineCount == 0)
                            {
                                newMarble = new Marble(marble.Number,new Cell(0 + marblesOnLineCount, marble.Cell.Column));
                            }
                            else
                            {
                                Hole closeHole = null;
                                Wall closeWall = null;

                                if (holesOnLineCount != 0)
                                {
                                    closeHole = holesOnLine.Where(i => i.Cell.Row == holesOnLine.Max(k => k.Cell.Row)).First();
                                }

                                if (wallsOnLineCount != 0)
                                {
                                    closeWall = wallOnLine.Where(i => i.SecondCell.Row == wallOnLine.Max(k => k.SecondCell.Row)).First();
                                    marblesOnLineCount = marblesOnLine.Where(i => i.Cell.Row >= closeWall.SecondCell.Row).Count();
                                }

                                if (closeHole == null)
                                {
                                    newMarble = new Marble(marble.Number,new Cell(closeWall.SecondCell.Row + marblesOnLineCount, marble.Cell.Column));
                                }
                                else if (closeWall == null)
                                {
                                    if (closeHole.Number != marble.Number)
                                        return "INCORRECT".GetHashCode();

                                    var tmpHoles = this.holes.ToList();
                                    tmpHoles.Remove(closeHole);
                                    this.holes = tmpHoles.ToArray();

                                    var tmpSortedMarbles = sortedMarbles.ToList();
                                    tmpSortedMarbles.Remove(marble);
                                    sortedMarbles = tmpSortedMarbles;
                                }
                                else
                                {
                                    if (closeHole.Cell.Row > closeWall.SecondCell.Row)
                                    {
                                        if (closeHole.Number != marble.Number)
                                            return "INCORRECT".GetHashCode();

                                        var tmpHoles = this.holes.ToList();
                                        tmpHoles.Remove(closeHole);
                                        this.holes = tmpHoles.ToArray();

                                        var tmpSortedMarbles = sortedMarbles.ToList();
                                        tmpSortedMarbles.Remove(marble);
                                        sortedMarbles = tmpSortedMarbles;
                                    }
                                    else
                                    {
                                        newMarble = new Marble(marble.Number,new Cell (closeWall.SecondCell.Row + marblesOnLineCount, marble.Cell.Column));
                                    }
                                }
                            }

                            if (newMarble != null)
                                newMarbles.Add(newMarble);
                        }

                        break;
                    }
                case GameAction.Down:
                    {
                        var sortedMarbles = this.marbles.OrderByDescending(i => i.Cell.Row).ToList();

                        foreach (var marble in sortedMarbles)
                        {
                            Marble newMarble = null;

                            var marblesOnLine = sortedMarbles.Where(i => i.Cell.Column == marble.Cell.Column & i.Cell.Row > marble.Cell.Row);
                            var holesOnLine = this.holes.Where(i => i.Cell.Column == marble.Cell.Column & i.Cell.Row > marble.Cell.Row);
                            var wallOnLine = this.walls.Where(i => i.Direction == WallDirection.Horizontal & 
                                                                i.FirstCell.Column == marble.Cell.Column & i.FirstCell.Row >= marble.Cell.Row);

                            var marblesOnLineCount = marblesOnLine.Count();
                            var holesOnLineCount = holesOnLine.Count();
                            var wallsOnLineCount = wallOnLine.Count();

                            if (holesOnLineCount + wallsOnLineCount == 0)
                            {
                                newMarble = new Marble(marble.Number, new Cell(this.boardSize - 1 - marblesOnLineCount, marble.Cell.Column));
                            }
                            else
                            {
                                Hole closeHole = null;
                                Wall closeWall = null;

                                if (holesOnLineCount != 0)
                                {
                                    closeHole = holesOnLine.Where(i => i.Cell.Row == holesOnLine.Min(k => k.Cell.Row)).First();
                                }

                                if (wallsOnLineCount != 0)
                                {
                                    closeWall = wallOnLine.Where(i => i.FirstCell.Row == wallOnLine.Min(k => k.FirstCell.Row)).First();
                                    marblesOnLineCount = marblesOnLine.Where(i => i.Cell.Row <= closeWall.FirstCell.Row).Count();
                                }

                                if (closeHole == null)
                                {
                                    newMarble = new Marble(marble.Number,new Cell(closeWall.FirstCell.Row - marblesOnLineCount, marble.Cell.Column));
                                }
                                else if (closeWall == null)
                                {
                                    if (closeHole.Number != marble.Number)
                                        return "INCORRECT".GetHashCode();

                                    var tmpHoles = this.holes.ToList();
                                    tmpHoles.Remove(closeHole);
                                    this.holes = tmpHoles.ToArray();

                                    var tmpSortedMarbles = sortedMarbles.ToList();
                                    tmpSortedMarbles.Remove(marble);
                                    sortedMarbles = tmpSortedMarbles;
                                }
                                else
                                {
                                    if (closeHole.Cell.Row < closeWall.FirstCell.Row)
                                    {
                                        if (closeHole.Number != marble.Number)
                                            return "INCORRECT".GetHashCode();

                                        var tmpHoles = this.holes.ToList();
                                        tmpHoles.Remove(closeHole);
                                        this.holes = tmpHoles.ToArray();

                                        var tmpSortedMarbles = sortedMarbles.ToList();
                                        tmpSortedMarbles.Remove(marble);
                                        sortedMarbles = tmpSortedMarbles;
                                    }
                                    else
                                    {
                                        newMarble = new Marble(marble.Number,new Cell(closeWall.FirstCell.Row - marblesOnLineCount, marble.Cell.Column));
                                    }
                                }
                            }

                            if (newMarble != null)
                                newMarbles.Add(newMarble);
                        }

                        break;
                    }
                case GameAction.Left:
                    {
                        var sortedMarbles = this.marbles.OrderBy(i => i.Cell.Column).ToList();

                        foreach (var marble in sortedMarbles)
                        {
                            Marble newMarble = null;

                            var marblesOnLine = sortedMarbles.Where(i => i.Cell.Row == marble.Cell.Row & i.Cell.Column < marble.Cell.Column);
                            var holesOnLine = this.holes.Where(i => i.Cell.Row == marble.Cell.Row & i.Cell.Column < marble.Cell.Column);
                            var wallOnLine = this.walls.Where(i => i.Direction == WallDirection.Vertical & 
                                                                i.FirstCell.Row == marble.Cell.Row & i.SecondCell.Column <= marble.Cell.Column);

                            var marblesOnLineCount = marblesOnLine.Count();
                            var holesOnLineCount = holesOnLine.Count();
                            var wallsOnLineCount = wallOnLine.Count();

                            if (holesOnLineCount + wallsOnLineCount == 0)
                            {
                                newMarble = new Marble(marble.Number,new Cell(marble.Cell.Row, 0 + marblesOnLineCount));
                            }
                            else
                            {
                                Hole closeHole = null;
                                Wall closeWall = null;

                                if (holesOnLineCount != 0)
                                {
                                    closeHole = holesOnLine.Where(i => i.Cell.Column == holesOnLine.Max(k => k.Cell.Column)).First();
                                }

                                if (wallsOnLineCount != 0)
                                {
                                    closeWall = wallOnLine.Where(i => i.SecondCell.Column == wallOnLine.Max(k => k.SecondCell.Column)).First();
                                    marblesOnLineCount = marblesOnLine.Where(i => i.Cell.Column >= closeWall.SecondCell.Column).Count();
                                }

                                if (closeHole == null)
                                {
                                    newMarble = new Marble(marble.Number,new Cell(marble.Cell.Row, closeWall.SecondCell.Column + marblesOnLineCount));
                                }
                                else if (closeWall == null)
                                {
                                    if (closeHole.Number != marble.Number)
                                        return "INCORRECT".GetHashCode();

                                    var tmpHoles = this.holes.ToList();
                                    tmpHoles.Remove(closeHole);
                                    this.holes = tmpHoles.ToArray();

                                    var tmpSortedMarbles = sortedMarbles.ToList();
                                    tmpSortedMarbles.Remove(marble);
                                    sortedMarbles = tmpSortedMarbles;
                                }
                                else
                                {
                                    if (closeHole.Cell.Column > closeWall.SecondCell.Column)
                                    {
                                        if (closeHole.Number != marble.Number)
                                            return "INCORRECT".GetHashCode();

                                        var tmpHoles = this.holes.ToList();
                                        tmpHoles.Remove(closeHole);
                                        this.holes = tmpHoles.ToArray();

                                        var tmpSortedMarbles = sortedMarbles.ToList();
                                        tmpSortedMarbles.Remove(marble);
                                        sortedMarbles = tmpSortedMarbles;
                                    }
                                    else
                                    {
                                        newMarble = new Marble(marble.Number,new Cell(marble.Cell.Row, closeWall.SecondCell.Column + marblesOnLineCount));
                                    }
                                }
                            }

                            if (newMarble != null)
                                newMarbles.Add(newMarble);
                        }

                        break;
                    }
                case GameAction.Right:
                    {
                        var sortedMarbles = this.marbles.OrderByDescending(i => i.Cell.Column).ToList();

                        foreach (var marble in sortedMarbles)
                        {
                            Marble newMarble = null;

                            var marblesOnLine = sortedMarbles.Where(i => i.Cell.Row == marble.Cell.Row & i.Cell.Column > marble.Cell.Column);
                            var holesOnLine = this.holes.Where(i => i.Cell.Row == marble.Cell.Row & i.Cell.Column > marble.Cell.Column);
                            var wallOnLine = this.walls.Where(i => i.Direction == WallDirection.Vertical &
                                                                i.FirstCell.Row == marble.Cell.Row & i.FirstCell.Column >= marble.Cell.Column);

                            var marblesOnLineCount = marblesOnLine.Count();
                            var holesOnLineCount = holesOnLine.Count();
                            var wallsOnLineCount = wallOnLine.Count();

                            if (holesOnLineCount + wallsOnLineCount == 0)
                            {
                                newMarble = new Marble(marble.Number,new Cell(marble.Cell.Row, this.boardSize - 1 - marblesOnLineCount));
                            }
                            else
                            {
                                Hole closeHole = null;
                                Wall closeWall = null;

                                if (holesOnLineCount != 0)
                                {
                                    closeHole = holesOnLine.Where(i => i.Cell.Column == holesOnLine.Min(k => k.Cell.Column)).First();
                                }

                                if (wallsOnLineCount != 0)
                                {
                                    closeWall = wallOnLine.Where(i => i.FirstCell.Column == wallOnLine.Min(k => k.FirstCell.Column)).First();
                                    marblesOnLineCount = marblesOnLine.Where(i => i.Cell.Column <= closeWall.FirstCell.Column).Count();
                                }

                                if (closeHole == null)
                                {
                                    newMarble = new Marble(marble.Number,new Cell(marble.Cell.Row, closeWall.FirstCell.Column - marblesOnLineCount));
                                }
                                else if (closeWall == null)
                                {
                                    if (closeHole.Number != marble.Number)
                                        return "INCORRECT".GetHashCode();

                                    var tmpHoles = this.holes.ToList();
                                    tmpHoles.Remove(closeHole);
                                    this.holes = tmpHoles.ToArray();

                                    var tmpSortedMarbles = sortedMarbles.ToList();
                                    tmpSortedMarbles.Remove(marble);
                                    sortedMarbles = tmpSortedMarbles;
                                }
                                else
                                {
                                    if (closeHole.Cell.Column < closeWall.FirstCell.Column)
                                    {
                                        if (closeHole.Number != marble.Number)
                                            return "INCORRECT".GetHashCode();

                                        var tmpHoles = this.holes.ToList();
                                        tmpHoles.Remove(closeHole);
                                        this.holes = tmpHoles.ToArray();

                                        var tmpSortedMarbles = sortedMarbles.ToList();
                                        tmpSortedMarbles.Remove(marble);
                                        sortedMarbles = tmpSortedMarbles;
                                    }
                                    else
                                    {
                                        newMarble = new Marble(marble.Number,new Cell(marble.Cell.Row, closeWall.FirstCell.Column - marblesOnLineCount));
                                    }
                                }
                            }

                            if (newMarble != null)
                                newMarbles.Add(newMarble);
                        }

                        break;
                    }
            }

            this.marbles = newMarbles.ToArray();

            return this.GetHashCode();
        }

        public override int GetHashCode()
        {
            var state = string.Join("", this.marbles.Select(marble => marble.Number.ToString() + (marble.Cell.Column + (this.boardSize * marble.Cell.Row)).ToString()));
            return state.GetHashCode();
        }

        public object Clone()
        {
            return new GameCase(this.gameNumber, this.boardSize, this.marbles, this.holes, this.walls);
        }
    }
}
