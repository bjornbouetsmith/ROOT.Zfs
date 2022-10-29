namespace ROOT.Zfs.Core.Helpers
{
    /// <summary>
    /// Point of this class is to do simple interning of strings, so all sources share the same instance of the string
    /// </summary>
    internal class PropertySource
    {
        public string Name { get; }

        public PropertySource(string name)
        {
            Name = name;
        }
    }
}