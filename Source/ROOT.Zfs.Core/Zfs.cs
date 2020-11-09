using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ROOT.Shared.Utils.OS;
using ROOT.Zfs.Core.Commands;
using ROOT.Zfs.Core.Info;

namespace ROOT.Zfs.Core
{
    public class Zfs
    {
        public static class ProcessCalls
        {
            public static ProcessCall GetVersion()
            {
                return new ProcessCall("/sbin/zfs", "--version");
            }
        }

        public static class Properties
        {
            public static IEnumerable<PropertyValue> GetProperties(string dataset, ProcessCall previousCall = null)
            {
                ProcessCall pc;

                if (previousCall != null)
                {
                    pc = previousCall | Commands.Properties.ProcessCalls.ListProperties(dataset);
                }
                else
                {
                    pc = Commands.Properties.ProcessCalls.ListProperties(dataset);
                }


                var response = pc.LoadResponse();
                if (response.Success)
                {
                    return DataSetProperties.FromStdOutput(response.StdOut);

                }

                throw response.ToException();
            }

            public static PropertyValue GetProperty(string dataset, string property, ProcessCall previousCall = null)
            {
                ProcessCall pc;
                var prop = DataSetProperties.Lookup(property);
                if (previousCall != null)
                {
                    pc = previousCall | Commands.Properties.ProcessCalls.GetProperty(dataset, prop);
                }
                else
                {
                    pc = Commands.Properties.ProcessCalls.GetProperty(dataset, prop);
                }


                var response = pc.LoadResponse();
                if (response.Success)
                {
                    return PropertyValue.FromString(response.StdOut);

                }

                throw response.ToException();
            }
            public static PropertyValue SetProperty(string dataset, string property, string value, ProcessCall previousCall = null)
            {
                ProcessCall pc;
                var prop = DataSetProperties.Lookup(property);
                if (previousCall != null)
                {
                    pc = previousCall | Commands.Properties.ProcessCalls.SetProperty(dataset, prop, value);
                }
                else
                {
                    pc = Commands.Properties.ProcessCalls.SetProperty(dataset, prop, value);
                }


                var response = pc.LoadResponse();
                if (response.Success)
                {
                    return GetProperty(dataset, property, previousCall);

                }

                throw response.ToException();
            }
        }

        public static IEnumerable<DataSet> GetDataSets(ProcessCall previousCall = null)
        {
            ProcessCall pc;

            if (previousCall != null)
            {
                pc = previousCall | DataSets.ProcessCalls.GetDataSets();
            }
            else
            {
                pc = DataSets.ProcessCalls.GetDataSets();
            }

            var response = pc.LoadResponse();
            if (response.Success)
            {

                foreach (var line in response.StdOut.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Skip(1))
                {
                    yield return DataSet.FromString(line);
                }
            }
            else
            {
                throw response.ToException();
            }
        }

        public IEnumerable<Snapshot> LoadSnapshots(string dataset)
        {

            ProcessCall pc = Snapshots.ProcessCalls.ListSnapshots(dataset);
            var response = pc.LoadResponse();
            if (response.Success)
            {
                Console.WriteLine($"Command: {pc.FullCommandLine} success");
                foreach (var line in response.StdOut.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    yield return Snapshot.FromString(line);
                }
            }
            else
            {
                throw response.ToException();
            }
        }
        
        public void DestroySnapshot(string dataset, string snapName)
        {
            ProcessCall pc = Snapshots.ProcessCalls.DestroySnapshot(dataset, snapName);
            var response = pc.LoadResponse();
            if (response.Success)
            {
                Console.WriteLine($"Command: {pc.FullCommandLine} success");
            }
            else
            {
                throw response.ToException();
            }

        }

        public void CreateSnapshot(string dataset)
        {
            CreateSnapshot(dataset, DateTime.UtcNow.ToString("yyyyMMddhhmmss"));
        }

        public void CreateSnapshot(string dataset, string snapName)
        {
            ProcessCall pc = Snapshots.ProcessCalls.CreateSnapshot(dataset, snapName);
            var response = pc.LoadResponse();
            if (response.Success)
            {
                Console.WriteLine($"Command: {pc.FullCommandLine} success");
            }
            else
            {
                throw response.ToException();
            }
        }
    }
}
