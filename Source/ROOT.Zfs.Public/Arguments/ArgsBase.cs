using System.Collections.Generic;

namespace ROOT.Zfs.Public.Arguments
{
    /// <summary>
    /// Base class for arguments
    /// </summary>
    public abstract class ArgsBase
    {
        /// <summary>
        /// Validates the arguments and returns whether or not they are valid
        /// </summary>
        /// <param name="errors">Any errors</param>
        /// <returns>true if valid;false otherwise</returns>
        public abstract bool Validate(out IList<string> errors);

        /// <summary>
        /// Builds the arguments into a string that can be passed onto the respective binary
        /// Expects the returned string to start with <paramref name="command"/>
        /// </summary>
        public abstract string BuildArgs(string command);

        /// <inheritdoc />
        public override string ToString()
        {
            return BuildArgs("");
        }
    }
}
