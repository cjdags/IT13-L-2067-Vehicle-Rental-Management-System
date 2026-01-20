using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.IO;

namespace VehicleRentalSystem
{
    public class MaintenanceForm : Form
    {
        private readonly bool _isEdit;
        private readonly DataRow _row;

        private ComboBox cmbVehicle;
        private ComboBox cmbType;
        private TextBox txtDescription;
        private TextBox txtCost;
        private DateTimePicker dtServiceDate;
        private DateTimePicker dtNextServiceDate;
        private TextBox txtProvider;
        private Button btnSave;
        private Button btnCancel;
        private Button btnAddPhoto;
        private ListBox lstPhotos;
        private readonly System.Collections.Generic.List<(byte[] Data, string ContentType, string Caption)> _pendingPhotos = new();

        public MaintenanceForm(DataRow existingRow = null)
        {
            _row = existingRow;
            _isEdit = existingRow != null;

            InitializeComponent();
            ThemeHelper.ApplyCardDialogLayout(this, _isEdit ? "Edit Maintenance" : "New Maintenance", new[] { btnSave, btnCancel });
            LoadVehicles();
            if (_isEdit) PopulateFieldsFromRow();
        }

        private void InitializeComponent()
        {
            cmbVehicle = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
            cmbType = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
            txtDescription = new TextBox { Multiline = true, Height = 90, ScrollBars = ScrollBars.Vertical };
            txtCost = new TextBox();
            dtServiceDate = new DateTimePicker { Format = DateTimePickerFormat.Short };
            dtNextServiceDate = new DateTimePicker { Format = DateTimePickerFormat.Short, ShowCheckBox = true };
            txtProvider = new TextBox();
            btnSave = new Button { Text = _isEdit ? "Save Changes" : "Create Record" };
            btnCancel = new Button { Text = "Cancel" };
            btnAddPhoto = new Button { Text = "Add Photo" };
            lstPhotos = new ListBox();

            cmbType.Items.AddRange(new object[] { "Regular Service", "Repair", "Inspection", "Oil Change" });
            if (cmbType.Items.Count > 0) cmbType.SelectedIndex = 0;

            btnSave.Click += BtnSave_Click;
            btnCancel.Click += (s, e) => DialogResult = DialogResult.Cancel;
            btnAddPhoto.Click += BtnAddPhoto_Click;

            var grid = new TableLayoutPanel
            {
                ColumnCount = 2,
                RowCount = 0,
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = new Padding(0),
                Margin = new Padding(0)
            };
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

            int currentRow = 0;
            int? lastColumn = null;
            Panel Field(string label, Control input, bool spanTwo = false, int? column = null)
            {
                var p = new Panel { Dock = DockStyle.Fill, AutoSize = true, Padding = new Padding(0, 0, 16, 12) };
                var lbl = new Label
                {
                    Text = label,
                    Dock = DockStyle.Top,
                    Height = 20,
                    Font = ThemeHelper.NormalFont,
                    ForeColor = ThemeHelper.TextColor,
                    Padding = new Padding(0, 0, 0, 4)
                };
                input.Dock = DockStyle.Top;
                input.Margin = new Padding(0);
                p.Controls.Add(input);
                p.Controls.Add(lbl);
                
                if (spanTwo)
                {
                    grid.RowCount++;
                    grid.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                    grid.Controls.Add(p, 0, currentRow);
                    grid.SetColumnSpan(p, 2);
                    currentRow++;
                    lastColumn = null;
                }
                else
                {
                    int col = column ?? (lastColumn == null ? 0 : 1);
                    bool needsNewRow = false;
                    
                    if (column.HasValue)
                    {
                        // Explicit column specified
                        if (lastColumn == null)
                        {
                            needsNewRow = true;
                        }
                        else if (lastColumn == col)
                        {
                            // Same column = new row (vertical stacking)
                            currentRow++;
                            needsNewRow = true;
                        }
                        else if (lastColumn == 1 && col == 0)
                        {
                            // Was in right column, now left = new row
                            currentRow++;
                            needsNewRow = true;
                        }
                        // else: lastColumn == 0 && col == 1, same row (horizontal)
                    }
                    else
                    {
                        // Auto placement
                        if (lastColumn == null || lastColumn == 1)
                        {
                            needsNewRow = true;
                        }
                    }
                    
                    if (needsNewRow)
                    {
                        grid.RowCount++;
                        grid.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                    }
                    
                    grid.Controls.Add(p, col, currentRow);
                    lastColumn = col;
                }
                return p;
            }

            // Vehicle - full width
            Field("Vehicle", cmbVehicle, spanTwo: true);
            
            // Maintenance Type and Cost - side by side (row 1)
            Field("Maintenance Type", cmbType, column: 0);
            Field("Cost", txtCost, column: 1);
            
            // Service Date and Next Service Date - side by side (row 2), vertically aligned with above
            Field("Service Date", dtServiceDate, column: 0);
            Field("Next Service Date", dtNextServiceDate, column: 1);
            
            // Service Provider - centered below (full width)
            Field("Service Provider", txtProvider, spanTwo: true);
            
            // Description - full width
            Field("Description", txtDescription, spanTwo: true);
            
            // Photos - expanded upward (increase height)
            lstPhotos.Height = 150; // Expanded from default
            Field("Photos", lstPhotos, spanTwo: true);
            
            // Add Photo button - smaller size, wrapped in a panel to control size
            btnAddPhoto.Height = 28; // Shrunk from default
            btnAddPhoto.Width = 100; // Smaller width
            btnAddPhoto.Dock = DockStyle.None; // Don't dock to fill
            btnAddPhoto.AutoSize = false;
            
            // Create a wrapper panel for the button to center it
            var buttonPanel = new Panel 
            { 
                Dock = DockStyle.Fill, 
                AutoSize = false, 
                Padding = new Padding(0, 0, 0, 12),
                Margin = new Padding(0),
                Height = 40 // Fixed height for button row
            };
            buttonPanel.Controls.Add(btnAddPhoto);
            btnAddPhoto.Anchor = AnchorStyles.Top;
            
            // Center the button horizontally
            void CenterButton()
            {
                if (buttonPanel.Width > 0)
                {
                    btnAddPhoto.Left = (buttonPanel.Width - btnAddPhoto.Width) / 2;
                }
            }
            
            // Update button position when panel resizes or loads
            buttonPanel.Resize += (s, e) => CenterButton();
            buttonPanel.Layout += (s, e) => CenterButton();
            
            grid.RowCount++;
            grid.RowStyles.Add(new RowStyle(SizeType.Absolute, 40)); // Fixed row height
            grid.Controls.Add(buttonPanel, 0, grid.RowCount - 1);
            grid.SetColumnSpan(buttonPanel, 2);
            
            // Center button after form is shown
            this.Load += (s, e) => CenterButton();

            Controls.Add(grid);

            ClientSize = new Size(820, 560);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;
        }

