﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Public.Arguments
{
    /// <summary>
    /// Contains the required information to mount a dataset
    /// </summary>
    public class MountArgs
    {
        /// <summary>
        /// The name of the dataset to mount
        /// </summary>
        public string Dataset { get; set; }

        /// <summary>
        /// Whether or not to mount the specified filesystem in <see cref="Dataset"/> or all possible filesystems
        /// Corresponds to '-a' in zfs mount
        /// </summary>
        public bool MountAllFileSystems { get; set; }

        /// <summary>
        /// Perform an overlay mount. Allows mounting in non-empty mountpoint. See mount(8) for more information.
        /// Corresponds to '-O' in zfs mount
        /// </summary>
        public bool OverLayMount { get; set; }

        /// <summary>
        /// Optional zfs properties to use temporarily for the duration of the mount.
        /// These will get converted to real mount options
        /// Corresponds to '-o' in zfs mount
        /// </summary>
        public PropertyValue[] Properties { get; set; }

        /// <summary>
        /// Load keys for encrypted filesystems as they are being mounted.
        /// This is equivalent to executing zfs load-key on each encryption root before mounting it.
        /// Note that if a filesystem has keylocation=prompt, the mount command will fail, since no interactive shell is available.
        /// Corresponds to '-l' in zfs mount
        /// </summary>
        public bool LoadKeys { get; set; }

        /// <summary>
        /// Attempt to force mounting of all filesystems, even those that couldn't normally be mounted (e.g. redacted datasets).
        /// Corresponds to '-f' in zfs mount
        /// </summary>
        public bool Force { get; set; }

        /// <summary>
        /// Validates the arguments and returns errors if not valid
        /// If no errors are found, the list <paramref name="errors"/> will be null
        /// </summary>
        public bool Validate(out IList<string> errors)
        {
            errors = null;

            if (string.IsNullOrWhiteSpace(Dataset) && !MountAllFileSystems)
            {
                errors ??= new List<string>();
                errors.Add("Dataset must be specified");
            }

            if (MountAllFileSystems && !string.IsNullOrWhiteSpace(Dataset))
            {
                errors ??= new List<string>();
                errors.Add("Specify either a dataset or MountAllFileSystems - not both");
            }

            return errors == null;
        }

        /// <summary>
        /// Converts this instance into its string representation, so it can be passed onto the zfs command
        /// </summary>
        public override string ToString()
        {
            StringBuilder args = new StringBuilder();
            if (OverLayMount)
            {
                args.Append(" -O");
            }

            if (Force)
            {
                args.Append(" -f");
            }

            if (LoadKeys)
            {
                args.Append(" -l");
            }


            if (Properties != null)
            {
                // Convert non empty Zfs properties into mount options
                // Invalid zfs properties will be ignored and filtered away because ToMountArguments
                // will retun either null or an empty string to indicate if they are valid or not
                var mountArgs = string.Join(',', Properties.Where(p => !string.IsNullOrWhiteSpace(p.Property)
                                                                  && !string.IsNullOrWhiteSpace(p.Value))
                                                            .Select(p => p.ToMountArgument())
                                                            .Where(s => !string.IsNullOrWhiteSpace(s)));

                if (mountArgs.Length > 0)
                {
                    args.Append(" -o ");
                    args.Append(mountArgs);
                }

            }

            if (!MountAllFileSystems)
            {
                args.Append($" {Dataset}");
            }
            else
            {
                args.Append(" -a");
            }

            return args.ToString();
        }
    }
}