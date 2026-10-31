using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HotelReservation
{
    public partial class Form4 : Form
    {
        private DataGridView gridArrivals;
        private TextBox txtSearch;
        private Button btnDelete;

        // Admin Room Addition Controls
        private ComboBox cmbRoomTypeAdmin;
        private NumericUpDown numRoomsToAdd;
        private Button btnAddRooms;

        public Form4()
        {
            InitializeComponent();
            this.AutoSize = false;
            this.WindowState = FormWindowState.Maximized;

            WireUpSidebarNavigation();
            SetupDashboardResponsiveness();
            SetupLiveGridAndControls();
            SetupAdminRoomManagerControls();
            RefreshDashboardData();

            this.Resize += (s, e) => AdjustDashboard();
        }

        private void SetupAdminRoomManagerControls()
        {
            // Expand groupBox5 height slightly to house the Admin controls neatly
            groupBox5.Height = 360;

            // Admin Section Header inside groupBox5
            Label lblAdminHeader = new Label
            {
                Text = "Admin Room Control",
                Font = new Font("Georgia", 10.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(122, 31, 62),
                Location = new Point(20, 280),
                AutoSize = true
            };
            groupBox5.Controls.Add(lblAdminHeader);

            // Dropdown: Choose room category
            cmbRoomTypeAdmin = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Georgia", 9f),
                Location = new Point(20, 310),
                Width = 140
            };
            cmbRoomTypeAdmin.Items.AddRange(new object[] { "Standard", "Suites", "Deluxe", "Release Cleaned" });
            cmbRoomTypeAdmin.SelectedIndex = 0;
            groupBox5.Controls.Add(cmbRoomTypeAdmin);

            // Numeric quantity counter
            numRoomsToAdd = new NumericUpDown
            {
                Location = new Point(170, 310),
                Width = 55,
                Font = new Font("Georgia", 9.5f),
                Minimum = 1,
                Maximum = 50,
                Value = 1
            };
            groupBox5.Controls.Add(numRoomsToAdd);

            // "+ Add" Button
            btnAddRooms = new Button
            {
                Text = "+ Add",
                Location = new Point(235, 307),
                Size = new Size(85, 29),
                BackColor = Color.FromArgb(122, 31, 62),
                ForeColor = Color.FromArgb(247, 227, 200),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Georgia", 9f, FontStyle.Bold)
            };
            btnAddRooms.FlatAppearance.BorderSize = 0;
            btnAddRooms.Click += BtnAddRooms_Click;
            groupBox5.Controls.Add(btnAddRooms);
        }

        private void BtnAddRooms_Click(object sender, EventArgs e)
        {
            string selectedCategory = cmbRoomTypeAdmin.SelectedItem?.ToString() ?? "Standard";
            int count = (int)numRoomsToAdd.Value;

            DataStore.AddFreeRooms(selectedCategory, count);

            MessageBox.Show(
                $"Successfully added {count} free room(s) to '{selectedCategory}'!\nInventory is now updated for guests.",
                "Inventory Updated",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            RefreshDashboardData();
        }

        private void SetupLiveGridAndControls()
        {
            // Hide legacy static labels inside panel2
            Control[] oldLabels = new Control[]
            {
                label3, label4, label5, label6, label7, label8,
                label9, label10, label11, label12, label14,
                label15, label16, label17, label18,
                panel3, panel4, panel5, panel6
            };
            foreach (var c in oldLabels)
            {
                if (c != null) c.Visible = false;
            }

            // 1. Live Search Box
            txtSearch = new TextBox
            {
                Location = new Point(28, 75),
                Size = new Size(320, 26),
                Font = new Font("Georgia", 9.5f),
                Text = "Search guest or room..."
            };
            txtSearch.Enter += (s, e) => { if (txtSearch.Text == "Search guest or room...") txtSearch.Text = ""; };
            txtSearch.Leave += (s, e) => { if (string.IsNullOrWhiteSpace(txtSearch.Text)) txtSearch.Text = "Search guest or room..."; };
            txtSearch.TextChanged += (s, e) => FilterGrid(txtSearch.Text);
            panel2.Controls.Add(txtSearch);

            // 2. Cancel Reservation Button
            btnDelete = new Button
            {
                Text = "Cancel Selected",
                Location = new Point(360, 72),
                Size = new Size(140, 32),
                BackColor = Color.FromArgb(122, 31, 62),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Georgia", 9f, FontStyle.Bold)
            };
            btnDelete.FlatAppearance.BorderSize = 0;
            btnDelete.Click += BtnDelete_Click;
            panel2.Controls.Add(btnDelete);

            // 3. Dynamic Grid
            gridArrivals = new DataGridView
            {
                Location = new Point(28, 115),
                Size = new Size(panel2.Width - 56, panel2.Height - 135),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                Font = new Font("Georgia", 9.5f)
            };

            gridArrivals.EnableHeadersVisualStyles = false;
            gridArrivals.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(247, 243, 240);
            gridArrivals.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(61, 42, 48);
            gridArrivals.ColumnHeadersDefaultCellStyle.Font = new Font("Georgia", 10f, FontStyle.Bold);

            gridArrivals.Columns.Add("Guest", "Guest");
            gridArrivals.Columns.Add("Room", "Room #");
            gridArrivals.Columns.Add("Type", "Room Type");
            gridArrivals.Columns.Add("Total", "Total (ETB)");
            gridArrivals.Columns.Add("Payment", "Payment");
            gridArrivals.Columns.Add("Status", "Status");

            panel2.Controls.Add(gridArrivals);
        }

        private void RefreshDashboardData()
        {
            FilterGrid("");

            // Top metric cards
            int occupiedCount = DataStore.Reservations.Count;
            label36.Text = $"{occupiedCount} of {DataStore.TotalRooms}";

            int arrivesToday = DataStore.Reservations.Count(r => r.CheckIn.Date == DateTime.Today);
            label37.Text = arrivesToday.ToString();

            int departsToday = DataStore.Reservations.Count(r => r.CheckOut.Date == DateTime.Today);
            label38.Text = departsToday.ToString();

            int pendingPayments = DataStore.Reservations.Count(r => r.PaymentStatus == "Pending");
            label39.Text = pendingPayments.ToString();

            // Room inventory counts (Capacity minus active bookings)
            int standardBooked = DataStore.Reservations.Count(r => r.RoomType.Contains("Standard"));
            int suitesBooked = DataStore.Reservations.Count(r => r.RoomType.Contains("Suite"));
            int deluxeBooked = DataStore.Reservations.Count(r => r.RoomType.Contains("Deluxe"));

            label24.Text = Math.Max(0, DataStore.StandardCapacity - standardBooked).ToString();
            label23.Text = Math.Max(0, DataStore.SuitesCapacity - suitesBooked).ToString();
            label25.Text = Math.Max(0, DataStore.DeluxeCapacity - deluxeBooked).ToString();
            label26.Text = DataStore.BeingCleanedCount.ToString();
        }

        private void FilterGrid(string keyword)
        {
            gridArrivals.Rows.Clear();
            string search = (keyword == "Search guest or room...") ? "" : keyword.Trim().ToLower();

            var list = DataStore.Reservations.AsEnumerable();
            if (!string.IsNullOrEmpty(search))
            {
                list = list.Where(r => r.GuestName.ToLower().Contains(search) || r.RoomNumber.ToLower().Contains(search));
            }

            foreach (var r in list)
            {
                gridArrivals.Rows.Add(r.GuestName, r.RoomNumber, r.RoomType, $"{r.TotalPrice:N0}", r.PaymentMethod, r.PaymentStatus);
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (gridArrivals.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a reservation to cancel.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int selectedIndex = gridArrivals.SelectedRows[0].Index;
            string guest = gridArrivals.Rows[selectedIndex].Cells["Guest"].Value?.ToString();

            DialogResult confirm = MessageBox.Show($"Are you sure you want to cancel the reservation for '{guest}'?", "Confirm Cancellation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm == DialogResult.Yes)
            {
                var match = DataStore.Reservations.FirstOrDefault(r => r.GuestName == guest);
                if (match != null)
                {
                    DataStore.Reservations.Remove(match);
                }
                RefreshDashboardData();
            }
        }

        private void WireUpSidebarNavigation()
        {
            label21.Cursor = Cursors.Hand; // Home
            label28.Cursor = Cursors.Hand; // Rooms and Suites
            label22.Cursor = Cursors.Hand; // Reservation
            label20.Cursor = Cursors.Hand; // Dashboard

            label21.Click += (s, e) => NavigateToHome();
            label28.Click += (s, e) => NavigateToRooms();
            label22.Click += (s, e) => NavigateToReservation();
            button3.Click += (s, e) => NavigateToReservation();
        }

        private void SetupDashboardResponsiveness()
        {
            groupBox1.Dock = DockStyle.Left;
            button3.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            AdjustDashboard();
        }

        private void AdjustDashboard()
        {
            int sidebarWidth = groupBox1.Width;
            int contentStartX = sidebarWidth + 40;
            int availableWidth = this.ClientSize.Width - contentStartX - 40;

            if (availableWidth < 600) availableWidth = 600;

            int spacing = 20;
            int cardWidth = (availableWidth - (3 * spacing)) / 4;

            groupBox2.Width = cardWidth;
            groupBox3.Width = cardWidth;
            groupBox4.Width = cardWidth;
            groupBox6.Width = cardWidth;

            groupBox2.Left = contentStartX;
            groupBox3.Left = contentStartX + cardWidth + spacing;
            groupBox4.Left = contentStartX + (cardWidth + spacing) * 2;
            groupBox6.Left = contentStartX + (cardWidth + spacing) * 3;

            panel2.Left = contentStartX;
            panel2.Width = (int)(availableWidth * 0.62);

            groupBox5.Left = panel2.Right + 30;
            groupBox5.Width = availableWidth - panel2.Width - 30;
        }

        private void NavigateToHome()
        {
            Home home = null;
            foreach (Form openForm in Application.OpenForms)
            {
                if (openForm is Home)
                {
                    home = (Home)openForm;
                    break;
                }
            }

            if (home != null)
            {
                home.Show();
                home.BringToFront();
            }
            else
            {
                new Home().Show();
            }

            this.Close();
        }

        private void NavigateToRooms()
        {
            Form2 rooms = new Form2();
            this.Hide();
            rooms.Show();
            rooms.FormClosed += (s, args) => this.Show();
        }

        private void NavigateToReservation()
        {
            Form3 reservation = new Form3();
            this.Hide();
            reservation.Show();
            reservation.FormClosed += (s, args) =>
            {
                this.Show();
                RefreshDashboardData();
            };
        }

        private void label19_Click(object sender, EventArgs e) { }
        private void label40_Click(object sender, EventArgs e) { }
        private void Form4_Load(object sender, EventArgs e) { }
    }
}