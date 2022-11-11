using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Web;

namespace ROOT.Zfs.Public.Arguments
{
    /// <summary>
    /// Base class for arguments
    /// </summary>
    public abstract class Args
    {
        /// <summary>
        /// The allowed characters in strings that should be passed onto zfs/zpool
        /// i.e. pool name, dataset name, snapshotname etc.
        /// </summary>
        protected const string AllowedCharsDefinition = "[0-9]|[a-z]|[A-Z]|_|-|/|:|\\.";

        private static readonly Regex AllowedChars = new Regex(AllowedCharsDefinition, RegexOptions.Compiled);

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
        protected virtual string BuildArgs(string command)
        {
            return command;
        }

        /// <summary>
        /// Returns a string representation of the current args class that can be passed directly onto the appropriate binary
        /// </summary>
        public sealed override string ToString() // Sealed because we dont want implementation to override tostring
        {
            return BuildArgs(Command);
        }

        /// <summary>
        /// Decodes the dataset into its correct format, in case we receive a url encoded dataset name
        /// </summary>
        protected static string Decode(string dataset)
        {
            if (dataset.Contains('%'))
            {
                return HttpUtility.UrlDecode(dataset);
            }
            return dataset;
        }

        /// <summary>
        /// Validates that the given string is 'safe' to be passed onto a command line
        /// </summary>
        protected static bool IsStringValid(string value)
        {
            // Simple check, just to see if we get back the number of characters passed in
            // And if not that means something was there which was not allowed
            return AllowedChars.Matches(value).Count == value.Length;
        }

        /// <summary>
        /// Validates the string is valid and returns any errors in the errors list
        /// </summary>
        /// <param name="value">The value to validate</param>
        /// <param name="allowEmpty">Whether or not null and string.empty is allowed</param>
        /// <param name="errors">The list of errors - will be allocated if null and an error is detected</param>
        /// <param name="nameOfString">Used for making an error message - do not set this, unless you set it to nameof(Value) where value is the name of the variable or property you are testing</param>
        protected static void ValidateString(string value, bool allowEmpty, ref IList<string> errors, [CallerArgumentExpression("value")] string nameOfString="")
        {
            if (string.IsNullOrWhiteSpace(value) && !allowEmpty)
            {
                errors ??=new List<string>();
                errors.Add($"{nameOfString} cannot be empty");
            }

            if (!string.IsNullOrWhiteSpace(value) )
            {
                var converted = Decode(value);
                if (!IsStringValid(converted))
                {
                    errors ??= new List<string>();
                    errors.Add($"{nameOfString} contains invalid characters");
                }
            }
        }
    }
}
