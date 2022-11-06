namespace ROOT.Zfs.Public.Arguments.Pool
{
    /// <summary>
    /// Represents all arguments to zpool attach
    /// </summary>
    public class PoolAttachArgs : PoolAttachReplaceArgs
    {
        /// <inheritdoc />
        public PoolAttachArgs() : base("attach")
        {
            
        }
    }
}