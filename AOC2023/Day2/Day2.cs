using Xunit.Abstractions;

namespace AOC2023.Day2;

public class Day2 : Day
{
    public Day2(ITestOutputHelper output) : base(output)
    {
    }

    public override string TestValue => "2286";

    public override async Task<string> Run(StreamReader inputData)
    {
        var games = new List<Game>();
        var l = "";
        while ((l = inputData.ReadLine()) != null)
        {
            var header = l.Split(':', StringSplitOptions.RemoveEmptyEntries)[0];
            var gameIndex = int.Parse(header.Substring(5));
            var data = l.Split(":", StringSplitOptions.RemoveEmptyEntries)[1].Split(";", StringSplitOptions.RemoveEmptyEntries);
            var states = new List<Game.State>();
            foreach(var d in data)
            {
                var colors = d.Split(",", StringSplitOptions.RemoveEmptyEntries);
                int red = 0;
                int green = 0;
                int blue = 0;
                foreach (var c in colors)
                {
                    int number = int.Parse(new string(c.Trim().TakeWhile(i => char.IsDigit(i)).ToArray()));
                    if(c.Contains("red"))
                        red = number;
                    if (c.Contains("green"))
                        green = number;
                    if (c.Contains("blue"))
                        blue = number;
                }
                states.Add(new Game.State(blue, green, red));
            }
            games.Add(new Game(gameIndex, states));
        }

        return games.Sum(g => g.PowerCount()).ToString();
    }

    private class Game
    {
        public int Id { get; }
        public List<State> States { get; }

        public Game(int id, List<State> states)
        {
            Id = id;
            States = states;
        }

        public bool IsPossible()
        {
            return States.All(s => s.Red <= 12) && 
                States.All(s => s.Green <= 13) &&
                States.All(s => s.Blue <= 14);
        }

        public long PowerCount()
        {
            var s = new State(
                States.Max(s => s.Blue),
                States.Max(s => s.Green),
                States.Max(s => s.Red)
            );

            return s.Red * s.Blue * s.Green;
        }

        public class State
        {
            public int Blue { get; }
            public int Green { get; }
            public int Red { get; }

            public State(int blue, int green, int red)
            {
                Blue = blue;
                Green = green;
                Red = red;
            }
        }
    }

}

