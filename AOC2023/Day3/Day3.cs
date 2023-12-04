using System.Numerics;
using Xunit.Abstractions;

namespace AOC2023.Day3;

public class Day3 : Day
{
    public Day3(ITestOutputHelper output) : base(output)
    {
    }

    public override string TestValue => "467835";

    public override async Task<string> Run(StreamReader inputData)
    {
        var board = new Dictionary<Vector2, Part>();

        for(int y = 0; ; y++)
        {
            var line = inputData.ReadLine();
            if(line == null)
                break;

            for(int x = 0; x < line.Length; x++)
            {
                if (line[x] == '.')
                    continue;
                var p = new Part();
                p.XStart = x;
                p.Y = y;

                if (char.IsDigit(line[x]))
                {
                    var value = new string(line.Skip(x).TakeWhile(c => char.IsDigit(c)).ToArray());
                    for(int step = x; step < x+value.Length; step++)
                    {
                        board.Add(new Vector2(step, y), p);
                    }
                    x += value.Length -1;
                    p.XEnd = x;
                    p.Value = value;
                }
                else { 
                    p.XEnd = x;
                    p.Value = new string(new[] { line[x] });
                    board.Add(new Vector2(x, y), p);
                }
            }
        }

        var res = board.Values.Distinct()
            .Where(p => p.Value == "*");


        return res.Sum(p => p.GetNeighborPartsRatio(board)).ToString();
    }

    private class Part
    {
        public string Value { get; set; }
        public int XStart { get; set; }
        public int XEnd { get; set; }
        public int Y { get; set; }

        public bool IsPart => int.TryParse(Value, out _);

        public bool HasNeighborPart(Dictionary<Vector2, Part> board)
        {
            var part = board
                .Where(k => k.Key.Y >= Y-1 && k.Key.Y <= Y+1)
                .Where(k => k.Key.X >= XStart - 1 && k.Key.X <= XEnd + 1)
                .Where(k => !k.Value.IsPart)
                .FirstOrDefault().Value;

            return part != null;
        }


        public long GetNeighborPartsRatio(Dictionary<Vector2, Part> board)
        {
            var parts = board
                .Where(k => k.Key.Y >= Y - 1 && k.Key.Y <= Y + 1)
                .Where(k => k.Key.X >= XStart - 1 && k.Key.X <= XEnd + 1)
                .Where(k => k.Value.IsPart)
                .Select(k => k.Value)
                .Distinct()
                .ToList();

            if (parts.Count == 2)
                return int.Parse(parts[0].Value) * int.Parse(parts[1].Value);
            else return 0;
        }
    }
}

