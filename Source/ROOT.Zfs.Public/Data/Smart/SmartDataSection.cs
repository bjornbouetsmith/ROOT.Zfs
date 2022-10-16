using System.Collections.Generic;

namespace ROOT.Zfs.Public.Data.Smart
{
    /// <summary>
    /// Smart data section data, i.e. the data from
    /// === START OF READ SMART DATA SECTION ===
    /// </summary>
    public class SmartDataSection
    {
        /// <summary>
        /// The overall status of the device as reported by smartctl
        /// i.e. the line:
        /// SMART overall-health self-assessment test result: PASSED
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Gets a status indicating whether or not this section failed to parse correctly
        /// </summary>
        public bool ParsingFailed { get; set; }

        /// <summary>
        /// A list of smart attributes
        /// </summary>
        public IList<SmartInfoAttribute> Attributes { get; set; }
    }
}