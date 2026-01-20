using System;
using System.Drawing;
using System.Windows.Forms;

namespace VehicleRentalSystem
{
    partial class MainDashboard
    {
        private System.ComponentModel.IContainer components = null;
        private Panel panelSidebar;
        private Panel panelMainContent;
        private Label lblLogo;
        private Button btnDashboard;
        private Button btnUsers;
        private Button btnVehiclesNav;
        private Button btnRates;
        private Button btnReports;
        private Button btnSettings;
        private Label lblUserInfo;
        private Label lblUserRole;
        private Label lblWelcome;
        private Label lblWelcomeSubtitle;
        private Panel panelStats;
        private Label lblTotalVehiclesValue;
        private Label lblAvailableVehiclesValue;
        private Label lblActiveRentalsValue;
        private Label lblTotalCustomersValue;
        private Panel panelButtons;
        private Button btnCustomers;
        private Button btnVehicles;
        private Button btnRentals;
        private Button btnNewRental;
        private Button btnPayments;
        private Button btnMaintenance;
        private Button btnDamageAssessments;
        private Button btnStaff;
        private Button btnVehicleTypes;
        private Button btnLogout;
        private PictureBox piclogo;
        private Label lblReportsBadge;
        private Button btnReservations;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.panelSidebar = new Panel();
            this.lblLogo = new Label();
            this.btnDashboard = new Button();
            this.btnUsers = new Button();
            this.btnVehiclesNav = new Button();
            this.btnRates = new Button();
            this.btnReports = new Button();
            this.btnReservations = new Button();
            this.btnSettings = new Button();
            this.lblUserInfo = new Label();
            this.lblUserRole = new Label();
            this.panelMainContent = new Panel();
            this.btnLogout = new Button();
            this.lblReportsBadge = new Label();
            this.lblWelcome = new Label();
            this.lblWelcomeSubtitle = new Label();
            this.panelStats = new Panel();
            this.panelButtons = new Panel();
            this.btnCustomers = new Button();
            this.btnVehicles = new Button();
            this.btnRentals = new Button();
            this.btnNewRental = new Button();
            this.btnPayments = new Button();
            this.btnMaintenance = new Button();
            this.btnDamageAssessments = new Button();
            this.btnStaff = new Button();
            this.btnVehicleTypes = new Button();
            
            this.panelSidebar.SuspendLayout();
            this.panelMainContent.SuspendLayout();
            this.panelStats.SuspendLayout();
            this.panelButtons.SuspendLayout();
            this.SuspendLayout();

            // Sidebar Panel
            this.panelSidebar.BackColor = ThemeHelper.SidebarColor;
            this.panelSidebar.BorderStyle = BorderStyle.FixedSingle;
            this.panelSidebar.Location = new System.Drawing.Point(0, 0);
            this.panelSidebar.Size = new System.Drawing.Size(256, 700);
            this.panelSidebar.Dock = DockStyle.Left;
            this.panelSidebar.Padding = new Padding(16);

            // Logo
            
            this.lblLogo.Text = "ZRC Car Rental";
            this.lblLogo.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            this.lblLogo.ForeColor = ThemeHelper.PrimaryColor;
            this.lblLogo.Location = new System.Drawing.Point(16, 16);
            this.lblLogo.Size = new System.Drawing.Size(200, 30);
            this.panelSidebar.Controls.Add(this.lblLogo);

            // Dashboard Button (Active)
            this.btnDashboard.Text = "Dashboard";
            this.btnDashboard.Font = ThemeHelper.NormalFont;
            this.btnDashboard.Location = new System.Drawing.Point(16, 70);
            this.btnDashboard.Size = new System.Drawing.Size(224, 40);
            this.btnDashboard.FlatStyle = FlatStyle.Flat;
            this.btnDashboard.FlatAppearance.BorderSize = 0;
            this.btnDashboard.BackColor = ThemeHelper.SidebarActiveBackgroundColor; // Opaque active background
            this.btnDashboard.ForeColor = ThemeHelper.PrimaryColor;
            this.btnDashboard.TextAlign = ContentAlignment.MiddleLeft;
            this.btnDashboard.Padding = new Padding(12, 0, 0, 0);
            this.btnDashboard.Cursor = Cursors.Hand;
            this.panelSidebar.Controls.Add(this.btnDashboard);

