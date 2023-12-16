
using Xunit.Abstractions;

namespace AOC2023.Day6;

public class Day6 : Day
{
    public Day6(ITestOutputHelper output) : base(output)
    {
    }

    public override string TestValue => "71503";

    public override async Task<string> Run(StreamReader inputData)
    {
        var l1 = inputData.ReadLine().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var l2 = inputData.ReadLine().Split(' ', StringSplitOptions.RemoveEmptyEntries);

        var race = new Race(
            long.Parse(string.Join(string.Empty, l1.Skip(1).ToArray()).Replace(" ", "")),
            long.Parse(string.Join(string.Empty, l2.Skip(1).ToArray()).Replace(" ", "")));

        return race.WinningPressTime().Count().ToString();
    }

    private class Race
    {
        public long Time { get; }

        public long Distance { get; }

        public Race(long time, long distance)
        {
            Time = time;
            Distance = distance;
        }

        public IEnumerable<long> WinningPressTime()
        {
            for(int i = 0; i < Time; i++)
            {
                var timeLeft = Time - i;
                var totalDistance = i * timeLeft;
                if (totalDistance > Distance)
                    yield return i;
            }
        }
    }
}
