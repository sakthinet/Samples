
using System.Data;
using OfficeOpenXml;

namespace OcrDataClassificationWebApp.Helpers
{
    public static class ExcelHelperUtility
    {
        public static DataTable LoadExcel(Stream stream)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage(stream);
            var ws = package.Workbook.Worksheets.First();
            var dt = new DataTable();

            foreach (var cell in ws.Cells[1, 1, 1, ws.Dimension.End.Column])
                dt.Columns.Add(cell.Text);

            for (int row = 2; row <= ws.Dimension.End.Row; row++)
            {
                var newRow = dt.NewRow();
                for (int col = 1; col <= ws.Dimension.End.Column; col++)
                    newRow[col - 1] = ws.Cells[row, col].Text;
                dt.Rows.Add(newRow);
            }

            if (!dt.Columns.Contains("Classification"))
                dt.Columns.Add("Classification");

            return dt;
        }
    }
}
