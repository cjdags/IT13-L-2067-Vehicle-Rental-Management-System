using System;
using System.Drawing;
using System.Windows.Forms;

namespace VehicleRentalSystem
{
    partial class LoginForm
    {
        private System.ComponentModel.IContainer components = null;
        private Label lblTitle;
        private Label lblUsername;
        private Label lblPassword;
        private TextBox txtUsername;
        private TextBox txtPassword;
        private Button btnLogin;
        private Button btnCancel;
        private Label lblDont;
        private LinkLabel linkSignUp;

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
            this.lblTitle = new Label();
            this.lblSubtitle = new Label();
            this.lblUsername = new Label();
            this.lblPassword = new Label();
            this.txtUsername = new TextBox();
            this.txtPassword = new TextBox();
            this.btnLogin = new Button();
            this.panelLeft = new Panel();
            this.panelRight = new Panel();
            this.SuspendLayout();
            
            // Left Panel (Login Form)
            this.panelLeft.BackColor = Color.White;
            this.panelLeft.Dock = DockStyle.Left;
            this.panelLeft.Size = new System.Drawing.Size(500, 600);
            this.panelLeft.Padding = new Padding(60);

            // Centered layout (prevents clunky hard-coded positions)
            TableLayoutPanel layoutLeft = new TableLayoutPanel();
            layoutLeft.Dock = DockStyle.Fill;
            layoutLeft.ColumnCount = 1;
            layoutLeft.RowCount = 9;
            layoutLeft.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            for (int i = 0; i < 9; i++)
            {
                layoutLeft.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            }
            layoutLeft.Padding = new Padding(0, 20, 0, 0);

            // Car Icon (using text as placeholder)
            Label lblCarIcon = new Label();
            lblCarIcon.Text = "🚗";
            lblCarIcon.Font = new Font("Segoe UI", 48F, FontStyle.Regular);
            lblCarIcon.ForeColor = ThemeHelper.PrimaryColor;
            lblCarIcon.AutoSize = true;
            lblCarIcon.Anchor = AnchorStyles.None;
            lblCarIcon.Margin = new Padding(0, 0, 0, 10);
            layoutLeft.Controls.Add(lblCarIcon, 0, 0);

            // Title
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new Font("Segoe UI", 32F, FontStyle.Bold);
            this.lblTitle.ForeColor = ThemeHelper.TextPrimaryColor;
            this.lblTitle.Text = "Welcome Back";
            this.lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            this.lblTitle.Anchor = AnchorStyles.None;
            this.lblTitle.Margin = new Padding(0, 0, 0, 6);
            layoutLeft.Controls.Add(this.lblTitle, 0, 1);

            // Subtitle
            this.lblSubtitle = new Label();
            this.lblSubtitle.AutoSize = true;
            this.lblSubtitle.Font = ThemeHelper.NormalFont;
            this.lblSubtitle.ForeColor = ThemeHelper.TextSecondaryColor;
            this.lblSubtitle.Text = "Please enter your details to log in.";
            this.lblSubtitle.TextAlign = ContentAlignment.MiddleCenter;
            this.lblSubtitle.Anchor = AnchorStyles.None;
            this.lblSubtitle.Margin = new Padding(0, 0, 0, 28);
            layoutLeft.Controls.Add(this.lblSubtitle, 0, 2);

            // Username Label
            this.lblUsername.AutoSize = true;
            this.lblUsername.Font = ThemeHelper.NormalFont;
            this.lblUsername.ForeColor = ThemeHelper.TextColor;
            this.lblUsername.Text = "Username";
            this.lblUsername.TextAlign = ContentAlignment.MiddleCenter;
            this.lblUsername.Anchor = AnchorStyles.None;
            this.lblUsername.Margin = new Padding(0, 0, 0, 8);
            layoutLeft.Controls.Add(this.lblUsername, 0, 3);

            // Username Input
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Font = new Font("Segoe UI", 14F);
            this.txtUsername.Multiline = true;
            this.txtUsername.Size = new System.Drawing.Size(320, 32);
            this.txtUsername.TextAlign = HorizontalAlignment.Center;
            this.txtUsername.Anchor = AnchorStyles.None;
            this.txtUsername.Margin = new Padding(0, 0, 0, 22);
            layoutLeft.Controls.Add(this.txtUsername, 0, 4);

