using System;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace VehicleRentalSystem
{
    public partial class CustomerForm : Form
    {
        private int? customerId;
        private TextBox txtFirstName;
        private TextBox txtLastName;
        private TextBox txtEmail;
        private TextBox txtPhone;
        private TextBox txtAddress;
        private TextBox txtCity;
        private TextBox txtState;
        private TextBox txtZipCode;
        private TextBox txtDriverLicense;
        private DateTimePicker dtpLicenseExpiry;
        private DateTimePicker dtpDateOfBirth;
        private CheckBox chkIsActive;
        private Button btnSave;
        private Button btnCancel;

        private Panel panelModal;
        private Label lblTitle;
        private Button btnCloseX;
        private Panel panelContent;

        public CustomerForm(int? customerId = null)
        {
            this.customerId = customerId;
            InitializeComponent();
            ThemeHelper.ApplyTheme(this);
            ThemeHelper.ApplyButtonTheme(btnSave, isPrimary: true);
            ThemeHelper.ApplyButtonTheme(btnCancel, isSecondary: true);

            // ApplyButtonTheme uses large padding which can clip text in our compact footer.
            // Re-apply compact footer sizing here.
            try
            {
                btnSave.UseCompatibleTextRendering = true;
                btnCancel.UseCompatibleTextRendering = true;
            }
            catch { }

            btnSave.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnSave.Height = 40;
            btnSave.Padding = new Padding(12, 6, 12, 6);
            btnSave.Width = Math.Min(260, Math.Max(140, TextRenderer.MeasureText(btnSave.Text, btnSave.Font).Width + 40));

            btnCancel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnCancel.Height = 40;
            btnCancel.Padding = new Padding(12, 6, 12, 6);
            btnCancel.Width = Math.Min(200, Math.Max(120, TextRenderer.MeasureText(btnCancel.Text, btnCancel.Font).Width + 40));

            this.DoubleBuffered = true;
            if (customerId.HasValue)
            {
                LoadCustomer(customerId.Value);
            }
        }

        private void InitializeComponent()
        {
            this.txtFirstName = new TextBox();
            this.txtLastName = new TextBox();
            this.txtEmail = new TextBox();
            this.txtPhone = new TextBox();
            this.txtAddress = new TextBox();
            this.txtCity = new TextBox();
            this.txtState = new TextBox();
            this.txtZipCode = new TextBox();
            this.txtDriverLicense = new TextBox();
            this.dtpLicenseExpiry = new DateTimePicker();
            this.dtpDateOfBirth = new DateTimePicker();
            this.chkIsActive = new CheckBox();
            this.btnSave = new Button();
            this.btnCancel = new Button();

            this.SuspendLayout();

            // Modal Panel (White Card)
            this.panelModal = new Panel();
            this.panelModal.BackColor = Color.White;
            this.panelModal.BorderStyle = BorderStyle.FixedSingle;
            // Fill the dialog (no gray backdrop around it)
            this.panelModal.Dock = DockStyle.Fill;
            this.panelModal.Padding = new Padding(0);

            // Header Panel
            Panel panelHeader = new Panel();
            panelHeader.BackColor = Color.White;
            panelHeader.BorderStyle = BorderStyle.None;
            panelHeader.Dock = DockStyle.Top;
            panelHeader.Height = 70;
            panelHeader.Padding = new Padding(24, 20, 24, 0);

            this.lblTitle = new Label();
            this.lblTitle.Text = customerId.HasValue ? "Edit Customer" : "Add New Customer";
            this.lblTitle.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            this.lblTitle.ForeColor = ThemeHelper.TextPrimaryColor;
            this.lblTitle.Location = new System.Drawing.Point(24, 20);
            this.lblTitle.Size = new System.Drawing.Size(600, 30);
            panelHeader.Controls.Add(this.lblTitle);

            this.btnCloseX = new Button();
            this.btnCloseX.Text = "✕";
            this.btnCloseX.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            this.btnCloseX.ForeColor = ThemeHelper.TextSecondaryColor;
            this.btnCloseX.BackColor = Color.White;
            this.btnCloseX.FlatStyle = FlatStyle.Flat;
            this.btnCloseX.FlatAppearance.BorderSize = 0;
            this.btnCloseX.FlatAppearance.MouseOverBackColor = ThemeHelper.ButtonHoverColor;
            this.btnCloseX.FlatAppearance.MouseDownBackColor = ThemeHelper.ButtonSecondaryColor;
            this.btnCloseX.Location = new System.Drawing.Point(850, 15);
            this.btnCloseX.Size = new System.Drawing.Size(30, 30);
            this.btnCloseX.Cursor = Cursors.Hand;
            this.btnCloseX.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };
            panelHeader.Controls.Add(this.btnCloseX);

            this.panelModal.Controls.Add(panelHeader);

            // Content Panel (no scroll; two-column grid so everything fits)
            this.panelContent = new Panel();
            this.panelContent.BackColor = Color.White;
            this.panelContent.Dock = DockStyle.Fill;
            this.panelContent.Padding = new Padding(24);
            // Scroll when needed
            this.panelContent.AutoScroll = true;

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

                if (input is TextBox tb)
                {
                    tb.Multiline = true;
                    tb.Height = 36;
                }
                else if (input is DateTimePicker dtp)
                {
                    dtp.Height = 36;
                }

                input.Dock = DockStyle.Top;
                p.Controls.Add(input);
                p.Controls.Add(lbl);
                return p;
            }

            Panel Section(string text)
            {
                return new Panel
                {
                    Height = 28,
                    Dock = DockStyle.Top,
                    Margin = new Padding(0, 0, 0, 12),
                    Controls =
                    {
                        new Label
                        {
                            Text = text,
                            Dock = DockStyle.Fill,
                            Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                            ForeColor = ThemeHelper.TextSecondaryColor
                        }
                    }
                };
            }

            var grid = new TableLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 2,
                RowCount = 0,
                Dock = DockStyle.Top
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

            // Personal
            var personalHeader = new Label
            {
                Text = "PERSONAL INFORMATION",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = ThemeHelper.TextSecondaryColor,
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 12)
            };
            grid.RowCount++;
            grid.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            grid.Controls.Add(personalHeader, 0, grid.RowCount - 1);
            grid.SetColumnSpan(personalHeader, 2);

            AddRow(Field("First Name", txtFirstName), Field("Last Name", txtLastName));
            AddRow(Field("Email", txtEmail), Field("Phone", txtPhone));
            AddRow(Field("Address", txtAddress), Field("City", txtCity));
            AddRow(Field("State", txtState), Field("Zip Code", txtZipCode));

            // License
            var licenseHeader = new Label
            {
                Text = "DRIVER'S LICENSE",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = ThemeHelper.TextSecondaryColor,
                AutoSize = true,
                Margin = new Padding(0, 12, 0, 12)
            };
            grid.RowCount++;
            grid.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            grid.Controls.Add(licenseHeader, 0, grid.RowCount - 1);
            grid.SetColumnSpan(licenseHeader, 2);

            dtpLicenseExpiry.Format = DateTimePickerFormat.Short;
            dtpDateOfBirth.Format = DateTimePickerFormat.Short;

            AddRow(Field("License Number", txtDriverLicense), Field("Expiry Date", dtpLicenseExpiry));
            AddRow(Field("Date of Birth", dtpDateOfBirth), new Panel { Height = 72, Margin = new Padding(0, 0, 0, 12), Dock = DockStyle.Fill });

            // Active toggle
            chkIsActive.Text = "Active";
            chkIsActive.AutoSize = true;
            var activeWrap = new Panel { Height = 36, Margin = new Padding(0, 0, 0, 0) };
            activeWrap.Controls.Add(chkIsActive);
            grid.RowCount++;
            grid.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            grid.Controls.Add(activeWrap, 0, grid.RowCount - 1);
            grid.SetColumnSpan(activeWrap, 2);

            this.panelContent.Controls.Add(grid);
            this.panelModal.Controls.Add(this.panelContent);

            // Footer Panel with Buttons
            Panel panelFooter = new Panel();
            panelFooter.BackColor = Color.White;
            panelFooter.BorderStyle = BorderStyle.None;
            panelFooter.Dock = DockStyle.Bottom;
            // Ensure there's enough vertical space for 40px buttons (avoid clipping)
            panelFooter.Height = 80;
            panelFooter.Padding = new Padding(24, 20, 24, 20);

            // Right-aligned footer buttons (simple + reliable even when content scrolls)
            var footerRight = new FlowLayoutPanel();
            footerRight.Dock = DockStyle.Right;
            footerRight.AutoSize = true;
            footerRight.WrapContents = false;
            footerRight.FlowDirection = FlowDirection.LeftToRight;
            footerRight.BackColor = Color.White;
            footerRight.Margin = new Padding(0);
            footerRight.Padding = new Padding(0);

            btnSave.Text = "Save Customer";
            btnSave.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnSave.Height = 40;
            btnSave.Padding = new Padding(12, 6, 12, 6);
            btnSave.Width = Math.Min(260, Math.Max(140, TextRenderer.MeasureText(btnSave.Text, btnSave.Font).Width + 40));
            btnSave.Margin = new Padding(8, 0, 0, 0);
            btnSave.Click += BtnSave_Click;

            btnCancel.Text = "Cancel";
            btnCancel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnCancel.Height = 40;
            btnCancel.Padding = new Padding(12, 6, 12, 6);
            btnCancel.Width = Math.Min(200, Math.Max(120, TextRenderer.MeasureText(btnCancel.Text, btnCancel.Font).Width + 40));
            btnCancel.Margin = new Padding(0, 0, 0, 0);
            btnCancel.Click += BtnCancel_Click;

            // Order: Cancel then Save
            footerRight.Controls.Add(btnCancel);
            footerRight.Controls.Add(btnSave);

            panelFooter.Controls.Add(footerRight);

            this.panelModal.Controls.Add(panelFooter);
            panelFooter.BringToFront();

            // Form Setup
            this.Controls.Add(this.panelModal);
            // Normal dialog: no dimmed overlay/background
            this.BackColor = ThemeHelper.BackgroundColor;
            this.ClientSize = new System.Drawing.Size(900, 700);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.WindowState = FormWindowState.Normal;

            this.ResumeLayout(false);
        }

        private void LoadCustomer(int id)
        {
            try
            {
                var parameters = new MySqlParameter[]
                {
                    new MySqlParameter("@p_customer_id", id)
                };
                DataTable dt = DatabaseHelper.ExecuteStoredProcedure("sp_GetCustomer", parameters);
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    txtFirstName.Text = row["first_name"].ToString();
                    txtLastName.Text = row["last_name"].ToString();
                    txtEmail.Text = row["email"].ToString();
                    txtPhone.Text = row["phone"].ToString();
                    txtAddress.Text = row["address"].ToString();
                    txtCity.Text = row["city"].ToString();
                    txtState.Text = row["state"].ToString();
                    txtZipCode.Text = row["zip_code"].ToString();
                    txtDriverLicense.Text = row["driver_license_number"].ToString();
                    if (row["driver_license_expiry"] != DBNull.Value)
                        dtpLicenseExpiry.Value = Convert.ToDateTime(row["driver_license_expiry"]);
                    if (row["date_of_birth"] != DBNull.Value)
                        dtpDateOfBirth.Value = Convert.ToDateTime(row["date_of_birth"]);
                    chkIsActive.Checked = Convert.ToBoolean(row["is_active"]);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading customer: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (ValidateInput())
            {
                try
                {
                    if (customerId.HasValue)
                    {
                        var parameters = new MySqlParameter[]
                        {
                            new MySqlParameter("@p_customer_id", customerId.Value),
                            new MySqlParameter("@p_first_name", txtFirstName.Text),
                            new MySqlParameter("@p_last_name", txtLastName.Text),
                            new MySqlParameter("@p_email", txtEmail.Text),
                            new MySqlParameter("@p_phone", txtPhone.Text),
                            new MySqlParameter("@p_address", txtAddress.Text),
                            new MySqlParameter("@p_city", txtCity.Text),
                            new MySqlParameter("@p_state", txtState.Text),
                            new MySqlParameter("@p_zip_code", txtZipCode.Text),
                            new MySqlParameter("@p_license_number", txtDriverLicense.Text),
                            new MySqlParameter("@p_license_expiry", dtpLicenseExpiry.Value),
                            new MySqlParameter("@p_date_of_birth", dtpDateOfBirth.Value),
                            new MySqlParameter("@p_is_active", chkIsActive.Checked)
                        };
                        DatabaseHelper.ExecuteStoredProcedure("sp_UpdateCustomer", parameters);
                    }
                    else
                    {
                        var parameters = new MySqlParameter[]
                        {
                            new MySqlParameter("@p_first_name", txtFirstName.Text),
                            new MySqlParameter("@p_last_name", txtLastName.Text),
                            new MySqlParameter("@p_email", txtEmail.Text),
                            new MySqlParameter("@p_phone", txtPhone.Text),
                            new MySqlParameter("@p_address", txtAddress.Text),
                            new MySqlParameter("@p_city", txtCity.Text),
                            new MySqlParameter("@p_state", txtState.Text),
                            new MySqlParameter("@p_zip_code", txtZipCode.Text),
                            new MySqlParameter("@p_license_number", txtDriverLicense.Text),
                            new MySqlParameter("@p_license_expiry", dtpLicenseExpiry.Value),
                            new MySqlParameter("@p_date_of_birth", dtpDateOfBirth.Value)
                        };
                        DatabaseHelper.ExecuteStoredProcedure("sp_CreateCustomer", parameters);
                    }

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving customer: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtFirstName.Text))
            {
                MessageBox.Show("First name is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtLastName.Text))
            {
                MessageBox.Show("Last name is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                MessageBox.Show("Phone number is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtDriverLicense.Text))
            {
                MessageBox.Show("Driver license number is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
