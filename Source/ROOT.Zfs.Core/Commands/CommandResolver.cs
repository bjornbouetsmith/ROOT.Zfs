using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROOT.Zfs.Core.Commands
{
    public class CommandResolver
    {
        private readonly string _version;

        public CommandResolver(string version)
        {
            _version = version;
        }
    }
}
