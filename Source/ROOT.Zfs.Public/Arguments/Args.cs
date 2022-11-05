using System.Collections.Generic;

namespace ROOT.Zfs.Public.Arguments
{
    /// <summary>
    /// Base class for arguments
    /// </summary>
    public abstract class Args
    {
        /// <summary>
        /// Gets the command this argument is meant for
        /// </summary>
        public string Command { get; }

        /// <summary>
        /// Creates a new args instance with the given command
        /// </summary>
        protected Args(string command)
        {
            Command = command;
        }

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
        protected abstract string BuildArgs(string command);

        /// <inheritdoc />
        public sealed override string ToString() // Sealed because we dont want implementation to override tostring
        {
            return BuildArgs(Command);
        }
    }
}
