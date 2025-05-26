
using System.Data;
using System.Globalization;
using CsvHelper;

namespace OcrDataClassificationWebApp.Helpers
{
    public static class CsvHelperUtility
    {
        public static DataTable LoadCsv(Stream stream)
        {
            var dt = new DataTable();
            using var reader = new StreamReader(stream);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            using var dr = new CsvDataReader(csv);
            dt.Load(dr);
            if (!dt.Columns.Contains("Classification"))
                dt.Columns.Add("Classification");
            return dt;
        }
    }
}
