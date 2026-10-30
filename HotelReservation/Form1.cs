using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace HotelReservation
{
    public partial class Home : Form
    {
        public Home()
        {
            InitializeComponent();
            this.AutoSize = false;
            this.WindowState = FormWindowState.Maximized;

            ApplyRoundedCorners();
            WireUpNavigation();
            SetupResponsiveness();

            this.Resize += (s, e) => AdjustLayout();
        }

        private void SetupResponsiveness()
        {
            // Dock hero section to fill remaining height
            groupBox3.Dock = DockStyle.Fill;

            // Anchor top-right navigation elements
            button3.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label5.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label6.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label7.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            // Anchor hero text and button to bottom-left of hero container
            label3.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label2.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label4.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            button2.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;

            AdjustLayout();
        }

        private void AdjustLayout()
        {
            // Header navigation buttons
            int rightMargin = this.ClientSize.Width - 40;
            button3.Left = rightMargin - button3.Width;
            label6.Left = button3.Left - 90;
            label7.Left = label6.Left - 130;
            label5.Left = label7.Left - 80;

            // Safe lookup for footer contacts inside groupBox1
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

                    int centerY = (groupBox1.Height - lbl8.Height) / 2;
                    lbl8.Top = centerY;
                    lbl9.Top = centerY;
                    lbl10.Top = centerY;
                }
            }
        }

        private void WireUpNavigation()
        {
            label5.Cursor = Cursors.Hand;
            label6.Cursor = Cursors.Hand;
            label7.Cursor = Cursors.Hand;

            button2.Click += ViewRooms_Click;
            button3.Click += BookNow_Click;
            label7.Click += ViewRooms_Click;
            label6.Click += BookNow_Click;
        }

        private void ViewRooms_Click(object sender, EventArgs e)
        {
            Form2 roomsForm = new Form2();
            this.Hide();
            roomsForm.Show();
            roomsForm.FormClosed += (s, args) => this.Show();
        }

        private void BookNow_Click(object sender, EventArgs e)
        {
            Form3 reservationForm = new Form3();
            this.Hide();
            reservationForm.Show();
            reservationForm.FormClosed += (s, args) => this.Show();
        }

        private void ApplyRoundedCorners()
        {
            int buttonCornerRadius = 18;
            int imageCornerRadius = 16;

            ApplyRoundedRegion(button2, buttonCornerRadius);
            ApplyRoundedRegion(button3, buttonCornerRadius);
            ApplyRoundedRegion(pictureBox1, imageCornerRadius);
        }

        private void ApplyRoundedRegion(Control control, int radius)
        {
            if (control == null) return;
            control.Resize += (sender, e) => UpdateControlRegion(control, radius);
            UpdateControlRegion(control, radius);
        }

        private void UpdateControlRegion(Control control, int radius)
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

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            Application.Exit();
        }
    }
}