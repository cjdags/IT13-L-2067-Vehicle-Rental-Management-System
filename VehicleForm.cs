using System;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.IO;
using System.Drawing;

namespace VehicleRentalSystem
{
    public partial class VehicleForm : Form
    {
        private int? vehicleId;
        private ComboBox cmbVehicleCategory;
        private TextBox txtMake;
        private TextBox txtModel;
        private NumericUpDown numYear;
        private TextBox txtLicensePlate;
        private TextBox txtVIN;
        private TextBox txtColor;
        private NumericUpDown numDailyRate;
        private ComboBox cmbStatus;
        private NumericUpDown numMileage;
        private ComboBox cmbFuelType;
        private ComboBox cmbTransmission;
        private NumericUpDown numSeatingCapacity;
        private Button btnSave;
        private Button btnCancel;
        private Button btnUploadImage;
        private ListBox lstImages;
        private PictureBox picPrimary;
        private Label lblImageInfo;
        private readonly List<VehicleImagePayload> pendingImages = new();
        private Button btnViewGallery;

        private record VehicleImagePayload(byte[] Data, string ContentType, string Caption, bool IsPrimary);

        public VehicleForm(int? vehicleId = null)
        {
            this.vehicleId = vehicleId;
            InitializeComponent();
            ThemeHelper.ApplyTheme(this);
            ThemeHelper.ApplyCardDialogLayout(
                this,
                title: vehicleId.HasValue ? "Edit Vehicle" : "Add New Vehicle",
                footerRightButtons: new[] { btnSave, btnCancel }
            );
            LoadVehicleCategories();
            if (vehicleId.HasValue)
            {
                LoadVehicle(vehicleId.Value);
                LoadVehicleImages(vehicleId.Value);
            }
        }

        private void InitializeComponent()
        {
            this.cmbVehicleCategory = new ComboBox();
            this.txtMake = new TextBox();
            this.txtModel = new TextBox();
            this.numYear = new NumericUpDown();
            this.txtLicensePlate = new TextBox();
            this.txtVIN = new TextBox();
            this.txtColor = new TextBox();
            this.numDailyRate = new NumericUpDown();
            this.cmbStatus = new ComboBox();
            this.numMileage = new NumericUpDown();
            this.cmbFuelType = new ComboBox();
            this.cmbTransmission = new ComboBox();
            this.numSeatingCapacity = new NumericUpDown();
            this.btnSave = new Button();
            this.btnCancel = new Button();
            this.btnUploadImage = new Button();
            this.lstImages = new ListBox();
            this.picPrimary = new PictureBox();
            this.lblImageInfo = new Label();
            this.btnViewGallery = new Button();

            this.SuspendLayout();

            // Configure controls
            cmbVehicleCategory.DropDownStyle = ComboBoxStyle.DropDownList;

            cmbStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbStatus.Items.AddRange(new string[] {  "OutOfService", "Retired","Available"});
            cmbStatus.SelectedIndex = 0;

            cmbFuelType.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbFuelType.Items.AddRange(new string[] { "Gasoline", "Diesel", "Electric", "Hybrid" });
            cmbFuelType.SelectedIndex = 0;

            cmbTransmission.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbTransmission.Items.AddRange(new string[] { "Manual", "Automatic" });
            cmbTransmission.SelectedIndex = 1;

            numYear.Minimum = 1900;
            numYear.Maximum = 2100;
            numYear.Value = DateTime.Now.Year;

            numDailyRate.DecimalPlaces = 2;
            numDailyRate.Minimum = 0;
            numDailyRate.Maximum = 10000;

            numMileage.Minimum = 0;
            numMileage.Maximum = 1000000;

            numSeatingCapacity.Minimum = 2;
            numSeatingCapacity.Maximum = 50;
            numSeatingCapacity.Value = 5;

            // Two-column responsive grid layout (fits without clipping on different DPI/window sizes)
            Panel Field(string label, Control input)
            {
                var p = new Panel { Height = 72, Margin = new Padding(0, 0, 24, 12), Dock = DockStyle.Fill };
                var lbl = new Label
                {
                    Text = label,
                    AutoSize = false,
                    Height = 18,
                    Dock = DockStyle.Top,
                    Font = ThemeHelper.NormalFont,
                    ForeColor = ThemeHelper.TextColor
                };

                // Make textboxes/combos a consistent height
                if (input is TextBox tb)
                {
                    tb.Multiline = true;
                    tb.Height = 36;
                }
                else if (input is ComboBox cb)
                {
                    cb.Height = 36;
                }
                else if (input is NumericUpDown nud)
                {
                    nud.Height = 36;
                }

                input.Dock = DockStyle.Top;
                p.Controls.Add(input);
                p.Controls.Add(lbl);
                return p;
            }

            var grid = new TableLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 2,
                RowCount = 0,
                Dock = DockStyle.Top,
                Margin = new Padding(0, 0, 0, 24),
                Padding = new Padding(0)
            };
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

