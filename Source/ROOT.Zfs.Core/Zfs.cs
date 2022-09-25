using System;
using ROOT.Shared.Utils.OS;
using ROOT.Zfs.Public;

namespace ROOT.Zfs.Core
{
    public class Zfs : ZfsBase, IZfs
    {
        public Zfs(RemoteProcessCall remoteConnection) 
            : base(remoteConnection)
        {
            Snapshots = new Snapshots(remoteConnection);
            DataSets = new DataSets(remoteConnection);
            Properties = new Properties(remoteConnection);
            Pool = new ZPool(remoteConnection);
        }

        public ISnapshots Snapshots { get; }
        public IDataSets DataSets { get; }
        public IProperties Properties { get; }
        public IZPool Pool { get; }
       
    }
}
