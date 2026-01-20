using System;
using System.Windows.Forms;
using System.Data;

namespace VehicleRentalSystem
{
    public partial class MainDashboard : Form
    {
        public MainDashboard()
        {
            InitializeComponent();
            // IMPORTANT: ApplyTheme() recursively overrides Panel/Button styles (e.g. makes panels white),
            // which breaks the custom dashboard layout defined in the Designer.
            ThemeHelper.ApplyBaseTheme(this);
            ApplySidebarNavThemes();
            ApplyButtonThemes();
            ApplyRbac();
            LoadDashboard();

            btnNewRental.Click -= btnNewRental_Click;
            btnNewRental.Click += btnNewRental_Click;
            if (btnReports != null)
                btnReports.Click += BtnReports_Click;
        }

        private void ApplySidebarNavThemes()
        {
            ApplySidebarNavButtonTheme(btnDashboard, isActive: true);
            ApplySidebarNavButtonTheme(btnUsers);
            ApplySidebarNavButtonTheme(btnVehiclesNav);
            ApplySidebarNavButtonTheme(btnRates);
            ApplySidebarNavButtonTheme(btnReports);
            ApplySidebarNavButtonTheme(btnReservations);
            ApplySidebarNavButtonTheme(btnSettings);
        }

        private static void ApplySidebarNavButtonTheme(Button button, bool isActive = false)
        {
            if (button == null) return;

            button.UseVisualStyleBackColor = false;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.Font = ThemeHelper.NormalFont;

            // Critical: avoid the generic button padding/font that can clip text in 40px sidebar rows.
            button.Padding = new Padding(12, 0, 0, 0);
            button.TextAlign = ContentAlignment.MiddleLeft;
            button.ImageAlign = ContentAlignment.MiddleLeft;

            button.BackColor = isActive ? ThemeHelper.SidebarActiveBackgroundColor : ThemeHelper.SidebarColor;
            button.ForeColor = isActive ? ThemeHelper.PrimaryColor : ThemeHelper.TextColor;

            button.FlatAppearance.MouseOverBackColor = ThemeHelper.SidebarHoverBackgroundColor;
            button.FlatAppearance.MouseDownBackColor = ThemeHelper.ButtonSecondaryColor;
        }

        private void ApplyButtonThemes()
        {
            // The dashboard "action grid" buttons are styled in the Designer (card buttons with borders).
            // Don't override them here with ThemeHelper.ApplyButtonTheme() (it removes borders and blends into the background).

            // Keep only logout consistent
            ThemeHelper.ApplyButtonTheme(btnLogout, isSecondary: true);
        }