            // Users Button
            this.btnUsers.Text = "Users";
            this.btnUsers.Font = ThemeHelper.NormalFont;
            this.btnUsers.Location = new System.Drawing.Point(16, 270);
            this.btnUsers.Size = new System.Drawing.Size(224, 40);
            this.btnUsers.FlatStyle = FlatStyle.Flat;
            this.btnUsers.FlatAppearance.BorderSize = 0;
            this.btnUsers.BackColor = ThemeHelper.SidebarColor;
            this.btnUsers.ForeColor = ThemeHelper.TextColor;
            this.btnUsers.TextAlign = ContentAlignment.MiddleLeft;
            this.btnUsers.Padding = new Padding(12, 0, 0, 0);
            this.btnUsers.Cursor = Cursors.Hand;
            this.btnUsers.Click += (s, e) => btnStaff_Click(s, e);
            this.panelSidebar.Controls.Add(this.btnUsers);

            // Vehicles Button
            this.btnVehiclesNav.Text = "Vehicles";
            this.btnVehiclesNav.Font = ThemeHelper.NormalFont;
            this.btnVehiclesNav.Location = new System.Drawing.Point(16, 120);
            this.btnVehiclesNav.Size = new System.Drawing.Size(224, 40);
            this.btnVehiclesNav.FlatStyle = FlatStyle.Flat;
            this.btnVehiclesNav.FlatAppearance.BorderSize = 0;
            this.btnVehiclesNav.BackColor = ThemeHelper.SidebarColor;
            this.btnVehiclesNav.ForeColor = ThemeHelper.TextColor;
            this.btnVehiclesNav.TextAlign = ContentAlignment.MiddleLeft;
            this.btnVehiclesNav.Padding = new Padding(12, 0, 0, 0);
            this.btnVehiclesNav.Cursor = Cursors.Hand;
            this.btnVehiclesNav.Click += (s, e) => btnVehicles_Click(s, e);
            this.panelSidebar.Controls.Add(this.btnVehiclesNav);

            // Rates Button
            //this.btnRates.Text = "Rates";
            //this.btnRates.Font = ThemeHelper.NormalFont;
            //this.btnRates.Location = new System.Drawing.Point(16, 220);
            //this.btnRates.Size = new System.Drawing.Size(224, 40);
            //this.btnRates.FlatStyle = FlatStyle.Flat;
            //this.btnRates.FlatAppearance.BorderSize = 0;
            //this.btnRates.BackColor = ThemeHelper.SidebarColor;
            //this.btnRates.ForeColor = ThemeHelper.TextColor;
            //this.btnRates.TextAlign = ContentAlignment.MiddleLeft;
            //this.btnRates.Padding = new Padding(12, 0, 0, 0);
            //this.btnRates.Cursor = Cursors.Hand;
            //this.btnRates.Click += (s, e) => btnVehicleTypes_Click(s, e);
            //this.panelSidebar.Controls.Add(this.btnRates);

            // Reports Button
            this.btnReports.Text = "Reports";
            this.btnReports.Font = ThemeHelper.NormalFont;
            this.btnReports.Location = new System.Drawing.Point(16, 220);
            this.btnReports.Size = new System.Drawing.Size(224, 40);
            this.btnReports.FlatStyle = FlatStyle.Flat;
            this.btnReports.FlatAppearance.BorderSize = 0;
            this.btnReports.BackColor = ThemeHelper.SidebarColor;
            this.btnReports.ForeColor = ThemeHelper.TextColor;
            this.btnReports.TextAlign = ContentAlignment.MiddleLeft;
            this.btnReports.Padding = new Padding(12, 0, 0, 0);
            this.btnReports.Cursor = Cursors.Hand;
            this.panelSidebar.Controls.Add(this.btnReports);

            // Reservations Button
            this.btnReservations.Text = "Reservations";
            this.btnReservations.Font = ThemeHelper.NormalFont;
            this.btnReservations.Location = new System.Drawing.Point(16, 170);
            this.btnReservations.Size = new System.Drawing.Size(224, 40);
            this.btnReservations.FlatStyle = FlatStyle.Flat;
            this.btnReservations.FlatAppearance.BorderSize = 0;
            this.btnReservations.BackColor = ThemeHelper.SidebarColor;
            this.btnReservations.ForeColor = ThemeHelper.TextColor;
            this.btnReservations.TextAlign = ContentAlignment.MiddleLeft;
            this.btnReservations.Padding = new Padding(12, 0, 0, 0);
            this.btnReservations.Cursor = Cursors.Hand;
            this.btnReservations.Click += new EventHandler(this.btnReservations_Click);
            this.panelSidebar.Controls.Add(this.btnReservations);

