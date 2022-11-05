using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Zfs.Public.Arguments.Pool;

namespace ROOT.Zfs.Tests.Public.Arguments.Pool
{
    [TestClass]
    public class PoolTrimArgsTest
    {
        [DataRow("tank", true)]
        [DataRow("", false)]
        [DataRow(null, false)]
        [DataRow("  ", false)]
        [TestMethod]
        public void ValidateShouldValidateCorrectly(string poolName, bool expectValid)
        {
            var args = new PoolTrimArgs { PoolName = poolName };

            var valid = args.Validate(out var errors);
            Console.WriteLine(string.Join(Environment.NewLine, errors ?? Array.Empty<string>()));
            Assert.AreEqual(expectValid, valid);

        }

        [DataRow("tank",null,false,"0B",default," tank")] // only pool
        [DataRow("tank", "/dev/sda", false, "0B", default, " tank /dev/sda")] // with device
        [DataRow("tank", "/dev/sda", true, "0B", default, " -d tank /dev/sda")] // secure
        [DataRow("tank", "/dev/sda", false, "0B", TrimAction.None, " tank /dev/sda")] // none
        [DataRow("tank", "/dev/sda", false, "0B", TrimAction.Suspend, " -s tank /dev/sda")] // suspend
        [DataRow("tank", "/dev/sda", false, "0B", TrimAction.Cancel, " -c tank /dev/sda")] // cancel
        [DataRow("tank", "/dev/sda", false, "512M", default, " -r 512M tank /dev/sda")] // with rate
        [DataRow("tank", "/dev/sda", true, "512G", TrimAction.None, " -d -r 512G tank /dev/sda")] // with rate and secure
        [TestMethod]
        public void ToStringTest(string poolName, string deviceName, bool secure, string trimRate, TrimAction trimAction, string expectedCommand)
        {
            var args = new PoolTrimArgs
            {
                PoolName = poolName,
                DeviceName = deviceName,
                Secure = secure,
                TrimRate = trimRate,
                Action = trimAction
            };

            var stringVer = args.ToString();
            Console.WriteLine(stringVer);
            Assert.AreEqual(expectedCommand, stringVer);
        }
    }
}
