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

        public static class DataSets
        {
            public static DataSet GetDataSet(string fullName, ProcessCall previousCall = null)
            {
                ProcessCall pc;

                if (previousCall != null)
                {
                    pc = previousCall | Commands.DataSets.ProcessCalls.GetDataSet(fullName);
                }
                else
                {
                    pc = Commands.DataSets.ProcessCalls.GetDataSets();
                }

                var response = pc.LoadResponse();

                var line = response.StdOut.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Skip(1).FirstOrDefault();
                if (line == null)
                {
                    return null;
                }

                return DataSet.FromString(line);
            }

            public static IEnumerable<DataSet> GetDataSets(ProcessCall previousCall = null)
            {
                ProcessCall pc;

                if (previousCall != null)
                {
                    pc = previousCall | Commands.DataSets.ProcessCalls.GetDataSets();
                }
                else
                {
                    pc = Commands.DataSets.ProcessCalls.GetDataSets();
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

            public static DataSet CreateDataSet(string dataSetName, PropertyValue[] properties = null, ProcessCall previousCall = null)
            {
                ProcessCall pc;

                if (previousCall != null)
                {
                    pc = previousCall | Commands.DataSets.ProcessCalls.CreateDataSet(dataSetName, properties);
                }
                else
                {
                    pc = Commands.DataSets.ProcessCalls.CreateDataSet(dataSetName);
                }

                var response = pc.LoadResponse();

                if (!response.Success)
                {
                    throw response.ToException();
                }

                if (previousCall != null)
                {

                    pc = previousCall | Commands.DataSets.ProcessCalls.GetDataSet(dataSetName);
                }
                else
                {
                    pc = Commands.DataSets.ProcessCalls.GetDataSet(dataSetName);
                }

                response = pc.LoadResponse();

                if (!response.Success)
                {
                    throw response.ToException();
                }

                var line = response.StdOut.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Skip(1).FirstOrDefault();
                if (line == null)
                {
                    throw new InvalidOperationException($"Weird issue, create gave no error, but dataset '{dataSetName}' could not be found");
                }

                return DataSet.FromString(line);
            }

            public static void DeleteDataSet(string fullName, RemoteProcessCall previousCall)
            {
                ProcessCall pc;

                if (previousCall != null)
                {
                    pc = previousCall | Commands.DataSets.ProcessCalls.DestroyDataSet(fullName);
                }
                else
                {
                    pc = Commands.DataSets.ProcessCalls.DestroyDataSet(fullName);
                }

                var response = pc.LoadResponse();

                if (!response.Success)
                {
                    throw response.ToException();
                }
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
