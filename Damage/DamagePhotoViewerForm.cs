using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace VehicleRentalSystem
{
    public class DamagePhotoViewerForm : Form
    {
        private readonly int damageId;
        private FlowLayoutPanel flow;

        public DamagePhotoViewerForm(int damageId)
        {
            this.damageId = damageId;
            InitializeComponent();
            ThemeHelper.ApplyBaseTheme(this);
            LoadPhotos();
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
            Text = "Damage Photos";
            StartPosition = FormStartPosition.CenterParent;
        }

        private void LoadPhotos()
        {
            try
            {
                var dt = DatabaseHelper.ExecuteStoredProcedure("sp_GetDamageReportPhotos",
                    new MySqlParameter("@p_damage_id", damageId));
                flow.Controls.Clear();
                foreach (DataRow row in dt.Rows)
                {
                    if (row["photo_data"] is not byte[] bytes) continue;
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
                    string caption = row["caption"]?.ToString() ?? "Photo";
                    var lbl = new Label
                    {
                        Text = caption,
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
                        Text = "No photos found.",
                        AutoSize = true,
                        Padding = new Padding(8)
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading photos: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
