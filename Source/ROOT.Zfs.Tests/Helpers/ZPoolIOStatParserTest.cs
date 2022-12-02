using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOT.Shared.Utils.Serialization;
using ROOT.Zfs.Core.Helpers;

namespace ROOT.Zfs.Tests.Helpers
{
    [TestClass]
    public class ZPoolIOStatParserTest
    {
        [TestMethod]
        public void SimpleIOStatsForDevicesOnly()
        {
            var stdOut = @"/dev/sde1       0       0       0       11      10660   164997
/dev/sdc1       0       0       0       10      10678   164997";

            var stat = ZPoolIOStatParser.ParseStdOut("tank", stdOut);
            Console.WriteLine(stat.Dump(new JsonFormatter()));
            Assert.AreEqual(2, stat.Stats.Count);
            Assert.AreEqual("/dev/sde1", stat.Stats[0].Device);
            Assert.AreEqual("/dev/sdc1", stat.Stats[1].Device);

        }

        [TestMethod]
        public void IOStatsWithLatencyForDevicesOnly()
        {
            var stdOut = @"/dev/sde1       0       0       0       11      10660   164997  592662  349240  183576  166498  8696    374208  15155   160523  434601  982802
/dev/sdc1       0       0       0       10      10678   164997  21777768        461249  1371204 183562  20402   763893  15401   219947  27787895        1105290";
            var stat = ZPoolIOStatParser.ParseStdOut("tank", stdOut);
            Console.WriteLine(stat.Dump(new JsonFormatter()));
            Assert.AreEqual(2, stat.Stats.Count);
            Assert.AreEqual("/dev/sde1", stat.Stats[0].Device);
            Assert.AreEqual("/dev/sdc1", stat.Stats[1].Device);
            Assert.IsNotNull(stat.Stats[0].LatencyStats);
            Assert.IsNotNull(stat.Stats[1].LatencyStats);
        }


        // Pool, where VDEV line is missing
        [TestMethod]
        public void UnsupportedOutputWithMissingVDEVLine()
        {
            var stdOut = @"rpool   3078639616      28059873280     0       30      3033    355200
/dev/sdi3       0       0       0       15      1528    177600
/dev/sdj3       0       0       0       14      1505    177600";

            var ex = Assert.ThrowsException<FormatException>(() => ZPoolIOStatParser.ParseStdOut("rpool", stdOut));
            Console.WriteLine(ex.Message);
        }

        [TestMethod]
        public void NewUnsupportedFormatShouldthrow() // Added an extra field at the end
        {

            var stdOut = @"rpool   3078639616      28059873280     0       30      3033    355200   10
mirror-0        3078639616      28059873280     0       30      3033    355200   10
/dev/sdi3       0       0       0       15      1528    177600   10
/dev/sdj3       0       0       0       14      1505    177600   10";

            var ex = Assert.ThrowsException<FormatException>(() => ZPoolIOStatParser.ParseStdOut("rpool", stdOut));
            Console.WriteLine(ex.Message);
        }


        [TestMethod]
        public void PoolWithSingleVDev()
        {
            var stdOut = @"rpool   3078639616      28059873280     0       30      3033    355200
mirror-0        3078639616      28059873280     0       30      3033    355200
/dev/sdi3       0       0       0       15      1528    177600
/dev/sdj3       0       0       0       14      1505    177600";

            var stats = ZPoolIOStatParser.ParseStdOut("rpool", stdOut);
            Console.WriteLine(stats.Dump(new JsonFormatter()));
            Assert.AreEqual("rpool", stats.Pool);
            Assert.AreEqual(1, stats.Stats.Count);
            var pool = stats.Stats[0];
            Assert.AreEqual(1, pool.ChildStats.Count);
            Assert.AreEqual("rpool", pool.Device);
            var mirror = pool.ChildStats[0];
            Assert.AreEqual("mirror-0", mirror.Device);
            Assert.AreEqual(2, mirror.ChildStats.Count);
        }

