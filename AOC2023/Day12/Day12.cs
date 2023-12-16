using Xunit.Abstractions;

namespace AOC2023.Day12;

public class Day12 : Day
{
    public Day12(ITestOutputHelper output) : base(output)
    {
    }

    public override string TestValue => "525152";

    public override async Task<string> Run(StreamReader inputData)
    {
        var l = "";
        var counter = 0;
        var sum = 0l;
        while ((l = inputData.ReadLine()) != null)
        {
            var digitsString = new string(l.SkipWhile(c => !char.IsDigit(c)).ToArray());
            var digits = Enumerable.Repeat(digitsString.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => int.Parse(s)).ToArray(), 5).SelectMany(c => c).ToArray();

            var newL = new string(Enumerable.Repeat(l.TakeWhile(c => c != ' '), 5).Aggregate((l1, l2) => Enumerable.Concat(Enumerable.Concat(l1, new[] { '?' }), l2)).ToArray());

            var springGroup = newL.Split('.', StringSplitOptions.RemoveEmptyEntries);
            var replaceable = newL.Select((c, i) => c == '?' ? i : -1).Where(i => i != -1).ToArray();
            var staticSpringCount = newL.Where(c => c == '#').Count();

            var toAdd = digits.Sum() - staticSpringCount;

            sum += CountArrangements(new(), newL, digits, 0, 0, 0);
        }

        return sum.ToString();
    }

    public long CountArrangements(Dictionary<Tuple<int, int, int>, long> cache, string line, int[] digits, int position, int totalSequences, int currentSequenceLength)
    {
        var key = new Tuple<int, int, int>(position, totalSequences, currentSequenceLength);
        if (cache.ContainsKey(key)) //cache hit
            return cache[key];

        if (position == line.Length) //If we are at the end, either we found all matches, and we are not in one OR we foudn almost all the match and we are in one of lenght equal to the last suite
        {
            return (totalSequences == digits.Length && currentSequenceLength == 0) || 
                (totalSequences == digits.Length - 1 && digits[totalSequences] == currentSequenceLength) 
                ? 1 : 0;
        }
        long total = 0;
        var c = line[position];
        if ((c == '.' || c == '?') && currentSequenceLength == 0) //We are not in a sequence, and we consider ? as a dot
        {
            total += CountArrangements(cache, line, digits, position + 1, totalSequences, 0);
        }
        if ((c == '.' || c == '?') && currentSequenceLength > 0 && totalSequences < digits.Length && digits[totalSequences] == currentSequenceLength) //We were in a sequence, but we consider ? as a dot and thus end it
        {
            total += CountArrangements(cache, line, digits, position + 1, totalSequences + 1, 0);
        }
        if (c == '#' || c == '?') //We are in a sequence (or starting one)
        {
            total += CountArrangements(cache, line, digits, position + 1, totalSequences, currentSequenceLength + 1);
        }
        cache[key] = total;
        return total;
    }
}
