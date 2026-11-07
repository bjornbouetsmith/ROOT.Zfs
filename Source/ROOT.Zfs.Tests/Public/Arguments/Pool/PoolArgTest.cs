using System;
using Xunit;
using ROOT.Zfs.Public.Arguments.Pool;

namespace ROOT.Zfs.Tests.Public.Arguments.Pool;

public class PoolArgTest
{
    private readonly ITestOutputHelper _output;

    public PoolArgTest(ITestOutputHelper output)
    {
        _output = output;
    }

    public class NameTest : PoolNameArgs
    {
        public NameTest() : base(string.Empty)
        {
        }

        protected override string BuildArgs(string command)
        {
            return string.Empty;
        }
    }

    [InlineData(null,false)]
    [InlineData("", false)]
    [InlineData(" ", false)]
    [InlineData("tank;rm -rf /", false)]
    [InlineData("tank && rm -rf /", false)]
    [InlineData("tank", true)]
    [InlineData("tank/myds", false)]
    [InlineData("tank-0d702869-7f72-4bdd-bbe8-6007fc58ad51", true)]
    [InlineData("tank%2fmyds", false)]
    [InlineData("tank:myds", true)]
    [InlineData("tank.myds", true)]
    [InlineData("tank_myds", true)]
    [Theory]
    public void ValidateTest(string name, bool expectedValid)
    {
        var poolArg = new NameTest { PoolName = name };

        var valid = poolArg.Validate(out var errors);

        _output.WriteLine(string.Join(Environment.NewLine,errors ?? Array.Empty<string>()));
        Assert.Equal(expectedValid, valid);
    }
}