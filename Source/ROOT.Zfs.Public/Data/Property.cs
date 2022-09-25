﻿namespace ROOT.Zfs.Public.Data
{
    public class Property
    {
        public Property(string name, bool settable = true, params string[] validValues)
        {
            Name = name;
            Settable = settable;
            ValidValues = validValues;
        }

        public string Name { get; set; }
        public string[] ValidValues { get; }
        public bool Settable { get; set; }
    }
}