namespace ROOT.Zfs.Public.Arguments.Properties
{
    /// <summary>
    /// Represents all arguments to get/set/reset property commands
    /// </summary>
    public abstract class PropertyArgs : Args
    {
        /// <inheritdoc />
        protected PropertyArgs(string command) : base(command)
        {
        }

        /// <summary>
        /// Gets or sets the property taget type
        /// </summary>
        public PropertyTarget PropertyTarget { get; set; }
        
        /// <summary>
        /// The target of the action, i.e. dataset or pool name
        /// Optional if used as a get available properties
        /// </summary>
        public string Target { get; set; }

        /// <summary>
        /// The name of the property to get/set
        /// Optional if used as a get all commanmd
        /// </summary>
        public string Property { get; set; }

        /// <summary>
        /// The value of the property to set
        /// Option - if used as get property/get all
        /// </summary>
        public string Value { get; set; }
    }
}
