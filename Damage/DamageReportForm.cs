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
    public class DamageReportForm : Form
    {
        private readonly bool _isEdit;
        private readonly DataRow _row;

        private ComboBox cmbRental;
        private Label lblRentalReadonly;
        private TextBox txtDescription;
        private TextBox txtLocation;
        private TextBox txtEstimatedCost;
        private ComboBox cmbStatus;
        private TextBox txtApprovedBy;
        private DateTimePicker dtApprovalDate;
        private DateTimePicker dtRepairDate;
        private TextBox txtActualRepairCost;
        private Button btnSave;
        private Button btnCancel;
        private Button btnAddPhoto;
        private ListBox lstPhotos;
        private readonly System.Collections.Generic.List<(byte[] Data, string ContentType, string Caption)> _pendingPhotos = new();

        public DamageReportForm(DataRow existingRow = null)
        {
            _row = existingRow;
            _isEdit = existingRow != null;

            InitializeComponent();
            ThemeHelper.ApplyCardDialogLayout(this, _isEdit ? "Edit Damage Report" : "New Damage Report", new[] { btnSave, btnCancel });
            LoadRentals();
            if (_isEdit) PopulateFieldsFromRow();
            ApplyRbac();
        }

        private void InitializeComponent()
        {
            // Standardize input control heights
            const int inputHeight = 23;
            const int labelHeight = 20;
            const int labelBottomPadding = 4;
            const int fieldBottomPadding = 12;
            const int columnPadding = 16;

            cmbRental = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Height = inputHeight };
            lblRentalReadonly = new Label { AutoSize = true, Font = ThemeHelper.NormalFont, ForeColor = ThemeHelper.TextColor, Padding = new Padding(0, 10, 0, 0) };
            txtDescription = new TextBox { Multiline = true, Height = 90, ScrollBars = ScrollBars.Vertical };
            txtLocation = new TextBox { Height = inputHeight };
            txtEstimatedCost = new TextBox { Height = inputHeight };
            cmbStatus = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Height = inputHeight };
            txtApprovedBy = new TextBox { Height = inputHeight };
            dtApprovalDate = new DateTimePicker { Format = DateTimePickerFormat.Custom, CustomFormat = "yyyy-MM-dd HH:mm", ShowCheckBox = true, Height = inputHeight };
            dtRepairDate = new DateTimePicker { Format = DateTimePickerFormat.Custom, CustomFormat = "yyyy-MM-dd HH:mm", ShowCheckBox = true, Height = inputHeight };
            txtActualRepairCost = new TextBox { Height = inputHeight };
            btnSave = new Button { Text = _isEdit ? "Save Changes" : "Create Report" };
            btnCancel = new Button { Text = "Cancel" };
            btnAddPhoto = new Button { Text = "Add Photo" };
            lstPhotos = new ListBox { Height = 120 };

            cmbStatus.Items.AddRange(new object[] { "Reported", "Under Review", "Approved", "Rejected", "Repaired" });
            cmbStatus.SelectedIndex = 0;
            cmbStatus.Enabled = _isEdit; // new reports start as Reported

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
                var p = new Panel 
                { 
                    Dock = DockStyle.Fill, 
                    AutoSize = false,
                    Padding = new Padding(0, 0, spanTwo ? 0 : columnPadding, fieldBottomPadding),
                    Margin = new Padding(0)
                };
                var lbl = new Label
                {
                    Text = label,
                    Dock = DockStyle.Top,
                    Height = labelHeight,
                    Font = ThemeHelper.NormalFont,
                    ForeColor = ThemeHelper.TextColor,
                    Padding = new Padding(0, 0, 0, labelBottomPadding),
                    Margin = new Padding(0),
                    AutoSize = false
                };
                
                // Ensure consistent height for all single-line inputs
                if (!(input is TextBox && ((TextBox)input).Multiline) && !(input is ListBox))
                {
                    input.Height = inputHeight;
                }
                
                input.Dock = DockStyle.Fill;
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

            // Rental - full width
            if (_isEdit)
                Field("Rental", lblRentalReadonly, spanTwo: true);
            else
                Field("Rental", cmbRental, spanTwo: true);

            // Damage Description - full width
            Field("Damage Description", txtDescription, spanTwo: true);

            // Damage Location and Estimated Cost - side by side (row 1)
            Field("Damage Location", txtLocation, column: 0);
            Field("Estimated Cost", txtEstimatedCost, column: 1);

            // Status and Approved By - side by side (row 2), vertically aligned with above
            Field("Status", cmbStatus, column: 0);
            Field("Approved By (User ID)", txtApprovedBy, column: 1);

            // Approval Date and Repair Date - horizontally aligned (side by side)
            Field("Approval Date", dtApprovalDate, column: 0);
            Field("Repair Date", dtRepairDate, column: 1);

            // Actual Repair Cost - centered below approval and repair dates
            Field("Actual Repair Cost", txtActualRepairCost, spanTwo: true);

            // Photos - expanded upward (increase height)
            lstPhotos.Height = 150; // Expanded from 120
            Field("Photos", lstPhotos, spanTwo: true);

            // Add Photo button - smaller size, wrapped in a panel to control size (consistent with MaintenanceForm)
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
            grid.Controls.Add(buttonPanel, 0, currentRow);
            grid.SetColumnSpan(buttonPanel, 2);
            
            // Center button after form is shown
            this.Load += (s, e) => CenterButton();

            Controls.Add(grid);

            ClientSize = new Size(900, 640);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;
        }

        private void LoadRentals()
        {
            try
            {
                var rentals = DatabaseHelper.ExecuteStoredProcedure("sp_GetAllRentals");
                var items = rentals.AsEnumerable()
                    .Select(r => new
                    {
                        Value = r.Field<int>("rental_id"),
                        Text = $"R#{r.Field<int>("rental_id")} - {r["vehicle_make"]} {r["vehicle_model"]} ({r["license_plate"]})"
                    })
                    .ToList();

                cmbRental.DataSource = items;
                cmbRental.DisplayMember = "Text";
                cmbRental.ValueMember = "Value";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading rentals: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PopulateFieldsFromRow()
        {
            if (_row == null) return;

            if (_row.Table.Columns.Contains("rental_id"))
            {
                int rentalId = Convert.ToInt32(_row["rental_id"]);
                lblRentalReadonly.Text = $"R#{rentalId} - {_row["vehicle_make"]} {_row["vehicle_model"]} ({_row["license_plate"]})";
                cmbRental.SelectedValue = rentalId;
            }

            txtDescription.Text = _row["damage_description"]?.ToString();
            txtLocation.Text = _row["damage_location"]?.ToString();
            txtEstimatedCost.Text = _row["estimated_cost"]?.ToString();

            if (_row.Table.Columns.Contains("status"))
            {
                string status = _row["status"]?.ToString() ?? "Reported";
                if (cmbStatus.Items.Contains(status))
                    cmbStatus.SelectedItem = status;
            }

            txtApprovedBy.Text = _row["approved_by"]?.ToString();

            if (DateTime.TryParse(_row["approval_date"]?.ToString(), out var approval))
            {
                dtApprovalDate.Value = approval;
                dtApprovalDate.Checked = true;
            }
            else dtApprovalDate.Checked = false;

            if (DateTime.TryParse(_row["repair_date"]?.ToString(), out var repair))
            {
                dtRepairDate.Value = repair;
                dtRepairDate.Checked = true;
            }
            else dtRepairDate.Checked = false;

            txtActualRepairCost.Text = _row["actual_repair_cost"]?.ToString();
        }

        private void ApplyRbac()
        {
            bool isAdmin = string.Equals(CurrentUser.Role, "Admin", StringComparison.OrdinalIgnoreCase);
            if (!isAdmin)
            {
                // Agents can file reports but cannot approve or change status
                cmbStatus.Enabled = false;
                txtApprovedBy.ReadOnly = true;
                dtApprovalDate.Enabled = false;
                dtRepairDate.Enabled = false;
                txtActualRepairCost.ReadOnly = true;
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateInput())
                return;

            try
            {
                if (_isEdit)
                {
                    SaveEdit();
                }
                else
                {
                    SaveCreate();
                }

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving damage report: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveCreate()
        {
            int rentalId = (int)cmbRental.SelectedValue;
            string description = txtDescription.Text.Trim();
            string location = txtLocation.Text.Trim();
            decimal estimatedCost = ParseDecimal(txtEstimatedCost.Text.Trim());

            var parameters = new MySqlParameter[]
            {
                new MySqlParameter("@p_rental_id", rentalId),
                new MySqlParameter("@p_reported_by", CurrentUser.UserId),
                new MySqlParameter("@p_damage_description", description),
                new MySqlParameter("@p_damage_location", location),
                new MySqlParameter("@p_estimated_cost", estimatedCost)
            };

            var dt = DatabaseHelper.ExecuteStoredProcedure("sp_CreateDamageReport", parameters);
            int damageId = dt.Rows.Count > 0 && dt.Columns.Contains("damage_id") ? Convert.ToInt32(dt.Rows[0]["damage_id"]) : 0;
            SavePhotos(damageId);
        }

        private void SaveEdit()
        {
            int damageId = Convert.ToInt32(_row["damage_id"]);
            string description = txtDescription.Text.Trim();
            string location = txtLocation.Text.Trim();
            decimal estimatedCost = ParseDecimal(txtEstimatedCost.Text.Trim());
            string status = cmbStatus.Text;

            object approvedBy = ParseNullableInt(txtApprovedBy.Text.Trim());
            object approvalDate = dtApprovalDate.Checked ? dtApprovalDate.Value : DBNull.Value;
            object repairDate = dtRepairDate.Checked ? dtRepairDate.Value : DBNull.Value;
            object actualRepairCost = ParseNullableDecimal(txtActualRepairCost.Text.Trim());

            var parameters = new MySqlParameter[]
            {
                new MySqlParameter("@p_damage_id", damageId),
                new MySqlParameter("@p_damage_description", description),
                new MySqlParameter("@p_damage_location", location),
                new MySqlParameter("@p_estimated_cost", estimatedCost),
                new MySqlParameter("@p_status", status),
                new MySqlParameter("@p_approved_by", approvedBy ?? (object)DBNull.Value),
                new MySqlParameter("@p_approval_date", approvalDate),
                new MySqlParameter("@p_repair_date", repairDate),
                new MySqlParameter("@p_actual_repair_cost", actualRepairCost ?? (object)DBNull.Value)
            };

            DatabaseHelper.ExecuteStoredProcedure("sp_UpdateDamageReport", parameters);
            SavePhotos(damageId);
        }

        private bool ValidateInput()
        {
            if (!_isEdit && cmbRental.SelectedItem == null)
            {
                MessageBox.Show("Please select a rental.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtDescription.Text))
            {
                MessageBox.Show("Damage description is required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!decimal.TryParse(txtEstimatedCost.Text.Trim(), out _))
            {
                MessageBox.Show("Estimated cost must be a number.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (_isEdit && string.IsNullOrWhiteSpace(cmbStatus.Text))
            {
                MessageBox.Show("Status is required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private static decimal ParseDecimal(string input)
        {
            return decimal.TryParse(input, out var val) ? val : 0m;
        }

        private static decimal? ParseNullableDecimal(string input)
        {
            return decimal.TryParse(input, out var val) ? val : (decimal?)null;
        }

        private static int? ParseNullableInt(string input)
        {
            return int.TryParse(input, out var val) ? val : (int?)null;
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

        private void SavePhotos(int damageId)
        {
            if (damageId <= 0 || _pendingPhotos.Count == 0) return;
            foreach (var photo in _pendingPhotos)
            {
                DatabaseHelper.ExecuteStoredProcedure(
                    "sp_AddDamageReportPhoto",
                    new MySqlParameter("@p_damage_id", damageId),
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
