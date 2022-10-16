namespace ROOT.Zfs.Public.Data.Smart
{
    /// <summary>
    /// Represents a single field from the '=== START OF INFORMATION SECTION ===' section
    /// </summary>
    public class SmartInfoField
    {
        /// <summary>
        /// The name of the field
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// The value of the field
        /// </summary>
        public string Value { get; set; }
    }
}