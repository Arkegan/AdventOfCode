
using System.Text;
using Xunit.Abstractions;

namespace AOC2023.Day14;

public class Day14 : Day
{
    public Day14(ITestOutputHelper output) : base(output)
    {
    }

    public override string TestValue => "64";

    public override async Task<string> Run(StreamReader inputData)
    {
        var l = "";
        var data = new List<string>();
        while((l = await inputData.ReadLineAsync()) != null)
        {
            data.Add(l);
        }

        var map = new Map(data);
        var cache = new List<Node[]>();

        var cycle = 0l;
        while (true)
        {
            map = map.MoveNorth();
            map = map.MoveWest();
            map = map.MoveSouth();
            map = map.MoveEast();
            cycle++;
            var vectors = map.Nodes.Values.OrderBy(x => x.Position.X).ThenBy(y => y.Position.Y).ToArray();
            if (cache.Any(v => v.SequenceEqual(vectors)))
            {
                var orgVector = cache.First(v => v.SequenceEqual(vectors));
                var orgIdx = cache.IndexOf(orgVector);

                var cycleLength = cache.Count - orgIdx;
                var cycleLeft = (1000000000 - cycle) % cycleLength;

                map = new Map(cache[orgIdx + (int)cycleLeft].ToDictionary(n => n.Position, n => n), map.Height, map.Width);
                return map.getLoad().ToString();
            }
            else
            {
                cache.Add(vectors);
            }
        }

        return map.getLoad().ToString();
    }

    public class Map
    {
        public Dictionary<Vector, Node> Nodes { get; } = new();
        public long Height { get; }
        public long Width { get; }

        public Map(List<string> data)
        {
            Height = data.Count;
            Width = data.First().Length;
            int y = 0;
            foreach(var l in data)
            {
                for(var x = 0; x < l.Length; x++)
                {
                    var pos = new Vector(x, y);
                    if (l[x] == '#')
                    {
                        Nodes.Add(pos, new Node(pos, false));
                    }
                    else if (l[x] == 'O')
                    {
                        Nodes.Add(pos, new Node(pos, true));
                    }
                }
                y++;
            }
        }

        public Map(Dictionary<Vector, Node> clone, long height, long width)
        {
            Nodes = clone;
            Height = height;
            Width = width;
        }

        public Map MoveNorth()
        {
            var newMap = new Dictionary<Vector, Node>();
            foreach(var n in Nodes.Values.OrderBy(n => n.Position.Y))
            {
                if (n.Moveable)
                {
                    var upY = newMap.Where(node => node.Key.X == n.Position.X)
                        .DefaultIfEmpty(new KeyValuePair<Vector, Node>(new Vector(n.Position.X, -1), new Node(new Vector(n.Position.X, -1), false)))
                        .Max(n => n.Key.Y) + 1;
                    var upPos = new Vector(n.Position.X, upY);
                    newMap.Add(upPos, new Node(upPos, true));
                }
                else
                {
                    newMap.Add(n.Position, n);
                }
            }

            return new Map(newMap, Height, Width);
        }
        public Map MoveWest()
        {
            var newMap = new Dictionary<Vector, Node>();
            foreach (var n in Nodes.Values.OrderBy(n => n.Position.X))
            {
                if (n.Moveable)
                {
                    var leftX = newMap.Where(node => node.Key.Y == n.Position.Y)
                        .DefaultIfEmpty(new KeyValuePair<Vector, Node>(new Vector(-1, n.Position.Y), new Node(new Vector(-1, n.Position.Y), false)))
                        .Max(n => n.Key.X) + 1;
                    var leftPos = new Vector(leftX, n.Position.Y);
                    newMap.Add(leftPos, new Node(leftPos, true));
                }
                else
                {
                    newMap.Add(n.Position, n);
                }
            }

            return new Map(newMap, Height, Width);
        }

        public Map MoveSouth()
        {
            var newMap = new Dictionary<Vector, Node>();
            foreach (var n in Nodes.Values.OrderByDescending(n => n.Position.Y))
            {
                if (n.Moveable)
                {
                    var downY = newMap.Where(node => node.Key.X == n.Position.X)
                        .DefaultIfEmpty(new KeyValuePair<Vector, Node>(new Vector(n.Position.X, Height), new Node(new Vector(n.Position.X, Height), false)))
                        .Min(n => n.Key.Y) - 1;
                    var downPos = new Vector(n.Position.X, downY);
                    newMap.Add(downPos, new Node(downPos, true));
                }
                else
                {
                    newMap.Add(n.Position, n);
                }
            }

            return new Map(newMap, Height, Width);
        }
        public Map MoveEast()
        {
            var newMap = new Dictionary<Vector, Node>();
            foreach (var n in Nodes.Values.OrderByDescending(n => n.Position.X))
            {
                if (n.Moveable)
                {
                    var rightX = newMap.Where(node => node.Key.Y == n.Position.Y)
                        .DefaultIfEmpty(new KeyValuePair<Vector, Node>(new Vector(Width, n.Position.Y), new Node(new Vector(Width, n.Position.Y), false)))
                        .Min(n => n.Key.X) - 1;
                    var rightPos = new Vector(rightX, n.Position.Y);
                    newMap.Add(rightPos, new Node(rightPos, true));
                }
                else
                {
                    newMap.Add(n.Position, n);
                }
            }

            return new Map(newMap, Height, Width);
        }

        public long getLoad()
        {
            var sum = 0l;
            foreach(var node in Nodes.Values.Where(n => n.Moveable))
            {
                sum += Height - node.Position.Y; 
            }
            return sum;
        }
    }

