using Xunit.Abstractions;

namespace AOC2023.Day15;

public class Day15 : Day
{
    public Day15(ITestOutputHelper output) : base(output)
    {
    }

    public override string TestValue => "145";
    private Dictionary<long, List<Lens>> _boxes = new();

    public override async Task<string> Run(StreamReader inputData)
    {
        var sum = 0l;
        foreach (var line in ReadInput(inputData))
        {
            var boxNumber = Hash(line.lens.label);
            if(line.op == '-')
            {
                if (!_boxes.ContainsKey(boxNumber))
                    continue;
                _boxes[boxNumber].RemoveAll(l => l.label == line.lens.label);
            }
            else
            {
                if (!_boxes.ContainsKey(boxNumber))
                    _boxes[boxNumber] = new List<Lens>();

                var existing = _boxes[boxNumber].FirstOrDefault(l => l.label == line.lens.label);
                if (existing != null)
                    _boxes[boxNumber][_boxes[boxNumber].IndexOf(existing)] = line.lens;
                else
                    _boxes[boxNumber].Add(line.lens);
            }
        }

        foreach(var box in _boxes)
        {
            var boxNumber = box.Key + 1;
            for(int i = 0; i < box.Value.Count; i++)
            {
                sum += boxNumber * (i + 1) * box.Value[i].strength;
            }
        }

        return sum.ToString();
    }


    public IEnumerable<Operation> ReadInput(StreamReader input)
    {
        var s = input.ReadToEnd();
        foreach(var i in s.Replace("\n", "").Split(','))
        {
            var label = new string(i.TakeWhile(c => c >= 'a' && c <= 'z').ToArray());
            var op = i.Substring(label.Length).Take(1).First();
            var fs = i.Length > label.Length + 1 ? int.Parse(i.Substring(label.Length + 1)) : 0;
            yield return new Operation(new(label, fs), op);
        }
    }

    public record Operation(Lens lens, char op);
    public record Lens(string label, int strength);

    private Dictionary<string, long> _cache = new();

    public long Hash(string s)
    {
        if(_cache.ContainsKey(s))
            return _cache[s];

        var current = 0l;
        foreach(var c in s)
        {
            current += c;
            current *= 17;
            current %= 256;
        }
        _cache[s] = current;
        return current;
    }
}