            void AddRow(Control left, Control right)
            {
                int r = grid.RowCount++;
                grid.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                if (left != null) grid.Controls.Add(left, 0, r);
                if (right != null) grid.Controls.Add(right, 1, r);
            }

            AddRow(Field("Vehicle Category", cmbVehicleCategory), Field("Status", cmbStatus));
            AddRow(Field("Make", txtMake), Field("Model", txtModel));
            AddRow(Field("Year", numYear), Field("Color", txtColor));
            AddRow(Field("License Plate", txtLicensePlate), Field("VIN", txtVIN));
            AddRow(Field("Daily Rate", numDailyRate), Field("Mileage", numMileage));
            AddRow(Field("Fuel Type", cmbFuelType), Field("Transmission", cmbTransmission));
            AddRow(Field("Seating Capacity", numSeatingCapacity), new Panel { Height = 72, Margin = new Padding(0, 0, 0, 12), Dock = DockStyle.Fill });

            // Images upload area spans full width - row layout with preview + list + buttons
            var imagesPanel = new Panel
            {
                Height = 320,
                Margin = new Padding(0, 0, 0, 12),
                Dock = DockStyle.Fill,
                Padding = new Padding(8)
            };
            var imagesLbl = new Label
            {
                Text = "Vehicle Images",
                AutoSize = false,
                Height = 32,
                Dock = DockStyle.Top,
                Font = ThemeHelper.LabelFont,
                ForeColor = ThemeHelper.TextColor,
                Padding = new Padding(0, 0, 0, 4)
            };

