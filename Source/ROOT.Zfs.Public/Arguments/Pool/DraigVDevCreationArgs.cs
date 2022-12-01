using ROOT.Zfs.Public.Data;
using System.Linq;

namespace ROOT.Zfs.Public.Arguments.Pool
{
    /// <summary>
    /// Specialized version of <see cref="VDevCreationArgs" /> for creating draid
    /// </summary>
    public class DraigVDevCreationArgs : VDevCreationArgs
    {
        /// <summary>
        /// Arguments for the draid
        /// </summary>
        public DraidArgs DraidArgs { get; set; }

        /// <summary>
        /// Returns the draid specialized string version of this instance
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Type.AsString()}:{DraidArgs.DataDevices}d:{DraidArgs.Children}c:{DraidArgs.Spares}s {string.Join(' ', Devices.Select(Args.Decode))}";
        }
    }
}