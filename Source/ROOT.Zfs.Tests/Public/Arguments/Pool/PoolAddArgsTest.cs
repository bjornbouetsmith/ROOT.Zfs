using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Zfs.Public.Arguments;
using ROOT.Zfs.Public.Arguments.Pool;
using ROOT.Zfs.Public.Data;
using ROOT.Zfs.Public.Data.Pools;

namespace ROOT.Zfs.Tests.Public.Arguments.Pool
{
    [TestClass]
    public class PoolAddArgsTest
    {
        [DataRow("tank", true, false, true)]
        [DataRow("tank", false, true, false)]
        [DataRow("tank", false, false, false)]
        [DataRow(null, true, false, false)]
        [DataRow("", true, false, false)]
        [DataRow(" ", true, false, false)]
        [TestMethod]
        public void ValidateTest(string poolName, bool addValidVdev, bool addInvalidDev, bool expectValid)
        {
            IList<VDevCreationArgs> vdevs = addValidVdev ? new List<VDevCreationArgs>
            {
                new() {
                    Type = VDevCreationType.Mirror,
                    Devices = new []{ "a", "b" } }
            } : addInvalidDev ? new List<VDevCreationArgs>
            {
                new() {
                    Type = VDevCreationType.Mirror,
                    Devices = new []{ "a"} }
            } : null;
            var args = new PoolAddArgs { PoolName = poolName, VDevs = vdevs };

            var valid = args.Validate(out var errors);
            Console.WriteLine(string.Join(Environment.NewLine, errors ?? Array.Empty<string>()));
            Assert.AreEqual(expectValid, valid);
        }

        [DataRow("tank", true, "add -f tank mirror a b")]
        [DataRow("tank", false, "add tank mirror a b")]
        [TestMethod]
        public void BuildArgsTestSimple(string pool, bool force, string expected)
        {
            var args = new PoolAddArgs
            {
                PoolName = pool,
                Force = force,
                VDevs = new List<VDevCreationArgs>
                {
                    new() {
                        Type = VDevCreationType.Mirror,
                        Devices = new []{ "a", "b" } }
                }
            };

            Assert.AreEqual(expected, args.ToString());

        }

        [DataRow("tank", "ashift=12", "add tank -o ashift=12 mirror a b")]
        [DataRow("tank", "ashift=12,atime=off", "add tank -o ashift=12 mirror a b")]
        [DataRow("tank", "atime=off", "add tank mirror a b")]
        [TestMethod]
        public void BuildArgsTestWithProperties(string pool, string properties, string expected)
        {
            var props = properties?.Split(',').Select(p => p.Split('=')).Select(a => new PropertyValue { Property = a[0], Value = a[1] }).ToArray();
            var args = new PoolAddArgs
            {
                PoolName = pool,
                VDevs = new List<VDevCreationArgs>
                {
                    new() {
                        Type = VDevCreationType.Mirror,
                        Devices = new []{ "a", "b" } }
                },
                PropertyValues = props
            };

            Assert.AreEqual(expected, args.ToString());
        }

        [TestMethod]
        public void MissingVDevsShouldThrowArgumentExceptionOnToString()
        {
            var args = new PoolAddArgs
            {
                PoolName = "tank"
            };

            var ex = Assert.ThrowsException<ArgumentException>(() => args.ToString());
            Assert.AreEqual("Missing Vdevs", ex.Message);

        }
    }
}
