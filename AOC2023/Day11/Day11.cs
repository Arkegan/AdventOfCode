using Xunit.Abstractions;

namespace AOC2023.Day11;

public class Day11 : Day
{
    public Day11(ITestOutputHelper output) : base(output)
    {
    }

    public override string TestValue => "1030";

    public override async Task<string> Run(StreamReader inputData)
    {
        var map = new Dictionary<VectorLong, Node>();
        var extendedMap = new Dictionary<VectorLong, Node>();
        var l = "";
        int y = 0;
        while((l = inputData.ReadLine()) != null)
        {
            int x = 0;
            foreach(var c in l)
            {
                var pos = new VectorLong(x, y);
                map.Add(pos, new Node(c, pos));
                x++;
            }
            y++;
        }
        var maxX = map.Keys.Max(k => k.X);
        var maxY = map.Keys.Max(k => k.Y);
        var extraRows = new List<long>();
        for(int cY = 0; cY <= maxY; cY++)
        {
            var row = map.Where(k => k.Key.Y == cY).Select(k => k.Value).ToList();
            if(row.All(n => n.Symbol == '.'))
                extraRows.Add(cY);
        }
        var extraColumns = new List<long>();
        for (int cX = 0; cX <= maxX; cX++)
        {
            var col = map.Where(k => k.Key.X == cX).Select(k => k.Value).ToList();
            if (col.All(n => n.Symbol == '.'))
                extraColumns.Add(cX);
        }

        var galaxyNode = map.Where(n => n.Value.Symbol == '#').Select(k =>
        {
            var g = k.Value;
            var expansion = 1000000 - 1;
            var newX = g.Position.X + (extraColumns.Count(c => c <= g.Position.X) * expansion);
            var newY = g.Position.Y + (extraRows.Count(c => c <= g.Position.Y) * expansion);

            return new Node('#', new VectorLong(newX, newY));
        }).ToList();



        var pairs = galaxyNode.SelectMany((g1, i) => galaxyNode.Skip(i + 1).Select(g2 => (g1, g2))).ToList();
        var sum = 0l;
        foreach(var pair in pairs)
        {
            sum += pair.g1.DistanceTo(pair.g2);
        }
        return sum.ToString();
    }

    private record VectorLong(long X, long Y) { }

    private class Node
    {
        public char Symbol { get; set; }
        public VectorLong Position { get; set; }
        public int Weight { get; set; } = 1;


        public float DistanceToTarget { get; set; } = -1;
        public float F => (DistanceToTarget != -1 && Cost != -1) ? DistanceToTarget + Cost : -1;
        public float Cost { get; set; } = 1;
        public Node? Parent { get; set; }

        public Node(char symbol, VectorLong position)
        {
            Symbol = symbol;
            Position = position;
        }

        public void Reset()
        {
            DistanceToTarget = -1;
            Cost = 1;
            Parent = null;
        }

        public long DistanceTo(Node other)
        {
            var diffX = Math.Abs(Position.X - other.Position.X);
            var diffY = Math.Abs(Position.Y - other.Position.Y);
            return diffX + diffY;
        }
    }
}
