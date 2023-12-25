
using Xunit.Abstractions;

namespace AOC2023.Day25;

public class Day25 : Day
{
    public Day25(ITestOutputHelper output) : base(output)
    {
    }

    public override string TestValue => "54";


    public override async Task<string> Run(StreamReader inputData)
    {
        var s = "";
        var nodes = new List<string>();
        var connections = new List<Connection>();

        while((s = await inputData.ReadLineAsync()) != null)
        {
            var data = s.Split(':', StringSplitOptions.RemoveEmptyEntries);
            var name = data[0];
            var cons = data[1].Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (!nodes.Contains(name))
                nodes.Add(name);
            foreach(var c in cons)
            {
                if(!nodes.Contains(c))
                    nodes.Add(c);
                if(!connections.Contains(new Connection(name, c)) && !connections.Contains(new Connection(c, name)))
                    connections.Add(new(name, c));
            }
        }
        var rand = new Random();
        while (true)
        {
            var subsets = nodes.Select(n => new List<string>() { n }).ToList();
            while (subsets.Count > 2)
            {
                var i = rand.Next(connections.Count);

                var ss1 = subsets.First(s => s.Contains(connections[i].node1));
                var ss2 = subsets.First(s => s.Contains(connections[i].node2));

                if(ss1 != ss2)
                {
                    subsets.Remove(ss2);
                    ss1.AddRange(ss2);
                }
            }

            if(Cuts(subsets, connections) == 3) //Result found
            {
                return (subsets[0].Count * subsets[1].Count).ToString();
            }
        }
    }

    public int Cuts(List<List<string>> subsets, List<Connection> connections)
    {
        var res = 0;
        for(int i = 0; i < connections.Count; i++)
        {
            var ss1 = subsets.First(s => s.Contains(connections[i].node1));
            var ss2 = subsets.First(s => s.Contains(connections[i].node2));
            if (ss1 != ss2)
                res++;
        }
        return res;
    }

    public record Connection(string node1, string node2);
}
