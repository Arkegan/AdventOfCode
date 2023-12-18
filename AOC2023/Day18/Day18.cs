
using Xunit.Abstractions;

namespace AOC2023.Day18;

public class Day18 : Day
{
    public Day18(ITestOutputHelper output) : base(output)
    {
    }

    public override string TestValue => "952408144115";

    public override async Task<string> Run(StreamReader inputData)
    {
        var s = "";
        var currentPos = new Vector(0, 0);
        var vertices = new List<Vector>();
        var extraArea = 0m;

        while((s = inputData.ReadLine()) != null)
        {
            //var direction = (Direction)s.First();
            //var length = int.Parse(s.Split(' ', StringSplitOptions.RemoveEmptyEntries)[1]);
            
            var hexColor = s.Split(' ', StringSplitOptions.RemoveEmptyEntries)[2].Trim('(', ')').Replace("#", "");
            var direction = hexColor.Last() switch
            {
                '0' => Direction.RIGHT,
                '1' => Direction.DOWN,
                '2' => Direction.LEFT,
                '3' => Direction.UP,
                _ => throw new Exception()
            };
            var length = Convert.ToInt64(new string(hexColor.Take(5).ToArray()), 16);
            extraArea += (decimal)length / 2;
            var newPos = currentPos.Move(direction, length);
            vertices.Add(newPos);
            currentPos = newPos;
        }

        var a =(long)(ShoeLace(vertices) + extraArea + 1);
        return a.ToString();
    }

    public long ShoeLace(List<Vector> vertices)
    {
        var total = 0m;
        for(var i = 0; i < vertices.Count - 1; i++)
        {
            var prev = vertices[i];
            var next = vertices[i + 1];

            total += prev.x * next.y - next.x * prev.y;
        }
        return (long)Math.Abs(total / 2);
    }

    public record Vector(long x, long y)
    {
        public Vector Move(Direction direction, long length)
        {
            switch (direction)
            {
                case Direction.UP:
                    return this with { y = y - length };
                case Direction.DOWN:
                    return this with { y = y + length };
                case Direction.LEFT:
                    return this with { x = x - length };
                case Direction.RIGHT:
                    return this with { x = x + length };
            }
            throw new Exception();
        }

        public long GetDistance(Vector other)
        {
            return Math.Abs(x - other.x) + Math.Abs(y - other.y);
        }
    }

    public enum Direction : int
    {
        UP = 'U',
        DOWN = 'D',
        LEFT = 'L',
        RIGHT = 'R'
    }
}
