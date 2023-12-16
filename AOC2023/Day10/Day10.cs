
using System.Numerics;
using Xunit.Abstractions;

namespace AOC2023.Day10;

public class Day10 : Day
{
    public Day10(ITestOutputHelper output) : base(output)
    {
    }

    public override string TestValue => "10";

    public override async Task<string> Run(StreamReader inputData)
    {
        var l = "";
        int y = 0;
        var nodes = new Dictionary<Vector2, Node>();
        while((l = inputData.ReadLine()) != null)
        {
            var x = 0;
            foreach (var c  in l)
            {
                var node = new Node(x, y, c);
                nodes.Add(node.Position, node);
                x++;
            }
            y++;
        }

        var startingNode = nodes.Values.First(n => n.IsStart);
        startingNode.Weight = 0;
        var path = new Path(startingNode, nodes);
        path.GetNext();
        var endNode = nodes.Values.MaxBy(n => n.Weight);
        var mainLoop = new List<Node>() { endNode };
        var nextNodes = endNode.GetNeighbour(nodes).Where(n => n.Weight == endNode.Weight - 1).ToList();
        while(nextNodes.Count > 0)
        {
            mainLoop.AddRange(nextNodes);
            nextNodes = nextNodes.SelectMany(n => n.GetNeighbour(nodes).Where(ne => ne.Weight == n.Weight - 1)).ToList();
        }

        var containedNodes = nodes.ToDictionary(k => k.Key, k => k.Value);
        var maxX = nodes.Max(n => n.Key.X);
        var maxY = nodes.Max(n => n.Key.Y);

        bool inside = false;
        for (int gy = 0; gy < maxY; gy++)
        {
            for (int gx = 0; gx < maxX; gx++)
            {
                var loopNode = mainLoop.FirstOrDefault(n => n == nodes[new Vector2(gx, gy)]);
                if (loopNode != null)
                {
                    if(loopNode.Directions.Contains(Direction.UP))
                        inside = !inside;
                    continue;
                }
                nodes[new Vector2(gx, gy)].Inside = inside;
            }
            inside = false;
        }

        var res = nodes.Values.Where(n => n.Inside).ToList();
        return res.Count().ToString();
    }

    public class Path
    {
        public List<Node> CurrentNodes { get; set; }
        public Dictionary<Vector2, Node> Visited { get; set; } = new();
        public long Weight { get; set; } = 1;
        public Dictionary<Vector2, Node> AllNodes { get; }

        public Path(Node currentNode, Dictionary<Vector2, Node> allNodes)
        {
            CurrentNodes = new List<Node> { currentNode };
            currentNode.Weight = 0;
            AllNodes = allNodes;
        }

        public void GetNext()
        {
            var nextNodes = new List<Node>();
            while (CurrentNodes.Count > 0)
            {
                foreach (var currentNode in CurrentNodes)
                {
                    foreach (var next in currentNode.GetNeighbour(AllNodes))
                    {
                        if (Visited.ContainsKey(next.Position))
                        {
                            var visitedNode = Visited[next.Position];
                            if (visitedNode.Weight > Weight)
                                visitedNode.Weight = Weight;
                        }
                        else
                        {
                            next.Weight = Weight;
                            Visited.Add(next.Position, next);
                            nextNodes.Add(next);
                        }
                    }
                }
                CurrentNodes = nextNodes.ToList();
                nextNodes.Clear();
                Weight++;
            }
        }
    }

    public class Node
    {
        public Vector2 Position { get; }
        public List<Direction> Directions { get; } = new();
        public bool IsStart { get; } = false;

        public long Weight { get; set; } = -1;
        public char VisualRepresentation { get; set; }
        public bool Inside { get; set; } = false;

        public Node(int x, int y, char c)
        {
            Position = new Vector2(x, y);
            VisualRepresentation = c;
            switch (c)
            {
                case 'S':
                    Directions.Add(Direction.UP);
                    Directions.Add(Direction.DOWN);
                    Directions.Add(Direction.LEFT);
                    Directions.Add(Direction.RIGHT);
                    IsStart = true;
                    break;
                case '|':
                    Directions.Add(Direction.UP);
                    Directions.Add(Direction.DOWN);
                    break;
                case '-':
                    Directions.Add(Direction.RIGHT);
                    Directions.Add(Direction.LEFT);
                    break;
                case 'L':
                    Directions.Add(Direction.UP);
                    Directions.Add(Direction.RIGHT);
                    break;
                case 'J':
                    Directions.Add(Direction.UP);
                    Directions.Add(Direction.LEFT);
                    break;
                case '7':
                    Directions.Add(Direction.DOWN);
                    Directions.Add(Direction.LEFT);
                    break;
                case 'F':
                    Directions.Add(Direction.DOWN);
                    Directions.Add(Direction.RIGHT);
                    break;
            }
        }

        public IEnumerable<Node> GetNeighbour(Dictionary<Vector2, Node> allNodes)
        {
            foreach (var direction in Directions)
            {
                Vector2 next = direction switch
                {
                    Direction.UP => new Vector2(Position.X, Position.Y - 1),
                    Direction.DOWN => new Vector2(Position.X, Position.Y + 1),
                    Direction.LEFT => new Vector2(Position.X - 1, Position.Y),
                    Direction.RIGHT => new Vector2(Position.X + 1, Position.Y),
                };

                if (!allNodes.ContainsKey(next))
                    continue;

                var node = allNodes[next];
                switch (direction)
                {
                    case Direction.UP:
                        if (!node.Directions.Contains(Direction.DOWN))
                        {
                            //Directions.Remove(Direction.UP);
                            continue;
                        }
                        break;
                    case Direction.DOWN:
                        if (!node.Directions.Contains(Direction.UP))
                        {
                            //Directions.Remove(Direction.DOWN);
                            continue;
                        }
                        break;
                    case Direction.LEFT:
                        if (!node.Directions.Contains(Direction.RIGHT))
                        {
                            //Directions.Remove(Direction.LEFT);
                            continue;
                        }
                        break;
                    case Direction.RIGHT:
                        if (!node.Directions.Contains(Direction.LEFT))
                        {
                            //Directions.Remove(Direction.RIGHT);
                            continue;
                        }
                        break;
                }
                yield return node;
            }
        }
    }

    public enum Direction
    {
        UP,
        DOWN,
        LEFT,
        RIGHT
    }
}