        [TestMethod]
        public void PoolWithMultipleVDevs()
        {
            var stdOut = @"tank    128797577216    3685133381632   2       81      97020   1224475
mirror-0        28464308224     925018431488    0       22      21338   329994
/dev/sde1       0       0       0       11      10660   164997
/dev/sdc1       0       0       0       10      10678   164997
mirror-1        27947773952     925534965760    0       20      21057   326390
/dev/sdd1       0       0       0       10      10546   164195
/dev/sdf1       0       0       0       10      10511   162195
mirror-2        28140531712     925342208000    0       20      21147   306302
/dev/sdb1       0       0       0       10      10573   153151
/dev/sda1       0       0       0       10      10573   153151
mirror-3        44244963328     909237776384    0       17      33476   261787
/dev/sdg1       0       0       0       8       16738   130893
/dev/sdh1       0       0       0       8       16738   130893";

            var stats = ZPoolIOStatParser.ParseStdOut("tank", stdOut);
            Console.WriteLine(stats.Dump(new JsonFormatter()));
            Assert.AreEqual("tank", stats.Pool);
            Assert.AreEqual(4, stats.Stats[0].ChildStats.Count);
            foreach (var child in stats.Stats[0].ChildStats)
            {
                Assert.AreEqual(2, child.ChildStats.Count);
            }

            for (int x = 0; x < stats.Stats[0].ChildStats.Count; x++)
            {
                var stat = stats.Stats[0].ChildStats[x];
                Assert.AreEqual("mirror-" + x, stat.Device);
            }
        }

        [TestMethod]
        public void PoolWithMultipleVDevsWithLatencyStats()
        {
            var stdOut = @"tank    128788054016    3685142904832   2       81      97040   1224386 7284034 416219  658858  179770  20722   573582  16964   192950  8269291 991184
mirror-0        28462534656     925020205056    0       22      21343   329971  4718430 404615  414865  174930  14492   569243  15278   189848  4962768 1044044
/dev/sde1       0       0       0       11      10662   164985  592662  349234  183576  166494  8696    374335  15155   160509  434601  982797
/dev/sdc1       0       0       0       10      10680   164985  21777768        461253  1371204 183557  20402   764151  15401   219933  27787895        1105290
mirror-1        27949854720     925532884992    0       20      21062   326366  3864915 419842  425722  179819  13596   560178  14036   200428  4045918 1046562
/dev/sdd1       0       0       0       10      10548   164183  19750693        482104  1518285 192767  19236   750613  13530   231655  26237249        980289
/dev/sdf1       0       0       0       10      10513   162182  351258  358478  184066  167058  8025    367606  14535   169749  178603  1113680
mirror-2        28138971136     925343768576    0       20      21152   306280  19787334        454511  1510924 185828  23219   787044  19225   203682  25984664        968329
/dev/sdb1       0       0       0       10      10576   153140  19711582        450034  1512118 186114  22942   789551  21727   198222  25852646        904689
/dev/sda1       0       0       0       10      10576   153140  19863010        459010  1509732 185541  23496   784537  16781   209174  26116674        1031970
mirror-3        44236693504     909246046208    0       17      33483   261768  10200402        380710  996829  178616  27861   377502  18501   174552  13838318        878946
/dev/sdg1       0       0       0       8       16741   130884  10122417        375900  996477  178390  26077   376543  17556   169585  13699519        820971
/dev/sdh1       0       0       0       8       16741   130884  10278453        385567  997181  178845  29650   378461  19452   179576  13977122        936920";

            var stats = ZPoolIOStatParser.ParseStdOut("tank", stdOut);
            Console.WriteLine(stats.Dump(new JsonFormatter()));
            Assert.AreEqual("tank", stats.Pool);
            Assert.AreEqual(4, stats.Stats[0].ChildStats.Count);
            for (int x = 0; x < stats.Stats[0].ChildStats.Count; x++)
            {
                var stat = stats.Stats[0].ChildStats[x];
                Assert.AreEqual("mirror-" + x, stat.Device);
                Assert.IsNotNull(stat.LatencyStats);
                foreach (var dev in stat.ChildStats)
                {
                    Assert.IsNotNull(dev.LatencyStats);
                }
            }
        }
    }
}
