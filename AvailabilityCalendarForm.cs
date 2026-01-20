// (duplicate removed)
using System;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace VehicleRentalSystem
{
    public partial class AvailabilityCalendarForm : Form
    {
        private DateTimePicker dtStart;
        private DateTimePicker dtEnd;
        private Button btnLoad;
        private DataGridView dgvEvents;

        public AvailabilityCalendarForm()
        {
            InitializeComponent();
            ThemeHelper.ApplyTheme(this);
            ThemeHelper.ApplyCardDialogLayout(
                this,
                title: "Availability Calendar",
                footerRightButtons: new[] { btnLoad }
            );
            LoadEvents();
        }

        private void InitializeComponent()
        {
            dtStart = new DateTimePicker();
            dtEnd = new DateTimePicker();
            btnLoad = new Button();
            dgvEvents = new DataGridView();

            SuspendLayout();

            dtStart.Format = DateTimePickerFormat.Short;
            dtEnd.Format = DateTimePickerFormat.Short;
            dtStart.Value = DateTime.Today;
            dtEnd.Value = DateTime.Today.AddDays(14);

            btnLoad.Text = "Load";
            btnLoad.Click += (_, __) => LoadEvents();

            var filterPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 50,
                FlowDirection = FlowDirection.LeftToRight,
                Padding = new Padding(12),
                AutoSize = false
            };
            filterPanel.Controls.Add(new Label { Text = "From:", AutoSize = true, TextAlign = System.Drawing.ContentAlignment.MiddleLeft, Padding = new Padding(0, 8, 4, 0) });
            filterPanel.Controls.Add(dtStart);
            filterPanel.Controls.Add(new Label { Text = "To:", AutoSize = true, TextAlign = System.Drawing.ContentAlignment.MiddleLeft, Padding = new Padding(8, 8, 4, 0) });
            filterPanel.Controls.Add(dtEnd);
            filterPanel.Controls.Add(btnLoad);

            dgvEvents.Dock = DockStyle.Fill;
            dgvEvents.ReadOnly = true;
            dgvEvents.AllowUserToAddRows = false;
            dgvEvents.AllowUserToDeleteRows = false;
            dgvEvents.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            Controls.Add(dgvEvents);
            Controls.Add(filterPanel);

            ClientSize = new System.Drawing.Size(960, 640);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            Text = "Availability Calendar";
            StartPosition = FormStartPosition.CenterParent;

            ResumeLayout(false);
        }

        private void LoadEvents()
        {
            if (dtEnd.Value < dtStart.Value)
                dtEnd.Value = dtStart.Value.AddDays(1);

            try
            {
                var dt = DatabaseHelper.ExecuteStoredProcedure(
                    "sp_GetAvailabilityCalendar",
                    new MySqlParameter("@p_start", dtStart.Value),
                    new MySqlParameter("@p_end", dtEnd.Value)
                );
                dgvEvents.DataSource = dt;
                foreach (DataGridViewColumn col in dgvEvents.Columns)
                {
                    col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading calendar: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
