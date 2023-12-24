
using System.Linq;
using Xunit.Abstractions;

namespace AOC2023.Day23;

public class Day23 : Day
{
    public Day23(ITestOutputHelper output) : base(output)
    {
    }

    public override string TestValue => "154";

    public override async Task<string> Run(StreamReader inputData)
    {
        var map = new Dictionary<Position, Node>();
        var y = 0;
        string? l;
        while ((l = await inputData.ReadLineAsync()) != null)
        {
            var x = 0;
            foreach (var c in l)
            {
                var pos = new Position(x, y);
                var node = new Node(pos, c);
                map.Add(pos, node);
                x++;
            }
            y++;
        }
        var start = map[new(1, 0)];
        var end = map[new(map.Max(m => m.Key.x)-1, map.Max(m => m.Key.y))];

        return FindLongestPath(start, end, map).ToString();
    }

    public long DistanceTo(Node start, List<Node> ends, Dictionary<Position, Node> map, out Node endNode)
    {
        var visited = new HashSet<Node>();
        var current = start;
        long steps = 1l;
        while(!ends.Contains(current))
        {
            visited.Add(current);
            var next = current.GetMoves(map).Except(visited).ToList();
            steps++;
            current = next.Single();
        }
        endNode = current;
        return steps;
    }

    public long FindLongestPath(Node start, Node end, Dictionary<Position, Node> map)
    {
        //Build chunks
        var pivotPoints = map.Where(n => n.Value.IsPivot(map)).Select(m => m.Value).ToList();
        var current = new List<Node>() { start };

        var initialChunkDistance = DistanceTo(start, pivotPoints, map, out var firstNode);
        var initialChunk = new Chunk(start, firstNode, initialChunkDistance);
        var chunks = new List<Chunk>() { initialChunk };

        foreach(var node in pivotPoints)
        {
            foreach(var next in node.GetMoves(map))
            {
                var d = DistanceTo(next, pivotPoints
                    .Union(new[] { end }) //Include end as pivot
                    .Except(new[] { node }) //No backtrack
                    .ToList(), map, out var target);
                var chunk = new Chunk(node, target, d);
                //Calculate neighbours
                foreach(var c in chunks)
                {
                    if (c.PointA == node || c.PointB == node || c.PointA == target || c.PointB == target)
                    {
                        if(!c.Neighbours.Contains(chunk))
                            c.Neighbours.Add(chunk);
                        if (!chunk.Neighbours.Contains(c)) 
                            chunk.Neighbours.Add(c);
                    }
                }

                chunks.Add(chunk);
            }
        }
        var lastChunk = chunks.Where(c => c.PointB == end).Single();

        var max = 0l;

        /*
        foreach(var p in GetPossiblePath(start, chunks.First(), end, chunks, new(), 0))
        {
            max = max < p ? p : max;
        }
        */

        var paths = new List<Path>() { new(initialChunk, initialChunk.PointB, new() { initialChunk }, new() { initialChunk }, lastChunk) };
        Path? bestPath = null;
        while (paths.Any())
        {
            paths = paths.SelectMany(p => p.Expand(chunks)).ToList();
            foreach (var p in paths)
            {
                if (p.Current == lastChunk)
                {
                    bestPath = (bestPath?.TotalDistance ?? 0) < p.TotalDistance ? p : bestPath;
                }
            }
        }

        return bestPath!.TotalDistance - 1; //minus start
    }

    public IEnumerable<long> GetPossiblePath(Node currentNode, Chunk current, Node end, List<Chunk> chunks, HashSet<Chunk> visited, long runningCount)
    {
        if(current.PointA == end || current.PointB == end)
        {
            yield return runningCount + current.Distance;
            yield break;
        }
        var neighbours = current.NeighboursFrom(currentNode).Except(visited).ToList();
        var newVisited = visited.Union(new[] { current }).ToHashSet();
        if (neighbours.Count <= 2)
        {
            //We won't be able to come back, so exclude both branch
            newVisited = newVisited.Union(neighbours).ToHashSet();
        }

        foreach (var chunk in neighbours)
        {
            var nextNode = currentNode == current.PointA ? current.PointB : current.PointA;
            foreach (var d in GetPossiblePath(nextNode, chunk, end, chunks, newVisited, runningCount + current.Distance + 1))
                yield return d;
        }
    }

