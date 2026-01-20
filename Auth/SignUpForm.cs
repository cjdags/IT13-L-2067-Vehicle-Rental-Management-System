using System;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace VehicleRentalSystem
{
    public partial class SignUpForm : Form
    {
        public string NewUsername { get; private set; } = "";
        public string NewPassword { get; private set; } = "";

        private TextBox txtUsername;
        private TextBox txtPassword;
        private TextBox txtEmail;
        private TextBox txtFirstName;
        private TextBox txtLastName;
        private TextBox txtPhone;
        private ComboBox cmbRole;
        private Button btnSave;
        private Button btnCancel;

        public SignUpForm()
        {
            InitializeComponent();
            ThemeHelper.ApplyBaseTheme(this);
            ThemeHelper.ApplyCardDialogLayout(this, "Create Account", new[] { btnSave, btnCancel });
        }

        private void InitializeComponent()
        {
            this.txtUsername = new TextBox();
            this.txtPassword = new TextBox();
            this.txtEmail = new TextBox();
            this.txtFirstName = new TextBox();
            this.txtLastName = new TextBox();
            this.txtPhone = new TextBox();
            this.cmbRole = new ComboBox();
            this.btnSave = new Button();
            this.btnCancel = new Button();

            this.SuspendLayout();

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
                else if (input is ComboBox cb)
                {
                    cb.Height = 36;
                }

                input.Dock = DockStyle.Top;
                p.Controls.Add(input);
                p.Controls.Add(lbl);
                return p;
            }

            // Configure inputs
            txtPassword.PasswordChar = '*';
            cmbRole.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbRole.Items.AddRange(new string[] { "Admin", "Rental Agent" });
            cmbRole.SelectedIndex = 1; // Default to Rental Agent

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

            AddRow(Field("Username", txtUsername), Field("Password", txtPassword));
            AddRow(Field("Email", txtEmail), Field("Phone (optional)", txtPhone));
            AddRow(Field("First Name", txtFirstName), Field("Last Name", txtLastName));
            AddRow(Field("Role", cmbRole), new Panel { Height = 72, Margin = new Padding(0, 0, 0, 12), Dock = DockStyle.Fill });

            this.Controls.Add(grid);

            btnSave.Text = "Create Account";
            btnSave.Click += BtnSave_Click;
            btnCancel.Text = "Cancel";
            btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            this.ClientSize = new System.Drawing.Size(960, 520);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;

            this.ResumeLayout(false);
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateInput()) return;

            try
            {
                var parameters = new MySqlParameter[]
                {
                    new MySqlParameter("@p_username", txtUsername.Text.Trim()),
                    new MySqlParameter("@p_password_hash", txtPassword.Text), // Note: in production, hash this
                    new MySqlParameter("@p_email", txtEmail.Text.Trim()),
                    new MySqlParameter("@p_first_name", txtFirstName.Text.Trim()),
                    new MySqlParameter("@p_last_name", txtLastName.Text.Trim()),
                    new MySqlParameter("@p_phone", txtPhone.Text.Trim()),
                    new MySqlParameter("@p_role", cmbRole.Text),
                    new MySqlParameter("@p_status", "Active")
                };

                DatabaseHelper.ExecuteStoredProcedure("sp_CreateUser", parameters);

                NewUsername = txtUsername.Text.Trim();
                NewPassword = txtPassword.Text;

                MessageBox.Show("Account created successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating account: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageBox.Show("Username is required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Password is required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Email is required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtFirstName.Text))
            {
                MessageBox.Show("First name is required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtLastName.Text))
            {
                MessageBox.Show("Last name is required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }
    }
}
