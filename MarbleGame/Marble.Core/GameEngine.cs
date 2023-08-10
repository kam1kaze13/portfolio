using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Marble.Core
{
    public class GameEngine
    {
        private int incorrectCode = "INCORRECT".GetHashCode();
        private HashSet<int> states = new HashSet<int>();
        private List<Node> nodes = new List<Node>();
        private readonly GameCase gameCase;
        private bool isResolve = false;
        private object locker = new object();
        private int stepsCount = -1;

        public GameEngine(GameCase gameCase)
        {
            this.gameCase = gameCase;
        }

        public string RunGameCase()
        {
            states.Add(this.gameCase.GetHashCode());

            nodes.Add(new Node(this.gameCase, null, this.gameCase.GetHashCode(), Node.NodeState.Waiting, 0));

            while (nodes.Where(i => i.State == Node.NodeState.Waiting).Count() > 0 && !this.isResolve)
            {
                var watingNodes = nodes.Where(i => i.State == Node.NodeState.Waiting).ToList();

                foreach (var node in watingNodes)
                {
                    List<Task> tasks = new List<Task>();

                    foreach (GameAction action in Enum.GetValues(typeof(GameAction)))
                    {
                        if (action != node.PrevMove)
                        {
                            tasks.Add(Task.Run(() => Move(node, action)));
                        }
                    }

                    Task.WaitAll(tasks.ToArray());
                    tasks.Clear();
                    node.State = Node.NodeState.Processing;
                }
            }

            return $"Result : {this.stepsCount}";
        }

        private void Move(Node node, GameAction action)
        {
            var gameCase = (GameCase)node.GameCase.Clone();
            var newState = gameCase.Move(action);

            lock (this.locker)
            {
                if (newState != this.incorrectCode)
                {
                    if (!states.Contains(newState))
                    {
                        states.Add(newState);

                        nodes.Add(new Node(gameCase, action, newState, Node.NodeState.Waiting, node.GenNum + 1));
                    }
                }
            }

            if (gameCase.HolesCount == 0)
            {
                this.isResolve = true;
                this.stepsCount = node.GenNum + 1;
            }
        }

        private class Node
        {
            public Node(GameCase gameCase, GameAction? prevMove, int mapCode, NodeState state, int genNum)
            {
                this.GameCase = gameCase;
                this.PrevMove = prevMove;
                this.MapCode = mapCode;
                this.State = state;
                this.GenNum = genNum;
            }

            public GameCase GameCase { get; }
            public GameAction? PrevMove { get; }
            public enum NodeState
            {
                Processing,
                Waiting,
            }
            public NodeState State { get; set; }
            public int MapCode { get; }
            public int GenNum { get; }
        }
    }
}
