
using System.Text.RegularExpressions;
using Xunit.Abstractions;

namespace AOC2023.Day8;

public class Day8 : Day
{
    public Day8(ITestOutputHelper output) : base(output)
    {
    }

    public override string TestValue => "6";

    public override async Task<string> Run(StreamReader inputData)
    {
        var l = "";
        var regex = new Regex("([A-Z0-9]*) = [(]([A-Z0-9]*), ([A-Z0-9]*)[)]");
        var nodes = new Dictionary<string, Node>();

        var instruction = inputData.ReadLine();
        inputData.ReadLine();

        while((l = inputData.ReadLine()) != null)
        {
            var groups = regex.Match(l).Groups;
            var name = groups[1].Value;
            var node = nodes.ContainsKey(name) ? nodes[name] : new Node() { Name = name };
            if(!nodes.ContainsKey(name))
                nodes.Add(name, node);
            var left = groups[2].Value;
            var right = groups[3].Value;

            if (nodes.ContainsKey(left))
                node.Left = nodes[left];
            else
            {
                node.Left = new Node { Name = left };
                nodes.Add(left, node.Left);
            }

            if (nodes.ContainsKey(right))
                node.Right = nodes[right];
            else
            {
                node.Right = new Node { Name = right };
                nodes.Add(right, node.Right);
            }
        }

        var minPath = new Dictionary<Node, List<int>>();
        var currentNodes = nodes.Where(n => n.Key.EndsWith('A')).Select(k => k.Value).ToList();
        foreach(var node in currentNodes)
        {
            minPath.Add(node, new List<int>());
            var visited = new List<Visited>();
            var count = 0;
            var currentNode = node;
            foreach (var c in GetInstructions(instruction))
            {
                currentNode = currentNode.Move(c);
                count++;
                if (currentNode.Name.EndsWith('Z'))
                {
                    minPath[node].Add(count);
                    var v = new Visited(currentNode, c);
                    if (visited.Contains(v))
                        break;
                    else visited.Add(v);
                }
            }
        }

        foreach(var d in minPath)
        {
            var cycle = d.Value[1] - d.Value[0];
            var delta = d.Value[0];
            d.Key.Cycle = cycle;
            d.Key.Delta = delta;
        }

        var finalNodes = minPath.Select(m => m.Key).ToList();
        var res = finalNodes.Select(n => n.Cycle).Aggregate((n1, n2) => lcm(n1, n2));
        return res.ToString();
    }
    static long gcf(long a, long b)
    {
        while (b != 0)
        {
            long temp = b;
            b = a % b;
            a = temp;
        }
        return a;
    }

    static long lcm(long a, long b)
    {
        return (a / gcf(a, b)) * b;
    }

    public record Visited(Node Node, char Direction);

    public IEnumerable<char> GetInstructions(string instructions)
    {
        while (true)
        {
            foreach (var c in instructions)
                yield return c;
        }
    }

    public class Node
    {
        public string Name { get; set; }
        public Node? Left { get; set; }
        public Node? Right { get; set; }

        public long Delta { get; set; }
        public long Cycle { get; set; }

        public Node Move(char direction)
        {
            return direction == 'R' ? Right : Left;
        }

        public long GetPosAtCycle(int i)
        {
            return 0;
        }
    }
}
