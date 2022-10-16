namespace ROOT.Zfs.Public.Data.Smart
{
    /// <summary>
    /// Represents smart info output for a single device as based off smartctl (www.smartmontools.org)
    /// </summary>
    public class SmartInfo
    {
        /// <summary>
        /// The ID of the device this smart info is regarding
        /// </summary>
        public string DeviceId { get; set; }
        /// <summary>
        /// Whether or not parsing failed for some or all fields
        /// </summary>
        public bool ParsingFailed { get; set; }
        
        /// <summary>
        /// The raw output from the smartctl command
        /// </summary>
        public string RawSmartInfo { get; set; }

        /// <summary>
        /// Smart info section data, i.e. the part from
        /// === START OF INFORMATION SECTION ===
        /// </summary>
        public SmartInfoSection InfoSection { get; set; }

        /// <summary>
        /// Smart data section data, i.e. the data from
        /// === START OF READ SMART DATA SECTION ===
        /// </summary>
        public SmartDataSection DataSection { get; set; }

        /// <summary>
        /// The number of bytes written.
        /// If fields cannot be parsed correctly this will be zero.
        /// Number can also be incorrect if Total_LBAs_Written is not reported in logical sector size
        /// </summary>
        public Size BytesWritten { get; set; }

        /// <summary>
        /// The number of bytes read.
        /// If fields cannot be parsed correctly this will be zero.
        /// Number can also be incorrect if Total_LBAs_Read is not reported in logical sector size
        /// </summary>
        public Size BytesRead { get; set; }
    }
}
