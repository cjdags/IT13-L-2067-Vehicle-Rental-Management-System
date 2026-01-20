using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace VehicleRentalSystem
{
    public class ImageGalleryForm : Form
    {
        private readonly int vehicleId;
        private FlowLayoutPanel flow;

        public ImageGalleryForm(int vehicleId)
        {
            this.vehicleId = vehicleId;
            InitializeComponent();
            ThemeHelper.ApplyBaseTheme(this);
            LoadImages();
        }

        private void InitializeComponent()
        {
            flow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                WrapContents = true,
                Padding = new Padding(12)
            };
            Controls.Add(flow);
            ClientSize = new Size(800, 600);
            Text = "Vehicle Images";
            StartPosition = FormStartPosition.CenterParent;
        }

        private void LoadImages()
        {
            try
            {
                var dt = DatabaseHelper.ExecuteStoredProcedure("sp_GetVehicleImages",
                    new MySqlParameter("@p_vehicle_id", vehicleId));

                flow.Controls.Clear();
                foreach (DataRow row in dt.Rows)
                {
                    if (row["image_data"] is not byte[] bytes) continue;
                    var pb = new PictureBox
                    {
                        Width = 180,
                        Height = 140,
                        SizeMode = PictureBoxSizeMode.Zoom,
                        BorderStyle = BorderStyle.FixedSingle,
                        Margin = new Padding(8)
                    };
                    using var ms = new MemoryStream(bytes);
                    pb.Image = Image.FromStream(ms);
                    string caption = row["caption"]?.ToString() ?? "Image";
                    bool isPrimary = row["is_primary"] != DBNull.Value && Convert.ToBoolean(row["is_primary"]);
                    var lbl = new Label
                    {
                        Text = isPrimary ? $"{caption} (primary)" : caption,
                        AutoSize = false,
                        Height = 24,
                        Dock = DockStyle.Bottom,
                        TextAlign = ContentAlignment.MiddleCenter
                    };
                    var panel = new Panel { Width = 180, Height = 170 };
                    panel.Controls.Add(pb);
                    panel.Controls.Add(lbl);
                    flow.Controls.Add(panel);
                }

                if (dt.Rows.Count == 0)
                {
                    flow.Controls.Add(new Label
                    {
                        Text = "No images found.",
                        AutoSize = true,
                        Padding = new Padding(8)
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading images: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
