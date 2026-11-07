using System;
using System.Collections.Generic;
using Xunit;
using ROOT.Zfs.Public.Arguments;

namespace ROOT.Zfs.Tests.Public.Arguments;

public class ArgsTest
{
    public class TestArg : Args
    {
        public TestArg() : base(string.Empty)
        {
        }

        public string Value { get; set; }
        public bool AllowEmpty { get; set; }

        public override bool Validate(out IList<string> errors)
        {
            errors = null;
            ValidateString(Value, AllowEmpty, ref errors);
            return errors == null;
        }

        protected override string BuildArgs(string command)
        {
            return command;
        }
    }

    public class TestArgWithoutValidation : Args
    {
        public TestArgWithoutValidation() : base("basic")
        {
        }

        public override bool Validate(out IList<string> errors)
        {
            errors= null;
            return true;
        }

        protected override string BuildArgs(string command)
        {
            return command;
        }
    }


    [InlineData(null, false, false)]
    [InlineData("", false, false)]
    [InlineData(" ", false, false)]
    [InlineData(null, true, true)]
    [InlineData("", true, true)]
    [InlineData(" ", true, true)]
    [InlineData("tank;rm -rf /", false, false)]
    [InlineData("tank && rm -rf /", false, false)]
    [InlineData("tank", false, true)]
    [InlineData("tank-0d702869-7f72-4bdd-bbe8-6007fc58ad51", false, true)]
    [InlineData("tank%252fmyds", false, false)]
    [InlineData("tank:myds", false, true)]
    [InlineData("tank.myds", false, true)]
    [InlineData("tank_myds", false, true)]
    [Theory]
    public void ValidateStringTest(string value, bool allowEmpty, bool expectedValid)
    {
        var arg = new TestArg { Value = value, AllowEmpty = allowEmpty };
        var valid = arg.Validate(out var errors);
        Console.WriteLine(string.Join(Environment.NewLine, errors ?? Array.Empty<string>()));
        Assert.Equal(expectedValid, valid);
    }

    [Fact]
    public void ArgsWillJustReturnCommand()
    {
        var arg = new TestArgWithoutValidation();
        Assert.Equal("basic", arg.ToString());
    }
}