        private void LoadVehicles()
        {
            try
            {
                var vehicles = DatabaseHelper.ExecuteStoredProcedure("sp_GetAllVehicles");
                var items = vehicles.AsEnumerable()
                    .Select(v => new
                    {
                        Value = v.Field<int>("vehicle_id"),
                        Text = $"{v["make"]} {v["model"]} ({v["license_plate"]})"
                    })
                    .ToList();

                cmbVehicle.DataSource = items;
                cmbVehicle.DisplayMember = "Text";
                cmbVehicle.ValueMember = "Value";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading vehicles: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PopulateFieldsFromRow()
        {
            if (_row == null) return;

            if (_row.Table.Columns.Contains("vehicle_id"))
            {
                int vehicleId = Convert.ToInt32(_row["vehicle_id"]);
                cmbVehicle.SelectedValue = vehicleId;
            }

            cmbType.SelectedItem = _row["maintenance_type"]?.ToString() ?? "Regular Service";
            txtDescription.Text = _row["description"]?.ToString();
            txtCost.Text = _row["cost"]?.ToString();
            txtProvider.Text = _row["service_provider"]?.ToString();

            if (DateTime.TryParse(_row["service_date"]?.ToString(), out var serviceDate))
                dtServiceDate.Value = serviceDate;
            if (DateTime.TryParse(_row["next_service_date"]?.ToString(), out var nextService))
            {
                dtNextServiceDate.Value = nextService;
                dtNextServiceDate.Checked = true;
            }
            else dtNextServiceDate.Checked = false;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateInput())
                return;

            try
            {
                if (_isEdit) SaveEdit();
                else SaveCreate();

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving maintenance record: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveCreate()
        {
            int vehicleId = (int)cmbVehicle.SelectedValue;
            string type = cmbType.Text;
            string description = txtDescription.Text.Trim();
            decimal cost = ParseDecimal(txtCost.Text.Trim());
            DateTime serviceDate = dtServiceDate.Value.Date;
            object nextServiceDate = dtNextServiceDate.Checked ? dtNextServiceDate.Value.Date : DBNull.Value;
            string provider = txtProvider.Text.Trim();

            var parameters = new MySqlParameter[]
            {
                new MySqlParameter("@p_vehicle_id", vehicleId),
                new MySqlParameter("@p_maintenance_type", type),
                new MySqlParameter("@p_description", description),
                new MySqlParameter("@p_cost", cost),
                new MySqlParameter("@p_service_date", serviceDate),
                new MySqlParameter("@p_next_service_date", nextServiceDate),
                new MySqlParameter("@p_service_provider", provider)
            };

            var dt = DatabaseHelper.ExecuteStoredProcedure("sp_CreateMaintenanceRecord", parameters);
            int maintenanceId = dt.Rows.Count > 0 && dt.Columns.Contains("maintenance_id") ? Convert.ToInt32(dt.Rows[0]["maintenance_id"]) : 0;
            SavePhotos(maintenanceId);
        }

        private void SaveEdit()
        {
            int maintenanceId = Convert.ToInt32(_row["maintenance_id"]);
            int vehicleId = (int)cmbVehicle.SelectedValue;
            string type = cmbType.Text;
            string description = txtDescription.Text.Trim();
            decimal cost = ParseDecimal(txtCost.Text.Trim());
            DateTime serviceDate = dtServiceDate.Value.Date;
            object nextServiceDate = dtNextServiceDate.Checked ? dtNextServiceDate.Value.Date : DBNull.Value;
            string provider = txtProvider.Text.Trim();

            var parameters = new MySqlParameter[]
            {
                new MySqlParameter("@p_maintenance_id", maintenanceId),
                new MySqlParameter("@p_vehicle_id", vehicleId),
                new MySqlParameter("@p_maintenance_type", type),
                new MySqlParameter("@p_description", description),
                new MySqlParameter("@p_cost", cost),
                new MySqlParameter("@p_service_date", serviceDate),
                new MySqlParameter("@p_next_service_date", nextServiceDate),
                new MySqlParameter("@p_service_provider", provider)
            };

            DatabaseHelper.ExecuteStoredProcedure("sp_UpdateMaintenanceRecord", parameters);
            SavePhotos(maintenanceId);
        }

        private bool ValidateInput()
        {
            if (cmbVehicle.SelectedItem == null)
            {
                MessageBox.Show("Please select a vehicle.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (string.IsNullOrWhiteSpace(cmbType.Text))
            {
                MessageBox.Show("Maintenance type is required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtDescription.Text))
            {
                MessageBox.Show("Description is required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (!decimal.TryParse(txtCost.Text.Trim(), out _))
            {
                MessageBox.Show("Cost must be a number.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private static decimal ParseDecimal(string input)
        {
            return decimal.TryParse(input, out var val) ? val : 0m;
        }

        private void BtnAddPhoto_Click(object sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog();
            ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp";
            ofd.Multiselect = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                foreach (var file in ofd.FileNames)
                {
                    try
                    {
                        byte[] data = System.IO.File.ReadAllBytes(file);
                        string contentType = GetContentType(file);
                        _pendingPhotos.Add((data, contentType, System.IO.Path.GetFileName(file)));
                        lstPhotos.Items.Add(System.IO.Path.GetFileName(file));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed to add photo: {ex.Message}", "Photo Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void SavePhotos(int maintenanceId)
        {
            if (maintenanceId <= 0 || _pendingPhotos.Count == 0) return;
            foreach (var photo in _pendingPhotos)
            {
                DatabaseHelper.ExecuteStoredProcedure(
                    "sp_AddMaintenancePhoto",
                    new MySqlParameter("@p_maintenance_id", maintenanceId),
                    new MySqlParameter("@p_photo_data", photo.Data),
                    new MySqlParameter("@p_content_type", photo.ContentType),
                    new MySqlParameter("@p_caption", photo.Caption)
                );
            }
            _pendingPhotos.Clear();
        }

        private string GetContentType(string filePath)
        {
            string ext = System.IO.Path.GetExtension(filePath).ToLowerInvariant();
            return ext switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                _ => "application/octet-stream"
            };
        }
    }
}
