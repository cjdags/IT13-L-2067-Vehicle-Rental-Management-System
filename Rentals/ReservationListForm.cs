using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace VehicleRentalSystem
{
    public class ReservationListForm : Form
    {
        private DataGridView dgvReservations;
        private Button btnStartRental;
        private Button btnClose;
        private TextBox txtSearch;
        private Button btnSearch;

        public ReservationListForm()
        {
            InitializeComponent();
            ThemeHelper.ApplyStandardListLayout(
                this,
                title: "Pending Reservations",
                addButton: null,
                searchBox: txtSearch,
                searchButton: btnSearch,
                dataGridView: dgvReservations,
                leftFooterButtons: new[] { btnStartRental },
                rightFooterButtons: new[] { btnClose },
                searchPlaceholder: "Search by customer or vehicle..."
            );
            ThemeHelper.WireSearch(txtSearch, btnSearch, dgvReservations, "Search by customer or vehicle...");
            btnStartRental.Click += BtnStartRental_Click;
            btnClose.Click += (s, e) => this.Close();
            LoadReservations();
        }

        private void InitializeComponent()
        {
            dgvReservations = new DataGridView();
            btnStartRental = new Button { Text = "Start Rental" };
            btnClose = new Button { Text = "Close" };
            txtSearch = new TextBox();
            btnSearch = new Button();

            SuspendLayout();
            dgvReservations.AllowUserToAddRows = false;
            dgvReservations.AllowUserToDeleteRows = false;
            dgvReservations.ReadOnly = true;
            dgvReservations.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvReservations.Dock = DockStyle.Fill;
            Controls.Add(dgvReservations);

            ClientSize = new System.Drawing.Size(1100, 700);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            Text = "Pending Reservations";
            StartPosition = FormStartPosition.CenterParent;
            ResumeLayout(false);
        }

        private void LoadReservations()
        {
            try
            {
                var dt = DatabaseHelper.ExecuteStoredProcedure("sp_GetAllReservations");
                // filter out completed and those already linked to rentals
                var rentals = DatabaseHelper.ExecuteQuery("SELECT reservation_id FROM Rentals WHERE reservation_id IS NOT NULL");
                var rentalIds = rentals.AsEnumerable()
                    .Select(r => r.Field<int?>("reservation_id"))
                    .Where(id => id.HasValue)
                    .Select(id => id.Value)
                    .ToHashSet();

                var pending = dt.AsEnumerable()
                    .Where(r =>
                    {
                        var status = r.Field<string>("status") ?? "";
                        if (status.Equals("Completed", StringComparison.OrdinalIgnoreCase)) return false;
                        int rid = r.Field<int>("reservation_id");
                        return !rentalIds.Contains(rid);
                    });

                dgvReservations.DataSource = pending.Any() ? pending.CopyToDataTable() : dt.Clone();

                if (dgvReservations.Columns.Count > 0)
                {
                    foreach (DataGridViewColumn col in dgvReservations.Columns)
                    {
                        col.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                        if (col.Name.Contains("id") || col.Name.Contains("_id")) col.Width = 80;
                        else if (col.Name.Contains("customer") || col.Name.Contains("vehicle")) col.Width = 140;
                        else if (col.Name.Contains("date") || col.Name.Contains("Date")) col.Width = 150;
                        else if (col.Name.Contains("status")) col.Width = 100;
                        else if (col.Name.Contains("amount")) col.Width = 120;
                        else col.Width = 120;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading reservations: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnStartRental_Click(object sender, EventArgs e)
        {
            if (dgvReservations.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a reservation.", "No selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var rowView = dgvReservations.SelectedRows[0].DataBoundItem as DataRowView;
            if (rowView == null) return;
            var row = rowView.Row;

            int reservationId = row.Field<int>("reservation_id");
            int customerId = row.Field<int>("customer_id");
            int vehicleId = row.Field<int>("vehicle_id");
            int rateId = row.Table.Columns.Contains("rate_id") ? row.Field<int>("rate_id") : 0;
            DateTime pickup = row.Field<DateTime>("pickup_date");
            DateTime ret = row.Field<DateTime>("return_date");
            decimal total = row.Table.Columns.Contains("total_amount") ? Convert.ToDecimal(row["total_amount"]) : 0m;

            // get current mileage
            int initialMileage = 0;
            try
            {
                var vdt = DatabaseHelper.ExecuteStoredProcedure("sp_GetVehicle", new MySqlParameter("@p_vehicle_id", vehicleId));
                if (vdt.Rows.Count > 0 && vdt.Rows[0]["mileage"] != DBNull.Value)
                    initialMileage = Convert.ToInt32(vdt.Rows[0]["mileage"]);
            }
            catch { }

            try
            {
                DatabaseHelper.ExecuteStoredProcedure("sp_CreateRental",
                    new MySqlParameter("@p_reservation_id", reservationId),
                    new MySqlParameter("@p_customer_id", customerId),
                    new MySqlParameter("@p_vehicle_id", vehicleId),
                    new MySqlParameter("@p_picked_up_by", CurrentUser.UserId),
                    new MySqlParameter("@p_pickup_date", pickup),
                    new MySqlParameter("@p_expected_return_date", ret),
                    new MySqlParameter("@p_initial_mileage", initialMileage),
                    new MySqlParameter("@p_total_amount", total)
                );

                MessageBox.Show("Rental started from reservation.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadReservations();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error starting rental: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
