using System;
using System.Collections.Generic;
using ROOT.Zfs.Public.Data;
using ROOT.Zfs.Public.Data.Pools;

namespace ROOT.Zfs.Public
{
    public interface IZPool
    {
        IEnumerable<CommandHistory> GetHistory(string pool, int skipLines = 0, DateTime afterDate = default);
        PoolStatus GetStatus(string pool); //zpool status {pool}
        IEnumerable<PoolInfo> GetAllPoolInfos(); //zpool list -v -P
        PoolInfo GetPoolInfo(string pool); //zpool list -v -P {pool}
    }
}