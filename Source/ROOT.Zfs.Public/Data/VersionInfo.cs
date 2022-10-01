using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROOT.Zfs.Public.Data
{
    /// <summary>
    /// Contains information from the command zfs --version
    /// </summary>
    public class VersionInfo
    {
        public string[] Lines { get; set; }
    }
}
