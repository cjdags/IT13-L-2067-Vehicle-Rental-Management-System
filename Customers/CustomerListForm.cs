using System;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace VehicleRentalSystem
{
    public partial class CustomerListForm : Form
    {
        public CustomerListForm()
        {
            InitializeComponent();
            ThemeHelper.ApplyStandardListLayout(
                this,
                title: "Customer Management",
                addButton: btnAdd,
                searchBox: txtSearch,
                searchButton: btnSearch,
                dataGridView: dgvCustomers,
                leftFooterButtons: new[] { btnEdit, btnDelete },
                rightFooterButtons: new[] { btnClose },
                searchPlaceholder: "Search by name, phone, or license..."
            );
            LoadCustomers();
        }

        private void LoadCustomers()
        {
            try
            {
                DataTable dt = DatabaseHelper.ExecuteStoredProcedure("sp_GetAllCustomers");
                dgvCustomers.DataSource = dt;
                
                // Set proper column widths instead of Fill mode
                if (dgvCustomers.Columns.Count > 0)
                {
                    foreach (DataGridViewColumn column in dgvCustomers.Columns)
                    {
                        column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                        // Set reasonable default widths based on column name
                        if (column.Name.Contains("id") || column.Name.Contains("_id"))
                            column.Width = 80;
                        else if (column.Name.Contains("name") || column.Name.Contains("Name"))
                            column.Width = 150;
                        else if (column.Name.Contains("email"))
                            column.Width = 200;
                        else if (column.Name.Contains("phone"))
                            column.Width = 120;
                        else if (column.Name.Contains("address"))
                            column.Width = 250;
                        else if (column.Name.Contains("license"))
                            column.Width = 120;
                        else if (column.Name.Contains("date") || column.Name.Contains("Date") || column.Name.Contains("_at"))
                            column.Width = 150;
                        else
                            column.Width = 120; // Default width
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading customers: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            CustomerForm form = new CustomerForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadCustomers();
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvCustomers.SelectedRows.Count > 0)
            {
                int customerId = Convert.ToInt32(dgvCustomers.SelectedRows[0].Cells["customer_id"].Value);
                CustomerForm form = new CustomerForm(customerId);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadCustomers();
                }
            }
            else
            {
                MessageBox.Show("Please select a customer to edit.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvCustomers.SelectedRows.Count > 0)
            {
                if (MessageBox.Show("Are you sure you want to delete this customer?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    int customerId = Convert.ToInt32(dgvCustomers.SelectedRows[0].Cells["customer_id"].Value);
                    var parameters = new MySqlParameter[]
                    {
                        new MySqlParameter("@p_customer_id", customerId)
                    };
                    DatabaseHelper.ExecuteStoredProcedure("sp_DeleteCustomer", parameters);
                    LoadCustomers();
                }
            }
            else
            {
                MessageBox.Show("Please select a customer to delete.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
