using System;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Drawing;

namespace VehicleRentalSystem
{
    public partial class VehicleListForm : Form
    {
        public VehicleListForm()
        {
            InitializeComponent();
            ThemeHelper.ApplyStandardListLayout(
                this,
                title: "Vehicle Management",
                addButton: btnAdd,
                searchBox: txtSearch,
                searchButton: btnSearch,
                dataGridView: dgvVehicles,
                leftFooterButtons: new[] { btnEdit, btnDelete },
                rightFooterButtons: new[] { btnClose },
                searchPlaceholder: "Search by Make, Model, ID..."
            );
            ApplyRbac();
            LoadVehicles();
        }

        private void LoadVehicles()
        {
            try
            {
                DataTable dt = DatabaseHelper.ExecuteStoredProcedure("sp_GetAllVehicles");
                dgvVehicles.DataSource = dt;
                
                // Set proper column widths instead of Fill mode
                if (dgvVehicles.Columns.Count > 0)
                {
                    foreach (DataGridViewColumn column in dgvVehicles.Columns)
                    {
                        column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                        // Set reasonable default widths
                        if (column.Name.Contains("id") || column.Name.Contains("_id"))
                            column.Width = 80;
                        else if (column.Name.Contains("make") || column.Name.Contains("model"))
                            column.Width = 120;
                        else if (column.Name.Contains("year"))
                            column.Width = 80;
                        else if (column.Name.Contains("license") || column.Name.Contains("plate"))
                            column.Width = 120;
                        else if (column.Name.Contains("vin"))
                            column.Width = 150;
                        else if (column.Name.Contains("color"))
                            column.Width = 100;
                        else if (column.Name.Contains("mileage") || column.Name.Contains("mile"))
                            column.Width = 100;
                        else if (column.Name.Contains("fuel") || column.Name.Contains("transmission"))
                            column.Width = 100;
                        else if (column.Name.Contains("seating") || column.Name.Contains("seat"))
                            column.Width = 80;
                        else if (column.Name.Contains("status"))
                            column.Width = 100;
                        else if (column.Name.Contains("rate") || column.Name.Contains("price"))
                            column.Width = 100;
                        else if (column.Name.Contains("date") || column.Name.Contains("Date") || column.Name.Contains("_at"))
                            column.Width = 150;
                        else
                            column.Width = 120; // Default width
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading vehicles: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            VehicleForm form = new VehicleForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadVehicles();
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvVehicles.SelectedRows.Count > 0)
            {
                int vehicleId = Convert.ToInt32(dgvVehicles.SelectedRows[0].Cells["vehicle_id"].Value);
                VehicleForm form = new VehicleForm(vehicleId);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadVehicles();
                }
            }
            else
            {
                MessageBox.Show("Please select a vehicle to edit.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvVehicles.SelectedRows.Count > 0)
            {
                if (MessageBox.Show("Are you sure you want to delete this vehicle?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    int vehicleId = Convert.ToInt32(dgvVehicles.SelectedRows[0].Cells["vehicle_id"].Value);
                    HardDeleteVehicle(vehicleId);
                    LoadVehicles();
                }
            }
            else
            {
                MessageBox.Show("Please select a vehicle to delete.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnViewGallery_Click(object sender, EventArgs e)
        {
            if (dgvVehicles.SelectedRows.Count > 0)
            {
                int vehicleId = Convert.ToInt32(dgvVehicles.SelectedRows[0].Cells["vehicle_id"].Value);
                using var gal = new ImageGalleryForm(vehicleId);
                gal.ShowDialog();
            }
            else
            {
                MessageBox.Show("Please select a vehicle to view its gallery.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ApplyRbac()
        {
            bool isAdmin = string.Equals(CurrentUser.Role, "Admin", StringComparison.OrdinalIgnoreCase);
            if (!isAdmin)
            {
                btnAdd.Enabled = false;
                btnEdit.Enabled = false;
                btnDelete.Enabled = false;
            }
        }

        private void HardDeleteVehicle(int vehicleId)
        {
            // Guard: prevent deleting vehicles with rentals/reservations references
            try
            {
                var checkDt = DatabaseHelper.ExecuteQuery(
                    @"SELECT 
                        (SELECT COUNT(*) FROM Rentals WHERE vehicle_id=@vid) AS rentals,
                        (SELECT COUNT(*) FROM Reservations WHERE vehicle_id=@vid) AS reservations,
                        (SELECT COUNT(*) FROM DamageReports dr JOIN Rentals r ON dr.rental_id = r.rental_id WHERE r.vehicle_id=@vid) AS damages",
                    new MySqlParameter("@vid", vehicleId));

                if (checkDt.Rows.Count > 0)
                {
                    var row = checkDt.Rows[0];
                    int rentals = Convert.ToInt32(row["rentals"]);
                    int reservations = Convert.ToInt32(row["reservations"]);
                    int damages = Convert.ToInt32(row["damages"]);
                    if (rentals > 0 || reservations > 0 || damages > 0)
                    {
                        MessageBox.Show("Cannot delete: vehicle has rentals/reservations/damage history. Mark it Retired instead.", "Delete blocked", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                using var conn = DatabaseHelper.GetConnection();
                conn.Open();
                using var tx = conn.BeginTransaction();

                void Exec(string sql)
                {
                    using var cmd = new MySqlCommand(sql, conn, tx);
                    cmd.Parameters.AddWithValue("@vid", vehicleId);
                    cmd.ExecuteNonQuery();
                }

                // Delete dependent rows then vehicle
                Exec("DELETE FROM MaintenancePhotos WHERE maintenance_id IN (SELECT maintenance_id FROM MaintenanceRecords WHERE vehicle_id=@vid);");
                Exec("DELETE FROM MaintenanceRecords WHERE vehicle_id=@vid;");
                Exec("DELETE FROM VehicleImages WHERE vehicle_id=@vid;");
                Exec("DELETE FROM VehicleFeatureMap WHERE vehicle_id=@vid;");
                Exec("DELETE FROM Vehicles WHERE vehicle_id=@vid;");

                tx.Commit();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Delete failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
