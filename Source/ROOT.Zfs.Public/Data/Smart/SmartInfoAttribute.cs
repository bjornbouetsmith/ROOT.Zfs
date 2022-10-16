namespace ROOT.Zfs.Public.Data.Smart
{
    /// <summary>
    /// Represents a single attribute for a smart infomation report
    /// </summary>
    public class SmartInfoAttribute
    {
        /// <summary>
        /// The ID of the attribute
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The name of the attribute
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The raw value of the attribute
        /// </summary>
        public string RawValue { get; set; }

        /// <summary>
        /// The entire line of data for debugging purposes
        /// </summary>
        public string RawLine { get; set; }
    }
}