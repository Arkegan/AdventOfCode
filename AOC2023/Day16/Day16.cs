
using Xunit.Abstractions;

namespace AOC2023.Day16;

public class Day16 : Day
{
    public Day16(ITestOutputHelper output) : base(output)
    {
    }

    public override string TestValue => "51";

    public override async Task<string> Run(StreamReader inputData)
    {
        var map = new Dictionary<Vector, char>();
        var s = "";
        var y = 0;
        while((s = inputData.ReadLine()) != null)
        {
            for(var x = 0; x < s.Length; x++)
            {
                var c = s[x];
                map.Add(new(x, y), c);
                
            }
            y++;
        }


        var maxEnergized = 0;
        for(int column = 0; column < map.Keys.Max(k => k.x); column++)
        {
            var minY = 0;
            var maxY = map.Keys.Max(k => k.y);

            var res = VisitAll(map, new(column, minY, Direction.DOWN)).DistinctBy(v => new Vector(v.x, v.y)).Count();
            if(res > maxEnergized)
                maxEnergized = res;
            res = VisitAll(map, new(column, maxY, Direction.UP)).DistinctBy(v => new Vector(v.x, v.y)).Count();
            if (res > maxEnergized)
                maxEnergized = res;
        }


        for (int row = 0; row < map.Keys.Max(k => k.y); row++)
        {
            var minX = 0;
            var maxX = map.Keys.Max(k => k.x);

            var res = VisitAll(map, new(minX, row, Direction.DOWN)).DistinctBy(v => new Vector(v.x, v.y)).Count();
            if (res > maxEnergized)
                maxEnergized = res;
            res = VisitAll(map, new(maxX, row, Direction.UP)).DistinctBy(v => new Vector(v.x, v.y)).Count();
            if (res > maxEnergized)
                maxEnergized = res;
        }

        return maxEnergized.ToString();
    }

    public HashSet<VisitedVector> VisitAll(Dictionary<Vector, char> map, VisitedVector currentPos)
    {
        var queue = new Queue<VisitedVector>();
        var visited = new HashSet<VisitedVector>();
        queue.Enqueue(currentPos);

        while(queue.Any())
        {
            var current = queue.Dequeue();

            if (visited.Contains(current))
                continue;
            if (!map.ContainsKey(new(current.x, current.y)))
                continue;

            visited.Add(current);

            if (map[new(current.x, current.y)] == '/')
            {
                var nextDirection = current.direction switch
                {
                    Direction.UP => Direction.RIGHT,
                    Direction.DOWN => Direction.LEFT,
                    Direction.LEFT => Direction.DOWN,
                    Direction.RIGHT => Direction.UP,
                };
                queue.Enqueue(current.Move(nextDirection));
            }
            else if (map[new(current.x, current.y)] == '\\')
            {
                var nextDirection = current.direction switch
                {
                    Direction.UP => Direction.LEFT,
                    Direction.DOWN => Direction.RIGHT,
                    Direction.LEFT => Direction.UP,
                    Direction.RIGHT => Direction.DOWN,
                };
                queue.Enqueue(current.Move(nextDirection));
            }
            else if (map[new(current.x, current.y)] == '|')
            {
                if (new[] { Direction.LEFT, Direction.RIGHT }.Contains(current.direction))
                {
                    queue.Enqueue(current.Move(Direction.UP));
                    queue.Enqueue(current.Move(Direction.DOWN));
                }
                else
                {
                    queue.Enqueue(current.Move());
                }
            }
            else if (map[new(current.x, current.y)] == '-')
            {
                if (new[] { Direction.UP, Direction.DOWN }.Contains(current.direction))
                {
                    queue.Enqueue(current.Move(Direction.LEFT));
                    queue.Enqueue(current.Move(Direction.RIGHT));
                }
                else
                {
                    queue.Enqueue(current.Move());
                }
            }
            else
            {
                queue.Enqueue(current.Move());
            }
        }
        return visited;
    }

    public enum Direction
    {
        UP,
        DOWN,
        LEFT, 
        RIGHT
    }

    public record Vector(long x, long y);
    public record VisitedVector(long x, long y, Direction direction) : Vector(x, y)
    {
        public VisitedVector Move(Direction newDirection)
        {
            return newDirection switch
            {
                Direction.UP => new VisitedVector(x, y - 1, newDirection),
                Direction.DOWN => new VisitedVector(x, y + 1, newDirection),
                Direction.LEFT => new VisitedVector(x - 1, y, newDirection),
                Direction.RIGHT => new VisitedVector(x + 1, y, newDirection),
            };
        }


        public VisitedVector Move()
        {
            return direction switch
            {
                Direction.UP => new VisitedVector(x, y - 1, direction),
                Direction.DOWN => new VisitedVector(x, y + 1, direction),
                Direction.LEFT => new VisitedVector(x - 1, y, direction),
                Direction.RIGHT => new VisitedVector(x + 1, y, direction),
            };
        }
    }
}