            // Password Label
            this.lblPassword.AutoSize = true;
            this.lblPassword.Font = ThemeHelper.NormalFont;
            this.lblPassword.ForeColor = ThemeHelper.TextColor;
            this.lblPassword.Text = "Password";
            this.lblPassword.TextAlign = ContentAlignment.MiddleCenter;
            this.lblPassword.Anchor = AnchorStyles.None;
            this.lblPassword.Margin = new Padding(0, 0, 0, 8);
            layoutLeft.Controls.Add(this.lblPassword, 0, 5);

            // Password Input
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.UseSystemPasswordChar = true;
            this.txtPassword.Font = new Font("Segoe UI", 14F);
            this.txtPassword.Multiline = false;
            this.txtPassword.Size = new System.Drawing.Size(320, 32);
            this.txtPassword.TextAlign = HorizontalAlignment.Center;
            this.txtPassword.Anchor = AnchorStyles.None;
            this.txtPassword.Margin = new Padding(0, 0, 0, 26);
            layoutLeft.Controls.Add(this.txtPassword, 0, 6);

            // Login Button
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(320, 48);
            this.btnLogin.TabIndex = 5;
            this.btnLogin.Text = "Log In";
            this.btnLogin.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            this.btnLogin.UseVisualStyleBackColor = false;
            this.btnLogin.Anchor = AnchorStyles.None;
            this.btnLogin.Margin = new Padding(0, 0, 0, 14);
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            layoutLeft.Controls.Add(this.btnLogin, 0, 7);

            // Sign Up Label + Link
            this.lblDont = new Label();
            this.lblDont.AutoSize = true;
            this.lblDont.Font = ThemeHelper.SmallFont;
            this.lblDont.ForeColor = ThemeHelper.TextSecondaryColor;
            this.lblDont.Text = "Don't have an account? ";
            this.lblDont.TextAlign = ContentAlignment.MiddleCenter;
            this.lblDont.Anchor = AnchorStyles.None;
            this.lblDont.Margin = new Padding(0, 0, 0, 0);

            this.linkSignUp = new LinkLabel();
            this.linkSignUp.Text = "Sign Up";
            this.linkSignUp.Font = ThemeHelper.SmallFont;
            this.linkSignUp.LinkColor = ThemeHelper.PrimaryColor;
            this.linkSignUp.ActiveLinkColor = ThemeHelper.PrimaryHoverColor;
            this.linkSignUp.AutoSize = true;
            this.linkSignUp.TextAlign = ContentAlignment.MiddleCenter;
            this.linkSignUp.Anchor = AnchorStyles.None;
            this.linkSignUp.Margin = new Padding(0, 0, 0, 0);
            this.linkSignUp.LinkBehavior = LinkBehavior.HoverUnderline;
            this.linkSignUp.Click += new System.EventHandler(this.linkSignUp_Click);

            // Put label and link in a FlowLayoutPanel so they appear on one row and centered
            FlowLayoutPanel signupRow = new FlowLayoutPanel();
            signupRow.AutoSize = true;
            signupRow.FlowDirection = FlowDirection.LeftToRight;
            signupRow.Anchor = AnchorStyles.None;
            signupRow.Controls.Add(this.lblDont);
            signupRow.Controls.Add(this.linkSignUp);

            layoutLeft.Controls.Add(signupRow, 0, 8);

            this.panelLeft.Controls.Add(layoutLeft);
            
            // Right Panel (Image Background - using solid color for now)
            this.panelRight.BackColor = Color.FromArgb(240, 242, 244);
            this.panelRight.Dock = DockStyle.Fill;
            this.panelRight.Size = new System.Drawing.Size(700, 600);
            
            // LoginForm
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 600);
            this.Controls.Add(this.panelRight);
            this.Controls.Add(this.panelLeft);
            this.AcceptButton = this.btnLogin;
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Login - Vehicle Rental System";
            this.ResumeLayout(false);
        }
        
        private Panel panelLeft;
        private Panel panelRight;
        private Label lblSubtitle;
    }
}
