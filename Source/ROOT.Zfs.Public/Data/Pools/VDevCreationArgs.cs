using System.Collections.Generic;

namespace ROOT.Zfs.Public.Data.Pools
{
    public class VDevCreationArgs
    {
        public VDevCreationType Type { get; set; }
        public IList<string> Devices { get; set; }

        public override string ToString()
        {
            return $"{Type.AsString()} {string.Join(' ', Devices)}";
        }

        public bool Validate(out string errorMessage)
        {
            //TODO: Write validation logic, i.e. mirror needs at least two devices etc.

            if (Type == VDevCreationType.Mirror && Devices?.Count < 2)
            {
                errorMessage = "Please provide at least two devices when creating a mirror";
                return false;
            }


            errorMessage = null;
            return true;
        }
    }
}