
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
        while ((s = inputData.ReadLine()) != string.Empty)
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
        while (work.Count > 0)
        {
            var curWork = work.ToList();
            foreach (var partRange in curWork)
            {
                work.Remove(partRange);
                var w = workflows[partRange.workflow];
                foreach (var res in w.Handle(partRange).ToList())
                {
                    if (res.workflow == "A")
                    {
                        allRes.Add(res);
                        sum += res.TotalCombination;
                    }
                    else if (res.workflow != "R")
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
        public long TotalCombination => (xMax - xMin + 1) * (mMax - mMin + 1) * (aMax - aMin + 1) * (sMax - sMin + 1);
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
                    foreach (var res in r.GetPossibilities(pa).ToList())
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
                            if (amount >= p.xMin && amount < p.xMax)
                            {
                                var p1 = p with { xMax = (p.xMax > amount - 1) ? amount - 1 : p.xMax };
                                yield return (p1, res);
                                if (p.xMax > amount - 1)
                                {
                                    var p2 = p with { xMin = amount };
                                    yield return (p2, null);
                                }
                            }
                            else
                                yield return (p, null);
                            break;
                        case 'm':
                            if (amount >= p.mMin && amount < p.mMax)
                            {
                                var p1 = p with { mMax = (p.mMax > amount - 1) ? amount - 1 : p.mMax };
                                yield return (p1, res);
                                if (p.mMax > amount - 1)
                                {
                                    var p2 = p with { mMin = amount };
                                    yield return (p2, null);
                                }
                            }
                            else
                                yield return (p, null);
                            break;
                        case 'a':
                            if (amount >= p.aMin && amount < p.aMax)
                            {
                                var p1 = p with { aMax = (p.aMax > amount - 1) ? amount - 1 : p.aMax };
                                yield return (p1, res);
                                if (p.aMax > amount - 1)
                                {
                                    var p2 = p with { aMin = amount };
                                    yield return (p2, null);
                                }
                            }
                            else
                                yield return (p, null);
                            break;
                        case 's':
                            if (amount >= p.sMin && amount < p.sMax)
                            {
                                var p1 = p with { sMax = (p.sMax > amount - 1) ? amount - 1 : p.sMax };
                                yield return (p1, res);
                                if (p.sMax > amount - 1)
                                {
                                    var p2 = p with { sMin = amount };
                                    yield return (p2, null);
                                }
                            }
                            else
                                yield return (p, null);
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
                            if (amount > p.xMin && amount <= p.xMax)
                            {
                                var p1 = p with { xMin = (p.xMin < amount + 1) ? amount + 1 : p.xMin };
                                yield return (p1, res);
                                if (p.xMin < amount + 1)
                                {
                                    var p2 = p with { xMax = amount };
                                    yield return (p2, null);
                                }
                            }
                            else
                                yield return (p, null);
                            break;
                        case 'm':
                            if (amount > p.mMin && amount <= p.mMax)
                            {
                                var p1 = p with { mMin = (p.mMin < amount + 1) ? amount + 1 : p.mMin };
                                yield return (p1, res);
                                if (p.mMin < amount + 1)
                                {
                                    var p2 = p with { mMax = amount };
                                    yield return (p2, null);
                                }
                            }
                            else
                                yield return (p, null);
                            break;
                        case 'a':
                            if (amount > p.aMin && amount <= p.aMax)
                            {
                                var p1 = p with { aMin = (p.aMin < amount + 1) ? amount + 1 : p.aMin };
                                yield return (p1, res);
                                if (p.aMin < amount + 1)
                                {
                                    var p2 = p with { aMax = amount };
                                    yield return (p2, null);
                                }
                            }
                            else
                                yield return (p, null);
                            break;
                        case 's':
                            if (amount > p.sMin && amount <= p.sMax)
                            {
                                var p1 = p with { sMin = (p.sMin < amount + 1) ? amount + 1 : p.sMin };
                                yield return (p1, res);
                                if (p.sMin < amount + 1)
                                {
                                    var p2 = p with { sMax = amount };
                                    yield return (p2, null);
                                }
                            }
                            else
                                yield return (p, null);
                            break;
                    }
                }
                else yield return (p, RuleString);
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
