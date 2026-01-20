using System;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace VehicleRentalSystem
{
    public partial class VehicleTypeListForm : Form
    {
        private DataGridView dgvVehicleTypes;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnDelete;
        private Button btnClose;

        public VehicleTypeListForm()
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
                title: "Vehicle Category Management",
                addButton: btnAdd,
                searchBox: txtSearch,
                searchButton: btnSearch,
                dataGridView: dgvVehicleTypes,
                leftFooterButtons: new[] { btnEdit, btnDelete },
                rightFooterButtons: new[] { btnClose },
                searchPlaceholder: "Search by category name..."
            );
            LoadVehicleTypes();
        }

        private TextBox txtSearch;
        private Button btnSearch;
        private Panel panelHeader;
        private Label lblTitle;
        private Panel panelTable;

        private void InitializeComponent()
        {
            this.dgvVehicleTypes = new DataGridView();
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
            
            this.lblTitle.Text = "Vehicle Category Management";
            this.lblTitle.Font = new Font("Segoe UI", 28F, FontStyle.Bold);
            this.lblTitle.ForeColor = ThemeHelper.TextPrimaryColor;
            this.lblTitle.Location = new System.Drawing.Point(16, 20);
            this.lblTitle.Size = new System.Drawing.Size(500, 40);
            this.panelHeader.Controls.Add(this.lblTitle);
            
            this.btnAdd.Location = new System.Drawing.Point(700, 20);
            this.btnAdd.Size = new System.Drawing.Size(120, 40);
            this.btnAdd.Text = "+ Add Category";
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
            this.txtSearch.Text = "Search by category name...";
            this.txtSearch.ForeColor = ThemeHelper.TextSecondaryColor;
            this.txtSearch.Enter += (s, e) => { if (txtSearch.Text == "Search by category name...") { txtSearch.Text = ""; txtSearch.ForeColor = ThemeHelper.TextColor; } };
            this.txtSearch.Leave += (s, e) => { if (string.IsNullOrWhiteSpace(txtSearch.Text)) { txtSearch.Text = "Search by category name..."; txtSearch.ForeColor = ThemeHelper.TextSecondaryColor; } };
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

            dgvVehicleTypes.AllowUserToAddRows = false;
            dgvVehicleTypes.AllowUserToDeleteRows = false;
            dgvVehicleTypes.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvVehicleTypes.Dock = DockStyle.Fill;
            dgvVehicleTypes.Name = "dgvVehicleTypes";
            dgvVehicleTypes.ReadOnly = true;
            dgvVehicleTypes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.panelTable.Controls.Add(dgvVehicleTypes);
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

            this.btnClose.Location = new System.Drawing.Point(712, 680);
            this.btnClose.Size = new System.Drawing.Size(100, 40);
            this.btnClose.Text = "Close";
            this.btnClose.Click += (s, e) => this.Close();
            this.Controls.Add(btnClose);

            this.ClientSize = new System.Drawing.Size(824, 740);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Text = "Vehicle Category Management";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ResumeLayout(false);
        }

        private void LoadVehicleTypes()
        {
            try
            {
                DataTable dt = DatabaseHelper.ExecuteStoredProcedure("sp_GetAllVehicleCategories");
                dgvVehicleTypes.DataSource = dt;
                
                // Set proper column widths instead of Fill mode
                if (dgvVehicleTypes.Columns.Count > 0)
                {
                    foreach (DataGridViewColumn column in dgvVehicleTypes.Columns)
                    {
                        column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                        // Set reasonable default widths
                        if (column.Name.Contains("id") || column.Name.Contains("_id"))
                            column.Width = 100;
                        else if (column.Name.Contains("name") || column.Name.Contains("Name"))
                            column.Width = 200;
                        else if (column.Name.Contains("description") || column.Name.Contains("Description"))
                            column.Width = 300;
                        else if (column.Name.Contains("date") || column.Name.Contains("Date") || column.Name.Contains("_at"))
                            column.Width = 180;
                        else
                            column.Width = 150; // Default width
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading vehicle categories: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            using (Form inputForm = new Form())
            {
                inputForm.Text = "Add Vehicle Type";
                inputForm.Size = new System.Drawing.Size(420, 200);
                inputForm.StartPosition = FormStartPosition.CenterParent;

                Label lblName = new Label() { Text = "Type Name:", Location = new System.Drawing.Point(10, 15), Size = new System.Drawing.Size(100, 20) };
                TextBox txtName = new TextBox() { Location = new System.Drawing.Point(120, 12), Size = new System.Drawing.Size(260, 22) };
                Label lblDesc = new Label() { Text = "Description:", Location = new System.Drawing.Point(10, 45), Size = new System.Drawing.Size(100, 20) };
                TextBox txtDesc = new TextBox() { Location = new System.Drawing.Point(120, 42), Size = new System.Drawing.Size(260, 22) };
                Label lblDaily = new Label() { Text = "Daily Rate:", Location = new System.Drawing.Point(10, 75), Size = new System.Drawing.Size(100, 20) };
                NumericUpDown numDaily = new NumericUpDown() { Location = new System.Drawing.Point(120, 72), Size = new System.Drawing.Size(120, 22), DecimalPlaces = 2, Maximum = 100000, Minimum = 0, Value = 0 };

                Button btnOK = new Button() { Text = "OK", DialogResult = DialogResult.OK, Location = new System.Drawing.Point(195, 120), Size = new System.Drawing.Size(75, 28) };
                Button btnCancel = new Button() { Text = "Cancel", DialogResult = DialogResult.Cancel, Location = new System.Drawing.Point(280, 120), Size = new System.Drawing.Size(75, 28) };

                inputForm.Controls.AddRange(new Control[] { lblName, txtName, lblDesc, txtDesc, lblDaily, numDaily, btnOK, btnCancel });
                inputForm.AcceptButton = btnOK;
                inputForm.CancelButton = btnCancel;

                if (inputForm.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(txtName.Text))
                {
                    try
                    {
                        // Create category
                        var catParams = new MySqlParameter[]
                        {
                            new MySqlParameter("@p_category_name", txtName.Text),
                            new MySqlParameter("@p_description", txtDesc.Text)
                        };
                        var catResult = DatabaseHelper.ExecuteStoredProcedure("sp_CreateVehicleCategory", catParams);
                        int categoryId = 0;
                        if (catResult.Rows.Count > 0 && catResult.Columns.Contains("category_id"))
                            categoryId = Convert.ToInt32(catResult.Rows[0]["category_id"]);

                        // Create default rental rate for this category
                        if (categoryId > 0)
                        {
                            DatabaseHelper.ExecuteStoredProcedure("sp_CreateRentalRate",
                                new MySqlParameter("@p_category_id", categoryId),
                                new MySqlParameter("@p_rate_name", "Default Daily Rate"),
                                new MySqlParameter("@p_daily_rate", numDaily.Value),
                                new MySqlParameter("@p_weekly_rate", DBNull.Value),
                                new MySqlParameter("@p_monthly_rate", DBNull.Value),
                                new MySqlParameter("@p_effective_from", DateTime.Today),
                                new MySqlParameter("@p_effective_to", DBNull.Value),
                                new MySqlParameter("@p_is_active", true)
                            );
                        }

                        LoadVehicleTypes();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error adding vehicle type: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dgvVehicleTypes.SelectedRows.Count > 0)
            {
                int categoryId = Convert.ToInt32(dgvVehicleTypes.SelectedRows[0].Cells["category_id"].Value);
                string currentName = dgvVehicleTypes.SelectedRows[0].Cells["category_name"].Value.ToString();
                string currentDesc = dgvVehicleTypes.SelectedRows[0].Cells["description"].Value?.ToString() ?? "";

                using (Form inputForm = new Form())
                {
                    inputForm.Text = "Edit Vehicle Type";
                    inputForm.Size = new System.Drawing.Size(400, 150);
                    inputForm.StartPosition = FormStartPosition.CenterParent;

                    Label lblName = new Label() { Text = "Type Name:", Location = new System.Drawing.Point(10, 15), Size = new System.Drawing.Size(100, 20) };
                    TextBox txtName = new TextBox() { Text = currentName, Location = new System.Drawing.Point(120, 12), Size = new System.Drawing.Size(250, 20) };
                    Label lblDesc = new Label() { Text = "Description:", Location = new System.Drawing.Point(10, 45), Size = new System.Drawing.Size(100, 20) };
                    TextBox txtDesc = new TextBox() { Text = currentDesc, Location = new System.Drawing.Point(120, 42), Size = new System.Drawing.Size(250, 20) };
                    Button btnOK = new Button() { Text = "OK", DialogResult = DialogResult.OK, Location = new System.Drawing.Point(195, 75), Size = new System.Drawing.Size(75, 25) };
                    Button btnCancel = new Button() { Text = "Cancel", DialogResult = DialogResult.Cancel, Location = new System.Drawing.Point(280, 75), Size = new System.Drawing.Size(75, 25) };

                    inputForm.Controls.AddRange(new Control[] { lblName, txtName, lblDesc, txtDesc, btnOK, btnCancel });
                    inputForm.AcceptButton = btnOK;
                    inputForm.CancelButton = btnCancel;

                    if (inputForm.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(txtName.Text))
                    {
                        try
                        {
                            var parameters = new MySqlParameter[]
                            {
                                new MySqlParameter("@p_category_id", categoryId),
                                new MySqlParameter("@p_category_name", txtName.Text),
                                new MySqlParameter("@p_description", txtDesc.Text)
                            };
                            DatabaseHelper.ExecuteStoredProcedure("sp_UpdateVehicleCategory", parameters);
                            LoadVehicleTypes();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error updating vehicle type: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgvVehicleTypes.SelectedRows.Count > 0)
            {
                if (MessageBox.Show("Are you sure you want to delete this vehicle type?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    int categoryId = Convert.ToInt32(dgvVehicleTypes.SelectedRows[0].Cells["category_id"].Value);
                    var parameters = new MySqlParameter[]
                    {
                        new MySqlParameter("@p_category_id", categoryId)
                    };
                    DatabaseHelper.ExecuteStoredProcedure("sp_DeleteVehicleCategory", parameters);
                    LoadVehicleTypes();
                }
            }
        }
    }
}