            // Reports badge
            this.lblReportsBadge.AutoSize = false;
            this.lblReportsBadge.Size = new System.Drawing.Size(28, 20);
            this.lblReportsBadge.BackColor = ThemeHelper.PrimaryColor;
            this.lblReportsBadge.ForeColor = ThemeHelper.TextOnPrimary;
            this.lblReportsBadge.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.lblReportsBadge.TextAlign = ContentAlignment.MiddleCenter;
            this.lblReportsBadge.Visible = false;
            this.lblReportsBadge.BorderStyle = BorderStyle.FixedSingle;
            this.lblReportsBadge.Padding = new Padding(0, 1, 0, 0);
            this.lblReportsBadge.Location = new System.Drawing.Point(196, 228); // near Reports button
            this.panelSidebar.Controls.Add(this.lblReportsBadge);

            // Settings Button
            //this.btnSettings.Text = "Settings";
            //this.btnSettings.Font = ThemeHelper.NormalFont;
            //this.btnSettings.Location = new System.Drawing.Point(16, 550);
            //this.btnSettings.Size = new System.Drawing.Size(224, 40);
            //this.btnSettings.FlatStyle = FlatStyle.Flat;
            //this.btnSettings.FlatAppearance.BorderSize = 0;
            //this.btnSettings.BackColor = ThemeHelper.SidebarColor;
            //this.btnSettings.ForeColor = ThemeHelper.TextColor;
            //this.btnSettings.TextAlign = ContentAlignment.MiddleLeft;
            //this.btnSettings.Padding = new Padding(12, 0, 0, 0);
            //this.btnSettings.Cursor = Cursors.Hand;
            //this.panelSidebar.Controls.Add(this.btnSettings);

            // User Info at bottom of sidebar
            this.lblUserInfo.Text = "User";
            this.lblUserInfo.Font = ThemeHelper.NormalFont;
            this.lblUserInfo.ForeColor = ThemeHelper.TextColor;
            this.lblUserInfo.Location = new System.Drawing.Point(16, 640);
            this.lblUserInfo.Size = new System.Drawing.Size(224, 20);
            this.panelSidebar.Controls.Add(this.lblUserInfo);

            this.lblUserRole.Text = "Role";
            this.lblUserRole.Font = ThemeHelper.SmallFont;
            this.lblUserRole.ForeColor = ThemeHelper.TextSecondaryColor;
            this.lblUserRole.Location = new System.Drawing.Point(16, 660);
            this.lblUserRole.Size = new System.Drawing.Size(224, 20);
            this.panelSidebar.Controls.Add(this.lblUserRole);

            // Main Content Panel
            this.panelMainContent.BackColor = ThemeHelper.BackgroundColor;
            this.panelMainContent.Dock = DockStyle.Fill;
            this.panelMainContent.Padding = new Padding(24);

            // --- Header (welcome + logout) ---
            Panel panelHeaderMain = new Panel();
            panelHeaderMain.BackColor = ThemeHelper.BackgroundColor;
            panelHeaderMain.Dock = DockStyle.Top;
            panelHeaderMain.Height = 120;
            panelHeaderMain.Padding = new Padding(0, 0, 0, 16);

            TableLayoutPanel headerLayout = new TableLayoutPanel();
            headerLayout.Dock = DockStyle.Fill;
            headerLayout.BackColor = ThemeHelper.BackgroundColor;
            headerLayout.ColumnCount = 2;
            headerLayout.RowCount = 1;
            headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            TableLayoutPanel headerTextLayout = new TableLayoutPanel();
            headerTextLayout.Dock = DockStyle.Fill;
            headerTextLayout.BackColor = ThemeHelper.BackgroundColor;
            headerTextLayout.ColumnCount = 1;
            headerTextLayout.RowCount = 2;
            headerTextLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            headerTextLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            this.lblWelcome.Text = "Dashboard";
            this.lblWelcome.Font = new Font("Segoe UI", 28F, FontStyle.Bold);
            this.lblWelcome.ForeColor = ThemeHelper.TextColor;
            this.lblWelcome.AutoSize = true;
            this.lblWelcome.Margin = new Padding(0, 0, 0, 4);

            this.lblWelcomeSubtitle.Text = "Welcome back! Here's what's happening today.";
            this.lblWelcomeSubtitle.Font = ThemeHelper.NormalFont;
            this.lblWelcomeSubtitle.ForeColor = ThemeHelper.TextSecondaryColor;
            this.lblWelcomeSubtitle.AutoSize = true;
            this.lblWelcomeSubtitle.Margin = new Padding(0, 0, 0, 0);

            headerTextLayout.Controls.Add(this.lblWelcome, 0, 0);
            headerTextLayout.Controls.Add(this.lblWelcomeSubtitle, 0, 1);

