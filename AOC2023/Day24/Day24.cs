
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

        /*
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
        }*/


        //bruteforce
        var h0 = hailstones[0];
        var h1 = hailstones[1];
        var h2 = hailstones[2];
        var h3 = hailstones[3];
        foreach (var rvx in Enumerable.Range(-300, 600))
        {
            foreach(var rvy in Enumerable.Range(-300, 600))
            {
                foreach (var rvz in Enumerable.Range(-300, 600))
                {
                    var h00 = h0 with { vx = h0.vx + rvx, vy = h0.vy + rvy, vz = h0.vz + rvz };
                    var h11 = h1 with { vx = h1.vx + rvx, vy = h1.vy + rvy, vz = h1.vz + rvz };
                    var h22 = h2 with { vx = h2.vx + rvx, vy = h2.vy + rvy, vz = h2.vz + rvz };
                    var h33 = h3 with { vx = h3.vx + rvx, vy = h3.vy + rvy, vz = h3.vz + rvz };

                    if (!h00.Intersect(h11, out var intersection1))
                        continue;
                    if (!h00.Intersect(h22, out var intersection2))
                        continue;
                    if (!h00.Intersect(h33, out var intersection3))
                        continue;
                    if (intersection1 != intersection2)
                        continue;
                    if (intersection2 != intersection3)
                        continue;
                    Output.WriteLine($"Found {intersection1} @ {rvx},{rvy},{rvz}");
                    return (intersection1.X + intersection1.Y + intersection3.Z).ToString();
                }
            }
        }

        return "";






        /*
        var sample = hailstones.Skip(5).Take(4).ToArray();
        /*
        //Reduce frame of reference to avoid overflow
        var deltaX = sample[0].x;
        var deltaY = sample[0].y;
        var deltaZ = sample[0].z;

        sample = sample.Select(h => h with { x = h.x - deltaX, y = h.y - deltaY, z = h.z - deltaZ }).ToArray();*/
        /*
        //take the plane defined by the first hailstone
        var hs1 = sample[1];
        var hs1_2 = sample[1] with { x = hs1.x + hs1.vx, y = hs1.y + hs1.vy, z = hs1.z + hs1.vz};
        
        var plane = Vector3.Cross(hs1.Point, hs1_2.Point);

        var hs2 = sample[2];
        var hs3 = sample[3];

        //find intersection of second & third hailstone with plane
        (var hs2_2, var t2) = IntersectPlane(plane, hs2);
        (var hs3_2, var t3) = IntersectPlane(plane, hs3);

        var rockDirection = (hs2_2 - hs3_2) / (t2-t3);
        var rockPosition = hs2_2 - rockDirection * t2;
        return "".ToString();
        */
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
    /*
    public (Vector3, float) IntersectPlane(Vector3 plane, Hail hail)
    {
        var position = Vector3.Dot(plane, hail.Point);
        var direction = Vector3.Dot(plane, hail.Direction);

        var deltaTime = (position) / (direction);
        var newPosition = hail.Point + (hail.Direction * deltaTime);
        return (newPosition, deltaTime);
    }*/

    public record Hail(int id, long x, long y, long z, long vx, long vy, long vz)
    {
        public DecimalVector3 Point => new(x, y, z);
        public DecimalVector3 Direction => new(vx, vy, vz);

        public Hail Previous => this with { x = x + vx, y = y + vy, z = z + vz };
        public Hail Next => this with { x = x+vx, y = y+vy, z = z+vz };

        public bool Intersect(Hail other, out DecimalVector3 intersection)
        {
            var lineVec3 = other.Point - Point;
            var crossVec1and2 = DecimalVector3.Cross(Direction, other.Direction);
            var crossVec3and2 = DecimalVector3.Cross(lineVec3, other.Direction);

            var planarFactor = DecimalVector3.Dot(lineVec3, crossVec1and2);

            if(planarFactor < 0.1m && SqrMagnitude(crossVec1and2) > 0.1m)
            {
                decimal s = DecimalVector3.Dot(crossVec3and2, crossVec1and2) / SqrMagnitude(crossVec1and2);

                intersection = Point + (Direction * s);
                return true;
            }
            intersection = new DecimalVector3(0,0,0);
            return false;
        }

        private static decimal SqrMagnitude(DecimalVector3 vector) => vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z;
    }

    public record DecimalVector3(decimal X, decimal Y, decimal Z)
    {
        public DecimalVector3(Vector3 vector) : this((decimal)vector.X, (decimal)vector.Y, (decimal)vector.Z)
        {

        }

        public static decimal Dot(DecimalVector3 left, DecimalVector3 right)
        {
            return (left.X * right.X)
                 + (left.Y * right.Y)
                 + (left.Z * right.Z);
        }
        public static DecimalVector3 Cross(DecimalVector3 left, DecimalVector3 right)
        {
            return new DecimalVector3(
                (left.Y * right.Z) - (left.Z * right.Y),
                (left.Z * right.X) - (left.X * right.Z),
                (left.X * right.Y) - (left.Y * right.X)
            );
        }

        public static DecimalVector3 operator -(DecimalVector3 left, DecimalVector3 right)
        {
            return new DecimalVector3(
                left.X - right.X,
                left.Y - right.Y,
                left.Z - right.Z
            );
        }
        public static DecimalVector3 operator +(DecimalVector3 left, DecimalVector3 right)
        {
            return new DecimalVector3(
                left.X + right.X,
                left.Y + right.Y,
                left.Z + right.Z
            );
        }

        public static DecimalVector3 operator *(DecimalVector3 left, decimal right)
        {
            return new DecimalVector3(
                left.X * right,
                left.Y * right,
                left.Z * right
            );
        }
    }
}
