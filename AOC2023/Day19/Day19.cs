
using Xunit.Abstractions;

namespace AOC2023.Day19;

public class Day19 : Day
{
    public Day19(ITestOutputHelper output) : base(output)
    {
    }

    public override string TestValue => "167409079868000";

    public override async Task<string> Run(StreamReader inputData)
    {
        var workflows = new Dictionary<string, Workflow>();
        var s = "";
        //Parse workflow
        while((s = inputData.ReadLine()) != string.Empty)
        {
            var w = new Workflow(s);

            workflows[w.Name] = w;
        }

        //Parse inputs
        /*
        var sum = 0l;
        while ((s = inputData.ReadLine()) != null)
        {
            var r = s.Trim('{', '}')
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(p => long.Parse(p.Split('=')[1]))
                .ToList();
            var part = new Part(r[0], r[1], r[2], r[3]);

            var work = "in";
            while (work != "A" && work != "R")
            {
                work = workflows[work].Handle(part);
            }
            if (work == "A")
                sum += part.Sum;
        }*/

        var sum = 0l;
        var pr = new PartRange(1, 4000, 1, 4000, 1, 4000, 1, 4000, "in");
        var work = new List<PartRange>() { pr };
        var allRes = new List<PartRange>();
        while(work.Count > 0)
        {
            var curWork = work.ToList();
            foreach(var partRange in curWork)
            {
                work.Remove(partRange);
                var w = workflows[partRange.workflow];
                foreach(var res in w.Handle(partRange))
                {
                    if(res.workflow == "A")
                    {
                        allRes.Add(res);
                        sum += res.TotalCombination;
                    }
                    else if(res.workflow != "R")
                    {
                        work.Add(res);
                    }
                }
            }
        }

        return sum.ToString();
    }

    public record Part(long x, long m, long a, long s)
    {
        public long Sum => x + a + m + s;
    }

    public record PartRange(long xMin, long xMax, long mMin, long mMax, long aMin, long aMax, long sMin, long sMax, string workflow)
    {
        public long TotalCombination => (xMax-xMin) * (mMax-mMin) * (aMax-aMin) * (sMax-sMin);
    }

    public class Workflow
    {
        public string Name { get; set; }
        public List<Rule> Rules { get; set; }

        public Workflow(string l)
        {
            Name = new string(l.TakeWhile(c => c != '{').ToArray());
            Rules = l.Substring(Name.Length).Trim('{', '}').Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => new Rule(s)).ToList();
        }

        public IEnumerable<PartRange> Handle(PartRange part)
        {
            var toTreat = new List<PartRange>() { part };

            foreach (var r in Rules)
            {
                var work = toTreat.ToList();
                toTreat.Clear();
                foreach (var pa in work)
                {
                    foreach (var res in r.GetPossibilities(pa))
                    {
                        if (res.Item2 != null)
                            yield return res.Item1 with { workflow = res.Item2 };
                        else
                            toTreat.Add(res.Item1);
                    }
                }
            }
            if (toTreat.Any())
                throw new Exception();
        }

        public class Rule
        {
            public string RuleString { get; set; }

            public Rule(string r)
            {
                RuleString = r;
            }

            public IEnumerable<(PartRange, string?)> GetPossibilities(PartRange p)
            {
                if (RuleString.Contains('<'))
                {
                    var property = RuleString.First();
                    var amount = long.Parse(new string(RuleString.SkipWhile(c => !char.IsDigit(c)).TakeWhile(c => char.IsDigit(c)).ToArray()));
                    var res = RuleString.Split(':', StringSplitOptions.RemoveEmptyEntries)[1];

                    switch (property)
                    {
                        case 'x':
                            var p1 = p with { xMax = amount - 1 };
                            var p2 = p with { xMin = amount };
                            yield return (p1, res);
                            yield return (p2, null);
                            break;
                        case 'm':
                            var mp1 = p with { mMax = amount - 1 };
                            var mp2 = p with { mMin = amount };
                            yield return (mp1, res);
                            yield return (mp2, null);
                            break;
                        case 'a':
                            var ap1 = p with { aMax = amount - 1 };
                            var ap2 = p with { aMin = amount };
                            yield return (ap1, res);
                            yield return (ap2, null);
                            break;
                        case 's':
                            var sp1 = p with { sMax = amount - 1 };
                            var sp2 = p with { sMin = amount };
                            yield return (sp1, res);
                            yield return (sp2, null);
                            break;
                    }
                }
                else if (RuleString.Contains('>'))
                {
                    var property = RuleString.First();
                    var amount = long.Parse(new string(RuleString.SkipWhile(c => !char.IsDigit(c)).TakeWhile(c => char.IsDigit(c)).ToArray()));
                    var res = RuleString.Split(':', StringSplitOptions.RemoveEmptyEntries)[1];

                    switch (property)
                    {
                        case 'x':
                            var p1 = p with { xMin = amount + 1 };
                            var p2 = p with { xMax = amount };
                            yield return (p1, res);
                            yield return (p2, null);
                            break;
                        case 'm':
                            var mp1 = p with { xMin = amount + 1 };
                            var mp2 = p with { xMax = amount };
                            yield return (mp1, res);
                            yield return (mp2, null);
                            break;
                        case 'a':
                            var ap1 = p with { xMin = amount + 1 };
                            var ap2 = p with { xMax = amount };
                            yield return (ap1, res);
                            yield return (ap2, null);
                            break;
                        case 's':
                            var sp1 = p with { xMin = amount + 1 };
                            var sp2 = p with { xMax = amount };
                            yield return (sp1, res);
                            yield return (sp2, null);
                            break;
                    }
                }
                else
                {
                    yield return (p, RuleString);
                }
            }

            public bool Apply(Part p, out string result)
            {
                if (RuleString.Contains('<'))
                {
                    var property = RuleString.First();
                    var amount = long.Parse(new string(RuleString.SkipWhile(c => !char.IsDigit(c)).TakeWhile(c => char.IsDigit(c)).ToArray()));
                    result = RuleString.Split(':', StringSplitOptions.RemoveEmptyEntries)[1];

                    switch (property)
                    {
                        case 'x':
                            return p.x < amount;
                        case 'm':
                            return p.m < amount;
                        case 'a':
                            return p.a < amount;
                        case 's':
                            return p.s < amount;
                    }
                    throw new Exception();
                }
                else if (RuleString.Contains('>'))
                {
                    var property = RuleString.First();
                    var amount = long.Parse(new string(RuleString.SkipWhile(c => !char.IsDigit(c)).TakeWhile(c => char.IsDigit(c)).ToArray()));
                    result = RuleString.Split(':', StringSplitOptions.RemoveEmptyEntries)[1];

                    switch (property)
                    {
                        case 'x':
                            return p.x > amount;
                        case 'm':
                            return p.m > amount;
                        case 'a':
                            return p.a > amount;
                        case 's':
                            return p.s > amount;
                    }
                    throw new Exception();
                }
                else
                {
                    result = RuleString;
                    return true;
                }
            }
        }
    }
}