            // Logout Button
            this.btnLogout.Text = "Logout";
            this.btnLogout.Font = ThemeHelper.NormalFont;
            this.btnLogout.Size = new System.Drawing.Size(110, 66);
            this.btnLogout.FlatStyle = FlatStyle.Flat;
            this.btnLogout.FlatAppearance.BorderSize = 0;
            this.btnLogout.BackColor = ThemeHelper.ButtonSecondaryColor;
            this.btnLogout.ForeColor = ThemeHelper.TextColor;
            this.btnLogout.Cursor = Cursors.Hand;
            this.btnLogout.Margin = new Padding(16, 8, 0, 0);
            this.btnLogout.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this.btnLogout.Click += new System.EventHandler(this.btnLogout_Click);

            headerLayout.Controls.Add(headerTextLayout, 0, 0);
            headerLayout.Controls.Add(this.btnLogout, 1, 0);
            panelHeaderMain.Controls.Add(headerLayout);

            // --- Stats row ---
            this.panelStats.BackColor = ThemeHelper.BackgroundColor;
            this.panelStats.Dock = DockStyle.Top;
            this.panelStats.Height = 130;
            this.panelStats.Padding = new Padding(0, 0, 0, 16);
            this.panelStats.AutoScroll = false;

            TableLayoutPanel statsGrid = new TableLayoutPanel();
            statsGrid.Dock = DockStyle.Fill;
            statsGrid.BackColor = ThemeHelper.BackgroundColor;
            statsGrid.ColumnCount = 4;
            statsGrid.RowCount = 1;
            statsGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            statsGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            statsGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            statsGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            statsGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            Panel WrapStatCard(Panel card, int rightPadding)
            {
                var wrap = new Panel
                {
                    Dock = DockStyle.Fill,
                    BackColor = ThemeHelper.BackgroundColor,
                    Padding = new Padding(0, 0, rightPadding, 0)
                };
                card.Dock = DockStyle.Fill;
                wrap.Controls.Add(card);
                return wrap;
            }

            statsGrid.Controls.Add(WrapStatCard(CreateStatCard("Total Vehicles", "0", 0), 16), 0, 0);
            statsGrid.Controls.Add(WrapStatCard(CreateStatCard("Available Vehicles", "0", 1), 16), 1, 0);
            statsGrid.Controls.Add(WrapStatCard(CreateStatCard("Active Rentals", "0", 2), 16), 2, 0);
            statsGrid.Controls.Add(WrapStatCard(CreateStatCard("Total Customers", "0", 3), 0), 3, 0);

            this.panelStats.Controls.Add(statsGrid);

            // --- Action grid ---
            this.panelButtons.BackColor = ThemeHelper.BackgroundColor;
            this.panelButtons.Dock = DockStyle.Fill;
            this.panelButtons.Padding = new Padding(0);

