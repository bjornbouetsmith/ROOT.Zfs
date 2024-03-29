﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Public.Arguments.Dataset
{
    /// <summary>
    /// Container for arguments needed to create a dataset
    /// </summary>
    public class DatasetCreationArgs : Args
    {
        /// <summary>
        /// Creates an instance of dataset creation args
        /// </summary>
        public DatasetCreationArgs() : base("create")
        {
        }

        /// <summary>
        /// The full name of the dataset to create including all parent datasets
        /// </summary>
        public string DatasetName { get; set; }

        /// <summary>
        /// The type of dataset to create.
        /// Only <see cref="DatasetTypes.Volume"/> &amp;<see cref="DatasetTypes.Filesystem"/> is suported to create as filesystem
        /// </summary>
        public DatasetTypes Type { get; set; }

        /// <summary>
        /// Any properties to apply to the dataset.
        /// If <see cref="CreateParents"/> are true, any properties will be ignored for any parent dataset created like this.
        /// Only the final most dataset will use these properties
        /// </summary>
        public PropertyValue[] Properties { get; set; }

        /// <summary>
        /// Any argumguments required for creating a <see cref="DatasetTypes.Volume"/>
        /// </summary>
        public VolumeCreationArgs VolumeArguments { get; set; }
        /// <summary>
        /// Creates all the non-existing parent datasets.
        /// Datasets created in this manner are automatically mounted according to the mountpoint property inherited from their parent.
        /// Any property specified on the command line using the -o option is ignored.
        /// If the target filesystem already exists, the operation completes successfully.
        /// </summary>
        public bool CreateParents { get; set; }

        /// <summary>
        /// Whether or not to mount a newly created <see cref="DatasetTypes.Filesystem"/> dataset.
        /// </summary>
        public bool DoNotMount { get; set; }

        /// <summary>
        /// Validates that the dataset creation arguments are valid
        /// </summary>
        /// <param name="errors">Any error messages;can be null if arguments are valid</param>
        /// <returns>true if valid;false otherwise</returns>
        public override bool Validate(out IList<string> errors)
        {
            errors = null;
            if (VolumeArguments != null)
            {
                VolumeArguments.Validate(out errors);
            }

            if (Type != DatasetTypes.Volume
                && Type != DatasetTypes.Filesystem)
            {
                errors ??= new List<string>();
                errors.Add("You can only create a dataset of type: Filesystem or Volume");
            }

            if (Type == DatasetTypes.Volume && VolumeArguments == null)
            {
                errors ??= new List<string>();
                errors.Add("VolumeArguments needs to be specified for a Volume");
            }

            ValidateString(DatasetName, false, ref errors);

            if (Properties != null && Properties.Length > 0)
            {
                foreach (var prop in Properties)
                {
                    ValidateString(prop.Property, false, ref errors, true);
                    ValidateString(prop.Value, false, ref errors, true);
                }
            }

            return errors == null;
        }

        /// <inheritdoc />
        protected override string BuildArgs(string command)
        {
            var args = new StringBuilder();
            args.Append(command);

            if (Type == DatasetTypes.Volume)
            {
                args.Append($" -b {VolumeArguments.BlockSize} -V {VolumeArguments.VolumeSize}");
                if (VolumeArguments.Sparse)
                {
                    args.Append(" -s");
                }
            }

            if (CreateParents)
            {
                args.Append(" -p");
            }

            if (DoNotMount && Type == DatasetTypes.Filesystem)
            {
                args.Append(" -u");
            }

            var propCommand = Properties != null ? string.Join(' ', Properties.Select(p => $"-o {Decode(p.Property)}={Decode(p.Value)}")) : string.Empty;
            if (propCommand != string.Empty)
            {
                args.Append($" {propCommand}");
            }

            args.Append($" {Decode(DatasetName)}");

            return args.ToString();
        }
    }

    /// <summary>
    /// Required options for creating a volumne
    /// </summary>
    public class VolumeCreationArgs
    {
        /// <summary>
        /// Creates a sparse volume with no reservation.
        /// See volsize in the Native Properties section of zfsprops(7) for more information about sparse volumes.
        /// </summary>
        public bool Sparse { get; set; }

        /// <summary>
        /// Gets or sets the block size for the volume.
        /// <see cref="BlockSizes"/> for valid block sizes
        /// </summary>
        public Size BlockSize { get; set; }

        /// <summary>
        /// The size of the volume.
        /// If the volume is not created sparse - the volume size is reserved in the pool.
        /// </summary>
        public Size VolumeSize { get; set; }

        /// <summary>
        /// Validates the volumen creation args and returns any errors if not valid
        /// </summary>
        /// <param name="errors">Any error messages;can be null if arguments are valid</param>
        /// <returns>true if valid;false otherwise</returns>
        public bool Validate(out IList<string> errors)
        {
            errors = null;
            if (VolumeSize.Bytes == 0UL)
            {
                errors = new List<string>();
                errors.Add("Please specify a VolumeSize greater than 0");
            }

            if (!BlockSizes.IsValid(BlockSize, out var error))
            {
                errors ??= new List<string>();
                errors.Add(error);
            }

            return errors == null;
        }
    }
}
