using System;
using ROOT.Shared.Utils.OS;
using ROOT.Zfs.Public.Arguments;
using ROOT.Zfs.Public.Arguments.Properties;

namespace ROOT.Zfs.Core.Commands
{
    /// <summary>
    /// Contains all the commands that relates to properties
    /// </summary>
    internal class PropertyCommands : Commands
    {
        /// <summary>
        /// Returns a command to get a single, many or all properties depending on the values on the args instance
        /// </summary>
        internal static IProcessCall Get(GetPropertyArgs args)
        {
            if (!args.Validate(out var errors))
            {
                throw new ArgumentException(string.Join(Environment.NewLine, errors), nameof(args));
            }
            var binary = args.PropertyTarget == PropertyTarget.Pool ? WhichZpool : WhichZfs;
            return new ProcessCall(binary, args.ToString());
        }

        /// <summary>
        /// Returns a command that will reset a property to inherited
        /// </summary>
        /// <exception cref="ArgumentException">If arguments are not valid</exception>
        internal static IProcessCall Inherit(InheritPropertyArgs args)
        {
            if (!args.Validate(out var errors))
            {
                throw new ArgumentException(string.Join(Environment.NewLine, errors), nameof(args));
            }

            return new ProcessCall(WhichZfs, args.ToString());
        }

        internal static IProcessCall Set(SetPropertyArgs args)
        {
            if (!args.Validate(out var errors))
            {
                throw new ArgumentException(string.Join(Environment.NewLine, errors), nameof(args));
            }
            var binary = args.PropertyTarget == PropertyTarget.Pool ? WhichZpool : WhichZfs;
            return new ProcessCall(binary, args.ToString());
        }
    }
}
