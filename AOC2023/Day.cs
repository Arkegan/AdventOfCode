using Xunit;
using Xunit.Abstractions;

namespace AOC2023;

public abstract class Day
{
    public ITestOutputHelper Output { get; }

    public Day(ITestOutputHelper output)
    {
        Output = output;
    }

    public abstract string TestValue { get; }

    [Fact]
    public async Task RunTest()
    {
        using var sr = new StreamReader(File.OpenRead($"{this.GetType().Name}/test.txt"));
        var res = await Run(sr);
        Output.WriteLine(res);
        Assert.Equal(TestValue, res);
    }


    [Fact]
    public async Task RunReal()
    {
        using var sr = new StreamReader(File.OpenRead($"{this.GetType().Name}/input.txt"));
        var res = await Run(sr);
        Output.WriteLine(res);
    }

    public abstract Task<string> Run(StreamReader inputData);
}

