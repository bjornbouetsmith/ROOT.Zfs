using System;
using System.Collections.Generic;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Public
{
    public interface IZPool
    {
        IEnumerable<CommandHistory> GetHistory(string pool, int skipLines = 0, DateTime afterDate = default);
    }
}