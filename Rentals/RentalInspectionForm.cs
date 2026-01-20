using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace VehicleRentalSystem
{
    public class RentalInspectionForm : Form
    {
        private readonly int rentalId;
        private readonly string inspectionType; // Pickup or Return
        private NumericUpDown numOdometer;
        private NumericUpDown numFuel;
        private NumericUpDown numClean;
        private TextBox txtNotes;
        private CheckedListBox chkItems;
        private Button btnAddPhoto;
        private ListBox lstPhotos;
        private Button btnSave;
        private List<PhotoPayload> pendingPhotos = new();

        private record PhotoPayload(byte[] Data, string ContentType, string Caption);

        public RentalInspectionForm(int rentalId, string inspectionType)
        {
            this.rentalId = rentalId;
            this.inspectionType = inspectionType;
            InitializeComponent();
            ThemeHelper.ApplyBaseTheme(this);
            ThemeHelper.ApplyCardDialogLayout(
                this,
                title: $"{inspectionType} Inspection",
                footerRightButtons: new[] { btnSave }
            );
        }

        private void InitializeComponent()
        {
            numOdometer = new NumericUpDown { Maximum = 1000000, Minimum = 0, Height = 36 };
            numFuel = new NumericUpDown { Maximum = 100, Minimum = 0, Height = 36 };
            numClean = new NumericUpDown { Maximum = 10, Minimum = 1, Height = 36, Value = 5 };
            txtNotes = new TextBox { Multiline = true, Height = 80, ScrollBars = ScrollBars.Vertical };
            chkItems = new CheckedListBox { Height = 120 };
            btnAddPhoto = new Button { Text = "Add Photo" };
            lstPhotos = new ListBox { Height = 80 };
            btnSave = new Button { Text = "Save" };
            btnSave.Click += BtnSave_Click;
            btnAddPhoto.Click += BtnAddPhoto_Click;

            string[] defaultItems =
            {
                "Exterior condition",
                "Interior condition",
                "Tires",
                "Lights",
                "Fuel cap",
                "Documents present",
                "Accessories present"
            };
            chkItems.Items.AddRange(defaultItems);

            Panel Field(string label, Control input)
            {
                var p = new Panel { Height = 72, Margin = new Padding(0, 0, 24, 12), Dock = DockStyle.Fill };
                var lbl = new Label
                {
                    Text = label,
                    Dock = DockStyle.Top,
                    Height = 18,
                    Font = ThemeHelper.NormalFont,
                    ForeColor = ThemeHelper.TextColor
                };
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
                Dock = DockStyle.Top,
                Padding = new Padding(0),
                Margin = new Padding(0)
            };
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

            grid.Controls.Add(Field("Odometer", numOdometer), 0, 0);
            grid.Controls.Add(Field("Fuel Level (%)", numFuel), 1, 0);
            grid.Controls.Add(Field("Cleanliness (1-10)", numClean), 0, 1);

            var notesPanel = Field("Notes", txtNotes);
            grid.Controls.Add(notesPanel, 0, 2);
            grid.SetColumnSpan(notesPanel, 2);

            var itemsPanel = new Panel { Height = 160, Margin = new Padding(0, 0, 0, 12), Dock = DockStyle.Fill };
            itemsPanel.Controls.Add(chkItems);
            var itemsLbl = new Label { Text = "Checklist", Dock = DockStyle.Top, Height = 18, Font = ThemeHelper.NormalFont, ForeColor = ThemeHelper.TextColor };
            chkItems.Dock = DockStyle.Fill;
            itemsPanel.Controls.Add(itemsLbl);
            grid.Controls.Add(itemsPanel, 0, 3);
            grid.SetColumnSpan(itemsPanel, 2);

            var photoPanel = new Panel { Height = 160, Margin = new Padding(0, 0, 0, 12), Dock = DockStyle.Fill };
            var photosLbl = new Label { Text = "Photos", Dock = DockStyle.Top, Height = 18, Font = ThemeHelper.NormalFont, ForeColor = ThemeHelper.TextColor };
            btnAddPhoto.Dock = DockStyle.Top;
            lstPhotos.Dock = DockStyle.Fill;
            photoPanel.Controls.Add(lstPhotos);
            photoPanel.Controls.Add(btnAddPhoto);
            photoPanel.Controls.Add(photosLbl);
            grid.Controls.Add(photoPanel, 0, 4);
            grid.SetColumnSpan(photoPanel, 2);

            this.Controls.Add(grid);
            this.ClientSize = new System.Drawing.Size(720, 640);
            this.StartPosition = FormStartPosition.CenterParent;
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
                        MessageBox.Show($"Failed to load image {file}: {ex.Message}", "Image Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            try
            {
                var inspParams = new MySqlParameter[]
                {
                    new MySqlParameter("@p_rental_id", rentalId),
                    new MySqlParameter("@p_inspection_type", inspectionType),
                    new MySqlParameter("@p_inspected_by", CurrentUser.UserId),
                    new MySqlParameter("@p_odometer", (int)numOdometer.Value),
                    new MySqlParameter("@p_fuel_level_percent", (int)numFuel.Value),
                    new MySqlParameter("@p_cleanliness_rating", (int)numClean.Value),
                    new MySqlParameter("@p_notes", txtNotes.Text)
                };
                var inspDt = DatabaseHelper.ExecuteStoredProcedure("sp_CreateRentalInspection", inspParams);
                int inspectionId = inspDt.Rows.Count > 0 ? Convert.ToInt32(inspDt.Rows[0]["inspection_id"]) : 0;

                // Checklist items
                foreach (var item in chkItems.Items)
                {
                    bool checkedItem = chkItems.CheckedItems.Contains(item);
                    var itemParams = new MySqlParameter[]
                    {
                        new MySqlParameter("@p_inspection_id", inspectionId),
                        new MySqlParameter("@p_item_label", item.ToString()),
                        new MySqlParameter("@p_item_status", checkedItem ? "OK" : "Issue"),
                        new MySqlParameter("@p_notes", checkedItem ? "" : "Issue noted")
                    };
                    DatabaseHelper.ExecuteStoredProcedure("sp_CreateRentalInspectionItem", itemParams);
                }

                // Photos
                foreach (var photo in pendingPhotos)
                {
                    var photoParams = new MySqlParameter[]
                    {
                        new MySqlParameter("@p_inspection_id", inspectionId),
                        new MySqlParameter("@p_photo_data", photo.Data),
                        new MySqlParameter("@p_content_type", photo.ContentType),
                        new MySqlParameter("@p_caption", photo.Caption)
                    };
                    DatabaseHelper.ExecuteStoredProcedure("sp_CreateRentalInspectionPhoto", photoParams);
                }

                MessageBox.Show("Inspection saved.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving inspection: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
