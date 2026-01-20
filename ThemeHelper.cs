using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace VehicleRentalSystem
{
    public static class ThemeHelper
    {
        // Color Palette - Light Theme (matching stitch design exactly)
        // Primary Colors
        public static Color PrimaryColor = Color.FromArgb(19, 127, 236);        // #137fec - Primary blue
        public static Color PrimaryHoverColor = Color.FromArgb(17, 115, 212);   // Primary hover
        public static Color PrimaryLightColor = Color.FromArgb(19, 127, 236);   // Primary with 20% opacity for backgrounds
        
        // Background Colors - Light Theme
        public static Color BackgroundColor = Color.FromArgb(246, 247, 248);    // #f6f7f8 - Light gray background
        public static Color BackgroundLightColor = Color.FromArgb(255, 255, 255); // White
        public static Color PanelColor = Color.FromArgb(255, 255, 255);         // White panels
        public static Color CardColor = Color.FromArgb(255, 255, 255);          // White cards
        public static Color SidebarColor = Color.FromArgb(255, 255, 255);       // White sidebar
        public static Color SidebarHoverColor = Color.FromArgb(19, 127, 236, 20); // Primary with 20% opacity
        // Use opaque colors for controls that don't support alpha BackColor
        public static Color SidebarHoverBackgroundColor = Color.FromArgb(248, 250, 252); // Light hover
        public static Color SidebarActiveBackgroundColor = Color.FromArgb(231, 243, 255); // Light blue active
        
        // Text Colors - Light Theme
        public static Color TextColor = Color.FromArgb(17, 20, 24);             // #111418 - Dark text on light
        public static Color TextPrimaryColor = Color.FromArgb(17, 20, 24);      // #111418 - Primary text
        public static Color TextSecondaryColor = Color.FromArgb(97, 117, 137);  // #617589 - Secondary text
        public static Color TextMutedColor = Color.FromArgb(150, 150, 150);     // Muted text
        public static Color TextOnPrimary = Color.FromArgb(255, 255, 255);      // White text on primary buttons
        
        // Border Colors
        public static Color BorderColor = Color.FromArgb(219, 224, 230);         // #dbe0e6 - Light border
        public static Color BorderDarkColor = Color.FromArgb(203, 213, 225);    // Slightly darker border
        public static Color BorderLightColor = Color.FromArgb(240, 242, 244);   // #f0f2f4 - Very light border
        
        // Button Colors
        public static Color ButtonColor = Color.FromArgb(255, 255, 255);        // White default button
        public static Color ButtonHoverColor = Color.FromArgb(248, 250, 252);    // Light gray hover
        public static Color ButtonActiveColor = PrimaryColor;                    // Primary button
        public static Color ButtonSecondaryColor = Color.FromArgb(241, 245, 249); // Gray secondary button
        
        // Input Colors
        public static Color InputBackground = Color.FromArgb(255, 255, 255);    // White input
        public static Color InputBackgroundDark = Color.FromArgb(255, 255, 255); // White (light theme)
        public static Color InputBorderColor = BorderColor;                      // Input border
        public static Color InputFocusColor = PrimaryColor;                      // Focus border
        public static Color InputTextColor = Color.FromArgb(17, 20, 24);        // Dark text in inputs
        
        // Table Colors
        public static Color TableHeaderColor = Color.FromArgb(246, 247, 248);   // #f6f7f8 - Light gray header
        public static Color TableRowColor = Color.FromArgb(255, 255, 255);      // White rows
        public static Color TableAlternateRowColor = Color.FromArgb(249, 250, 251); // Very light gray alternate
        public static Color TableBorderColor = BorderColor;                      // Table borders
        
        // Status Colors
        public static Color SuccessColor = Color.FromArgb(34, 197, 94);          // Green
        public static Color WarningColor = Color.FromArgb(251, 191, 36);         // Yellow
        public static Color ErrorColor = Color.FromArgb(239, 68, 68);           // Red
        public static Color InfoColor = PrimaryColor;                            // Info blue
        
        // Status Badge Colors
        public static Color BadgeAvailableColor = Color.FromArgb(34, 197, 94);   // Green for Available
        public static Color BadgeRentedColor = Color.FromArgb(251, 191, 36);    // Yellow for Rented
        public static Color BadgeMaintenanceColor = Color.FromArgb(239, 68, 68); // Red for Maintenance
        public static Color BadgeAdminColor = Color.FromArgb(19, 127, 236);     // Blue for Admin
        public static Color BadgeAgentColor = Color.FromArgb(107, 114, 128);    // Gray for Rental Agent

        // Modern Fonts - Inter style (fallback to Segoe UI) - Increased sizes for better visibility
        public static Font TitleFont = new Font("Segoe UI", 22F, FontStyle.Bold);
        public static Font SubtitleFont = new Font("Segoe UI", 18F, FontStyle.Bold);
        public static Font HeaderFont = new Font("Segoe UI", 12F, FontStyle.Bold);
        public static Font NormalFont = new Font("Segoe UI", 11F, FontStyle.Regular);
        public static Font SmallFont = new Font("Segoe UI", 10F, FontStyle.Regular);
        public static Font ButtonFont = new Font("Segoe UI", 11F, FontStyle.Bold);
        public static Font LabelFont = new Font("Segoe UI", 11F, FontStyle.Regular);

        // Spacing Constants
        public static int StandardPadding = 16;
        public static int LargePadding = 24;
        public static int SmallPadding = 8;
        public static int BorderRadius = 8;  // Rounded-lg equivalent
        public static int BorderRadiusLarge = 12; // Rounded-xl equivalent

        /// <summary>
        /// Applies the light theme to a form (matching stitch design)
        /// </summary>
        public static void ApplyTheme(Form form)
        {
            form.BackColor = BackgroundColor; // Light gray background
            form.ForeColor = TextColor; // Dark text
            form.Font = NormalFont;
            form.Padding = new Padding(StandardPadding);

            // Apply theme to all controls recursively
            ApplyThemeToControls(form.Controls);
        }

        /// <summary>
        /// Applies only the base theme to a form (no recursive control changes).
        /// Useful for forms that build their own layout/panels.
        /// </summary>
        public static void ApplyBaseTheme(Form form)
        {
            form.BackColor = BackgroundColor;
            form.ForeColor = TextColor;
            form.Font = NormalFont;
        }

        /// <summary>
        /// Applies the dark theme to a control and its children
        /// </summary>
        private static void ApplyThemeToControls(Control.ControlCollection controls)
        {
            foreach (Control control in controls)
            {
                if (control is Button button)
                {
                    ApplyButtonTheme(button);
                }
                else if (control is Label label)
                {
                    label.ForeColor = TextColor; // Dark text on light background
                    label.BackColor = Color.Transparent;
                    // Ensure labels have readable font size
                    if (label.Font.Size < 10F)
                    {
                        label.Font = LabelFont;
                    }
                }
                else if (control is TextBox textBox)
                {
                    textBox.BackColor = InputBackground; // White background
                    textBox.ForeColor = InputTextColor; // Dark text
                    textBox.BorderStyle = BorderStyle.FixedSingle;
                    textBox.Font = new Font("Segoe UI", 11F, FontStyle.Regular);
                }
                else if (control is ComboBox comboBox)
                {
                    comboBox.BackColor = InputBackground; // White background
                    comboBox.ForeColor = InputTextColor; // Dark text
                    comboBox.FlatStyle = FlatStyle.Flat;
                    comboBox.Font = new Font("Segoe UI", 11F, FontStyle.Regular);
                    comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
                }
                else if (control is DataGridView dataGridView)
                {
                    ApplyDataGridViewTheme(dataGridView);
                }
                else if (control is NumericUpDown numericUpDown)
                {
                    numericUpDown.BackColor = InputBackground; // White background
                    numericUpDown.ForeColor = InputTextColor; // Dark text
                    numericUpDown.Font = new Font("Segoe UI", 11F, FontStyle.Regular);
                    numericUpDown.BorderStyle = BorderStyle.FixedSingle;
                }
                else if (control is DateTimePicker dateTimePicker)
                {
                    dateTimePicker.BackColor = InputBackground; // White background
                    dateTimePicker.ForeColor = InputTextColor; // Dark text
                    dateTimePicker.CalendarForeColor = InputTextColor;
                    dateTimePicker.CalendarMonthBackground = InputBackground;
                    dateTimePicker.CalendarTitleBackColor = TableHeaderColor;
                    dateTimePicker.CalendarTitleForeColor = TextColor;
                    dateTimePicker.CalendarTrailingForeColor = TextSecondaryColor;
                    dateTimePicker.Font = new Font("Segoe UI", 11F, FontStyle.Regular);
                }
                else if (control is Panel panel)
                {
                    panel.BackColor = PanelColor; // White panels
                    panel.ForeColor = TextColor; // Dark text
                    panel.Padding = new Padding(StandardPadding);
                }
                else if (control is GroupBox groupBox)
                {
                    groupBox.BackColor = PanelColor;
                    groupBox.ForeColor = TextColor;
                }

                // Recursively apply to child controls
                if (control.HasChildren)
                {
                    ApplyThemeToControls(control.Controls);
                }
            }
        }

        /// <summary>
        /// Applies light theme to a button (matching stitch design)
        /// </summary>
        public static void ApplyButtonTheme(Button button, bool isPrimary = false, bool isSecondary = false)
        {
            if (isPrimary)
            {
                button.BackColor = PrimaryColor;
                button.ForeColor = TextOnPrimary; // White text on primary
                button.FlatAppearance.MouseOverBackColor = PrimaryHoverColor;
                button.FlatAppearance.MouseDownBackColor = Color.FromArgb(15, 105, 200);
            }
            else if (isSecondary)
            {
                button.BackColor = ButtonSecondaryColor;
                button.ForeColor = TextColor; // Dark text
                button.FlatAppearance.MouseOverBackColor = Color.FromArgb(226, 232, 240);
                button.FlatAppearance.MouseDownBackColor = Color.FromArgb(203, 213, 225);
            }
            else
            {
                button.BackColor = ButtonColor; // White
                button.ForeColor = TextColor; // Dark text
                button.FlatAppearance.MouseOverBackColor = ButtonHoverColor;
                button.FlatAppearance.MouseDownBackColor = Color.FromArgb(241, 245, 249);
            }

            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            button.Cursor = Cursors.Hand;
            button.Padding = new Padding(16, 10, 16, 10);
            button.UseVisualStyleBackColor = false;
        }

        /// <summary>
        /// Applies light theme to a DataGridView (matching stitch design)
        /// </summary>
        public static void ApplyDataGridViewTheme(DataGridView dgv)
        {
            EnableDoubleBuffering(dgv);

            dgv.BackgroundColor = CardColor; // White background
            dgv.GridColor = TableBorderColor;
            dgv.BorderStyle = BorderStyle.None; // border is handled by the surrounding "card" panel
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.None;
            dgv.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dgv.EnableHeadersVisualStyles = false;
            dgv.RowHeadersVisible = false;
            dgv.AllowUserToResizeRows = false;
            // Slightly taller rows for DPI scaling + our padding (prevents text overlap/"meshed" rendering)
            dgv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            dgv.RowTemplate.Height = 56;
            dgv.StandardTab = true;
            dgv.MultiSelect = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            
            // Enable horizontal scrolling and prevent column squishing
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dgv.AllowUserToResizeColumns = true;
            dgv.ScrollBars = ScrollBars.Both;
            dgv.AutoSize = false;

            // Header styling - Light gray header with dark text
            dgv.ColumnHeadersDefaultCellStyle.BackColor = TableHeaderColor; // Light gray #f6f7f8
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = TextSecondaryColor; // Gray text
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = TableHeaderColor;
            dgv.ColumnHeadersDefaultCellStyle.SelectionForeColor = TextSecondaryColor;
            dgv.ColumnHeadersDefaultCellStyle.Padding = new Padding(24, 12, 24, 12);
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgv.ColumnHeadersHeight = 56;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;

            // Row styling - White rows with dark text
            dgv.DefaultCellStyle.BackColor = TableRowColor; // White
            dgv.DefaultCellStyle.ForeColor = TextColor; // Dark text
            // IMPORTANT: Use opaque selection colors. DataGridView can render alpha colors as black.
            dgv.DefaultCellStyle.SelectionBackColor = SidebarActiveBackgroundColor; // Light blue selection
            dgv.DefaultCellStyle.SelectionForeColor = TextColor;
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 10.5F, FontStyle.Regular);
            dgv.DefaultCellStyle.Padding = new Padding(24, 10, 24, 10);
            dgv.DefaultCellStyle.WrapMode = DataGridViewTriState.False;
            dgv.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            // Alternating rows - Very light gray
            dgv.AlternatingRowsDefaultCellStyle.BackColor = TableAlternateRowColor; // Very light gray
            dgv.AlternatingRowsDefaultCellStyle.ForeColor = TextColor;
            dgv.AlternatingRowsDefaultCellStyle.SelectionBackColor = SidebarActiveBackgroundColor;
            dgv.AlternatingRowsDefaultCellStyle.SelectionForeColor = TextColor;
            dgv.AlternatingRowsDefaultCellStyle.Font = new Font("Segoe UI", 10.5F, FontStyle.Regular);
            dgv.AlternatingRowsDefaultCellStyle.Padding = dgv.DefaultCellStyle.Padding;
            dgv.AlternatingRowsDefaultCellStyle.WrapMode = DataGridViewTriState.False;
            dgv.AlternatingRowsDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            // Row headers
            dgv.RowHeadersDefaultCellStyle.BackColor = CardColor;
            dgv.RowHeadersDefaultCellStyle.ForeColor = TextColor;

            // Don't start with an ugly selected first row
            dgv.DataBindingComplete += (s, e) =>
            {
                try
                {
                    dgv.ClearSelection();
                    dgv.CurrentCell = null;
                }
                catch { }
            };
        }

        private static readonly ConditionalWeakTable<TextBox, System.Windows.Forms.Timer> _searchTimers = new();

        private static void EnableDoubleBuffering(Control control)
        {
            // WinForms DataGridView flickers / "ghosts" text on rapid updates unless double buffered.
            try
            {
                var prop = control.GetType().GetProperty("DoubleBuffered",
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                prop?.SetValue(control, true, null);
            }
            catch
            {
                // ignore
            }
        }

        /// <summary>
        /// Creates a styled button with the theme
        /// </summary>
        public static Button CreateThemedButton(string text, int x, int y, int width = 100, int height = 36, bool isPrimary = false)
        {
            Button button = new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(width, height)
            };
            ApplyButtonTheme(button, isPrimary);
            return button;
        }

        /// <summary>
        /// Creates a styled label with the theme
        /// </summary>
        public static Label CreateThemedLabel(string text, int x, int y, int width = 150, int height = 20, bool isHeader = false)
        {
            Label label = new Label
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(width, height),
                ForeColor = TextColor, // Always use high contrast white
                BackColor = Color.Transparent,
                Font = isHeader ? HeaderFont : LabelFont // Use LabelFont for better visibility
            };
            return label;
        }

        /// <summary>
        /// Creates a professional card/panel container
        /// </summary>
        public static Panel CreateCard(int x, int y, int width, int height)
        {
            Panel card = new Panel
            {
                Location = new Point(x, y),
                Size = new Size(width, height),
                BackColor = CardColor,
                Padding = new Padding(StandardPadding)
            };
            return card;
        }

        /// <summary>
        /// Creates a header panel for list forms (matching stitch design)
        /// </summary>
        public static Panel CreateListFormHeader(string title, Button addButton)
        {
            Panel header = new Panel
            {
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Dock = DockStyle.Top,
                Height = 80,
                Padding = new Padding(16)
            };

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = Color.White
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            Label lblTitle = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 28F, FontStyle.Bold),
                ForeColor = TextPrimaryColor,
                Dock = DockStyle.Fill,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleLeft
            };
            layout.Controls.Add(lblTitle, 0, 0);

            if (addButton != null)
            {
                // Compact primary button for header (prevents text clipping)
                addButton.AutoSize = false;
                addButton.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                ApplyButtonTheme(addButton, isPrimary: true);
                addButton.Size = new Size(170, 40);
                addButton.Padding = new Padding(14, 6, 14, 6);

                var right = new FlowLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    AutoSize = true,
                    WrapContents = false,
                    FlowDirection = FlowDirection.RightToLeft,
                    BackColor = Color.White,
                    Padding = new Padding(0, 8, 0, 8),
                    Margin = new Padding(0)
                };
                addButton.Margin = new Padding(0);
                right.Controls.Add(addButton);
                layout.Controls.Add(right, 1, 0);
            }

            header.Controls.Add(layout);

            return header;
        }

        /// <summary>
        /// Creates a search panel for list forms
        /// </summary>
        public static Panel CreateSearchPanel(TextBox searchBox, Button searchButton)
        {
            return CreateSearchPanel(searchBox, searchButton, "Search...");
        }

        /// <summary>
        /// Creates a search panel for list forms with a placeholder.
        /// </summary>
        public static Panel CreateSearchPanel(TextBox searchBox, Button searchButton, string placeholder)
        {
            Panel searchPanel = new Panel
            {
                BackColor = BackgroundColor,
                Dock = DockStyle.Top,
                Height = 80,
                Padding = new Padding(16)
            };

            searchBox.Location = new Point(16, 20);
            searchBox.Size = new Size(400, 40);
            // WinForms single-line TextBox ignores Height; use multiline for consistent 40px height.
            searchBox.Multiline = true;
            searchBox.Height = 40;
            searchBox.Font = NormalFont;
            searchBox.BackColor = Color.White;
            searchBox.ForeColor = TextSecondaryColor;
            searchBox.Text = placeholder;
            searchBox.Enter += (s, e) => 
            { 
                if (searchBox.Text == placeholder) 
                { 
                    searchBox.Text = ""; 
                    searchBox.ForeColor = TextColor; 
                } 
            };
            searchBox.Leave += (s, e) => 
            { 
                if (string.IsNullOrWhiteSpace(searchBox.Text)) 
                { 
                    searchBox.Text = placeholder; 
                    searchBox.ForeColor = TextSecondaryColor; 
                } 
            };
            searchPanel.Controls.Add(searchBox);

            if (searchButton != null)
            {
                searchButton.Location = new Point(430, 20);
                searchButton.Size = new Size(100, 40);
                searchButton.Text = "Search";
                searchButton.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                ApplyButtonTheme(searchButton, isPrimary: true);
                // Compact padding so "Search" isn't clipped
                searchButton.Padding = new Padding(12, 6, 12, 6);
                searchPanel.Controls.Add(searchButton);
            }

            return searchPanel;
        }

        /// <summary>
        /// Creates a white "card" container for tables and other content.
        /// </summary>
        public static Panel CreateCardContainer(Control content, Padding? padding = null)
        {
            var card = new Panel
            {
                BackColor = CardColor,
                BorderStyle = BorderStyle.FixedSingle,
                Dock = DockStyle.Fill,
                Padding = padding ?? new Padding(0)
            };

            if (content != null)
            {
                content.Dock = DockStyle.Fill;
                card.Controls.Add(content);
            }

            return card;
        }

        /// <summary>
        /// Standard layout for list forms: header + search + card table + footer.
        /// Re-parents the passed controls into a consistent layout.
        /// </summary>
        public static void ApplyStandardListLayout(
            Form form,
            string title,
            Button addButton,
            TextBox searchBox,
            Button searchButton,
            DataGridView dataGridView,
            IEnumerable<Button> leftFooterButtons,
            IEnumerable<Button> rightFooterButtons,
            string searchPlaceholder)
        {
            if (form == null) throw new ArgumentNullException(nameof(form));
            if (searchBox == null) throw new ArgumentNullException(nameof(searchBox));
            if (dataGridView == null) throw new ArgumentNullException(nameof(dataGridView));

            ApplyBaseTheme(form);
            form.Padding = Padding.Empty;

            form.SuspendLayout();
            try
            {
                // Ensure the key controls are detached from any old containers
                searchBox.Parent?.Controls.Remove(searchBox);
                searchButton?.Parent?.Controls.Remove(searchButton);
                addButton?.Parent?.Controls.Remove(addButton);
                dataGridView.Parent?.Controls.Remove(dataGridView);

                foreach (var b in leftFooterButtons ?? Enumerable.Empty<Button>())
                    b?.Parent?.Controls.Remove(b);
                foreach (var b in rightFooterButtons ?? Enumerable.Empty<Button>())
                    b?.Parent?.Controls.Remove(b);

                form.Controls.Clear();

                var root = new Panel
                {
                    Dock = DockStyle.Fill,
                    BackColor = BackgroundColor,
                    Padding = new Padding(StandardPadding)
                };

                // Header
                var header = CreateListFormHeader(title, addButton);

                // Search
                var searchPanel = CreateSearchPanel(searchBox, searchButton, searchPlaceholder);

                // Card/Table
                ApplyDataGridViewTheme(dataGridView);
                var card = CreateCardContainer(dataGridView);

                // Spacer between search and card (matches stitch spacing)
                var spacer = new Panel
                {
                    Dock = DockStyle.Top,
                    Height = SmallPadding,
                    BackColor = BackgroundColor
                };

                // Footer
                var footer = new Panel
                {
                    Dock = DockStyle.Bottom,
                    Height = 78,
                    BackColor = BorderLightColor, // slightly darker to boost contrast against light buttons
                    Padding = new Padding(0, 12, 0, 6)
                };

                var leftFlow = new FlowLayoutPanel
                {
                    Dock = DockStyle.Left,
                    AutoSize = true,
                    WrapContents = false,
                    FlowDirection = FlowDirection.LeftToRight,
                    BackColor = BorderLightColor,
                    Padding = new Padding(0),
                    Margin = new Padding(0)
                };

                var rightFlow = new FlowLayoutPanel
                {
                    Dock = DockStyle.Right,
                    AutoSize = true,
                    WrapContents = false,
                    FlowDirection = FlowDirection.RightToLeft,
                    BackColor = BorderLightColor,
                    Padding = new Padding(0),
                    Margin = new Padding(0)
                };

                foreach (var b in (leftFooterButtons ?? Enumerable.Empty<Button>()).Where(x => x != null))
                {
                    // Make left actions solid blue for clear contrast on light footers.
                    ApplyButtonTheme(b, isPrimary: true);
                    b.AutoSize = false;
                    b.Height = 44;
                    b.Padding = new Padding(14, 10, 14, 10);
                    b.UseCompatibleTextRendering = false; // rely on GDI text for better clarity
                    b.BackColor = PrimaryColor;
                    b.ForeColor = TextOnPrimary;
                    b.TextAlign = ContentAlignment.MiddleCenter;
                    b.FlatAppearance.BorderSize = 0;
                    b.FlatAppearance.BorderColor = PrimaryColor;
                    b.Width = Math.Max(130, TextRenderer.MeasureText(b.Text ?? "", b.Font).Width + 40);
                    b.Margin = new Padding(0, 0, 10, 0);
                    leftFlow.Controls.Add(b);
                }

                foreach (var b in (rightFooterButtons ?? Enumerable.Empty<Button>()).Where(x => x != null))
                {
                    if (b.Text?.Equals("Close", StringComparison.OrdinalIgnoreCase) == true)
                        ApplyButtonTheme(b, isSecondary: true);
                    else
                        ApplyButtonTheme(b, isPrimary: false, isSecondary: true);

                    b.AutoSize = false;
                    b.Height = 44;
                    b.Padding = new Padding(14, 10, 14, 10);
                    b.UseCompatibleTextRendering = false;
                    // Keep right buttons light but with stronger border for contrast on the footer
                    b.BackColor = ButtonColor;
                    b.ForeColor = TextColor;
                    b.TextAlign = ContentAlignment.MiddleCenter;
                    b.FlatAppearance.BorderSize = 1;
                    b.FlatAppearance.BorderColor = BorderDarkColor;
                    b.Width = Math.Max(b.Text?.Equals("Close", StringComparison.OrdinalIgnoreCase) == true ? 150 : 130,
                        TextRenderer.MeasureText(b.Text ?? "", b.Font).Width + 40);
                    if (b.Text?.Equals("Close", StringComparison.OrdinalIgnoreCase) == true)
                        b.Margin = new Padding(16, 0, 0, 0);
                    else
                        b.Margin = new Padding(10, 0, 0, 0);
                    rightFlow.Controls.Add(b);
                }

                footer.Controls.Add(rightFlow);
                footer.Controls.Add(leftFlow);

                // Dock order: add fill first, then footer, then search/header on top
                root.Controls.Add(card);
                root.Controls.Add(footer);
                root.Controls.Add(spacer);
                root.Controls.Add(searchPanel);
                root.Controls.Add(header);

                form.Controls.Add(root);

                // Wire live search filtering
                WireSearch(searchBox, searchButton, dataGridView, searchPlaceholder);
            }
            finally
            {
                form.ResumeLayout(performLayout: true);
            }
        }

        /// <summary>
        /// Converts a standard dialog form into a "card" layout: gray backdrop + white bordered card,
        /// with a header title and a footer action bar. Existing controls are moved into the content area.
        /// </summary>
        public static void ApplyCardDialogLayout(
            Form form,
            string title,
            IEnumerable<Button> footerRightButtons,
            IEnumerable<Button> footerLeftButtons = null)
        {
            if (form == null) throw new ArgumentNullException(nameof(form));

            ApplyBaseTheme(form);
            form.Padding = Padding.Empty;

            // Capture current controls before we rebuild
            var all = form.Controls.Cast<Control>().ToList();
            var originalClientSize = form.ClientSize;

            form.SuspendLayout();
            try
            {
                form.Controls.Clear();

                var root = new Panel
                {
                    Dock = DockStyle.Fill,
                    BackColor = BackgroundColor,
                    Padding = new Padding(LargePadding)
                };

                var card = new Panel
                {
                    Dock = DockStyle.Fill,
                    BackColor = CardColor,
                    BorderStyle = BorderStyle.FixedSingle
                };

                var header = new Panel
                {
                    Dock = DockStyle.Top,
                    Height = 72,
                    BackColor = CardColor,
                    Padding = new Padding(24, 16, 24, 0)
                };

                var lblTitle = new Label
                {
                    Text = title,
                    Font = new Font("Segoe UI", 20F, FontStyle.Bold),
                    ForeColor = TextPrimaryColor,
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleLeft
                };
                header.Controls.Add(lblTitle);

                var content = new Panel
                {
                    Dock = DockStyle.Fill,
                    BackColor = CardColor,
                    // Scrollbars for long forms (requested)
                    AutoScroll = true,
                    Padding = new Padding(24, 16, 24, 16)
                };

                var footer = new Panel
                {
                    Dock = DockStyle.Bottom,
                    Height = 72,
                    BackColor = CardColor,
                    Padding = new Padding(32, 12, 32, 12)
                };

                var leftFlow = new FlowLayoutPanel
                {
                    Dock = DockStyle.Left,
                    AutoSize = true,
                    WrapContents = false,
                    FlowDirection = FlowDirection.LeftToRight,
                    BackColor = CardColor
                };

                var rightFlow = new FlowLayoutPanel
                {
                    Dock = DockStyle.Right,
                    AutoSize = true,
                    WrapContents = false,
                    FlowDirection = FlowDirection.RightToLeft,
                    BackColor = CardColor
                };

                foreach (var b in (footerLeftButtons ?? Enumerable.Empty<Button>()).Where(x => x != null))
                {
                    ApplyButtonTheme(b, isSecondary: true);
                    ConfigureDialogFooterButton(b, isPrimary: false, isSecondary: true);
                    b.Margin = new Padding(0, 0, 12, 0);
                    leftFlow.Controls.Add(b);
                }

                foreach (var b in (footerRightButtons ?? Enumerable.Empty<Button>()).Where(x => x != null))
                {
                    // default: first (right-most) is primary
                    ApplyButtonTheme(b, isPrimary: true);
                    ConfigureDialogFooterButton(b, isPrimary: true, isSecondary: false);
                    b.Margin = new Padding(12, 0, 0, 0);
                    rightFlow.Controls.Add(b);
                }

                footer.Controls.Add(rightFlow);
                footer.Controls.Add(leftFlow);

                // Move existing controls into content, except footer buttons
                var footerButtons = new HashSet<Button>((footerLeftButtons ?? Enumerable.Empty<Button>())
                    .Concat(footerRightButtons ?? Enumerable.Empty<Button>())
                    .Where(x => x != null));

                foreach (var c in all)
                {
                    if (c is Button btn && footerButtons.Contains(btn))
                        continue;
                    content.Controls.Add(c);
                }

                // Normalize absolute-positioned controls so they all start with a consistent inset.
                // Even with scrollbars, this prevents left-clipped labels/inputs.
                NormalizeAbsoluteLayoutControls(content, insetX: 24, insetY: 16);

                card.Controls.Add(content);
                card.Controls.Add(footer);
                card.Controls.Add(header);

                root.Controls.Add(card);
                form.Controls.Add(root);

                // Ensure footer button styles: first right button primary, others secondary
                bool first = true;
                foreach (var b in (footerRightButtons ?? Enumerable.Empty<Button>()).Where(x => x != null))
                {
                    ApplyButtonTheme(b, isPrimary: first, isSecondary: !first);
                    ConfigureDialogFooterButton(b, isPrimary: first, isSecondary: !first);
                    first = false;
                }
                foreach (var b in (footerLeftButtons ?? Enumerable.Empty<Button>()).Where(x => x != null))
                {
                    ApplyButtonTheme(b, isSecondary: true);
                    ConfigureDialogFooterButton(b, isPrimary: false, isSecondary: true);
                }

                // Keep a sane default size; allow scrolling instead of auto-growing the dialog
                if (originalClientSize.Width < 900 || originalClientSize.Height < 560)
                {
                    form.ClientSize = new Size(Math.Max(originalClientSize.Width, 900), Math.Max(originalClientSize.Height, 560));
                }
            }
            finally
            {
                form.ResumeLayout(performLayout: true);
            }
        }

        private static void ConfigureDialogFooterButton(Button b, bool isPrimary, bool isSecondary)
        {
            if (b == null) return;

            // Override ApplyButtonTheme's big padding for compact 40px footer buttons (prevents text clipping).
            b.UseVisualStyleBackColor = false;
            b.AutoSize = false;
            b.Height = 40;
            b.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            b.TextAlign = ContentAlignment.MiddleCenter;
            b.Padding = new Padding(12, 6, 12, 6);

            try
            {
                // Ensure text renders correctly even with custom padding
                b.UseCompatibleTextRendering = true;
            }
            catch { }

            int measured = TextRenderer.MeasureText(b.Text ?? "", b.Font).Width;
            b.Width = Math.Min(240, Math.Max(120, measured + 36));

            // Re-apply hover colors explicitly (compact buttons still need good feedback)
            if (isPrimary)
            {
                b.FlatAppearance.MouseOverBackColor = PrimaryHoverColor;
                b.FlatAppearance.MouseDownBackColor = Color.FromArgb(15, 105, 200);
            }
            else if (isSecondary)
            {
                b.FlatAppearance.MouseOverBackColor = Color.FromArgb(226, 232, 240);
                b.FlatAppearance.MouseDownBackColor = Color.FromArgb(203, 213, 225);
            }
            else
            {
                b.FlatAppearance.MouseOverBackColor = ButtonHoverColor;
                b.FlatAppearance.MouseDownBackColor = Color.FromArgb(241, 245, 249);
            }
        }

        private static void NormalizeAbsoluteLayoutControls(Control content, int insetX, int insetY)
        {
            if (content == null) return;

            int minX = int.MaxValue;
            int minY = int.MaxValue;
            foreach (Control c in content.Controls)
            {
                if (!c.Visible) continue;
                minX = Math.Min(minX, c.Left);
                minY = Math.Min(minY, c.Top);
            }

            if (minX == int.MaxValue || minY == int.MaxValue)
                return;

            // Shift so the left/top-most control aligns to inset
            int dx = insetX - minX;
            int dy = insetY - minY;

            // Only shift if needed (prevents repeatedly moving controls if called twice)
            if (dx == 0 && dy == 0) return;

            foreach (Control c in content.Controls)
            {
                if (!c.Visible) continue;
                c.Location = new Point(c.Left + dx, c.Top + dy);
            }
        }

        // NOTE: FitDialogToContent removed in favor of scrollbars on popups.

        /// <summary>
        /// Adds live filtering to a DataGridView bound to a DataTable/DataView, using RowFilter across all columns.
        /// </summary>
        public static void WireSearch(TextBox searchBox, Button searchButton, DataGridView dataGridView, string placeholder)
        {
            if (searchBox == null || dataGridView == null) return;

            var timer = _searchTimers.GetValue(searchBox, tb =>
            {
                var t = new System.Windows.Forms.Timer { Interval = 180 };
                t.Tick += (s, e) =>
                {
                    t.Stop();
                    Apply();
                };
                return t;
            });

            void Apply()
            {
                try
                {
                    string raw = searchBox.Text ?? "";
                    string term = raw.Trim();
                    if (string.IsNullOrWhiteSpace(term) || string.Equals(term, placeholder, StringComparison.OrdinalIgnoreCase))
                    {
                        ClearFilter(dataGridView);
                        return;
                    }
                    ApplyFilter(dataGridView, term);
                    dataGridView.Invalidate();
                }
                catch
                {
                    // Ignore filter errors (e.g., non-filterable data sources)
                }
            }

            // Avoid double-wiring if called multiple times
            searchBox.TextChanged -= SearchBox_TextChanged;
            searchBox.TextChanged += SearchBox_TextChanged;
            if (searchButton != null)
            {
                searchButton.Click -= SearchButton_Click;
                searchButton.Click += SearchButton_Click;
            }
            searchBox.KeyDown -= SearchBox_KeyDown;
            searchBox.KeyDown += SearchBox_KeyDown;

            void SearchBox_TextChanged(object sender, EventArgs e)
            {
                // Debounce repaint-heavy filtering to avoid "meshed" text artifacts during rapid typing.
                timer.Stop();
                timer.Start();
            }

            void SearchButton_Click(object sender, EventArgs e)
            {
                timer.Stop();
                Apply();
            }
            void SearchBox_KeyDown(object sender, KeyEventArgs e)
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    timer.Stop();
                    Apply();
                }
            }
        }

        private static void ClearFilter(DataGridView dataGridView)
        {
            if (dataGridView.DataSource is DataTable dt)
            {
                dt.DefaultView.RowFilter = "";
                return;
            }
            if (dataGridView.DataSource is DataView dv)
            {
                dv.RowFilter = "";
                return;
            }
            if (dataGridView.DataSource is BindingSource bs)
            {
                bs.RemoveFilter();
            }
        }

        private static void ApplyFilter(DataGridView dataGridView, string term)
        {
            DataView dv = null;
            if (dataGridView.DataSource is DataTable dt)
            {
                dv = dt.DefaultView;
            }
            else if (dataGridView.DataSource is DataView view)
            {
                dv = view;
            }
            else if (dataGridView.DataSource is BindingSource bs)
            {
                // Try to filter the underlying view if possible
                if (bs.List is DataView viewFromBs)
                    dv = viewFromBs;
                else if (bs.DataSource is DataTable tableFromBs)
                    dv = tableFromBs.DefaultView;
            }

            if (dv == null || dv.Table == null) return;

            string escaped = term.Replace("'", "''");
            var parts = new List<string>();
            foreach (DataColumn col in dv.Table.Columns)
            {
                string name = col.ColumnName.Replace("]", "]]");
                parts.Add($"CONVERT([{name}], 'System.String') LIKE '%{escaped}%'");
            }

            dv.RowFilter = parts.Count == 0 ? "" : string.Join(" OR ", parts);
        }
    }
}
