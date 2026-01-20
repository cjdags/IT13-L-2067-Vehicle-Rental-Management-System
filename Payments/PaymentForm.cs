using System;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace VehicleRentalSystem
{
    public partial class PaymentForm : Form
    {
        private int? paymentId;
        private ComboBox cmbRental;
        private ComboBox cmbReservation;
        private ComboBox cmbPaymentType;
        private ComboBox cmbPaymentMethod;
        private NumericUpDown numAmount;
        private TextBox txtTransactionReference;
        private ComboBox cmbStatus;
        private Button btnSave;
        private Button btnCancel;

        public PaymentForm(int? paymentId = null)
        {
            this.paymentId = paymentId;
            InitializeComponent();
            ThemeHelper.ApplyTheme(this);
            ThemeHelper.ApplyCardDialogLayout(
                this,
                title: paymentId.HasValue ? "Edit Payment" : "Add New Payment",
                footerRightButtons: new[] { btnSave, btnCancel }
            );
            LoadRentals();
            LoadReservations();
            if (paymentId.HasValue)
            {
                LoadPayment(paymentId.Value);
            }
        }

        private void InitializeComponent()
        {
            this.cmbRental = new ComboBox();
            this.cmbReservation = new ComboBox();
            this.cmbPaymentType = new ComboBox();
            this.cmbPaymentMethod = new ComboBox();
            this.numAmount = new NumericUpDown();
            this.txtTransactionReference = new TextBox();
            this.cmbStatus = new ComboBox();
            this.btnSave = new Button();
            this.btnCancel = new Button();

            this.SuspendLayout();

            cmbRental.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbReservation.DropDownStyle = ComboBoxStyle.DropDownList;

            cmbPaymentType.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbPaymentType.Items.AddRange(new string[] { "Deposit", "Full Payment", "Refund", "Additional Charge" });
            cmbPaymentType.SelectedIndex = 1;

            cmbPaymentMethod.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbPaymentMethod.Items.AddRange(new string[] { "Cash", "Credit Card", "Debit Card", "Bank Transfer" });
            cmbPaymentMethod.SelectedIndex = 0;

            cmbStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbStatus.Items.AddRange(new string[] { "Pending", "Completed", "Failed", "Refunded" });
            cmbStatus.SelectedIndex = 1;

            numAmount.DecimalPlaces = 2;
            numAmount.Minimum = 0;
            numAmount.Maximum = 100000;

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
                else if (input is NumericUpDown nud)
                {
                    nud.Height = 36;
                }

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

            AddRow(Field("Rental (optional)", cmbRental), Field("Reservation (optional)", cmbReservation));
            AddRow(Field("Payment Type", cmbPaymentType), Field("Payment Method", cmbPaymentMethod));
            AddRow(Field("Amount", numAmount), Field("Status", cmbStatus));

            // Transaction reference spans full width
            var refPanel = Field("Transaction Reference", txtTransactionReference);
            refPanel.Margin = new Padding(0, 0, 0, 12);
            grid.RowCount++;
            grid.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            grid.Controls.Add(refPanel, 0, grid.RowCount - 1);
            grid.SetColumnSpan(refPanel, 2);

            this.Controls.Add(grid);

            btnSave.Text = "Save";
            btnSave.Click += BtnSave_Click;
            btnCancel.Text = "Cancel";
            btnCancel.Click += BtnCancel_Click;

            this.ClientSize = new System.Drawing.Size(960, 520);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Text = paymentId.HasValue ? "Edit Payment" : "Add New Payment";
            this.StartPosition = FormStartPosition.CenterScreen;

            this.ResumeLayout(false);
        }

        private void LoadRentals()
        {
            try
            {
                DataTable dt = DatabaseHelper.ExecuteStoredProcedure("sp_GetAllRentals");
                dt.Columns.Add("display_text", typeof(string));
                
                foreach (DataRow row in dt.Rows)
                {
                    row["display_text"] = $"Rental #{row["rental_id"]} - {row["customer_first_name"]} {row["customer_last_name"]} - {row["vehicle_make"]} {row["vehicle_model"]}";
                }

                DataRow emptyRow = dt.NewRow();
                emptyRow["rental_id"] = DBNull.Value;
                emptyRow["display_text"] = "-- Select Rental (Optional) --";
                dt.Rows.InsertAt(emptyRow, 0);

                cmbRental.DataSource = dt;
                cmbRental.DisplayMember = "display_text";
                cmbRental.ValueMember = "rental_id";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading rentals: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadReservations()
        {
            try
            {
                DataTable dt = DatabaseHelper.ExecuteStoredProcedure("sp_GetAllReservations");
                dt.Columns.Add("display_text", typeof(string));
                
                foreach (DataRow row in dt.Rows)
                {
                    row["display_text"] = $"Reservation #{row["reservation_id"]} - {row["customer_first_name"]} {row["customer_last_name"]} - {row["vehicle_make"]} {row["vehicle_model"]}";
                }

                DataRow emptyRow = dt.NewRow();
                emptyRow["reservation_id"] = DBNull.Value;
                emptyRow["display_text"] = "-- Select Reservation (Optional) --";
                dt.Rows.InsertAt(emptyRow, 0);

                cmbReservation.DataSource = dt;
                cmbReservation.DisplayMember = "display_text";
                cmbReservation.ValueMember = "reservation_id";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading reservations: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadPayment(int id)
        {
            try
            {
                var parameters = new MySqlParameter[]
                {
                    new MySqlParameter("@p_payment_id", id)
                };
                DataTable dt = DatabaseHelper.ExecuteStoredProcedure("sp_GetPayment", parameters);
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    if (row["rental_id"] != DBNull.Value)
                        cmbRental.SelectedValue = Convert.ToInt32(row["rental_id"]);
                    if (row["reservation_id"] != DBNull.Value)
                        cmbReservation.SelectedValue = Convert.ToInt32(row["reservation_id"]);
                    cmbPaymentType.Text = row["payment_type"].ToString();
                    cmbPaymentMethod.Text = row["payment_method"].ToString();
                    numAmount.Value = Convert.ToDecimal(row["amount"]);
                    txtTransactionReference.Text = row["transaction_reference"]?.ToString() ?? "";
                    cmbStatus.Text = row["status"].ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading payment: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (ValidateInput())
            {
                try
                {
                    object rentalIdValue = cmbRental.SelectedValue;
                    object reservationIdValue = cmbReservation.SelectedValue;

                    if (rentalIdValue == DBNull.Value || rentalIdValue == null)
                        rentalIdValue = DBNull.Value;
                    if (reservationIdValue == DBNull.Value || reservationIdValue == null)
                        reservationIdValue = DBNull.Value;

                    var parameters = new MySqlParameter[]
                    {
                        new MySqlParameter("@p_rental_id", rentalIdValue),
                        new MySqlParameter("@p_reservation_id", reservationIdValue),
                        new MySqlParameter("@p_payment_type", cmbPaymentType.Text),
                        new MySqlParameter("@p_payment_method", cmbPaymentMethod.Text),
                        new MySqlParameter("@p_amount", numAmount.Value),
                        new MySqlParameter("@p_transaction_reference", txtTransactionReference.Text),
                        new MySqlParameter("@p_status", cmbStatus.Text),
                        new MySqlParameter("@p_processed_by", CurrentUser.UserId)
                    };

                    if (paymentId.HasValue)
                    {
                        // Update existing payment
                        var updateParams = new MySqlParameter[]
                        {
                            new MySqlParameter("@p_payment_id", paymentId.Value),
                            new MySqlParameter("@p_payment_type", cmbPaymentType.Text),
                            new MySqlParameter("@p_payment_method", cmbPaymentMethod.Text),
                            new MySqlParameter("@p_amount", numAmount.Value),
                            new MySqlParameter("@p_payment_date", DateTime.Now),
                            new MySqlParameter("@p_transaction_reference", txtTransactionReference.Text),
                            new MySqlParameter("@p_status", cmbStatus.Text)
                        };
                        DatabaseHelper.ExecuteStoredProcedure("sp_UpdatePayment", updateParams);
                    }
                    else
                    {
                        // Create new payment
                        DatabaseHelper.ExecuteStoredProcedure("sp_CreatePayment", parameters);
                    }

                    MessageBox.Show("Payment saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving payment: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private bool ValidateInput()
        {
            if (cmbRental.SelectedValue == null && cmbReservation.SelectedValue == null)
            {
                MessageBox.Show("Please select either a rental or reservation.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (numAmount.Value <= 0)
            {
                MessageBox.Show("Amount must be greater than zero.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
