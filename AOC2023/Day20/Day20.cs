
using System;
using System.Diagnostics;
using Xunit.Abstractions;

namespace AOC2023.Day20;

public class Day20 : Day
{
    public Day20(ITestOutputHelper output) : base(output)
    {
    }

    public override string TestValue => "32000000";

    public override async Task<string> Run(StreamReader inputData)
    {
        var l = "";
        var modules = new Dictionary<string, Module>();
        while((l = inputData.ReadLine()) != null)
        {
            var module = new Module(l);
            modules.Add(module.Name, module);
        }
        foreach (var m in modules)
            m.Value.InitState(modules);

        var modulesToBeOn = modules
            .Where(m => m.Value.Links.Contains("rx")) //hp -> hp need send low pulse, thus need all links to be high
            .SelectMany(m => modules.Where(mo => mo.Value.Links.Contains(m.Key)).Select(mo => mo.Value)) //sr, sn, rf, vq needs to be high
            .ToList();

        var cycles = modulesToBeOn.ToDictionary(m => m, _ => 0l);


        var sumLow = 0l;
        var sumHigh = 0l;
        var buttonPress = 0l;
        var found = false;
        while(!found)
        {
            var broadCaster = modules["broadcaster"];
            var next = broadCaster.Toggle(null, false);
            buttonPress++;
            sumLow++;
            while (next.Any() && !found)
            {
                var nextNext = new List<Signal>();
                foreach(var n in next)
                {
                    if (n.signal) sumHigh++;
                    else sumLow++;

                    if(!modules.ContainsKey(n.receiver)) 
                        continue;

                    if (modulesToBeOn.Any(m => m.Name == n.receiver) && !n.signal)
                    {
                        var endMo = modulesToBeOn.First(m => m.Name == n.receiver);
                        if (cycles[endMo] == 0)
                            cycles[endMo] = buttonPress;
                    }

                    found = cycles.All(c => c.Value != 0);

                    var m = modules[n.receiver];
                    nextNext.AddRange(m.Toggle(n.sender, n.signal));
                }
                next = nextNext;
            }
        }

        var res = cycles.Select(c => c.Value).Aggregate(lcm);

        return (res).ToString();
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

    public record Signal(string sender, string receiver, bool signal);

    public class Module
    {
        public char? ModuleType { get; }
        public string Name { get; }
        public List<string> Links { get; }

        public bool State { get; set; } = false; //false = low, true = high
        public Dictionary<string, bool> InvState { get; private set; }

        public Module(string s)
        {
            if (s.StartsWith("broadcaster"))
            {
                ModuleType = null;
                Name = "broadcaster";
            }
            else
            {
                ModuleType = s[0];
                Name = new string(s.Substring(1).TakeWhile(c => c != '-').ToArray()).Trim();
            }
            var rest = new string(s.SkipWhile(c => c != '>').Skip(2).ToArray());
            Links = rest.Replace(" ", "").Split(',').ToList();
        }

        public void InitState(Dictionary<string, Module> allModule)
        {
            if(ModuleType == '&')
            {
                InvState = allModule.Values.Where(m => m.Links.Contains(Name)).ToDictionary(l => l.Name, _ => false);
            }
        }

        public List<Signal> Toggle(string sender, bool signal)//false = low, true = high
        {
            switch(ModuleType)
            {
                case '%':
                    if(signal)
                        return new List<Signal>();
                    State = !State;
                    return Links.Select(l => new Signal(Name, l, State)).ToList();
                case '&':
                    InvState[sender] = signal;
                    State = !InvState.All(s => s.Value);
                    return Links.Select(l => new Signal(Name, l, State)).ToList();
                default:
                    return Links.Select(l => new Signal(Name, l, signal)).ToList();
            }
        }
    }
}
