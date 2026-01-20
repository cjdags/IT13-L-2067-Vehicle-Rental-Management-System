using System;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace VehicleRentalSystem
{
    public partial class CreateRentalForm : Form
    {
        private ComboBox cmbCustomer;
        private ComboBox cmbVehicle;
        private DateTimePicker dtpRentalDate;
        private DateTimePicker dtpReturnDate;
        private NumericUpDown numDays;
        private NumericUpDown numDailyRate;
        private NumericUpDown numDiscount;
        private Label lblSubtotal;
        private Label lblTotal;
        private TextBox txtNotes;
        private Button btnCalculate;
        private Button btnSave;
        private Button btnCancel;

        public CreateRentalForm()
        {
            InitializeComponent();
            ThemeHelper.ApplyTheme(this);
            ThemeHelper.ApplyCardDialogLayout(
                this,
                title: "Create New Rental",
                footerRightButtons: new[] { btnSave, btnCancel },
                footerLeftButtons: new[] { btnCalculate }
            );
            LoadCustomers();
            LoadAvailableVehicles();
            dtpRentalDate.Value = DateTime.Now;
            dtpReturnDate.Value = DateTime.Now.AddDays(1);
            numDays.Value = 1;
        }

        private void InitializeComponent()
        {
            this.cmbCustomer = new ComboBox();
            this.cmbVehicle = new ComboBox();
            this.dtpRentalDate = new DateTimePicker();
            this.dtpReturnDate = new DateTimePicker();
            this.numDays = new NumericUpDown();
            this.numDailyRate = new NumericUpDown();
            this.numDiscount = new NumericUpDown();
            this.lblSubtotal = new Label();
            this.lblTotal = new Label();
            this.txtNotes = new TextBox();
            this.btnCalculate = new Button();
            this.btnSave = new Button();
            this.btnCancel = new Button();

            this.SuspendLayout();

            cmbCustomer.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbVehicle.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbVehicle.SelectedIndexChanged += CmbVehicle_SelectedIndexChanged;

            dtpRentalDate.Format = DateTimePickerFormat.Short;
            dtpReturnDate.Format = DateTimePickerFormat.Short;
            dtpReturnDate.ValueChanged += DtpReturnDate_ValueChanged;

            numDays.Minimum = 1;
            numDays.Maximum = 365;
            numDays.ValueChanged += NumDays_ValueChanged;

            numDailyRate.DecimalPlaces = 2;
            numDailyRate.Minimum = 0;
            numDailyRate.Maximum = 10000;
            numDailyRate.ReadOnly = true;

            numDiscount.DecimalPlaces = 2;
            numDiscount.Minimum = 0;
            numDiscount.Maximum = 10000;
            numDiscount.ValueChanged += NumDiscount_ValueChanged;

            Panel Field(string label, Control input, int height = 72)
            {
                var p = new Panel { Height = height, Margin = new Padding(0, 0, 24, 12), Dock = DockStyle.Fill };
                var lbl = new Label
                {
                    Text = label,
                    AutoSize = false,
                    Height = 18,
                    Dock = DockStyle.Top,
                    Font = ThemeHelper.NormalFont,
                    ForeColor = ThemeHelper.TextColor
                };

                if (input is TextBox tb)
                {
                    tb.Multiline = true;
                    tb.Height = 36;
                }
                else if (input is ComboBox cb)
                {
                    cb.Height = 36;
                }
                else if (input is DateTimePicker dtp)
                {
                    dtp.Height = 36;
                }
                else if (input is NumericUpDown nud)
                {
                    nud.Height = 36;
                }

                input.Dock = DockStyle.Top;
                p.Controls.Add(input);
                p.Controls.Add(lbl);
                return p;
            }

            Panel ReadOnlyValue(string label, Label valueLabel, bool bold = false)
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
                valueLabel.Dock = DockStyle.Top;
                valueLabel.Height = 36;
                valueLabel.Font = bold ? new Font("Segoe UI", 11F, FontStyle.Bold) : ThemeHelper.NormalFont;
                valueLabel.ForeColor = ThemeHelper.TextColor;
                valueLabel.TextAlign = ContentAlignment.MiddleLeft;
                p.Controls.Add(valueLabel);
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

            AddRow(Field("Customer", cmbCustomer), Field("Vehicle", cmbVehicle));
            AddRow(Field("Rental Date", dtpRentalDate), Field("Expected Return", dtpReturnDate));
            AddRow(Field("Total Days", numDays), Field("Daily Rate", numDailyRate));
            AddRow(Field("Discount", numDiscount), ReadOnlyValue("Subtotal", lblSubtotal));
            AddRow(ReadOnlyValue("Total Amount", lblTotal, bold: true), new Panel { Height = 72, Margin = new Padding(0, 0, 0, 12), Dock = DockStyle.Fill });

            // Notes spans full width (optional, keep small)
            txtNotes.Multiline = true;
            txtNotes.ScrollBars = ScrollBars.Vertical;
            txtNotes.Height = 64;
            var notesPanel = new Panel { Height = 100, Margin = new Padding(0, 0, 0, 12), Dock = DockStyle.Fill };
            var notesLbl = new Label { Text = "Notes", Dock = DockStyle.Top, Height = 18, Font = ThemeHelper.NormalFont, ForeColor = ThemeHelper.TextColor };
            txtNotes.Dock = DockStyle.Top;
            notesPanel.Controls.Add(txtNotes);
            notesPanel.Controls.Add(notesLbl);
            grid.RowCount++;
            grid.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            grid.Controls.Add(notesPanel, 0, grid.RowCount - 1);
            grid.SetColumnSpan(notesPanel, 2);

            this.Controls.Add(grid);

            btnCalculate.Text = "Calculate";
            btnCalculate.Click += BtnCalculate_Click;
            btnSave.Text = "Save";
            btnSave.Click += BtnSave_Click;
            btnCancel.Text = "Cancel";
            btnCancel.Click += BtnCancel_Click;

            this.ClientSize = new System.Drawing.Size(960, 600);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Text = "Create New Rental";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ResumeLayout(false);
        }

        private void LoadCustomers()
        {
            try
            {
                DataTable dt = DatabaseHelper.ExecuteStoredProcedure("sp_GetAllCustomers");
                // Add a computed column for full name
                dt.Columns.Add("full_name", typeof(string));
                foreach (DataRow row in dt.Rows)
                {
                    row["full_name"] = $"{row["first_name"]} {row["last_name"]}";
                }
                cmbCustomer.DataSource = dt;
                cmbCustomer.DisplayMember = "full_name";
                cmbCustomer.ValueMember = "customer_id";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading customers: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadAvailableVehicles()
        {
            try
            {
                DataTable dt = DatabaseHelper.ExecuteStoredProcedure("sp_GetAvailableVehicles");
                // Add a computed column for display
                dt.Columns.Add("display_text", typeof(string));
                foreach (DataRow row in dt.Rows)
                {
                    row["display_text"] = $"{row["make"]} {row["model"]} ({row["license_plate"]})";
                }
                cmbVehicle.DataSource = dt;
                cmbVehicle.DisplayMember = "display_text";
                cmbVehicle.ValueMember = "vehicle_id";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading vehicles: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CmbVehicle_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbVehicle.SelectedValue != null)
            {
                try
                {
                    var parameters = new MySqlParameter[]
                    {
                        new MySqlParameter("@p_vehicle_id", cmbVehicle.SelectedValue)
                    };
                    DataTable dt = DatabaseHelper.ExecuteStoredProcedure("sp_GetVehicle", parameters);
                    if (dt.Rows.Count > 0)
                    {
                        numDailyRate.Value = Convert.ToDecimal(dt.Rows[0]["daily_rate"]);
                        CalculateTotal();
                    }
                }
                catch { }
            }
        }

        private void DtpReturnDate_ValueChanged(object sender, EventArgs e)
        {
            TimeSpan diff = dtpReturnDate.Value - dtpRentalDate.Value;
            if (diff.Days > 0)
            {
                numDays.Value = diff.Days;
            }
            CalculateTotal();
        }

        private void NumDays_ValueChanged(object sender, EventArgs e)
        {
            dtpReturnDate.Value = dtpRentalDate.Value.AddDays((double)numDays.Value);
            CalculateTotal();
        }

        private void NumDiscount_ValueChanged(object sender, EventArgs e)
        {
            CalculateTotal();
        }

        private void BtnCalculate_Click(object sender, EventArgs e)
        {
            CalculateTotal();
        }

        private void CalculateTotal()
        {
            decimal subtotal = (decimal)numDays.Value * numDailyRate.Value;
            decimal total = subtotal - numDiscount.Value;
            lblSubtotal.Text = $"${subtotal:F2}";
            lblTotal.Text = $"${total:F2}";
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (ValidateInput())
            {
                try
                {
                    CalculateTotal();
                    decimal subtotal = (decimal)numDays.Value * numDailyRate.Value;
                    decimal total = subtotal - numDiscount.Value;

                    var parameters = new MySqlParameter[]
                    {
                        new MySqlParameter("@p_reservation_id", DBNull.Value),
                        new MySqlParameter("@p_customer_id", cmbCustomer.SelectedValue),
                        new MySqlParameter("@p_vehicle_id", cmbVehicle.SelectedValue),
                        new MySqlParameter("@p_picked_up_by", CurrentUser.UserId),
                        new MySqlParameter("@p_pickup_date", dtpRentalDate.Value),
                        new MySqlParameter("@p_expected_return_date", dtpReturnDate.Value),
                        new MySqlParameter("@p_initial_mileage", 0),
                        new MySqlParameter("@p_total_amount", total)
                    };

                    DatabaseHelper.ExecuteStoredProcedure("sp_CreateRental", parameters);
                    MessageBox.Show("Rental created successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error creating rental: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private bool ValidateInput()
        {
            if (cmbCustomer.SelectedValue == null)
            {
                MessageBox.Show("Please select a customer.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (cmbVehicle.SelectedValue == null)
            {
                MessageBox.Show("Please select a vehicle.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
