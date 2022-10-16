using System.Collections.Generic;

namespace ROOT.Zfs.Public.Data.Smart
{
    /// <summary>
    /// Smart info section data, i.e. the part from
    /// === START OF INFORMATION SECTION ===
    /// </summary>
    public class SmartInfoSection
    {
        /// <summary>
        /// List of fields, where one field corresponds to a single line
        /// </summary>
        public IList<SmartInfoField> Fields { get; set; }
    }
}