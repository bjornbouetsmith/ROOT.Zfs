namespace ROOT.Zfs.Public.Data
{
    /// <summary>
    /// Represents a property on either a dataset or pool that can be set to a value.
    /// Properties that are readonly will not be represented by this type
    /// </summary>
    public class Property
    {
        /// <summary>
        /// Creates a new instance of the property class with the given property name and range of valid values
        /// </summary>
        public Property(string name, params string[] validValues)
        {
            Name = name;
            ValidValues = validValues;
        }

        /// <summary>
        /// The name of the property
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Expected valid values.
        /// This should not be taken verbatim, but interpreted by a human.
        /// </summary>
        public string[] ValidValues { get; }
    }
}
