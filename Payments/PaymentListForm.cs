using System;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace VehicleRentalSystem
{
    public partial class PaymentListForm : Form
    {
        private DataGridView dgvPayments;
        private Button btnAdd;
        private Button btnView;
        private Button btnClose;

        public PaymentListForm()
        {
            InitializeComponent();
            ThemeHelper.ApplyStandardListLayout(
                this,
                title: "Payment Management",
                addButton: btnAdd,
                searchBox: txtSearch,
                searchButton: btnSearch,
                dataGridView: dgvPayments,
                leftFooterButtons: new[] { btnView },
                rightFooterButtons: new[] { btnClose },
                searchPlaceholder: "Search by payment ID, rental ID, or amount..."
            );
            LoadPayments();
        }

        private TextBox txtSearch;
        private Button btnSearch;
        private Panel panelHeader;
        private Label lblTitle;
        private Panel panelTable;

        private void InitializeComponent()
        {
            this.dgvPayments = new DataGridView();
            this.btnAdd = new Button();
            this.btnView = new Button();
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
            
            this.lblTitle.Text = "Payment Management";
            this.lblTitle.Font = new Font("Segoe UI", 28F, FontStyle.Bold);
            this.lblTitle.ForeColor = ThemeHelper.TextPrimaryColor;
            this.lblTitle.Location = new System.Drawing.Point(16, 20);
            this.lblTitle.Size = new System.Drawing.Size(500, 40);
            this.panelHeader.Controls.Add(this.lblTitle);
            
            this.btnAdd.Location = new System.Drawing.Point(1100, 20);
            this.btnAdd.Size = new System.Drawing.Size(120, 40);
            this.btnAdd.Text = "+ Add Payment";
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
            this.txtSearch.Text = "Search by payment ID, rental ID, or amount...";
            this.txtSearch.ForeColor = ThemeHelper.TextSecondaryColor;
            this.txtSearch.Enter += (s, e) => { if (txtSearch.Text == "Search by payment ID, rental ID, or amount...") { txtSearch.Text = ""; txtSearch.ForeColor = ThemeHelper.TextColor; } };
            this.txtSearch.Leave += (s, e) => { if (string.IsNullOrWhiteSpace(txtSearch.Text)) { txtSearch.Text = "Search by payment ID, rental ID, or amount..."; txtSearch.ForeColor = ThemeHelper.TextSecondaryColor; } };
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

            dgvPayments.AllowUserToAddRows = false;
            dgvPayments.AllowUserToDeleteRows = false;
            dgvPayments.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvPayments.Dock = DockStyle.Fill;
            dgvPayments.Name = "dgvPayments";
            dgvPayments.ReadOnly = true;
            dgvPayments.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.panelTable.Controls.Add(dgvPayments);
            this.Controls.Add(this.panelTable);

            // Action Buttons
            this.btnView.Location = new System.Drawing.Point(16, 680);
            this.btnView.Size = new System.Drawing.Size(120, 40);
            this.btnView.Text = "View Details";
            this.btnView.Click += BtnView_Click;
            this.Controls.Add(btnView);

            this.btnClose.Location = new System.Drawing.Point(1100, 680);
            this.btnClose.Size = new System.Drawing.Size(100, 40);
            this.btnClose.Text = "Close";
            this.btnClose.Click += (s, e) => this.Close();
            this.Controls.Add(btnClose);

            this.ClientSize = new System.Drawing.Size(1224, 740);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Text = "Payment Management";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ResumeLayout(false);
        }

        private void LoadPayments()
        {
            try
            {
                DataTable dt = DatabaseHelper.ExecuteStoredProcedure("sp_GetAllPayments");
                dgvPayments.DataSource = dt;
                
                // Set proper column widths instead of Fill mode
                if (dgvPayments.Columns.Count > 0)
                {
                    foreach (DataGridViewColumn column in dgvPayments.Columns)
                    {
                        column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                        // Set reasonable default widths
                        if (column.Name.Contains("id") || column.Name.Contains("_id"))
                            column.Width = 80;
                        else if (column.Name.Contains("type") || column.Name.Contains("method"))
                            column.Width = 120;
                        else if (column.Name.Contains("amount"))
                            column.Width = 120;
                        else if (column.Name.Contains("reference") || column.Name.Contains("transaction"))
                            column.Width = 150;
                        else if (column.Name.Contains("status"))
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
                MessageBox.Show($"Error loading payments: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            PaymentForm form = new PaymentForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadPayments();
            }
        }

        private void BtnView_Click(object sender, EventArgs e)
        {
            if (dgvPayments.SelectedRows.Count > 0)
            {
                int paymentId = Convert.ToInt32(dgvPayments.SelectedRows[0].Cells["payment_id"].Value);
                PaymentForm form = new PaymentForm(paymentId);
                form.ShowDialog();
                LoadPayments();
            }
            else
            {
                MessageBox.Show("Please select a payment to view.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
