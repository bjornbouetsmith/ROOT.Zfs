using System.Collections.Generic;

namespace ROOT.Zfs.Public.Arguments.Dataset
{
    /// <summary>
    /// Simple args class to contains the name of a dataset and required validations
    /// </summary>
    public abstract class DatasetNameArgs : Args
    {
        /// <summary>
        /// name of pool
        /// </summary>
        public string Name { get; set; }

        /// <inheritdoc />
        protected DatasetNameArgs(string command) : base(command)
        {
        }

        /// <inheritdoc />
        public override bool Validate(out IList<string> errors)
        {
            errors = null;
            ValidateString(Name, false, ref errors);
            return errors == null;
        }
    }
}
