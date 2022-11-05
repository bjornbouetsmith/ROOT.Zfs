using System.Collections.Generic;
using System.Text;

namespace ROOT.Zfs.Public.Arguments
{
    /// <summary>
    /// Contains the required information to unmount a dataset
    /// </summary>
    public class UnmountArgs : Args
    {
        /// <inheritdoc />
        public UnmountArgs() : base("unmount")
        {
        }

        /// <summary>
        /// The name of the filesystem or mountpoint to unmount
        /// This can either be a filesystem name, or a full path to a mountpoint, i.e, /mnt/tank
        /// Mutually exclusive with <see cref="UnmountAllFileSystems"/>
        /// </summary>
        public string Filesystem { get; set; }

        /// <summary>
        /// Whether or not to unmount the specified filesystem in <see cref="Filesystem"/> or all possible filesystems.
        /// Mutually exclusing with specifying a mountpoint <see cref="Filesystem"/>
        /// Corresponds to '-a' in zfs unmount
        /// </summary>
        public bool UnmountAllFileSystems { get; set; }

        /// <summary>
        /// Unload keys for any encryption roots unmounted by this command.
        /// Corresponds to '-u' in zfs unmount
        /// </summary>
        public bool UnloadKeys { get; set; }

        /// <summary>
        /// Forcefully unmount the file system, even if it is currently in use.
        /// This option is not supported on Linux.
        /// Corresponds to '-f' in zfs mount
        /// </summary>
        public bool Force { get; set; }

        /// <summary>
        /// Validates the arguments and returns errors if not valid
        /// If no errors are found, the list <paramref name="errors"/> will be null
        /// </summary>
        public override bool Validate(out IList<string> errors)
        {
            errors = null;

            if (string.IsNullOrWhiteSpace(Filesystem) && !UnmountAllFileSystems)
            {
                errors = new List<string>();
                errors.Add("Filesystem or mountpoint must be specified");
            }

            if (UnmountAllFileSystems && !string.IsNullOrWhiteSpace(Filesystem))
            {
                errors ??= new List<string>();
                errors.Add("Specify either a filesystem/mountpoint or UnMountAllFileSystems - not both");
            }

            return errors == null;
        }

        /// <inheritdoc />
        protected override string BuildArgs(string command)
        {
            var args = new StringBuilder();
            
            args.Append(command);

            if (Force)
            {
                args.Append(" -f");
            }

            if (UnloadKeys)
            {
                args.Append(" -u");
            }

            if (!UnmountAllFileSystems)
            {
                args.Append($" {Filesystem}");
            }
            else
            {
                args.Append(" -a");
            }

            return args.ToString();
        }
    }
}
