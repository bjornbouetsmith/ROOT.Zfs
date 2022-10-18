using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ROOT.Zfs.Public.Data;
using ROOT.Zfs.Public.Data.Smart;

namespace ROOT.Zfs.Core.Helpers
{
    internal static class SmartInfoParser
    {
        internal static SmartInfo ParseStdOut(string deviceId, string stdOut)
        {
            var info = new SmartInfo();
            info.DeviceId = deviceId;
            info.RawSmartInfo = stdOut;
            try
            {
                info.InfoSection = ParseSmartInfoSection(stdOut);
                info.DataSection = ParseSmartInfoDataSection(stdOut);
                var lbaSize = GetLbaSize(info.InfoSection);
                var lbaWritten = GetLbasWritten(info.DataSection.Attributes);
                if (lbaWritten == 0)
                {
                    // Try to get from nvme log
                    lbaWritten = GetNvmeUnits(false, stdOut);
                }

                var lbaRead = GetLbasRead(info.DataSection.Attributes);
                if (lbaRead == 0)
                {
                    // Try to get from nvme log
                    lbaRead = GetNvmeUnits(true, stdOut);
                }
                info.BytesWritten = new Size(lbaSize * lbaWritten);
                info.BytesRead = new Size(lbaSize * lbaRead);
                info.ParsingFailed |= info.DataSection.ParsingFailed;
            }
            catch (Exception e)
            {
                info.ParsingFailed = true;
                Trace.WriteLine(e);
            }
            return info;
        }


        private static void ParseStatus(SmartDataSection section, string stdOut)
        {
            const string pattern = "SMART overall-health self-assessment test result:";
            var index = stdOut.IndexOf(pattern, StringComparison.OrdinalIgnoreCase);
            if (index == -1)
            {
                section.ParsingFailed = true;
                section.Status = "Unknown";
                return;
            }
            var indexOfEOL = stdOut.IndexOf('\n', index);
            if (indexOfEOL == -1)
            {
                section.ParsingFailed = true;
                section.Status = "Unknown";
                return;
            }
            var startOfStatus = index + pattern.Length;

            section.Status = stdOut[startOfStatus..indexOfEOL].Trim();
        }

        internal static SmartInfoSection ParseSmartInfoSection(string stdOut)
        {
            var section = new SmartInfoSection();
            section.Fields = new List<SmartInfoField>();
            const string start = "=== START OF INFORMATION SECTION ===";
            const string end = "=== START OF";
            var startIndex = stdOut.IndexOf(start, StringComparison.InvariantCulture) + start.Length;
            var endIndex = stdOut.IndexOf(end, startIndex, StringComparison.InvariantCulture);

            var infoSection = stdOut[startIndex..endIndex].Trim();
            var lines = infoSection.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {

                var indexOfColon = line.IndexOf(':');
                if (indexOfColon == -1)
                {
                    // end of section - non standard format, so just break out
                    break;
                }

                var fn = line[..indexOfColon].Trim();
                var value = line[(indexOfColon + 1)..].Trim();
                section.Fields.Add(new SmartInfoField { Name = fn, Value = value });
            }

            return section;
        }

        private static long GetLbaSize(SmartInfoSection section)
        {
            // Usually its either: 512 bytes logical, 4096 bytes physical
            // or 512 bytes logical/physical
            var lbaField =
                section.Fields.FirstOrDefault(s => s.Name == "Sector Size")
                ?? section.Fields.FirstOrDefault(s => s.Name == "Sector Sizes")
                ?? section.Fields.FirstOrDefault(s => s.Name.Contains("Formatted LBA Size"));

            if (lbaField == null)
            {
                return 0;
            }
            // We are only interested in logical, so just return up until first space
            var indexOfSpace = lbaField.Value.IndexOf(' ', StringComparison.InvariantCulture);
            if (indexOfSpace == -1)
            {
                return long.Parse(lbaField.Value);
            }

            var lba = lbaField.Value[..indexOfSpace].Trim();
            return long.Parse(lba);

        }

        private static long GetLbasWritten(IList<SmartInfoAttribute> attributes)
        {
            var attribute = attributes.FirstOrDefault(a => (a.Id == 241 || a.Id == 246) && a.Name == "Total_LBAs_Written");
            if (attribute == null)
            {
                return 0;
            }

            return long.Parse(attribute.RawValue);
        }

        private static long GetLbasRead(IList<SmartInfoAttribute> attributes)
        {
            var attribute = attributes.FirstOrDefault(a => a.Id == 242 && a.Name == "Total_LBAs_Read");
            if (attribute == null)
            {
                return 0;
            }

            return long.Parse(attribute.RawValue);
        }

        private static long GetNvmeUnits(bool read, string stdOut)
        {
            const string reads = "Data Units Read:";
            const string writes = "Data Units Written:";
            if (!stdOut.Contains("NVMe Log", StringComparison.InvariantCulture))
            {
                // Not a nvme
                return 0;
            }
            var indexOfField = stdOut.IndexOf(read ? reads : writes, StringComparison.InvariantCulture);
            if (indexOfField == -1)
            {
                // No field present
                return 0;
            }

            indexOfField += read ? reads.Length : writes.Length;
            var eofIndex = stdOut.IndexOf('\n', indexOfField);
            var value = stdOut[indexOfField..eofIndex].Trim();
            var parts = value.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var units = parts[0].Replace(",", "");
            return long.Parse(units);

        }

        internal static SmartDataSection ParseSmartInfoDataSection(string stdOut)
        {
            const string startHeader = "=== START OF READ SMART DATA SECTION ===";
            const string startOfAttributesHeader = "SMART Attributes Data Structure revision number";
            const string endAttributes = "SMART Error Log Version:";
            var section = new SmartDataSection();


            section.Attributes = new List<SmartInfoAttribute>();
            var startIndex = stdOut.IndexOf(startHeader, StringComparison.InvariantCulture) + startHeader.Length;
            if (startIndex == startHeader.Length - 1)
            {
                // Not found, possibly a nvmedrive
                return section;
            }

            var endIndex = stdOut.IndexOf(startOfAttributesHeader, StringComparison.InvariantCulture);
            var data = stdOut[startIndex..endIndex];

            ParseStatus(section, data);

            try
            {
                var attributeStart = stdOut.IndexOf(startOfAttributesHeader, StringComparison.InvariantCulture) + startOfAttributesHeader.Length;
                var attributeEnd = stdOut.IndexOf(endAttributes, StringComparison.InvariantCulture);
                attributeStart = stdOut.IndexOf("ID#", attributeStart, StringComparison.InvariantCulture);
                var attributesSection = stdOut[attributeStart..attributeEnd];
                foreach (var attribute in ParseSmartInfoAttributes(attributesSection))
                {
                    section.Attributes.Add(attribute);
                }
            }
            catch (Exception e)
            {
                section.ParsingFailed = true;
                Trace.WriteLine(e);
            }

            return section;
        }

        private static IEnumerable<SmartInfoAttribute> ParseSmartInfoAttributes(string stdOut)
        {
            foreach (var line in stdOut.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Skip(1))
            {
                yield return ParseAttributeLine(line);

            }
        }

        internal static SmartInfoAttribute ParseAttributeLine(string line)
        {
            var attribute = new SmartInfoAttribute();
            var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (int.TryParse(parts[0], out var id))
            {
                attribute.Id = id;
            }
            attribute.Name = parts[1];
            attribute.RawValue = string.Join(" ", parts.Skip(9));
            attribute.RawLine = line;
            return attribute;
        }
    }
}
