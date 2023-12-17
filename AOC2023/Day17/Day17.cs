using System.Diagnostics;
using Xunit.Abstractions;

namespace AOC2023.Day17;

public class Day17 : Day
{
    public Day17(ITestOutputHelper output) : base(output)
    {
    }

    public override string TestValue => "94";

    public override async Task<string> Run(StreamReader inputData)
    {
        var s = "";
        int y = 0;
        var map = new Dictionary<Vector, int>();
        while ((s = inputData.ReadLine()) != null)
        {
            for (int x = 0; x < s.Length; x++)
            {
                map.Add(new(x, y), int.Parse(s[x].ToString()));
            }
            y++;
        }

        var path = FindPath(new(0, 0), new(map.Max(m => m.Key.x), map.Max(m => m.Key.y)), map);

        return path.ToString();
    }

    public int FindPath(Vector start, Vector end, Dictionary<Vector, int> map)
    {
        var queue = new PriorityQueue<Visited, int>();
        var visited = new Dictionary<Visited, int>();
        queue.Enqueue(new(0, 0, 0, Direction.RIGHT), 0);
        queue.Enqueue(new(0, 0, 0, Direction.DOWN), 0);

        while (queue.Count != 0)
        {
            var current = queue.Peek();
            var weight = queue.UnorderedItems.First(i => i.Element == current).Priority;
            queue.Dequeue();
            if (!visited.ContainsKey(current))
                visited[current] = weight;
            else continue;

            foreach(var n in current.GetNeighbours())
            {
                if (!map.ContainsKey(new(n.x, n.y)))
                    continue;
                if (visited.ContainsKey(n))
                    continue;
                var newWeight = weight + map[new(n.x, n.y)];
                queue.Enqueue(n, newWeight);

                if (new Vector(n.x, n.y) == end && n.step > 2)
                    return newWeight;
            }
        }

        return -1;
    }

    public record Vector(int x, int y);

    public record Visited(int x, int y, int step, Direction direction)
    {
        public IEnumerable<Visited> GetNeighbours()
        {
            switch (direction)
            {
                case Direction.UP:
                    if (step < 9) yield return this with { step = step + 1, y = y - 1 };
                    if (step > 2) yield return this with { step = 0, x = x - 1, direction = Direction.LEFT };
                    if (step > 2) yield return this with { step = 0, x = x + 1, direction = Direction.RIGHT };
                    break;
                case Direction.DOWN:
                    if (step < 9) yield return this with { step = step + 1, y = y + 1 };
                    if (step > 2) yield return this with { step = 0, x = x - 1, direction = Direction.LEFT };
                    if (step > 2) yield return this with { step = 0, x = x + 1, direction = Direction.RIGHT };
                    break;
                case Direction.LEFT:
                    if (step < 9) yield return this with { step = step + 1, x = x - 1 };
                    if (step > 2) yield return this with { step = 0, y = y - 1, direction = Direction.UP };
                    if (step > 2) yield return this with { step = 0, y = y + 1, direction = Direction.DOWN };
                    break;
                case Direction.RIGHT:
                    if (step < 9) yield return this with { step = step + 1, x = x + 1 };
                    if (step > 2) yield return this with { step = 0, y = y - 1, direction = Direction.UP };
                    if (step > 2) yield return this with { step = 0, y = y + 1, direction = Direction.DOWN };
                    break;
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