            TableLayoutPanel actionsGrid = new TableLayoutPanel();
            actionsGrid.Dock = DockStyle.Fill;
            actionsGrid.BackColor = ThemeHelper.BackgroundColor;
            actionsGrid.ColumnCount = 3;
            actionsGrid.RowCount = 3;
            actionsGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333F));
            actionsGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333F));
            actionsGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333F));
            actionsGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3333F));
            actionsGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3333F));
            actionsGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3333F));
            actionsGrid.Padding = new Padding(0);

            void ConfigureActionButton(Button b, string text, EventHandler onClick, bool primary = false, int marginRight = 20)
            {
                b.Text = text;
                b.Dock = DockStyle.Fill;
                // Enforce uniform tile sizing via minimums to prevent shrink on different DPI
                b.MinimumSize = new Size(0, 64);
                b.Height = 64;
                // Uniform margins for all action buttons (symmetric left/right, consistent bottom spacing)
                b.Margin = new Padding(12, 8, 12, 16);
                b.TextAlign = ContentAlignment.MiddleCenter;
                b.Font = ThemeHelper.ButtonFont;
                b.Cursor = Cursors.Hand;
                b.FlatStyle = FlatStyle.Flat;
                b.FlatAppearance.BorderColor = ThemeHelper.BorderColor;
                b.FlatAppearance.BorderSize = 1;
                b.FlatAppearance.MouseOverBackColor = ThemeHelper.ButtonHoverColor;
                b.FlatAppearance.MouseDownBackColor = ThemeHelper.ButtonSecondaryColor;
                b.BackColor = ThemeHelper.CardColor;
                b.ForeColor = ThemeHelper.TextColor;
                b.Click += onClick;
                if (primary)
                {
                    b.BackColor = ThemeHelper.PrimaryColor;
                    b.ForeColor = ThemeHelper.TextOnPrimary;
                    b.FlatAppearance.BorderSize = 0;
                }
            }

            ConfigureActionButton(this.btnCustomers, "Customer Management", new System.EventHandler(this.btnCustomers_Click), marginRight: 20);
            ConfigureActionButton(this.btnVehicles, "Vehicle Management", new System.EventHandler(this.btnVehicles_Click), marginRight: 20);
            ConfigureActionButton(this.btnRentals, "View Rentals", new System.EventHandler(this.btnRentals_Click), marginRight: 0);

            ConfigureActionButton(this.btnNewRental, "New Rental", new System.EventHandler(this.btnNewRental_Click), primary: true, marginRight: 20);
            ConfigureActionButton(this.btnPayments, "Payments", new System.EventHandler(this.btnPayments_Click), marginRight: 20);
            ConfigureActionButton(this.btnMaintenance, "Maintenance", new System.EventHandler(this.btnMaintenance_Click), marginRight: 0);

            ConfigureActionButton(this.btnDamageAssessments, "Damage Reports", new System.EventHandler(this.btnDamageAssessments_Click), marginRight: 20);
            ConfigureActionButton(this.btnStaff, "User Management", new System.EventHandler(this.btnStaff_Click), marginRight: 20);
            ConfigureActionButton(this.btnVehicleTypes, "Vehicle Categories", new System.EventHandler(this.btnVehicleTypes_Click), marginRight: 0);

            actionsGrid.Controls.Add(this.btnCustomers, 0, 0);
            actionsGrid.Controls.Add(this.btnVehicles, 1, 0);
            actionsGrid.Controls.Add(this.btnRentals, 2, 0);
            actionsGrid.Controls.Add(this.btnNewRental, 0, 1);
            actionsGrid.Controls.Add(this.btnPayments, 1, 1);
            actionsGrid.Controls.Add(this.btnMaintenance, 2, 1);
            actionsGrid.Controls.Add(this.btnDamageAssessments, 0, 2);
            actionsGrid.Controls.Add(this.btnStaff, 1, 2);
            actionsGrid.Controls.Add(this.btnVehicleTypes, 2, 2);

            this.panelButtons.Controls.Add(actionsGrid);

            // Add in docking order: fill first, then stats, then header
            this.panelMainContent.Controls.Add(this.panelButtons);
            this.panelMainContent.Controls.Add(this.panelStats);
            this.panelMainContent.Controls.Add(panelHeaderMain);

            // Main Dashboard Form
            this.ClientSize = new System.Drawing.Size(1200, 700);
            // Important: WinForms docking is applied in reverse z-order.
            // Add the FILL panel first, then the LEFT sidebar so the sidebar reserves space.
            this.Controls.Add(this.panelMainContent);
            this.Controls.Add(this.panelSidebar);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Text = "Vehicle Rental Management System - Dashboard";
            this.StartPosition = FormStartPosition.CenterScreen;

            this.panelSidebar.ResumeLayout(false);
            this.panelMainContent.ResumeLayout(false);
            this.panelMainContent.PerformLayout();
            this.panelStats.ResumeLayout(false);
            this.panelStats.PerformLayout();
            this.panelButtons.ResumeLayout(false);
            this.panelButtons.PerformLayout();
            this.ResumeLayout(false);
        }

        private Panel CreateStatCard(string title, string value, int index)
        {
            Panel card = new Panel();
            card.BackColor = ThemeHelper.CardColor;
            card.BorderStyle = BorderStyle.FixedSingle;
            card.Margin = new Padding(0);
            card.Padding = new Padding(16);

            Label lblTitle = new Label();
            lblTitle.Text = title;
            lblTitle.Font = ThemeHelper.SmallFont;
            lblTitle.ForeColor = ThemeHelper.TextSecondaryColor;
            lblTitle.Location = new Point(16, 16);
            lblTitle.Size = new Size(148, 20);
            card.Controls.Add(lblTitle);

            Label lblValue = new Label();
            lblValue.Name = $"lblValue_{title.Replace(" ", "")}";
            lblValue.Text = value;
            lblValue.Font = new Font("Segoe UI", 24F, FontStyle.Bold);
            lblValue.ForeColor = ThemeHelper.TextColor;
            lblValue.Location = new Point(16, 40);
            lblValue.Size = new Size(148, 40);
            card.Controls.Add(lblValue);

            // Store reference for updating - labels are created dynamically
            // The references will be set in LoadDashboard method

            return card;
        }
    }
}
