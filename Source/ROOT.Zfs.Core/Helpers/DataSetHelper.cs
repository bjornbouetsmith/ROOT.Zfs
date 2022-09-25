using System.Web;

namespace ROOT.Zfs.Core.Helpers
{
    public static class DataSetHelper
    {
        public static string Decode(string dataset) 
        {
            if (dataset.Contains('%'))
            {
                return HttpUtility.UrlDecode(dataset);
            }
            return dataset;
        }

        public static string CreateDataSetName(string parent, string dataSet)
        {
            return $"{parent}/{dataSet}";
        }
    }
}