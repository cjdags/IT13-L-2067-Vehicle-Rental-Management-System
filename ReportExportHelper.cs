using System.Data;
using System.IO;
using System.Text;

namespace VehicleRentalSystem
{
    public static class ReportExportHelper
    {
        public static void ToCsv(DataTable dt, string path)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                sb.Append(dt.Columns[i].ColumnName);
                if (i < dt.Columns.Count - 1) sb.Append(",");
            }
            sb.AppendLine();
            foreach (DataRow row in dt.Rows)
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    sb.Append((row[i]?.ToString() ?? "").Replace(",", " "));
                    if (i < dt.Columns.Count - 1) sb.Append(",");
                }
                sb.AppendLine();
            }
            File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
        }
    }
}
