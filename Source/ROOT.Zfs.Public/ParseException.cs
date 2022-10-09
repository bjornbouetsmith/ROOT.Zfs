using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace ROOT.Zfs.Public
{
    [ExcludeFromCodeCoverage]
    [Serializable]
    public class ParseException : Exception
    {
        public int Index { get; }
        public string Contents { get; }

        public ParseException(int index, string contents) : base($@"Failed to parse trimmed input:
Look to index:{index} when discarding empty linies,
Content:
{contents}")
        {
            Index = index;
            Contents = contents;
        }

        [ExcludeFromCodeCoverage]
        protected ParseException(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        {
            Index = info.GetInt32(nameof(Index));
            Contents = info.GetString(nameof(Index));
        }

        [ExcludeFromCodeCoverage]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(Index), Index);
            info.AddValue(nameof(Contents), Contents);
            base.GetObjectData(info, context);
        }
    }
}