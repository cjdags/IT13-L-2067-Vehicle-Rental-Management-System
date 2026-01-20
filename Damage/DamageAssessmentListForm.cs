using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace VehicleRentalSystem
{
    public partial class DamageAssessmentListForm : Form
    {
        private DataGridView dgvDamageAssessments;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnDelete;
        private Button btnViewPhotos;
        private Button btnClose;

        public DamageAssessmentListForm()
        {
            InitializeComponent();
            ThemeHelper.ApplyStandardListLayout(
                this,
                title: "Damage Report Management",
                addButton: btnAdd,
                searchBox: txtSearch,
                searchButton: btnSearch,
                dataGridView: dgvDamageAssessments,
                leftFooterButtons: new[] { btnEdit, btnDelete, btnViewPhotos },
                rightFooterButtons: new[] { btnClose },
                searchPlaceholder: "Search by report ID, vehicle ID, or rental ID..."
            );

            ThemeHelper.WireSearch(txtSearch, btnSearch, dgvDamageAssessments, "Search by report ID, vehicle ID, or rental ID...");

            btnAdd.Click += BtnAdd_Click;
            btnEdit.Click += BtnEdit_Click;
            btnDelete.Click += BtnDelete_Click;
            btnViewPhotos.Click += BtnViewPhotos_Click;
            btnClose.Click += (s, e) => this.Close();
            dgvDamageAssessments.CellDoubleClick += (s, e) => BtnEdit_Click(s, e);

            LoadDamageAssessments();
            ApplyRbac();
        }

        private TextBox txtSearch;
        private Button btnSearch;
        private Panel panelHeader;
        private Label lblTitle;
        private Panel panelTable;

        private void InitializeComponent()
        {
            this.dgvDamageAssessments = new DataGridView();
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
            
            this.lblTitle.Text = "Damage Report Management";
            this.lblTitle.Font = new Font("Segoe UI", 28F, FontStyle.Bold);
            this.lblTitle.ForeColor = ThemeHelper.TextPrimaryColor;
            this.lblTitle.Location = new System.Drawing.Point(16, 20);
            this.lblTitle.Size = new System.Drawing.Size(500, 40);
            this.panelHeader.Controls.Add(this.lblTitle);

            this.btnAdd.Text = "+ Add Report";
            this.btnAdd.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.Controls.Add(this.panelHeader);

            // Edit/Delete buttons (wired after ApplyStandardListLayout)
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
            this.txtSearch.Text = "Search by report ID, vehicle ID, or rental ID...";
            this.txtSearch.ForeColor = ThemeHelper.TextSecondaryColor;
            this.txtSearch.Enter += (s, e) => { if (txtSearch.Text == "Search by report ID, vehicle ID, or rental ID...") { txtSearch.Text = ""; txtSearch.ForeColor = ThemeHelper.TextColor; } };
            this.txtSearch.Leave += (s, e) => { if (string.IsNullOrWhiteSpace(txtSearch.Text)) { txtSearch.Text = "Search by report ID, vehicle ID, or rental ID..."; txtSearch.ForeColor = ThemeHelper.TextSecondaryColor; } };
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

            dgvDamageAssessments.AllowUserToAddRows = false;
            dgvDamageAssessments.AllowUserToDeleteRows = false;
            dgvDamageAssessments.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvDamageAssessments.Dock = DockStyle.Fill;
            dgvDamageAssessments.Name = "dgvDamageAssessments";
            dgvDamageAssessments.ReadOnly = true;
            dgvDamageAssessments.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.panelTable.Controls.Add(dgvDamageAssessments);
            this.Controls.Add(this.panelTable);

            this.ClientSize = new System.Drawing.Size(1224, 740);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Text = "Damage Report Management";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ResumeLayout(false);
        }

        private void LoadDamageAssessments()
        {
            try
            {
                DataTable dt = DatabaseHelper.ExecuteStoredProcedure("sp_GetAllDamageReports");
                dgvDamageAssessments.DataSource = dt;
                
                // Set proper column widths instead of Fill mode
                if (dgvDamageAssessments.Columns.Count > 0)
                {
                    foreach (DataGridViewColumn column in dgvDamageAssessments.Columns)
                    {
                        column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                        // Set reasonable default widths
                        if (column.Name.Contains("id") || column.Name.Contains("_id"))
                            column.Width = 80;
                        else if (column.Name.Contains("description") || column.Name.Contains("Description"))
                            column.Width = 200;
                        else if (column.Name.Contains("location") || column.Name.Contains("Location"))
                            column.Width = 150;
                        else if (column.Name.Contains("cost") || column.Name.Contains("Cost"))
                            column.Width = 120;
                        else if (column.Name.Contains("status") || column.Name.Contains("Status"))
                            column.Width = 120;
                        else if (column.Name.Contains("date") || column.Name.Contains("Date"))
                            column.Width = 150;
                        else
                            column.Width = 120; // Default width
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading damage reports: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private DataRow GetSelectedRow()
        {
            if (dgvDamageAssessments?.CurrentRow?.DataBoundItem is DataRowView view)
                return view.Row;
            return null;
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            using var form = new DamageReportForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadDamageAssessments();
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            var row = GetSelectedRow();
            if (row == null)
            {
                MessageBox.Show("Please select a damage report to edit.", "Select a row", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using var form = new DamageReportForm(row);
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadDamageAssessments();
            }
        }

        private void BtnViewPhotos_Click(object sender, EventArgs e)
        {
            var row = GetSelectedRow();
            if (row == null)
            {
                MessageBox.Show("Please select a damage report to view photos.", "Select a row", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int damageId = Convert.ToInt32(row["damage_id"]);
            using var viewer = new DamagePhotoViewerForm(damageId);
            viewer.ShowDialog();
        }

        private void ApplyRbac()
        {
            bool isAdmin = string.Equals(CurrentUser.Role, "Admin", StringComparison.OrdinalIgnoreCase);
            if (!isAdmin)
            {
                btnDelete.Enabled = false;
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            var row = GetSelectedRow();
            if (row == null)
            {
                MessageBox.Show("Please select a damage report to delete.", "Select a row", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int damageId = Convert.ToInt32(row["damage_id"]);
            if (MessageBox.Show("Are you sure you want to delete this damage report?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                try
                {
                    DatabaseHelper.ExecuteStoredProcedure("sp_DeleteDamageReport", new[]
                    {
                        new MySql.Data.MySqlClient.MySqlParameter("@p_damage_id", damageId)
                    });
                    LoadDamageAssessments();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting damage report: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
