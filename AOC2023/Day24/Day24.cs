
using System.Diagnostics;
using System.Numerics;
using Xunit.Abstractions;

namespace AOC2023.Day24;

public class Day24 : Day
{
    public Day24(ITestOutputHelper output) : base(output)
    {
    }

    public override string TestValue => "2";

    public override async Task<string> Run(StreamReader inputData)
    {
        var hid = 0;
        var s = "";
        var hailstones = new List<Hail>();
        while((s = await inputData.ReadLineAsync()) != null)
        {
            var n = s.Split((char[])[',', '@'], StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToList();
            var hail = new Hail(hid, n[0], n[1], n[2], n[3], n[4], n[5]);
            hailstones.Add(hail);
            hid++;
        }

        
        var count = 0;
        for(int i = 0; i != hailstones.Count - 1; i++)
        {
            for(int j = i+1; j != hailstones.Count; j++)
            {
                var hail1 = hailstones[i] with { z = 0, vz = 0 };
                var hail2 = hailstones[j] with { z = 0, vz = 0 };

                if (!hail1.Intersect(hail2, out var intersection))
                    continue;

                if (intersection is not { X: >= 200000000000000 and <= 400000000000000, Y: >= 200000000000000 and <= 400000000000000 })
                    continue;
                
                if ((intersection.X > hail1.x && hail1.vx < 0) || (intersection.X < hail1.x && hail1.vx > 0))
                    continue;
                if ((intersection.Y > hail1.y && hail1.vy < 0) || (intersection.Y < hail1.y && hail1.vy > 0))
                    continue;
                if ((intersection.Z > hail1.z && hail1.vz < 0) || (intersection.Z < hail1.z && hail1.vz > 0))
                    continue;
                if ((intersection.X > hail2.x && hail2.vx < 0) || (intersection.X < hail2.x && hail2.vx > 0))
                    continue;
                if ((intersection.Y > hail2.y && hail2.vy < 0) || (intersection.Y < hail2.y && hail2.vy > 0))
                    continue;
                if ((intersection.Z > hail2.z && hail2.vz < 0) || (intersection.Z < hail2.z && hail2.vz > 0))
                    continue;
                count++;
            }
        }

        return count.ToString();
        
        /*
        var t1 = hailstones.Select(h => h.Next).ToHashSet();
        var t2 = t1.Select(h => h.Next).ToHashSet();
        var t3 = t2.Select(h => h.Next).ToHashSet();
        var t4 = t3.Select(h => h.Next).ToHashSet();
        var t5 = t4.Select(h => h.Next).ToHashSet();

        var possibilities = t1.SelectMany(stone1 =>
        {
            return t2.Where(t => t.id != stone1.id).Select(stone2 =>
            {
                var direction = stone2.Point - stone1.Point;
                var position = new Vector3(stone1.x - direction.X, stone1.y - direction.Y, stone1.z - direction.Z);
                return (position, direction, visited: new[] { stone1.id, stone2.id });
            });
        });

        var d = possibilities.ToList();

        var hasIt = d.Any(v => v.direction.X == -3 && v.direction.Y == 1 && v.direction.Z == 2);

        possibilities = possibilities.Where(p =>
        {
            var newPosition = new Vector3(p.position.X + 3 * p.direction.X, p.position.Y + 3 * p.direction.Y, p.position.Z + 3 * p.direction.Z);
            var next = t3.FirstOrDefault(h => !p.visited.Contains(h.id) && h.Point == newPosition);
            return next != null;
        });

        var res = possibilities.ToList();
        foreach(var r in res)
        {
            Output.WriteLine($"({r.position.X}, {r.position.Y}, {r.position.Z}) @ ({r.direction.X}, {r.direction.Y}, {r.direction.Z})");
        }
        return "";*/
    }

    public record Hail(int id, long x, long y, long z, long vx, long vy, long vz)
    {
        public Vector3 Point => new(x, y, z);
        public Vector3 Direction => new(vx, vy, vz);

        public Hail Previous => this with { x = x + vx, y = y + vy, z = z + vz };
        public Hail Next => this with { x = x+vx, y = y+vy, z = z+vz };

        public bool Intersect(Hail other, out Vector3 intersection)
        {
            var lineVec3 = other.Point - Point;
            var crossVec1and2 = Vector3.Cross(Direction, other.Direction);
            var crossVec3and2 = Vector3.Cross(lineVec3, other.Direction);

            var planarFactor = Vector3.Dot(lineVec3, crossVec1and2);

            if(MathF.Abs(planarFactor) < 0.00001f && SqrMagnitude(crossVec1and2) > 0.00001f)
            {
                float s = Vector3.Dot(crossVec3and2, crossVec1and2) / SqrMagnitude(crossVec1and2);
                intersection = Point + (Direction * s);
                return true;
            }
            intersection = Vector3.Zero;
            return false;
        }

        private static float SqrMagnitude(Vector3 vector) => vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z;
    }
}
