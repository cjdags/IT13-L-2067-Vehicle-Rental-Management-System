using System;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace VehicleRentalSystem
{
    public partial class ReturnRentalForm : Form
    {
        private int rentalId;
        private Label lblRentalInfo;
        private DateTimePicker dtpActualReturn;
        private NumericUpDown numReturnMileage;
        private NumericUpDown numInitialMileage;
         private NumericUpDown numLateFee;
        private NumericUpDown numMileageOverage;
        private NumericUpDown numFuelCharge;
        private NumericUpDown numCleaningFee;
        private NumericUpDown numDamageCharge;
        private NumericUpDown numTollCharge;
        private NumericUpDown numOtherCharge;
        private NumericUpDown numMileageRate;
        private NumericUpDown numFuelPercent;
        private NumericUpDown numCleanliness;
        private Label lblBaseTotal; 
        private Label lblGrandTotal;
        private Label lblChargesBreakdown;
        private Button btnSave;
        private Button btnCancel;
        private Button btnInspection;
        private decimal baseTotalCached = 0m;
        private int initialMileageCached = 0;
        private DateTime expectedReturnCached = DateTime.Now;

        public ReturnRentalForm(int rentalId)
        {
            this.rentalId = rentalId;
            InitializeComponent();
            ThemeHelper.ApplyTheme(this);
            ThemeHelper.ApplyCardDialogLayout(
                this,
                title: "Return Vehicle",
                footerRightButtons: new[] { btnSave, btnCancel },
                footerLeftButtons: new[] { btnInspection }
            );
            LoadRentalInfo();
        }

        private void InitializeComponent()
        {
            this.lblRentalInfo = new Label();
            this.dtpActualReturn = new DateTimePicker();
            this.numReturnMileage = new NumericUpDown();
            this.numInitialMileage = new NumericUpDown();
            this.numLateFee = new NumericUpDown();
            this.numMileageOverage = new NumericUpDown();
            this.numFuelCharge = new NumericUpDown();
            this.numCleaningFee = new NumericUpDown();
            this.numDamageCharge = new NumericUpDown();
            this.numTollCharge = new NumericUpDown();
            this.numOtherCharge = new NumericUpDown();
            this.numMileageRate = new NumericUpDown();
            this.numFuelPercent = new NumericUpDown();
            this.numCleanliness = new NumericUpDown();
            this.lblBaseTotal = new Label();
            this.lblGrandTotal = new Label();
            this.lblChargesBreakdown = new Label();
            this.btnSave = new Button();
            this.btnCancel = new Button();
            this.btnInspection = new Button();

            this.SuspendLayout();

            int labelWidth = 180;

            // Configure numeric ranges
            foreach (var nud in new[] { numLateFee, numMileageOverage, numMileageRate, numFuelCharge, numCleaningFee, numDamageCharge, numTollCharge, numOtherCharge })
            {
                nud.DecimalPlaces = 2;
                nud.Maximum = 1000000;
                nud.Minimum = 0;
                nud.Height = 36;
                nud.ValueChanged += (_, __) => RecalculateTotals();
            }

            numReturnMileage.Maximum = 10000000;
            numInitialMileage.Maximum = 10000000;
            numFuelPercent.Maximum = 100;
            numFuelPercent.Height = 36;
            numFuelPercent.Value = 100;
            numFuelPercent.ValueChanged += (_, __) => RecalculateTotals();
            numCleanliness.Maximum = 5;
            numCleanliness.Minimum = 1;
            numCleanliness.Value = 3;
            numCleanliness.Height = 36;
            numCleanliness.ValueChanged += (_, __) => RecalculateTotals();

            // Layout using table
            Panel Field(string label, Control input)
            {
                var p = new Panel { Height = 72, Margin = new Padding(0, 0, 24, 12), Dock = DockStyle.Fill };
                var lbl = new Label { Text = label, AutoSize = false, Height = 18, Dock = DockStyle.Top, Font = ThemeHelper.NormalFont, ForeColor = ThemeHelper.TextColor };
                if (input is DateTimePicker or NumericUpDown)
                    input.Height = 36;
                input.Dock = DockStyle.Top;
                p.Controls.Add(input);
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

            lblRentalInfo.AutoSize = true;
            lblRentalInfo.Dock = DockStyle.Top;
            lblRentalInfo.Padding = new Padding(0, 0, 0, 12);

            AddRow(Field("Actual Return Date", dtpActualReturn = new DateTimePicker { Format = DateTimePickerFormat.Custom, CustomFormat = "MM/dd/yyyy HH:mm" }),
                  Field("Initial Mileage", numInitialMileage));
            AddRow(Field("Return Mileage", numReturnMileage), Field("Late Fee", numLateFee));
            AddRow(Field("Mileage Overage Rate (/mile)", numMileageRate), Field("Fuel Charge", numFuelCharge));
            AddRow(Field("Fuel Level (%)", numFuelPercent), Field("Cleanliness (1-5)", numCleanliness));
            AddRow(Field("Cleaning Fee", numCleaningFee), Field("Damage Charge", numDamageCharge));
            AddRow(Field("Toll Charge", numTollCharge), Field("Other Charge", numOtherCharge));

            var totalsPanel = new Panel { Height = 110 , Margin = new Padding(0, 0, 0, 30), Padding = new Padding (0,0,0,10),Dock = DockStyle.Fill };
            var baseLbl = new Label { Text = "Base Total", Dock = DockStyle.Top, Height = 18, Font = ThemeHelper.NormalFont, ForeColor = ThemeHelper.TextColor };
            lblBaseTotal.Dock = DockStyle.Top;
            lblBaseTotal.Height = 24;
            lblGrandTotal.Dock = DockStyle.Top;
            lblGrandTotal.Height = 100;
            lblGrandTotal.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            lblChargesBreakdown.Dock = DockStyle.Top;
            lblChargesBreakdown.Height = 28;
            lblChargesBreakdown.Font = ThemeHelper.SmallFont;
            totalsPanel.Controls.Add(lblGrandTotal);
            totalsPanel.Controls.Add(lblChargesBreakdown);
            totalsPanel.Controls.Add(lblBaseTotal);
            totalsPanel.Controls.Add(baseLbl);
            grid.RowCount++;
            grid.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            grid.Controls.Add(totalsPanel, 0, grid.RowCount - 1);
            grid.SetColumnSpan(totalsPanel, 2);

            btnSave.Text = "Complete Return";
            btnSave.Click += BtnSave_Click;
            btnCancel.Text = "Cancel";
            btnCancel.Click += BtnCancel_Click;
            btnInspection.Text = "Return Inspection";
            btnInspection.Click += BtnInspection_Click;

            Controls.Add(grid);
            Controls.Add(lblRentalInfo);

            ClientSize = new System.Drawing.Size(820, 700);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            Text = "Return Vehicle";
            StartPosition = FormStartPosition.CenterScreen;
            ResumeLayout(false);
        }

        private void LoadRentalInfo()
        {
            try
            {
                var parameters = new MySqlParameter[]
                {
                    new MySqlParameter("@p_rental_id", rentalId)
                };
                DataTable dt = DatabaseHelper.ExecuteStoredProcedure("sp_GetRental", parameters);
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    baseTotalCached = Convert.ToDecimal(row["total_amount"]);
                    lblBaseTotal.Text = $"Base: ${baseTotalCached:F2}";
                    lblGrandTotal.Text = $"Grand Total: ${baseTotalCached:F2}";
                    expectedReturnCached = Convert.ToDateTime(row["expected_return_date"]);

                    lblRentalInfo.Text = $"Rental ID: {row["rental_id"]}\n" +
                                       $"Customer: {row["customer_first_name"]} {row["customer_last_name"]}\n" +
                                       $"Vehicle: {row["vehicle_make"]} {row["vehicle_model"]} ({row["license_plate"]})\n" +
                                       $"Pickup Date: {Convert.ToDateTime(row["pickup_date"]):MM/dd/yyyy}\n" +
                                       $"Expected Return: {Convert.ToDateTime(row["expected_return_date"]):MM/dd/yyyy}\n" +
                                       $"Base Total: ${baseTotalCached:F2}";
                    
                    if (row["initial_mileage"] != DBNull.Value)
                    {
                        initialMileageCached = Convert.ToInt32(row["initial_mileage"]);
                        numInitialMileage.Value = initialMileageCached;
                        numReturnMileage.Value = initialMileageCached;
                    }
                    RecalculateTotals();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading rental info: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                decimal chargesTotal = AddCharges(rentalId);
                decimal grandTotal = baseTotalCached + chargesTotal;

                var parameters = new MySqlParameter[]
                {
                    new MySqlParameter("@p_rental_id", rentalId),
                    new MySqlParameter("@p_expected_return_date", dtpActualReturn.Value),
                    new MySqlParameter("@p_actual_return_date", dtpActualReturn.Value),
                    new MySqlParameter("@p_return_mileage", (int)numReturnMileage.Value),
                    new MySqlParameter("@p_returned_to", CurrentUser.UserId),
                    new MySqlParameter("@p_status", "Completed"),
                    new MySqlParameter("@p_total_amount", grandTotal)
                };

                DatabaseHelper.ExecuteStoredProcedure("sp_UpdateRental", parameters);

                // Create invoice
                DatabaseHelper.ExecuteStoredProcedure(
                    "sp_CreateInvoice",
                    new MySqlParameter("@p_rental_id", rentalId),
                    new MySqlParameter("@p_subtotal", baseTotalCached + chargesTotal),
                    new MySqlParameter("@p_taxes", 0),
                    new MySqlParameter("@p_discounts", 0),
                    new MySqlParameter("@p_total", grandTotal),
                    new MySqlParameter("@p_balance_due", grandTotal),
                    new MySqlParameter("@p_issued_by", CurrentUser.UserId)
                );

                MessageBox.Show("Vehicle returned successfully with charges applied.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error returning vehicle: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private decimal AddCharges(int rentalId)
        {
            decimal total = 0;
            var breakdown = new List<string>();

            void AddCharge(string type, string desc, decimal qty, decimal unit)
            {
                if (unit <= 0 || qty == 0) return;
                total += qty * unit;
                breakdown.Add($"{type}: {qty} x {unit:C2}");
                DatabaseHelper.ExecuteStoredProcedure(
                    "sp_CreateRentalCharge",
                    new MySqlParameter("@p_rental_id", rentalId),
                    new MySqlParameter("@p_charge_type", type),
                    new MySqlParameter("@p_description", desc),
                    new MySqlParameter("@p_quantity", qty),
                    new MySqlParameter("@p_unit_amount", unit)
                );
            }

            if (numLateFee.Value > 0) AddCharge("LateFee", "Late return", 1, numLateFee.Value);

            int overMiles = (int)(numReturnMileage.Value - numInitialMileage.Value);
            if (overMiles > 0 && numMileageRate.Value > 0)
            {
                AddCharge("MileageOverage", $"Mileage overage ({overMiles} mi)", overMiles, numMileageRate.Value);
            }

            if (numFuelCharge.Value > 0) AddCharge("Fuel", "Fuel charge", 1, numFuelCharge.Value);
            if (numCleaningFee.Value > 0) AddCharge("Cleaning", "Cleaning", 1, numCleaningFee.Value);
            if (numDamageCharge.Value > 0) AddCharge("Damage", "Damage", 1, numDamageCharge.Value);
            if (numTollCharge.Value > 0) AddCharge("Toll", "Toll/violation", 1, numTollCharge.Value);
            if (numOtherCharge.Value > 0) AddCharge("Other", "Other", 1, numOtherCharge.Value);

            lblGrandTotal.Text = $"Grand Total: ${(total + baseTotalCached):F2}";
            lblChargesBreakdown.Text = breakdown.Count > 0 ? $"Charges: {string.Join("; ", breakdown)}" : "Charges: none";
            return total;
        }

        private void RecalculateTotals()
        {
            decimal charges = 0;
            charges += numLateFee.Value;
            int overMiles = (int)(numReturnMileage.Value - numInitialMileage.Value);
            if (overMiles > 0 && numMileageRate.Value > 0)
                charges += overMiles * numMileageRate.Value;
            charges += numFuelCharge.Value + numCleaningFee.Value + numDamageCharge.Value + numTollCharge.Value + numOtherCharge.Value;

            lblGrandTotal.Text = $"Grand Total: ${(baseTotalCached + charges):F2}";
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BtnInspection_Click(object? sender, EventArgs e)
        {
            using var insp = new InspectionForm(rentalId, "Return");
            insp.ShowDialog();
        }
    }
}
