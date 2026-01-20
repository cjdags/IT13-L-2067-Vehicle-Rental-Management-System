using System;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace VehicleRentalSystem
{
    public partial class RentalListForm : Form
    {
        private DataGridView dgvRentals;
        private Button btnAdd;
        private Button btnView;
        private Button btnReturn;
        private Button btnClose;

        public RentalListForm()
        {
            InitializeComponent();
            ThemeHelper.ApplyStandardListLayout(
                this,
                title: "Rental Management",
                addButton: btnAdd,
                searchBox: txtSearch,
                searchButton: btnSearch,
                dataGridView: dgvRentals,
                leftFooterButtons: new[] { btnView, btnReturn },
                rightFooterButtons: new[] { btnClose },
                searchPlaceholder: "Search by customer, vehicle, or rental ID..."
            );
            LoadRentals();
        }

        private TextBox txtSearch;
        private Button btnSearch;
        private Panel panelHeader;
        private Label lblTitle;
        private Panel panelTable;

        private void InitializeComponent()
        {
            this.dgvRentals = new DataGridView();
            //this.btnAdd = new Button();
            this.btnView = new Button();
            this.btnReturn = new Button();
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
            this.lblTitle.Text = "Rental Management";
            this.lblTitle.Font = new Font("Segoe UI", 28F, FontStyle.Bold);
            this.lblTitle.ForeColor = ThemeHelper.TextPrimaryColor;
            this.lblTitle.Location = new System.Drawing.Point(16, 20);
            this.lblTitle.Size = new System.Drawing.Size(500, 40);
            this.panelHeader.Controls.Add(this.lblTitle);

            //this.btnAdd.Text = "+ Add Rental";
            //this.btnAdd.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            //this.btnAdd.Click += BtnAdd_Click;
            //this.Controls.Add(this.panelHeader);

            // Search Panel
            Panel panelSearch = new Panel();
            panelSearch.BackColor = ThemeHelper.BackgroundColor;
            panelSearch.Dock = DockStyle.Top;
            panelSearch.Height = 80;
            panelSearch.Padding = new Padding(16);
            
            this.txtSearch.Location = new System.Drawing.Point(16, 20);
            this.txtSearch.Size = new System.Drawing.Size(400, 40);
            this.txtSearch.Font = ThemeHelper.NormalFont;
            this.txtSearch.Text = "Search by customer, vehicle, or rental ID...";
            this.txtSearch.ForeColor = ThemeHelper.TextSecondaryColor;
            this.txtSearch.Enter += (s, e) => { if (txtSearch.Text == "Search by customer, vehicle, or rental ID...") { txtSearch.Text = ""; txtSearch.ForeColor = ThemeHelper.TextColor; } };
            this.txtSearch.Leave += (s, e) => { if (string.IsNullOrWhiteSpace(txtSearch.Text)) { txtSearch.Text = "Search by customer, vehicle, or rental ID..."; txtSearch.ForeColor = ThemeHelper.TextSecondaryColor; } };
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

            dgvRentals.AllowUserToAddRows = false;
            dgvRentals.AllowUserToDeleteRows = false;
            dgvRentals.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvRentals.Dock = DockStyle.Fill;
            dgvRentals.Name = "dgvRentals";
            dgvRentals.ReadOnly = true;
            dgvRentals.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.panelTable.Controls.Add(dgvRentals);
            this.Controls.Add(this.panelTable);

            // Action Buttons
            this.btnView.Location = new System.Drawing.Point(16, 680);
            this.btnView.Size = new System.Drawing.Size(120, 40);
            this.btnView.Text = "View Details";
            this.btnView.Click += BtnView_Click;
            this.Controls.Add(btnView);

            this.btnReturn.Location = new System.Drawing.Point(146, 680);
            this.btnReturn.Size = new System.Drawing.Size(120, 40);
            this.btnReturn.Text = "Return Vehicle";
            this.btnReturn.Click += BtnReturn_Click;
            this.Controls.Add(btnReturn);

            this.btnClose.Location = new System.Drawing.Point(1100, 680);
            this.btnClose.Size = new System.Drawing.Size(100, 40);
            this.btnClose.Text = "Close";
            this.btnClose.Click += BtnClose_Click;
            this.Controls.Add(btnClose);

            this.ClientSize = new System.Drawing.Size(1224, 740);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Text = "Rental Management";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ResumeLayout(false);
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            using var form = new ReservationWizardForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadRentals();
            }
        }

        private void LoadRentals()
        {
            try
            {
                DataTable dt = DatabaseHelper.ExecuteStoredProcedure("sp_GetAllRentals");
                dgvRentals.DataSource = dt;
                
                // Set proper column widths instead of Fill mode
                if (dgvRentals.Columns.Count > 0)
                {
                    foreach (DataGridViewColumn column in dgvRentals.Columns)
                    {
                        column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                        // Set reasonable default widths
                        if (column.Name.Contains("id") || column.Name.Contains("_id"))
                            column.Width = 80;
                        else if (column.Name.Contains("customer") || column.Name.Contains("Customer"))
                            column.Width = 150;
                        else if (column.Name.Contains("vehicle") || column.Name.Contains("Vehicle"))
                            column.Width = 150;
                        else if (column.Name.Contains("date") || column.Name.Contains("Date"))
                            column.Width = 150;
                        else if (column.Name.Contains("mileage") || column.Name.Contains("mile"))
                            column.Width = 100;
                        else if (column.Name.Contains("status"))
                            column.Width = 100;
                        else if (column.Name.Contains("amount") || column.Name.Contains("total"))
                            column.Width = 120;
                        else
                            column.Width = 120; // Default width
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading rentals: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnView_Click(object sender, EventArgs e)
        {
            if (dgvRentals.SelectedRows.Count > 0)
            {
                int rentalId = Convert.ToInt32(dgvRentals.SelectedRows[0].Cells["rental_id"].Value);
                // Could open a detail form here
                MessageBox.Show($"Rental ID: {rentalId}", "Rental Details", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnReturn_Click(object sender, EventArgs e)
        {
            if (dgvRentals.SelectedRows.Count > 0)
            {
                int rentalId = Convert.ToInt32(dgvRentals.SelectedRows[0].Cells["rental_id"].Value);
                var statusObj = dgvRentals.SelectedRows[0].Cells["status"]?.Value;
                var status = statusObj?.ToString() ?? "";
                if (string.Equals(status, "Completed", StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show("This rental is already returned/completed.", "Already Returned", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                ReturnRentalForm form = new ReturnRentalForm(rentalId);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadRentals();
                }
            }
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
