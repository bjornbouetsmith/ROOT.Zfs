using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Zfs.Public.Arguments;

namespace ROOT.Zfs.Tests.Public.Arguments
{
    [TestClass]
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


        [DataRow(null, false, false)]
        [DataRow("", false, false)]
        [DataRow(" ", false, false)]
        [DataRow(null, true, true)]
        [DataRow("", true, true)]
        [DataRow(" ", true, true)]
        [DataRow("tank;rm -rf /", false, false)]
        [DataRow("tank && rm -rf /", false, false)]
        [DataRow("tank", false, true)]
        [DataRow("tank-0d702869-7f72-4bdd-bbe8-6007fc58ad51", false, true)]
        [DataRow("tank%252fmyds", false, false)]
        [DataRow("tank:myds", false, true)]
        [DataRow("tank.myds", false, true)]
        [DataRow("tank_myds", false, true)]
        [TestMethod]
        public void ValidateStringTest(string value, bool allowEmpty, bool expectedValid)
        {
            var arg = new TestArg { Value = value, AllowEmpty = allowEmpty };
            var valid = arg.Validate(out var errors);
            Console.WriteLine(string.Join(Environment.NewLine, errors ?? Array.Empty<string>()));
            Assert.AreEqual(expectedValid, valid);
        }

        [TestMethod]
        public void ArgsWillJustReturnCommand()
        {
            var arg = new TestArgWithoutValidation();
            Assert.AreEqual("basic", arg.ToString());
        }
    }
}