    public IEnumerable<long> GetDistances(Node start, Dictionary<(Node, Node), long> points, long runningSum)
    {
        foreach(var next in points.Where(p => p.Key.Item1 == start))
        {
            foreach(var d in GetDistances(next.Key.Item2, points, runningSum + next.Value))
                yield return d;
        }
        yield return runningSum;
    }

    public class Path
    {
        public HashSet<Chunk> Visited { get; } = new();
        public HashSet<Chunk> Explored { get; }
        public Chunk Last { get; }
        public Chunk Current { get; }
        public Node CurrentNode { get; }

        public long TotalDistance => Visited.Sum(v => v.Distance);

        public Path(Chunk current, Node currentNode, HashSet<Chunk> visited, HashSet<Chunk> explored, Chunk last)
        {
            Current = current;
            CurrentNode = currentNode;
            Visited = visited;
            Explored = explored;
            Last = last;
        }

        public IEnumerable<Path> Expand(List<Chunk> chunks)
        {
            var possibilities = chunks.Except(Explored).Where(c => c.PointA == CurrentNode || c.PointB == CurrentNode).ToList();
            var newExplored = Explored.ToHashSet();
            if(possibilities.Count <= 2)
            {
                foreach (var p in possibilities)
                    newExplored.Add(p);
            }
            foreach(var p in possibilities)
            {
                newExplored.Add(p);
                yield return new Path(p, CurrentNode == p.PointA ? p.PointB : p.PointA, Visited.Union(new[] { p }).ToHashSet(), newExplored, Last);
            }
        }
    }

    public record Position(int x, int y);

    public class Node
    {
        public Position Position { get; set; }
        public char Type { get; set; }
        public long Cost { get; set; } = 1;
        public Node? Parent { get; set; } = null;

        public Node(Position position, char type)
        {
            Position = position;
            Type = type;
        }

        public bool IsPivot(Dictionary<Position, Node> map) => Type != '#' && GetMoves(map).All(n => n.Type != '.');

        public IEnumerable<Node> GetMoves(Dictionary<Position, Node> map)
        {
            var up = Position with { y = Position.y - 1 };
            var down = Position with { y = Position.y + 1 };
            var left = Position with { x = Position.x - 1 };
            var right = Position with { x = Position.x + 1 };
            switch(Type)
            {
                case '.':
                    if (map.ContainsKey(up) && !new[] { '#', 'v' }.Contains(map[up].Type)) yield return map[up];
                    if (map.ContainsKey(down) && !new[] { '#', '^' }.Contains(map[down].Type)) yield return map[down];
                    if (map.ContainsKey(left) && !new[] { '#', '>' }.Contains(map[left].Type)) yield return map[left];
                    if (map.ContainsKey(right) && !new[] { '#', '<' }.Contains(map[right].Type)) yield return map[right];
                    break;
                case '^':
                    if (map.ContainsKey(up) && !new[] { '#', 'v' }.Contains(map[up].Type)) yield return map[up];
                    break;
                case '>':
                    if (map.ContainsKey(right) && !new[] { '#', '<' }.Contains(map[right].Type)) yield return map[right];
                    break;
                case '<':
                    if (map.ContainsKey(left) && !new[] { '#', '>' }.Contains(map[left].Type)) yield return map[left];
                    break;
                case 'v':
                    if (map.ContainsKey(down) && !new[] { '#', '^' }.Contains(map[down].Type)) yield return map[down];
                    break;
                default:
                    throw new Exception("unknown node type");

            }
        }

        public override string ToString()
        {
            return $"{Position.x},{Position.y} | {Type}";
        }
    }

    public class Chunk
    {
        public Node PointA { get; }
        public Node PointB { get; }
        public long Distance { get; }
        public List<Chunk> Neighbours { get; } = new();

        public List<Chunk> NeighboursFrom(Node current)
        {
            var next = current == PointA ? PointB : PointA;
            return Neighbours.Where(n => n.PointA == next || n.PointB == next).ToList();
        }

        public Chunk(Node pointA, Node pointB, long distance)
        {
            PointA = pointA;
            PointB = pointB;
            Distance = distance;
        }
    }
}
