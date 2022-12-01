using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Zfs.Public.Arguments.Pool;

namespace ROOT.Zfs.Tests.Public.Arguments.Pool
{
    [TestClass]
    public class PoolArgTest
    {
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

        [DataRow(null,false)]
        [DataRow("", false)]
        [DataRow(" ", false)]
        [DataRow("tank;rm -rf /", false)]
        [DataRow("tank && rm -rf /", false)]
        [DataRow("tank", true)]
        [DataRow("tank/myds", false)]
        [DataRow("tank-0d702869-7f72-4bdd-bbe8-6007fc58ad51", true)]
        [DataRow("tank%2fmyds", false)]
        [DataRow("tank:myds", true)]
        [DataRow("tank.myds", true)]
        [DataRow("tank_myds", true)]
        [TestMethod]
        public void ValidateTest(string name, bool expectedValid)
        {
            var poolArg = new NameTest { Name = name };

            var valid = poolArg.Validate(out var errors);

            Console.WriteLine(string.Join(Environment.NewLine,errors ?? Array.Empty<string>()));
            Assert.AreEqual(expectedValid, valid);
        }
    }
}