    public record Node
    {
        public Node(Vector position, bool moveable)
        {
            Position = position;
            Moveable = moveable;
        }

        public bool Moveable { get; set; }
        public Vector Position { get; set; }
    }

    public record Vector(long X, long Y);

    public static void PrintRocks(Rock[,] rocks)
    {
        int h = rocks.GetLength(0);
        int w = rocks.GetLength(1);
        Console.WriteLine("====== Rendered Rocks ======");
        Console.WriteLine();
        for (int i = 0; i < h; i++)
        {
            StringBuilder s = new StringBuilder();
            for (int j = 0; j < w; j++)
            {
                s.Append(rocks[i, j].ToString());
            }
            Console.WriteLine(s.ToString());
        }
        Console.WriteLine();
        Console.WriteLine("====== End Rendered Rocks ======");
    }
    public enum RockType
    {
        Ground = '.',
        Square = '#',
        Round = 'O'
    };
    public class Rock
    {
        public RockType Type { get; set; }

        public int X { get; set; }
        public int Y { get; set; }


        public Rock(int y, int x, char c)
        {
            X = x;
            Y = y;
            if (c == '.') Type = RockType.Ground;
            else if (c == '#') Type = RockType.Square;
            else Type = RockType.Round;
        }

        public bool Square => Type == RockType.Square;
        public bool Round => Type == RockType.Round;
        public bool Ground => Type == RockType.Ground;
        public override string ToString()
        {
            return $"{(char)Type}";
        }
    }
    public static long HashArray(Rock[,] arr)
    {
        int h = arr.GetLength(0);
        int w = arr.GetLength(1);
        long hash = 17;
        for (int i = 0; i < h; i++)
        {
            for (int j = 0; j < w; j++)
            {
                hash = hash * 31 + (int)arr[i, j].Type;
            }
        }

        return hash;
    }
    public static void Part2(string[] lines)
    {
        int H = lines.Length;
        int W = lines[0].Length;
        Rock[,] rocks = new Rock[H, W];
        for (int y = 0; y < H; y++)
        {
            for (int x = 0; x < W; x++)
            {
                rocks[y, x] = new Rock(y, x, lines[y][x]);
            }
        }
        Dictionary<long, Rock[,]> dict = new Dictionary<long, Rock[,]>();

        PrintRocks(rocks);

        long endHash = 0;
        long cnt = 0;

        long numIters = 1_000_000_000;
        for (int i = 0; i < numIters; i++)
        {
            long hash = HashArray(rocks);

            if (i % 10_000 == 0)
            {
                Console.WriteLine(i.ToString("N0"));
                Console.WriteLine(dict.Count);
            }
            if (dict.ContainsKey(hash))
            {
                cnt = i;
                endHash = hash;
                break;
            }
            var RockCopy = (Rock[,])rocks.Clone();
            // North
            for (int y = 0; y < H; y++)
            {
                for (int x = 0; x < W; x++)
                {
                    Rock r = rocks[y, x];

                    if (r.Round)
                    {
                        int k = y - 1;

                        while (k >= 0 && rocks[k, x].Ground)
                        {
                            var save = rocks[k, x];
                            rocks[k, x] = r;
                            rocks[k + 1, x] = save;
                            k--;
                        }
                    }
                }
            }

            // West
            for (int x = 0; x < W; x++)
            {
                for (int y = 0; y < H; y++)
                {
                    Rock r = rocks[y, x];

                    if (r.Round)
                    {
                        int k = x - 1;

                        while (k >= 0 && rocks[y, k].Ground)
                        {
                            var save = rocks[y, k];
                            rocks[y, k] = r;
                            rocks[y, k + 1] = save;
                            k--;
                        }
                    }
                }
            }

            // South
            for (int y = H - 1; y >= 0; y--)
            {
                for (int x = 0; x < W; x++)
                {
                    Rock r = rocks[y, x];

                    if (r.Round)
                    {
                        int k = y + 1;

                        while (k < rocks.GetLength(0) && rocks[k, x].Ground)
                        {
                            var save = rocks[k, x];
                            rocks[k, x] = r;
                            rocks[k - 1, x] = save;
                            k++;
                        }
                    }
                }
            }

            // East
            for (int x = W - 1; x >= 0; x--)
            {
                for (int y = 0; y < H; y++)
                {
                    Rock r = rocks[y, x];

                    if (r.Round)
                    {
                        int k = x + 1;

                        while (k < rocks.GetLength(1) && rocks[y, k].Ground)
                        {
                            var save = rocks[y, k];
                            rocks[y, k] = r;
                            rocks[y, k - 1] = save;
                            k++;
                        }
                    }
                }
            }

            dict[HashArray(RockCopy)] = (Rock[,])rocks.Clone();
        }




        List<long> Loop = new List<long>();

        while (!Loop.Contains(endHash))
        {
            Loop.Add(endHash);
            endHash = HashArray(dict[endHash]);
        }


        // Off-by-one creeped in here. Go figure
        long toGo = numIters - cnt - 1;
        long endIdx = toGo % Loop.Count;


        long final = Loop[(int)endIdx];

        var finalRocks = dict[final];

        PrintRocks(finalRocks);


        int sum = 0;
        for (int y = 0; y < H; y++)
        {
            for (int x = 0; x < W; x++)
            {
                Rock r = finalRocks[y, x];

                if (r.Round)
                {
                    int rowstrength = H - y;

                    sum += rowstrength;
                }
            }
        }


        Console.WriteLine(sum);

    }
}
