using ROOT.Zfs.Tests.VersionResponses;

namespace ROOT.Zfs.Tests
{
    internal class VersionResponse2_1_5_2 : VersionAgnosticResponse, IVersionResponse
    {
        public VersionResponse2_1_5_2() : base("2.1.5-2")
        {

        }
        public override string LoadResponse(string commandLine)
        {
            switch (commandLine)
            {
                case "list -Hpr -o type,creation,name,used,refer,avail,mountpoint -t filesystem":
                    return GetFileSystems();
                
                default:
                    return base.LoadResponse(commandLine);
            }
        }

        private static string GetFileSystems()
        {
            return @"filesystem      1663702213      tank    1110973952      33792   15011930624     /tank
filesystem      1664632596      tank/45dfe1b7-5e45-496c-b887-dcc9eded8201       24576   24576   15011930624     /tank/45dfe1b7-5e45-496c-b887-dcc9eded8201
filesystem      1663777408      tank/myds       279552  25600   15011930624     /tank/myds
filesystem      1664209129      tank/myds/ROOT  75776   25600   15011930624     /tank/myds/ROOT
filesystem      1664210834      tank/myds/ROOT/child    50176   25600   15011930624     /tank/myds/ROOT/child
filesystem      1664210840      tank/myds/ROOT/child/granchild  24576   24576   15011930624     /tank/myds/ROOT/child/granchild
filesystem      1663946563      tank/myds34     24576   24576   524263424       /tank/myds34
filesystem      1663956157      tank/myds37     24576   24576   15011930624     /tank/myds37
filesystem      1663946343      tank/myds44     24576   24576   15011930624     /tank/myds44
filesystem      1663781153      tank/mytestds2  24576   24576   1073717248      /tank/mytestds2";
        }


    }
}