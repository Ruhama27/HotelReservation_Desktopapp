using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace HotelReservation
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
            this.AutoSize = false;
            this.WindowState = FormWindowState.Maximized;

            ApplyCustomCurvature();
            WireUpNavigation();

            this.Resize += (s, e) => CenterReservationContent();
            CenterReservationContent();
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

            // --- Safe Top Contact Bar (groupBox1) ---
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

            // --- Main Header Navigation (groupBox2) ---
            label6.Left = rightMargin - label6.Width;
            label7.Left = label6.Left - label7.Width - 30;
            label5.Left = label7.Left - label5.Width - 30;

            // Center header banner title
            label2.Left = (this.ClientSize.Width - label2.Width) / 2;

            // --- Main Content Columns ---
            int totalBlockWidth = panel1.Width + 40 + groupBox6.Width;
            int startX = Math.Max(30, (this.ClientSize.Width - totalBlockWidth) / 2);
            int rightColumnX = startX + panel1.Width + 40;

            // Left Column
            panel1.Left = startX;
            panel2.Left = startX;
            panel7.Left = startX;

            // Right Column
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