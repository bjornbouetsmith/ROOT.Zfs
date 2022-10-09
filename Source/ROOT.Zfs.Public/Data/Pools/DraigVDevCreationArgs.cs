namespace ROOT.Zfs.Public.Data.Pools
{
    public class DraigVDevCreationArgs : VDevCreationArgs
    {
        public DraidArgs DraidArgs { get; set; }

        public override string ToString()
        {
            return $"{Type.AsString()}:{DraidArgs.DataDevices}d:{DraidArgs.Children}c:{DraidArgs.Spares}s {string.Join(' ', Devices)}";
        }
    }
}