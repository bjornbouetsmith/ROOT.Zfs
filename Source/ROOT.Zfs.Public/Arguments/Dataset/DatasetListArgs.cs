using System.Collections.Generic;
using System.Text;
using ROOT.Zfs.Public.Data;

namespace ROOT.Zfs.Public.Arguments.Dataset
{
    /// <summary>
    /// Contains the arguments to the zfs list command
    /// </summary>
    public class DatasetListArgs : Args
    {
        /// <inheritdoc />
        public DatasetListArgs() : base("list")
        {
        }

        /// <summary>
        /// The types of datasets to get
        /// </summary>
        public DatasetTypes DatasetTypes { get; set; }

        /// <summary>
        /// The root to list types for - or the single element wanted
        /// </summary>
        public string Root { get; set; }

        /// <summary>
        /// Whether or not to include children
        /// </summary>
        public bool IncludeChildren { get; set; }


        /// <inheritdoc />
        public override bool Validate(out IList<string> errors)
        {
            errors = null;
            ValidateString(Root, true, ref errors);

            return errors == null;
        }

        /// <inheritdoc />
        protected override string BuildArgs(string command)
        {
            var args = new StringBuilder();
            args.Append(command);
            args.Append(" -Hp");
            if (IncludeChildren && !string.IsNullOrWhiteSpace(Root))// include children only makes sense if you have a root, and zfs does not care if you add -r -d 99 or not if you do not provide a root, output is the same
            {
                args.Append('r');
            }

            args.Append(" -o type,creation,name,used,refer,avail,mountpoint,origin");
            
            if (IncludeChildren && !string.IsNullOrWhiteSpace(Root)) // include children only makes sense if you have a root, and zfs does not care if you add -d 99 or not if you do not provide a root
            {
                args.Append(" -d 99");
            }
            
            args.Append($" -t {DatasetTypes.AsString()}");

            var root = Decode(Root);
            if (!string.IsNullOrWhiteSpace(root))
            {
                args.Append($" {root}");
            }

            return args.ToString();
        }
    }
}
