using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Drawing.Printing;

namespace VehicleRentalSystem
{
    public class ReportsForm : Form
    {
        private DataGridView dgvMetrics;
        private Button btnExport;
        private Button btnRefresh;
        private Button btnPrint;
        private DateTimePicker dtStart;
        private DateTimePicker dtEnd;

        public ReportsForm()
        {
            InitializeComponent();
            ThemeHelper.ApplyBaseTheme(this);
            ThemeHelper.ApplyCardDialogLayout(
                this,
                title: "Reports & Analytics",
                footerRightButtons: new[] { btnExport, btnRefresh, btnPrint }
            );
            LoadMetrics();
        }

        private void InitializeComponent()
        {
            dgvMetrics = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            btnExport = new Button { Text = "Export CSV" };
            btnRefresh = new Button { Text = "Refresh" };
            btnPrint = new Button { Text = "Print" };
            dtStart = new DateTimePicker { Format = DateTimePickerFormat.Short, Value = DateTime.Today.AddMonths(-1) };
            dtEnd = new DateTimePicker { Format = DateTimePickerFormat.Short, Value = DateTime.Today };

            btnExport.Click += (_, __) => ExportCsv();
            btnRefresh.Click += (_, __) => LoadMetrics();
            btnPrint.Click += (_, __) => PrintMetrics();
            dtStart.ValueChanged += (_, __) => LoadMetrics();
            dtEnd.ValueChanged += (_, __) => LoadMetrics();

            var filterPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 48,
                Padding = new Padding(8),
                FlowDirection = FlowDirection.LeftToRight
            };
            filterPanel.Controls.Add(new Label { Text = "From:", AutoSize = true, Padding = new Padding(0, 8, 4, 0) });
            filterPanel.Controls.Add(dtStart);
            filterPanel.Controls.Add(new Label { Text = "To:", AutoSize = true, Padding = new Padding(8, 8, 4, 0) });
            filterPanel.Controls.Add(dtEnd);

            Controls.Add(dgvMetrics);
            Controls.Add(filterPanel);
            ClientSize = new System.Drawing.Size(960, 640);
            StartPosition = FormStartPosition.CenterParent;
            Text = "Reports & Analytics";
        }

        private void LoadMetrics()
        {
            try
            {
                var vehicles = DatabaseHelper.ExecuteStoredProcedure("sp_GetAllVehicles");
                var customers = DatabaseHelper.ExecuteStoredProcedure("sp_GetAllCustomers");
                var rentals = DatabaseHelper.ExecuteStoredProcedure("sp_GetAllRentals");
                var activeRentals = DatabaseHelper.ExecuteStoredProcedure("sp_GetActiveRentals");
                var damages = DatabaseHelper.ExecuteStoredProcedure("sp_GetAllDamageReports");
                var maint = DatabaseHelper.ExecuteStoredProcedure("sp_GetAllMaintenanceRecords");
                var payments = DatabaseHelper.ExecuteStoredProcedure("sp_GetAllPayments");

                DateTime start = dtStart.Value.Date;
                DateTime end = dtEnd.Value.Date.AddDays(1).AddTicks(-1);

                decimal revenue = 0;
                foreach (DataRow row in payments.Rows)
                {
                    if (row["status"]?.ToString() == "Completed" && row["amount"] != DBNull.Value)
                    {
                        if (DateTime.TryParse(row["payment_date"]?.ToString(), out var pdt) && pdt >= start && pdt <= end)
                            revenue += Convert.ToDecimal(row["amount"]);
                    }
                }

                int totalVehicles = vehicles.Rows.Count;
                int availableVehicles = 0;
                foreach (DataRow row in vehicles.Rows)
                    if (row["status"]?.ToString() == "Available") availableVehicles++;

                int underMaint = 0;
                foreach (DataRow row in vehicles.Rows)
                    if (row["status"]?.ToString() == "Maintenance") underMaint++;

                var rentalsInRange = rentals.AsEnumerable()
                    .Where(r => DateTime.TryParse(r["pickup_date"]?.ToString(), out var pd) && pd >= start && pd <= end)
                    .ToList();
                double utilization = totalVehicles == 0 ? 0 : (double)activeRentals.Rows.Count / totalVehicles * 100;

                var metrics = new DataTable();
                metrics.Columns.Add("Metric");
                metrics.Columns.Add("Value");
                void Add(string name, string value) => metrics.Rows.Add(name, value);

                Add("Total Revenue (payments)", $"{revenue:C2}");
                Add("Total Vehicles", totalVehicles.ToString());
                Add("Available Vehicles", availableVehicles.ToString());
                Add("Vehicles Under Maintenance", underMaint.ToString());
                Add("Active Rentals", activeRentals.Rows.Count.ToString());
                Add("Rentals (in range)", rentalsInRange.Count.ToString());
                Add("Total Customers", customers.Rows.Count.ToString());
                Add("Open Damage Reports", damages.Select("status <> 'Repaired'").Length.ToString());
                Add("Maintenance Records", maint.Rows.Count.ToString());
                Add("Utilization % (active/total)", $"{utilization:F1}%");

                dgvMetrics.DataSource = metrics;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading metrics: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExportCsv()
        {
            if (dgvMetrics.DataSource is not DataTable dt || dt.Rows.Count == 0)
            {
                MessageBox.Show("No data to export.", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using var sfd = new SaveFileDialog { Filter = "CSV Files|*.csv", FileName = "reports_metrics.csv" };
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                ReportExportHelper.ToCsv(dt, sfd.FileName);
                MessageBox.Show("Exported CSV.", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void PrintMetrics()
        {
            if (dgvMetrics.DataSource is not DataTable dt || dt.Rows.Count == 0)
            {
                MessageBox.Show("No data to print.", "Print", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            PrintDocument pd = new PrintDocument();
            int rowIndex = 0;
            pd.PrintPage += (s, e) =>
            {
                float y = e.MarginBounds.Top;
                var font = new System.Drawing.Font("Segoe UI", 10);
                while (rowIndex < dt.Rows.Count)
                {
                    var row = dt.Rows[rowIndex];
                    string line = $"{row["Metric"]}: {row["Value"]}";
                    e.Graphics.DrawString(line, font, System.Drawing.Brushes.Black, e.MarginBounds.Left, y);
                    y += font.GetHeight(e.Graphics) + 4;
                    rowIndex++;
                    if (y > e.MarginBounds.Bottom)
                    {
                        e.HasMorePages = true;
                        return;
                    }
                }
                e.HasMorePages = false;
            };

            using var dlg = new PrintDialog { Document = pd, UseEXDialog = true };
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                pd.Print();
            }
        }
    }
}
