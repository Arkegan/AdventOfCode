
using System.Text;
using Xunit.Abstractions;

namespace AOC2023.Day13;

public class Day13 : Day
{
    public Day13(ITestOutputHelper output) : base(output)
    {
    }

    public override string TestValue => "400";

    public override async Task<string> Run(StreamReader inputData)
    {
        var l = "";
        var patterns = new List<List<string>>();
        var currentPattern = new List<string>();
        while((l = await inputData.ReadLineAsync()) != null)
        {
            if(l == "")
            {
                patterns.Add(currentPattern);
                currentPattern = new List<string>();
                continue;
            }

            currentPattern.Add(l);
        }
        patterns.Add(currentPattern);
        var c = 0;
        var sum = 0L;
        foreach(var p in patterns)
        {
            var stop = false;

            var vReflection = FindVerticalReflection(p);
            var hReflection = FindHorizontalReflection(p);
            var oldScore = vReflection == 0 ? hReflection*100 : vReflection;

            for (var x = 0; x < p.First().Length; x++)
            {
                for(var y = 0; y < p.Count; y++)
                {
                    var newP = p.ToList();
                    var sb = new StringBuilder(newP[y]);
                    sb[x] = sb[x] == '#' ? '.' : '#';
                    newP[y] = sb.ToString();

                    var vReflection2 = FindVerticalReflection(newP, vReflection);
                    var hReflection2 = FindHorizontalReflection(newP, hReflection);

                    var newScore = vReflection2 == 0 ? hReflection2 * 100 : vReflection2;
                    if(newScore != 0 && oldScore != newScore)
                    {
                        stop = true;
                        c++;
                        sum += newScore;
                        break;
                    }
                }

                if (stop) break;
            }
            if(!stop)
                throw new Exception("Pattern not found");
        }

        return sum.ToString();
    }

    public int FindHorizontalReflection(List<string> pattern, int exclude = 0)
    {
        for(int i = 1; i<pattern.Count; i++)
        {
            var up = pattern.Take(i).ToList();
            var down = pattern.Skip(i).ToList();

            if(up.Count > down.Count)
                up = up.Skip(up.Count - down.Count).ToList();
            if (down.Count > up.Count)
                down = down.Take(up.Count).ToList();

            down.Reverse();

            var diff = CountDifferences(up, down);
            if(diff <= 0 && i != exclude) 
                return i;
        }
        return 0;
    }

    public int CountDifferences(List<string> first, List<string> second)
    {
        int res = 0;
        for(int i = 0; i < first.Count; i++)
        {
            var firstL = first[i];
            var secondL = second[i];

            for(var y = 0; y != firstL.Length; y++)
            {
                if (firstL[y] != secondL[y])
                    res++;

                if (res > 1)
                    return res;
            }
        }
        return res;
    }

    public int FindVerticalReflection(List<string> pattern, int exclude = 0)
    {
        var l = pattern.First();
        for(int i = 1; i < l.Length; i++)
        {
            if(IsVerticalReflection(l, i, out _))
            {
                if(pattern.Skip(1).All(p => IsVerticalReflection(p, i, out _)))
                {
                    if (i == exclude)
                        continue;
                    return i;
                }
            }
        }

        return 0;
    }

    public bool IsVerticalReflection(string l, int pos, out int diffCount)
    {
        diffCount = 0;
        var left = l.Substring(0, pos);
        var right = l.Substring(pos);

        if (left.Length > right.Length)
            left = left.Substring(left.Length - right.Length, right.Length);
        if (right.Length > left.Length)
            right = right.Substring(0, left.Length);

        left = new string(left.Reverse().ToArray());

        for(var i = 0; i < left.Length; i++)
        {
            if (left[i] == right[i])
                continue;
            diffCount++;
            return false;
        }
        return true;
    }
}
