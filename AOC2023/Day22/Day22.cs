
using System.Net.WebSockets;
using System.Numerics;
using Xunit.Abstractions;

namespace AOC2023.Day22;

public class Day22 : Day
{
    public Day22(ITestOutputHelper output) : base(output)
    {
    }

    public override string TestValue => "5";

    public override async Task<string> Run(StreamReader inputData)
    {
        var l = "";
        int c = 0;
        var bricks = new List<Brick>();
        while((l = await inputData.ReadLineAsync()) != null)
        {
            var b = new Brick(c.ToString(), l);
            bricks.Add(b);
            c++;
        }

        bricks = Fall(bricks, out var _);

        var canBeRemoved = 0;
        var totalMove = 0;
        foreach(var b in bricks)
        {
            var newList = bricks.Except(new[] { b }).ToList();

            newList = Fall(newList, out var moveCount);
            if (moveCount == 0)
                canBeRemoved++;
            totalMove += moveCount;
        }

        return totalMove.ToString();
    }

    public List<Brick> Fall(List<Brick> bricks, out int moveCount)
    {
        moveCount = 0;
        bricks = bricks.OrderBy(b => b.startZ).ToList();
        for(var i = 0; i != bricks.Count; i++)
        {
            var b = bricks[i];
            var newPos = b.MoveDown(bricks);
            if (newPos != b)
            {
                //Ok to move down
                bricks[i] = newPos;
                moveCount++;
            }
        }
        return bricks;
    } 

    public record Brick(string Name, int startX, int endX, int startY, int endY, int startZ, int endZ)
    {
        public Brick(string name, string s) : this(name, 0,0,0,0,0,0)
        {
            var s2 = s.Split(new char[]{ '~', ',' }, StringSplitOptions.RemoveEmptyEntries);
            startX = int.Parse(s2[0]);
            endX = int.Parse(s2[3]);
            startY = int.Parse(s2[1]);
            endY = int.Parse(s2[4]);
            startZ = int.Parse(s2[2]);
            endZ = int.Parse(s2[5]);
        }

        public Brick MoveDown(List<Brick> other)
        {
            if (startZ == 1)
                return this;
            var otherB = other.Except(new[] { this });
            var supportingBrickZ = otherB.Where(other =>
            {
                if (other.endZ >= startZ)
                    return false;
                var yCollision = false;
                var xCollision = false;
                //Check y axis
                if (other.startY <= endY && other.endY >= startY)
                    yCollision = true;
                //Check x axis
                if (other.startX <= endX && other.endX >= startX)
                    xCollision = true;
                return yCollision && xCollision;
            }).DefaultIfEmpty(new("Floor", 0, 10000, 0, 100000, 0, 0)).Max(b => b.endZ);

            var delta = startZ - supportingBrickZ - 1;
            return this with
            {
                startZ = startZ - delta,
                endZ = endZ - delta,
            };
        }

        public bool Collide(Brick other)
        {
            var zCollision = false;
            var yCollision = false;
            var xCollision = false;

            //Check elevation
            if (other.startZ <= endZ && other.endZ >= startZ)
                zCollision = true;
            //Check y axis
            if (other.startY <= endY && other.endY >= startY)
                yCollision = true;
            //Check x axis
            if (other.startX <= endX && other.endX >= startX)
                xCollision = true;

            return zCollision && yCollision && xCollision;
        }
    }
}
