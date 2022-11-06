using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Shared.Utils.OS;
using ROOT.Shared.Utils.Serialization;
using ROOT.Zfs.Core;
using ROOT.Zfs.Public;
using ROOT.Zfs.Public.Arguments;

namespace ROOT.Zfs.Tests.Integration.Fake
{
    [TestClass]
    public class FakePropertiesTest
    {
        private readonly FakeRemoteConnection _remoteProcessCall = new FakeRemoteConnection("2.1.5-2");

        private IProperties GetProperties()
        {
            return new Properties(_remoteProcessCall);
        }

        [TestMethod, TestCategory("FakeIntegration")]
        public void ListAllProperties()
        {
            var pr = GetProperties();
            var args = new GetPropertyArgs { PropertyTarget = PropertyTarget.Dataset, Target = "tank/myds" };
            var props = pr.Get(args);
            Assert.IsNotNull(props);
            foreach (var prop in props)
            {
                Console.WriteLine(prop.Dump(new JsonFormatter()));
            }
        }

        [TestMethod, TestCategory("FakeIntegration")]
        public void SetDataSetProperty()
        {
            var pr = GetProperties();
            var args = new SetPropertyArgs { PropertyTarget = PropertyTarget.Dataset, Target = "tank/myds", Property = "atime", Value = "off" };
            pr.Set(args);

            var commands = _remoteProcessCall.GetCommandsInvoked();
            Assert.AreEqual(2, commands.Count); // Set+Get

            Assert.AreEqual("/sbin/zfs set atime=off tank/myds", commands[0]);
            Assert.AreEqual("/sbin/zfs get atime tank/myds -H", commands[1]);
        }

        /// <summary>
        /// We cannot test that setting works with the current fake implementation.
        /// </summary>
        [TestMethod, TestCategory("FakeIntegration")]
        public void GetDataSetProperty()
        {
            var pr = GetProperties();
            var args = new GetPropertyArgs { PropertyTarget = PropertyTarget.Dataset, Target = "tank/myds", Property = "atime"};
            var val = pr.Get(args).First();
            
            Assert.AreEqual("off", val.Value);
            var commands = _remoteProcessCall.GetCommandsInvoked();
            Assert.AreEqual(1, commands.Count); // Get
            
            Assert.AreEqual("/sbin/zfs get atime tank/myds -H", commands[0]);
        }

        [TestMethod, TestCategory("FakeIntegration")]
        public void GetAvailableDatasetProperties()
        {
            var pr = GetProperties();
            var props = pr.GetAvailable(PropertyTarget.Dataset).ToList();
            Assert.IsTrue(props.Count > 0);
            Console.WriteLine(props.Dump(new JsonFormatter()));
        }

        [TestMethod, TestCategory("FakeIntegration")]
        public void GetAvailablePoolProperties()
        {
            var pr = GetProperties();
            var props = pr.GetAvailable(PropertyTarget.Pool).ToList();
            Assert.IsTrue(props.Count > 0);
            Console.WriteLine(props.Dump(new JsonFormatter()));
        }

        /// <summary>
        /// We cannot test that reset works with the current fake implementation.
        /// </summary>
        [TestMethod, TestCategory("FakeIntegration")]
        public void ResetPropertyTest()
        {
            var pr = GetProperties();

            var target = "tank/myds";
            var property = "atime";

            var args = new GetPropertyArgs { PropertyTarget = PropertyTarget.Dataset, Target = target, Property = property };
            var before = pr.Get(args).First();
            Assert.AreEqual("off", before.Value);
            var setArg = new SetPropertyArgs { PropertyTarget = PropertyTarget.Dataset, Target = target, Property = property, Value = "on" };
            pr.Set(setArg);
            var inheritArgs = new InheritPropertyArgs { PropertyTarget = PropertyTarget.Dataset, Target = target, Property = property };
            pr.Reset(inheritArgs);

            var commands = _remoteProcessCall.GetCommandsInvoked();

            Console.WriteLine(commands.Dump(new JsonFormatter()));
            // Get, set & get, inherit
            Assert.AreEqual(4, commands.Count);

        }
    }
}
