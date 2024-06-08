namespace ROOT.Zfs.Public.Data.Datasets
{
    /// <summary>
    /// Represents a dataset inside ZFS
    /// </summary>
    public class Dataset
    {
        /// <summary>
        /// The name of the dataset
        /// </summary>
        public string DatasetName { get; set; }
        
        /// <summary>
        /// The type of dataset, i.e. Filesyste, Volume etc.
        /// </summary>
        public DatasetTypes Type { get; set; }
        
        /// <summary>
        /// Size taken up by this dataset
        /// </summary>
        public Size Used { get; set; }

        /// <summary>
        /// Available space in this dataset
        /// </summary>
        public Size Available { get; set; }

        /// <summary>
        /// Space referred by the dataset
        /// </summary>
        public Size Refer { get; set; }

        /// <summary>
        /// Mountpoint of the dataset if any
        /// </summary>
        public string Mountpoint { get; set; }

        /// <summary>
        /// Whether or not this dataset is a clone
        /// Determined by inspecting the dataset property 'origin'
        /// If origin is different from '-' its a clone
        /// </summary>
        public bool IsClone { get; set; }
    }
}
