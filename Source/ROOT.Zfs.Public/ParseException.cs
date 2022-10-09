using System;
using System.Runtime.Serialization;

namespace ROOT.Zfs.Public
{
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

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("index", Index);
            info.AddValue(Contents, Contents);
            base.GetObjectData(info, context);
        }
    }
}