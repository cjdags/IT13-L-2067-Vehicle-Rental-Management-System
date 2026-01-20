using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace VehicleRentalSystem
{
    public partial class MaintenanceListForm : Form
    {
        private DataGridView dgvMaintenance;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnDelete;
        private Button btnClose;
        private Button btnViewPhotos;

        public MaintenanceListForm()
        {
            InitializeComponent();
            ThemeHelper.ApplyStandardListLayout(
                this,
                title: "Maintenance Management",
                addButton: btnAdd,
                searchBox: txtSearch,
                searchButton: btnSearch,
                dataGridView: dgvMaintenance,
                leftFooterButtons: new[] { btnEdit, btnDelete, btnViewPhotos },
                rightFooterButtons: new[] { btnClose },
                searchPlaceholder: "Search by vehicle ID or description..."
            );

            ThemeHelper.WireSearch(txtSearch, btnSearch, dgvMaintenance, "Search by vehicle ID or description...");
            btnAdd.Click += BtnAdd_Click;
            btnEdit.Click += BtnEdit_Click;
            btnDelete.Click += BtnDelete_Click;
            btnViewPhotos.Click += BtnViewPhotos_Click;
            btnClose.Click += (s, e) => this.Close();
            dgvMaintenance.CellDoubleClick += (s, e) => BtnEdit_Click(s, e);
            LoadMaintenance();
            ApplyRbac();
        }

        private TextBox txtSearch;
        private Button btnSearch;
        private Panel panelHeader;
        private Label lblTitle;
        private Panel panelTable;

        private void InitializeComponent()
        {
            this.dgvMaintenance = new DataGridView();
            this.btnAdd = new Button();
            this.btnEdit = new Button();
            this.btnDelete = new Button();
            this.btnClose = new Button();
            this.btnViewPhotos = new Button();
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
            
            this.lblTitle.Text = "Maintenance Management";
            this.lblTitle.Font = new Font("Segoe UI", 28F, FontStyle.Bold);
            this.lblTitle.ForeColor = ThemeHelper.TextPrimaryColor;
            this.lblTitle.Location = new System.Drawing.Point(16, 20);
            this.lblTitle.Size = new System.Drawing.Size(500, 40);
            this.panelHeader.Controls.Add(this.lblTitle);
            
            this.btnAdd.Location = new System.Drawing.Point(1100, 20);
            this.btnAdd.Size = new System.Drawing.Size(120, 40);
            this.btnAdd.Text = "+ Add Record";
            this.btnAdd.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.panelHeader.Controls.Add(this.btnAdd);
            this.Controls.Add(this.panelHeader);

            //buttons
            this.btnEdit.Text = "Edit";
            this.btnEdit.Font = new Font("Segoe UI", 10F, FontStyle.Bold);

            this.btnDelete.Text = "Delete";
            this.btnDelete.Font = new Font("Segoe UI", 10F, FontStyle.Bold);

            this.btnViewPhotos.Text = "View Photos";
            this.btnViewPhotos.Font = new Font("Segoe UI", 10F, FontStyle.Bold);

            this.btnClose.Text = "Close";
            this.btnClose.Font = new Font("Segoe UI", 10F, FontStyle.Bold);

            // Search Panel
            Panel panelSearch = new Panel();
            panelSearch.BackColor = ThemeHelper.BackgroundColor;
            panelSearch.Dock = DockStyle.Top;
            panelSearch.Height = 80;
            panelSearch.Padding = new Padding(16);
            
            this.txtSearch.Location = new System.Drawing.Point(16, 20);
            this.txtSearch.Size = new System.Drawing.Size(400, 40);
            this.txtSearch.Font = ThemeHelper.NormalFont;
            this.txtSearch.Text = "Search by vehicle ID or description...";
            this.txtSearch.ForeColor = ThemeHelper.TextSecondaryColor;
            this.txtSearch.Enter += (s, e) => { if (txtSearch.Text == "Search by vehicle ID or description...") { txtSearch.Text = ""; txtSearch.ForeColor = ThemeHelper.TextColor; } };
            this.txtSearch.Leave += (s, e) => { if (string.IsNullOrWhiteSpace(txtSearch.Text)) { txtSearch.Text = "Search by vehicle ID or description..."; txtSearch.ForeColor = ThemeHelper.TextSecondaryColor; } };
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

            dgvMaintenance.AllowUserToAddRows = false;
            dgvMaintenance.AllowUserToDeleteRows = false;
            dgvMaintenance.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvMaintenance.Dock = DockStyle.Fill;
            dgvMaintenance.Name = "dgvMaintenance";
            dgvMaintenance.ReadOnly = true;
            dgvMaintenance.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.panelTable.Controls.Add(dgvMaintenance);
            this.Controls.Add(this.panelTable);

            // Action Buttons
            this.ClientSize = new System.Drawing.Size(1224, 740);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Text = "Maintenance Management";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ResumeLayout(false);
        }

        private void LoadMaintenance()
        {
            try
            {
                DataTable dt = DatabaseHelper.ExecuteStoredProcedure("sp_GetAllMaintenanceRecords");
                dgvMaintenance.DataSource = dt;
                
                // Set proper column widths instead of Fill mode
                if (dgvMaintenance.Columns.Count > 0)
                {
                    foreach (DataGridViewColumn column in dgvMaintenance.Columns)
                    {
                        column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                        // Set reasonable default widths
                        if (column.Name.Contains("id") || column.Name.Contains("_id"))
                            column.Width = 80;
                        else if (column.Name.Contains("type") || column.Name.Contains("Type"))
                            column.Width = 120;
                        else if (column.Name.Contains("description") || column.Name.Contains("Description"))
                            column.Width = 200;
                        else if (column.Name.Contains("cost") || column.Name.Contains("Cost"))
                            column.Width = 100;
                        else if (column.Name.Contains("date") || column.Name.Contains("Date"))
                            column.Width = 150;
                        else if (column.Name.Contains("provider") || column.Name.Contains("Provider"))
                            column.Width = 150;
                        else
                            column.Width = 120; // Default width
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading maintenance records: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private DataRow GetSelectedRow()
        {
            if (dgvMaintenance?.CurrentRow?.DataBoundItem is DataRowView view)
                return view.Row;
            return null;
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            using var form = new MaintenanceForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadMaintenance();
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            var row = GetSelectedRow();
            if (row == null)
            {
                MessageBox.Show("Please select a maintenance record to edit.", "Select a row", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using var form = new MaintenanceForm(row);
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadMaintenance();
            }
        }

        private void BtnViewPhotos_Click(object sender, EventArgs e)
        {
            var row = GetSelectedRow();
            if (row == null)
            {
                MessageBox.Show("Please select a maintenance record to view photos.", "Select a row", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int maintenanceId = Convert.ToInt32(row["maintenance_id"]);
            using var viewer = new MaintenancePhotoViewerForm(maintenanceId);
            viewer.ShowDialog();
        }

        private void ApplyRbac()
        {
            bool isAdmin = string.Equals(CurrentUser.Role, "Admin", StringComparison.OrdinalIgnoreCase);
            if (!isAdmin)
            {
                btnAdd.Enabled = false;
                btnEdit.Enabled = false;
                btnDelete.Enabled = false;
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            var row = GetSelectedRow();
            if (row == null)
            {
                MessageBox.Show("Please select a maintenance record to delete.", "Select a row", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int maintenanceId = Convert.ToInt32(row["maintenance_id"]);
            int vehicleId = row.Table.Columns.Contains("vehicle_id") ? Convert.ToInt32(row["vehicle_id"]) : 0;
            if (MessageBox.Show("Are you sure you want to delete this maintenance record?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                try
                {
                    DatabaseHelper.ExecuteStoredProcedure("sp_DeleteMaintenanceRecord", new[]
                    {
                        new MySqlParameter("@p_maintenance_id", maintenanceId)
                    });
                    if (vehicleId > 0)
                    {
                        DatabaseHelper.ExecuteNonQuery("UPDATE Vehicles SET status = 'Available' WHERE vehicle_id = @vid",
                            new MySqlParameter("@vid", vehicleId));
                    }
                    LoadMaintenance();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting maintenance record: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