            var imageLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                AutoSize = false
            };
            imageLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 240F));
            imageLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            // Left: primary preview with caption
            var previewPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(0, 0, 12, 0) };
            var previewLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2
            };
            previewLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            previewLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            lblImageInfo.AutoSize = false;
            lblImageInfo.Dock = DockStyle.Fill;
            lblImageInfo.Height = 28;
            lblImageInfo.TextAlign = ContentAlignment.MiddleLeft;
            lblImageInfo.Font = ThemeHelper.NormalFont;
            lblImageInfo.ForeColor = ThemeHelper.TextColor;
            lblImageInfo.Padding = new Padding(4, 0, 0, 6);
            lblImageInfo.Text = "No images";
            lblImageInfo.Visible = true;

            picPrimary.Dock = DockStyle.Fill;
            picPrimary.Height = 220;
            picPrimary.SizeMode = PictureBoxSizeMode.Zoom;
            picPrimary.BorderStyle = BorderStyle.FixedSingle;

            previewLayout.Controls.Add(lblImageInfo, 0, 0);
            previewLayout.Controls.Add(picPrimary, 0, 1);
            previewPanel.Controls.Add(previewLayout);

            // Right: list + buttons
            var rightPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2
            };
            rightPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            rightPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            lstImages.Dock = DockStyle.Fill;
            lstImages.Visible = true;
            lstImages.BorderStyle = BorderStyle.FixedSingle;

            var buttonsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
                WrapContents = false,
                Padding = new Padding(0, 8, 0, 0)
            };

            btnUploadImage.Text = "Upload Image";
            btnUploadImage.AutoSize = true;
            btnUploadImage.Height = 32;
            btnUploadImage.Click += BtnUploadImage_Click;

            btnViewGallery.Text = "View Gallery";
            btnViewGallery.AutoSize = true;
            btnViewGallery.Height = 32;
            btnViewGallery.Click += BtnViewGallery_Click;
            btnViewGallery.Visible = true;

            buttonsPanel.Controls.Add(btnUploadImage);
            buttonsPanel.Controls.Add(btnViewGallery);

            rightPanel.Controls.Add(lstImages, 0, 0);
            rightPanel.Controls.Add(buttonsPanel, 0, 1);

            imageLayout.Controls.Add(previewPanel, 0, 0);
            imageLayout.Controls.Add(rightPanel, 1, 0);

            imagesPanel.Controls.Add(imageLayout);
            imagesPanel.Controls.Add(imagesLbl);
            grid.RowCount++;
            grid.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            grid.Controls.Add(imagesPanel, 0, grid.RowCount - 1);
            grid.SetColumnSpan(imagesPanel, 2);

            this.Controls.Add(grid);

            // Footer buttons are placed by ThemeHelper.ApplyCardDialogLayout; keep them instantiated only
            btnSave.Text = "Save";
            btnSave.Click += BtnSave_Click;
            btnCancel.Text = "Cancel";
            btnCancel.Click += BtnCancel_Click;

            this.ClientSize = new System.Drawing.Size(960, 560);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Text = vehicleId.HasValue ? "Edit Vehicle" : "Add New Vehicle";
            this.StartPosition = FormStartPosition.CenterScreen;

            this.ResumeLayout(false);
        }

        private void LoadVehicleCategories()
        {
            try
            {
                DataTable dt = DatabaseHelper.ExecuteStoredProcedure("sp_GetAllVehicleCategories");
                cmbVehicleCategory.DataSource = dt;
                cmbVehicleCategory.DisplayMember = "category_name";
                cmbVehicleCategory.ValueMember = "category_id";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading vehicle categories: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadVehicle(int id)
        {
            try
            {
                var parameters = new MySqlParameter[]
                {
                    new MySqlParameter("@p_vehicle_id", id)
                };
                DataTable dt = DatabaseHelper.ExecuteStoredProcedure("sp_GetVehicle", parameters);
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    if (row["category_id"] != DBNull.Value)
                        cmbVehicleCategory.SelectedValue = Convert.ToInt32(row["category_id"]);
                    txtMake.Text = row["make"].ToString();
                    txtModel.Text = row["model"].ToString();
                    numYear.Value = Convert.ToInt32(row["year"]);
                    txtLicensePlate.Text = row["license_plate"].ToString();
                    txtVIN.Text = row["vin"]?.ToString() ?? "";
                    txtColor.Text = row["color"]?.ToString() ?? "";
                    numDailyRate.Value = Convert.ToDecimal(row["daily_rate"]);
                    cmbStatus.Text = row["status"].ToString();
                    numMileage.Value = Convert.ToInt32(row["mileage"]);
                    if (row["fuel_type"] != DBNull.Value)
                        cmbFuelType.Text = row["fuel_type"].ToString();
                    if (row["transmission"] != DBNull.Value)
                        cmbTransmission.Text = row["transmission"].ToString();
                    if (row["seating_capacity"] != DBNull.Value)
                        numSeatingCapacity.Value = Convert.ToInt32(row["seating_capacity"]);

                    LoadVehicleImages(id);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading vehicle: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadVehicleImages(int id)
        {
            try
            {
                var dt = DatabaseHelper.ExecuteStoredProcedure("sp_GetVehicleImages", new MySqlParameter("@p_vehicle_id", id));
                lstImages.Items.Clear();
                picPrimary.Image = null;
                lblImageInfo.Text = "No images";

                foreach (DataRow row in dt.Rows)
                {
                    var caption = row["caption"]?.ToString() ?? "Image";
                    var isPrimary = row["is_primary"] != DBNull.Value && Convert.ToBoolean(row["is_primary"]);
                    lstImages.Items.Add($"{caption}{(isPrimary ? " (primary)" : "")}");

                    if (isPrimary && row["image_data"] is byte[] bytes)
                    {
                        using var ms = new MemoryStream(bytes);
                        picPrimary.Image = Image.FromStream(ms);
                        lblImageInfo.Text = $"Primary: {caption}";
                    }
                }

                // If no primary found, show first image
                if (picPrimary.Image == null && dt.Rows.Count > 0 && dt.Rows[0]["image_data"] is byte[] first)
                {
                    using var ms = new MemoryStream(first);
                    picPrimary.Image = Image.FromStream(ms);
                    lblImageInfo.Text = $"Primary: {dt.Rows[0]["caption"]}";
                }
            }
            catch
            {
                // ignore preview errors
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (ValidateInput())
            {
                try
                {
                    var parameters = new MySqlParameter[]
                    {
                        new MySqlParameter("@p_vehicle_id", vehicleId ?? 0),
                        new MySqlParameter("@p_category_id", cmbVehicleCategory.SelectedValue),
                        new MySqlParameter("@p_make", txtMake.Text),
                        new MySqlParameter("@p_model", txtModel.Text),
                        new MySqlParameter("@p_year", (int)numYear.Value),
                        new MySqlParameter("@p_color", txtColor.Text),
                        new MySqlParameter("@p_license_plate", txtLicensePlate.Text),
                        new MySqlParameter("@p_vin", txtVIN.Text),
                        new MySqlParameter("@p_mileage", (int)numMileage.Value),
                        new MySqlParameter("@p_fuel_type", cmbFuelType.Text),
                        new MySqlParameter("@p_transmission", cmbTransmission.Text),
                        new MySqlParameter("@p_seating_capacity", (int)numSeatingCapacity.Value),
                        new MySqlParameter("@p_status", cmbStatus.Text),
                        new MySqlParameter("@p_daily_rate", numDailyRate.Value)
                    };

                    int savedVehicleId;
                    if (vehicleId.HasValue)
                    {
                        DatabaseHelper.ExecuteStoredProcedure("sp_UpdateVehicle", parameters);
                        savedVehicleId = vehicleId.Value;
                    }
                    else
                    {
                        var dt = DatabaseHelper.ExecuteStoredProcedure("sp_CreateVehicle", parameters);
                        savedVehicleId = dt.Rows.Count > 0 ? Convert.ToInt32(dt.Rows[0]["vehicle_id"]) : 0;
                        vehicleId = savedVehicleId;
                    }

                    SavePendingImages(savedVehicleId);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving vehicle: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnViewGallery_Click(object? sender, EventArgs e)
        {
            if (vehicleId.HasValue)
            {
                using var gal = new ImageGalleryForm(vehicleId.Value);
                gal.ShowDialog();
            }
            else
            {
                MessageBox.Show("Save the vehicle first to view gallery.", "Gallery", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private bool ValidateInput()
        {
            if (cmbVehicleCategory.SelectedValue == null)
            {
                MessageBox.Show("Please select a vehicle category.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtMake.Text))
            {
                MessageBox.Show("Make is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtModel.Text))
            {
                MessageBox.Show("Model is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtLicensePlate.Text))
            {
                MessageBox.Show("License plate is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BtnUploadImage_Click(object? sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog();
            ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp";
            ofd.Multiselect = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                bool first = pendingImages.Count == 0;
                foreach (var file in ofd.FileNames)
                {
                    try
                    {
                        byte[] data = File.ReadAllBytes(file);
                        string contentType = GetContentType(file);
                        pendingImages.Add(new VehicleImagePayload(data, contentType, Path.GetFileName(file), first));
                        lstImages.Items.Add($"{Path.GetFileName(file)}{(first ? " (primary)" : "")}");
                        first = false;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed to load image {file}: {ex.Message}", "Image Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void SavePendingImages(int savedVehicleId)
        {
            if (savedVehicleId <= 0 || pendingImages.Count == 0) return;
            bool primarySet = false;
            foreach (var img in pendingImages)
            {
                var parms = new MySqlParameter[]
                {
                    new MySqlParameter("@p_vehicle_id", savedVehicleId),
                    new MySqlParameter("@p_image_data", img.Data),
                    new MySqlParameter("@p_content_type", img.ContentType),
                    new MySqlParameter("@p_caption", img.Caption),
                    new MySqlParameter("@p_is_primary", !primarySet && img.IsPrimary ? true : primarySet ? false : img.IsPrimary)
                };
                DatabaseHelper.ExecuteStoredProcedure("sp_CreateVehicleImage", parms);
                if (!primarySet && img.IsPrimary)
                    primarySet = true;
            }
            pendingImages.Clear();
        }

        private string GetContentType(string filePath)
        {
            string ext = Path.GetExtension(filePath).ToLowerInvariant();
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
