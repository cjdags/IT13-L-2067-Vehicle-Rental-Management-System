using System;
using System.Data;
using System.Windows.Forms;
using BCrypt.Net;
using MySql.Data.MySqlClient;

namespace VehicleRentalSystem
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
            // Use base theme only; ApplyTheme() would override our custom layout/padding on panels.
            ThemeHelper.ApplyBaseTheme(this);

            // Explicit control styling
            ThemeHelper.ApplyButtonTheme(btnLogin, isPrimary: true);
            txtUsername.BackColor = ThemeHelper.InputBackground;
            txtUsername.ForeColor = ThemeHelper.TextColor;
            txtUsername.BorderStyle = BorderStyle.FixedSingle;
            txtPassword.BackColor = ThemeHelper.InputBackground;
            txtPassword.ForeColor = ThemeHelper.TextColor;
            txtPassword.BorderStyle = BorderStyle.FixedSingle;
            txtPassword.UseSystemPasswordChar = true;
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter both username and password.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var dt = DatabaseHelper.ExecuteQuery(
                    @"SELECT user_id, username, password_hash, role, first_name, last_name, status
                      FROM Users
                      WHERE username = @username
                      LIMIT 1",
                    new MySqlParameter("@username", username));

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Invalid username or password.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var row = dt.Rows[0];
                string storedHash = row["password_hash"]?.ToString() ?? "";

                bool passwordMatches;
                if (!string.IsNullOrWhiteSpace(storedHash) && storedHash.StartsWith("$2")) // bcrypt hash
                {
                    passwordMatches = BCrypt.Net.BCrypt.Verify(password, storedHash);
                }
                else
                {
                    // Fallback: plain text (not recommended, kept for legacy data)
                    passwordMatches = string.Equals(storedHash, password);
                }

                if (!passwordMatches)
                {
                    MessageBox.Show("Invalid username or password.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Optional: ensure user is active
                if (!string.Equals(row["status"]?.ToString(), "Active", StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show("Your account is not active.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Store current user info
                CurrentUser.UserId = Convert.ToInt32(row["user_id"]);
                CurrentUser.Username = username;
                CurrentUser.Role = row["role"]?.ToString() ?? "";
                CurrentUser.FullName = $"{row["first_name"]} {row["last_name"]}";

                this.Hide();
                MainDashboard mainDashboard = new MainDashboard();
                mainDashboard.ShowDialog();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Login error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void linkSignUp_Click(object sender, EventArgs e)
        {
            using (var form = new SignUpForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    // Prefill login with the new credentials
                    txtUsername.Text = form.NewUsername;
                    txtPassword.Text = form.NewPassword;
                }
            }
        }
    }

    public static class CurrentUser
    {
        public static int UserId { get; set; }
        public static string Username { get; set; } = "";
        public static string Role { get; set; } = "";
        public static string FullName { get; set; } = "";
    }
}
