using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Zfs.Public.Data.Pools;

namespace ROOT.Zfs.Tests.Public.Arguments
{
    [TestClass]
    public class PoolCreationArgsTest
    {
        [TestMethod]
        public void NameShouldBeSet()
        {
            var args = new PoolCreationArgs();

            var valid = args.Validate(out var errors);
            Assert.IsFalse(valid);

            Assert.AreEqual(2, errors.Count);
            Assert.IsTrue(errors.Any(e => e.Contains("Please provide a name")));
            Assert.IsTrue(errors.Any(e => e.Contains("Please provide vdevs")));
        }

        [TestMethod]
        public void VDevsMustBeSpecified()
        {
            var args = new PoolCreationArgs { Name = "tank" };

            var valid = args.Validate(out var errors);
            Assert.IsFalse(valid);
            Assert.AreEqual(1, errors.Count);
            Assert.IsTrue(errors[0].Contains("Please provide vdevs"));

            args.VDevs = new VDevCreationArgs[] { };
            valid = args.Validate(out errors);
            Assert.IsFalse(valid);
            Assert.AreEqual(1, errors.Count);
            Assert.IsTrue(errors[0].Contains("Please provide vdevs"));
        }

        [TestMethod]
        public void AnyTypeOfVdevShouldRequireAtLeastOneDevice()
        {
            var args = new PoolCreationArgs
            {
                Name = "tank",
                VDevs =
                    new[]
                    {
                        new VDevCreationArgs
                        {
                            Type= VDevCreationType.Mirror,
                        }
                    }
            };
            var valid = args.Validate(out var errors);
            Assert.IsFalse(valid);
            Console.WriteLine(string.Join(Environment.NewLine, errors));
            Assert.AreEqual(1, errors.Count);
            Assert.IsTrue(errors[0].Contains("Please provide the proper amount of minimum"));
        }


        [TestMethod]
        public void MirrorShouldRequireAtLeastTwoDevices()
        {
            var args = new PoolCreationArgs
            {
                Name = "tank",
                VDevs =
                    new []
                    { 
                        new VDevCreationArgs
                        {
                            Type= VDevCreationType.Mirror,
                            Devices = new[]{"/dev/sda"}
                        }
                    }
            };
            var valid = args.Validate(out var errors);
            Assert.IsFalse(valid);
            Console.WriteLine(string.Join(Environment.NewLine, errors));
            Assert.AreEqual(1, errors.Count);
            Assert.IsTrue(errors[0].Contains("Please provide at least two devices"));

        }
    }
}