        private void LoadDashboard()
        {
            // Update welcome subtitle
            if (lblWelcomeSubtitle != null)
            {
                lblWelcomeSubtitle.Text = $"Welcome back, {CurrentUser.FullName}! Here's what's happening today.";
            }
            
            // Update user info in sidebar
            if (lblUserInfo != null)
            {
                lblUserInfo.Text = CurrentUser.FullName;
            }
            if (lblUserRole != null)
            {
                lblUserRole.Text = CurrentUser.Role;
            }
            
            // Load statistics and update stat cards
            try
            {
                var vehicles = DatabaseHelper.ExecuteStoredProcedure("sp_GetAllVehicles");
                var customers = DatabaseHelper.ExecuteStoredProcedure("sp_GetAllCustomers");
                var activeRentals = DatabaseHelper.ExecuteStoredProcedure("sp_GetActiveRentals");
                var availableVehicles = DatabaseHelper.ExecuteStoredProcedure("sp_GetAvailableVehicles");

                // Find and update stat card labels (nested inside layout containers)
                foreach (Control lbl in FindControls(panelStats))
                {
                    if (lbl is not Label valueLabel || !valueLabel.Name.StartsWith("lblValue_"))
                        continue;

                    if (valueLabel.Name.Contains("TotalVehicles"))
                        valueLabel.Text = vehicles.Rows.Count.ToString();
                    else if (valueLabel.Name.Contains("AvailableVehicles"))
                        valueLabel.Text = availableVehicles.Rows.Count.ToString();
                    else if (valueLabel.Name.Contains("ActiveRentals"))
                        valueLabel.Text = activeRentals.Rows.Count.ToString();
                    else if (valueLabel.Name.Contains("TotalCustomers"))
                        valueLabel.Text = customers.Rows.Count.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading dashboard: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            UpdateReportsBadge();
        }

        private void ApplyRbac()
        {
            bool isAdmin = string.Equals(CurrentUser.Role, "Admin", StringComparison.OrdinalIgnoreCase);

            // Sidebar admin-only sections
            if (btnUsers != null) btnUsers.Visible = isAdmin;
            if (btnRates != null) btnRates.Visible = isAdmin;
            if (btnReports != null) btnReports.Visible = isAdmin;
            if (btnReservations != null) btnReservations.Visible = true; // both roles can view pending reservations

            // Dashboard action buttons
            if (btnStaff != null) btnStaff.Visible = isAdmin;
            if (btnVehicleTypes != null) btnVehicleTypes.Visible = isAdmin;
        }

        private static System.Collections.Generic.IEnumerable<Control> FindControls(Control root)
        {
            if (root == null) yield break;

            foreach (Control child in root.Controls)
            {
                yield return child;
                foreach (var grandChild in FindControls(child))
                    yield return grandChild;
            }
        }

        private void btnCustomers_Click(object sender, EventArgs e)
        {
            CustomerListForm form = new CustomerListForm();
            form.ShowDialog();
            LoadDashboard();
        }

        private void btnVehicles_Click(object sender, EventArgs e)
        {
            VehicleListForm form = new VehicleListForm();
            form.ShowDialog();
            LoadDashboard();
        }

        private void btnRentals_Click(object sender, EventArgs e)
        {
            RentalListForm form = new RentalListForm();
            form.ShowDialog();
            LoadDashboard();
        }

        private void btnNewRental_Click(object sender, EventArgs e)
        {
            ReservationWizardForm form = new ReservationWizardForm();
            form.ShowDialog();
            LoadDashboard();
        }

        private void BtnReports_Click(object? sender, EventArgs e)
        {
            using var reports = new ReportsForm();
            reports.ShowDialog();
        }

        private void btnPayments_Click(object sender, EventArgs e)
        {
            PaymentListForm form = new PaymentListForm();
            form.ShowDialog();
        }

        private void btnMaintenance_Click(object sender, EventArgs e)
        {
            MaintenanceListForm form = new MaintenanceListForm();
            form.ShowDialog();
            LoadDashboard();
        }

        private void btnDamageAssessments_Click(object sender, EventArgs e)
        {
            DamageAssessmentListForm form = new DamageAssessmentListForm();
            form.ShowDialog();
        }

        private void btnReservations_Click(object sender, EventArgs e)
        {
            using var form = new ReservationListForm();
            form.ShowDialog();
        }

        private void btnStaff_Click(object sender, EventArgs e)
        {
            if (CurrentUser.Role == "Admin")
            {
                StaffListForm form = new StaffListForm();
                form.ShowDialog();
            }
            else
            {
                MessageBox.Show("You do not have permission to access this feature.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnVehicleTypes_Click(object sender, EventArgs e)
        {
            VehicleTypeListForm form = new VehicleTypeListForm();
            form.ShowDialog();
            LoadDashboard();
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to logout?", "Confirm Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                this.Hide();
                LoginForm loginForm = new LoginForm();
                loginForm.ShowDialog();
                this.Close();
            }
        }

        private void UpdateReportsBadge()
        {
            if (lblReportsBadge == null) return;
            try
            {
                var dt = DatabaseHelper.ExecuteStoredProcedure("sp_GetAllDamageReports");
                int pending = 0;
                foreach (DataRow row in dt.Rows)
                {
                    var status = row["status"]?.ToString();
                    if (string.Equals(status, "Reported", StringComparison.OrdinalIgnoreCase))
                        pending++;
                }

                if (pending > 99) pending = 99;
                if (pending > 0)
                {
                    lblReportsBadge.Text = pending.ToString();
                    lblReportsBadge.Visible = true;
                }
                else
                {
                    lblReportsBadge.Visible = false;
                }
            }
            catch
            {
                lblReportsBadge.Visible = false;
            }
        }
    }
}
