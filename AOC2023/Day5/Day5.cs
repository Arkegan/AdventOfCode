
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

        seeds = seeds.Chunk(2).SelectMany(s =>
        {
            var start = s[0];
            var end = s[1];
            return Enumerable.Range((int)start, (int)Math.Abs(end - start));
        }).Select(s => (long)s).ToList();



        var r = seeds.Select(s => ssMapper.Convert(s));
        r = r.Select(s => sfMapper.Convert(s));
        r = r.Select(s => fwMapper.Convert(s));
        r = r.Select(s => wlMapper.Convert(s));
        r = r.Select(s => ltMapper.Convert(s));
        r = r.Select(s => thMapper.Convert(s));
        r = r.Select(s => hlMapper.Convert(s));
            
        return r.Min().ToString();
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


        public class MapperInner
        {
            public long DestRangeStart { get; set; }
            public long SourceRangeStart { get; set; }
            public long RangeLength { get; set; }
        }
    }
}
