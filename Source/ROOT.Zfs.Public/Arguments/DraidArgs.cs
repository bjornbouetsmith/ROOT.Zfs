namespace ROOT.Zfs.Public.Arguments
{
    /// <summary>
    /// Extra arguments only for draid
    /// </summary>
    public class DraidArgs
    {
        /// <summary>
        /// Number of data devices
        /// </summary>
        public int DataDevices { get; set; }
        
        /// <summary>
        /// Number of children
        /// </summary>
        public int Children { get; set; }

        /// <summary>
        /// Number of spares
        /// </summary>
        public int Spares { get; set; }
    }
}