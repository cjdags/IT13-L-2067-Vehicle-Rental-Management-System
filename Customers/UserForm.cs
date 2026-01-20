using System;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace VehicleRentalSystem
{
    public partial class UserForm : Form
    {
        private int? userId;
        private TextBox txtUsername;
        private TextBox txtPassword;
        private TextBox txtEmail;
        private TextBox txtFirstName;
        private TextBox txtLastName;
        private TextBox txtPhone;
        private ComboBox cmbRole;
        private ComboBox cmbStatus;
        private Button btnSave;
        private Button btnCancel;

        public UserForm(int? userId = null)
        {
            this.userId = userId;
            InitializeComponent();
            ThemeHelper.ApplyTheme(this);
            ThemeHelper.ApplyCardDialogLayout(
                this,
                title: userId.HasValue ? "Edit User" : "Add New User",
                footerRightButtons: new[] { btnSave, btnCancel }
            );
            if (userId.HasValue)
            {
                LoadUser(userId.Value);
            }
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
            this.cmbStatus = new ComboBox();
            this.btnSave = new Button();
            this.btnCancel = new Button();

            this.SuspendLayout();

            cmbRole.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbRole.Items.AddRange(new string[] { "Admin", "Rental Agent" });
            cmbRole.SelectedIndex = 1;

            cmbStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbStatus.Items.AddRange(new string[] { "Active", "Inactive" });
            cmbStatus.SelectedIndex = 0;

            txtPassword.PasswordChar = '*';

            Panel Field(string label, Control input, string helpText = null)
            {
                var p = new Panel { Height = helpText == null ? 72 : 92, Margin = new Padding(0, 0, 24, 12), Dock = DockStyle.Fill };
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
                if (!string.IsNullOrWhiteSpace(helpText))
                {
                    var help = new Label
                    {
                        Text = helpText,
                        Dock = DockStyle.Top,
                        Height = 16,
                        Font = ThemeHelper.SmallFont,
                        ForeColor = ThemeHelper.TextSecondaryColor
                    };
                    p.Controls.Add(help);
                }
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

            AddRow(Field("Username", txtUsername), Field("Email", txtEmail));
            AddRow(Field("Password", txtPassword, userId.HasValue ? "Leave blank to keep current password" : null), Field("Phone", txtPhone));
            AddRow(Field("First Name", txtFirstName), Field("Last Name", txtLastName));
            AddRow(Field("Role", cmbRole), Field("Status", cmbStatus));

            this.Controls.Add(grid);

            btnSave.Text = "Save";
            btnSave.Click += BtnSave_Click;
            btnCancel.Text = "Cancel";
            btnCancel.Click += BtnCancel_Click;

            this.ClientSize = new System.Drawing.Size(960, 520);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Text = userId.HasValue ? "Edit User" : "Add New User";
            this.StartPosition = FormStartPosition.CenterScreen;

            this.ResumeLayout(false);
        }

        private void LoadUser(int id)
        {
            try
            {
                var parameters = new MySqlParameter[]
                {
                    new MySqlParameter("@p_user_id", id)
                };
                DataTable dt = DatabaseHelper.ExecuteStoredProcedure("sp_GetUser", parameters);
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    txtUsername.Text = row["username"].ToString();
                    txtEmail.Text = row["email"].ToString();
                    txtFirstName.Text = row["first_name"].ToString();
                    txtLastName.Text = row["last_name"].ToString();
                    txtPhone.Text = row["phone"]?.ToString() ?? "";
                    cmbRole.Text = row["role"].ToString();
                    cmbStatus.Text = row["status"].ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading user: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (ValidateInput())
            {
                try
                {
                    // For password hashing, in production you should use proper hashing (BCrypt, etc.)
                    // For now, we'll use a simple approach - in production, hash the password
                    string passwordHash = txtPassword.Text;
                    if (userId.HasValue && string.IsNullOrWhiteSpace(txtPassword.Text))
                    {
                        // If editing and password is blank, pass null to keep current password
                        passwordHash = null;
                    }
                    else if (string.IsNullOrWhiteSpace(txtPassword.Text))
                    {
                        MessageBox.Show("Password is required for new users.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    else
                    {
                        // In production, hash the password here
                        // For demo: passwordHash = BCrypt.Net.BCrypt.HashPassword(txtPassword.Text);
                        passwordHash = txtPassword.Text; // This should be hashed in production
                    }

                    var parameters = new MySqlParameter[]
                    {
                        new MySqlParameter("@p_user_id", userId ?? 0),
                        new MySqlParameter("@p_username", txtUsername.Text),
                        new MySqlParameter("@p_password_hash", passwordHash),
                        new MySqlParameter("@p_email", txtEmail.Text),
                        new MySqlParameter("@p_first_name", txtFirstName.Text),
                        new MySqlParameter("@p_last_name", txtLastName.Text),
                        new MySqlParameter("@p_phone", txtPhone.Text),
                        new MySqlParameter("@p_role", cmbRole.Text),
                        new MySqlParameter("@p_status", cmbStatus.Text)
                    };

                    if (userId.HasValue)
                    {
                        DatabaseHelper.ExecuteStoredProcedure("sp_UpdateUser", parameters);
                    }
                    else
                    {
                        DatabaseHelper.ExecuteStoredProcedure("sp_CreateUser", parameters);
                    }

                    MessageBox.Show("User saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving user: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageBox.Show("Username is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Email is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
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
            return true;
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
