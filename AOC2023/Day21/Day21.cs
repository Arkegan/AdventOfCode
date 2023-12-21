
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace AOC2023.Day21;

public class Day21 : Day
{
    public Day21(ITestOutputHelper output) : base(output)
    {
    }

    public override string TestValue => "16733044";

    public override async Task<string> Run(StreamReader inputData)
    {
        var l = "";
        var y = 0l;
        var map = new Dictionary<Position, bool>();
        var currentPosition = new List<Position>();
        while((l = await inputData.ReadLineAsync()) != null)
        {
            var x = 0l;
            foreach(var ch in l)
            {
                map.Add(new(x, y), ch != '#');
                if (ch == 'S')
                    currentPosition.Add(new(x, y));
                x++;
            }
            y++;
        }
        var gridSize = map.Max(m => m.Key.x) + 1;

        //S is straight in the middle, for test and input

        var max = 0l;
        var targetStep = 26501365;
        var visitedOdd = new HashSet<Position>();
        var visitedEven = new HashSet<Position>();
        var visited = new HashSet<Position>();
        var nbMaps = targetStep / gridSize;
        var extraMaps = targetStep % gridSize;

        var growth = new List<long>();

        
        var measurement = new Dictionary<long, long>()
        {
            { extraMaps, 0 }, //walking this much counts the extra fill after we filled each map
            { gridSize + extraMaps, 0 }, //filled a map
            { 2 * gridSize + extraMaps, 0 }, //And filled again
            { 3 * gridSize + extraMaps, 0 }, //And filled again
            { 4 * gridSize + extraMaps, 0 }, //And filled again
            { 5 * gridSize + extraMaps, 0 }, //And filled again
            { 6 * gridSize + extraMaps, 0 }, //And filled again
            { 7 * gridSize + extraMaps, 0 }, //And filled again
            { 8 * gridSize + extraMaps, 0 }, //And filled again
            { 9 * gridSize + extraMaps, 0 }, //And filled again
            { 10 * gridSize + extraMaps, 0 }, //And filled again
        };

        /*
        measurement = Enumerable.Range(1, 300).ToDictionary(e => (long)e, e => 0l);
        measurement = measurement.Where(m => m.Key %2 == 1).ToDictionary(k => k.Key, k => k.Value);*/

        for (int i = 1; i <= measurement.Last().Key; i++)
        {
            var v = i % 2 == 0 ? visitedEven : visitedOdd;
            var next = currentPosition.SelectMany(c => c.Move(map, gridSize, v)).Distinct().ToList();
            foreach (var n in next)
            {
                v.Add(n);
                visited.Add(n);
            }
            max = v.Count > max ? v.Count : max;
            currentPosition = next;

            if(measurement.ContainsKey(i))
                measurement[i] = max;
            if (measurement.Values.All(v => v != 0))
                break;
            /*
            if (i % 55 == 0) //target is 5*11*481843
            {
                measurement.Add(i, max);
            }*/
        }

        var target = 26501365l;
        //each point is gridSize apart, so take target = target/gridSize + target%gridSize

        var r = 26501365 % (gridSize * 2);
        var r2 = 26501365 / (gridSize * 2);
        var res1 = 14494l * (5 * 5) - 14311l * 5 + 3542l;
        var res2 = 14494l * (5 * 5) + 14677l * 5 + 3725l;


        //var res = 14494l * (202300l * 202300l) + 183 * 202300l + 10;
        //var res = 57976l * (101150l * 101150l) - 86598l * 101150l + 32347l;
        //var res = 14494l * (202300l * 202300l) - 14311l * 202300l + 3542l;
        var res = 14494l * (202300l* 202300l) + 14677l * 202300l + 3725l;

        //consider 0 to be half map
        Output.WriteLine(measurement.Select((m, i) => $"{{{i},{m.Value}}}").Aggregate((s1, s2) => s1 + "," + s2));
        return res.ToString();
    }

    public bool FullMap(long gridSize, int growth, HashSet<Position> visited, Dictionary<Position, bool> map)
    {
        var toVisit = map.Keys.Where(k => map[k]).ToHashSet();
        return toVisit.All(toVisit => visited.Contains(toVisit));
    }

    [Theory]
    [InlineData(-1, 1, 99, 1)]
    [InlineData(-50, 50, 50, 50)]
    [InlineData(99, 99, 99, 99)]
    [InlineData(1, 1, 1, 1)]
    [InlineData(-1, -1, 99, 99)]
    [InlineData(-101, -101, 99, 99)]
    [InlineData(0, -1, 0, 99)]
    public void TestNormalize(long x, long y, long expectedX, long expectedY)
    {
        var gs = 100;
        Assert.Equal(new Position(expectedX, expectedY), new Position(x, y).Normalize(gs));
    }

    public record Position(long x, long y)
    {
        public Position Normalize(long gridSize)
        {
            var newX = (x % gridSize + gridSize) % gridSize;
            var newY = (y % gridSize + gridSize) % gridSize;
            return new(newX, newY);
        }

        public IEnumerable<Position> Move(Dictionary<Position, bool> map, long gridSize, HashSet<Position> visited)
        {
            var up = this with { y = y - 1 };
            var down = this with { y = y + 1 };
            var left = this with { x = x - 1 };
            var right = this with { x = x + 1 };

            if (map[up.Normalize(gridSize)] && !visited.Contains(up))
                yield return up;
            if (map[down.Normalize(gridSize)] && !visited.Contains(down))
                yield return down;
            if (map[left.Normalize(gridSize)] && !visited.Contains(left))
                yield return left;
            if (map[right.Normalize(gridSize)] && !visited.Contains(right))
                yield return right;
        }
    }
}
