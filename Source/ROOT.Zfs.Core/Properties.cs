using System.Collections.Generic;
using System.Linq;
using ROOT.Shared.Utils.OS;
using ROOT.Zfs.Core.Commands;
using ROOT.Zfs.Core.Helpers;
using ROOT.Zfs.Public;
using ROOT.Zfs.Public.Arguments;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Core
{
    internal class Properties : ZfsBase, IProperties
    {
        internal Properties(IProcessCall remoteConnection) : base(remoteConnection)
        {
        }

        /// <inheritdoc />
        public IList<Property> GetAvailable(PropertyTarget targetType)
        {
            var args = new GetPropertyArgs { PropertyTarget = targetType };
            return GetAvailable(args);
        }

        private IList<Property> GetAvailable(GetPropertyArgs args)
        {
            // create new intance, just in case someone re-uses an existing GetPropertyArgs with extra values set that can cause us to call with wrong arguments
            var newArgs = new GetPropertyArgs { PropertyTarget = args.PropertyTarget };

            var cmd = BuildCommand(PropertyCommands.Get(newArgs));
            // get -H returns an error, so to get available we have to create bogus command, which will return on std error
            var response = cmd.LoadResponse(false);
            var expectedColumns = args.PropertyTarget == PropertyTarget.Pool ? 3 : 4;
            return PropertiesParser.FromStdOutput(response.StdError, expectedColumns);
        }

        /// <inheritdoc />
        public IList<PropertyValue> Get(GetPropertyArgs args)
        {
            var pc = BuildCommand(PropertyCommands.Get(args));

            var response = pc.LoadResponse(true);
            return DatasetProperties.FromStdOutput(response.StdOut);
        }

        /// <inheritdoc />
        public PropertyValue Set(SetPropertyArgs args)
        {
            var pc = BuildCommand(PropertyCommands.Set(args));

            pc.LoadResponse(true);
            var getArgs = new GetPropertyArgs { PropertyTarget = args.PropertyTarget, Property = args.Property, Target = args.Target };

            // Should be safe - if not, then the set above would have failed - if we could not find a property to set
            return Get(getArgs).First();
        }

        /// <inheritdoc />
        public void Reset(InheritPropertyArgs args)
        {
            var pc = BuildCommand(PropertyCommands.Inherit(args));

            pc.LoadResponse(true);
        }
    }
}
