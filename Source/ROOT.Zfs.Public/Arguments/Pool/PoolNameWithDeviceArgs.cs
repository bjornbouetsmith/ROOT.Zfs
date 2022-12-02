using System.Collections.Generic;

namespace ROOT.Zfs.Public.Arguments.Pool
{
    /// <summary>
    /// Class with shared properties between online/offline/detach/clear
    /// </summary>
    public abstract class PoolNameWithDeviceArgs : Args
    {
        private readonly bool _requireDevice;

        /// <inheritdoc />
        protected PoolNameWithDeviceArgs(string command, bool requireDevice) : base(command)
        {
            _requireDevice = requireDevice;
        }

        /// <summary>
        /// The name of the pool in which to either take a device offline/online or detach
        /// </summary>
        public string PoolName { get; set; }

        /// <summary>
        /// The device to take either offline/online or detach
        /// </summary>
        public string Device { get; set; }

        /// <inheritdoc />
        public override bool Validate(out IList<string> errors)
        {
            errors = null;
            ValidateString(PoolName, false, ref errors);
            ValidateString(Device, !_requireDevice, ref errors);

            return errors == null;
        }
    }
}