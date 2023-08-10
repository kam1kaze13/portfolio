using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marble.Core
{
    public static class FileHelper
    {
        public static IEnumerable<GameCase> ReadFile(FileStream stream)
        {
            var reader = new StreamReader(stream);

            string line = "";
            int count = 1;
            while ((line = reader.ReadLine()) != "0 0 0")
            {
                var components = line.Split(' ');

                var boardSize = Int32.Parse(components[0]);
                var countMarbles = Int32.Parse(components[1]);
                var countWalls = Int32.Parse(components[2]);

                var marbles = new Marble[countMarbles];

                for (int i = 0; i < countMarbles; i++)
                {
                    line = reader.ReadLine();
                    components = line.Split(' ');

                    marbles[i] = new Marble(i + 1, new Cell(Int32.Parse(components[0]), Int32.Parse(components[1])));
                }

                var holes = new Hole[countMarbles];

                for (int i = 0; i < countMarbles; i++)
                {
                    line = reader.ReadLine();
                    components = line.Split(' ');

                    holes[i] = new Hole(i + 1, new Cell(Int32.Parse(components[0]), Int32.Parse(components[1])));
                }

                var walls = new Wall[countWalls];

                for (int i = 0; i < countWalls; i++)
                {
                    line = reader.ReadLine();
                    components = line.Split(' ');

                    walls[i] = new Wall(new Cell(Int32.Parse(components[0]), Int32.Parse(components[1])), new Cell(Int32.Parse(components[2]), Int32.Parse(components[3])));
                }

                var gameCase = new GameCase(count, boardSize, marbles, holes, walls);
                count++;

                yield return gameCase;
            }

        }
    }
}
