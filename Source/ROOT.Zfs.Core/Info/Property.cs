using System;
using System.Collections.Generic;
using System.Linq;

namespace ROOT.Zfs.Core.Info
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

    public static class DataSetProperties
    {
        //TODO: Add all these
        /*
         aclinherit      YES      YES   discard | noallow | restricted | passthrough | passthrough-x
        acltype         YES      YES   noacl | posixacl
        atime           YES      YES   on | off
        canmount        YES       NO   on | off | noauto
        checksum        YES      YES   on | off | fletcher2 | fletcher4 | sha256 | sha512 | skein | edonr
        compression     YES      YES   on | off | lzjb | gzip | gzip-[1-9] | zle | lz4
        context         YES       NO   <selinux context>
        copies          YES      YES   1 | 2 | 3
        dedup           YES      YES   on | off | verify | sha256[,verify], sha512[,verify], skein[,verify], edonr,verify
        defcontext      YES       NO   <selinux defcontext>
        devices         YES      YES   on | off
        dnodesize       YES      YES   legacy | auto | 1k | 2k | 4k | 8k | 16k
        exec            YES      YES   on | off
        filesystem_limit YES       NO   <count> | none
        fscontext       YES       NO   <selinux fscontext>
        keylocation     YES       NO   prompt | <file URI>
        logbias         YES      YES   latency | throughput
        mlslabel        YES      YES   <sensitivity label>
        mountpoint      YES      YES   <path> | legacy | none
        nbmand          YES      YES   on | off
        overlay         YES      YES   on | off
        primarycache    YES      YES   all | none | metadata
        quota           YES       NO   <size> | none
        readonly        YES      YES   on | off
        recordsize      YES      YES   512 to 1M, power of 2
        redundant_metadata YES      YES   all | most
        refquota        YES       NO   <size> | none
        refreservation  YES       NO   <size> | none
        relatime        YES      YES   on | off
        reservation     YES       NO   <size> | none
        rootcontext     YES       NO   <selinux rootcontext>
        secondarycache  YES      YES   all | none | metadata
        setuid          YES      YES   on | off
        sharenfs        YES      YES   on | off | share(1M) options
        sharesmb        YES      YES   on | off | sharemgr(1M) options
        snapdev         YES      YES   hidden | visible
        snapdir         YES      YES   hidden | visible
        snapshot_limit  YES       NO   <count> | none
        special_small_blocks YES      YES   zero or 512 to 1M, power of 2
        version         YES       NO   1 | 2 | 3 | 4 | 5 | current
        volmode         YES      YES   default | full | geom | dev | none
        volsize         YES       NO   <size>
        vscan           YES      YES   on | off
        xattr           YES      YES   on | off | dir | sa
        zoned           YES      YES   on | off
        userquota@...   YES       NO   <size> | none
        groupquota@...  YES       NO   <size> | none
        projectquota@... YES       NO   <size> | none
        userobjquota@... YES       NO   <size> | none
        groupobjquota@... YES       NO   <size> | none
        projectobjquota@... YES       NO   <size> | none
         */
        private static readonly Dictionary<string, Property> _properties = new Dictionary<string, Property>()
        {
            {"aclinherit", new Property("aclinherit",true, "discard", "noallow", "restricted", "passthrough", "passthrough-x")},
            {"acltype", new Property("noacl", true,"posixacl")},
            {"atime", new Property("atime", true,"on","off")},
            {"canmount", new Property("canmount",true, "on","off","noauto")},
            {"checksum", new Property("checksum",true, "on","off","fletcher2","fletcher4","sha256","sha512","skein","edonr")},
            {"compression", new Property("compression", true,"on","off","lzjb","gzip","gzip-[1-9]","zle","lz4")},
            {"recordsize", new Property("recordsize",true, "512","1K","2K","4K","8K","16K","32K","64K","128K","256K","512K","1M")},
            {"sync", new Property("sync",true, "standard","always","disabled")},
            {"readonly", new Property("readonly", true,"on","off")},
            {"quota", new Property("quota", true,"none","0")},
            {"mountpoint", new Property("mountpoint", true,"")},
            {"creation", new Property("creation", false,"")},
            {"used", new Property("used", false,"0")},
            {"dedup", new Property("dedup",true, "on","off","verify")},
            {"exec", new Property("exec", true,"on","off","inherit")},

        };

        public static Property Lookup(string name)
        {
            if (!_properties.TryGetValue(name, out var property))
            {
                property = new Property(name, false, "Unkown property values");
                _properties[name] = property;
            }

            return property;
        }

        public static IEnumerable<PropertyValue> FromStdOutput(string stdOutput)
        {
            foreach (var line in stdOutput.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                yield return PropertyValue.FromString(line);
            }
        }
    }

    public class PropertyValue
    {
        public Property Property { get; }
        public PropertySource Source { get; }
        public string Value { get; }

        public PropertyValue(Property property, PropertySource source, string value)
        {
            Property = property;
            Source = source;
            Value = value;
        }

        public static PropertyValue FromString(string line)
        {
            var parts = line.Split(new[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 4)
            {
                throw new ArgumentException($"{line} could not be parsed, expected 4 parts, got: {parts.Length} - requires an output of NAME\\tPROPERTY\\tVALUE\\tSOURCE to be used for property list i.e. zfs get all -H", nameof(line));
            }

            var property = DataSetProperties.Lookup(parts[1]);
            var value = parts[2];
            var source = PropertySources.Lookup(parts[3]);
            return new PropertyValue(property, source, value);
        }
    }

    public class PropertySource
    {
        public string Name { get; }

        public PropertySource(string name)
        {
            Name = name;
        }
    }

    public class PropertySources
    {
        public static readonly PropertySource Local = new PropertySource("local");
        public static readonly PropertySource Inherited = new PropertySource("inherited");
        public static readonly PropertySource Default = new PropertySource("default");
        public static readonly PropertySource Zfs = new PropertySource("-");
        public static readonly PropertySource Unknown = new PropertySource("Uknown");

        public static PropertySource Lookup(string source)
        {
            if (source.Equals("local", StringComparison.OrdinalIgnoreCase))
            {
                return Local;
            }

            if (source.Equals("default", StringComparison.OrdinalIgnoreCase))
            {
                return Default;
            }
            if (source.Equals("-", StringComparison.OrdinalIgnoreCase))
            {
                return Zfs;
            }

            if (source.StartsWith("inherited", StringComparison.OrdinalIgnoreCase))
            {
                return Inherited;
            }

            return Unknown;
        }
    }


}
