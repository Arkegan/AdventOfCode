
using System.Diagnostics;
using Xunit.Abstractions;

namespace AOC2023.Day5;

public class Day5 : Day
{
    public Day5(ITestOutputHelper output) : base(output)
    {
    }

    public override string TestValue => "46";

    public override async Task<string> Run(StreamReader inputData)
    {
        var seeds = inputData.ReadLine().Split(' ', StringSplitOptions.RemoveEmptyEntries).Skip(1).Select(s => long.Parse(s)).ToList();
        inputData.ReadLine();
        inputData.ReadLine();
        var l = "";
        var lines = new List<string>();
        while((l = inputData.ReadLine()) != "")
        {
            lines.Add(l);
        }
        var ssMapper = new Mapper(MapperType.SeedToSoil, lines.ToList());
        lines.Clear();
        inputData.ReadLine();

        while ((l = inputData.ReadLine()) != "")
        {
            lines.Add(l);
        }
        var sfMapper = new Mapper(MapperType.SoilToFertilizer, lines.ToList());
        lines.Clear();
        inputData.ReadLine();

        while ((l = inputData.ReadLine()) != "")
        {
            lines.Add(l);
        }
        var fwMapper = new Mapper(MapperType.FertilizerToWater, lines.ToList());
        lines.Clear();
        inputData.ReadLine();

        while ((l = inputData.ReadLine()) != "")
        {
            lines.Add(l);
        }
        var wlMapper = new Mapper(MapperType.WaterToLight, lines.ToList());
        lines.Clear();
        inputData.ReadLine();

        while ((l = inputData.ReadLine()) != "")
        {
            lines.Add(l);
        }
        var ltMapper = new Mapper(MapperType.LightToTemperature, lines.ToList());
        lines.Clear();
        inputData.ReadLine();

        while ((l = inputData.ReadLine()) != "")
        {
            lines.Add(l);
        }
        var thMapper = new Mapper(MapperType.TemperatureToHumidity, lines.ToList());
        lines.Clear();
        inputData.ReadLine();

        while ((l = inputData.ReadLine()) != null)
        {
            lines.Add(l);
        }
        var hlMapper = new Mapper(MapperType.HumidityToLocation, lines.ToList());
        lines.Clear();
        inputData.ReadLine();

        var seedRange = seeds.Chunk(2).Select(s =>
        {
            var start = s[0];
            var range = s[1];
            return new SeedRange(start, start + range - 1);
        }).OrderBy(r => r.Start).ToList();

        var test = ssMapper.Convert(seedRange.First()).ToList();

        var r = seedRange.SelectMany(s => ssMapper.Convert(s)).ToList();
        r = r.SelectMany(s => sfMapper.Convert(s)).ToList();
        r = r.SelectMany(s => fwMapper.Convert(s)).ToList();
        r = r.SelectMany(s => wlMapper.Convert(s)).ToList();
        r = r.SelectMany(s => ltMapper.Convert(s)).ToList();
        r = r.SelectMany(s => thMapper.Convert(s)).ToList();
        r = r.SelectMany(s => hlMapper.Convert(s)).ToList();
            
        return r.Min(r => r.Start).ToString();
    }

    private enum MapperType
    {
        SeedToSoil,
        SoilToFertilizer,
        FertilizerToWater,
        WaterToLight,
        LightToTemperature,
        TemperatureToHumidity,
        HumidityToLocation
    }

    private class SeedRange
    {
        public SeedRange(long start, long end)
        {
            Start = start;
            End = end;

            if(Start > End)
                Debugger.Break();
        }

        public long Start { get; set; }
        public long End { get; set; }
    }

    private class Mapper
    {
        public MapperType Type { get; set; }
        public List<MapperInner> Inner { get; set; } = new();

        public Mapper(MapperType type, List<string> data)
        {
            Type = type;
            foreach(var item in data)
            {
                var s = item.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(a => long.Parse(a)).ToList();
                Inner.Add(new MapperInner { SourceRangeStart = s[0], DestRangeStart = s[1], RangeLength = s[2] });
            }

            Inner = Inner.OrderBy(i => i.DestRangeStart).ToList();
        }

        public long Convert(long n)
        {
            foreach(var inner in Inner)
            {
                if(n >= inner.DestRangeStart && n < inner.DestRangeStart + inner.RangeLength)
                {
                    var delta = n - inner.DestRangeStart;
                    return inner.SourceRangeStart + delta;
                }
            }
            return n;
        }



        public IEnumerable<SeedRange> Convert(SeedRange n)
        {
            var start = n.Start;
            foreach (var inner in Inner)
            {
                if (start > inner.DestRangeEnd)
                    continue;
                if (n.End < inner.DestRangeStart)
                    break;

                if (inner.DestRangeStart > start)
                {
                    yield return new SeedRange(start, inner.DestRangeStart - 1);
                    start = inner.DestRangeStart;
                }

                var end = Math.Min(n.End, inner.DestRangeEnd);
                yield return new SeedRange(inner.GetNewPos(start), inner.GetNewPos(end));

                if (end == n.End)
                    yield break;
                start = end + 1;
                
            }

            yield return new SeedRange(start, n.End);
        }

        public class MapperInner
        {
            public long DestRangeStart { get; set; }
            public long DestRangeEnd => DestRangeStart + RangeLength - 1;
            public long SourceRangeStart { get; set; }
            public long RangeLength { get; set; }

            public long GetNewPos(long n)
            {
                if (n >= DestRangeStart && n < DestRangeStart + RangeLength)
                {
                    var delta = n - DestRangeStart;
                    return SourceRangeStart + delta;
                }

                return n;
            }
        }
    }
}
