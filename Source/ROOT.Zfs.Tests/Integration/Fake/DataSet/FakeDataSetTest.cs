using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Shared.Utils.OS;
using ROOT.Shared.Utils.Serialization;
using ROOT.Zfs.Core;
using ROOT.Zfs.Public;
using ROOT.Zfs.Public.Arguments.Dataset;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Tests.Integration.Fake.DataSet
{
    public abstract class FakeDataSetTest
    {
        internal abstract FakeRemoteConnection CreateProcessCall();

        internal IDatasets GetDatasets(FakeRemoteConnection remoteConnection = null)
        {
            return new Datasets(remoteConnection ?? CreateProcessCall());
        }

        [TestMethod, TestCategory("FakeIntegration")]
        public void ListTest()
        {
            var ds = GetDatasets();
            var dataSets = ds.List(new DatasetListArgs());
            Assert.IsNotNull(dataSets);
            foreach (var set in dataSets)
            {
                Console.WriteLine(set.Dump(new JsonFormatter()));
            }
        }



        [TestMethod, TestCategory("FakeIntegration")]
        public void GetDataSetShouldReturnDataSet()
        {
            var ds = GetDatasets();
            var args = new DatasetListArgs { Root = "tank" };
            var root = ds.List(args).FirstOrDefault();

            Assert.IsNotNull(root);
            Assert.AreEqual("tank", root.DatasetName);
            Assert.AreEqual(DatasetTypes.Filesystem, root.Type);
            Console.WriteLine(root.Dump(new JsonFormatter()));
        }

        [TestMethod, TestCategory("FakeIntegration")]
        public void GetNonExistingDataSetShouldThrowException()
        {
            var ds = GetDatasets();
            Assert.ThrowsException<ProcessCallException>(() => ds.List(new DatasetListArgs { Root = "ungabunga" }).FirstOrDefault());
        }

        [TestMethod, TestCategory("FakeIntegration")]
        public void CreateDateSetTest()
        {
            var ds = GetDatasets();
            var args = new DatasetCreationArgs
            {
                DatasetName = "tank/myds",
                Type = DatasetTypes.Filesystem,
                Properties = new[] { new PropertyValue { Property = "atime", Value = "off" } }

            };
            var dataset = ds.Create(args);
            Assert.IsNotNull(dataset);
        }

        [TestMethod, TestCategory("FakeIntegration")]
        public void CreateDateSetInvalidTest()
        {
            var ds = GetDatasets();
            var args = new DatasetCreationArgs
            {
                DatasetName = "tank/myds",
                Type = DatasetTypes.Bookmark,
                Properties = new[] { new PropertyValue { Property = "atime", Value = "off" } }

            };
            Assert.ThrowsException<ArgumentException>(() => ds.Create(args));
        }

        [TestMethod, TestCategory("FakeIntegration")]
        public void DestroyDataSetTest()
        {
            var ds = GetDatasets();
            var destroyArgs = new DatasetDestroyArgs { Dataset = "tank/myds", DestroyFlags = DatasetDestroyFlags.Recursive };
            var response = ds.Destroy(destroyArgs);
            Assert.AreEqual(DatasetDestroyFlags.Recursive, response.Flags);
        }

        [TestMethod, TestCategory("FakeIntegration")]
        public void PromoteDatasetTest()
        {
            var processCall = CreateProcessCall();
            var ds = GetDatasets(processCall);
            ds.Promote(new PromoteArgs { Name = "tank/myds" });
            var commands = processCall.GetCommandsInvoked();
            Assert.AreEqual(1, commands.Count);
            Assert.AreEqual("/sbin/zfs promote tank/myds", commands[0]);
        }

        [TestMethod, TestCategory("FakeIntegration")]
        public void MountDatasetTest()
        {
            var processCall = CreateProcessCall();
            var ds = GetDatasets(processCall);
            ds.Mount(new MountArgs { Filesystem = "tank/myds" });
            var commands = processCall.GetCommandsInvoked();
            Assert.AreEqual(1, commands.Count);
            Assert.AreEqual("/sbin/zfs mount tank/myds", commands[0]);
        }

        [TestMethod, TestCategory("FakeIntegration")]
        public void UnmountDatasetTest()
        {
            var processCall = CreateProcessCall();
            var ds = GetDatasets(processCall);
            ds.Unmount(new UnmountArgs { Filesystem = "tank/myds" });
            var commands = processCall.GetCommandsInvoked();
            Assert.AreEqual(1, commands.Count);
            Assert.AreEqual("/sbin/zfs unmount tank/myds", commands[0]);
        }
    }
}
