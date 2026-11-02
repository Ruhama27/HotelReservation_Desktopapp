using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace HotelReservation
{
    public partial class Form4 : Form
    {
        private DataGridView gridArrivals;
        private TextBox txtSearch;
        private Button btnVerifyPayment;
        private Button btnRejectPayment;
        private Button btnCheckOut;
        private Button btnDelete;

        private ComboBox cmbCleaningRooms;
        private Button btnReleaseToBusiness;
        private Label lblCleaningSummary;
        private Button btnManageRooms;

        public Form4()
        {
            InitializeComponent();
            this.AutoSize = false;
            this.WindowState = FormWindowState.Maximized;

            WireUpSidebarNavigation();
            SetupDashboardResponsiveness();
            SetupPolishedArrivalsTable();
            SetupAdminRoomReleaseControls();
            SetupManageRoomsButton();
            ApplyRoundedInterface();
            RefreshDashboardData();

            this.Resize += (s, e) => AdjustDashboard();
        }

        private void SetupPolishedArrivalsTable()
        {
            Control[] oldLabels = new Control[]
            {
                label3, label4, label5, label6, label7, label8,
                label9, label10, label11, label12, label14,
                label15, label16, label17, label18,
                panel3, panel4, panel5, panel6
            };
            foreach (var c in oldLabels) if (c != null) c.Visible = false;

            // Search Bar
            txtSearch = new TextBox
            {
                Location = new Point(25, 72),
                Size = new Size(200, 28),
                Font = new Font("Georgia", 9.5f),
                Text = "Search guest, room, ref..."
            };
            txtSearch.Enter += (s, e) => { if (txtSearch.Text == "Search guest, room, ref...") txtSearch.Text = ""; };
            txtSearch.Leave += (s, e) => { if (string.IsNullOrWhiteSpace(txtSearch.Text)) txtSearch.Text = "Search guest, room, ref..."; };
            txtSearch.TextChanged += (s, e) => FilterGrid(txtSearch.Text);
            panel2.Controls.Add(txtSearch);

            // Verify Payment Action Button
            btnVerifyPayment = new Button
            {
                Text = "✓ Verify Payment",
                Location = new Point(235, 70),
                Size = new Size(130, 32),
                BackColor = Color.FromArgb(34, 139, 34),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Georgia", 8.5f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnVerifyPayment.FlatAppearance.BorderSize = 0;
            btnVerifyPayment.Click += BtnVerifyPayment_Click;
            panel2.Controls.Add(btnVerifyPayment);

            // Reject Payment Action Button
            btnRejectPayment = new Button
            {
                Text = "✕ Reject",
                Location = new Point(372, 70),
                Size = new Size(80, 32),
                BackColor = Color.FromArgb(178, 34, 34),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Georgia", 8.5f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnRejectPayment.FlatAppearance.BorderSize = 0;
            btnRejectPayment.Click += BtnRejectPayment_Click;
            panel2.Controls.Add(btnRejectPayment);

            // Check Out Action Button
            btnCheckOut = new Button
            {
                Text = "Check Out",
                Location = new Point(458, 70),
                Size = new Size(100, 32),
                BackColor = Color.FromArgb(185, 123, 46),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Georgia", 8.5f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnCheckOut.FlatAppearance.BorderSize = 0;
            btnCheckOut.Click += BtnCheckOut_Click;
            panel2.Controls.Add(btnCheckOut);

            // Delete Record Button
            btnDelete = new Button
            {
                Text = "Delete",
                Location = new Point(565, 70),
                Size = new Size(75, 32),
                BackColor = Color.FromArgb(122, 31, 62),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Georgia", 8.5f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnDelete.FlatAppearance.BorderSize = 0;
            btnDelete.Click += BtnDelete_Click;
            panel2.Controls.Add(btnDelete);

            // DataGridView Setup
            gridArrivals = new DataGridView
            {
                Location = new Point(25, 112),
                Size = new Size(panel2.Width - 50, panel2.Height - 128),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                GridColor = Color.FromArgb(240, 235, 230),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                AllowUserToResizeRows = false,
                RowTemplate = { Height = 40 },
                Font = new Font("Segoe UI", 9.5f)
            };

            gridArrivals.EnableHeadersVisualStyles = false;
            gridArrivals.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            gridArrivals.ColumnHeadersHeight = 44;
            gridArrivals.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(247, 243, 240);
            gridArrivals.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(61, 42, 48);
            gridArrivals.ColumnHeadersDefaultCellStyle.Font = new Font("Georgia", 9.5f, FontStyle.Bold);
            gridArrivals.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(247, 243, 240);
            gridArrivals.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.FromArgb(61, 42, 48);
            gridArrivals.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            gridArrivals.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;
            gridArrivals.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;

            gridArrivals.DefaultCellStyle.BackColor = Color.White;
            gridArrivals.DefaultCellStyle.ForeColor = Color.FromArgb(50, 50, 50);
            gridArrivals.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            gridArrivals.DefaultCellStyle.Padding = new Padding(6, 0, 6, 0);
            gridArrivals.DefaultCellStyle.SelectionBackColor = Color.FromArgb(245, 230, 225);
            gridArrivals.DefaultCellStyle.SelectionForeColor = Color.FromArgb(122, 31, 62);
            gridArrivals.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(253, 251, 250);

            gridArrivals.Columns.Add("Guest", "Guest Name");
            gridArrivals.Columns.Add("Room", "Room #");
            gridArrivals.Columns.Add("Total", "Total (ETB)");
            gridArrivals.Columns.Add("Method", "Method");
            gridArrivals.Columns.Add("TxnRef", "Receipt / Ref");
            gridArrivals.Columns.Add("PaymentStatus", "Payment");
            gridArrivals.Columns.Add("StayStatus", "Stay");

            // Use fill weights instead of fixed widths so every heading remains
            // readable at both normal and maximized window sizes.
            gridArrivals.Columns["Guest"].FillWeight = 1.35f;
            gridArrivals.Columns["Room"].FillWeight = 0.75f;
            gridArrivals.Columns["Total"].FillWeight = 1.0f;
            gridArrivals.Columns["Method"].FillWeight = 1.05f;
            gridArrivals.Columns["TxnRef"].FillWeight = 1.25f;
            gridArrivals.Columns["PaymentStatus"].FillWeight = 1.15f;
            gridArrivals.Columns["StayStatus"].FillWeight = 0.75f;

            gridArrivals.Columns["Guest"].MinimumWidth = 115;
            gridArrivals.Columns["Room"].MinimumWidth = 70;
            gridArrivals.Columns["Total"].MinimumWidth = 95;
            gridArrivals.Columns["Method"].MinimumWidth = 95;
            gridArrivals.Columns["TxnRef"].MinimumWidth = 120;
            gridArrivals.Columns["PaymentStatus"].MinimumWidth = 90;
            gridArrivals.Columns["StayStatus"].MinimumWidth = 70;

            gridArrivals.CellFormatting += GridArrivals_CellFormatting;

            panel2.Controls.Add(gridArrivals);
        }

        private void GridArrivals_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (gridArrivals.Columns[e.ColumnIndex].Name == "PaymentStatus" && e.Value != null)
            {
                string val = e.Value.ToString();
                if (val.Contains("Paid"))
                {
                    e.CellStyle.ForeColor = Color.FromArgb(34, 139, 34);
                    e.CellStyle.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
                }
                else if (val.Contains("Pending"))
                {
                    e.CellStyle.ForeColor = Color.FromArgb(185, 123, 46);
                    e.CellStyle.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
                }
                else if (val.Contains("Rejected"))
                {
                    e.CellStyle.ForeColor = Color.FromArgb(178, 34, 34);
                    e.CellStyle.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
                }
            }
        }

        private void BtnVerifyPayment_Click(object sender, EventArgs e)
        {
            if (gridArrivals.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a reservation row to verify.", "Select Row", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int index = gridArrivals.SelectedRows[0].Index;
            string guest = gridArrivals.Rows[index].Cells["Guest"].Value?.ToString();
            string txnRef = gridArrivals.Rows[index].Cells["TxnRef"].Value?.ToString();
            string method = gridArrivals.Rows[index].Cells["Method"].Value?.ToString();
            string total = gridArrivals.Rows[index].Cells["Total"].Value?.ToString();
            string curStatus = gridArrivals.Rows[index].Cells["PaymentStatus"].Value?.ToString();

            if (curStatus.Contains("Paid"))
            {
                MessageBox.Show($"Payment for '{guest}' is already verified.", "Already Verified", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DialogResult confirm = MessageBox.Show(
                $"VERIFY PAYMENT DETAILS:\n\nGuest: {guest}\nAmount Due: {total} ETB\nPayment Method: {method}\nReference Code: {txnRef}\n\nHave you verified that {total} ETB was received via {method} with code '{txnRef}'?",
                "Confirm Payment Verification",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                var match = DataStore.Reservations.FirstOrDefault(r => r.GuestName == guest && r.TransactionRef == txnRef);
                if (match != null)
                {
                    match.PaymentStatus = "Paid (Verified)";
                }

                MessageBox.Show($"Payment approved successfully for {guest}!", "Payment Verified", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RefreshDashboardData();
            }
        }

        private void BtnRejectPayment_Click(object sender, EventArgs e)
        {
            if (gridArrivals.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a reservation row to flag.", "Select Row", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int index = gridArrivals.SelectedRows[0].Index;
            string guest = gridArrivals.Rows[index].Cells["Guest"].Value?.ToString();
            string txnRef = gridArrivals.Rows[index].Cells["TxnRef"].Value?.ToString();

            DialogResult confirm = MessageBox.Show(
                $"Are you sure you want to REJECT the payment reference '{txnRef}' for '{guest}'?",
                "Reject Payment",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm == DialogResult.Yes)
            {
                var match = DataStore.Reservations.FirstOrDefault(r => r.GuestName == guest && r.TransactionRef == txnRef);
                if (match != null)
                {
                    match.PaymentStatus = "Rejected (Unpaid)";
                }
                RefreshDashboardData();
            }
        }

        private void SetupAdminRoomReleaseControls()
        {
            groupBox5.Height = 370;

            Label lblDivider = new Label
            {
                Text = "Admin Room Release",
                Font = new Font("Georgia", 10.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(122, 31, 62),
                Location = new Point(20, 260),
                AutoSize = true
            };
            groupBox5.Controls.Add(lblDivider);

            lblCleaningSummary = new Label
            {
                Text = "Select cleaned room to put back in business:",
                Font = new Font("Segoe UI", 8.5f),
                ForeColor = Color.FromArgb(80, 80, 80),
                Location = new Point(20, 286),
                AutoSize = true
            };
            groupBox5.Controls.Add(lblCleaningSummary);

            cmbCleaningRooms = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Georgia", 9f),
                Location = new Point(20, 312),
                Width = 150
            };
            groupBox5.Controls.Add(cmbCleaningRooms);

            btnReleaseToBusiness = new Button
            {
                Text = "✓ Make Available",
                Location = new Point(180, 309),
                Size = new Size(130, 29),
                BackColor = Color.FromArgb(122, 31, 62),
                ForeColor = Color.FromArgb(247, 227, 200),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Georgia", 8.5f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnReleaseToBusiness.FlatAppearance.BorderSize = 0;
            btnReleaseToBusiness.Click += BtnReleaseToBusiness_Click;
            groupBox5.Controls.Add(btnReleaseToBusiness);
        }

        private void SetupManageRoomsButton()
        {
            btnManageRooms = new Button
            {
                Text = "Manage Rooms",
                Location = new Point(28, 555),
                Size = new Size(225, 38),
                BackColor = Color.FromArgb(185, 123, 46),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Georgia", 9f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnManageRooms.FlatAppearance.BorderSize = 0;
            btnManageRooms.Click += (s, e) =>
            {
                using (Form6 roomManager = new Form6())
                {
                    roomManager.ShowDialog(this);
                }
                RefreshDashboardData();
            };
            groupBox1.Controls.Add(btnManageRooms);
        }

        private void ApplyRoundedInterface()
        {
            // Cards and action controls use the same rounded visual language.
            ApplyRoundedRegion(groupBox2, 12);
            ApplyRoundedRegion(groupBox3, 12);
            ApplyRoundedRegion(groupBox4, 12);
            ApplyRoundedRegion(groupBox6, 12);
            ApplyRoundedRegion(panel2, 16);
            ApplyRoundedRegion(groupBox5, 16);

            ApplyRoundedRegion(button3, 10);
            ApplyRoundedRegion(btnVerifyPayment, 7);
            ApplyRoundedRegion(btnRejectPayment, 7);
            ApplyRoundedRegion(btnCheckOut, 7);
            ApplyRoundedRegion(btnDelete, 7);
            ApplyRoundedRegion(btnReleaseToBusiness, 7);
        }

        private void ApplyRoundedRegion(Control control, int radius)
        {
            if (control == null) return;

            control.Resize += (s, e) => UpdateRoundedRegion(control, radius);
            UpdateRoundedRegion(control, radius);
        }

        private void UpdateRoundedRegion(Control control, int radius)
        {
            if (control.Width <= 0 || control.Height <= 0) return;

            using (GraphicsPath path = new GraphicsPath())
            {
                int diameter = Math.Min(radius * 2, Math.Min(control.Width, control.Height));
                Rectangle bounds = new Rectangle(0, 0, control.Width - 1, control.Height - 1);
                path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90);
                path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 90);
                path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
                path.AddArc(bounds.X, bounds.Bottom - diameter, diameter, diameter, 90, 90);
                path.CloseFigure();
                control.Region = new Region(path);
            }
        }

        private void RefreshDashboardData()
        {
            FilterGrid("");

            int occupiedCount = DataStore.Rooms.Count(r => r.Status == RoomStatus.Occupied);
            label36.Text = $"{occupiedCount} of {DataStore.TotalRooms}";

            int arrivesToday = DataStore.Reservations.Count(r => r.CheckIn.Date == DateTime.Today && r.StayStatus == "Active");
            label37.Text = arrivesToday.ToString();

            int departsToday = DataStore.Reservations.Count(r => r.CheckOut.Date == DateTime.Today);
            label38.Text = departsToday.ToString();

            int pendingPayments = DataStore.Reservations.Count(r => r.PaymentStatus.Contains("Pending"));
            label39.Text = pendingPayments.ToString();

            int freeStandard = DataStore.Rooms.Count(r => r.RoomType.Contains("Standard") && r.Status == RoomStatus.Available);
            int freeSuites = DataStore.Rooms.Count(r => r.RoomType.Contains("Suite") && r.Status == RoomStatus.Available);
            int freeDeluxe = DataStore.Rooms.Count(r => r.RoomType.Contains("Deluxe") && r.Status == RoomStatus.Available);
            int cleaningCount = DataStore.Rooms.Count(r => r.Status == RoomStatus.Cleaning);

            label24.Text = freeStandard.ToString();
            label23.Text = freeSuites.ToString();
            label25.Text = freeDeluxe.ToString();
            label26.Text = cleaningCount.ToString();

            cmbCleaningRooms.Items.Clear();
            var cleaningRooms = DataStore.Rooms.Where(r => r.Status == RoomStatus.Cleaning).ToList();
            foreach (var cr in cleaningRooms)
            {
                cmbCleaningRooms.Items.Add($"Room {cr.RoomNumber} ({cr.RoomType.Split(' ')[0]})");
            }

            if (cmbCleaningRooms.Items.Count > 0)
            {
                cmbCleaningRooms.SelectedIndex = 0;
                btnReleaseToBusiness.Enabled = true;
            }
            else
            {
                cmbCleaningRooms.Items.Add("No rooms in cleaning");
                cmbCleaningRooms.SelectedIndex = 0;
                btnReleaseToBusiness.Enabled = false;
            }
        }

        private void FilterGrid(string keyword)
        {
            gridArrivals.Rows.Clear();
            string search = (keyword == "Search guest, room, ref...") ? "" : keyword.Trim().ToLower();

            var list = DataStore.Reservations.AsEnumerable();
            if (!string.IsNullOrEmpty(search))
            {
                list = list.Where(r => r.GuestName.ToLower().Contains(search) ||
                                       r.RoomNumber.ToLower().Contains(search) ||
                                       r.TransactionRef.ToLower().Contains(search));
            }

            foreach (var r in list)
            {
                gridArrivals.Rows.Add(r.GuestName, r.RoomNumber, $"{r.TotalPrice:N0}", r.PaymentMethod, r.TransactionRef, r.PaymentStatus, r.StayStatus);
            }
        }

        private void BtnCheckOut_Click(object sender, EventArgs e)
        {
            if (gridArrivals.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an active guest from the table to check out.", "Select Guest", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int index = gridArrivals.SelectedRows[0].Index;
            string roomNum = gridArrivals.Rows[index].Cells["Room"].Value?.ToString();
            string guest = gridArrivals.Rows[index].Cells["Guest"].Value?.ToString();
            string status = gridArrivals.Rows[index].Cells["StayStatus"].Value?.ToString();

            if (status == "Departed")
            {
                MessageBox.Show($"Guest '{guest}' has already checked out.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DialogResult res = MessageBox.Show($"Check out '{guest}' from Room {roomNum}?\nThe room will be placed in 'Cleaning' status.", "Confirm Check-Out", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (res == DialogResult.Yes)
            {
                DataStore.CheckOutGuest(roomNum);
                MessageBox.Show($"Guest checked out. Room {roomNum} is now being cleaned.", "Room in Cleaning", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RefreshDashboardData();
            }
        }

        private void BtnReleaseToBusiness_Click(object sender, EventArgs e)
        {
            string selectedText = cmbCleaningRooms.SelectedItem?.ToString() ?? "";
            if (string.IsNullOrEmpty(selectedText) || selectedText.StartsWith("No rooms")) return;

            string roomNum = selectedText.Split(' ')[1];
            DataStore.MakeRoomAvailable(roomNum);

            MessageBox.Show($"Room #{roomNum} has been inspected and returned back to Available business!", "Room Available", MessageBoxButtons.OK, MessageBoxIcon.Information);
            RefreshDashboardData();
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (gridArrivals.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a reservation record to delete.", "Select Row", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int index = gridArrivals.SelectedRows[0].Index;
            string guest = gridArrivals.Rows[index].Cells["Guest"].Value?.ToString();
            string roomNum = gridArrivals.Rows[index].Cells["Room"].Value?.ToString();

            DialogResult res = MessageBox.Show($"Delete reservation record for '{guest}'?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (res == DialogResult.Yes)
            {
                var match = DataStore.Reservations.FirstOrDefault(r => r.GuestName == guest && r.RoomNumber == roomNum);
                if (match != null)
                {
                    DataStore.Reservations.Remove(match);
                    DataStore.MakeRoomAvailable(roomNum);
                }
                RefreshDashboardData();
            }
        }

        private void WireUpSidebarNavigation()
        {
            label21.Cursor = Cursors.Hand;
            label28.Cursor = Cursors.Hand;
            label22.Cursor = Cursors.Hand;
            label20.Cursor = Cursors.Hand;

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