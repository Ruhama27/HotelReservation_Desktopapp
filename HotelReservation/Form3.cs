using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace HotelReservation
{
    public partial class Form3 : Form
    {
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

            ApplyCustomCurvature();
            WireUpNavigation();
            SetupBusinessLogic();

            this.Resize += (s, e) => CenterReservationContent();
            CenterReservationContent();
        }

        private void SetupBusinessLogic()
        {
            // Populate Guests Dropdown if empty
            if (comboBox1.Items.Count == 0)
            {
                comboBox1.Items.AddRange(new object[] { "1 Guest", "2 Guests", "3 Guests", "4+ Guests" });
                comboBox1.SelectedIndex = 0;
            }

            // Set default dates
            dateTimePicker1.Value = DateTime.Today;
            dateTimePicker2.Value = DateTime.Today.AddDays(1);

            // Default payment choice
            radioButton1.Checked = true;

            // Wire recalculation triggers
            dateTimePicker1.ValueChanged += (s, e) => RecalculateTotal();
            dateTimePicker2.ValueChanged += (s, e) => RecalculateTotal();

            CheckBox[] serviceChecks = new CheckBox[] { checkBox1, checkBox2, checkBox3, checkBox4, checkBox5 };
            foreach (var chk in serviceChecks)
            {
                chk.CheckedChanged += (s, e) => RecalculateTotal();
            }

            // Confirm Booking Action
            button1.Click += ConfirmBooking_Click;

            RecalculateTotal();
        }

        private void RecalculateTotal()
        {
            int nights = (dateTimePicker2.Value.Date - dateTimePicker1.Value.Date).Days;
            if (nights <= 0) nights = 1;

            decimal roomTotal = nights * _nightlyRate;

            // Each add-on service is 900 ETB
            decimal servicesTotal = 0;
            if (checkBox1.Checked) servicesTotal += 900m;
            if (checkBox2.Checked) servicesTotal += 900m;
            if (checkBox3.Checked) servicesTotal += 900m;
            if (checkBox4.Checked) servicesTotal += 900m;
            if (checkBox5.Checked) servicesTotal += 900m;

            _calculatedTotal = roomTotal + servicesTotal;

            // Update badge text to show total
            label22.Text = $"{_calculatedTotal:N0} ETB ({nights}n)";
        }

        private void ConfirmBooking_Click(object sender, EventArgs e)
        {
            // 1. Input Validation
            string guestName = textBox1.Text.Trim();
            string phone = textBox2.Text.Trim();

            if (string.IsNullOrWhiteSpace(guestName))
            {
                MessageBox.Show("Please enter the guest's full name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox1.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(phone))
            {
                MessageBox.Show("Please enter a valid contact phone number.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox2.Focus();
                return;
            }

            if (dateTimePicker2.Value.Date <= dateTimePicker1.Value.Date)
            {
                MessageBox.Show("Check-out date must be after the check-in date.", "Date Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dateTimePicker2.Focus();
                return;
            }

            // 2. Determine Payment Method
            string paymentMethod = "CBE";
            if (radioButton2.Checked) paymentMethod = "Telebirr";
            else if (radioButton3.Checked) paymentMethod = "Others";

            // 3. Collect Add-on Services
            List<string> selectedServices = new List<string>();
            if (checkBox1.Checked) selectedServices.Add("Restaurant");
            if (checkBox2.Checked) selectedServices.Add("Laundry");
            if (checkBox3.Checked) selectedServices.Add("Spa");
            if (checkBox4.Checked) selectedServices.Add("Airport Pickup");
            if (checkBox5.Checked) selectedServices.Add("Gym");

            // 4. Create and Save Reservation into In-Memory DataStore
            Reservation newBooking = new Reservation
            {
                GuestName = guestName,
                PhoneNumber = phone,
                RoomType = _selectedRoomType,
                RoomNumber = DataStore.AssignRandomRoomNumber(_selectedRoomType),
                CheckIn = dateTimePicker1.Value.Date,
                CheckOut = dateTimePicker2.Value.Date,
                TotalPrice = _calculatedTotal,
                PaymentMethod = paymentMethod,
                PaymentStatus = "Paid",
                Services = selectedServices
            };

            DataStore.Reservations.Insert(0, newBooking);

            MessageBox.Show(
                $"Reservation successfully confirmed!\n\nGuest: {newBooking.GuestName}\nRoom: #{newBooking.RoomNumber} ({newBooking.RoomType})\nTotal: {newBooking.TotalPrice:N0} ETB\nPayment: {newBooking.PaymentMethod}",
                "Booking Confirmed",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            // Navigate to Dashboard
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

            label6.Left = rightMargin - label6.Width;
            label7.Left = label6.Left - label7.Width - 30;
            label5.Left = label7.Left - label5.Width - 30;

            label2.Left = (this.ClientSize.Width - label2.Width) / 2;

            int totalBlockWidth = panel1.Width + 40 + groupBox6.Width;
            int startX = Math.Max(30, (this.ClientSize.Width - totalBlockWidth) / 2);
            int rightColumnX = startX + panel1.Width + 40;

            panel1.Left = startX;
            panel2.Left = startX;
            panel7.Left = startX;

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
    }
}