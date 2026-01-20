using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace VehicleRentalSystem
{
    public partial class InspectionForm : Form
    {
        private readonly int rentalId;
        private readonly string inspectionType; // "Pickup" or "Return"
        private NumericUpDown numOdometer;
        private NumericUpDown numFuel;
        private NumericUpDown numCleanliness;
        private TextBox txtNotes;
        private CheckedListBox chkItems;
        private Button btnAddPhoto;
        private ListBox lstPhotos;
        private Button btnSave;
        private Button btnCancel;
        private readonly List<PhotoPayload> pendingPhotos = new();

        private record PhotoPayload(byte[] Data, string ContentType, string Caption);

        public InspectionForm(int rentalId, string inspectionType)
        {
            this.rentalId = rentalId;
            this.inspectionType = inspectionType;
            InitializeComponent();
            ThemeHelper.ApplyTheme(this);
            ThemeHelper.ApplyCardDialogLayout(
                this,
                title: $"{inspectionType} Inspection",
                footerRightButtons: new[] { btnSave, btnCancel },
                footerLeftButtons: new[] { btnAddPhoto }
            );
        }

        private void InitializeComponent()
        {
            numOdometer = new NumericUpDown();
            numFuel = new NumericUpDown();
            numCleanliness = new NumericUpDown();
            txtNotes = new TextBox();
            chkItems = new CheckedListBox();
            btnAddPhoto = new Button();
            lstPhotos = new ListBox();
            btnSave = new Button();
            btnCancel = new Button();

            SuspendLayout();

            numOdometer.Maximum = 2000000;
            numFuel.Maximum = 100;
            numFuel.Value = 100;
            numCleanliness.Maximum = 10;
            numCleanliness.Value = 8;

            txtNotes.Multiline = true;
            txtNotes.Height = 80;
            txtNotes.ScrollBars = ScrollBars.Vertical;

            chkItems.Items.AddRange(new object[]
            {
                "Exterior OK",
                "Interior Clean",
                "Tires OK",
                "Lights OK",
                "Fluids OK",
                "Accessories Present"
            });
            chkItems.Height = 120;

            btnAddPhoto.Text = "Add Photo";
            btnAddPhoto.Click += BtnAddPhoto_Click;

            btnSave.Text = "Save";
            btnSave.Click += BtnSave_Click;

            btnCancel.Text = "Cancel";
            btnCancel.Click += (_, __) => Close();

            Panel Field(string label, Control input)
            {
                var p = new Panel { Height = 72, Margin = new Padding(0, 0, 24, 12), Dock = DockStyle.Fill };
                var lbl = new Label { Text = label, Dock = DockStyle.Top, Height = 18, Font = ThemeHelper.NormalFont, ForeColor = ThemeHelper.TextColor };
                if (input is NumericUpDown or ComboBox or DateTimePicker)
                    input.Height = 36;
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
                Margin = new Padding(0),
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

            AddRow(Field("Odometer", numOdometer), Field("Fuel Level (%)", numFuel));
            AddRow(Field("Cleanliness (1-10)", numCleanliness), Field("Notes", txtNotes));

            var checklistPanel = new Panel { Height = 160, Margin = new Padding(0, 0, 0, 12), Dock = DockStyle.Fill };
            var checklistLbl = new Label { Text = "Checklist", Dock = DockStyle.Top, Height = 18, Font = ThemeHelper.NormalFont, ForeColor = ThemeHelper.TextColor };
            chkItems.Dock = DockStyle.Fill;
            checklistPanel.Controls.Add(chkItems);
            checklistPanel.Controls.Add(checklistLbl);
            grid.RowCount++;
            grid.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            grid.Controls.Add(checklistPanel, 0, grid.RowCount - 1);
            grid.SetColumnSpan(checklistPanel, 2);

            var photosPanel = new Panel { Height = 140, Margin = new Padding(0, 0, 0, 12), Dock = DockStyle.Fill };
            var photosLbl = new Label { Text = "Photos", Dock = DockStyle.Top, Height = 18, Font = ThemeHelper.NormalFont, ForeColor = ThemeHelper.TextColor };
            lstPhotos.Dock = DockStyle.Fill;
            photosPanel.Controls.Add(lstPhotos);
            photosPanel.Controls.Add(photosLbl);
            grid.RowCount++;
            grid.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            grid.Controls.Add(photosPanel, 0, grid.RowCount - 1);
            grid.SetColumnSpan(photosPanel, 2);

            Controls.Add(grid);

            ClientSize = new System.Drawing.Size(840, 620);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            Text = $"{inspectionType} Inspection";
            StartPosition = FormStartPosition.CenterParent;

            ResumeLayout(false);
        }

        private void BtnAddPhoto_Click(object? sender, EventArgs e)
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
                        byte[] data = File.ReadAllBytes(file);
                        string contentType = GetContentType(file);
                        pendingPhotos.Add(new PhotoPayload(data, contentType, Path.GetFileName(file)));
                        lstPhotos.Items.Add(Path.GetFileName(file));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed to load photo {file}: {ex.Message}", "Photo Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private string GetContentType(string file)
        {
            string ext = Path.GetExtension(file).ToLowerInvariant();
            return ext switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                _ => "application/octet-stream"
            };
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            try
            {
                var inspDt = DatabaseHelper.ExecuteStoredProcedure(
                    "sp_CreateRentalInspection",
                    new MySqlParameter("@p_rental_id", rentalId),
                    new MySqlParameter("@p_inspection_type", inspectionType),
                    new MySqlParameter("@p_inspected_by", CurrentUser.UserId),
                    new MySqlParameter("@p_odometer", numOdometer.Value),
                    new MySqlParameter("@p_fuel_level_percent", (int)numFuel.Value),
                    new MySqlParameter("@p_cleanliness_rating", (int)numCleanliness.Value),
                    new MySqlParameter("@p_notes", txtNotes.Text)
                );
                int inspectionId = inspDt.Rows.Count > 0 ? Convert.ToInt32(inspDt.Rows[0]["inspection_id"]) : 0;

                foreach (var item in chkItems.CheckedItems)
                {
                    DatabaseHelper.ExecuteStoredProcedure(
                        "sp_CreateRentalInspectionItem",
                        new MySqlParameter("@p_inspection_id", inspectionId),
                        new MySqlParameter("@p_item_label", item.ToString()),
                        new MySqlParameter("@p_item_status", "OK"),
                        new MySqlParameter("@p_notes", "")
                    );
                }

                foreach (var photo in pendingPhotos)
                {
                    DatabaseHelper.ExecuteStoredProcedure(
                        "sp_CreateRentalInspectionPhoto",
                        new MySqlParameter("@p_inspection_id", inspectionId),
                        new MySqlParameter("@p_photo_data", photo.Data),
                        new MySqlParameter("@p_content_type", photo.ContentType),
                        new MySqlParameter("@p_caption", photo.Caption)
                    );
                }

                MessageBox.Show("Inspection saved.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving inspection: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
