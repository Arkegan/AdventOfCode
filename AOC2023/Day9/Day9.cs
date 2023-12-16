
using Xunit.Abstractions;

namespace AOC2023.Day9;

public class Day9 : Day
{
    public Day9(ITestOutputHelper output) : base(output)
    {
    }

    public override string TestValue => "2";

    public override async Task<string> Run(StreamReader inputData)
    {
        var list = new List<List<long>>();
        var s = "";
        while((s = inputData.ReadLine()) != null)
        {
            list.Add(s.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(i => long.Parse(i)).ToList());
        }
        foreach(var l in list)
        {
            var details = new List<List<long>>();
            details.Add(l);
            var newList = l.ToList();
            while (!newList.All(l => l == 0))
            {
                newList = GetDiffList(newList).ToList();
                details.Add(newList);
            }
            details.Reverse();
            var valueToAdd = 0L;
            foreach (var detailList in details)
            {
                detailList.Insert(0, detailList.First() + valueToAdd);
                var nextBottomValue = detailList.First();
                valueToAdd = -nextBottomValue;
            }
            details.Reverse();
            l.Add(details.First().First());
        }

        return list.Select(r => r.First()).Sum().ToString();
    }

    public IEnumerable<long> GetDiffList(List<long> list)
    {
        var current = list.First();
        foreach(var l in list.Skip(1))
        {
            yield return l - current;
            current = l;
        }
    }
}
