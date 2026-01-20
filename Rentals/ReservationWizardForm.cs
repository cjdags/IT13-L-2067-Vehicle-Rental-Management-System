using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace VehicleRentalSystem
{
    public partial class ReservationWizardForm : Form
    {
        private ComboBox cmbCustomer;
        private ComboBox cmbCategory;
        private ComboBox cmbVehicle;
        private ComboBox cmbRate;
        private DateTimePicker dtpPickup;
        private DateTimePicker dtpReturn;
        private NumericUpDown numDailyRate;
        private Label lblVehicleMileage;
        private Label lblTotal;
        private TextBox txtNotes;
        private CheckBox chkStartRental;
        private Button btnSave;
        private Button btnCancel;
        private Button btnRefreshAvailability;

        public ReservationWizardForm()
        {
            InitializeComponent();
            ThemeHelper.ApplyTheme(this);
            ThemeHelper.ApplyCardDialogLayout(
                this,
                title: "Reservation Wizard",
                footerRightButtons: new[] { btnSave, btnCancel },
                footerLeftButtons: new[] { btnRefreshAvailability }
            );
            LoadCustomers();
            LoadCategories();
            LoadRates();
            RefreshAvailability();
            CalculateTotal();
        }

        private void InitializeComponent()
        {
            cmbCustomer = new ComboBox();
            cmbCategory = new ComboBox();
            cmbVehicle = new ComboBox();
            cmbRate = new ComboBox();
            dtpPickup = new DateTimePicker();
            dtpReturn = new DateTimePicker();
            numDailyRate = new NumericUpDown();
            lblVehicleMileage = new Label();
            lblTotal = new Label();
            txtNotes = new TextBox();
            chkStartRental = new CheckBox();
            btnSave = new Button();
            btnCancel = new Button();
            btnRefreshAvailability = new Button();

            SuspendLayout();

            cmbCustomer.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbCategory.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbVehicle.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbRate.DropDownStyle = ComboBoxStyle.DropDownList;

            dtpPickup.Format = DateTimePickerFormat.Custom;
            dtpPickup.CustomFormat = "MM/dd/yyyy HH:mm";
            dtpPickup.Value = DateTime.Now;
            dtpPickup.ValueChanged += (_, __) => RefreshAvailability();

            dtpReturn.Format = DateTimePickerFormat.Custom;
            dtpReturn.CustomFormat = "MM/dd/yyyy HH:mm";
            dtpReturn.Value = DateTime.Now.AddDays(1);
            dtpReturn.ValueChanged += (_, __) => RefreshAvailability();

            numDailyRate.DecimalPlaces = 2;
            numDailyRate.Minimum = 0;
            numDailyRate.Maximum = 100000;
            numDailyRate.ValueChanged += (_, __) => CalculateTotal();

            chkStartRental.Text = "Start rental now (creates rental immediately)";
            chkStartRental.Dock = DockStyle.Top;

            lblTotal.AutoSize = false;
            lblTotal.Height = 28;
            lblTotal.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            lblTotal.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);

            txtNotes.Multiline = true;
            txtNotes.Height = 60;
            txtNotes.ScrollBars = ScrollBars.Vertical;

            btnSave.Text = "Save";
            btnSave.Click += BtnSave_Click;

            btnCancel.Text = "Cancel";
            btnCancel.Click += (_, __) => this.Close();

            btnRefreshAvailability.Text = "Refresh Availability";
            btnRefreshAvailability.Click += (_, __) => RefreshAvailability();

            Panel Field(string label, Control input)
            {
                var p = new Panel { Height = 72, Margin = new Padding(0, 0, 24, 12), Dock = DockStyle.Fill };
                var lbl = new Label
                {
                    Text = label,
                    AutoSize = false,
                    Height = 18,
                    Dock = DockStyle.Top,
                    Font = ThemeHelper.NormalFont,
                    ForeColor = ThemeHelper.TextColor
                };
                if (input is ComboBox or DateTimePicker or NumericUpDown)
                {
                    input.Height = 36;
                }
                input.Dock = DockStyle.Top;
                p.Controls.Add(input);
                p.Controls.Add(lbl);
                return p;
            }

            Panel ReadOnlyField(string label, Label value)
            {
                var p = new Panel { Height = 72, Margin = new Padding(0, 0, 24, 12), Dock = DockStyle.Fill };
                var lbl = new Label
                {
                    Text = label,
                    AutoSize = false,
                    Height = 18,
                    Dock = DockStyle.Top,
                    Font = ThemeHelper.NormalFont,
                    ForeColor = ThemeHelper.TextColor
                };
                value.Dock = DockStyle.Top;
                value.Height = 36;
                value.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                p.Controls.Add(value);
                p.Controls.Add(lbl);
                return p;
            }

            var grid = new TableLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 2,
                RowCount = 0,
                Dock = DockStyle.Top,
                Margin = new Padding(0),
                Padding = new Padding(0)
            };
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

            void AddRow(Control left, Control right)
            {
                int r = grid.RowCount++;
                grid.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                if (left != null) grid.Controls.Add(left, 0, r);
                if (right != null) grid.Controls.Add(right, 1, r);
            }

            AddRow(Field("Customer", cmbCustomer), Field("Vehicle Category", cmbCategory));
            AddRow(Field("Pickup Date/Time", dtpPickup), Field("Return Date/Time", dtpReturn));
            AddRow(Field("Available Vehicle", cmbVehicle), ReadOnlyField("Vehicle Mileage", lblVehicleMileage));
            AddRow(Field("Rate", cmbRate), Field("Daily Rate (override optional)", numDailyRate));
            AddRow(ReadOnlyField("Total", lblTotal), new Panel { Height = 1, Dock = DockStyle.Fill });

            var notesPanel = new Panel { Height = 120, Margin = new Padding(0, 0, 0, 12), Dock = DockStyle.Fill };
            var notesLbl = new Label { Text = "Notes", Dock = DockStyle.Top, Height = 18, Font = ThemeHelper.NormalFont, ForeColor = ThemeHelper.TextColor };
            txtNotes.Dock = DockStyle.Top;
            notesPanel.Controls.Add(txtNotes);
            notesPanel.Controls.Add(notesLbl);
            grid.RowCount++;
            grid.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            grid.Controls.Add(notesPanel, 0, grid.RowCount - 1);
            grid.SetColumnSpan(notesPanel, 2);

            var chkPanel = new Panel { Height = 40, Margin = new Padding(0, 0, 0, 12), Dock = DockStyle.Top };
            chkPanel.Controls.Add(chkStartRental);
            grid.RowCount++;
            grid.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            grid.Controls.Add(chkPanel, 0, grid.RowCount - 1);
            grid.SetColumnSpan(chkPanel, 2);

            Controls.Add(grid);

            ClientSize = new System.Drawing.Size(960, 640);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            Text = "Reservation Wizard";
            StartPosition = FormStartPosition.CenterParent;

            ResumeLayout(false);
        }

        private void LoadCustomers()
        {
            var dt = DatabaseHelper.ExecuteStoredProcedure("sp_GetAllCustomers");
            dt.Columns.Add("full_name", typeof(string));
            foreach (DataRow row in dt.Rows)
                row["full_name"] = $"{row["first_name"]} {row["last_name"]}";
            cmbCustomer.DataSource = dt;
            cmbCustomer.DisplayMember = "full_name";
            cmbCustomer.ValueMember = "customer_id";
        }

        private void LoadCategories()
        {
            var dt = DatabaseHelper.ExecuteStoredProcedure("sp_GetAllVehicleCategories");
            cmbCategory.DataSource = dt;
            cmbCategory.DisplayMember = "category_name";
            cmbCategory.ValueMember = "category_id";
            cmbCategory.SelectedIndexChanged += (_, __) => { LoadRates(); RefreshAvailability(); };
        }

        private void LoadRates()
        {
            DataTable dt;
            if (cmbCategory.SelectedValue != null)
            {
                dt = DatabaseHelper.ExecuteStoredProcedure("sp_GetRentalRatesByCategory",
                    new MySqlParameter("@p_category_id", cmbCategory.SelectedValue));
            }
            else
            {
                dt = DatabaseHelper.ExecuteStoredProcedure("sp_GetAllRentalRates");
            }

            cmbRate.DataSource = dt;
            cmbRate.DisplayMember = "rate_name";
            cmbRate.ValueMember = "rate_id";
            cmbRate.SelectedIndexChanged += (_, __) =>
            {
                if (cmbRate.SelectedItem is DataRowView drv && drv.Row.Table.Columns.Contains("daily_rate"))
                {
                    numDailyRate.Value = Convert.ToDecimal(drv["daily_rate"]);
                    CalculateTotal();
                }
            };
            if (cmbRate.Items.Count > 0)
            {
                cmbRate.SelectedIndex = 0;
                if (cmbRate.SelectedItem is DataRowView drv)
                    numDailyRate.Value = Convert.ToDecimal(drv["daily_rate"]);
            }
        }

        private void RefreshAvailability()
        {
            if (dtpReturn.Value <= dtpPickup.Value)
            {
                dtpReturn.Value = dtpPickup.Value.AddDays(1);
            }

            var vehicles = DatabaseHelper.ExecuteStoredProcedure("sp_GetAllVehicles");
            var events = DatabaseHelper.ExecuteStoredProcedure(
                "sp_GetAvailabilityCalendar",
                new MySqlParameter("@p_start", dtpPickup.Value),
                new MySqlParameter("@p_end", dtpReturn.Value)
            );

            int? selectedCategory = cmbCategory.SelectedValue as int?;

            IEnumerable<DataRow> filtered = vehicles.AsEnumerable();
            if (selectedCategory.HasValue)
                filtered = filtered.Where(r => Convert.ToInt32(r["category_id"]) == selectedCategory.Value);

            // Exclude vehicles with overlapping events
            var busyVehicleIds = new HashSet<int>();
            foreach (DataRow e in events.Rows)
            {
                int vid = Convert.ToInt32(e["vehicle_id"]);
                DateTime start = Convert.ToDateTime(e["start_at"]);
                DateTime end = Convert.ToDateTime(e["end_at"]);
                if (!(end <= dtpPickup.Value || start >= dtpReturn.Value))
                    busyVehicleIds.Add(vid);
            }
            filtered = filtered.Where(r => !busyVehicleIds.Contains(Convert.ToInt32(r["vehicle_id"])));

            if (!filtered.Any())
            {
                cmbVehicle.DataSource = null;
                cmbVehicle.Text = "No available vehicles for range";
                lblVehicleMileage.Text = "";
            }
            else
            {
                var dt = filtered.CopyToDataTable();
                dt.Columns.Add("display_text", typeof(string));
                foreach (DataRow row in dt.Rows)
                {
                    row["display_text"] = $"{row["make"]} {row["model"]} ({row["license_plate"]})";
                }
                cmbVehicle.DataSource = dt;
                cmbVehicle.DisplayMember = "display_text";
                cmbVehicle.ValueMember = "vehicle_id";
                cmbVehicle.SelectedIndexChanged += (_, __) =>
                {
                    if (cmbVehicle.SelectedItem is DataRowView drv && drv.Row.Table.Columns.Contains("mileage"))
                    {
                        lblVehicleMileage.Text = $"Mileage: {drv["mileage"]} mi";
                    }
                    CalculateTotal();
                };
            }
            CalculateTotal();
        }

        private void CalculateTotal()
        {
            TimeSpan diff = dtpReturn.Value - dtpPickup.Value;
            if (diff.TotalDays <1) diff = TimeSpan.FromDays(1);
            decimal days = (decimal)diff.TotalDays;
            decimal total = days * numDailyRate.Value;
            lblTotal.Text = $"${total:F2}";
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateInput()) return;

            try
            {
                TimeSpan diff = dtpReturn.Value - dtpPickup.Value;
                decimal days = (decimal)Math.Ceiling(diff.TotalDays);
                decimal total = days * numDailyRate.Value;

                var parms = new MySqlParameter[]
                {
                    new MySqlParameter("@p_customer_id", cmbCustomer.SelectedValue),
                    new MySqlParameter("@p_vehicle_id", cmbVehicle.SelectedValue),
                    new MySqlParameter("@p_rate_id", cmbRate.SelectedValue),
                    new MySqlParameter("@p_created_by", CurrentUser.UserId),
                    new MySqlParameter("@p_pickup_date", dtpPickup.Value),
                    new MySqlParameter("@p_return_date", dtpReturn.Value),
                    new MySqlParameter("@p_total_amount", total),
                    new MySqlParameter("@p_notes", txtNotes.Text)
                };

                var resDt = DatabaseHelper.ExecuteStoredProcedure("sp_CreateReservation", parms);
                int reservationId = resDt.Rows.Count > 0 ? Convert.ToInt32(resDt.Rows[0]["reservation_id"]) : 0;

                if (chkStartRental.Checked)
                {
                    // Start rental immediately using the reservation info
                int initialMileage = 0;
                if (cmbVehicle.SelectedItem is DataRowView vdrv && vdrv.Row.Table.Columns.Contains("mileage"))
                {
                    initialMileage = Convert.ToInt32(vdrv["mileage"]);
                }

                var rentalParams = new MySqlParameter[]
                    {
                        new MySqlParameter("@p_reservation_id", reservationId),
                        new MySqlParameter("@p_customer_id", cmbCustomer.SelectedValue),
                        new MySqlParameter("@p_vehicle_id", cmbVehicle.SelectedValue),
                        new MySqlParameter("@p_picked_up_by", CurrentUser.UserId),
                        new MySqlParameter("@p_pickup_date", dtpPickup.Value),
                        new MySqlParameter("@p_expected_return_date", dtpReturn.Value),
                    new MySqlParameter("@p_initial_mileage", initialMileage),
                        new MySqlParameter("@p_total_amount", total)
                    };
                    DatabaseHelper.ExecuteStoredProcedure("sp_CreateRental", rentalParams);
                }

                MessageBox.Show("Reservation saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving reservation: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateInput()
        {
            if (cmbCustomer.SelectedValue == null)
            {
                MessageBox.Show("Select a customer.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (cmbVehicle.SelectedValue == null)
            {
                MessageBox.Show("Select an available vehicle.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (dtpReturn.Value <= dtpPickup.Value)
            {
                MessageBox.Show("Return date must be after pickup date.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }
    }
}
