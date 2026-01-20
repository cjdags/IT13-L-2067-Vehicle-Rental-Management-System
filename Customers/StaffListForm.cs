using System;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace VehicleRentalSystem
{
    public partial class StaffListForm : Form
    {
        private DataGridView dgvStaff;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnDelete;
        private Button btnClose;

        public StaffListForm()
        {
            if (!string.Equals(CurrentUser.Role, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Access denied. Admins only.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.Close();
                return;
            }
            InitializeComponent();
            ThemeHelper.ApplyStandardListLayout(
                this,
                title: "User Management",
                addButton: btnAdd,
                searchBox: txtSearch,
                searchButton: btnSearch,
                dataGridView: dgvStaff,
                leftFooterButtons: new[] { btnEdit, btnDelete },
                rightFooterButtons: new[] { btnClose },
                searchPlaceholder: "Search by name or email..."
            );
            LoadStaff();
        }

        private TextBox txtSearch;
        private Button btnSearch;
        private Panel panelHeader;
        private Label lblTitle;
        private Panel panelTable;

        private void InitializeComponent()
        {
            this.dgvStaff = new DataGridView();
            this.btnAdd = new Button();
            this.btnEdit = new Button();
            this.btnDelete = new Button();
            this.btnClose = new Button();
            this.txtSearch = new TextBox();
            this.btnSearch = new Button();
            this.panelHeader = new Panel();
            this.lblTitle = new Label();
            this.panelTable = new Panel();

            this.SuspendLayout();

            // Header Panel
            this.panelHeader.BackColor = Color.White;
            this.panelHeader.BorderStyle = BorderStyle.FixedSingle;
            this.panelHeader.Dock = DockStyle.Top;
            this.panelHeader.Height = 80;
            this.panelHeader.Padding = new Padding(16);
            
            // Title
            this.lblTitle.Text = "User Management";
            this.lblTitle.Font = new Font("Segoe UI", 28F, FontStyle.Bold);
            this.lblTitle.ForeColor = ThemeHelper.TextPrimaryColor;
            this.lblTitle.Location = new System.Drawing.Point(16, 20);
            this.lblTitle.Size = new System.Drawing.Size(500, 40);
            this.panelHeader.Controls.Add(this.lblTitle);
            
            // Add Button
            this.btnAdd.Location = new System.Drawing.Point(1100, 20);
            this.btnAdd.Size = new System.Drawing.Size(120, 40);
            this.btnAdd.Text = "+ Add New User";
            this.btnAdd.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.btnAdd.Click += BtnAdd_Click;
            this.panelHeader.Controls.Add(this.btnAdd);
            this.Controls.Add(this.panelHeader);

            // Search Panel
            Panel panelSearch = new Panel();
            panelSearch.BackColor = ThemeHelper.BackgroundColor;
            panelSearch.Dock = DockStyle.Top;
            panelSearch.Height = 80;
            panelSearch.Padding = new Padding(16);
            
            this.txtSearch.Location = new System.Drawing.Point(16, 20);
            this.txtSearch.Size = new System.Drawing.Size(400, 40);
            this.txtSearch.Font = ThemeHelper.NormalFont;
            this.txtSearch.Text = "Search by name or email...";
            this.txtSearch.ForeColor = ThemeHelper.TextSecondaryColor;
            this.txtSearch.Enter += (s, e) => { if (txtSearch.Text == "Search by name or email...") { txtSearch.Text = ""; txtSearch.ForeColor = ThemeHelper.TextColor; } };
            this.txtSearch.Leave += (s, e) => { if (string.IsNullOrWhiteSpace(txtSearch.Text)) { txtSearch.Text = "Search by name or email..."; txtSearch.ForeColor = ThemeHelper.TextSecondaryColor; } };
            panelSearch.Controls.Add(this.txtSearch);
            
            this.btnSearch.Location = new System.Drawing.Point(430, 20);
            this.btnSearch.Size = new System.Drawing.Size(100, 40);
            this.btnSearch.Text = "Search";
            this.btnSearch.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            panelSearch.Controls.Add(this.btnSearch);
            this.Controls.Add(panelSearch);

            // Table Panel
            this.panelTable.BackColor = Color.White;
            this.panelTable.BorderStyle = BorderStyle.FixedSingle;
            this.panelTable.Dock = DockStyle.Fill;
            this.panelTable.Padding = new Padding(0);

            dgvStaff.AllowUserToAddRows = false;
            dgvStaff.AllowUserToDeleteRows = false;
            dgvStaff.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvStaff.Dock = DockStyle.Fill;
            dgvStaff.Name = "dgvStaff";
            dgvStaff.ReadOnly = true;
            dgvStaff.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.panelTable.Controls.Add(dgvStaff);
            this.Controls.Add(this.panelTable);

            // Action Buttons
            this.btnEdit.Location = new System.Drawing.Point(16, 680);
            this.btnEdit.Size = new System.Drawing.Size(100, 40);
            this.btnEdit.Text = "Edit";
            this.btnEdit.Click += BtnEdit_Click;
            this.Controls.Add(btnEdit);

            this.btnDelete.Location = new System.Drawing.Point(126, 680);
            this.btnDelete.Size = new System.Drawing.Size(100, 40);
            this.btnDelete.Text = "Delete";
            this.btnDelete.Click += BtnDelete_Click;
            this.Controls.Add(btnDelete);

            this.btnClose.Location = new System.Drawing.Point(1100, 680);
            this.btnClose.Size = new System.Drawing.Size(100, 40);
            this.btnClose.Text = "Close";
            this.btnClose.Click += (s, e) => this.Close();
            this.Controls.Add(btnClose);

            this.ClientSize = new System.Drawing.Size(1224, 740);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Text = "User Management";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ResumeLayout(false);
        }

        private void LoadStaff()
        {
            try
            {
                DataTable dt = DatabaseHelper.ExecuteStoredProcedure("sp_GetAllUsers");
                dgvStaff.DataSource = dt;
                
                // Set proper column widths instead of Fill mode
                if (dgvStaff.Columns.Count > 0)
                {
                    foreach (DataGridViewColumn column in dgvStaff.Columns)
                    {
                        column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                        // Set reasonable default widths
                        if (column.Name.Contains("id") || column.Name.Contains("_id"))
                            column.Width = 80;
                        else if (column.Name.Contains("username") || column.Name.Contains("email"))
                            column.Width = 150;
                        else if (column.Name.Contains("first_name") || column.Name.Contains("last_name"))
                            column.Width = 120;
                        else if (column.Name.Contains("phone"))
                            column.Width = 120;
                        else if (column.Name.Contains("role") || column.Name.Contains("status"))
                            column.Width = 100;
                        else if (column.Name.Contains("date") || column.Name.Contains("Date") || column.Name.Contains("_at"))
                            column.Width = 150;
                        else
                            column.Width = 120; // Default width
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading users: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            UserForm form = new UserForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadStaff();
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dgvStaff.SelectedRows.Count > 0)
            {
                int userId = Convert.ToInt32(dgvStaff.SelectedRows[0].Cells["user_id"].Value);
                UserForm form = new UserForm(userId);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadStaff();
                }
            }
            else
            {
                MessageBox.Show("Please select a user to edit.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgvStaff.SelectedRows.Count > 0)
            {
                int userId = Convert.ToInt32(dgvStaff.SelectedRows[0].Cells["user_id"].Value);
                string username = dgvStaff.SelectedRows[0].Cells["username"].Value.ToString();
                
                // Prevent deleting the current user
                if (userId == CurrentUser.UserId)
                {
                    MessageBox.Show("You cannot delete your own account.", "Cannot Delete", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (MessageBox.Show($"Are you sure you want to deactivate user '{username}'?\n\nNote: Users are deactivated rather than deleted.", "Confirm Deactivate", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try
                    {
                        var parameters = new MySqlParameter[]
                        {
                            new MySqlParameter("@p_user_id", userId)
                        };
                        DatabaseHelper.ExecuteStoredProcedure("sp_DeleteUser", parameters);
                        LoadStaff();
                        MessageBox.Show("User deactivated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error deactivating user: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a user to deactivate.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
