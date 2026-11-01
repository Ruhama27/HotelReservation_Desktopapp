using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace HotelReservation
{
    public partial class Form3 : Form
    {
        private Label lblDashboard;
        private Label lblPaymentHeader;
        private TextBox txtTxnRef;
        private Label lblTxnPrompt;

        private string _selectedRoomType = "Deluxe Room";
        private decimal _nightlyRate = 4200m;
        private decimal _calculatedTotal = 4200m;

        public Form3() : this("Deluxe Room", 4200m) { }

        public Form3(string roomType, decimal nightlyRate)
        {
            InitializeComponent();
            this.AutoSize = false;
            this.WindowState = FormWindowState.Maximized;

            _selectedRoomType = roomType;
            _nightlyRate = nightlyRate;

            SetupDashboardHeaderLink();
            SetupCleanPanel7Layout();
            ApplyCustomCurvature();
            WireUpNavigation();
            SetupBusinessLogic();

            this.Resize += (s, e) => CenterReservationContent();
            CenterReservationContent();
        }

        private void SetupDashboardHeaderLink()
        {
            lblDashboard = new Label
            {
                Text = "Dashboard",
                Font = new Font("Georgia", 10F, FontStyle.Regular),
                ForeColor = Color.FromArgb(61, 42, 48),
                AutoSize = true,
                Cursor = Cursors.Hand
            };

            lblDashboard.Click += (s, e) =>
            {
                Form4 dashboard = new Form4();
                this.Hide();
                dashboard.Show();
                dashboard.FormClosed += (senderObj, args) => this.Show();
            };

            groupBox2.Controls.Add(lblDashboard);
        }

        private void SetupCleanPanel7Layout()
        {
            // Row 1: Labels for Name and Phone.
            // label14 and label15 belong to the Additional Services card and
            // contain service prices. Moving them here makes them overlap its
            // heading, so use the labels that actually belong to panel7.
            label20.Location = new Point(22, 24);
            label21.Location = new Point(230, 24);

            // Row 2: TextBoxes for Name and Phone
            textBox1.Location = new Point(22, 52);
            textBox1.Size = new Size(185, 26);

            textBox2.Location = new Point(230, 52);
            textBox2.Size = new Size(185, 26);

            // Row 3: Section label for Payment Provider
            lblPaymentHeader = new Label
            {
                Text = "Select Payment Channel:",
                Font = new Font("Georgia", 8.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(61, 42, 48),
                Location = new Point(22, 96),
                AutoSize = true
            };
            panel7.Controls.Add(lblPaymentHeader);

            // Row 4: Radio Buttons placed safely underneath
            radioButton1.Location = new Point(25, 124); // CBE
            radioButton2.Location = new Point(145, 124); // Telebirr
            radioButton3.Location = new Point(265, 124); // Others
            radioButton1.BringToFront();
            radioButton2.BringToFront();
            radioButton3.BringToFront();

            // Row 5: Transaction Reference Label
            lblTxnPrompt = new Label
            {
                Text = "CBE Transfer / Transaction Code (e.g. FT...):",
                Font = new Font("Georgia", 8.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(61, 42, 48),
                Location = new Point(22, 162),
                AutoSize = true
            };
            panel7.Controls.Add(lblTxnPrompt);

            // Row 6: Transaction Reference TextBox
            txtTxnRef = new TextBox
            {
                Location = new Point(25, 188),
                Size = new Size(390, 26),
                Font = new Font("Georgia", 9.5f),
                Text = "FT26305"
            };
            panel7.Controls.Add(txtTxnRef);

            // Toggle prompt text based on selection
            radioButton1.CheckedChanged += (s, e) => UpdateTxnPrompt();
            radioButton2.CheckedChanged += (s, e) => UpdateTxnPrompt();
            radioButton3.CheckedChanged += (s, e) => UpdateTxnPrompt();
        }

        private void UpdateTxnPrompt()
        {
            if (radioButton1.Checked)
            {
                lblTxnPrompt.Text = "CBE Transfer / Transaction Code (e.g. FT...):";
                txtTxnRef.Enabled = true;
            }
            else if (radioButton2.Checked)
            {
                lblTxnPrompt.Text = "Telebirr Payment Receipt Code (e.g. TB...):";
                txtTxnRef.Enabled = true;
            }
            else
            {
                lblTxnPrompt.Text = "Cash on Arrival (Desk Payment):";
                txtTxnRef.Text = "PAY-AT-DESK";
                txtTxnRef.Enabled = false;
            }
        }

        private void SetupBusinessLogic()
        {
            // Keep the text in the form consistent with the reservation UI.
            label12.Text = "Guests";
            checkBox1.Text = "Restaurant meals";
            checkBox2.Text = "Laundry";

            // Replace the legacy designer entries ("Guste") with the labels
            // used by the current reservation interface.
            comboBox1.Items.Clear();
            comboBox1.Items.AddRange(new object[] { "1 Guest", "2 Guests", "3 Guests", "4+ Guests" });
            comboBox1.SelectedIndex = 0;

            dateTimePicker1.Value = DateTime.Today;
            dateTimePicker2.Value = DateTime.Today.AddDays(1);
            radioButton1.Checked = true;

            dateTimePicker1.ValueChanged += (s, e) => RecalculateTotal();
            dateTimePicker2.ValueChanged += (s, e) => RecalculateTotal();

            CheckBox[] serviceChecks = new CheckBox[] { checkBox1, checkBox2, checkBox3, checkBox4, checkBox5 };
            foreach (var chk in serviceChecks)
            {
                chk.CheckedChanged += (s, e) => RecalculateTotal();
            }

            button1.Click += ConfirmBooking_Click;

            RecalculateTotal();
        }

        private void RecalculateTotal()
        {
            int nights = (dateTimePicker2.Value.Date - dateTimePicker1.Value.Date).Days;
            if (nights <= 0) nights = 1;

            decimal roomTotal = nights * _nightlyRate;

            decimal servicesTotal = 0;
            if (checkBox1.Checked) servicesTotal += 900m;
            if (checkBox2.Checked) servicesTotal += 900m;
            if (checkBox3.Checked) servicesTotal += 900m;
            if (checkBox4.Checked) servicesTotal += 900m;
            if (checkBox5.Checked) servicesTotal += 900m;

            _calculatedTotal = roomTotal + servicesTotal;
            label22.Text = $"{_calculatedTotal:N0} ETB ({nights}n)";
        }

        private void ConfirmBooking_Click(object sender, EventArgs e)
        {
            string guestName = textBox1.Text.Trim();
            string phone = textBox2.Text.Trim();
            string txnCode = txtTxnRef.Text.Trim();

            if (string.IsNullOrWhiteSpace(guestName))
            {
                MessageBox.Show("Please enter the guest's full name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox1.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(phone))
            {
                MessageBox.Show("Please enter a contact phone number.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox2.Focus();
                return;
            }

            if (dateTimePicker2.Value.Date <= dateTimePicker1.Value.Date)
            {
                MessageBox.Show("Check-out date must be after check-in date.", "Date Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dateTimePicker2.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txnCode))
            {
                MessageBox.Show("Please enter the transaction reference code or receipt number.", "Payment Information Needed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTxnRef.Focus();
                return;
            }

            var assignedRoom = DataStore.AllocateAvailableRoom(_selectedRoomType);
            if (assignedRoom == null)
            {
                MessageBox.Show("Sorry, no rooms are currently available in this category! Please check with front-desk admin.", "No Rooms Available", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string paymentMethod = "CBE";
            if (radioButton2.Checked) paymentMethod = "Telebirr";
            else if (radioButton3.Checked) paymentMethod = "Others (Cash)";

            Reservation newBooking = new Reservation
            {
                GuestName = guestName,
                PhoneNumber = phone,
                RoomType = assignedRoom.RoomType,
                RoomNumber = assignedRoom.RoomNumber,
                CheckIn = dateTimePicker1.Value.Date,
                CheckOut = dateTimePicker2.Value.Date,
                TotalPrice = _calculatedTotal,
                PaymentMethod = paymentMethod,
                TransactionRef = txnCode,
                PaymentStatus = "Pending Verification",
                StayStatus = "Active"
            };

            DataStore.Reservations.Insert(0, newBooking);

            MessageBox.Show(
                $"Reservation submitted for verification!\n\nGuest: {newBooking.GuestName}\nAssigned Room: #{assignedRoom.RoomNumber}\nTotal: {newBooking.TotalPrice:N0} ETB\nPayment Method: {newBooking.PaymentMethod}\nRef Code: {newBooking.TransactionRef}\n\nStatus: Pending Admin Payment Verification.",
                "Submitted for Verification",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            Form4 dashboard = new Form4();
            this.Hide();
            dashboard.Show();
            dashboard.FormClosed += (s, args) => this.Close();
        }

        private void WireUpNavigation()
        {
            label5.Cursor = Cursors.Hand;
            label6.Cursor = Cursors.Hand;
            label7.Cursor = Cursors.Hand;

            label5.Click += Home_Click;
            label7.Click += RoomsAndSuites_Click;
        }

        private void CenterReservationContent()
        {
            int rightMargin = this.ClientSize.Width - 50;

            if (groupBox1 != null)
            {
                Control lbl8 = groupBox1.Controls["label8"];
                Control lbl9 = groupBox1.Controls["label9"];
                Control lbl10 = groupBox1.Controls["label10"];

                if (lbl8 != null && lbl9 != null && lbl10 != null)
                {
                    int itemSpacing = 35;
                    lbl10.Left = rightMargin - lbl10.Width;
                    lbl9.Left = lbl10.Left - lbl9.Width - itemSpacing;
                    lbl8.Left = lbl9.Left - lbl8.Width - itemSpacing;

                    int barCenterY = (groupBox1.Height - lbl8.Height) / 2;
                    lbl8.Top = barCenterY;
                    lbl9.Top = barCenterY;
                    lbl10.Top = barCenterY;
                }
            }

            if (lblDashboard != null)
            {
                lblDashboard.Left = rightMargin - lblDashboard.Width;
                lblDashboard.Top = label6.Top;
                label6.Left = lblDashboard.Left - label6.Width - 25;
            }
            else
            {
                label6.Left = rightMargin - label6.Width;
            }

            label7.Left = label6.Left - label7.Width - 25;
            label5.Left = label7.Left - label5.Width - 25;

            label2.Left = (this.ClientSize.Width - label2.Width) / 2;

            int totalBlockWidth = panel1.Width + 40 + groupBox6.Width;
            int startX = Math.Max(30, (this.ClientSize.Width - totalBlockWidth) / 2);
            int rightColumnX = startX + panel1.Width + 40;

            // Left Column: Intake sections
            panel1.Left = startX;
            panel2.Left = startX;

            panel7.Left = startX;
            panel7.Height = 230; // Tall enough to hold Name/Phone, RadioButtons, and TxnRef with spacing

            // Adjust text boxes inside panel7 if panel is wider
            if (txtTxnRef != null)
            {
                txtTxnRef.Width = panel7.Width - 50;
            }

            // Right Column: Summary & Confirm Button
            groupBox6.Left = rightColumnX;
            groupBox5.Left = rightColumnX;
            panel9.Left = rightColumnX;
            button1.Left = rightColumnX + groupBox5.Width - button1.Width;
        }

        private void Home_Click(object sender, EventArgs e)
        {
            Home homeForm = null;
            foreach (Form openForm in Application.OpenForms)
            {
                if (openForm is Home)
                {
                    homeForm = (Home)openForm;
                    break;
                }
            }

            if (homeForm != null)
            {
                homeForm.Show();
                homeForm.BringToFront();
            }
            else
            {
                new Home().Show();
            }

            this.Close();
        }

        private void RoomsAndSuites_Click(object sender, EventArgs e)
        {
            Form2 roomsForm = null;
            foreach (Form openForm in Application.OpenForms)
            {
                if (openForm is Form2)
                {
                    roomsForm = (Form2)openForm;
                    break;
                }
            }

            if (roomsForm != null)
            {
                roomsForm.Show();
                roomsForm.BringToFront();
                this.Close();
            }
            else
            {
                Form2 newRoomsForm = new Form2();
                this.Hide();
                newRoomsForm.Show();
                newRoomsForm.FormClosed += (s, args) => this.Show();
            }
        }

        private void ApplyCustomCurvature()
        {
            int buttonRadius = 14;
            int cardRadius = 18;

            ApplyRoundedRegion(button1, buttonRadius);
            ApplyRoundedRegion(panel1, cardRadius);
            ApplyRoundedRegion(panel2, cardRadius);
            ApplyRoundedRegion(panel7, cardRadius);
            ApplyRoundedRegion(groupBox5, cardRadius);
            ApplyRoundedRegion(groupBox6, cardRadius);
            ApplyRoundedRegion(pictureBox1, 16);
        }

        private void ApplyRoundedRegion(Control control, int radius)
        {
            if (control == null) return;
            control.Resize += (s, e) => UpdateRegion(control, radius);
            UpdateRegion(control, radius);
        }

        private void UpdateRegion(Control control, int radius)
        {
            if (control.Width <= 0 || control.Height <= 0) return;

            using (GraphicsPath path = GetRoundedRectanglePath(control.ClientRectangle, radius))
            {
                control.Region = new Region(path);
            }
        }

        private GraphicsPath GetRoundedRectanglePath(Rectangle bounds, int radius)
        {
            int diameter = radius * 2;
            GraphicsPath path = new GraphicsPath();

            if (radius <= 0)
            {
                path.AddRectangle(bounds);
                return path;
            }

            Rectangle arc = new Rectangle(bounds.Location, new Size(diameter, diameter));
            path.AddArc(arc, 180, 90);
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);
            path.CloseFigure();
            return path;
        }

        private void groupBox2_Enter(object sender, EventArgs e) { }

        private void label14_Click(object sender, EventArgs e)
        {

        }
    }
}