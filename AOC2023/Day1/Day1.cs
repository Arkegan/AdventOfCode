using Xunit.Abstractions;

namespace AOC2023.Day1;

public class Day1 : Day
{
    public Day1(ITestOutputHelper output) : base(output)
    {
    }

    Dictionary<string, char> numbers = new()
    {
        { "one", '1' },
        { "two", '2' },
        { "three", '3'},
        { "four", '4' },
        { "five", '5' },
        { "six", '6' },
        { "seven", '7' },
        { "eight", '8' },
        { "nine", '9' },
    };

    public override string TestValue => "281";

    public override async Task<string> Run(StreamReader inputData)
    {
        var sum = 0;
        var l = "";

        while((l = inputData.ReadLine()) != null)
        {
            var first = (string?)null;
            var last = (string?)null;
            for(int i = 0; i < l.Length; i++)
            {
                first = SeekNumber(l.Substring(i), false);
                if (first != null)
                    break;
            }
            var reversed = new string(l.Reverse().ToArray());
            for(int i = 0; i < l.Length; i++)
            {
                last = SeekNumber(reversed.Substring(i), true);
                if (last != null)
                    break;
            }

            var res = int.Parse($"{first}{last}");
            sum += res;
        }

        return sum.ToString();
    }

    private string? SeekNumber(string line, bool reversed)
    {
        if (char.IsDigit(line.First()))
            return new string(new[] { line.First() });

        if (!reversed)
        {
            var possibilities = numbers.Keys.Where(k => k.Length <= line.Length && k.StartsWith(line.First()));
            foreach (var p in possibilities)
            {
                try
                {
                    var sub = line.Substring(0, p.Length);
                    if (sub == p)
                        return new string(new[] { numbers[sub] });
                }
                catch(Exception) 
                {
                    continue;
                }
            }
        }
        else
        {
            var possibilities = numbers.Keys
                .Select(k => new string(k.Reverse().ToArray()))
                .Where(k => k.Length <= line.Length && k.StartsWith(line.First()));
            foreach (var p in possibilities)
            {
                var sub = line.Substring(0, p.Length);
                if (sub == p)
                    return new string(new[] { numbers[new string(sub.Reverse().ToArray())] });
            }
        }

        return null;
    }
}

