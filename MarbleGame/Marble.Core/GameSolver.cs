using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marble.Core
{
    public class GameSolver
    {
        public static void SolveGames(string path)
        {
            using (var fs = new FileStream(path, FileMode.Open))
            {
                var gameCases = FileHelper.ReadFile(fs);

                foreach (var gameCase in gameCases)
                {
                    var gameEngine = new GameEngine(gameCase);
                    var result = Task.Run(() => gameEngine.RunGameCase());
                    Console.WriteLine(result.Result);
                }
            }
        }
    }
}
