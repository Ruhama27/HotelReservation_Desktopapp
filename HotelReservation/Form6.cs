using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HotelReservation
{
    public partial class Form6 : Form
    {
        private DataGridView roomsGrid;
        private DataGridView servicesGrid;

        public Form6()
        {
            InitializeComponent();
            WindowState = FormWindowState.Maximized;
            AutoScroll = false;
            Text = "Room Management";
            BuildManagementTables();
            SetupHeaderNavigation();
        }

        private void SetupHeaderNavigation()
        {
            label5.Cursor = Cursors.Hand;
            label6.Cursor = Cursors.Hand;
            label7.Cursor = Cursors.Hand;

            label5.Click += (s, e) => NavigateTo(new Home());
            label7.Click += (s, e) => NavigateTo(new Form2());
            label6.Click += (s, e) => NavigateTo(new Form3());

            groupBox2.Resize += (s, e) => ArrangeHeaderNavigation();
            ArrangeHeaderNavigation();
        }

        private void ArrangeHeaderNavigation()
        {
            int rightMargin = groupBox2.ClientSize.Width - 40;
            int spacing = 28;
            int centerY = (groupBox2.ClientSize.Height - label5.Height) / 2;

            label6.Top = centerY;
            label7.Top = centerY;
            label5.Top = centerY;

            label6.Left = rightMargin - label6.Width;
            label7.Left = label6.Left - label7.Width - spacing;
            label5.Left = label7.Left - label5.Width - spacing;
        }

        private void NavigateTo(Form nextForm)
        {
            Hide();
            nextForm.FormClosed += (s, e) => Close();
            nextForm.Show();
        }

        private void BuildManagementTables()
        {
            groupBox3.Controls.Clear();
            groupBox3.Location = new Point(0, 115);
            groupBox3.Size = new Size(ClientSize.Width, ClientSize.Height - 115);
            groupBox3.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            Label title = new Label
            {
                Text = "Room and Service Management",
                Font = new Font("Georgia", 24f, FontStyle.Bold | FontStyle.Italic),
                ForeColor = Color.FromArgb(61, 42, 48),
                AutoSize = true,
                Location = new Point(38, 25)
            };
            Label hint = new Label
            {
                Text = "Edit prices and statuses, then select Save Changes.",
                ForeColor = Color.FromArgb(100, 85, 88),
                AutoSize = true,
                Location = new Point(42, 68)
            };

            Label roomTitle = CreateSectionLabel("Rooms", 38, 105);
            roomsGrid = CreateGrid(new Point(38, 140), new Size(850, 300));
            roomsGrid.Columns.Add("RoomNumber", "Room #");
            roomsGrid.Columns.Add("RoomType", "Room Type");
            roomsGrid.Columns.Add("NightlyRate", "Nightly Price (ETB)");
            roomsGrid.Columns.Add(new DataGridViewComboBoxColumn
            {
                Name = "Status",
                HeaderText = "Status",
                DataSource = Enum.GetNames(typeof(RoomStatus))
            });

            Label serviceTitle = CreateSectionLabel("Services", 38, 465);
            servicesGrid = CreateGrid(new Point(38, 500), new Size(850, 135));
            servicesGrid.Columns.Add("Name", "Service");
            servicesGrid.Columns.Add("Price", "Price (ETB)");
            servicesGrid.Columns.Add(new DataGridViewCheckBoxColumn
            {
                Name = "Available",
                HeaderText = "Available"
            });

            Button save = CreateButton("Save Changes", Color.FromArgb(122, 31, 62), new Point(38, 650), 145);
            save.Click += SaveChanges_Click;
            Button close = CreateButton("Close", Color.FromArgb(185, 123, 46), new Point(198, 650), 105);
            close.Click += (s, e) => Close();

            groupBox3.Controls.AddRange(new Control[]
            {
                title, hint, roomTitle, roomsGrid, serviceTitle, servicesGrid, save, close
            });
            groupBox3.Resize += (s, e) => ResizeManagementTables();
            LoadTables();
        }

        private Label CreateSectionLabel(string text, int x, int y)
        {
            return new Label
            {
                Text = text,
                Font = new Font("Georgia", 16f, FontStyle.Bold | FontStyle.Italic),
                ForeColor = Color.FromArgb(122, 31, 62),
                AutoSize = true,
                Location = new Point(x, y)
            };
        }

        private DataGridView CreateGrid(Point location, Size size)
        {
            DataGridView grid = new DataGridView
            {
                Location = location,
                Size = size,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                EnableHeadersVisualStyles = false,
                Font = new Font("Segoe UI", 9.5f),
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(122, 31, 62);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Georgia", 9.5f, FontStyle.Bold);
            grid.ColumnHeadersHeight = 34;
            grid.DefaultCellStyle.Padding = new Padding(6, 0, 6, 0);
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(253, 251, 250);
            return grid;
        }

        private Button CreateButton(string text, Color color, Point location, int width)
        {
            Button button = new Button
            {
                Text = text,
                Location = location,
                Size = new Size(width, 38),
                BackColor = color,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Georgia", 9f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            button.FlatAppearance.BorderSize = 0;
            return button;
        }

        private void LoadTables()
        {
            foreach (Room room in DataStore.Rooms)
                roomsGrid.Rows.Add(room.RoomNumber, room.RoomType, room.NightlyRate, room.Status.ToString());

            foreach (HotelService service in DataStore.Services)
                servicesGrid.Rows.Add(service.Name, service.Price, service.IsAvailable);
        }

        private void SaveChanges_Click(object sender, EventArgs e)
        {
            roomsGrid.EndEdit();
            servicesGrid.EndEdit();

            if (!ValidatePrices(roomsGrid, "NightlyRate") || !ValidatePrices(servicesGrid, "Price"))
                return;

            foreach (DataGridViewRow row in roomsGrid.Rows)
            {
                Room room = DataStore.Rooms.FirstOrDefault(r => r.RoomNumber == Convert.ToString(row.Cells["RoomNumber"].Value));
                if (room == null) continue;
                room.NightlyRate = Convert.ToDecimal(row.Cells["NightlyRate"].Value);
                room.Status = (RoomStatus)Enum.Parse(typeof(RoomStatus), Convert.ToString(row.Cells["Status"].Value));
            }

            for (int i = 0; i < servicesGrid.Rows.Count && i < DataStore.Services.Count; i++)
            {
                HotelService service = DataStore.Services[i];
                service.Price = Convert.ToDecimal(servicesGrid.Rows[i].Cells["Price"].Value);
                service.IsAvailable = Convert.ToBoolean(servicesGrid.Rows[i].Cells["Available"].Value);
            }

            MessageBox.Show("Room and service settings saved successfully.", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private bool ValidatePrices(DataGridView grid, string columnName)
        {
            foreach (DataGridViewRow row in grid.Rows)
            {
                decimal price;
                if (!decimal.TryParse(Convert.ToString(row.Cells[columnName].Value), out price) || price < 0)
                {
                    MessageBox.Show("Prices must be valid non-negative numbers.", "Invalid Price", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    grid.CurrentCell = row.Cells[columnName];
                    grid.BeginEdit(true);
                    return false;
                }
            }
            return true;
        }

        private void ResizeManagementTables()
        {
            if (roomsGrid == null || servicesGrid == null) return;
            int width = Math.Max(600, groupBox3.ClientSize.Width - 76);
            roomsGrid.Width = width;
            servicesGrid.Width = width;
        }

        private void groupBox3_Enter(object sender, EventArgs e) { }
    }
}
